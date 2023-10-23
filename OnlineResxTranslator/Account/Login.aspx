<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Account_Login" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %></h2>
    <hr />

    <div class="row">
        <div class="col-md-8">
            <section id="loginForm">
                <asp:Panel ID="pnl_login" runat="server" CssClass="form-horizontal" DefaultButton="btnLogin">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-2 control-label">User</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="UserName" CssClass="form-control bg-dark text-white" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName"
                                CssClass="text-danger" Display="Dynamic" ErrorMessage="Das Benutzernamefeld ist erforderlich." />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control bg-dark text-white" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" 
                                                        Display="Dynamic" ErrorMessage="Das Kennwortfeld ist erforderlich." />
                        </div>
                    </div>
                    <div class="form-group form-user-setting">
                        <div class="col-md-offset-2 col-md-10">
                            <div class="checkbox">
                                <asp:CheckBox runat="server" ID="RememberMe" />
                                <asp:Label runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group form-user-setting">
                        <div class="col-md-offset-2 col-md-10">
                            <asp:LinkButton ID="btnLogin"
                                runat="server"
                                CssClass="btn btn-primary"
                                OnClick="LogIn">
                                <i class="bi bi-box-arrow-in-right" aria-hidden="true"></i> &nbsp;Login
                            </asp:LinkButton>
                        </div>
                    </div>
                </asp:Panel>
                <% if (SiteMaster.OpenRegistrationAllowed)
                    { %>
                <p>
                    <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register</asp:HyperLink>
                    if you don't have an account yet.
                </p>
                <% } %>
            </section>
        </div>
    </div>
</asp:Content>

