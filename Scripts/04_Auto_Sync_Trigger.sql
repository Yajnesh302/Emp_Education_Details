-- ========================================================================
-- APPLICATION: EMPLOYEE EDUCATION MANAGEMENT SYSTEM
-- TARGET DATABASE: ORACLE 11g R2 / 12c+
-- SCRIPT: 04_Auto_Sync_Trigger.sql
-- DESCRIPTION: Real-time Auto-Sync Database Trigger from HR_EMP_EDUCATION
--              to EDU_EMP_EDUCATION. Fires ONLY on NEW INSERT entries.
--              (Existing data changes / updates will NOT trigger or modify records).
-- ========================================================================
SET DEFINE OFF;

-- OPTIONAL: Explicitly drop existing trigger if you want a fresh start
-- (Note: CREATE OR REPLACE will automatically overwrite it anyway)
-- DROP TRIGGER TRG_SYNC_HR_EMP_EDU;

-- TIP FOR TESTING: If you want to test on your backup table without touching live data,
-- simply change "ON HR_EMP_EDUCATION" below to "ON BACKUP_HR_EMP_EDUCATION".

CREATE OR REPLACE TRIGGER TRG_SYNC_HR_EMP_EDU
AFTER INSERT ON HR_EMP_EDUCATION
FOR EACH ROW
DECLARE
    v_main_code  VARCHAR2(20)  := 'OTHER';
    v_sub_code   VARCHAR2(50)  := 'GENERAL';
    v_main_name  VARCHAR2(200) := 'OTHER';
    v_sub_name   VARCHAR2(200) := 'General';
    v_spec       VARCHAR2(200) := 'General';
    v_inst_name  VARCHAR2(250) := NULL;
    v_emp_name   VARCHAR2(150);
    v_desig      VARCHAR2(100);
    v_div        VARCHAR2(100);
    v_grade_norm VARCHAR2(50)  := NULL;
    v_is_highest NUMBER(1)     := 0;
    v_exists     NUMBER        := 0;
    v_emp_found  NUMBER        := 0;
BEGIN
    -- 0. Check whether PCNO exists in hrdata.empdetails
    -- Only import into EDU_EMP_EDUCATION if PCNO exists in hrdata.empdetails
    BEGIN
        EXECUTE IMMEDIATE 
            'SELECT NAME, DESIGNATION, DIVNAME FROM hrdata.empdetails WHERE TRIM(PCNO) = TRIM(:1) AND ROWNUM = 1'
            INTO v_emp_name, v_desig, v_div
            USING :NEW.PCNO;
        v_emp_found := 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            v_emp_found := 0;
        WHEN OTHERS THEN
            -- Fallback check for synonym or local table without schema prefix
            BEGIN
                EXECUTE IMMEDIATE 
                    'SELECT NAME, DESIGNATION, DIVNAME FROM empdetails WHERE TRIM(PCNO) = TRIM(:1) AND ROWNUM = 1'
                    INTO v_emp_name, v_desig, v_div
                    USING :NEW.PCNO;
                v_emp_found := 1;
            EXCEPTION
                WHEN OTHERS THEN
                    v_emp_found := 0;
            END;
    END;

    -- If PCNO does not exist in hrdata.empdetails, do NOT import
    IF v_emp_found = 0 THEN
        RETURN;
    END IF;

    -- Fallback default name if blank in empdetails
    IF v_emp_name IS NULL OR TRIM(v_emp_name) = '' THEN
        v_emp_name := 'Employee ' || :NEW.PCNO;
    END IF;

    -- 1. Resolve Qualification Mapping from EDU_QUAL_MAPPING
    BEGIN
        SELECT m.NEW_QUAL_MAIN_CODE, NVL(m.NEW_QUAL_SUB_CODE, 'GENERAL')
        INTO v_main_code, v_sub_code
        FROM EDU_QUAL_MAPPING m
        WHERE m.OLD_QFN_ID = :NEW.QFNID AND m.IS_ACTIVE = 1 AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            v_main_code := 'OTHER';
            v_sub_code  := 'GENERAL';
    END;

    -- Resolve Main Qualification Name
    BEGIN
        SELECT QUAL_NAME INTO v_main_name 
        FROM EDU_QUAL_MAIN 
        WHERE QUAL_CODE = v_main_code AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            v_main_name := v_main_code;
    END;

    -- 2. Resolve University / Board Name using canonical legacy resolution
    v_inst_name := PKG_EDU_MIGRATION.FN_RESOLVE_LEGACY_UNIV(:NEW.UNIVINSTID, :NEW.COLLEGENAME);
    IF v_inst_name IS NOT NULL AND TRIM(v_inst_name) IN ('Not Specified', '*', 'None', 'NA', 'N/A', '') THEN
        v_inst_name := NULL;
    END IF;

    -- If university is valid and not yet registered in EDU_UNIVERSITY master table,
    -- automatically insert it globally so it is available to ALL users across the application
    IF v_inst_name IS NOT NULL THEN
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

    -- 3. Resolve Subject / Specialization and Sub-Qualification Name
    -- (In legacy HR_EMP_EDUCATION, there is no separate SUB_QUALIFICATION column; branch was stored in SUBJECTID)
    v_spec := 'General';
    IF :NEW.SUBJECTID IS NOT NULL AND :NEW.SUBJECTID > 0 THEN
        BEGIN
            SELECT SUBJECT_NAME INTO v_spec 
            FROM EDU_SUBJECT 
            WHERE SUBJECT_ID = :NEW.SUBJECTID AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    -- Normalize '*' or blank subject to 'General'
    IF TRIM(v_spec) = '*' OR v_spec IS NULL OR TRIM(v_spec) = '' THEN
        v_spec := 'General';
    END IF;

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

    -- 4. Check if record already exists in EDU_EMP_EDUCATION
    -- If record with same OLD_CTREDN or exact same qualification + year exists, DO NOT insert duplicate
    SELECT COUNT(*) INTO v_exists 
    FROM EDU_EMP_EDUCATION 
    WHERE PCNO = :NEW.PCNO 
      AND (
          OLD_CTREDN = :NEW.CTREDN
          OR (QUAL_MAIN_CODE = v_main_code AND QUAL_SUB_CODE = v_sub_code AND YEAR_PASSED = NVL(:NEW.YEARPASSED, 2000))
      );

    IF v_exists > 0 THEN
        -- Existing record found: do not trigger or alter existing records
        RETURN;
    END IF;

    -- 5. Highest Qualification Flag (Only evaluated for brand new entries)
    IF UPPER(TRIM(NVL(:NEW.HIGHESTQFN, 'N'))) IN ('Y', 'YES', '1', 'TRUE') THEN
        v_is_highest := 1;
        -- Clear previous highest for this PCNO in the new system
        UPDATE EDU_EMP_EDUCATION SET IS_HIGHEST = 0 WHERE PCNO = :NEW.PCNO;
    ELSE
        v_is_highest := 0;
    END IF;

    -- 6. Normalize Grade / Division (do not fabricate fake 'Pass' values)
    v_grade_norm := PKG_EDU_MIGRATION.FN_NORMALIZE_GRADE(:NEW.GRADE);

    -- 7. INSERT new row with default 'Draft' status so employee can review and submit
    INSERT INTO EDU_EMP_EDUCATION (
        PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
        QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
        YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
        CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS,
        SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED, OLD_CTREDN
    ) VALUES (
        :NEW.PCNO, v_emp_name, v_desig, v_div,
        v_main_code, v_main_name, v_sub_code, v_sub_name,
        NVL(:NEW.YEARPASSED, 2000), v_inst_name, :NEW.COLLEGENAME, v_spec,
        NVL(TO_CHAR(:NEW.PERCENTAGE), 'N/A'), v_grade_norm, v_is_highest,
        NULL, 'Draft',
        NULL, SYSDATE, 'AUTO_TRIGGER', 1, :NEW.CTREDN
    );
END;
/
