Option Explicit On
Option Strict On

'/****************************** Module Header ******************************\
'* Module Name:    LangSwitcherFeatureEventReceiver.vb
'* Project:        VBSharePointLangSwitcher
'* Copyright (c) Microsoft Corporation
'*
'* Add or remove the SwitcherModule in Web.config.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************/
Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.Administration

''' <summary>
''' This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
''' </summary>
''' <remarks>
''' The GUID attached to this class may be used during packaging and should not be modified.
''' </remarks>

<GuidAttribute("cf4e4655-f3d1-497e-ac66-4d7a9691e378")> _
Public Class LangSwitcherFeatureEventReceiver
    Inherits SPFeatureReceiver
    Private Const WebConfigModificationOwner As String = "MyTestOwner"

    ' For not so obvious reasons web.config modifications inside collections 
    ' are added based on the value of the key attribute in alphabetic order.
    ' Because we need to add the DualLayout module after the 
    ' PublishingHttpModule, we prefix the name with 'Q-'.
    ' The owner of the web.config modification, useful for removing a 
    ' group of modifications
    ' Make sure that the name is a unique XPath selector for the element 
    ' we are adding. This name is used for removing the element
    ' We are going to add a new XML node to web.config
    ' The XPath to the location of the parent node in web.config
    ' Sequence is important if there are multiple equal nodes that 
    ' can't be identified with an XPath explression
    ' The XML to insert as child node, make sure that used names match the Name selector
    Private Shared ReadOnly Modifications As SPWebConfigModification() = {New SPWebConfigModification() With { _
        .Owner = WebConfigModificationOwner, _
        .Name = "add[@name='HTTPSwitcherModule']", _
        .Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode, _
        .Path = "configuration/system.webServer/modules", _
        .Sequence = 0, _
        .Value = "<add name='HTTPSwitcherModule' type='VBSharePointLangSwitcher.LangSwitcherPage.HTTPSwitcherModule, VBSharePointLangSwitcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f39f860f90694f6f' />" _
    }}

    Public Overrides Sub FeatureInstalled(properties As SPFeatureReceiverProperties)
    End Sub

    Public Overrides Sub FeatureUninstalling(properties As SPFeatureReceiverProperties)
    End Sub

    Public Overrides Sub FeatureActivated(properties As SPFeatureReceiverProperties)
        Dim sps As SPSite = DirectCast(properties.Feature.Parent, SPSite)
        Dim webApp As SPWebApplication = sps.WebApplication

        If webApp IsNot Nothing Then
            AddWebConfigModifications(webApp, Modifications)
        End If
    End Sub

    Public Overrides Sub FeatureDeactivating(properties As SPFeatureReceiverProperties)
        Dim webApp As SPWebApplication = TryCast(properties.Feature.Parent, SPWebApplication)
        If webApp IsNot Nothing Then
            RemoveWebConfigModificationsByOwner(webApp, WebConfigModificationOwner)
        End If
    End Sub

    ''' <summary>
    ''' Add a collection of web modifications to the web application.
    ''' </summary>
    ''' <param name="webApp">The web application to add the modifications to</param>
    ''' <param name="modifications">The collection of modifications</param>
    Private Sub AddWebConfigModifications(webApp As SPWebApplication, modifications As IEnumerable(Of SPWebConfigModification))
        For Each modification As SPWebConfigModification In modifications
            webApp.WebConfigModifications.Add(modification)
        Next

        ' Commit modification additions to the specified web application.
        webApp.Update()
        ' Push modifications through the farm.
        webApp.WebService.ApplyWebConfigModifications()
    End Sub

    ''' <summary>
    ''' Remove modifications from the web application.
    ''' </summary>
    ''' <param name="webApp">The web application to remove the modifications from.</param>
    ''' <param name="owner">Remove all modifications that belong to the owner.</param>
    Private Sub RemoveWebConfigModificationsByOwner(webApp As SPWebApplication, owner As String)
        Dim modificationCollection As System.Collections.ObjectModel.Collection(Of SPWebConfigModification) = webApp.WebConfigModifications
        Dim removeCollection As New System.Collections.ObjectModel.Collection(Of SPWebConfigModification)()

        Dim count As Integer = modificationCollection.Count
        For i As Integer = 0 To count - 1
            Dim modification As SPWebConfigModification = modificationCollection(i)
            If modification.Owner = owner Then
                ' Collect modifications to delete.
                removeCollection.Add(modification)
            End If
        Next

        ' Delete the modifications from the web application.
        If removeCollection.Count > 0 Then
            For Each modificationItem As SPWebConfigModification In removeCollection
                webApp.WebConfigModifications.Remove(modificationItem)
            Next

            ' Commit modification removals to the specified web application.
            webApp.Update()
            ' Push modifications through the farm.
            webApp.WebService.ApplyWebConfigModifications()
        End If
    End Sub
End Class