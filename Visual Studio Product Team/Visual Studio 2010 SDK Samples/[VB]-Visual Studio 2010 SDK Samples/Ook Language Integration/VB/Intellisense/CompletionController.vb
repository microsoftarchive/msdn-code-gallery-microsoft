Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Editor
Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio.Utilities
Imports Microsoft.VisualStudio
Imports System.Runtime.InteropServices

Namespace OokLanguage

    <Export(GetType(IVsTextViewCreationListener)),
    ContentType("ook!"),
    TextViewRole(PredefinedTextViewRoles.Interactive)>
    Friend NotInheritable Class VsTextViewCreationListener
        Implements IVsTextViewCreationListener

        <Import()>
        Private AdaptersFactory As IVsEditorAdaptersFactoryService = Nothing

        <Import()>
        Private CompletionBroker As ICompletionBroker = Nothing

        Public Sub VsTextViewCreated(ByVal textViewAdapter As IVsTextView) Implements IVsTextViewCreationListener.VsTextViewCreated
            Dim view As IWpfTextView = AdaptersFactory.GetWpfTextView(textViewAdapter)
            Debug.Assert(view IsNot Nothing)
            Dim filter As New CommandFilter(view, CompletionBroker)
            Dim [next] As IOleCommandTarget
            textViewAdapter.AddCommandFilter(filter, [next])
            filter.Next = [next]
        End Sub

    End Class

    Friend NotInheritable Class CommandFilter
        Implements IOleCommandTarget

        Private _currentSession As ICompletionSession

        Public Sub New(ByVal textView As IWpfTextView, ByVal broker As ICompletionBroker)
            _currentSession = Nothing
            Me.TextView = textView
            Me.Broker = broker
        End Sub

        Private privateTextView As IWpfTextView
        Public Property TextView As IWpfTextView
            Get
                Return privateTextView
            End Get
            Private Set(ByVal value As IWpfTextView)
                privateTextView = value
            End Set
        End Property

        Private privateBroker As ICompletionBroker
        Public Property Broker As ICompletionBroker
            Get
                Return privateBroker
            End Get
            Private Set(ByVal value As ICompletionBroker)
                privateBroker = value
            End Set
        End Property
        Public Property [Next] As IOleCommandTarget

        Private Function GetTypeChar(ByVal pvaIn As IntPtr) As Char
            Return ChrW(CUShort(Marshal.GetObjectForNativeVariant(pvaIn)))
        End Function

        Public Function Exec(ByRef pguidCmdGroup As Guid, ByVal nCmdID As UInteger, ByVal nCmdexecopt As UInteger, ByVal pvaIn As IntPtr, ByVal pvaOut As IntPtr) As Integer Implements IOleCommandTarget.Exec
            Dim handled As Boolean = False
            Dim hresult As Integer = VSConstants.S_OK

            ' 1. Pre-process
            If pguidCmdGroup = VSConstants.VSStd2K Then
                Select Case CType(nCmdID, VSConstants.VSStd2KCmdID)
                    Case VSConstants.VSStd2KCmdID.AUTOCOMPLETE, VSConstants.VSStd2KCmdID.COMPLETEWORD
                        handled = StartSession()
                    Case VSConstants.VSStd2KCmdID.RETURN
                        handled = Complete(False)
                    Case VSConstants.VSStd2KCmdID.TAB
                        handled = Complete(True)
                    Case VSConstants.VSStd2KCmdID.CANCEL
                        handled = Cancel()
                End Select
            End If

            If Not handled Then
                hresult = [Next].Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            End If

            If ErrorHandler.Succeeded(hresult) Then
                If pguidCmdGroup = VSConstants.VSStd2K Then
                    Select Case CType(nCmdID, VSConstants.VSStd2KCmdID)
                        Case VSConstants.VSStd2KCmdID.TYPECHAR
                            Dim ch As Char = GetTypeChar(pvaIn)
                            If ch = " "c Then
                                StartSession()
                            ElseIf _currentSession IsNot Nothing Then
                                Filter()
                            End If
                        Case VSConstants.VSStd2KCmdID.BACKSPACE
                            Filter()
                    End Select
                End If
            End If

            Return hresult
        End Function

        Private Sub Filter()
            If _currentSession Is Nothing Then
                Return
            End If

            _currentSession.SelectedCompletionSet.SelectBestMatch()
            _currentSession.SelectedCompletionSet.Recalculate()
        End Sub

        Private Function Cancel() As Boolean
            If _currentSession Is Nothing Then
                Return False
            End If

            _currentSession.Dismiss()

            Return True
        End Function

        Private Function Complete(ByVal force As Boolean) As Boolean
            If _currentSession Is Nothing Then
                Return False
            End If

            If (Not _currentSession.SelectedCompletionSet.SelectionStatus.IsSelected) AndAlso (Not force) Then
                _currentSession.Dismiss()
                Return False
            Else
                _currentSession.Commit()
                Return True
            End If
        End Function

        Private Function StartSession() As Boolean
            If _currentSession IsNot Nothing Then
                Return False
            End If

            Dim caret As SnapshotPoint = TextView.Caret.Position.BufferPosition
            Dim snapshot As ITextSnapshot = caret.Snapshot

            If Not Broker.IsCompletionActive(TextView) Then
                _currentSession = Broker.CreateCompletionSession(TextView, snapshot.CreateTrackingPoint(caret, PointTrackingMode.Positive), True)
            Else
                _currentSession = Broker.GetSessions(TextView)(0)
            End If

            AddHandler _currentSession.Dismissed, Sub(sender, args) _currentSession = Nothing
            _currentSession.Start()

            Return True
        End Function

        Public Function QueryStatus(ByRef pguidCmdGroup As Guid, ByVal cCmds As UInteger, ByVal prgCmds() As OLECMD, ByVal pCmdText As IntPtr) As Integer Implements IOleCommandTarget.QueryStatus
            If pguidCmdGroup = VSConstants.VSStd2K Then
                Select Case CType(prgCmds(0).cmdID, VSConstants.VSStd2KCmdID)
                    Case VSConstants.VSStd2KCmdID.AUTOCOMPLETE, VSConstants.VSStd2KCmdID.COMPLETEWORD
                        prgCmds(0).cmdf = CUInt(OLECMDF.OLECMDF_ENABLED) Or CUInt(OLECMDF.OLECMDF_SUPPORTED)
                        Return VSConstants.S_OK
                End Select
            End If

            Return [Next].QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText)
        End Function

    End Class
End Namespace