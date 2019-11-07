<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeclareRecords.aspx.cs" Inherits="RM_DeclareRecords.Layouts.RM_DeclareRecords.DeclareRecords" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h1>Declaring Records in Code</h1>
    <p>
        This application page demonstrates how to declare and un-declare items as 
        records in a SharePoint list. It declares or undeclares all the items in the
        list. You must have enabled the In-Place Records 
        Management feature at the site collection level before you use this sample.
    </p>
    <p>
        List Name: 
        <asp:TextBox ID="listNameTextbox" runat="server"></asp:TextBox>
    </p>
    <asp:Button ID="declareRecordsButton" OnClick="declareRecordsButton_Click" runat="server" Text="Declare Records" />
    <asp:Button ID="undeclareRecordsButton" OnClick="undeclareRecordsButton_Click" runat="server" Text="Undeclare Records" />
    <p>
        <asp:Label ID="resultsLabel" runat="server" Text=""></asp:Label>
    </p>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    RM Declare Records
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    RM Declare Records
</asp:Content>
