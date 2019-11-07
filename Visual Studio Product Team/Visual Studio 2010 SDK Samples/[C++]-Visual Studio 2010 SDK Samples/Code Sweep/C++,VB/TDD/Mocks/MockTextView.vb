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
    Friend Class MockTextView
        Implements IVsTextView
        Public Class SetCaretPosEventArgs
            Inherits EventArgs
            Public ReadOnly Line As Integer
            Public ReadOnly Column As Integer
            Public Sub New(ByVal line As Integer, ByVal column As Integer)
                Me.Line = line
                Me.Column = column
            End Sub
        End Class

        Public Event OnSetCaretPos As EventHandler(Of SetCaretPosEventArgs)

#Region "IVsTextView Members"

        Public Function AddCommandFilter(ByVal pNewCmdTarg As Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget, <System.Runtime.InteropServices.Out()> ByRef ppNextCmdTarg As Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget) As Integer Implements IVsTextView.AddCommandFilter
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CenterColumns(ByVal iLine As Integer, ByVal iLeftCol As Integer, ByVal iColCount As Integer) As Integer Implements IVsTextView.CenterColumns
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CenterLines(ByVal iTopLine As Integer, ByVal iCount As Integer) As Integer Implements IVsTextView.CenterLines
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ClearSelection(ByVal fMoveToAnchor As Integer) As Integer Implements IVsTextView.ClearSelection
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CloseView() As Integer Implements IVsTextView.CloseView
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnsureSpanVisible(ByVal span As TextSpan) As Integer Implements IVsTextView.EnsureSpanVisible
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetBuffer(<System.Runtime.InteropServices.Out()> ByRef ppBuffer As IVsTextLines) As Integer Implements IVsTextView.GetBuffer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetCaretPos(<System.Runtime.InteropServices.Out()> ByRef piLine As Integer, <System.Runtime.InteropServices.Out()> ByRef piColumn As Integer) As Integer Implements IVsTextView.GetCaretPos
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLineAndColumn(ByVal iPos As Integer, <System.Runtime.InteropServices.Out()> ByRef piLine As Integer, <System.Runtime.InteropServices.Out()> ByRef piIndex As Integer) As Integer Implements IVsTextView.GetLineAndColumn
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLineHeight(<System.Runtime.InteropServices.Out()> ByRef piLineHeight As Integer) As Integer Implements IVsTextView.GetLineHeight
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetNearestPosition(ByVal iLine As Integer, ByVal iCol As Integer, <System.Runtime.InteropServices.Out()> ByRef piPos As Integer, <System.Runtime.InteropServices.Out()> ByRef piVirtualSpaces As Integer) As Integer Implements IVsTextView.GetNearestPosition
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetPointOfLineColumn(ByVal iLine As Integer, ByVal iCol As Integer, ByVal ppt As Microsoft.VisualStudio.OLE.Interop.POINT()) As Integer Implements IVsTextView.GetPointOfLineColumn
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetScrollInfo(ByVal iBar As Integer, <System.Runtime.InteropServices.Out()> ByRef piMinUnit As Integer, <System.Runtime.InteropServices.Out()> ByRef piMaxUnit As Integer, <System.Runtime.InteropServices.Out()> ByRef piVisibleUnits As Integer, <System.Runtime.InteropServices.Out()> ByRef piFirstVisibleUnit As Integer) As Integer Implements IVsTextView.GetScrollInfo
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetSelectedText(<System.Runtime.InteropServices.Out()> ByRef pbstrText As String) As Integer Implements IVsTextView.GetSelectedText
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetSelection(<System.Runtime.InteropServices.Out()> ByRef piAnchorLine As Integer, <System.Runtime.InteropServices.Out()> ByRef piAnchorCol As Integer, <System.Runtime.InteropServices.Out()> ByRef piEndLine As Integer, <System.Runtime.InteropServices.Out()> ByRef piEndCol As Integer) As Integer Implements IVsTextView.GetSelection
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetSelectionDataObject(<System.Runtime.InteropServices.Out()> ByRef ppIDataObject As Microsoft.VisualStudio.OLE.Interop.IDataObject) As Integer Implements IVsTextView.GetSelectionDataObject
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetSelectionMode() As TextSelMode Implements IVsTextView.GetSelectionMode
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetSelectionSpan(ByVal pSpan As TextSpan()) As Integer Implements IVsTextView.GetSelectionSpan
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetTextStream(ByVal iTopLine As Integer, ByVal iTopCol As Integer, ByVal iBottomLine As Integer, ByVal iBottomCol As Integer, <System.Runtime.InteropServices.Out()> ByRef pbstrText As String) As Integer Implements IVsTextView.GetTextStream
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetWindowHandle() As IntPtr Implements IVsTextView.GetWindowHandle
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetWordExtent(ByVal iLine As Integer, ByVal iCol As Integer, ByVal dwFlags As UInteger, ByVal pSpan As TextSpan()) As Integer Implements IVsTextView.GetWordExtent
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function HighlightMatchingBrace(ByVal dwFlags As UInteger, ByVal cSpans As UInteger, ByVal rgBaseSpans As TextSpan()) As Integer Implements IVsTextView.HighlightMatchingBrace
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Initialize(ByVal pBuffer As IVsTextLines, ByVal hwndParent As IntPtr, ByVal InitFlags As UInteger, ByVal pInitView As INITVIEW()) As Integer Implements IVsTextView.Initialize
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function PositionCaretForEditing(ByVal iLine As Integer, ByVal cIndentLevels As Integer) As Integer Implements IVsTextView.PositionCaretForEditing
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RemoveCommandFilter(ByVal pCmdTarg As Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget) As Integer Implements IVsTextView.RemoveCommandFilter
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ReplaceTextOnLine(ByVal iLine As Integer, ByVal iStartCol As Integer, ByVal iCharsToReplace As Integer, ByVal pszNewText As String, ByVal iNewLen As Integer) As Integer Implements IVsTextView.ReplaceTextOnLine
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RestrictViewRange(ByVal iMinLine As Integer, ByVal iMaxLine As Integer, ByVal pClient As IVsViewRangeClient) As Integer Implements IVsTextView.RestrictViewRange
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SendExplicitFocus() As Integer Implements IVsTextView.SendExplicitFocus
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetBuffer(ByVal pBuffer As IVsTextLines) As Integer Implements IVsTextView.SetBuffer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetCaretPos(ByVal iLine As Integer, ByVal iColumn As Integer) As Integer Implements IVsTextView.SetCaretPos
            If OnSetCaretPosEvent IsNot Nothing Then
                RaiseEvent OnSetCaretPos(Me, New SetCaretPosEventArgs(iLine, iColumn))
            End If
            Return VSConstants.S_OK
        End Function

        Public Function SetScrollPosition(ByVal iBar As Integer, ByVal iFirstVisibleUnit As Integer) As Integer Implements IVsTextView.SetScrollPosition
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetSelection(ByVal iAnchorLine As Integer, ByVal iAnchorCol As Integer, ByVal iEndLine As Integer, ByVal iEndCol As Integer) As Integer Implements IVsTextView.SetSelection
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetSelectionMode(ByVal iSelMode As TextSelMode) As Integer Implements IVsTextView.SetSelectionMode
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetTopLine(ByVal iBaseLine As Integer) As Integer Implements IVsTextView.SetTopLine
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UpdateCompletionStatus(ByVal pCompSet As IVsCompletionSet, ByVal dwFlags As UInteger) As Integer Implements IVsTextView.UpdateCompletionStatus
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UpdateTipWindow(ByVal pTipWindow As IVsTipWindow, ByVal dwFlags As UInteger) As Integer Implements IVsTextView.UpdateTipWindow
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UpdateViewFrameCaption() As Integer Implements IVsTextView.UpdateViewFrameCaption
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
