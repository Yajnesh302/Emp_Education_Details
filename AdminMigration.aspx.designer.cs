namespace EducationApp {
    
    public partial class AdminMigration {
        
        protected global::System.Web.UI.WebControls.Literal litLegacyTotal;
        protected global::System.Web.UI.WebControls.Literal litMigratedTotal;
        protected global::System.Web.UI.WebControls.Literal litPendingTotal;
        protected global::System.Web.UI.WebControls.Literal litEmployeesTotal;
        protected global::System.Web.UI.WebControls.DropDownList ddlMigStatus;
        protected global::System.Web.UI.WebControls.Button btnRunMigration;
        protected global::System.Web.UI.WebControls.Panel pnlMigResult;
        protected global::System.Web.UI.WebControls.Literal litMigResult;
        protected global::System.Web.UI.WebControls.Literal litMappingCount;
        protected global::System.Web.UI.WebControls.GridView gvMappings;
        protected global::System.Web.UI.WebControls.Panel pnlEditMapModal;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseMapModalX;
        protected global::System.Web.UI.WebControls.HiddenField hfEditMapId;
        protected global::System.Web.UI.WebControls.TextBox txtEditMapOldName;
        protected global::System.Web.UI.WebControls.DropDownList ddlEditMapMainQual;
        protected global::System.Web.UI.WebControls.TextBox txtEditMapSubCode;
        protected global::System.Web.UI.WebControls.TextBox txtEditMapNotes;
        protected global::System.Web.UI.WebControls.Button btnCancelMapEdit;
        protected global::System.Web.UI.WebControls.Button btnSaveMapEdit;
        protected global::System.Web.UI.WebControls.GridView gvMigrationLogs;
    }
}
