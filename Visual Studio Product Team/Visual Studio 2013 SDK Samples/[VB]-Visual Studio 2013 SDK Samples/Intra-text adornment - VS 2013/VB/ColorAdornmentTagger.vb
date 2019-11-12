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

Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Tagging

Namespace IntraTextAdornmentSample

	''' <summary>
	''' Provides color swatch adornments in place of color constants.
	''' </summary>
	Friend NotInheritable Class ColorAdornmentTagger
		Inherits IntraTextAdornmentTagTransformer(Of ColorTag, ColorAdornment)

		Friend Shared Function GetTagger(ByVal view As IWpfTextView, ByVal colorTagger As Lazy(Of ITagAggregator(Of ColorTag))) As ITagger(Of IntraTextAdornmentTag)

			Return view.Properties.GetOrCreateSingletonProperty(Of ColorAdornmentTagger)(Function() New ColorAdornmentTagger(view, colorTagger.Value))

		End Function

		Private Sub New(ByVal view As IWpfTextView, ByVal colorTagger As ITagAggregator(Of ColorTag))

			MyBase.New(view, colorTagger)

		End Sub

		Protected Overrides Function CreateAdornment(ByVal dataTag As ColorTag) As ColorAdornment

			Return New ColorAdornment(dataTag)

		End Function

		Protected Overrides Sub UpdateAdornment(ByVal adornment As ColorAdornment, ByVal dataTag As ColorTag)

			adornment.Update(dataTag)

		End Sub

		Public Overrides Sub Dispose()

			MyBase.Dispose()

			_view.Properties.RemoveProperty(GetType(ColorAdornmentTagger))

		End Sub

	End Class

End Namespace
