<%@ Page Title="Master Data Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminMasterData.aspx.cs" Inherits="EducationApp.AdminMasterData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .nav-tabs-custom {
            border-bottom: 2px solid var(--border-color);
        }
        .nav-tabs-custom .nav-link {
            font-weight: 600;
            color: var(--text-muted);
            border: none;
            padding: 0.8rem 1.4rem;
            border-radius: 8px 8px 0 0;
            margin-right: 4px;
            transition: all 0.2s ease;
        }
        .nav-tabs-custom .nav-link.active {
            color: var(--primary);
            background: #ffffff;
            border-bottom: 3px solid var(--primary);
        }

        /* Modern Sleek Action Buttons */
        .action-btn-group {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
        }

        .btn-action {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 5px;
            padding: 5px 10px;
            font-size: 0.8rem;
            font-weight: 600;
            border-radius: 6px;
            border: 1px solid transparent;
            transition: all 0.15s ease-in-out;
            text-decoration: none !important;
            cursor: pointer;
            line-height: 1.2;
        }

        .btn-action i {
            font-size: 0.78rem;
        }

        /* Edit: Soft Blue / Indigo */
        .btn-action-edit {
            background-color: #eff6ff;
            color: #2563eb;
            border-color: #bfdbfe;
        }
        .btn-action-edit:hover {
            background-color: #2563eb;
            color: #ffffff;
            border-color: #2563eb;
            box-shadow: 0 3px 8px rgba(37, 99, 235, 0.25);
            transform: translateY(-1px);
        }

        /* Delete: Soft Rose / Crimson */
        .btn-action-delete {
            background-color: #fef2f2;
            color: #dc2626;
            border-color: #fecaca;
        }
        .btn-action-delete:hover {
            background-color: #dc2626;
            color: #ffffff;
            border-color: #dc2626;
            box-shadow: 0 3px 8px rgba(220, 38, 38, 0.25);
            transform: translateY(-1px);
        }

        /* Activate: Soft Emerald */
        .btn-action-activate {
            background-color: #ecfdf5;
            color: #059669;
            border-color: #a7f3d0;
        }
        .btn-action-activate:hover {
            background-color: #059669;
            color: #ffffff;
            border-color: #059669;
            box-shadow: 0 3px 8px rgba(5, 150, 105, 0.25);
            transform: translateY(-1px);
        }

        /* Revoke: Soft Amber */
        .btn-action-revoke {
            background-color: #fffbeb;
            color: #d97706;
            border-color: #fde68a;
        }
        .btn-action-revoke:hover {
            background-color: #d97706;
            color: #ffffff;
            border-color: #d97706;
            box-shadow: 0 3px 8px rgba(217, 119, 6, 0.25);
            transform: translateY(-1px);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Built-in AJAX ScriptManager for Partial Postbacks (Zero Full Page Refresh) -->
    <asp:ScriptManager ID="smMasterData" runat="server" EnablePartialRendering="true" />

    <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
            <h3 class="font-weight-bold text-gray-900 mb-1">
                <i class="fas fa-layer-group text-primary mr-2"></i> Master Catalog Management
            </h3>
            <p class="text-muted small mb-0">Configure Main Qualifications, Sub Qualifications, Universities, Subjects, and Admin Access</p>
        </div>
        <div>
            <a href="AdminDashboard.aspx" class="btn btn-edu-outline font-weight-bold">
                <i class="fas fa-arrow-left mr-1"></i> Back to Dashboard
            </a>
        </div>
    </div>

    <!-- Asynchronous UpdatePanel to Ensure Zero Full Page Reloads -->
    <asp:UpdatePanel ID="upMasterData" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <!-- Active Tab State HiddenField -->
            <asp:HiddenField ID="hfActiveTab" runat="server" Value="#tab-main" />

            <!-- Master Tabs -->
            <ul class="nav nav-tabs nav-tabs-custom mb-4" id="masterTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <a class='<%= GetTabLinkClass("#tab-main") %>' id="tab-main-tab" data-bs-toggle="tab" data-bs-target="#tab-main" data-toggle="tab" href="#tab-main" role="tab" aria-controls="tab-main">
                        <i class="fas fa-award mr-1"></i> Main Qualifications
                    </a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class='<%= GetTabLinkClass("#tab-sub") %>' id="tab-sub-tab" data-bs-toggle="tab" data-bs-target="#tab-sub" data-toggle="tab" href="#tab-sub" role="tab" aria-controls="tab-sub">
                        <i class="fas fa-code-branch mr-1"></i> Sub Qualifications
                    </a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class='<%= GetTabLinkClass("#tab-univ") %>' id="tab-univ-tab" data-bs-toggle="tab" data-bs-target="#tab-univ" data-toggle="tab" href="#tab-univ" role="tab" aria-controls="tab-univ">
                        <i class="fas fa-university mr-1"></i> Universities &amp; Boards
                    </a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class='<%= GetTabLinkClass("#tab-college") %>' id="tab-college-tab" data-bs-toggle="tab" data-bs-target="#tab-college" data-toggle="tab" href="#tab-college" role="tab" aria-controls="tab-college">
                        <i class="fas fa-school mr-1"></i> Colleges &amp; Schools
                    </a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class='<%= GetTabLinkClass("#tab-subj") %>' id="tab-subj-tab" data-bs-toggle="tab" data-bs-target="#tab-subj" data-toggle="tab" href="#tab-subj" role="tab" aria-controls="tab-subj">
                        <i class="fas fa-book mr-1"></i> Subjects / Trades
                    </a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class='<%= GetTabLinkClass("#tab-admins") %>' id="tab-admins-tab" data-bs-toggle="tab" data-bs-target="#tab-admins" data-toggle="tab" href="#tab-admins" role="tab" aria-controls="tab-admins">
                        <i class="fas fa-user-shield mr-1"></i> Admin Privileges
                    </a>
                </li>
            </ul>

            <div class="tab-content">
                <!-- 1. MAIN QUALIFICATIONS -->
                <div class='<%= GetTabPaneClass("#tab-main") %>' id="tab-main" role="tabpanel">
                    <div class="row">
                        <div class="col-lg-4 mb-4">
                            <div class="edu-card">
                                <div class="edu-card-header">
                                    <div class="edu-card-title"><i class="fas fa-plus-circle"></i> Add Main Qualification</div>
                                </div>
                                <div class="edu-card-body">
                                    <div class="form-group">
                                        <label class="form-label-custom">Qualification Code <span class="req">*</span></label>
                                        <asp:TextBox ID="txtMainCode" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. BE, MTECH, BSC"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Full Qualification Name <span class="req">*</span></label>
                                        <asp:TextBox ID="txtMainName" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Bachelor of Engineering"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Category / Level</label>
                                        <asp:DropDownList ID="ddlMainCategory" runat="server" CssClass="form-control form-control-custom searchable-select">
                                            <asp:ListItem Value="Secondary School">Secondary School (10th)</asp:ListItem>
                                            <asp:ListItem Value="Higher Secondary">Higher Secondary (12th/PUC)</asp:ListItem>
                                            <asp:ListItem Value="Diploma">Diploma</asp:ListItem>
                                            <asp:ListItem Value="Undergraduate" Selected="True">Undergraduate Degree</asp:ListItem>
                                            <asp:ListItem Value="Postgraduate">Postgraduate Degree</asp:ListItem>
                                            <asp:ListItem Value="Doctorate / Research">Doctorate / Research</asp:ListItem>
                                            <asp:ListItem Value="Vocational / Trade">Vocational / ITI</asp:ListItem>
                                            <asp:ListItem Value="Other">Other</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <asp:Button ID="btnAddMainQual" runat="server" Text="Save Main Qualification" CssClass="btn btn-edu-primary btn-block font-weight-bold" OnClick="btnAddMainQual_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-8">
                            <div class="edu-card">
                                <div class="edu-card-header d-flex flex-wrap justify-content-between align-items-center py-2.5 px-3">
                                    <div class="edu-card-title mb-2 mb-sm-0">
                                        <i class="fas fa-list"></i> Catalog of Main Qualifications
                                    </div>
                                    <div class="d-flex align-items-center" style="min-width: 260px; max-width: 340px; width: 100%;">
                                        <div class="input-group input-group-sm">
                                            <input type="text" id="searchMainQuals" class="form-control form-control-sm tab-search-input" data-target-grid="MainContent_gvMainQuals" data-input-id="searchMainQuals" placeholder="Search code, name, category..." autocomplete="off" />
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-sm btn-edu-primary font-weight-bold btn-tab-search" data-target-grid="MainContent_gvMainQuals" data-input-id="searchMainQuals" title="Search">
                                                    <i class="fas fa-search mr-1"></i> Search
                                                </button>
                                                <button type="button" class="btn btn-sm btn-light border btn-tab-search-clear" data-target-grid="MainContent_gvMainQuals" data-input-id="searchMainQuals" title="Clear search">
                                                    <i class="fas fa-times"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="edu-card-body p-0">
                                    <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                        <asp:GridView ID="gvMainQuals" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                                            OnRowCommand="gvMainQuals_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="QUAL_CODE" HeaderText="Code" ItemStyle-Width="100px" ItemStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="QUAL_NAME" HeaderText="Qualification Name" />
                                                <asp:BoundField DataField="QUAL_CATEGORY" HeaderText="Category" ItemStyle-Width="180px" />
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <div class="action-btn-group">
                                                            <asp:LinkButton ID="btnEditMain" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("QUAL_CODE") %>'
                                                                CssClass="btn-action btn-action-edit" ToolTip="Edit Qualification">
                                                                <i class="fas fa-pen"></i> Edit
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="btnDeleteMain" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("QUAL_CODE") %>'
                                                                OnClientClick="if (!confirm('Are you sure you want to delete this qualification?')) return false;"
                                                                CssClass="btn-action btn-action-delete" ToolTip="Delete Qualification">
                                                                <i class="fas fa-trash-alt"></i> Delete
                                                            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 2. SUB QUALIFICATIONS -->
                <div class='<%= GetTabPaneClass("#tab-sub") %>' id="tab-sub" role="tabpanel">
                    <div class="row">
                        <div class="col-lg-4 mb-4">
                            <div class="edu-card">
                                <div class="edu-card-header">
                                    <div class="edu-card-title"><i class="fas fa-plus-circle"></i> Add Sub Qualification</div>
                                </div>
                                <div class="edu-card-body">
                                    <div class="form-group">
                                        <label class="form-label-custom">Link to Main Qualification</label>
                                        <asp:DropDownList ID="ddlSubParentMain" runat="server" CssClass="form-control form-control-custom searchable-select">
                                        </asp:DropDownList>
                                        <small class="text-muted">Leave unselected to make available for all qualifications.</small>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Sub Code <span class="req">*</span></label>
                                        <asp:TextBox ID="txtSubCode" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. COMPS, DMECH, E&C"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Sub Qualification Name / Stream <span class="req">*</span></label>
                                        <asp:TextBox ID="txtSubName" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Computer Science Engineering"></asp:TextBox>
                                    </div>
                                    <asp:Button ID="btnAddSubQual" runat="server" Text="Save Sub Qualification" CssClass="btn btn-edu-primary btn-block font-weight-bold" OnClick="btnAddSubQual_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-8">
                            <div class="edu-card">
                                <div class="edu-card-header d-flex flex-wrap justify-content-between align-items-center py-2.5 px-3">
                                    <div class="edu-card-title mb-2 mb-sm-0">
                                        <i class="fas fa-list"></i> Sub Qualifications / Streams
                                    </div>
                                    <div class="d-flex align-items-center" style="min-width: 260px; max-width: 340px; width: 100%;">
                                        <div class="input-group input-group-sm">
                                            <input type="text" id="searchSubQuals" class="form-control form-control-sm tab-search-input" data-target-grid="MainContent_gvSubQuals" data-input-id="searchSubQuals" placeholder="Search code, stream, parent..." autocomplete="off" />
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-sm btn-edu-primary font-weight-bold btn-tab-search" data-target-grid="MainContent_gvSubQuals" data-input-id="searchSubQuals" title="Search">
                                                    <i class="fas fa-search mr-1"></i> Search
                                                </button>
                                                <button type="button" class="btn btn-sm btn-light border btn-tab-search-clear" data-target-grid="MainContent_gvSubQuals" data-input-id="searchSubQuals" title="Clear search">
                                                    <i class="fas fa-times"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="edu-card-body p-0">
                                    <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                        <asp:GridView ID="gvSubQuals" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                                            OnRowCommand="gvSubQuals_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="SUB_CODE" HeaderText="Code" ItemStyle-Width="100px" ItemStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="SUB_NAME" HeaderText="Sub Qualification / Stream Name" />
                                                <asp:BoundField DataField="MAIN_QUAL_CODE" HeaderText="Parent Qual" ItemStyle-Width="120px" NullDisplayText="All" />
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <div class="action-btn-group">
                                                            <asp:LinkButton ID="btnEditSub" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("SUB_ID") %>'
                                                                CssClass="btn-action btn-action-edit" ToolTip="Edit Sub Qualification">
                                                                <i class="fas fa-pen"></i> Edit
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="btnDeleteSub" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("SUB_ID") %>'
                                                                OnClientClick="if (!confirm('Are you sure you want to delete this sub qualification?')) return false;"
                                                                CssClass="btn-action btn-action-delete" ToolTip="Delete Sub Qualification">
                                                                <i class="fas fa-trash-alt"></i> Delete
                                                            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 3. UNIVERSITIES -->
                <div class='<%= GetTabPaneClass("#tab-univ") %>' id="tab-univ" role="tabpanel">
                    <div class="row">
                        <div class="col-lg-4 mb-4">
                            <div class="edu-card">
                                <div class="edu-card-header">
                                    <div class="edu-card-title"><i class="fas fa-plus-circle"></i> Add University / Board</div>
                                </div>
                                <div class="edu-card-body">
                                    <div class="form-group">
                                        <label class="form-label-custom">University / Board Name <span class="req">*</span></label>
                                        <asp:TextBox ID="txtUnivName" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. VTU, Belgaum / CBSE Board"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">State or Type</label>
                                        <asp:TextBox ID="txtUnivType" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. State University / Central / Deemed"></asp:TextBox>
                                    </div>
                                    <asp:Button ID="btnAddUniv" runat="server" Text="Save University" CssClass="btn btn-edu-primary btn-block font-weight-bold" OnClick="btnAddUniv_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-8">
                            <div class="edu-card">
                                <div class="edu-card-header d-flex flex-wrap justify-content-between align-items-center py-2.5 px-3">
                                    <div class="edu-card-title mb-2 mb-sm-0">
                                        <i class="fas fa-list"></i> Recognized Universities &amp; Institutes
                                    </div>
                                    <div class="d-flex align-items-center" style="min-width: 260px; max-width: 340px; width: 100%;">
                                        <div class="input-group input-group-sm">
                                            <input type="text" id="searchUniversities" class="form-control form-control-sm tab-search-input" data-target-grid="MainContent_gvUniversities" data-input-id="searchUniversities" placeholder="Search university, state, type..." autocomplete="off" />
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-sm btn-edu-primary font-weight-bold btn-tab-search" data-target-grid="MainContent_gvUniversities" data-input-id="searchUniversities" title="Search">
                                                    <i class="fas fa-search mr-1"></i> Search
                                                </button>
                                                <button type="button" class="btn btn-sm btn-light border btn-tab-search-clear" data-target-grid="MainContent_gvUniversities" data-input-id="searchUniversities" title="Clear search">
                                                    <i class="fas fa-times"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="edu-card-body p-0">
                                    <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                        <asp:GridView ID="gvUniversities" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                                            OnRowCommand="gvUniversities_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="UNIV_ID" HeaderText="ID" ItemStyle-Width="70px" ItemStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="UNIV_NAME" HeaderText="University / Institution Name" />
                                                <asp:BoundField DataField="STATE_OR_TYPE" HeaderText="State / Type" ItemStyle-Width="180px" />
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <div class="action-btn-group">
                                                            <asp:LinkButton ID="btnEditUniv" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("UNIV_ID") %>'
                                                                CssClass="btn-action btn-action-edit" ToolTip="Edit University">
                                                                <i class="fas fa-pen"></i> Edit
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="btnDeleteUniv" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("UNIV_ID") %>'
                                                                OnClientClick="if (!confirm('Are you sure you want to delete this university?')) return false;"
                                                                CssClass="btn-action btn-action-delete" ToolTip="Delete University">
                                                                <i class="fas fa-trash-alt"></i> Delete
                                                            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 3b. COLLEGES & SCHOOLS -->
                <div class='<%= GetTabPaneClass("#tab-college") %>' id="tab-college" role="tabpanel">
                    <div class="row">
                        <div class="col-lg-4 mb-4">
                            <div class="edu-card">
                                <div class="edu-card-header">
                                    <div class="edu-card-title"><i class="fas fa-plus-circle"></i> Add College / School</div>
                                </div>
                                <div class="edu-card-body">
                                    <div class="form-group">
                                        <label class="form-label-custom">College / School Name <span class="req">*</span></label>
                                        <asp:TextBox ID="txtCollegeNameAdd" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Mangalore Institute of Technology and Engineering (MITE)"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Category / Type</label>
                                        <asp:DropDownList ID="ddlCollegeTypeAdd" runat="server" CssClass="form-control form-control-custom">
                                            <asp:ListItem Value="Engineering College" Selected="True">Engineering College</asp:ListItem>
                                            <asp:ListItem Value="Degree / Autonomous College">Degree / Autonomous College</asp:ListItem>
                                            <asp:ListItem Value="PU College / Higher Secondary">PU College / Higher Secondary (11th/12th)</asp:ListItem>
                                            <asp:ListItem Value="High School / Secondary">High School / Secondary (10th)</asp:ListItem>
                                            <asp:ListItem Value="Polytechnic / Diploma">Polytechnic / Diploma</asp:ListItem>
                                            <asp:ListItem Value="Medical / Dental / Pharmacy">Medical / Dental / Pharmacy</asp:ListItem>
                                            <asp:ListItem Value="Vocational / ITI">Vocational / ITI</asp:ListItem>
                                            <asp:ListItem Value="Other">Other Educational Institute</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Location / City / State</label>
                                        <asp:TextBox ID="txtCollegeLocationAdd" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Moodabidri, Mangaluru, Karnataka"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Affiliated University / Board</label>
                                        <asp:TextBox ID="txtCollegeAffilAdd" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Visvesvaraya Technological University (VTU)"></asp:TextBox>
                                    </div>
                                    <asp:Button ID="btnAddCollege" runat="server" Text="Save College / School" CssClass="btn btn-edu-primary btn-block font-weight-bold" OnClick="btnAddCollege_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-8">
                            <div class="edu-card">
                                <div class="edu-card-header d-flex flex-wrap justify-content-between align-items-center py-2.5 px-3">
                                    <div class="edu-card-title mb-2 mb-sm-0">
                                        <i class="fas fa-school"></i> Colleges, Schools &amp; Institutes
                                    </div>
                                    <div class="d-flex align-items-center" style="min-width: 260px; max-width: 360px; width: 100%;">
                                        <div class="input-group input-group-sm">
                                            <input type="text" id="searchColleges" class="form-control form-control-sm tab-search-input" data-target-grid="MainContent_gvColleges" data-input-id="searchColleges" placeholder="Search college, type, location..." autocomplete="off" />
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-sm btn-edu-primary font-weight-bold btn-tab-search" data-target-grid="MainContent_gvColleges" data-input-id="searchColleges" title="Search">
                                                    <i class="fas fa-search mr-1"></i> Search
                                                </button>
                                                <button type="button" class="btn btn-sm btn-light border btn-tab-search-clear" data-target-grid="MainContent_gvColleges" data-input-id="searchColleges" title="Clear search">
                                                    <i class="fas fa-times"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="edu-card-body p-0">
                                    <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                        <asp:GridView ID="gvColleges" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                                            OnRowCommand="gvColleges_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="COLLEGE_ID" HeaderText="ID" ItemStyle-Width="60px" ItemStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="COLLEGE_NAME" HeaderText="College / School Name" />
                                                <asp:BoundField DataField="COLLEGE_TYPE" HeaderText="Type / Level" ItemStyle-Width="150px" />
                                                <asp:BoundField DataField="LOCATION" HeaderText="Location" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="AFFILIATED_UNIV" HeaderText="Affiliation / Board" ItemStyle-Width="170px" />
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="140px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <div class="action-btn-group">
                                                            <asp:LinkButton ID="btnEditCollege" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("COLLEGE_ID") %>'
                                                                CssClass="btn-action btn-action-edit" ToolTip="Edit College">
                                                                <i class="fas fa-pen"></i> Edit
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="btnDeleteCollege" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("COLLEGE_ID") %>'
                                                                OnClientClick="if (!confirm('Are you sure you want to delete this college/school?')) return false;"
                                                                CssClass="btn-action btn-action-delete" ToolTip="Delete College">
                                                                <i class="fas fa-trash-alt"></i> Delete
                                                            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 4. SUBJECTS -->
                <div class='<%= GetTabPaneClass("#tab-subj") %>' id="tab-subj" role="tabpanel">
                    <div class="row">
                        <div class="col-lg-4 mb-4">
                            <div class="edu-card">
                                <div class="edu-card-header">
                                    <div class="edu-card-title"><i class="fas fa-plus-circle"></i> Add Subject / Trade</div>
                                </div>
                                <div class="edu-card-body">
                                    <div class="form-group">
                                        <label class="form-label-custom">Subject / Trade Name <span class="req">*</span></label>
                                        <asp:TextBox ID="txtSubjName" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Microelectronics, Fitter"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Subject Group / Domain</label>
                                        <asp:TextBox ID="txtSubjGroup" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. Electronics, Computer Science"></asp:TextBox>
                                    </div>
                                    <asp:Button ID="btnAddSubj" runat="server" Text="Save Subject" CssClass="btn btn-edu-primary btn-block font-weight-bold" OnClick="btnAddSubj_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-8">
                            <div class="edu-card">
                                <div class="edu-card-header d-flex flex-wrap justify-content-between align-items-center py-2.5 px-3">
                                    <div class="edu-card-title mb-2 mb-sm-0">
                                        <i class="fas fa-list"></i> Subjects &amp; Disciplines
                                    </div>
                                    <div class="d-flex align-items-center" style="min-width: 260px; max-width: 340px; width: 100%;">
                                        <div class="input-group input-group-sm">
                                            <input type="text" id="searchSubjects" class="form-control form-control-sm tab-search-input" data-target-grid="MainContent_gvSubjects" data-input-id="searchSubjects" placeholder="Search subject, trade, group..." autocomplete="off" />
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-sm btn-edu-primary font-weight-bold btn-tab-search" data-target-grid="MainContent_gvSubjects" data-input-id="searchSubjects" title="Search">
                                                    <i class="fas fa-search mr-1"></i> Search
                                                </button>
                                                <button type="button" class="btn btn-sm btn-light border btn-tab-search-clear" data-target-grid="MainContent_gvSubjects" data-input-id="searchSubjects" title="Clear search">
                                                    <i class="fas fa-times"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="edu-card-body p-0">
                                    <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                        <asp:GridView ID="gvSubjects" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                                            OnRowCommand="gvSubjects_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="SUBJECT_ID" HeaderText="ID" ItemStyle-Width="70px" ItemStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="SUBJECT_NAME" HeaderText="Subject / Trade Name" />
                                                <asp:BoundField DataField="SUBJECT_GROUP" HeaderText="Domain Group" ItemStyle-Width="180px" />
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <div class="action-btn-group">
                                                            <asp:LinkButton ID="btnEditSubj" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("SUBJECT_ID") %>'
                                                                CssClass="btn-action btn-action-edit" ToolTip="Edit Subject">
                                                                <i class="fas fa-pen"></i> Edit
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="btnDeleteSubj" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("SUBJECT_ID") %>'
                                                                OnClientClick="if (!confirm('Are you sure you want to delete this subject?')) return false;"
                                                                CssClass="btn-action btn-action-delete" ToolTip="Delete Subject">
                                                                <i class="fas fa-trash-alt"></i> Delete
                                                            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 5. ADMIN PRIVILEGES -->
                <div class='<%= GetTabPaneClass("#tab-admins") %>' id="tab-admins" role="tabpanel">
                    <div class="row">
                        <div class="col-lg-4 mb-4">
                            <div class="edu-card">
                                <div class="edu-card-header">
                                    <div class="edu-card-title"><i class="fas fa-user-plus"></i> Grant Admin Role</div>
                                </div>
                                <div class="edu-card-body">
                                    <div class="form-group">
                                        <label class="form-label-custom">Employee PCNO <span class="req">*</span></label>
                                        <asp:TextBox ID="txtAdminPCNO" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. 1002, 1003"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label-custom">Admin Display Name</label>
                                        <asp:TextBox ID="txtAdminName" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. HR Admin / Education Officer"></asp:TextBox>
                                    </div>
                                    <asp:Button ID="btnAddAdmin" runat="server" Text="Grant Admin Access" CssClass="btn btn-edu-primary btn-block font-weight-bold" OnClick="btnAddAdmin_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-8">
                            <div class="edu-card">
                                <div class="edu-card-header d-flex flex-wrap justify-content-between align-items-center py-2.5 px-3">
                                    <div class="edu-card-title mb-2 mb-sm-0">
                                        <i class="fas fa-users-cog"></i> Authorized Administrators
                                    </div>
                                    <div class="d-flex align-items-center" style="min-width: 260px; max-width: 340px; width: 100%;">
                                        <div class="input-group input-group-sm">
                                            <input type="text" id="searchAdmins" class="form-control form-control-sm tab-search-input" data-target-grid="MainContent_gvAdmins" data-input-id="searchAdmins" placeholder="Search PCNO, name, role..." autocomplete="off" />
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-sm btn-edu-primary font-weight-bold btn-tab-search" data-target-grid="MainContent_gvAdmins" data-input-id="searchAdmins" title="Search">
                                                    <i class="fas fa-search mr-1"></i> Search
                                                </button>
                                                <button type="button" class="btn btn-sm btn-light border btn-tab-search-clear" data-target-grid="MainContent_gvAdmins" data-input-id="searchAdmins" title="Clear search">
                                                    <i class="fas fa-times"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="edu-card-body p-0">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvAdmins" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                                            OnRowCommand="gvAdmins_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="PCNO" HeaderText="PCNO" ItemStyle-Width="100px" ItemStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="USER_NAME" HeaderText="Name" />
                                                <asp:BoundField DataField="ROLE" HeaderText="Role" ItemStyle-Width="120px" />
                                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="110px">
                                                    <ItemTemplate>
                                                        <%# Convert.ToInt32(Eval("IS_ACTIVE")) == 1 
                                                            ? "<span class='badge badge-success'>Active</span>" 
                                                            : "<span class='badge badge-secondary'>Revoked</span>" %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="210px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <div class="action-btn-group">
                                                            <asp:LinkButton ID="btnEditAdmin" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("PCNO") %>'
                                                                CssClass="btn-action btn-action-edit" ToolTip="Edit Administrator">
                                                                <i class="fas fa-pen"></i> Edit
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="btnToggleAdmin" runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("PCNO") %>'
                                                                CssClass='<%# Convert.ToInt32(Eval("IS_ACTIVE")) == 1 ? "btn-action btn-action-revoke" : "btn-action btn-action-activate" %>'
                                                                ToolTip='<%# Convert.ToInt32(Eval("IS_ACTIVE")) == 1 ? "Revoke Admin Access" : "Activate Admin Access" %>'>
                                                                <i class='<%# Convert.ToInt32(Eval("IS_ACTIVE")) == 1 ? "fas fa-user-slash" : "fas fa-user-check" %>'></i>
                                                                <%# Convert.ToInt32(Eval("IS_ACTIVE")) == 1 ? "Revoke" : "Activate" %>
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="btnDeleteAdmin" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("PCNO") %>'
                                                                Visible='<%# Eval("PCNO").ToString() != "1001" %>'
                                                                OnClientClick="if (!confirm('Are you sure you want to permanently delete this administrator?')) return false;"
                                                                CssClass="btn-action btn-action-delete" ToolTip="Delete Administrator">
                                                                <i class="fas fa-trash-alt"></i> Delete
                                                            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- 1. EDIT MODAL: MAIN QUALIFICATION -->
            <asp:Panel ID="pnlEditMainModal" runat="server" Visible="false">
                <div class="custom-modal-backdrop" tabindex="-1" role="dialog">
                    <div class="custom-modal-dialog" style="max-width: 520px;">
                        <div class="custom-modal-content">
                            <div class="modal-header bg-light py-3 px-4 d-flex justify-content-between align-items-center" style="border-bottom: 1.5px solid #e2e8f0;">
                                <h5 class="modal-title font-weight-bold text-gray-900 mb-0" style="font-size: 1.15rem;">
                                    <i class="fas fa-edit text-primary mr-2"></i> Edit Main Qualification
                                </h5>
                                <asp:LinkButton ID="btnCloseEditMainX" runat="server" CssClass="close text-muted" OnClick="btnCloseModals_Click" style="font-size: 1.5rem; line-height: 1;">
                                    <span aria-hidden="true">&times;</span>
                                </asp:LinkButton>
                            </div>
                            <div class="modal-body p-4">
                                <asp:HiddenField ID="hfEditMainCodeOrig" runat="server" />
                                <div class="form-group">
                                    <label class="form-label-custom">Qualification Code</label>
                                    <asp:TextBox ID="txtEditMainCode" runat="server" ReadOnly="true" CssClass="form-control form-control-custom bg-light font-weight-bold"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Qualification Name <span class="req">*</span></label>
                                    <asp:TextBox ID="txtEditMainName" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Category / Level <span class="req">*</span></label>
                                    <asp:DropDownList ID="ddlEditMainCategory" runat="server" CssClass="form-control form-control-custom">
                                        <asp:ListItem Value="Secondary School">Secondary School (10th)</asp:ListItem>
                                        <asp:ListItem Value="Higher Secondary">Higher Secondary (12th/PUC)</asp:ListItem>
                                        <asp:ListItem Value="Diploma">Diploma</asp:ListItem>
                                        <asp:ListItem Value="Undergraduate">Undergraduate Degree</asp:ListItem>
                                        <asp:ListItem Value="Postgraduate">Postgraduate Degree</asp:ListItem>
                                        <asp:ListItem Value="Doctorate / Research">Doctorate / Research</asp:ListItem>
                                        <asp:ListItem Value="Vocational / Trade">Vocational / ITI</asp:ListItem>
                                        <asp:ListItem Value="Other">Other</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="modal-footer bg-light py-3 px-4 d-flex justify-content-end" style="border-top: 1.5px solid #e2e8f0; gap: 8px;">
                                <asp:Button ID="btnCancelEditMain" runat="server" Text="Cancel" CssClass="btn btn-edu-outline" OnClick="btnCloseModals_Click" />
                                <asp:Button ID="btnSaveEditMain" runat="server" Text="Update Qualification" CssClass="btn btn-edu-primary font-weight-bold" OnClick="btnSaveEditMain_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- 2. EDIT MODAL: SUB QUALIFICATION -->
            <asp:Panel ID="pnlEditSubModal" runat="server" Visible="false">
                <div class="custom-modal-backdrop" tabindex="-1" role="dialog">
                    <div class="custom-modal-dialog" style="max-width: 520px;">
                        <div class="custom-modal-content">
                            <div class="modal-header bg-light py-3 px-4 d-flex justify-content-between align-items-center" style="border-bottom: 1.5px solid #e2e8f0;">
                                <h5 class="modal-title font-weight-bold text-gray-900 mb-0" style="font-size: 1.15rem;">
                                    <i class="fas fa-edit text-primary mr-2"></i> Edit Sub Qualification
                                </h5>
                                <asp:LinkButton ID="btnCloseEditSubX" runat="server" CssClass="close text-muted" OnClick="btnCloseModals_Click" style="font-size: 1.5rem; line-height: 1;">
                                    <span aria-hidden="true">&times;</span>
                                </asp:LinkButton>
                            </div>
                            <div class="modal-body p-4">
                                <asp:HiddenField ID="hfEditSubId" runat="server" />
                                <div class="form-group">
                                    <label class="form-label-custom">Link to Main Qualification</label>
                                    <asp:DropDownList ID="ddlEditSubParentMain" runat="server" CssClass="form-control form-control-custom">
                                    </asp:DropDownList>
                                    <small class="text-muted">Leave unselected to make available for all qualifications.</small>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Sub Code <span class="req">*</span></label>
                                    <asp:TextBox ID="txtEditSubCode" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Sub Qualification Name / Stream <span class="req">*</span></label>
                                    <asp:TextBox ID="txtEditSubName" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                            </div>
                            <div class="modal-footer bg-light py-3 px-4 d-flex justify-content-end" style="border-top: 1.5px solid #e2e8f0; gap: 8px;">
                                <asp:Button ID="btnCancelEditSub" runat="server" Text="Cancel" CssClass="btn btn-edu-outline" OnClick="btnCloseModals_Click" />
                                <asp:Button ID="btnSaveEditSub" runat="server" Text="Update Sub Qualification" CssClass="btn btn-edu-primary font-weight-bold" OnClick="btnSaveEditSub_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- 3. EDIT MODAL: UNIVERSITY -->
            <asp:Panel ID="pnlEditUnivModal" runat="server" Visible="false">
                <div class="custom-modal-backdrop" tabindex="-1" role="dialog">
                    <div class="custom-modal-dialog" style="max-width: 520px;">
                        <div class="custom-modal-content">
                            <div class="modal-header bg-light py-3 px-4 d-flex justify-content-between align-items-center" style="border-bottom: 1.5px solid #e2e8f0;">
                                <h5 class="modal-title font-weight-bold text-gray-900 mb-0" style="font-size: 1.15rem;">
                                    <i class="fas fa-edit text-primary mr-2"></i> Edit University / Board
                                </h5>
                                <asp:LinkButton ID="btnCloseEditUnivX" runat="server" CssClass="close text-muted" OnClick="btnCloseModals_Click" style="font-size: 1.5rem; line-height: 1;">
                                    <span aria-hidden="true">&times;</span>
                                </asp:LinkButton>
                            </div>
                            <div class="modal-body p-4">
                                <asp:HiddenField ID="hfEditUnivId" runat="server" />
                                <div class="form-group">
                                    <label class="form-label-custom">University ID</label>
                                    <asp:TextBox ID="txtEditUnivIdDisplay" runat="server" ReadOnly="true" CssClass="form-control form-control-custom bg-light font-weight-bold"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">University / Board Name <span class="req">*</span></label>
                                    <asp:TextBox ID="txtEditUnivName" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">State or Type</label>
                                    <asp:TextBox ID="txtEditUnivType" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                            </div>
                            <div class="modal-footer bg-light py-3 px-4 d-flex justify-content-end" style="border-top: 1.5px solid #e2e8f0; gap: 8px;">
                                <asp:Button ID="btnCancelEditUniv" runat="server" Text="Cancel" CssClass="btn btn-edu-outline" OnClick="btnCloseModals_Click" />
                                <asp:Button ID="btnSaveEditUniv" runat="server" Text="Update University" CssClass="btn btn-edu-primary font-weight-bold" OnClick="btnSaveEditUniv_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- 3b. EDIT MODAL: COLLEGE / SCHOOL -->
            <asp:Panel ID="pnlEditCollegeModal" runat="server" Visible="false">
                <div class="custom-modal-backdrop" tabindex="-1" role="dialog">
                    <div class="custom-modal-dialog" style="max-width: 520px;">
                        <div class="custom-modal-content">
                            <div class="modal-header bg-light py-3 px-4 d-flex justify-content-between align-items-center" style="border-bottom: 1.5px solid #e2e8f0;">
                                <h5 class="modal-title font-weight-bold text-gray-900 mb-0" style="font-size: 1.15rem;">
                                    <i class="fas fa-edit text-primary mr-2"></i> Edit College / School
                                </h5>
                                <asp:LinkButton ID="btnCloseEditCollegeX" runat="server" CssClass="close text-muted" OnClick="btnCloseModals_Click" style="font-size: 1.5rem; line-height: 1;">
                                    <span aria-hidden="true">&times;</span>
                                </asp:LinkButton>
                            </div>
                            <div class="modal-body p-4">
                                <asp:HiddenField ID="hfEditCollegeId" runat="server" />
                                <div class="form-group">
                                    <label class="form-label-custom">College ID</label>
                                    <asp:TextBox ID="txtEditCollegeIdDisplay" runat="server" ReadOnly="true" CssClass="form-control form-control-custom bg-light font-weight-bold"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">College / School Name <span class="req">*</span></label>
                                    <asp:TextBox ID="txtEditCollegeName" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Category / Type</label>
                                    <asp:DropDownList ID="ddlEditCollegeType" runat="server" CssClass="form-control form-control-custom">
                                        <asp:ListItem Value="Engineering College">Engineering College</asp:ListItem>
                                        <asp:ListItem Value="Degree / Autonomous College">Degree / Autonomous College</asp:ListItem>
                                        <asp:ListItem Value="PU College / Higher Secondary">PU College / Higher Secondary (11th/12th)</asp:ListItem>
                                        <asp:ListItem Value="High School / Secondary">High School / Secondary (10th)</asp:ListItem>
                                        <asp:ListItem Value="Polytechnic / Diploma">Polytechnic / Diploma</asp:ListItem>
                                        <asp:ListItem Value="Medical / Dental / Pharmacy">Medical / Dental / Pharmacy</asp:ListItem>
                                        <asp:ListItem Value="Vocational / ITI">Vocational / ITI</asp:ListItem>
                                        <asp:ListItem Value="Other">Other Educational Institute</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Location / City / State</label>
                                    <asp:TextBox ID="txtEditCollegeLocation" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Affiliated University / Board</label>
                                    <asp:TextBox ID="txtEditCollegeAffil" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                            </div>
                            <div class="modal-footer bg-light py-3 px-4 d-flex justify-content-end" style="border-top: 1.5px solid #e2e8f0; gap: 8px;">
                                <asp:Button ID="btnCancelEditCollege" runat="server" Text="Cancel" CssClass="btn btn-edu-outline" OnClick="btnCloseModals_Click" />
                                <asp:Button ID="btnSaveEditCollege" runat="server" Text="Update College / School" CssClass="btn btn-edu-primary font-weight-bold" OnClick="btnSaveEditCollege_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- 4. EDIT MODAL: SUBJECT -->
            <asp:Panel ID="pnlEditSubjModal" runat="server" Visible="false">
                <div class="custom-modal-backdrop" tabindex="-1" role="dialog">
                    <div class="custom-modal-dialog" style="max-width: 520px;">
                        <div class="custom-modal-content">
                            <div class="modal-header bg-light py-3 px-4 d-flex justify-content-between align-items-center" style="border-bottom: 1.5px solid #e2e8f0;">
                                <h5 class="modal-title font-weight-bold text-gray-900 mb-0" style="font-size: 1.15rem;">
                                    <i class="fas fa-edit text-primary mr-2"></i> Edit Subject / Discipline
                                </h5>
                                <asp:LinkButton ID="btnCloseEditSubjX" runat="server" CssClass="close text-muted" OnClick="btnCloseModals_Click" style="font-size: 1.5rem; line-height: 1;">
                                    <span aria-hidden="true">&times;</span>
                                </asp:LinkButton>
                            </div>
                            <div class="modal-body p-4">
                                <asp:HiddenField ID="hfEditSubjId" runat="server" />
                                <div class="form-group">
                                    <label class="form-label-custom">Subject ID</label>
                                    <asp:TextBox ID="txtEditSubjIdDisplay" runat="server" ReadOnly="true" CssClass="form-control form-control-custom bg-light font-weight-bold"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Subject / Trade Name <span class="req">*</span></label>
                                    <asp:TextBox ID="txtEditSubjName" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Domain Group</label>
                                    <asp:TextBox ID="txtEditSubjGroup" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                            </div>
                            <div class="modal-footer bg-light py-3 px-4 d-flex justify-content-end" style="border-top: 1.5px solid #e2e8f0; gap: 8px;">
                                <asp:Button ID="btnCancelEditSubj" runat="server" Text="Cancel" CssClass="btn btn-edu-outline" OnClick="btnCloseModals_Click" />
                                <asp:Button ID="btnSaveEditSubj" runat="server" Text="Update Subject" CssClass="btn btn-edu-primary font-weight-bold" OnClick="btnSaveEditSubj_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- 5. EDIT MODAL: ADMIN -->
            <asp:Panel ID="pnlEditAdminModal" runat="server" Visible="false">
                <div class="custom-modal-backdrop" tabindex="-1" role="dialog">
                    <div class="custom-modal-dialog" style="max-width: 520px;">
                        <div class="custom-modal-content">
                            <div class="modal-header bg-light py-3 px-4 d-flex justify-content-between align-items-center" style="border-bottom: 1.5px solid #e2e8f0;">
                                <h5 class="modal-title font-weight-bold text-gray-900 mb-0" style="font-size: 1.15rem;">
                                    <i class="fas fa-edit text-primary mr-2"></i> Edit Administrator
                                </h5>
                                <asp:LinkButton ID="btnCloseEditAdminX" runat="server" CssClass="close text-muted" OnClick="btnCloseModals_Click" style="font-size: 1.5rem; line-height: 1;">
                                    <span aria-hidden="true">&times;</span>
                                </asp:LinkButton>
                            </div>
                            <div class="modal-body p-4">
                                <asp:HiddenField ID="hfEditAdminPcnoOrig" runat="server" />
                                <div class="form-group">
                                    <label class="form-label-custom">Employee PCNO</label>
                                    <asp:TextBox ID="txtEditAdminPcnoDisplay" runat="server" ReadOnly="true" CssClass="form-control form-control-custom bg-light font-weight-bold"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Display Name <span class="req">*</span></label>
                                    <asp:TextBox ID="txtEditAdminName" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label-custom">Role</label>
                                    <asp:DropDownList ID="ddlEditAdminRole" runat="server" CssClass="form-control form-control-custom">
                                        <asp:ListItem Value="Admin">System Administrator</asp:ListItem>
                                        <asp:ListItem Value="Viewer">Audit Viewer</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="modal-footer bg-light py-3 px-4 d-flex justify-content-end" style="border-top: 1.5px solid #e2e8f0; gap: 8px;">
                                <asp:Button ID="btnCancelEditAdmin" runat="server" Text="Cancel" CssClass="btn btn-edu-outline" OnClick="btnCloseModals_Click" />
                                <asp:Button ID="btnSaveEditAdmin" runat="server" Text="Update Administrator" CssClass="btn btn-edu-primary font-weight-bold" OnClick="btnSaveEditAdmin_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function switchToTab(targetId) {
            if (!targetId || targetId.charAt(0) !== '#') return;

            // 1. Activate tab link
            $('#masterTabs .nav-link').removeClass('active');
            var $activeLink = $('#masterTabs .nav-link[href="' + targetId + '"], #masterTabs .nav-link[data-bs-target="' + targetId + '"]');
            if ($activeLink.length) {
                $activeLink.addClass('active');
            }

            // 2. Activate tab pane
            $('.tab-content > .tab-pane').removeClass('show active');
            var $targetPane = $(targetId);
            if ($targetPane.length) {
                $targetPane.addClass('show active');
            }

            // 3. Keep hidden field and URL history synchronized
            $('#<%= hfActiveTab.ClientID %>').val(targetId);
            if (window.history && window.history.replaceState) {
                window.history.replaceState(null, null, targetId);
            }
        }

        function filterTable(gridId, query) {
            var $table = $('#' + gridId);
            if (!$table.length) return;

            var $tbody = $table.find('tbody');
            if (!$tbody.length) $tbody = $table;

            var $rows = $tbody.find('> tr:not(.tab-search-no-results)');
            var cleanQuery = (query || '').toLowerCase().trim();

            $tbody.find('.tab-search-no-results').remove();

            var visibleCount = 0;
            $rows.each(function () {
                var $row = $(this);
                if ($row.find('th').length > 0) return; // Skip table header row

                var text = $row.text().toLowerCase();
                if (!cleanQuery || text.indexOf(cleanQuery) > -1) {
                    $row.show();
                    visibleCount++;
                } else {
                    $row.hide();
                }
            });

            if (visibleCount === 0 && cleanQuery) {
                var colCount = $table.find('tr:first > th, tr:first > td').length || 4;
                $tbody.append('<tr class="tab-search-no-results text-center"><td colspan="' + colCount + '" class="text-muted p-4"><i class="fas fa-search mr-1 text-primary"></i> No matching records found for "<strong>' + $('<div>').text(cleanQuery).html() + '</strong>"</td></tr>');
            }
        }

        function initMasterDataUI() {
            EduApp.makeSearchable('#<%= ddlMainCategory.ClientID %>', '-- Select Category / Level --');
            EduApp.makeSearchable('#<%= ddlSubParentMain.ClientID %>', '-- Link to Main Qualification --');
        }

        $(document).ready(function () {
            initMasterDataUI();

            // Direct delegated click handler for instant tab switching
            $(document).on('click', '#masterTabs a', function (e) {
                e.preventDefault();
                var target = $(this).attr('data-bs-target') || $(this).attr('href');
                switchToTab(target);
            });

            // Handle browser back/forward or manual hash change
            $(window).on('hashchange', function () {
                if (window.location.hash) {
                    switchToTab(window.location.hash);
                }
            });

            // Restore active tab on load
            var initialTab = window.location.hash || $('#<%= hfActiveTab.ClientID %>').val() || '#tab-main';
            switchToTab(initialTab);

            // Tab Search: click Search button
            $(document).on('click', '.btn-tab-search', function (e) {
                e.preventDefault();
                var gridId = $(this).attr('data-target-grid');
                var inputId = $(this).attr('data-input-id');
                var q = $('#' + inputId).val();
                filterTable(gridId, q);
            });

            // Tab Search: click Clear button
            $(document).on('click', '.btn-tab-search-clear', function (e) {
                e.preventDefault();
                var gridId = $(this).attr('data-target-grid');
                var inputId = $(this).attr('data-input-id');
                $('#' + inputId).val('');
                filterTable(gridId, '');
            });

            // Tab Search: press Enter in search input
            $(document).on('keydown', '.tab-search-input', function (e) {
                if (e.which === 13) {
                    e.preventDefault();
                    var gridId = $(this).attr('data-target-grid');
                    var q = $(this).val();
                    filterTable(gridId, q);
                }
            });

            // Tab Search: live filtering as user types
            $(document).on('input', '.tab-search-input', function () {
                var gridId = $(this).attr('data-target-grid');
                var q = $(this).val();
                filterTable(gridId, q);
            });
        });

        // Re-initialize searchable dropdowns and active tabs seamlessly after every partial AJAX postback
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                initMasterDataUI();
                var curTab = $('#<%= hfActiveTab.ClientID %>').val() || '#tab-main';
                switchToTab(curTab);

                // Re-apply any active searches after partial postback
                $('.tab-search-input').each(function () {
                    var q = $(this).val();
                    var gridId = $(this).attr('data-target-grid');
                    if (q) {
                        filterTable(gridId, q);
                    }
                });
            });
        }
    </script>
</asp:Content>
