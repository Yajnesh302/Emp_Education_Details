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
                   !DBHelper.IsValidGrade(grade != null ? grade.ToString() : "");
        }

        public string RenderFieldValue(object value, string fieldPrompt)
        {
            string s = value != null ? value.ToString().Trim() : "";

            // Specialization / Major Subject can legitimately have "N/A", "NA", "Not Applicable", "General" etc.
            if (fieldPrompt != null && fieldPrompt.IndexOf("Specialization", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (DBHelper.IsInvalidSpecialization(s))
                {
                    return "<span class='text-muted' style='color:#94a3b8; font-weight:500;'>&mdash;</span>";
                }
                return Server.HtmlEncode(s);
            }

            // Grade / Division must be a valid recognized grade
            if (fieldPrompt != null && fieldPrompt.IndexOf("Grade", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (!DBHelper.IsValidGrade(s))
                {
                    return "<span class='text-muted' style='color:#94a3b8; font-weight:500;'>&mdash;</span>";
                }
                return Server.HtmlEncode(DBHelper.NormalizeGrade(s));
            }

            if (fieldPrompt != null && fieldPrompt.IndexOf("College", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (s.IndexOf("Direct Certification", StringComparison.OrdinalIgnoreCase) >= 0 || s.IndexOf("Same as Issuing", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "<span class='badge badge-light border text-muted' style='font-size: 0.8rem; font-weight: normal;'><i class='fas fa-certificate text-primary mr-1'></i> Direct Certification</span>";
                }
                if (s.Equals("University Campus / Department", StringComparison.OrdinalIgnoreCase))
                {
                    return "<span class='text-muted small'><i class='fas fa-university mr-1 text-primary'></i> University Campus</span>";
                }
            }

            if (DBHelper.IsPlaceholderOrInvalid(s))
            {
                return "<span class='text-muted' style='color:#94a3b8; font-weight:500;'>&mdash;</span>";
            }
            return Server.HtmlEncode(s);
        }

        public string RenderScoreDisplay(object scoreVal)
        {
            string s = scoreVal != null ? scoreVal.ToString().Trim() : "";
            if (DBHelper.IsPlaceholderOrInvalid(s))
            {
                return "<span class='text-muted' style='color:#94a3b8; font-weight:500;'>&mdash;</span>";
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
                return "<span class='text-muted' style='color:#94a3b8; font-weight:500;'>&mdash;</span>";
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
                       "<div class='mt-1'><span class='badge-pending-review badge-record-incomplete' title='Imported from legacy records. Click Edit to verify and complete details.'><i class='fas fa-pen mr-1'></i> Details Pending</span></div>";
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
            string empName = pcno;
            string desig = "---";
            string div = "---";

            if (profile != null)
            {
                empName = profile["NAME"] != DBNull.Value ? profile["NAME"].ToString() : pcno;
                desig = profile["DESIGNATION"] != DBNull.Value ? profile["DESIGNATION"].ToString() : "---";
                div = profile["DIVNAME"] != DBNull.Value ? profile["DIVNAME"].ToString() : "---";
            }
            else
            {
                empName = UserSession.CurrentUserName;
                desig = UserSession.Designation;
                div = UserSession.Division;
            }

            litEmpName.Text = empName;
            litEmpDesignation.Text = desig;
            litEmpDivision.Text = div;

            if (imgEmpPhoto != null)
            {
                string defaultAvatarUrl = ResolveUrl("~/Static/Images/default_avatar.png");
                imgEmpPhoto.ImageUrl = defaultAvatarUrl;
                imgEmpPhoto.Attributes["data-default-avatar"] = defaultAvatarUrl;
                imgEmpPhoto.Attributes["onerror"] = "if (this.src !== this.getAttribute('data-default-avatar')) { this.src = this.getAttribute('data-default-avatar'); }";

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
            if (litCardQualCount != null) litCardQualCount.Text = qualCount.ToString();

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
                    litSubmissionStatusPill.Text = string.Format("<div class='emp-status-badge status-draft' style='background:#fffbeb; border:1px solid #fde68a; color:#92400e;'><i class='fas fa-pencil-alt mr-1'></i> {0} Draft ({1} Pending Review)</div>", draftCount, draftValidationErrors.Count);
                    phDraftNotice.Visible = false;
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

            // Universities & Certifying Bodies
            DataTable dtUniv = DBHelper.GetUniversities(true);
            ddlUniversity.DataSource = dtUniv;
            ddlUniversity.DataTextField = "UNIV_NAME";
            ddlUniversity.DataValueField = "UNIV_NAME";
            ddlUniversity.DataBind();
            ddlUniversity.Items.Insert(0, new ListItem("-- Select University / Board / Certifying Authority --", ""));

            string[] fallbackAuthorities = new string[] {
                "National Council for Vocational Training (NCVT) / DGT",
                "Directorate General of Training (DGT), Ministry of Skill Development",
                "Defence Research and Development Organisation (DRDO / LRDE)",
                "Centre for Development of Advanced Computing (C-DAC)",
                "National Institute of Electronics & Information Technology (NIELIT / DOEACC)",
                "State Board of Technical Education and Training (SBTET / DTE)",
                "Other Recognized Certifying Body / Institute",
                "Other Recognized University / Board"
            };
            foreach (var auth in fallbackAuthorities)
            {
                if (ddlUniversity.Items.FindByText(auth) == null)
                {
                    ddlUniversity.Items.Add(new ListItem(auth, auth));
                }
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
            ddlCollege.Items.Add(new ListItem("University Campus / Department (Studied directly at University)", "University Campus / Department"));
            ddlCollege.Items.Add(new ListItem("Direct Certification / Same as Issuing Authority (No Separate College)", "Direct Certification / Same as Issuing Authority"));
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

            // Check for University Campus / Department aliases or same as selected University
            if (item == null && (
                college.IndexOf("University Campus", StringComparison.OrdinalIgnoreCase) >= 0 ||
                college.IndexOf("Same as University", StringComparison.OrdinalIgnoreCase) >= 0 ||
                college.IndexOf("Univ Campus", StringComparison.OrdinalIgnoreCase) >= 0 ||
                college.IndexOf("Direct Certification", StringComparison.OrdinalIgnoreCase) >= 0 ||
                college.IndexOf("Same as Issuing", StringComparison.OrdinalIgnoreCase) >= 0 ||
                (ddlUniversity.SelectedItem != null && !string.IsNullOrEmpty(ddlUniversity.SelectedItem.Text) &&
                 ddlUniversity.SelectedItem.Text.Equals(college, StringComparison.OrdinalIgnoreCase))))
            {
                item = ddlCollege.Items.FindByValue("Direct Certification / Same as Issuing Authority") ??
                       ddlCollege.Items.FindByValue("University Campus / Department");
            }

            if (item != null && !item.Value.Equals("OTHER", StringComparison.OrdinalIgnoreCase))
            {
                ddlCollege.SelectedValue = item.Value;
                pnlCustomCollege.Style["display"] = "none";
                txtCustomCollege.Text = "";
            }
            else
            {
                // College is not in the recognized master list:
                // Select "OTHER" in dropdown and show the custom input field with the user's college name pre-filled
                if (ddlCollege.Items.FindByValue("OTHER") != null)
                {
                    ddlCollege.SelectedValue = "OTHER";
                }
                pnlCustomCollege.Style["display"] = "block";
                txtCustomCollege.Text = college.Equals("OTHER", StringComparison.OrdinalIgnoreCase) ? "" : college;
            }
        }

        private string GetSelectedCollegeName()
        {
            if (ddlCollege.SelectedValue == "Direct Certification / Same as Issuing Authority" || ddlCollege.SelectedValue == "University Campus / Department")
            {
                return ddlCollege.SelectedValue;
            }
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

            var seenPairs = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (dtSub != null)
            {
                foreach (DataRow r in dtSub.Rows)
                {
                    string subCode = r["SUB_CODE"].ToString().Trim();
                    string subName = r["SUB_NAME"].ToString().Trim();
                    string mainCode = r["MAIN_QUAL_CODE"] != DBNull.Value ? r["MAIN_QUAL_CODE"].ToString().Trim() : "";

                    string key = mainCode + "|" + subCode;
                    if (seenPairs.Contains(key)) continue;
                    seenPairs.Add(key);

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
            hfModalOpen.Value = "1";
            litModalTitle.Text = "Add Qualification Record";
            PopulateDropdowns();
            ClearModalForm();
            pnlModal.Style["display"] = "flex";
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlModal.Style["display"] = "none";
            hfModalOpen.Value = "0";
            hfEditEduId.Value = "0";
            pnlFormError.Visible = false;
        }

        public void OpenRecordForEdit(int eduId)
        {
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
                string subName = r["QUAL_SUB_NAME"] != DBNull.Value ? r["QUAL_SUB_NAME"].ToString().Trim() : "";

                // Normalize 10th standard records that had legacy codes ('GEN', 'NA') or incorrect veterinary label
                if ((mainCode == "MTRI" || mainCode == "10TH") && (subCode == "GEN" || subCode == "NA" || subName.IndexOf("Animal Genetic", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    subCode = "GENERAL";
                    subName = "General (All Subjects)";
                }
                else if (mainCode == "BMTRI" && (subCode == "GEN" || subCode == "NA" || subName.IndexOf("Animal Genetic", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    subCode = "GENERAL";
                    subName = "General (Below 10th)";
                }
                else
                {
                    string canonicalSubName = DBHelper.GetSubQualificationName(mainCode, subCode);
                    if (!string.IsNullOrEmpty(canonicalSubName))
                    {
                        subName = canonicalSubName;
                    }
                }

                hfSelectedSubQual.Value = subCode;
                hfSelectedSubQualName.Value = subName;
                if (ddlSubQual.Items.FindByValue(subCode) != null)
                {
                    ddlSubQual.SelectedValue = subCode;
                }
                else
                {
                    ddlSubQual.Items.Add(new ListItem(!string.IsNullOrEmpty(subName) ? subName : subCode, subCode));
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
                inst = DBHelper.NormalizeUniversityName(inst);

                if (!DBHelper.IsPlaceholderOrInvalid(inst))
                {
                    string canonical = DBHelper.EnsureUniversityExists(inst);
                    if (!string.IsNullOrEmpty(canonical))
                    {
                        inst = canonical;
                    }

                    if (ddlUniversity.Items.FindByValue(inst) != null)
                    {
                        ddlUniversity.SelectedValue = inst;
                    }
                    else
                    {
                        // If the university is not in the list, leave it blank (do NOT add a new item)
                        ddlUniversity.SelectedIndex = 0;
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
                
                string rawGrade = r["GRADE_DIVISION"] != DBNull.Value ? r["GRADE_DIVISION"].ToString() : "";
                string normGrade = DBHelper.NormalizeGrade(rawGrade);
                if (!string.IsNullOrEmpty(normGrade) && ddlGrade.Items.FindByValue(normGrade) != null)
                {
                    ddlGrade.SelectedValue = normGrade;
                }
                else
                {
                    ddlGrade.SelectedIndex = 0;
                }

                chkIsHighest.Checked = (Convert.ToInt32(r["IS_HIGHEST"]) == 1);
                string rem = r["REMARKS"] != DBNull.Value ? r["REMARKS"].ToString() : "";
                if (!string.IsNullOrEmpty(rem) && (rem.StartsWith("Migrated from", StringComparison.OrdinalIgnoreCase) || rem.StartsWith("Auto-synced", StringComparison.OrdinalIgnoreCase)))
                {
                    rem = "";
                }
                txtRemarks.Text = rem;

                bool hasPlaceholders = DBHelper.IsPlaceholderOrInvalid(college) ||
                                       DBHelper.IsPlaceholderOrInvalid(inst) ||
                                       DBHelper.IsInvalidSpecialization(specVal) ||
                                       DBHelper.IsPlaceholderOrInvalid(cgpaVal) ||
                                       !DBHelper.IsValidGrade(rawGrade) ||
                                       DBHelper.IsPlaceholderOrInvalid(yr);

                if (hasPlaceholders)
                {
                    pnlFormError.Visible = true;
                    pnlFormError.CssClass = "alert alert-warning py-2 small mb-3";
                    litFormError.Text = "<i class='fas fa-info-circle mr-1 text-warning'></i> <strong>Imported Record:</strong> Please verify and complete the details for this qualification before saving.";
                }
                else
                {
                    pnlFormError.Visible = false;
                }
                pnlModal.Style["display"] = "flex";
                hfModalOpen.Value = "1";
            }
        }

        protected void gvEducation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRecord")
            {
                int eduId = Convert.ToInt32(e.CommandArgument);
                OpenRecordForEdit(eduId);
            }
            else if (e.CommandName == "DeleteRecord")
            {
                pnlModal.Style["display"] = "none";
                hfModalOpen.Value = "0";
                hfEditEduId.Value = "0";
                pnlFormError.Visible = false;

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
                pnlModal.Style["display"] = "none";
                hfModalOpen.Value = "0";
                hfEditEduId.Value = "0";
                pnlFormError.Visible = false;

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
                pnlModal.Style["display"] = "none";
                hfModalOpen.Value = "0";
                hfEditEduId.Value = "0";
                pnlFormError.Visible = false;

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

            // Normalize Matriculation / 10th sub-qualifications
            if ((mainCode == "MTRI" || mainCode == "10TH" || mainCode == "BMTRI") && (subCode == "NA" || subCode == "GEN"))
            {
                subCode = "GENERAL";
            }

            // Validation of required fields
            if (string.IsNullOrEmpty(mainCode) || DBHelper.IsPlaceholderOrInvalid(mainCode))
            {
                ShowFormError("Please select a valid Main Qualification from the list.");
                return;
            }
            if (string.IsNullOrEmpty(subCode) || (DBHelper.IsPlaceholderOrInvalid(subCode) && subCode != "GENERAL"))
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
                bool isCert = DBHelper.IsCertificateQualification(ddlMainQual.SelectedValue, ddlMainQual.SelectedItem != null ? ddlMainQual.SelectedItem.Text : "");
                string fieldName = isCert ? "Certifying Body / Issuing Authority" : "University / Board";
                ShowFormError("Please select a valid " + fieldName + " from the list. Placeholder values like '*' are not allowed.");
                return;
            }
            string college = GetSelectedCollegeName();
            if (string.IsNullOrEmpty(college) || DBHelper.IsPlaceholderOrInvalid(college))
            {
                bool isCert = DBHelper.IsCertificateQualification(ddlMainQual.SelectedValue, ddlMainQual.SelectedItem != null ? ddlMainQual.SelectedItem.Text : "");
                if (isCert)
                {
                    college = "Direct Certification / Same as Issuing Authority";
                }
                else
                {
                    ShowFormError("Please select or enter a valid College / School Name. Placeholder values like '*' are not allowed.");
                    return;
                }
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

            string selectedGrade = ddlGrade.SelectedValue;
            string grade = DBHelper.NormalizeGrade(selectedGrade);
            if (string.IsNullOrEmpty(grade) || !DBHelper.IsValidGrade(grade))
            {
                ShowFormError("Please select a valid Grade / Division from the list. Grade / Division is mandatory.");
                return;
            }

            int eduId = 0;
            int.TryParse(hfEditEduId.Value, out eduId);

            string mainName = (ddlMainQual.SelectedItem != null && !string.IsNullOrEmpty(ddlMainQual.SelectedItem.Text)) 
                ? ddlMainQual.SelectedItem.Text : mainCode;

            // Resolve sub-qualification name from hidden field or scoped DB lookup to prevent cross-degree collision
            string clientSubName = !string.IsNullOrEmpty(hfSelectedSubQualName.Value) ? hfSelectedSubQualName.Value.Trim() : null;
            string dbSubName = DBHelper.GetSubQualificationName(mainCode, subCode);
            string subName = !string.IsNullOrEmpty(clientSubName) 
                ? clientSubName 
                : (!string.IsNullOrEmpty(dbSubName) ? dbSubName : ((ddlSubQual.SelectedItem != null && !string.IsNullOrEmpty(ddlSubQual.SelectedItem.Text)) ? ddlSubQual.SelectedItem.Text : subCode));

            // Safeguard against 'Animal Genetics and Breeding' overriding 10th Standard or non-veterinary degrees
            if ((mainCode == "MTRI" || mainCode == "10TH" || mainCode == "BMTRI" || !mainCode.StartsWith("BV")) 
                && subName.IndexOf("Animal Genetic", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (mainCode == "MTRI" || mainCode == "10TH")
                {
                    subCode = "GENERAL";
                    subName = "General (All Subjects)";
                }
                else if (mainCode == "BMTRI")
                {
                    subCode = "GENERAL";
                    subName = "General (Below 10th)";
                }
                else
                {
                    subName = !string.IsNullOrEmpty(dbSubName) ? dbSubName : "General";
                }
            }

            if (subCode == "GENERAL" && (mainCode == "MTRI" || mainCode == "10TH"))
            {
                subName = "General (All Subjects)";
            }
            else if (subCode == "GENERAL" && mainCode == "BMTRI")
            {
                subName = "General (Below 10th)";
            }
            int year = Convert.ToInt32(ddlYearPassed.SelectedValue);
            string spec = txtSpecialization.Text.Trim();
            string inst = ddlUniversity.SelectedValue;
            if (!string.IsNullOrEmpty(inst) && !DBHelper.IsPlaceholderOrInvalid(inst))
            {
                string canonical = DBHelper.EnsureUniversityExists(inst);
                inst = !string.IsNullOrEmpty(canonical) ? canonical : inst;
            }
            else
            {
                inst = null;
            }
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
                hfModalOpen.Value = "0";
                hfEditEduId.Value = "0";
                pnlFormError.Visible = false;
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
            pnlModal.Style["display"] = "none";
            hfModalOpen.Value = "0";
            hfEditEduId.Value = "0";
            pnlFormError.Visible = false;
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
            hfSelectedSubQualName.Value = "";
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
            hfModalOpen.Value = "1";

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

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (hfModalOpen.Value != "1")
            {
                pnlModal.Style["display"] = "none";
            }
        }
    }
}
