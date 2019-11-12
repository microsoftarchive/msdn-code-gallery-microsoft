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
Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.Utilities
Imports System.Globalization

Namespace CompletionTooltipCustomization

    <Export(GetType(IUIElementProvider(Of Completion, ICompletionSession))),
    Name("SampleCompletionTooltipCustomization"),
    Order(),
    ContentType("text")>
    Friend Class CompletionTooltipCustomizationProvider
        Implements IUIElementProvider(Of Completion, ICompletionSession)

        Public Function GetUIElement(ByVal itemToRender As Completion, ByVal context As ICompletionSession, ByVal elementType As UIElementType) As UIElement Implements IUIElementProvider(Of Completion, ICompletionSession).GetUIElement
            If elementType = UIElementType.Tooltip Then
                Return New CompletionTooltipCustomization(itemToRender)
            End If

            Return Nothing
        End Function

    End Class

    'Class that contains the custom formats for the tooltip
    Friend Class CompletionTooltipCustomization
        Inherits TextBlock

        Friend Sub New(ByVal completion As Completion)
            Me.Text = String.Format(CultureInfo.CurrentCulture, "{0}: {1}", completion.DisplayText, completion.Description)
            Me.FontSize = 24
            Me.FontStyle = FontStyles.Italic
        End Sub

    End Class

End Namespace
