<%--/****************************** Module Header ******************************\
 * Module Name:  ChangePermissions.aspx
 * Project:      CSSPSChangeUserListPermission
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrates how to change the user permissions in a list or library 
 * by using JSOM.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/--%>
<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePermissions.aspx.cs" Inherits="CSSPSChangeUserListPermission.Layouts.CSSPSChangeUserListPermission.ChangePermissions" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>

<% #if SOME_UNDEFINED_CONSTANT %> 
<script type="text/ecmascript" src="/_layouts/SP.core.debug.js" ></script>
<script type="text/ecmascript" src="/_layouts/SP.runtime.debug.js" ></script>
<script type="text/ecmascript" src="/_layouts/SP.debug.js" ></script>
<% #endif %>

    <script type="text/javascript">
        $(document).ready(function () {
            // Call the function.
            CustomPermission();
        });

        // Web of current client context.
        var web;
        // The list\Library will be operation.
        var oList;
        // The user will be operation.
        var oUser;
        // The roles will be operation.
        var roles;

        function CustomPermission() {
            // Get current client context.
            var clientContext = new SP.ClientContext.get_current();
            // Get current web.
            this.web = clientContext.get_web();
            // Provide the existing list name.
            this.oList = this.web.get_lists().getByTitle('RatingList');
            // Break the inheritance. 
            this.oList.breakRoleInheritance(false, true);
            // Get the user(domain\username).
            this.oUser = this.web.ensureUser("seiyasu\\seiya");

            // Define the roles that will be operation.
            this.roles = SP.RoleDefinitionBindingCollection.newObject(clientContext);
            this.roles.add(web.get_roleDefinitions().getByType(SP.RoleType.contributor));

            // Register the role for the user.
            this.oList.get_roleAssignments().add(this.oUser, this.roles)

            clientContext.load(this.web);
            clientContext.load(this.oUser);
            clientContext.load(this.oList);
            //Make a query call to execute the above statements
            clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySuccess),
                Function.createDelegate(this, this.onQueryFailure));
        }
      
        function onQuerySuccess() {
            $('#message').text('Updated');
        }
        function onQueryFailure(sender, args) {
            alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
        }
</script> 



</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div>
        <p id="message">
            <!-- The following content will be replaced with the user name when you run the app - see App.js -->
            initializing...
        </p>
    </div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
My Application Page
</asp:Content>