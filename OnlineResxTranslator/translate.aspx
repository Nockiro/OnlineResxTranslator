<%@ Page Title="Translate" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="translate.aspx.cs" Inherits="_Translate" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: this.Title %>.</h2>
    <h4>You've translated <%: this.User.Identity.GetTranslatedStrings() %> strings until so far.</h4>
    <h1>
        <asp:Label ID="CurrentFile" runat="server" /></h1>

    <asp:Repeater ID="SummaryRepeater" runat="server">
        <HeaderTemplate>
            <table>
                <tr>
                    <th>Language Code</th>
                    <th>Completed</th>
                    <th>Last Update</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>

            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>


    <asp:Repeater ID="TextElements" runat="server">
        <HeaderTemplate>
            <table class="TLTable">
                <tr>
                    <th class="TLElement">Name of Element</th>
                    <th class="TLEnglish">English Text</th>
                    <th class="TLTranslated">Translated Text</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>

        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Button ID="Save" runat="server" Text="Save!" />
</asp:Content>
