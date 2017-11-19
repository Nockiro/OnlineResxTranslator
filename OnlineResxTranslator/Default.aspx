<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1><%: SiteMaster.ProjectName %></h1>
        <p class="lead"><%: SiteMaster.ProjectDescription %></p>
        <asp:LoginView runat="server" ViewStateMode="Disabled">
            <AnonymousTemplate>
                <p><a href="account/login" class="btn btn-primary btn-lg">Let's go &raquo;</a></p>
            </AnonymousTemplate>
            <LoggedInTemplate>
                <p><a href="translate" class="btn btn-primary btn-lg">Let's go &raquo;</a></p>
            </LoggedInTemplate>
        </asp:LoginView>

        <div class="panel panel-success">
            <div class="panel-heading">Completely translated languages (yet)</div>
            <div class="panel-body">
                <div class="alert alert-info">
                    <% foreach (System.Data.DataRow row in XMLFile.ComputeSummary("ProjectFolder", 100.0, 100.0).Rows)
                        { %>
                    <div class="container">
                        <p>
                            <h4 class="progress-label make-space"><%: new System.Globalization.CultureInfo(row["LanguageCode"].ToString()).EnglishName%></h4>
                            <div class="progress progress-success">
                                <div class="progress-bar" role="progressbar" aria-valuenow="<%: ((double)row["Percentage"]).ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) %>"
                                    aria-valuemin="0" aria-valuemax="100" style="width: <%: ((double)row["Percentage"]).ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) %>%">
                                    <%: ((double)row["Percentage"]).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) %>%
                                </div>
                            </div>
                        </p>
                    </div>

                    <% } %>
                </div>

            </div>
        </div>
        <div class="panel panel-warning">
            <div class="panel-heading">Not completely translated</div>
            <div class="panel-body">
                <div class="alert alert-info">
                    <% foreach (System.Data.DataRow row in XMLFile.ComputeSummary("ProjectFolder").Rows)
                        { %>
                    <div class="container">
                        <p>
                            <h4 class="progress-label make-space"><%: new System.Globalization.CultureInfo(row["LanguageCode"].ToString()).EnglishName%></h4>
                            <div class="progress progress-success">
                                <div class="progress-bar" role="progressbar" aria-valuenow="<%: ((double)row["Percentage"]).ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) %>"
                                    aria-valuemin="0" aria-valuemax="100" style="width: <%: ((double)row["Percentage"]).ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) %>%">
                                    <%: ((double)row["Percentage"]).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) %>%
                                </div>
                            </div>
                        </p>
                    </div>

                    <% } %>
                </div>
            </div>
        </div>
    </div>
    <%--
        <div class="row">
            <div class="col-md-4">
                <h2>Getting started</h2>
                <p>
                    ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
                </p>
                <p>
                    <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
                </p>
            </div>
            <div class="col-md-4">
                <h2>Get more libraries</h2>
                <p>
                    NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
                </p>
                <p>
                    <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
                </p>
            </div>
            <div class="col-md-4">
                <h2>Web Hosting</h2>
                <p>
                    You can easily find a web hosting company that offers the right mix of features and price for your applications.
                </p>
                <p>
                    <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
                </p>
            </div>
        </div>--%>
</asp:Content>
