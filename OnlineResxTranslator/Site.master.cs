using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    public static string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
    public static string ProjectDescription = "Translate " + ProjectName + " into other languages!";
    public static Boolean OpenRegistrationAllowed = ConfigurationManager.AppSettings["EnableOpenRegistration"] != "false";
    public static List<ProjectHelper.ProjectInfo> projects = ProjectHelper.getProjects();

    protected void SelectProject(object sender, CommandEventArgs e)
    {
        Session["CurrentlyChosenProject"] = projects.Find(t => t.ID == Convert.ToInt32(e.CommandArgument));
        // reset selected filename
        Session["SelectedFilename"] = null;

        Response.Redirect(Request.RawUrl);
    }

    protected void SelectLanguage(object sender, CommandEventArgs e)
    {
        Session["CurrentlyChosenLanguage"] = e.CommandArgument;
    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
        Session.Clear();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        // Der Code unten schützt vor XSRF-Angriffen.
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Das Anti-XSRF-Token aus dem Cookie verwenden
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Neues Anti-XSRF-Token generieren und im Cookie speichern
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;



        if (Session["ErrorMessage"] != null && !String.IsNullOrEmpty(Session["ErrorMessage"].ToString()))
        {
            ErrorMessage.Visible = true;
            FailureText.Text = Session["ErrorMessage"].ToString();
            return;
        }
        else ErrorMessage.Visible = false;

        if (Context.User.Identity.IsAuthenticated && Session["CurrentlyChosenProject"] == null)
        {
            projects = Context.User.Identity.getUserProjects();
            Session["CurrentlyChosenProject"] = projects.Count > 0 ? projects[0] : null;
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Repeater projectListRepeater = (Repeater)loginview1.FindControl("projectList");

        // after a LOT of not getting why everything in asp has to be so completly idiotic I don't care if the user is logged in or not, just check if the element is there
        if (projectListRepeater != null)
        {
            projectListRepeater.DataSource = Context.User.Identity.getUserProjects();
            projectListRepeater.DataBind();
        }

        Repeater languageListRepeater = (Repeater)loginview1.FindControl("rpt_languages");

        if (languageListRepeater != null)
        {
            List<CultureInfo> list = Context.User.Identity.getUserLanguages();
            if (list.Count > 1)
            {
                languageListRepeater.DataSource = list;
                languageListRepeater.DataBind();
            }
            else
                loginview1.FindControl("langList").Visible = false;
        }
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Anti-XSRF-Token festlegen
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // Anti-XSRF-Token überprüfen
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Fehler bei der Überprüfung des Anti-XSRF-Tokens.");
            }
        }
    }
}