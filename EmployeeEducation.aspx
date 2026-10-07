<%@ Page Title="My Education Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmployeeEducation.aspx.cs" Inherits="EducationApp.EmployeeEducation" %>
<%@ Import Namespace="EducationApp.Utils" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .emp-profile-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 14px;
            box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03);
            padding: 1.25rem 1.5rem;
            margin-bottom: 1.5rem;
        }
        .emp-profile-avatar {
            width: 48px;
            height: 48px;
            min-width: 48px;
            border-radius: 12px;
            background: linear-gradient(135deg, #1e40af 0%, #3b82f6 100%);
            color: #ffffff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.35rem;
            box-shadow: 0 4px 10px rgba(37, 99, 235, 0.25);
            margin-right: 1.25rem;
        }
        .emp-meta-box {
            display: flex;
            flex-direction: column;
        }
        .emp-meta-label {
            font-size: 0.72rem;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            color: #64748b;
            margin-bottom: 2px;
        }
        .emp-meta-value {
            font-size: 0.95rem;
            font-weight: 700;
            color: #0f172a;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        .gold-star {
            color: #f59e0b;
            font-size: 0.9rem;
            margin-right: 3px;
        }
        /* Custom Modal Positioning */
        .custom-modal-backdrop {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            width: 100vw;
            height: 100vh;
            z-index: 1060;
            background: rgba(15, 23, 42, 0.65);
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px 16px;
            overflow-y: auto;
        }
        .custom-modal-dialog {
            width: 100%;
            max-width: 860px;
            margin: auto;
        }
        .custom-modal-content {
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.45);
            overflow: hidden;
            border: 1px solid rgba(226, 232, 240, 0.9);
        }
        .custom-modal-close {
            font-size: 1.75rem;
            font-weight: 400;
            line-height: 1;
            color: #64748b;
            text-decoration: none !important;
            padding: 0 8px;
            cursor: pointer;
            border: none;
            background: transparent;
            transition: color 0.15s ease;
        }
        .custom-modal-close:hover {
            color: #0f172a;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page Header & Action Bar -->
    <div class="d-flex flex-wrap justify-content-between align-items-center mb-4">
        <div>
            <h3 class="font-weight-bold text-gray-900 mb-1">
                <i class="fas fa-user-graduate text-primary mr-2"></i> My Education &amp; Qualification Details
            </h3>
            <p class="text-muted small mb-0">Record, review, and maintain your official educational qualifications</p>
        </div>
        <div class="mt-2 mt-md-0">
            <asp:Button ID="btnOpenAddModal" runat="server" Text="+ Add Qualification" CssClass="btn btn-edu-primary font-weight-bold px-3 py-2 shadow-sm" OnClick="btnOpenAddModal_Click" />
        </div>
    </div>

    <div class="row">
        <!-- LEFT SIDE: Employee Information & Photo Card -->
        <div class="emp-profile-sidebar-col col-12 mb-4">
            <div class="emp-side-card shadow-sm text-center">
                <!-- Decorative Top Banner -->
                <div class="emp-side-banner"></div>

                <!-- Profile Photo Frame -->
                <div class="emp-side-avatar-wrapper">
                    <div class="emp-side-avatar-frame">
                        <asp:Image ID="imgEmpPhoto" runat="server" AlternateText="Employee Photo" />
                    </div>
                </div>

                <!-- Employee Details -->
                <div class="p-3 pt-1 px-3">
                    <h5 class="font-weight-bold text-gray-900 mb-1" style="font-size: 1.12rem; line-height: 1.3;">
                        <asp:Literal ID="litEmpName" runat="server"></asp:Literal>
                    </h5>

                    <div class="mb-2">
                        <span class="badge badge-light border text-primary font-weight-bold px-2 py-1" style="font-size: 0.82rem;">
                            <i class="fas fa-id-badge mr-1"></i> PCNO: <asp:Literal ID="litEmpPCNO" runat="server"></asp:Literal>
                        </span>
                    </div>

                    <div class="text-muted small mb-1 font-weight-bold" style="font-size: 0.85rem;">
                        <asp:Literal ID="litEmpDesignation" runat="server">---</asp:Literal>
                    </div>

                    <div class="text-secondary small font-weight-bold mb-3" style="font-size: 0.85rem;">
                        <i class="fas fa-building mr-1 text-muted"></i>
                        <asp:Literal ID="litEmpDivision" runat="server">---</asp:Literal>
                    </div>

                    <hr class="my-2.5" style="border-top: 1px solid #f1f5f9;" />

                    <!-- Current Submission Status -->
                    <div class="mb-3">
                        <div class="text-uppercase text-muted font-weight-bold mb-1.5" style="font-size: 0.7rem; letter-spacing: 0.5px;">
                            Profile Status
                        </div>
                        <asp:Literal ID="litSubmissionStatusPill" runat="server"></asp:Literal>
                    </div>

                    <!-- Quick Academic Stats -->
                    <div class="p-2.5 rounded text-left mb-3" style="background: #f8fafc; border: 1px solid #e2e8f0;">
                        <div class="d-flex justify-content-between align-items-center mb-1">
                            <span class="small text-muted font-weight-600" style="font-size: 0.82rem;">Qualifications:</span>
                            <span class="badge badge-primary font-weight-bold px-2"><asp:Literal ID="litStatQualCount" runat="server">0</asp:Literal></span>
                        </div>
                        <div class="d-flex justify-content-between align-items-center">
                            <span class="small text-muted font-weight-600" style="font-size: 0.82rem;">Highest:</span>
                            <span class="small font-weight-bold text-gray-900 text-truncate ml-1" style="max-width: 145px; font-size: 0.82rem;">
                                <asp:Literal ID="litStatHighestQual" runat="server">---</asp:Literal>
                            </span>
                        </div>
                    </div>

                    <!-- Add Qualification Shortcut Button -->
                    <div>
                        <asp:Button ID="btnSidebarAdd" runat="server" Text="+ Add Qualification" CssClass="btn btn-edu-primary btn-block font-weight-bold shadow-sm py-2" style="font-size: 0.88rem;" OnClick="btnOpenAddModal_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- RIGHT SIDE: Main Qualifications Table & Actions -->
        <div class="emp-main-content-col col-12">
            <!-- Status Alerts / Guidance -->
            <asp:PlaceHolder ID="phSubmittedNotice" runat="server" Visible="false">
                <div class="alert alert-success d-flex align-items-center mb-4 py-3 px-4 shadow-sm" style="border-radius: 12px; border-left: 5px solid #10b981; background: #ecfdf5;">
                    <i class="fas fa-check-circle fa-2x text-success mr-3 flex-shrink-0"></i>
                    <div>
                        <div class="font-weight-bold text-gray-900" style="font-size: 1rem;">Education Records Submitted &amp; Verified</div>
                        <div class="small text-muted">
                            Your submitted qualification records are locked as official entries. You can add new qualifications anytime using the <strong>"+ Add Qualification"</strong> button; newly added entries can be saved as draft and submitted.
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phDraftNotice" runat="server" Visible="false">
                <div class="alert alert-warning d-flex flex-wrap align-items-center justify-content-between mb-4 py-3 px-4 shadow-sm" style="border-radius: 12px; border-left: 5px solid #f59e0b; background: #fffbeb;">
                    <div class="d-flex align-items-center mb-2 mb-md-0">
                        <i class="fas fa-pencil-alt fa-2x text-warning mr-3 flex-shrink-0"></i>
                        <div>
                            <div class="font-weight-bold text-gray-900" style="font-size: 1rem;">Unsubmitted Records in Draft Mode</div>
                            <div class="small text-muted">
                                You have qualification records saved as draft. You can add more, edit, or remove draft entries. Click <strong>"Submit Education Details"</strong> to officially submit and lock your records.
                            </div>
                        </div>
                    </div>
                    <div class="mt-2 mt-md-0 text-md-right">
                        <asp:Button ID="btnFinalSubmitBanner" runat="server" Text="Submit Education Details" CssClass="btn btn-edu-success font-weight-bold shadow-sm px-4 btn-submit-with-caution" OnClick="btnFinalSubmit_Click" />
                        <div class="small text-danger font-weight-bold mt-1">
                            <i class="fas fa-exclamation-triangle mr-1"></i> Caution: Once submitted, records cannot be edited
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <!-- Education Records Table Card -->
            <div class="edu-card shadow-sm mb-4">
        <div class="edu-card-header d-flex justify-content-between align-items-center px-4 py-3" style="padding-left: 1.25rem !important; padding-right: 1.25rem !important;">
            <div class="edu-card-title mb-0">
                <i class="fas fa-graduation-cap text-primary mr-2"></i> Recorded Qualifications
            </div>
            <div class="small text-muted">
                <i class="fas fa-info-circle mr-1"></i> Ensure all qualifications from 10th Standard onwards are recorded
            </div>
        </div>
        <div class="edu-card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvEducation" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                    UseAccessibleHeader="true"
                    OnRowCommand="gvEducation_RowCommand" EmptyDataText="No education details recorded yet. Click '+ Add Qualification' above to add your first qualification.">
                    <Columns>
                        <asp:TemplateField HeaderText="S.No" ItemStyle-Width="45px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <span class="font-weight-bold text-muted"><%# Container.DataItemIndex + 1 %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Main Qualification" ItemStyle-Width="180px">
                            <ItemTemplate>
                                <div class="font-weight-bold text-gray-900" style="font-size: 0.95rem; line-height: 1.35;"><%# Eval("QUAL_MAIN_NAME") %></div>
                                <div class="mt-1 d-inline-flex flex-wrap align-items-center" style="gap: 6px;">
                                    <span class="badge badge-light border text-muted px-2 py-1"><%# Eval("QUAL_MAIN_CODE") %></span>
                                    <%# Convert.ToInt32(Eval("IS_HIGHEST")) == 1 ? "<span class='badge-highest-tick' title='Designated Highest Qualification'><i class='fas fa-check mr-1'></i>Highest</span>" : "" %>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Stream / Specialization" ItemStyle-Width="160px">
                            <ItemTemplate>
                                <div class="font-weight-bold text-gray-900"><%# Eval("QUAL_SUB_NAME") %></div>
                                <div class="small text-muted"><%# Eval("SPECIALIZATION") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="YEAR_PASSED" HeaderText="Passing Year" ItemStyle-Width="85px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-Font-Bold="true" />

                        <asp:TemplateField HeaderText="Institute / University" ItemStyle-Width="200px">
                            <ItemTemplate>
                                <div class="font-weight-bold text-gray-900"><%# Eval("INSTITUTE_NAME") %></div>
                                <div class="small text-muted"><%# Eval("COLLEGE_NAME") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Performance" ItemStyle-Width="110px">
                            <ItemTemplate>
                                <div><strong>Score:</strong> <%# Eval("CGPA_MARKS") %></div>
                                <div class="small text-muted"><strong>Grade:</strong> <%# Eval("GRADE_DIVISION") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="105px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%# Eval("STATUS").ToString() == "Submitted" 
                                    ? "<span class='badge-status badge-submitted'><i class='fas fa-check-circle mr-1'></i> Submitted</span>" 
                                    : "<span class='badge-status badge-draft'><i class='fas fa-pencil-alt mr-1'></i> Draft</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="125px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <div class="d-inline-flex align-items-center">
                                    <asp:PlaceHolder ID="phActions" runat="server" Visible='<%# CanEditRecord(Eval("STATUS")) %>'>
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditRecord" CommandArgument='<%# Eval("EDU_ID") %>' CssClass="btn btn-sm btn-outline-primary mr-1" ToolTip="Edit Record">
                                            <i class="fas fa-edit"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteRecord" CommandArgument='<%# Eval("EDU_ID") %>' CssClass="btn btn-sm btn-outline-danger mr-1" ToolTip="Delete Record" OnClientClick="return confirm('Are you sure you want to delete this qualification record?');">
                                            <i class="fas fa-trash-alt"></i>
                                        </asp:LinkButton>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phAdminRevert" runat="server" Visible='<%# UserSession.IsAdmin && Eval("STATUS").ToString() == "Submitted" %>'>
                                        <asp:LinkButton ID="btnAdminRevert" runat="server" CommandName="AdminRevert" CommandArgument='<%# Eval("EDU_ID") %>' CssClass="btn btn-sm btn-outline-warning mr-1" ToolTip="Admin: Revert to Draft" OnClientClick="return confirm('Admin: Revert this record to Draft mode?');">
                                            <i class="fas fa-unlock"></i>
                                        </asp:LinkButton>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phAdminSubmit" runat="server" Visible='<%# UserSession.IsAdmin && Eval("STATUS").ToString() == "Draft" %>'>
                                        <asp:LinkButton ID="btnAdminSubmit" runat="server" CommandName="AdminSubmit" CommandArgument='<%# Eval("EDU_ID") %>' CssClass="btn btn-sm btn-outline-success mr-1" ToolTip="Admin: Mark as Submitted and lock" OnClientClick="return confirm('Admin: Lock this record as Submitted?');">
                                            <i class="fas fa-lock"></i>
                                        </asp:LinkButton>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phLockedView" runat="server" Visible='<%# IsLockedRecord(Eval("STATUS")) %>'>
                                        <span class="badge badge-light border text-muted px-2 py-1"><i class="fas fa-lock mr-1 text-secondary"></i> Locked</span>
                                    </asp:PlaceHolder>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle CssClass="text-center p-4 text-muted font-italic" />
                </asp:GridView>
            </div>
        </div>
        <div class="card-footer bg-light d-flex flex-wrap justify-content-between align-items-center py-3" style="padding-left: 1.25rem !important; padding-right: 1.25rem !important;">
            <div class="small text-muted mb-2 mb-md-0">
                <i class="fas fa-info-circle mr-1 text-primary"></i> Note: Mark exactly one entry as <strong>Highest Qualification</strong> representing your highest degree.
            </div>
            <asp:PlaceHolder ID="phBottomSubmitBtn" runat="server" Visible="false">
                <div class="text-right">
                    <asp:Button ID="btnBottomFinalSubmit" runat="server" Text="Submit Education Details" CssClass="btn btn-edu-success font-weight-bold shadow-sm btn-submit-with-caution" OnClick="btnFinalSubmit_Click" />
                    <div class="small text-danger font-weight-bold mt-1">
                        <i class="fas fa-exclamation-triangle mr-1"></i> Caution: Once submitted, records cannot be edited
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    </div> <!-- Close col-xl-9 col-lg-8 -->
    </div> <!-- Close row -->

    <!-- ADD / EDIT QUALIFICATION MODAL FORM -->
    <asp:Panel ID="pnlModal" runat="server" Visible="false">
        <div class="custom-modal-backdrop" tabindex="-1" role="dialog">
            <div class="custom-modal-dialog" role="document">
                <div class="custom-modal-content">
                    <div class="modal-header bg-light py-3 px-4" style="border-bottom: 1.5px solid #e2e8f0;">
                        <h5 class="modal-title font-weight-bold text-gray-900 mb-0 d-flex align-items-center">
                            <i class="fas fa-graduation-cap text-primary mr-2" style="font-size: 1.25rem;"></i>
                            <asp:Literal ID="litModalTitle" runat="server">Add Qualification Record</asp:Literal>
                        </h5>
                        <asp:LinkButton ID="btnCloseModalX" runat="server" CssClass="custom-modal-close" OnClick="btnCloseModal_Click" aria-label="Close" ToolTip="Close dialog">
                            &times;
                        </asp:LinkButton>
                    </div>

                    <div class="modal-body p-4" style="max-height: calc(85vh - 130px); overflow-y: auto;">
                        <asp:HiddenField ID="hfEditEduId" runat="server" Value="0" />

                        <asp:Panel ID="pnlFormError" runat="server" Visible="false" CssClass="alert alert-danger py-2 small mb-3">
                            <i class="fas fa-exclamation-triangle mr-1"></i>
                            <asp:Literal ID="litFormError" runat="server"></asp:Literal>
                        </asp:Panel>

                        <div class="row">
                            <!-- Main Qualification -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom">Main Qualification <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlMainQual" runat="server" CssClass="form-control form-control-custom searchable-select">
                                </asp:DropDownList>
                            </div>

                            <!-- Sub Qualification / Stream (Cascading) -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom">Sub Qualification / Stream <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlSubQual" runat="server" CssClass="form-control form-control-custom searchable-select">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row">
                            <!-- Passing Year -->
                            <div class="col-md-4 form-group">
                                <label class="form-label-custom">Passing Year <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlYearPassed" runat="server" CssClass="form-control form-control-custom searchable-select">
                                </asp:DropDownList>
                            </div>

                            <!-- Specialization / Branch -->
                            <div class="col-md-8 form-group">
                                <label class="form-label-custom">Specialization / Major Subject <span class="req">*</span></label>
                                <asp:TextBox ID="txtSpecialization" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Computer Science, Mechanical Engg, Physics, etc."></asp:TextBox>
                            </div>
                        </div>

                        <div class="row">
                            <!-- University / Board -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom">University / Board / Recognized Institute <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlUniversity" runat="server" CssClass="form-control form-control-custom searchable-select">
                                </asp:DropDownList>
                            </div>

                            <!-- College Name -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom">College / School Name <span class="req">*</span></label>
                                <asp:TextBox ID="txtCollegeName" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. University College of Engineering / High School"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row">
                            <!-- CGPA / Marks / Percentage -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom">CGPA / Percentage / Marks <span class="req">*</span></label>
                                <asp:TextBox ID="txtCgpaMarks" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. 8.45 CGPA or 78.50%"></asp:TextBox>
                            </div>

                            <!-- Grade / Division -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom">Grade / Division <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlGrade" runat="server" CssClass="form-control form-control-custom searchable-select">
                                    <asp:ListItem Value="">-- Select Grade / Division --</asp:ListItem>
                                    <asp:ListItem Value="First Class with Distinction">First Class with Distinction</asp:ListItem>
                                    <asp:ListItem Value="First Class">First Class</asp:ListItem>
                                    <asp:ListItem Value="Second Class">Second Class</asp:ListItem>
                                    <asp:ListItem Value="Pass Class">Pass Class</asp:ListItem>
                                    <asp:ListItem Value="Grade A+">Grade A+</asp:ListItem>
                                    <asp:ListItem Value="Grade A">Grade A</asp:ListItem>
                                    <asp:ListItem Value="Grade B+">Grade B+</asp:ListItem>
                                    <asp:ListItem Value="Grade B">Grade B</asp:ListItem>
                                    <asp:ListItem Value="Exemplary">Exemplary</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <!-- Highest Qualification Checkbox -->
                        <div class="p-3 rounded mb-3" style="background: #f8fafc; border: 1.5px solid #e2e8f0;">
                            <label class="d-flex align-items-center mb-0" style="cursor: pointer;" for="<%= chkIsHighest.ClientID %>">
                                <asp:CheckBox ID="chkIsHighest" runat="server" style="transform: scale(1.35); margin-right: 12px; cursor: pointer;" />
                                <div>
                                    <span class="font-weight-bold text-gray-900" style="font-size: 0.95rem;">
                                        <i class="fas fa-check-circle text-success mr-1"></i> Mark as Highest Qualification
                                    </span>
                                    <div class="small text-muted mt-1" style="line-height: 1.35;">
                                        Check this box if this qualification represents your highest achieved degree. (Any previously designated highest degree will automatically be updated).
                                    </div>
                                </div>
                            </label>
                        </div>

                        <!-- Remarks (Optional) -->
                        <div class="form-group mb-0">
                            <label class="form-label-custom">Remarks (Optional)</label>
                            <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control form-control-custom" placeholder="Any additional notes, roll numbers, or honors..."></asp:TextBox>
                        </div>
                    </div>

                    <div class="modal-footer bg-light py-3 px-4 d-flex justify-content-between align-items-center" style="border-top: 1.5px solid #e2e8f0;">
                        <div>
                            <asp:Button ID="btnCancelModal" runat="server" Text="Cancel" CssClass="btn btn-edu-outline px-3" OnClick="btnCloseModal_Click" />
                        </div>
                        <div class="d-flex align-items-center" style="gap: 10px;">
                            <span class="small text-muted mr-1 d-none d-md-inline">
                                <i class="fas fa-exclamation-circle text-warning mr-1"></i> Once submitted, cannot be edited
                            </span>
                            <asp:Button ID="btnSaveDraft" runat="server" Text="Save as Draft" CssClass="btn btn-secondary font-weight-bold px-3 shadow-sm" OnClick="btnSaveDraft_Click" ToolTip="Save this qualification as a draft to review, edit, or submit later" />
                            <asp:Button ID="btnSaveAndSubmit" runat="server" Text="Save &amp; Submit" CssClass="btn btn-edu-success font-weight-bold px-4 shadow-sm btn-submit-with-caution" OnClick="btnSaveAndSubmit_Click" ToolTip="Save and officially submit this qualification record" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            if ($('#<%= ddlMainQual.ClientID %>').length > 0) {
                EduApp.initCascadingDropdowns('<%= ddlMainQual.ClientID %>', '<%= ddlSubQual.ClientID %>');
                EduApp.makeSearchable('#<%= ddlUniversity.ClientID %>', '-- Search University / Board / Institute --');
                EduApp.makeSearchable('#<%= ddlYearPassed.ClientID %>', '-- Search Passing Year --');
                EduApp.makeSearchable('#<%= ddlGrade.ClientID %>', '-- Search Grade / Division --');
            }
        });
    </script>
</asp:Content>
