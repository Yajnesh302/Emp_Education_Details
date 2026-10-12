namespace EducationApp {
    
    public partial class AdminMasterData {
        
        protected global::System.Web.UI.ScriptManager smMasterData;
        protected global::System.Web.UI.UpdatePanel upMasterData;
        protected global::System.Web.UI.WebControls.HiddenField hfActiveTab;
        protected global::System.Web.UI.WebControls.TextBox txtMainCode;
        protected global::System.Web.UI.WebControls.TextBox txtMainName;
        protected global::System.Web.UI.WebControls.DropDownList ddlMainCategory;
        protected global::System.Web.UI.WebControls.Button btnAddMainQual;
        protected global::System.Web.UI.WebControls.GridView gvMainQuals;
        protected global::System.Web.UI.WebControls.DropDownList ddlSubParentMain;
        protected global::System.Web.UI.WebControls.TextBox txtSubCode;
        protected global::System.Web.UI.WebControls.TextBox txtSubName;
        protected global::System.Web.UI.WebControls.Button btnAddSubQual;
        protected global::System.Web.UI.WebControls.GridView gvSubQuals;
        protected global::System.Web.UI.WebControls.TextBox txtUnivName;
        protected global::System.Web.UI.WebControls.TextBox txtUnivType;
        protected global::System.Web.UI.WebControls.Button btnAddUniv;
        protected global::System.Web.UI.WebControls.GridView gvUniversities;
        protected global::System.Web.UI.WebControls.TextBox txtCollegeNameAdd;
        protected global::System.Web.UI.WebControls.DropDownList ddlCollegeTypeAdd;
        protected global::System.Web.UI.WebControls.TextBox txtCollegeLocationAdd;
        protected global::System.Web.UI.WebControls.TextBox txtCollegeAffilAdd;
        protected global::System.Web.UI.WebControls.Button btnAddCollege;
        protected global::System.Web.UI.WebControls.GridView gvColleges;
        protected global::System.Web.UI.WebControls.TextBox txtSubjName;
        protected global::System.Web.UI.WebControls.TextBox txtSubjGroup;
        protected global::System.Web.UI.WebControls.Button btnAddSubj;
        protected global::System.Web.UI.WebControls.GridView gvSubjects;
        protected global::System.Web.UI.WebControls.TextBox txtAdminPCNO;
        protected global::System.Web.UI.WebControls.TextBox txtAdminName;
        protected global::System.Web.UI.WebControls.Button btnAddAdmin;
        protected global::System.Web.UI.WebControls.GridView gvAdmins;

        // Edit Modals
        protected global::System.Web.UI.WebControls.Panel pnlEditMainModal;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseEditMainX;
        protected global::System.Web.UI.WebControls.HiddenField hfEditMainCodeOrig;
        protected global::System.Web.UI.WebControls.TextBox txtEditMainCode;
        protected global::System.Web.UI.WebControls.TextBox txtEditMainName;
        protected global::System.Web.UI.WebControls.DropDownList ddlEditMainCategory;
        protected global::System.Web.UI.WebControls.Button btnCancelEditMain;
        protected global::System.Web.UI.WebControls.Button btnSaveEditMain;

        protected global::System.Web.UI.WebControls.Panel pnlEditSubModal;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseEditSubX;
        protected global::System.Web.UI.WebControls.HiddenField hfEditSubId;
        protected global::System.Web.UI.WebControls.DropDownList ddlEditSubParentMain;
        protected global::System.Web.UI.WebControls.TextBox txtEditSubCode;
        protected global::System.Web.UI.WebControls.TextBox txtEditSubName;
        protected global::System.Web.UI.WebControls.Button btnCancelEditSub;
        protected global::System.Web.UI.WebControls.Button btnSaveEditSub;

        protected global::System.Web.UI.WebControls.Panel pnlEditUnivModal;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseEditUnivX;
        protected global::System.Web.UI.WebControls.HiddenField hfEditUnivId;
        protected global::System.Web.UI.WebControls.TextBox txtEditUnivIdDisplay;
        protected global::System.Web.UI.WebControls.TextBox txtEditUnivName;
        protected global::System.Web.UI.WebControls.TextBox txtEditUnivType;
        protected global::System.Web.UI.WebControls.Button btnCancelEditUniv;
        protected global::System.Web.UI.WebControls.Button btnSaveEditUniv;

        protected global::System.Web.UI.WebControls.Panel pnlEditCollegeModal;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseEditCollegeX;
        protected global::System.Web.UI.WebControls.HiddenField hfEditCollegeId;
        protected global::System.Web.UI.WebControls.TextBox txtEditCollegeIdDisplay;
        protected global::System.Web.UI.WebControls.TextBox txtEditCollegeName;
        protected global::System.Web.UI.WebControls.DropDownList ddlEditCollegeType;
        protected global::System.Web.UI.WebControls.TextBox txtEditCollegeLocation;
        protected global::System.Web.UI.WebControls.TextBox txtEditCollegeAffil;
        protected global::System.Web.UI.WebControls.Button btnCancelEditCollege;
        protected global::System.Web.UI.WebControls.Button btnSaveEditCollege;

        protected global::System.Web.UI.WebControls.Panel pnlEditSubjModal;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseEditSubjX;
        protected global::System.Web.UI.WebControls.HiddenField hfEditSubjId;
        protected global::System.Web.UI.WebControls.TextBox txtEditSubjIdDisplay;
        protected global::System.Web.UI.WebControls.TextBox txtEditSubjName;
        protected global::System.Web.UI.WebControls.TextBox txtEditSubjGroup;
        protected global::System.Web.UI.WebControls.Button btnCancelEditSubj;
        protected global::System.Web.UI.WebControls.Button btnSaveEditSubj;

        protected global::System.Web.UI.WebControls.Panel pnlEditAdminModal;
        protected global::System.Web.UI.WebControls.LinkButton btnCloseEditAdminX;
        protected global::System.Web.UI.WebControls.HiddenField hfEditAdminPcnoOrig;
        protected global::System.Web.UI.WebControls.TextBox txtEditAdminPcnoDisplay;
        protected global::System.Web.UI.WebControls.TextBox txtEditAdminName;
        protected global::System.Web.UI.WebControls.DropDownList ddlEditAdminRole;
        protected global::System.Web.UI.WebControls.Button btnCancelEditAdmin;
        protected global::System.Web.UI.WebControls.Button btnSaveEditAdmin;
    }
}
