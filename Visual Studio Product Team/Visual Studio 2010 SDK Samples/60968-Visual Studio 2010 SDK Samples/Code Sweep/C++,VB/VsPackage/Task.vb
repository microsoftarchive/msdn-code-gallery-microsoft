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
Imports System.Diagnostics
Imports Microsoft.VisualStudio.TextManager.Interop
Imports System.Runtime.InteropServices
Imports Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class Task
        Implements IVsTaskItem, IVsTaskItem3
        Public Enum TaskFields
            Priority
            PriorityNumber
            Term
            [Class]
            Replacement
            Comment
            File
            Line
            Project
        End Enum

        Public Sub New(ByVal term As String, ByVal severity As Integer, ByVal termClass As String, ByVal comment As String, ByVal replacement As String, ByVal filePath As String, ByVal line As Integer, ByVal column As Integer, ByVal projectFile As String, ByVal lineText As String, ByVal provider As TaskProvider, ByVal serviceProvider As IServiceProvider)
            _term = term
            _severity = severity
            _class = termClass
            _comment = ParseLinks(comment)
            _file = filePath
            _line = line
            _column = column
            _provider = provider
            _replacement = replacement
            _serviceProvider = serviceProvider
            _projectFile = projectFile
            _lineText = lineText
        End Sub

        Public ReadOnly Property ProjectFile() As String
            Get
                Return _projectFile
            End Get
        End Property

        Public Property Ignored() As Boolean
            Get
                If _projectFile IsNot Nothing AndAlso _projectFile.Length > 0 Then
                    Dim project As IVsProject = ProjectUtilities.GetProjectByFileName(_projectFile)
                    If project IsNot Nothing Then
                        Dim ignoreMe As BuildTask.IIgnoreInstance = BuildTask.Factory.GetIgnoreInstance(_file, _lineText, _term, _column)
                        For Each instance As BuildTask.IIgnoreInstance In Factory.GetProjectConfigurationStore(project).IgnoreInstances
                            If instance.CompareTo(ignoreMe) = 0 Then
                                Return True
                            End If
                        Next instance
                    End If
                End If

                Return False
            End Get

            Set(ByVal value As Boolean)
                If value <> Ignored Then
                    If _projectFile IsNot Nothing AndAlso _projectFile.Length > 0 Then
                        Dim project As IVsProject = ProjectUtilities.GetProjectByFileName(_projectFile)
                        If project IsNot Nothing Then
                            Dim store As IProjectConfigurationStore = Factory.GetProjectConfigurationStore(project)
                            Dim ignoreMe As BuildTask.IIgnoreInstance = BuildTask.Factory.GetIgnoreInstance(_file, _lineText, _term, _column)

                            If value Then
                                store.IgnoreInstances.Add(ignoreMe)
                            Else
                                For Each instance As BuildTask.IIgnoreInstance In store.IgnoreInstances
                                    If instance.CompareTo(ignoreMe) = 0 Then
                                        store.IgnoreInstances.Remove(instance)
                                        Return
                                    End If
                                Next instance
                            End If
                        End If
                    End If
                End If
            End Set
        End Property

#Region "IVsTaskItem Members"

        Public Function CanDelete(<System.Runtime.InteropServices.Out()> ByRef pfCanDelete As Integer) As Integer Implements IVsTaskItem.CanDelete
            pfCanDelete = 0
            Return VSConstants.S_OK
        End Function

        Public Function Category(ByVal pCat As VSTASKCATEGORY()) As Integer Implements IVsTaskItem.Category
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function Column(<System.Runtime.InteropServices.Out()> ByRef piCol As Integer) As Integer Implements IVsTaskItem.Column
            piCol = 0
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function Document(<System.Runtime.InteropServices.Out()> ByRef pbstrMkDocument As String) As Integer Implements IVsTaskItem.Document
            pbstrMkDocument = ""
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function HasHelp(<System.Runtime.InteropServices.Out()> ByRef pfHasHelp As Integer) As Integer Implements IVsTaskItem.HasHelp
            pfHasHelp = 0
            Return VSConstants.S_OK
        End Function

        Public Function ImageListIndex(<System.Runtime.InteropServices.Out()> ByRef pIndex As Integer) As Integer Implements IVsTaskItem.ImageListIndex
            pIndex = 0
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function IsReadOnly(ByVal field As VSTASKFIELD, <System.Runtime.InteropServices.Out()> ByRef pfReadOnly As Integer) As Integer Implements IVsTaskItem.IsReadOnly
            pfReadOnly = 1
            Return VSConstants.S_OK
        End Function

        Public Function Line(<System.Runtime.InteropServices.Out()> ByRef piLine As Integer) As Integer Implements IVsTaskItem.Line
            piLine = 0
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function NavigateTo() As Integer Implements IVsTaskItem.NavigateTo
            Dim hr As Integer = VSConstants.S_OK
            Dim openDoc As IVsUIShellOpenDocument = TryCast(_serviceProvider.GetService(GetType(SVsUIShellOpenDocument)), IVsUIShellOpenDocument)

            Dim sp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider = Nothing
            Dim hierarchy As IVsUIHierarchy = Nothing
            Dim itemID As UInteger = 0
            Dim frame As IVsWindowFrame = Nothing
            Dim viewGuid As Guid = VSConstants.LOGVIEWID_TextView

            hr = openDoc.OpenDocumentViaProject(_file, viewGuid, sp, hierarchy, itemID, frame)
            Debug.Assert(hr = VSConstants.S_OK, "OpenDocumentViaProject did not return S_OK.")

            hr = frame.Show()
            Debug.Assert(hr = VSConstants.S_OK, "Show did not return S_OK.")

            Dim viewPtr As IntPtr = IntPtr.Zero
            Dim textLinesGuid As Guid = GetType(IVsTextLines).GUID
            hr = frame.QueryViewInterface(textLinesGuid, viewPtr)
            Debug.Assert(hr = VSConstants.S_OK, "QueryViewInterface did not return S_OK.")

            Dim textLines As IVsTextLines = TryCast(Marshal.GetUniqueObjectForIUnknown(viewPtr), IVsTextLines)

            Dim textMgr As IVsTextManager = TryCast(_serviceProvider.GetService(GetType(SVsTextManager)), IVsTextManager)
            Dim textView As IVsTextView = Nothing
            hr = textMgr.GetActiveView(0, textLines, textView)
            Debug.Assert(hr = VSConstants.S_OK, "QueryViewInterface did not return S_OK.")

            If textView IsNot Nothing Then
                If _line >= 0 Then
                    textView.SetCaretPos(_line, Math.Max(_column, 0))
                End If
            End If

            Return VSConstants.S_OK
        End Function

        Public Function NavigateToHelp() As Integer Implements IVsTaskItem.NavigateToHelp
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function OnDeleteTask() As Integer Implements IVsTaskItem.OnDeleteTask
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function OnFilterTask(ByVal fVisible As Integer) As Integer Implements IVsTaskItem.OnFilterTask
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function SubcategoryIndex(<System.Runtime.InteropServices.Out()> ByRef pIndex As Integer) As Integer Implements IVsTaskItem.SubcategoryIndex
            pIndex = 0
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function get_Checked(<System.Runtime.InteropServices.Out()> ByRef pfChecked As Integer) As Integer Implements IVsTaskItem.get_Checked
            pfChecked = 0
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function get_Priority(ByVal ptpPriority As VSTASKPRIORITY()) As Integer Implements IVsTaskItem.get_Priority
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function get_Text(<System.Runtime.InteropServices.Out()> ByRef pbstrName As String) As Integer Implements IVsTaskItem.get_Text
            pbstrName = ""
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function put_Checked(ByVal fChecked As Integer) As Integer Implements IVsTaskItem.put_Checked
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function put_Priority(ByVal tpPriority As VSTASKPRIORITY) As Integer Implements IVsTaskItem.put_Priority
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function put_Text(ByVal bstrName As String) As Integer Implements IVsTaskItem.put_Text
            Return VSConstants.E_NOTIMPL
        End Function

#End Region

#Region "IVsTaskItem3 Members"

        Public Function GetColumnValue(ByVal iField As Integer, <System.Runtime.InteropServices.Out()> ByRef ptvtType As UInteger, <System.Runtime.InteropServices.Out()> ByRef ptvfFlags As UInteger, <System.Runtime.InteropServices.Out()> ByRef pvarValue As Object, <System.Runtime.InteropServices.Out()> ByRef pbstrAccessibilityName As String) As Integer Implements IVsTaskItem3.GetColumnValue
            ptvfFlags = 0
            pbstrAccessibilityName = ""

            Select Case CType(iField, TaskFields)
                Case TaskFields.Class
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_TEXT)
                    pvarValue = _class
                Case TaskFields.Comment
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_LINKTEXT)
                    pvarValue = _comment
                Case TaskFields.File
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_TEXT)
                    ptvfFlags = CUInt(__VSTASKVALUEFLAGS.TVF_FILENAME)
                    pvarValue = _file
                Case TaskFields.Line
                    If _line = -1 Then
                        ptvtType = CUInt(__VSTASKVALUETYPE.TVT_NULL)
                        pvarValue = Nothing
                    Else
                        ptvtType = CUInt(__VSTASKVALUETYPE.TVT_BASE10)
                        pvarValue = _line + 1 ' Display as one-based coordinate.
                    End If
                Case TaskFields.Priority
                    ptvfFlags = CUInt(__VSTASKVALUEFLAGS.TVF_HORZ_CENTER)
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_IMAGE)
                    pvarValue = TaskProvider.GetImageIndexForSeverity(_severity)
                    If _severity <= 1 Then
                        pbstrAccessibilityName = Resources.HighPriority
                    ElseIf _severity = 2 Then
                        pbstrAccessibilityName = Resources.MediumPriority
                    Else
                        pbstrAccessibilityName = Resources.LowPriority
                    End If
                Case TaskFields.PriorityNumber
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_BASE10)
                    pvarValue = _severity
                Case TaskFields.Project
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_TEXT)
                    If _projectFile IsNot Nothing AndAlso _projectFile.Length > 0 Then
                        pvarValue = ProjectUtilities.GetUniqueProjectNameFromFile(_projectFile)
                    Else
                        pvarValue = ""
                    End If
                Case TaskFields.Replacement
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_TEXT)
                    pvarValue = _replacement
                Case TaskFields.Term
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_TEXT)
                    pvarValue = _term
                Case Else
                    ptvtType = CUInt(__VSTASKVALUETYPE.TVT_NULL)
                    pvarValue = Nothing
                    Return VSConstants.E_INVALIDARG
            End Select

            If Ignored Then
                ptvfFlags = ptvfFlags Or CUInt(__VSTASKVALUEFLAGS.TVF_STRIKETHROUGH)
            End If

            Return VSConstants.S_OK
        End Function

        Public Function GetDefaultEditField(<System.Runtime.InteropServices.Out()> ByRef piField As Integer) As Integer Implements IVsTaskItem3.GetDefaultEditField
            piField = -1
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function GetEnumCount(ByVal iField As Integer, <System.Runtime.InteropServices.Out()> ByRef pnValues As Integer) As Integer Implements IVsTaskItem3.GetEnumCount
            pnValues = 0
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function GetEnumValue(ByVal iField As Integer, ByVal iValue As Integer, <System.Runtime.InteropServices.Out()> ByRef pvarValue As Object, <System.Runtime.InteropServices.Out()> ByRef pbstrAccessibilityName As String) As Integer Implements IVsTaskItem3.GetEnumValue
            pvarValue = Nothing
            pbstrAccessibilityName = ""
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function GetNavigationStatusText(<System.Runtime.InteropServices.Out()> ByRef pbstrText As String) As Integer Implements IVsTaskItem3.GetNavigationStatusText
            pbstrText = _comment
            Return VSConstants.S_OK
        End Function

        Public Function GetSurrogateProviderGuid(<System.Runtime.InteropServices.Out()> ByRef pguidProvider As Guid) As Integer Implements IVsTaskItem3.GetSurrogateProviderGuid
            pguidProvider = Guid.Empty
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function GetTaskName(<System.Runtime.InteropServices.Out()> ByRef pbstrName As String) As Integer Implements IVsTaskItem3.GetTaskName
            pbstrName = _term
            Return VSConstants.S_OK
        End Function

        Public Function GetTaskProvider(<System.Runtime.InteropServices.Out()> ByRef ppProvider As IVsTaskProvider3) As Integer Implements IVsTaskItem3.GetTaskProvider
            ppProvider = _provider
            Return VSConstants.S_OK
        End Function

        Public Function GetTipText(ByVal iField As Integer, <System.Runtime.InteropServices.Out()> ByRef pbstrTipText As String) As Integer Implements IVsTaskItem3.GetTipText
            pbstrTipText = ""
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function IsDirty(<System.Runtime.InteropServices.Out()> ByRef pfDirty As Integer) As Integer Implements IVsTaskItem3.IsDirty
            pfDirty = 0
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function OnLinkClicked(ByVal iField As Integer, ByVal iLinkIndex As Integer) As Integer Implements IVsTaskItem3.OnLinkClicked
            Dim startFinder As TextFinder = AddressOf AnonymousMethod1

            Dim endFinder As TextFinder = AddressOf AnonymousMethod2

            Dim span As Span = FindNthSpan(_comment, iLinkIndex, startFinder, endFinder)

            If span IsNot Nothing Then
                Dim browser As IVsWebBrowsingService = TryCast(_serviceProvider.GetService(GetType(SVsWebBrowsingService)), IVsWebBrowsingService)
                Dim frame As IVsWindowFrame = Nothing

                Dim hr As Integer = browser.Navigate(_comment.Substring(span.Start + 1, span.Length - 2), 0, frame)
                Debug.Assert(hr = VSConstants.S_OK, "Navigate did not return S_OK.")
                Return VSConstants.S_OK
            Else
                Debug.Assert(False, "Invalid link index sent to OnLinkClicked.")
                Return VSConstants.E_INVALIDARG
            End If
        End Function
        Private Function AnonymousMethod1(ByVal text As String, ByVal startIndex As Integer) As Integer
            Return text.IndexOf("@", startIndex)
        End Function
        Private Function AnonymousMethod2(ByVal text As String, ByVal startIndex As Integer) As Integer
            Return text.IndexOf("@", startIndex + 1)
        End Function

        Public Function SetColumnValue(ByVal iField As Integer, ByRef pvarValue As Object) As Integer Implements IVsTaskItem3.SetColumnValue
            Return VSConstants.E_NOTIMPL
        End Function

#End Region

#Region "Private Members"

        Private ReadOnly _term As String
        Private ReadOnly _severity As Integer
        Private ReadOnly _class As String
        Private ReadOnly _comment As String
        Private ReadOnly _file As String
        Private ReadOnly _line As Integer
        Private ReadOnly _column As Integer
        Private ReadOnly _provider As TaskProvider
        Private ReadOnly _replacement As String
        Private ReadOnly _serviceProvider As IServiceProvider
        Private ReadOnly _projectFile As String
        Private ReadOnly _lineText As String

        Private Class Span
            Public Sub New(ByVal start As Integer, ByVal length As Integer)
                _start = start
                _length = length
            End Sub

            Private ReadOnly _start As Integer

            Public ReadOnly Property Start() As Integer
                Get
                    Return _start
                End Get
            End Property

            Private ReadOnly _length As Integer

            Public ReadOnly Property Length() As Integer
                Get
                    Return _length
                End Get
            End Property
        End Class

        Private Delegate Function TextFinder(ByVal text As String, ByVal startIndex As Integer) As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="startFinder"></param>
        ''' <param name="endFinder"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <c>startFinder(text, startIndex)</c> is used to find the beginning of the next span.
        ''' If it returns a positive index, <c>endFinder(text, startFinder(text, startIndex))</c>
        ''' is used to find the end of the span.  <c>endFinder</c> should return the index of the
        ''' last character in the span (not the first character after the span).
        ''' </remarks>
        Private Shared Function FindSpan(ByVal text As String, ByVal startIndex As Integer, ByVal startFinder As TextFinder, ByVal endFinder As TextFinder) As Span
            Dim start As Integer = startFinder(text, startIndex)

            If start >= 0 Then
                Dim [end] As Integer = endFinder(text, start)
                Return New Span(start, [end] - start + 1)
            Else
                Return New Span(-1, -1)
            End If
        End Function

        Private Shared Function FindSpans(ByVal text As String, ByVal startFinder As TextFinder, ByVal endFinder As TextFinder) As IEnumerable(Of Span)
            Dim index As Integer = 0
            Dim results As New List(Of Span)
            Dim span As Span = FindSpan(text, index, startFinder, endFinder)
            Do While span.Start >= 0
                index = span.Start + span.Length
                results.Add(span)
                span = FindSpan(text, index, startFinder, endFinder)
            Loop
            Return results
        End Function

        Private Shared Function FindNthSpan(ByVal text As String, ByVal spanIndex As Integer, ByVal startFinder As TextFinder, ByVal endFinder As TextFinder) As Span
            For Each span As Span In FindSpans(text, startFinder, endFinder)
                If spanIndex = 0 Then
                    Return span
                Else
                    spanIndex -= 1
                End If
            Next span

            Return Nothing
        End Function

        ''' <summary>
        ''' Finds http links in the given text and surrounds them with '@' so they will be treated
        ''' as links by the task list.
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' In VS 2005, the task list has a bug which prevents links from being displayed properly
        ''' unless they begin somewhere on the first line of a task cell.
        ''' </remarks>
        Private Shared Function ParseLinks(ByVal text As String) As String
            Dim startFinder As TextFinder = AddressOf AnonymousMethod3

            Dim endFinder As TextFinder = AddressOf AnonymousMethod4

            Dim result As New StringBuilder()
            Dim previousSpanEnd As Integer = 0

            For Each linkSpan As Span In FindSpans(text, startFinder, endFinder)
                If linkSpan.Start > previousSpanEnd Then
                    result.Append(text.Substring(previousSpanEnd, linkSpan.Start - (previousSpanEnd)))
                End If
                result.Append("@"c)
                result.Append(text.Substring(linkSpan.Start, linkSpan.Length))
                result.Append("@"c)
                previousSpanEnd = linkSpan.Start + linkSpan.Length
            Next linkSpan

            If previousSpanEnd < text.Length Then
                result.Append(text.Substring(previousSpanEnd))
            End If

            Return result.ToString()
        End Function
        Private Shared Function AnonymousMethod3(ByVal text2 As String, ByVal startIndex As Integer) As Integer
            Return text2.IndexOf("http://", startIndex)
        End Function
        Private Shared Function AnonymousMethod4(ByVal text2 As String, ByVal startIndex As Integer) As Integer
            If startIndex > 0 AndAlso text2.Chars(startIndex - 1) = """"c Then
                Dim endQuote As Integer = text2.IndexOf(""""c, startIndex)
                If endQuote >= 0 Then
                    Return endQuote
                Else
                    Return text2.Length - 1
                End If
            Else
                Dim nextWhitespace As Integer = text2.IndexOfAny(New Char() {" "c, ControlChars.Tab, ControlChars.Lf, ControlChars.Cr}, startIndex)
                If nextWhitespace = -1 Then
                    nextWhitespace = text2.Length
                End If
                If Char.IsPunctuation(text2.Chars(nextWhitespace - 1)) Then
                    nextWhitespace -= 1
                End If
                Return nextWhitespace - 1
            End If
        End Function

#End Region
    End Class
End Namespace
