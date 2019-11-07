' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System

Namespace Global.PrintSample
    Partial Public NotInheritable Class ScenarioOutput1
        Inherits Page

        ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
        Private rootPage As MainPage = Nothing

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
            ' Get a pointer to our main page.
            rootPage = TryCast(e.Parameter, MainPage)
        End Sub
    End Class
End Namespace
