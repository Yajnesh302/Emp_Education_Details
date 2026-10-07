using System;
using System.Web.Security;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            UserSession.Clear();
            Response.Redirect("Login.aspx");
        }
    }
}
