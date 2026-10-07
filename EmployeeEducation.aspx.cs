using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class EmployeeEducation : System.Web.UI.Page
    {
        public string CurrentPCNO
        {
            get { return UserSession.CurrentPCNO; }
        }

        private string _highestQualTitle = "---";
        public string HighestQualTitle
        {
            get { return _highestQualTitle; }
            set { _highestQualTitle = value; }
        }

        public bool CanEditRecord(object status)
        {
            string s = status != null ? status.ToString() : "";
            return s.Equals("Draft", StringComparison.OrdinalIgnoreCase) || UserSession.IsAdmin;
        }

        public bool IsLockedRecord(object status)
        {
            string s = status != null ? status.ToString() : "";
            return s.Equals("Submitted", StringComparison.OrdinalIgnoreCase) && !UserSession.IsAdmin;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsAuthenticated)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadEmployeeHeader();
                BindEducationGrid();
                PopulateDropdowns();
            }
        }

        private void LoadEmployeeHeader()
        {
            string pcno = CurrentPCNO;
            litEmpPCNO.Text = pcno;

            DataRow profile = DBHelper.GetEmployeeProfile(pcno);
            if (profile != null)
            {
                litEmpName.Text = profile["NAME"] != DBNull.Value ? profile["NAME"].ToString() : pcno;
                litEmpDesignation.Text = profile["DESIGNATION"] != DBNull.Value ? profile["DESIGNATION"].ToString() : "---";
                litEmpDivision.Text = profile["DIVNAME"] != DBNull.Value ? profile["DIVNAME"].ToString() : "---";
            }
            else
            {
                litEmpName.Text = UserSession.CurrentUserName;
                litEmpDesignation.Text = UserSession.Designation;
                litEmpDivision.Text = UserSession.Division;
            }

            if (imgEmpPhoto != null)
            {
                imgEmpPhoto.ImageUrl = "Static/Images/default_avatar.png";
                imgEmpPhoto.Attributes["onerror"] = "this.onerror=null; this.src='Static/Images/default_avatar.png';";

                bool enableIntranetPhotos = string.Equals(System.Configuration.ConfigurationManager.AppSettings["EnableIntranetPhotos"], "true", StringComparison.OrdinalIgnoreCase);
                if (enableIntranetPhotos && !string.IsNullOrEmpty(pcno) && pcno != "---")
                {
                    imgEmpPhoto.Attributes["data-intranet-photo"] = string.Format("http://windflower.lrde.com/empphotos/Dsc_{0}.jpg", pcno);
                }
                else
                {
                    imgEmpPhoto.Attributes.Remove("data-intranet-photo");
                }
            }
        }

        private void BindEducationGrid()
        {
            DataTable dt = DBHelper.GetEmployeeEducations(CurrentPCNO);
            gvEducation.DataSource = dt;
            gvEducation.DataBind();

            if (gvEducation.HeaderRow != null)
            {
                gvEducation.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            bool hasRecords = dt != null && dt.Rows.Count > 0;
            int draftCount = 0;
            int submittedCount = 0;

            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    string st = r["STATUS"] != DBNull.Value ? r["STATUS"].ToString() : "";
                    if (st.Equals("Draft", StringComparison.OrdinalIgnoreCase))
                    {
                        draftCount++;
                    }
                    else if (st.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
                    {
                        submittedCount++;
                    }
                }
            }

            bool hasDrafts = draftCount > 0;

            // Users can ALWAYS add new qualifications
            btnOpenAddModal.Visible = true;
            if (btnSidebarAdd != null) btnSidebarAdd.Visible = true;

            // Sidebar stats
            int qualCount = (dt != null) ? dt.Rows.Count : 0;
            litStatQualCount.Text = qualCount.ToString();

            string highestName = "None";
            HighestQualTitle = "None";
            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToInt32(r["IS_HIGHEST"]) == 1)
                    {
                        highestName = r["QUAL_MAIN_CODE"].ToString();
                        HighestQualTitle = r["QUAL_MAIN_NAME"].ToString();
                        break;
                    }
                }
            }
            litStatHighestQual.Text = highestName;

            if (hasDrafts)
            {
                litSubmissionStatusPill.Text = string.Format("<span class='badge-status badge-draft' style='font-size: 0.9rem; padding: 6px 16px;'><i class='fas fa-pencil-alt mr-1'></i> {0} Draft Record{1} Pending</span>", draftCount, draftCount > 1 ? "s" : "");
                phDraftNotice.Visible = true;
                phSubmittedNotice.Visible = false;
                phBottomSubmitBtn.Visible = true;
            }
            else if (hasRecords)
            {
                litSubmissionStatusPill.Text = "<span class='badge-status badge-submitted' style='font-size: 0.9rem; padding: 6px 16px;'><i class='fas fa-check-circle mr-1'></i> Submitted &amp; Verified</span>";
                phSubmittedNotice.Visible = true;
                phDraftNotice.Visible = false;
                phBottomSubmitBtn.Visible = false;
            }
            else
            {
                litSubmissionStatusPill.Text = "<span class='badge-status badge-light border text-muted' style='font-size: 0.9rem; padding: 6px 16px;'><i class='fas fa-info-circle mr-1'></i> No Qualifications Recorded</span>";
                phSubmittedNotice.Visible = false;
                phDraftNotice.Visible = false;
                phBottomSubmitBtn.Visible = false;
            }
        }

        private void PopulateDropdowns()
        {
            // Main Qualifications
            DataTable dtMain = DBHelper.GetMainQualifications(true);
            ddlMainQual.DataSource = dtMain;
            ddlMainQual.DataTextField = "QUAL_NAME";
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

            // Passing Years (Descending from current year to 1960)
            ddlYearPassed.Items.Clear();
            ddlYearPassed.Items.Add(new ListItem("-- Year --", ""));
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear; y >= 1960; y--)
            {
                ddlYearPassed.Items.Add(new ListItem(y.ToString(), y.ToString()));
            }

            // Load All Sub Qualifications with data-main attribute
            PopulateAllSubQualifications();
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
                    ListItem li = new ListItem(r["SUB_NAME"].ToString(), r["SUB_CODE"].ToString());
                    if (r["MAIN_QUAL_CODE"] != DBNull.Value && !string.IsNullOrEmpty(r["MAIN_QUAL_CODE"].ToString()))
                    {
                        li.Attributes["data-main"] = r["MAIN_QUAL_CODE"].ToString();
                    }
                    ddlSubQual.Items.Add(li);
                }
            }

            // Fallbacks
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

        private void PopulateSubQualDropdown(string mainCode)
        {
            PopulateAllSubQualifications();
        }

        protected void ddlMainQual_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retained for postback compatibility
        }

        protected void btnOpenAddModal_Click(object sender, EventArgs e)
        {
            hfEditEduId.Value = "0";
            litModalTitle.Text = "Add Qualification Record";
            ClearModalForm();
            pnlModal.Visible = true;
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlModal.Visible = false;
            pnlFormError.Visible = false;
        }

        protected void gvEducation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRecord")
            {
                int eduId = Convert.ToInt32(e.CommandArgument);
                DataRow r = DBHelper.GetEducationRecordById(eduId);
                if (r != null)
                {
                    string recStatus = r["STATUS"] != DBNull.Value ? r["STATUS"].ToString() : "";
                    if (!UserSession.IsAdmin && recStatus.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowAlert("Submitted qualification records are locked as official entries and cannot be edited.", "warning");
                        return;
                    }

                    hfEditEduId.Value = eduId.ToString();
                    litModalTitle.Text = "Edit Qualification Record";

                    string mainCode = r["QUAL_MAIN_CODE"].ToString();
                    if (ddlMainQual.Items.FindByValue(mainCode) != null)
                    {
                        ddlMainQual.SelectedValue = mainCode;
                    }
                    PopulateSubQualDropdown(mainCode);

                    string subCode = r["QUAL_SUB_CODE"].ToString();
                    if (ddlSubQual.Items.FindByValue(subCode) != null)
                    {
                        ddlSubQual.SelectedValue = subCode;
                    }

                    string yr = r["YEAR_PASSED"].ToString();
                    if (ddlYearPassed.Items.FindByValue(yr) != null)
                    {
                        ddlYearPassed.SelectedValue = yr;
                    }

                    txtSpecialization.Text = r["SPECIALIZATION"] != DBNull.Value ? r["SPECIALIZATION"].ToString() : "";
                    
                    string inst = r["INSTITUTE_NAME"] != DBNull.Value ? r["INSTITUTE_NAME"].ToString() : "";
                    if (ddlUniversity.Items.FindByValue(inst) != null)
                    {
                        ddlUniversity.SelectedValue = inst;
                    }
                    else if (ddlUniversity.Items.Count > 0)
                    {
                        ddlUniversity.Items.Insert(1, new ListItem(inst, inst));
                        ddlUniversity.SelectedValue = inst;
                    }

                    txtCollegeName.Text = r["COLLEGE_NAME"] != DBNull.Value ? r["COLLEGE_NAME"].ToString() : "";
                    txtCgpaMarks.Text = r["CGPA_MARKS"] != DBNull.Value ? r["CGPA_MARKS"].ToString() : "";
                    
                    string grade = r["GRADE_DIVISION"] != DBNull.Value ? r["GRADE_DIVISION"].ToString() : "";
                    if (ddlGrade.Items.FindByValue(grade) != null)
                    {
                        ddlGrade.SelectedValue = grade;
                    }

                    chkIsHighest.Checked = (Convert.ToInt32(r["IS_HIGHEST"]) == 1);
                    txtRemarks.Text = r["REMARKS"] != DBNull.Value ? r["REMARKS"].ToString() : "";

                    pnlFormError.Visible = false;
                    pnlModal.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteRecord")
            {
                int eduId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    bool deleted = DBHelper.DeleteEducationRecord(eduId, CurrentPCNO, UserSession.CurrentUserName, UserSession.IsAdmin);
                    if (deleted)
                    {
                        BindEducationGrid();
                        ShowAlert("Qualification record deleted successfully.", "success");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting record: " + ex.Message, "danger");
                }
            }
            else if (e.CommandName == "AdminRevert")
            {
                if (!UserSession.IsAdmin) return;
                int eduId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    bool reverted = DBHelper.AdminRevertToDraft(eduId, UserSession.CurrentPCNO, "Unlocked by Admin from Education page");
                    if (reverted)
                    {
                        BindEducationGrid();
                        ShowAlert("Record #" + eduId + " successfully reverted to Draft mode.", "success");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error reverting record: " + ex.Message, "danger");
                }
            }
            else if (e.CommandName == "AdminSubmit")
            {
                if (!UserSession.IsAdmin) return;
                int eduId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    bool submitted = DBHelper.AdminSubmitRecord(eduId, UserSession.CurrentPCNO, "Marked as Submitted and locked by Admin from Education page");
                    if (submitted)
                    {
                        BindEducationGrid();
                        ShowAlert("Record #" + eduId + " successfully changed to Submitted mode and locked.", "success");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error submitting record: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            SaveRecordWithStatus("Draft");
        }

        protected void btnSaveAndSubmit_Click(object sender, EventArgs e)
        {
            SaveRecordWithStatus("Submitted");
        }

        private void SaveRecordWithStatus(string status)
        {
            pnlFormError.Visible = false;

            // Validation of required fields
            if (string.IsNullOrEmpty(ddlMainQual.SelectedValue))
            {
                ShowFormError("Please select Main Qualification.");
                return;
            }
            if (string.IsNullOrEmpty(ddlSubQual.SelectedValue))
            {
                ShowFormError("Please select Sub Qualification / Stream.");
                return;
            }
            if (string.IsNullOrEmpty(ddlYearPassed.SelectedValue))
            {
                ShowFormError("Please select Passing Year.");
                return;
            }
            if (string.IsNullOrEmpty(txtSpecialization.Text.Trim()))
            {
                ShowFormError("Please enter Specialization / Major Subject.");
                return;
            }
            if (string.IsNullOrEmpty(ddlUniversity.SelectedValue))
            {
                ShowFormError("Please select University / Board / Institute.");
                return;
            }
            if (string.IsNullOrEmpty(txtCgpaMarks.Text.Trim()))
            {
                ShowFormError("Please enter CGPA / Marks / Percentage.");
                return;
            }
            if (string.IsNullOrEmpty(ddlGrade.SelectedValue))
            {
                ShowFormError("Please select Grade / Division.");
                return;
            }

            int eduId = 0;
            int.TryParse(hfEditEduId.Value, out eduId);

            string mainCode = ddlMainQual.SelectedValue;
            string mainName = ddlMainQual.SelectedItem.Text;
            string subCode = ddlSubQual.SelectedValue;
            string subName = ddlSubQual.SelectedItem.Text;
            int year = Convert.ToInt32(ddlYearPassed.SelectedValue);
            string spec = txtSpecialization.Text.Trim();
            string inst = ddlUniversity.SelectedValue;
            string college = txtCollegeName.Text.Trim();
            string cgpa = txtCgpaMarks.Text.Trim();
            string grade = ddlGrade.SelectedValue;
            bool isHighest = chkIsHighest.Checked;
            string remarks = txtRemarks.Text.Trim();

            try
            {
                DBHelper.SaveEducationRecord(
                    eduId,
                    CurrentPCNO,
                    litEmpName.Text,
                    litEmpDesignation.Text,
                    litEmpDivision.Text,
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
                    UserSession.CurrentUserName);

                pnlModal.Visible = false;
                BindEducationGrid();

                if (status.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
                {
                    ShowAlert("Qualification record saved and submitted successfully!", "success");
                }
                else
                {
                    ShowAlert("Qualification record saved as Draft. You can edit it or click 'Submit Education Details' to finalize.", "info");
                }
            }
            catch (Exception ex)
            {
                ShowFormError("Error saving record: " + ex.Message);
            }
        }

        protected void btnFinalSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int affected = DBHelper.SubmitAllEducationRecords(CurrentPCNO, UserSession.CurrentUserName);
                BindEducationGrid();
                ShowAlert("All education records have been successfully submitted and locked!", "success");
            }
            catch (Exception ex)
            {
                ShowAlert("Error submitting education records: " + ex.Message, "danger");
            }
        }

        private void ClearModalForm()
        {
            ddlMainQual.SelectedIndex = 0;
            ddlSubQual.SelectedIndex = 0;
            ddlYearPassed.SelectedIndex = 0;
            txtSpecialization.Text = "";
            ddlUniversity.SelectedIndex = 0;
            txtCollegeName.Text = "";
            txtCgpaMarks.Text = "";
            ddlGrade.SelectedIndex = 0;
            chkIsHighest.Checked = false;
            txtRemarks.Text = "";
            pnlFormError.Visible = false;
        }

        private void ShowFormError(string msg)
        {
            pnlFormError.Visible = true;
            litFormError.Text = msg;
        }

        private void ShowAlert(string msg, string type)
        {
            if (Master is SiteMaster)
            {
                ((SiteMaster)Master).ShowAlert(msg, type);
            }
            else
            {
                ShowToast(msg, type, 3000);
            }
        }

        private void ShowToast(string msg, string type = "info", int durationMs = 3000)
        {
            if (Master is SiteMaster)
            {
                ((SiteMaster)Master).ShowToast(msg, type, durationMs);
            }
            else
            {
                string cleanMsg = System.Web.HttpUtility.JavaScriptStringEncode(msg, false);
                string script = string.Format(
                    "(function(){{function r(){{if(window.EduApp&&EduApp.showToast){{EduApp.showToast('{0}','{1}',{2});}}else{{setTimeout(r,50);}}}}if(document.readyState==='loading'){{document.addEventListener('DOMContentLoaded',r);}}else{{r();}}}})();",
                    cleanMsg, type, durationMs
                );
                ClientScript.RegisterStartupScript(this.GetType(), "Toast_" + Guid.NewGuid().ToString("N"), script, true);
            }
        }
    }
}
