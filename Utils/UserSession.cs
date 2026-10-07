using System;
using System.Web;

namespace EducationApp.Utils
{
    public static class UserSession
    {
        public static string CurrentPCNO
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    return HttpContext.Current.Session["PCNO"] as string;
                }
                return null;
            }
            set
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["PCNO"] = value;
                }
            }
        }

        public static string CurrentUserName
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    return HttpContext.Current.Session["UserName"] as string ?? CurrentPCNO;
                }
                return null;
            }
            set
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["UserName"] = value;
                }
            }
        }

        public static string Designation
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    return HttpContext.Current.Session["Designation"] as string ?? "";
                }
                return "";
            }
            set
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["Designation"] = value;
                }
            }
        }

        public static string Division
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    return HttpContext.Current.Session["Division"] as string ?? "";
                }
                return "";
            }
            set
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["Division"] = value;
                }
            }
        }

        public static string Role
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    return HttpContext.Current.Session["Role"] as string ?? "User";
                }
                return "User";
            }
            set
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["Role"] = value;
                }
            }
        }

        public static bool IsAdmin
        {
            get
            {
                return Role != null && Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool IsAuthenticated
        {
            get
            {
                return !string.IsNullOrEmpty(CurrentPCNO);
            }
        }

        public static void Clear()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }
        }
    }
}
