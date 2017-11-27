﻿using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using localhost;

public partial class Account_Register : Page {
    protected void CreateUser_Click(object sender, EventArgs e)
    {
        var manager = new UserManager();
        var user = new ApplicationUser() { UserName = UserName.Text };
        IdentityResult result = manager.Create(user, Password.Text);

        if (result.Succeeded)
        {
            var roleresult = manager.AddToRole(user.Id, "user");

            // if user already is logged in, we assume he doesn't need to switch to the new account
            if (User.Identity.IsAuthenticated)
                return;

            IdentityHelper.SignIn(manager, user, isPersistent: false);
            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
        }
        else
        {
            ErrorMessage.Text = result.Errors.FirstOrDefault();
        }
    }
}