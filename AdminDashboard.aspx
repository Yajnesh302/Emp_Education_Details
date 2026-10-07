<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="EducationApp.AdminDashboard" %>

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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page Title -->
    <div class="d-flex flex-wrap justify-content-between align-items-center mb-4">
        <div>
            <h3 class="font-weight-bold text-gray-900 mb-1">
                <i class="fas fa-chart-line text-primary mr-2"></i> Employee Education Administration
            </h3>
            <p class="text-muted small mb-0">Overview of employee academic profiles, record statuses, and unlock management</p>
        </div>
        <div class="mt-2 mt-md-0">
            <asp:Button ID="btnExportCsv" runat="server" Text="Export to Excel (CSV)" CssClass="btn btn-edu-outline font-weight-bold mr-2" OnClick="btnExportCsv_Click" />
            <a href="AdminMasterData.aspx" class="btn btn-edu-primary font-weight-bold">
                <i class="fas fa-layer-group mr-1"></i> Master Catalog
            </a>
        </div>
    </div>

    <!-- KPI Metric Cards -->
    <div class="row mb-4">
        <div class="col-xl-3 col-md-6 mb-3">
            <div class="stat-card">
                <div class="stat-icon" style="background: #e0f2fe; color: #0284c7;">
                    <i class="fas fa-users"></i>
                </div>
                <div class="stat-content">
                    <p>Total Employees</p>
                    <h3><asp:Literal ID="litStatEmployees" runat="server">0</asp:Literal></h3>
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
                    <h3><asp:Literal ID="litStatSubmitted" runat="server">0</asp:Literal></h3>
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
                    <h3><asp:Literal ID="litStatDraft" runat="server">0</asp:Literal></h3>
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
                    <h3><asp:Literal ID="litStatMigrated" runat="server">0</asp:Literal></h3>
                </div>
            </div>
        </div>
    </div>

    <!-- Filter & Search Bar -->
    <div class="filter-panel">
        <div class="row align-items-end">
            <div class="col-md-4 form-group mb-md-0">
                <label class="form-label-custom">Search by PCNO, Name, Institute</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control form-control-custom" placeholder="Search keyword..."></asp:TextBox>
            </div>

            <div class="col-md-3 form-group mb-md-0">
                <label class="form-label-custom">Status Filter</label>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control form-control-custom searchable-select">
                    <asp:ListItem Value="ALL">All Statuses</asp:ListItem>
                    <asp:ListItem Value="Submitted">Submitted (Locked)</asp:ListItem>
                    <asp:ListItem Value="Draft">Draft (Editable)</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="col-md-3 form-group mb-md-0">
                <label class="form-label-custom">Main Qualification</label>
                <asp:DropDownList ID="ddlQualFilter" runat="server" CssClass="form-control form-control-custom searchable-select">
                </asp:DropDownList>
            </div>

            <div class="col-md-2 form-group mb-md-0">
                <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn btn-edu-primary btn-block font-weight-bold" OnClick="btnFilter_Click" />
            </div>
        </div>
    </div>

    <!-- Employee Education Master Table -->
    <div class="edu-card shadow-sm mb-4">
        <div class="edu-card-header d-flex justify-content-between align-items-center px-4 py-3" style="padding-left: 2rem !important; padding-right: 2rem !important;">
            <div class="edu-card-title mb-0">
                <i class="fas fa-table text-primary mr-2"></i> All Employee Education Records (<asp:Literal ID="litFilteredCount" runat="server">0</asp:Literal>)
            </div>
        </div>
        <div class="edu-card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvAdminEducation" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                    UseAccessibleHeader="true"
                    OnRowCommand="gvAdminEducation_RowCommand" EmptyDataText="No records found matching current search criteria.">
                    <Columns>
                        <asp:BoundField DataField="PCNO" HeaderText="PCNO" ItemStyle-Width="80px" ItemStyle-Font-Bold="true" />

                        <asp:TemplateField HeaderText="Employee Name & Dept">
                            <ItemTemplate>
                                <div class="font-weight-bold text-gray-900"><%# Eval("EMP_NAME") %></div>
                                <div class="small text-muted"><%# Eval("DESIGNATION") %> &bull; <%# Eval("DIVISION_NAME") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Qualification" ItemStyle-Width="240px">
                            <ItemTemplate>
                                <div class="font-weight-bold text-gray-900"><%# Eval("QUAL_MAIN_NAME") %></div>
                                <div class="mt-1 d-inline-flex flex-wrap align-items-center" style="gap: 6px;">
                                    <span class="badge badge-light border text-muted px-2 py-0" style="font-size: 0.75rem;"><%# Eval("QUAL_MAIN_CODE") %></span>
                                    <%# Convert.ToInt32(Eval("IS_HIGHEST")) == 1 ? "<span class='badge-highest-tick' title='Designated Highest Qualification'><i class='fas fa-check mr-1'></i>Highest</span>" : "" %>
                                </div>
                                <div class="small text-muted mt-1"><%# Eval("QUAL_SUB_NAME") %> &bull; <%# Eval("SPECIALIZATION") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="YEAR_PASSED" HeaderText="Year" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" />

                        <asp:TemplateField HeaderText="Institute / University">
                            <ItemTemplate>
                                <div class="font-weight-bold small"><%# Eval("INSTITUTE_NAME") %></div>
                                <div class="small text-muted"><%# Eval("COLLEGE_NAME") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Performance" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <div class="small"><strong><%# Eval("CGPA_MARKS") %></strong></div>
                                <div class="small text-muted"><%# Eval("GRADE_DIVISION") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="110px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%# Eval("STATUS").ToString() == "Submitted" 
                                    ? "<span class='badge-status badge-submitted'><i class='fas fa-lock mr-1'></i> Submitted</span>" 
                                    : "<span class='badge-status badge-draft'><i class='fas fa-pencil-alt mr-1'></i> Draft</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Admin Actions" ItemStyle-Width="195px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="phRevert" runat="server" Visible='<%# Eval("STATUS").ToString() == "Submitted" %>'>
                                    <asp:LinkButton ID="btnRevertDraft" runat="server" CommandName="RevertToDraft" CommandArgument='<%# Eval("EDU_ID") %>' CssClass="btn btn-sm btn-outline-warning font-weight-bold" ToolTip="Unlock record and revert to Draft" OnClientClick="return confirm('Unlock this education record and revert to Draft mode? The employee will be able to edit it.');">
                                        <i class="fas fa-unlock mr-1"></i> Revert to Draft
                                    </asp:LinkButton>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phAlreadyDraft" runat="server" Visible='<%# Eval("STATUS").ToString() == "Draft" %>'>
                                    <asp:LinkButton ID="btnSubmitDraft" runat="server" CommandName="SubmitToFinal" CommandArgument='<%# Eval("EDU_ID") %>' CssClass="btn btn-sm btn-outline-success font-weight-bold" ToolTip="Mark record as Submitted and lock" OnClientClick="return confirm('Change this draft record to Submitted mode and lock it?');">
                                        <i class="fas fa-lock mr-1"></i> Mark as Submitted
                                    </asp:LinkButton>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle CssClass="text-center p-4 text-muted font-italic" />
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            EduApp.makeSearchable('#<%= ddlStatusFilter.ClientID %>', 'All Statuses');
            EduApp.makeSearchable('#<%= ddlQualFilter.ClientID %>', 'All Qualifications');
        });
    </script>
</asp:Content>
