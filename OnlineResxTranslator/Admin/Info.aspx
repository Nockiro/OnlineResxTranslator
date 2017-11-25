<%@ Page Title="Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Info.aspx.cs" Inherits="Admin_Info" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %></h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <hr />

    <h3>Users</h3>
    <p>There are currently <%= getOnlineUsers().Count %> User(s) online:</p>
    <%  foreach (System.Security.Principal.IPrincipal user in getOnlineUsers())
        {
    %>

    <li><%=user.Identity.GetUserName()%></li>

    <%  }  %>
    <hr />


    <asp:UpdatePanel ID="UpdateProjectsPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h3>Projects</h3>
            <p>There are currently <%= projectList.Rows.Count - 1%> Project(s) registered: </p>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting"
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
