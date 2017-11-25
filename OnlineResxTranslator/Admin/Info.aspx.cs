using ASP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

public partial class Admin_Info : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public List<System.Security.Principal.IPrincipal> getOnlineUsers()
    {
        return global_asax.Sessions;
    }

    public class UserInfo {
        string UserName { get; set; }
        string Role { get; set; }
    }

}