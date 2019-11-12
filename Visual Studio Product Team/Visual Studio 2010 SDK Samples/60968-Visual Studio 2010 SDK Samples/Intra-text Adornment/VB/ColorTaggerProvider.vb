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
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace IntraTextAdornmentSample

	<Export(GetType(ITaggerProvider)), ContentType("text"), TagType(GetType(ColorTag))>
	Friend NotInheritable Class ColorTaggerProvider
		Implements ITaggerProvider

		Public Function CreateTagger(Of T As ITag)(ByVal buffer As ITextBuffer) As ITagger(Of T) Implements ITaggerProvider.CreateTagger

			If buffer Is Nothing Then
				Throw New ArgumentNullException("buffer")
			End If

			Return TryCast(buffer.Properties.GetOrCreateSingletonProperty(Of ColorTagger)(Function() New ColorTagger(buffer)), ITagger(Of T))

		End Function

	End Class

End Namespace
