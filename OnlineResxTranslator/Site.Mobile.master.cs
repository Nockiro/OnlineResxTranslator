using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Site_Mobile : MasterPage {


    public static string ProjectName = SiteMaster.ProjectName;
    public static string ProjectDescription = SiteMaster.ProjectDescription;
    public static Boolean OpenRegistrationAllowed = SiteMaster.OpenRegistrationAllowed;

    public Site_Mobile()
    {
        if (!String.IsNullOrEmpty(Session["ErrorMessage"]?.ToString()))
        {
            MainContent.Visible = false;
            ErrorMessage.Visible = true;
            FailureText.Text = Session["ErrorMessage"].ToString();
            return;
        }
        else ErrorMessage.Visible = false;
    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
    }
}
