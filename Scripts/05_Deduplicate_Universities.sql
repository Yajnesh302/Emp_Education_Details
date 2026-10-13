-- ========================================================================
-- APPLICATION: EMPLOYEE EDUCATION MANAGEMENT SYSTEM
-- TARGET DATABASE: ORACLE 11g R2 / 12c+ / 19c+
-- SCRIPT: 05_Deduplicate_Universities.sql
-- DESCRIPTION: Deduplicates university and institution catalog in EDU_UNIVERSITY
--              and harmonizes existing employee records in EDU_EMP_EDUCATION.
--              Specifically consolidates abbreviation/full-name duplicates
--              such as VTU, JNTU, IGNOU, CBSE, ICSE, IITs, NITs, etc.
-- ========================================================================

SET DEFINE OFF;

PROMPT [INFO] Starting university deduplication and standardization...

-- ------------------------------------------------------------------------
-- STEP 1: Standardize Canonical Master Records in EDU_UNIVERSITY
-- ------------------------------------------------------------------------

-- VTU: Ensure canonical ID 129 has the full, official standardized title
UPDATE EDU_UNIVERSITY
SET UNIV_NAME = 'Visvesvaraya Technological University (VTU), Belagavi',
    STATE_OR_TYPE = 'State University',
    IS_ACTIVE = 1
WHERE UNIV_ID = 129;

-- Premier Institutes and Universities Canonical Standardization
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Anna University, Chennai' WHERE UNIV_ID = 1;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Bangalore University, Bengaluru' WHERE UNIV_ID = 2;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Andhra University, Visakhapatnam' WHERE UNIV_ID = 3;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Kanpur' WHERE UNIV_ID = 4;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Madras' WHERE UNIV_ID = 5;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Delhi' WHERE UNIV_ID = 6;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Science (IISc), Bangalore' WHERE UNIV_ID = 7;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Guwahati' WHERE UNIV_ID = 9;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Roorkee' WHERE UNIV_ID = 10;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Banaras Hindu University (BHU), Varanasi' WHERE UNIV_ID = 11;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Birla Institute of Technology and Science (BITS), Pilani' WHERE UNIV_ID = 12;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Kharagpur' WHERE UNIV_ID = 13;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT / REC), Warangal' WHERE UNIV_ID = 14;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT / REC), Tiruchirappalli' WHERE UNIV_ID = 15;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology Karnataka (NITK / REC), Surathkal' WHERE UNIV_ID = 16;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT / REC), Calicut' WHERE UNIV_ID = 17;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT / REC), Silchar' WHERE UNIV_ID = 18;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Malaviya National Institute of Technology (MNIT), Jaipur' WHERE UNIV_ID = 19;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Karnatak University, Dharwad' WHERE UNIV_ID = 20;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'University of Mysore, Mysuru' WHERE UNIV_ID = 21;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Mangalore University, Konaje' WHERE UNIV_ID = 22;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Kuvempu University, Shankaraghatta, Shivamogga' WHERE UNIV_ID = 23;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Jawaharlal Nehru Technological University Kakinada (JNTUK)' WHERE UNIV_ID = 25;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Jawaharlal Nehru Technological University Anantapur (JNTUA)' WHERE UNIV_ID = 26;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Acharya Nagarjuna University, Guntur' WHERE UNIV_ID = 27;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Osmania University, Hyderabad' WHERE UNIV_ID = 28;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'University of Hyderabad (UoH / Central University), Hyderabad' WHERE UNIV_ID = 30;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'University of Madras, Chennai' WHERE UNIV_ID = 33;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Cochin University of Science and Technology (CUSAT), Kochi' WHERE UNIV_ID = 38;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Maulana Azad National Institute of Technology (MANIT), Bhopal' WHERE UNIV_ID = 40;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Madurai Kamaraj University (MKU), Madurai' WHERE UNIV_ID = 43;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Jawaharlal Nehru University (JNU), New Delhi' WHERE UNIV_ID = 44;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'University of Delhi (DU), New Delhi' WHERE UNIV_ID = 45;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'University of Kerala, Thiruvananthapuram' WHERE UNIV_ID = 48;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT / REC), Hamirpur' WHERE UNIV_ID = 49;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Mahatma Gandhi University, Kerala' WHERE UNIV_ID = 50;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Sardar Vallabhbhai National Institute of Technology (SVNIT), Surat' WHERE UNIV_ID = 51;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Rajiv Gandhi Proudyogiki Vishwavidyalaya (RGPV), Bhopal' WHERE UNIV_ID = 55;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Bombay' WHERE UNIV_ID = 56;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Utkal University, Bhubaneswar' WHERE UNIV_ID = 61;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'University of Calicut, Malappuram' WHERE UNIV_ID = 65;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Motilal Nehru National Institute of Technology (MNNIT), Allahabad' WHERE UNIV_ID = 68;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Savitribai Phule Pune University (SPPU / University of Pune)' WHERE UNIV_ID = 71;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'University of Calcutta (Calcutta University), Kolkata' WHERE UNIV_ID = 73;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Deen Dayal Upadhyaya Gorakhpur University (DDU), Gorakhpur' WHERE UNIV_ID = 74;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'University of Lucknow, Lucknow' WHERE UNIV_ID = 78;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Aligarh Muslim University (AMU), Aligarh' WHERE UNIV_ID = 84;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT / REC), Rourkela' WHERE UNIV_ID = 86;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indira Gandhi National Open University (IGNOU), New Delhi' WHERE UNIV_ID = 89;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT / REC), Durgapur' WHERE UNIV_ID = 95;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Mahatma Jyotiba Phule Rohilkhand University (MJPRU), Bareilly' WHERE UNIV_ID = 96;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Dr. B. R. Ambedkar National Institute of Technology (NIT), Jalandhar' WHERE UNIV_ID = 102;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Jamia Millia Islamia (JMI), New Delhi' WHERE UNIV_ID = 103;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Panjab University, Chandigarh' WHERE UNIV_ID = 109;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'North-Eastern Hill University (NEHU), Shillong' WHERE UNIV_ID = 119;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Patna University, Patna' WHERE UNIV_ID = 125;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT), Jamshedpur' WHERE UNIV_ID = 140;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT), Kurukshetra' WHERE UNIV_ID = 146;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'ABV-Indian Institute of Information Technology and Management (ABV-IIITM), Gwalior' WHERE UNIV_ID = 151;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT), Raipur' WHERE UNIV_ID = 156;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT), Patna' WHERE UNIV_ID = 158;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Information Technology (IIIT), Allahabad' WHERE UNIV_ID = 163;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru' WHERE UNIV_ID = 166;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Department of Pre-University Education (PUE), Karnataka' WHERE UNIV_ID = 167;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'West Bengal Board of Secondary Education (WBBSE)' WHERE UNIV_ID = 172;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'West Bengal Council of Higher Secondary Education (WBCHSE)' WHERE UNIV_ID = 173;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'State Board of Technical Education and Training (SBTET), Tamil Nadu' WHERE UNIV_ID = 181;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Bihar School Examination Board (BSEB), Patna' WHERE UNIV_ID = 204;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'West Bengal State Council of Technical and Vocational Education (WBSCTE), Kolkata' WHERE UNIV_ID = 219;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Central Board of Secondary Education (CBSE), New Delhi' WHERE UNIV_ID = 221;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Gandhinagar' WHERE UNIV_ID = 223;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Patna' WHERE UNIV_ID = 226;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Jawaharlal Nehru Technological University (JNTU), Hyderabad' WHERE UNIV_ID = 264;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Council for Vocational Training (NCVT)' WHERE UNIV_ID = 274;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Jodhpur' WHERE UNIV_ID = 275;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Management (IIM), Lucknow' WHERE UNIV_ID = 278;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Central University of Rajasthan, Ajmer' WHERE UNIV_ID = 281;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT), Agartala' WHERE UNIV_ID = 283;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow' WHERE UNIV_ID = 284;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Indore' WHERE UNIV_ID = 289;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT), Delhi' WHERE UNIV_ID = 290;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)' WHERE UNIV_ID = 297;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'National Institute of Technology (NIT), Srinagar' WHERE UNIV_ID = 298;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Board of Secondary Education, Assam (SEBA)' WHERE UNIV_ID = 304;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Hyderabad' WHERE UNIV_ID = 307;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Mandi' WHERE UNIV_ID = 310;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (BHU), Varanasi' WHERE UNIV_ID = 323;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Ropar' WHERE UNIV_ID = 325;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Indian Institute of Technology (IIT), Bhubaneswar' WHERE UNIV_ID = 338;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)' WHERE UNIV_ID = 340;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Uttarakhand Board of Technical Education, Roorkee (UBTER)' WHERE UNIV_ID = 342;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Assam Higher Secondary Education Council (AHSEC)' WHERE UNIV_ID = 344;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Mangalore Institute of Technology and Engineering (MITE), Moodabidri' WHERE UNIV_ID = 549;
UPDATE EDU_UNIVERSITY SET UNIV_NAME = 'Other Recognized University / Board' WHERE UNIV_ID = 550;

COMMIT;

-- ------------------------------------------------------------------------
-- STEP 2: Harmonize Existing Employee Records in EDU_EMP_EDUCATION
-- ------------------------------------------------------------------------

-- Update VTU occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Visvesvaraya Technological University (VTU), Belagavi'
WHERE UPPER(TRIM(INSTITUTE_NAME)) = 'VTU'
   OR UPPER(INSTITUTE_NAME) LIKE '%VISWESWARAIAH%'
   OR UPPER(INSTITUTE_NAME) LIKE '%VISVESVARAYA TECHNOLOGICAL UNIVERSITY%';

-- Update IISc occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Indian Institute of Science (IISc), Bangalore'
WHERE UPPER(TRIM(INSTITUTE_NAME)) IN ('IISC', 'IISC, BANGALORE', 'IISC BANGALORE');

-- Update JNTU occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Jawaharlal Nehru Technological University (JNTU), Hyderabad'
WHERE UPPER(TRIM(INSTITUTE_NAME)) = 'JNTU'
   OR UPPER(INSTITUTE_NAME) LIKE 'JAWAHARLAL NEHRU TECHNOLOGICAL UNIVERSITY%HYDERABAD%';

-- Update IGNOU occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Indira Gandhi National Open University (IGNOU), New Delhi'
WHERE UPPER(TRIM(INSTITUTE_NAME)) = 'IGNOU'
   OR UPPER(INSTITUTE_NAME) LIKE '%IGNOU%NEW DELHI%';

-- Update CBSE occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Central Board of Secondary Education (CBSE), New Delhi'
WHERE UPPER(TRIM(INSTITUTE_NAME)) IN ('CBSE', 'CBSE BOARD', 'CBSE BOARD NEW DELHI');

-- Update ICSE occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)'
WHERE UPPER(TRIM(INSTITUTE_NAME)) IN ('ICSE', 'ISC', 'ICSE BOARD', 'ISCE BOARD', 'ICSE / ISC BOARD');

-- Update PUE Karnataka occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Department of Pre-University Education (PUE), Karnataka'
WHERE UPPER(TRIM(INSTITUTE_NAME)) IN ('PUE', 'PRE UNIVERSITY BOARD');

-- Update NIT Surathkal occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'National Institute of Technology Karnataka (NITK / REC), Surathkal'
WHERE UPPER(TRIM(INSTITUTE_NAME)) IN ('REC, SURATKAL', 'KREC SURATHKAL', 'NIT, SURATHKAL', 'NIT SURATHKAL');

-- Update NIT Warangal occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'National Institute of Technology (NIT / REC), Warangal'
WHERE UPPER(TRIM(INSTITUTE_NAME)) IN ('REC, WARANGAL', 'NIT, WARANGAL');

-- Update NIT Trichy occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'National Institute of Technology (NIT / REC), Tiruchirappalli'
WHERE UPPER(TRIM(INSTITUTE_NAME)) IN ('REC, TIRUCHY', 'NIT TRICHY');

-- Update JNU occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Jawaharlal Nehru University (JNU), New Delhi'
WHERE UPPER(TRIM(INSTITUTE_NAME)) IN ('JNU,DELHI', 'JNU');

-- Update Anna University occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Anna University, Chennai'
WHERE UPPER(TRIM(INSTITUTE_NAME)) = 'ANNA UNIVERSITY';

-- Update Bangalore University occurrences
UPDATE EDU_EMP_EDUCATION
SET INSTITUTE_NAME = 'Bangalore University, Bengaluru'
WHERE UPPER(TRIM(INSTITUTE_NAME)) = 'BANGALORE UNIVERSITY';

COMMIT;

-- ------------------------------------------------------------------------
-- STEP 3: Remove / Deactivate Duplicate and Obsolete Abbreviations in EDU_UNIVERSITY
-- ------------------------------------------------------------------------

-- 1. Remove obsolete standalone abbreviations that are now covered by full forms
DELETE FROM EDU_UNIVERSITY
WHERE UNIV_ID IN (
    0,    -- '*' dummy entry
    8,    -- 'U.P.T.U'
    53,   -- 'Madurai University'
    59,   -- 'JNU,Delhi'
    62,   -- 'Mahatma Gandhi University,Kerala'
    72,   -- 'University of Calicut'
    75,   -- 'KREC Surathkal'
    91,   -- 'Gorakhpur University'
    93,   -- 'Banaras Hindu University'
    100,  -- 'Rohilkhand University'
    114,  -- 'REC,Allahabad'
    120,  -- 'ISCE BOARD'
    124,  -- 'University of Kerala'
    133,  -- 'MNIT,Jaipur'
    138,  -- 'SVNIT, Surat'
    141,  -- 'NIT, Durgapur'
    142,  -- 'NIT, Hamirpur'
    143,  -- 'NIT, Silchar'
    144,  -- 'NIT, Warangal'
    145,  -- 'RGPV, Bhopal'
    155,  -- 'Calicut University'
    157,  -- 'NIT, Rourkela'
    159,  -- 'delhi university'
    168,  -- 'VTU' (User specifically cited duplicate of Visvesvaraya Technological University)
    174,  -- 'WBSCTE'
    175,  -- 'JNTU'
    182,  -- 'AU'
    183,  -- 'IISC'
    184,  -- 'CSBE'
    185,  -- 'CUSAT'
    186,  -- 'nagarjuna University'
    187,  -- 'JNTU, Ananthapur, AP'
    188,  -- 'IIT Kharagpur'
    191,  -- 'IISC'
    192,  -- 'osmania University'
    193,  -- 'NIT, Calicut'
    202,  -- 'University of Hyderabad'
    208,  -- 'Anna University'
    222,  -- 'IIT, Rajasthan'
    227,  -- 'ANU, Guntar'
    237,  -- 'NIT Trichy'
    248,  -- 'IGNOU'
    259,  -- 'UNIVERSITY OF MADRAS'
    260,  -- 'BANGALORE UNIVERSITY'
    271,  -- 'University of Poona'
    276,  -- 'NIT, SURATHKAL'
    288,  -- 'Pune University'
    291,  -- 'MANIT, Bhopal'
    295,  -- 'Maharastra State Board'
    303,  -- 'D.U'
    308,  -- 'UPTU, Lucknow'
    313,  -- 'AKTU'
    316,  -- 'IIT, Roorkee'
    341,  -- 'ISC'
    343,  -- 'SEBA'
    345,  -- 'W.B.B.S.E'
    346,  -- 'W.B.C.H.S.E'
    347,  -- 'Maharashtra State Board'
    350,  -- 'Visvesvaraya Technological University (VTU), Belagavi' (Consolidated to ID 129)
    351,  -- 'Jawaharlal Nehru Technological University (JNTU), Hyderabad'
    354,  -- 'Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow'
    356,  -- 'Savitribai Phule Pune University (SPPU), Pune'
    357,  -- 'University of Calcutta (Calcutta University), Kolkata'
    359,  -- 'Panjab University, Chandigarh'
    361,  -- 'University of Lucknow, Lucknow'
    362,  -- 'Patna University, Patna'
    363,  -- 'Utkal University, Bhubaneswar'
    367,  -- 'Aligarh Muslim University (AMU), Aligarh'
    368,  -- 'Jamia Millia Islamia, New Delhi'
    371,  -- 'North-Eastern Hill University (NEHU), Shillong'
    372,  -- 'Indira Gandhi National Open University (IGNOU), New Delhi'
    377,  -- 'Central University of Rajasthan, Ajmer'
    387,  -- 'IIT Bombay'
    388,  -- 'IIT Hyderabad'
    389,  -- 'IIT Indore'
    390,  -- 'IIT (BHU) Varanasi'
    392,  -- 'IIT Gandhinagar'
    393,  -- 'IIT Ropar'
    394,  -- 'IIT Patna'
    395,  -- 'IIT Bhubaneswar'
    396,  -- 'IIT Mandi'
    397,  -- 'IIT Jodhpur'
    404,  -- 'National Institute of Technology (NIT), Tiruchirappalli'
    405,  -- 'National Institute of Technology Karnataka (NITK), Surathkal'
    406,  -- 'National Institute of Technology (NIT), Warangal'
    407,  -- 'National Institute of Technology (NIT), Rourkela'
    408,  -- 'National Institute of Technology (NIT), Calicut'
    410,  -- 'Malaviya National Institute of Technology (MNIT), Jaipur'
    411,  -- 'Motilal Nehru National Institute of Technology (MNNIT), Allahabad'
    412,  -- 'Sardar Vallabhbhai National Institute of Technology (SVNIT), Surat'
    413,  -- 'National Institute of Technology (NIT), Kurukshetra'
    414,  -- 'Maulana Azad National Institute of Technology (MANIT), Bhopal'
    415,  -- 'National Institute of Technology (NIT), Durgapur'
    416,  -- 'National Institute of Technology (NIT), Jamshedpur'
    417,  -- 'National Institute of Technology (NIT), Hamirpur'
    418,  -- 'National Institute of Technology (NIT), Silchar'
    419,  -- 'National Institute of Technology (NIT), Srinagar'
    420,  -- 'National Institute of Technology (NIT), Raipur'
    421,  -- 'National Institute of Technology (NIT), Agartala'
    423,  -- 'National Institute of Technology (NIT), Patna'
    427,  -- 'National Institute of Technology (NIT), Delhi'
    436,  -- 'IIIT Allahabad'
    437,  -- 'ABV-IIITM Gwalior'
    450,  -- 'Indian Institute of Management (IIM), Lucknow'
    451,  -- 'Indian Institute of Management (IIM), Lucknow'
    519,  -- 'Central Board of Secondary Education (CBSE), New Delhi'
    520,  -- 'Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)'
    523,  -- 'Department of Pre-University Education (PUE), Karnataka'
    529,  -- 'Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)'
    531,  -- 'West Bengal Board of Secondary Education (WBBSE)'
    532,  -- 'West Bengal Council of Higher Secondary Education (WBCHSE)'
    534,  -- 'Bihar School Examination Board (BSEB), Patna'
    540,  -- 'Assam Higher Secondary Education Council (AHSEC)'
    541,  -- 'Board of Secondary Education, Assam (SEBA)'
    544   -- 'State Board of Technical Education and Training (SBTET), Tamil Nadu'
);

-- 2. General Cleanup: Delete any remaining exact duplicates (case-insensitive), preserving the smallest UNIV_ID
DELETE FROM EDU_UNIVERSITY
WHERE ROWID NOT IN (
    SELECT MIN(ROWID)
    FROM EDU_UNIVERSITY
    GROUP BY UPPER(TRIM(UNIV_NAME))
);

COMMIT;

PROMPT [SUCCESS] University deduplication completed successfully!
PROMPT Canonical VTU Entry: ID 129 -> 'Visvesvaraya Technological University (VTU), Belagavi'
