'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockWebBrowsingService
        Implements IVsWebBrowsingService
        Public Class NavigateEventArgs
            Inherits EventArgs
            Public ReadOnly Url As String
            Public Sub New(ByVal url As String)
                Me.Url = url
            End Sub
        End Class

        Public Event OnNavigate As EventHandler(Of NavigateEventArgs)

#Region "IVsWebBrowsingService Members"

        Public Function CreateExternalWebBrowser(ByVal dwCreateFlags As UInteger, ByVal dwResolution As VSPREVIEWRESOLUTION, ByVal lpszURL As String) As Integer Implements IVsWebBrowsingService.CreateExternalWebBrowser
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CreateWebBrowser(ByVal dwCreateFlags As UInteger, ByRef rguidOwner As Guid, ByVal lpszBaseCaption As String, ByVal lpszStartURL As String, ByVal pUser As IVsWebBrowserUser, <System.Runtime.InteropServices.Out()> ByRef ppBrowser As IVsWebBrowser, <System.Runtime.InteropServices.Out()> ByRef ppFrame As IVsWindowFrame) As Integer Implements IVsWebBrowsingService.CreateWebBrowser
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CreateWebBrowserEx(ByVal dwCreateFlags As UInteger, ByRef rguidPersistenceSlot As Guid, ByVal dwId As UInteger, ByVal lpszBaseCaption As String, ByVal lpszStartURL As String, ByVal pUser As IVsWebBrowserUser, <System.Runtime.InteropServices.Out()> ByRef ppBrowser As IVsWebBrowser, <System.Runtime.InteropServices.Out()> ByRef ppFrame As IVsWindowFrame) As Integer Implements IVsWebBrowsingService.CreateWebBrowserEx
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetFirstWebBrowser(ByRef rguidPersistenceSlot As Guid, <System.Runtime.InteropServices.Out()> ByRef ppFrame As IVsWindowFrame, <System.Runtime.InteropServices.Out()> ByRef ppBrowser As IVsWebBrowser) As Integer Implements IVsWebBrowsingService.GetFirstWebBrowser
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetWebBrowserEnum(ByRef rguidPersistenceSlot As Guid, <System.Runtime.InteropServices.Out()> ByRef ppenum As IEnumWindowFrames) As Integer Implements IVsWebBrowsingService.GetWebBrowserEnum
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Navigate(ByVal lpszURL As String, ByVal dwNaviageFlags As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppFrame As IVsWindowFrame) As Integer Implements IVsWebBrowsingService.Navigate
            If OnNavigateEvent IsNot Nothing Then
                RaiseEvent OnNavigate(Me, New NavigateEventArgs(lpszURL))
            End If

            ppFrame = New MockWindowFrame()
            Return VSConstants.S_OK
        End Function

#End Region
    End Class
End Namespace
