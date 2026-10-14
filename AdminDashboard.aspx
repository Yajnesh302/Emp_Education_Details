<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdminDashboard.aspx.cs" Inherits="EducationApp.AdminDashboard" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .filter-panel {
                background: #ffffff;
                border: 1px solid var(--border-color);
                border-radius: 12px;
                padding: 1.25rem;
                margin-bottom: 1.5rem;
                box-shadow: var(--shadow-sm);
            }

            .custom-modal-dialog {
                width: 95%;
                max-width: 1040px;
                margin: auto;
            }

            .edu-grid-pager {
                background: #f8fafc;
                border-top: 1px solid var(--border-color);
                padding: 0.85rem 1.5rem;
            }

            .edu-grid-pager table {
                margin: 0 auto;
                border-collapse: separate;
                border-spacing: 5px;
            }

            .edu-grid-pager td {
                padding: 0;
                border: none;
            }

            .edu-grid-pager a,
            .edu-grid-pager span {
                display: inline-flex;
                align-items: center;
                justify-content: center;
                min-width: 36px;
                height: 36px;
                padding: 0 12px;
                border-radius: 6px;
                font-size: 0.875rem;
                font-weight: 600;
                text-decoration: none;
                transition: all 0.15s ease-in-out;
            }

            .edu-grid-pager a {
                background-color: #ffffff;
                color: #334155;
                border: 1px solid #cbd5e1;
            }

            .edu-grid-pager a:hover {
                background-color: #f1f5f9;
                color: #0284c7;
                border-color: #94a3b8;
                transform: translateY(-1px);
            }

            .edu-grid-pager span {
                background-color: #0284c7;
                color: #ffffff;
                border: 1px solid #0284c7;
                box-shadow: 0 2px 4px rgba(2, 132, 199, 0.25);
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <!-- Built-in AJAX ScriptManager for Partial Postbacks (Zero Full Page Refresh) -->
        <asp:ScriptManager ID="smAdmin" runat="server" EnablePartialRendering="true" />

        <!-- Page Title -->
        <div class="d-flex flex-wrap justify-content-between align-items-center mb-4">
            <div>
                <h3 class="font-weight-bold text-gray-900 mb-1">
                    <i class="fas fa-chart-line text-primary mr-2"></i> Employee Education Administration
                </h3>
                <p class="text-muted small mb-0">Overview of employee academic profiles, record statuses, and unlock
                    management</p>
            </div>
            <div class="mt-2 mt-md-0">
                <asp:Button ID="btnExportCsv" runat="server" Text="Export to Excel (CSV)"
                    CssClass="btn btn-edu-outline font-weight-bold mr-2" OnClick="btnExportCsv_Click" />
                <a href="AdminMasterData.aspx" class="btn btn-edu-primary font-weight-bold">
                    <i class="fas fa-layer-group mr-1"></i> Master Catalog
                </a>
            </div>
        </div>

        <!-- Asynchronous UpdatePanel to Ensure Zero Full Page Reloads on Filters, Actions, and Modals -->
        <asp:UpdatePanel ID="upAdminDashboard" runat="server" UpdateMode="Always">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnExportCsv" />
            </Triggers>
            <ContentTemplate>
                <!-- Tracking Hidden Field for Edit Modal State -->
                <asp:HiddenField ID="hfModalOpen" runat="server" Value="0" />

                <!-- KPI Metric Cards -->
                <div class="row mb-4">
                    <div class="col-xl-3 col-md-6 mb-3">
                        <div class="stat-card">
                            <div class="stat-icon" style="background: #e0f2fe; color: #0284c7;">
                                <i class="fas fa-users"></i>
                            </div>
                            <div class="stat-content">
                                <p>Total Employees</p>
                                <h3>
                                    <asp:Literal ID="litStatEmployees" runat="server">0</asp:Literal>
                                </h3>
                            </div>
                        </div>
                    </div>

                    <div class="col-xl-3 col-md-6 mb-3">
                        <div class="stat-card">
                            <div class="stat-icon" style="background: #dcfce7; color: #16a34a;">
                                <i class="fas fa-check-double"></i>
                            </div>
                            <div class="stat-content">
                                <p>Submitted Records</p>
                                <h3>
                                    <asp:Literal ID="litStatSubmitted" runat="server">0</asp:Literal>
                                </h3>
                            </div>
                        </div>
                    </div>

                    <div class="col-xl-3 col-md-6 mb-3">
                        <div class="stat-card">
                            <div class="stat-icon" style="background: #fef3c7; color: #d97706;">
                                <i class="fas fa-file-signature"></i>
                            </div>
                            <div class="stat-content">
                                <p>Draft (In Progress)</p>
                                <h3>
                                    <asp:Literal ID="litStatDraft" runat="server">0</asp:Literal>
                                </h3>
                            </div>
                        </div>
                    </div>

                    <div class="col-xl-3 col-md-6 mb-3">
                        <div class="stat-card">
                            <div class="stat-icon" style="background: #ede9fe; color: #7c3aed;">
                                <i class="fas fa-sync-alt"></i>
                            </div>
                            <div class="stat-content">
                                <p>Historical Records Imported</p>
                                <h3>
                                    <asp:Literal ID="litStatMigrated" runat="server">0</asp:Literal>
                                </h3>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Filter & Search Bar with Live Search as you type -->
                <div class="filter-panel">
                    <div class="row align-items-end">
                        <div class="col-lg-4 col-md-6 form-group mb-md-0">
                            <label class="form-label-custom">Search Records</label>
                            <div class="position-relative">
                                <i class="fas fa-search text-muted position-absolute" style="left: 14px; top: 50%; transform: translateY(-50%); z-index: 4; font-size: 0.9rem;"></i>
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control form-control-custom"
                                    style="padding-left: 2.4rem !important; padding-right: 2.4rem !important;"
                                    placeholder="Type PCNO, Name, University, Branch..." autocomplete="off"></asp:TextBox>
                                <span id="btnLiveSearchClear" class="position-absolute text-muted" 
                                    style="right: 14px; top: 50%; transform: translateY(-50%); z-index: 4; display: none; font-size: 1.25rem; line-height: 1; cursor: pointer;" 
                                    title="Clear search">&times;</span>
                                <span id="liveSearchSpinner" class="position-absolute text-primary" 
                                    style="right: 14px; top: 50%; transform: translateY(-50%); z-index: 4; display: none; font-size: 0.9rem;">
                                    <i class="fas fa-circle-notch fa-spin"></i>
                                </span>
                            </div>
                        </div>

                        <div class="col-lg-2 col-md-3 form-group mb-md-0">
                            <label class="form-label-custom">Status</label>
                            <asp:DropDownList ID="ddlStatusFilter" runat="server"
                                CssClass="form-control form-control-custom searchable-select">
                                <asp:ListItem Value="ALL">All Statuses</asp:ListItem>
                                <asp:ListItem Value="Submitted">Submitted (Locked)</asp:ListItem>
                                <asp:ListItem Value="Draft">Draft (Editable)</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-lg-3 col-md-3 form-group mb-md-0">
                            <label class="form-label-custom">Main Qualification</label>
                            <asp:DropDownList ID="ddlQualFilter" runat="server"
                                CssClass="form-control form-control-custom searchable-select">
                            </asp:DropDownList>
                        </div>

                        <div class="col-lg-2 col-md-6 form-group mb-md-0">
                            <label class="form-label-custom">Highest Qualification</label>
                            <asp:DropDownList ID="ddlHighestFilter" runat="server"
                                CssClass="form-control form-control-custom searchable-select">
                                <asp:ListItem Value="ALL">All Records</asp:ListItem>
                                <asp:ListItem Value="1">Highest Only</asp:ListItem>
                                <asp:ListItem Value="0">Non-Highest</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-lg-1 col-md-6 form-group mb-md-0 d-flex align-items-end">
                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset"
                                CssClass="btn btn-outline-secondary btn-block font-weight-bold" OnClick="btnResetFilter_Click" 
                                ToolTip="Clear search and reset all filters" />
                            <!-- Hidden Filter Trigger Button (Fired automatically on live typing & dropdown change) -->
                            <asp:Button ID="btnFilter" runat="server" Text="Filter" style="display:none;"
                                OnClick="btnFilter_Click" />
                        </div>
                    </div>
                </div>
                <!-- Employee Education Master Table -->
                <div class="edu-card shadow-sm mb-4">
                    <div class="edu-card-header d-flex flex-wrap justify-content-between align-items-center px-4 py-3"
                        style="padding-left: 2rem !important; padding-right: 2rem !important; gap: 12px;">
                        <div class="edu-card-title mb-0 d-flex align-items-center flex-wrap" style="gap: 10px;">
                            <div>
                                <i class="fas fa-table text-primary mr-2"></i> All Employee Education Records (<asp:Literal
                                    ID="litFilteredCount" runat="server">0</asp:Literal>)
                            </div>
                            <span class="badge badge-light border text-muted px-2 py-1 font-weight-normal" style="font-size: 0.85rem;">
                                <asp:Literal ID="litPagingSummary" runat="server">Showing 0 of 0</asp:Literal>
                            </span>
                        </div>
                        <div class="d-flex align-items-center" style="gap: 8px;">
                            <span class="small text-muted font-weight-bold">Rows:</span>
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-control form-control-sm" style="width: auto; display: inline-block; font-weight: 600;">
                                <asp:ListItem Value="25" Selected="True">25 / page</asp:ListItem>
                                <asp:ListItem Value="50">50 / page</asp:ListItem>
                                <asp:ListItem Value="100">100 / page</asp:ListItem>
                                <asp:ListItem Value="200">200 / page</asp:ListItem>
                                <asp:ListItem Value="999999">Show All</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="edu-card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="gvAdminEducation" runat="server" AutoGenerateColumns="false"
                                CssClass="edu-table" GridLines="None" UseAccessibleHeader="true"
                                AllowPaging="true" PageSize="25"
                                OnPageIndexChanging="gvAdminEducation_PageIndexChanging"
                                OnRowCommand="gvAdminEducation_RowCommand"
                                EmptyDataText="No records found matching current search criteria.">
                                <PagerSettings Mode="NumericFirstLast" FirstPageText="&laquo; First" LastPageText="Last &raquo;" NextPageText="Next &rsaquo;" PreviousPageText="&lsaquo; Prev" Position="Bottom" PageButtonCount="7" />
                                <PagerStyle CssClass="edu-grid-pager" HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField HeaderText="PCNO" ItemStyle-Width="90px">
                                        <ItemTemplate>
                                            <span
                                                class="badge badge-light border text-dark font-weight-bold px-2 py-1 shadow-xs d-inline-flex align-items-center"
                                                style="font-size: 0.82rem; gap: 4px;">
                                                <i class="fas fa-id-badge text-muted"></i>
                                                <span>
                                                    <%# Eval("PCNO") %>
                                                </span>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Employee Name & Dept">
                                        <ItemTemplate>
                                            <div class="font-weight-bold text-gray-900">
                                                <%# Eval("EMP_NAME") %>
                                            </div>
                                            <div class="small text-muted">
                                                <%# Eval("DESIGNATION") %> &bull; <%# Eval("DIVISION_NAME") %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Qualification" ItemStyle-Width="240px">
                                        <ItemTemplate>
                                            <div class="font-weight-bold text-gray-900">
                                                <%# Eval("QUAL_MAIN_NAME") %>
                                            </div>
                                            <div class="mt-1 d-inline-flex flex-wrap align-items-center"
                                                style="gap: 6px;">
                                                <span class="badge badge-light border text-muted px-2 py-0"
                                                    style="font-size: 0.75rem;">
                                                    <%# Eval("QUAL_MAIN_CODE") %>
                                                </span>
                                                <%# Convert.ToInt32(Eval("IS_HIGHEST"))==1
                                                    ? "<span class='badge-highest-tick' title='Designated Highest Qualification'><i class='fas fa-check mr-1'></i>Highest</span>"
                                                    : "" %>
                                            </div>
                                            <div class="small text-muted mt-1">
                                                <%# RenderSubQualAndSpecialization(Eval("QUAL_SUB_NAME"), Eval("SPECIALIZATION")) %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="YEAR_PASSED" HeaderText="Year" ItemStyle-Width="70px"
                                        ItemStyle-HorizontalAlign="Center" />

                                    <asp:TemplateField HeaderText="Institute / University">
                                        <ItemTemplate>
                                            <div class="font-weight-bold small">
                                                <%# Eval("INSTITUTE_NAME") %>
                                            </div>
                                            <div class="small text-muted">
                                                <%# Eval("COLLEGE_NAME") != null && Eval("COLLEGE_NAME").ToString().Contains("Direct Certification") ? "<span class='badge badge-light border text-muted' style='font-size:0.75rem;'><i class='fas fa-certificate text-primary mr-1'></i> Direct Certification</span>" : Eval("COLLEGE_NAME") %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Performance" ItemStyle-Width="120px">
                                        <ItemTemplate>
                                            <div class="small"><strong>
                                                    <%# Eval("CGPA_MARKS") %>
                                                </strong></div>
                                            <div class="small text-muted">
                                                <%# Eval("GRADE_DIVISION") %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="110px"
                                        ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%# Eval("STATUS").ToString()=="Submitted"
                                                ? "<span class='badge-status badge-submitted'><i class='fas fa-lock mr-1'></i> Submitted</span>"
                                                : "<span class='badge-status badge-draft'><i class='fas fa-pencil-alt mr-1'></i> Draft</span>"
                                                %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Admin Actions" ItemStyle-Width="180px"
                                        ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <div class="d-inline-flex align-items-center flex-wrap"
                                                style="gap: 6px; justify-content: center;">
                                                <asp:LinkButton ID="btnEditRecord" runat="server"
                                                    CommandName="EditRecord" CommandArgument='<%# Eval("EDU_ID") %>'
                                                    CssClass="btn btn-sm btn-primary font-weight-bold px-2 py-1 shadow-xs"
                                                    ToolTip="Edit this qualification record directly on dashboard">
                                                    <i class="fas fa-edit mr-1"></i> Edit
                                                </asp:LinkButton>

                                                <asp:PlaceHolder ID="phRevert" runat="server"
                                                    Visible='<%# Eval("STATUS").ToString() == "Submitted" %>'>
                                                    <asp:LinkButton ID="btnRevertDraft" runat="server"
                                                        CommandName="RevertToDraft"
                                                        CommandArgument='<%# Eval("EDU_ID") %>'
                                                        CssClass="btn btn-sm btn-outline-warning font-weight-bold px-2 py-1"
                                                        ToolTip="Unlock record and revert to Draft"
                                                        data-confirm="Unlock this qualification record and revert it to Draft mode? The employee will be able to edit and re-submit it."
                                                        data-confirm-title="Revert Record to Draft"
                                                        data-confirm-type="warning"
                                                        data-confirm-btn="Yes, Revert to Draft">
                                                        <i class="fas fa-unlock mr-1"></i> Revert
                                                    </asp:LinkButton>
                                                </asp:PlaceHolder>

                                                <asp:PlaceHolder ID="phAlreadyDraft" runat="server"
                                                    Visible='<%# Eval("STATUS").ToString() == "Draft" %>'>
                                                    <asp:LinkButton ID="btnSubmitDraft" runat="server"
                                                        CommandName="SubmitToFinal"
                                                        CommandArgument='<%# Eval("EDU_ID") %>'
                                                        CssClass="btn btn-sm btn-outline-success font-weight-bold px-2 py-1"
                                                        ToolTip="Mark record as Submitted and lock"
                                                        data-confirm="Lock this record as Submitted? The record will become an official locked entry."
                                                        data-confirm-title="Lock Record as Submitted"
                                                        data-confirm-type="success"
                                                        data-confirm-btn="Yes, Lock as Submitted">
                                                        <i class="fas fa-lock mr-1"></i> Submit
                                                    </asp:LinkButton>
                                                </asp:PlaceHolder>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataRowStyle CssClass="text-center p-4 text-muted font-italic" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <!-- In-Page Admin Edit Modal -->
                <asp:Panel ID="pnlModal" runat="server" CssClass="custom-modal-backdrop" style="display: none;"
                    tabindex="-1" role="dialog">
                    <div class="custom-modal-dialog" role="document">
                        <div class="custom-modal-content">
                            <div class="modal-header bg-light py-3 px-4 d-flex justify-content-between align-items-center"
                                style="border-bottom: 1.5px solid #e2e8f0;">
                                <div>
                                    <h5
                                        class="modal-title font-weight-bold text-gray-900 mb-0 d-flex align-items-center">
                                        <i class="fas fa-graduation-cap text-primary mr-2"
                                            style="font-size: 1.25rem;"></i>
                                        <span id="lblModalTitleText">Edit Qualification Record</span>
                                    </h5>
                                    <div class="small text-muted mt-1">
                                        Modifying record for <strong>
                                            <asp:Literal ID="litEditEmpName" runat="server"></asp:Literal>
                                        </strong> (PCNO: <asp:Literal ID="litEditEmpPCNO" runat="server"></asp:Literal>)
                                    </div>
                                </div>
                                <asp:LinkButton ID="btnCloseModalX" runat="server" CssClass="custom-modal-close"
                                    OnClientClick="EduApp.closeModal(); return false;" OnClick="btnCloseModal_Click"
                                    CausesValidation="false" aria-label="Close" ToolTip="Close dialog">&times;
                                </asp:LinkButton>
                            </div>

                            <div class="modal-body p-4" style="max-height: calc(85vh - 140px); overflow-y: auto;">
                                <!-- Hidden tracking fields -->
                                <asp:HiddenField ID="hfEditEduId" runat="server" Value="0" />
                                <asp:HiddenField ID="hfEditPCNO" runat="server" Value="" />
                                <asp:HiddenField ID="hfEditEmpName" runat="server" Value="" />
                                <asp:HiddenField ID="hfEditDesig" runat="server" Value="" />
                                <asp:HiddenField ID="hfEditDiv" runat="server" Value="" />
                                <asp:HiddenField ID="hfSelectedMainQual" runat="server" Value="" />
                                <asp:HiddenField ID="hfSelectedSubQual" runat="server" Value="" />
                                <asp:HiddenField ID="hfSelectedSubQualName" runat="server" Value="" />

                                <!-- Employee Readonly Info Banner -->
                                <div class="alert alert-info py-2 px-3 mb-3 small d-flex align-items-center justify-content-between"
                                    style="border-radius: 8px;">
                                    <div>
                                        <i class="fas fa-id-card text-primary mr-1"></i>
                                        <strong>Employee:</strong>
                                        <asp:Literal ID="litEditBannerEmp" runat="server"></asp:Literal>
                                    </div>
                                    <div>
                                        <span class="badge badge-light border text-muted">Record ID: #<asp:Literal
                                                ID="litEditRecordId" runat="server"></asp:Literal></span>
                                    </div>
                                </div>

                                <!-- Client-side instant error banner (0 page reload, immediate focus) -->
                                <div id="modalClientErrorBanner" class="alert alert-danger py-2 small mb-3"
                                    style="display: none;">
                                    <i class="fas fa-exclamation-triangle mr-1"></i>
                                    <span id="modalClientErrorText"></span>
                                </div>

                                <asp:Panel ID="pnlFormError" runat="server" Visible="false"
                                    CssClass="alert alert-danger py-2 small mb-3">
                                    <i class="fas fa-exclamation-triangle mr-1"></i>
                                    <asp:Literal ID="litFormError" runat="server"></asp:Literal>
                                </asp:Panel>

                                <div class="row">
                                    <!-- Main Qualification -->
                                    <div class="col-md-6 form-group">
                                        <label class="form-label-custom">Main Qualification <span
                                                class="req">*</span></label>
                                        <asp:DropDownList ID="ddlMainQual" runat="server"
                                            CssClass="form-control form-control-custom searchable-select">
                                        </asp:DropDownList>
                                    </div>

                                    <!-- Sub Qualification / Stream -->
                                    <div class="col-md-6 form-group">
                                        <label class="form-label-custom">Sub Qualification / Stream <span
                                                class="req">*</span></label>
                                        <asp:DropDownList ID="ddlSubQual" runat="server"
                                            CssClass="form-control form-control-custom searchable-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="row">
                                    <!-- Passing Year -->
                                    <div class="col-md-4 form-group">
                                        <label class="form-label-custom">Passing Year <span class="req">*</span></label>
                                        <asp:DropDownList ID="ddlYearPassed" runat="server"
                                            CssClass="form-control form-control-custom searchable-select">
                                        </asp:DropDownList>
                                    </div>

                                    <!-- Specialization / Branch -->
                                    <div class="col-md-8 form-group">
                                        <label class="form-label-custom">Specialization / Major Subject <span
                                                class="req">*</span></label>
                                        <asp:TextBox ID="txtSpecialization" runat="server"
                                            CssClass="form-control form-control-custom"
                                            placeholder="e.g. Computer Science, Mechanical Engg, or N/A"></asp:TextBox>
                                        <small class="form-text text-muted mt-1"><i
                                                class="fas fa-info-circle mr-1 text-primary"></i> Enter branch/major
                                            subject, or <strong>N/A</strong> for general/school qualifications.</small>
                                    </div>
                                </div>

                                <div class="row">
                                    <!-- University / Board -->
                                    <div class="col-md-6 form-group">
                                        <label class="form-label-custom" id="lblAdminUniversity">University / Board / Recognized Institute <span
                                                class="req">*</span></label>
                                        <asp:DropDownList ID="ddlUniversity" runat="server"
                                            CssClass="form-control form-control-custom searchable-select">
                                        </asp:DropDownList>
                                        <small id="helpAdminUniversityCert" class="form-text text-muted mt-1" style="display: none;">
                                            <i class="fas fa-certificate mr-1 text-primary"></i> For certificates &amp; vocational courses, select the issuing authority (e.g. NCVT, DRDO/LRDE, C-DAC, State Tech Board) or 'Other Certifying Body'.
                                        </small>
                                    </div>

                                    <!-- College Name -->
                                    <div class="col-md-6 form-group">
                                        <label class="form-label-custom" id="lblAdminCollege">College / School Name <span
                                                class="req">*</span></label>
                                        <asp:DropDownList ID="ddlCollege" runat="server"
                                            CssClass="form-control form-control-custom searchable-select">
                                        </asp:DropDownList>
                                        <div class="custom-control custom-checkbox mt-1">
                                            <input type="checkbox" class="custom-control-input" id="chkAdminSameAsUniv" />
                                            <label class="custom-control-label small text-muted font-weight-normal cursor-pointer" for="chkAdminSameAsUniv" id="lblAdminSameAsUniv">
                                                <i class="fas fa-university mr-1 text-primary"></i> Studied directly at University Campus / Department (Same as University)
                                            </label>
                                        </div>
                                        <asp:Panel ID="pnlCustomCollege" runat="server" CssClass="mt-2"
                                            style="display: none;">
                                            <label class="form-label-custom small text-muted mb-1"><i
                                                    class="fas fa-pen mr-1"></i> Specify College / School Name: <span
                                                    class="req">*</span></label>
                                            <asp:TextBox ID="txtCustomCollege" runat="server"
                                                CssClass="form-control form-control-custom"
                                                placeholder="e.g. Mangalore Institute of Technology and Engineering (MITE)">
                                            </asp:TextBox>
                                        </asp:Panel>
                                    </div>
                                </div>

                                <div class="row">
                                    <!-- Score Type & CGPA / Percentage / Marks -->
                                    <div class="col-md-6 form-group">
                                        <label class="form-label-custom">Score Type &amp; Value <span
                                                class="req">*</span></label>
                                        <div class="input-group">
                                            <div class="input-group-prepend">
                                                <asp:DropDownList ID="ddlScoreType" runat="server"
                                                    CssClass="form-control form-control-custom"
                                                    style="width: 140px; border-top-right-radius: 0; border-bottom-right-radius: 0; background-color: #f8fafc; font-weight: 600; color: #1e293b;">
                                                    <asp:ListItem Value="Percentage" Selected="True">Percentage (%)
                                                    </asp:ListItem>
                                                    <asp:ListItem Value="CGPA">CGPA (10 pt)</asp:ListItem>
                                                    <asp:ListItem Value="Marks">Marks / Total</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <asp:TextBox ID="txtCgpaMarks" runat="server"
                                                CssClass="form-control form-control-custom" placeholder="e.g. 78.50%"
                                                style="border-top-left-radius: 0; border-bottom-left-radius: 0;">
                                            </asp:TextBox>
                                        </div>
                                        <small class="form-text text-muted mt-1" id="scoreHelpText">
                                            <i class="fas fa-info-circle mr-1 text-primary"></i> Legacy records may be
                                            CGPA or % (e.g. <strong>78.5%</strong> or <strong>8.45 CGPA</strong>).
                                        </small>
                                    </div>

                                    <!-- Grade / Division -->
                                    <div class="col-md-6 form-group">
                                        <label class="form-label-custom">Grade / Division <span
                                                class="req">*</span></label>
                                        <asp:DropDownList ID="ddlGrade" runat="server"
                                            CssClass="form-control form-control-custom searchable-select">
                                            <asp:ListItem Value="">-- Select Grade / Division --</asp:ListItem>
                                            <asp:ListItem Value="First Class with Distinction">First Class with
                                                Distinction</asp:ListItem>
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

                                <div class="row">
                                    <!-- Status Selection (Draft vs Submitted) -->
                                    <div class="col-md-6 form-group">
                                        <label class="form-label-custom">Record Status <span
                                                class="req">*</span></label>
                                        <asp:DropDownList ID="ddlRecordStatus" runat="server"
                                            CssClass="form-control form-control-custom">
                                            <asp:ListItem Value="Draft">Draft (Editable)</asp:ListItem>
                                            <asp:ListItem Value="Submitted">Submitted (Locked Official Entry)
                                            </asp:ListItem>
                                        </asp:DropDownList>
                                        <small class="form-text text-muted mt-1"><i
                                                class="fas fa-info-circle mr-1 text-primary"></i> Administrators can
                                            directly set status to Draft or Submitted.</small>
                                    </div>

                                    <!-- Highest Qualification Checkbox -->
                                    <div class="col-md-6 form-group d-flex align-items-center pt-md-4">
                                        <div class="p-3 rounded w-100"
                                            style="background: #f8fafc; border: 1.5px solid #e2e8f0;">
                                            <label class="d-flex align-items-center mb-0" style="cursor: pointer;"
                                                for="<%= chkIsHighest.ClientID %>">
                                                <asp:CheckBox ID="chkIsHighest" runat="server"
                                                    style="transform: scale(1.35); margin-right: 12px; cursor: pointer;" />
                                                <div>
                                                    <span class="font-weight-bold text-gray-900"
                                                        style="font-size: 0.95rem;">
                                                        <i class="fas fa-check-circle text-success mr-1"></i> Highest
                                                        Qualification
                                                    </span>
                                                    <div class="small text-muted mt-1" style="line-height: 1.35;">
                                                        Designates this qualification as the highest degree achieved.
                                                    </div>
                                                </div>
                                            </label>
                                        </div>
                                    </div>
                                </div>

                                <!-- Remarks -->
                                <div class="form-group mb-0">
                                    <label class="form-label-custom">Remarks (Optional)</label>
                                    <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Rows="2"
                                        CssClass="form-control form-control-custom"
                                        placeholder="Administrative notes, roll numbers, or honors..."></asp:TextBox>
                                </div>
                            </div>

                            <div class="modal-footer bg-light py-3 px-4 d-flex justify-content-between align-items-center"
                                style="border-top: 1.5px solid #e2e8f0;">
                                <div>
                                    <asp:Button ID="btnCancelModal" runat="server" Text="Cancel"
                                        CssClass="btn btn-edu-outline px-3"
                                        OnClientClick="EduApp.closeModal(); return false;" OnClick="btnCloseModal_Click"
                                        CausesValidation="false" />
                                </div>
                                <div class="d-flex align-items-center" style="gap: 10px;">
                                    <asp:Button ID="btnSaveAdminEdit" runat="server" Text="Save Changes"
                                        CssClass="btn btn-edu-primary font-weight-bold px-4 shadow-sm"
                                        OnClientClick="return EduApp.validateModalForm(false);"
                                        OnClick="btnSaveAdminEdit_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Content>

    <asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
        <!-- Embedded Full Sub Qualifications Catalog (Ensures 100% Zero-Loss on Postbacks) -->
        <script id="subQualDataScript">
            window.EDU_ALL_SUB_QUALS = <%= GetSubQualsJson() %>;
        </script>
        <script>
            var liveSearchTimer = null;
            var lastSearchedVal = null;
            var isLiveSearchRunning = false;
            var pendingSearchVal = null;
            var savedCursorStart = null;
            var savedCursorEnd = null;
            var wasSearchFocused = false;

            function setupLiveSearch() {
                var $txt = $('#<%= txtSearch.ClientID %>');
                var $clearBtn = $('#btnLiveSearchClear');
                var $spinner = $('#liveSearchSpinner');
                var $btnFilter = $('#<%= btnFilter.ClientID %>');

                if ($txt.length === 0) return;

                if (lastSearchedVal === null) {
                    lastSearchedVal = $txt.val();
                }

                function updateClearButton() {
                    var v = $txt.val();
                    if (v && v.trim().length > 0) {
                        if (!isLiveSearchRunning) {
                            $clearBtn.show();
                            $spinner.hide();
                        }
                    } else {
                        $clearBtn.hide();
                        if (!isLiveSearchRunning) $spinner.hide();
                    }
                }
                updateClearButton();

                // Clear button click
                $clearBtn.off('click.clear').on('click.clear', function (e) {
                    e.preventDefault();
                    $txt.val('').focus();
                    savedCursorStart = 0;
                    savedCursorEnd = 0;
                    wasSearchFocused = true;
                    updateClearButton();
                    clearTimeout(liveSearchTimer);
                    triggerLiveSearch(true);
                });

                // Live typing in txtSearch
                $txt.off('input.livesearch keyup.livesearch').on('input.livesearch keyup.livesearch', function (e) {
                    var navKeys = ['ArrowDown', 'ArrowUp', 'ArrowLeft', 'ArrowRight', 'Shift', 'Control', 'Alt', 'Meta', 'CapsLock', 'Tab'];
                    if (navKeys.indexOf(e.key) !== -1) {
                        return;
                    }

                    updateClearButton();

                    if (e.key === 'Enter') {
                        e.preventDefault();
                        clearTimeout(liveSearchTimer);
                        triggerLiveSearch(true);
                        return;
                    }

                    clearTimeout(liveSearchTimer);
                    liveSearchTimer = setTimeout(function () {
                        triggerLiveSearch(false);
                    }, 320);
                });

                // Prevent standard full form submission on Enter in search box
                $txt.off('keydown.enter').on('keydown.enter', function (e) {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        clearTimeout(liveSearchTimer);
                        triggerLiveSearch(true);
                    }
                });

                // Auto-search immediately on dropdown changes
                $('#<%= ddlStatusFilter.ClientID %>, #<%= ddlQualFilter.ClientID %>, #<%= ddlHighestFilter.ClientID %>').off('change.livesearch').on('change.livesearch', function () {
                    triggerLiveSearch(true);
                });

                function triggerLiveSearch(immediate) {
                    var currentVal = $txt.val();

                    var el = $txt[0];
                    if (el) {
                        wasSearchFocused = (document.activeElement === el);
                        if (el.setSelectionRange) {
                            savedCursorStart = el.selectionStart;
                            savedCursorEnd = el.selectionEnd;
                        }
                    }

                    if (isLiveSearchRunning) {
                        pendingSearchVal = currentVal;
                        return;
                    }

                    if (!immediate && currentVal === lastSearchedVal) {
                        return;
                    }

                    lastSearchedVal = currentVal;
                    isLiveSearchRunning = true;
                    $clearBtn.hide();
                    $spinner.show();

                    $btnFilter.click();
                }
            }

            function initAdminEducationUI() {
                // Dashboard Table Filter Dropdowns
                EduApp.makeSearchable('#<%= ddlStatusFilter.ClientID %>', 'All Statuses');
                EduApp.makeSearchable('#<%= ddlQualFilter.ClientID %>', 'All Qualifications');
                EduApp.makeSearchable('#<%= ddlHighestFilter.ClientID %>', 'All Records');

                // Initialize live search as-you-type handler
                setupLiveSearch();

                // Modal Dropdowns & Cascading Logic
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

                    function syncAdminSameAsUnivCheckbox() {
                        var val = $('#<%= ddlCollege.ClientID %>').val();
                        var isSame = (val === 'University Campus / Department' || val === 'Direct Certification / Same as Issuing Authority');
                        $('#chkAdminSameAsUniv').prop('checked', isSame);
                    }

                    $('#chkAdminSameAsUniv').off('change').on('change', function () {
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
                        syncAdminSameAsUnivCheckbox();
                    });

                    // Initial check in case of existing selection
                    checkCollegeCustomInput();
                    syncAdminSameAsUnivCheckbox();
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
                initAdminEducationUI();
            });

            // Re-initialize searchable dropdowns and UI seamlessly after every partial AJAX postback
            if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    isLiveSearchRunning = false;
                    $('#liveSearchSpinner').hide();

                    initAdminEducationUI();
                    EduApp.initIntranetPhotos();

                    // If modal tracking field indicates closed (0), ensure modal backdrop is strictly hidden
                    var modalState = $('input[id$="hfModalOpen"]').val();
                    if (modalState === '0') {
                        $('[id$="pnlModal"]').css('display', 'none');
                    }

                    // Restore search input focus and cursor position seamlessly
                    var $newTxt = $('#<%= txtSearch.ClientID %>');
                    if ($newTxt.length > 0) {
                        lastSearchedVal = $newTxt.val();
                        if (wasSearchFocused) {
                            $newTxt.focus();
                            var el = $newTxt[0];
                            if (el && el.setSelectionRange && savedCursorStart !== null) {
                                try {
                                    el.setSelectionRange(savedCursorStart, savedCursorEnd);
                                } catch (err) { }
                            }
                        }

                        // If user kept typing while postback was in flight, execute the pending search
                        if (pendingSearchVal !== null && pendingSearchVal !== lastSearchedVal) {
                            var nextSearch = pendingSearchVal;
                            pendingSearchVal = null;
                            clearTimeout(liveSearchTimer);
                            liveSearchTimer = setTimeout(function () {
                                $newTxt.val(nextSearch);
                                $('#<%= btnFilter.ClientID %>').click();
                            }, 200);
                        } else {
                            pendingSearchVal = null;
                        }
                    }
                });
            }
        </script>
    </asp:Content>