-- ========================================================================
-- APPLICATION: EMPLOYEE EDUCATION MANAGEMENT SYSTEM
-- TARGET DATABASE: ORACLE 11g R2 / 12c+
-- SCRIPT: 03_Migration_Procedure.sql
-- DESCRIPTION: Automated, Configurable PL/SQL Migration from HR_EMP_EDUCATION
--              to EDU_EMP_EDUCATION without altering original table.
-- ========================================================================

-- OPTIONAL: Explicitly drop existing package if you want a fresh start
-- (Note: CREATE OR REPLACE will automatically overwrite it anyway)
-- DROP PACKAGE PKG_EDU_MIGRATION;

CREATE OR REPLACE PACKAGE PKG_EDU_MIGRATION AS
    PROCEDURE SP_RUN_MIGRATION(
        p_executed_by    IN  VARCHAR2,
        p_default_status IN  VARCHAR2 DEFAULT 'Submitted',
        p_source_table   IN  VARCHAR2 DEFAULT 'HR_EMP_EDUCATION',
        p_out_total_read OUT NUMBER,
        p_out_migrated   OUT NUMBER,
        p_out_skipped    OUT NUMBER,
        p_out_message    OUT VARCHAR2
    );
END PKG_EDU_MIGRATION;
/

CREATE OR REPLACE PACKAGE BODY PKG_EDU_MIGRATION AS

    PROCEDURE SP_RUN_MIGRATION(
        p_executed_by    IN  VARCHAR2,
        p_default_status IN  VARCHAR2 DEFAULT 'Submitted',
        p_source_table   IN  VARCHAR2 DEFAULT 'HR_EMP_EDUCATION',
        p_out_total_read OUT NUMBER,
        p_out_migrated   OUT NUMBER,
        p_out_skipped    OUT NUMBER,
        p_out_message    OUT VARCHAR2
    ) IS
        v_total_read     NUMBER := 0;
        v_total_migrated NUMBER := 0;
        v_total_skipped  NUMBER := 0;
        v_log_id         NUMBER;
        v_status_to_use  VARCHAR2(20) := NVL(p_default_status, 'Submitted');
        v_source_tbl     VARCHAR2(100) := NVL(p_source_table, 'HR_EMP_EDUCATION');
        
        -- Dynamic cursor for legacy table (HR_EMP_EDUCATION or BACKUP_HR_EMP_EDUCATION)
        TYPE t_cursor IS REF CURSOR;
        c_old_data       t_cursor;
        
        -- Variables for row
        v_pcno           VARCHAR2(30);
        v_ctredn         NUMBER;
        v_qfnid          NUMBER;
        v_univinstid     NUMBER;
        v_collegename    VARCHAR2(250);
        v_subjectid      NUMBER;
        v_yearpassed     NUMBER;
        v_grade          VARCHAR2(50);
        v_percentage     VARCHAR2(50);
        v_highestqfn     VARCHAR2(10);
        
        -- Resolved variables
        v_main_code      VARCHAR2(20);
        v_main_name      VARCHAR2(200);
        v_sub_code       VARCHAR2(50);
        v_sub_name       VARCHAR2(200);
        v_inst_name      VARCHAR2(250);
        v_spec           VARCHAR2(200);
        v_is_highest     NUMBER(1);
        v_emp_name       VARCHAR2(150);
        v_designation    VARCHAR2(100);
        v_division       VARCHAR2(100);
        v_exists         NUMBER;
        v_emp_found      NUMBER := 0;
        v_row_status     VARCHAR2(20);
        
    BEGIN
        -- Log entry
        SELECT SEQ_EDU_MIG_LOG.NEXTVAL INTO v_log_id FROM DUAL;
        INSERT INTO EDU_MIGRATION_LOG (LOG_ID, MIGRATION_DATE, EXECUTED_BY, STATUS, LOG_DETAILS)
        VALUES (v_log_id, SYSDATE, p_executed_by, 'IN_PROGRESS', 'Migration started by ' || p_executed_by);
        COMMIT;

        -- Open cursor on legacy table (defaults to HR_EMP_EDUCATION or accepts BACKUP_HR_EMP_EDUCATION)
        OPEN c_old_data FOR 
            'SELECT PCNO, CTREDN, QFNID, UNIVINSTID, COLLEGENAME, SUBJECTID, YEARPASSED, GRADE, PERCENTAGE, HIGHESTQFN FROM ' || v_source_tbl;

        LOOP
            FETCH c_old_data INTO 
                v_pcno, v_ctredn, v_qfnid, v_univinstid, v_collegename, 
                v_subjectid, v_yearpassed, v_grade, v_percentage, v_highestqfn;
            EXIT WHEN c_old_data%NOTFOUND;

            v_total_read := v_total_read + 1;

            -- 0. Check whether PCNO exists in hrdata.empdetails
            -- Only import into EDU_EMP_EDUCATION if PCNO is present in hrdata.empdetails
            v_emp_found   := 0;
            v_emp_name    := NULL;
            v_designation := NULL;
            v_division    := NULL;

            BEGIN
                EXECUTE IMMEDIATE 
                    'SELECT NAME, DESIGNATION, DIVNAME FROM hrdata.empdetails WHERE TRIM(PCNO) = TRIM(:1) AND ROWNUM = 1'
                    INTO v_emp_name, v_designation, v_division
                    USING v_pcno;
                v_emp_found := 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    v_emp_found := 0;
                WHEN OTHERS THEN
                    BEGIN
                        EXECUTE IMMEDIATE 
                            'SELECT NAME, DESIGNATION, DIVNAME FROM empdetails WHERE TRIM(PCNO) = TRIM(:1) AND ROWNUM = 1'
                            INTO v_emp_name, v_designation, v_division
                            USING v_pcno;
                        v_emp_found := 1;
                    EXCEPTION
                        WHEN OTHERS THEN
                            v_emp_found := 0;
                    END;
            END;

            IF v_emp_found = 0 THEN
                -- Skip record because employee PCNO does not exist in hrdata.empdetails
                v_total_skipped := v_total_skipped + 1;
            ELSE
                -- Check if already migrated for this PCNO and CTREDN
                SELECT COUNT(*) INTO v_exists 
                FROM EDU_EMP_EDUCATION 
                WHERE PCNO = v_pcno AND OLD_CTREDN = v_ctredn;

                IF v_exists > 0 THEN
                    v_total_skipped := v_total_skipped + 1;
                ELSE
                    -- Fallback default name if blank in empdetails
                    IF v_emp_name IS NULL OR TRIM(v_emp_name) = '' THEN
                        v_emp_name := 'Employee ' || v_pcno;
                    END IF;

                    -- 1. Resolve Qualification Mapping
                    BEGIN
                        SELECT m.NEW_QUAL_MAIN_CODE, NVL(m.NEW_QUAL_SUB_CODE, 'GENERAL')
                        INTO v_main_code, v_sub_code
                        FROM EDU_QUAL_MAPPING m
                        WHERE m.OLD_QFN_ID = v_qfnid AND m.IS_ACTIVE = 1 AND ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            v_main_code := 'OTHER';
                            v_sub_code  := 'GENERAL';
                    END;

                    -- Resolve Main Qualification Name
                    BEGIN
                        SELECT QUAL_NAME INTO v_main_name FROM EDU_QUAL_MAIN WHERE QUAL_CODE = v_main_code AND ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            v_main_name := v_main_code;
                    END;

                    -- 2. Resolve University / Institute Name
                    v_inst_name := NVL(v_collegename, 'Not Specified');
                    IF v_univinstid IS NOT NULL AND v_univinstid > 0 THEN
                        BEGIN
                            SELECT UNIV_NAME INTO v_inst_name FROM EDU_UNIVERSITY WHERE UNIV_ID = v_univinstid AND ROWNUM = 1;
                        EXCEPTION
                            WHEN NO_DATA_FOUND THEN
                                NULL;
                        END;
                    END IF;

                    -- 3. Resolve Subject / Specialization & Sub-Qualification
                    -- In legacy HR_EMP_EDUCATION, there is no separate SUB_QUALIFICATION column.
                    -- Legacy branch/discipline was recorded in SUBJECTID (referencing EDU_SUBJECT).
                    v_spec := 'General';
                    IF v_subjectid IS NOT NULL AND v_subjectid > 0 THEN
                        BEGIN
                            SELECT SUBJECT_NAME INTO v_spec FROM EDU_SUBJECT WHERE SUBJECT_ID = v_subjectid AND ROWNUM = 1;
                        EXCEPTION
                            WHEN NO_DATA_FOUND THEN
                                NULL;
                        END;
                    END IF;

                    -- Normalize '*' or empty subject to 'General'
                    IF TRIM(v_spec) = '*' OR v_spec IS NULL OR TRIM(v_spec) = '' THEN
                        v_spec := 'General';
                    END IF;

                    -- Default Sub Name to the legacy subject / branch
                    v_sub_name := v_spec;

                    -- Intelligently resolve canonical QUAL_SUB_CODE and SUB_NAME from EDU_QUAL_SUB:
                    -- Priority 1: Match SUB_NAME under matching MAIN_QUAL_CODE
                    -- Priority 2: Match SUB_NAME across global sub-qualifications
                    -- Fallback: Retain mapping default (v_sub_code) with v_sub_name := v_spec
                    BEGIN
                        SELECT SUB_CODE, SUB_NAME INTO v_sub_code, v_sub_name
                        FROM EDU_QUAL_SUB
                        WHERE UPPER(TRIM(SUB_NAME)) = UPPER(TRIM(v_spec))
                          AND (MAIN_QUAL_CODE = v_main_code OR MAIN_QUAL_CODE IS NULL)
                          AND ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            BEGIN
                                SELECT SUB_CODE, SUB_NAME INTO v_sub_code, v_sub_name
                                FROM EDU_QUAL_SUB
                                WHERE UPPER(TRIM(SUB_NAME)) = UPPER(TRIM(v_spec))
                                  AND ROWNUM = 1;
                            EXCEPTION
                                WHEN NO_DATA_FOUND THEN
                                    NULL; -- Keep mapping default v_sub_code and v_sub_name := v_spec
                            END;
                    END;

                    -- 4. Highest Qualification Flag
                    IF UPPER(TRIM(NVL(v_highestqfn, 'N'))) IN ('Y', 'YES', '1', 'TRUE') THEN
                        v_is_highest := 1;
                    ELSE
                        v_is_highest := 0;
                    END IF;

                -- 5. Status Determination:
                -- If record contains placeholder values like '*', force to Draft so employee must correct it
                IF TRIM(v_collegename) = '*' OR TRIM(v_inst_name) = '*' OR TRIM(v_inst_name) = 'Not Specified' 
                   OR TRIM(v_spec) = '*' OR TRIM(v_grade) = '*' 
                   OR TRIM(NVL(v_percentage, '*')) = '*' OR UPPER(TRIM(v_percentage)) IN ('N/A', 'NA', '-', '--', '?', 'NOT SPECIFIED', 'NONE', 'NULL')
                   OR v_yearpassed IS NULL OR v_yearpassed = 0 THEN
                    v_row_status := 'Draft';
                ELSE
                    v_row_status := v_status_to_use;
                END IF;

                -- 6. Insert into EDU_EMP_EDUCATION
                INSERT INTO EDU_EMP_EDUCATION (
                    PCNO,
                    EMP_NAME,
                    DESIGNATION,
                    DIVISION_NAME,
                    QUAL_MAIN_CODE,
                    QUAL_MAIN_NAME,
                    QUAL_SUB_CODE,
                    QUAL_SUB_NAME,
                    YEAR_PASSED,
                    INSTITUTE_NAME,
                    COLLEGE_NAME,
                    SPECIALIZATION,
                    CGPA_MARKS,
                    GRADE_DIVISION,
                    IS_HIGHEST,
                    REMARKS,
                    STATUS,
                    SUBMITTED_DATE,
                    CREATED_ON,
                    CREATED_BY,
                    IS_MIGRATED,
                    OLD_CTREDN
                ) VALUES (
                    v_pcno,
                    v_emp_name,
                    v_designation,
                    v_division,
                    v_main_code,
                    v_main_name,
                    v_sub_code,
                    v_sub_name,
                    NVL(v_yearpassed, 2000),
                    v_inst_name,
                    v_collegename,
                    v_spec,
                    NVL(TO_CHAR(v_percentage), 'N/A'),
                    NVL(v_grade, 'Pass'),
                    v_is_highest,
                    'Migrated from ' || v_source_tbl || ' (CTREDN: ' || v_ctredn || ')',
                    v_row_status,
                    CASE WHEN v_row_status = 'Submitted' THEN SYSDATE ELSE NULL END,
                    SYSDATE,
                    p_executed_by,
                    1,
                    v_ctredn
                );

                v_total_migrated := v_total_migrated + 1;
                END IF;
            END IF;

        END LOOP;

        CLOSE c_old_data;

        COMMIT;

        -- Update Migration Log
        UPDATE EDU_MIGRATION_LOG
        SET TOTAL_READ = v_total_read,
            TOTAL_MIGRATED = v_total_migrated,
            TOTAL_SKIPPED = v_total_skipped,
            STATUS = 'COMPLETED',
            LOG_DETAILS = 'Migration completed successfully. Read: ' || v_total_read || 
                          ', Migrated: ' || v_total_migrated || ', Skipped: ' || v_total_skipped
        WHERE LOG_ID = v_log_id;
        COMMIT;

        p_out_total_read := v_total_read;
        p_out_migrated   := v_total_migrated;
        p_out_skipped    := v_total_skipped;
        p_out_message    := 'Success: ' || v_total_migrated || ' records migrated, ' || v_total_skipped || ' skipped.';

    EXCEPTION
        WHEN OTHERS THEN
            IF c_old_data%ISOPEN THEN
                CLOSE c_old_data;
            END IF;
            ROLLBACK;
            UPDATE EDU_MIGRATION_LOG
            SET STATUS = 'FAILED',
                LOG_DETAILS = 'Error: ' || SQLERRM
            WHERE LOG_ID = v_log_id;
            COMMIT;
            p_out_total_read := v_total_read;
            p_out_migrated   := v_total_migrated;
            p_out_skipped    := v_total_skipped;
            p_out_message    := 'Error during migration: ' || SQLERRM;
    END SP_RUN_MIGRATION;

END PKG_EDU_MIGRATION;
/
