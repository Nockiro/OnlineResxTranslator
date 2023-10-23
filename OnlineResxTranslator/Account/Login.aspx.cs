using System;
using Identity;
using Identity.Model;
using Microsoft.AspNet.Identity;

namespace Account
{
    public partial class Account_Login : PageBase
    {
        protected override void Page_LoadBegin(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validate the user password
                var manager = new UserManager();
                ApplicationUser user = manager.Find(UserName.Text, Password.Text);

                if (user != null)
                {
                    IdentityHelper.SignIn(manager, user, RememberMe.Checked);
                    manager.UpdateSecurityStampAsync(user.Id);
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                }
                else
                {
                    showError("Invalid username or password.");
                }
            }
        }
    }
}