Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security

''' <summary>
''' This class, inheriting from SPFeatureReceiver, can include methods that execute
''' when a feature is activated, deactivating, installed, and uninstalling. Commonly,
''' code is run in the FeatureActivated handler to complete the set up of a feature.
''' In FeatureDeactivating it is important to clean up any changes made in FeatureActivated.
''' In Visual Studio, to add a Feature Receiver, right-click a Feature and then click 
''' "Add Event Receiver". 
''' </summary>
''' <remarks>
''' The GUID attached to this class may be used during packaging and should not be modified.
''' </remarks>

<GuidAttribute("79ea7882-4f93-4d69-b4cb-2113bb1639d3")> _
Public Class ExampleFeatureEventReceiver 
    Inherits SPFeatureReceiver

    'This event executes just after the feature is activated
    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
        'This feature has the Web scope, so the properties.Feature.Parent property returns an SPWeb object
        Dim currentWeb As SPWeb = DirectCast(properties.Feature.Parent, SPWeb)
        'NOTE: If the scope was Site, Parent would return a SPSite.
        'If the scope was Web Application, Parent would return a SPWebApplication.
        currentWeb.Description += " The Example Feature is activated!"
        currentWeb.Update()
    End Sub

    'This event fires when a user deactivates the feature
    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
        'In this hanlder you must make sure you clean up the changes you made in FeatureActivated
        Dim currentWeb As SPWeb = DirectCast(properties.Feature.Parent, SPWeb)
        currentWeb.Description = currentWeb.Description.Replace(" The Example Feature is activated!", String.Empty)
        currentWeb.Update()
    End Sub

End Class
