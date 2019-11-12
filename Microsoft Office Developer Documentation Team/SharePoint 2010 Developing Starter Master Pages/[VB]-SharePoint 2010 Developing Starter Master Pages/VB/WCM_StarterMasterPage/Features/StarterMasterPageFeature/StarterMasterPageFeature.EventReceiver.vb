Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security

''' <summary>
''' This feature receiver sets the master page of the current site to be the
''' master page deployed by the StarterMasterPageModule.
''' </summary>

<GuidAttribute("60e066dd-0263-41e8-89ad-fc6b9b32bc5b")> _
Public Class StarterMasterPageFeatureEventReceiver 
    Inherits SPFeatureReceiver

    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
        'Get the current web site, ensuring proper disposal
        Using currentWeb As SPWeb = TryCast(properties.Feature.Parent, SPWeb)
            'Set the master page values and update the SPWeb
            currentWeb.MasterUrl = "/_catalogs/masterpage/_starter_publishing.master"
            currentWeb.CustomMasterUrl = "/_catalogs/masterpage/_starter_publishing.master"
            currentWeb.Update()
        End Using
    End Sub

    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
        'We must set the master page back to the default: v4.master
        Using currentWeb As SPWeb = TryCast(properties.Feature.Parent, SPWeb)
            currentWeb.MasterUrl = "/_catalogs/masterpage/v4.master"
            currentWeb.CustomMasterUrl = "/_catalogs/masterpage/v4.master"
            currentWeb.Update()
        End Using
    End Sub

End Class
