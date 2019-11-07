Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Administration
Imports Microsoft.SharePoint.Security

<GuidAttribute("db6ea9eb-f71c-45cf-b8c3-1dd06ec007ba")> _
Public Class ExampleWorkflowActivityFeatureEventReceiver 
    Inherits SPFeatureReceiver


    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
        'Get the current web application
        Dim currentWebApp As SPWebApplication = TryCast(properties.Feature.Parent, SPWebApplication)

        'Create a config modification object and set its properties
        Dim modification As SPWebConfigModification = New SPWebConfigModification()
        modification.Name = "AuthType"
        modification.Owner = "ExampleActivityLibrary"
        modification.Path = "configuration/System.Workflow.ComponentModel.WorkflowCompiler/authorizedTypes"
        modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode
        modification.Value = "<authorizedType Assembly=""ExampleActivityLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=dc326d8658ad3d11"" " + _
            "Namespace=""ExampleActivityLibrary"" TypeName=""*"" Authorized=""True"" />"

        'Add and apply the modification
        currentWebApp.WebConfigModifications.Add(modification)
        currentWebApp.WebService.ApplyWebConfigModifications()

    End Sub


        'Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
        'End Sub
End Class
