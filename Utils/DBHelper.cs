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
                                    GRADE_DIVISION VARCHAR2(50) NOT NULL,
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
                    "UPDATE EDU_QUAL_SUB SET MAIN_QUAL_CODE = 'BE' WHERE MAIN_QUAL_CODE IS NULL AND SUB_CODE IN ('CIVIL', 'DMECH', 'COMPE', 'COMPS', 'ELCL', 'E&C', 'ELECT', 'INDST', 'INSTR', 'AERO', 'AUTO', 'CHEMI', 'NANO', 'RACE', 'ADEL', 'ADMT', 'ANMT', 'APH', 'APE', 'APO', 'ARAN', 'CREO', 'CTSP', 'CSYG', 'DENGG', 'DEMQ', 'DSCE', 'AENGG', 'ELTE', 'FLM', 'GAST', 'GUIM', 'GUIW', 'HEPEN', 'INCS', 'INEC', 'INOP', 'LEOE', 'MCDY', 'MACDE', 'MRENG', 'MIELE', 'OEAOC', 'POE', 'PRENG', 'REBTE', 'SOSTA', 'SPER', 'SIT', 'SYSCO', 'SYSSO', 'TELES', 'THERE', 'TODE', 'TOOEN', 'WELTE', 'COMME', 'ELINS', 'SEFEG', 'GENENG', 'AI', 'AM', 'AROSE', 'AUTMO', 'AVONI', 'DSP', 'EE', 'IM', 'INDE', 'INSSP', 'IS', 'IT', 'M&S', 'MAREN', 'MATEC', 'MD', 'METAL', 'MME', 'MSD', 'NARCH', 'PLEN', 'RFM', 'TEXTL')",
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
                "MTRI|10 th|Secondary School",
                "BMTRI|Less Than 10th|Secondary School",
                "PUC|PRE UNIVERSITY COURSE|Higher Secondary",
                "SCI|12 th|Higher Secondary",
                "INTER|INTERMEDIATE OR PLUS 2|Higher Secondary",
                "ITI|ITI|Vocational / Trade",
                "CC|CERTIFICATE COURSE|Vocational / Trade",
                "DIPL|DIPLOMA|Diploma",
                "DENG|DIPLOMA IN ENGINEERING|Diploma",
                "BE|BACHELOR OF ENGINEERING|Undergraduate",
                "BTECH|BACHELOR OF TECHNOLOGY|Undergraduate",
                "BSC|BACHELOR OF SCIENCE|Undergraduate",
                "BCOM|BACHELOR OF COMMERCE|Undergraduate",
                "BA|BACHOLER OF ARTS|Undergraduate",
                "BCA|BACHELOR OF COMPUTER APP.|Undergraduate",
                "BBA|BACHELOR OF BUSINESS ADMINSTRATION|Undergraduate",
                "BBM|BACHELOR OF BUSINESS MANAGEMENT|Undergraduate",
                "MBBS|BACHELOR OF MEDICINE & BACHELOR OF SURGERY|Undergraduate",
                "BDS|BACHELOR OF DENTAL SURGERY|Undergraduate",
                "LLB|BACHELOR OF LAWS|Undergraduate",
                "BARCH|BACHELOR OF ARCHITECTURE|Undergraduate",
                "AMIE|AMIE/IETE|Undergraduate",
                "BSCEN|BACHELOR OF SCIENCE (ENGG.)|Undergraduate",
                "ME|MASTER OF ENGINEERING|Postgraduate",
                "MTECH|MASTER OF TECHNOLOGY|Postgraduate",
                "MSC|MASTER OF SCIENCE|Postgraduate",
                "MCOM|MASTER OF COMMERCE|Postgraduate",
                "MA|MASTER OF ARTS|Postgraduate",
                "MCA|MASTER OF COMPUTER APPLICATIONS|Postgraduate",
                "MBA|MASTER OF BUSINESS ADMINSTRATION|Postgraduate",
                "MSW|MASTER OF SOCIAL WORK|Postgraduate",
                "LLM|MASTER OF LAWS|Postgraduate",
                "MSENG|MASTER OF SCIENCE (ENGINEERING)|Postgraduate",
                "MS|MASTER OF SURGERY|Postgraduate",
                "MD|DOCTORATE IN MEDICINE|Postgraduate",
                "MEd|MASTER OF EDUCATION|Postgraduate",
                "B Ed|BACHELOR OF EDUCATION|Undergraduate",
                "PHD|DOCTORATE IN PHILOSOPHY|Doctorate / Research",
                "MPHIL|MASTER OF PHILOSOPHY|Doctorate / Research",
                "PGD|POST GRADUATE DIPLOMA|Post Graduate Diploma",
                "OTHER|OTHER|Other"
            };

            foreach (var item in items)
            {
                var parts = item.Split('|');
                using (OracleCommand cmd = new OracleCommand("INSERT INTO EDU_QUAL_MAIN (QUAL_CODE, QUAL_NAME, QUAL_CATEGORY, IS_ACTIVE) VALUES (:C, :N, :CAT, 1)", conn))
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
                Tuple.Create("BE", "COMPS", "Computer Science & Engineering"),
                Tuple.Create("BE", "COMPE", "Computer Engineering"),
                Tuple.Create("BE", "IT", "Information Technology"),
                Tuple.Create("BE", "SOFTE", "Software Engineering"),
                Tuple.Create("BE", "AI", "Artificial Intelligence & Data Science"),
                Tuple.Create("BE", "E&C", "Electronics & Communication"),
                Tuple.Create("BE", "ELCL", "Electrical & Electronics"),
                Tuple.Create("BE", "ELTE", "Electronics & Telecommunication"),
                Tuple.Create("BE", "DMECH", "Mechanical Engineering"),
                Tuple.Create("BE", "CIVIL", "Civil Engineering"),
                Tuple.Create("BE", "AERO", "Aeronautical Engineering"),
                Tuple.Create("BE", "AROSE", "Aerospace Engineering"),
                Tuple.Create("BE", "INDST", "Industrial Engineering"),
                Tuple.Create("BE", "INSTR", "Instrumentation Engineering"),
                Tuple.Create("BE", "CHEMI", "Chemical Engineering"),
                Tuple.Create("BE", "METAL", "Metallurgy & Material Science"),
                Tuple.Create("BE", "AUTMO", "Automobile Engineering"),
                Tuple.Create("BE", "RACE", "Radar & Communication Engineering"),
                Tuple.Create("BE", "GENENG", "General Engineering"),
                Tuple.Create("BE", "OTHER", "Other Engineering Stream"),

                // B.Tech
                Tuple.Create("BTECH", "COMPS", "Computer Science & Engineering"),
                Tuple.Create("BTECH", "IT", "Information Technology"),
                Tuple.Create("BTECH", "E&C", "Electronics & Communication"),
                Tuple.Create("BTECH", "ELCL", "Electrical & Electronics"),
                Tuple.Create("BTECH", "DMECH", "Mechanical Engineering"),
                Tuple.Create("BTECH", "CIVIL", "Civil Engineering"),
                Tuple.Create("BTECH", "GENENG", "General Engineering"),
                Tuple.Create("BTECH", "OTHER", "Other B.Tech Stream"),

                // M.Tech / ME
                Tuple.Create("MTECH", "COMPS", "Computer Science & Engineering"),
                Tuple.Create("MTECH", "SOFTE", "Software Engineering"),
                Tuple.Create("MTECH", "E&C", "Digital Electronics & Communication"),
                Tuple.Create("MTECH", "RACE", "Radar & Communication"),
                Tuple.Create("MTECH", "VLSI", "VLSI Design"),
                Tuple.Create("MTECH", "DMECH", "Machine Design / Thermal"),
                Tuple.Create("MTECH", "POE", "Power Electronics"),
                Tuple.Create("MTECH", "DSP", "Digital Signal Processing"),
                Tuple.Create("MTECH", "GENENG", "General M.Tech"),
                Tuple.Create("MTECH", "OTHER", "Other M.Tech Specialization"),

                Tuple.Create("ME", "COMPS", "Computer Science"),
                Tuple.Create("ME", "E&C", "Electronics & Communication"),
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
                Tuple.Create("MTRI", "NA", "All Subjects (General Matriculation)"),
                Tuple.Create("MTRI", "OTHER", "Other Board Specialization"),
                Tuple.Create("BMTRI", "NA", "General (Below 10th)"),

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
                Tuple.Create("DENG", "E&C", "Electronics & Communication"),
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
                "25|Jawaharlal Nehru Technological University Kakinada (JNTUK)",
                "26|Jawaharlal Nehru Technological University Anantapur (JNTUA)",
                "27|Acharya Nagarjuna University, Guntur",
                "28|Osmania University, Hyderabad",
                "30|University of Hyderabad (UoH / Central University), Hyderabad",
                "33|University of Madras, Chennai",
                "38|Cochin University of Science and Technology (CUSAT), Kochi",
                "40|Maulana Azad National Institute of Technology (MANIT), Bhopal",
                "43|Madurai Kamaraj University (MKU), Madurai",
                "44|Jawaharlal Nehru University (JNU), New Delhi",
                "45|University of Delhi (DU), New Delhi",
                "48|University of Kerala, Thiruvananthapuram",
                "49|National Institute of Technology (NIT / REC), Hamirpur",
                "50|Mahatma Gandhi University, Kerala",
                "51|Sardar Vallabhbhai National Institute of Technology (SVNIT), Surat",
                "55|Rajiv Gandhi Proudyogiki Vishwavidyalaya (RGPV), Bhopal",
                "56|Indian Institute of Technology (IIT), Bombay",
                "61|Utkal University, Bhubaneswar",
                "65|University of Calicut, Malappuram",
                "68|Motilal Nehru National Institute of Technology (MNNIT), Allahabad",
                "71|Savitribai Phule Pune University (SPPU / University of Pune)",
                "73|University of Calcutta (Calcutta University), Kolkata",
                "74|Deen Dayal Upadhyaya Gorakhpur University (DDU), Gorakhpur",
                "78|University of Lucknow, Lucknow",
                "84|Aligarh Muslim University (AMU), Aligarh",
                "86|National Institute of Technology (NIT / REC), Rourkela",
                "89|Indira Gandhi National Open University (IGNOU), New Delhi",
                "95|National Institute of Technology (NIT / REC), Durgapur",
                "96|Mahatma Jyotiba Phule Rohilkhand University (MJPRU), Bareilly",
                "102|Dr. B. R. Ambedkar National Institute of Technology (NIT), Jalandhar",
                "103|Jamia Millia Islamia (JMI), New Delhi",
                "109|Panjab University, Chandigarh",
                "119|North-Eastern Hill University (NEHU), Shillong",
                "125|Patna University, Patna",
                "129|Visvesvaraya Technological University (VTU), Belagavi",
                "140|National Institute of Technology (NIT), Jamshedpur",
                "146|National Institute of Technology (NIT), Kurukshetra",
                "151|ABV-Indian Institute of Information Technology and Management (ABV-IIITM), Gwalior",
                "156|National Institute of Technology (NIT), Raipur",
                "158|National Institute of Technology (NIT), Patna",
                "163|Indian Institute of Information Technology (IIIT), Allahabad",
                "166|Karnataka School Examination and Assessment Board (KSEEB / KSEAB), Bengaluru",
                "167|Department of Pre-University Education (PUE), Karnataka",
                "172|West Bengal Board of Secondary Education (WBBSE)",
                "173|West Bengal Council of Higher Secondary Education (WBCHSE)",
                "181|State Board of Technical Education and Training (SBTET), Tamil Nadu",
                "204|Bihar School Examination Board (BSEB), Patna",
                "219|West Bengal State Council of Technical and Vocational Education (WBSCTE), Kolkata",
                "221|Central Board of Secondary Education (CBSE), New Delhi",
                "223|Indian Institute of Technology (IIT), Gandhinagar",
                "226|Indian Institute of Technology (IIT), Patna",
                "264|Jawaharlal Nehru Technological University (JNTU), Hyderabad",
                "274|National Council for Vocational Training (NCVT)",
                "275|Indian Institute of Technology (IIT), Jodhpur",
                "278|Indian Institute of Management (IIM), Lucknow",
                "281|Central University of Rajasthan, Ajmer",
                "283|National Institute of Technology (NIT), Agartala",
                "284|Dr. A.P.J. Abdul Kalam Technical University (AKTU / UPTU), Lucknow",
                "289|Indian Institute of Technology (IIT), Indore",
                "290|National Institute of Technology (NIT), Delhi",
                "297|Maharashtra State Board of Secondary and Higher Secondary Education (MSBSHSE)",
                "298|National Institute of Technology (NIT), Srinagar",
                "304|Board of Secondary Education, Assam (SEBA)",
                "307|Indian Institute of Technology (IIT), Hyderabad",
                "310|Indian Institute of Technology (IIT), Mandi",
                "323|Indian Institute of Technology (BHU), Varanasi",
                "325|Indian Institute of Technology (IIT), Ropar",
                "338|Indian Institute of Technology (IIT), Bhubaneswar",
                "340|Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)",
                "342|Uttarakhand Board of Technical Education, Roorkee (UBTER)",
                "344|Assam Higher Secondary Education Council (AHSEC)",
                "549|Mangalore Institute of Technology and Engineering (MITE), Moodabidri",
                "550|Other Recognized University / Board"
            };

            foreach (var u in univs)
            {
                var p = u.Split('|');
                using (OracleCommand cmd = new OracleCommand("INSERT INTO EDU_UNIVERSITY (UNIV_ID, UNIV_NAME, IS_ACTIVE) VALUES (:ID, :NAME, 1)", conn))
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
                "1|Manufacturing Technology|Mechanical",
                "2|Microwave and Radar Electronics|Electronics",
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
                "19|Computer Applications|Computer Science",
                "20|Commerce|Commerce",
                "22|Management|Management",
                "51|Software Systems|Computer Science",
                "92|Science|Science",
                "93|General|General",
                "94|Mathematics & Science|General",
                "99|Other|General"
            };

            foreach (var s in subs)
            {
                var p = s.Split('|');
                using (OracleCommand cmd = new OracleCommand("INSERT INTO EDU_SUBJECT (SUBJECT_ID, SUBJECT_NAME, SUBJECT_GROUP, IS_ACTIVE) VALUES (:ID, :NAME, :GRP, 1)", conn))
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
            // Core mapping from legacy App 2 QFNID to new Qualification code
            Tuple<int, string, string, string, string, string>[] maps = new Tuple<int, string, string, string, string, string>[]
            {
                Tuple.Create(1, "B.Sc", "B.Sc", "BSC", "GENSC", "Bachelor of Science"),
                Tuple.Create(2, "BE", "BE", "BE", "GENENG", "Bachelor of Engineering"),
                Tuple.Create(3, "ME", "ME", "ME", "GENENG", "Master of Engineering"),
                Tuple.Create(4, "M.Sc", "M.Sc", "MSC", "GENSC", "Master of Science"),
                Tuple.Create(5, "AMIETE", "BE", "AMIE", "GENENG", "AMIETE to AMIE"),
                Tuple.Create(6, "B.Tech", "BE", "BTECH", "GENENG", "Bachelor of Technology"),
                Tuple.Create(7, "M.Tech", "ME", "MTECH", "GENENG", "Master of Technology"),
                Tuple.Create(8, "AMIE", "BE", "AMIE", "GENENG", "AMIE/IETE"),
                Tuple.Create(9, "MCA", "MCA", "MCA", "COMPS", "Master of Computer Applications"),
                Tuple.Create(10, "MBA", "MBA", "MBA", "MANG", "Master of Business Administration"),
                Tuple.Create(11, "Ph.D", "Ph.D", "PHD", "RESEARCH", "Doctorate in Philosophy"),
                Tuple.Create(12, "M.Phil", "M.Phil", "MPHIL", "RESEARCH", "Master of Philosophy"),
                Tuple.Create(13, "Diploma", "DIP", "DIPL", "GENERAL", "Diploma"),
                Tuple.Create(14, "M.Li.Sc", "M.Li.Sc", "MLIS", "BLIB", "Library Science"),
                Tuple.Create(15, "B.Com", "B.Com", "BCOM", "COM", "Bachelor of Commerce"),
                Tuple.Create(16, "BA", "BA", "BA", "ARTS", "Bachelor of Arts"),
                Tuple.Create(17, "MA", "MA", "MA", "ARTS", "Master of Arts"),
                Tuple.Create(18, "M.Com", "M.Com", "MCOM", "COM", "Master of Commerce"),
                Tuple.Create(19, "SSLC/X Class", "SSLC", "MTRI", "NA", "SSLC mapped to 10th"),
                Tuple.Create(20, "PUC/XII", "PUC/XII", "PUC", "PCMB", "PUC mapped to Pre University Course"),
                Tuple.Create(21, "B.Sc. (Engg.)", "B.Sc. (Engg.)", "BSCEN", "GENENG", "B.Sc. Engineering"),
                Tuple.Create(26, "DEE", "Diploma", "DENG", "ELCL", "Diploma in Electrical Engg"),
                Tuple.Create(37, "DME", "Diploma", "DENG", "DMECH", "Diploma in Mechanical Engg"),
                Tuple.Create(41, "I.T.I", "ITI", "ITI", "FIT", "ITI Trade"),
                Tuple.Create(45, "MBBS", "Medicine", "MBBS", "MED", "MBBS"),
                Tuple.Create(48, "DCE", "DCE", "DENG", "CIVIL", "Diploma in Civil Engg"),
                Tuple.Create(50, "LLB", "LAW", "LLB", "LAW", "Bachelor of Laws"),
                Tuple.Create(54, "FITTER", "ITI", "ITI", "FIT", "Fitter ITI"),
                Tuple.Create(55, "BBA", "BBA", "BBA", "MANG", "BBA"),
                Tuple.Create(59, "BBM", "BBM", "BBM", "MANG", "BBM"),
                Tuple.Create(65, "BCA", "BCA", "BCA", "COMPS", "BCA"),
                Tuple.Create(68, "SSC", "SSC", "MTRI", "NA", "SSC (10th)"),
                Tuple.Create(70, "AISSCE", "AISSCE", "SCI", "PCMB", "AISSCE (12th)"),
                Tuple.Create(80, "Below SSLC", "Below SSLC", "BMTRI", "NA", "Below SSLC")
            };

            foreach (var m in maps)
            {
                using (OracleCommand cmd = new OracleCommand(@"
                    INSERT INTO EDU_QUAL_MAPPING 
                    (OLD_QFN_ID, OLD_QFN_NAME, OLD_QFN_GROUP, NEW_QUAL_MAIN_CODE, NEW_QUAL_SUB_CODE, NOTES, IS_ACTIVE)
                    VALUES (:OID, :ONAME, :OGRP, :NMC, :NSC, :NOTES, 1)", conn))
                {
                    cmd.Parameters.Add(new OracleParameter("OID", m.Item1));
                    cmd.Parameters.Add(new OracleParameter("ONAME", m.Item2));
                    cmd.Parameters.Add(new OracleParameter("OGRP", m.Item3));
                    cmd.Parameters.Add(new OracleParameter("NMC", m.Item4));
                    cmd.Parameters.Add(new OracleParameter("NSC", m.Item5));
                    cmd.Parameters.Add(new OracleParameter("NOTES", m.Item6));
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
                SELECT EDU_ID, PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
                       QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
                       YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
                       CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS,
                       SUBMITTED_DATE, CREATED_ON, IS_MIGRATED
                FROM EDU_EMP_EDUCATION
                WHERE PCNO = :PCNO
                ORDER BY IS_HIGHEST DESC, YEAR_PASSED DESC, EDU_ID DESC";

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
                if (IsPlaceholderOrInvalid(collegeName)) throw new ArgumentException("College / School Name cannot be empty or placeholder ('*').");
                if (IsPlaceholderOrInvalid(gradeDivision)) throw new ArgumentException("Grade / Division cannot be empty or placeholder ('*').");
                if (IsInvalidSpecialization(specialization)) throw new ArgumentException("Specialization cannot be empty or placeholder ('*'). ('N/A' is allowed).");
                if (IsPlaceholderOrInvalid(cgpaMarks)) throw new ArgumentException("Score (Percentage / CGPA / Marks) cannot be empty or placeholder ('*').");
            }

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
                if (IsPlaceholderOrInvalid(inst))
                {
                    missingFields.Add("University / Board / Institute");
                }
                if (IsPlaceholderOrInvalid(college))
                {
                    missingFields.Add("College / School Name");
                }
                if (IsInvalidSpecialization(spec))
                {
                    missingFields.Add("Specialization / Major Subject");
                }
                if (IsPlaceholderOrInvalid(cgpa))
                {
                    missingFields.Add("Score (Percentage / CGPA / Marks)");
                }
                if (IsPlaceholderOrInvalid(grade))
                {
                    missingFields.Add("Grade / Division");
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

        public static DataTable GetAdminEducationOverview(string search, string statusFilter, string qualFilter, string divisionFilter)
        {
            string query = @"
                SELECT EDU_ID, PCNO, EMP_NAME, DESIGNATION, DIVISION_NAME,
                       QUAL_MAIN_CODE, QUAL_MAIN_NAME, QUAL_SUB_CODE, QUAL_SUB_NAME,
                       YEAR_PASSED, INSTITUTE_NAME, COLLEGE_NAME, SPECIALIZATION,
                       CGPA_MARKS, GRADE_DIVISION, IS_HIGHEST, REMARKS, STATUS,
                       SUBMITTED_DATE, CREATED_ON, IS_MIGRATED
                FROM EDU_EMP_EDUCATION
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
            if (dtSub != null)
            {
                foreach (DataRow r in dtSub.Rows)
                {
                    string subCode = r["SUB_CODE"].ToString().Trim();
                    string subName = r["SUB_NAME"].ToString().Trim();
                    string mainCode = r["MAIN_QUAL_CODE"] != DBNull.Value ? r["MAIN_QUAL_CODE"].ToString().Trim() : "";
                    subList.Add(new { code = subCode, name = subName, main = mainCode });
                }
            }
            subList.Add(new { code = "GENERAL", name = "General", main = "" });
            subList.Add(new { code = "OTHER", name = "Other (Specify in Specialization)", main = "" });

            _cachedSubQualJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(subList);
            return _cachedSubQualJson;
        }

        public static void InvalidateSubQualificationsCache()
        {
            _cachedSubQualJson = null;
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
                string sqlCol = "SELECT COLLEGE_NAME FROM EDU_COLLEGE WHERE IS_ACTIVE = 1 ORDER BY COLLEGE_NAME ASC";
                DataTable dtCol = ExecuteQuery(sqlCol);
                if (dtCol != null && dtCol.Rows.Count > 0)
                {
                    foreach (DataRow r in dtCol.Rows)
                    {
                        string name = r["COLLEGE_NAME"].ToString().Trim();
                        if (!string.IsNullOrEmpty(name) && seen.Add(name))
                        {
                            dt.Rows.Add(name);
                        }
                    }
                    DataView dvMaster = dt.DefaultView;
                    dvMaster.Sort = "COLLEGE_NAME ASC";
                    return dvMaster.ToTable();
                }
            }
            catch { }

            // 2. Fallback to standard catalog only if EDU_COLLEGE table is not yet populated
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

                    // University resolution
                    string instName = r["COLLEGENAME"] != DBNull.Value ? r["COLLEGENAME"].ToString() : "Not Specified";
                    if (r["UNIVINSTID"] != DBNull.Value)
                    {
                        int uid = Convert.ToInt32(r["UNIVINSTID"]);
                        if (univDict.ContainsKey(uid)) instName = univDict[uid];
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

                    int yearPassed = r["YEARPASSED"] != DBNull.Value ? Convert.ToInt32(r["YEARPASSED"]) : 2000;
                    string grade = r["GRADE"] != DBNull.Value ? r["GRADE"].ToString() : "Pass";
                    string pct = r["PERCENTAGE"] != DBNull.Value ? r["PERCENTAGE"].ToString() : "N/A";
                    string college = r["COLLEGENAME"] != DBNull.Value ? r["COLLEGENAME"].ToString() : "";

                    // If record contains placeholder values like '*', force to Draft so employee must verify and select proper values
                    string rowStatus = defaultStatus;
                    if (IsPlaceholderOrInvalid(college) || IsPlaceholderOrInvalid(instName) ||
                        IsInvalidSpecialization(specName) || IsPlaceholderOrInvalid(grade) ||
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
                        new OracleParameter("GRD", grade),
                        new OracleParameter("HIGH", isHighest ? 1 : 0),
                        new OracleParameter("REM", "Migrated from " + legTable + " (CTREDN: " + ctredn + ")"),
                        new OracleParameter("ST", rowStatus),
                        new OracleParameter("USR", executedBy),
                        new OracleParameter("CTR", ctredn));

                    totalMigrated++;
                }

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
