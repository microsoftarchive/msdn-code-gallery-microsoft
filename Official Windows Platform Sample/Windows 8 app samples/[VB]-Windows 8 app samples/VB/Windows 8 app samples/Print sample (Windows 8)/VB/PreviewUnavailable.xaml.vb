'' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
'' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
'' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
'' PARTICULAR PURPOSE.
''
'' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.Foundation
Imports Windows.UI.Xaml.Controls

Partial Public NotInheritable Class PreviewUnavailable
    Inherits Page

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Preview unavailable page constructor
    ''' </summary>
    ''' <param name="paperSize">The printing page paper size</param>
    ''' <param name="printableSize">The usable/"printable" area on the page</param>
    Public Sub New(paperSize As Size, printableSize As Size)
        InitializeComponent()

        page.Width = paperSize.Width
        page.Height = paperSize.Height

        printablePage.Width = printableSize.Width
        printablePage.Height = printableSize.Height
    End Sub


End Class
