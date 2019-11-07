Imports System.Text
Imports Microsoft.VisualStudio.Text.Tagging

Namespace Microsoft.VisualStudio.Language.Spellchecker
	Friend Interface IMisspellingTag
		Inherits ITag
		ReadOnly Property Suggestions() As IEnumerable(Of String)
	End Interface
End Namespace
