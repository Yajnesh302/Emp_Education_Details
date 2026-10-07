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
        }
        .nav-tabs-custom .nav-link.active {
            color: var(--primary);
            background: #ffffff;
            border-bottom: 3px solid var(--primary);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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

    <!-- Active Tab State Preservation -->
    <asp:HiddenField ID="hfActiveTab" runat="server" Value="#tab-main" />

    <!-- Master Tabs -->
    <ul class="nav nav-tabs nav-tabs-custom mb-4" id="masterTabs" role="tablist">
        <li class="nav-item">
            <a class="nav-link active" id="tab-main-tab" data-toggle="tab" href="#tab-main" role="tab">
                <i class="fas fa-award mr-1"></i> Main Qualifications
            </a>
        </li>
        <li class="nav-item">
            <a class="nav-link" id="tab-sub-tab" data-toggle="tab" href="#tab-sub" role="tab">
                <i class="fas fa-code-branch mr-1"></i> Sub Qualifications
            </a>
        </li>
        <li class="nav-item">
            <a class="nav-link" id="tab-univ-tab" data-toggle="tab" href="#tab-univ" role="tab">
                <i class="fas fa-university mr-1"></i> Universities & Boards
            </a>
        </li>
        <li class="nav-item">
            <a class="nav-link" id="tab-subj-tab" data-toggle="tab" href="#tab-subj" role="tab">
                <i class="fas fa-book mr-1"></i> Subjects / Trades
            </a>
        </li>
        <li class="nav-item">
            <a class="nav-link" id="tab-admins-tab" data-toggle="tab" href="#tab-admins" role="tab">
                <i class="fas fa-user-shield mr-1"></i> Admin Privileges
            </a>
        </li>
    </ul>

    <div class="tab-content">
        <!-- 1. MAIN QUALIFICATIONS -->
        <div class="tab-pane fade show active" id="tab-main" role="tabpanel">
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
                        <div class="edu-card-header">
                            <div class="edu-card-title"><i class="fas fa-list"></i> Catalog of Main Qualifications</div>
                        </div>
                        <div class="edu-card-body p-0">
                            <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                <asp:GridView ID="gvMainQuals" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None">
                                    <Columns>
                                        <asp:BoundField DataField="QUAL_CODE" HeaderText="Code" ItemStyle-Width="100px" ItemStyle-Font-Bold="true" />
                                        <asp:BoundField DataField="QUAL_NAME" HeaderText="Qualification Name" />
                                        <asp:BoundField DataField="QUAL_CATEGORY" HeaderText="Category" ItemStyle-Width="180px" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 2. SUB QUALIFICATIONS -->
        <div class="tab-pane fade" id="tab-sub" role="tabpanel">
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
                        <div class="edu-card-header">
                            <div class="edu-card-title"><i class="fas fa-list"></i> Sub Qualifications / Streams</div>
                        </div>
                        <div class="edu-card-body p-0">
                            <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                <asp:GridView ID="gvSubQuals" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None">
                                    <Columns>
                                        <asp:BoundField DataField="SUB_CODE" HeaderText="Code" ItemStyle-Width="100px" ItemStyle-Font-Bold="true" />
                                        <asp:BoundField DataField="SUB_NAME" HeaderText="Sub Qualification / Stream Name" />
                                        <asp:BoundField DataField="MAIN_QUAL_CODE" HeaderText="Parent Qual" ItemStyle-Width="120px" NullDisplayText="All" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 3. UNIVERSITIES -->
        <div class="tab-pane fade" id="tab-univ" role="tabpanel">
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
                        <div class="edu-card-header">
                            <div class="edu-card-title"><i class="fas fa-list"></i> Recognized Universities & Institutes</div>
                        </div>
                        <div class="edu-card-body p-0">
                            <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                <asp:GridView ID="gvUniversities" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None">
                                    <Columns>
                                        <asp:BoundField DataField="UNIV_ID" HeaderText="ID" ItemStyle-Width="70px" ItemStyle-Font-Bold="true" />
                                        <asp:BoundField DataField="UNIV_NAME" HeaderText="University / Institution Name" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 4. SUBJECTS -->
        <div class="tab-pane fade" id="tab-subj" role="tabpanel">
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
                        <div class="edu-card-header">
                            <div class="edu-card-title"><i class="fas fa-list"></i> Subjects & Disciplines</div>
                        </div>
                        <div class="edu-card-body p-0">
                            <div class="table-responsive" style="max-height: 520px; overflow-y: auto;">
                                <asp:GridView ID="gvSubjects" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None">
                                    <Columns>
                                        <asp:BoundField DataField="SUBJECT_ID" HeaderText="ID" ItemStyle-Width="70px" ItemStyle-Font-Bold="true" />
                                        <asp:BoundField DataField="SUBJECT_NAME" HeaderText="Subject / Trade Name" />
                                        <asp:BoundField DataField="SUBJECT_GROUP" HeaderText="Domain Group" ItemStyle-Width="180px" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 5. ADMIN PRIVILEGES -->
        <div class="tab-pane fade" id="tab-admins" role="tabpanel">
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
                        <div class="edu-card-header">
                            <div class="edu-card-title"><i class="fas fa-users-cog"></i> Authorized Administrators</div>
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
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="130px" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnToggleAdmin" runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("PCNO") %>'
                                                    CssClass='<%# Convert.ToInt32(Eval("IS_ACTIVE")) == 1 ? "btn btn-sm btn-outline-danger" : "btn btn-sm btn-outline-success" %>'>
                                                    <%# Convert.ToInt32(Eval("IS_ACTIVE")) == 1 ? "Revoke" : "Activate" %>
                                                </asp:LinkButton>
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            EduApp.makeSearchable('#<%= ddlMainCategory.ClientID %>', '-- Select Category / Level --');
            EduApp.makeSearchable('#<%= ddlSubParentMain.ClientID %>', '-- Link to Main Qualification --');

            function activateTab(tabId) {
                if (tabId) {
                    var $tabLink = $('a[data-toggle="tab"][href="' + tabId + '"]');
                    if ($tabLink.length) {
                        $tabLink.tab('show');
                    }
                }
            }

            // Restore tab from URL hash (e.g. #tab-admins) or hidden field
            var currentHash = window.location.hash;
            var savedTab = $('#<%= hfActiveTab.ClientID %>').val();
            if (currentHash) {
                activateTab(currentHash);
            } else if (savedTab) {
                activateTab(savedTab);
            }

            // Respond to browser back/forward or manual hash change
            $(window).on('hashchange', function () {
                if (window.location.hash) {
                    activateTab(window.location.hash);
                }
            });

            // Update URL hash and hidden field whenever a tab is clicked
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var tabHref = $(e.target).attr('href');
                $('#<%= hfActiveTab.ClientID %>').val(tabHref);
                if (window.history && window.history.replaceState) {
                    window.history.replaceState(null, null, tabHref);
                } else {
                    window.location.hash = tabHref;
                }
            });
        });
    </script>
</asp:Content>
