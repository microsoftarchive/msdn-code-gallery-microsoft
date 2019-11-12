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

Imports System.Globalization
Imports System.Text.RegularExpressions
Imports Microsoft.VisualStudio.Text

Namespace IntraTextAdornmentSample

	''' <summary>
	''' Determines which spans of text likely refer to color values.
	''' </summary>
	''' <remarks>
	''' This is a data-only component. The ITagger interface is a good fit for presenting data-about-text.
	''' </remarks>
	Friend NotInheritable Class ColorTagger
		Inherits RegexTagger(Of ColorTag)

		Friend Sub New(ByVal buffer As ITextBuffer)

			MyBase.New(buffer, { New Regex("\b[\dA-F]{6}\b", RegexOptions.Compiled Or RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase) })

		End Sub

		Protected Overrides Function TryCreateTagForMatch(ByVal match As Match) As ColorTag

			Dim color As Color = ParseColor(match.ToString())

			Debug.Assert(match.Length = 6)

			Return New ColorTag(color)

		End Function

		Private Shared Function ParseColor(ByVal hexColor As String) As Color

			Dim number As Integer

			If Not Int32.TryParse(hexColor, NumberStyles.HexNumber, CultureInfo.InvariantCulture, number) Then

				Debug.Fail("unable to parse " & hexColor)
				Return Colors.Transparent

			End If

            Dim r As Byte = CByte(number >> 16 And 255)
            Dim g As Byte = CByte(number >> 8 And 255)
            Dim b As Byte = CByte(number >> 0 And 255)

			Return Color.FromRgb(r, g, b)

		End Function

	End Class

End Namespace
