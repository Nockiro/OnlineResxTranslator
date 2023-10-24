using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Identity;
using static ProjectHelper;

public partial class SiteMaster : MasterPage
{
    private static List<ProjectInfo> projects = getProjects();

    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;
    private List<UserProjectListItem> projectDropdownList;
    private List<UserLanguageListItem> languageDropdownList;

    public const string SiteVersion = "2.1.0";
    public static string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
    public static string ProjectDescription = "Translate " + ProjectName + " into other languages!";
    public static bool OpenRegistrationAllowed = ConfigurationManager.AppSettings["EnableOpenRegistration"] != "false";

    /// <summary>
    /// Represents one language that the list the user chooses from contains.
    /// </summary>
    protected internal class UserLanguageListItem
    {
        public string TwoLetterISOLanguageName { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }

        public UserLanguageListItem(CultureInfo cultureInfo)
        {
            TwoLetterISOLanguageName = cultureInfo.TwoLetterISOLanguageName;
            DisplayName = cultureInfo.DisplayName;
        }
    }

    /// <summary>
    /// Represents one project that the list the user chooses from contains.
    /// </summary>
    protected internal class UserProjectListItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }

        public UserProjectListItem(ProjectInfo projectInfo)
        {
            Id = projectInfo.ID;
            DisplayName = projectInfo.Name;
        }
    }

    protected void SelectProject(object sender, CommandEventArgs e)
    {
        Session["CurrentlyChosenProject"] = projects.Find(t => t.ID == Convert.ToInt32(e.CommandArgument));
        // reset selected filename
        Session["SelectedFilename"] = null;

        updateMenuActiveIndicators();

        Response.Redirect(Request.RawUrl);
    }

    protected void SelectLanguage(object sender, CommandEventArgs e)
    {
        Session["CurrentlyChosenLanguage"] = e.CommandArgument;
        updateMenuActiveIndicators();
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

        Page.PreLoad += Page_PreLoad;
        Page.LoadComplete += Page_LoadComplete;

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

    private void Page_LoadComplete(object sender, EventArgs e)
    {
        initializeMenu();
    }

    private void Page_Load(object sender, EventArgs e)
    {
        updateLanguageDropdownList();
        updateProjectDropdownList();

        initializeMenu();
        updateMenuActiveIndicators();
    }

    private void Page_PreLoad(object sender, EventArgs e)
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

    private void initializeMenu()
    {
        Repeater projectListRepeater = (Repeater)loginview1.FindControl("projectList");

        // after a LOT of not getting why everything in asp has to be so completly idiotic I don't care if the user is logged in or not, just check if the element is there
        if (projectListRepeater != null)
        {
            projectListRepeater.DataSource = projectDropdownList;
            projectListRepeater.DataBind();
        }

        Repeater languageListRepeater = (Repeater)loginview1.FindControl("rpt_languages");
        if (languageListRepeater == null)
            return;

        if (languageDropdownList.Count > 1)
        {
            languageListRepeater.DataSource = languageDropdownList;
            languageListRepeater.DataBind();
        }
        else
            loginview1.FindControl("langList").Visible = false;
    }

    private void updateLanguageDropdownList()
    {
        List<CultureInfo> cultureInfos = Context.User.Identity.getUserLanguages();
        languageDropdownList = cultureInfos.Select(cultureInfo => new UserLanguageListItem(cultureInfo)).ToList();
    }

    private void updateProjectDropdownList()
    {
        List<ProjectInfo> userProjects = Context.User.Identity.getUserProjects();
        projectDropdownList = userProjects.Select(projectInfo => new UserProjectListItem(projectInfo)).ToList();
    }

    private void updateMenuActiveIndicators()
    {
        ProjectInfo currentUserProject = (ProjectInfo)Session["CurrentlyChosenProject"];
        string currentUserLanguage = Context.User.Identity.getUserLanguage(Session);

        foreach (UserProjectListItem projectItem in projectDropdownList)
        {
            projectItem.IsActive = projectItem.Id == currentUserProject.ID;
        }

        foreach (UserLanguageListItem languageItem in languageDropdownList)
        {
            languageItem.IsActive = currentUserLanguage.Equals(languageItem.TwoLetterISOLanguageName);
        }
    }
}