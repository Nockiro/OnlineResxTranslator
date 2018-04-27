using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using localhost;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Collections.Generic;

public partial class Account_Register : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Repeater languageListRepeater = (Repeater)langList.FindControl("rpt_languages");

        if (languageListRepeater != null)
        {
            var cultureList = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList();
            cultureList.Sort((p1, p2) => string.Compare(p1.EnglishName, p2.EnglishName, true));

            languageListRepeater.DataSource = cultureList;
            languageListRepeater.DataBind();
        }
    }
    protected void CreateUser_Click(object sender, EventArgs e)
    {
        var userManager = new UserManager();
        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

        var user = new ApplicationUser() { UserName = UserName.Text };
        user.DefaultLanguage = Request.Form["langSelect"];

        IdentityResult result = userManager.Create(user, Password.Text);

        if (result.Succeeded)
        {
            // check if the necessary roles are created in the database - if not, do it now
            if (!roleManager.RoleExists("user"))
                roleManager.Create(new IdentityRole("user"));
            if (!roleManager.RoleExists("admin"))
                roleManager.Create(new IdentityRole("admin"));

            // the first user created is always the admin
            var roleresult = userManager.AddToRole(user.Id, userManager.Users.Count() > 1 ? "user" : "admin");


            // if user already is logged in, we assume he doesn't need to switch to the new account
            if (User.Identity.IsAuthenticated)
                return;

            IdentityHelper.SignIn(userManager, user, isPersistent: false);
            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
        }
        else
        {
            ErrorMessage.Text = result.Errors.FirstOrDefault();
        }
    }
}