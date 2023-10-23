<%@ Page Title="Translate" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="translate.aspx.cs" ValidateRequest="false" Inherits="_Translate" %>
<%@ Import Namespace="Identity" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: this.Title %></h2>


    <%if (Session["CurrentlyChosenProject"] != null)
        { %>
    <div class="card">
        <div class="card-header">
            <h4 style="display: inline-block;">
                <asp:Label ID="SelectedProject" CssClass="" runat="server" /></h4>
            <button type="button" style="float: right;" class="btn btn-xl btn-primary" data-bs-toggle="collapse" data-bs-target="#fileListSpoiler" aria-expanded="true" aria-controls="fileListSpoiler" id="filelisttoggle">Toggle file list</button>
        </div>
        <%-- collapse the file list if a file was selected --%>
        <div class="collapse<%: Session["SelectedFilename"] == null ? ".show" : "" %>" id="fileListSpoiler">
            <div class="card-body">
                <asp:GridView ID="FileList" runat="server" CssClass="table table-hover table-striped table-bordered" AutoGenerateColumns="False" AllowSorting="true" DataKeyNames="name"
                    UseAccessibleHeader="true" OnRowCommand="FileList_RowCommand">
                    <Columns>
                        <asp:ButtonField DataTextField="name" HeaderText="Filename" CommandName="EditFile" />
                        <asp:BoundField DataField="percentcompleted" HeaderText="Completed %" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="caption" HeaderText="Caption" NullDisplayText="" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="UpPanTranslations" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div>
                <h4 style="float: left;"><i>You've translated <%: this.User.Identity.GetTranslatedStrings() %> strings until so far.</i></h4>
                <asp:CheckBox ID="cb_showOnlyUntr" runat="server" Text="Show only untranslated elements" CssClass="right" AutoPostBack="true" OnCheckedChanged="cb_showOnlyUntr_CheckedChanged" />
            </div>
            <br />
            <hr style="clear: both;" />
            <h2>
                <asp:Label ID="CurrentFile" runat="server" /></h2>

            <asp:Repeater ID="TextElements" runat="server">
                <HeaderTemplate>
                    <table class="TLTable table table-striped table-hover table-bordered">
                        <tr>
                            <th class="TLElement">Name of Element</th>
                            <th class="TLEnglish">English Text</th>
                            <th class="TLTranslated">Translated Text</th>
                            <th class="TLComment help" title="Note: This is just for you translators.">Comment</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>

                    <tr class="<%# IsTranslatedCSS(DataBinder.Eval(Container,"DataItem.Translation")) %>">
                        <td class="TLElement">
                            <asp:Label ID="Element" runat="server" Text='<%#Eval("TextName")%>' /></td>
                        <td class="TLEnglish"><%#DataBinder.Eval(Container,"DataItem.English")%></td>

                        <td class="TLTranslated col-xs-6">
                            <asp:TextBox ID="TranslatedText" runat="server" Text='<%#Server.HtmlDecode((string)Eval("Translation"))%>'
                                TextMode="MultiLine" CssClass="TLTranslatedInput" Style="width: 100%" /></td>
                        <td class="TLComment col-xs-2">
                            <asp:TextBox ID="TranslateComment" runat="server" Text='<%#Eval("Comment")%>'
                                TextMode="MultiLine" Style="width: 100%" /></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Button ID="Save" runat="server" Text="Save!" CssClass="btn btn-primary" Style="position: fixed; width: 30%; top: 100%; margin-top: -64px; right: 32px;" OnClick="Save_Click" />
            <asp:UpdateProgress ID="SaveUpdateProgress" runat="server" AssociatedUpdatePanelID="UpPanTranslations" DisplayAfter="100">
                <ProgressTemplate>
                    <div style="position: fixed; top: 100%; margin-top: -62px; right: 38px;">
                        <asp:Label ID="lbl_update_uncompl" runat="server" Font-Size="larger" Text="Wait a second..."></asp:Label>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </ContentTemplate>
    </asp:UpdatePanel>

    <% }
        else
        { %>
    <div class="alert alert-warning">
        <strong>Warning!</strong> No projects registered. Please add one in the administration panel.
    </div>
    <% } %>
</asp:Content>
