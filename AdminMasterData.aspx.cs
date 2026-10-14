using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class AdminMasterData : System.Web.UI.Page
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
                LoadAllMasterData();
            }
        }

        private void LoadAllMasterData()
        {
            // 1. Main Quals
            DataTable dtMain = DBHelper.GetMainQualifications(false);
            gvMainQuals.DataSource = dtMain;
            gvMainQuals.DataBind();

            ddlSubParentMain.DataSource = dtMain;
            ddlSubParentMain.DataTextField = "DISPLAY_NAME";
            ddlSubParentMain.DataValueField = "QUAL_CODE";
            ddlSubParentMain.DataBind();
            ddlSubParentMain.Items.Insert(0, new ListItem("-- Available for All Qualifications --", ""));

            ddlEditSubParentMain.DataSource = dtMain;
            ddlEditSubParentMain.DataTextField = "DISPLAY_NAME";
            ddlEditSubParentMain.DataValueField = "QUAL_CODE";
            ddlEditSubParentMain.DataBind();
            ddlEditSubParentMain.Items.Insert(0, new ListItem("-- Available for All Qualifications --", ""));

            // 2. Sub Quals
            DataTable dtSub = DBHelper.GetSubQualifications(null, false);
            gvSubQuals.DataSource = dtSub;
            gvSubQuals.DataBind();

            // 3. Universities
            DataTable dtUniv = DBHelper.GetUniversities(false);
            gvUniversities.DataSource = dtUniv;
            gvUniversities.DataBind();

            // 3b. Colleges & Schools
            DataTable dtColleges = DBHelper.GetCollegesMaster(false);
            gvColleges.DataSource = dtColleges;
            gvColleges.DataBind();

            // 4. Subjects
            DataTable dtSubj = DBHelper.GetSubjects(false);
            gvSubjects.DataSource = dtSubj;
            gvSubjects.DataBind();

            // 5. Admins
            DataTable dtAdmins = DBHelper.GetAdminUsers();
            gvAdmins.DataSource = dtAdmins;
            gvAdmins.DataBind();
        }

        protected void btnAddMainQual_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-main";
            string code = txtMainCode.Text.Trim();
            string name = txtMainName.Text.Trim();
            string cat = ddlMainCategory.SelectedValue;

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter both qualification code and name.", "danger");
                return;
            }

            try
            {
                bool added = DBHelper.AddMainQualification(code, name, cat);
                if (added)
                {
                    txtMainCode.Text = "";
                    txtMainName.Text = "";
                    LoadAllMasterData();
                    ShowAlert("Main qualification '" + code + "' added successfully!", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error adding qualification: " + ex.Message, "danger");
            }
        }

        protected void btnAddSubQual_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-sub";
            string mainCode = ddlSubParentMain.SelectedValue;
            string code = txtSubCode.Text.Trim();
            string name = txtSubName.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter both sub code and name.", "danger");
                return;
            }

            try
            {
                bool added = DBHelper.AddSubQualification(mainCode, code, name);
                if (added)
                {
                    txtSubCode.Text = "";
                    txtSubName.Text = "";
                    LoadAllMasterData();
                    ShowAlert("Sub qualification '" + code + "' added successfully!", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error adding sub qualification: " + ex.Message, "danger");
            }
        }

        protected void btnAddUniv_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-univ";
            string name = txtUnivName.Text.Trim();
            string type = txtUnivType.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter university / board name.", "danger");
                return;
            }

            try
            {
                bool added = DBHelper.AddUniversity(name, type);
                if (added)
                {
                    txtUnivName.Text = "";
                    txtUnivType.Text = "";
                    LoadAllMasterData();
                    ShowAlert("University '" + name + "' added successfully!", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error adding university: " + ex.Message, "danger");
            }
        }

        protected void btnAddCollege_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-college";
            string name = txtCollegeNameAdd.Text.Trim();
            string type = ddlCollegeTypeAdd.SelectedValue;
            string loc = txtCollegeLocationAdd.Text.Trim();
            string affil = txtCollegeAffilAdd.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter college / school name.", "danger");
                return;
            }

            try
            {
                bool added = DBHelper.AddCollege(name, type, loc, affil);
                if (added)
                {
                    txtCollegeNameAdd.Text = "";
                    txtCollegeLocationAdd.Text = "";
                    txtCollegeAffilAdd.Text = "";
                    LoadAllMasterData();
                    ShowAlert("College / School '" + name + "' added successfully!", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error adding college / school: " + ex.Message, "danger");
            }
        }

        protected void btnAddSubj_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-subj";
            string name = txtSubjName.Text.Trim();
            string grp = txtSubjGroup.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter subject / trade name.", "danger");
                return;
            }

            try
            {
                bool added = DBHelper.AddSubject(name, grp);
                if (added)
                {
                    txtSubjName.Text = "";
                    txtSubjGroup.Text = "";
                    LoadAllMasterData();
                    ShowAlert("Subject '" + name + "' added successfully!", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error adding subject: " + ex.Message, "danger");
            }
        }

        protected void btnAddAdmin_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-admins";
            string pcno = txtAdminPCNO.Text.Trim();
            string name = txtAdminName.Text.Trim();

            if (string.IsNullOrEmpty(pcno))
            {
                ShowAlert("Please enter employee PCNO to grant admin role.", "danger");
                return;
            }

            try
            {
                bool added = DBHelper.AddAdminUser(pcno, name);
                if (added)
                {
                    txtAdminPCNO.Text = "";
                    txtAdminName.Text = "";
                    LoadAllMasterData();
                    ShowAlert("Admin privileges granted to PCNO " + pcno + "!", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error granting admin privileges: " + ex.Message, "danger");
            }
        }

        // ------------------ MAIN QUALIFICATIONS ------------------
        protected void gvMainQuals_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            hfActiveTab.Value = "#tab-main";
            string code = e.CommandArgument != null ? e.CommandArgument.ToString() : "";
            if (string.IsNullOrEmpty(code)) return;

            if (e.CommandName == "EditRow")
            {
                DataTable dt = DBHelper.GetMainQualifications(false);
                DataRow targetRow = null;
                foreach (DataRow r in dt.Rows)
                {
                    if (string.Equals(r["QUAL_CODE"].ToString(), code, StringComparison.OrdinalIgnoreCase))
                    {
                        targetRow = r;
                        break;
                    }
                }

                if (targetRow != null)
                {
                    CloseAllModals();
                    hfEditMainCodeOrig.Value = code;
                    txtEditMainCode.Text = code;
                    txtEditMainName.Text = targetRow["QUAL_NAME"] != DBNull.Value ? targetRow["QUAL_NAME"].ToString() : "";
                    string cat = targetRow["QUAL_CATEGORY"] != DBNull.Value ? targetRow["QUAL_CATEGORY"].ToString() : "";
                    if (ddlEditMainCategory.Items.FindByValue(cat) != null)
                    {
                        ddlEditMainCategory.SelectedValue = cat;
                    }
                    pnlEditMainModal.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                CloseAllModals();
                try
                {
                    bool deleted = DBHelper.DeleteMainQualification(code);
                    if (deleted)
                    {
                        LoadAllMasterData();
                        ShowAlert("Main qualification '" + code + "' deleted successfully.", "success");
                    }
                    else
                    {
                        ShowAlert("Could not delete main qualification '" + code + "'.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting main qualification: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSaveEditMain_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-main";
            string code = hfEditMainCodeOrig.Value.Trim();
            string name = txtEditMainName.Text.Trim();
            string cat = ddlEditMainCategory.SelectedValue;

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                ShowAlert("Qualification name cannot be empty.", "danger");
                return;
            }

            try
            {
                bool updated = DBHelper.UpdateMainQualification(code, name, cat);
                if (updated)
                {
                    CloseAllModals();
                    LoadAllMasterData();
                    ShowAlert("Main qualification '" + code + "' updated successfully.", "success");
                }
                else
                {
                    ShowAlert("Failed to update main qualification.", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating qualification: " + ex.Message, "danger");
            }
        }

        // ------------------ SUB QUALIFICATIONS ------------------
        protected void gvSubQuals_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            hfActiveTab.Value = "#tab-sub";
            if (e.CommandArgument == null) return;
            int subId;
            if (!int.TryParse(e.CommandArgument.ToString(), out subId)) return;

            if (e.CommandName == "EditRow")
            {
                DataTable dt = DBHelper.GetSubQualifications(null, false);
                DataRow targetRow = null;
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToInt32(r["SUB_ID"]) == subId)
                    {
                        targetRow = r;
                        break;
                    }
                }

                if (targetRow != null)
                {
                    CloseAllModals();
                    hfEditSubId.Value = subId.ToString();
                    string parent = targetRow["MAIN_QUAL_CODE"] != DBNull.Value ? targetRow["MAIN_QUAL_CODE"].ToString() : "";
                    if (ddlEditSubParentMain.Items.FindByValue(parent) != null)
                    {
                        ddlEditSubParentMain.SelectedValue = parent;
                    }
                    else
                    {
                        ddlEditSubParentMain.SelectedIndex = 0;
                    }
                    txtEditSubCode.Text = targetRow["SUB_CODE"] != DBNull.Value ? targetRow["SUB_CODE"].ToString() : "";
                    txtEditSubName.Text = targetRow["SUB_NAME"] != DBNull.Value ? targetRow["SUB_NAME"].ToString() : "";
                    pnlEditSubModal.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                CloseAllModals();
                try
                {
                    bool deleted = DBHelper.DeleteSubQualification(subId);
                    if (deleted)
                    {
                        LoadAllMasterData();
                        ShowAlert("Sub qualification deleted successfully.", "success");
                    }
                    else
                    {
                        ShowAlert("Could not delete sub qualification.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting sub qualification: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSaveEditSub_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-sub";
            int subId;
            if (!int.TryParse(hfEditSubId.Value, out subId))
            {
                ShowAlert("Invalid sub qualification reference.", "danger");
                return;
            }

            string parentMain = ddlEditSubParentMain.SelectedValue;
            string code = txtEditSubCode.Text.Trim();
            string name = txtEditSubName.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter both sub qualification code and name.", "danger");
                return;
            }

            try
            {
                bool updated = DBHelper.UpdateSubQualification(subId, parentMain, code, name);
                if (updated)
                {
                    CloseAllModals();
                    LoadAllMasterData();
                    ShowAlert("Sub qualification '" + code + "' updated successfully.", "success");
                }
                else
                {
                    ShowAlert("Failed to update sub qualification.", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating sub qualification: " + ex.Message, "danger");
            }
        }

        // ------------------ UNIVERSITIES & BOARDS ------------------
        protected void gvUniversities_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            hfActiveTab.Value = "#tab-univ";
            if (e.CommandArgument == null) return;
            int univId;
            if (!int.TryParse(e.CommandArgument.ToString(), out univId)) return;

            if (e.CommandName == "EditRow")
            {
                DataTable dt = DBHelper.GetUniversities(false);
                DataRow targetRow = null;
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToInt32(r["UNIV_ID"]) == univId)
                    {
                        targetRow = r;
                        break;
                    }
                }

                if (targetRow != null)
                {
                    CloseAllModals();
                    hfEditUnivId.Value = univId.ToString();
                    txtEditUnivIdDisplay.Text = univId.ToString();
                    txtEditUnivName.Text = targetRow["UNIV_NAME"] != DBNull.Value ? targetRow["UNIV_NAME"].ToString() : "";
                    txtEditUnivType.Text = targetRow["STATE_OR_TYPE"] != DBNull.Value ? targetRow["STATE_OR_TYPE"].ToString() : "";
                    pnlEditUnivModal.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                CloseAllModals();
                try
                {
                    bool deleted = DBHelper.DeleteUniversity(univId);
                    if (deleted)
                    {
                        LoadAllMasterData();
                        ShowAlert("University deleted successfully.", "success");
                    }
                    else
                    {
                        ShowAlert("Could not delete university.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting university: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSaveEditUniv_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-univ";
            int univId;
            if (!int.TryParse(hfEditUnivId.Value, out univId))
            {
                ShowAlert("Invalid university reference.", "danger");
                return;
            }

            string name = txtEditUnivName.Text.Trim();
            string type = txtEditUnivType.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter university / board name.", "danger");
                return;
            }

            try
            {
                bool updated = DBHelper.UpdateUniversity(univId, name, type);
                if (updated)
                {
                    CloseAllModals();
                    LoadAllMasterData();
                    ShowAlert("University '" + name + "' updated successfully.", "success");
                }
                else
                {
                    ShowAlert("Failed to update university.", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating university: " + ex.Message, "danger");
            }
        }

        // ------------------ COLLEGES & SCHOOLS ------------------
        protected void gvColleges_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            hfActiveTab.Value = "#tab-college";
            if (e.CommandArgument == null) return;
            int collegeId;
            if (!int.TryParse(e.CommandArgument.ToString(), out collegeId)) return;

            if (e.CommandName == "EditRow")
            {
                DataTable dt = DBHelper.GetCollegesMaster(false);
                DataRow targetRow = null;
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToInt32(r["COLLEGE_ID"]) == collegeId)
                    {
                        targetRow = r;
                        break;
                    }
                }

                if (targetRow != null)
                {
                    CloseAllModals();
                    hfEditCollegeId.Value = collegeId.ToString();
                    txtEditCollegeIdDisplay.Text = collegeId.ToString();
                    txtEditCollegeName.Text = targetRow["COLLEGE_NAME"] != DBNull.Value ? targetRow["COLLEGE_NAME"].ToString() : "";
                    string cType = targetRow["COLLEGE_TYPE"] != DBNull.Value ? targetRow["COLLEGE_TYPE"].ToString() : "Engineering College";
                    if (ddlEditCollegeType.Items.FindByValue(cType) != null)
                    {
                        ddlEditCollegeType.SelectedValue = cType;
                    }
                    else
                    {
                        ddlEditCollegeType.SelectedIndex = 0;
                    }
                    txtEditCollegeLocation.Text = targetRow["LOCATION"] != DBNull.Value ? targetRow["LOCATION"].ToString() : "";
                    txtEditCollegeAffil.Text = targetRow["AFFILIATED_UNIV"] != DBNull.Value ? targetRow["AFFILIATED_UNIV"].ToString() : "";
                    pnlEditCollegeModal.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                CloseAllModals();
                try
                {
                    bool deleted = DBHelper.DeleteCollege(collegeId);
                    if (deleted)
                    {
                        LoadAllMasterData();
                        ShowAlert("College / School deleted successfully.", "success");
                    }
                    else
                    {
                        ShowAlert("Could not delete college / school.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting college / school: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSaveEditCollege_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-college";
            int collegeId;
            if (!int.TryParse(hfEditCollegeId.Value, out collegeId))
            {
                ShowAlert("Invalid college / school reference.", "danger");
                return;
            }

            string name = txtEditCollegeName.Text.Trim();
            string type = ddlEditCollegeType.SelectedValue;
            string loc = txtEditCollegeLocation.Text.Trim();
            string affil = txtEditCollegeAffil.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter college / school name.", "danger");
                return;
            }

            try
            {
                bool updated = DBHelper.UpdateCollege(collegeId, name, type, loc, affil);
                if (updated)
                {
                    CloseAllModals();
                    LoadAllMasterData();
                    ShowAlert("College / School '" + name + "' updated successfully.", "success");
                }
                else
                {
                    ShowAlert("Failed to update college / school.", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating college / school: " + ex.Message, "danger");
            }
        }

        // ------------------ SUBJECTS & DISCIPLINES ------------------
        protected void gvSubjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            hfActiveTab.Value = "#tab-subj";
            if (e.CommandArgument == null) return;
            int subjId;
            if (!int.TryParse(e.CommandArgument.ToString(), out subjId)) return;

            if (e.CommandName == "EditRow")
            {
                DataTable dt = DBHelper.GetSubjects(false);
                DataRow targetRow = null;
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToInt32(r["SUBJECT_ID"]) == subjId)
                    {
                        targetRow = r;
                        break;
                    }
                }

                if (targetRow != null)
                {
                    CloseAllModals();
                    hfEditSubjId.Value = subjId.ToString();
                    txtEditSubjIdDisplay.Text = subjId.ToString();
                    txtEditSubjName.Text = targetRow["SUBJECT_NAME"] != DBNull.Value ? targetRow["SUBJECT_NAME"].ToString() : "";
                    txtEditSubjGroup.Text = targetRow["SUBJECT_GROUP"] != DBNull.Value ? targetRow["SUBJECT_GROUP"].ToString() : "";
                    pnlEditSubjModal.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                CloseAllModals();
                try
                {
                    bool deleted = DBHelper.DeleteSubject(subjId);
                    if (deleted)
                    {
                        LoadAllMasterData();
                        ShowAlert("Subject deleted successfully.", "success");
                    }
                    else
                    {
                        ShowAlert("Could not delete subject.", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting subject: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSaveEditSubj_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-subj";
            int subjId;
            if (!int.TryParse(hfEditSubjId.Value, out subjId))
            {
                ShowAlert("Invalid subject reference.", "danger");
                return;
            }

            string name = txtEditSubjName.Text.Trim();
            string group = txtEditSubjGroup.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter subject name.", "danger");
                return;
            }

            try
            {
                bool updated = DBHelper.UpdateSubject(subjId, name, group);
                if (updated)
                {
                    CloseAllModals();
                    LoadAllMasterData();
                    ShowAlert("Subject '" + name + "' updated successfully.", "success");
                }
                else
                {
                    ShowAlert("Failed to update subject.", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating subject: " + ex.Message, "danger");
            }
        }

        // ------------------ AUTHORIZED ADMINISTRATORS ------------------
        protected void gvAdmins_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            hfActiveTab.Value = "#tab-admins";
            string pcno = e.CommandArgument != null ? e.CommandArgument.ToString() : "";
            if (string.IsNullOrEmpty(pcno)) return;

            if (e.CommandName == "ToggleActive")
            {
                CloseAllModals();
                if (pcno == "1001")
                {
                    ShowAlert("Primary system administrator (PCNO 1001) cannot be revoked.", "warning");
                    return;
                }

                try
                {
                    // Check current state
                    DataTable dt = DBHelper.GetAdminUsers();
                    bool currentActive = true;
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r["PCNO"].ToString().Equals(pcno, StringComparison.OrdinalIgnoreCase))
                        {
                            currentActive = Convert.ToInt32(r["IS_ACTIVE"]) == 1;
                            break;
                        }
                    }

                    DBHelper.ToggleAdminUser(pcno, !currentActive);
                    LoadAllMasterData();
                    ShowAlert("Admin privilege for PCNO " + pcno + " updated.", "success");
                }
                catch (Exception ex)
                {
                    ShowAlert("Error toggling admin privilege: " + ex.Message, "danger");
                }
            }
            else if (e.CommandName == "EditRow")
            {
                DataTable dt = DBHelper.GetAdminUsers();
                DataRow targetRow = null;
                foreach (DataRow r in dt.Rows)
                {
                    if (r["PCNO"].ToString().Equals(pcno, StringComparison.OrdinalIgnoreCase))
                    {
                        targetRow = r;
                        break;
                    }
                }

                if (targetRow != null)
                {
                    CloseAllModals();
                    hfEditAdminPcnoOrig.Value = pcno;
                    txtEditAdminPcnoDisplay.Text = pcno;
                    txtEditAdminName.Text = targetRow["USER_NAME"] != DBNull.Value ? targetRow["USER_NAME"].ToString() : "";
                    string role = targetRow["ROLE"] != DBNull.Value ? targetRow["ROLE"].ToString() : "Admin";
                    if (ddlEditAdminRole.Items.FindByValue(role) != null)
                    {
                        ddlEditAdminRole.SelectedValue = role;
                    }
                    pnlEditAdminModal.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                CloseAllModals();
                if (pcno == "1001")
                {
                    ShowAlert("Primary system administrator (PCNO 1001) cannot be deleted.", "warning");
                    return;
                }

                try
                {
                    bool deleted = DBHelper.DeleteAdminUser(pcno);
                    if (deleted)
                    {
                        LoadAllMasterData();
                        ShowAlert("Administrator " + pcno + " deleted successfully.", "success");
                    }
                    else
                    {
                        ShowAlert("Could not delete administrator " + pcno + ".", "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting administrator: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSaveEditAdmin_Click(object sender, EventArgs e)
        {
            hfActiveTab.Value = "#tab-admins";
            string pcno = hfEditAdminPcnoOrig.Value.Trim();
            string name = txtEditAdminName.Text.Trim();
            string role = ddlEditAdminRole.SelectedValue;

            if (string.IsNullOrEmpty(pcno) || string.IsNullOrEmpty(name))
            {
                ShowAlert("Please enter administrator display name.", "danger");
                return;
            }

            try
            {
                bool updated = DBHelper.UpdateAdminUser(pcno, name, role);
                if (updated)
                {
                    CloseAllModals();
                    LoadAllMasterData();
                    ShowAlert("Administrator " + pcno + " updated successfully.", "success");
                }
                else
                {
                    ShowAlert("Failed to update administrator.", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating administrator: " + ex.Message, "danger");
            }
        }

        // ------------------ MODAL UTILITY ------------------
        protected void btnCloseModals_Click(object sender, EventArgs e)
        {
            CloseAllModals();
        }

        private void CloseAllModals()
        {
            pnlEditMainModal.Visible = false;
            pnlEditSubModal.Visible = false;
            pnlEditUnivModal.Visible = false;
            pnlEditCollegeModal.Visible = false;
            pnlEditSubjModal.Visible = false;
            pnlEditAdminModal.Visible = false;
        }

        public string GetTabLinkClass(string tabId)
        {
            string active = hfActiveTab != null ? hfActiveTab.Value : "#tab-main";
            if (string.IsNullOrEmpty(active)) active = "#tab-main";
            return string.Equals(active, tabId, StringComparison.OrdinalIgnoreCase) ? "nav-link active" : "nav-link";
        }

        public string GetTabPaneClass(string tabId)
        {
            string active = hfActiveTab != null ? hfActiveTab.Value : "#tab-main";
            if (string.IsNullOrEmpty(active)) active = "#tab-main";
            return string.Equals(active, tabId, StringComparison.OrdinalIgnoreCase) ? "tab-pane fade show active" : "tab-pane fade";
        }

        private void ShowAlert(string msg, string type)
        {
            string cleanMsg = System.Web.HttpUtility.JavaScriptStringEncode(msg, false);
            string script = string.Format(
                "(function(){{function r(){{if(window.EduApp&&EduApp.showToast){{EduApp.showToast('{0}','{1}',3500);}}else{{setTimeout(r,50);}}}}r();}})();",
                cleanMsg, type
            );
            ScriptManager.RegisterStartupScript(this, GetType(), "Toast_" + Guid.NewGuid().ToString("N"), script, true);
        }
    }
}
