Imports System.ComponentModel.Composition
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Editor
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Operations
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio.Utilities

Namespace TypingSpeed

    <Export(GetType(IVsTextViewCreationListener)),
    TextViewRole(PredefinedTextViewRoles.Editable),
    ContentType("text")>
    Friend NotInheritable Class VsTextViewListener
        Implements IVsTextViewCreationListener

        <Import()>
        Friend AdapterService As IVsEditorAdaptersFactoryService = Nothing

        Public Sub VsTextViewCreated(ByVal textViewAdapter As IVsTextView) Implements IVsTextViewCreationListener.VsTextViewCreated
            Dim textView As ITextView = AdapterService.GetWpfTextView(textViewAdapter)
            If textView Is Nothing Then
                Return
            End If

            Dim adornment = textView.Properties.GetProperty(Of TypingSpeedMeter)(GetType(TypingSpeedMeter))
            textView.Properties.GetOrCreateSingletonProperty(Function() New TypeCharFilter(textViewAdapter, textView, adornment))
        End Sub

    End Class

    Friend NotInheritable Class TypeCharFilter
        Implements IOleCommandTarget

        Private nextCommandHandler As IOleCommandTarget
        Private adornment As TypingSpeedMeter
        Private textView As ITextView
        Friend Property typedChars As Integer

        Friend Sub New(ByVal adapter As IVsTextView, ByVal textView As ITextView, ByVal adornment As TypingSpeedMeter)
            Me.textView = textView
            Me.adornment = adornment
            adapter.AddCommandFilter(Me, nextCommandHandler)
        End Sub

        Public Function Exec(ByRef pguidCmdGroup As Guid, ByVal nCmdID As UInteger, ByVal nCmdexecopt As UInteger, ByVal pvaIn As IntPtr, ByVal pvaOut As IntPtr) As Integer Implements IOleCommandTarget.Exec
            Dim hr As Integer = VSConstants.S_OK
            Dim typedChar As Char

            If TryGetTypedChar(pguidCmdGroup, nCmdID, pvaIn, typedChar) Then
                adornment.updateBar(typedChars)
                typedChars += 1
            End If

            hr = nextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            Return hr
        End Function

        Public Function QueryStatus(ByRef pguidCmdGroup As Guid, ByVal cCmds As UInteger, ByVal prgCmds() As OLECMD, ByVal pCmdText As IntPtr) As Integer Implements IOleCommandTarget.QueryStatus
            Return nextCommandHandler.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText)
        End Function

        Private Function TryGetTypedChar(ByVal cmdGroup As Guid, ByVal nCmdID As UInteger, ByVal pvaIn As IntPtr, <System.Runtime.InteropServices.Out()> ByRef typedChar As Char) As Boolean
            typedChar = Char.MinValue

            If cmdGroup <> VSConstants.VSStd2K OrElse nCmdID <> CUInt(VSConstants.VSStd2KCmdID.TYPECHAR) Then
                Return False
            End If

            typedChar = CChar(ChrW(CUShort(Marshal.GetObjectForNativeVariant(pvaIn))))
            Return True
        End Function

    End Class
End Namespace
