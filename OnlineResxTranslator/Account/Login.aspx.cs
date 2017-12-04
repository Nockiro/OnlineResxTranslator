using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Web;
using System.Web.UI;
using localhost;

public partial class Account_Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterHyperLink.NavigateUrl = "Register";

        var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        var redirectProblem = HttpUtility.UrlEncode(Request.QueryString["re"]);

        if (!String.IsNullOrEmpty(redirectProblem)) {
            switch (redirectProblem)
            {
                // forbidden
                case "403":
                    FailureText.Text = "You... don't have access. Sorry.";
                    break;
                case "se":
                    FailureText.Text = "Apparently your session is expired - please login.";
                    break;

            }
            ErrorMessage.Visible = true;
        }

        if (!String.IsNullOrEmpty(returnUrl))
        {
            RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
        }
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
                FailureText.Text = "Invalid username or password.";
                ErrorMessage.Visible = true;
            }
        }
    }
}