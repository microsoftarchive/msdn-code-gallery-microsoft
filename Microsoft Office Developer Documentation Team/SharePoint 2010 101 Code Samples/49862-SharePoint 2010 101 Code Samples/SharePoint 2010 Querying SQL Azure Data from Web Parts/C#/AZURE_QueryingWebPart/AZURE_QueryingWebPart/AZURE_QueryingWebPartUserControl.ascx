<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AZURE_QueryingWebPartUserControl.ascx.cs" Inherits="AZURE_QueryingWebPart.AZURE_QueryingWebPart.AZURE_QueryingWebPartUserControl" %>

<table>
    <tr>
        <td>
            <strong>Product Catalog</strong>
        </td>
    </tr>
    <tr>
        <td>
            <asp:GridView ID="datagridProducts" 
                runat="server" 
                BackColor="White"
                style="font-family: Calibri; font-size: small"
                >
                <AlternatingRowStyle BackColor="#99CCFF" ForeColor="Black" />
                <HeaderStyle BackColor="#0099CC" ForeColor="White" />
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td>
            <asp:LinkButton ID="lnkbtnGetStoreInformation" 
                runat="server"         
                style="font-family: Calibri; font-size: small"
                onclick="lnkbtnGetProducts_Click">
                    Get Product Data
            </asp:LinkButton>
        </td>
    </tr>
</table>
