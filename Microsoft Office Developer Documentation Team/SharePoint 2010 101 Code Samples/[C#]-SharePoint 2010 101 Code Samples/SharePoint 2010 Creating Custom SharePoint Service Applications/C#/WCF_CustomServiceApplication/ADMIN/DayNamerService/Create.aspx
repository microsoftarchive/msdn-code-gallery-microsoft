<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="IisWebServiceApplicationPoolSection" Src="~/_admin/IisWebServiceApplicationPoolSection.ascx" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="WCF_CustomServiceApplication.Admin.Create" MasterPageFile="~/_layouts/dialog.master" %>

<asp:Content ContentPlaceHolderID="PlaceHolderDialogHeaderPageTitle" runat="server">
    Day Namer Service Application
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderDialogDescription" runat="server">
    Create New Day Namer Service Application
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderDialogBodyMainSection" runat="server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="ms-authoringcontrols">
        <wssuc:InputFormSection Title="Name" Description="Specify a name for the service application." runat="server">
            <template_inputformcontrols>
                <wssuc:InputFormControl LabelAssocatiedControlID="ServiceAppName" runat="server">
                    <template_control>
                        <SharePoint:InputFormTextBox ID="ServiceAppName" class="ms-input" Columns="35" runat="server" />
                    </template_control>
                </wssuc:InputFormControl>
            </template_inputformcontrols>
        </wssuc:InputFormSection>
        
        <wssuc:IisWebServiceApplicationPoolSection ID="ApplicationPoolSelection" runat="server" />
        
		<wssuc:InputFormSection
			Title="Add to default proxy list"
			Description="The setting makes this service application available by default for Web applications in this farm to use. Do not check this setting if you wish to specify manually which Web applications should use this service application."
			runat="server">
			<Template_InputFormControls>
				<wssuc:InputFormControl LabelAssociatedControlID="DefaultServiceApp" runat="server">
					<Template_control>
					   <SharePoint:InputFormCheckBox ID="DefaultServiceApp" runat="server" Checked="True"
						             Text="Add this service application's proxy to the farm's default proxy list." />
					</Template_control>
				</wssuc:InputFormControl>
			</Template_InputFormControls>
		</wssuc:InputFormSection>

		<SharePoint:FormDigest runat="server" />		
    </table>
</asp:Content>