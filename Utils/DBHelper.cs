using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EducationApp.Utils
{
    public static class DBHelper
    {
        private static readonly object _schemaLock = new object();
        private static bool _schemaEnsured = false;

        public static string GetConnectionString()
        {
            var cs = ConfigurationManager.ConnectionStrings["EducationDB"];
            if (cs != null && !string.IsNullOrEmpty(cs.ConnectionString))
            {
                return cs.ConnectionString;
            }

            var csCompany = ConfigurationManager.ConnectionStrings["CompanyDB"];
            if (csCompany != null && !string.IsNullOrEmpty(csCompany.ConnectionString))
            {
                return csCompany.ConnectionString;
            }

            return "User Id=system;Password=root;Data Source=//127.0.0.1:1521/xe;Min Pool Size=2;Max Pool Size=50;";
        }

        #region ADO.NET Core Execution Methods

        public static DataTable ExecuteQuery(string query, params OracleParameter[] parameters)
        {
            EnsureSchema();
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(GetConnectionString()))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            cmd.Parameters.Add(CloneParameter(p));
                        }
                    }
                    using (OracleDataAdapter sda = new OracleDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static int ExecuteNonQuery(string query, params OracleParameter[] parameters)
        {
            EnsureSchema();
            int rowsAffected = 0;
            using (OracleConnection conn = new OracleConnection(GetConnectionString()))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            cmd.Parameters.Add(CloneParameter(p));
                        }
                    }
                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public static object ExecuteScalar(string query, params OracleParameter[] parameters)
        {
            EnsureSchema();
            object result = null;
            using (OracleConnection conn = new OracleConnection(GetConnectionString()))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            cmd.Parameters.Add(CloneParameter(p));
                        }
                    }
                    conn.Open();
                    result = cmd.ExecuteScalar();
                }
            }
            return result;
        }

        private static OracleParameter CloneParameter(OracleParameter param)
        {
            if (param == null) return null;
            return new OracleParameter(param.ParameterName, param.Value)
            {
                Direction = param.Direction,
                DbType = param.DbType,
                Size = param.Size,
                IsNullable = param.IsNullable
            };
        }

        #endregion

        #region Schema Initialization & Auto-Setup

        public static void EnsureSchema()
        {
            if (_schemaEnsured) return;

            lock (_schemaLock)
            {
                if (_schemaEnsured) return;

                try
                {
                    using (OracleConnection conn = new OracleConnection(GetConnectionString()))
                    {
                        conn.Open();

                        // 1. Check EDU_EMP_EDUCATION
                        if (!TableExists(conn, "EDU_EMP_EDUCATION"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_EMP_EDUCATION (
                                    EDU_ID NUMBER NOT NULL,
                                    PCNO VARCHAR2(30) NOT NULL,
                                    EMP_NAME VARCHAR2(150),
                                    DESIGNATION VARCHAR2(100),
                                    DIVISION_NAME VARCHAR2(100),
                                    QUAL_MAIN_CODE VARCHAR2(20) NOT NULL,
                                    QUAL_MAIN_NAME VARCHAR2(200),
                                    QUAL_SUB_CODE VARCHAR2(50) NOT NULL,
                                    QUAL_SUB_NAME VARCHAR2(200),
                                    YEAR_PASSED NUMBER(4) NOT NULL,
                                    INSTITUTE_NAME VARCHAR2(250) NOT NULL,
                                    COLLEGE_NAME VARCHAR2(250),
                                    SPECIALIZATION VARCHAR2(200) NOT NULL,
                                    CGPA_MARKS VARCHAR2(50) NOT NULL,
                                    GRADE_DIVISION VARCHAR2(50),
                                    IS_HIGHEST NUMBER(1) DEFAULT 0 NOT NULL,
                                    REMARKS VARCHAR2(1000),
                                    STATUS VARCHAR2(20) DEFAULT 'Draft' NOT NULL,
                                    SUBMITTED_DATE DATE,
                                    CREATED_ON DATE DEFAULT SYSDATE NOT NULL,
                                    CREATED_BY VARCHAR2(30),
                                    MODIFIED_ON DATE,
                                    MODIFIED_BY VARCHAR2(30),
                                    IS_MIGRATED NUMBER(1) DEFAULT 0 NOT NULL,
                                    OLD_CTREDN NUMBER,
                                    CONSTRAINT PK_EDU_EMP_EDUCATION PRIMARY KEY (EDU_ID)
                                )");

                            ExecuteSql(conn, "CREATE SEQUENCE SEQ_EDU_EMP_EDU START WITH 1 INCREMENT BY 1 NOCACHE");
                            ExecuteSql(conn, @"
                                CREATE OR REPLACE TRIGGER TRG_EDU_EMP_EDUCATION_BI
                                BEFORE INSERT ON EDU_EMP_EDUCATION
                                FOR EACH ROW
                                BEGIN
                                    IF :NEW.EDU_ID IS NULL THEN
                                        SELECT SEQ_EDU_EMP_EDU.NEXTVAL INTO :NEW.EDU_ID FROM DUAL;
                                    END IF;
                                END;");

                            ExecuteSql(conn, "CREATE INDEX IDX_EDU_EMP_PCNO ON EDU_EMP_EDUCATION(PCNO)");
                            ExecuteSql(conn, "CREATE INDEX IDX_EDU_EMP_STATUS ON EDU_EMP_EDUCATION(STATUS)");
                        }

                        // 2. Check EDU_ADMIN_USERS
                        if (!TableExists(conn, "EDU_ADMIN_USERS"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_ADMIN_USERS (
                                    PCNO VARCHAR2(30) NOT NULL,
                                    USER_NAME VARCHAR2(100),
                                    ROLE VARCHAR2(30) DEFAULT 'Admin' NOT NULL,
                                    IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL,
                                    CREATED_ON DATE DEFAULT SYSDATE NOT NULL,
                                    CONSTRAINT PK_EDU_ADMIN_USERS PRIMARY KEY (PCNO)
                                )");

                            ExecuteSql(conn, "INSERT INTO EDU_ADMIN_USERS (PCNO, USER_NAME, ROLE, IS_ACTIVE) VALUES ('1001', 'System Administrator', 'Admin', 1)");
                        }

                        // 3. Check EDU_QUAL_MAIN
                        if (!TableExists(conn, "EDU_QUAL_MAIN"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_QUAL_MAIN (
                                    QUAL_CODE VARCHAR2(20) NOT NULL,
                                    QUAL_NAME VARCHAR2(200) NOT NULL,
                                    QUAL_CATEGORY VARCHAR2(50),
                                    DISPLAY_ORDER NUMBER DEFAULT 0,
                                    IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL,
                                    CONSTRAINT PK_EDU_QUAL_MAIN PRIMARY KEY (QUAL_CODE)
                                )");
                            SeedMainQualifications(conn);
                        }

                        // 4. Check EDU_QUAL_SUB
                        if (!TableExists(conn, "EDU_QUAL_SUB"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_QUAL_SUB (
                                    SUB_ID NUMBER NOT NULL,
                                    MAIN_QUAL_CODE VARCHAR2(20),
                                    SUB_CODE VARCHAR2(50) NOT NULL,
                                    SUB_NAME VARCHAR2(200) NOT NULL,
                                    IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL,
                                    DISPLAY_ORDER NUMBER DEFAULT 0,
                                    CONSTRAINT PK_EDU_QUAL_SUB PRIMARY KEY (SUB_ID)
                                )");
                            ExecuteSql(conn, "CREATE SEQUENCE SEQ_EDU_QUAL_SUB START WITH 1 INCREMENT BY 1 NOCACHE");
                            ExecuteSql(conn, @"
                                CREATE OR REPLACE TRIGGER TRG_EDU_QUAL_SUB_BI
                                BEFORE INSERT ON EDU_QUAL_SUB
                                FOR EACH ROW
                                BEGIN
                                    IF :NEW.SUB_ID IS NULL THEN
                                        SELECT SEQ_EDU_QUAL_SUB.NEXTVAL INTO :NEW.SUB_ID FROM DUAL;
                                    END IF;
                                END;");
                            SeedSubQualifications(conn);
                        }

                        // 5. Check EDU_UNIVERSITY
                        if (!TableExists(conn, "EDU_UNIVERSITY"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_UNIVERSITY (
                                    UNIV_ID NUMBER NOT NULL,
                                    UNIV_NAME VARCHAR2(250) NOT NULL,
                                    STATE_OR_TYPE VARCHAR2(100),
                                    IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL,
                                    CONSTRAINT PK_EDU_UNIVERSITY PRIMARY KEY (UNIV_ID)
                                )");
                            ExecuteSql(conn, "CREATE SEQUENCE SEQ_EDU_UNIVERSITY START WITH 1000 INCREMENT BY 1 NOCACHE");
                            SeedUniversities(conn);
                        }

                        // 6. Check EDU_SUBJECT
                        if (!TableExists(conn, "EDU_SUBJECT"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_SUBJECT (
                                    SUBJECT_ID NUMBER NOT NULL,
                                    SUBJECT_NAME VARCHAR2(200) NOT NULL,
                                    SUBJECT_GROUP VARCHAR2(100),
                                    IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL,
                                    CONSTRAINT PK_EDU_SUBJECT PRIMARY KEY (SUBJECT_ID)
                                )");
                            ExecuteSql(conn, "CREATE SEQUENCE SEQ_EDU_SUBJECT START WITH 1000 INCREMENT BY 1 NOCACHE");
                            SeedSubjects(conn);
                        }

                        // 6b. Check EDU_COLLEGE
                        if (!TableExists(conn, "EDU_COLLEGE"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_COLLEGE (
                                    COLLEGE_ID NUMBER NOT NULL,
                                    COLLEGE_NAME VARCHAR2(250) NOT NULL,
                                    COLLEGE_TYPE VARCHAR2(100),
                                    LOCATION VARCHAR2(150),
                                    AFFILIATED_UNIV VARCHAR2(250),
                                    IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL,
                                    CONSTRAINT PK_EDU_COLLEGE PRIMARY KEY (COLLEGE_ID)
                                )");
                            ExecuteSql(conn, "CREATE SEQUENCE SEQ_EDU_COLLEGE START WITH 1000 INCREMENT BY 1 NOCACHE");
                            SeedColleges(conn);
                        }

                        // 7. Check EDU_QUAL_MAPPING
                        if (!TableExists(conn, "EDU_QUAL_MAPPING"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_QUAL_MAPPING (
                                    MAPPING_ID NUMBER NOT NULL,
                                    OLD_QFN_ID NUMBER NOT NULL,
                                    OLD_QFN_NAME VARCHAR2(150),
                                    OLD_QFN_GROUP VARCHAR2(100),
                                    NEW_QUAL_MAIN_CODE VARCHAR2(20) NOT NULL,
                                    NEW_QUAL_SUB_CODE VARCHAR2(50) DEFAULT 'GENERAL',
                                    NOTES VARCHAR2(250),
                                    IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL,
                                    CONSTRAINT PK_EDU_QUAL_MAPPING PRIMARY KEY (MAPPING_ID),
                                    CONSTRAINT UQ_EDU_OLD_QFN_ID UNIQUE (OLD_QFN_ID)
                                )");
                            ExecuteSql(conn, "CREATE SEQUENCE SEQ_EDU_QUAL_MAP START WITH 1 INCREMENT BY 1 NOCACHE");
                            ExecuteSql(conn, @"
                                CREATE OR REPLACE TRIGGER TRG_EDU_QUAL_MAPPING_BI
                                BEFORE INSERT ON EDU_QUAL_MAPPING
                                FOR EACH ROW
                                BEGIN
                                    IF :NEW.MAPPING_ID IS NULL THEN
                                        SELECT SEQ_EDU_QUAL_MAP.NEXTVAL INTO :NEW.MAPPING_ID FROM DUAL;
                                    END IF;
                                END;");
                            SeedQualificationMappings(conn);
                        }

                        // 8. Check EDU_MIGRATION_LOG
                        if (!TableExists(conn, "EDU_MIGRATION_LOG"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_MIGRATION_LOG (
                                    LOG_ID NUMBER NOT NULL,
                                    MIGRATION_DATE DATE DEFAULT SYSDATE NOT NULL,
                                    EXECUTED_BY VARCHAR2(30),
                                    TOTAL_READ NUMBER DEFAULT 0,
                                    TOTAL_MIGRATED NUMBER DEFAULT 0,
                                    TOTAL_SKIPPED NUMBER DEFAULT 0,
                                    STATUS VARCHAR2(50),
                                    LOG_DETAILS VARCHAR2(4000),
                                    CONSTRAINT PK_EDU_MIGRATION_LOG PRIMARY KEY (LOG_ID)
                                )");
                            ExecuteSql(conn, "CREATE SEQUENCE SEQ_EDU_MIG_LOG START WITH 1 INCREMENT BY 1 NOCACHE");
                        }

                        // 9. Check EDU_AUDIT_LOG
                        if (!TableExists(conn, "EDU_AUDIT_LOG"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE EDU_AUDIT_LOG (
                                    AUDIT_ID NUMBER NOT NULL,
                                    PCNO VARCHAR2(30) NOT NULL,
                                    EDU_ID NUMBER,
                                    ACTION_TYPE VARCHAR2(50) NOT NULL,
                                    ACTION_BY VARCHAR2(30) NOT NULL,
                                    ACTION_DATE DATE DEFAULT SYSDATE NOT NULL,
                                    COMMENTS VARCHAR2(1000),
                                    CONSTRAINT PK_EDU_AUDIT_LOG PRIMARY KEY (AUDIT_ID)
                                )");
                            ExecuteSql(conn, "CREATE SEQUENCE SEQ_EDU_AUDIT START WITH 1 INCREMENT BY 1 NOCACHE");
                        }

                        // 10. Check HR_EMP_EDUCATION (Legacy table) - create mock table if not present for development/migration demo
                        if (!TableExists(conn, "HR_EMP_EDUCATION"))
                        {
                            ExecuteSql(conn, @"
                                CREATE TABLE HR_EMP_EDUCATION (
                                    PCNO VARCHAR2(30) NOT NULL,
                                    CTREDN NUMBER NOT NULL,
                                    QFNID NUMBER NOT NULL,
                                    UNIVINSTID NUMBER,
                                    COLLEGENAME VARCHAR2(250),
                                    SUBJECTID NUMBER,
                                    YEARPASSED NUMBER(4),
                                    GRADE VARCHAR2(50),
                                    PERCENTAGE VARCHAR2(50),
                                    HIGHESTQFN VARCHAR2(10),
                                    CONSTRAINT PK_HR_EMP_EDUCATION PRIMARY KEY (PCNO, CTREDN)
                                )");
                            SeedSampleLegacyEducation(conn);
                        }

                        // 11. Check EMPDETAILS mock table for development if neither EMPDETAILS nor HRDATA.EMPDETAILS exists
                        if (!TableExists(conn, "EMPDETAILS") && !TableExists(conn, "HRDATA.EMPDETAILS"))
                        {
                            try
                            {
                                ExecuteSql(conn, @"
                                    CREATE TABLE EMPDETAILS (
                                        PCNO VARCHAR2(30) NOT NULL,
                                        NAME VARCHAR2(100),
                                        DESIGNATION VARCHAR2(100),
                                        DIVNAME VARCHAR2(100),
                                        CONSTRAINT PK_EMPDETAILS PRIMARY KEY (PCNO)
                                    )");
                                ExecuteSql(conn, "INSERT INTO EMPDETAILS (PCNO, NAME, DESIGNATION, DIVNAME) VALUES ('1001', 'System Administrator', 'Administrator', 'IT-ADMIN')");
                                ExecuteSql(conn, "INSERT INTO EMPDETAILS (PCNO, NAME, DESIGNATION, DIVNAME) VALUES ('1002', 'Ramesh Kumar', 'Senior Engineer', 'AERODYNAMICS')");
                                ExecuteSql(conn, "INSERT INTO EMPDETAILS (PCNO, NAME, DESIGNATION, DIVNAME) VALUES ('1003', 'Priya Sharma', 'Scientist C', 'PROPULSION')");
                            }
                            catch { }
                        }

                        // 12. Ensure Sub Qualifications are mapped to Main Qualifications
                        EnsureSubQualificationsLinked(conn);

                        // 13. Ensure legacy migration metadata is cleared from REMARKS column
                        try
                        {
                            if (TableExists(conn, "EDU_EMP_EDUCATION"))
                            {
                                ExecuteSql(conn, "UPDATE EDU_EMP_EDUCATION SET REMARKS = NULL WHERE REMARKS LIKE 'Migrated from%' OR REMARKS LIKE 'Auto-synced%'");
                            }
                        }
                        catch { }

                        // 14. Fix Veterinary code collision (GEN -> AGB) and consolidate Matriculation/10th (NA/GEN -> GENERAL)
                        try
                        {
                            if (TableExists(conn, "EDU_QUAL_SUB"))
                            {
                                // Disambiguate Veterinary Animal Genetics and Breeding from GEN to AGB
                                ExecuteSql(conn, "UPDATE EDU_QUAL_SUB SET SUB_CODE = 'AGB' WHERE SUB_NAME LIKE '%Animal Genetics%' AND SUB_CODE = 'GEN'");

                                // Consolidate MTRI (10th) sub-qualifications to standard 'GENERAL'
                                ExecuteSql(conn, "UPDATE EDU_QUAL_SUB SET SUB_CODE = 'GENERAL', SUB_NAME = 'General (All Subjects)' WHERE MAIN_QUAL_CODE = 'MTRI' AND SUB_CODE IN ('GEN', 'GENERAL')");
                                ExecuteSql(conn, "DELETE FROM EDU_QUAL_SUB WHERE MAIN_QUAL_CODE = 'MTRI' AND SUB_CODE = 'NA'");

                                // Consolidate BMTRI (Below 10th)
                                ExecuteSql(conn, "UPDATE EDU_QUAL_SUB SET SUB_CODE = 'GENERAL', SUB_NAME = 'General (Below 10th)' WHERE MAIN_QUAL_CODE = 'BMTRI' AND SUB_CODE = 'NA'");

                                // Automatic deduplication of EDU_QUAL_SUB for identical (MAIN_QUAL_CODE, SUB_CODE) pairs
                                ExecuteSql(conn, @"
                                    DELETE FROM EDU_QUAL_SUB a
                                    WHERE a.ROWID > (
                                        SELECT MIN(b.ROWID)
                                        FROM EDU_QUAL_SUB b
                                        WHERE NVL(b.MAIN_QUAL_CODE, '__NULL__') = NVL(a.MAIN_QUAL_CODE, '__NULL__')
                                          AND b.SUB_CODE = a.SUB_CODE
                                    )");
                            }

                            if (TableExists(conn, "EDU_QUAL_MAPPING"))
                            {
                                ExecuteSql(conn, "UPDATE EDU_QUAL_MAPPING SET NEW_QUAL_SUB_CODE = 'GENERAL' WHERE NEW_QUAL_MAIN_CODE IN ('MTRI', 'BMTRI') AND NEW_QUAL_SUB_CODE IN ('NA', 'GEN')");
                            }

                            if (TableExists(conn, "EDU_EMP_EDUCATION"))
                            {
                                // Auto-heal existing saved/draft records displaying 'Animal Genetics and Breeding' for 10th Standard
                                ExecuteSql(conn, @"UPDATE EDU_EMP_EDUCATION 
                                                   SET QUAL_SUB_CODE = 'GENERAL', QUAL_SUB_NAME = 'General (All Subjects)' 
                                                   WHERE QUAL_MAIN_CODE IN ('MTRI', '10TH') 
                                                     AND (QUAL_SUB_CODE IN ('GEN', 'NA') OR QUAL_SUB_NAME LIKE '%Animal Genetics%')");

                                ExecuteSql(conn, @"UPDATE EDU_EMP_EDUCATION 
                                                   SET QUAL_SUB_CODE = 'GENERAL', QUAL_SUB_NAME = 'General (Below 10th)' 
                                                   WHERE QUAL_MAIN_CODE = 'BMTRI' 
                                                     AND (QUAL_SUB_CODE IN ('GEN', 'NA') OR QUAL_SUB_NAME LIKE '%Animal Genetics%')");

                                // Auto-heal records where QUAL_SUB_NAME was populated with SPECIALIZATION instead of canonical SUB_NAME from EDU_QUAL_SUB
                                ExecuteSql(conn, @"UPDATE EDU_EMP_EDUCATION e
                                                   SET e.QUAL_SUB_NAME = (
                                                       SELECT qs.SUB_NAME FROM EDU_QUAL_SUB qs
                                                       WHERE qs.SUB_CODE = e.QUAL_SUB_CODE
                                                         AND (qs.MAIN_QUAL_CODE = e.QUAL_MAIN_CODE OR qs.MAIN_QUAL_CODE IS NULL)
                                                         AND ROWNUM = 1
                                                   )
                                                   WHERE e.QUAL_SUB_CODE IS NOT NULL
                                                     AND EXISTS (
                                                       SELECT 1 FROM EDU_QUAL_SUB qs
                                                       WHERE qs.SUB_CODE = e.QUAL_SUB_CODE
                                                         AND (qs.MAIN_QUAL_CODE = e.QUAL_MAIN_CODE OR qs.MAIN_QUAL_CODE IS NULL)
                                                         AND qs.SUB_NAME <> e.QUAL_SUB_NAME
                                                     )");
                            }

                            InvalidateSubQualificationsCache();

                            // 16. Ensure NAC, NTC, CC, ITI and LRDE trades are fully populated
                            EnsureVocationalAndApprenticeshipMasterData(conn);
                        }
                        catch { }
                    }

                    _schemaEnsured = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("EnsureSchema Warning: " + ex.Message);
                    _schemaEnsured = true; // allow app to continue
                }
            }
        }

        private static void EnsureVocationalAndApprenticeshipMasterData(OracleConnection conn)
        {
            try
            {
                if (!TableExists(conn, "EDU_QUAL_MAIN") || !TableExists(conn, "EDU_QUAL_SUB")) return;

                // 1. Ensure NAC, NTC, NCVT, CC, ITI are in EDU_QUAL_MAIN
                ExecuteSql(conn, @"
                    MERGE INTO EDU_QUAL_MAIN dest
                    USING (
                        SELECT 'NAC' AS C, 'NATIONAL APPRENTICESHIP CERTIFICATE' AS N, 'Vocational / Certificate' AS CAT FROM DUAL UNION ALL
                        SELECT 'NTC' AS C, 'NATIONAL TRADE CERTIFICATE' AS N, 'Vocational / Certificate' AS CAT FROM DUAL UNION ALL
                        SELECT 'NCVT' AS C, 'NATIONAL COUNCIL FOR VOCATIONAL TRAINING CERTIFICATE' AS N, 'Vocational / Certificate' AS CAT FROM DUAL UNION ALL
                        SELECT 'CC' AS C, 'CERTIFICATE COURSE' AS N, 'Vocational / Certificate' AS CAT FROM DUAL UNION ALL
                        SELECT 'ITI' AS C, 'INDUSTRIAL TRAINING INSTITUTE (ITI)' AS N, 'Vocational / Certificate' AS CAT FROM DUAL
                    ) src ON (dest.QUAL_CODE = src.C)
                    WHEN NOT MATCHED THEN INSERT (QUAL_CODE, QUAL_NAME, QUAL_CATEGORY, IS_ACTIVE) VALUES (src.C, src.N, src.CAT, 1)");

                // 2. Ensure EDU_QUAL_MAPPING maps old QFN 42 to NAC and 44 to NTC
                if (TableExists(conn, "EDU_QUAL_MAPPING"))
                {
                    ExecuteSql(conn, "UPDATE EDU_QUAL_MAPPING SET NEW_QUAL_MAIN_CODE = 'NAC', NEW_QUAL_SUB_CODE = 'APPR' WHERE OLD_QFN_ID = 42");
                    ExecuteSql(conn, "UPDATE EDU_QUAL_MAPPING SET NEW_QUAL_MAIN_CODE = 'NTC', NEW_QUAL_SUB_CODE = 'VOCATIONAL' WHERE OLD_QFN_ID = 44");
                    ExecuteSql(conn, "UPDATE EDU_QUAL_MAPPING SET NEW_QUAL_MAIN_CODE = 'CC', NEW_QUAL_SUB_CODE = 'STENO' WHERE OLD_QFN_ID = 43");
                }

                // 3. Update existing records in EDU_EMP_EDUCATION where QFN 42 / NAC was previously labeled as CC
                if (TableExists(conn, "EDU_EMP_EDUCATION"))
                {
                    ExecuteSql(conn, @"
                        UPDATE EDU_EMP_EDUCATION
                        SET QUAL_MAIN_CODE = 'NAC',
                            QUAL_MAIN_NAME = 'NATIONAL APPRENTICESHIP CERTIFICATE'
                        WHERE QUAL_MAIN_CODE = 'CC'
                          AND (
                              UPPER(SPECIALIZATION) LIKE '%APPRENTICE%'
                              OR UPPER(REMARKS) LIKE '%NAC%'
                              OR UPPER(QUAL_SUB_NAME) LIKE '%APPRENTICE%'
                          )");
                }

                // 4. Seed comprehensive LRDE-specific Sub Qualifications for NAC, ITI, CC, NTC, NCVT
                string[] lrdeMains = new string[] { "NAC", "ITI", "CC", "NTC", "NCVT" };
                Tuple<string, string>[] lrdeTrades = new Tuple<string, string>[]
                {
                    Tuple.Create("RADAR", "Radar Mechanic / Radar Systems Technician"),
                    Tuple.Create("EL_MECH", "Electronics Mechanic"),
                    Tuple.Create("RADIO_TV", "Radio and Television Mechanic"),
                    Tuple.Create("RF_MW", "RF and Microwave Technician"),
                    Tuple.Create("INSTR_MECH", "Instrument Mechanic / Electronic Instrumentation"),
                    Tuple.Create("PCB_SMT", "PCB Fabrication, Design & SMT Assembly"),
                    Tuple.Create("OPTO", "Optoelectronics & Fiber Optics Technician"),
                    Tuple.Create("TELECOM", "Telecommunication & Network Systems"),
                    Tuple.Create("CALIB", "Test, Calibration & Measurement Technician"),
                    Tuple.Create("ELECT", "Electrician / Industrial Electrician"),
                    Tuple.Create("WIRE", "Wireman & Military Cable Harnessing"),
                    Tuple.Create("PWR_SYS", "Power Electronics & UPS Systems"),
                    Tuple.Create("RAC", "Refrigeration & Air Conditioning (Chiller / Radar Cooling)"),
                    Tuple.Create("FIT", "Fitter (Mechanical / Assembly / Bench Fitter)"),
                    Tuple.Create("MEC", "Machinist / Precision Machinist"),
                    Tuple.Create("CNC", "CNC Operator cum Programmer (Milling & Turning)"),
                    Tuple.Create("TUR", "Turner / Precision Lathe Work"),
                    Tuple.Create("TOOL_DIE", "Tool and Die Maker (Press Tools, Jigs & Fixtures)"),
                    Tuple.Create("DRAUGHT_M", "Draughtsman (Mechanical / CAD Technician)"),
                    Tuple.Create("DRAUGHT_C", "Draughtsman (Civil / Structural)"),
                    Tuple.Create("WEL", "Welder (Gas, Electric, TIG / MIG Welding)"),
                    Tuple.Create("SHEET_MET", "Sheet Metal Worker / Enclosure Fabrication"),
                    Tuple.Create("ELECTRO", "Electroplating & Surface Finishing"),
                    Tuple.Create("COPA", "Computer Operator & Programming Assistant (COPA)"),
                    Tuple.Create("ICTSM", "Information & Communication Technology (ICTSM)"),
                    Tuple.Create("STENO", "Stenography & Secretarial Practice"),
                    Tuple.Create("QA_NDT", "Quality Assurance & Non-Destructive Testing (NDT)"),
                    Tuple.Create("VOCATIONAL", "Vocational Training (Technical Trades)"),
                    Tuple.Create("APPR", "National Apprenticeship Training (General)"),
                    Tuple.Create("OTHER", "Other Technical Trade (Specify in Specialization)")
                };

                foreach (string mainCode in lrdeMains)
                {
                    foreach (var trade in lrdeTrades)
                    {
                        using (OracleCommand cmd = new OracleCommand(@"
                            MERGE INTO EDU_QUAL_SUB dest
                            USING (SELECT :MQ AS MQ, :SC AS SC, :SN AS SN FROM DUAL) src
                            ON (dest.MAIN_QUAL_CODE = src.MQ AND dest.SUB_CODE = src.SC)
                            WHEN NOT MATCHED THEN INSERT (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE)
                            VALUES (src.MQ, src.SC, src.SN, 1)
                            WHEN MATCHED THEN UPDATE SET dest.SUB_NAME = src.SN, dest.IS_ACTIVE = 1", conn))
                        {
                            cmd.Parameters.Add(new OracleParameter("MQ", mainCode));
                            cmd.Parameters.Add(new OracleParameter("SC", trade.Item1));
                            cmd.Parameters.Add(new OracleParameter("SN", trade.Item2));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Global fallback entries with MAIN_QUAL_CODE IS NULL
                foreach (var trade in lrdeTrades)
                {
                    using (OracleCommand cmd = new OracleCommand(@"
                        MERGE INTO EDU_QUAL_SUB dest
                        USING (SELECT :SC AS SC, :SN AS SN FROM DUAL) src
                        ON (dest.MAIN_QUAL_CODE IS NULL AND dest.SUB_CODE = src.SC)
                        WHEN NOT MATCHED THEN INSERT (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE)
                        VALUES (NULL, src.SC, src.SN, 1)", conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("SC", trade.Item1));
                        cmd.Parameters.Add(new OracleParameter("SN", trade.Item2));
                        cmd.ExecuteNonQuery();
                    }
                }

                // 5. Ensure standard Certifying Authorities and Vocational Bodies exist in EDU_UNIVERSITY
                if (TableExists(conn, "EDU_UNIVERSITY"))
                {
                    string[] certifyingBodies = new string[]
                    {
                        "National Council for Vocational Training (NCVT) / DGT",
                        "Directorate General of Training (DGT), Ministry of Skill Development",
                        "Defence Research and Development Organisation (DRDO / LRDE)",
                        "Centre for Development of Advanced Computing (C-DAC)",
                        "National Institute of Electronics & Information Technology (NIELIT / DOEACC)",
                        "State Board of Technical Education and Training (SBTET / DTE)",
                        "Other Recognized Certifying Body / Institute"
                    };

                    foreach (string body in certifyingBodies)
                    {
                        try
                        {
                            using (OracleCommand cmd = new OracleCommand(@"
                                MERGE INTO EDU_UNIVERSITY dest
                                USING (SELECT :N AS N FROM DUAL) src
                                ON (UPPER(TRIM(dest.UNIV_NAME)) = UPPER(TRIM(src.N)))
                                WHEN NOT MATCHED THEN INSERT (UNIV_ID, UNIV_NAME, STATE_OR_TYPE, IS_ACTIVE)
                                VALUES (NVL((SELECT MAX(UNIV_ID) + 1 FROM EDU_UNIVERSITY), 900), src.N, 'Certifying Body / Authority', 1)", conn))
                            {
                                cmd.Parameters.Add(new OracleParameter("N", body));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch { }
                    }
                }

                InvalidateSubQualificationsCache();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EnsureVocationalAndApprenticeshipMasterData Warning: " + ex.Message);
            }
        }

        private static void EnsureSubQualificationsLinked(OracleConnection conn)
        {
            try
            {
                if (!TableExists(conn, "EDU_QUAL_SUB")) return;

                using (OracleCommand chkCmd = new OracleCommand("SELECT COUNT(*) FROM EDU_QUAL_SUB WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE NOT IN ('NA', 'OTHER', 'GENERAL')", conn))
                {
                    object unlinkedObj = chkCmd.ExecuteScalar();
                    int unlinked = unlinkedObj != null && unlinkedObj != DBNull.Value ? Convert.ToInt32(unlinkedObj) : 0;
                    if (unlinked == 0) return;
                }

                string[] linkStatements = new string[]
                {
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'BE' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('CIVIL', 'DMECH', 'COMPE', 'COMPS', 'ELCL', 'EC', 'E&C', 'ELECT', 'INDST', 'INSTR', 'AERO', 'AUTO', 'CHEMI', 'NANO', 'RACE', 'ADEL', 'ADMT', 'ANMT', 'APH', 'APE', 'APO', 'ARAN', 'CREO', 'CTSP', 'CSYG', 'DENGG', 'DEMQ', 'DSCE', 'AENGG', 'ELTE', 'FLM', 'GAST', 'GUIM', 'GUIW', 'HEPEN', 'INCS', 'INEC', 'INOP', 'LEOE', 'MCDY', 'MACDE', 'MRENG', 'MIELE', 'OEAOC', 'POE', 'PRENG', 'REBTE', 'SOSTA', 'SPER', 'SIT', 'SYSCO', 'SYSSO', 'TELES', 'THERE', 'TODE', 'TOOEN', 'WELTE', 'COMME', 'ELINS', 'SEFEG', 'GENENG', 'AI', 'AM', 'AROSE', 'AUTMO', 'AVONI', 'DSP', 'EE', 'IM', 'INDE', 'INSSP', 'IS', 'IT', 'MS', 'M&S', 'MAREN', 'MATEC', 'MD', 'METAL', 'MME', 'MSD', 'NARCH', 'PLEN', 'RFM', 'TEXTL')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'BSC' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('PHY', 'CHEM', 'MATH', 'BOT', 'ZOO', 'STATS', 'BIO', 'BIOT', 'BICHM', 'GENSC', 'ENV', 'MICBIO', 'MATSC', 'GEOL', 'AGRSC', 'INDM', 'NPHYS', 'OCGG', 'ORACH', 'PHRCH', 'PHYIN', 'PHYME', 'PLBRE', 'PEMLL', 'AG', 'BP', 'FSAT', 'HG', 'HORT', 'MIBIO', 'OPERA', 'POUSC')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'BA' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('ENG', 'HND', 'HST', 'ECO', 'POL', 'SOCIO', 'PSY', 'PHIL', 'GEO', 'SS', 'HS', 'PAD', 'TRANS', 'JY', 'AP', 'HP', 'BLIB', 'DSST', 'SRD')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'BCOM' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('COM', 'ACC', 'FINAN', 'ACF')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'BBA' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('HMCT')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'MBA' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('MANG', 'HR', 'MARKT', 'PM', 'TECHM', 'MM')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'ITI' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('FIT', 'TUR', 'MEC', 'WEL', 'EL', 'CR', 'DM', 'SMW', 'COPA', 'P', 'BB', 'AC', 'DTP', 'DR(M)', 'DPHG', 'PHG')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'DENG' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('DCHEM')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'MCA' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('CA', 'SOFTE', 'SS1')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'MBBS' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('MED', 'CP', 'PHYL', 'MBC', 'BIOMD', 'DEN', 'VET')",
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'LLB' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('LAW', 'IPR')"
                };

                foreach (string sql in linkStatements)
                {
                    try
                    {
                        ExecuteSql(conn, sql);
                    }
                    catch { }
                }

                try
                {
                    ExecuteSql(conn, @"
                        INSERT INTO EDU_QUAL_SUB (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE)
                        SELECT 'BTECH', SUB_CODE, SUB_NAME, 1
                        FROM EDU_QUAL_SUB
                        WHERE MAIN_QUAL_CODE = 'BE'
                          AND SUB_CODE NOT IN (SELECT SUB_CODE FROM EDU_QUAL_SUB WHERE MAIN_QUAL_CODE = 'BTECH')");

                    ExecuteSql(conn, @"
                        INSERT INTO EDU_QUAL_SUB (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE)
                        SELECT 'B.E', SUB_CODE, SUB_NAME, 1
                        FROM EDU_QUAL_SUB
                        WHERE MAIN_QUAL_CODE = 'BE'
                          AND SUB_CODE NOT IN (SELECT SUB_CODE FROM EDU_QUAL_SUB WHERE MAIN_QUAL_CODE = 'B.E')");

                    ExecuteSql(conn, @"
                        INSERT INTO EDU_QUAL_SUB (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE)
                        SELECT 'B.TECH', SUB_CODE, SUB_NAME, 1
                        FROM EDU_QUAL_SUB
                        WHERE MAIN_QUAL_CODE IN ('BTECH', 'BE')
                          AND SUB_CODE NOT IN (SELECT SUB_CODE FROM EDU_QUAL_SUB WHERE MAIN_QUAL_CODE = 'B.TECH')");

                    ExecuteSql(conn, @"
                        INSERT INTO EDU_QUAL_SUB (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE)
                        SELECT 'M.E', SUB_CODE, SUB_NAME, 1
                        FROM EDU_QUAL_SUB
                        WHERE MAIN_QUAL_CODE = 'ME'
                          AND SUB_CODE NOT IN (SELECT SUB_CODE FROM EDU_QUAL_SUB WHERE MAIN_QUAL_CODE = 'M.E')");

                    ExecuteSql(conn, @"
                        INSERT INTO EDU_QUAL_SUB (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE)
                        SELECT 'M.TECH', SUB_CODE, SUB_NAME, 1
                        FROM EDU_QUAL_SUB
                        WHERE MAIN_QUAL_CODE IN ('MTECH', 'ME')
                          AND SUB_CODE NOT IN (SELECT SUB_CODE FROM EDU_QUAL_SUB WHERE MAIN_QUAL_CODE = 'M.TECH')");
                }
                catch { }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EnsureSubQualificationsLinked: " + ex.Message);
            }
        }

        private static bool TableExists(OracleConnection conn, string tableName)
        {
            using (OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM USER_TABLES WHERE UPPER(TABLE_NAME) = :TNAME", conn))
            {
                cmd.Parameters.Add(new OracleParameter("TNAME", tableName.ToUpper()));
                object res = cmd.ExecuteScalar();
                return res != null && Convert.ToInt32(res) > 0;
            }
        }

        private static void ExecuteSql(OracleConnection conn, string sql)
        {
            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Master Seed Helpers

        private static void SeedMainQualifications(OracleConnection conn)
        {
            string[] items = new string[]
            {
                "ADST|ADVANCED DIPLOMA IN SCIENCE AND TECHNOLOGY|Diploma",
                "AMD|DIPLOMA IN AVIATION MEDICINE|Degree",
                "AMIE|AMIE/IETE|Degree",
                "ATC|ADVANCED TRAINING CERTIFICATE|Vocational / Certificate",
                "B ED|BACHELOR OF EDUCATION|Degree",
                "BA|BACHOLER OF ARTS|Undergraduate",
                "BAL|BACHELOR OF ACADEMIC LAWS|Degree",
                "BAM|BACHELOR OF AYURVEDIC MEDICINE|Degree",
                "BAMS|BACHELOR OF AYURVEDIC MEDICINE AND SURGERY|Undergraduate",
                "BARCH|BACHELOR OF ARCHITECTURE|Undergraduate",
                "BBA|BACHELOR OF BUSINESS ADMINISTRATION|Undergraduate",
                "BBM|BACHELOR OF BUSINESS MANAGEMENT|Undergraduate",
                "BBS|BACHELOR OF BUSINESS STUDIES|Degree",
                "BCA|BACHELOR OF COMPUTER APP.|Undergraduate",
                "BCL|BACHELOR OF CIVIL LAWS|Degree",
                "BCOM|BACHELOR OF COMMERCE|Undergraduate",
                "BD|BACHELOR OF DRAMATICS|Degree",
                "BDES|BACHELOR OF DESIGN|Undergraduate",
                "BDNCE|BACHELOR OF DANCE|Degree",
                "BDS|BACHELOR  OF DENTAL SURGERY|Undergraduate",
                "BE|BACHELOR OF ENGINEERING|Undergraduate",
                "BFA|BACHELOR OF FINE ARTS|Undergraduate",
                "BFSC|BACHELOR OF FISHERIES SCIENCE|Undergraduate",
                "BGL|BACHELOR OF GENERAL LAWS|Degree",
                "BHM|BACHELOR OF HOTEL MANAGEMENT|Undergraduate",
                "BHMS|BACHELOR OF HOMOEOPATHIC MEDICINE AND SURGERY|Undergraduate",
                "BHRD|BACHELOR OF HUMAN RESOURCE DEVELOPMENT|Degree",
                "BHSC|BACHELOR OF HOME SCIENCE|Degree",
                "BIJ|BACHELOR OF JOURNALISM|Degree",
                "BIJC|BACHELOR OF JOURNALISM AND COMMUNICATION|Degree",
                "BIL|BACHELOR OF INDUSTRIAL LAW|Degree",
                "BIM|BACHELOR OF INDIAN MEDICINE|Degree",
                "BJMC|BACHELOR OF JOURNALISM AND MASS COMMUNICATION|Undergraduate",
                "BLIB|BACHELOR OF LIBRARY SCIENCE|Undergraduate",
                "BLISC|BACHELOR OF LIBRARY AND INFORMATION SCIENCE|Undergraduate",
                "BLITT|BACHELOR OF LITERATURE|Degree",
                "BMCJ|BACHELOR OF MASS COMMUNICATION AND JOURNALISM|Degree",
                "BME|BACHELOR OF MECHANICAL ENGG|Degree",
                "BMETT|BACHELOR OF METALLURGY|Degree",
                "BMRSC|BACHELOR OF MEDICAL RESEARCH SCIENCE|Degree",
                "BMS|BACHELOR OF MEDICINE AND SURGERY(HOMEO)|Degree",
                "BMTRI|Less Than 10th|Secondary School",
                "BNYS|BACHELOR OF NATUROPATHY AND YOGIC SCIENCES|Undergraduate",
                "BOL|BACHELOR OF ORIENTAL LEARNING|Degree",
                "BPA|BACHELOR OF PERFORMING ART|Degree",
                "BPED|BACHELOR OF PHYSICAL EDUCATION|Degree",
                "BPES|BACHELOR OF PHYSICAL EDUCATION AND SPORT SCIENCES|Degree",
                "BPHARM|BACHELOR OF PHARMACY|Undergraduate",
                "BPLAN|BACHELOR OF PLANNING|Undergraduate",
                "BPT|BACHELOR OF PHYSIOTHERAPY|Undergraduate",
                "BRS|BACHELOR OF RURAL STUDIES|Degree",
                "BSAM|BACHELOR OF SHUDDHA AURVEDIC MEDICINE|Degree",
                "BSC|BACHELOR OF SCIENCE|Undergraduate",
                "BSCEN|BACHELOR OF SCIENCE (ENGG.)|Degree",
                "BSOSC|BACHELOR OF SOCIAL SCIENCE|Degree",
                "BSTAT|BACHELOR OF STATISTICS|Undergraduate",
                "BSW|BACHELOR OF SOCIAL WORK|Undergraduate",
                "BT|BACHELOR OF TEACHING|Degree",
                "BTECH|BACHELOR OF TECHNOLOGY|Undergraduate",
                "BTEXT|BACHELOR OF TEXTILES|Degree",
                "BTH|BACHELOR OF THEOLOGY|Degree",
                "BTHO|BACHELOR OF OCCUPATIONAL THERAPY|Degree",
                "BUMMS|BACHELOR OF UNANI MEDICENE WITH MORDERN MEDICINE AND SURGERY|Degree",
                "BUMS|BACHELOR OF UNANI MEDICINE AND SURGERY|Degree",
                "BUTS|BACHELOR OF UNANI TIB AND SURGERY|Degree",
                "BVA|BACHELOR OF VISUAL ARTS|Degree",
                "BVASC|BACHELOR OF VERTERINARY ANIMAL SCIENCE|Degree",
                "BVOC|BACHELOR OF VOCATIONAL STUDIES|Undergraduate",
                "BVSAH|BACHELOR OF VERTERINARY SCIENCE AND ANIMAL HUSBANDRY|Degree",
                "BVSC|BACHELOR OF VETERINARY SCIENCE|Undergraduate",
                "CA|CHARTERED ACCOUNTANCY (ICAI)|Professional Certification",
                "CC|CERTIFICATE COURSE|Vocational / Certificate",
                "CFA|CHARTERED FINANCIAL ANALYST|Professional Certification",
                "CMA|COST AND MANAGEMENT ACCOUNTANCY (ICMAI)|Professional Certification",
                "CS|COMPANY SECRETARY (ICSI)|Professional Certification",
                "DA|DIPLOMA IN ANAESTHESIA|Diploma",
                "DAM|DIPLOMA IN AYURVEDIC MEDICINE|Diploma",
                "DAYM|DOCTOR OF AYURVEDIC MEDICINE|Diploma",
                "DBA|DIPLOMA IN BUSINESS MANAGEMENT|Diploma",
                "DCD|DIPLOMA IN CHEST DISEASES|Diploma",
                "DCE|DIPLOMA IN CIVIL ENGINEERING|Diploma",
                "DCH|DIPLOMA IN  CHILD HEALTH|Diploma",
                "DCL|DOCTOR OF CIVIL LAWS|Diploma",
                "DCP|DIPLOMA IN CLINICAL PATHOLOGY|Diploma",
                "DCPM|DIPLOMA IN  CLINICAL PATHOLOGY AND MICROBIOLOGY|Diploma",
                "DD|DIPLOMA IN  DERMATOLOGY|Diploma",
                "DDM|DIPLOMA IN  DERMATOLOGICAL MEDICINE|Diploma",
                "DDR|DIPLOMA IN DIAGNOSTIC RADIOLOGY|Diploma",
                "DDVD|DIPLOMA IN DERMATOLOGY AND VENEREAL DISEASES|Diploma",
                "DE|DIPLOMA IN EMBRYOLOGY|Diploma",
                "DECE|DIPLOMA IN ELECTRONICS AND COMMUNICATION ENGINEERING|Diploma",
                "DED|DOCTOR OF EDUCATION|Diploma",
                "DEE|DIPLOMA IN ELECTRICAL ENGINEERING|Diploma",
                "DENG|DIPLOMA IN ENGINEERING|Diploma",
                "DENGG|DOCTOR OF ENGINEERING|Diploma",
                "DFM|DIPLOMA IN FORENSIC MEDICINE|Diploma",
                "DGO|DIPLOMA IN GYNACOLOGY AND OBSTRICTS|Diploma",
                "DH|DIPLOMA IN INDUSTRIAL HEALTH|Diploma",
                "DHE|DIPLOMA IN HEALTH EDUCATION|Diploma",
                "DHRM|DIPLOMA IN HUMAN RESOURCE MANAGEMENT|Diploma",
                "DHY|DOCTOR OF HYGIENE|Diploma",
                "DIIT|POST GRADUATE DIPLOMA OF IIT|Post Graduate Diploma",
                "DIP|DIPLOMA IN PSYCHIARTRIC|Diploma",
                "DIPL|DIPLOMA|Diploma",
                "DIT|DIPLOMA IN INFORMATION TECHNOLOGY|Diploma",
                "DIV|DIPLOMA IN VOCATIONAL EDUCATION|Diploma",
                "DLE|DIPLOMA IN ELECTRICAL / ELECTRONICS ENGINEERING|Diploma",
                "DLIB|DIPLOMA IN LIBRARY SCIENCE|Diploma",
                "DLITT|DIPLOMA IN LITERATURE|Diploma",
                "DLO|DIPLOMA IN LARYNGOLOGY AND OTOLOGY|Diploma",
                "DLW|DIPLOMA IN LABOUR WELFARE|Diploma",
                "DM|DOCTORATE OF MEDICINE (SUPER SPECIALTY)|Doctorate / Research",
                "DMA|DOCTOR OF AURVEDIC MEDICINE|Diploma",
                "DMCW|VMETERNITY AND CHILD WELFARE|Diploma",
                "DME|DIPLOMA IN MEDICINAL ENGINEERING|Diploma",
                "DMIT|DIPLOMA OF MADRAS INSTITUTE OF TECHNOLOGY|Diploma",
                "DMLT|DIPLOMA IN MEDICAL LABORATORY TECHNOLOGY|Diploma",
                "DMM|DIPLOMA IN MARKETING MANAGEMENT|Diploma",
                "DMP|DIPLOMA IN  MEDICAL PATHOGY AND BACTERIOLOGY|Diploma",
                "DMR|DIPLOMA IN MEDICAL RADIOLOGY|Diploma",
                "DMRD|DIPLOMA IN MEDICAL RADIO DIANOSIS|Diploma",
                "DMRE|DIPLOMA IN  MEDICAL RADIOLOLLGY AND ELECTROLOGY|Diploma",
                "DMRIT|DIPLOMA IN  DIPLOMA IN RADIO ISTOTOPE|Diploma",
                "DMRT|DIPLOMA IN  MEDICAL RADITION THERPY|Diploma",
                "DMUS|DOCTOR OF MUSIC|Diploma",
                "DNB|DIPLOMATE OF NATIONAL BOARD|Postgraduate",
                "DO|DIPLOMA IN OPHOPAEDIC SURGERY|Diploma",
                "DOEACCA|DOEACC 'A' LEVEL (PG DIPLOMA EQUIVALENT)|Post Graduate Diploma",
                "DOEACCO|DOEACC 'O' LEVEL (DIPLOMA EQUIVALENT)|Diploma",
                "DOL|DOCTOR OF ORIENTAL LEARNING|Diploma",
                "DOMS|DIPLOMA IN  OPHALMIC MEDICINE AND SURGERY|Diploma",
                "DPD|DIPLOMA IN PUBLIC ADMINISTRATION|Diploma",
                "DPE|DIPLOMA IN PHYSICAL EDUCATION|Diploma",
                "DPHIL|DOCTRATE IN PHILOSOPHY|Diploma",
                "DPM|DIPLOMA IN  PSYCHOLOGICAL MEDICINE|Diploma",
                "DPMNR|DIPLOMA IN PHYSICAL MEDICINE AND REHABILITATION|Diploma",
                "DRE|DIPLOMA IN RADIO ENGINEERING|Diploma",
                "DSC|DIPLOMA IN SCIENCES|Diploma",
                "DSS|DIPLOMA IN SOCIAL SERVICE|Diploma",
                "DSTD|DIPLOMA IN SEXUALLY TRANSMITED DISEASES|Diploma",
                "DTCD|DIPLOMA IN TUBERCULOSIS AND CHEST DESEASES|Diploma",
                "DTE|DIPLOMA IN TECHNICAL EDUCATION|Diploma",
                "DTH|DOCTOR OF THEOLOGY|Diploma",
                "DTMH|DIPLOMA IN TROPICAL MEDICINE AND HGIENE|Diploma",
                "DUAL|DUAL DEGREE (INTEGRATED BE/BTECH + ME/MTECH)|Undergraduate",
                "DUGA|DIPLOMA IN  URBAN GOVT. AND ADMINISTRATION|Diploma",
                "DUM|DOCTOR OF UNANI MEDICINE|Diploma",
                "DV|DIPLOMA IN VENERAEOLOGY|Diploma",
                "DVND|DIPLOMA IN VENERAEOLOGY AND DERMETOLOGY|Diploma",
                "EG|ENGINEERING GRADUATES|Degree",
                "EPG|ENGINEERING POST GRADUATES|Degree",
                "GDT|GRADUATE|Degree",
                "GMP|GENERAL MANAGEMENT PROGRAMME (GMP)|Post Graduate Diploma",
                "INTER|INTERMEDIATE OR PLUS 2|Higher Secondary / Pre-University",
                "ITI|ITI|Vocational / Certificate",
                "JD|DIPLOMA IN JOURNALISM|Degree",
                "LLB|BACHELOR OF LAWS|Undergraduate",
                "LLD|DOCTOR OF LAWS|Degree",
                "LLM|MASTER OF LAWS|Postgraduate",
                "M ED|MASTER OF EDUCATION|Degree",
                "MA|MASTER OF ARTS|Postgraduate",
                "MAMM|MASTER OF AGRICULTURAL MARKETING MANAGEMENT|Degree",
                "MARCH|MASTER OF ARCHITECHURE|Degree",
                "MAS|MASTER OF APPLIED SCIENCE|Degree",
                "MAYM|MASTER OF AYURVEDIC MEDICINE|Degree",
                "MAYSC|MASTER OF AYURVEDIC SCIENCE|Degree",
                "MBA|MASTER OF BUSINESS ADMINSTRATION|Postgraduate",
                "MBBS|BACHELOR OF MEDICINE AND BACHELOR OF SURGERY|Undergraduate",
                "MBF|MASTER OF BUSINESS FINANCE|Degree",
                "MBM|MASTER OF BUSINESS MANAGEMENT|Degree",
                "MBS|MASTER OF  BUSINESS STATISTICS|Degree",
                "MCA|MASTER OF COMPUTER APPLICATIONS|Postgraduate",
                "MCH|MASTER OF CHIRURGIAE (SUPER SPECIALTY)|Doctorate / Research",
                "MCJ|MASTER OF COMMUNICATION AND JOURNALISM|Degree",
                "MCL|MASTER OF COMPARITIVE LAWS|Degree",
                "MCM|MASTER IN COMPUTER MANAGEMENT|Degree",
                "MCOM|MASTER OF COMMERCE|Postgraduate",
                "MCP|MASTER OF CITY PLANNING|Degree",
                "MCRP|MASTER OF CITY AND REGIONAL PLANNING|Degree",
                "MCT|MASTER OF COLLEGE TEACHING|Degree",
                "MD|DOCTRATE IN MEDICINE|Postgraduate",
                "MDAY|DOCTOR OF MEDICINE (AYURVEDIC)|Degree",
                "MDES|MASTER OF DESIGN|Postgraduate",
                "MDS|MASTER OF DENTAL SURGERY|Degree",
                "ME|MASTER OF ENGINEERING|Postgraduate",
                "MFA|MASTER OF FINE ARTS|Postgraduate",
                "MFC|MASTER OF FINANCE AND CONTROL|Degree",
                "MFOOD|MASTER OF FOOD AND TECHNOLOGY|Degree",
                "MFSC|MASTER OF FISHERIES SCIENCE|Degree",
                "MHED|MASTER OF HIGHER EDUCATION|Degree",
                "MHSC|MASTER OF HOME SCIENCE|Degree",
                "MIRPM|MASTER OF  RELATION AND PERSONNEL MANG|Degree",
                "MJ|MASTER OF JOURNALISM|Degree",
                "MJMC|MASTER OF JOURNALISM AND MASS COMMUNICATION|Postgraduate",
                "MLIB|MASTER OF LIBRARY SCIENCE|Degree",
                "MLIS|MASTER OF  LIBRARY AND INFORMATION SCIENCE|Degree",
                "MLITT|MASTER OF LITERATURE OR LETTERS|Degree",
                "MLS|MASTER OF LABOUR STUDIES|Degree",
                "MLW|MASTER OF LABOUR WELFARE|Degree",
                "MMED|MASTER OF SCIENCE  MEDICAL|Degree",
                "MMS|MASTER OF MANAGEMENT STUDIES|Degree",
                "MMSC|MASTER OF MANGEMENT SCIENCE|Degree",
                "MPE|MASTER OF PHYSICAL EDUCATION|Degree",
                "MPES|MASTER OF PHYSICAL EDUCATION AND SPORTS|Degree",
                "MPH|MPHIL|Doctorate / Research",
                "MPHARM|MASTER OF PHARMACY|Postgraduate",
                "MPHIL|MASTER OF PHILOSOPHY|Doctorate / Research",
                "MPLAN|MASTER OF PLANNING|Postgraduate",
                "MPM|MASTER OF  PHYSICAL MANAGEMENT|Degree",
                "MPT|MASTER OF PHYSIOTHERAPY|Postgraduate",
                "MRP|MASTER OF REGIONAL PLANNING|Degree",
                "MRS|MASTER OF  RURAL STUDIES|Degree",
                "MS|MASTER OF SURGERY|Postgraduate",
                "MSC|MASTER OF SCIENCE|Postgraduate",
                "MSEE|MASTER OF SCIENCE IN ELECTRICAL ENGINEERING|Postgraduate",
                "MSENG|MASTER OF SCIENCE (ENGINEERING)|Degree",
                "MSTAT|MASTER OF STATISTICS|Postgraduate",
                "MSW|MASTER OF SOCIAL WORK|Postgraduate",
                "MTA|MASTER OF TOURISM ADMINISTRATION|Degree",
                "MTECH|MASTER OF TECHNOLOGY|Postgraduate",
                "MTH|MASTER OF THEOLOGY|Degree",
                "MTP|MASTER OF TOWN PLANNING|Degree",
                "MTRI|10 th|Secondary School",
                "MURP|MASTER OF URBAN AND REGIONAL PLANNING|Degree",
                "MVOC|MASTER OF VOCATIONAL STUDIES|Postgraduate",
                "MVSC|MASTER OFVERTINARY SCIENCE|Degree",
                "NAC|NATIONAL APPRENTICESHIP CERTIFICATE|Vocational / Certificate",
                "NCVT|NATIONAL COUNCIL FOR VOCATIONAL TRAINING CERTIFICATE|Vocational / Certificate",
                "NTC|NATIONAL TRADE CERTIFICATE|Vocational / Certificate",
                "OTHER|OTHER|Degree",
                "PG|POST GRADUATE|Degree",
                "PGD|POST GRADUATE DIPLOMA|Degree",
                "PGDBA|POST GRADUATE DIPLOMA IN BUSINESS ADMINISTRATION|Post Graduate Diploma",
                "PGDCA|POST GRADUATE DIPLOMA IN COMPUTER APPLICATIONS|Postgraduate",
                "PGDCS|POST GRADUATE DIPLOMA IN COMPUTER SCIENCE|Post Graduate Diploma",
                "PGDFM|POST GRADUATE DIPLOMA IN FINANCIAL MANAGEMENT|Post Graduate Diploma",
                "PGDHRM|POST GRADUATE DIPLOMA IN HUMAN RESOURCE MANAGEMENT|Post Graduate Diploma",
                "PGDIIT|POST GRADUATE DIIT|Post Graduate Diploma",
                "PGDIM|POST GRADUATE DIPLOMA IN INDUSTRIAL MANAGEMENT|Post Graduate Diploma",
                "PGDIR|POST GRADUATE DIPLOMA IN INDUSTRIAL RELATIONS|Post Graduate Diploma",
                "PGDM|POST GRADUATE DIPLOMA IN MANAGEMENT|Postgraduate",
                "PGDMM|POST GRADUATE DIPLOMA IN MATERIALS / MARKETING MANAGEMENT|Post Graduate Diploma",
                "PGDPM|POST GRADUATE DIPLOMA IN PERSONNEL MANAGEMENT|Post Graduate Diploma",
                "PGDVLSI|POST GRADUATE DIPLOMA IN VLSI DESIGN|Post Graduate Diploma",
                "PHD|DOCTORATE IN PHILOSOPHY|Doctorate / Research",
                "PHDD|PHD (DOCTORATE)|Doctorate / Research",
                "POSTDIP|POST DIPLOMA COURSE|Diploma",
                "PPC|PRE PROFESSIONAL COURSE|Degree",
                "PSY|MASTER OF PSYCHOLOGY|Degree",
                "PUC|PRE UNIVERSITY COURSE|Higher Secondary / Pre-University",
                "SCI|12 th|Higher Secondary / Pre-University",
                "SOCIO|MASTER OF SOCIOLOGY|Degree",
                "STENO|CERTIFICATE IN STENOGRAPHY|Vocational / Certificate",
                "TD|DIPLOMA IN  TEACHING|Degree",
                "TTE|TRANING FOR TEACHING ENGLISH|Degree",
                "TTG|TRANING OF TEACHERS IN GEOGRAPHY|Degree",
                "UG|Undergraduate|Degree",
            };

            foreach (var item in items)
            {
                var parts = item.Split('|');
                using (OracleCommand cmd = new OracleCommand(@"
                    MERGE INTO EDU_QUAL_MAIN dest
                    USING (SELECT :C AS C, :N AS N, :CAT AS CAT FROM DUAL) src
                    ON (dest.QUAL_CODE = src.C)
                    WHEN NOT MATCHED THEN INSERT (QUAL_CODE, QUAL_NAME, QUAL_CATEGORY, IS_ACTIVE) VALUES (src.C, src.N, src.CAT, 1)", conn))
                {
                    cmd.Parameters.Add(new OracleParameter("C", parts[0]));
                    cmd.Parameters.Add(new OracleParameter("N", parts[1]));
                    cmd.Parameters.Add(new OracleParameter("CAT", parts[2]));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void SeedSubQualifications(OracleConnection conn)
        {
            // Seed common and specialized branches
            Tuple<string, string, string>[] subs = new Tuple<string, string, string>[]
            {
                // Engineering Streams
                Tuple.Create("BE", "COMPS", "Computer Science and Engineering"),
                Tuple.Create("BE", "COMPE", "Computer Engineering"),
                Tuple.Create("BE", "IT", "Information Technology"),
                Tuple.Create("BE", "SOFTE", "Software Engineering"),
                Tuple.Create("BE", "AI", "Artificial Intelligence and Data Science"),
                Tuple.Create("BE", "EC", "Electronics and Communication"),
                Tuple.Create("BE", "ELCL", "Electrical and Electronics"),
                Tuple.Create("BE", "ELTE", "Electronics and Telecommunication"),
                Tuple.Create("BE", "DMECH", "Mechanical Engineering"),
                Tuple.Create("BE", "CIVIL", "Civil Engineering"),
                Tuple.Create("BE", "AERO", "Aeronautical Engineering"),
                Tuple.Create("BE", "AROSE", "Aerospace Engineering"),
                Tuple.Create("BE", "INDST", "Industrial Engineering"),
                Tuple.Create("BE", "INSTR", "Instrumentation Engineering"),
                Tuple.Create("BE", "CHEMI", "Chemical Engineering"),
                Tuple.Create("BE", "METAL", "Metallurgy and Material Science"),
                Tuple.Create("BE", "AUTMO", "Automobile Engineering"),
                Tuple.Create("BE", "RACE", "Radar and Communication Engineering"),
                Tuple.Create("BE", "GENENG", "General Engineering"),
                Tuple.Create("BE", "OTHER", "Other Engineering Stream"),

                // B.Tech
                Tuple.Create("BTECH", "COMPS", "Computer Science and Engineering"),
                Tuple.Create("BTECH", "IT", "Information Technology"),
                Tuple.Create("BTECH", "EC", "Electronics and Communication"),
                Tuple.Create("BTECH", "ELCL", "Electrical and Electronics"),
                Tuple.Create("BTECH", "DMECH", "Mechanical Engineering"),
                Tuple.Create("BTECH", "CIVIL", "Civil Engineering"),
                Tuple.Create("BTECH", "GENENG", "General Engineering"),
                Tuple.Create("BTECH", "OTHER", "Other B.Tech Stream"),

                // M.Tech / ME
                Tuple.Create("MTECH", "COMPS", "Computer Science and Engineering"),
                Tuple.Create("MTECH", "SOFTE", "Software Engineering"),
                Tuple.Create("MTECH", "EC", "Digital Electronics and Communication"),
                Tuple.Create("MTECH", "RACE", "Radar and Communication"),
                Tuple.Create("MTECH", "VLSI", "VLSI Design"),
                Tuple.Create("MTECH", "DMECH", "Machine Design / Thermal"),
                Tuple.Create("MTECH", "POE", "Power Electronics"),
                Tuple.Create("MTECH", "DSP", "Digital Signal Processing"),
                Tuple.Create("MTECH", "GENENG", "General M.Tech"),
                Tuple.Create("MTECH", "OTHER", "Other M.Tech Specialization"),

                Tuple.Create("ME", "COMPS", "Computer Science"),
                Tuple.Create("ME", "EC", "Electronics and Communication"),
                Tuple.Create("ME", "DMECH", "Mechanical Engineering"),
                Tuple.Create("ME", "GENENG", "General ME"),
                Tuple.Create("ME", "OTHER", "Other ME Specialization"),

                // Science Streams
                Tuple.Create("BSC", "GENSC", "General Science"),
                Tuple.Create("BSC", "PHY", "Physics"),
                Tuple.Create("BSC", "CHEM", "Chemistry"),
                Tuple.Create("BSC", "MATH", "Mathematics"),
                Tuple.Create("BSC", "COMPS", "Computer Science"),
                Tuple.Create("BSC", "IT", "Information Technology"),
                Tuple.Create("BSC", "STATS", "Statistics"),
                Tuple.Create("BSC", "BIO", "Biology"),
                Tuple.Create("BSC", "BIOT", "Biotechnology"),
                Tuple.Create("BSC", "OTHER", "Other Science Stream"),

                Tuple.Create("MSC", "GENSC", "General Science"),
                Tuple.Create("MSC", "PHY", "Physics"),
                Tuple.Create("MSC", "CHEM", "Chemistry"),
                Tuple.Create("MSC", "MATH", "Mathematics"),
                Tuple.Create("MSC", "COMPS", "Computer Science"),
                Tuple.Create("MSC", "IT", "Information Technology"),
                Tuple.Create("MSC", "STATS", "Statistics"),
                Tuple.Create("MSC", "OTHER", "Other M.Sc Stream"),

                // Commerce & Management
                Tuple.Create("BCOM", "COM", "General Commerce"),
                Tuple.Create("BCOM", "ACC", "Accounting & Finance"),
                Tuple.Create("BCOM", "TAX", "Taxation"),
                Tuple.Create("BCOM", "OTHER", "Other Commerce Stream"),

                Tuple.Create("MCOM", "COM", "Commerce"),
                Tuple.Create("MCOM", "FINAN", "Finance"),
                Tuple.Create("MCOM", "ACC", "Accounts"),
                Tuple.Create("MCOM", "OTHER", "Other M.Com Stream"),

                Tuple.Create("MBA", "MANG", "General Management"),
                Tuple.Create("MBA", "HR", "Human Resource Management"),
                Tuple.Create("MBA", "FINAN", "Finance"),
                Tuple.Create("MBA", "MARKT", "Marketing"),
                Tuple.Create("MBA", "OPER", "Operations & Supply Chain"),
                Tuple.Create("MBA", "IT", "Information Systems / IT"),
                Tuple.Create("MBA", "OTHER", "Other MBA Specialization"),

                Tuple.Create("BBA", "MANG", "Business Administration"),
                Tuple.Create("BBA", "HR", "Human Resources"),
                Tuple.Create("BBA", "FINAN", "Finance"),
                Tuple.Create("BBA", "MARKT", "Marketing"),
                Tuple.Create("BBA", "OTHER", "Other BBA Stream"),

                // Arts
                Tuple.Create("BA", "ENG", "English Literature"),
                Tuple.Create("BA", "HND", "Hindi"),
                Tuple.Create("BA", "HST", "History"),
                Tuple.Create("BA", "ECO", "Economics"),
                Tuple.Create("BA", "POL", "Political Science"),
                Tuple.Create("BA", "SOCIO", "Sociology"),
                Tuple.Create("BA", "PSY", "Psychology"),
                Tuple.Create("BA", "OTHER", "Other Arts Subject"),

                Tuple.Create("MA", "ENG", "English"),
                Tuple.Create("MA", "ECO", "Economics"),
                Tuple.Create("MA", "HST", "History"),
                Tuple.Create("MA", "POL", "Political Science"),
                Tuple.Create("MA", "SOCIO", "Sociology"),
                Tuple.Create("MA", "PSY", "Psychology"),
                Tuple.Create("MA", "OTHER", "Other M.A. Stream"),

                // Computer Applications
                Tuple.Create("BCA", "COMPS", "Computer Applications / Software"),
                Tuple.Create("BCA", "OTHER", "Other BCA Stream"),
                Tuple.Create("MCA", "COMPS", "Computer Applications / Software Systems"),
                Tuple.Create("MCA", "OTHER", "Other MCA Stream"),

                // 10th
                Tuple.Create("MTRI", "GENERAL", "General (All Subjects)"),
                Tuple.Create("MTRI", "OTHER", "Other Board Specialization"),
                Tuple.Create("BMTRI", "GENERAL", "General (Below 10th)"),

                // PUC / 12th
                Tuple.Create("PUC", "PCMB", "Physics, Chemistry, Maths, Biology (PCMB)"),
                Tuple.Create("PUC", "PCM", "Physics, Chemistry, Maths (PCM)"),
                Tuple.Create("PUC", "COMMERCE", "Commerce / Accountancy"),
                Tuple.Create("PUC", "ARTS", "Arts / Humanities"),
                Tuple.Create("PUC", "OTHER", "Other Stream"),

                Tuple.Create("SCI", "PCMB", "Science (PCMB)"),
                Tuple.Create("SCI", "PCM", "Science (PCM)"),
                Tuple.Create("SCI", "COMMERCE", "Commerce"),
                Tuple.Create("SCI", "ARTS", "Arts"),
                Tuple.Create("SCI", "OTHER", "Other 12th Stream"),

                // Diploma
                Tuple.Create("DENG", "DMECH", "Mechanical Engineering"),
                Tuple.Create("DENG", "EC", "Electronics and Communication"),
                Tuple.Create("DENG", "ELCL", "Electrical Engineering"),
                Tuple.Create("DENG", "CIVIL", "Civil Engineering"),
                Tuple.Create("DENG", "COMPS", "Computer Science"),
                Tuple.Create("DENG", "AUTMO", "Automobile Engineering"),
                Tuple.Create("DENG", "OTHER", "Other Diploma Branch"),

                Tuple.Create("DIPL", "GENERAL", "General Diploma"),
                Tuple.Create("DIPL", "MANG", "Diploma in Management"),
                Tuple.Create("DIPL", "OTHER", "Other Diploma"),

                // ITI
                Tuple.Create("ITI", "FIT", "Fitter"),
                Tuple.Create("ITI", "EL", "Electrician"),
                Tuple.Create("ITI", "WEL", "Welder"),
                Tuple.Create("ITI", "MEC", "Machinist"),
                Tuple.Create("ITI", "TUR", "Turner"),
                Tuple.Create("ITI", "DM", "Diesel Mechanic"),
                Tuple.Create("ITI", "COPA", "Computer Operator & Programming Asst"),
                Tuple.Create("ITI", "OTHER", "Other ITI Trade"),

                // Doctorate
                Tuple.Create("PHD", "RESEARCH", "Doctoral Research / Thesis"),
                Tuple.Create("PHD", "OTHER", "Other Ph.D Area"),

                // Fallback General
                Tuple.Create("OTHER", "GENERAL", "General"),
                Tuple.Create("OTHER", "OTHER", "Other - Specify in Specialization")
            };

            foreach (var sub in subs)
            {
                using (OracleCommand cmd = new OracleCommand(@"
                    INSERT INTO EDU_QUAL_SUB (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE) 
                    VALUES (:MQ, :SC, :SN, 1)", conn))
                {
                    cmd.Parameters.Add(new OracleParameter("MQ", sub.Item1));
                    cmd.Parameters.Add(new OracleParameter("SC", sub.Item2));
                    cmd.Parameters.Add(new OracleParameter("SN", sub.Item3));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void SeedUniversities(OracleConnection conn)
        {
            string[] univs = new string[]
            {
                "1|Anna University, Chennai",
                "2|Bangalore University, Bengaluru",
                "3|Andhra University, Visakhapatnam",
                "4|Indian Institute of Technology (IIT), Kanpur",
                "5|Indian Institute of Technology (IIT), Madras",
                "6|Indian Institute of Technology (IIT), Delhi",
                "7|Indian Institute of Science (IISc), Bangalore",
                "9|Indian Institute of Technology (IIT), Guwahati",
                "10|Indian Institute of Technology (IIT), Roorkee",
                "11|Banaras Hindu University (BHU), Varanasi",
                "12|Birla Institute of Technology and Science (BITS), Pilani",
                "13|Indian Institute of Technology (IIT), Kharagpur",
                "14|National Institute of Technology (NIT / REC), Warangal",
                "15|National Institute of Technology (NIT / REC), Tiruchirappalli",
                "16|National Institute of Technology Karnataka (NITK / REC), Surathkal",
                "17|National Institute of Technology (NIT / REC), Calicut",
                "18|National Institute of Technology (NIT / REC), Silchar",
                "19|Malaviya National Institute of Technology (MNIT), Jaipur",
                "20|Karnatak University, Dharwad",
                "21|University of Mysore, Mysuru",
                "22|Mangalore University, Konaje",
                "23|Kuvempu University, Shankaraghatta, Shivamogga",
                "24|Annamalai University",
                "25|Jawaharlal Nehru Technological University Kakinada (JNTUK)",
                "26|Jawaharlal Nehru Technological University Anantapur (JNTUA)",
                "27|Acharya Nagarjuna University, Guntur",
                "28|Osmania University, Hyderabad",
                "29|Sri Venkateswara University",
                "30|University of Hyderabad (UoH / Central University), Hyderabad",
                "31|Bharathiar University",
                "32|Bharathidasan University",
                "33|University of Madras, Chennai",
                "34|Rajasthan University",
                "35|Kakatiya University",
                "36|Manonmani Sundaranathan University",
                "37|Sastra University",
                "38|Cochin University of Science and Technology (CUSAT), Kochi",
                "39|Vikram University, Ujjain",
                "40|Maulana Azad National Institute of Technology (MANIT), Bhopal",
                "41|Jainarayan Vayas University, Jodhpur",
                "42|Gulbarga University, Gulbarga",
                "43|Madurai Kamaraj University (MKU), Madurai",
                "44|Jawaharlal Nehru University (JNU), New Delhi",
                "45|University of Delhi (DU), New Delhi",
                "46|Pondicherry University",
                "47|U BTE Roorkee",
                "48|University of Kerala, Thiruvananthapuram",
                "49|National Institute of Technology (NIT / REC), Hamirpur",
                "50|Mahatma Gandhi University, Kerala",
                "51|Sardar Vallabhbhai National Institute of Technology (SVNIT), Surat",
                "52|Gurunanak Dev University, Punjab",
                "54|RD University",
                "55|Rajiv Gandhi Proudyogiki Vishwavidyalaya (RGPV), Bhopal",
                "56|Indian Institute of Technology (IIT), Bombay",
                "57|BIT Mesra Ranchi",
                "58|IAT,Pune",
                "60|Avadh University",
                "61|Utkal University, Bhubaneswar",
                "63|Geherwal University",
                "64|Allahabad University",
                "65|University of Calicut, Malappuram",
                "66|IETE, New Delhi",
                "67|Institution of Engineers, New Delhi",
                "68|Motilal Nehru National Institute of Technology (MNNIT), Allahabad",
                "69|MNB Garwal University",
                "70|MERRUT University",
                "71|Savitribai Phule Pune University (SPPU / University of Pune)",
                "73|University of Calcutta (Calcutta University), Kolkata",
                "74|Deen Dayal Upadhyaya Gorakhpur University (DDU), Gorakhpur",
                "76|Dr.H.S.Gaur University, Sagar",
                "77|Barkatulla University, Bhopal",
                "78|University of Lucknow, Lucknow",
                "79|Kumoan University, Nainital",
                "80|BITS Ranchi",
                "81|Bundel Khand University",
                "82|Dayalbagh University",
                "83|REC, Bhalki",
                "84|Aligarh Muslim University (AMU), Aligarh",
                "85|Ranchi University",
                "86|National Institute of Technology (NIT / REC), Rourkela",
                "87|Sambalpur University, Orissa",
                "88|AIMA,New Delhi",
                "89|Indira Gandhi National Open University (IGNOU), New Delhi",
                "90|MIT,Madras",
                "92|Jabalpur University",
                "94|Burdwan University",
                "95|National Institute of Technology (NIT / REC), Durgapur",
                "96|Mahatma Jyotiba Phule Rohilkhand University (MJPRU), Bareilly",
                "97|Maharshi Dayanand University, Rohtak",
                "98|Gujarat University",
                "99|B.S.B.A University, Muz,Bihar",
                "101|South Gujarat University",
                "102|Dr. B. R. Ambedkar National Institute of Technology (NIT), Jalandhar",
                "103|Jamia Millia Islamia (JMI), New Delhi",
                "104|Amaravati University",
                "105|Marthwada University",
                "106|Centre for Electronics Design Technology",
                "107|MANIPUR UNIVERSITY",
                "108|Army University",
                "109|Panjab University, Chandigarh",
                "110|IAI",
                "111|Bhopal University",
                "112|Punjab Technical University",
                "113|Dr.Ambedkar University",
                "115|M.B.T.I Kanpur",
                "116|I.E.T, C.S.J.M University Kanpur",
                "117|D.A.V.V, Indore",
                "118|Meghalaya Board of School Education",
                "119|North-Eastern Hill University (NEHU), Shillong",
                "121|Assam University",
                "122|Institution of Engineers(India), Kolkatta",
                "123|Govind Ballabh Pant University of Agriculture and Technology",
                "125|Patna University, Patna",
                "126|DOEACC Centre",
                "127|NIIT Ltd.",
                "128|VIDYASAGAR UNIVERSITY",
                "129|Visvesvaraya Technological University (VTU), Belagavi",
                "130|University of Kalyani",
                "131|Bengal Engineering College, Shibpore",
                "132|Pt. RaviShankar University, Raipur",
                "134|Sri KrishnaDevaraya University",
                "135|Nagpur University",
                "136|Kannur University",
                "137|V B University, Hazaribagh",
                "139|IIMM,Bangalore",
                "140|National Institute of Technology (NIT), Jamshedpur",
                "146|National Institute of Technology (NIT), Kurukshetra",
                "147|Govind Ballab Pant Engg. College",
                "148|UPTUI, KNIT",
                "149|UP BTE, Lucknow",
                "150|West Bengal University",
                "151|ABV-Indian Institute of Information Technology and Management (ABV-IIITM), Gwalior",
                "152|BES University",
                "153|DIAT, Pune",
                "154|Kurukshetra University",
                "156|National Institute of Technology (NIT), Raipur",
                "158|National Institute of Technology (NIT), Patna",
                "160|Dilasagram Convent high school, ballarpur",
                "161|Gurunanak College of Science, Ballarpur",
                "162|Govt. College of Engineering, Karad",
                "163|Indian Institute of Information Technology (IIIT), Allahabad",
                "164|U A Board",
                "165|UP Board Allahabad",
                "166|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "167|Department of Pre-University Education (PUE), Karnataka",
                "169|General Education Department",
                "170|Higher Secondary Board",
                "171|BTER Jodhpur",
                "172|West Bengal Board of Secondary Education (WBBSE)",
                "173|West Bengal Council of Higher Secondary Education (WBCHSE)",
                "176|Board of Secondary School Education",
                "177|Board of Intermediate Education",
                "178|IIT, Bangalore",
                "179|BIT, Durg",
                "180|MP Board",
                "181|State Board of Technical Education and Training (SBTET), Tamil Nadu",
                "189|State board of technical Education",
                "190|Kerala Board",
                "194|ICSE/DPS, shimla",
                "195|CBSE/RED, Jhajjar",
                "196|K V Ponda (CBSE)",
                "197|Board of Public Exam. Kerala",
                "198|Board of Technical Education",
                "199|Z P High school",
                "200|Dept.of Tech.Edu, Kerala",
                "201|Board of Vocational HR SEC Edn",
                "203|UP Board",
                "204|Bihar School Examination Board (BSEB), Patna",
                "205|BIEE, Bihar",
                "206|V.B.U, Hazaribag",
                "207|PSG College of Technology, Coimbatore",
                "209|MAGADH UNIVERSITY BODH GAYA",
                "210|BIEC PATNA",
                "211|Veer Kunwar Singh University, ARA",
                "212|BSE, AP",
                "213|BIE, AP",
                "214|University of Bradford",
                "215|B.T.E UP",
                "216|Govt of A.P, Dept of Emp and Training",
                "217|Ministry of Labour and EMP",
                "218|Pre University Board",
                "219|West Bengal State Council of Technical and Vocational Education (WBSCTE), Kolkata",
                "220|Jiwaji University",
                "221|Central Board of Secondary Education (CBSE), New Delhi",
                "223|Indian Institute of Technology (IIT), Gandhinagar",
                "224|VVB SS School",
                "225|CM science College,Darbhanga (BSEB)",
                "226|Indian Institute of Technology (IIT), Patna",
                "228|Dept. Tech. Edu., Karnataka",
                "229|S.B.T.E Bihar",
                "230|B.S.E.H",
                "231|H.S.B.T.E",
                "232|UTTARAKHAND TECHNICAL UNIVERSITY",
                "233|NATIONAL COLLEGE BANGALORE",
                "234|ST. JOSEPHS COLLEGE BANGALORE",
                "235|SSC BOARD",
                "236|INTER BOARD",
                "238|VINAYAKA MISSION UNIVERSITY SALEM",
                "239|CETE STQC,Hyderabad",
                "240|IDEMI MUMBAI",
                "241|Karnataka Board",
                "242|BSE, ODISHA",
                "243|HSCE, ODISHA",
                "244|SCTE and VT, ODISHA",
                "245|Sikkim Manipal University",
                "246|B.S.E.R, AJMER",
                "247|B.T.E.R, Jodhpur",
                "249|AMITE",
                "250|Pune Board",
                "251|Mumbai Board",
                "252|P E C, Chandigarh",
                "253|RGUKT, Basar",
                "254|Nashik Board",
                "255|Automomous. Inst of Govt of Maharashtra",
                "256|Himachal Pradesh University Shimla Himachal Pradesh",
                "257|HP BOARD OF SCHOOL EDUCATION DHARAMSHAL HIMACHAL PRADESH",
                "258|ASSAM STATE BOARD",
                "261|DREXEL UNIVERSITY, PA, USA",
                "262|BIHAR INTERMEDIATE EDUCATION COUNCIL, PATNA",
                "263|Biju Patnaik University of Technology, Orissa, Rourkela",
                "264|Jawaharlal Nehru Technological University (JNTU), Hyderabad",
                "265|Karnataka State Open University",
                "266|Shivaji University, Kolhapur",
                "267|Bharati Vidypeetha University, Pune",
                "268|VHSE BOARD",
                "269|Board of Hariyana",
                "270|Oxford Evening Polytechnic, JP Nagar, Bengaluru",
                "272|GPM College",
                "273|GRM Sr Sec School",
                "274|National Council for Vocational Training (NCVT)",
                "275|Indian Institute of Technology (IIT), Jodhpur",
                "277|Tripura",
                "278|Indian Institute of Management (IIM), Lucknow",
                "279|AP Board",
                "280|Tamilnadu State Board",
                "281|Central University of Rajasthan, Ajmer",
                "282|University of Ajmer",
                "283|National Institute of Technology (NIT), Agartala",
                "284|Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow",
                "285|CHSE, ODISHA",
                "286|CHSS, Jaipur",
                "287|JNVU, JODHPUR",
                "289|Indian Institute of Technology (IIT), Indore",
                "290|National Institute of Technology (NIT), Delhi",
                "292|GGV, Bilaspur",
                "293|UTTARAKHAND BOARD",
                "294|GRAPHIC ERA UNIVERSITY",
                "296|VIT, PUNE",
                "297|Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)",
                "298|National Institute of Technology (NIT), Srinagar",
                "299|Siddarth University",
                "300|RBSE, Ajmer",
                "301|Rajasthan board",
                "302|Banarthali University",
                "304|Board of Secondary Education, Assam (SEBA)",
                "305|SCTE, Assam",
                "306|NERIST, Nirjuli",
                "307|Indian Institute of Technology (IIT), Hyderabad",
                "309|TRRRSVM MIC Kashipur",
                "310|Indian Institute of Technology (IIT), Mandi",
                "311|KV Bhaderwah",
                "312|BSF Sr Sec School",
                "314|MIT Muzaharpur",
                "315|YMCA University",
                "317|SDM Sr Sec School",
                "318|Green Meadows",
                "319|G.B.P.U.A.T, Pantnagar",
                "320|KV (AFS), Bormer",
                "321|Dundlod Public School Jhunjhunu",
                "322|KVM Public Sr Sec School, Sikar",
                "323|Indian Institute of Technology (BHU), Varanasi",
                "324|CMM Jabalpur",
                "325|Indian Institute of Technology (IIT), Ropar",
                "326|BVDU, Pune",
                "327|MIT-SDE, Pune",
                "328|GOVT.POLYTECHNIC BANKA",
                "329|DNS COLLEGE BHUSIARAJOUN",
                "330|H.S.SALEMPUR",
                "331|SBTE, Bihar",
                "332|JAC board",
                "333|SBTE Jharkhand",
                "334|SSIS,DERABASSI",
                "335|GMSSS,CHANDIGARH SEC-16",
                "336|GGDSD,PUNJAB UNIVERSITY",
                "337|SSK NAND INTERCOLL",
                "338|Indian Institute of Technology (IIT), Bhubaneswar",
                "339|S.V GOVT POLYTECHNIC",
                "340|Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)",
                "342|Uttarakhand Board of Technical Education, Roorkee (UBTER)",
                "344|Assam Higher Secondary Education Council (AHSEC)",
                "348|Hemwanti Nandan Bahuguna Garhwal University",
                "349|MADAN MOHAN MALAVIYA UNIVERSITY OF TECHNOLOGY, GORAKHPUR, UP",
                "352|APJ Abdul Kalam Technological University (KTU), Kerala",
                "353|Gujarat Technological University (GTU), Ahmedabad",
                "355|University of Mumbai (Mumbai University)",
                "358|Jadavpur University, Kolkata",
                "360|Punjabi University, Patiala",
                "364|Gauhati University, Guwahati",
                "365|University of Jammu, Jammu",
                "366|University of Kashmir, Srinagar",
                "369|Tezpur University, Assam",
                "370|Visva-Bharati University, Santiniketan",
                "373|Central University of Karnataka, Kalaburagi",
                "374|Central University of Tamil Nadu, Thiruvarur",
                "375|Central University of Kerala, Kasaragod",
                "376|Central University of Andhra Pradesh, Anantapur",
                "378|Central University of Gujarat, Gandhinagar",
                "379|Central University of Punjab, Bathinda",
                "380|Central University of Haryana, Mahendragarh",
                "381|Central University of South Bihar, Gaya",
                "382|Central University of Jharkhand, Ranchi",
                "383|Central University of Odisha, Koraput",
                "384|Central University of Jammu, Samba",
                "385|Central University of Kashmir, Ganderbal",
                "386|Central University of Himachal Pradesh, Dharamshala",
                "391|IIT (ISM) Dhanbad",
                "398|IIT Tirupati",
                "399|IIT Palakkad",
                "400|IIT Dharwad",
                "401|IIT Bhilai",
                "402|IIT Goa",
                "403|IIT Jammu",
                "409|Visvesvaraya National Institute of Technology (VNIT), Nagpur",
                "422|National Institute of Technology (NIT), Goa",
                "424|National Institute of Technology (NIT), Meghalaya",
                "425|National Institute of Technology (NIT), Manipur",
                "426|National Institute of Technology (NIT), Mizoram",
                "428|National Institute of Technology (NIT), Sikkim",
                "429|National Institute of Technology (NIT), Arunachal Pradesh",
                "430|National Institute of Technology (NIT), Uttarakhand",
                "431|National Institute of Technology (NIT), Andhra Pradesh",
                "432|Indian Institute of Engineering Science and Technology (IIEST), Shibpur",
                "433|IIIT Hyderabad",
                "434|IIIT Bangalore",
                "435|IIIT Delhi (Indraprastha Institute of Information Technology)",
                "438|PDPM IIITDM Jabalpur",
                "439|IIITDM Kancheepuram",
                "440|IIITDM Kurnool",
                "441|IIIT Lucknow",
                "442|IIIT Pune",
                "443|IIIT Kota",
                "444|IIIT Sri City",
                "445|IIIT Vadodara",
                "446|IIIT Guwahati",
                "447|Indian Institute of Management (IIM), Ahmedabad",
                "448|Indian Institute of Management (IIM), Bangalore",
                "449|Indian Institute of Management (IIM), Calcutta",
                "452|Indian Institute of Management (IIM), Indore",
                "453|Indian Institute of Management (IIM), Shillong",
                "454|Indian Institute of Management (IIM), Rohtak",
                "455|Indian Institute of Management (IIM), Ranchi",
                "456|Indian Institute of Management (IIM), Raipur",
                "457|Indian Institute of Management (IIM), Tiruchirappalli",
                "458|Indian Institute of Management (IIM), Udaipur",
                "459|Indian Institute of Management (IIM), Kashipur",
                "460|Indian Institute of Management (IIM), Visakhapatnam",
                "461|Indian Institute of Management (IIM), Bodh Gaya",
                "462|Indian Institute of Management (IIM), Amritsar",
                "463|Indian Institute of Management (IIM), Nagpur",
                "464|Indian Institute of Management (IIM), Sambalpur",
                "465|Indian Institute of Management (IIM), Sirmaur",
                "466|Indian Institute of Management (IIM), Jammu",
                "467|Indian Institute of Management (IIM), Mumbai",
                "468|All India Institute of Medical Sciences (AIIMS), New Delhi",
                "469|All India Institute of Medical Sciences (AIIMS), Bhopal",
                "470|All India Institute of Medical Sciences (AIIMS), Bhubaneswar",
                "471|All India Institute of Medical Sciences (AIIMS), Jodhpur",
                "472|All India Institute of Medical Sciences (AIIMS), Patna",
                "473|All India Institute of Medical Sciences (AIIMS), Raipur",
                "474|All India Institute of Medical Sciences (AIIMS), Rishikesh",
                "475|Postgraduate Institute of Medical Education and Research (PGIMER), Chandigarh",
                "476|Jawaharlal Institute of Postgraduate Medical Education and Research (JIPMER), Puducherry",
                "477|National Institute of Mental Health and Neurosciences (NIMHANS), Bangalore",
                "478|Christian Medical College (CMC), Vellore",
                "480|Kasturba Medical College (KMC), Manipal",
                "481|Rajiv Gandhi University of Health Sciences (RGUHS), Karnataka",
                "482|Dr. MGR Educational and Research Institute / Medical University, Tamil Nadu",
                "483|NTR University of Health Sciences (Dr. YSR UHS), Vijayawada",
                "484|Kaloji Narayana Rao University of Health Sciences (KNRUHS), Warangal",
                "485|Maharashtra University of Health Sciences (MUHS), Nashik",
                "486|Kerala University of Health Sciences (KUHS), Thrissur",
                "487|Manipal Academy of Higher Education (MAHE), Manipal",
                "488|Vellore Institute of Technology (VIT), Vellore",
                "489|Vellore Institute of Technology (VIT), Chennai",
                "490|SRM Institute of Science and Technology, Chennai",
                "491|Thapar Institute of Engineering and Technology, Patiala",
                "492|Amrita Vishwa Vidyapeetham, Coimbatore",
                "493|Kalinga Institute of Industrial Technology (KIIT), Bhubaneswar",
                "495|Symbiosis International University, Pune",
                "496|Narsee Monjee Institute of Management Studies (NMIMS), Mumbai",
                "497|Christ (Deemed to be University), Bengaluru",
                "498|Jain (Deemed-to-be University), Bengaluru",
                "499|PES University, Bengaluru",
                "500|RV University, Bengaluru",
                "501|M. S. Ramaiah University of Applied Sciences, Bengaluru",
                "502|Dayananda Sagar University, Bengaluru",
                "503|REVA University, Bengaluru",
                "504|BMS College of Engineering (Autonomous), Bengaluru",
                "505|Shiv Nadar University, Greater Noida",
                "506|Ashoka University, Sonipat",
                "507|O. P. Jindal Global University, Sonipat",
                "508|ICFAI Foundation for Higher Education, Hyderabad",
                "509|GITAM (Deemed to be University), Visakhapatnam",
                "510|Amity University, Noida",
                "511|Chandigarh University, Mohali",
                "512|Lovely Professional University (LPU), Punjab",
                "513|UPES (University of Petroleum and Energy Studies), Dehradun",
                "514|The Institute of Chartered Accountants of India (ICAI)",
                "515|The Institute of Cost Accountants of India (ICMAI)",
                "516|The Institute of Company Secretaries of India (ICSI)",
                "517|National Board of Examinations in Medical Sciences (NBEMS / DNB)",
                "518|Bar Council of India (BCI)",
                "521|National Institute of Open Schooling (NIOS)",
                "522|Karnataka School Examination and Assessment Board (KSEAB / KSEEB)",
                "524|Board of Intermediate Education, Andhra Pradesh (BIEAP)",
                "525|Board of Secondary Education, Andhra Pradesh (BSEAP)",
                "526|Telangana State Board of Intermediate Education (TSBIE)",
                "527|Telangana State Board of Secondary Education (BSETS)",
                "528|Tamil Nadu State Board of School Examinations",
                "530|Uttar Pradesh Madhyamik Shiksha Parishad (UPMSP)",
                "533|Kerala Board of Public Examinations (KBPE / DHSE Kerala)",
                "535|Rajasthan Board of Secondary Education (RBSE / BSER), Ajmer",
                "536|Madhya Pradesh Board of Secondary Education (MPBSE), Bhopal",
                "537|Gujarat Secondary and Higher Secondary Education Board (GSHSEB)",
                "538|Punjab School Education Board (PSEB), Mohali",
                "539|Haryana Board of School Education (HBSE / BSEH), Bhiwani",
                "542|State Board of Technical Education and Training (SBTET), Andhra Pradesh",
                "543|State Board of Technical Education and Training (SBTET), Telangana",
                "545|Department of Technical Education (DTE), Karnataka",
                "546|Maharashtra State Board of Technical Education (MSBTE), Mumbai",
                "547|Board of Technical Education (BTE), Uttar Pradesh",
                "548|Board of Technical Education (BTE), Delhi",
                "549|Mangalore Institute of Technology and Engineering (MITE), Moodabidri",
                "550|Other Recognized University / Board",
            };

            foreach (var u in univs)
            {
                var p = u.Split('|');
                using (OracleCommand cmd = new OracleCommand(@"
                    MERGE INTO EDU_UNIVERSITY dest
                    USING (SELECT :ID AS ID, :NAME AS NAME FROM DUAL) src
                    ON (dest.UNIV_ID = src.ID)
                    WHEN NOT MATCHED THEN INSERT (UNIV_ID, UNIV_NAME, IS_ACTIVE) VALUES (src.ID, src.NAME, 1)", conn))
                {
                    cmd.Parameters.Add(new OracleParameter("ID", Convert.ToInt32(p[0])));
                    cmd.Parameters.Add(new OracleParameter("NAME", p[1]));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void SeedSubjects(OracleConnection conn)
        {
            string[] subs = new string[]
            {
                "0|*|*",
                "1|Manufacturing Technology|Mechanical",
                "2|Microwave and Radar Electronics|Electronics",
                "3|Antennas|Electronics",
                "4|Structures|Civil Engg",
                "5|Information Technology|Computer Science",
                "6|Computer Science|Computer Science",
                "7|Electronics and Communications|Electronics",
                "8|Electrical and Electronics|Electrical",
                "9|Telecommunication Engineering|Telecommunication",
                "10|Mechanical Engineering|Mechanical",
                "11|Civil Engineering|Civil",
                "12|Physics|Physics",
                "13|Chemistry|Chemistry",
                "14|Mathematics|Mathematics",
                "15|Information Science|Computer Science",
                "16|Chemical Engineering|Chemical",
                "17|Computer Engineering|Computer Science",
                "18|Instrumentation Technology|Electronics",
                "19|Computer Applications|Computer Science",
                "20|Commerce|Commerce",
                "21|Arts|Arts",
                "22|Management|Management",
                "23|English|Languages",
                "24|Electronics|Electronics",
                "25|Engineering Design|Mechanical",
                "26|Communication Systems|Electronics",
                "27|Electronics and Instrumentation|Electronics",
                "28|Power Electronics|Electrical",
                "29|Electronics and Telecommunication|Electronics",
                "30|Radar Systems|Radar",
                "31|Electrical Engineering|Electrical",
                "32|Applied Physics|Physics",
                "33|Microwave Engineering|Electronics",
                "34|Modelling and Simulation|Physics",
                "35|Advanced Electronics|Electronics",
                "36|Computer Technology|Computer Science",
                "37|Heat Power|Heat Power",
                "38|Thermal Engineering|Mechanical",
                "39|Laser and ElectroOptics|Electronics",
                "40|Engineering Systems|Computer Science",
                "41|Signal Processing|Electronics",
                "42|Finance|Management",
                "43|Solid State Technology|Physics",
                "44|Spherical Vector Wave Function|*",
                "45|VLSI Design|Electronics",
                "46|Library Science|*",
                "47|Control and Instrumentation|Electrical",
                "48|Radar and Communcation|Radar",
                "49|Process Engineering|*",
                "50|Machine Design|Mechanical",
                "51|Software Systems|Computer Science",
                "52|System Science and Automation|Computer Science",
                "53|Aerospace Engineering|Aerospace",
                "54|Industrial Electronics|Electronics",
                "55|Communication Engineering|Electronics",
                "56|Electronic Design|Electronics",
                "57|Production Engineering|Mechanical",
                "58|Sound and TV|Electronics",
                "59|Automobile Engineering|Mechanical",
                "60|Computer Communication|Computer Science",
                "61|Optical Communication|Electronics",
                "62|Craftsman|Craftsman",
                "63|Electricals|Electrical",
                "64|Radio Mechanics|Electronics",
                "65|Public Relations|Administratiorn",
                "66|Buisiness Administration|Administration",
                "67|Business Management|Management",
                "68|Law|Law",
                "69|ElectroPlating|Chemical",
                "70|Power Systems|Electrical",
                "71|Reliability Engineering|Electronics",
                "72|Marketing|Management",
                "73|Digital Systems|Electronics",
                "74|OPTOELECTRONICS|OPTOELECTRONICS",
                "75|Hindi|Arts",
                "76|Digital Electronics and Advanced Communications|Electronics",
                "77|Computer and Information Science|Computer",
                "78|Microwave Signals and Systems|Electronics",
                "79|Material science|Mechanical",
                "80|Human Resource Management|HRM",
                "81|POLITICAL SCIENCE|GENERAL",
                "82|ECONOMICS|GENERAL",
                "83|RADIO AND TRANSISTOR|ELECTRONICS",
                "84|Secratarial Practice|PA",
                "85|Printing Technology|Printing",
                "86|Communication Control and Networking|Computer Science",
                "87|Zoology|Biological Sciences",
                "88|Sociology|Arts",
                "89|High Voltage Engineering|Electronics",
                "90|Computer Science and Engineering|Computer Science",
                "91|DLLELW|Miscellaneous",
                "92|science|science",
                "93|General|general",
                "94|Maths|Maths",
                "95|Control System Engineering|Electrical",
                "96|RF and Microwave|Electronics and Electrical Communication Engineering",
                "97|INSTRUMENTATION AND CONTROL ENGINEERING|INSTRUMENTATION",
                "98|Radio Frequency Design and Technology|Electronics",
                "99|MS|MASTER OF SCIENCE",
                "100|MS|MS",
                "101|MICROELECTRONICS|MICROELECTRONICS",
                "102|PCMB|INTERMEDIATE",
                "103|MS|ENGINEERING",
                "104|M.S.|M.S.",
                "105|PCM|PCM",
                "106|MASTER OF SCINECE|MASTER OF SCIENCE",
                "107|Computer and Information Science|Computer Science",
                "108|MACHINIST|MECHANICAL",
                "109|INSTRUMENTATION ENGINEERING|INSTRUMENTATION",
            };

            foreach (var s in subs)
            {
                var p = s.Split('|');
                using (OracleCommand cmd = new OracleCommand(@"
                    MERGE INTO EDU_SUBJECT dest
                    USING (SELECT :ID AS ID, :NAME AS NAME, :GRP AS GRP FROM DUAL) src
                    ON (dest.SUBJECT_ID = src.ID)
                    WHEN NOT MATCHED THEN INSERT (SUBJECT_ID, SUBJECT_NAME, SUBJECT_GROUP, IS_ACTIVE) VALUES (src.ID, src.NAME, src.GRP, 1)", conn))
                {
                    cmd.Parameters.Add(new OracleParameter("ID", Convert.ToInt32(p[0])));
                    cmd.Parameters.Add(new OracleParameter("NAME", p[1]));
                    cmd.Parameters.Add(new OracleParameter("GRP", p[2]));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void SeedColleges(OracleConnection conn)
        {
            string[] colleges = new string[]
            {
                "1|Mangalore Institute of Technology and Engineering (MITE), Moodabidri|Engineering College|Moodabidri / Mangaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "2|RV College of Engineering (RVCE), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "3|BMS College of Engineering (BMSCE), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "4|M.S. Ramaiah Institute of Technology (MSRIT), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "5|PES Institute of Technology / PES University, Bengaluru|Engineering College|Bengaluru|PES University, Bengaluru",
                "6|Bangalore Institute of Technology (BIT), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "7|Dayananda Sagar College of Engineering (DSCE), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "8|Sir M. Visvesvaraya Institute of Technology (SMVIT), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "9|Siddaganga Institute of Technology (SIT), Tumakuru|Engineering College|Tumakuru|Visvesvaraya Technological University (VTU), Belagavi",
                "10|The National Institute of Engineering (NIE), Mysuru|Engineering College|Mysuru|Visvesvaraya Technological University (VTU), Belagavi",
                "11|Sri Jayachamarajendra College of Engineering (SJCE), Mysuru|Engineering College|Mysuru|JSS Science and Technology University",
                "12|NMAM Institute of Technology (NMAMIT), Nitte|Engineering College|Nitte / Karkala|Nitte (Deemed to be University)",
                "13|Canara Engineering College (CEC), Benjanapadavu, Mangaluru|Engineering College|Mangaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "14|St. Joseph Engineering College (SJEC), Vamanjoor, Mangaluru|Engineering College|Mangaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "15|Sahyadri College of Engineering & Management, Adyar, Mangaluru|Engineering College|Mangaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "16|P.A. College of Engineering (PACE), Mangaluru|Engineering College|Mangaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "17|Srinivas Institute of Technology (SIT), Valachil, Mangaluru|Engineering College|Mangaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "18|Bearys Institute of Technology (BIT), Mangaluru|Engineering College|Mangaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "19|Alva's Institute of Engineering & Technology (AIET), Moodabidri|Engineering College|Moodabidri|Visvesvaraya Technological University (VTU), Belagavi",
                "20|Shri Madhwa Vadiraja Institute of Technology & Management (SMVITM), Bantakal|Engineering College|Udupi|Visvesvaraya Technological University (VTU), Belagavi",
                "21|B.N.M. Institute of Technology (BNMIT), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "22|RNS Institute of Technology (RNSIT), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "23|JSS Academy of Technical Education (JSSATE), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "24|New Horizon College of Engineering (NHCE), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "25|CMR Institute of Technology (CMRIT), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "26|The Oxford College of Engineering, Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "27|Nitte Meenakshi Institute of Technology (NMIT), Bengaluru|Engineering College|Bengaluru|Visvesvaraya Technological University (VTU), Belagavi",
                "28|KLE Technological University / BVB College of Engineering, Hubballi|Engineering College|Hubballi|KLE Technological University",
                "29|Basaveshwar Engineering College (BEC), Bagalkot|Engineering College|Bagalkot|Visvesvaraya Technological University (VTU), Belagavi",
                "30|SDM College of Engineering and Technology (SDMCET), Dharwad|Engineering College|Dharwad|Visvesvaraya Technological University (VTU), Belagavi",
                "31|Malnad College of Engineering (MCE), Hassan|Engineering College|Hassan|Visvesvaraya Technological University (VTU), Belagavi",
                "32|Bapuji Institute of Engineering and Technology (BIET), Davangere|Engineering College|Davangere|Visvesvaraya Technological University (VTU), Belagavi",
                "33|University Visvesvaraya College of Engineering (UVCE), Bengaluru|Engineering College|Bengaluru|Bangalore University, Bengaluru",
                "34|College of Engineering, Pune (COEP)|Engineering College|Pune|Savitribai Phule Pune University (SPPU / University of Pune)",
                "35|Veermata Jijabai Technological Institute (VJTI), Mumbai|Engineering College|Mumbai|University of Mumbai (Mumbai University)",
                "36|College of Engineering, Guindy (CEG), Anna University, Chennai|Engineering College|Chennai|Anna University, Chennai",
                "37|PSG College of Technology, Coimbatore|Engineering College|Coimbatore|Anna University, Chennai",
                "38|Thapar Institute of Engineering and Technology, Patiala|Engineering College|Patiala|Thapar Institute of Engineering and Technology",
                "39|Birla Institute of Technology (BIT), Mesra, Ranchi|Engineering College|Ranchi|BIT Mesra",
                "40|Manipal Institute of Technology (MIT), Manipal|Engineering College|Manipal|Manipal Academy of Higher Education (MAHE)",
                "41|St. Aloysius College (Autonomous), Mangaluru|Degree / Autonomous College|Mangaluru|Mangalore University, Konaje",
                "42|St. Agnes College (Autonomous), Mangaluru|Degree / Autonomous College|Mangaluru|Mangalore University, Konaje",
                "43|Canara College, Mangaluru|Degree College|Mangaluru|Mangalore University, Konaje",
                "44|SDM College (Autonomous), Ujire|Degree / Autonomous College|Ujire|Mangalore University, Konaje",
                "45|Bhandarkars' Arts & Science College, Kundapura|Degree College|Kundapura|Mangalore University, Konaje",
                "46|Poornaprajna College (PPC), Udupi|Degree College|Udupi|Mangalore University, Konaje",
                "47|Mahatma Gandhi Memorial College (MGM), Udupi|Degree College|Udupi|Mangalore University, Konaje",
                "48|St. Joseph's University / College, Bengaluru|Degree / Autonomous College|Bengaluru|Bangalore University, Bengaluru",
                "49|Mount Carmel College (MCC), Bengaluru|Degree / Autonomous College|Bengaluru|Bengaluru City University",
                "50|National College, Basavanagudi, Bengaluru|Degree / Autonomous College|Bengaluru|Bangalore University, Bengaluru",
                "51|Canara Pre-University College, Mangaluru|PU College / Higher Secondary|Mangaluru|Department of Pre-University Education (PUE), Karnataka",
                "52|St. Aloysius Pre-University College, Mangaluru|PU College / Higher Secondary|Mangaluru|Department of Pre-University Education (PUE), Karnataka",
                "53|Expert Pre-University College, Mangaluru|PU College / Higher Secondary|Mangaluru|Department of Pre-University Education (PUE), Karnataka",
                "54|Sharada Pre-University College, Mangaluru|PU College / Higher Secondary|Mangaluru|Department of Pre-University Education (PUE), Karnataka",
                "55|Alva's Pre-University College, Moodabidri|PU College / Higher Secondary|Moodabidri|Department of Pre-University Education (PUE), Karnataka",
                "56|National Pre-University College, Bengaluru|PU College / Higher Secondary|Bengaluru|Department of Pre-University Education (PUE), Karnataka",
                "57|Kendriya Vidyalaya (KV)|Senior Secondary School|All India|Central Board of Secondary Education (CBSE), New Delhi",
                "58|Delhi Public School (DPS)|Senior Secondary School|All India|Central Board of Secondary Education (CBSE), New Delhi",
                "59|Karnataka (Govt) Polytechnic (KPT), Mangaluru|Polytechnic / Diploma|Mangaluru|Department of Technical Education (DTE), Karnataka",
                "60|Govt ITI College / Industrial Training Institute|Vocational / ITI|State / Central|National Council for Vocational Training (NCVT)",
                "61|Government High School (State Board)|High School / Secondary|Karnataka / State Board|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "62|National High School, Basavanagudi, Bengaluru|High School / Secondary|Bengaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "63|St. Joseph High School / Boys' High School|High School / Secondary|Bengaluru / Karnataka|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "64|Govt High School|High School / Secondary|State Board|State Secondary School Board",
                "65|Bangalore High School (BHS), Bengaluru|High School / Secondary|Bengaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "66|St. Germain High School, Bengaluru|High School / Secondary|Bengaluru|Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)",
                "67|Bishop Cotton Boys' / Girls' School, Bengaluru|Senior Secondary School|Bengaluru|Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)",
                "68|Baldwin Boys' / Girls' High School, Bengaluru|Senior Secondary School|Bengaluru|Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)",
                "69|Cluny Convent High School, Bengaluru|High School / Secondary|Bengaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "70|Sophia High School, Bengaluru|Senior Secondary School|Bengaluru|Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)",
                "71|Sacred Heart Girls' High School, Bengaluru|High School / Secondary|Bengaluru|Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)",
                "72|Sri Kumaran Children's Home, Bengaluru|Senior Secondary School|Bengaluru|Central Board of Secondary Education (CBSE), New Delhi",
                "73|National Public School (NPS), Bengaluru / India|Senior Secondary School|Bengaluru / Pan-India|Central Board of Secondary Education (CBSE), New Delhi",
                "74|The Frank Anthony Public School (FAPS), Bengaluru|Senior Secondary School|Bengaluru|Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)",
                "75|Marimallappa's High School & PU College, Mysuru|High School / PU College|Mysuru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "76|Sadvidya Composite High School & PU College, Mysuru|High School / PU College|Mysuru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "77|Sarada Vilas High School, Mysuru|High School / Secondary|Mysuru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "78|Demonstration Multipurpose School (DMS / RIE), Mysuru|Senior Secondary School|Mysuru|Central Board of Secondary Education (CBSE), New Delhi",
                "79|Canara High School (Main / Urwa), Mangaluru|High School / Secondary|Mangaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "80|St. Aloysius High School, Mangaluru|High School / Secondary|Mangaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "81|Sharada Vidyalaya, Mangaluru|Senior Secondary School|Mangaluru|Central Board of Secondary Education (CBSE), New Delhi",
                "82|St. Ann's High School, Mangaluru|High School / Secondary|Mangaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "83|Rosario High School, Mangaluru|High School / Secondary|Mangaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "84|Milagres High School, Mangaluru|High School / Secondary|Mangaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "85|BEM High School, Mangaluru|High School / Secondary|Mangaluru|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "86|Alva's High School, Moodabidri|High School / Secondary|Moodabidri|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "87|Jain High School & PU College, Moodabidri|High School / PU College|Moodabidri|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "88|Rotary English Medium School, Moodabidri|High School / Secondary|Moodabidri|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "89|Poornaprajna High School, Udupi|High School / Secondary|Udupi|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "90|Christian High School, Udupi|High School / Secondary|Udupi|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "91|Karnatak High School, Dharwad|High School / Secondary|Dharwad|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "92|Basel Mission High School, Dharwad|High School / Secondary|Dharwad|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "93|RLS High School, Dharwad|High School / Secondary|Dharwad|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "94|St. Paul's High School, Belagavi|High School / Secondary|Belagavi|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "95|Beynon Smith High School, Belagavi|High School / Secondary|Belagavi|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "96|Vanita Vidyalaya High School, Belagavi|High School / Secondary|Belagavi|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "97|Jawahar Navodaya Vidyalaya (JNV)|Senior Secondary School|All India / District Level|Central Board of Secondary Education (CBSE), New Delhi",
                "98|Sainik School|Senior Secondary School|All India|Central Board of Secondary Education (CBSE), New Delhi",
                "99|Rashtriya Military School (RMS)|Senior Secondary School|All India|Central Board of Secondary Education (CBSE), New Delhi",
                "100|Army Public School (APS)|Senior Secondary School|All India / Military Cantonments|Central Board of Secondary Education (CBSE), New Delhi",
                "101|Air Force School|Senior Secondary School|All India / AFS Bases|Central Board of Secondary Education (CBSE), New Delhi",
                "102|Navy Children School (NCS)|Senior Secondary School|All India / Naval Bases|Central Board of Secondary Education (CBSE), New Delhi",
                "103|Atomic Energy Central School (AECS)|Senior Secondary School|All India / DAE Centers|Central Board of Secondary Education (CBSE), New Delhi",
                "104|DAV Public School|Senior Secondary School|All India|Central Board of Secondary Education (CBSE), New Delhi",
                "105|Chinmaya Vidyalaya|Senior Secondary School|All India|Central Board of Secondary Education (CBSE), New Delhi",
                "106|Bharatiya Vidya Bhavan (BVB) School|Senior Secondary School|All India|Central Board of Secondary Education (CBSE), New Delhi",
                "107|Ryan International School|Senior Secondary School|All India|Central Board of Secondary Education (CBSE), New Delhi",
                "108|Don Bosco High School|High School / Secondary|All India|State Board / ICSE / CBSE",
                "109|Government Model Higher Primary School|Primary / Middle School|State Board|Department of School Education",
                "110|Government Pre-University College (State PU Board)|PU College / Higher Secondary|Karnataka / State|Department of Pre-University Education (PUE), Karnataka",
                "111|Zilla Parishad High School (ZPHS)|High School / Secondary|State Board|Board of Secondary Education",
                "112|Municipal / Corporation High School|High School / Secondary|City / Municipal|State Secondary Board",
                "113|Morarji Desai Residential School (MDRS)|High School / Secondary|Karnataka|Karnataka Residential Educational Institutions Society (KREIS)",
                "114|Kittur Rani Channamma Residential School|High School / Secondary|Karnataka|Karnataka Residential Educational Institutions Society (KREIS)",
                "115|Eklavya Model Residential School (EMRS)|Senior Secondary School|All India / Tribal Affairs|Central Board of Secondary Education (CBSE), New Delhi",
                "116|St. Mary's High School|High School / Secondary|State / Central|State Board / CBSE / ICSE",
                "117|St. Xavier's High School|High School / Secondary|State / Central|State Board / CBSE / ICSE",
                "118|St. Francis High School|High School / Secondary|State / Central|State Board / CBSE / ICSE",
                "119|Holy Cross High School|High School / Secondary|State / Central|State Board / CBSE / ICSE",
                "120|Convent / Missionary High School|High School / Secondary|State / Central|State Board / ICSE / CBSE",
                "121|Private Aided High School|High School / Secondary|State Board|State Secondary Education Board",
                "122|Private Unaided / CBSE / ICSE School|Senior Secondary School|CBSE / ICSE / State|CBSE / CISCE / State Board"
            };

            foreach (var c in colleges)
            {
                var p = c.Split('|');
                using (OracleCommand cmd = new OracleCommand(@"
                    INSERT INTO EDU_COLLEGE (COLLEGE_ID, COLLEGE_NAME, COLLEGE_TYPE, LOCATION, AFFILIATED_UNIV, IS_ACTIVE) 
                    VALUES (:ID, :NAME, :TYPE, :LOC, :UNIV, 1)", conn))
                {
                    cmd.Parameters.Add(new OracleParameter("ID", Convert.ToInt32(p[0])));
                    cmd.Parameters.Add(new OracleParameter("NAME", p[1]));
                    cmd.Parameters.Add(new OracleParameter("TYPE", p[2]));
                    cmd.Parameters.Add(new OracleParameter("LOC", p[3]));
                    cmd.Parameters.Add(new OracleParameter("UNIV", p[4]));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void SeedQualificationMappings(OracleConnection conn)
        {
            string[] maps = new string[]
            {
                "0|*|*|OTHER|GENERAL|Not specified / Asterisk",
                "1|B.Sc|B.Sc|BSC|GENSC|Bachelor of Science",
                "2|BE|BE|BE|GENENG|Bachelor of Engineering",
                "3|ME|ME|ME|GENENG|Master of Engineering",
                "4|M.Sc|M.Sc|MSC|GENSC|Master of Science",
                "5|AMIETE|BE|AMIE|GENENG|AMIETE to AMIE/IETE",
                "6|B.Tech|BE|BTECH|GENENG|Bachelor of Technology",
                "7|M.Tech|ME|MTECH|GENENG|Master of Technology",
                "8|AMIE|BE|AMIE|GENENG|AMIE/IETE",
                "9|MCA|MCA|MCA|CA|Master of Computer Applications",
                "10|MBA|MBA|MBA|MANG|Master of Business Administration",
                "11|Ph.D|Ph.D|PHD|RESEARCH|Doctorate in Philosophy",
                "12|M.Phil|M.Phil|MPHIL|RESEARCH|Master of Philosophy",
                "13|Diploma|DIP|DIPL|GENERAL|Diploma",
                "14|M.Li.Sc|M.Li.Sc|MLIS|BLIB|Master of Library and Info Science",
                "15|B.Com|B.Com|BCOM|COM|Bachelor of Commerce",
                "16|BA|BA|BA|ARTS|Bachelor of Arts",
                "17|MA|MA|MA|ARTS|Master of Arts",
                "18|M.Com|M.Com|MCOM|COM|Master of Commerce",
                "19|SSLC/X Class|SSLC|MTRI|GENERAL|SSLC / X Class mapped to 10th",
                "20|PUC/XII|PUC/XII|PUC|GENSC|PUC / XII mapped to Pre University Course",
                "21|B.Sc. (Engg.)|B.Sc. (Engg.)|BSCEN|GENENG|Bachelor of Science (Engg.)",
                "22|DMIT|DIP|DIPL|GENERAL|DMIT mapped to Diploma",
                "23|DIIT|*|DIPL|GENERAL|DIIT mapped to Diploma",
                "24|Post Diploma|Post Diploma Course|PGD|GENERAL|Post Diploma",
                "25|DRE|DRE|DENG|RACE|DRE - Diploma in Radar / Radio Engg",
                "26|DEE|Diploma|DENG|ELCL|DEE - Diploma in Electrical Engg",
                "27|MIETE|ME|ME|GENENG|MIETE mapped to Master of Engineering",
                "28|M.Sc.(Engg)|M.Tech|MSENG|GENENG|M.Sc.(Engg)",
                "29|PGDIM|PGDIM|PGD|MANG|PGDIM - Post Graduate Diploma in Management",
                "30|DIM|Diploma in Management|DBA|MANG|DIM - Diploma in Management",
                "31|ADM|MANAGEMENT|DBA|MANG|ADM - Advanced Diploma in Management",
                "32|SDM|MANAGEMENT|DBA|MANG|SDM - Senior Diploma in Management",
                "33|DLE (Hons)|Diploma|DENG|ELCL|DLE - Diploma in Electrical/Electronics",
                "34|Grad. IETE|*|AMIE|GENENG|Grad. IETE mapped to AMIE",
                "35|B.Ed.|*|B Ed|EDUCATION|Bachelor of Education",
                "36|DTE|Diploma|DENG|GENENG|DTE - Diploma in Technical Education",
                "37|DME|Diploma|DENG|DMECH|DME - Diploma in Mechanical Engineering",
                "38|A.D.I.M|Management|DBA|MANG|A.D.I.M - Advanced Diploma in Management",
                "39|D.H.R.M|Management|DBA|HR|D.H.R.M - Diploma in Human Resource Management",
                "40|BTE|BTE|DENG|GENENG|BTE - Board of Technical Education",
                "41|I.T.I|ITI|ITI|VOCATIONAL|I.T.I",
                "42|NAC|NAC|NAC|APPR|NAC - National Apprenticeship Certificate",
                "43|Stenography|Stenography|CC|STENO|Stenography Certificate Course",
                "44|NTC|NTC|NTC|VOCATIONAL|NTC - National Trade Certificate",
                "45|MBBS|Medicine|MBBS|MED|Bachelor of Medicine and Bachelor of Surgery",
                "46|B.Sc. PAR I|B.Sc.|BSC|GENSC|B.Sc. Part I",
                "47|PG|*|PG|GENERAL|Post Graduate (General)",
                "48|DCE|DCE|DENG|CIVIL|DCE - Diploma in Civil Engineering",
                "49|B.Li.Sc|B.Li.Sc|BLISC|BLIB|Bachelor of Library and Info Science",
                "50|LLB|LAW|LLB|LAW|Bachelor of Laws",
                "51|ATC|ATC|CC|VOCATIONAL|ATC - Advanced Training Course",
                "52|RVTI(w)|RVTI(w)|ITI|VOCATIONAL|RVTI(w) - Vocational Training",
                "53|HMT|HMT|CC|VOCATIONAL|HMT Training Course",
                "54|FITTER|ITI|ITI|FIT|Fitter ITI Trade",
                "55|BBA|BBA|BBA|MANG|Bachelor of Business Administration",
                "56|'A' Level|DOEACC|PGD|COMPS|DOEACC 'A' Level (PG Diploma equivalent)",
                "57|'O' Level|DOEACC|DIPL|COMPS|DOEACC 'O' Level (Diploma equivalent)",
                "58|PGDHRM|PGD|PGD|HR|PGDHRM - PG Diploma in HR Management",
                "59|BBM|BBM|BBM|MANG|Bachelor of Business Management",
                "60|PGDMM|PGD|PGD|MARKT|PGDMM - PG Diploma in Marketing",
                "61|ADST|Diploma|DIPL|GENSC|ADST - Advanced Diploma in Science/Tech",
                "62|BL|Law|LLB|LAW|BL - Bachelor of Law",
                "63|MSW|MSW|MSW|SS|Master of Social Work",
                "64|HDMM|MANAGEMENT|DBA|MARKT|HDMM - Higher Diploma in Marketing",
                "65|BCA|BCA|BCA|COMPS|Bachelor of Computer Applications",
                "66|NCVT|NCVT|ITI|VOCATIONAL|NCVT - National Council for Vocational Training",
                "67|M S E E|Masters|ME|GENENG|M S E E - Master of Science in Electrical Engg",
                "68|SSC|SSC|MTRI|GENERAL|SSC - Secondary School Certificate (10th)",
                "69|AISSE|AISSE|MTRI|GENERAL|AISSE - All India Secondary School Exam (10th)",
                "70|AISSCE|AISSCE|SCI|GENSC|AISSCE - All India Sr School Certificate (12th)",
                "71|D.E.C.E|DECE|DENG|EC|D.E.C.E - Diploma in Electronics and Comm",
                "72|PGD (VLSI)|PGD|PGD|ELECT|PGD (VLSI)",
                "73|THSLC|THSLC|MTRI|GENERAL|THSLC - Technical High School Leaving Cert (10th)",
                "74|VHSC|VHSC|SCI|GENSC|VHSC - Vocational Higher Secondary (12th)",
                "75|PGDCS|PGDCS|PGD|COMPS|PGDCS - PG Diploma in Computer Science",
                "76|GRDUATION|GRDUATION|GDT|GENERAL|Graduation (General)",
                "77|PGDFM|PGDFM|PGD|FINAN|PGDFM - PG Diploma in Financial Management",
                "78|Dual Degree|BE|BE|GENENG|Dual Degree (BE/BTech)",
                "79|PG Dip and IR|Diploma|PGD|HR|PG Dip in Industrial Relations",
                "80|Below SSLC|Below SSLC|BMTRI|GENERAL|Below SSLC (Less than 10th)",
                "81|MS|MS|MSENG|GENENG|Master of Science (Engineering)",
                "82|BLI Sc|BLI Sc|BLISC|BLIB|Bachelor of Library Science",
                "83|MLI Sc|MLI Sc|MLIS|BLIB|Master of Library Science",
                "84|LLM|Law|LLM|LAW|Master of Laws",
                "85|VHSE|VHSE|SCI|GENSC|VHSE - Vocational Higher Secondary Exam (12th)",
                "86|PG DIIT|*|PGD|GENENG|PG DIIT - Post Graduate DIIT",
                "87|GMP|*|PGD|MANG|GMP - General Management Programme",
                "88|PGDPM|PGDPM|PGD|HR|PGDPM - PG Diploma in Personnel Management",
                "89|DMM|DMM|DIPL|MARKT|DMM - Diploma in Marketing Management",
                "90|PGDMM|PGDMM|PGD|MARKT|PGDMM - PG Diploma in Materials/Marketing",
                "91|DIT|DIT|DIPL|IT|DIT - Diploma in Information Technology",
                "92|DIV|DIV|DIPL|GENERAL|DIV - Diploma in Vocational",
                "93|PGDBA|PGDBA|PGD|MANG|PGDBA - PG Diploma in Business Administration",
            };

            foreach (var m in maps)
            {
                var parts = m.Split('|');
                using (OracleCommand cmd = new OracleCommand(@"
                    MERGE INTO EDU_QUAL_MAPPING dest
                    USING (SELECT :OID AS OID FROM DUAL) src
                    ON (dest.OLD_QFN_ID = src.OID)
                    WHEN NOT MATCHED THEN INSERT (OLD_QFN_ID, OLD_QFN_NAME, OLD_QFN_GROUP, NEW_QUAL_MAIN_CODE, NEW_QUAL_SUB_CODE, NOTES, IS_ACTIVE)
                    VALUES (:OID, :ONAME, :OGRP, :NMC, :NSC, :NOTES, 1)", conn))
                {
                    cmd.Parameters.Add(new OracleParameter("OID", Convert.ToInt32(parts[0])));
                    cmd.Parameters.Add(new OracleParameter("ONAME", parts[1]));
                    cmd.Parameters.Add(new OracleParameter("OGRP", parts[2]));
                    cmd.Parameters.Add(new OracleParameter("NMC", parts[3]));
                    cmd.Parameters.Add(new OracleParameter("NSC", parts[4]));
                    cmd.Parameters.Add(new OracleParameter("NOTES", parts[5]));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void SeedSampleLegacyEducation(OracleConnection conn)
        {
            // Seed a few sample legacy records in HR_EMP_EDUCATION to test and demo migration
            string[] legacyData = new string[]
            {
                "1002|1|19|166|National High School|93|2010|Distinction|88.5|N",
                "1002|2|20|167|National PU College|94|2012|First Class|84.2|N",
                "1002|3|2|129|BMS College of Engineering|6|2016|First Class|78.4|Y",
                "1003|1|19|235|Govt High School|93|2011|First Class|72.0|N",
                "1003|2|41|1|Govt ITI College|1|2013|First Class|80.5|Y",
                "2947|1|19|166|St. Joseph High School|93|2008|First Class|76.0|N",
                "2947|2|26|129|PES Polytechnic|8|2011|Distinction|82.3|Y"
            };

            foreach (var item in legacyData)
            {
                var p = item.Split('|');
                using (OracleCommand cmd = new OracleCommand(@"
                    INSERT INTO HR_EMP_EDUCATION 
                    (PCNO, CTREDN, QFNID, UNIVINSTID, COLLEGENAME, SUBJECTID, YEARPASSED, GRADE, PERCENTAGE, HIGHESTQFN)
                    VALUES (:PCNO, :CTREDN, :QFNID, :UNIV, :COLL, :SUB, :YR, :GRD, :PCT, :HIGH)", conn))
                {
                    cmd.Parameters.Add(new OracleParameter("PCNO", p[0]));
                    cmd.Parameters.Add(new OracleParameter("CTREDN", Convert.ToInt32(p[1])));
                    cmd.Parameters.Add(new OracleParameter("QFNID", Convert.ToInt32(p[2])));
                    cmd.Parameters.Add(new OracleParameter("UNIV", Convert.ToInt32(p[3])));
                    cmd.Parameters.Add(new OracleParameter("COLL", p[4]));
                    cmd.Parameters.Add(new OracleParameter("SUB", Convert.ToInt32(p[5])));
                    cmd.Parameters.Add(new OracleParameter("YR", Convert.ToInt32(p[6])));
                    cmd.Parameters.Add(new OracleParameter("GRD", p[7]));
                    cmd.Parameters.Add(new OracleParameter("PCT", p[8]));
                    cmd.Parameters.Add(new OracleParameter("HIGH", p[9]));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region User Profile & Role Methods

        public static bool IsAdmin(string pcno)
        {
            if (string.IsNullOrEmpty(pcno)) return false;

            // Check config override
            string adminPcnos = ConfigurationManager.AppSettings["AdminPCNOs"];
            if (!string.IsNullOrEmpty(adminPcnos))
            {
                string[] pcnos = adminPcnos.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string p in pcnos)
                {
                    if (p.Trim().Equals(pcno.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
                }
            }

            // Default fallback for 1001
            if (pcno.Trim() == "1001") return true;

            try
            {
                string query = "SELECT COUNT(*) FROM EDU_ADMIN_USERS WHERE PCNO = :PCNO AND IS_ACTIVE = 1";
                object result = ExecuteScalar(query, new OracleParameter("PCNO", pcno.Trim()));
                return result != null && Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the given PCNO exists in the master hrdata.empdetails table (or fallback empdetails).
        /// Returns true and outputs employee details if found; returns false otherwise.
        /// </summary>
        public static bool CheckEmployeeExistsInHrData(string pcno, out string empName, out string designation, out string division)
        {
            empName = null;
            designation = "";
            division = "";

            if (string.IsNullOrWhiteSpace(pcno))
                return false;

            string cleanPcno = pcno.Trim();

            // 1. Try HRDATA.EMPDETAILS first
            try
            {
                string query = "SELECT PCNO, NAME, DESIGNATION, DIVNAME FROM HRDATA.EMPDETAILS WHERE TRIM(PCNO) = :PCNO AND ROWNUM <= 1";
                DataTable dt = ExecuteQuery(query, new OracleParameter("PCNO", cleanPcno));
                if (dt != null && dt.Rows.Count > 0)
                {
                    empName = dt.Rows[0]["NAME"] != DBNull.Value ? dt.Rows[0]["NAME"].ToString() : ("Employee " + cleanPcno);
                    designation = dt.Rows[0]["DESIGNATION"] != DBNull.Value ? dt.Rows[0]["DESIGNATION"].ToString() : "";
                    division = dt.Rows[0]["DIVNAME"] != DBNull.Value ? dt.Rows[0]["DIVNAME"].ToString() : "";
                    return true;
                }
            }
            catch { }

            // 2. Try SYSTEM.EMPDETAILS / local EMPDETAILS
            try
            {
                string query = "SELECT PCNO, NAME, DESIGNATION, DIVNAME FROM EMPDETAILS WHERE TRIM(PCNO) = :PCNO AND ROWNUM <= 1";
                DataTable dt = ExecuteQuery(query, new OracleParameter("PCNO", cleanPcno));
                if (dt != null && dt.Rows.Count > 0)
                {
                    empName = dt.Rows[0]["NAME"] != DBNull.Value ? dt.Rows[0]["NAME"].ToString() : ("Employee " + cleanPcno);
                    designation = dt.Rows[0]["DESIGNATION"] != DBNull.Value ? dt.Rows[0]["DESIGNATION"].ToString() : "";
                    division = dt.Rows[0]["DIVNAME"] != DBNull.Value ? dt.Rows[0]["DIVNAME"].ToString() : "";
                    return true;
                }
            }
            catch { }

            return false;
        }

        public static DataRow GetEmployeeProfile(string pcno)
        {
            if (string.IsNullOrEmpty(pcno)) return null;

            // Try HRDATA.EMPDETAILS first
            try
            {
                string query = "SELECT PCNO, NAME, DESIGNATION, DIVNAME FROM HRDATA.EMPDETAILS WHERE TRIM(PCNO) = :PCNO AND ROWNUM <= 1";
                DataTable dt = ExecuteQuery(query, new OracleParameter("PCNO", pcno.Trim()));
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
            }
            catch { }

            // Try SYSTEM.EMPDETAILS / EMPDETAILS
            try
            {
                string query = "SELECT PCNO, NAME, DESIGNATION, DIVNAME FROM EMPDETAILS WHERE TRIM(PCNO) = :PCNO AND ROWNUM <= 1";
                DataTable dt = ExecuteQuery(query, new OracleParameter("PCNO", pcno.Trim()));
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
            }
            catch { }

            // Create fallback row
            DataTable fallbackTable = new DataTable();
            fallbackTable.Columns.Add("PCNO");
            fallbackTable.Columns.Add("NAME");
            fallbackTable.Columns.Add("DESIGNATION");
            fallbackTable.Columns.Add("DIVNAME");
            DataRow row = fallbackTable.NewRow();
            row["PCNO"] = pcno;
            row["NAME"] = "Employee " + pcno;
            row["DESIGNATION"] = IsAdmin(pcno) ? "Administrator" : "Staff Member";
            row["DIVNAME"] = IsAdmin(pcno) ? "D-ADMIN" : "D-USER";
            return row;
        }

        #endregion

        #region Employee Education Operations

        public static DataTable GetEmployeeEducations(string pcno)
        {
            string query = @"
                SELECT e.EDU_ID, e.PCNO, e.EMP_NAME, e.DESIGNATION, e.DIVISION_NAME,
                       e.QUAL_MAIN_CODE, e.QUAL_MAIN_NAME, e.QUAL_SUB_CODE,
                       COALESCE(
                           (SELECT qs.SUB_NAME FROM EDU_QUAL_SUB qs 
                            WHERE qs.SUB_CODE = e.QUAL_SUB_CODE 
                              AND (qs.MAIN_QUAL_CODE = e.QUAL_MAIN_CODE OR qs.MAIN_QUAL_CODE IS NULL)
                              AND ROWNUM = 1),
                           e.QUAL_SUB_NAME,
                           e.QUAL_SUB_CODE
                       ) AS QUAL_SUB_NAME,
                       e.YEAR_PASSED, e.INSTITUTE_NAME, e.COLLEGE_NAME, e.SPECIALIZATION,
                       e.CGPA_MARKS, e.GRADE_DIVISION, e.IS_HIGHEST, e.REMARKS, e.STATUS,
                       e.SUBMITTED_DATE, e.CREATED_ON, e.IS_MIGRATED
                FROM EDU_EMP_EDUCATION e
                WHERE e.PCNO = :PCNO
                ORDER BY e.IS_HIGHEST DESC, e.YEAR_PASSED DESC, e.EDU_ID DESC";

            return ExecuteQuery(query, new OracleParameter("PCNO", pcno.Trim()));
        }

        public static DataRow GetEducationRecordById(int eduId)
        {
            string query = @"
                SELECT * FROM EDU_EMP_EDUCATION WHERE EDU_ID = :ID";
            DataTable dt = ExecuteQuery(query, new OracleParameter("ID", eduId));
            return (dt != null && dt.Rows.Count > 0) ? dt.Rows[0] : null;
        }

        public static bool IsEmployeeSubmitted(string pcno)
        {
            // If employee has any records and ALL are submitted
            string query = @"
                SELECT COUNT(*) AS TOTAL,
                       SUM(CASE WHEN STATUS = 'Submitted' THEN 1 ELSE 0 END) AS SUBMITTED
                FROM EDU_EMP_EDUCATION
                WHERE PCNO = :PCNO";

            DataTable dt = ExecuteQuery(query, new OracleParameter("PCNO", pcno.Trim()));
            if (dt != null && dt.Rows.Count > 0)
            {
                int total = dt.Rows[0]["TOTAL"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["TOTAL"]) : 0;
                int submitted = dt.Rows[0]["SUBMITTED"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["SUBMITTED"]) : 0;
                return total > 0 && total == submitted;
            }
            return false;
        }

        public static int SaveEducationRecord(
            int eduId,
            string pcno,
            string empName,
            string desig,
            string division,
            string mainQualCode,
            string mainQualName,
            string subQualCode,
            string subQualName,
            int yearPassed,
            string instituteName,
            string collegeName,
            string specialization,
            string cgpaMarks,
            string gradeDivision,
            bool isHighest,
            string remarks,
            string status,
            string currentUser)
        {
            // If isHighest is set, clear highest flag on any other records for this employee
            if (isHighest)
            {
                string clearHighestSql = "UPDATE EDU_EMP_EDUCATION SET IS_HIGHEST = 0 WHERE PCNO = :PCNO";
                ExecuteNonQuery(clearHighestSql, new OracleParameter("PCNO", pcno.Trim()));
            }

            int highestVal = isHighest ? 1 : 0;

            if (string.Equals(status, "Submitted", StringComparison.OrdinalIgnoreCase))
            {
                if (IsPlaceholderOrInvalid(mainQualCode)) throw new ArgumentException("Main Qualification cannot be empty or placeholder ('*').");
                if (IsPlaceholderOrInvalid(subQualCode)) throw new ArgumentException("Sub Qualification / Stream cannot be empty or placeholder ('*').");
                if (yearPassed < 1950 || yearPassed > DateTime.Now.Year) throw new ArgumentException("Passing Year is invalid.");
                if (IsPlaceholderOrInvalid(instituteName)) throw new ArgumentException("University / Board cannot be empty or placeholder ('*').");
                if (IsPlaceholderOrInvalid(gradeDivision) || !IsValidGrade(gradeDivision)) throw new ArgumentException("Grade / Division cannot be empty or invalid. A valid Grade must be selected.");
                if (IsInvalidSpecialization(specialization)) throw new ArgumentException("Specialization cannot be empty or placeholder ('*'). ('N/A' is allowed).");
                if (IsPlaceholderOrInvalid(cgpaMarks)) throw new ArgumentException("Score (Percentage / CGPA / Marks) cannot be empty or placeholder ('*').");
            }

            gradeDivision = NormalizeGrade(gradeDivision);

            if (eduId > 0)
            {
                // Update
                string updateSql = @"
                    UPDATE EDU_EMP_EDUCATION SET
                        QUAL_MAIN_CODE = :MC,
                        QUAL_MAIN_NAME = :MN,
                        QUAL_SUB_CODE  = :SC,
                        QUAL_SUB_NAME  = :SN,
                        YEAR_PASSED    = :YP,
                        INSTITUTE_NAME = :INST,
                        COLLEGE_NAME   = :COLL,
                        SPECIALIZATION = :SPEC,
                        CGPA_MARKS     = :CGPA,
                        GRADE_DIVISION = :GRD,
                        IS_HIGHEST     = :HIGH,
                        REMARKS        = :REM,
                        STATUS         = :ST,
                        SUBMITTED_DATE = CASE WHEN :ST = 'Submitted' AND SUBMITTED_DATE IS NULL THEN SYSDATE ELSE SUBMITTED_DATE END,
                        MODIFIED_ON    = SYSDATE,
                        MODIFIED_BY    = :USR
                    WHERE EDU_ID = :ID";

                ExecuteNonQuery(updateSql,
                    new OracleParameter("MC", mainQualCode),
                    new OracleParameter("MN", mainQualName ?? ""),
                    new OracleParameter("SC", subQualCode),
                    new OracleParameter("SN", subQualName ?? ""),
                    new OracleParameter("YP", yearPassed),
                    new OracleParameter("INST", instituteName ?? ""),
                    new OracleParameter("COLL", collegeName ?? ""),
                    new OracleParameter("SPEC", specialization ?? ""),
                    new OracleParameter("CGPA", cgpaMarks ?? ""),
                    new OracleParameter("GRD", gradeDivision ?? ""),
                    new OracleParameter("HIGH", highestVal),
                    new OracleParameter("REM", remarks ?? ""),
                    new OracleParameter("ST", status),
                    new OracleParameter("USR", currentUser),
                    new OracleParameter("ID", eduId));

                LogAudit(pcno, eduId, "EditRecord", currentUser, "Updated education record " + eduId);
                return eduId;
            }
            else
            {
                // Insert
                string insertSql = @"
                    INSERT INTO EDU_EMP_EDUCATION (
                        PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
                        QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
                        YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
                        CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS,
                        SUBMITTED_DATE, CREATED_ON, CREATED_BY
                    ) VALUES (
                        :PCNO, :ENAME, :DESIG, :DIV,
                        :MC, :MN, :SC, :SN,
                        :YP, :INST, :COLL, :SPEC,
                        :CGPA, :GRD, :HIGH, :REM, :ST,
                        CASE WHEN :ST = 'Submitted' THEN SYSDATE ELSE NULL END,
                        SYSDATE, :USR
                    )";

                ExecuteNonQuery(insertSql,
                    new OracleParameter("PCNO", pcno.Trim()),
                    new OracleParameter("ENAME", empName ?? ""),
                    new OracleParameter("DESIG", desig ?? ""),
                    new OracleParameter("DIV", division ?? ""),
                    new OracleParameter("MC", mainQualCode),
                    new OracleParameter("MN", mainQualName ?? ""),
                    new OracleParameter("SC", subQualCode),
                    new OracleParameter("SN", subQualName ?? ""),
                    new OracleParameter("YP", yearPassed),
                    new OracleParameter("INST", instituteName ?? ""),
                    new OracleParameter("COLL", collegeName ?? ""),
                    new OracleParameter("SPEC", specialization ?? ""),
                    new OracleParameter("CGPA", cgpaMarks ?? ""),
                    new OracleParameter("GRD", gradeDivision ?? ""),
                    new OracleParameter("HIGH", highestVal),
                    new OracleParameter("REM", remarks ?? ""),
                    new OracleParameter("ST", status),
                    new OracleParameter("USR", currentUser));

                object maxIdObj = ExecuteScalar("SELECT MAX(EDU_ID) FROM EDU_EMP_EDUCATION WHERE PCNO = :PCNO", new OracleParameter("PCNO", pcno.Trim()));
                int newId = maxIdObj != null && maxIdObj != DBNull.Value ? Convert.ToInt32(maxIdObj) : 0;
                LogAudit(pcno, newId, "AddRecord", currentUser, "Added new education entry " + mainQualCode);
                return newId;
            }
        }

        public static bool DeleteEducationRecord(int eduId, string pcno, string currentUser, bool isAdmin)
        {
            string checkSql = "SELECT STATUS FROM EDU_EMP_EDUCATION WHERE EDU_ID = :ID";
            DataTable dt = ExecuteQuery(checkSql, new OracleParameter("ID", eduId));
            if (dt == null || dt.Rows.Count == 0) return false;

            string status = dt.Rows[0]["STATUS"].ToString();
            if (!isAdmin && status.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Submitted education records are locked and cannot be deleted by users.");
            }

            string deleteSql = "DELETE FROM EDU_EMP_EDUCATION WHERE EDU_ID = :ID";
            int affected = ExecuteNonQuery(deleteSql, new OracleParameter("ID", eduId));
            if (affected > 0)
            {
                LogAudit(pcno, eduId, "DeleteRecord", currentUser, "Deleted education entry " + eduId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether a given string value represents a dirty, placeholder, or unselected value
        /// such as '*', '-', 'NA', 'Not Specified', '?', etc. imported from legacy database.
        /// </summary>
        public static bool IsPlaceholderOrInvalid(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            string t = value.Trim();
            if (t == "*" || t == "**" || t == "***" ||
                t == "-" || t == "--" || t == "---" ||
                t == "." || t == ".." || t == "?" || t == "??" ||
                t.Equals("NA", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("N/A", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("NOT APPLICABLE", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("NOT SPECIFIED", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("NONE", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validates Specialization / Major Subject.
        /// Allows 'N/A', 'NA', 'Not Applicable', 'General', 'None', etc. as valid entries,
        /// especially for SSLC / 10th / Matriculation where no specialization exists.
        /// Only empty strings or actual placeholder symbols ('*', '?', etc.) are blocked.
        /// </summary>
        public static bool IsInvalidSpecialization(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            string t = value.Trim();
            if (t == "*" || t == "**" || t == "***" || t == "?" || t == "??")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Normalizes raw or legacy grade strings into the canonical, standardized Grade/Division names
        /// used in the application dropdowns (e.g. 'First Class with Distinction', 'First Class', 'Second Class', 'Pass Class', etc.).
        /// Returns empty string if missing, placeholder, or invalid.
        /// </summary>
        public static string NormalizeGrade(string rawGrade)
        {
            if (string.IsNullOrWhiteSpace(rawGrade) || IsPlaceholderOrInvalid(rawGrade))
                return "";

            string g = rawGrade.Trim().ToUpperInvariant();

            if (g.Contains("DISTINCTION") || g == "FCD" || g == "FIRST CLASS WITH DISTINCTION")
                return "First Class with Distinction";
            if (g.Contains("FIRST") || g == "1ST CLASS" || g == "1ST" || g == "FC" || g == "I CLASS" || g == "CLASS I")
                return "First Class";
            if (g.Contains("SECOND") || g == "2ND CLASS" || g == "2ND" || g == "SC" || g == "II CLASS" || g == "CLASS II")
                return "Second Class";
            if (g.Contains("PASS") || g == "PASSED" || g == "PC" || g.Contains("THIRD") || g == "3RD CLASS" || g == "3RD")
                return "Pass Class";
            if (g == "GRADE A+" || g == "A+")
                return "Grade A+";
            if (g == "GRADE A" || g == "A")
                return "Grade A";
            if (g == "GRADE B+" || g == "B+")
                return "Grade B+";
            if (g == "GRADE B" || g == "B")
                return "Grade B";
            if (g.Contains("EXEMPLARY"))
                return "Exemplary";

            return "";
        }

        /// <summary>
        /// Validates whether a Grade / Division string is one of the recognized, valid grades.
        /// </summary>
        public static bool IsValidGrade(string grade)
        {
            string norm = NormalizeGrade(grade);
            return !string.IsNullOrEmpty(norm);
        }

        /// <summary>
        /// Checks whether a given qualification is a Certificate, Apprenticeship, or Vocational training qualification
        /// which is typically issued by a certifying council/authority rather than an academic university.
        /// </summary>
        public static bool IsCertificateQualification(string qualCode, string qualName = "")
        {
            if (string.IsNullOrWhiteSpace(qualCode) && string.IsNullOrWhiteSpace(qualName))
                return false;

            string c = (qualCode ?? "").Trim().ToUpper();
            string n = (qualName ?? "").Trim().ToUpper();

            string[] certCodes = new string[] { "CC", "NAC", "NTC", "NCVT", "ITI", "ATC", "CERT" };
            if (Array.IndexOf(certCodes, c) >= 0) return true;

            if (n.Contains("CERTIFICATE") || n.Contains("APPRENTICE") || n.Contains("VOCATIONAL") || 
                n.Contains("TRADE") || n.Contains("TRAINING") || n.Contains("ITI") || n.Contains("NAC"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates all draft records for an employee to ensure none contain placeholder values (like '*')
        /// or unselected required fields before official submission.
        /// Returns a list of detailed human-readable validation error messages.
        /// </summary>
        public static System.Collections.Generic.List<string> ValidateDraftRecordsForSubmission(string pcno)
        {
            var errors = new System.Collections.Generic.List<string>();
            string query = @"
                SELECT EDU_ID, QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
                       YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
                       CGPA_MARKS, GRADE_DIVISION
                FROM EDU_EMP_EDUCATION
                WHERE PCNO = :PCNO AND STATUS = 'Draft'
                ORDER BY YEAR_PASSED ASC, EDU_ID ASC";

            DataTable dt = ExecuteQuery(query, new OracleParameter("PCNO", pcno.Trim()));
            if (dt == null || dt.Rows.Count == 0)
            {
                return errors;
            }

            int currentYear = DateTime.Now.Year;

            foreach (DataRow r in dt.Rows)
            {
                int eduId = Convert.ToInt32(r["EDU_ID"]);
                string mainCode = r["QUAL_MAIN_CODE"] != DBNull.Value ? r["QUAL_MAIN_CODE"].ToString() : "";
                string mainName = r["QUAL_MAIN_NAME"] != DBNull.Value ? r["QUAL_MAIN_NAME"].ToString() : "";
                string subCode = r["QUAL_SUB_CODE"] != DBNull.Value ? r["QUAL_SUB_CODE"].ToString() : "";
                string subName = r["QUAL_SUB_NAME"] != DBNull.Value ? r["QUAL_SUB_NAME"].ToString() : "";
                string inst = r["INSTITUTE_NAME"] != DBNull.Value ? r["INSTITUTE_NAME"].ToString() : "";
                string college = r["COLLEGE_NAME"] != DBNull.Value ? r["COLLEGE_NAME"].ToString() : "";
                string spec = r["SPECIALIZATION"] != DBNull.Value ? r["SPECIALIZATION"].ToString() : "";
                string cgpa = r["CGPA_MARKS"] != DBNull.Value ? r["CGPA_MARKS"].ToString() : "";
                string grade = r["GRADE_DIVISION"] != DBNull.Value ? r["GRADE_DIVISION"].ToString() : "";

                int yr = 0;
                if (r["YEAR_PASSED"] != DBNull.Value)
                {
                    int.TryParse(r["YEAR_PASSED"].ToString(), out yr);
                }

                var missingFields = new System.Collections.Generic.List<string>();

                if (IsPlaceholderOrInvalid(mainCode))
                {
                    missingFields.Add("Main Qualification");
                }
                if (IsPlaceholderOrInvalid(subCode) || IsPlaceholderOrInvalid(subName))
                {
                    missingFields.Add("Sub Qualification / Stream");
                }
                if (yr < 1950 || yr > currentYear)
                {
                    missingFields.Add("Passing Year (must be between 1950 and " + currentYear + ")");
                }
                bool isCert = IsCertificateQualification(mainCode, mainName);
                if (IsPlaceholderOrInvalid(inst))
                {
                    missingFields.Add(isCert ? "Certifying Body / Issuing Authority" : "University / Board / Institute");
                }
                if (IsPlaceholderOrInvalid(college))
                {
                    if (isCert)
                    {
                        college = "Direct Certification / Same as Issuing Authority";
                    }
                    else
                    {
                        missingFields.Add("College / School Name");
                    }
                }
                if (IsInvalidSpecialization(spec))
                {
                    missingFields.Add("Specialization / Major Subject");
                }
                if (IsPlaceholderOrInvalid(cgpa))
                {
                    missingFields.Add("Score (Percentage / CGPA / Marks)");
                }
                if (IsPlaceholderOrInvalid(grade) || !IsValidGrade(grade))
                {
                    missingFields.Add("Grade / Division (must select a valid grade from the list)");
                }

                if (missingFields.Count > 0)
                {
                    string qualLabel = !string.IsNullOrEmpty(mainName) ? mainName : (!string.IsNullOrEmpty(mainCode) ? mainCode : "Qualification #" + eduId);
                    errors.Add(string.Format("Record #{0} ({1}): Contains placeholder or unselected values in <strong>{2}</strong>.", 
                        eduId, qualLabel, string.Join(", ", missingFields)));
                }
            }

            return errors;
        }

        public static int SubmitAllEducationRecords(string pcno, string currentUser)
        {
            var validationErrors = ValidateDraftRecordsForSubmission(pcno);
            if (validationErrors.Count > 0)
            {
                throw new InvalidOperationException("Submission blocked: Some draft qualification records contain placeholder values (such as '*'). All required fields must have proper selections before submitting.");
            }

            string submitSql = @"
                UPDATE EDU_EMP_EDUCATION SET 
                    STATUS = 'Submitted', 
                    SUBMITTED_DATE = SYSDATE,
                    MODIFIED_ON = SYSDATE,
                    MODIFIED_BY = :USR
                WHERE PCNO = :PCNO AND STATUS = 'Draft'";

            int affected = ExecuteNonQuery(submitSql,
                new OracleParameter("USR", currentUser),
                new OracleParameter("PCNO", pcno.Trim()));

            if (affected > 0)
            {
                LogAudit(pcno, null, "FinalSubmit", currentUser, "Final submission of " + affected + " education records.");
            }
            return affected;
        }

        public static bool AdminRevertToDraft(int eduId, string adminPcno, string remarks)
        {
            string sql = @"
                UPDATE EDU_EMP_EDUCATION SET
                    STATUS = 'Draft',
                    SUBMITTED_DATE = NULL,
                    MODIFIED_ON = SYSDATE,
                    MODIFIED_BY = :ADMIN
                WHERE EDU_ID = :ID";

            int affected = ExecuteNonQuery(sql,
                new OracleParameter("ADMIN", adminPcno),
                new OracleParameter("ID", eduId));

            if (affected > 0)
            {
                object pcnoObj = ExecuteScalar("SELECT PCNO FROM EDU_EMP_EDUCATION WHERE EDU_ID = :ID", new OracleParameter("ID", eduId));
                string targetPcno = pcnoObj != null ? pcnoObj.ToString() : "";
                LogAudit(targetPcno, eduId, "AdminRevertToDraft", adminPcno, "Admin reverted record to Draft. Remarks: " + remarks);
                return true;
            }
            return false;
        }

        public static bool AdminRevertAllToDraftForEmployee(string pcno, string adminPcno, string remarks)
        {
            string sql = @"
                UPDATE EDU_EMP_EDUCATION SET
                    STATUS = 'Draft',
                    SUBMITTED_DATE = NULL,
                    MODIFIED_ON = SYSDATE,
                    MODIFIED_BY = :ADMIN
                WHERE PCNO = :PCNO";

            int affected = ExecuteNonQuery(sql,
                new OracleParameter("ADMIN", adminPcno),
                new OracleParameter("PCNO", pcno.Trim()));

            if (affected > 0)
            {
                LogAudit(pcno, null, "AdminRevertAllToDraft", adminPcno, "Admin unlocked all records to Draft for PCNO " + pcno + ". Remarks: " + remarks);
                return true;
            }
            return false;
        }

        public static bool AdminSubmitRecord(int eduId, string adminPcno, string remarks)
        {
            string sql = @"
                UPDATE EDU_EMP_EDUCATION SET
                    STATUS = 'Submitted',
                    SUBMITTED_DATE = SYSDATE,
                    MODIFIED_ON = SYSDATE,
                    MODIFIED_BY = :ADMIN
                WHERE EDU_ID = :ID";

            int affected = ExecuteNonQuery(sql,
                new OracleParameter("ADMIN", adminPcno),
                new OracleParameter("ID", eduId));

            if (affected > 0)
            {
                object pcnoObj = ExecuteScalar("SELECT PCNO FROM EDU_EMP_EDUCATION WHERE EDU_ID = :ID", new OracleParameter("ID", eduId));
                string targetPcno = pcnoObj != null ? pcnoObj.ToString() : "";
                LogAudit(targetPcno, eduId, "AdminSubmitRecord", adminPcno, "Admin marked record as Submitted. Remarks: " + remarks);
                return true;
            }
            return false;
        }

        public static bool AdminSubmitAllForEmployee(string pcno, string adminPcno, string remarks)
        {
            string sql = @"
                UPDATE EDU_EMP_EDUCATION SET
                    STATUS = 'Submitted',
                    SUBMITTED_DATE = SYSDATE,
                    MODIFIED_ON = SYSDATE,
                    MODIFIED_BY = :ADMIN
                WHERE PCNO = :PCNO";

            int affected = ExecuteNonQuery(sql,
                new OracleParameter("ADMIN", adminPcno),
                new OracleParameter("PCNO", pcno.Trim()));

            if (affected > 0)
            {
                LogAudit(pcno, null, "AdminSubmitAll", adminPcno, "Admin marked all records as Submitted for PCNO " + pcno + ". Remarks: " + remarks);
                return true;
            }
            return false;
        }

        #endregion

        #region Admin Overview & Master Data

        public static DataTable GetAdminEducationOverview(string search, string statusFilter, string qualFilter, string divisionFilter, string highestFilter = "ALL")
        {
            string query = @"
                SELECT e.EDU_ID, e.PCNO, e.EMP_NAME, e.DESIGNATION, e.DIVISION_NAME,
                       e.QUAL_MAIN_CODE, e.QUAL_MAIN_NAME, e.QUAL_SUB_CODE,
                       COALESCE(
                           (SELECT qs.SUB_NAME FROM EDU_QUAL_SUB qs 
                            WHERE qs.SUB_CODE = e.QUAL_SUB_CODE 
                              AND (qs.MAIN_QUAL_CODE = e.QUAL_MAIN_CODE OR qs.MAIN_QUAL_CODE IS NULL)
                              AND ROWNUM = 1),
                           e.QUAL_SUB_NAME,
                           e.QUAL_SUB_CODE
                       ) AS QUAL_SUB_NAME,
                       e.YEAR_PASSED, e.INSTITUTE_NAME, e.COLLEGE_NAME, e.SPECIALIZATION,
                       e.CGPA_MARKS, e.GRADE_DIVISION, e.IS_HIGHEST, e.REMARKS, e.STATUS,
                       e.SUBMITTED_DATE, e.CREATED_ON, e.IS_MIGRATED
                FROM EDU_EMP_EDUCATION e
                WHERE 1 = 1";

            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND (UPPER(PCNO) LIKE :SRCH OR UPPER(EMP_NAME) LIKE :SRCH OR UPPER(SPECIALIZATION) LIKE :SRCH OR UPPER(INSTITUTE_NAME) LIKE :SRCH)";
                parameters.Add(new OracleParameter("SRCH", "%" + search.Trim().ToUpper() + "%"));
            }

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "ALL")
            {
                query += " AND STATUS = :ST";
                parameters.Add(new OracleParameter("ST", statusFilter));
            }

            if (!string.IsNullOrEmpty(qualFilter) && qualFilter != "ALL")
            {
                query += " AND QUAL_MAIN_CODE = :QC";
                parameters.Add(new OracleParameter("QC", qualFilter));
            }

            if (!string.IsNullOrEmpty(divisionFilter) && divisionFilter != "ALL")
            {
                query += " AND UPPER(DIVISION_NAME) LIKE :DIV";
                parameters.Add(new OracleParameter("DIV", "%" + divisionFilter.Trim().ToUpper() + "%"));
            }

            if (!string.IsNullOrEmpty(highestFilter) && highestFilter != "ALL")
            {
                if (highestFilter == "1" || highestFilter.Equals("YES", StringComparison.OrdinalIgnoreCase))
                {
                    query += " AND IS_HIGHEST = 1";
                }
                else if (highestFilter == "0" || highestFilter.Equals("NO", StringComparison.OrdinalIgnoreCase))
                {
                    query += " AND (IS_HIGHEST = 0 OR IS_HIGHEST IS NULL)";
                }
            }

            query += " ORDER BY PCNO ASC, IS_HIGHEST DESC, YEAR_PASSED DESC";

            return ExecuteQuery(query, parameters.ToArray());
        }

        public static Dictionary<string, int> GetDashboardStats()
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();
            stats["TotalEmployees"] = 0;
            stats["TotalRecords"] = 0;
            stats["SubmittedCount"] = 0;
            stats["DraftCount"] = 0;
            stats["MigratedCount"] = 0;

            try
            {
                string query = @"
                    SELECT 
                        COUNT(DISTINCT PCNO) AS EMP_COUNT,
                        COUNT(*) AS RECORD_COUNT,
                        SUM(CASE WHEN STATUS = 'Submitted' THEN 1 ELSE 0 END) AS SUBMITTED,
                        SUM(CASE WHEN STATUS = 'Draft' THEN 1 ELSE 0 END) AS DRAFT,
                        SUM(CASE WHEN IS_MIGRATED = 1 THEN 1 ELSE 0 END) AS MIGRATED
                    FROM EDU_EMP_EDUCATION";

                DataTable dt = ExecuteQuery(query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    stats["TotalEmployees"] = r["EMP_COUNT"] != DBNull.Value ? Convert.ToInt32(r["EMP_COUNT"]) : 0;
                    stats["TotalRecords"] = r["RECORD_COUNT"] != DBNull.Value ? Convert.ToInt32(r["RECORD_COUNT"]) : 0;
                    stats["SubmittedCount"] = r["SUBMITTED"] != DBNull.Value ? Convert.ToInt32(r["SUBMITTED"]) : 0;
                    stats["DraftCount"] = r["DRAFT"] != DBNull.Value ? Convert.ToInt32(r["DRAFT"]) : 0;
                    stats["MigratedCount"] = r["MIGRATED"] != DBNull.Value ? Convert.ToInt32(r["MIGRATED"]) : 0;
                }
            }
            catch { }

            return stats;
        }

        public static DataTable GetMainQualifications(bool activeOnly = true)
        {
            string query = "SELECT QUAL_CODE, QUAL_NAME, QUAL_CATEGORY, (QUAL_CODE || ' - ' || QUAL_NAME) AS DISPLAY_NAME FROM EDU_QUAL_MAIN";
            if (activeOnly) query += " WHERE IS_ACTIVE = 1";
            query += " ORDER BY QUAL_CATEGORY ASC, QUAL_NAME ASC";
            return ExecuteQuery(query);
        }

        public static DataTable GetSubQualifications(string mainQualCode = null, bool activeOnly = true)
        {
            string query = "SELECT SUB_ID, MAIN_QUAL_CODE, SUB_CODE, SUB_NAME FROM EDU_QUAL_SUB WHERE 1 = 1";
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (activeOnly)
            {
                query += " AND IS_ACTIVE = 1";
            }

            if (!string.IsNullOrEmpty(mainQualCode))
            {
                query += " AND (MAIN_QUAL_CODE = :MQ OR MAIN_QUAL_CODE IS NULL)";
                parameters.Add(new OracleParameter("MQ", mainQualCode));
            }

            query += " ORDER BY SUB_NAME ASC";
            return ExecuteQuery(query, parameters.ToArray());
        }

        private static string _cachedSubQualJson = null;

        public static string GetSubQualificationsJson()
        {
            if (_cachedSubQualJson != null) return _cachedSubQualJson;

            DataTable dtSub = GetSubQualifications(null, true);
            var subList = new System.Collections.Generic.List<object>();
            var seen = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (dtSub != null)
            {
                foreach (DataRow r in dtSub.Rows)
                {
                    string subCode = r["SUB_CODE"].ToString().Trim();
                    string subName = r["SUB_NAME"].ToString().Trim();
                    string mainCode = r["MAIN_QUAL_CODE"] != DBNull.Value ? r["MAIN_QUAL_CODE"].ToString().Trim() : "";
                    string key = mainCode + "|" + subCode;
                    if (seen.Contains(key)) continue;
                    seen.Add(key);

                    subList.Add(new { code = subCode, name = subName, main = mainCode });
                }
            }
            if (!seen.Contains("|GENERAL"))
            {
                subList.Add(new { code = "GENERAL", name = "General", main = "" });
            }
            if (!seen.Contains("|OTHER"))
            {
                subList.Add(new { code = "OTHER", name = "Other (Specify in Specialization)", main = "" });
            }

            _cachedSubQualJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(subList);
            return _cachedSubQualJson;
        }

        public static void InvalidateSubQualificationsCache()
        {
            _cachedSubQualJson = null;
        }

        public static string GetSubQualificationName(string mainQualCode, string subQualCode)
        {
            if (string.IsNullOrEmpty(subQualCode)) return string.Empty;
            try
            {
                string query = "SELECT SUB_NAME FROM EDU_QUAL_SUB WHERE SUB_CODE = :SC";
                List<OracleParameter> pars = new List<OracleParameter> { new OracleParameter("SC", subQualCode) };
                if (!string.IsNullOrEmpty(mainQualCode))
                {
                    query += " AND (MAIN_QUAL_CODE = :MQ OR MAIN_QUAL_CODE IS NULL) ORDER BY CASE WHEN MAIN_QUAL_CODE = :MQ THEN 0 ELSE 1 END";
                    pars.Add(new OracleParameter("MQ", mainQualCode));
                }
                object obj = ExecuteScalar(query, pars.ToArray());
                if (obj != null && obj != DBNull.Value)
                {
                    return obj.ToString().Trim();
                }
            }
            catch { }
            return string.Empty;
        }

        public static DataTable GetUniversities(bool activeOnly = true)
        {
            // Consolidate duplicates by UNIV_NAME using MIN(UNIV_ID) and filter standalone redundant abbreviations
            string query = "SELECT MIN(UNIV_ID) AS UNIV_ID, UNIV_NAME, MAX(STATE_OR_TYPE) AS STATE_OR_TYPE FROM EDU_UNIVERSITY";
            if (activeOnly)
            {
                query += " WHERE IS_ACTIVE = 1 AND UPPER(TRIM(UNIV_NAME)) NOT IN ('VTU', 'JNTU', 'IISC', 'SEBA', 'AHSEC', 'WBBSE', 'WBCHSE', 'ISC', 'CSBE', 'D.U', '*')";
            }
            query += " GROUP BY UNIV_NAME ORDER BY UNIV_NAME ASC";
            return ExecuteQuery(query);
        }

        public static DataTable GetCollegesMaster(bool activeOnly = false)
        {
            string query = "SELECT COLLEGE_ID, COLLEGE_NAME, COLLEGE_TYPE, LOCATION, AFFILIATED_UNIV, IS_ACTIVE FROM EDU_COLLEGE";
            if (activeOnly) query += " WHERE IS_ACTIVE = 1";
            query += " ORDER BY COLLEGE_NAME ASC";
            try
            {
                return ExecuteQuery(query);
            }
            catch
            {
                return new DataTable();
            }
        }

        public static DataTable GetColleges()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("COLLEGE_NAME", typeof(string));

            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1. Fetch colleges directly from official EDU_COLLEGE master table
            try
            {
                string sqlCol = "SELECT COLLEGE_NAME FROM EDU_COLLEGE WHERE IS_ACTIVE = 1";
                DataTable dtCol = ExecuteQuery(sqlCol);
                if (dtCol != null && dtCol.Rows.Count > 0)
                {
                    foreach (DataRow r in dtCol.Rows)
                    {
                        string name = r["COLLEGE_NAME"].ToString().Trim();
                        if (!string.IsNullOrEmpty(name) && !IsPlaceholderOrInvalid(name) && 
                            !name.Equals("OTHER", StringComparison.OrdinalIgnoreCase) && 
                            !name.StartsWith("Other /", StringComparison.OrdinalIgnoreCase) && 
                            seen.Add(name))
                        {
                            dt.Rows.Add(name);
                        }
                    }
                }
            }
            catch { }

            // 2. Standard and prominent catalog (fallback & baseline institutions)
            string[] standardColleges = new string[]
            {
                // Premier Engineering Colleges (Karnataka & India)
                "Mangalore Institute of Technology and Engineering (MITE), Moodabidri",
                "RV College of Engineering (RVCE), Bengaluru",
                "BMS College of Engineering (BMSCE), Bengaluru",
                "M.S. Ramaiah Institute of Technology (MSRIT), Bengaluru",
                "PES Institute of Technology / PES University, Bengaluru",
                "Bangalore Institute of Technology (BIT), Bengaluru",
                "Dayananda Sagar College of Engineering (DSCE), Bengaluru",
                "Sir M. Visvesvaraya Institute of Technology (SMVIT), Bengaluru",
                "Siddaganga Institute of Technology (SIT), Tumakuru",
                "The National Institute of Engineering (NIE), Mysuru",
                "Sri Jayachamarajendra College of Engineering (SJCE), Mysuru",
                "NMAM Institute of Technology (NMAMIT), Nitte",
                "Canara Engineering College (CEC), Benjanapadavu, Mangaluru",
                "St. Joseph Engineering College (SJEC), Vamanjoor, Mangaluru",
                "Sahyadri College of Engineering & Management, Adyar, Mangaluru",
                "P.A. College of Engineering (PACE), Mangaluru",
                "Srinivas Institute of Technology (SIT), Valachil, Mangaluru",
                "Bearys Institute of Technology (BIT), Mangaluru",
                "Alva's Institute of Engineering & Technology (AIET), Moodabidri",
                "Shri Madhwa Vadiraja Institute of Technology & Management (SMVITM), Bantakal",
                "B.N.M. Institute of Technology (BNMIT), Bengaluru",
                "RNS Institute of Technology (RNSIT), Bengaluru",
                "JSS Academy of Technical Education (JSSATE), Bengaluru",
                "New Horizon College of Engineering (NHCE), Bengaluru",
                "CMR Institute of Technology (CMRIT), Bengaluru",
                "The Oxford College of Engineering, Bengaluru",
                "Nitte Meenakshi Institute of Technology (NMIT), Bengaluru",
                "KLE Technological University / BVB College of Engineering, Hubballi",
                "Basaveshwar Engineering College (BEC), Bagalkot",
                "SDM College of Engineering and Technology (SDMCET), Dharwad",
                "Malnad College of Engineering (MCE), Hassan",
                "Bapuji Institute of Engineering and Technology (BIET), Davangere",
                "University Visvesvaraya College of Engineering (UVCE), Bengaluru",
                "College of Engineering, Pune (COEP)",
                "Veermata Jijabai Technological Institute (VJTI), Mumbai",
                "College of Engineering, Guindy (CEG), Anna University, Chennai",
                "PSG College of Technology, Coimbatore",
                "Thapar Institute of Engineering and Technology, Patiala",
                "Birla Institute of Technology (BIT), Mesra, Ranchi",
                "Manipal Institute of Technology (MIT), Manipal",

                // Seshadripuram Educational Trust Institutions
                "Seshadripuram College, Bengaluru",
                "Seshadripuram First Grade College (SFGC), Yelahanka, Bengaluru",
                "Seshadripuram Institute of Commerce and Management (SICM), Bengaluru",
                "Seshadripuram Academy of Business Studies (SABS), Kengeri, Bengaluru",
                "Seshadripuram Evening College, Bengaluru",
                "Seshadripuram Pre-University College, Bengaluru",

                // Degree, Science, Arts & Commerce Colleges
                "St. Aloysius College (Autonomous), Mangaluru",
                "St. Agnes College (Autonomous), Mangaluru",
                "Canara College, Mangaluru",
                "SDM College (Autonomous), Ujire",
                "Bhandarkars' Arts & Science College, Kundapura",
                "Poornaprajna College (PPC), Udupi",
                "Mahatma Gandhi Memorial College (MGM), Udupi",
                "St. Joseph's University / College, Bengaluru",
                "Mount Carmel College (MCC), Bengaluru",
                "National College, Basavanagudi, Bengaluru",
                "MES College of Arts, Commerce and Science, Bengaluru",
                "MES Degree College of Arts, Commerce and Science, Malleshwaram, Bengaluru",
                "Vijaya College, Basavanagudi, Bengaluru",
                "BMS College of Commerce & Management, Bengaluru",
                "Kristu Jayanti College (Autonomous), Bengaluru",
                "Presidency College, Hebbal, Bengaluru",
                "Jyoti Nivas College (Autonomous), Bengaluru",
                "Acharya Bangalore B-School (ABBS), Bengaluru",
                "KLE Society's S. Nijalingappa College, Rajajinagar, Bengaluru",
                "The Oxford College of Science / Arts / Commerce, Bengaluru",
                "St. Xavier's College, Mumbai",
                "Loyola College, Chennai",
                "Madras Christian College (MCC), Chennai",
                "Fergusson College, Pune",
                "St. Stephen's College, Delhi",
                "Hindu College, University of Delhi",
                "Hansraj College, University of Delhi",
                "Shri Ram College of Commerce (SRCC), Delhi",
                "Presidency College, Kolkata",

                // Pre-University (PU) Colleges & Higher Secondary Schools
                "Canara Pre-University College, Mangaluru",
                "St. Aloysius Pre-University College, Mangaluru",
                "Expert Pre-University College, Mangaluru",
                "Sharada Pre-University College, Mangaluru",
                "Alva's Pre-University College, Moodabidri",
                "National Pre-University College, Bengaluru",
                "MES Pre-University College, Bengaluru",
                "Christ Junior College, Bengaluru",
                "Mahesh PU College, Mangaluru",
                "Government Pre-University College (State PU Board)",
                "Kendriya Vidyalaya (KV)",
                "Jawahar Navodaya Vidyalaya (JNV)",
                "Delhi Public School (DPS)",
                "DAV Public School",
                "Army Public School",
                "National Public School (NPS), Bengaluru",
                "Bishop Cotton Boys' / Girls' School, Bengaluru",
                "St. Joseph's Boys' High School, Bengaluru",

                // Polytechnic & Technical Institutes
                "Karnataka (Govt) Polytechnic (KPT), Mangaluru",
                "Government Polytechnic, Bengaluru",
                "S.J. Polytechnic, Bengaluru",
                "PES Polytechnic, Bengaluru",
                "Govt ITI College / Industrial Training Institute"
            };

            foreach (var c in standardColleges)
            {
                if (seen.Add(c))
                {
                    dt.Rows.Add(c);
                }
            }

            DataView dv = dt.DefaultView;
            dv.Sort = "COLLEGE_NAME ASC";
            return dv.ToTable();
        }

        public static DataTable GetSubjects(bool activeOnly = true)
        {
            string query = "SELECT SUBJECT_ID, SUBJECT_NAME, SUBJECT_GROUP FROM EDU_SUBJECT";
            if (activeOnly) query += " WHERE IS_ACTIVE = 1";
            query += " ORDER BY SUBJECT_NAME ASC";
            return ExecuteQuery(query);
        }

        public static bool AddMainQualification(string code, string name, string category)
        {
            string sql = "INSERT INTO EDU_QUAL_MAIN (QUAL_CODE, QUAL_NAME, QUAL_CATEGORY, IS_ACTIVE) VALUES (:C, :N, :CAT, 1)";
            return ExecuteNonQuery(sql,
                new OracleParameter("C", code.Trim().ToUpper()),
                new OracleParameter("N", name.Trim()),
                new OracleParameter("CAT", category.Trim())) > 0;
        }

        public static bool UpdateMainQualification(string code, string name, string category)
        {
            string sql = "UPDATE EDU_QUAL_MAIN SET QUAL_NAME = :N, QUAL_CATEGORY = :CAT WHERE QUAL_CODE = :C";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", name.Trim()),
                new OracleParameter("CAT", category.Trim()),
                new OracleParameter("C", code.Trim().ToUpper())) > 0;
        }

        public static bool DeleteMainQualification(string code)
        {
            // Unbind child sub-qualifications so they are not orphaned
            ExecuteNonQuery("UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = NULL WHERE MAIN_QUAL_CODE = :C",
                new OracleParameter("C", code.Trim().ToUpper()));

            string sql = "DELETE FROM EDU_QUAL_MAIN WHERE QUAL_CODE = :C";
            return ExecuteNonQuery(sql,
                new OracleParameter("C", code.Trim().ToUpper())) > 0;
        }

        public static bool AddSubQualification(string mainCode, string subCode, string subName)
        {
            string sql = "INSERT INTO EDU_QUAL_SUB (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE) VALUES (:MQ, :SC, :SN, 1)";
            return ExecuteNonQuery(sql,
                new OracleParameter("MQ", string.IsNullOrEmpty(mainCode) ? (object)DBNull.Value : mainCode.Trim().ToUpper()),
                new OracleParameter("SC", subCode.Trim().ToUpper()),
                new OracleParameter("SN", subName.Trim())) > 0;
        }

        public static bool UpdateSubQualification(int subId, string mainCode, string subCode, string subName)
        {
            string sql = "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = :MQ, SUB_CODE = :SC, SUB_NAME = :SN WHERE SUB_ID = :ID";
            return ExecuteNonQuery(sql,
                new OracleParameter("MQ", string.IsNullOrEmpty(mainCode) ? (object)DBNull.Value : mainCode.Trim().ToUpper()),
                new OracleParameter("SC", subCode.Trim().ToUpper()),
                new OracleParameter("SN", subName.Trim()),
                new OracleParameter("ID", subId)) > 0;
        }

        public static bool DeleteSubQualification(int subId)
        {
            string sql = "DELETE FROM EDU_QUAL_SUB WHERE SUB_ID = :ID";
            return ExecuteNonQuery(sql,
                new OracleParameter("ID", subId)) > 0;
        }

        
        public static string EnsureUniversityExists(string name, string stateOrType = "Recognized Institution")
        {
            if (string.IsNullOrWhiteSpace(name) || IsPlaceholderOrInvalid(name))
                return name;

            string normalized = NormalizeUniversityName(name.Trim());

            try
            {
                // Check if exists in EDU_UNIVERSITY (case-insensitive)
                string checkSql = "SELECT UNIV_NAME FROM EDU_UNIVERSITY WHERE UPPER(TRIM(UNIV_NAME)) = :N AND ROWNUM = 1";
                object res = ExecuteScalar(checkSql, new OracleParameter("N", normalized.ToUpper()));
                if (res != null && res != DBNull.Value && !string.IsNullOrWhiteSpace(res.ToString()))
                {
                    return res.ToString();
                }

                // Insert into EDU_UNIVERSITY so it is permanently and globally available for all users
                AddUniversity(normalized, stateOrType);
                return normalized;
            }
            catch
            {
                return normalized;
            }
        }

        public static string NormalizeUniversityName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;
            string s = name.Trim();
            string upper = s.ToUpper();

            if (upper == "VTU" || upper.Contains("VISVESVARAYA TECHNOLOGICAL") || upper.Contains("VISWESWARAIAH"))
                return "Visvesvaraya Technological University (VTU), Belagavi";
            if (upper == "IISC" || upper == "IISC, BANGALORE" || upper == "IISC BANGALORE")
                return "Indian Institute of Science (IISc), Bangalore";
            if (upper == "JNTU" || (upper.StartsWith("JAWAHARLAL NEHRU TECHNOLOGICAL UNIVERSITY") && upper.Contains("HYDERABAD")))
                return "Jawaharlal Nehru Technological University (JNTU), Hyderabad";
            if (upper == "IGNOU" || (upper.Contains("IGNOU") && upper.Contains("DELHI")))
                return "Indira Gandhi National Open University (IGNOU), New Delhi";
            if (upper == "CBSE" || upper == "CBSE BOARD" || upper == "CBSE BOARD NEW DELHI" || upper == "CSBE")
                return "Central Board of Secondary Education (CBSE), New Delhi";
            if (upper == "ICSE" || upper == "ISC" || upper == "ICSE BOARD" || upper == "ISCE BOARD" || upper == "ICSE / ISC BOARD")
                return "Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)";
            if (upper == "PUE" || upper == "PRE UNIVERSITY BOARD")
                return "Department of Pre-University Education (PUE), Karnataka";
            if (upper == "REC, SURATKAL" || upper == "KREC SURATHKAL" || upper == "NIT, SURATHKAL" || upper == "NIT SURATHKAL")
                return "National Institute of Technology Karnataka (NITK / REC), Surathkal";
            if (upper == "REC, WARANGAL" || upper == "NIT, WARANGAL" || upper == "NIT WARANGAL")
                return "National Institute of Technology (NIT / REC), Warangal";
            if (upper == "REC, TIRUCHY" || upper == "NIT TRICHY" || upper == "NIT, TIRUCHIRAPPALLI")
                return "National Institute of Technology (NIT / REC), Tiruchirappalli";
            if (upper == "JNU,DELHI" || upper == "JNU")
                return "Jawaharlal Nehru University (JNU), New Delhi";
            if (upper == "ANNA UNIVERSITY")
                return "Anna University, Chennai";
            if (upper == "BANGALORE UNIVERSITY")
                return "Bangalore University, Bengaluru";

            return s;
        }

        private static Dictionary<int, string> _legacyUnivMap;
        public static Dictionary<int, string> GetLegacyUniversityMap()
        {
            if (_legacyUnivMap == null)
            {
                var legacyMap = new Dictionary<int, string>();
            legacyMap[0] = "Other Recognized University / Board";
            legacyMap[1] = "Anna University, Chennai";
            legacyMap[2] = "Bangalore University, Bengaluru";
            legacyMap[3] = "Andhra University, Visakhapatnam";
            legacyMap[4] = "Indian Institute of Technology (IIT), Kanpur";
            legacyMap[5] = "Indian Institute of Technology (IIT), Madras";
            legacyMap[6] = "Indian Institute of Technology (IIT), Delhi";
            legacyMap[7] = "Indian Institute of Science (IISc), Bangalore";
            legacyMap[8] = "Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow";
            legacyMap[9] = "Indian Institute of Technology (IIT), Guwahati";
            legacyMap[10] = "Indian Institute of Technology (IIT), Roorkee";
            legacyMap[11] = "Banaras Hindu University (BHU), Varanasi";
            legacyMap[12] = "Birla Institute of Technology and Science (BITS), Pilani";
            legacyMap[13] = "Indian Institute of Technology (IIT), Kharagpur";
            legacyMap[14] = "National Institute of Technology (NIT / REC), Warangal";
            legacyMap[15] = "National Institute of Technology (NIT / REC), Tiruchirappalli";
            legacyMap[16] = "National Institute of Technology Karnataka (NITK / REC), Surathkal";
            legacyMap[17] = "National Institute of Technology (NIT / REC), Calicut";
            legacyMap[18] = "National Institute of Technology (NIT / REC), Silchar";
            legacyMap[19] = "Malaviya National Institute of Technology (MNIT), Jaipur";
            legacyMap[20] = "Karnatak University, Dharwad";
            legacyMap[21] = "University of Mysore, Mysuru";
            legacyMap[22] = "Mangalore University, Konaje";
            legacyMap[23] = "Kuvempu University, Shankaraghatta, Shivamogga";
            legacyMap[24] = "Annamalai University";
            legacyMap[25] = "Jawaharlal Nehru Technological University Kakinada (JNTUK)";
            legacyMap[26] = "Jawaharlal Nehru Technological University Anantapur (JNTUA)";
            legacyMap[27] = "Acharya Nagarjuna University, Guntur";
            legacyMap[28] = "Osmania University, Hyderabad";
            legacyMap[29] = "Sri Venkateswara University";
            legacyMap[30] = "University of Hyderabad (UoH / Central University), Hyderabad";
            legacyMap[31] = "Bharathiar University";
            legacyMap[32] = "Bharathidasan University";
            legacyMap[33] = "University of Madras, Chennai";
            legacyMap[34] = "Rajasthan University";
            legacyMap[35] = "Kakatiya University";
            legacyMap[36] = "Manonmani Sundaranathan University";
            legacyMap[37] = "Sastra University";
            legacyMap[38] = "Cochin University of Science and Technology (CUSAT), Kochi";
            legacyMap[39] = "Vikram University, Ujjain";
            legacyMap[40] = "Maulana Azad National Institute of Technology (MANIT), Bhopal";
            legacyMap[41] = "Jainarayan Vayas University, Jodhpur";
            legacyMap[42] = "Gulbarga University, Gulbarga";
            legacyMap[43] = "Madurai Kamaraj University (MKU), Madurai";
            legacyMap[44] = "Jawaharlal Nehru University (JNU), New Delhi";
            legacyMap[45] = "University of Delhi (DU), New Delhi";
            legacyMap[46] = "Pondicherry University";
            legacyMap[47] = "U BTE Roorkee";
            legacyMap[48] = "University of Kerala, Thiruvananthapuram";
            legacyMap[49] = "National Institute of Technology (NIT / REC), Hamirpur";
            legacyMap[50] = "Mahatma Gandhi University, Kerala";
            legacyMap[51] = "Sardar Vallabhbhai National Institute of Technology (SVNIT), Surat";
            legacyMap[52] = "Gurunanak Dev University, Punjab";
            legacyMap[53] = "Madurai Kamaraj University (MKU), Madurai";
            legacyMap[54] = "RD University";
            legacyMap[55] = "Rajiv Gandhi Proudyogiki Vishwavidyalaya (RGPV), Bhopal";
            legacyMap[56] = "Indian Institute of Technology (IIT), Bombay";
            legacyMap[57] = "BIT Mesra Ranchi";
            legacyMap[58] = "IAT,Pune";
            legacyMap[59] = "Jawaharlal Nehru University (JNU), New Delhi";
            legacyMap[60] = "Avadh University";
            legacyMap[61] = "Utkal University, Bhubaneswar";
            legacyMap[62] = "Mahatma Gandhi University, Kerala";
            legacyMap[63] = "Geherwal University";
            legacyMap[64] = "Allahabad University";
            legacyMap[65] = "University of Calicut, Malappuram";
            legacyMap[66] = "IETE, New Delhi";
            legacyMap[67] = "Institution of Engineers, New Delhi";
            legacyMap[68] = "Motilal Nehru National Institute of Technology (MNNIT), Allahabad";
            legacyMap[69] = "MNB Garwal University";
            legacyMap[70] = "MERRUT University";
            legacyMap[71] = "Savitribai Phule Pune University (SPPU / University of Pune)";
            legacyMap[72] = "University of Calicut, Malappuram";
            legacyMap[73] = "University of Calcutta (Calcutta University), Kolkata";
            legacyMap[74] = "Deen Dayal Upadhyaya Gorakhpur University (DDU), Gorakhpur";
            legacyMap[75] = "National Institute of Technology Karnataka (NITK / REC), Surathkal";
            legacyMap[76] = "Dr.H.S.Gaur University, Sagar";
            legacyMap[77] = "Barkatulla University, Bhopal";
            legacyMap[78] = "University of Lucknow, Lucknow";
            legacyMap[79] = "Kumoan University, Nainital";
            legacyMap[80] = "BITS Ranchi";
            legacyMap[81] = "Bundel Khand University";
            legacyMap[82] = "Dayalbagh University";
            legacyMap[83] = "REC, Bhalki";
            legacyMap[84] = "Aligarh Muslim University (AMU), Aligarh";
            legacyMap[85] = "Ranchi University";
            legacyMap[86] = "National Institute of Technology (NIT / REC), Rourkela";
            legacyMap[87] = "Sambalpur University, Orissa";
            legacyMap[88] = "AIMA,New Delhi";
            legacyMap[89] = "Indira Gandhi National Open University (IGNOU), New Delhi";
            legacyMap[90] = "MIT,Madras";
            legacyMap[91] = "Deen Dayal Upadhyaya Gorakhpur University (DDU), Gorakhpur";
            legacyMap[92] = "Jabalpur University";
            legacyMap[93] = "Banaras Hindu University (BHU), Varanasi";
            legacyMap[94] = "Burdwan University";
            legacyMap[95] = "National Institute of Technology (NIT / REC), Durgapur";
            legacyMap[96] = "Mahatma Jyotiba Phule Rohilkhand University (MJPRU), Bareilly";
            legacyMap[97] = "Maharshi Dayanand University, Rohtak";
            legacyMap[98] = "Gujarat University";
            legacyMap[99] = "B.S.B.A University, Muz,Bihar";
            legacyMap[100] = "Mahatma Jyotiba Phule Rohilkhand University (MJPRU), Bareilly";
            legacyMap[101] = "South Gujarat University";
            legacyMap[102] = "Dr. B. R. Ambedkar National Institute of Technology (NIT), Jalandhar";
            legacyMap[103] = "Jamia Millia Islamia (JMI), New Delhi";
            legacyMap[104] = "Amaravati University";
            legacyMap[105] = "Marthwada University";
            legacyMap[106] = "Centre for Electronics Design Technology";
            legacyMap[107] = "MANIPUR UNIVERSITY";
            legacyMap[108] = "Army University";
            legacyMap[109] = "Panjab University, Chandigarh";
            legacyMap[110] = "IAI";
            legacyMap[111] = "Bhopal University";
            legacyMap[112] = "Punjab Technical University";
            legacyMap[113] = "Dr.Ambedkar University";
            legacyMap[114] = "Motilal Nehru National Institute of Technology (MNNIT), Allahabad";
            legacyMap[115] = "M.B.T.I Kanpur";
            legacyMap[116] = "I.E.T, C.S.J.M University Kanpur";
            legacyMap[117] = "D.A.V.V, Indore";
            legacyMap[118] = "Meghalaya Board of School Education";
            legacyMap[119] = "North-Eastern Hill University (NEHU), Shillong";
            legacyMap[120] = "Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)";
            legacyMap[121] = "Assam University";
            legacyMap[122] = "Institution of Engineers(India), Kolkatta";
            legacyMap[123] = "Govind Ballabh Pant University of Agriculture and Technology";
            legacyMap[124] = "University of Kerala, Thiruvananthapuram";
            legacyMap[125] = "Patna University, Patna";
            legacyMap[126] = "DOEACC Centre";
            legacyMap[127] = "NIIT Ltd.";
            legacyMap[128] = "VIDYASAGAR UNIVERSITY";
            legacyMap[129] = "Visvesvaraya Technological University (VTU), Belagavi";
            legacyMap[130] = "University of Kalyani";
            legacyMap[131] = "Bengal Engineering College, Shibpore";
            legacyMap[132] = "Pt. RaviShankar University, Raipur";
            legacyMap[133] = "Malaviya National Institute of Technology (MNIT), Jaipur";
            legacyMap[134] = "Sri KrishnaDevaraya University";
            legacyMap[135] = "Nagpur University";
            legacyMap[136] = "Kannur University";
            legacyMap[137] = "V B University, Hazaribagh";
            legacyMap[138] = "Sardar Vallabhbhai National Institute of Technology (SVNIT), Surat";
            legacyMap[139] = "IIMM,Bangalore";
            legacyMap[140] = "National Institute of Technology (NIT), Jamshedpur";
            legacyMap[141] = "National Institute of Technology (NIT / REC), Durgapur";
            legacyMap[142] = "National Institute of Technology (NIT / REC), Hamirpur";
            legacyMap[143] = "National Institute of Technology (NIT / REC), Silchar";
            legacyMap[144] = "National Institute of Technology (NIT / REC), Warangal";
            legacyMap[145] = "Rajiv Gandhi Proudyogiki Vishwavidyalaya (RGPV), Bhopal";
            legacyMap[146] = "National Institute of Technology (NIT), Kurukshetra";
            legacyMap[147] = "Govind Ballab Pant Engg. College";
            legacyMap[148] = "UPTUI, KNIT";
            legacyMap[149] = "UP BTE, Lucknow";
            legacyMap[150] = "West Bengal University";
            legacyMap[151] = "ABV-Indian Institute of Information Technology and Management (ABV-IIITM), Gwalior";
            legacyMap[152] = "BES University";
            legacyMap[153] = "DIAT, Pune";
            legacyMap[154] = "Kurukshetra University";
            legacyMap[155] = "University of Calicut, Malappuram";
            legacyMap[156] = "National Institute of Technology (NIT), Raipur";
            legacyMap[157] = "National Institute of Technology (NIT / REC), Rourkela";
            legacyMap[158] = "National Institute of Technology (NIT), Patna";
            legacyMap[159] = "University of Delhi (DU), New Delhi";
            legacyMap[160] = "Dilasagram Convent high school, ballarpur";
            legacyMap[161] = "Gurunanak College of Science, Ballarpur";
            legacyMap[162] = "Govt. College of Engineering, Karad";
            legacyMap[163] = "Indian Institute of Information Technology (IIIT), Allahabad";
            legacyMap[164] = "U A Board";
            legacyMap[165] = "UP Board Allahabad";
            legacyMap[166] = "Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru";
            legacyMap[167] = "Department of Pre-University Education (PUE), Karnataka";
            legacyMap[168] = "Visvesvaraya Technological University (VTU), Belagavi";
            legacyMap[169] = "General Education Department";
            legacyMap[170] = "Higher Secondary Board";
            legacyMap[171] = "BTER Jodhpur";
            legacyMap[172] = "West Bengal Board of Secondary Education (WBBSE)";
            legacyMap[173] = "West Bengal Council of Higher Secondary Education (WBCHSE)";
            legacyMap[174] = "West Bengal State Council of Technical and Vocational Education (WBSCTE), Kolkata";
            legacyMap[175] = "Jawaharlal Nehru Technological University (JNTU), Hyderabad";
            legacyMap[176] = "Board of Secondary School Education";
            legacyMap[177] = "Board of Intermediate Education";
            legacyMap[178] = "IIT, Bangalore";
            legacyMap[179] = "BIT, Durg";
            legacyMap[180] = "MP Board";
            legacyMap[181] = "State Board of Technical Education and Training (SBTET), Tamil Nadu";
            legacyMap[182] = "Andhra University, Visakhapatnam";
            legacyMap[183] = "Indian Institute of Science (IISc), Bangalore";
            legacyMap[184] = "Central Board of Secondary Education (CBSE), New Delhi";
            legacyMap[185] = "Cochin University of Science and Technology (CUSAT), Kochi";
            legacyMap[186] = "Acharya Nagarjuna University, Guntur";
            legacyMap[187] = "Jawaharlal Nehru Technological University Anantapur (JNTUA)";
            legacyMap[188] = "Indian Institute of Technology (IIT), Kharagpur";
            legacyMap[189] = "State board of technical Education";
            legacyMap[190] = "Kerala Board";
            legacyMap[191] = "Indian Institute of Science (IISc), Bangalore";
            legacyMap[192] = "Osmania University, Hyderabad";
            legacyMap[193] = "National Institute of Technology (NIT / REC), Calicut";
            legacyMap[194] = "ICSE/DPS, shimla";
            legacyMap[195] = "CBSE/RED, Jhajjar";
            legacyMap[196] = "K V Ponda (CBSE)";
            legacyMap[197] = "Board of Public Exam. Kerala";
            legacyMap[198] = "Board of Technical Education";
            legacyMap[199] = "Z P High school";
            legacyMap[200] = "Dept.of Tech.Edu, Kerala";
            legacyMap[201] = "Board of Vocational HR SEC Edn";
            legacyMap[202] = "University of Hyderabad (UoH / Central University), Hyderabad";
            legacyMap[203] = "UP Board";
            legacyMap[204] = "Bihar School Examination Board (BSEB), Patna";
            legacyMap[205] = "BIEE, Bihar";
            legacyMap[206] = "V.B.U, Hazaribag";
            legacyMap[207] = "PSG College of Technology, Coimbatore";
            legacyMap[208] = "Anna University, Chennai";
            legacyMap[209] = "MAGADH UNIVERSITY BODH GAYA";
            legacyMap[210] = "BIEC PATNA";
            legacyMap[211] = "Veer Kunwar Singh University, ARA";
            legacyMap[212] = "BSE, AP";
            legacyMap[213] = "BIE, AP";
            legacyMap[214] = "University of Bradford";
            legacyMap[215] = "B.T.E UP";
            legacyMap[216] = "Govt of A.P, Dept of Emp and Training";
            legacyMap[217] = "Ministry of Labour and EMP";
            legacyMap[218] = "Pre University Board";
            legacyMap[219] = "West Bengal State Council of Technical and Vocational Education (WBSCTE), Kolkata";
            legacyMap[220] = "Jiwaji University";
            legacyMap[221] = "Central Board of Secondary Education (CBSE), New Delhi";
            legacyMap[222] = "Indian Institute of Technology (IIT), Jodhpur";
            legacyMap[223] = "Indian Institute of Technology (IIT), Gandhinagar";
            legacyMap[224] = "VVB SS School";
            legacyMap[225] = "CM science College,Darbhanga (BSEB)";
            legacyMap[226] = "Indian Institute of Technology (IIT), Patna";
            legacyMap[227] = "Acharya Nagarjuna University, Guntur";
            legacyMap[228] = "Dept. Tech. Edu., Karnataka";
            legacyMap[229] = "S.B.T.E Bihar";
            legacyMap[230] = "B.S.E.H";
            legacyMap[231] = "H.S.B.T.E";
            legacyMap[232] = "UTTARAKHAND TECHNICAL UNIVERSITY";
            legacyMap[233] = "NATIONAL COLLEGE BANGALORE";
            legacyMap[234] = "ST. JOSEPHS COLLEGE BANGALORE";
            legacyMap[235] = "SSC BOARD";
            legacyMap[236] = "INTER BOARD";
            legacyMap[237] = "National Institute of Technology (NIT / REC), Tiruchirappalli";
            legacyMap[238] = "VINAYAKA MISSION UNIVERSITY SALEM";
            legacyMap[239] = "CETE STQC,Hyderabad";
            legacyMap[240] = "IDEMI MUMBAI";
            legacyMap[241] = "Karnataka Board";
            legacyMap[242] = "BSE, ODISHA";
            legacyMap[243] = "HSCE, ODISHA";
            legacyMap[244] = "SCTE and VT, ODISHA";
            legacyMap[245] = "Sikkim Manipal University";
            legacyMap[246] = "B.S.E.R, AJMER";
            legacyMap[247] = "B.T.E.R, Jodhpur";
            legacyMap[248] = "Indira Gandhi National Open University (IGNOU), New Delhi";
            legacyMap[249] = "AMITE";
            legacyMap[250] = "Pune Board";
            legacyMap[251] = "Mumbai Board";
            legacyMap[252] = "P E C, Chandigarh";
            legacyMap[253] = "RGUKT, Basar";
            legacyMap[254] = "Nashik Board";
            legacyMap[255] = "Automomous. Inst of Govt of Maharashtra";
            legacyMap[256] = "Himachal Pradesh University Shimla Himachal Pradesh";
            legacyMap[257] = "HP BOARD OF SCHOOL EDUCATION DHARAMSHAL HIMACHAL PRADESH";
            legacyMap[258] = "ASSAM STATE BOARD";
            legacyMap[259] = "University of Madras, Chennai";
            legacyMap[260] = "Bangalore University, Bengaluru";
            legacyMap[261] = "DREXEL UNIVERSITY, PA, USA";
            legacyMap[262] = "BIHAR INTERMEDIATE EDUCATION COUNCIL, PATNA";
            legacyMap[263] = "Biju Patnaik University of Technology, Orissa, Rourkela";
            legacyMap[264] = "Jawaharlal Nehru Technological University (JNTU), Hyderabad";
            legacyMap[265] = "Karnataka State Open University";
            legacyMap[266] = "Shivaji University, Kolhapur";
            legacyMap[267] = "Bharati Vidypeetha University, Pune";
            legacyMap[268] = "VHSE BOARD";
            legacyMap[269] = "Board of Hariyana";
            legacyMap[270] = "Oxford Evening Polytechnic, JP Nagar, Bengaluru";
            legacyMap[271] = "Savitribai Phule Pune University (SPPU / University of Pune)";
            legacyMap[272] = "GPM College";
            legacyMap[273] = "GRM Sr Sec School";
            legacyMap[274] = "National Council for Vocational Training (NCVT)";
            legacyMap[275] = "Indian Institute of Technology (IIT), Jodhpur";
            legacyMap[276] = "National Institute of Technology Karnataka (NITK / REC), Surathkal";
            legacyMap[277] = "Tripura";
            legacyMap[278] = "Indian Institute of Management (IIM), Lucknow";
            legacyMap[279] = "AP Board";
            legacyMap[280] = "Tamilnadu State Board";
            legacyMap[281] = "Central University of Rajasthan, Ajmer";
            legacyMap[282] = "University of Ajmer";
            legacyMap[283] = "National Institute of Technology (NIT), Agartala";
            legacyMap[284] = "Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow";
            legacyMap[285] = "CHSE, ODISHA";
            legacyMap[286] = "CHSS, Jaipur";
            legacyMap[287] = "JNVU, JODHPUR";
            legacyMap[288] = "Savitribai Phule Pune University (SPPU / University of Pune)";
            legacyMap[289] = "Indian Institute of Technology (IIT), Indore";
            legacyMap[290] = "National Institute of Technology (NIT), Delhi";
            legacyMap[291] = "Maulana Azad National Institute of Technology (MANIT), Bhopal";
            legacyMap[292] = "GGV, Bilaspur";
            legacyMap[293] = "UTTARAKHAND BOARD";
            legacyMap[294] = "GRAPHIC ERA UNIVERSITY";
            legacyMap[295] = "Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)";
            legacyMap[296] = "VIT, PUNE";
            legacyMap[297] = "Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)";
            legacyMap[298] = "National Institute of Technology (NIT), Srinagar";
            legacyMap[299] = "Siddarth University";
            legacyMap[300] = "RBSE, Ajmer";
            legacyMap[301] = "Rajasthan board";
            legacyMap[302] = "Banarthali University";
            legacyMap[303] = "University of Delhi (DU), New Delhi";
            legacyMap[304] = "Board of Secondary Education, Assam (SEBA)";
            legacyMap[305] = "SCTE, Assam";
            legacyMap[306] = "NERIST, Nirjuli";
            legacyMap[307] = "Indian Institute of Technology (IIT), Hyderabad";
            legacyMap[308] = "Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow";
            legacyMap[309] = "TRRRSVM MIC Kashipur";
            legacyMap[310] = "Indian Institute of Technology (IIT), Mandi";
            legacyMap[311] = "KV Bhaderwah";
            legacyMap[312] = "BSF Sr Sec School";
            legacyMap[313] = "Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow";
            legacyMap[314] = "MIT Muzaharpur";
            legacyMap[315] = "YMCA University";
            legacyMap[316] = "Indian Institute of Technology (IIT), Roorkee";
            legacyMap[317] = "SDM Sr Sec School";
            legacyMap[318] = "Green Meadows";
            legacyMap[319] = "G.B.P.U.A.T, Pantnagar";
            legacyMap[320] = "KV (AFS), Bormer";
            legacyMap[321] = "Dundlod Public School Jhunjhunu";
            legacyMap[322] = "KVM Public Sr Sec School, Sikar";
            legacyMap[323] = "Indian Institute of Technology (BHU), Varanasi";
            legacyMap[324] = "CMM Jabalpur";
            legacyMap[325] = "Indian Institute of Technology (IIT), Ropar";
            legacyMap[326] = "BVDU, Pune";
            legacyMap[327] = "MIT-SDE, Pune";
            legacyMap[328] = "GOVT.POLYTECHNIC BANKA";
            legacyMap[329] = "DNS COLLEGE BHUSIARAJOUN";
            legacyMap[330] = "H.S.SALEMPUR";
            legacyMap[331] = "SBTE, Bihar";
            legacyMap[332] = "JAC board";
            legacyMap[333] = "SBTE Jharkhand";
            legacyMap[334] = "SSIS,DERABASSI";
            legacyMap[335] = "GMSSS,CHANDIGARH SEC-16";
            legacyMap[336] = "GGDSD,PUNJAB UNIVERSITY";
            legacyMap[337] = "SSK NAND INTERCOLL";
            legacyMap[338] = "Indian Institute of Technology (IIT), Bhubaneswar";
            legacyMap[339] = "S.V GOVT POLYTECHNIC";
            legacyMap[340] = "Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)";
            legacyMap[341] = "Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)";
            legacyMap[342] = "Uttarakhand Board of Technical Education, Roorkee (UBTER)";
            legacyMap[343] = "Board of Secondary Education, Assam (SEBA)";
            legacyMap[344] = "Assam Higher Secondary Education Council (AHSEC)";
            legacyMap[345] = "West Bengal Board of Secondary Education (WBBSE)";
            legacyMap[346] = "West Bengal Council of Higher Secondary Education (WBCHSE)";
            legacyMap[347] = "Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)";
            legacyMap[348] = "Hemwanti Nandan Bahuguna Garhwal University";
            legacyMap[349] = "MADAN MOHAN MALAVIYA UNIVERSITY OF TECHNOLOGY, GORAKHPUR, UP";
                _legacyUnivMap = legacyMap;
            }
            return _legacyUnivMap;
        }

        public static string GetCanonicalLegacyUniversity(int uid, string colName)
        {
            var map = GetLegacyUniversityMap();
            if (uid >= 0 && map.ContainsKey(uid))
            {
                return map[uid];
            }
            if (!string.IsNullOrWhiteSpace(colName) && !IsPlaceholderOrInvalid(colName))
            {
                return NormalizeUniversityName(colName);
            }
            return "Not Specified";
        }

        public static bool AddUniversity(string name, string stateOrType)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string trimmedName = name.Trim();

            // Prevent duplicate insertion
            string checkSql = "SELECT COUNT(*) FROM EDU_UNIVERSITY WHERE UPPER(TRIM(UNIV_NAME)) = :N";
            object cnt = ExecuteScalar(checkSql, new OracleParameter("N", trimmedName.ToUpper()));
            if (cnt != null && Convert.ToInt32(cnt) > 0)
            {
                string activateSql = "UPDATE EDU_UNIVERSITY SET IS_ACTIVE = 1 WHERE UPPER(TRIM(UNIV_NAME)) = :N";
                ExecuteNonQuery(activateSql, new OracleParameter("N", trimmedName.ToUpper()));
                return true;
            }

            string sql = "INSERT INTO EDU_UNIVERSITY (UNIV_ID, UNIV_NAME, STATE_OR_TYPE, IS_ACTIVE) VALUES (SEQ_EDU_UNIVERSITY.NEXTVAL, :N, :ST, 1)";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", trimmedName),
                new OracleParameter("ST", stateOrType ?? "")) > 0;
        }

        public static bool UpdateUniversity(int univId, string name, string stateOrType)
        {
            string sql = "UPDATE EDU_UNIVERSITY SET UNIV_NAME = :N, STATE_OR_TYPE = :ST WHERE UNIV_ID = :ID";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", name.Trim()),
                new OracleParameter("ST", stateOrType ?? ""),
                new OracleParameter("ID", univId)) > 0;
        }

        public static bool DeleteUniversity(int univId)
        {
            string sql = "DELETE FROM EDU_UNIVERSITY WHERE UNIV_ID = :ID";
            return ExecuteNonQuery(sql,
                new OracleParameter("ID", univId)) > 0;
        }

        public static bool AddCollege(string name, string type, string location, string affiliatedUniv)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string trimmedName = name.Trim();

            // Prevent duplicate insertion
            string checkSql = "SELECT COUNT(*) FROM EDU_COLLEGE WHERE UPPER(TRIM(COLLEGE_NAME)) = :N";
            object cnt = ExecuteScalar(checkSql, new OracleParameter("N", trimmedName.ToUpper()));
            if (cnt != null && Convert.ToInt32(cnt) > 0)
            {
                string activateSql = @"UPDATE EDU_COLLEGE 
                                       SET IS_ACTIVE = 1, COLLEGE_TYPE = :T, LOCATION = :L, AFFILIATED_UNIV = :U 
                                       WHERE UPPER(TRIM(COLLEGE_NAME)) = :N";
                ExecuteNonQuery(activateSql,
                    new OracleParameter("T", type ?? ""),
                    new OracleParameter("L", location ?? ""),
                    new OracleParameter("U", affiliatedUniv ?? ""),
                    new OracleParameter("N", trimmedName.ToUpper()));
                return true;
            }

            string sql = @"INSERT INTO EDU_COLLEGE (COLLEGE_ID, COLLEGE_NAME, COLLEGE_TYPE, LOCATION, AFFILIATED_UNIV, IS_ACTIVE) 
                           VALUES (SEQ_EDU_COLLEGE.NEXTVAL, :N, :T, :L, :U, 1)";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", trimmedName),
                new OracleParameter("T", type ?? ""),
                new OracleParameter("L", location ?? ""),
                new OracleParameter("U", affiliatedUniv ?? "")) > 0;
        }

        public static bool UpdateCollege(int collegeId, string name, string type, string location, string affiliatedUniv)
        {
            string sql = @"UPDATE EDU_COLLEGE 
                           SET COLLEGE_NAME = :N, COLLEGE_TYPE = :T, LOCATION = :L, AFFILIATED_UNIV = :U 
                           WHERE COLLEGE_ID = :ID";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", name.Trim()),
                new OracleParameter("T", type ?? ""),
                new OracleParameter("L", location ?? ""),
                new OracleParameter("U", affiliatedUniv ?? ""),
                new OracleParameter("ID", collegeId)) > 0;
        }

        public static bool DeleteCollege(int collegeId)
        {
            string sql = "DELETE FROM EDU_COLLEGE WHERE COLLEGE_ID = :ID";
            return ExecuteNonQuery(sql,
                new OracleParameter("ID", collegeId)) > 0;
        }

        public static bool AddSubject(string name, string group)
        {
            string sql = "INSERT INTO EDU_SUBJECT (SUBJECT_ID, SUBJECT_NAME, SUBJECT_GROUP, IS_ACTIVE) VALUES (SEQ_EDU_SUBJECT.NEXTVAL, :N, :GRP, 1)";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", name.Trim()),
                new OracleParameter("GRP", group ?? "")) > 0;
        }

        public static bool UpdateSubject(int subjectId, string name, string group)
        {
            string sql = "UPDATE EDU_SUBJECT SET SUBJECT_NAME = :N, SUBJECT_GROUP = :GRP WHERE SUBJECT_ID = :ID";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", name.Trim()),
                new OracleParameter("GRP", group ?? ""),
                new OracleParameter("ID", subjectId)) > 0;
        }

        public static bool DeleteSubject(int subjectId)
        {
            string sql = "DELETE FROM EDU_SUBJECT WHERE SUBJECT_ID = :ID";
            return ExecuteNonQuery(sql,
                new OracleParameter("ID", subjectId)) > 0;
        }

        public static DataTable GetAdminUsers()
        {
            return ExecuteQuery("SELECT PCNO, USER_NAME, ROLE, IS_ACTIVE, CREATED_ON FROM EDU_ADMIN_USERS ORDER BY PCNO ASC");
        }

        public static bool AddAdminUser(string pcno, string name)
        {
            string sql = @"
                MERGE INTO EDU_ADMIN_USERS dest
                USING (SELECT :PCNO AS PCNO, :NAME AS NAME FROM DUAL) src
                ON (dest.PCNO = src.PCNO)
                WHEN MATCHED THEN UPDATE SET IS_ACTIVE = 1
                WHEN NOT MATCHED THEN INSERT (PCNO, USER_NAME, ROLE, IS_ACTIVE) VALUES (src.PCNO, src.NAME, 'Admin', 1)";

            return ExecuteNonQuery(sql,
                new OracleParameter("PCNO", pcno.Trim()),
                new OracleParameter("NAME", name ?? "Admin")) > 0;
        }

        public static bool UpdateAdminUser(string pcno, string name, string role)
        {
            string sql = "UPDATE EDU_ADMIN_USERS SET USER_NAME = :N, ROLE = :R WHERE PCNO = :PCNO";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", name.Trim()),
                new OracleParameter("R", role ?? "Admin"),
                new OracleParameter("PCNO", pcno.Trim())) > 0;
        }

        public static bool DeleteAdminUser(string pcno)
        {
            if (pcno.Trim() == "1001") return false;
            string sql = "DELETE FROM EDU_ADMIN_USERS WHERE PCNO = :PCNO";
            return ExecuteNonQuery(sql,
                new OracleParameter("PCNO", pcno.Trim())) > 0;
        }

        public static bool ToggleAdminUser(string pcno, bool active)
        {
            string sql = "UPDATE EDU_ADMIN_USERS SET IS_ACTIVE = :ACT WHERE PCNO = :PCNO";
            return ExecuteNonQuery(sql,
                new OracleParameter("ACT", active ? 1 : 0),
                new OracleParameter("PCNO", pcno.Trim())) > 0;
        }

        #endregion

        #region Data Migration Engine (HR_EMP_EDUCATION -> EDU_EMP_EDUCATION)

        public static string GetLegacyEducationTableName()
        {
            string tbl = ConfigurationManager.AppSettings["LegacyEducationTable"];
            if (!string.IsNullOrWhiteSpace(tbl))
            {
                return tbl.Trim();
            }
            return "HR_EMP_EDUCATION";
        }

        public static DataTable GetMigrationStats()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LegacyTotal", typeof(int));
            dt.Columns.Add("MigratedCount", typeof(int));
            dt.Columns.Add("PendingCount", typeof(int));
            dt.Columns.Add("LegacyEmployees", typeof(int));

            int legTotal = 0;
            int legEmp = 0;
            int migrated = 0;

            try
            {
                string legTable = GetLegacyEducationTableName();
                object totObj = ExecuteScalar("SELECT COUNT(*) FROM " + legTable);
                legTotal = totObj != null ? Convert.ToInt32(totObj) : 0;

                object empObj = ExecuteScalar("SELECT COUNT(DISTINCT PCNO) FROM " + legTable);
                legEmp = empObj != null ? Convert.ToInt32(empObj) : 0;

                object migObj = ExecuteScalar("SELECT COUNT(*) FROM EDU_EMP_EDUCATION WHERE IS_MIGRATED = 1");
                migrated = migObj != null ? Convert.ToInt32(migObj) : 0;
            }
            catch { }

            DataRow r = dt.NewRow();
            r["LegacyTotal"] = legTotal;
            r["MigratedCount"] = migrated;
            r["PendingCount"] = Math.Max(0, legTotal - migrated);
            r["LegacyEmployees"] = legEmp;
            dt.Rows.Add(r);
            return dt;
        }

        public static DataTable GetMigrationMappings()
        {
            string query = @"
                SELECT m.MAPPING_ID, m.OLD_QFN_ID, m.OLD_QFN_NAME, m.OLD_QFN_GROUP,
                       m.NEW_QUAL_MAIN_CODE, m.NEW_QUAL_SUB_CODE, m.NOTES, m.IS_ACTIVE,
                       q.QUAL_NAME AS NEW_QUAL_NAME
                FROM EDU_QUAL_MAPPING m
                LEFT JOIN EDU_QUAL_MAIN q ON m.NEW_QUAL_MAIN_CODE = q.QUAL_CODE
                ORDER BY m.OLD_QFN_ID ASC";
            return ExecuteQuery(query);
        }

        public static bool UpdateMigrationMapping(int mappingId, string newMainCode, string newSubCode, string notes)
        {
            string sql = @"
                UPDATE EDU_QUAL_MAPPING SET
                    NEW_QUAL_MAIN_CODE = :MC,
                    NEW_QUAL_SUB_CODE = :SC,
                    NOTES = :NOTES
                WHERE MAPPING_ID = :ID";

            return ExecuteNonQuery(sql,
                new OracleParameter("MC", newMainCode),
                new OracleParameter("SC", newSubCode ?? "GENERAL"),
                new OracleParameter("NOTES", notes ?? ""),
                new OracleParameter("ID", mappingId)) > 0;
        }

        public static DataTable GetMigrationLogs()
        {
            return ExecuteQuery("SELECT LOG_ID, MIGRATION_DATE, EXECUTED_BY, TOTAL_READ, TOTAL_MIGRATED, TOTAL_SKIPPED, STATUS, LOG_DETAILS FROM EDU_MIGRATION_LOG ORDER BY LOG_ID DESC");
        }

        public static string ExecuteDataMigration(string executedBy, string defaultStatus = "Submitted")
        {
            int totalRead = 0;
            int totalMigrated = 0;
            int totalSkipped = 0;
            int skippedNoEmpDetails = 0;
            int skippedAlreadyMigrated = 0;

            EnsureSchema();

            try
            {
                string legTable = GetLegacyEducationTableName();

                // Ensure GRADE_DIVISION is nullable in EDU_EMP_EDUCATION to allow migrating draft/incomplete legacy records
                try
                {
                    ExecuteNonQuery("ALTER TABLE EDU_EMP_EDUCATION MODIFY (GRADE_DIVISION NULL)");
                }
                catch { }

                // Fetch legacy records
                string queryOld = "SELECT PCNO, CTREDN, QFNID, UNIVINSTID, COLLEGENAME, SUBJECTID, YEARPASSED, GRADE, PERCENTAGE, HIGHESTQFN FROM " + legTable + " ORDER BY PCNO, CTREDN";
                DataTable dtOld = ExecuteQuery(queryOld);

                if (dtOld == null || dtOld.Rows.Count == 0)
                {
                    return "No records found in " + legTable + " table to migrate.";
                }

                totalRead = dtOld.Rows.Count;

                // Pre-cache Mappings
                DataTable dtMaps = ExecuteQuery("SELECT OLD_QFN_ID, NEW_QUAL_MAIN_CODE, NEW_QUAL_SUB_CODE FROM EDU_QUAL_MAPPING WHERE IS_ACTIVE = 1");
                Dictionary<int, Tuple<string, string>> mapDict = new Dictionary<int, Tuple<string, string>>();
                foreach (DataRow mr in dtMaps.Rows)
                {
                    int qid = Convert.ToInt32(mr["OLD_QFN_ID"]);
                    mapDict[qid] = Tuple.Create(mr["NEW_QUAL_MAIN_CODE"].ToString(), mr["NEW_QUAL_SUB_CODE"].ToString());
                }

                // Pre-cache Main Qual Names
                DataTable dtMainQuals = ExecuteQuery("SELECT QUAL_CODE, QUAL_NAME FROM EDU_QUAL_MAIN");
                Dictionary<string, string> qualNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow qr in dtMainQuals.Rows)
                {
                    qualNames[qr["QUAL_CODE"].ToString()] = qr["QUAL_NAME"].ToString();
                }

                // Pre-cache Universities
                DataTable dtUniv = ExecuteQuery("SELECT UNIV_ID, UNIV_NAME FROM EDU_UNIVERSITY");
                Dictionary<int, string> univDict = new Dictionary<int, string>();
                foreach (DataRow ur in dtUniv.Rows)
                {
                    univDict[Convert.ToInt32(ur["UNIV_ID"])] = ur["UNIV_NAME"].ToString();
                }

                // Pre-cache Subjects
                DataTable dtSubj = ExecuteQuery("SELECT SUBJECT_ID, SUBJECT_NAME FROM EDU_SUBJECT");
                Dictionary<int, string> subjDict = new Dictionary<int, string>();
                foreach (DataRow sr in dtSubj.Rows)
                {
                    subjDict[Convert.ToInt32(sr["SUBJECT_ID"])] = sr["SUBJECT_NAME"].ToString();
                }

                // Cache employee check to avoid repetitive DB lookups
                Dictionary<string, Tuple<bool, string, string, string>> empCache = new Dictionary<string, Tuple<bool, string, string, string>>(StringComparer.OrdinalIgnoreCase);

                // Process migration
                foreach (DataRow r in dtOld.Rows)
                {
                    string pcno = r["PCNO"].ToString().Trim();
                    int ctredn = Convert.ToInt32(r["CTREDN"]);

                    // 1. Check whether PCNO exists in hrdata.empdetails before importing
                    Tuple<bool, string, string, string> empInfo;
                    if (!empCache.TryGetValue(pcno, out empInfo))
                    {
                        string eName, eDesig, eDiv;
                        bool exists = CheckEmployeeExistsInHrData(pcno, out eName, out eDesig, out eDiv);
                        empInfo = Tuple.Create(exists, eName, eDesig, eDiv);
                        empCache[pcno] = empInfo;
                    }

                    if (!empInfo.Item1)
                    {
                        // PCNO is not in hrdata.empdetails -> Do NOT import
                        totalSkipped++;
                        skippedNoEmpDetails++;
                        continue;
                    }

                    // 2. Check if already migrated
                    string checkExists = "SELECT COUNT(*) FROM EDU_EMP_EDUCATION WHERE PCNO = :PCNO AND OLD_CTREDN = :CTREDN";
                    object existsObj = ExecuteScalar(checkExists,
                        new OracleParameter("PCNO", pcno),
                        new OracleParameter("CTREDN", ctredn));

                    if (existsObj != null && Convert.ToInt32(existsObj) > 0)
                    {
                        totalSkipped++;
                        skippedAlreadyMigrated++;
                        continue;
                    }

                    int qfnid = Convert.ToInt32(r["QFNID"]);
                    string mainCode = "OTHER";
                    string subCode = "GENERAL";

                    if (mapDict.ContainsKey(qfnid))
                    {
                        mainCode = mapDict[qfnid].Item1;
                        subCode = mapDict[qfnid].Item2;
                    }

                    string mainName = qualNames.ContainsKey(mainCode) ? qualNames[mainCode] : mainCode;

                    // University resolution using canonical legacy catalog & aliases
                    int uid = -1;
                    if (r["UNIVINSTID"] != DBNull.Value)
                    {
                        int.TryParse(r["UNIVINSTID"].ToString(), out uid);
                    }
                    string rawCol = r["COLLEGENAME"] != DBNull.Value ? r["COLLEGENAME"].ToString() : "";
                    string instName = GetCanonicalLegacyUniversity(uid, rawCol);
                    instName = NormalizeUniversityName(instName);

                    // Ensure university is registered in EDU_UNIVERSITY master table globally for ALL users
                    if (!IsPlaceholderOrInvalid(instName))
                    {
                        instName = EnsureUniversityExists(instName, "Recognized Institution");
                    }

                    // Subject / Specialization resolution
                    string specName = "General";
                    if (r["SUBJECTID"] != DBNull.Value)
                    {
                        int sid = Convert.ToInt32(r["SUBJECTID"]);
                        if (subjDict.ContainsKey(sid)) specName = subjDict[sid];
                    }

                    // Highest qualification
                    string highStr = r["HIGHESTQFN"] != DBNull.Value ? r["HIGHESTQFN"].ToString().Trim().ToUpper() : "N";
                    bool isHighest = (highStr == "Y" || highStr == "YES" || highStr == "1" || highStr == "TRUE");

                    // Employee Profile details from verified empdetails
                    string empName = !string.IsNullOrWhiteSpace(empInfo.Item2) ? empInfo.Item2 : ("Employee " + pcno);
                    string desig = empInfo.Item3 ?? "";
                    string div = empInfo.Item4 ?? "";

                    int yearPassed = 2000;
                    if (r["YEARPASSED"] != DBNull.Value)
                    {
                        int.TryParse(r["YEARPASSED"].ToString(), out yearPassed);
                    }

                    string rawGrade = r["GRADE"] != DBNull.Value ? r["GRADE"].ToString() : "";
                    string normGrade = NormalizeGrade(rawGrade);
                    string pct = r["PERCENTAGE"] != DBNull.Value ? r["PERCENTAGE"].ToString() : "N/A";
                    string college = r["COLLEGENAME"] != DBNull.Value ? r["COLLEGENAME"].ToString() : "";

                    // If record contains placeholder values or missing/invalid grade, force to Draft so employee must verify and select proper values
                    string rowStatus = defaultStatus;
                    if (IsPlaceholderOrInvalid(college) || IsPlaceholderOrInvalid(instName) ||
                        IsInvalidSpecialization(specName) || string.IsNullOrEmpty(normGrade) || !IsValidGrade(normGrade) ||
                        IsPlaceholderOrInvalid(pct) ||
                        yearPassed < 1950)
                    {
                        rowStatus = "Draft";
                    }

                    // Normalize score format if raw number (e.g. 78.4 -> 78.4%, 8.45 -> 8.45 CGPA)
                    string finalScore = pct;
                    if (!IsPlaceholderOrInvalid(pct))
                    {
                        double pNum;
                        if (double.TryParse(pct.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out pNum))
                        {
                            if (pNum > 10.0 && pNum <= 100.0)
                            {
                                finalScore = pNum.ToString("0.##") + "%";
                            }
                            else if (pNum > 0.0 && pNum <= 10.0)
                            {
                                finalScore = pNum.ToString("0.##") + " CGPA";
                            }
                        }
                    }

                    // Check if this exact qualification already exists for this employee in EDU_EMP_EDUCATION
                    string checkQualExists = @"
                        SELECT COUNT(*) FROM EDU_EMP_EDUCATION 
                        WHERE PCNO = :PCNO AND QUAL_MAIN_CODE = :MC AND QUAL_SUB_CODE = :SC AND YEAR_PASSED = :YP";
                    object qualExistsObj = ExecuteScalar(checkQualExists,
                        new OracleParameter("PCNO", pcno),
                        new OracleParameter("MC", mainCode),
                        new OracleParameter("SC", subCode),
                        new OracleParameter("YP", yearPassed));

                    if (qualExistsObj != null && Convert.ToInt32(qualExistsObj) > 0)
                    {
                        totalSkipped++;
                        skippedAlreadyMigrated++;
                        continue;
                    }

                    string insertSql = @"
                        INSERT INTO EDU_EMP_EDUCATION (
                            PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
                            QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
                            YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
                            CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS,
                            SUBMITTED_DATE, CREATED_ON, CREATED_BY, IS_MIGRATED, OLD_CTREDN
                        ) VALUES (
                            :PCNO, :ENAME, :DESIG, :DIV,
                            :MC, :MN, :SC, :SN,
                            :YP, :INST, :COLL, :SPEC,
                            :CGPA, :GRD, :HIGH, :REM, :ST,
                            CASE WHEN :ST = 'Submitted' THEN SYSDATE ELSE NULL END,
                            SYSDATE, :USR, 1, :CTR
                        )";

                    ExecuteNonQuery(insertSql,
                        new OracleParameter("PCNO", pcno),
                        new OracleParameter("ENAME", empName),
                        new OracleParameter("DESIG", desig),
                        new OracleParameter("DIV", div),
                        new OracleParameter("MC", mainCode),
                        new OracleParameter("MN", mainName),
                        new OracleParameter("SC", subCode),
                        new OracleParameter("SN", specName),
                        new OracleParameter("YP", yearPassed),
                        new OracleParameter("INST", instName),
                        new OracleParameter("COLL", college),
                        new OracleParameter("SPEC", specName),
                        new OracleParameter("CGPA", finalScore),
                        new OracleParameter("GRD", !string.IsNullOrEmpty(normGrade) ? (object)normGrade : DBNull.Value),
                        new OracleParameter("HIGH", isHighest ? 1 : 0),
                        new OracleParameter("REM", DBNull.Value),
                        new OracleParameter("ST", rowStatus),
                        new OracleParameter("USR", executedBy),
                        new OracleParameter("CTR", ctredn));

                    totalMigrated++;
                }

                // Post-migration deduplication of IS_HIGHEST: Ensure at most one record per employee has IS_HIGHEST = 1
                try
                {
                    string dedupeHighestSql = @"
                        MERGE INTO EDU_EMP_EDUCATION dest
                        USING (
                            SELECT EDU_ID, 
                                   ROW_NUMBER() OVER (PARTITION BY PCNO ORDER BY YEAR_PASSED DESC, EDU_ID DESC) AS RN
                            FROM EDU_EMP_EDUCATION
                            WHERE IS_HIGHEST = 1
                        ) src
                        ON (dest.EDU_ID = src.EDU_ID AND src.RN > 1)
                        WHEN MATCHED THEN UPDATE SET dest.IS_HIGHEST = 0";
                    ExecuteNonQuery(dedupeHighestSql);

                    string ensureHighestSql = @"
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
                        WHEN MATCHED THEN UPDATE SET dest.IS_HIGHEST = 1";
                    ExecuteNonQuery(ensureHighestSql);
                }
                catch { }

                // Log migration
                string logDetails = string.Format(
                    "Migration executed successfully. Total Read: {0}, Migrated: {1}, Skipped: {2} ({3} already existed, {4} skipped because PCNO not found in hrdata.empdetails).",
                    totalRead, totalMigrated, totalSkipped, skippedAlreadyMigrated, skippedNoEmpDetails);

                string logSql = @"
                    INSERT INTO EDU_MIGRATION_LOG (LOG_ID, MIGRATION_DATE, EXECUTED_BY, TOTAL_READ, TOTAL_MIGRATED, TOTAL_SKIPPED, STATUS, LOG_DETAILS)
                    VALUES (SEQ_EDU_MIG_LOG.NEXTVAL, SYSDATE, :USR, :TR, :TM, :TS, 'COMPLETED', :DET)";

                ExecuteNonQuery(logSql,
                    new OracleParameter("USR", executedBy),
                    new OracleParameter("TR", totalRead),
                    new OracleParameter("TM", totalMigrated),
                    new OracleParameter("TS", totalSkipped),
                    new OracleParameter("DET", logDetails));

                return string.Format(
                    "Migration completed successfully! Processed: {0} records. Newly Migrated: {1}, Skipped: {2} ({3} already existed, {4} not in hrdata.empdetails).",
                    totalRead, totalMigrated, totalSkipped, skippedAlreadyMigrated, skippedNoEmpDetails);
            }
            catch (Exception ex)
            {
                return "Migration encountered an error: " + ex.Message;
            }
        }

        #endregion

        #region Audit Trail

        public static void LogAudit(string pcno, int? eduId, string actionType, string actionBy, string comments)
        {
            try
            {
                string sql = @"
                    INSERT INTO EDU_AUDIT_LOG (AUDIT_ID, PCNO, EDU_ID, ACTION_TYPE, ACTION_BY, ACTION_DATE, COMMENTS)
                    VALUES (SEQ_EDU_AUDIT.NEXTVAL, :PCNO, :EID, :ATYPE, :ABY, SYSDATE, :COM)";

                ExecuteNonQuery(sql,
                    new OracleParameter("PCNO", pcno ?? ""),
                    new OracleParameter("EID", eduId.HasValue ? (object)eduId.Value : DBNull.Value),
                    new OracleParameter("ATYPE", actionType ?? ""),
                    new OracleParameter("ABY", actionBy ?? ""),
                    new OracleParameter("COM", comments ?? ""));
            }
            catch { }
        }

        #endregion
    }
}
