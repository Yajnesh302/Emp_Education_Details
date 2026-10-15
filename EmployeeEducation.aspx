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
            width: 95%;
            max-width: 1040px;
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

        /* Action Capsule Bar & Action Icon Buttons */
        .admin-actions-bar {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            padding: 3px 6px;
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            box-shadow: 0 1px 2px rgba(0, 0, 0, 0.03);
        }

        .btn-admin-act {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 32px;
            height: 32px;
            border-radius: 6px;
            font-size: 0.85rem;
            text-decoration: none !important;
            border: 1px solid transparent;
            transition: all 0.18s cubic-bezier(0.16, 1, 0.3, 1);
            cursor: pointer;
        }

        /* Edit action button: Soft Sky Blue */
        .btn-admin-act.act-edit {
            background-color: #f0f9ff;
            color: #0284c7;
            border-color: #bae6fd;
        }
        .btn-admin-act.act-edit:hover {
            background-color: #0284c7;
            color: #ffffff !important;
            border-color: #0284c7;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(2, 132, 199, 0.28);
        }

        /* Revert action button: Soft Warning Amber */
        .btn-admin-act.act-revert {
            background-color: #fffbeb;
            color: #d97706;
            border-color: #fde68a;
        }
        .btn-admin-act.act-revert:hover {
            background-color: #d97706;
            color: #ffffff !important;
            border-color: #d97706;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(217, 119, 6, 0.28);
        }

        /* Submit action button: Soft Success Emerald */
        .btn-admin-act.act-submit {
            background-color: #ecfdf5;
            color: #059669;
            border-color: #a7f3d0;
        }
        .btn-admin-act.act-submit:hover {
            background-color: #059669;
            color: #ffffff !important;
            border-color: #059669;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(5, 150, 105, 0.28);
        }

        /* Delete action button: Soft Danger Crimson */
        .btn-admin-act.act-delete {
            background-color: #fef2f2;
            color: #dc2626;
            border-color: #fecaca;
        }
        .btn-admin-act.act-delete:hover {
            background-color: #dc2626;
            color: #ffffff !important;
            border-color: #dc2626;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(220, 38, 38, 0.28);
        }

        /* Locked Status Badge */
        .badge-locked {
            background-color: #f1f5f9;
            color: #64748b;
            border: 1px solid #cbd5e1;
            padding: 5px 12px;
            border-radius: 20px;
            font-size: 0.78rem;
            font-weight: 700;
            display: inline-flex;
            align-items: center;
            letter-spacing: 0.3px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Built-in AJAX ScriptManager for Partial Postbacks (Zero Full Page Refresh) -->
    <asp:ScriptManager ID="smEducation" runat="server" EnablePartialRendering="true" />



    <!-- Page Header & Action Bar -->
    <div class="d-flex flex-wrap justify-content-between align-items-center mb-4">
        <div>
            <h3 class="font-weight-bold text-gray-900 mb-1">
                <i class="fas fa-user-graduate text-primary mr-2"></i> My Education &amp; Qualification Details
            </h3>
            <p class="text-muted small mb-0">Record, review, and maintain your official educational qualifications</p>
        </div>
    </div>

    <div class="row">
        <!-- LEFT SIDE: Employee Information & Photo Card -->
        <div class="col-xl-3 col-lg-4 col-md-5 col-12 mb-4 emp-profile-sidebar-col">
            <div class="emp-side-card text-center">
                <!-- Decorative Top Banner -->
                <div class="emp-side-banner">
                    <span class="emp-side-banner-tag"><i class="fas fa-shield-alt mr-1"></i> LRDE</span>
                </div>

                <!-- Profile Photo Frame -->
                <div class="emp-side-avatar-wrapper">
                    <div class="emp-side-avatar-frame">
                        <asp:Image ID="imgEmpPhoto" runat="server" AlternateText="Employee Photo" />
                    </div>
                </div>

                <!-- Employee Details -->
                <div class="emp-side-body">
                    <h5 class="emp-name-title">
                        <asp:Literal ID="litEmpName" runat="server"></asp:Literal>
                    </h5>

                    <div>
                        <span class="emp-pcno-pill">
                            <i class="fas fa-id-badge mr-1"></i> PCNO: <asp:Literal ID="litEmpPCNO" runat="server"></asp:Literal>
                        </span>
                    </div>

                    <div class="emp-designation-text">
                        <asp:Literal ID="litEmpDesignation" runat="server">---</asp:Literal>
                    </div>

                    <div class="emp-division-text">
                        <i class="fas fa-building text-muted mr-1"></i>
                        <asp:Literal ID="litEmpDivision" runat="server">---</asp:Literal>
                    </div>

                    <hr class="emp-side-divider" />

                    <asp:UpdatePanel ID="upSidebarStats" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <!-- Current Submission Status -->
                            <div class="mb-3">
                                <div class="emp-section-subtitle">
                                    <i class="fas fa-check-circle mr-1 text-primary"></i> Profile Status
                                </div>
                                <asp:Literal ID="litSubmissionStatusPill" runat="server"></asp:Literal>
                            </div>

                            <!-- Quick Academic Stats 2-Tile Grid -->
                            <div class="emp-metrics-grid">
                                <div class="emp-metric-tile">
                                    <i class="fas fa-graduation-cap metric-tile-icon"></i>
                                    <div class="metric-tile-number">
                                        <asp:Literal ID="litStatQualCount" runat="server">0</asp:Literal>
                                    </div>
                                    <div class="metric-tile-caption">Qualifications</div>
                                </div>
                                <div class="emp-metric-tile" title='<%= HighestQualTitle %>'>
                                    <i class="fas fa-award metric-tile-icon"></i>
                                    <div class="metric-tile-text">
                                        <asp:Literal ID="litStatHighestQual" runat="server">None</asp:Literal>
                                    </div>
                                    <div class="metric-tile-caption">Highest Degree</div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <!-- Add Qualification Shortcut Button -->
                    <div>
                        <asp:Button ID="btnSidebarAdd" runat="server" Text="+ Add Qualification" CssClass="btn btn-sidebar-action font-weight-bold" OnClientClick="EduApp.openAddModal(); return false;" OnClick="btnOpenAddModal_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- RIGHT SIDE: Main Qualifications Table & Actions -->
        <div class="col-xl-9 col-lg-8 col-md-7 col-12 emp-main-content-col">
            <asp:UpdatePanel ID="upEducation" runat="server" UpdateMode="Always">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSidebarAdd" EventName="Click" />
                </Triggers>
                <ContentTemplate>
            <!-- Tracking Hidden Field for Qualification Modal State -->
            <asp:HiddenField ID="hfModalOpen" runat="server" Value="0" />

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
                <div class="alert alert-warning d-flex align-items-center mb-4 py-3 px-4 shadow-sm" style="border-radius: 12px; border-left: 5px solid #f59e0b; background: #fffbeb;">
                    <i class="fas fa-pencil-alt fa-2x text-warning mr-3 flex-shrink-0"></i>
                    <div>
                        <div class="font-weight-bold text-gray-900" style="font-size: 1rem;">Unsubmitted Records in Draft Mode</div>
                        <div class="small text-muted mt-1">
                            You have qualification records saved as draft. Review all details, designate your <strong>Highest Qualification</strong>, and click <strong>"Submit Education Details"</strong> in the table header to officially lock and submit your profile.
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <!-- Incomplete Legacy Draft Records Notice Banner -->
            <asp:PlaceHolder ID="phDraftIncompleteNotice" runat="server" Visible="false">
                <div class="alert alert-warning d-flex align-items-center mb-4 py-3 px-4 shadow-sm" style="border-radius: 12px; border-left: 5px solid #f59e0b; background: #fffdf5; border-color: #fef3c7;">
                    <div class="mr-3 flex-shrink-0" style="width: 44px; height: 44px; border-radius: 10px; background: #fef3c7; display: flex; align-items: center; justify-content: center;">
                        <i class="fas fa-file-signature text-warning" style="font-size: 1.25rem;"></i>
                    </div>
                    <div class="flex-grow-1">
                        <div class="d-flex align-items-center flex-wrap" style="gap: 8px;">
                            <span class="font-weight-bold text-gray-900" style="font-size: 1rem;">Imported Qualifications - Verification Required</span>
                            <span class="badge badge-warning font-weight-bold px-2 py-0.5" style="background: #fde68a; color: #92400e; font-size: 0.75rem; border-radius: 6px;">
                                <asp:Literal ID="litDraftIncompleteCount" runat="server">0</asp:Literal> record(s) pending details
                            </span>
                        </div>
                        <div class="small text-muted mt-1" style="line-height: 1.45;">
                            Your qualification records were synced from the previous database. Some entries have missing details (such as College, Stream, or Year). Please click <strong><i class="fas fa-edit text-primary"></i> Edit</strong> on each record tagged with <em>"Details Pending"</em> below to verify and complete your details before final submission.
                        </div>
                        <asp:PlaceHolder ID="phDraftDetailsHidden" runat="server" Visible="false">
                            <asp:Literal ID="litDraftIncompleteDetails" runat="server"></asp:Literal>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </asp:PlaceHolder>

            <!-- Education Records Table Card -->
            <div class="edu-card shadow-sm mb-4">
        <div class="edu-card-header d-flex flex-wrap justify-content-between align-items-center px-4 py-3" style="padding-left: 1.25rem !important; padding-right: 1.25rem !important; gap: 12px;">
            <div class="d-flex align-items-center flex-wrap" style="gap: 10px;">
                <div class="edu-card-title mb-0">
                    <i class="fas fa-graduation-cap text-primary mr-2"></i> Recorded Qualifications
                </div>
                <span class="badge badge-light border text-muted px-2 py-1 font-weight-bold" style="font-size: 0.8rem; border-radius: 6px;">
                    <asp:Literal ID="litCardQualCount" runat="server">0</asp:Literal> Records
                </span>
            </div>

            <!-- Unified Action Area (Single Clean Action Point) -->
            <div class="d-flex align-items-center flex-wrap" style="gap: 10px;">
                <asp:Button ID="btnOpenAddModal" runat="server" Text="+ Add Qualification" CssClass="btn btn-edu-primary font-weight-bold px-3 py-2 shadow-xs" OnClientClick="EduApp.openAddModal(); return false;" OnClick="btnOpenAddModal_Click" />
                <asp:PlaceHolder ID="phBottomSubmitBtn" runat="server" Visible="false">
                    <div class="d-inline-flex align-items-center" style="gap: 8px;">
                        <asp:Button ID="btnBottomFinalSubmit" runat="server" Text="Submit Education Details" CssClass="btn btn-edu-success font-weight-bold shadow-sm px-3 py-2 btn-submit-with-caution" OnClick="btnFinalSubmit_Click" />
                        <span class="edu-lock-indicator" title="Once submitted, records are permanently locked and cannot be edited">
                            <i class="fas fa-lock"></i> Locked on submit
                        </span>
                    </div>
                </asp:PlaceHolder>
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
                                <div class="font-weight-bold text-gray-900"><%# RenderFieldValue(Eval("QUAL_SUB_NAME"), "Stream / Branch") %></div>
                                <div class="small text-muted"><%# RenderFieldValue(Eval("SPECIALIZATION"), "Specialization") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Passing Year" ItemStyle-Width="85px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%# RenderYearValue(Eval("YEAR_PASSED")) %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Institute / University" ItemStyle-Width="200px">
                            <ItemTemplate>
                                <div class="font-weight-bold text-gray-900"><%# RenderFieldValue(Eval("INSTITUTE_NAME"), "University / Institute") %></div>
                                <div class="small text-muted"><%# RenderFieldValue(Eval("COLLEGE_NAME"), "College / School") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Performance" ItemStyle-Width="125px">
                            <ItemTemplate>
                                <div><span class="text-muted small">Score: </span><%# RenderScoreDisplay(Eval("CGPA_MARKS")) %></div>
                                <div class="small text-muted"><span class="text-muted small">Class: </span><%# RenderFieldValue(Eval("GRADE_DIVISION"), "Grade") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="125px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%# RenderStatusBadge(Eval("STATUS"), Eval("QUAL_MAIN_CODE"), Eval("QUAL_SUB_CODE"), Eval("YEAR_PASSED"), Eval("INSTITUTE_NAME"), Eval("COLLEGE_NAME"), Eval("SPECIALIZATION"), Eval("CGPA_MARKS"), Eval("GRADE_DIVISION")) %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="120px" ItemStyle-Width="120px"
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="phActions" runat="server" Visible='<%# CanEditRecord(Eval("STATUS")) %>'>
                                    <div class="admin-actions-bar">
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditRecord" CommandArgument='<%# Eval("EDU_ID") %>'
                                            CssClass="btn-admin-act act-edit" ToolTip="Edit Qualification Record">
                                            <i class="fas fa-pencil-alt"></i>
                                        </asp:LinkButton>
                                        <asp:PlaceHolder ID="phAdminRevert" runat="server" Visible='<%# UserSession.IsAdmin && Eval("STATUS").ToString() == "Submitted" %>'>
                                            <asp:LinkButton ID="btnAdminRevert" runat="server" CommandName="AdminRevert" CommandArgument='<%# Eval("EDU_ID") %>'
                                                CssClass="btn-admin-act act-revert" ToolTip="Admin: Revert to Draft (Unlock)"
                                                data-confirm="Revert this record to Draft mode? The employee will be able to edit and re-submit it."
                                                data-confirm-title="Revert Record to Draft"
                                                data-confirm-type="warning"
                                                data-confirm-btn="Yes, Revert to Draft">
                                                <i class="fas fa-unlock-alt"></i>
                                            </asp:LinkButton>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phAdminSubmit" runat="server" Visible='<%# UserSession.IsAdmin && Eval("STATUS").ToString() == "Draft" %>'>
                                            <asp:LinkButton ID="btnAdminSubmit" runat="server" CommandName="AdminSubmit" CommandArgument='<%# Eval("EDU_ID") %>'
                                                CssClass="btn-admin-act act-submit" ToolTip="Admin: Mark as Submitted and lock"
                                                data-confirm="Lock this record as Submitted? The record will become an official locked entry."
                                                data-confirm-title="Lock Record as Submitted"
                                                data-confirm-type="success"
                                                data-confirm-btn="Yes, Lock Record">
                                                <i class="fas fa-lock"></i>
                                            </asp:LinkButton>
                                        </asp:PlaceHolder>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteRecord" CommandArgument='<%# Eval("EDU_ID") %>'
                                            CssClass="btn-admin-act act-delete" ToolTip="Permanently Delete Qualification Record"
                                            data-confirm='<%# "Are you sure you want to permanently delete this qualification record (" + Eval("QUAL_MAIN_NAME") + ")? This action cannot be undone." %>'
                                            data-confirm-title="Delete Qualification Record"
                                            data-confirm-type="danger"
                                            data-confirm-btn="Yes, Delete Record">
                                            <i class="fas fa-trash-alt"></i>
                                        </asp:LinkButton>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phLockedView" runat="server" Visible='<%# IsLockedRecord(Eval("STATUS")) %>'>
                                    <span class="badge-status badge-locked" title="Officially submitted qualification records are locked from editing">
                                        <i class="fas fa-lock mr-1 text-muted"></i> Locked
                                    </span>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle CssClass="text-center p-4 text-muted font-italic" />
                </asp:GridView>
            </div>
        </div>
        <div class="card-footer bg-light d-flex flex-wrap justify-content-between align-items-center py-3" style="padding-left: 1.25rem !important; padding-right: 1.25rem !important;">
            <div class="small text-muted mb-0">
                <i class="fas fa-info-circle mr-1 text-primary"></i> Note: Mark exactly one entry as <strong>Highest Qualification</strong> representing your highest degree.
            </div>
        </div>
    </div>

    <!-- ADD / EDIT QUALIFICATION MODAL FORM -->
    <asp:Panel ID="pnlModal" runat="server" CssClass="custom-modal-backdrop" style="display: none;" tabindex="-1" role="dialog">
            <div class="custom-modal-dialog" role="document">
                <div class="custom-modal-content">
                    <div class="modal-header bg-light py-3 px-4" style="border-bottom: 1.5px solid #e2e8f0;">
                        <h5 class="modal-title font-weight-bold text-gray-900 mb-0 d-flex align-items-center">
                            <i class="fas fa-graduation-cap text-primary mr-2" style="font-size: 1.25rem;"></i>
                            <span id="lblModalTitleText"><asp:Literal ID="litModalTitle" runat="server">Add Qualification Record</asp:Literal></span>
                        </h5>
                        <asp:LinkButton ID="btnCloseModalX" runat="server" CssClass="custom-modal-close" OnClientClick="EduApp.closeModal(); return false;" OnClick="btnCloseModal_Click" aria-label="Close" ToolTip="Close dialog">
                            &times;
                        </asp:LinkButton>
                    </div>

                    <div class="modal-body p-4" style="max-height: calc(85vh - 130px); overflow-y: auto;">
                        <asp:HiddenField ID="hfEditEduId" runat="server" Value="0" />
                        <asp:HiddenField ID="hfSelectedMainQual" runat="server" />
                        <asp:HiddenField ID="hfSelectedSubQual" runat="server" />
                        <asp:HiddenField ID="hfSelectedSubQualName" runat="server" />

                        <!-- Client-side instant error banner (0 page reload, immediate focus) -->
                        <div id="modalClientErrorBanner" class="alert alert-danger py-2 small mb-3" style="display: none;">
                            <i class="fas fa-exclamation-triangle mr-1"></i>
                            <span id="modalClientErrorText"></span>
                        </div>

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
                                <asp:TextBox ID="txtSpecialization" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Computer Science, Mechanical Engg, or N/A"></asp:TextBox>
                                <small class="form-text text-muted mt-1"><i class="fas fa-info-circle mr-1 text-primary"></i> Enter branch/major subject, or <strong>N/A</strong> for general/school qualifications.</small>
                            </div>
                        </div>

                        <div class="row">
                            <!-- University / Board -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom" id="lblUniversity">University / Board / Recognized Institute <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlUniversity" runat="server" CssClass="form-control form-control-custom searchable-select">
                                </asp:DropDownList>
                                <small id="helpUniversityCert" class="form-text text-muted mt-1" style="display: none;">
                                    <i class="fas fa-certificate mr-1 text-primary"></i> For certificates &amp; vocational courses, select the issuing authority (e.g. NCVT, DRDO/LRDE, C-DAC, State Tech Board) or 'Other Certifying Body'.
                                </small>
                            </div>

                            <!-- College Name -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom" id="lblCollege">College / School Name <span class="req">*</span></label>
                                <asp:DropDownList ID="ddlCollege" runat="server" CssClass="form-control form-control-custom searchable-select">
                                </asp:DropDownList>
                                <div class="custom-control custom-checkbox mt-1">
                                    <input type="checkbox" class="custom-control-input" id="chkSameAsUniv" />
                                    <label class="custom-control-label small text-muted font-weight-normal cursor-pointer" for="chkSameAsUniv" id="lblSameAsUniv">
                                        <i class="fas fa-university mr-1 text-primary"></i> Studied directly at University Campus / Department (Same as University)
                                    </label>
                                </div>
                                <asp:Panel ID="pnlCustomCollege" runat="server" CssClass="mt-2" style="display: none;">
                                    <label class="form-label-custom small text-muted mb-1"><i class="fas fa-pen mr-1"></i> Specify College / School Name: <span class="req">*</span></label>
                                    <asp:TextBox ID="txtCustomCollege" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Mangalore Institute of Technology and Engineering (MITE)"></asp:TextBox>
                                </asp:Panel>
                            </div>
                        </div>

                        <div class="row">
                            <!-- Score Type & CGPA / Percentage / Marks -->
                            <div class="col-md-6 form-group">
                                <label class="form-label-custom">Score Type &amp; Value <span class="req">*</span></label>
                                <div class="input-group">
                                    <div class="input-group-prepend">
                                        <asp:DropDownList ID="ddlScoreType" runat="server" CssClass="form-control form-control-custom score-type-select" style="width: 145px; height: 42px; padding: 0.35rem 0.65rem; line-height: normal; border-top-right-radius: 0; border-bottom-right-radius: 0; background-color: #f8fafc; font-weight: 600; color: #1e293b;">
                                            <asp:ListItem Value="Percentage" Selected="True">Percentage (%)</asp:ListItem>
                                            <asp:ListItem Value="CGPA">CGPA (10 pt)</asp:ListItem>
                                            <asp:ListItem Value="Marks">Marks / Total</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <asp:TextBox ID="txtCgpaMarks" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. 78.50%" style="height: 42px; border-top-left-radius: 0; border-bottom-left-radius: 0;"></asp:TextBox>
                                </div>
                                <small class="form-text text-muted mt-1" id="scoreHelpText">
                                    <i class="fas fa-info-circle mr-1 text-primary"></i> Legacy records may be CGPA or % (e.g. <strong>78.5%</strong> or <strong>8.45 CGPA</strong>).
                                </small>
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
                            <asp:Button ID="btnCancelModal" runat="server" Text="Cancel" CssClass="btn btn-edu-outline px-3" OnClientClick="EduApp.closeModal(); return false;" OnClick="btnCloseModal_Click" />
                        </div>
                        <div class="d-flex align-items-center" style="gap: 10px;">
                            <span class="small text-muted mr-1 d-none d-md-inline">
                                <i class="fas fa-exclamation-circle text-warning mr-1"></i> Once submitted, cannot be edited
                            </span>
                            <asp:Button ID="btnSaveDraft" runat="server" Text="Save as Draft" CssClass="btn btn-secondary font-weight-bold px-3 shadow-sm btn-save-draft" OnClientClick="return EduApp.validateModalForm(false);" OnClick="btnSaveDraft_Click" ToolTip="Save this qualification as a draft to review, edit, or submit later" />
                            <asp:Button ID="btnSaveAndSubmit" runat="server" Text="Save &amp; Submit" CssClass="btn btn-edu-success font-weight-bold px-4 shadow-sm btn-submit-with-caution" OnClientClick="return EduApp.validateModalForm(true);" OnClick="btnSaveAndSubmit_Click" ToolTip="Save and officially submit this qualification record" />
                        </div>
                    </div>
                </div>
            </div>
    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div> <!-- Close col-xl-9 col-lg-8 -->
    </div> <!-- Close row -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <!-- Embedded Full Sub Qualifications Catalog (Ensures 100% Zero-Loss on Postbacks) -->
    <script id="subQualDataScript">
        window.EDU_ALL_SUB_QUALS = <%= GetSubQualsJson() %>;
    </script>
    <script>
        function initEducationUI() {
            if ($('#<%= ddlMainQual.ClientID %>').length > 0) {
                EduApp.initCascadingDropdowns('<%= ddlMainQual.ClientID %>', '<%= ddlSubQual.ClientID %>');
                EduApp.makeSearchable('#<%= ddlUniversity.ClientID %>', '-- Search University / Board / Institute --');
                EduApp.makeSearchable('#<%= ddlCollege.ClientID %>', '-- Search College / School Name --');
                EduApp.makeSearchable('#<%= ddlYearPassed.ClientID %>', '-- Search Passing Year --');
                EduApp.makeSearchable('#<%= ddlGrade.ClientID %>', '-- Search Grade / Division --');

                function checkCollegeCustomInput() {
                    var val = $('#<%= ddlCollege.ClientID %>').val();
                    if (val === 'OTHER' || (val && val.indexOf('Other') === 0)) {
                        $('#<%= pnlCustomCollege.ClientID %>').slideDown(150);
                        $('#<%= txtCustomCollege.ClientID %>').focus();
                    } else {
                        $('#<%= pnlCustomCollege.ClientID %>').slideUp(150);
                    }
                }

                function syncSameAsUnivCheckbox() {
                    var val = $('#<%= ddlCollege.ClientID %>').val();
                    var isSame = (val === 'University Campus / Department' || val === 'Direct Certification / Same as Issuing Authority');
                    $('#chkSameAsUniv').prop('checked', isSame);
                }

                $('#chkSameAsUniv').off('change').on('change', function () {
                    var isCert = EduApp.isCertificateQualification($('#<%= ddlMainQual.ClientID %>').val());
                    var targetVal = isCert ? 'Direct Certification / Same as Issuing Authority' : 'University Campus / Department';
                    if ($(this).is(':checked')) {
                        $('#<%= ddlCollege.ClientID %>').val(targetVal).trigger('change');
                    } else {
                        var currVal = $('#<%= ddlCollege.ClientID %>').val();
                        if (currVal === targetVal || currVal === 'University Campus / Department' || currVal === 'Direct Certification / Same as Issuing Authority') {
                            $('#<%= ddlCollege.ClientID %>').val('').trigger('change');
                        }
                    }
                });

                $('#<%= ddlCollege.ClientID %>').off('change.custom').on('change.custom', function () {
                    checkCollegeCustomInput();
                    syncSameAsUnivCheckbox();
                });

                // Initial check in case of existing selection
                checkCollegeCustomInput();
                syncSameAsUnivCheckbox();
                EduApp.adaptFieldsForQualification($('#<%= ddlMainQual.ClientID %>').val());

                // Dynamic placeholder for Score Type (Percentage / CGPA / Marks)
                $('#<%= ddlScoreType.ClientID %>').off('change.score').on('change.score', function () {
                    var sType = $(this).val();
                    var $input = $('#<%= txtCgpaMarks.ClientID %>');
                    if (sType === 'Percentage') {
                        $input.attr('placeholder', 'e.g. 78.50%');
                    } else if (sType === 'CGPA') {
                        $input.attr('placeholder', 'e.g. 8.45');
                    } else {
                        $input.attr('placeholder', 'e.g. 850 / 1000');
                    }
                });
            }
        }

        $(document).ready(function () {
            initEducationUI();
        });

        // Re-initialize searchable dropdowns and UI seamlessly after every partial AJAX postback
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                initEducationUI();
                EduApp.initIntranetPhotos();

                // If modal tracking field indicates closed (0), ensure modal backdrop is strictly hidden
                var modalState = $('input[id$="hfModalOpen"]').val();
                if (modalState === '0') {
                    $('[id$="pnlModal"]').css('display', 'none');
                }
            });
        }
    </script>
</asp:Content>
