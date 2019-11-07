<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETImageEditUpload._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Personal Information</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2>
            OverView of Persons</h2>
        <asp:SqlDataSource ID="SqlDSPersonOverView" runat="server" ConnectionString="<%$ ConnectionStrings:db_PersonsConnectionString %>"
            SelectCommand="SELECT [Id], [PersonName] FROM [tb_personInfo]"></asp:SqlDataSource>
        <asp:GridView ID="gvPersonOverView" runat="server" CellPadding="4" EnableModelValidation="True"
            ForeColor="#333333" GridLines="None" Width="70%" AutoGenerateColumns="False"
            DataKeyNames="Id" DataSourceID="SqlDSPersonOverView" OnSelectedIndexChanged="gvPersonOverView_SelectedIndexChanged"
            AllowPaging="True" AllowSorting="True">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True"
                    SortExpression="Id" />
                <asp:BoundField DataField="PersonName" HeaderText="PersonName" SortExpression="PersonName" />
                <asp:CommandField ShowSelectButton="True" HeaderText="Click to see Details" SelectText="Details..." />
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <EmptyDataTemplate>
                No Data Available, Please Insert data with the help of the FormView...<br />
            </EmptyDataTemplate>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        </asp:GridView>
    </div>
    <h2>
        Person In Detail</h2>
    <asp:FormView ID="fvPersonDetails" runat="server" Width="50%" DataSourceID="SqlDSPersonDetails"
        EnableModelValidation="True" DataKeyNames="Id" DataMember="DefaultView" OnItemInserting="fvPersonDetails_ItemInserting"
        OnItemUpdating="fvPersonDetails_ItemUpdating" BackColor="#DEBA84" BorderColor="#DEBA84"
        BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" GridLines="Both"
        OnItemUpdated="fvPersonDetails_ItemUpdated" OnItemDeleted="fvPersonDetails_ItemDeleted"
        OnItemDeleting="fvPersonDetails_ItemDeleting" OnItemInserted="fvPersonDetails_ItemInserted"
        OnModeChanging="fvPersonDetails_ModeChanging">
        <ItemTemplate>
            <table width="100%">
                <tr>
                    <th>
                        Person Name:
                    </th>
                    <td colspan="2">
                        <%#Eval("PersonName") %>
                    </td>
                </tr>
                <tr>
                    <th>
                        Person Image:
                    </th>
                    <td colspan="2">
                        <img src='ImageHandler/ImageHandler.ashx?id=<%#Eval("Id") %>' width="200" alt=""
                            height="200" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Text="Edit" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" Text="Delete"
                            OnClientClick="return confirm('Are you sure to delete it completely?');" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkNew" runat="server" CommandName="New" Text="New" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <EditItemTemplate>
            <table width="100%">
                <tr>
                    <th>
                        Person Name:
                    </th>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" Text=' <%#Bind("PersonName") %>' MaxLength="20" />
                        <asp:RequiredFieldValidator ID="reqName" runat="server" ControlToValidate="txtName"
                            ErrorMessage="Name is required!">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th>
                        Person Image:
                    </th>
                    <td>
                        <asp:FileUpload ID="fupEditImage" runat="server" />
                        <asp:CustomValidator ID="cmvImageType" runat="server" ControlToValidate="fupEditImage"
                            ErrorMessage="File is invalid!" OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" Text="Update" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" Text="Cancel"
                            CausesValidation="False" />
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
        <EditRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
        <InsertItemTemplate>
            <table width="100%">
                <tr>
                    <th>
                        Person Name:
                    </th>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" MaxLength="20" Text='<%#Bind("PersonName") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName"
                            ErrorMessage="Name is required!">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th>
                        Person Image:
                    </th>
                    <td>
                        <asp:FileUpload ID="fupInsertImage" runat="server" />
                        <asp:CustomValidator ID="cmvImageType" runat="server" ControlToValidate="fupInsertImage"
                            ErrorMessage="File is invalid!" OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkInsert" runat="server" CommandName="Insert" Text="Insert" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkInsertCancel" runat="server" CommandName="Cancel" Text="Cancel"
                            CausesValidation="False" />
                    </td>
                </tr>
            </table>
        </InsertItemTemplate>
        <PagerStyle HorizontalAlign="Center" ForeColor="#8C4510" />
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
    </asp:FormView>
    <asp:SqlDataSource ID="SqlDSPersonDetails" runat="server" ConnectionString="<%$ ConnectionStrings:db_PersonsConnectionString %>"
        DeleteCommand="DELETE FROM tb_personInfo WHERE (Id = @Id)" InsertCommand="INSERT INTO tb_personInfo(PersonName, PersonImage, PersonImageType) VALUES (@PersonName, @PersonImage, @PersonImageType)"
        SelectCommand="SELECT [Id], [PersonName] FROM [tb_personInfo] where id=@id" UpdateCommand="UPDATE tb_personInfo SET PersonName = @PersonName, PersonImage = @PersonImage, PersonImageType = @PersonImageType WHERE (Id = @Id)">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="PersonName" Type="String" />
            <asp:Parameter Name="PersonImage" DbType="Binary" ConvertEmptyStringToNull="true" />
            <asp:Parameter Name="PersonImageType" Type="String" ConvertEmptyStringToNull="true" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="PersonName" Type="String" />
            <asp:Parameter Name="PersonImage" DbType="Binary" ConvertEmptyStringToNull="true" />
            <asp:Parameter Name="PersonImageType" Type="String" ConvertEmptyStringToNull="true" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
        <SelectParameters>
            <asp:ControlParameter Name="id" Type="Int32" ControlID="gvPersonOverView" PropertyName="SelectedValue"
                DefaultValue="0" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>
</body>
</html>
