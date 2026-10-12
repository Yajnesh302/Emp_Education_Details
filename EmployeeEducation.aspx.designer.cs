namespace EducationApp {
    
    public partial class EmployeeEducation {
        
        protected global::System.Web.UI.WebControls.Image imgEmpPhoto;
        protected global::System.Web.UI.WebControls.Literal litEmpPCNO;
        protected global::System.Web.UI.WebControls.Literal litEmpName;
        protected global::System.Web.UI.WebControls.Literal litEmpDesignation;
        protected global::System.Web.UI.WebControls.Literal litEmpDivision;
        protected global::System.Web.UI.WebControls.Literal litSubmissionStatusPill;
        protected global::System.Web.UI.WebControls.Literal litStatQualCount;
        protected global::System.Web.UI.WebControls.Literal litStatHighestQual;
        protected global::System.Web.UI.WebControls.Button btnSidebarAdd;
        protected global::System.Web.UI.WebControls.PlaceHolder phSubmittedNotice;
        protected global::System.Web.UI.WebControls.PlaceHolder phDraftNotice;
        protected global::System.Web.UI.WebControls.PlaceHolder phDraftIncompleteNotice;
        protected global::System.Web.UI.WebControls.Literal litDraftIncompleteCount;
        protected global::System.Web.UI.WebControls.Literal litDraftIncompleteDetails;
        protected global::System.Web.UI.WebControls.Button btnFinalSubmitBanner;
        protected global::System.Web.UI.WebControls.Button btnOpenAddModal;
        protected global::System.Web.UI.WebControls.GridView gvEducation;
        protected global::System.Web.UI.WebControls.PlaceHolder phBottomSubmitBtn;
        protected global::System.Web.UI.WebControls.Button btnBottomFinalSubmit;
        protected global::System.Web.UI.WebControls.Panel pnlModal;
        protected global::System.Web.UI.WebControls.Literal litModalTitle;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseModalX;
        protected global::System.Web.UI.ScriptManager smEducation;
        protected global::System.Web.UI.UpdatePanel upEducation;
        protected global::System.Web.UI.WebControls.HiddenField hfSelectedMainQual;
        protected global::System.Web.UI.WebControls.HiddenField hfSelectedSubQual;
        protected global::System.Web.UI.WebControls.HiddenField hfEditEduId;
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
        protected global::System.Web.UI.WebControls.CheckBox chkIsHighest;
        protected global::System.Web.UI.WebControls.TextBox txtRemarks;
        protected global::System.Web.UI.WebControls.Button btnCancelModal;
        protected global::System.Web.UI.WebControls.Button btnSaveDraft;
        protected global::System.Web.UI.WebControls.Button btnSaveAndSubmit;
    }
}
