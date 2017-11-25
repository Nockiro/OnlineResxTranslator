<%@ Page Title="Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Info.aspx.cs" Inherits="Admin_Info" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %></h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <hr />

    <p>There are currently <%= getOnlineUsers().Count %> User(s) online:</p>
    <%  foreach (System.Security.Principal.IPrincipal user in getOnlineUsers())
        {
    %>

    <li><%=user.Identity.GetUserName()%></li>

    <%  }  %>
    <hr />
    Thanks.
</asp:Content>
