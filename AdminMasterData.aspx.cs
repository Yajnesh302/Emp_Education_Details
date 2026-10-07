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
            ddlSubParentMain.DataTextField = "QUAL_NAME";
            ddlSubParentMain.DataValueField = "QUAL_CODE";
            ddlSubParentMain.DataBind();
            ddlSubParentMain.Items.Insert(0, new ListItem("-- Available for All Qualifications --", ""));

            // 2. Sub Quals
            DataTable dtSub = DBHelper.GetSubQualifications(null, false);
            gvSubQuals.DataSource = dtSub;
            gvSubQuals.DataBind();

            // 3. Universities
            DataTable dtUniv = DBHelper.GetUniversities(false);
            gvUniversities.DataSource = dtUniv;
            gvUniversities.DataBind();

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

        protected void gvAdmins_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            hfActiveTab.Value = "#tab-admins";
            if (e.CommandName == "ToggleActive")
            {
                string pcno = e.CommandArgument.ToString();
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
