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
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.TextManager.Interop

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockWindowFrame
        Implements IVsWindowFrame
        Public TextLines As MockTextLines = Nothing

#Region "IVsWindowFrame Members"

        Public Function CloseFrame(ByVal grfSaveOptions As UInteger) As Integer Implements IVsWindowFrame.CloseFrame
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetFramePos(ByVal pdwSFP As VSSETFRAMEPOS(), <System.Runtime.InteropServices.Out()> ByRef pguidRelativeTo As Guid, <System.Runtime.InteropServices.Out()> ByRef px As Integer, <System.Runtime.InteropServices.Out()> ByRef py As Integer, <System.Runtime.InteropServices.Out()> ByRef pcx As Integer, <System.Runtime.InteropServices.Out()> ByRef pcy As Integer) As Integer Implements IVsWindowFrame.GetFramePos
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetGuidProperty(ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pguid As Guid) As Integer Implements IVsWindowFrame.GetGuidProperty
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetProperty(ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pvar As Object) As Integer Implements IVsWindowFrame.GetProperty
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Hide() As Integer Implements IVsWindowFrame.Hide
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IsOnScreen(<System.Runtime.InteropServices.Out()> ByRef pfOnScreen As Integer) As Integer Implements IVsWindowFrame.IsOnScreen
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IsVisible() As Integer Implements IVsWindowFrame.IsVisible
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function QueryViewInterface(ByRef riid As Guid, <System.Runtime.InteropServices.Out()> ByRef ppv As IntPtr) As Integer Implements IVsWindowFrame.QueryViewInterface
            If riid = GetType(IVsTextLines).GUID Then
                ppv = Marshal.GetIUnknownForObject(TextLines)
                Return VSConstants.S_OK
            Else
                ppv = IntPtr.Zero
                Return VSConstants.E_NOINTERFACE
            End If
        End Function

        Public Function SetFramePos(ByVal dwSFP As VSSETFRAMEPOS, ByRef rguidRelativeTo As Guid, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer) As Integer Implements IVsWindowFrame.SetFramePos
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetGuidProperty(ByVal propid As Integer, ByRef rguid As Guid) As Integer Implements IVsWindowFrame.SetGuidProperty
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetProperty(ByVal propid As Integer, ByVal var As Object) As Integer Implements IVsWindowFrame.SetProperty
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Show() As Integer Implements IVsWindowFrame.Show
            Return VSConstants.S_OK
        End Function

        Public Function ShowNoActivate() As Integer Implements IVsWindowFrame.ShowNoActivate
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
