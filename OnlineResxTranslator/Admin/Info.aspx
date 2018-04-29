<%@ Page Title="Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Info.aspx.cs" Inherits="Admin_Info" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %></h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <hr />

    <h3>Users</h3>
    <p>
        There are currently <%= getOnlineUsers().Count %> User(s) online:
    <%  Response.Write(string.Join(", ", getOnlineUsers().Select(user => user.Identity.GetUserName())));  %>
    </p>

    <asp:UpdatePanel ID="UpdateUserPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <p>There are currently <%= userList.Rows.Count %> Users(s) registered: </p>
            <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False"
                OnRowEditing="gvUsers_RowEditing" OnRowUpdating="gvUsers_RowUpdating" OnRowCancelingEdit="gvUsers_RowCancelingEdit" OnRowDeleting="gvUsers_RowDeleting"
                CssClass="table table-striped table-hover">
                <Columns>
                    <asp:TemplateField HeaderText="ID">
                        <ItemTemplate>
                            <asp:Label ID="lbl_UserID" runat="server" Text='<%#Eval("UserID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="User Name">
                        <ItemTemplate>
                            <asp:Label ID="lbl_UserName" runat="server" Text='<%#Eval("UserName") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_UserName" runat="server" Text='<%#Eval("UserName") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="User Email">
                        <ItemTemplate>
                            <asp:Label ID="lbl_UserMail" runat="server" Text='<%#Eval("UserMail") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_UserMail" runat="server" TextMode="Email" Text='<%#Eval("UserMail") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Visible Projects">
                        <HeaderTemplate>
                            <asp:Label ID="hd_vp" CssClass="help" ToolTip="Projects have to be seperated by comma (,)" runat="server" Text="Visible Projects"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_Projects" runat="server" Text='<%#Eval("UserProjects") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_Projects" runat="server" Text='<%#Eval("UserProjects") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Default Language">
                        <ItemTemplate>
                            <asp:Label ID="lbl_UserDefLang" runat="server" Text='<%#Eval("UserDefaultLanguage") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_UserDefLang" runat="server" Text='<%#Eval("UserDefaultLanguage") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Additional Languages">
                        <HeaderTemplate>
                            <asp:Label ID="hd_vp" CssClass="help" ToolTip="Note: For more than one language, seperate by comma (,). Note also: The user will only get the language menu shown if there is one than more language for him available." runat="server" Text="Additional Languages"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_UserLang" runat="server" Text='<%#Eval("UserLanguages") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_UserLang" runat="server" Text='<%#Eval("UserLanguages") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                            <asp:LinkButton ID="btn_Delete" runat="server" Text="Delete" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to *DELETE* this user?')" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btn_Update" runat="server" Text="Update" CommandName="Update" />
                            <asp:LinkButton ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdateProjectsPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h3>Projects</h3>
            <p>There are currently <%= projectList.Rows.Count - 1%> Project(s) registered: </p>
            <asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="False"
                OnRowEditing="gvProjects_RowEditing" OnRowUpdating="gvProjects_RowUpdating" OnRowCancelingEdit="gvProjects_RowCancelingEdit" OnRowDeleting="gvProjects_RowDeleting"
                CssClass="table table-striped table-hover">
                <Columns>
                    <asp:TemplateField HeaderText="ID">
                        <ItemTemplate>
                            <asp:Label ID="lbl_ID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Project Name">
                        <ItemTemplate>
                            <asp:Label ID="lbl_project" runat="server" Text='<%#Eval("project") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_project" runat="server" Text='<%#Eval("project") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Project folder (rel. to root folder configured in web.config)">
                        <ItemTemplate>
                            <asp:Label ID="lbl_folder" runat="server" Text='<%#Eval("folder") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_folder" runat="server" Text='<%#Eval("folder") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="FTP targets">
                        <HeaderTemplate>
                            <asp:Label ID="hd_ftps" CssClass="help" ToolTip="If you want to upload edited files to a ftp server, enter the ID of the FTP target - for more than one, separate them with ','" runat="server" Text="FTP targets"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_ftps" runat="server" Text='<%#Eval("ftps") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_ftps" runat="server" Text='<%#Eval("ftps") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                            <asp:LinkButton ID="btn_Delete" runat="server" Text="Delete" CommandName="Delete" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btn_Update" runat="server" Text="Update" CommandName="Update" />
                            <asp:LinkButton ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="UpdateFTPPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h3>FTP-Servers</h3>
            <p>There are currently <%= ftpList.Rows.Count - 1%> FTP target(s) registered: </p>
            <asp:GridView ID="gvFtps" runat="server" AutoGenerateColumns="False"
                OnRowEditing="gvFtps_RowEditing" OnRowUpdating="gvFtps_RowUpdating" OnRowCancelingEdit="gvFtps_RowCancelingEdit" OnRowDeleting="gvFtps_RowDeleting"
                CssClass="table table-striped table-hover">
                <Columns>
                    <asp:TemplateField HeaderText="ID">
                        <ItemTemplate>
                            <asp:Label ID="lbl_FTPID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Server adress">
                        <ItemTemplate>
                            <asp:Label ID="lbl_server" runat="server" Text='<%#Eval("server") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_server" runat="server" Text='<%#Eval("server") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Username">
                        <ItemTemplate>
                            <asp:Label ID="lbl_user" runat="server" Text='<%#Eval("username") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_user" runat="server" Text='<%#Eval("username") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Password">
                        <ItemTemplate>
                            <asp:Label ID="lbl_pass" runat="server" Text=''></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_pass" runat="server" TextMode="Password" Text='<%#Eval("password") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Path">
                        <ItemTemplate>
                            <asp:Label ID="lbl_path" runat="server" Text='<%#Eval("path") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tb_path" runat="server" Text='<%#Eval("path") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                            <asp:LinkButton ID="btn_Delete" runat="server" Text="Delete" CommandName="Delete" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btn_Update" runat="server" Text="Update" CommandName="Update" />
                            <asp:LinkButton ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
