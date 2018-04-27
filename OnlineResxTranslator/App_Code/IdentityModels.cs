using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.Identity.Owin;
using localhost;
using System.Globalization;
using System.Web.SessionState;

namespace localhost
{
    // Sie können Benutzerdaten für den Benutzer hinzufügen, indem Sie der User-Klasse weitere Eigenschaften hinzufügen. Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public int TranslatedStrings { get; set; }
        public string DefaultLanguage { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }

    #region Hilfsprogramme
    public class UserManager : UserManager<ApplicationUser>
    {
        public UserManager()
            : base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
        {
        }
    }
}

public static class IdentityExtensions
{

    /// <summary>
    /// Gets the users currently selected language
    /// </summary>
    /// <param name="Identity">The users current identity</param>
    /// <param name="Session">The current page session</param>
    public static string getUserLanguage(this IIdentity Identity, HttpSessionState Session)
    {
        if (Session["CurrentlyChosenLanguage"] == null || String.IsNullOrEmpty((string)Session["CurrentlyChosenLanguage"]) || (string)Session["CurrentlyChosenLanguage"] == "iv")
        {
            List<CultureInfo> availableLangs = Identity.getUserLanguages();
            Session["CurrentlyChosenLanguage"] = availableLangs.Count > 0 ? availableLangs[0].TwoLetterISOLanguageName : "";
        }

        return (string)Session["CurrentlyChosenLanguage"];
    }

    public static List<CultureInfo> getUserLanguages(this IIdentity identity)
    {
            return ProjectHelper.getLanguages(identity);
    }

    public static List<ProjectHelper.ProjectInfo> getUserProjects(this IIdentity identity)
    {
        return ProjectHelper.getProjects(identity.GetUserId());
    }

    public static int GetTranslatedStrings(this IIdentity identity)
    {
        if (identity == null)
        {
            throw new ArgumentNullException("identity");
        }
        var ci = identity as ClaimsIdentity;
        if (ci != null)
        {
            return Convert.ToInt32(ci.FindFirst("TranslatedStrings").Value);
        }
        return -1;
    }

    public static string GetDefaultLanguage(this IIdentity identity)
    {
        if (identity == null)
        {
            throw new ArgumentNullException("identity");
        }

        var ci = identity as ClaimsIdentity;
        if (ci != null)
        {
            return ci.FindFirst("Language").Value;
        }
        return "";
    }


    public static void SetTranslatedStrings(this IIdentity identity, int value)
    {
        if (identity == null)
        {
            throw new ArgumentNullException("identity");
        }

        var ci = identity as ClaimsIdentity;
        if (ci != null)
        {
            SQLHelper sqlhelper = new SQLHelper().OpenConnection();

            // update user table
            sqlhelper.UpdateTable("AspNetUsers", "id = '" + identity.GetUserId() + "'",
             new KeyValuePair<string, string>("TranslatedStrings", value.ToString()));

            sqlhelper.CloseConnection();
        }

        return;
    }
}

namespace localhost
{
    public static class IdentityHelper
    {
        // Wird für XSRF beim Verknüpfen externer Anmeldungen verwendet.
        public const string XsrfKey = "XsrfId";

        public static void SignIn(UserManager manager, ApplicationUser user, bool isPersistent)
        {
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            identity.AddClaim(new Claim("TranslatedStrings", user.TranslatedStrings.ToString()));
            identity.AddClaim(new Claim("Language", user.DefaultLanguage));

            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        public const string ProviderNameKey = "providerName";
        public static string GetProviderNameFromRequest(HttpRequest request)
        {
            return request[ProviderNameKey];
        }

        public static string GetExternalLoginRedirectUrl(string accountProvider)
        {
            return "/Account/RegisterExternalLogin?" + ProviderNameKey + "=" + accountProvider;
        }

        private static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        public static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!String.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }
    }

    #endregion
}