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


Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Formatting

Namespace ToDoGlyphFactory

    Friend Class ToDoGlyphFactory
        Implements IGlyphFactory

        Public Function GenerateGlyph(ByVal line As IWpfTextViewLine, ByVal tag As IGlyphTag) As UIElement Implements IGlyphFactory.GenerateGlyph
            Return New TodoGlyph
        End Function

    End Class
End Namespace
