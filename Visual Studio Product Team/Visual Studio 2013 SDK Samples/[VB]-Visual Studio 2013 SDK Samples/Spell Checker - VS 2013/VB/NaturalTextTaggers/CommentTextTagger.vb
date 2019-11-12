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
Imports System.Globalization
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace Microsoft.VisualStudio.Language.Spellchecker
	''' <summary>
	''' Provides tags for Doc Comment regions
	''' </summary>
	Friend Class CommentTextTagger
		Implements ITagger(Of NaturalTextTag)
		#Region "Private Fields"
		Private _buffer As ITextBuffer
		Private _classifier As IClassifier
		#End Region

		#Region "MEF Imports / Exports"
		''' <summary>
		''' MEF connector for the Natural Text Tagger.
		''' </summary>
		<Export(GetType(ITaggerProvider)), ContentType("code"), TagType(GetType(NaturalTextTag))> _
		Friend Class CommentTextTaggerProvider
			Implements ITaggerProvider
			#Region "MEF Imports"
            Private _classifierAggregatorService As IClassifierAggregatorService
            <Import()> _
            Friend Property ClassifierAggregatorService() As IClassifierAggregatorService
                Get
                    Return _classifierAggregatorService
                End Get
                Set(ByVal value As IClassifierAggregatorService)
                    _classifierAggregatorService = value
                End Set
            End Property
			#End Region

			#Region "ITaggerProvider"
			Public Function CreateTagger(Of T As ITag)(ByVal buffer As ITextBuffer) As ITagger(Of T) Implements ITaggerProvider.CreateTagger
				If buffer Is Nothing Then
					Throw New ArgumentNullException("buffer")
				End If
                Return TryCast(New CommentTextTagger(buffer, ClassifierAggregatorService.GetClassifier(buffer)), ITagger(Of T))
			End Function

			#End Region
		End Class
		#End Region

		#Region "Constructor"
		''' <summary>
		''' Constructor for Natural Text Tagger.
		''' </summary>
		''' <param name="buffer">Relevant buffer.</param>
		''' <param name="classifiers">List of all available classifiers.</param>
		Public Sub New(ByVal buffer As ITextBuffer, ByVal classifier As IClassifier)
			_buffer = buffer
			_classifier = classifier
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
            If _classifier IsNot Nothing Then
                For Each snapshotSpan In spans
                    Debug.Assert(snapshotSpan.Snapshot.TextBuffer Is _buffer)
                    For Each classificationSpan As ClassificationSpan In _classifier.GetClassificationSpans(snapshotSpan)
                        If classificationSpan.ClassificationType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("comment") OrElse classificationSpan.ClassificationType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("string") Then
                            list.Add(New TagSpan(Of NaturalTextTag)(classificationSpan.Span, New NaturalTextTag()))
                        End If
                    Next classificationSpan
                Next snapshotSpan
            End If

            Return list
		End Function


		Public Event TagsChanged As EventHandler(Of SnapshotSpanEventArgs) Implements ITagger(Of NaturalTextTag).TagsChanged

		#End Region

	End Class
End Namespace
