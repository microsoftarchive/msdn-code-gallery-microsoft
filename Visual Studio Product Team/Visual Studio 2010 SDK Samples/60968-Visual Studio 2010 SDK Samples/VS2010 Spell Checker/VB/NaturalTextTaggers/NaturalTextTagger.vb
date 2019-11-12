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

' Copyright (c) Microsoft Corporation.  All rights reserved.
'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace Microsoft.VisualStudio.Language.Spellchecker
	''' <summary>
	''' Provides tags for text files.
	''' </summary>
	Friend Class NaturalTextTagger
		Implements ITagger(Of NaturalTextTag)
		#Region "Private Fields"
		Private _buffer As ITextBuffer
		#End Region

		#Region "MEF Imports / Exports"
		''' <summary>
		''' MEF connector for the Natural Text Tagger.
		''' </summary>
		<Export(GetType(ITaggerProvider)), ContentType("plaintext"), TagType(GetType(NaturalTextTag))> _
		Friend Class NaturalTextTaggerProvider
			Implements ITaggerProvider
			Public Function CreateTagger(Of T As ITag)(ByVal buffer As ITextBuffer) As ITagger(Of T) Implements ITaggerProvider.CreateTagger
				If buffer Is Nothing Then
					Throw New ArgumentNullException("buffer")
				End If

                Return TryCast(New NaturalTextTagger(buffer), ITagger(Of T))
			End Function
		End Class
		#End Region

		#Region "Constructor"
		''' <summary>
		''' Constructor for Natural Text Tagger.
		''' </summary>
		''' <param name="buffer">Relevant buffer.</param>
		Public Sub New(ByVal buffer As ITextBuffer)
			_buffer = buffer
		End Sub
		#End Region

		#Region "ITagger<NaturalTextTag> Members"
		''' <summary>
		''' Returns tags on demand.
		''' </summary>
		''' <param name="spans">Spans collection to get tags for.</param>
		''' <returns>Tags in provided spans.</returns>
		Public Function GetTags(ByVal spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of NaturalTextTag)) Implements ITagger(Of NaturalTextTag).GetTags
            Dim list As New List(Of ITagSpan(Of NaturalTextTag))
            For Each snapshotSpan In spans
                Debug.Assert(snapshotSpan.Snapshot.TextBuffer Is _buffer)
                list.Add(New TagSpan(Of NaturalTextTag)(snapshotSpan, New NaturalTextTag()))
            Next snapshotSpan
            Return list
		End Function


		Public Event TagsChanged As EventHandler(Of SnapshotSpanEventArgs) Implements ITagger(Of NaturalTextTag).TagsChanged

		#End Region

	End Class
End Namespace
