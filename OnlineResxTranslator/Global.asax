<%@ Application Language="C#" %>
<%@ Import Namespace="localhost" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>

<script RunAt="server">
    private static readonly List<System.Security.Principal.IPrincipal> _sessions = new List<System.Security.Principal.IPrincipal>();
    private static readonly object padlock = new object();

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
    
    public static List<System.Security.Principal.IPrincipal> Sessions
    {
        get
        {
            return _sessions;
        }
    }

    protected void Session_Start(object sender, EventArgs e)
    {
        lock (padlock)
        {
            _sessions.Add(User);
        }
    }
    protected void Session_End(object sender, EventArgs e)
    {
        lock (padlock)
        {
            _sessions.Remove(User);
        }
    }

</script>
