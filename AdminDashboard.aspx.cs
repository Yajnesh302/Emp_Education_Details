using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsAuthenticated)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!UserSession.IsAdmin)
            {
                Response.Redirect("EmployeeEducation.aspx");
                return;
            }

            if (!IsPostBack)
            {
                PopulateFilters();
                LoadDashboardStats();
                BindGrid();
            }
        }

        private void PopulateFilters()
        {
            DataTable dtMain = DBHelper.GetMainQualifications(false);
            ddlQualFilter.DataSource = dtMain;
            ddlQualFilter.DataTextField = "QUAL_NAME";
            ddlQualFilter.DataValueField = "QUAL_CODE";
            ddlQualFilter.DataBind();
            ddlQualFilter.Items.Insert(0, new ListItem("All Qualifications", "ALL"));
        }

        private void LoadDashboardStats()
        {
            Dictionary<string, int> stats = DBHelper.GetDashboardStats();
            litStatEmployees.Text = stats["TotalEmployees"].ToString();
            litStatSubmitted.Text = stats["SubmittedCount"].ToString();
            litStatDraft.Text = stats["DraftCount"].ToString();
            litStatMigrated.Text = stats["MigratedCount"].ToString();
        }

        private void BindGrid()
        {
            string search = txtSearch.Text.Trim();
            string status = ddlStatusFilter.SelectedValue;
            string qual = ddlQualFilter.SelectedValue;

            DataTable dt = DBHelper.GetAdminEducationOverview(search, status, qual, "ALL");
            gvAdminEducation.DataSource = dt;
            gvAdminEducation.DataBind();

            if (gvAdminEducation.HeaderRow != null)
            {
                gvAdminEducation.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            litFilteredCount.Text = (dt != null ? dt.Rows.Count : 0).ToString();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void gvAdminEducation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRecord")
            {
                int eduId = Convert.ToInt32(e.CommandArgument);
                OpenRecordForEdit(eduId);
            }
            else if (e.CommandName == "RevertToDraft")
            {
                int eduId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    bool reverted = DBHelper.AdminRevertToDraft(eduId, UserSession.CurrentPCNO, "Unlocked by Administrator from Dashboard");
                    if (reverted)
                    {
                        LoadDashboardStats();
                        BindGrid();
                        ShowAlert("Record #" + eduId + " successfully reverted to Draft mode. The employee can now edit it.", "success");
                    }
                    else
                    {
                        ShowAlert("Could not revert record. Please verify database connectivity.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error reverting record: " + ex.Message, "danger");
                }
            }
            else if (e.CommandName == "SubmitToFinal")
            {
                int eduId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    bool submitted = DBHelper.AdminSubmitRecord(eduId, UserSession.CurrentPCNO, "Marked as Submitted and locked by Administrator from Dashboard");
                    if (submitted)
                    {
                        LoadDashboardStats();
                        BindGrid();
                        ShowAlert("Record #" + eduId + " successfully changed to Submitted mode and locked.", "success");
                    }
                    else
                    {
                        ShowAlert("Could not submit record. Please verify database connectivity.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error submitting record: " + ex.Message, "danger");
                }
            }
        }

        private void PopulateModalDropdowns()
        {
            // Main Qualifications
            DataTable dtMain = DBHelper.GetMainQualifications(true);
            ddlMainQual.DataSource = dtMain;
            ddlMainQual.DataTextField = "DISPLAY_NAME";
            ddlMainQual.DataValueField = "QUAL_CODE";
            ddlMainQual.DataBind();
            ddlMainQual.Items.Insert(0, new ListItem("-- Select Main Qualification --", ""));

            // Universities
            DataTable dtUniv = DBHelper.GetUniversities(true);
            ddlUniversity.DataSource = dtUniv;
            ddlUniversity.DataTextField = "UNIV_NAME";
            ddlUniversity.DataValueField = "UNIV_NAME";
            ddlUniversity.DataBind();
            ddlUniversity.Items.Insert(0, new ListItem("-- Select University / Board --", ""));
            if (ddlUniversity.Items.FindByText("Other Recognized University / Board") == null)
            {
                ddlUniversity.Items.Add(new ListItem("Other Recognized University / Board", "Other Recognized University / Board"));
            }

            // Passing Years
            ddlYearPassed.Items.Clear();
            ddlYearPassed.Items.Add(new ListItem("-- Year --", ""));
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear; y >= 1960; y--)
            {
                ddlYearPassed.Items.Add(new ListItem(y.ToString(), y.ToString()));
            }

            // Colleges
            PopulateColleges();

            // All Sub Qualifications with data-main attribute for client cascading
            PopulateAllSubQualifications();
        }

        public string GetSubQualsJson()
        {
            return DBHelper.GetSubQualificationsJson();
        }

        private void PopulateAllSubQualifications()
        {
            DataTable dtSub = DBHelper.GetSubQualifications(null, true);
            ddlSubQual.Items.Clear();
            ddlSubQual.Items.Add(new ListItem("-- Select Sub Qualification / Stream --", ""));

            if (dtSub != null)
            {
                foreach (DataRow r in dtSub.Rows)
                {
                    string subCode = r["SUB_CODE"].ToString().Trim();
                    string subName = r["SUB_NAME"].ToString().Trim();
                    string mainCode = r["MAIN_QUAL_CODE"] != DBNull.Value ? r["MAIN_QUAL_CODE"].ToString().Trim() : "";

                    ListItem li = new ListItem(subName, subCode);
                    if (!string.IsNullOrEmpty(mainCode))
                    {
                        li.Attributes["data-main"] = mainCode;
                    }
                    ddlSubQual.Items.Add(li);
                }
            }

            // Universal fallbacks
            if (ddlSubQual.Items.FindByValue("GENERAL") == null)
            {
                ListItem liGen = new ListItem("General", "GENERAL");
                liGen.Attributes["data-main"] = "";
                ddlSubQual.Items.Add(liGen);
            }
            if (ddlSubQual.Items.FindByValue("OTHER") == null)
            {
                ListItem liOth = new ListItem("Other (Specify in Specialization)", "OTHER");
                liOth.Attributes["data-main"] = "";
                ddlSubQual.Items.Add(liOth);
            }
        }

        private void PopulateColleges()
        {
            DataTable dtColleges = DBHelper.GetColleges();
            ddlCollege.Items.Clear();
            ddlCollege.Items.Add(new ListItem("-- Select College / School Name --", ""));
            if (dtColleges != null)
            {
                foreach (DataRow r in dtColleges.Rows)
                {
                    string cName = r["COLLEGE_NAME"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cName))
                    {
                        ddlCollege.Items.Add(new ListItem(cName, cName));
                    }
                }
            }
            ddlCollege.Items.Add(new ListItem("Other / College Not in List (Type Manually)", "OTHER"));
        }

        private void PopulateSubQualDropdown(string mainCode)
        {
            PopulateAllSubQualifications();
        }

        private void BindCollegeSelection(string college)
        {
            if (string.IsNullOrEmpty(college) || DBHelper.IsPlaceholderOrInvalid(college))
            {
                if (ddlCollege.Items.Count > 0) ddlCollege.SelectedIndex = 0;
                txtCustomCollege.Text = "";
                pnlCustomCollege.Style["display"] = "none";
                return;
            }

            ListItem item = ddlCollege.Items.FindByValue(college);
            if (item == null)
            {
                foreach (ListItem itm in ddlCollege.Items)
                {
                    if (itm.Value.Equals(college, StringComparison.OrdinalIgnoreCase))
                    {
                        item = itm;
                        break;
                    }
                }
            }

            if (item != null && !item.Value.Equals("OTHER", StringComparison.OrdinalIgnoreCase))
            {
                ddlCollege.SelectedValue = item.Value;
                pnlCustomCollege.Style["display"] = "none";
                txtCustomCollege.Text = "";
            }
            else
            {
                if (ddlCollege.Items.FindByValue("OTHER") != null)
                {
                    ddlCollege.SelectedValue = "OTHER";
                }
                pnlCustomCollege.Style["display"] = "block";
                txtCustomCollege.Text = college.Equals("OTHER", StringComparison.OrdinalIgnoreCase) ? "" : college;
            }
        }

        private string GetSelectedCollegeName()
        {
            if (ddlCollege.SelectedValue == "OTHER" || ddlCollege.SelectedValue.StartsWith("Other"))
            {
                return txtCustomCollege.Text.Trim();
            }
            return ddlCollege.SelectedValue.Trim();
        }

        public void OpenRecordForEdit(int eduId)
        {
            DataRow r = DBHelper.GetEducationRecordById(eduId);
            if (r == null)
            {
                ShowAlert("Record not found.", "warning");
                return;
            }

            PopulateModalDropdowns();

            hfEditEduId.Value = eduId.ToString();
            string pcno = r["PCNO"].ToString();
            string empName = r["EMP_NAME"] != DBNull.Value ? r["EMP_NAME"].ToString() : pcno;
            string desig = r["DESIGNATION"] != DBNull.Value ? r["DESIGNATION"].ToString() : "---";
            string div = r["DIVISION_NAME"] != DBNull.Value ? r["DIVISION_NAME"].ToString() : "---";

            hfEditPCNO.Value = pcno;
            hfEditEmpName.Value = empName;
            hfEditDesig.Value = desig;
            hfEditDiv.Value = div;

            litEditEmpName.Text = Server.HtmlEncode(empName);
            litEditEmpPCNO.Text = Server.HtmlEncode(pcno);
            litEditBannerEmp.Text = Server.HtmlEncode(empName) + " &bull; PCNO: " + Server.HtmlEncode(pcno) + " &bull; " + Server.HtmlEncode(desig) + " (" + Server.HtmlEncode(div) + ")";
            litEditRecordId.Text = eduId.ToString();

            string mainCode = r["QUAL_MAIN_CODE"].ToString();
            hfSelectedMainQual.Value = mainCode;
            if (ddlMainQual.Items.FindByValue(mainCode) != null)
            {
                ddlMainQual.SelectedValue = mainCode;
            }
            PopulateSubQualDropdown(mainCode);

            string subCode = r["QUAL_SUB_CODE"].ToString();
            string subName = r["QUAL_SUB_NAME"] != DBNull.Value ? r["QUAL_SUB_NAME"].ToString().Trim() : "";

            if ((mainCode == "MTRI" || mainCode == "10TH") && (subCode == "GEN" || subCode == "NA" || subName.IndexOf("Animal Genetic", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                subCode = "GENERAL";
                subName = "General (All Subjects)";
            }
            else if (mainCode == "BMTRI" && (subCode == "GEN" || subCode == "NA" || subName.IndexOf("Animal Genetic", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                subCode = "GENERAL";
                subName = "General (Below 10th)";
            }

            hfSelectedSubQual.Value = subCode;
            hfSelectedSubQualName.Value = subName;
            if (ddlSubQual.Items.FindByValue(subCode) != null)
            {
                ddlSubQual.SelectedValue = subCode;
            }
            else
            {
                ddlSubQual.Items.Add(new ListItem(!string.IsNullOrEmpty(subName) ? subName : subCode, subCode));
                ddlSubQual.SelectedValue = subCode;
            }

            string yr = r["YEAR_PASSED"].ToString();
            if (!DBHelper.IsPlaceholderOrInvalid(yr) && ddlYearPassed.Items.FindByValue(yr) != null)
            {
                ddlYearPassed.SelectedValue = yr;
            }
            else
            {
                ddlYearPassed.SelectedIndex = 0;
            }

            string specVal = r["SPECIALIZATION"] != DBNull.Value ? r["SPECIALIZATION"].ToString() : "";
            txtSpecialization.Text = DBHelper.IsInvalidSpecialization(specVal) ? "" : specVal;

            string inst = r["INSTITUTE_NAME"] != DBNull.Value ? r["INSTITUTE_NAME"].ToString().Trim() : "";

            // Canonical resolution for legacy abbreviations and outdated spellings
            if (inst.Equals("VTU", StringComparison.OrdinalIgnoreCase) ||
                inst.IndexOf("Visweswaraiah", StringComparison.OrdinalIgnoreCase) >= 0 ||
                inst.IndexOf("Visvesvaraya Technological", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                inst = "Visvesvaraya Technological University (VTU), Belagavi";
            }
            else if (inst.Equals("IISC", StringComparison.OrdinalIgnoreCase) || inst.Equals("IISC, BANGALORE", StringComparison.OrdinalIgnoreCase))
            {
                inst = "Indian Institute of Science (IISc), Bangalore";
            }
            else if (inst.Equals("JNTU", StringComparison.OrdinalIgnoreCase))
            {
                inst = "Jawaharlal Nehru Technological University (JNTU), Hyderabad";
            }
            else if (inst.Equals("IGNOU", StringComparison.OrdinalIgnoreCase) || inst.IndexOf("IGNOU, NEW DELHI", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                inst = "Indira Gandhi National Open University (IGNOU), New Delhi";
            }
            else if (inst.Equals("CBSE Board", StringComparison.OrdinalIgnoreCase) || inst.Equals("CBSE Board New Delhi", StringComparison.OrdinalIgnoreCase))
            {
                inst = "Central Board of Secondary Education (CBSE), New Delhi";
            }
            else if (inst.Equals("ICSE Board", StringComparison.OrdinalIgnoreCase) || inst.Equals("ISCE BOARD", StringComparison.OrdinalIgnoreCase) || inst.Equals("ICSE / ISC Board", StringComparison.OrdinalIgnoreCase))
            {
                inst = "Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)";
            }
            else if (inst.Equals("PUE", StringComparison.OrdinalIgnoreCase))
            {
                inst = "Department of Pre-University Education (PUE), Karnataka";
            }

            if (!DBHelper.IsPlaceholderOrInvalid(inst))
            {
                if (ddlUniversity.Items.FindByValue(inst) != null)
                {
                    ddlUniversity.SelectedValue = inst;
                }
                else if (ddlUniversity.Items.Count > 0)
                {
                    ddlUniversity.Items.Insert(1, new ListItem(inst, inst));
                    ddlUniversity.SelectedValue = inst;
                }
            }
            else
            {
                ddlUniversity.SelectedIndex = 0;
            }

            string college = r["COLLEGE_NAME"] != DBNull.Value ? r["COLLEGE_NAME"].ToString() : "";
            BindCollegeSelection(college);

            string cgpaVal = r["CGPA_MARKS"] != DBNull.Value ? r["CGPA_MARKS"].ToString() : "";
            if (DBHelper.IsPlaceholderOrInvalid(cgpaVal))
            {
                txtCgpaMarks.Text = "";
                ddlScoreType.SelectedValue = "Percentage";
            }
            else
            {
                string trimmed = cgpaVal.Trim();
                if (trimmed.Contains("%"))
                {
                    ddlScoreType.SelectedValue = "Percentage";
                    txtCgpaMarks.Text = trimmed;
                }
                else if (trimmed.ToUpper().Contains("CGPA"))
                {
                    ddlScoreType.SelectedValue = "CGPA";
                    txtCgpaMarks.Text = trimmed;
                }
                else if (trimmed.Contains("/"))
                {
                    ddlScoreType.SelectedValue = "Marks";
                    txtCgpaMarks.Text = trimmed;
                }
                else
                {
                    double num;
                    if (double.TryParse(trimmed, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out num))
                    {
                        if (num > 10.0 && num <= 100.0)
                        {
                            ddlScoreType.SelectedValue = "Percentage";
                            txtCgpaMarks.Text = num.ToString("0.##") + "%";
                        }
                        else if (num <= 10.0)
                        {
                            ddlScoreType.SelectedValue = "CGPA";
                            txtCgpaMarks.Text = num.ToString("0.##");
                        }
                        else
                        {
                            ddlScoreType.SelectedValue = "Marks";
                            txtCgpaMarks.Text = trimmed;
                        }
                    }
                    else
                    {
                        ddlScoreType.SelectedValue = "Percentage";
                        txtCgpaMarks.Text = trimmed;
                    }
                }
            }

            string grade = r["GRADE_DIVISION"] != DBNull.Value ? r["GRADE_DIVISION"].ToString() : "";
            if (!DBHelper.IsPlaceholderOrInvalid(grade) && ddlGrade.Items.FindByValue(grade) != null)
            {
                ddlGrade.SelectedValue = grade;
            }
            else
            {
                ddlGrade.SelectedIndex = 0;
            }

            string recStatus = r["STATUS"] != DBNull.Value ? r["STATUS"].ToString() : "Draft";
            if (ddlRecordStatus.Items.FindByValue(recStatus) != null)
            {
                ddlRecordStatus.SelectedValue = recStatus;
            }

            chkIsHighest.Checked = (Convert.ToInt32(r["IS_HIGHEST"]) == 1);
            string rem = r["REMARKS"] != DBNull.Value ? r["REMARKS"].ToString() : "";
            if (!string.IsNullOrEmpty(rem) && (rem.StartsWith("Migrated from", StringComparison.OrdinalIgnoreCase) || rem.StartsWith("Auto-synced", StringComparison.OrdinalIgnoreCase)))
            {
                rem = "";
            }
            txtRemarks.Text = rem;

            bool hasPlaceholders = DBHelper.IsPlaceholderOrInvalid(college) ||
                                   DBHelper.IsPlaceholderOrInvalid(inst) ||
                                   DBHelper.IsInvalidSpecialization(specVal) ||
                                   DBHelper.IsPlaceholderOrInvalid(cgpaVal) ||
                                   DBHelper.IsPlaceholderOrInvalid(grade) ||
                                   DBHelper.IsPlaceholderOrInvalid(yr);

            if (hasPlaceholders)
            {
                pnlFormError.Visible = true;
                pnlFormError.CssClass = "alert alert-warning py-2 small mb-3";
                litFormError.Text = "<i class='fas fa-info-circle mr-1 text-warning'></i> <strong>Imported Record:</strong> Please verify and complete the details for this qualification before saving.";
            }
            else
            {
                pnlFormError.Visible = false;
            }
            pnlModal.Style["display"] = "flex";
        }

        protected void ddlMainQual_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retained for postback compatibility
            pnlModal.Style["display"] = "flex";
        }

        protected void ddlCollege_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retained for postback compatibility
            pnlModal.Style["display"] = "flex";
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlModal.Style["display"] = "none";
            pnlFormError.Visible = false;
        }

        protected void btnSaveAdminEdit_Click(object sender, EventArgs e)
        {
            pnlFormError.Visible = false;

            string mainCode = ddlMainQual.SelectedValue;
            if (string.IsNullOrEmpty(mainCode) && !string.IsNullOrEmpty(hfSelectedMainQual.Value))
            {
                mainCode = hfSelectedMainQual.Value;
            }

            string subCode = ddlSubQual.SelectedValue;
            if (string.IsNullOrEmpty(subCode) && !string.IsNullOrEmpty(hfSelectedSubQual.Value))
            {
                subCode = hfSelectedSubQual.Value;
            }

            // Normalize Matriculation / 10th sub-qualifications
            if ((mainCode == "MTRI" || mainCode == "10TH" || mainCode == "BMTRI") && (subCode == "NA" || subCode == "GEN"))
            {
                subCode = "GENERAL";
            }

            if (string.IsNullOrEmpty(mainCode) || DBHelper.IsPlaceholderOrInvalid(mainCode))
            {
                ShowFormError("Please select a valid Main Qualification.");
                return;
            }
            if (string.IsNullOrEmpty(subCode) || (DBHelper.IsPlaceholderOrInvalid(subCode) && subCode != "GENERAL"))
            {
                ShowFormError("Please select a valid Sub Qualification / Stream.");
                return;
            }
            if (string.IsNullOrEmpty(ddlYearPassed.SelectedValue) || DBHelper.IsPlaceholderOrInvalid(ddlYearPassed.SelectedValue))
            {
                ShowFormError("Please select a valid Passing Year.");
                return;
            }
            if (string.IsNullOrEmpty(txtSpecialization.Text.Trim()) || DBHelper.IsInvalidSpecialization(txtSpecialization.Text.Trim()))
            {
                ShowFormError("Please enter Specialization / Major Subject (or 'N/A' if not applicable).");
                return;
            }
            if (string.IsNullOrEmpty(ddlUniversity.SelectedValue) || DBHelper.IsPlaceholderOrInvalid(ddlUniversity.SelectedValue))
            {
                ShowFormError("Please select a valid University / Board.");
                return;
            }
            string college = GetSelectedCollegeName();
            if (string.IsNullOrEmpty(college) || DBHelper.IsPlaceholderOrInvalid(college))
            {
                ShowFormError("Please select or specify a valid College / School Name.");
                return;
            }
            string rawScore = txtCgpaMarks.Text.Trim();
            if (string.IsNullOrEmpty(rawScore) || DBHelper.IsPlaceholderOrInvalid(rawScore))
            {
                ShowFormError("Please enter valid Marks / Percentage / CGPA.");
                return;
            }

            string scoreType = ddlScoreType.SelectedValue;
            string cgpa = rawScore;
            double scoreNum;
            string cleanNumStr = rawScore.Replace("%", "").Replace("CGPA", "").Replace("cgpa", "").Trim();
            if (double.TryParse(cleanNumStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out scoreNum))
            {
                if (scoreType == "Percentage")
                {
                    if (scoreNum < 0 || scoreNum > 100)
                    {
                        ShowFormError("Percentage must be between 0.00% and 100.00%.");
                        return;
                    }
                    cgpa = scoreNum.ToString("0.##") + "%";
                }
                else if (scoreType == "CGPA")
                {
                    if (scoreNum < 0 || scoreNum > 10)
                    {
                        ShowFormError("CGPA must be between 0.00 and 10.00.");
                        return;
                    }
                    cgpa = scoreNum.ToString("0.##") + " CGPA";
                }
            }

            if (string.IsNullOrEmpty(ddlGrade.SelectedValue) || DBHelper.IsPlaceholderOrInvalid(ddlGrade.SelectedValue))
            {
                ShowFormError("Please select a valid Grade / Division.");
                return;
            }

            int eduId = 0;
            int.TryParse(hfEditEduId.Value, out eduId);
            string pcno = hfEditPCNO.Value;
            string empName = hfEditEmpName.Value;
            string desig = hfEditDesig.Value;
            string div = hfEditDiv.Value;

            string mainName = (ddlMainQual.SelectedItem != null && !string.IsNullOrEmpty(ddlMainQual.SelectedItem.Text))
                ? ddlMainQual.SelectedItem.Text : mainCode;

            string dbSubName = DBHelper.GetSubQualificationName(mainCode, subCode);
            string subName = !string.IsNullOrEmpty(dbSubName) ? dbSubName : ((ddlSubQual.SelectedItem != null && !string.IsNullOrEmpty(ddlSubQual.SelectedItem.Text)) ? ddlSubQual.SelectedItem.Text : subCode);

            int year = Convert.ToInt32(ddlYearPassed.SelectedValue);
            string spec = txtSpecialization.Text.Trim();
            string inst = ddlUniversity.SelectedValue;
            string grade = ddlGrade.SelectedValue;
            bool isHighest = chkIsHighest.Checked;
            string remarks = txtRemarks.Text.Trim();
            string status = ddlRecordStatus.SelectedValue;

            try
            {
                DBHelper.SaveEducationRecord(
                    eduId,
                    pcno,
                    empName,
                    desig,
                    div,
                    mainCode,
                    mainName,
                    subCode,
                    subName,
                    year,
                    inst,
                    college,
                    spec,
                    cgpa,
                    grade,
                    isHighest,
                    remarks,
                    status,
                    UserSession.CurrentUserName + " (Admin Edit)");

                pnlModal.Style["display"] = "none";
                LoadDashboardStats();
                BindGrid();
                ShowAlert("Qualification record for " + Server.HtmlEncode(empName) + " (PCNO: " + pcno + ") successfully updated by Administrator.", "success");
            }
            catch (Exception ex)
            {
                ShowFormError("Error saving record: " + ex.Message);
            }
        }

        private void ShowFormError(string msg)
        {
            pnlFormError.Visible = true;
            pnlFormError.CssClass = "alert alert-danger py-2 small mb-3";
            litFormError.Text = Server.HtmlEncode(msg);
            pnlModal.Style["display"] = "flex";
        }

        protected void btnExportCsv_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            string status = ddlStatusFilter.SelectedValue;
            string qual = ddlQualFilter.SelectedValue;

            DataTable dt = DBHelper.GetAdminEducationOverview(search, status, qual, "ALL");
            if (dt == null || dt.Rows.Count == 0)
            {
                ShowAlert("No records to export.", "warning");
                return;
            }

            StringBuilder sb = new StringBuilder();
            // Header
            sb.AppendLine("PCNO,Employee Name,Designation,Division,Main Qualification Code,Main Qualification,Sub Qualification,Specialization,Year Passed,Institute,College,CGPA/Marks,Grade,Highest,Status,Migrated");

            foreach (DataRow r in dt.Rows)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\"",
                    EscapeCsv(r["PCNO"]),
                    EscapeCsv(r["EMP_NAME"]),
                    EscapeCsv(r["DESIGNATION"]),
                    EscapeCsv(r["DIVISION_NAME"]),
                    EscapeCsv(r["QUAL_MAIN_CODE"]),
                    EscapeCsv(r["QUAL_MAIN_NAME"]),
                    EscapeCsv(r["QUAL_SUB_NAME"]),
                    EscapeCsv(r["SPECIALIZATION"]),
                    EscapeCsv(r["YEAR_PASSED"]),
                    EscapeCsv(r["INSTITUTE_NAME"]),
                    EscapeCsv(r["COLLEGE_NAME"]),
                    EscapeCsv(r["CGPA_MARKS"]),
                    EscapeCsv(r["GRADE_DIVISION"]),
                    Convert.ToInt32(r["IS_HIGHEST"]) == 1 ? "Yes" : "No",
                    EscapeCsv(r["STATUS"]),
                    Convert.ToInt32(r["IS_MIGRATED"]) == 1 ? "Yes" : "No"
                ));
            }

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Employee_Education_Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
            Response.Charset = "";
            Response.ContentType = "text/csv";
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        private string EscapeCsv(object val)
        {
            if (val == null || val == DBNull.Value) return "";
            return val.ToString().Replace("\"", "\"\"");
        }

        private void ShowAlert(string msg, string type)
        {
            if (Master is SiteMaster)
            {
                ((SiteMaster)Master).ShowAlert(msg, type);
            }
        }
    }
}
