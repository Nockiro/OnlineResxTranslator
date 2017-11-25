using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class About : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!User.Identity.IsAuthenticated || !User.IsInRole("user"))
        {
            Response.Redirect("/Account/Login.aspx?ReturnUrl=/Translate&re=403");
        }
    }
}