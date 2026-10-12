using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using EducationApp.Utils;

namespace EducationApp
{
    public partial class EmployeeEducation : System.Web.UI.Page
    {
        public string CurrentPCNO
        {
            get { return UserSession.CurrentPCNO; }
        }

        private string _highestQualTitle = "---";
        public string HighestQualTitle
        {
            get { return _highestQualTitle; }
            set { _highestQualTitle = value; }
        }

        public bool CanEditRecord(object status)
        {
            string s = status != null ? status.ToString() : "";
            return s.Equals("Draft", StringComparison.OrdinalIgnoreCase) || UserSession.IsAdmin;
        }

        public bool IsLockedRecord(object status)
        {
            string s = status != null ? status.ToString() : "";
            return s.Equals("Submitted", StringComparison.OrdinalIgnoreCase) && !UserSession.IsAdmin;
        }

        public bool IsRecordIncomplete(object mainCode, object subCode, object year, object inst, object college, object spec, object cgpa, object grade, object status)
        {
            string st = status != null ? status.ToString() : "";
            if (!st.Equals("Draft", StringComparison.OrdinalIgnoreCase)) return false;

            int yr = 0;
            if (year != null) int.TryParse(year.ToString(), out yr);

            return DBHelper.IsPlaceholderOrInvalid(mainCode != null ? mainCode.ToString() : "") ||
                   DBHelper.IsPlaceholderOrInvalid(subCode != null ? subCode.ToString() : "") ||
                   yr < 1950 ||
                   DBHelper.IsPlaceholderOrInvalid(inst != null ? inst.ToString() : "") ||
                   DBHelper.IsPlaceholderOrInvalid(college != null ? college.ToString() : "") ||
                   DBHelper.IsInvalidSpecialization(spec != null ? spec.ToString() : "") ||
                   DBHelper.IsPlaceholderOrInvalid(cgpa != null ? cgpa.ToString() : "") ||
                   DBHelper.IsPlaceholderOrInvalid(grade != null ? grade.ToString() : "");
        }

        public string RenderFieldValue(object value, string fieldPrompt)
        {
            string s = value != null ? value.ToString().Trim() : "";

            // Specialization / Major Subject can legitimately have "N/A", "NA", "Not Applicable", "General" etc.
            if (fieldPrompt != null && fieldPrompt.IndexOf("Specialization", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (DBHelper.IsInvalidSpecialization(s))
                {
                    return string.Format("<span class='text-danger font-weight-bold' title='Placeholder or empty Specialization ({0}). Please click Edit to enter a valid value.'><i class='fas fa-exclamation-circle mr-1'></i>{1}</span>", 
                        Server.HtmlEncode(s), string.IsNullOrEmpty(s) ? "(Needs Subject/NA)" : Server.HtmlEncode(s) + " (Enter valid)");
                }
                return Server.HtmlEncode(s);
            }

            if (DBHelper.IsPlaceholderOrInvalid(s))
            {
                return string.Format("<span class='text-danger font-weight-bold' title='Placeholder or unselected value ({0}). Please click Edit to select a valid option.'><i class='fas fa-exclamation-circle mr-1'></i>{1}</span>", 
                    Server.HtmlEncode(s), s == "" ? "(Needs Selection)" : Server.HtmlEncode(s) + " (Select valid)");
            }
            return Server.HtmlEncode(s);
        }

        public string RenderScoreDisplay(object scoreVal)
        {
            string s = scoreVal != null ? scoreVal.ToString().Trim() : "";
            if (DBHelper.IsPlaceholderOrInvalid(s))
            {
                return string.Format("<span class='text-danger font-weight-bold' title='Missing or placeholder score ({0}). Please click Edit to enter your valid Percentage or CGPA.'><i class='fas fa-exclamation-circle mr-1'></i>{1}</span>", 
                    Server.HtmlEncode(s), string.IsNullOrEmpty(s) ? "(Needs Score)" : Server.HtmlEncode(s) + " (Enter valid)");
            }

            // If already formatted with % or CGPA or Marks
            if (s.Contains("%") || s.ToUpper().Contains("CGPA") || s.ToUpper().Contains("MARKS") || s.Contains("/"))
            {
                return "<span class='font-weight-bold text-dark'>" + Server.HtmlEncode(s) + "</span>";
            }

            // Numeric check: If > 10 and <= 100, format as Percentage; if <= 10, format as CGPA
            double num;
            if (double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out num))
            {
                if (num > 10.0 && num <= 100.0)
                {
                    return "<span class='font-weight-bold text-dark'>" + num.ToString("0.##") + "%</span>";
                }
                else if (num > 0.0 && num <= 10.0)
                {
                    return "<span class='font-weight-bold text-dark'>" + num.ToString("0.##") + " CGPA</span>";
                }
            }

            return "<span class='font-weight-bold text-dark'>" + Server.HtmlEncode(s) + "</span>";
        }

        public string RenderYearValue(object yearVal)
        {
            int yr = 0;
            if (yearVal != null) int.TryParse(yearVal.ToString(), out yr);
            if (yr < 1950 || yr > DateTime.Now.Year)
            {
                return "<span class='text-danger font-weight-bold' title='Missing or invalid passing year. Please click Edit to select proper year.'><i class='fas fa-exclamation-circle mr-1'></i>" + (yr > 0 ? yr.ToString() : "Missing") + "</span>";
            }
            return "<span class='font-weight-bold text-dark'>" + yr.ToString() + "</span>";
        }

        public string RenderStatusBadge(object status, object mainCode, object subCode, object year, object inst, object college, object spec, object cgpa, object grade)
        {
            string st = status != null ? status.ToString() : "";
            if (st.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
            {
                return "<span class='badge-status badge-submitted'><i class='fas fa-check-circle mr-1'></i> Submitted</span>";
            }

            bool isIncomplete = IsRecordIncomplete(mainCode, subCode, year, inst, college, spec, cgpa, grade, status);
            if (isIncomplete)
            {
                return "<span class='badge-status badge-draft'><i class='fas fa-pencil-alt mr-1'></i> Draft</span>" +
                       "<div class='mt-1'><span class='badge badge-danger text-white px-2 py-0.5 badge-record-incomplete' style='font-size:0.7rem; font-weight:700;' title='Contains placeholder values (*). Must be edited before submitting.'><i class='fas fa-exclamation-triangle mr-1'></i>Action Required</span></div>";
            }
            return "<span class='badge-status badge-draft'><i class='fas fa-pencil-alt mr-1'></i> Draft</span>";
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
                LoadEmployeeHeader();
                BindEducationGrid();
                PopulateDropdowns();
            }
        }

        private void LoadEmployeeHeader()
        {
            string pcno = CurrentPCNO;
            litEmpPCNO.Text = pcno;

            DataRow profile = DBHelper.GetEmployeeProfile(pcno);
            if (profile != null)
            {
                litEmpName.Text = profile["NAME"] != DBNull.Value ? profile["NAME"].ToString() : pcno;
                litEmpDesignation.Text = profile["DESIGNATION"] != DBNull.Value ? profile["DESIGNATION"].ToString() : "---";
                litEmpDivision.Text = profile["DIVNAME"] != DBNull.Value ? profile["DIVNAME"].ToString() : "---";
            }
            else
            {
                litEmpName.Text = UserSession.CurrentUserName;
                litEmpDesignation.Text = UserSession.Designation;
                litEmpDivision.Text = UserSession.Division;
            }

            if (imgEmpPhoto != null)
            {
                imgEmpPhoto.ImageUrl = "Static/Images/default_avatar.png";
                imgEmpPhoto.Attributes["onerror"] = "this.onerror=null; this.src='Static/Images/default_avatar.png';";

                bool enableIntranetPhotos = string.Equals(System.Configuration.ConfigurationManager.AppSettings["EnableIntranetPhotos"], "true", StringComparison.OrdinalIgnoreCase);
                if (enableIntranetPhotos && !string.IsNullOrEmpty(pcno) && pcno != "---")
                {
                    imgEmpPhoto.Attributes["data-intranet-photo"] = string.Format("http://windflower.lrde.com/empphotos/Dsc_{0}.jpg", pcno);
                }
                else
                {
                    imgEmpPhoto.Attributes.Remove("data-intranet-photo");
                }
            }
        }

        private void BindEducationGrid()
        {
            DataTable dt = DBHelper.GetEmployeeEducations(CurrentPCNO);
            gvEducation.DataSource = dt;
            gvEducation.DataBind();

            if (gvEducation.HeaderRow != null)
            {
                gvEducation.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            bool hasRecords = dt != null && dt.Rows.Count > 0;
            int draftCount = 0;
            int submittedCount = 0;

            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    string st = r["STATUS"] != DBNull.Value ? r["STATUS"].ToString() : "";
                    if (st.Equals("Draft", StringComparison.OrdinalIgnoreCase))
                    {
                        draftCount++;
                    }
                    else if (st.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
                    {
                        submittedCount++;
                    }
                }
            }

            bool hasDrafts = draftCount > 0;

            // Users can ALWAYS add new qualifications
            btnOpenAddModal.Visible = true;
            if (btnSidebarAdd != null) btnSidebarAdd.Visible = true;

            // Sidebar stats
            int qualCount = (dt != null) ? dt.Rows.Count : 0;
            litStatQualCount.Text = qualCount.ToString();

            string highestName = "None";
            HighestQualTitle = "None";
            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToInt32(r["IS_HIGHEST"]) == 1)
                    {
                        highestName = r["QUAL_MAIN_CODE"].ToString();
                        HighestQualTitle = r["QUAL_MAIN_NAME"].ToString();
                        break;
                    }
                }
            }
            litStatHighestQual.Text = highestName;

            var draftValidationErrors = DBHelper.ValidateDraftRecordsForSubmission(CurrentPCNO);
            bool hasIncompleteDrafts = (draftValidationErrors != null && draftValidationErrors.Count > 0);

            if (hasDrafts)
            {
                if (hasIncompleteDrafts)
                {
                    litSubmissionStatusPill.Text = string.Format("<div class='emp-status-badge status-draft' style='background:#fef2f2; border:1px solid #fecaca; color:#b91c1c;'><i class='fas fa-exclamation-triangle mr-1'></i> {0} Draft ({1} Incomplete)</div>", draftCount, draftValidationErrors.Count);
                    phDraftNotice.Visible = true;
                    phDraftIncompleteNotice.Visible = true;
                    litDraftIncompleteCount.Text = draftValidationErrors.Count.ToString();
                    litDraftIncompleteDetails.Text = string.Join("", draftValidationErrors.ConvertAll(err => "<li class='mb-1'>" + err + "</li>"));
                }
                else
                {
                    litSubmissionStatusPill.Text = string.Format("<div class='emp-status-badge status-draft'><i class='fas fa-pencil-alt mr-1'></i> {0} Draft Record{1} Ready</div>", draftCount, draftCount > 1 ? "s" : "");
                    phDraftNotice.Visible = true;
                    phDraftIncompleteNotice.Visible = false;
                }
                phSubmittedNotice.Visible = false;
                phBottomSubmitBtn.Visible = true;
            }
            else if (hasRecords)
            {
                litSubmissionStatusPill.Text = "<div class='emp-status-badge status-submitted'><i class='fas fa-check-circle mr-1'></i> Submitted &amp; Verified</div>";
                phSubmittedNotice.Visible = true;
                phDraftNotice.Visible = false;
                phDraftIncompleteNotice.Visible = false;
                phBottomSubmitBtn.Visible = false;
            }
            else
            {
                litSubmissionStatusPill.Text = "<div class='emp-status-badge status-empty'><i class='fas fa-info-circle mr-1'></i> No Qualifications Recorded</div>";
                phSubmittedNotice.Visible = false;
                phDraftNotice.Visible = false;
                phDraftIncompleteNotice.Visible = false;
                phBottomSubmitBtn.Visible = false;
            }
        }

        private void PopulateDropdowns()
        {
            // Main Qualifications
            DataTable dtMain = DBHelper.GetMainQualifications(true);
            ddlMainQual.DataSource = dtMain;
            ddlMainQual.DataTextField = "DISPLAY_NAME";
            ddlMainQual.DataValueField = "QUAL_CODE";
            ddlMainQual.DataBind();
            ddlMainQual.Items.Insert(0, new ListItem("-- Select Main Qualification --", ""));

            // Universities
            DataTable dtUniv = DBHelper.GetUniversities(true);
            ddlUniversity.DataSource = dtUniv;
            ddlUniversity.DataTextField = "UNIV_NAME";
            ddlUniversity.DataValueField = "UNIV_NAME";
            ddlUniversity.DataBind();
            ddlUniversity.Items.Insert(0, new ListItem("-- Select University / Board --", ""));
            if (ddlUniversity.Items.FindByText("Other Recognized University / Board") == null)
            {
                ddlUniversity.Items.Add(new ListItem("Other Recognized University / Board", "Other Recognized University / Board"));
            }

            // Passing Years (Descending from current year to 1960)
            ddlYearPassed.Items.Clear();
            ddlYearPassed.Items.Add(new ListItem("-- Year --", ""));
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear; y >= 1960; y--)
            {
                ddlYearPassed.Items.Add(new ListItem(y.ToString(), y.ToString()));
            }

            // Colleges / Schools
            PopulateColleges();

            // Load All Sub Qualifications with data-main attribute
            PopulateAllSubQualifications();
        }

        private void PopulateColleges()
        {
            DataTable dtColleges = DBHelper.GetColleges();
            ddlCollege.Items.Clear();
            ddlCollege.Items.Add(new ListItem("-- Select College / School Name --", ""));
            if (dtColleges != null)
            {
                foreach (DataRow r in dtColleges.Rows)
                {
                    string cName = r["COLLEGE_NAME"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cName))
                    {
                        ddlCollege.Items.Add(new ListItem(cName, cName));
                    }
                }
            }
            ddlCollege.Items.Add(new ListItem("Other / College Not in List (Type Manually)", "OTHER"));
        }

        private void BindCollegeSelection(string college)
        {
            if (string.IsNullOrEmpty(college) || DBHelper.IsPlaceholderOrInvalid(college))
            {
                ResetCollegeSelection();
                return;
            }

            ListItem item = ddlCollege.Items.FindByValue(college);
            if (item == null)
            {
                foreach (ListItem itm in ddlCollege.Items)
                {
                    if (itm.Value.Equals(college, StringComparison.OrdinalIgnoreCase))
                    {
                        item = itm;
                        break;
                    }
                }
            }

            if (item != null)
            {
                ddlCollege.SelectedValue = item.Value;
                pnlCustomCollege.Style["display"] = "none";
                txtCustomCollege.Text = "";
            }
            else
            {
                ddlCollege.Items.Insert(1, new ListItem(college, college));
                ddlCollege.SelectedValue = college;
                pnlCustomCollege.Style["display"] = "none";
                txtCustomCollege.Text = college;
            }
        }

        private string GetSelectedCollegeName()
        {
            if (ddlCollege.SelectedValue == "OTHER" || ddlCollege.SelectedValue.StartsWith("Other"))
            {
                return txtCustomCollege.Text.Trim();
            }
            return ddlCollege.SelectedValue.Trim();
        }

        private void ResetCollegeSelection()
        {
            if (ddlCollege.Items.Count > 0) ddlCollege.SelectedIndex = 0;
            txtCustomCollege.Text = "";
            pnlCustomCollege.Style["display"] = "none";
        }

        private void PopulateAllSubQualifications()
        {
            DataTable dtSub = DBHelper.GetSubQualifications(null, true);
            ddlSubQual.Items.Clear();
            ddlSubQual.Items.Add(new ListItem("-- Select Sub Qualification / Stream --", ""));

            var subList = new System.Collections.Generic.List<object>();

            if (dtSub != null)
            {
                foreach (DataRow r in dtSub.Rows)
                {
                    string subCode = r["SUB_CODE"].ToString().Trim();
                    string subName = r["SUB_NAME"].ToString().Trim();
                    string mainCode = r["MAIN_QUAL_CODE"] != DBNull.Value ? r["MAIN_QUAL_CODE"].ToString().Trim() : "";

                    ListItem li = new ListItem(subName, subCode);
                    if (!string.IsNullOrEmpty(mainCode))
                    {
                        li.Attributes["data-main"] = mainCode;
                    }
                    ddlSubQual.Items.Add(li);

                    subList.Add(new { code = subCode, name = subName, main = mainCode });
                }
            }

            // Fallbacks
            if (ddlSubQual.Items.FindByValue("GENERAL") == null)
            {
                ListItem liGen = new ListItem("General", "GENERAL");
                liGen.Attributes["data-main"] = "";
                ddlSubQual.Items.Add(liGen);
                subList.Add(new { code = "GENERAL", name = "General", main = "" });
            }
            if (ddlSubQual.Items.FindByValue("OTHER") == null)
            {
                ListItem liOth = new ListItem("Other (Specify in Specialization)", "OTHER");
                liOth.Attributes["data-main"] = "";
                ddlSubQual.Items.Add(liOth);
                subList.Add(new { code = "OTHER", name = "Other (Specify in Specialization)", main = "" });
            }

            // Register global JSON data so client-side JavaScript never loses sub qualifications across postbacks
            string json = DBHelper.GetSubQualificationsJson();
            string script = string.Format("window.EDU_ALL_SUB_QUALS = {0};", json);
            if (ScriptManager.GetCurrent(this) != null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SubQualJsonData", script, true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SubQualJsonData", script, true);
            }
        }

        public string GetSubQualsJson()
        {
            return DBHelper.GetSubQualificationsJson();
        }

        private void PopulateSubQualDropdown(string mainCode)
        {
            PopulateAllSubQualifications();
        }

        protected void ddlMainQual_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retained for postback compatibility
        }

        protected void btnOpenAddModal_Click(object sender, EventArgs e)
        {
            hfEditEduId.Value = "0";
            litModalTitle.Text = "Add Qualification Record";
            PopulateDropdowns();
            ClearModalForm();
            pnlModal.Style["display"] = "flex";
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlModal.Style["display"] = "none";
            pnlFormError.Visible = false;
        }

        protected void gvEducation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRecord")
            {
                int eduId = Convert.ToInt32(e.CommandArgument);
                DataRow r = DBHelper.GetEducationRecordById(eduId);
                if (r != null)
                {
                    string recStatus = r["STATUS"] != DBNull.Value ? r["STATUS"].ToString() : "";
                    if (!UserSession.IsAdmin && recStatus.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowAlert("Submitted qualification records are locked as official entries and cannot be edited.", "warning");
                        return;
                    }

                    hfEditEduId.Value = eduId.ToString();
                    litModalTitle.Text = "Edit Qualification Record";

                    string mainCode = r["QUAL_MAIN_CODE"].ToString();
                    hfSelectedMainQual.Value = mainCode;
                    if (ddlMainQual.Items.FindByValue(mainCode) != null)
                    {
                        ddlMainQual.SelectedValue = mainCode;
                    }
                    PopulateSubQualDropdown(mainCode);

                    string subCode = r["QUAL_SUB_CODE"].ToString();
                    hfSelectedSubQual.Value = subCode;
                    if (ddlSubQual.Items.FindByValue(subCode) != null)
                    {
                        ddlSubQual.SelectedValue = subCode;
                    }
                    else
                    {
                        ddlSubQual.Items.Add(new ListItem(subCode, subCode));
                        ddlSubQual.SelectedValue = subCode;
                    }

                    string yr = r["YEAR_PASSED"].ToString();
                    if (!DBHelper.IsPlaceholderOrInvalid(yr) && ddlYearPassed.Items.FindByValue(yr) != null)
                    {
                        ddlYearPassed.SelectedValue = yr;
                    }
                    else
                    {
                        ddlYearPassed.SelectedIndex = 0;
                    }

                    string specVal = r["SPECIALIZATION"] != DBNull.Value ? r["SPECIALIZATION"].ToString() : "";
                    txtSpecialization.Text = DBHelper.IsInvalidSpecialization(specVal) ? "" : specVal;
                    
                    string inst = r["INSTITUTE_NAME"] != DBNull.Value ? r["INSTITUTE_NAME"].ToString().Trim() : "";

                    // Canonical resolution for legacy abbreviations and outdated spellings
                    if (inst.Equals("VTU", StringComparison.OrdinalIgnoreCase) ||
                        inst.IndexOf("Visweswaraiah", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        inst.IndexOf("Visvesvaraya Technological", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        inst = "Visvesvaraya Technological University (VTU), Belagavi";
                    }
                    else if (inst.Equals("IISC", StringComparison.OrdinalIgnoreCase) || inst.Equals("IISC, BANGALORE", StringComparison.OrdinalIgnoreCase))
                    {
                        inst = "Indian Institute of Science (IISc), Bangalore";
                    }
                    else if (inst.Equals("JNTU", StringComparison.OrdinalIgnoreCase))
                    {
                        inst = "Jawaharlal Nehru Technological University (JNTU), Hyderabad";
                    }
                    else if (inst.Equals("IGNOU", StringComparison.OrdinalIgnoreCase) || inst.IndexOf("IGNOU, NEW DELHI", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        inst = "Indira Gandhi National Open University (IGNOU), New Delhi";
                    }
                    else if (inst.Equals("CBSE Board", StringComparison.OrdinalIgnoreCase) || inst.Equals("CBSE Board New Delhi", StringComparison.OrdinalIgnoreCase))
                    {
                        inst = "Central Board of Secondary Education (CBSE), New Delhi";
                    }
                    else if (inst.Equals("ICSE Board", StringComparison.OrdinalIgnoreCase) || inst.Equals("ISCE BOARD", StringComparison.OrdinalIgnoreCase) || inst.Equals("ICSE / ISC Board", StringComparison.OrdinalIgnoreCase))
                    {
                        inst = "Council for the Indian School Certificate Examinations (CISCE / ICSE / ISC)";
                    }
                    else if (inst.Equals("PUE", StringComparison.OrdinalIgnoreCase))
                    {
                        inst = "Department of Pre-University Education (PUE), Karnataka";
                    }

                    if (!DBHelper.IsPlaceholderOrInvalid(inst))
                    {
                        if (ddlUniversity.Items.FindByValue(inst) != null)
                        {
                            ddlUniversity.SelectedValue = inst;
                        }
                        else if (ddlUniversity.Items.Count > 0)
                        {
                            ddlUniversity.Items.Insert(1, new ListItem(inst, inst));
                            ddlUniversity.SelectedValue = inst;
                        }
                    }
                    else
                    {
                        ddlUniversity.SelectedIndex = 0;
                    }

                    string college = r["COLLEGE_NAME"] != DBNull.Value ? r["COLLEGE_NAME"].ToString() : "";
                    BindCollegeSelection(college);
                    
                    string cgpaVal = r["CGPA_MARKS"] != DBNull.Value ? r["CGPA_MARKS"].ToString() : "";
                    if (DBHelper.IsPlaceholderOrInvalid(cgpaVal))
                    {
                        txtCgpaMarks.Text = "";
                        ddlScoreType.SelectedValue = "Percentage";
                    }
                    else
                    {
                        string trimmed = cgpaVal.Trim();
                        if (trimmed.Contains("%"))
                        {
                            ddlScoreType.SelectedValue = "Percentage";
                            txtCgpaMarks.Text = trimmed;
                        }
                        else if (trimmed.ToUpper().Contains("CGPA"))
                        {
                            ddlScoreType.SelectedValue = "CGPA";
                            txtCgpaMarks.Text = trimmed;
                        }
                        else if (trimmed.Contains("/"))
                        {
                            ddlScoreType.SelectedValue = "Marks";
                            txtCgpaMarks.Text = trimmed;
                        }
                        else
                        {
                            double num;
                            if (double.TryParse(trimmed, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out num))
                            {
                                if (num > 10.0 && num <= 100.0)
                                {
                                    ddlScoreType.SelectedValue = "Percentage";
                                    txtCgpaMarks.Text = num.ToString("0.##") + "%";
                                }
                                else if (num <= 10.0)
                                {
                                    ddlScoreType.SelectedValue = "CGPA";
                                    txtCgpaMarks.Text = num.ToString("0.##");
                                }
                                else
                                {
                                    ddlScoreType.SelectedValue = "Marks";
                                    txtCgpaMarks.Text = trimmed;
                                }
                            }
                            else
                            {
                                ddlScoreType.SelectedValue = "Percentage";
                                txtCgpaMarks.Text = trimmed;
                            }
                        }
                    }
                    
                    string grade = r["GRADE_DIVISION"] != DBNull.Value ? r["GRADE_DIVISION"].ToString() : "";
                    if (!DBHelper.IsPlaceholderOrInvalid(grade) && ddlGrade.Items.FindByValue(grade) != null)
                    {
                        ddlGrade.SelectedValue = grade;
                    }
                    else
                    {
                        ddlGrade.SelectedIndex = 0;
                    }

                    chkIsHighest.Checked = (Convert.ToInt32(r["IS_HIGHEST"]) == 1);
                    txtRemarks.Text = r["REMARKS"] != DBNull.Value ? r["REMARKS"].ToString() : "";

                    bool hasPlaceholders = DBHelper.IsPlaceholderOrInvalid(college) ||
                                           DBHelper.IsPlaceholderOrInvalid(inst) ||
                                           DBHelper.IsInvalidSpecialization(specVal) ||
                                           DBHelper.IsPlaceholderOrInvalid(cgpaVal) ||
                                           DBHelper.IsPlaceholderOrInvalid(grade) ||
                                           DBHelper.IsPlaceholderOrInvalid(yr);

                    if (hasPlaceholders)
                    {
                        pnlFormError.Visible = true;
                        litFormError.Text = "<i class='fas fa-exclamation-triangle mr-1'></i> <strong>Incomplete Legacy Data:</strong> This record was imported with placeholder values (such as '*'). Please select your proper qualification, college/school, university, passing year, and grade before submitting.";
                    }
                    else
                    {
                        pnlFormError.Visible = false;
                    }
                    pnlModal.Style["display"] = "flex";
                }
            }
            else if (e.CommandName == "DeleteRecord")
            {
                int eduId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    bool deleted = DBHelper.DeleteEducationRecord(eduId, CurrentPCNO, UserSession.CurrentUserName, UserSession.IsAdmin);
                    if (deleted)
                    {
                        BindEducationGrid();
                        ShowAlert("Qualification record deleted successfully.", "success");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting record: " + ex.Message, "danger");
                }
            }
            else if (e.CommandName == "AdminRevert")
            {
                if (!UserSession.IsAdmin) return;
                int eduId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    bool reverted = DBHelper.AdminRevertToDraft(eduId, UserSession.CurrentPCNO, "Unlocked by Admin from Education page");
                    if (reverted)
                    {
                        BindEducationGrid();
                        ShowAlert("Record #" + eduId + " successfully reverted to Draft mode.", "success");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error reverting record: " + ex.Message, "danger");
                }
            }
            else if (e.CommandName == "AdminSubmit")
            {
                if (!UserSession.IsAdmin) return;
                int eduId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    bool submitted = DBHelper.AdminSubmitRecord(eduId, UserSession.CurrentPCNO, "Marked as Submitted and locked by Admin from Education page");
                    if (submitted)
                    {
                        BindEducationGrid();
                        ShowAlert("Record #" + eduId + " successfully changed to Submitted mode and locked.", "success");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error submitting record: " + ex.Message, "danger");
                }
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            SaveRecordWithStatus("Draft");
        }

        protected void btnSaveAndSubmit_Click(object sender, EventArgs e)
        {
            SaveRecordWithStatus("Submitted");
        }

        private void SaveRecordWithStatus(string status)
        {
            pnlFormError.Visible = false;

            string mainCode = ddlMainQual.SelectedValue;
            if (string.IsNullOrEmpty(mainCode) && !string.IsNullOrEmpty(hfSelectedMainQual.Value))
            {
                mainCode = hfSelectedMainQual.Value;
            }

            string subCode = ddlSubQual.SelectedValue;
            if (string.IsNullOrEmpty(subCode) && !string.IsNullOrEmpty(hfSelectedSubQual.Value))
            {
                subCode = hfSelectedSubQual.Value;
            }

            // Validation of required fields
            if (string.IsNullOrEmpty(mainCode) || DBHelper.IsPlaceholderOrInvalid(mainCode))
            {
                ShowFormError("Please select a valid Main Qualification from the list.");
                return;
            }
            if (string.IsNullOrEmpty(subCode) || DBHelper.IsPlaceholderOrInvalid(subCode))
            {
                ShowFormError("Please select a valid Sub Qualification / Stream from the list.");
                return;
            }
            if (string.IsNullOrEmpty(ddlYearPassed.SelectedValue) || DBHelper.IsPlaceholderOrInvalid(ddlYearPassed.SelectedValue))
            {
                ShowFormError("Please select a valid Passing Year from the list.");
                return;
            }
            if (string.IsNullOrEmpty(txtSpecialization.Text.Trim()) || DBHelper.IsInvalidSpecialization(txtSpecialization.Text.Trim()))
            {
                ShowFormError("Please enter Specialization / Major Subject (or 'N/A' if not applicable). Placeholder values like '*' are not allowed.");
                return;
            }
            if (string.IsNullOrEmpty(ddlUniversity.SelectedValue) || DBHelper.IsPlaceholderOrInvalid(ddlUniversity.SelectedValue))
            {
                ShowFormError("Please select a valid University / Board from the list. Placeholder values like '*' are not allowed.");
                return;
            }
            string college = GetSelectedCollegeName();
            if (string.IsNullOrEmpty(college) || DBHelper.IsPlaceholderOrInvalid(college))
            {
                ShowFormError("Please select or enter a valid College / School Name. Placeholder values like '*' are not allowed.");
                return;
            }
            string rawScore = txtCgpaMarks.Text.Trim();
            if (string.IsNullOrEmpty(rawScore) || DBHelper.IsPlaceholderOrInvalid(rawScore))
            {
                ShowFormError("Please enter valid Marks / Percentage / CGPA. Placeholder values like '*' are not allowed.");
                return;
            }

            string scoreType = ddlScoreType.SelectedValue;
            string cgpa = rawScore;
            double scoreNum;
            string cleanNumStr = rawScore.Replace("%", "").Replace("CGPA", "").Replace("cgpa", "").Trim();
            if (double.TryParse(cleanNumStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out scoreNum))
            {
                if (scoreType == "Percentage")
                {
                    if (scoreNum < 0 || scoreNum > 100)
                    {
                        ShowFormError("Percentage must be a valid value between 0.00% and 100.00%.");
                        return;
                    }
                    cgpa = scoreNum.ToString("0.##") + "%";
                }
                else if (scoreType == "CGPA")
                {
                    if (scoreNum < 0 || scoreNum > 10)
                    {
                        ShowFormError("CGPA must be a valid value between 0.00 and 10.00 (on standard 10-point scale).");
                        return;
                    }
                    cgpa = scoreNum.ToString("0.##") + " CGPA";
                }
            }

            if (string.IsNullOrEmpty(ddlGrade.SelectedValue) || DBHelper.IsPlaceholderOrInvalid(ddlGrade.SelectedValue))
            {
                ShowFormError("Please select a valid Grade / Division from the list.");
                return;
            }

            int eduId = 0;
            int.TryParse(hfEditEduId.Value, out eduId);

            string mainName = (ddlMainQual.SelectedItem != null && !string.IsNullOrEmpty(ddlMainQual.SelectedItem.Text)) 
                ? ddlMainQual.SelectedItem.Text : mainCode;
            string subName = (ddlSubQual.SelectedItem != null && !string.IsNullOrEmpty(ddlSubQual.SelectedItem.Text)) 
                ? ddlSubQual.SelectedItem.Text : subCode;
            int year = Convert.ToInt32(ddlYearPassed.SelectedValue);
            string spec = txtSpecialization.Text.Trim();
            string inst = ddlUniversity.SelectedValue;
            string grade = ddlGrade.SelectedValue;
            bool isHighest = chkIsHighest.Checked;
            string remarks = txtRemarks.Text.Trim();

            try
            {
                DBHelper.SaveEducationRecord(
                    eduId,
                    CurrentPCNO,
                    litEmpName.Text,
                    litEmpDesignation.Text,
                    litEmpDivision.Text,
                    mainCode,
                    mainName,
                    subCode,
                    subName,
                    year,
                    inst,
                    college,
                    spec,
                    cgpa,
                    grade,
                    isHighest,
                    remarks,
                    status,
                    UserSession.CurrentUserName);

                pnlModal.Style["display"] = "none";
                BindEducationGrid();

                if (status.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
                {
                    ShowAlert("Qualification record saved and submitted successfully!", "success");
                }
                else
                {
                    ShowAlert("Qualification record saved as Draft. You can edit it or click 'Submit Education Details' to finalize.", "info");
                }
            }
            catch (Exception ex)
            {
                ShowFormError("Error saving record: " + ex.Message);
            }
        }

        protected void btnFinalSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var validationErrors = DBHelper.ValidateDraftRecordsForSubmission(CurrentPCNO);
                if (validationErrors != null && validationErrors.Count > 0)
                {
                    string errList = "<ul class='mb-0 mt-2 pl-3'>" + string.Join("", validationErrors.ConvertAll(err => "<li>" + err + "</li>")) + "</ul>";
                    ShowAlert("<strong>Submission Blocked:</strong> Some draft qualification records contain incomplete or placeholder values (such as '*'). You cannot submit until proper values are selected for all fields:" + errList, "danger");
                    BindEducationGrid();
                    return;
                }

                int affected = DBHelper.SubmitAllEducationRecords(CurrentPCNO, UserSession.CurrentUserName);
                BindEducationGrid();
                ShowAlert("All education records have been successfully submitted and locked!", "success");
            }
            catch (Exception ex)
            {
                ShowAlert("Error submitting education records: " + ex.Message, "danger");
            }
        }

        private void ClearModalForm()
        {
            ddlMainQual.SelectedIndex = 0;
            ddlSubQual.SelectedIndex = 0;
            hfSelectedMainQual.Value = "";
            hfSelectedSubQual.Value = "";
            ddlYearPassed.SelectedIndex = 0;
            txtSpecialization.Text = "";
            ddlUniversity.SelectedIndex = 0;
            ResetCollegeSelection();
            ddlScoreType.SelectedIndex = 0;
            txtCgpaMarks.Text = "";
            ddlGrade.SelectedIndex = 0;
            chkIsHighest.Checked = false;
            txtRemarks.Text = "";
            pnlFormError.Visible = false;
        }

        private void ShowFormError(string msg)
        {
            pnlFormError.Visible = true;
            litFormError.Text = msg;
            pnlModal.Style["display"] = "flex";

            // Preserve Sub Qualification selection and ensure items are present
            string subCode = !string.IsNullOrEmpty(ddlSubQual.SelectedValue) ? ddlSubQual.SelectedValue : hfSelectedSubQual.Value;
            if (!string.IsNullOrEmpty(subCode))
            {
                hfSelectedSubQual.Value = subCode;
                if (ddlSubQual.Items.FindByValue(subCode) == null)
                {
                    ddlSubQual.Items.Add(new ListItem(subCode, subCode));
                }
                ddlSubQual.SelectedValue = subCode;
            }
        }

        private void ShowAlert(string msg, string type)
        {
            if (Master is SiteMaster)
            {
                ((SiteMaster)Master).ShowAlert(msg, type);
            }
            else
            {
                ShowToast(msg, type, 3000);
            }
        }

        private void ShowToast(string msg, string type = "info", int durationMs = 3000)
        {
            if (Master is SiteMaster)
            {
                ((SiteMaster)Master).ShowToast(msg, type, durationMs);
            }
            else
            {
                string cleanMsg = System.Web.HttpUtility.JavaScriptStringEncode(msg, false);
                string script = string.Format(
                    "(function(){{function r(){{if(window.EduApp&&EduApp.showToast){{EduApp.showToast('{0}','{1}',{2});}}else{{setTimeout(r,50);}}}}if(document.readyState==='loading'){{document.addEventListener('DOMContentLoaded',r);}}else{{r();}}}})();",
                    cleanMsg, type, durationMs
                );
                ClientScript.RegisterStartupScript(this.GetType(), "Toast_" + Guid.NewGuid().ToString("N"), script, true);
            }
        }
    }
}
