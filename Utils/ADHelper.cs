using System;
using System.DirectoryServices;
using System.Configuration;

namespace EducationApp.Utils
{
    public static class ADHelper
    {
        public static bool IsBypassAllowed
        {
            get
            {
                string val = ConfigurationManager.AppSettings["AllowOfflineBypass"];
                if (string.IsNullOrEmpty(val)) return false;
                bool result;
                if (bool.TryParse(val, out result)) return result;
                return val.Trim().Equals("true", StringComparison.OrdinalIgnoreCase)
                    || val.Trim().Equals("1", StringComparison.OrdinalIgnoreCase)
                    || val.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string AuthenticateAndGetPCNO(string username, string password)
        {
            if (string.IsNullOrEmpty(username)) return null;

            string normalizedUser = username.Trim().ToLower();

            // Local offline testing bypass controlled by Web.config ("AllowOfflineBypass")
            if (IsBypassAllowed)
            {
                if (normalizedUser == "1001" || normalizedUser == "admin" || normalizedUser == "aadmin")
                {
                    return "1001";
                }
                if (normalizedUser == "1002") return "1002";
                if (normalizedUser == "1003") return "1003";
                if (normalizedUser == "1004") return "1004";
                if (normalizedUser == "2947") return "2947";
                if (normalizedUser == "3350") return "3350";
            }

            string pcno = null;
            string ldapPath = ConfigurationManager.AppSettings["ADConnectionPath"];

            if (string.IsNullOrEmpty(ldapPath))
            {
                // Fallback to username as PCNO only if bypass is permitted
                if (IsBypassAllowed)
                {
                    return username.Trim();
                }
                throw new Exception("Active Directory LDAP path is not configured in Web.config (ADConnectionPath).");
            }

            try
            {
                // In C# DirectoryEntry doesn't validate credentials until NativeObject or search is performed
                using (DirectoryEntry entry = new DirectoryEntry(ldapPath, username, password))
                {
                    object native = entry.NativeObject; // Forces authentication check

                    using (DirectorySearcher search = new DirectorySearcher(entry))
                    {
                        search.Filter = "(SAMAccountName=" + username.Trim() + ")";
                        search.PropertiesToLoad.Add("EmployeeID");
                        SearchResult result = search.FindOne();

                        if (result != null)
                        {
                            using (DirectoryEntry dsresult = result.GetDirectoryEntry())
                            {
                                if (dsresult.Properties.Contains("EmployeeID"))
                                {
                                    pcno = dsresult.Properties["EmployeeID"][0].ToString();
                                }
                                else
                                {
                                    // Fallback if EmployeeID property is not populated
                                    pcno = username.Trim();
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Authentication succeeded, but user was not found in directory search.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (IsBypassAllowed)
                {
                    // If offline bypass is enabled and LDAP connection cannot be reached, allow username as PCNO
                    return username.Trim();
                }

                throw new Exception("Active Directory Error: " + ex.Message, ex);
            }

            return pcno;
        }
    }
}
