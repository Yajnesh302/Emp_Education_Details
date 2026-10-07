using System;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsAuthenticated)
            {
                Response.Redirect("Login.aspx");
            }
            else if (UserSession.IsAdmin)
            {
                Response.Redirect("AdminDashboard.aspx");
            }
            else
            {
                Response.Redirect("EmployeeEducation.aspx");
            }
        }
    }
}
