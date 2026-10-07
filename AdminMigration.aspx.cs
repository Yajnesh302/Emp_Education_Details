using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class AdminMigration : System.Web.UI.Page
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
                LoadStats();
                BindMappings();
                BindLogs();
                PopulateMainQualDropdown();
            }
        }

        private void LoadStats()
        {
            DataTable dt = DBHelper.GetMigrationStats();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                litLegacyTotal.Text = r["LegacyTotal"].ToString();
                litMigratedTotal.Text = r["MigratedCount"].ToString();
                litPendingTotal.Text = r["PendingCount"].ToString();
                litEmployeesTotal.Text = r["LegacyEmployees"].ToString();
            }
        }

        private void BindMappings()
        {
            DataTable dt = DBHelper.GetMigrationMappings();
            gvMappings.DataSource = dt;
            gvMappings.DataBind();
            litMappingCount.Text = (dt != null ? dt.Rows.Count : 0).ToString();
        }

        private void BindLogs()
        {
            DataTable dt = DBHelper.GetMigrationLogs();
            gvMigrationLogs.DataSource = dt;
            gvMigrationLogs.DataBind();
        }

        private void PopulateMainQualDropdown()
        {
            DataTable dtMain = DBHelper.GetMainQualifications(false);
            ddlEditMapMainQual.DataSource = dtMain;
            ddlEditMapMainQual.DataTextField = "QUAL_NAME";
            ddlEditMapMainQual.DataValueField = "QUAL_CODE";
            ddlEditMapMainQual.DataBind();
        }

        protected void btnRunMigration_Click(object sender, EventArgs e)
        {
            string statusMode = ddlMigStatus.SelectedValue;
            string user = UserSession.CurrentUserName ?? "Admin";

            string resultMsg = DBHelper.ExecuteDataMigration(user, statusMode);
            LoadStats();
            BindLogs();

            pnlMigResult.Visible = true;
            litMigResult.Text = resultMsg;
            ShowAlert(resultMsg, "info");
        }

        protected void gvMappings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditMapping")
            {
                int mapId = Convert.ToInt32(e.CommandArgument);
                DataTable dt = DBHelper.GetMigrationMappings();
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToInt32(r["MAPPING_ID"]) == mapId)
                    {
                        hfEditMapId.Value = mapId.ToString();
                        txtEditMapOldName.Text = string.Format("[ID: {0}] {1} ({2})", r["OLD_QFN_ID"], r["OLD_QFN_NAME"], r["OLD_QFN_GROUP"]);
                        
                        string mainCode = r["NEW_QUAL_MAIN_CODE"].ToString();
                        if (ddlEditMapMainQual.Items.FindByValue(mainCode) != null)
                        {
                            ddlEditMapMainQual.SelectedValue = mainCode;
                        }

                        txtEditMapSubCode.Text = r["NEW_QUAL_SUB_CODE"] != DBNull.Value ? r["NEW_QUAL_SUB_CODE"].ToString() : "";
                        txtEditMapNotes.Text = r["NOTES"] != DBNull.Value ? r["NOTES"].ToString() : "";

                        pnlEditMapModal.Visible = true;
                        break;
                    }
                }
            }
        }

        protected void btnCloseMapModal_Click(object sender, EventArgs e)
        {
            pnlEditMapModal.Visible = false;
        }

        protected void btnSaveMapEdit_Click(object sender, EventArgs e)
        {
            int mapId = Convert.ToInt32(hfEditMapId.Value);
            string newMain = ddlEditMapMainQual.SelectedValue;
            string newSub = txtEditMapSubCode.Text.Trim();
            string notes = txtEditMapNotes.Text.Trim();

            try
            {
                bool updated = DBHelper.UpdateMigrationMapping(mapId, newMain, newSub, notes);
                if (updated)
                {
                    pnlEditMapModal.Visible = false;
                    BindMappings();
                    ShowAlert("Qualification mapping rule updated successfully!", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating mapping: " + ex.Message, "danger");
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
