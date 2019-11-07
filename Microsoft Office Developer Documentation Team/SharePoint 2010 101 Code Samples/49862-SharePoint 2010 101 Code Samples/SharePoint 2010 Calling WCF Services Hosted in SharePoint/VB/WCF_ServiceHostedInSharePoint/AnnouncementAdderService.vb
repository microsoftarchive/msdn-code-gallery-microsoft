Imports Microsoft.SharePoint.Client.Services
Imports System.ServiceModel.Activation

Namespace WCF_ServiceHostedInSharePoint
    'This project shows you how to create a WCF service hosted in SharePoint
    'That way, you can use the full SharePoint server-side object model
    'then call the service from a client application.

    'Note that, as well as the Service Contract (ISharePointAnnouncementAdder.cs) and
    'the service implementation (this file), this project deploys the AddAnnouncement.svc
    'file to the ISAPI folder in SharePoint. That makes the service available in _vti_bin folders

    'These attributes instruct the SharePoint service factory to create a metadata exchange end point for the service
    'Without these attributes, we would have to use the SharePoint web.config file to configure the WCF service
    <BasicHttpBindingServiceMetadataExchangeEndpoint()> _
    <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Required)> _
    Public Class AnnouncementAdderService
        Implements ISharePointAnnouncementAdder

        'Implement the AddAnnouncement method
        Public Function AddAnnouncement(ByVal Title As String, ByVal Body As String) As Boolean Implements ISharePointAnnouncementAdder.AddAnnouncement
            Try
                'Get the announcements list
                Dim announcementsList As SPList = SPContext.Current.Web.Lists("Announcements")
                'Add a new announcement
                Dim newAnnouncement As SPListItem = announcementsList.AddItem()
                newAnnouncement("Title") = Title
                newAnnouncement("Body") = Body
                newAnnouncement.Update()
                'Let the client know everything went well
                Return True
            Catch ex As Exception
                'The method did not succeed
                Return False
            End Try
        End Function
    End Class

End Namespace
