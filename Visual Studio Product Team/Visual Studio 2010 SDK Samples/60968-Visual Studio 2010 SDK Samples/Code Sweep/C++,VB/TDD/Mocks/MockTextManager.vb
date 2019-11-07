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
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockTextManager
        Implements IVsTextManager
        Private ReadOnly _views As New Dictionary(Of String, MockTextView)()

        Public Function AddView(ByVal fileName As String) As MockTextView
            Dim view As New MockTextView()
            _views.Add(fileName, view)
            Return view
        End Function

#Region "IVsTextManager Members"

        Public Function AdjustFileChangeIgnoreCount(ByVal pBuffer As IVsTextBuffer, ByVal fIgnore As Integer) As Integer Implements IVsTextManager.AdjustFileChangeIgnoreCount
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function AttemptToCheckOutBufferFromScc(ByVal pBufData As IVsUserData, <System.Runtime.InteropServices.Out()> ByRef pfCheckoutSucceeded As Integer) As Integer Implements IVsTextManager.AttemptToCheckOutBufferFromScc
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function AttemptToCheckOutBufferFromScc2(ByVal pszFileName As String, <System.Runtime.InteropServices.Out()> ByRef pfCheckoutSucceeded As Integer, <System.Runtime.InteropServices.Out()> ByRef piStatusFlags As Integer) As Integer Implements IVsTextManager.AttemptToCheckOutBufferFromScc2
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CreateSelectionAction(ByVal pBuffer As IVsTextBuffer, <System.Runtime.InteropServices.Out()> ByRef ppAction As IVsTextSelectionAction) As Integer Implements IVsTextManager.CreateSelectionAction
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnumBuffers(<System.Runtime.InteropServices.Out()> ByRef ppEnum As IVsEnumTextBuffers) As Integer Implements IVsTextManager.EnumBuffers
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnumIndependentViews(ByVal pBuffer As IVsTextBuffer, <System.Runtime.InteropServices.Out()> ByRef ppEnum As IVsEnumIndependentViews) As Integer Implements IVsTextManager.EnumIndependentViews
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnumLanguageServices(<System.Runtime.InteropServices.Out()> ByRef ppEnum As IVsEnumGUID) As Integer Implements IVsTextManager.EnumLanguageServices
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnumViews(ByVal pBuffer As IVsTextBuffer, <System.Runtime.InteropServices.Out()> ByRef ppEnum As IVsEnumTextViews) As Integer Implements IVsTextManager.EnumViews
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetActiveView(ByVal fMustHaveFocus As Integer, ByVal pBuffer As IVsTextBuffer, <System.Runtime.InteropServices.Out()> ByRef ppView As IVsTextView) As Integer Implements IVsTextManager.GetActiveView
            Dim fileName As String = (CType(pBuffer, MockTextLines)).FileName

            If _views.ContainsKey(fileName) Then
                ppView = _views(fileName)
                Return VSConstants.S_OK
            Else
                ppView = Nothing
                Return VSConstants.E_INVALIDARG
            End If
        End Function

        Public Function GetBufferSccStatus(ByVal pBufData As IVsUserData, <System.Runtime.InteropServices.Out()> ByRef pbNonEditable As Integer) As Integer Implements IVsTextManager.GetBufferSccStatus
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetBufferSccStatus2(ByVal pszFileName As String, <System.Runtime.InteropServices.Out()> ByRef pbNonEditable As Integer, <System.Runtime.InteropServices.Out()> ByRef piStatusFlags As Integer) As Integer Implements IVsTextManager.GetBufferSccStatus2
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetMarkerTypeCount(<System.Runtime.InteropServices.Out()> ByRef piMarkerTypeCount As Integer) As Integer Implements IVsTextManager.GetMarkerTypeCount
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetMarkerTypeInterface(ByVal iMarkerTypeID As Integer, <System.Runtime.InteropServices.Out()> ByRef ppMarkerType As IVsTextMarkerType) As Integer Implements IVsTextManager.GetMarkerTypeInterface
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetPerLanguagePreferences(ByVal pLangPrefs As LANGPREFERENCES()) As Integer Implements IVsTextManager.GetPerLanguagePreferences
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetRegisteredMarkerTypeID(ByRef pguidMarker As Guid, <System.Runtime.InteropServices.Out()> ByRef piMarkerTypeID As Integer) As Integer Implements IVsTextManager.GetRegisteredMarkerTypeID
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetShortcutManager(<System.Runtime.InteropServices.Out()> ByRef ppShortcutMgr As IVsShortcutManager) As Integer Implements IVsTextManager.GetShortcutManager
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetUserPreferences(ByVal pViewPrefs As VIEWPREFERENCES(), ByVal pFramePrefs As FRAMEPREFERENCES(), ByVal pLangPrefs As LANGPREFERENCES(), ByVal pColorPrefs As FONTCOLORPREFERENCES()) As Integer Implements IVsTextManager.GetUserPreferences
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IgnoreNextFileChange(ByVal pBuffer As IVsTextBuffer) As Integer Implements IVsTextManager.IgnoreNextFileChange
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function MapFilenameToLanguageSID(ByVal pszFileName As String, <System.Runtime.InteropServices.Out()> ByRef pguidLangSID As Guid) As Integer Implements IVsTextManager.MapFilenameToLanguageSID
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function NavigateToLineAndColumn(ByVal pBuffer As IVsTextBuffer, ByRef guidDocViewType As Guid, ByVal iStartRow As Integer, ByVal iStartIndex As Integer, ByVal iEndRow As Integer, ByVal iEndIndex As Integer) As Integer Implements IVsTextManager.NavigateToLineAndColumn
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function NavigateToPosition(ByVal pBuffer As IVsTextBuffer, ByRef guidDocViewType As Guid, ByVal iPos As Integer, ByVal iLen As Integer) As Integer Implements IVsTextManager.NavigateToPosition
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RegisterBuffer(ByVal pBuffer As IVsTextBuffer) As Integer Implements IVsTextManager.RegisterBuffer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RegisterIndependentView(ByVal pUnk As Object, ByVal pBuffer As IVsTextBuffer) As Integer Implements IVsTextManager.RegisterIndependentView
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RegisterView(ByVal pView As IVsTextView, ByVal pBuffer As IVsTextBuffer) As Integer Implements IVsTextManager.RegisterView
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetFileChangeAdvise(ByVal pszFileName As String, ByVal fStart As Integer) As Integer Implements IVsTextManager.SetFileChangeAdvise
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetPerLanguagePreferences(ByVal pLangPrefs As LANGPREFERENCES()) As Integer Implements IVsTextManager.SetPerLanguagePreferences
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetUserPreferences(ByVal pViewPrefs As VIEWPREFERENCES(), ByVal pFramePrefs As FRAMEPREFERENCES(), ByVal pLangPrefs As LANGPREFERENCES(), ByVal pColorPrefs As FONTCOLORPREFERENCES()) As Integer Implements IVsTextManager.SetUserPreferences
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SuspendFileChangeAdvise(ByVal pszFileName As String, ByVal fSuspend As Integer) As Integer Implements IVsTextManager.SuspendFileChangeAdvise
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnregisterBuffer(ByVal pBuffer As IVsTextBuffer) As Integer Implements IVsTextManager.UnregisterBuffer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnregisterIndependentView(ByVal pUnk As Object, ByVal pBuffer As IVsTextBuffer) As Integer Implements IVsTextManager.UnregisterIndependentView
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnregisterView(ByVal pView As IVsTextView) As Integer Implements IVsTextManager.UnregisterView
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
