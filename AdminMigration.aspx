<%@ Page Title="Historical Data Import" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminMigration.aspx.cs" Inherits="EducationApp.AdminMigration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .mig-stat-box {
            background: #ffffff;
            border: 1px solid var(--border-color);
            border-radius: 12px;
            padding: 1.25rem 1.5rem;
            text-align: center;
            box-shadow: var(--shadow-sm);
        }
        .mig-stat-box h2 {
            font-size: 2.2rem;
            font-weight: 800;
            margin: 0;
            line-height: 1.1;
        }
        .mig-stat-box p {
            font-size: 0.8rem;
            text-transform: uppercase;
            font-weight: 700;
            color: var(--text-muted);
            margin-top: 4px;
            margin-bottom: 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
            <h3 class="font-weight-bold text-gray-900 mb-1">
                <i class="fas fa-exchange-alt text-primary mr-2"></i> Historical Data Import
            </h3>
            <p class="text-muted small mb-0">
                Safely transfer historical employee academic records into the portal structure.
            </p>
        </div>
        <div>
            <a href="AdminDashboard.aspx" class="btn btn-edu-outline font-weight-bold">
                <i class="fas fa-arrow-left mr-1"></i> Back to Dashboard
            </a>
        </div>
    </div>

    <!-- Safety Notice -->
    <div class="alert alert-info py-3 mb-4" style="border-radius: 10px;">
        <div class="d-flex align-items-center">
            <i class="fas fa-shield-alt fa-2x mr-3 text-info"></i>
            <div>
                <strong>Safe Transfer Guarantee:</strong>
                <div class="small">
                    The import process is strictly non-destructive.
                    It will <strong>never modify, overwrite, or delete</strong> any existing historical records.
                    Only records whose employee PCNO exists in <code>hrdata.empdetails</code> will be imported.
                </div>
            </div>
        </div>
    </div>

    <!-- Migration KPI Stats -->
    <div class="row mb-4">
        <div class="col-md-3 mb-3">
            <div class="mig-stat-box">
                <h2 class="text-primary"><asp:Literal ID="litLegacyTotal" runat="server">0</asp:Literal></h2>
                <p>Total Historical Records</p>
            </div>
        </div>
        <div class="col-md-3 mb-3">
            <div class="mig-stat-box">
                <h2 class="text-success"><asp:Literal ID="litMigratedTotal" runat="server">0</asp:Literal></h2>
                <p>Successfully Migrated</p>
            </div>
        </div>
        <div class="col-md-3 mb-3">
            <div class="mig-stat-box">
                <h2 class="text-warning"><asp:Literal ID="litPendingTotal" runat="server">0</asp:Literal></h2>
                <p>Pending Migration</p>
            </div>
        </div>
        <div class="col-md-3 mb-3">
            <div class="mig-stat-box">
                <h2 class="text-secondary"><asp:Literal ID="litEmployeesTotal" runat="server">0</asp:Literal></h2>
                <p>Distinct Employees</p>
            </div>
        </div>
    </div>

    <!-- Execute Migration Panel -->
    <div class="edu-card mb-4">
        <div class="edu-card-header">
            <div class="edu-card-title">
                <i class="fas fa-play-circle"></i> Execute Migration Workflow
            </div>
        </div>
        <div class="edu-card-body">
            <div class="row align-items-center">
                <div class="col-md-5 mb-3 mb-md-0">
                    <label class="form-label-custom">Default Record Status upon Migration</label>
                    <asp:DropDownList ID="ddlMigStatus" runat="server" CssClass="form-control form-control-custom searchable-select">
                        <asp:ListItem Value="Submitted" Selected="True">Submitted (Locked - Official Record)</asp:ListItem>
                        <asp:ListItem Value="Draft">Draft (Editable by Employee)</asp:ListItem>
                    </asp:DropDownList>
                    <small class="text-muted">Determines whether migrated records are immediately locked or left in draft for employee review.</small>
                </div>
                <div class="col-md-4 mb-3 mb-md-0">
                    <div class="small font-weight-bold text-gray-800 mb-1">Mapping Engine Status</div>
                    <div class="badge badge-success p-2"><i class="fas fa-check-circle mr-1"></i> Configurable Mappings Ready (e.g. SSLC -> 10th MTRI)</div>
                </div>
                <div class="col-md-3 text-md-right">
                    <asp:Button ID="btnRunMigration" runat="server" Text="Execute Import Now" CssClass="btn btn-edu-primary btn-block font-weight-bold py-2" OnClick="btnRunMigration_Click"
                        data-confirm="Start importing historical qualification records into the system? This will migrate existing employee educational records into the portal database."
                        data-confirm-title="Execute Data Import"
                        data-confirm-type="warning"
                        data-confirm-btn="Yes, Execute Import" />
                </div>
            </div>

            <asp:Panel ID="pnlMigResult" runat="server" Visible="false" CssClass="mt-3 alert alert-success">
                <i class="fas fa-info-circle mr-1"></i>
                <asp:Literal ID="litMigResult" runat="server"></asp:Literal>
            </asp:Panel>
        </div>
    </div>

    <!-- Configurable Qualification Mapping Table -->
    <div class="edu-card mb-4">
        <div class="edu-card-header d-flex justify-content-between align-items-center">
            <div class="edu-card-title">
                <i class="fas fa-map-signs"></i> Qualification Mapping Matrix
            </div>
            <div class="small text-muted">
                Configured: <asp:Literal ID="litMappingCount" runat="server">0</asp:Literal> mappings
            </div>
        </div>
        <div class="edu-card-body p-0">
            <div class="table-responsive" style="max-height: 400px; overflow-y: auto;">
                <asp:GridView ID="gvMappings" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                    OnRowCommand="gvMappings_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="OLD_QFN_ID" HeaderText="Old QFNID" ItemStyle-Width="90px" ItemStyle-Font-Bold="true" />
                        <asp:BoundField DataField="OLD_QFN_NAME" HeaderText="Legacy Qualification Name" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="OLD_QFN_GROUP" HeaderText="Legacy Group" ItemStyle-Width="120px" />
                        
                        <asp:TemplateField HeaderText="Target Main Qualification">
                            <ItemTemplate>
                                <span class="badge badge-primary"><%# Eval("NEW_QUAL_MAIN_CODE") %></span>
                                <span class="ml-1 font-weight-bold"><%# Eval("NEW_QUAL_NAME") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="NEW_QUAL_SUB_CODE" HeaderText="Target Sub Code" ItemStyle-Width="130px" />
                        <asp:BoundField DataField="NOTES" HeaderText="Mapping Rationale" />

                        <asp:TemplateField HeaderText="Edit Mapping" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEditMap" runat="server" CommandName="EditMapping" CommandArgument='<%# Eval("MAPPING_ID") %>' CssClass="btn btn-sm btn-outline-primary" ToolTip="Modify Mapping">
                                    <i class="fas fa-pencil-alt"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Edit Mapping Modal -->
    <asp:Panel ID="pnlEditMapModal" runat="server" Visible="false">
        <div class="modal fade show" style="display: block; background: rgba(15, 23, 42, 0.6);" tabindex="-1" role="dialog">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content" style="border-radius: 12px; border: none;">
                    <div class="modal-header bg-light">
                        <h5 class="modal-title font-weight-bold text-gray-900">
                            <i class="fas fa-edit text-primary mr-1"></i> Edit Qualification Mapping
                        </h5>
                        <asp:LinkButton ID="btnCloseMapModalX" runat="server" CssClass="close" OnClick="btnCloseMapModal_Click">
                            <span aria-hidden="true">&times;</span>
                        </asp:LinkButton>
                    </div>
                    <div class="modal-body p-4">
                        <asp:HiddenField ID="hfEditMapId" runat="server" Value="0" />
                        <div class="form-group">
                            <label class="form-label-custom">Legacy Qualification</label>
                            <asp:TextBox ID="txtEditMapOldName" runat="server" ReadOnly="true" CssClass="form-control form-control-custom bg-light"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label-custom">Target Main Qualification <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlEditMapMainQual" runat="server" CssClass="form-control form-control-custom searchable-select">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label class="form-label-custom">Target Sub Code</label>
                            <asp:TextBox ID="txtEditMapSubCode" runat="server" CssClass="form-control form-control-custom" placeholder="e.g. GENERAL, COMPS, NA"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label-custom">Notes / Description</label>
                            <asp:TextBox ID="txtEditMapNotes" runat="server" CssClass="form-control form-control-custom"></asp:TextBox>
                        </div>
                    </div>
                    <div class="modal-footer bg-light">
                        <asp:Button ID="btnCancelMapEdit" runat="server" Text="Cancel" CssClass="btn btn-edu-outline" OnClick="btnCloseMapModal_Click" />
                        <asp:Button ID="btnSaveMapEdit" runat="server" Text="Update Mapping" CssClass="btn btn-edu-primary font-weight-bold" OnClick="btnSaveMapEdit_Click" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Migration Audit Log Table -->
    <div class="edu-card">
        <div class="edu-card-header">
            <div class="edu-card-title">
                <i class="fas fa-history"></i> Execution History & Migration Logs
            </div>
        </div>
        <div class="edu-card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvMigrationLogs" runat="server" AutoGenerateColumns="false" CssClass="edu-table" GridLines="None"
                    EmptyDataText="No migration executions recorded yet.">
                    <Columns>
                        <asp:BoundField DataField="LOG_ID" HeaderText="Log ID" ItemStyle-Width="70px" />
                        <asp:BoundField DataField="MIGRATION_DATE" HeaderText="Date & Time" ItemStyle-Width="160px" />
                        <asp:BoundField DataField="EXECUTED_BY" HeaderText="Executed By" ItemStyle-Width="120px" />
                        <asp:BoundField DataField="TOTAL_READ" HeaderText="Read" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="TOTAL_MIGRATED" HeaderText="Migrated" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="TOTAL_SKIPPED" HeaderText="Skipped" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" />
                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <span class='badge <%# Eval("STATUS").ToString() == "COMPLETED" ? "badge-success" : "badge-danger" %>'>
                                    <%# Eval("STATUS") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="LOG_DETAILS" HeaderText="Details" />
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
            EduApp.makeSearchable('#<%= ddlMigStatus.ClientID %>', 'Default Status');
            if ($('#<%= ddlEditMapMainQual.ClientID %>').length > 0) {
                EduApp.makeSearchable('#<%= ddlEditMapMainQual.ClientID %>', '-- Search Target Qualification --');
            }
        });
    </script>
</asp:Content>
