namespace EducationApp {
    
    public partial class AdminDashboard {
        
        protected global::System.Web.UI.ScriptManager smAdmin;
        protected global::System.Web.UI.UpdatePanel upAdminDashboard;
        protected global::System.Web.UI.WebControls.Button btnExportCsv;
        protected global::System.Web.UI.WebControls.Literal litStatEmployees;
        protected global::System.Web.UI.WebControls.Literal litStatSubmitted;
        protected global::System.Web.UI.WebControls.Literal litStatDraft;
        protected global::System.Web.UI.WebControls.Literal litStatMigrated;
        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.DropDownList ddlStatusFilter;
        protected global::System.Web.UI.WebControls.DropDownList ddlQualFilter;
        protected global::System.Web.UI.WebControls.DropDownList ddlHighestFilter;
        protected global::System.Web.UI.WebControls.Button btnFilter;
        protected global::System.Web.UI.WebControls.Button btnResetFilter;
        protected global::System.Web.UI.WebControls.Literal litFilteredCount;
        protected global::System.Web.UI.WebControls.Literal litPagingSummary;
        protected global::System.Web.UI.WebControls.DropDownList ddlPageSize;
        protected global::System.Web.UI.WebControls.GridView gvAdminEducation;

        // In-Page Admin Edit Modal Controls
        protected global::System.Web.UI.WebControls.HiddenField hfModalOpen;
        protected global::System.Web.UI.WebControls.Panel pnlModal;
        protected global::System.Web.UI.WebControls.Literal litEditEmpName;
        protected global::System.Web.UI.WebControls.Literal litEditEmpPCNO;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseModalX;
        protected global::System.Web.UI.WebControls.HiddenField hfEditEduId;
        protected global::System.Web.UI.WebControls.HiddenField hfEditPCNO;
        protected global::System.Web.UI.WebControls.HiddenField hfEditEmpName;
        protected global::System.Web.UI.WebControls.HiddenField hfEditDesig;
        protected global::System.Web.UI.WebControls.HiddenField hfEditDiv;
        protected global::System.Web.UI.WebControls.HiddenField hfSelectedMainQual;
        protected global::System.Web.UI.WebControls.HiddenField hfSelectedSubQual;
        protected global::System.Web.UI.WebControls.HiddenField hfSelectedSubQualName;
        protected global::System.Web.UI.WebControls.Literal litEditBannerEmp;
        protected global::System.Web.UI.WebControls.Literal litEditRecordId;
        protected global::System.Web.UI.WebControls.Panel pnlFormError;
        protected global::System.Web.UI.WebControls.Literal litFormError;
        protected global::System.Web.UI.WebControls.DropDownList ddlMainQual;
        protected global::System.Web.UI.WebControls.DropDownList ddlSubQual;
        protected global::System.Web.UI.WebControls.DropDownList ddlYearPassed;
        protected global::System.Web.UI.WebControls.TextBox txtSpecialization;
        protected global::System.Web.UI.WebControls.DropDownList ddlUniversity;
        protected global::System.Web.UI.WebControls.DropDownList ddlCollege;
        protected global::System.Web.UI.WebControls.Panel pnlCustomCollege;
        protected global::System.Web.UI.WebControls.TextBox txtCustomCollege;
        protected global::System.Web.UI.WebControls.DropDownList ddlScoreType;
        protected global::System.Web.UI.WebControls.TextBox txtCgpaMarks;
        protected global::System.Web.UI.WebControls.DropDownList ddlGrade;
        protected global::System.Web.UI.WebControls.DropDownList ddlRecordStatus;
        protected global::System.Web.UI.WebControls.CheckBox chkIsHighest;
        protected global::System.Web.UI.WebControls.TextBox txtRemarks;
        protected global::System.Web.UI.WebControls.Button btnCancelModal;
        protected global::System.Web.UI.WebControls.Button btnSaveAdminEdit;
    }
}
