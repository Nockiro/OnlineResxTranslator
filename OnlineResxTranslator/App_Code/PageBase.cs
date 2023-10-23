using System;
using System.Collections.Generic;
using System.Web.UI;
using Extensions;

/// <summary>
/// Base class for pages which can use some extended functions (eg error messages)
/// </summary>
public class PageBase : Page
{
    public PageBase()
    {
    }

    /// <summary>
    /// Gets called at the end of a page loading process
    /// </summary>
    protected virtual void Page_LoadEnd()
    {
        // if we wouldn't delete the message in the session, it would get shown over and over
        Session["ErrorMessage"] = null;
    }

    /// <summary>
    /// Will be executed in case page loading was not interrupted by e.g. an error message
    /// </summary>
    protected virtual void Page_LoadBegin(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// It's recommended to NOT override this method in order, instead use <see cref="Page_LoadBegin"/>
    /// </summary>
    protected virtual void Page_Load(object sender, EventArgs e)
    {
        // prevent loading of default content if there is an error
        if (Session["ErrorMessage"] != null && !String.IsNullOrEmpty(Session["ErrorMessage"].ToString()))
        { }
        else
            Page_LoadBegin(sender, e);

        Page_LoadEnd();
    }

    /// <summary>
    /// Shows error message at the beginning of a page (Attention: Needs page to use <see cref="Page_LoadBegin"/> and master page to handle the variable
    /// Make sure to redirect the user if sensitive content isn't allowed on the current page
    /// </summary>
    /// <param name="msg">Message to be shown</param>
    /// <param name="redirect">if given, errorwill be shown on this page</param>
    /// <param name="origin">can be used for redirects after showing the message</param>
    protected void showError(string msg, string redirect = null, string origin = null)
    {
        Session["ErrorMessage"] = msg;

        Uri EndURI = redirect != null ? new Uri(new Uri(Request.Url.GetLeftPart(UriPartial.Authority)), redirect) : Request.Url;
        if (!String.IsNullOrEmpty(origin))
            EndURI = EndURI.ExtendQuery(new Dictionary<string, string> { { "returnURL", origin } });

        Response.Redirect(EndURI.PathAndQuery);
    }
}