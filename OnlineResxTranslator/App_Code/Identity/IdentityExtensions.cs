using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.SessionState;
using Microsoft.AspNet.Identity;

namespace Identity
{
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
            try
            {
                return ProjectHelper.getLanguages(identity);
            }
            catch (Exception)
            {
                return new List<CultureInfo>();
            }
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

        /// <summary>
        /// Retrieves the user's source language or "en" if it doesn't exist
        /// </summary>
        public static string GetSourceLanguage(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirst("SourceLanguage").Value;
            }
            return "en";
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
                SqlManager sqlManager = new SqlManager().OpenConnection();

                // update user table
                sqlManager.UpdateTable("AspNetUsers", "id = '" + identity.GetUserId() + "'",
                    new KeyValuePair<string, string>("TranslatedStrings", value.ToString()));

                sqlManager.CloseConnection();
            }

            return;
        }
    }
}