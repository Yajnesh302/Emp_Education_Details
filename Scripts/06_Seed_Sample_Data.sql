-- ========================================================================
-- APPLICATION: EMPLOYEE EDUCATION MANAGEMENT SYSTEM
-- SCRIPT: 06_Seed_Sample_Data.sql
-- DESCRIPTION: High-quality sample employee education records (35 employees,
--              ~130 education records) for comprehensive pagination, filter,
--              and search testing in AdminDashboard.
-- ========================================================================
SET DEFINE OFF;

-- Clean up any prior test run for PCNO range 2001 - 2035 (Idempotent)
DELETE FROM EDU_EMP_EDUCATION WHERE PCNO BETWEEN '2001' AND '2035';
COMMIT;

-- ------------------------------------------------------------------------
-- Employee 2001: Rajesh Sharma (DKRM/ITISG - Senior Scientist)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2001', 'Rajesh Sharma', 'Senior Scientist', 'DKRM/ITISG',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2004, 'Central Board of Secondary Education', 'Delhi Public School, R.K. Puram', 'General Science & Mathematics',
    '88.5%', 'Distinction', 0, 'Completed with school merit', 'Submitted', SYSDATE - 120, SYSDATE - 150, '2001', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2001', 'Rajesh Sharma', 'Senior Scientist', 'DKRM/ITISG',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2006, 'Central Board of Secondary Education', 'Delhi Public School, R.K. Puram', 'Computer Science Stream',
    '91.2%', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 120, SYSDATE - 150, '2001', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2001', 'Rajesh Sharma', 'Senior Scientist', 'DKRM/ITISG',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'COMPS', 'Computer Science & Engineering',
    2010, 'Visvesvaraya Technological University (VTU)', 'BMS College of Engineering, Bangalore', 'Computer Science',
    '8.6 CGPA', 'First Class with Distinction', 0, 'Rank holder in college', 'Submitted', SYSDATE - 120, SYSDATE - 150, '2001', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2001', 'Rajesh Sharma', 'Senior Scientist', 'DKRM/ITISG',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'SOFTE', 'Software Engineering',
    2013, 'Indian Institute of Science (IISc)', 'IISc Bangalore', 'Advanced Software Systems & AI',
    '9.2 CGPA', 'First Class with Distinction', 1, 'Highest degree awarded', 'Submitted', SYSDATE - 120, SYSDATE - 150, '2001', 0
);

-- ------------------------------------------------------------------------
-- Employee 2002: Priya Sundaram (AVIONICS - Lead Systems Engineer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2002', 'Priya Sundaram', 'Lead Systems Engineer', 'AVIONICS',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2005, 'Karnataka Secondary Education Examination Board', 'National High School Bangalore', 'General Secondary',
    '92.4%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 110, SYSDATE - 140, '2002', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2002', 'Priya Sundaram', 'Lead Systems Engineer', 'AVIONICS',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCME', 'Science: Physics, Chemistry, Mathematics, Electronics (PCME)',
    2007, 'Department of Pre-University Education Karnataka', 'National College Basavanagudi', 'PCME Stream',
    '89.6%', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 110, SYSDATE - 140, '2002', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2002', 'Priya Sundaram', 'Lead Systems Engineer', 'AVIONICS',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'E&C', 'Electronics & Communication',
    2011, 'Visvesvaraya Technological University (VTU)', 'RV College of Engineering, Bangalore', 'Electronics & Communication',
    '8.8 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 110, SYSDATE - 140, '2002', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2002', 'Priya Sundaram', 'Lead Systems Engineer', 'AVIONICS',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'RACE', 'Radar & Communication Engineering',
    2014, 'Indian Institute of Technology (IIT) Madras', 'IIT Madras', 'Signal & Radar Systems',
    '9.4 CGPA', 'First Class with Distinction', 1, 'Sponsored candidate program', 'Submitted', SYSDATE - 110, SYSDATE - 140, '2002', 0
);

-- ------------------------------------------------------------------------
-- Employee 2003: Amitabh Verma (RADAR SYSTEMS - Scientist / Engineer SD)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2003', 'Amitabh Verma', 'Scientist / Engineer SD', 'RADAR SYSTEMS',
    '10TH', '10TH / MATRICULATION / SSLC', 'ICSE', 'ICSE Secondary Curriculum (Class X)',
    2002, 'Council for the Indian School Certificate Examinations', 'St. Joseph''s Boys'' High School', 'General Science',
    '85.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 95, SYSDATE - 130, '2003', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2003', 'Amitabh Verma', 'Scientist / Engineer SD', 'RADAR SYSTEMS',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2004, 'Council for the Indian School Certificate Examinations', 'St. Joseph''s PU College', 'PCMB Science',
    '86.5%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 95, SYSDATE - 130, '2003', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2003', 'Amitabh Verma', 'Scientist / Engineer SD', 'RADAR SYSTEMS',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'ELTE', 'Electronics & Telecommunication',
    2008, 'University of Mumbai', 'VJTI Mumbai', 'Telecommunication Engineering',
    '78.4%', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 95, SYSDATE - 130, '2003', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2003', 'Amitabh Verma', 'Scientist / Engineer SD', 'RADAR SYSTEMS',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'RACE', 'Radar & Communication Engineering',
    2011, 'Indian Institute of Technology (IIT) Kharagpur', 'IIT Kharagpur', 'Microwave & Radar Technology',
    '8.9 CGPA', 'First Class with Distinction', 1, 'Published 2 IEEE papers', 'Submitted', SYSDATE - 95, SYSDATE - 130, '2003', 0
);

-- ------------------------------------------------------------------------
-- Employee 2004: Sunita Deshmukh (QUALITY ASSURANCE - Technical Officer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2004', 'Sunita Deshmukh', 'Technical Officer', 'QUALITY ASSURANCE',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2007, 'Maharashtra State Board of Secondary and Higher Secondary Education', 'Balmohan Vidyamandir Pune', 'General',
    '84.2%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 80, SYSDATE - 120, '2004', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2004', 'Sunita Deshmukh', 'Technical Officer', 'QUALITY ASSURANCE',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2009, 'Maharashtra State Board', 'Fergusson College Pune', 'Science',
    '82.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 80, SYSDATE - 120, '2004', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2004', 'Sunita Deshmukh', 'Technical Officer', 'QUALITY ASSURANCE',
    'BSC', 'BACHELOR OF SCIENCE (B.SC.)', 'ELEC', 'Electronics',
    2012, 'Savitribai Phule Pune University', 'Fergusson College Pune', 'Applied Electronics',
    '79.5%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 80, SYSDATE - 120, '2004', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2004', 'Sunita Deshmukh', 'Technical Officer', 'QUALITY ASSURANCE',
    'MSC', 'MASTER OF SCIENCE (M.SC.)', 'ELEC', 'Electronics',
    2014, 'Savitribai Phule Pune University', 'Department of Electronic Science', 'Embedded Systems & Instrumentation',
    '8.2 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 80, SYSDATE - 120, '2004', 0
);

-- ------------------------------------------------------------------------
-- Employee 2005: Karthik Ramanathan (PROPULSION - Mechanical Specialist)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2005', 'Karthik Ramanathan', 'Mechanical Specialist', 'PROPULSION',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2006, 'Central Board of Secondary Education', 'Kendriya Vidyalaya IIT Chennai', 'General',
    '90.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 70, SYSDATE - 110, '2005', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2005', 'Karthik Ramanathan', 'Mechanical Specialist', 'PROPULSION',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2008, 'Central Board of Secondary Education', 'Kendriya Vidyalaya CLRI', 'PCMC',
    '88.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 70, SYSDATE - 110, '2005', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2005', 'Karthik Ramanathan', 'Mechanical Specialist', 'PROPULSION',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'DMECH', 'Mechanical Engineering',
    2012, 'Anna University', 'College of Engineering Guindy (CEG), Chennai', 'Thermal Engineering',
    '8.7 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 70, SYSDATE - 110, '2005', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2005', 'Karthik Ramanathan', 'Mechanical Specialist', 'PROPULSION',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'AERO', 'Aeronautical Engineering',
    2015, 'Indian Institute of Technology (IIT) Kanpur', 'IIT Kanpur', 'Aerospace Propulsion & Combustion',
    '9.1 CGPA', 'First Class with Distinction', 1, 'Thesis on turbomachinery', 'Submitted', SYSDATE - 70, SYSDATE - 110, '2005', 0
);

-- ------------------------------------------------------------------------
-- Employee 2006: Ananya Mukherjee (DKRM/ITISG - Data Scientist) [Draft State]
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2006', 'Ananya Mukherjee', 'Data Scientist', 'DKRM/ITISG',
    '10TH', '10TH / MATRICULATION / SSLC', 'ICSE', 'ICSE Secondary Curriculum (Class X)',
    2008, 'Council for the Indian School Certificate Examinations', 'Modern High School for Girls, Kolkata', 'General',
    '93.6%', 'Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 45, '2006', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2006', 'Ananya Mukherjee', 'Data Scientist', 'DKRM/ITISG',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMS', 'Science: Physics, Chemistry, Mathematics, Statistics (PCMS)',
    2010, 'Council for the Indian School Certificate Examinations', 'South Point High School, Kolkata', 'PCMS',
    '94.2%', 'Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 45, '2006', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2006', 'Ananya Mukherjee', 'Data Scientist', 'DKRM/ITISG',
    'BSC', 'BACHELOR OF SCIENCE (B.SC.)', 'CS', 'Computer Science',
    2013, 'University of Calcutta', 'St. Xavier''s College Kolkata', 'Computer Science & Statistics',
    '84.5%', 'First Class with Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 45, '2006', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2006', 'Ananya Mukherjee', 'Data Scientist', 'DKRM/ITISG',
    'MSC', 'MASTER OF SCIENCE (M.SC.)', 'CS', 'Computer Science',
    2015, 'University of Delhi', 'Department of Computer Science', 'Data Science & Machine Learning',
    '8.9 CGPA', 'First Class with Distinction', 1, 'In-progress draft review', 'Draft', NULL, SYSDATE - 45, '2006', 0
);

-- ------------------------------------------------------------------------
-- Employee 2007: Vikramaditya Rathore (SIGNAL PROCESSING - Senior Engineer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2007', 'Vikramaditya Rathore', 'Senior Engineer', 'SIGNAL PROCESSING',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2003, 'Central Board of Secondary Education', 'St. Xavier''s Senior Secondary School, Jaipur', 'General',
    '81.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 60, SYSDATE - 100, '2007', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2007', 'Vikramaditya Rathore', 'Senior Engineer', 'SIGNAL PROCESSING',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2005, 'Central Board of Secondary Education', 'St. Xavier''s Jaipur', 'Science',
    '83.5%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 60, SYSDATE - 100, '2007', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2007', 'Vikramaditya Rathore', 'Senior Engineer', 'SIGNAL PROCESSING',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'E&C', 'Electronics & Communication',
    2009, 'Rajasthan Technical University', 'Malaviya National Institute of Technology (MNIT) Jaipur', 'Digital Signal Processing',
    '7.8 CGPA', 'First Class', 0, NULL, 'Submitted', SYSDATE - 60, SYSDATE - 100, '2007', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2007', 'Vikramaditya Rathore', 'Senior Engineer', 'SIGNAL PROCESSING',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'RACE', 'Radar & Communication Engineering',
    2012, 'Indian Institute of Technology (IIT) Delhi', 'IIT Delhi', 'Advanced Signal Processing & Estimation',
    '8.7 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 60, SYSDATE - 100, '2007', 0
);

-- ------------------------------------------------------------------------
-- Employee 2008: Meenakshi Iyer (D-ADMIN/STORE - HR Specialist)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2008', 'Meenakshi Iyer', 'HR Specialist', 'D-ADMIN/STORE',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2009, 'Tamil Nadu State Board', 'PSBB Millennium School Chennai', 'General',
    '89.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 50, SYSDATE - 90, '2008', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2008', 'Meenakshi Iyer', 'HR Specialist', 'D-ADMIN/STORE',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'COMM', 'Commerce',
    2011, 'Tamil Nadu State Board', 'PSBB Senior Secondary Chennai', 'Commerce & Accountancy',
    '91.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 50, SYSDATE - 90, '2008', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2008', 'Meenakshi Iyer', 'HR Specialist', 'D-ADMIN/STORE',
    'BBA', 'BACHELOR OF BUSINESS ADMINISTRATION (B.B.A.)', 'HR', 'Human Resources',
    2014, 'University of Madras', 'Loyola College Chennai', 'Business Administration & HR',
    '82.0%', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 50, SYSDATE - 90, '2008', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2008', 'Meenakshi Iyer', 'HR Specialist', 'D-ADMIN/STORE',
    'MBA', 'MASTER OF BUSINESS ADMINISTRATION (M.B.A.)', 'HRM', 'Human Resource Management',
    2016, 'Bharathidasan Institute of Management (BIM) Trichy', 'BIM Trichy', 'HR & Organizational Development',
    '8.5 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 50, SYSDATE - 90, '2008', 0
);

-- ------------------------------------------------------------------------
-- Employee 2009: Farhan Akhtar Qureshi (R&D/EMBEDDED - Embedded Systems Engineer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2009', 'Farhan Akhtar Qureshi', 'Embedded Systems Engineer', 'R&D/EMBEDDED',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2010, 'Central Board of Secondary Education', 'Jamia Senior Secondary School, New Delhi', 'General',
    '87.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 40, SYSDATE - 75, '2009', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2009', 'Farhan Akhtar Qureshi', 'Embedded Systems Engineer', 'R&D/EMBEDDED',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2012, 'Aligarh Muslim University Board', 'Sayyid Hamid Senior Secondary School AMU', 'PCMC',
    '85.5%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 40, SYSDATE - 75, '2009', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2009', 'Farhan Akhtar Qureshi', 'Embedded Systems Engineer', 'R&D/EMBEDDED',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'COMPE', 'Computer Engineering',
    2016, 'Jamia Millia Islamia', 'Faculty of Engineering & Technology, New Delhi', 'Embedded Computing Systems',
    '8.4 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 40, SYSDATE - 75, '2009', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2009', 'Farhan Akhtar Qureshi', 'Embedded Systems Engineer', 'R&D/EMBEDDED',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'COMPS', 'Computer Science & Engineering',
    2018, 'Indian Institute of Technology (IIT) Roorkee', 'IIT Roorkee', 'Real-Time Operating Systems (RTOS)',
    '9.0 CGPA', 'First Class with Distinction', 1, 'Final thesis pending HR audit', 'Draft', NULL, SYSDATE - 40, '2009', 0
);

-- ------------------------------------------------------------------------
-- Employee 2010: Deepa Narayan Nair (QUALITY ASSURANCE - Quality Inspector)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2010', 'Deepa Narayan Nair', 'Quality Inspector', 'QUALITY ASSURANCE',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2007, 'Kerala Board of Public Examinations', 'St. Teresa''s Convent Girls HSS Ernakulam', 'General',
    '95.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 35, SYSDATE - 60, '2010', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2010', 'Deepa Narayan Nair', 'Quality Inspector', 'QUALITY ASSURANCE',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2009, 'Directorate of Higher Secondary Education Kerala', 'Maharaja''s College Ernakulam', 'Science',
    '92.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 35, SYSDATE - 60, '2010', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2010', 'Deepa Narayan Nair', 'Quality Inspector', 'QUALITY ASSURANCE',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'INSTR', 'Instrumentation Engineering',
    2013, 'APJ Abdul Kalam Technological University (KTU)', 'Govt Engineering College Thrissur', 'Industrial Process Instrumentation',
    '8.6 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 35, SYSDATE - 60, '2010', 0
);

-- ------------------------------------------------------------------------
-- Employee 2011: Suresh Babu Goud (DKRM/MAINT - Senior Technician)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2011', 'Suresh Babu Goud', 'Senior Technician', 'DKRM/MAINT',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2001, 'Board of Secondary Education Andhra Pradesh', 'ZP High School Warangal', 'General',
    '72.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 30, SYSDATE - 50, '2011', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2011', 'Suresh Babu Goud', 'Senior Technician', 'DKRM/MAINT',
    'ITI', 'INDUSTRIAL TRAINING INSTITUTE (ITI)', 'ELECT', 'Electrician / Industrial Electrician',
    2003, 'National Council for Vocational Training (NCVT)', 'Govt Industrial Training Institute Mallepally', 'Industrial Electrician Trade',
    '84.0%', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 30, SYSDATE - 50, '2011', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2011', 'Suresh Babu Goud', 'Senior Technician', 'DKRM/MAINT',
    'DIPL', 'DIPLOMA', 'ELCL', 'Electrical & Electronics',
    2006, 'State Board of Technical Education and Training AP', 'Govt Polytechnic Masab Tank Hyderabad', 'Electrical Engineering Systems',
    '76.5%', 'First Class', 1, 'Upgraded from ITI', 'Submitted', SYSDATE - 30, SYSDATE - 50, '2011', 0
);

-- ------------------------------------------------------------------------
-- Employee 2012: Kavita Patel (MATERIALS LAB - Materials Scientist)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2012', 'Kavita Patel', 'Materials Scientist', 'MATERIALS LAB',
    '10TH', '10TH / MATRICULATION / SSLC', 'GEN', 'General (All Subjects)',
    2005, 'Gujarat Secondary and Higher Secondary Education Board', 'Navchetan High School Ahmedabad', 'General',
    '86.4%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 28, SYSDATE - 45, '2012', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2012', 'Kavita Patel', 'Materials Scientist', 'MATERIALS LAB',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2007, 'Gujarat Secondary Education Board', 'St. Xavier''s High School Ahmedabad', 'Science',
    '84.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 28, SYSDATE - 45, '2012', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2012', 'Kavita Patel', 'Materials Scientist', 'MATERIALS LAB',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'METAL', 'Metallurgy & Material Science',
    2011, 'Gujarat Technological University (GTU)', 'L.D. College of Engineering Ahmedabad', 'Metallurgical Engineering',
    '8.3 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 28, SYSDATE - 45, '2012', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2012', 'Kavita Patel', 'Materials Scientist', 'MATERIALS LAB',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'METAL', 'Metallurgy & Material Science',
    2013, 'Indian Institute of Technology (IIT) Bombay', 'IIT Bombay', 'Advanced Aerospace Nanomaterials',
    '9.1 CGPA', 'First Class with Distinction', 1, 'Gold medalist', 'Submitted', SYSDATE - 28, SYSDATE - 45, '2012', 0
);

-- ------------------------------------------------------------------------
-- Employee 2013: Harish Chandra Joshi (CYBER SECURITY - Cyber Security Analyst)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2013', 'Harish Chandra Joshi', 'Cyber Security Analyst', 'CYBER SECURITY',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2009, 'Central Board of Secondary Education', 'Birla Vidyamandir Nainital', 'General',
    '88.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 25, SYSDATE - 40, '2013', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2013', 'Harish Chandra Joshi', 'Cyber Security Analyst', 'CYBER SECURITY',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2011, 'Central Board of Secondary Education', 'Army Public School Dehradun', 'PCMC',
    '89.5%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 25, SYSDATE - 40, '2013', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2013', 'Harish Chandra Joshi', 'Cyber Security Analyst', 'CYBER SECURITY',
    'BCA', 'BACHELOR OF COMPUTER APPLICATIONS (B.C.A.)', 'CYBER', 'Cyber Security and Network Forensics',
    2014, 'Graphic Era Deemed to be University', 'Graphic Era Institute Dehradun', 'Computer Applications & Networking',
    '8.1 CGPA', 'First Class', 0, NULL, 'Submitted', SYSDATE - 25, SYSDATE - 40, '2013', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2013', 'Harish Chandra Joshi', 'Cyber Security Analyst', 'CYBER SECURITY',
    'MCA', 'MASTER OF COMPUTER APPLICATIONS (M.C.A.)', 'CYBER', 'Cyber Security and Network Forensics',
    2017, 'National Institute of Technology (NIT) Kurukshetra', 'NIT Kurukshetra', 'Information Forensics & Applied Cryptography',
    '8.8 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 25, SYSDATE - 40, '2013', 0
);

-- ------------------------------------------------------------------------
-- Employee 2014: Sneha Kulkarni (AVIONICS - Avionics Engineer) [Draft State]
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2014', 'Sneha Kulkarni', 'Avionics Engineer', 'AVIONICS',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2008, 'Karnataka Secondary Education Board', 'Sardar High School Belgaum', 'General',
    '91.5%', 'Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 18, '2014', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2014', 'Sneha Kulkarni', 'Avionics Engineer', 'AVIONICS',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2010, 'Karnataka Pre-University Board', 'RPD College of Science Belgaum', 'Science',
    '90.0%', 'Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 18, '2014', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2014', 'Sneha Kulkarni', 'Avionics Engineer', 'AVIONICS',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'AROSE', 'Aerospace Engineering',
    2014, 'Visvesvaraya Technological University (VTU)', 'M.S. Ramaiah Institute of Technology Bangalore', 'Flight Avionics & Guidance Systems',
    '8.9 CGPA', 'First Class with Distinction', 1, 'Draft pending certificate upload', 'Draft', NULL, SYSDATE - 18, '2014', 0
);

-- ------------------------------------------------------------------------
-- Employee 2015: Dr. Arvind Swaminathan (DKRM/ITISG - Chief Architect)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2015', 'Dr. Arvind Swaminathan', 'Chief Architect', 'DKRM/ITISG',
    '10TH', '10TH / MATRICULATION / SSLC', 'ICSE', 'ICSE Secondary Curriculum (Class X)',
    1999, 'Council for the Indian School Certificate Examinations', 'Don Bosco Matriculation Chennai', 'General',
    '91.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 85, SYSDATE - 135, '2015', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2015', 'Dr. Arvind Swaminathan', 'Chief Architect', 'DKRM/ITISG',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2001, 'Council for the Indian School Certificate Examinations', 'Don Bosco Chennai', 'PCMC',
    '93.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 85, SYSDATE - 135, '2015', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2015', 'Dr. Arvind Swaminathan', 'Chief Architect', 'DKRM/ITISG',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'COMPS', 'Computer Science & Engineering',
    2005, 'Indian Institute of Technology (IIT) Madras', 'IIT Madras', 'High Performance Distributed Computing',
    '9.3 CGPA', 'First Class with Distinction', 0, 'Institute Silver Medal', 'Submitted', SYSDATE - 85, SYSDATE - 135, '2015', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2015', 'Dr. Arvind Swaminathan', 'Chief Architect', 'DKRM/ITISG',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'COMPS', 'Computer Science & Engineering',
    2007, 'Indian Institute of Technology (IIT) Madras', 'IIT Madras', 'Fault-Tolerant Distributed Architectures',
    '9.6 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 85, SYSDATE - 135, '2015', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2015', 'Dr. Arvind Swaminathan', 'Chief Architect', 'DKRM/ITISG',
    'PHDD', 'PHD (DOCTORATE)', 'CS', 'Computer Science and Information Technology',
    2012, 'Indian Institute of Science (IISc)', 'Supercomputer Education & Research Centre IISc', 'Scalable Parallel Algorithms for Radar Simulations',
    'Awarded', 'Doctorate', 1, 'Doctoral Dissertation Awarded', 'Submitted', SYSDATE - 85, SYSDATE - 135, '2015', 0
);

-- ------------------------------------------------------------------------
-- Employee 2016: Bhavna Chawla (D-ADMIN/STORE - Operations Manager)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2016', 'Bhavna Chawla', 'Operations Manager', 'D-ADMIN/STORE',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2004, 'Central Board of Secondary Education', 'Convent of Jesus and Mary Ambala', 'General',
    '84.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 92, SYSDATE - 125, '2016', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2016', 'Bhavna Chawla', 'Operations Manager', 'D-ADMIN/STORE',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'COMM', 'Commerce',
    2006, 'Central Board of Secondary Education', 'DAV Public School Chandigarh', 'Commerce',
    '87.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 92, SYSDATE - 125, '2016', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2016', 'Bhavna Chawla', 'Operations Manager', 'D-ADMIN/STORE',
    'BCOM', 'BACHELOR OF COMMERCE (B.COM.)', 'ACC', 'Accounting & Finance',
    2009, 'Panjab University Chandigarh', 'MCM DAV College for Women Chandigarh', 'Financial Accounting & Costing',
    '76.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 92, SYSDATE - 125, '2016', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2016', 'Bhavna Chawla', 'Operations Manager', 'D-ADMIN/STORE',
    'MBA', 'MASTER OF BUSINESS ADMINISTRATION (M.B.A.)', 'OPS', 'Operations and Supply Chain Management',
    2012, 'Symbiosis International University', 'SIBM Pune', 'Supply Chain Operations & Inventory Control',
    '8.4 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 92, SYSDATE - 125, '2016', 0
);

-- ------------------------------------------------------------------------
-- Employee 2017: Gopal Krishna Bhat (ELECTRICAL SYSTEMS - Instrumentation Specialist)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2017', 'Gopal Krishna Bhat', 'Instrumentation Specialist', 'ELECTRICAL SYSTEMS',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2003, 'Karnataka Secondary Education Board', 'Canara High School Mangalore', 'General',
    '88.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 65, SYSDATE - 98, '2017', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2017', 'Gopal Krishna Bhat', 'Instrumentation Specialist', 'ELECTRICAL SYSTEMS',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2005, 'Department of Pre-University Education Karnataka', 'St. Aloysius PU College Mangalore', 'PCMB Science',
    '85.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 65, SYSDATE - 98, '2017', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2017', 'Gopal Krishna Bhat', 'Instrumentation Specialist', 'ELECTRICAL SYSTEMS',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'INSTR', 'Instrumentation Engineering',
    2009, 'Visvesvaraya Technological University (VTU)', 'NMAM Institute of Technology Nitte', 'Instrumentation & Control',
    '8.2 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 65, SYSDATE - 98, '2017', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2017', 'Gopal Krishna Bhat', 'Instrumentation Specialist', 'ELECTRICAL SYSTEMS',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'INSTR', 'Instrumentation Engineering',
    2012, 'National Institute of Technology Karnataka (NITK) Surathkal', 'NITK Surathkal', 'Industrial Process Control & Automation',
    '8.8 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 65, SYSDATE - 98, '2017', 0
);

-- ------------------------------------------------------------------------
-- Employee 2018: Pooja Agarwal (FINANCE - Finance Officer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2018', 'Pooja Agarwal', 'Finance Officer', 'FINANCE',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2007, 'Central Board of Secondary Education', 'Ryan International School Noida', 'General',
    '91.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 58, SYSDATE - 88, '2018', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2018', 'Pooja Agarwal', 'Finance Officer', 'FINANCE',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'COMM_M', 'Commerce with Mathematics',
    2009, 'Central Board of Secondary Education', 'DPS Noida', 'Commerce with Mathematics',
    '94.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 58, SYSDATE - 88, '2018', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2018', 'Pooja Agarwal', 'Finance Officer', 'FINANCE',
    'BCOM', 'BACHELOR OF COMMERCE (B.COM.)', 'ACC', 'Accounting & Finance',
    2012, 'University of Delhi', 'Shri Ram College of Commerce (SRCC) Delhi', 'B.Com Honours in Financial Accounting',
    '85.0%', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 58, SYSDATE - 88, '2018', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2018', 'Pooja Agarwal', 'Finance Officer', 'FINANCE',
    'MBA', 'MASTER OF BUSINESS ADMINISTRATION (M.B.A.)', 'FIN', 'Financial Management',
    2014, 'University of Delhi', 'Faculty of Management Studies (FMS) Delhi', 'Corporate Finance & Investment Banking',
    '8.7 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 58, SYSDATE - 88, '2018', 0
);

-- ------------------------------------------------------------------------
-- Employee 2019: Rohan Deshpande (FLIGHT TESTING - Aero-Structural Engineer) [Draft]
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2019', 'Rohan Deshpande', 'Aero-Structural Engineer', 'FLIGHT TESTING',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2006, 'Maharashtra State Board', 'Saraswati Vidyalaya Nagpur', 'General',
    '89.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 52, SYSDATE - 82, '2019', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2019', 'Rohan Deshpande', 'Aero-Structural Engineer', 'FLIGHT TESTING',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2008, 'Maharashtra State Board', 'Shivaji Science College Nagpur', 'PCMB Science',
    '87.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 52, SYSDATE - 82, '2019', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2019', 'Rohan Deshpande', 'Aero-Structural Engineer', 'FLIGHT TESTING',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'AERO', 'Aeronautical Engineering',
    2012, 'Visvesvaraya National Institute of Technology (VNIT) Nagpur', 'VNIT Nagpur', 'Aero-Structural Dynamics',
    '8.5 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 52, SYSDATE - 82, '2019', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2019', 'Rohan Deshpande', 'Aero-Structural Engineer', 'FLIGHT TESTING',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'AERO', 'Aeronautical Engineering',
    2014, 'Indian Institute of Technology (IIT) Bombay', 'IIT Bombay', 'Aeroelastic Flight Flutter Analysis',
    '9.2 CGPA', 'First Class with Distinction', 1, 'Draft pending HOD recommendation', 'Draft', NULL, SYSDATE - 20, '2019', 0
);

-- ------------------------------------------------------------------------
-- Employee 2020: Tanvi Saxena (R&D/EMBEDDED - Hardware Verification Engineer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2020', 'Tanvi Saxena', 'Hardware Verification Engineer', 'R&D/EMBEDDED',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2011, 'Central Board of Secondary Education', 'Delhi Public School Lucknow', 'General',
    '92.5%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 48, SYSDATE - 70, '2020', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2020', 'Tanvi Saxena', 'Hardware Verification Engineer', 'R&D/EMBEDDED',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2013, 'Central Board of Secondary Education', 'City Montessori School Lucknow', 'PCMC',
    '90.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 48, SYSDATE - 70, '2020', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2020', 'Tanvi Saxena', 'Hardware Verification Engineer', 'R&D/EMBEDDED',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'E&C', 'Electronics & Communication',
    2017, 'Dr. A.P.J. Abdul Kalam Technical University (AKTU)', 'Institute of Engineering and Technology (IET) Lucknow', 'VLSI Design & SystemVerilog',
    '8.7 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 48, SYSDATE - 70, '2020', 0
);

-- ------------------------------------------------------------------------
-- Employee 2021: Manoj Kumar Tiwari (SYSTEM SAFETY - Safety Assurance Specialist)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2021', 'Manoj Kumar Tiwari', 'Safety Assurance Specialist', 'SYSTEM SAFETY',
    '10TH', '10TH / MATRICULATION / SSLC', 'GEN', 'General (All Subjects)',
    2000, 'Board of High School and Intermediate Education UP', 'Govt Inter College Varanasi', 'General High School',
    '76.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 75, SYSDATE - 105, '2021', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2021', 'Manoj Kumar Tiwari', 'Safety Assurance Specialist', 'SYSTEM SAFETY',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2002, 'Board of High School and Intermediate Education UP', 'Queens Inter College Varanasi', 'Science',
    '79.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 75, SYSDATE - 105, '2021', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2021', 'Manoj Kumar Tiwari', 'Safety Assurance Specialist', 'SYSTEM SAFETY',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'INDST', 'Industrial Engineering',
    2006, 'Dr. B.R. Ambedkar National Institute of Technology Jalandhar', 'NIT Jalandhar', 'Industrial Safety & Reliability Engineering',
    '7.9 CGPA', 'First Class', 0, NULL, 'Submitted', SYSDATE - 75, SYSDATE - 105, '2021', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2021', 'Manoj Kumar Tiwari', 'Safety Assurance Specialist', 'SYSTEM SAFETY',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'INDST', 'Industrial Engineering',
    2009, 'Indian Institute of Technology (IIT) Roorkee', 'IIT Roorkee', 'Safety Risk Assessment & Fault Tree Modeling',
    '8.6 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 75, SYSDATE - 105, '2021', 0
);

-- ------------------------------------------------------------------------
-- Employee 2022: Shalini Nambiar (SIGNAL PROCESSING - Communications Lead)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2022', 'Shalini Nambiar', 'Communications Lead', 'SIGNAL PROCESSING',
    '10TH', '10TH / MATRICULATION / SSLC', 'ICSE', 'ICSE Secondary Curriculum (Class X)',
    2005, 'Council for the Indian School Certificate Examinations', 'Chinmaya Vidyalaya Kozhikode', 'General',
    '94.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 62, SYSDATE - 92, '2022', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2022', 'Shalini Nambiar', 'Communications Lead', 'SIGNAL PROCESSING',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2007, 'Council for the Indian School Certificate Examinations', 'Chinmaya Vidyalaya Kozhikode', 'PCMC',
    '92.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 62, SYSDATE - 92, '2022', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2022', 'Shalini Nambiar', 'Communications Lead', 'SIGNAL PROCESSING',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'ELTE', 'Electronics & Telecommunication',
    2011, 'National Institute of Technology (NIT) Calicut', 'NIT Calicut', 'Digital & Optical Communications',
    '8.8 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 62, SYSDATE - 92, '2022', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2022', 'Shalini Nambiar', 'Communications Lead', 'SIGNAL PROCESSING',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'RACE', 'Radar & Communication Engineering',
    2014, 'Indian Institute of Technology (IIT) Madras', 'IIT Madras', 'Wideband Satellite & Telemetry Systems',
    '9.3 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 62, SYSDATE - 92, '2022', 0
);

-- ------------------------------------------------------------------------
-- Employee 2023: Dinesh Karthik (DKRM/MAINT - Maintenance Supervisor)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2023', 'Dinesh Karthik', 'Maintenance Supervisor', 'DKRM/MAINT',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2004, 'Tamil Nadu State Board', 'Corporation High School Madurai', 'General',
    '75.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 42, SYSDATE - 65, '2023', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2023', 'Dinesh Karthik', 'Maintenance Supervisor', 'DKRM/MAINT',
    'ITI', 'INDUSTRIAL TRAINING INSTITUTE (ITI)', 'TOOL_DIE', 'Tool and Die Maker (Press Tools, Jigs & Fixtures)',
    2006, 'State Council for Technical Education & Vocational Training', 'Govt ITI Madurai', 'Tool & Die Making Trade',
    '86.0%', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 42, SYSDATE - 65, '2023', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2023', 'Dinesh Karthik', 'Maintenance Supervisor', 'DKRM/MAINT',
    'DIPL', 'DIPLOMA', 'DMECH', 'Mechanical Engineering',
    2009, 'State Board of Technical Education Tamil Nadu', 'Tamil Nadu Polytechnic College Madurai', 'Mechanical Precision Maintenance',
    '81.0%', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 42, SYSDATE - 65, '2023', 0
);

-- ------------------------------------------------------------------------
-- Employee 2024: Aishwarya Srinivasan (DKRM/ITISG - Software Quality Specialist) [Draft]
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2024', 'Aishwarya Srinivasan', 'Software Quality Specialist', 'DKRM/ITISG',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2012, 'Central Board of Secondary Education', 'DAV Senior Secondary School Chennai', 'General',
    '95.0%', 'Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 12, '2024', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2024', 'Aishwarya Srinivasan', 'Software Quality Specialist', 'DKRM/ITISG',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2014, 'Central Board of Secondary Education', 'DAV Boys Senior Secondary Chennai', 'PCMC',
    '93.5%', 'Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 12, '2024', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2024', 'Aishwarya Srinivasan', 'Software Quality Specialist', 'DKRM/ITISG',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'COMPS', 'Computer Science & Engineering',
    2018, 'Anna University', 'SSN College of Engineering Chennai', 'Automated Software Testing & Security',
    '8.9 CGPA', 'First Class with Distinction', 1, 'Draft pending manager review', 'Draft', NULL, SYSDATE - 12, '2024', 0
);

-- ------------------------------------------------------------------------
-- Employee 2025: Naveen Kumar Shetty (RADAR SYSTEMS - Radar Calibration Engineer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2025', 'Naveen Kumar Shetty', 'Radar Calibration Engineer', 'RADAR SYSTEMS',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2006, 'Karnataka Secondary Education Board', 'Poornaprajna High School Udupi', 'General',
    '88.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 40, SYSDATE - 70, '2025', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2025', 'Naveen Kumar Shetty', 'Radar Calibration Engineer', 'RADAR SYSTEMS',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCME', 'Science: Physics, Chemistry, Mathematics, Electronics (PCME)',
    2008, 'Department of Pre-University Education Karnataka', 'Bhandarkars PU College Kundapura', 'PCME',
    '86.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 40, SYSDATE - 70, '2025', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2025', 'Naveen Kumar Shetty', 'Radar Calibration Engineer', 'RADAR SYSTEMS',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'E&C', 'Electronics & Communication',
    2012, 'Visvesvaraya Technological University (VTU)', 'Manipal Institute of Technology (MIT) Manipal', 'RF & Microwave Systems',
    '8.4 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 40, SYSDATE - 70, '2025', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2025', 'Naveen Kumar Shetty', 'Radar Calibration Engineer', 'RADAR SYSTEMS',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'RACE', 'Radar & Communication Engineering',
    2015, 'Defence Institute of Advanced Technology (DIAT) Pune', 'DIAT Deemed University Pune', 'Radar Transmitter & Array Calibration',
    '9.0 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 40, SYSDATE - 70, '2025', 0
);

-- ------------------------------------------------------------------------
-- Employee 2026: Ritu Ganguly (MATERIALS LAB - Assistant Technical Manager)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2026', 'Ritu Ganguly', 'Assistant Technical Manager', 'MATERIALS LAB',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2007, 'Central Board of Secondary Education', 'Salt Lake School Kolkata', 'General',
    '90.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 32, SYSDATE - 60, '2026', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2026', 'Ritu Ganguly', 'Assistant Technical Manager', 'MATERIALS LAB',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2009, 'Central Board of Secondary Education', 'Salt Lake Point School Kolkata', 'Science',
    '89.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 32, SYSDATE - 60, '2026', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2026', 'Ritu Ganguly', 'Assistant Technical Manager', 'MATERIALS LAB',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'CHEMI', 'Chemical Engineering',
    2013, 'Jadavpur University Kolkata', 'Jadavpur University Faculty of Engineering', 'Polymer & Chemical Engineering',
    '8.6 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 32, SYSDATE - 60, '2026', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2026', 'Ritu Ganguly', 'Assistant Technical Manager', 'MATERIALS LAB',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'CHEMI', 'Chemical Engineering',
    2015, 'Indian Institute of Technology (IIT) Kharagpur', 'IIT Kharagpur', 'Composite Materials & Advanced Polymers',
    '9.1 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 32, SYSDATE - 60, '2026', 0
);

-- ------------------------------------------------------------------------
-- Employee 2027: Pradeep Chandran (ELECTRICAL SYSTEMS - Electrical Substation Engineer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2027', 'Pradeep Chandran', 'Electrical Substation Engineer', 'ELECTRICAL SYSTEMS',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2002, 'Kerala State Board', 'Model Boys High School Trivandrum', 'General',
    '82.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 50, SYSDATE - 80, '2027', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2027', 'Pradeep Chandran', 'Electrical Substation Engineer', 'ELECTRICAL SYSTEMS',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2004, 'Directorate of Higher Secondary Education Kerala', 'SMV Higher Secondary School Trivandrum', 'Science',
    '80.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 50, SYSDATE - 80, '2027', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2027', 'Pradeep Chandran', 'Electrical Substation Engineer', 'ELECTRICAL SYSTEMS',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'ELCL', 'Electrical & Electronics',
    2008, 'University of Kerala', 'College of Engineering Trivandrum (CET)', 'Power Systems & Substation Design',
    '7.9 CGPA', 'First Class', 0, NULL, 'Submitted', SYSDATE - 50, SYSDATE - 80, '2027', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2027', 'Pradeep Chandran', 'Electrical Substation Engineer', 'ELECTRICAL SYSTEMS',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'ELCL', 'Electrical & Electronics',
    2011, 'National Institute of Technology (NIT) Tiruchirappalli', 'NIT Trichy', 'High Voltage Engineering & Protection',
    '8.7 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 50, SYSDATE - 80, '2027', 0
);

-- ------------------------------------------------------------------------
-- Employee 2028: Geetha Balasubramanian (D-ADMIN/STORE - Stores Executive)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2028', 'Geetha Balasubramanian', 'Stores Executive', 'D-ADMIN/STORE',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2008, 'Tamil Nadu State Board', 'Vidyodaya Girls High School Chennai', 'General',
    '87.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 22, SYSDATE - 50, '2028', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2028', 'Geetha Balasubramanian', 'Stores Executive', 'D-ADMIN/STORE',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'COMM', 'Commerce',
    2010, 'Tamil Nadu State Board', 'Vidyodaya Chennai', 'Commerce Stream',
    '89.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 22, SYSDATE - 50, '2028', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2028', 'Geetha Balasubramanian', 'Stores Executive', 'D-ADMIN/STORE',
    'BBA', 'BACHELOR OF BUSINESS ADMINISTRATION (B.B.A.)', 'MANG', 'Business Administration',
    2013, 'University of Madras', 'Ethiraj College for Women Chennai', 'Inventory & Logistics Management',
    '81.0%', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 22, SYSDATE - 50, '2028', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2028', 'Geetha Balasubramanian', 'Stores Executive', 'D-ADMIN/STORE',
    'MBA', 'MASTER OF BUSINESS ADMINISTRATION (M.B.A.)', 'OPS', 'Operations and Supply Chain Management',
    2015, 'Anna University', 'Department of Management Studies (DOMS) Chennai', 'Supply Chain & ERP Operations',
    '8.4 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 22, SYSDATE - 50, '2028', 0
);

-- ------------------------------------------------------------------------
-- Employee 2029: Sachin Tendulkar Rao (FLIGHT TESTING - Telemetry Engineer) [Draft]
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2029', 'Sachin Tendulkar Rao', 'Telemetry Engineer', 'FLIGHT TESTING',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2008, 'Central Board of Secondary Education', 'National Public School Rajajinagar Bangalore', 'General',
    '89.0%', 'Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 10, '2029', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2029', 'Sachin Tendulkar Rao', 'Telemetry Engineer', 'FLIGHT TESTING',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2010, 'Central Board of Secondary Education', 'National Public School Bangalore', 'PCMC',
    '88.0%', 'First Class', 0, NULL, 'Draft', NULL, SYSDATE - 10, '2029', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2029', 'Sachin Tendulkar Rao', 'Telemetry Engineer', 'FLIGHT TESTING',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'E&C', 'Electronics & Communication',
    2014, 'Visvesvaraya Technological University (VTU)', 'PES Institute of Technology Bangalore', 'Wireless Telemetry Systems',
    '8.3 CGPA', 'First Class with Distinction', 1, 'Draft state', 'Draft', NULL, SYSDATE - 10, '2029', 0
);

-- ------------------------------------------------------------------------
-- Employee 2030: Dr. Alka Saxena (RADAR SYSTEMS - Research Scientist)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2030', 'Dr. Alka Saxena', 'Research Scientist', 'RADAR SYSTEMS',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2001, 'Central Board of Secondary Education', 'St. Mary''s Convent Inter College Allahabad', 'General',
    '94.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 95, SYSDATE - 140, '2030', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2030', 'Dr. Alka Saxena', 'Research Scientist', 'RADAR SYSTEMS',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2003, 'Central Board of Secondary Education', 'St. Mary''s Convent Allahabad', 'Science',
    '93.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 95, SYSDATE - 140, '2030', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2030', 'Dr. Alka Saxena', 'Research Scientist', 'RADAR SYSTEMS',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'E&C', 'Electronics & Communication',
    2007, 'Motilal Nehru National Institute of Technology (MNNIT) Allahabad', 'MNNIT Allahabad', 'Phased Array Antenna Engineering',
    '9.1 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 95, SYSDATE - 140, '2030', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2030', 'Dr. Alka Saxena', 'Research Scientist', 'RADAR SYSTEMS',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'RACE', 'Radar & Communication Engineering',
    2009, 'Indian Institute of Technology (IIT) Roorkee', 'IIT Roorkee', 'Active Electronically Scanned Array (AESA)',
    '9.4 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 95, SYSDATE - 140, '2030', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2030', 'Dr. Alka Saxena', 'Research Scientist', 'RADAR SYSTEMS',
    'PHDD', 'PHD (DOCTORATE)', 'ENG', 'Engineering and Technology',
    2015, 'Indian Institute of Science (IISc)', 'Department of Electrical Communication Engineering IISc', 'Multifunction Airborne Phased Array Radar Architecture',
    'Awarded', 'Doctorate', 1, 'National radar patent holder', 'Submitted', SYSDATE - 95, SYSDATE - 140, '2030', 0
);

-- ------------------------------------------------------------------------
-- Employee 2031: Deepak Mohan Lal (DKRM/ITISG - Database Administrator)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2031', 'Deepak Mohan Lal', 'Database Administrator', 'DKRM/ITISG',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2009, 'Central Board of Secondary Education', 'St. John''s High School Chandigarh', 'General',
    '86.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 15, SYSDATE - 35, '2031', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2031', 'Deepak Mohan Lal', 'Database Administrator', 'DKRM/ITISG',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMC', 'Physics, Chemistry, Mathematics, Computer Science (PCMC)',
    2011, 'Central Board of Secondary Education', 'DAV Senior Secondary School Chandigarh', 'PCMC',
    '88.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 15, SYSDATE - 35, '2031', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2031', 'Deepak Mohan Lal', 'Database Administrator', 'DKRM/ITISG',
    'BCA', 'BACHELOR OF COMPUTER APPLICATIONS (B.C.A.)', 'DBA', 'Database Systems and Data Warehousing',
    2014, 'Panjab University Chandigarh', 'DAV College Sector 10 Chandigarh', 'Relational Database Architecture',
    '8.2 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 15, SYSDATE - 35, '2031', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2031', 'Deepak Mohan Lal', 'Database Administrator', 'DKRM/ITISG',
    'MCA', 'MASTER OF COMPUTER APPLICATIONS (M.C.A.)', 'DBA', 'Database Systems and Data Warehousing',
    2017, 'National Institute of Technology (NIT) Jalandhar', 'NIT Jalandhar', 'Oracle Enterprise Cluster & High Availability',
    '8.7 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 15, SYSDATE - 35, '2031', 0
);

-- ------------------------------------------------------------------------
-- Employee 2032: Sunil Kumar Yadav (DKRM/MAINT - Machinist Specialist) [Draft]
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2032', 'Sunil Kumar Yadav', 'Machinist Specialist', 'DKRM/MAINT',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2007, 'Board of School Education Haryana', 'Govt Senior Secondary School Gurgaon', 'General',
    '74.0%', 'First Class', 0, NULL, 'Draft', NULL, SYSDATE - 8, '2032', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2032', 'Sunil Kumar Yadav', 'Machinist Specialist', 'DKRM/MAINT',
    'ITI', 'INDUSTRIAL TRAINING INSTITUTE (ITI)', 'CNC', 'CNC Operator cum Programmer (Milling & Turning)',
    2009, 'National Council for Vocational Training (NCVT)', 'Govt ITI Gurgaon', 'CNC Milling & Precision Turning',
    '85.0%', 'First Class with Distinction', 0, NULL, 'Draft', NULL, SYSDATE - 8, '2032', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2032', 'Sunil Kumar Yadav', 'Machinist Specialist', 'DKRM/MAINT',
    'DIPL', 'DIPLOMA', 'DMECH', 'Mechanical Engineering',
    2012, 'Haryana State Board of Technical Education', 'Govt Polytechnic Manesar', 'Mechanical Engineering & Tooling',
    '78.0%', 'First Class', 1, 'Draft diploma submission', 'Draft', NULL, SYSDATE - 8, '2032', 0
);

-- ------------------------------------------------------------------------
-- Employee 2033: Revathi Krishnan (SYSTEM SAFETY - Safety Compliance Officer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2033', 'Revathi Krishnan', 'Safety Compliance Officer', 'SYSTEM SAFETY',
    '10TH', '10TH / MATRICULATION / SSLC', 'ICSE', 'ICSE Secondary Curriculum (Class X)',
    2010, 'Council for the Indian School Certificate Examinations', 'Sishya School Adyar Chennai', 'General',
    '91.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 14, SYSDATE - 30, '2033', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2033', 'Revathi Krishnan', 'Safety Compliance Officer', 'SYSTEM SAFETY',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2012, 'Council for the Indian School Certificate Examinations', 'Sishya Chennai', 'Science Stream',
    '89.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 14, SYSDATE - 30, '2033', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2033', 'Revathi Krishnan', 'Safety Compliance Officer', 'SYSTEM SAFETY',
    'BTECH', 'BACHELOR OF TECHNOLOGY (B.TECH)', 'CHEMI', 'Chemical Engineering',
    2016, 'Anna University', 'Alagappa Chettiar College of Technology (ACTech) Chennai', 'Hazardous Materials & Process Safety',
    '8.3 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 14, SYSDATE - 30, '2033', 0
);

-- ------------------------------------------------------------------------
-- Employee 2034: Tarun Pratap Singh (FLIGHT TESTING - Avionics Test Pilot Engineer)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2034', 'Tarun Pratap Singh', 'Avionics Test Pilot Engineer', 'FLIGHT TESTING',
    '10TH', '10TH / MATRICULATION / SSLC', 'CBSE', 'CBSE Secondary Curriculum (Class X)',
    2004, 'Central Board of Secondary Education', 'Army Public School Pune', 'General',
    '88.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 20, SYSDATE - 45, '2034', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2034', 'Tarun Pratap Singh', 'Avionics Test Pilot Engineer', 'FLIGHT TESTING',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2006, 'Central Board of Secondary Education', 'Army Public School Kirkee Pune', 'PCMB',
    '87.0%', 'First Class', 0, NULL, 'Submitted', SYSDATE - 20, SYSDATE - 45, '2034', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2034', 'Tarun Pratap Singh', 'Avionics Test Pilot Engineer', 'FLIGHT TESTING',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'AERO', 'Aeronautical Engineering',
    2010, 'Savitribai Phule Pune University', 'Maharashtra Institute of Technology (MIT) Pune', 'Aerospace Flight Dynamics & Testing',
    '8.2 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 20, SYSDATE - 45, '2034', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2034', 'Tarun Pratap Singh', 'Avionics Test Pilot Engineer', 'FLIGHT TESTING',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'AROSE', 'Aerospace Engineering',
    2013, 'Indian Institute of Space Science and Technology (IIST)', 'IIST Trivandrum', 'Control Systems & Flight Telemetry',
    '8.9 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 20, SYSDATE - 45, '2034', 0
);

-- ------------------------------------------------------------------------
-- Employee 2035: Vandana Hegde (QUALITY ASSURANCE - Senior Quality Auditor)
-- ------------------------------------------------------------------------
INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2035', 'Vandana Hegde', 'Senior Quality Auditor', 'QUALITY ASSURANCE',
    '10TH', '10TH / MATRICULATION / SSLC', 'SSLC', 'State Board / SSLC Curriculum',
    2008, 'Karnataka Secondary Education Board', 'Mahila Seva Samaja Bangalore', 'General',
    '93.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 10, SYSDATE - 25, '2035', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2035', 'Vandana Hegde', 'Senior Quality Auditor', 'QUALITY ASSURANCE',
    '12TH', '12TH / HIGHER SECONDARY (10+2)', 'PCMB', 'Physics, Chemistry, Mathematics, Biology (PCMB)',
    2010, 'Department of Pre-University Education Karnataka', 'MES PU College Malleswaram Bangalore', 'Science',
    '91.0%', 'Distinction', 0, NULL, 'Submitted', SYSDATE - 10, SYSDATE - 25, '2035', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2035', 'Vandana Hegde', 'Senior Quality Auditor', 'QUALITY ASSURANCE',
    'BE', 'BACHELOR OF ENGINEERING (B.E.)', 'INDST', 'Industrial Engineering',
    2014, 'Visvesvaraya Technological University (VTU)', 'BMS College of Engineering Bangalore', 'Quality Assurance & Six Sigma',
    '8.7 CGPA', 'First Class with Distinction', 0, NULL, 'Submitted', SYSDATE - 10, SYSDATE - 25, '2035', 0
);

INSERT INTO EDU_EMP_EDUCATION (
    PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
    QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
    YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
    CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS, SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED
) VALUES (
    '2035', 'Vandana Hegde', 'Senior Quality Auditor', 'QUALITY ASSURANCE',
    'MTECH', 'MASTER OF TECHNOLOGY (M.TECH)', 'INDST', 'Industrial Engineering',
    2016, 'National Institute of Industrial Engineering (NITIE / IIM Mumbai)', 'NITIE Mumbai', 'Reliability Engineering & Quality Systems',
    '9.2 CGPA', 'First Class with Distinction', 1, NULL, 'Submitted', SYSDATE - 10, SYSDATE - 25, '2035', 0
);

COMMIT;
