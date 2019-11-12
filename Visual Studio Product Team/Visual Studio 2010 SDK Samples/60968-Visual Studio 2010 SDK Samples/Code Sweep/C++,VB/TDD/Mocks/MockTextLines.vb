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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockTextLines
        Implements IVsTextLines
        Public ReadOnly FileName As String = Nothing

        Public Sub New(ByVal fileName As String)
            Me.FileName = fileName
        End Sub

#Region "IVsTextLines Members"

        Public Function AdviseTextLinesEvents(ByVal pSink As IVsTextLinesEvents, <System.Runtime.InteropServices.Out()> ByRef pdwCookie As UInteger) As Integer Implements IVsTextLines.AdviseTextLinesEvents
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CanReplaceLines(ByVal iStartLine As Integer, ByVal iStartIndex As Integer, ByVal iEndLine As Integer, ByVal iEndIndex As Integer, ByVal iNewLen As Integer) As Integer Implements IVsTextLines.CanReplaceLines
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CopyLineText(ByVal iStartLine As Integer, ByVal iStartIndex As Integer, ByVal iEndLine As Integer, ByVal iEndIndex As Integer, ByVal pszBuf As IntPtr, ByRef pcchBuf As Integer) As Integer Implements IVsTextLines.CopyLineText
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CreateEditPoint(ByVal iLine As Integer, ByVal iIndex As Integer, <System.Runtime.InteropServices.Out()> ByRef ppEditPoint As Object) As Integer Implements IVsTextLines.CreateEditPoint
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CreateLineMarker(ByVal iMarkerType As Integer, ByVal iStartLine As Integer, ByVal iStartIndex As Integer, ByVal iEndLine As Integer, ByVal iEndIndex As Integer, ByVal pClient As IVsTextMarkerClient, ByVal ppMarker As IVsTextLineMarker()) As Integer Implements IVsTextLines.CreateLineMarker
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CreateTextPoint(ByVal iLine As Integer, ByVal iIndex As Integer, <System.Runtime.InteropServices.Out()> ByRef ppTextPoint As Object) As Integer Implements IVsTextLines.CreateTextPoint
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnumMarkers(ByVal iStartLine As Integer, ByVal iStartIndex As Integer, ByVal iEndLine As Integer, ByVal iEndIndex As Integer, ByVal iMarkerType As Integer, ByVal dwFlags As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppEnum As IVsEnumLineMarkers) As Integer Implements IVsTextLines.EnumMarkers
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function FindMarkerByLineIndex(ByVal iMarkerType As Integer, ByVal iStartingLine As Integer, ByVal iStartingIndex As Integer, ByVal dwFlags As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppMarker As IVsTextLineMarker) As Integer Implements IVsTextLines.FindMarkerByLineIndex
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLanguageServiceID(<System.Runtime.InteropServices.Out()> ByRef pguidLangService As Guid) As Integer Implements IVsTextLines.GetLanguageServiceID, IVsTextBuffer.GetLanguageServiceID
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLastLineIndex(<System.Runtime.InteropServices.Out()> ByRef piLine As Integer, <System.Runtime.InteropServices.Out()> ByRef piIndex As Integer) As Integer Implements IVsTextLines.GetLastLineIndex, IVsTextBuffer.GetLastLineIndex
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLengthOfLine(ByVal iLine As Integer, <System.Runtime.InteropServices.Out()> ByRef piLength As Integer) As Integer Implements IVsTextLines.GetLengthOfLine, IVsTextBuffer.GetLengthOfLine
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLineCount(<System.Runtime.InteropServices.Out()> ByRef piLineCount As Integer) As Integer Implements IVsTextLines.GetLineCount, IVsTextBuffer.GetLineCount
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLineData(ByVal iLine As Integer, ByVal pLineData As LINEDATA(), ByVal pMarkerData As MARKERDATA()) As Integer Implements IVsTextLines.GetLineData
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLineDataEx(ByVal dwFlags As UInteger, ByVal iLine As Integer, ByVal iStartIndex As Integer, ByVal iEndIndex As Integer, ByVal pLineData As LINEDATAEX(), ByVal pMarkerData As MARKERDATA()) As Integer Implements IVsTextLines.GetLineDataEx
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLineIndexOfPosition(ByVal iPosition As Integer, <System.Runtime.InteropServices.Out()> ByRef piLine As Integer, <System.Runtime.InteropServices.Out()> ByRef piColumn As Integer) As Integer Implements IVsTextLines.GetLineIndexOfPosition, IVsTextBuffer.GetLineIndexOfPosition
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetLineText(ByVal iStartLine As Integer, ByVal iStartIndex As Integer, ByVal iEndLine As Integer, ByVal iEndIndex As Integer, <System.Runtime.InteropServices.Out()> ByRef pbstrBuf As String) As Integer Implements IVsTextLines.GetLineText
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetMarkerData(ByVal iTopLine As Integer, ByVal iBottomLine As Integer, ByVal pMarkerData As MARKERDATA()) As Integer Implements IVsTextLines.GetMarkerData
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetPairExtents(ByVal pSpanIn As TextSpan(), ByVal pSpanOut As TextSpan()) As Integer Implements IVsTextLines.GetPairExtents
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetPositionOfLine(ByVal iLine As Integer, <System.Runtime.InteropServices.Out()> ByRef piPosition As Integer) As Integer Implements IVsTextLines.GetPositionOfLine, IVsTextBuffer.GetPositionOfLine
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetPositionOfLineIndex(ByVal iLine As Integer, ByVal iIndex As Integer, <System.Runtime.InteropServices.Out()> ByRef piPosition As Integer) As Integer Implements IVsTextLines.GetPositionOfLineIndex, IVsTextBuffer.GetPositionOfLineIndex
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetSize(<System.Runtime.InteropServices.Out()> ByRef piLength As Integer) As Integer Implements IVsTextLines.GetSize, IVsTextBuffer.GetSize
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetStateFlags(<System.Runtime.InteropServices.Out()> ByRef pdwReadOnlyFlags As UInteger) As Integer Implements IVsTextLines.GetStateFlags, IVsTextBuffer.GetStateFlags
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetUndoManager(<System.Runtime.InteropServices.Out()> ByRef ppUndoManager As Microsoft.VisualStudio.OLE.Interop.IOleUndoManager) As Integer Implements IVsTextLines.GetUndoManager, IVsTextBuffer.GetUndoManager
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IVsTextLinesReserved1(ByVal iLine As Integer, ByVal pLineData As LINEDATA(), ByVal fAttributes As Integer) As Integer Implements IVsTextLines.IVsTextLinesReserved1
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function InitializeContent(ByVal pszText As String, ByVal iLength As Integer) As Integer Implements IVsTextLines.InitializeContent, IVsTextBuffer.InitializeContent
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function LockBuffer() As Integer Implements IVsTextLines.LockBuffer, IVsTextBuffer.LockBuffer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function LockBufferEx(ByVal dwFlags As UInteger) As Integer Implements IVsTextLines.LockBufferEx, IVsTextBuffer.LockBufferEx
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ReleaseLineData(ByVal pLineData As LINEDATA()) As Integer Implements IVsTextLines.ReleaseLineData
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ReleaseLineDataEx(ByVal pLineData As LINEDATAEX()) As Integer Implements IVsTextLines.ReleaseLineDataEx
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ReleaseMarkerData(ByVal pMarkerData As MARKERDATA()) As Integer Implements IVsTextLines.ReleaseMarkerData
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reload(ByVal fUndoable As Integer) As Integer Implements IVsTextLines.Reload, IVsTextBuffer.Reload
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ReloadLines(ByVal iStartLine As Integer, ByVal iStartIndex As Integer, ByVal iEndLine As Integer, ByVal iEndIndex As Integer, ByVal pszText As IntPtr, ByVal iNewLen As Integer, ByVal pChangedSpan As TextSpan()) As Integer Implements IVsTextLines.ReloadLines
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ReplaceLines(ByVal iStartLine As Integer, ByVal iStartIndex As Integer, ByVal iEndLine As Integer, ByVal iEndIndex As Integer, ByVal pszText As IntPtr, ByVal iNewLen As Integer, ByVal pChangedSpan As TextSpan()) As Integer Implements IVsTextLines.ReplaceLines
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ReplaceLinesEx(ByVal dwFlags As UInteger, ByVal iStartLine As Integer, ByVal iStartIndex As Integer, ByVal iEndLine As Integer, ByVal iEndIndex As Integer, ByVal pszText As IntPtr, ByVal iNewLen As Integer, ByVal pChangedSpan As TextSpan()) As Integer Implements IVsTextLines.ReplaceLinesEx
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved1() As Integer Implements IVsTextLines.Reserved1, IVsTextBuffer.Reserved1
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved10() As Integer Implements IVsTextLines.Reserved10, IVsTextBuffer.Reserved10
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved2() As Integer Implements IVsTextLines.Reserved2, IVsTextBuffer.Reserved2
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved3() As Integer Implements IVsTextLines.Reserved3, IVsTextBuffer.Reserved3
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved4() As Integer Implements IVsTextLines.Reserved4, IVsTextBuffer.Reserved4
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved5() As Integer Implements IVsTextLines.Reserved5, IVsTextBuffer.Reserved5
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved6() As Integer Implements IVsTextLines.Reserved6, IVsTextBuffer.Reserved6
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved7() As Integer Implements IVsTextLines.Reserved7, IVsTextBuffer.Reserved7
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved8() As Integer Implements IVsTextLines.Reserved8, IVsTextBuffer.Reserved8
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function Reserved9() As Integer Implements IVsTextLines.Reserved9, IVsTextBuffer.Reserved9
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetLanguageServiceID(ByRef guidLangService As Guid) As Integer Implements IVsTextLines.SetLanguageServiceID, IVsTextBuffer.SetLanguageServiceID
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetStateFlags(ByVal dwReadOnlyFlags As UInteger) As Integer Implements IVsTextLines.SetStateFlags, IVsTextBuffer.SetStateFlags
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnadviseTextLinesEvents(ByVal dwCookie As UInteger) As Integer Implements IVsTextLines.UnadviseTextLinesEvents
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnlockBuffer() As Integer Implements IVsTextLines.UnlockBuffer, IVsTextBuffer.UnlockBuffer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnlockBufferEx(ByVal dwFlags As UInteger) As Integer Implements IVsTextLines.UnlockBufferEx, IVsTextBuffer.UnlockBufferEx
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
