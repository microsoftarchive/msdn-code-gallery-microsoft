Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security

''' <summary>
''' Content Types are often created declaratively. In SharePoint 2010 you can also create
''' them in code. If you do this, you get extra functionality. For example, you get more
''' control over upgrades. In this example we'll add a new Site Column and a custom 
''' Content Type in Feature Receiver code.
''' </summary>
''' <remarks>
''' The GUID attached to this class may be used during packaging and should not be modified.
''' </remarks>

<GuidAttribute("8287f38f-5b42-44a8-9e1c-7997084d4338")> _
Public Class ContentTypeFeatureEventReceiver 
    Inherits SPFeatureReceiver

    'This GUID uniquely idenitifies the custom field
    Public Shared ReadOnly MyFieldId As Guid = New Guid("B9EE12F5-B540-4F11-A21B-68A524014C45")
    'This is the XML used to create the field
    Public Shared ReadOnly MyFieldDefXml As String = _
         "<Field ID=""{B9EE12F5-B540-4F11-A21B-68A524014C43}""" + _
         " Name=""ContosoProductName"" StaticName=""ContosoProductName""" + _
         " Type=""Text"" DisplayName=""Contoso Product Name""" + _
         " Group=""Product Columns"" DisplaceOnUpgrade=""TRUE"" />"

    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
        'Get references to the site and web, ensuring correct disposal
        Using site As SPSite = DirectCast(properties.Feature.Parent, SPSite)

            Using currentWeb As SPWeb = site.RootWeb

                'Check if the custom field already exists.
                If currentWeb.AvailableFields.Contains(MyFieldId) = False Then
                    'Create the new field
                    currentWeb.Fields.AddFieldAsXml(MyFieldDefXml)
                    currentWeb.Update()
                End If

                'Check if the content type already exists
                Dim myContentType As SPContentType = currentWeb.ContentTypes("Product Announcement Content Type")
                If myContentType Is Nothing Then
                    'Our content type will be based on the Annoucement content type
                    Dim announcementContentType As SPContentType = currentWeb.AvailableContentTypes(SPBuiltInContentTypeId.Announcement)

                    'Create the new content type
                    myContentType = New SPContentType(announcementContentType, currentWeb.ContentTypes, "Product Announcement Content Type")

                    'Add the custom field to it
                    Dim newFieldLink As SPFieldLink = New SPFieldLink(currentWeb.AvailableFields("Contoso Product Name"))
                    myContentType.FieldLinks.Add(newFieldLink)

                    'Add the new content type to the site
                    currentWeb.ContentTypes.Add(myContentType)
                    currentWeb.Update()

                    'Add it to the Announcements list
                    Dim annoucementsList As SPList = currentWeb.Lists("Announcements")
                    annoucementsList.ContentTypesEnabled = True
                    annoucementsList.ContentTypes.Add(myContentType)
                    annoucementsList.Update()
                End If

            End Using

        End Using

    End Sub

    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)

        'Get references to the site and web, ensuring correct disposal
        Using site As SPSite = DirectCast(properties.Feature.Parent, SPSite)

            Using currentWeb As SPWeb = site.RootWeb

                'get the custom content type 
                Dim myContentType As SPContentType = currentWeb.ContentTypes("Product Announcement Content Type")
                If myContentType IsNot Nothing Then
                    'Remove it from the Announcements list
                    Dim annoucementsList As SPList = currentWeb.Lists("Announcements")
                    annoucementsList.ContentTypesEnabled = True
                    Dim ctID As SPContentTypeId = annoucementsList.ContentTypes.BestMatch(myContentType.Id)
                    annoucementsList.ContentTypes.Delete(ctID)
                    annoucementsList.Update()

                    'Remove it from the site
                    currentWeb.ContentTypes.Delete(myContentType.Id)
                    currentWeb.Update()
                End If

                Try
                    'Remove the field
                    currentWeb.Fields.Delete("ContosoProductName")
                    currentWeb.Update()
                Catch ex As Exception
                    'Field was not in the collection
                End Try

            End Using

        End Using

    End Sub

End Class
