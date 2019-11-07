'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
' Copyright (c) Microsoft Corporation.  All rights reserved.
'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Utilities

Namespace VSLTK.Intellisense

    <Export(GetType(IIntellisenseControllerProvider)), Name("Template QuickInfo Controller"), ContentType("text")>
    Friend Class TemplateQuickInfoControllerProvider
        Implements IIntellisenseControllerProvider

        <Import()>
        Friend Property QuickInfoBroker As IQuickInfoBroker

        Public Function TryCreateIntellisenseController(ByVal textView As ITextView, ByVal subjectBuffers As IList(Of ITextBuffer)) As IIntellisenseController Implements IIntellisenseControllerProvider.TryCreateIntellisenseController
            Return New TemplateQuickInfoController(textView, subjectBuffers, Me)
        End Function

    End Class
End Namespace