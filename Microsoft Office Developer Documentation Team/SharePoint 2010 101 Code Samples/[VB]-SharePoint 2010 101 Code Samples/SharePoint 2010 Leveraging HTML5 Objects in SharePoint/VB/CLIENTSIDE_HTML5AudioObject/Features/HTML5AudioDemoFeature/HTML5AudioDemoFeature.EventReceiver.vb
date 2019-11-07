Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security

<GuidAttribute("0d1889b1-9b87-4c54-80a0-a91eea5a56ee")> _
Public Class HTML5AudioDemoFeatureEventReceiver 
    Inherits SPFeatureReceiver

    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)

        'Get the current site collection and web site, ensuring proper disposal
        Using currentSite As SPSite = TryCast(properties.Feature.Parent, SPSite)

            Using currentWeb As SPWeb = currentSite.RootWeb

                'Set the master page values and update the SPWeb
                currentWeb.MasterUrl = "/_catalogs/masterpage/HTML5.v4.master"
                currentWeb.CustomMasterUrl = "/_catalogs/masterpage/HTML5.v4.master"
                currentWeb.Update()

            End Using

        End Using

    End Sub

    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)

        'We must set the master page back to the default: v4.master
        Using currentSite As SPSite = TryCast(properties.Feature.Parent, SPSite)

            Using currentWeb As SPWeb = currentSite.RootWeb

                currentWeb.MasterUrl = "/_catalogs/masterpage/v4.master"
                currentWeb.CustomMasterUrl = "/_catalogs/masterpage/v4.master"
                currentWeb.Update()

            End Using

        End Using

    End Sub

End Class
