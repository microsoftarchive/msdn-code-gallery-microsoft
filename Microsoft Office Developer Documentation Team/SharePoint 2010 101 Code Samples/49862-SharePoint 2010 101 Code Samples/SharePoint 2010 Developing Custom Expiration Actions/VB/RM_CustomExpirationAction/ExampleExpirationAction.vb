Imports System.Xml
Imports Microsoft.Office.RecordsManagement.PolicyFeatures
Imports Microsoft.SharePoint

''' <summary>
''' An Information Management Policy is a set of rules that govern how a certain content
''' type in a certain list or library behaves. Out of the box, a policy can have up 
''' to four policy items in it. These define 1. how user actions are audited, 2. how 
''' items are labelled, 3. how items get barcodes for id purposes, 4. How long items
''' are retained and what happens when they expire. Each of these 4 is called a Policy
''' Feature and you can define extra policy features in code. 
''' For the retention and expiration policy feature, you can define a custom Expiration 
''' Action. There are 8 out-of-the-box Expiration Actions, such as Permanently Delete 
''' and Declare Record. This code sample creates and deploys a custom expiration action.
''' </summary>
''' <remarks>
''' To create and deploy a custom expiration action you must define a class that 
''' implements the IExpirationAction interface and its OnExpiration method. Then use
''' a feature receiver to install the action. 
''' </remarks>
Public Class ExampleExpirationAction
    Implements IExpirationAction

    Private expiredItem As SPListItem

    Public Sub OnExpiration(ByVal item As Microsoft.SharePoint.SPListItem, ByVal parametersData As System.Xml.XmlNode, ByVal expiredDate As Date) Implements Microsoft.Office.RecordsManagement.PolicyFeatures.IExpirationAction.OnExpiration

        expiredItem = item
        SPSecurity.RunWithElevatedPrivileges(AddressOf AddAnnouncement)

    End Sub

    Private Sub AddAnnouncement()

        Using currentWeb As SPWeb = expiredItem.Web
            Dim announcementsList As SPList = currentWeb.Lists("Announcements")
            Dim newAnnouncement As SPListItem = announcementsList.Items.Add()
            newAnnouncement("Title") = "An item has expired"
            newAnnouncement("Body") = "Item is at: " + expiredItem.Url.ToString()
            newAnnouncement.Update()
        End Using

    End Sub

End Class
