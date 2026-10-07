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
                "1|Anna University",
                "2|Bangalore University",
                "3|Andhra University",
                "4|IIT Kanpur",
                "5|IIT Madras",
                "6|IIT Delhi",
                "7|IISc, Bangalore",
                "11|Benaras Hindu University",
                "12|BITS, Pilani",
                "13|IIT, Kharagpur",
                "14|NIT, Warangal (REC)",
                "16|NIT, Surathkal (KREC)",
                "20|Karnataka University",
                "21|Mysore University",
                "22|Mangalore University",
                "23|Kuvempu University",
                "28|Osmania University",
                "45|Delhi University",
                "48|Kerala University",
                "56|IIT Bombay",
                "129|Visvesvaraya Technological University (VTU), Belgaum",
                "160|CBSE Board",
                "166|KSEEB (Karnataka Secondary Education Board)",
                "167|PUE (Department of Pre-University Education)",
                "188|IIT Kharagpur",
                "221|CBSE Board New Delhi",
                "235|State SSC Board",
                "236|State Intermediate Board",
                "241|Karnataka State Board",
                "248|IGNOU, New Delhi",
                "264|JNTU, Hyderabad",
                "340|ICSE / ISC Board",
                "350|Other Recognized University / Board"
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

        public static DataRow GetEmployeeProfile(string pcno)
        {
            if (string.IsNullOrEmpty(pcno)) return null;

            // Try HRDATA.EMPDETAILS first
            try
            {
                string query = "SELECT PCNO, NAME, DESIGNATION, DIVNAME FROM HRDATA.EMPDETAILS WHERE PCNO = :PCNO AND ROWNUM <= 1";
                DataTable dt = ExecuteQuery(query, new OracleParameter("PCNO", pcno.Trim()));
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
            }
            catch { }

            // Try SYSTEM.EMPDETAILS
            try
            {
                string query = "SELECT PCNO, NAME, DESIGNATION, DIVNAME FROM SYSTEM.EMPDETAILS WHERE PCNO = :PCNO AND ROWNUM <= 1";
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

        public static int SubmitAllEducationRecords(string pcno, string currentUser)
        {
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
            string query = "SELECT QUAL_CODE, QUAL_NAME, QUAL_CATEGORY FROM EDU_QUAL_MAIN";
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

        public static DataTable GetUniversities(bool activeOnly = true)
        {
            string query = "SELECT UNIV_ID, UNIV_NAME FROM EDU_UNIVERSITY";
            if (activeOnly) query += " WHERE IS_ACTIVE = 1";
            query += " ORDER BY UNIV_NAME ASC";
            return ExecuteQuery(query);
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

        public static bool AddSubQualification(string mainCode, string subCode, string subName)
        {
            string sql = "INSERT INTO EDU_QUAL_SUB (MAIN_QUAL_CODE, SUB_CODE, SUB_NAME, IS_ACTIVE) VALUES (:MQ, :SC, :SN, 1)";
            return ExecuteNonQuery(sql,
                new OracleParameter("MQ", string.IsNullOrEmpty(mainCode) ? (object)DBNull.Value : mainCode.Trim().ToUpper()),
                new OracleParameter("SC", subCode.Trim().ToUpper()),
                new OracleParameter("SN", subName.Trim())) > 0;
        }

        public static bool AddUniversity(string name, string stateOrType)
        {
            string sql = "INSERT INTO EDU_UNIVERSITY (UNIV_ID, UNIV_NAME, STATE_OR_TYPE, IS_ACTIVE) VALUES (SEQ_EDU_UNIVERSITY.NEXTVAL, :N, :ST, 1)";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", name.Trim()),
                new OracleParameter("ST", stateOrType ?? "")) > 0;
        }

        public static bool AddSubject(string name, string group)
        {
            string sql = "INSERT INTO EDU_SUBJECT (SUBJECT_ID, SUBJECT_NAME, SUBJECT_GROUP, IS_ACTIVE) VALUES (SEQ_EDU_SUBJECT.NEXTVAL, :N, :GRP, 1)";
            return ExecuteNonQuery(sql,
                new OracleParameter("N", name.Trim()),
                new OracleParameter("GRP", group ?? "")) > 0;
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

        public static bool ToggleAdminUser(string pcno, bool active)
        {
            string sql = "UPDATE EDU_ADMIN_USERS SET IS_ACTIVE = :ACT WHERE PCNO = :PCNO";
            return ExecuteNonQuery(sql,
                new OracleParameter("ACT", active ? 1 : 0),
                new OracleParameter("PCNO", pcno.Trim())) > 0;
        }

        #endregion

        #region Data Migration Engine (HR_EMP_EDUCATION -> EDU_EMP_EDUCATION)

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
                object totObj = ExecuteScalar("SELECT COUNT(*) FROM HR_EMP_EDUCATION");
                legTotal = totObj != null ? Convert.ToInt32(totObj) : 0;

                object empObj = ExecuteScalar("SELECT COUNT(DISTINCT PCNO) FROM HR_EMP_EDUCATION");
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

            EnsureSchema();

            try
            {
                // Fetch legacy records
                string queryOld = "SELECT PCNO, CTREDN, QFNID, UNIVINSTID, COLLEGENAME, SUBJECTID, YEARPASSED, GRADE, PERCENTAGE, HIGHESTQFN FROM HR_EMP_EDUCATION ORDER BY PCNO, CTREDN";
                DataTable dtOld = ExecuteQuery(queryOld);

                if (dtOld == null || dtOld.Rows.Count == 0)
                {
                    return "No records found in HR_EMP_EDUCATION table to migrate.";
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

                // Process migration
                foreach (DataRow r in dtOld.Rows)
                {
                    string pcno = r["PCNO"].ToString();
                    int ctredn = Convert.ToInt32(r["CTREDN"]);

                    // Check if already migrated
                    string checkExists = "SELECT COUNT(*) FROM EDU_EMP_EDUCATION WHERE PCNO = :PCNO AND OLD_CTREDN = :CTREDN";
                    object existsObj = ExecuteScalar(checkExists,
                        new OracleParameter("PCNO", pcno),
                        new OracleParameter("CTREDN", ctredn));

                    if (existsObj != null && Convert.ToInt32(existsObj) > 0)
                    {
                        totalSkipped++;
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

                    // Employee Profile lookup
                    DataRow empProfile = GetEmployeeProfile(pcno);
                    string empName = empProfile != null ? empProfile["NAME"].ToString() : ("Employee " + pcno);
                    string desig = empProfile != null ? empProfile["DESIGNATION"].ToString() : "";
                    string div = empProfile != null ? empProfile["DIVNAME"].ToString() : "";

                    int yearPassed = r["YEARPASSED"] != DBNull.Value ? Convert.ToInt32(r["YEARPASSED"]) : 2000;
                    string grade = r["GRADE"] != DBNull.Value ? r["GRADE"].ToString() : "Pass";
                    string pct = r["PERCENTAGE"] != DBNull.Value ? r["PERCENTAGE"].ToString() : "N/A";
                    string college = r["COLLEGENAME"] != DBNull.Value ? r["COLLEGENAME"].ToString() : "";

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
                        new OracleParameter("CGPA", pct),
                        new OracleParameter("GRD", grade),
                        new OracleParameter("HIGH", isHighest ? 1 : 0),
                        new OracleParameter("REM", "Migrated from HR_EMP_EDUCATION (CTREDN: " + ctredn + ")"),
                        new OracleParameter("ST", defaultStatus),
                        new OracleParameter("USR", executedBy),
                        new OracleParameter("CTR", ctredn));

                    totalMigrated++;
                }

                // Log migration
                string logSql = @"
                    INSERT INTO EDU_MIGRATION_LOG (LOG_ID, MIGRATION_DATE, EXECUTED_BY, TOTAL_READ, TOTAL_MIGRATED, TOTAL_SKIPPED, STATUS, LOG_DETAILS)
                    VALUES (SEQ_EDU_MIG_LOG.NEXTVAL, SYSDATE, :USR, :TR, :TM, :TS, 'COMPLETED', :DET)";

                ExecuteNonQuery(logSql,
                    new OracleParameter("USR", executedBy),
                    new OracleParameter("TR", totalRead),
                    new OracleParameter("TM", totalMigrated),
                    new OracleParameter("TS", totalSkipped),
                    new OracleParameter("DET", "Migration executed successfully. Total Read: " + totalRead + ", Migrated: " + totalMigrated + ", Skipped: " + totalSkipped));

                return "Migration completed successfully! Processed: " + totalRead + " records. Newly Migrated: " + totalMigrated + ", Skipped (Already existed): " + totalSkipped + ".";
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
