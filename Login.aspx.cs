using System;
using System.Data;
using System.Web.Security;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (UserSession.IsAuthenticated)
                {
                    RedirectByRole();
                }
                else
                {
                    txtUsername.Focus();
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            pnlError.Visible = false;
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Please enter your Username / PCNO.");
                return;
            }

            string pcno = null;

            try
            {
                pcno = ADHelper.AuthenticateAndGetPCNO(username, password);
            }
            catch (Exception ex)
            {
                ShowError("Authentication Error: " + ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(pcno))
            {
                ShowError("Invalid credentials or employee not found in directory.");
                return;
            }

            // Fetch employee profile from HR data
            string empName = pcno;
            string designation = "";
            string division = "";

            try
            {
                DataRow profile = DBHelper.GetEmployeeProfile(pcno);
                if (profile != null)
                {
                    empName = profile["NAME"] != DBNull.Value ? profile["NAME"].ToString() : pcno;
                    designation = profile["DESIGNATION"] != DBNull.Value ? profile["DESIGNATION"].ToString() : "";
                    division = profile["DIVNAME"] != DBNull.Value ? profile["DIVNAME"].ToString() : "";
                }
            }
            catch { }

            bool isAdmin = DBHelper.IsAdmin(pcno);

            // Store in Session
            UserSession.CurrentPCNO = pcno;
            UserSession.CurrentUserName = empName;
            UserSession.Designation = designation;
            UserSession.Division = division;
            UserSession.Role = isAdmin ? "Admin" : "User";

            FormsAuthentication.SetAuthCookie(pcno, false);
            RedirectByRole();
        }

        private void RedirectByRole()
        {
            if (UserSession.IsAdmin)
            {
                Response.Redirect("AdminDashboard.aspx");
            }
            else
            {
                Response.Redirect("EmployeeEducation.aspx");
            }
        }

        private void ShowError(string msg)
        {
            pnlError.Visible = true;
            litErrorMsg.Text = msg;
        }
    }
}
