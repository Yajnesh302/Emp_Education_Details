using System;
using System.Configuration;
using System.IO;
using System.Web.UI;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class SiteMaster : MasterPage
    {
        public bool IsUserAdmin
        {
            get { return UserSession.IsAdmin; }
        }

        public string CurrentUserRole
        {
            get { return UserSession.Role; }
        }

        public string CurrentPCNO
        {
            get { return UserSession.CurrentPCNO ?? "---"; }
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
                phAdminNav.Visible = UserSession.IsAdmin;
            }
        }

        public string GetActiveNav(string pageName)
        {
            string currentPage = Path.GetFileName(Request.Path);
            return string.Equals(currentPage, pageName, StringComparison.OrdinalIgnoreCase) ? "active" : "";
        }

        public void ShowAlert(string message, string type = "success")
        {
            ShowToast(message, type, 3000);
        }

        public void ShowToast(string message, string type = "info", int durationMs = 3000)
        {
            pnlGlobalAlert.Visible = false;
            string cleanMsg = System.Web.HttpUtility.JavaScriptStringEncode(message, false);
            string script = string.Format(
                "(function(){{function r(){{if(window.EduApp&&EduApp.showToast){{EduApp.showToast('{0}','{1}',{2});}}else{{setTimeout(r,50);}}}}if(document.readyState==='loading'){{document.addEventListener('DOMContentLoaded',r);}}else{{r();}}}})();",
                cleanMsg, type, durationMs
            );
            if (ScriptManager.GetCurrent(Page) != null)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Toast_" + Guid.NewGuid().ToString("N"), script, true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "Toast_" + Guid.NewGuid().ToString("N"), script, true);
            }
        }
    }
}
