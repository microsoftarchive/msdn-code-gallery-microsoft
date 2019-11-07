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
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Utilities
Imports Microsoft.VisualStudio.Text.Tagging

Namespace ToDoGlyphFactory

    <Export(GetType(IGlyphFactoryProvider)),
    Name("ToDoGlyph"),
    Order(Before:="VsTextMarker"),
    ContentType("code"),
    TagType(GetType(ToDoTag))>
    Friend NotInheritable Class ToDoGlyphFactoryProvider
        Implements IGlyphFactoryProvider

        Public Function GetGlyphFactory(ByVal view As IWpfTextView, ByVal margin As IWpfTextViewMargin) As IGlyphFactory Implements IGlyphFactoryProvider.GetGlyphFactory
            Return New ToDoGlyphFactory
        End Function

    End Class
End Namespace
