# Employee Education Management System
## Complete Deployment & Migration Guide

> **Target Audience:** Developer / IT Admin deploying this project to a new company PC or server.

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Prerequisites](#2-prerequisites)
3. [File/Folder Structure](#3-filefolder-structure)
4. [SQL Scripts — Explained One by One](#4-sql-scripts--explained-one-by-one)
   - [01_Schema_DDL.sql](#41-01_schema_ddlsql--the-foundation)
   - [02_Seed_MasterData.sql](#42-02_seed_masterdatasql--the-data-you-need-to-start)
   - [03_Migration_Procedure.sql](#43-03_migration_proceduresql--importing-old-hr-data)
   - [04_Auto_Sync_Trigger.sql](#44-04_auto_sync_triggersql--keeping-data-in-sync-forever)
5. [Step-by-Step: Fresh Deployment on a New PC](#5-step-by-step-fresh-deployment-on-a-new-pc)
6. [Configuring Web.config for Your Environment](#6-configuring-webconfig-for-your-environment)
7. [Running the Application](#7-running-the-application)
8. [How Migration Works Inside the Application](#8-how-migration-works-inside-the-application)
9. [Admin Panel & Managing Data](#9-admin-panel--managing-data)
10. [Troubleshooting Common Issues](#10-troubleshooting-common-issues)

---

## 1. Project Overview

This is an **ASP.NET Web Forms** application built on **.NET Framework 4.5**. It allows employees to record, save as draft, and submit their educational qualifications. Admins can view, manage, and run migrations from the old HR system.

**Tech Stack:**
| Layer | Technology |
|---|---|
| Frontend | ASP.NET Web Forms (ASPX) + Vanilla CSS + JS |
| Backend | C# (.NET 4.5) |
| Database | Oracle 11g R2 / 12c (XE edition supported) |
| Auth | LDAP / Active Directory (with offline bypass option) |
| ORM | Oracle.ManagedDataAccess.Client (no Entity Framework) |

---

## 2. Prerequisites

Before copying the project to a new PC, make sure the following are installed:

| Software | Version | Where to Get |
|---|---|---|
| **Oracle Database XE** | 11g R2 or 12c | https://www.oracle.com/database/technologies/xe-downloads.html |
| **Visual Studio** | 2015 or later | https://visualstudio.microsoft.com/ |
| **IIS or IIS Express** | Comes with Visual Studio | Built-in |
| **.NET Framework 4.5** | Target framework | Comes with VS / Windows Update |
| **Oracle Data Access** | Oracle.ManagedDataAccess NuGet | Already in project's `bin/` folder |

> **Note:** Oracle XE is free. It is enough for this application unless you have thousands of employees.

---

## 3. File/Folder Structure

```
Education_Details/
│
├── Scripts/                       ← ALL SQL files go here (run these in Oracle)
│   ├── 01_Schema_DDL.sql          ← Step 1: Create tables, sequences, indexes
│   ├── 02_Seed_MasterData.sql     ← Step 2: Insert qualification lists, universities etc.
│   ├── 03_Migration_Procedure.sql ← Step 3 (optional): Import old HR data
│   └── 04_Auto_Sync_Trigger.sql   ← Step 4 (optional): Live sync from old HR table
│
├── Web.config                 ← Database connection & app settings (EDIT THIS)
├── Site.Master                ← Main layout: header, sidebar, shared scripts
├── Login.aspx                 ← Login page
├── EmployeeEducation.aspx     ← Main employee-facing page
├── AdminPanel/                ← Admin pages
│
├── Static/
│   ├── css/custom-education.css  ← Main stylesheet
│   └── js/education-app.js       ← Client-side JS logic
│
└── bin/
    └── Oracle.ManagedDataAccess.dll  ← Oracle driver (already included)
```

---

## 4. SQL Scripts — Explained One by One

These four scripts are located in `Education_Details/Scripts/`. You must run them **in order** (01 → 02 → 03 → 04) using SQL*Plus, Oracle SQL Developer, or any Oracle-compatible tool.

---

### 4.1 `01_Schema_DDL.sql` — The Foundation

**What it does:** Creates the entire database structure from scratch — all tables, sequences, triggers, and indexes.

**Run when:** First time only (fresh installation on any PC).

**What it creates:**

| Object | Name | Purpose |
|---|---|---|
| Table | `EDU_ADMIN_USERS` | List of admin users (by PCNO) |
| Table | `EDU_QUAL_MAIN` | Master list of main qualifications (e.g., B.E., M.Tech, ITI) |
| Table | `EDU_QUAL_SUB` | Sub-qualifications under each main qualification |
| Table | `EDU_UNIVERSITY` | List of universities and institutes |
| Table | `EDU_SUBJECT` | List of subjects/specializations |
| Table | `EDU_EMP_EDUCATION` | **Core table** — stores each employee's education records |
| Table | `EDU_QUAL_MAPPING` | Maps old HR qualification IDs to new qualification codes |
| Table | `EDU_MIGRATION_LOG` | Logs every time a migration is run |
| Table | `EDU_AUDIT_LOG` | Logs every Save Draft, Submit, Admin Edit action |
| Sequences | `SEQ_EDU_*` | Auto-increment ID generators (Oracle equivalent of IDENTITY) |
| Triggers | `TRG_*_BI` | Before-Insert triggers that auto-assign IDs using sequences |
| Indexes | `IDX_EDU_EMP_*` | Performance indexes on PCNO, STATUS, QUAL_MAIN_CODE |

**Key design decisions in `EDU_EMP_EDUCATION`:**

```sql
STATUS VARCHAR2(20) DEFAULT 'Draft' NOT NULL  -- 'Draft' or 'Submitted'
IS_MIGRATED NUMBER(1) DEFAULT 0               -- 1 if imported from old HR system
OLD_CTREDN NUMBER                             -- Keeps track of original HR record ID
```

- `STATUS = 'Draft'` → Employee can still edit the record
- `STATUS = 'Submitted'` → Record is locked, cannot be edited
- `IS_MIGRATED = 1` → Record came from the old `HR_EMP_EDUCATION` table

**Safety note:** The commented-out `DROP TABLE` lines at the top are intentionally commented. **Do not uncomment them** on a production database — they will delete all data.

---

### 4.2 `02_Seed_MasterData.sql` — The Data You Need to Start

**What it does:** Fills all the master tables with real data so dropdowns in the application are populated.

**Run when:** Right after `01_Schema_DDL.sql`. Also safe to re-run anytime — it uses `MERGE` statements which only insert if the record does not already exist (no duplicates).

**What it inserts:**

| Section | What's Inserted | Approx Count |
|---|---|---|
| Admin Users | Default admin (PCNO: 1001) | 1 record |
| Main Qualifications (`EDU_QUAL_MAIN`) | B.E., M.Tech, ITI, MBA, MBBS, 10th, 12th, Diploma, PhD, etc. | 100+ qualifications |
| Sub-Qualifications (`EDU_QUAL_SUB`) | Branches under each — e.g., B.E. → Civil, Mechanical, CS | Hundreds |
| Universities (`EDU_UNIVERSITY`) | List of universities and boards (state-wise) | Hundreds |
| Subjects (`EDU_SUBJECT`) | Specialization subjects | Many records |
| Qualification Mapping (`EDU_QUAL_MAPPING`) | Maps old HR `QFNID` numbers to new qualification codes | Matches old HR data |

**Why `MERGE` instead of `INSERT`?**

A `MERGE` statement checks if a record with the same key already exists:
- If it **does not exist** → INSERT
- If it **already exists** → Do nothing (or update if defined)

This means you can run `02_Seed_MasterData.sql` multiple times without duplicating data. This is very useful during upgrades.

**Important for deployment:** This file is comprehensive (~198KB, 1,328+ lines) containing 140+ UGC/AICTE main qualifications, linked degree-specific sub-qualifications, and 548+ Central/State universities and examination boards. Run it completely — do not stop midway. In SQL Developer, use **Run Script (F5)** not **Run Statement (Ctrl+Enter)**.

---

### 4.3 `03_Migration_Procedure.sql` — Importing Old HR Data

**What it does:** Creates a PL/SQL stored procedure (`PKG_EDU_MIGRATION.SP_RUN_MIGRATION`) that reads records from the old HR table (`HR_EMP_EDUCATION`) and copies them into the new `EDU_EMP_EDUCATION` table in the correct format.

**Run when:**
- After running scripts 01 and 02
- **Only run if** you have an existing `HR_EMP_EDUCATION` table in your Oracle database (i.e., you are migrating from an older HR system)
- If this is a completely fresh installation with no old data, you can **skip this script**

**How it works (step by step inside the procedure):**

```
For each row in HR_EMP_EDUCATION:
  1. Check if employee PCNO exists in hrdata.empdetails (skip import if not found)
  2. Check if already migrated (skip if OLD_CTREDN already exists)
  3. Look up the old QFNID in EDU_QUAL_MAPPING to get the new QUAL_MAIN_CODE
  4. Resolve the university name from EDU_UNIVERSITY using UNIVINSTID
  5. Resolve subject/specialization from EDU_SUBJECT using SUBJECTID:
     - Legacy HR_EMP_EDUCATION has no separate SUB_QUALIFICATION column; it used SUBJECTID.
     - Resolves SUBJECT_NAME into SPECIALIZATION.
     - Intelligently checks EDU_QUAL_SUB matching the subject name to resolve QUAL_SUB_CODE and canonical QUAL_SUB_NAME (e.g. MCA + "Computer Applications" -> SUB_CODE 'CA').
     - Falls back to mapping default or 'GENERAL' if no exact sub-qualification is registered.
  6. Fetch employee name, designation, division from hrdata.empdetails
  7. Insert into EDU_EMP_EDUCATION with IS_MIGRATED = 1
  8. Log the result in EDU_MIGRATION_LOG
```

**How to execute the migration from Oracle SQL Developer:**

```sql
DECLARE
    v_read     NUMBER;
    v_migrated NUMBER;
    v_skipped  NUMBER;
    v_msg      VARCHAR2(4000);
BEGIN
    PKG_EDU_MIGRATION.SP_RUN_MIGRATION(
        p_executed_by    => 'YOUR_PCNO',
        p_default_status => 'Submitted',
        p_out_total_read => v_read,
        p_out_migrated   => v_migrated,
        p_out_skipped    => v_skipped,
        p_out_message    => v_msg
    );
    DBMS_OUTPUT.PUT_LINE('Message: ' || v_msg);
    DBMS_OUTPUT.PUT_LINE('Read: ' || v_read || ', Migrated: ' || v_migrated || ', Skipped: ' || v_skipped);
END;
/
```

**Parameters explained:**

| Parameter | What to Put |
|---|---|
| `p_executed_by` | Your PCNO or username (for audit log) |
| `p_default_status` | `'Submitted'` = import as locked records; `'Draft'` = employees can still edit them |
| `p_out_*` | Output variables — the procedure fills these automatically |

**It is safe to re-run:** Records that are already migrated (matched by `OLD_CTREDN`) are skipped automatically.

**Check migration results:**
```sql
SELECT * FROM EDU_MIGRATION_LOG ORDER BY MIGRATION_DATE DESC;
```

---

### 4.4 `04_Auto_Sync_Trigger.sql` — Keeping Data in Sync Forever

**What it does:** Creates a database trigger (`TRG_SYNC_HR_EMP_EDU`) on the **old** `HR_EMP_EDUCATION` table. After this trigger is active, any time someone inserts or updates a record in the old HR system, it **automatically reflects in the new `EDU_EMP_EDUCATION` table** in real time.

**Run when:**
- Only if the old HR system (`HR_EMP_EDUCATION`) is still actively being used alongside this new application
- **Do not run** if HR_EMP_EDUCATION no longer exists or is no longer maintained

**Why this is useful:**

If your HR department still updates education records in the old system while employees start using this new system, this trigger ensures both tables stay in sync without any manual effort.

**How it works:**

```
AFTER INSERT OR UPDATE on HR_EMP_EDUCATION (fires for each row):
  1. Check if employee PCNO exists in hrdata.empdetails (if not found, do not import - exit immediately)
  2. Resolve qualification code using EDU_QUAL_MAPPING
  3. Resolve university name using EDU_UNIVERSITY
  4. Resolve subject/specialization using EDU_SUBJECT & map to EDU_QUAL_SUB (resolves both SPECIALIZATION and QUAL_SUB_CODE/QUAL_SUB_NAME)
  5. Fetch employee details from hrdata.empdetails
  6. If record already exists in EDU_EMP_EDUCATION → UPDATE it (preserves existing status)
  7. If record does not exist → INSERT it with STATUS = 'Draft' (allows employee to review, verify, and submit)
```

**Important difference from Script 03:**

| | Script 03 (Procedure) | Script 04 (Trigger) |
|---|---|---|
| When it runs | You manually call it once | Runs automatically on every INSERT/UPDATE |
| Use case | One-time bulk import | Ongoing live sync |
| Can be stopped | Yes, just don't call it | Only by disabling/dropping the trigger |

**To disable the trigger temporarily:**
```sql
ALTER TRIGGER TRG_SYNC_HR_EMP_EDU DISABLE;
-- To re-enable:
ALTER TRIGGER TRG_SYNC_HR_EMP_EDU ENABLE;
```

---

## 5. Step-by-Step: Fresh Deployment on a New PC

### Step 1 — Install Oracle XE
1. Download Oracle XE from https://www.oracle.com/database/technologies/xe-downloads.html
2. Install it. During installation, set a password for the `SYSTEM` user. **Remember this password.**
3. Verify it is running: Open Oracle SQL Developer → Connect with `system / <your password>` → `localhost:1521/xe`

### Step 2 — Copy the Project
1. Copy the entire `Education_Details/` folder to the new PC (e.g., `C:\Projects\Education_Details\`)
2. Make sure all subfolders are copied including `bin/`, `Static/`, `Scripts/`

### Step 3 — Run the SQL Scripts (in order)

Open Oracle SQL Developer and connect as `system`:

```
1. Open 01_Schema_DDL.sql       → Press F5 (Run Script) → Wait for "Table created", "Trigger created"
2. Open 02_Seed_MasterData.sql  → Press F5 → Wait for all MERGE statements (1-2 minutes)
3. Open 03_Migration_Procedure.sql → Press F5 → You should see "Package created"
4. Open 04_Auto_Sync_Trigger.sql   → Press F5 → Only if HR_EMP_EDUCATION table exists
```

> **Always use F5 (Run Script), not Ctrl+Enter (Run Statement)** for these files.

### Step 4 — Update Web.config
Edit the connection string to match your Oracle credentials (see Section 6).

### Step 5 — Open in Visual Studio
1. Open Visual Studio → File → Open → Project/Solution
2. Browse to `Education_Details/` and open the `.sln` file
3. Visual Studio will load the project

### Step 6 — Run the Application
Press **F5** in Visual Studio. IIS Express will start and the browser will open automatically.

---

## 6. Configuring `Web.config` for Your Environment

Open `Web.config` in the project root and update the following:

### Database Connection

```xml
<connectionStrings>
  <add name="EducationDB"
    connectionString="User Id=system;Password=YOUR_PASSWORD;Data Source=//127.0.0.1:1521/xe;"
    providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>
```

| Setting | What to Change |
|---|---|
| `User Id=system` | Your Oracle username (usually `system` for XE) |
| `Password=root` | Change to your Oracle password set during XE installation |
| `Data Source=//127.0.0.1:1521/xe` | Replace `127.0.0.1` with server IP if Oracle is remote |

### App Settings

```xml
<appSettings>
  <add key="AllowOfflineBypass" value="true" />
  <!-- Intranet photos: false for home/offline testing, true for live company intranet -->
  <add key="EnableIntranetPhotos" value="false" />
  <add key="ADConnectionPath" value="LDAP://192.168.0.106/DC=ad01,DC=yajnesh,DC=com" />
  <add key="AdminPCNOs" value="1001" />
</appSettings>
```

| Key | Value | When to Change |
|---|---|---|
| `AllowOfflineBypass` | `true` | Set `false` in production to enforce AD login |
| `EnableIntranetPhotos` | `false` | Set to `true` when running on the company network/intranet to fetch live employee photos from `windflower.lrde.com/empphotos/`. Keep `false` when testing from home or outside the intranet so the browser never hangs on unreachable TCP connections. |
| `ADConnectionPath` | LDAP path | Change IP to your company's Active Directory server |
| `AdminPCNOs` | `1001` | Replace with your actual admin PCNO(s), comma-separated |

---

## 7. Running the Application

### Option A — Via Visual Studio (Development)
- Press **F5** to start with debugging
- Press **Ctrl+F5** to start without debugging (faster)
- IIS Express starts on a port like `http://localhost:51240`

### Option B — Via IIS (Production)
1. Open **IIS Manager**
2. Create a new site pointing to the `Education_Details/` folder
3. Set Application Pool to **.NET CLR v4.0**, **Integrated** pipeline mode
4. Browse to `http://localhost/` or `http://your-server-ip/`

### First Login
- Login page: `Login.aspx`
- If `AllowOfflineBypass = true`: Enter any valid PCNO to log in (for testing)
- If `AllowOfflineBypass = false`: Must authenticate against Active Directory

---

## 8. How Migration Works Inside the Application

The Admin Panel has a **"Run Migration"** button. End-to-end flow:

```
[Admin clicks "Run Migration" in browser]
         ↓
[AdminPanel.aspx.cs calls PKG_EDU_MIGRATION.SP_RUN_MIGRATION via ADO.NET]
         ↓
[Oracle reads every row from HR_EMP_EDUCATION]
         ↓
  For each row:
    - Already migrated? → SKIP
    - Not yet migrated?
        → Look up qualification mapping (EDU_QUAL_MAPPING)
        → Look up university name (EDU_UNIVERSITY)
        → Look up specialization (EDU_SUBJECT)
        → Fetch employee details (hrdata.empdetails)
        → INSERT into EDU_EMP_EDUCATION with IS_MIGRATED = 1
         ↓
[Migration log written to EDU_MIGRATION_LOG]
         ↓
[Result (read/migrated/skipped) returned to browser as toast notification]
```

**Useful verification queries:**

```sql
-- View all migrated records
SELECT PCNO, EMP_NAME, QUAL_MAIN_NAME, STATUS, IS_MIGRATED
FROM EDU_EMP_EDUCATION WHERE IS_MIGRATED = 1;

-- See migration run history
SELECT * FROM EDU_MIGRATION_LOG ORDER BY MIGRATION_DATE DESC;

-- Full audit trail
SELECT * FROM EDU_AUDIT_LOG ORDER BY ACTION_DATE DESC;
```

---

## 9. Admin Panel & Managing Data

### Who Is an Admin?
Any PCNO listed in `Web.config` → `AdminPCNOs`, or inserted into `EDU_ADMIN_USERS` table.

To add a new admin via SQL:
```sql
INSERT INTO EDU_ADMIN_USERS (PCNO, USER_NAME, ROLE, IS_ACTIVE)
VALUES ('2005', 'Rajesh Kumar', 'Admin', 1);
COMMIT;
```

### Adding New Qualifications
Use the Admin Panel in the application to add new qualifications. They immediately appear in employee dropdowns. No SQL needed.

### Changing Record Status: Submitted ↔ Draft (Admin Only)
Employees cannot edit submitted records. Only an admin can unlock or re-lock them. Admins can do this directly from the **Admin Dashboard** table with one click, or via SQL:

**1. Revert from Submitted to Draft (Unlock for employee to edit):**
```sql
UPDATE EDU_EMP_EDUCATION
SET STATUS = 'Draft', SUBMITTED_DATE = NULL, MODIFIED_ON = SYSDATE, MODIFIED_BY = 'ADMIN_PCNO'
WHERE EDU_ID = <edu_id_here>;
COMMIT;

INSERT INTO EDU_AUDIT_LOG (PCNO, EDU_ID, ACTION_TYPE, ACTION_BY, COMMENTS)
VALUES ('<emp_pcno>', <edu_id>, 'AdminRevertToDraft', 'ADMIN_PCNO', 'Reverted for correction');
COMMIT;
```

**2. Change from Draft to Submitted (Lock / Re-lock if reverted by mistake):**
```sql
UPDATE EDU_EMP_EDUCATION
SET STATUS = 'Submitted', SUBMITTED_DATE = SYSDATE, MODIFIED_ON = SYSDATE, MODIFIED_BY = 'ADMIN_PCNO'
WHERE EDU_ID = <edu_id_here>;
COMMIT;

INSERT INTO EDU_AUDIT_LOG (PCNO, EDU_ID, ACTION_TYPE, ACTION_BY, COMMENTS)
VALUES ('<emp_pcno>', <edu_id>, 'AdminSubmitRecord', 'ADMIN_PCNO', 'Locked as Submitted by Admin');
COMMIT;
```

---

## 10. Troubleshooting Common Issues

### "ORA-12541: TNS:no listener"
Oracle service is not running.
```
Windows Services → Start "OracleServiceXE" and "OracleXETNSListener"
```
Or from admin command prompt:
```
net start OracleServiceXE
net start OracleXETNSListener
```

### "ORA-01017: invalid username/password"
Wrong credentials in `Web.config`. Check `User Id` and `Password` match your Oracle setup.

### "Table or view does not exist"
Script `01_Schema_DDL.sql` was not run or had errors. Re-run it and check for errors.

### Dropdown shows no qualifications (empty list)
Script `02_Seed_MasterData.sql` was not run. Run it and verify:
```sql
SELECT COUNT(*) FROM EDU_QUAL_MAIN;
SELECT COUNT(*) FROM EDU_UNIVERSITY;
```

### Migration fails with "HR_EMP_EDUCATION does not exist"
The old HR table doesn't exist on this server. Scripts 03 and 04 are **optional** — skip them if you have no old HR data.

### Login fails even with correct password
`AllowOfflineBypass = false` and Active Directory is not reachable. For testing, set `AllowOfflineBypass = true` in `Web.config`. For production, confirm `ADConnectionPath` IP with your IT team.

### Page takes 20-30 seconds to load or tab keeps spinning outside company intranet
The application was attempting to download employee photos directly from the company intranet server (`windflower.lrde.com/empphotos/`). When outside the company network or VPN, the browser TCP connection attempts time out after 20-30 seconds, causing the browser tab to keep spinning.
**Fix:** Set `<add key="EnableIntranetPhotos" value="false" />` in `Web.config`. The application will immediately use the local default avatar asset and render instantaneously (under 0.1s).

### Page shows old CSS/JS styles
Hard refresh with **Ctrl+Shift+R**. The application uses version strings in asset URLs to prevent caching issues.

---

## Quick Reference: Which Scripts to Run and When

| Scenario | Script 01 | Script 02 | Script 03 | Script 04 |
|---|:---:|:---:|:---:|:---:|
| **Fresh install, no old data** | ✅ Run | ✅ Run | ❌ Skip | ❌ Skip |
| **Fresh install + import old HR data** | ✅ Run | ✅ Run | ✅ Run once | Optional |
| **Upgrade existing installation** | ❌ Skip | ✅ Safe to re-run | ❌ Skip | ❌ Skip |
| **Added new qualifications to seed file** | ❌ Skip | ✅ Safe to re-run | ❌ Skip | ❌ Skip |
| **Old HR system still actively used** | ✅ Done | ✅ Done | ✅ Run once | ✅ Run once |

---

*Documentation for Employee Education Management System — Education_Details project*
