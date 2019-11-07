'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private _employee As Employee

    Public Sub New()
        Me.InitializeComponent()
        Scenario3Reset(Nothing, Nothing)
    End Sub

    Private Sub Scenario3Reset(sender As Object, e As RoutedEventArgs)
        _employee = New Employee()
        _employee.Name = "Jane Doe"
        _employee.Organization = "Contoso"
        AddHandler _employee.PropertyChanged, AddressOf employeeChanged

        rootPage.DataContext = _employee
        tbBoundDataModelStatus.Text = ""
    End Sub

    Private Sub employeeChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        tbBoundDataModelStatus.Text = "The property:'" & e.PropertyName & "' was changed"
    End Sub

End Class
