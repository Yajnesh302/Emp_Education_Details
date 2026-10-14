-- ========================================================================
-- APPLICATION: EMPLOYEE EDUCATION MANAGEMENT SYSTEM
-- TARGET DATABASE: ORACLE 11g R2 / 12c+
-- SCRIPT: 03_Migration_Procedure.sql
-- DESCRIPTION: Automated, Configurable PL/SQL Migration from HR_EMP_EDUCATION
--              to EDU_EMP_EDUCATION without altering original table.
-- ========================================================================
SET DEFINE OFF;

-- OPTIONAL: Explicitly drop existing package if you want a fresh start
-- (Note: CREATE OR REPLACE will automatically overwrite it anyway)
-- DROP PACKAGE PKG_EDU_MIGRATION;

CREATE OR REPLACE PACKAGE PKG_EDU_MIGRATION AS
    FUNCTION FN_RESOLVE_LEGACY_UNIV(
        p_univ_id     IN NUMBER,
        p_collegename IN VARCHAR2
    ) RETURN VARCHAR2;

    FUNCTION FN_NORMALIZE_GRADE(
        p_grade IN VARCHAR2
    ) RETURN VARCHAR2;

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

    FUNCTION FN_RESOLVE_LEGACY_UNIV(
        p_univ_id     IN NUMBER,
        p_collegename IN VARCHAR2
    ) RETURN VARCHAR2 IS
        v_name VARCHAR2(250);
    BEGIN
        -- 1. Check known alias / duplicate legacy IDs from legacy HR system
        IF p_univ_id IS NOT NULL THEN
            CASE p_univ_id
            WHEN 0 THEN RETURN 'Other Recognized University / Board';
            WHEN 8 THEN RETURN 'Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow';
            WHEN 53 THEN RETURN 'Madurai Kamaraj University (MKU), Madurai';
            WHEN 59 THEN RETURN 'Jawaharlal Nehru University (JNU), New Delhi';
            WHEN 62 THEN RETURN 'Mahatma Gandhi University, Kerala';
            WHEN 72 THEN RETURN 'University of Calicut, Malappuram';
            WHEN 75 THEN RETURN 'National Institute of Technology Karnataka (NITK / REC), Surathkal';
            WHEN 91 THEN RETURN 'Deen Dayal Upadhyaya Gorakhpur University (DDU), Gorakhpur';
            WHEN 93 THEN RETURN 'Banaras Hindu University (BHU), Varanasi';
            WHEN 100 THEN RETURN 'Mahatma Jyotiba Phule Rohilkhand University (MJPRU), Bareilly';
            WHEN 114 THEN RETURN 'Motilal Nehru National Institute of Technology (MNNIT), Allahabad';
            WHEN 120 THEN RETURN 'Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)';
            WHEN 124 THEN RETURN 'University of Kerala, Thiruvananthapuram';
            WHEN 133 THEN RETURN 'Malaviya National Institute of Technology (MNIT), Jaipur';
            WHEN 138 THEN RETURN 'Sardar Vallabhbhai National Institute of Technology (SVNIT), Surat';
            WHEN 141 THEN RETURN 'National Institute of Technology (NIT / REC), Durgapur';
            WHEN 142 THEN RETURN 'National Institute of Technology (NIT / REC), Hamirpur';
            WHEN 143 THEN RETURN 'National Institute of Technology (NIT / REC), Silchar';
            WHEN 144 THEN RETURN 'National Institute of Technology (NIT / REC), Warangal';
            WHEN 145 THEN RETURN 'Rajiv Gandhi Proudyogiki Vishwavidyalaya (RGPV), Bhopal';
            WHEN 155 THEN RETURN 'University of Calicut, Malappuram';
            WHEN 157 THEN RETURN 'National Institute of Technology (NIT / REC), Rourkela';
            WHEN 159 THEN RETURN 'University of Delhi (DU), New Delhi';
            WHEN 168 THEN RETURN 'Visvesvaraya Technological University (VTU), Belagavi';
            WHEN 174 THEN RETURN 'West Bengal State Council of Technical and Vocational Education (WBSCTE), Kolkata';
            WHEN 175 THEN RETURN 'Jawaharlal Nehru Technological University (JNTU), Hyderabad';
            WHEN 182 THEN RETURN 'Andhra University, Visakhapatnam';
            WHEN 183 THEN RETURN 'Indian Institute of Science (IISc), Bangalore';
            WHEN 184 THEN RETURN 'Central Board of Secondary Education (CBSE), New Delhi';
            WHEN 185 THEN RETURN 'Cochin University of Science and Technology (CUSAT), Kochi';
            WHEN 186 THEN RETURN 'Acharya Nagarjuna University, Guntur';
            WHEN 187 THEN RETURN 'Jawaharlal Nehru Technological University Anantapur (JNTUA)';
            WHEN 188 THEN RETURN 'Indian Institute of Technology (IIT), Kharagpur';
            WHEN 191 THEN RETURN 'Indian Institute of Science (IISc), Bangalore';
            WHEN 192 THEN RETURN 'Osmania University, Hyderabad';
            WHEN 193 THEN RETURN 'National Institute of Technology (NIT / REC), Calicut';
            WHEN 202 THEN RETURN 'University of Hyderabad (UoH / Central University), Hyderabad';
            WHEN 208 THEN RETURN 'Anna University, Chennai';
            WHEN 222 THEN RETURN 'Indian Institute of Technology (IIT), Jodhpur';
            WHEN 227 THEN RETURN 'Acharya Nagarjuna University, Guntur';
            WHEN 237 THEN RETURN 'National Institute of Technology (NIT / REC), Tiruchirappalli';
            WHEN 248 THEN RETURN 'Indira Gandhi National Open University (IGNOU), New Delhi';
            WHEN 259 THEN RETURN 'University of Madras, Chennai';
            WHEN 260 THEN RETURN 'Bangalore University, Bengaluru';
            WHEN 271 THEN RETURN 'Savitribai Phule Pune University (SPPU / University of Pune)';
            WHEN 276 THEN RETURN 'National Institute of Technology Karnataka (NITK / REC), Surathkal';
            WHEN 288 THEN RETURN 'Savitribai Phule Pune University (SPPU / University of Pune)';
            WHEN 291 THEN RETURN 'Maulana Azad National Institute of Technology (MANIT), Bhopal';
            WHEN 295 THEN RETURN 'Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)';
            WHEN 303 THEN RETURN 'University of Delhi (DU), New Delhi';
            WHEN 308 THEN RETURN 'Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow';
            WHEN 313 THEN RETURN 'Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow';
            WHEN 316 THEN RETURN 'Indian Institute of Technology (IIT), Roorkee';
            WHEN 341 THEN RETURN 'Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)';
            WHEN 343 THEN RETURN 'Board of Secondary Education, Assam (SEBA)';
            WHEN 345 THEN RETURN 'West Bengal Board of Secondary Education (WBBSE)';
            WHEN 346 THEN RETURN 'West Bengal Council of Higher Secondary Education (WBCHSE)';
            WHEN 347 THEN RETURN 'Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)';
                ELSE NULL;
            END CASE;
        END IF;

        -- 2. Direct lookup in EDU_UNIVERSITY master table by UNIV_ID
        IF p_univ_id IS NOT NULL AND p_univ_id > 0 THEN
            BEGIN
                SELECT UNIV_NAME INTO v_name 
                FROM EDU_UNIVERSITY 
                WHERE UNIV_ID = p_univ_id AND ROWNUM = 1;
                RETURN v_name;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    NULL;
            END;
        END IF;

        -- 3. Harmonize from Collegename if university ID was not provided or not mapped
        IF p_collegename IS NOT NULL AND TRIM(p_collegename) NOT IN ('*', 'Not Specified', 'None', 'NA', 'N/A', '') THEN
            IF UPPER(TRIM(p_collegename)) = 'VTU' OR UPPER(p_collegename) LIKE '%VISVESVARAYA TECHNOLOGICAL%' OR UPPER(p_collegename) LIKE '%VISWESWARAIAH%' THEN
                RETURN 'Visvesvaraya Technological University (VTU), Belagavi';
            ELSIF UPPER(TRIM(p_collegename)) = 'IISC' OR UPPER(p_collegename) LIKE '%INDIAN INSTITUTE OF SCIENCE%' THEN
                RETURN 'Indian Institute of Science (IISc), Bangalore';
            ELSIF UPPER(TRIM(p_collegename)) = 'JNTU' OR UPPER(p_collegename) LIKE 'JAWAHARLAL NEHRU TECHNOLOGICAL%' THEN
                RETURN 'Jawaharlal Nehru Technological University (JNTU), Hyderabad';
            ELSIF UPPER(TRIM(p_collegename)) = 'IGNOU' OR UPPER(p_collegename) LIKE '%INDIRA GANDHI NATIONAL OPEN%' THEN
                RETURN 'Indira Gandhi National Open University (IGNOU), New Delhi';
            ELSIF UPPER(TRIM(p_collegename)) IN ('CBSE', 'CBSE BOARD', 'CBSE BOARD NEW DELHI') THEN
                RETURN 'Central Board of Secondary Education (CBSE), New Delhi';
            ELSIF UPPER(TRIM(p_collegename)) IN ('ICSE', 'ISC', 'ICSE BOARD', 'ISCE BOARD', 'ICSE / ISC BOARD') THEN
                RETURN 'Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)';
            ELSIF UPPER(TRIM(p_collegename)) IN ('PUE', 'PRE UNIVERSITY BOARD') THEN
                RETURN 'Department of Pre-University Education (PUE), Karnataka';
            ELSIF UPPER(TRIM(p_collegename)) IN ('REC, SURATKAL', 'KREC SURATHKAL', 'NIT, SURATHKAL', 'NIT SURATHKAL') THEN
                RETURN 'National Institute of Technology Karnataka (NITK / REC), Surathkal';
            ELSIF UPPER(TRIM(p_collegename)) IN ('REC, WARANGAL', 'NIT, WARANGAL') THEN
                RETURN 'National Institute of Technology (NIT / REC), Warangal';
            ELSIF UPPER(TRIM(p_collegename)) IN ('REC, TIRUCHY', 'NIT TRICHY') THEN
                RETURN 'National Institute of Technology (NIT / REC), Tiruchirappalli';
            ELSIF UPPER(TRIM(p_collegename)) IN ('JNU,DELHI', 'JNU') THEN
                RETURN 'Jawaharlal Nehru University (JNU), New Delhi';
            ELSIF UPPER(TRIM(p_collegename)) = 'ANNA UNIVERSITY' THEN
                RETURN 'Anna University, Chennai';
            ELSIF UPPER(TRIM(p_collegename)) = 'BANGALORE UNIVERSITY' THEN
                RETURN 'Bangalore University, Bengaluru';
            END IF;

            -- Check if collegename directly matches an existing university in EDU_UNIVERSITY
            BEGIN
                SELECT UNIV_NAME INTO v_name
                FROM EDU_UNIVERSITY
                WHERE UPPER(TRIM(UNIV_NAME)) = UPPER(TRIM(p_collegename))
                  AND ROWNUM = 1;
                RETURN v_name;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    NULL;
            END;

            RETURN TRIM(p_collegename);
        END IF;

        RETURN 'Not Specified';
    END FN_RESOLVE_LEGACY_UNIV;

    FUNCTION FN_NORMALIZE_GRADE(p_grade IN VARCHAR2) RETURN VARCHAR2 IS
        v_g VARCHAR2(100);
    BEGIN
        IF p_grade IS NULL THEN
            RETURN NULL;
        END IF;
        v_g := UPPER(TRIM(p_grade));
        IF v_g IN ('*', '-', '--', 'NA', 'N/A', '?', '??', 'NONE', 'NULL', 'NOT SPECIFIED', 'NOT APPLICABLE', '') THEN
            RETURN NULL;
        END IF;

        IF v_g LIKE '%DISTINCTION%' OR v_g = 'FCD' OR v_g = 'FIRST CLASS WITH DISTINCTION' THEN
            RETURN 'First Class with Distinction';
        ELSIF v_g LIKE '%FIRST%' OR v_g = '1ST CLASS' OR v_g = '1ST' OR v_g = 'FC' OR v_g = 'I CLASS' OR v_g = 'CLASS I' THEN
            RETURN 'First Class';
        ELSIF v_g LIKE '%SECOND%' OR v_g = '2ND CLASS' OR v_g = '2ND' OR v_g = 'SC' OR v_g = 'II CLASS' OR v_g = 'CLASS II' THEN
            RETURN 'Second Class';
        ELSIF v_g LIKE '%PASS%' OR v_g = 'PASSED' OR v_g = 'PC' OR v_g LIKE '%THIRD%' OR v_g = '3RD CLASS' OR v_g = '3RD' THEN
            RETURN 'Pass Class';
        ELSIF v_g IN ('GRADE A+', 'A+') THEN
            RETURN 'Grade A+';
        ELSIF v_g IN ('GRADE A', 'A') THEN
            RETURN 'Grade A';
        ELSIF v_g IN ('GRADE B+', 'B+') THEN
            RETURN 'Grade B+';
        ELSIF v_g IN ('GRADE B', 'B') THEN
            RETURN 'Grade B';
        ELSIF v_g LIKE '%EXEMPLARY%' THEN
            RETURN 'Exemplary';
        END IF;

        RETURN NULL;
    END FN_NORMALIZE_GRADE;


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
        v_grade_norm     VARCHAR2(100);
        v_is_highest     NUMBER(1);
        v_emp_name       VARCHAR2(150);
        v_designation    VARCHAR2(100);
        v_division       VARCHAR2(100);
        v_exists         NUMBER;
        v_emp_found      NUMBER := 0;
        v_row_status     VARCHAR2(20);
        v_err_msg        VARCHAR2(4000);
        
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

                    -- 2. Resolve University / Institute Name using comprehensive legacy catalog & aliases
                    v_inst_name := FN_RESOLVE_LEGACY_UNIV(v_univinstid, v_collegename);

                    -- If university is not yet registered in EDU_UNIVERSITY master table,
                    -- automatically insert it globally so it is available to ALL users across the application
                    IF v_inst_name IS NOT NULL AND TRIM(v_inst_name) NOT IN ('Not Specified', '*', 'None', 'NA', 'N/A', '') THEN
                        DECLARE
                            v_univ_cnt NUMBER := 0;
                        BEGIN
                            SELECT COUNT(*) INTO v_univ_cnt 
                            FROM EDU_UNIVERSITY 
                            WHERE UPPER(TRIM(UNIV_NAME)) = UPPER(TRIM(v_inst_name));

                            IF v_univ_cnt = 0 THEN
                                INSERT INTO EDU_UNIVERSITY (UNIV_ID, UNIV_NAME, STATE_OR_TYPE, IS_ACTIVE)
                                VALUES (SEQ_EDU_UNIVERSITY.NEXTVAL, TRIM(v_inst_name), 'Recognized University / Board', 1);
                            END IF;
                        EXCEPTION
                            WHEN OTHERS THEN
                                NULL;
                        END;
                    END IF;

                    -- 3. Resolve Subject / Specialization and Sub-Qualification
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
                                    -- Resolve canonical Sub-Qualification Name from EDU_QUAL_SUB for the mapped sub code
                                    BEGIN
                                        SELECT SUB_NAME INTO v_sub_name
                                        FROM EDU_QUAL_SUB
                                        WHERE SUB_CODE = v_sub_code
                                          AND (MAIN_QUAL_CODE = v_main_code OR MAIN_QUAL_CODE IS NULL)
                                          AND ROWNUM = 1;
                                    EXCEPTION
                                        WHEN NO_DATA_FOUND THEN
                                            NULL; -- Retain fallback
                                    END;
                            END;
                    END;

                    -- 4. Highest Qualification Flag
                    IF UPPER(TRIM(NVL(v_highestqfn, 'N'))) IN ('Y', 'YES', '1', 'TRUE') THEN
                        v_is_highest := 1;
                    ELSE
                        v_is_highest := 0;
                    END IF;

                -- 4b. Resolve and Normalize Grade / Division
                v_grade_norm := FN_NORMALIZE_GRADE(v_grade);

                -- 5. Status Determination:
                -- If record contains placeholder values like '*', or missing/invalid grade, force to Draft so employee must correct it
                IF TRIM(v_collegename) = '*' OR TRIM(v_inst_name) = '*' OR TRIM(v_inst_name) = 'Not Specified' 
                   OR TRIM(v_spec) = '*' OR v_grade_norm IS NULL 
                   OR TRIM(NVL(v_percentage, '*')) = '*' OR UPPER(TRIM(v_percentage)) IN ('N/A', 'NA', '-', '--', '?', 'NOT SPECIFIED', 'NONE', 'NULL')
                   OR v_yearpassed IS NULL OR v_yearpassed = 0 THEN
                    v_row_status := 'Draft';
                ELSE
                    v_row_status := v_status_to_use;
                END IF;

                -- Check if this employee already has this exact qualification, stream, and passing year in EDU_EMP_EDUCATION
                -- (handles dirty legacy duplicates with different CTREDN, or records entered manually before migration)
                SELECT COUNT(*) INTO v_exists
                FROM EDU_EMP_EDUCATION
                WHERE PCNO = v_pcno
                  AND QUAL_MAIN_CODE = v_main_code
                  AND QUAL_SUB_CODE = v_sub_code
                  AND YEAR_PASSED = NVL(v_yearpassed, 2000);

                IF v_exists > 0 THEN
                    v_total_skipped := v_total_skipped + 1;
                ELSE
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
                        v_grade_norm,
                        v_is_highest,
                        NULL,
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
            END IF;

        END LOOP;

        CLOSE c_old_data;

        -- 7. Deduplicate IS_HIGHEST Flag:
        -- Ensure each employee has at most ONE qualification marked as highest (IS_HIGHEST = 1).
        -- In legacy HR databases, multiple records often have HIGHESTQFN = 'Y'.
        -- Retain IS_HIGHEST = 1 only for the latest YEAR_PASSED / highest EDU_ID.
        MERGE INTO EDU_EMP_EDUCATION dest
        USING (
            SELECT EDU_ID, 
                   ROW_NUMBER() OVER (PARTITION BY PCNO ORDER BY YEAR_PASSED DESC, EDU_ID DESC) AS RN
            FROM EDU_EMP_EDUCATION
            WHERE IS_HIGHEST = 1
        ) src
        ON (dest.EDU_ID = src.EDU_ID AND src.RN > 1)
        WHEN MATCHED THEN UPDATE SET dest.IS_HIGHEST = 0;

        -- Ensure every employee with qualifications has at least ONE highest qualification marked
        MERGE INTO EDU_EMP_EDUCATION dest
        USING (
            SELECT EDU_ID,
                   ROW_NUMBER() OVER (PARTITION BY PCNO ORDER BY YEAR_PASSED DESC, EDU_ID DESC) AS RN
            FROM EDU_EMP_EDUCATION e1
            WHERE NOT EXISTS (
                SELECT 1 FROM EDU_EMP_EDUCATION e2 WHERE e2.PCNO = e1.PCNO AND e2.IS_HIGHEST = 1
            )
        ) src
        ON (dest.EDU_ID = src.EDU_ID AND src.RN = 1)
        WHEN MATCHED THEN UPDATE SET dest.IS_HIGHEST = 1;

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
            v_err_msg := SUBSTR(SQLERRM, 1, 3900);
            IF c_old_data%ISOPEN THEN
                CLOSE c_old_data;
            END IF;
            ROLLBACK;
            UPDATE EDU_MIGRATION_LOG
            SET STATUS = 'FAILED',
                LOG_DETAILS = 'Error: ' || v_err_msg
            WHERE LOG_ID = v_log_id;
            COMMIT;
            p_out_total_read := v_total_read;
            p_out_migrated   := v_total_migrated;
            p_out_skipped    := v_total_skipped;
            p_out_message    := 'Error during migration: ' || v_err_msg;
    END SP_RUN_MIGRATION;

END PKG_EDU_MIGRATION;
/

-- Clean up imported remarks from any previously migrated records
UPDATE EDU_EMP_EDUCATION 
SET REMARKS = NULL 
WHERE REMARKS LIKE 'Migrated from%' OR REMARKS LIKE 'Auto-synced%';
COMMIT;

