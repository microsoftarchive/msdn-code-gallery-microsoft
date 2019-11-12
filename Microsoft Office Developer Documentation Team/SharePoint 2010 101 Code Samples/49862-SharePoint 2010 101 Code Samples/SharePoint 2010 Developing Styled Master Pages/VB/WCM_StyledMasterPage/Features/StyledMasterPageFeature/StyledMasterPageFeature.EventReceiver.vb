Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security

<GuidAttribute("3e42e481-5a18-4612-8718-65747d0902c5")> _
Public Class StyledMasterPageFeatureEventReceiver 
    Inherits SPFeatureReceiver

    'When the feature is activated set the default and custom master page
    'properties to the new master page.
    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
        'Get the current web site, ensuring proper disposal
        Using currentWeb As SPWeb = TryCast(properties.Feature.Parent, SPWeb)
            'Set the master page values and update the SPWeb
            currentWeb.MasterUrl = "/_catalogs/masterpage/StyledExample.master"
            currentWeb.CustomMasterUrl = "/_catalogs/masterpage/StyledExample.master"
            currentWeb.Update()
        End Using
    End Sub

    'When the feature is deactivated clean up by setting the master page back to v4.master
    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
        'We must set the master page back to the default: v4.master
        Using currentWeb As SPWeb = TryCast(properties.Feature.Parent, SPWeb)
            currentWeb.MasterUrl = "/_catalogs/masterpage/v4.master"
            currentWeb.CustomMasterUrl = "/_catalogs/masterpage/v4.master"
            currentWeb.Update()
        End Using
    End Sub

End Class
