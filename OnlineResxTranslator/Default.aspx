<%@ Page Debug="true" Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron fill">
        <h1><%: SiteMaster.ProjectName %></h1>
        <p class="lead">
            <%: SiteMaster.ProjectDescription %>
        </p>
        <%if (Session["CurrentlyChosenProject"] != null) { %>
            <asp:UpdatePanel ID="UpdtPnlForPbs" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <hr />
                    <h4>Currently selected project: <b><%= ((ProjectHelper.ProjectInfo)Session["CurrentlyChosenProject"]).Name %></b></h4>
                    <h4>Currently selected language: <b><%= new System.Globalization.CultureInfo(User.Identity.getUserLanguage(Session)).EnglishName %></b></h4>
                    <h4>There are <b><%= (new localhost.UserManager()).Users.ToList().Count(u => u.DefaultLanguage == User.Identity.getUserLanguage(Session)) %> Users</b> registered for your language, so keep that in mind while translating.</b></h4>
                    <br />
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            Completely translated languages (yet)
                
                            <asp:LoginView runat="server" ViewStateMode="Disabled">
                                <LoggedInTemplate>
                                    <asp:Button ID="btn_recalculatePoints"
                                        runat="server"
                                        Text="⟳"
                                        ToolTip="Recalculate progress"
                                        Style="float: right;"
                                        CommandName="recalcPercentage"
                                        CommandArgument="Complete"
                                        OnCommand="recalculatePoints_Click"></asp:Button>
                                </LoggedInTemplate>
                            </asp:LoginView>
                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdtPnlForPbs">
                                <ProgressTemplate>
                                    <div style="float: right; position: relative; left: 4px; top: -26px;">
                                        <asp:Label ID="lbl_update_compl" runat="server" Font-Size="XX-Small" Text="Recalculating..."></asp:Label>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                        <div class="panel-body">
                            <div class="alert alert-info">
                                <asp:Repeater ID="SuccessRepeater" runat="server" ItemType="ProjectHelper.ProjectFileShortSummary">
                                    <ItemTemplate>
                                        <div class="container">
                                            <p>
                                                <h4 class="progress-label make-space"><%# new System.Globalization.CultureInfo(Item.LangCode).EnglishName%></h4>

                                                <div class="progress progress-success">
                                                    <div class="progress-bar" role="progressbar" aria-valuenow="<%# Item.Percentage.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) %>"
                                                        aria-valuemin="0" aria-valuemax="100" style="width: <%# Item.Percentage.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) %>%">
                                                        <%# Item.Percentage.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) %>%
                                                    </div>
                                                </div>
                                            </p>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdtPnlForUncPbs" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="panel panel-warning">
                        <div class="panel-heading">
                            Not completely translated
                
                            <asp:LoginView runat="server" ViewStateMode="Disabled">
                                <LoggedInTemplate>
                                    <asp:Button ID="btn_recalculatePointsUnc"
                                        runat="server"
                                        Text="⟳"
                                        ToolTip="Recalculate progress"
                                        Style="float: right;"
                                        CommandName="recalcPercentage"
                                        CommandArgument="Uncomplete"
                                        OnCommand="recalculatePoints_Click"></asp:Button>
                                </LoggedInTemplate>
                            </asp:LoginView>

                            <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdtPnlForUncPbs">
                                <ProgressTemplate>
                                    <div style="float: right; position: relative; left: 4px; top: -26px;">
                                        <asp:Label ID="lbl_update_uncompl" runat="server" Font-Size="XX-Small" Text="Recalculating..."></asp:Label>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                        <div class="panel-body">
                            <div class="alert alert-info">
                                <asp:Repeater ID="UncompletedRepeater" runat="server" ItemType="ProjectHelper.ProjectFileShortSummary">
                                    <ItemTemplate>
                                        <div class="container">
                                            <p>
                                                <h4 class="progress-label make-space"><%# new System.Globalization.CultureInfo(Item.LangCode).EnglishName%></h4>

                                                <div class="progress progress-success">
                                                    <div class="progress-bar" role="progressbar" aria-valuenow="<%# Item.Percentage.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) %>"
                                                        aria-valuemin="0" aria-valuemax="100" style="width: <%# Item.Percentage.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) %>%">
                                                        <%# Item.Percentage.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) %>%
                                                    </div>
                                                </div>
                                            </p>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        <% } else if (Context.User.Identity.IsAuthenticated) { %>
            <div class="alert alert-warning">
                <strong>Warning!</strong> No projects registered. Please add one in the administration panel.
            </div>
        <% } else { %>
            <div class="alert alert-info">
                <strong>Note:</strong> You are not logged in and therefore not allowed to see this projects' statistics.
            </div>
        <% } %>

        <asp:LoginView runat="server" ViewStateMode="Disabled">
            <AnonymousTemplate>
                <p><a runat="server" href="~/account/login" class="btn btn-primary btn-lg">Let's go &raquo;</a></p>
            </AnonymousTemplate>
            <LoggedInTemplate>
        <%if (Session["CurrentlyChosenProject"] != null) { %>
                <p><a runat="server" href="~/translate" class="btn btn-primary btn-lg">Let's go &raquo;</a></p>
        <% } else { %>
                <p><a runat="server" href="~/admin/info" class="btn btn-primary btn-lg">Let's go &raquo;</a></p>
        <% } %>
            </LoggedInTemplate>
        </asp:LoginView>
    </div>
</asp:Content>
