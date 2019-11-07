Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Administration
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.UserCode

''' <summary>
''' This feature receiver installs the custom solution validator on feature activation.
''' It also cleans up on feature deactivation
''' </summary>
''' <remarks>
''' The GUID attached to this class may be used during packaging and should not be modified.
''' </remarks>
<GuidAttribute("b390b919-709f-4309-940e-4aa8ec976436")> _
Public Class SolutionValidatorFeatureEventReceiver 
    Inherits SPFeatureReceiver


    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
        'Add the Custom Solution Validators to the Solution Validators collection
        SPUserCodeService.Local.SolutionValidators.Add(New DemoSolutionValidator(SPUserCodeService.Local))
    End Sub

    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
        'Remove the Solution Validator. The Guid is the one that marks the DemoSolutionValidator class
        SPUserCodeService.Local.SolutionValidators.Remove(New Guid("93DA2F64-4438-4547-B969-226834DB9AB5"))
    End Sub

End Class
