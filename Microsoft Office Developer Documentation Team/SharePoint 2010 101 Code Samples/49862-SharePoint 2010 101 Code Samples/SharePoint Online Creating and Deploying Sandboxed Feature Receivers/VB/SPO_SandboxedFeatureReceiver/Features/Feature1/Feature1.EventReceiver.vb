Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security

''' <summary>
''' This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
''' </summary>
''' <remarks>
''' The GUID attached to this class may be used during packaging and should not be modified.
''' </remarks>

<GuidAttribute("28c032fc-bb4a-4030-8ef3-bb6cacbebcfa")> _
Public Class Feature1EventReceiver 
    Inherits SPFeatureReceiver

    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
        'This example illustrates a sandboxed feature receiver
        'Because it works in the sandbox, you can use it in 
        'SharePoint Online by deploying its .wsp file to the 
        'solutions gallery. The feature receiver modifies the
        'description of any site where it is activated.

        'To test this solution before deployment, set the Site URL 
        'property of the project to match your test SharePoint farm, then
        'use F5. Try deactivating the deployed feature - the extra
        'description should disappear from the site homepage

        'To deploy this project to your SharePoint Online site, upload
        'the SPO_SandboxedWebPart.wsp solution file from the bin/debug
        'folder to your solution gallery. Then activate the solution.

        'using keyword ensures proper disposal
        Using currentWeb As SPWeb = DirectCast(properties.Feature.Parent, SPWeb)
            If checkSandbox() Then
                'The feature receiver is Sandboxed. Set the description.
                currentWeb.Description += " This description was set by a feature receiver running in the sandbox"
            Else
                'The feature receiver is not Sandboxed. Set the description.
                currentWeb.Description += " This description was set by a feature receiver running outside the sandbox"
            End If
            'Save the new description
            currentWeb.Update()
        End Using
        
    End Sub


    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)

        Using currentWeb As SPWeb = DirectCast(properties.Feature.Parent, SPWeb)
            If checkSandbox() Then
                'The feature receiver is Sandboxed. Set the description.
                currentWeb.Description = currentWeb.Description.Replace(" This description was set by a feature receiver running in the sandbox", String.Empty)
            Else
                'The feature receiver is not Sandboxed. Set the description.
                currentWeb.Description = currentWeb.Description.Replace(" This description was set by a feature receiver running outside the sandbox", String.Empty)
            End If
            'Save the new description
            currentWeb.Update()
        End Using

    End Sub

    Private Function checkSandbox() As Boolean
        'This method returns true only if the code is running in the sandbox
        If System.AppDomain.CurrentDomain.FriendlyName.Contains("Sandbox") Then
            Return True
        Else
            Return False
        End If
    End Function


End Class
