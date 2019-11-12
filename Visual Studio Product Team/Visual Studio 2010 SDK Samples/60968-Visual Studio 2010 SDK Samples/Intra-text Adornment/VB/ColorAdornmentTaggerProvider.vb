'***************************************************************************
'
'    Copyright (c) Microsoft Corporation. All rights reserved.
'    This code is licensed under the Visual Studio SDK license terms.
'    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'***************************************************************************

Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace IntraTextAdornmentSample

	<Export(GetType(IViewTaggerProvider)), ContentType("text"), ContentType("projection"), TagType(GetType(IntraTextAdornmentTag))>
	Friend NotInheritable Class ColorAdornmentTaggerProvider
		Implements IViewTaggerProvider

        <Import()>
        Friend BufferTagAggregatorFactoryService As IBufferTagAggregatorFactoryService

		Public Function CreateTagger(Of T As ITag)(ByVal textView As ITextView, ByVal buffer As ITextBuffer) As ITagger(Of T) Implements IViewTaggerProvider.CreateTagger

			If textView Is Nothing Then
				Throw New ArgumentNullException("textView")
			End If

			If buffer Is Nothing Then
				Throw New ArgumentNullException("buffer")
			End If

			If buffer IsNot textView.TextBuffer Then
				Return Nothing
			End If

			Return TryCast(ColorAdornmentTagger.GetTagger(CType(textView, IWpfTextView), New Lazy(Of ITagAggregator(Of ColorTag))(Function() BufferTagAggregatorFactoryService.CreateTagAggregator(Of ColorTag)(textView.TextBuffer))), ITagger(Of T))

		End Function

	End Class

End Namespace
