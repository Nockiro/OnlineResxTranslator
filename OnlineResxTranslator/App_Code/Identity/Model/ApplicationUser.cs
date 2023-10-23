using Microsoft.AspNet.Identity.EntityFramework;

namespace Identity.Model
{
    public class ApplicationUser : IdentityUser
    {
        public int TranslatedStrings { get; set; }

        /// <summary>
        /// User's default language he translates into, can be entered on registration
        /// </summary>
        public string DefaultLanguage { get; set; }

        /// <summary>
        /// Language the user translates _from_
        /// </summary>
        public string SourceLanguage { get; set; }
    }
}