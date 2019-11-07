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


Namespace IntraTextAdornmentSample

	Friend NotInheritable Class ColorAdornment
		Inherits Button

		Private rect As Rectangle

		Friend Sub New(ByVal colorTag As ColorTag)

			rect = New Rectangle With {.Stroke = Brushes.Black, .StrokeThickness = 1, .Width = 20, .Height = 10}

			Update(colorTag)

			Me.Content = rect

		End Sub

		Private Function MakeBrush(ByVal color As Color) As Brush

			Dim brush = New SolidColorBrush(color)
			brush.Freeze()
			Return brush

		End Function

		Friend Sub Update(ByVal colorTag As ColorTag)

			rect.Fill = MakeBrush(colorTag.Color)

		End Sub

	End Class

End Namespace
