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
            if (e.CommandName == "RevertToDraft")
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
