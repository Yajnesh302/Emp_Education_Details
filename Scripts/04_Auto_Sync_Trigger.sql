-- ========================================================================
-- APPLICATION: EMPLOYEE EDUCATION MANAGEMENT SYSTEM
-- TARGET DATABASE: ORACLE 11g R2 / 12c+
-- SCRIPT: 04_Auto_Sync_Trigger.sql
-- DESCRIPTION: Real-time Auto-Sync Database Trigger from HR_EMP_EDUCATION
--              to EDU_EMP_EDUCATION. Fires on INSERT or UPDATE.
-- ========================================================================

CREATE OR REPLACE TRIGGER TRG_SYNC_HR_EMP_EDU
AFTER INSERT OR UPDATE ON HR_EMP_EDUCATION
FOR EACH ROW
DECLARE
    v_main_code  VARCHAR2(20)  := 'OTHER';
    v_sub_code   VARCHAR2(50)  := 'GENERAL';
    v_main_name  VARCHAR2(200) := 'OTHER';
    v_sub_name   VARCHAR2(200) := 'General';
    v_inst_name  VARCHAR2(250) := 'Not Specified';
    v_emp_name   VARCHAR2(150) := 'Employee ' || :NEW.PCNO;
    v_desig      VARCHAR2(100) := '';
    v_div        VARCHAR2(100) := '';
    v_is_highest NUMBER(1)     := 0;
    v_exists     NUMBER        := 0;
BEGIN
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

    -- 2. Resolve University / Board Name
    v_inst_name := NVL(:NEW.COLLEGENAME, 'Not Specified');
    IF :NEW.UNIVINSTID IS NOT NULL AND :NEW.UNIVINSTID > 0 THEN
        BEGIN
            SELECT UNIV_NAME INTO v_inst_name 
            FROM EDU_UNIVERSITY 
            WHERE UNIV_ID = :NEW.UNIVINSTID AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    -- 3. Resolve Subject / Specialization Name
    v_sub_name := 'General';
    IF :NEW.SUBJECTID IS NOT NULL AND :NEW.SUBJECTID > 0 THEN
        BEGIN
            SELECT SUBJECT_NAME INTO v_sub_name 
            FROM EDU_SUBJECT 
            WHERE SUBJECT_ID = :NEW.SUBJECTID AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    -- 4. Highest Qualification Flag
    IF UPPER(TRIM(NVL(:NEW.HIGHESTQFN, 'N'))) IN ('Y', 'YES', '1', 'TRUE') THEN
        v_is_highest := 1;
        -- Clear previous highest for this PCNO
        UPDATE EDU_EMP_EDUCATION SET IS_HIGHEST = 0 WHERE PCNO = :NEW.PCNO;
    ELSE
        v_is_highest := 0;
    END IF;

    -- 5. Fetch Employee Details from hrdata.empdetails if available
    BEGIN
        EXECUTE IMMEDIATE 
            'SELECT NAME, DESIGNATION, DIVNAME FROM hrdata.empdetails WHERE PCNO = :1 AND ROWNUM = 1'
            INTO v_emp_name, v_desig, v_div
            USING :NEW.PCNO;
    EXCEPTION
        WHEN OTHERS THEN
            v_emp_name := 'Employee ' || :NEW.PCNO;
    END;

    -- 6. Check if record already exists in EDU_EMP_EDUCATION
    SELECT COUNT(*) INTO v_exists 
    FROM EDU_EMP_EDUCATION 
    WHERE PCNO = :NEW.PCNO AND OLD_CTREDN = :NEW.CTREDN;

    IF v_exists > 0 THEN
        -- UPDATE existing row
        UPDATE EDU_EMP_EDUCATION SET
            EMP_NAME        = v_emp_name,
            DESIGNATION     = v_desig,
            DIVISION_NAME   = v_div,
            QUAL_MAIN_CODE  = v_main_code,
            QUAL_MAIN_NAME  = v_main_name,
            QUAL_SUB_CODE   = v_sub_code,
            QUAL_SUB_NAME   = v_sub_name,
            YEAR_PASSED     = NVL(:NEW.YEARPASSED, 2000),
            INSTITUTE_NAME  = v_inst_name,
            COLLEGE_NAME    = :NEW.COLLEGENAME,
            SPECIALIZATION  = v_sub_name,
            CGPA_MARKS      = NVL(TO_CHAR(:NEW.PERCENTAGE), 'N/A'),
            GRADE_DIVISION  = NVL(:NEW.GRADE, 'Pass'),
            IS_HIGHEST      = v_is_highest,
            MODIFIED_ON     = SYSDATE,
            MODIFIED_BY     = 'AUTO_TRIGGER'
        WHERE PCNO = :NEW.PCNO AND OLD_CTREDN = :NEW.CTREDN;
    ELSE
        -- INSERT new row with default 'Draft' status so employee can review and submit
        INSERT INTO EDU_EMP_EDUCATION (
            PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
            QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
            YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
            CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS,
            SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED, OLD_CTREDN
        ) VALUES (
            :NEW.PCNO, v_emp_name, v_desig, v_div,
            v_main_code, v_main_name, v_sub_code, v_sub_name,
            NVL(:NEW.YEARPASSED, 2000), v_inst_name, :NEW.COLLEGENAME, v_sub_name,
            NVL(TO_CHAR(:NEW.PERCENTAGE), 'N/A'), NVL(:NEW.GRADE, 'Pass'), v_is_highest,
            'Auto-synced from HR_EMP_EDUCATION', 'Draft',
            NULL, SYSDATE, 'AUTO_TRIGGER', 1, :NEW.CTREDN
        );
    END IF;
END;
/
