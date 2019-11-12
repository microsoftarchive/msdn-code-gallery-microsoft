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
    Friend Class MockStatusBar
        Implements IVsStatusbar
        Public Class ProgressArgs
            Inherits EventArgs
            Public ReadOnly Cookie As UInteger
            Public ReadOnly InProgress As Integer
            Public ReadOnly Label As String
            Public ReadOnly Complete As UInteger
            Public ReadOnly Total As UInteger
            Public Sub New(ByVal cookie As UInteger, ByVal inProgress As Integer, ByVal label As String, ByVal complete As UInteger, ByVal total As UInteger)
                Me.Cookie = cookie
                Me.InProgress = inProgress
                Me.Label = label
                Me.Complete = complete
                Me.Total = total
            End Sub
        End Class
        Public Event OnProgress As EventHandler(Of ProgressArgs)

#Region "IVsStatusbar Members"

        Public Function Animation(ByVal fOnOff As Integer, ByRef pvIcon As Object) As Integer Implements IVsStatusbar.Animation
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Clear() As Integer Implements IVsStatusbar.Clear
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function FreezeOutput(ByVal fFreeze As Integer) As Integer Implements IVsStatusbar.FreezeOutput
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetFreezeCount(<System.Runtime.InteropServices.Out()> ByRef plCount As Integer) As Integer Implements IVsStatusbar.GetFreezeCount
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetText(<System.Runtime.InteropServices.Out()> ByRef pszText As String) As Integer Implements IVsStatusbar.GetText
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IsCurrentUser(ByVal pUser As IVsStatusbarUser, ByRef pfCurrent As Integer) As Integer Implements IVsStatusbar.IsCurrentUser
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IsFrozen(<System.Runtime.InteropServices.Out()> ByRef pfFrozen As Integer) As Integer Implements IVsStatusbar.IsFrozen
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Progress(ByRef pdwCookie As UInteger, ByVal fInProgress As Integer, ByVal pwszLabel As String, ByVal nComplete As UInteger, ByVal nTotal As UInteger) As Integer Implements IVsStatusbar.Progress
            If OnProgressEvent IsNot Nothing Then
                RaiseEvent OnProgress(Me, New ProgressArgs(pdwCookie, fInProgress, pwszLabel, nComplete, nTotal))
            End If
            Return VSConstants.S_OK
        End Function

        Public Function SetColorText(ByVal pszText As String, ByVal crForeColor As UInteger, ByVal crBackColor As UInteger) As Integer Implements IVsStatusbar.SetColorText
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetInsMode(ByRef pvInsMode As Object) As Integer Implements IVsStatusbar.SetInsMode
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetLineChar(ByRef pvLine As Object, ByRef pvChar As Object) As Integer Implements IVsStatusbar.SetLineChar
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetLineColChar(ByRef pvLine As Object, ByRef pvCol As Object, ByRef pvChar As Object) As Integer Implements IVsStatusbar.SetLineColChar
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetSelMode(ByRef pvSelMode As Object) As Integer Implements IVsStatusbar.SetSelMode
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetText(ByVal pszText As String) As Integer Implements IVsStatusbar.SetText
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetXYWH(ByRef pvX As Object, ByRef pvY As Object, ByRef pvW As Object, ByRef pvH As Object) As Integer Implements IVsStatusbar.SetXYWH
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
