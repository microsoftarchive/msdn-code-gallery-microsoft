Imports System

Public Class Form1
    Private client As New ServiceReference.Service1Client()

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim localities As String() = {"Los Angeles", "Rio de Janeiro", "New York", "London", "Paris", "Rome", _
                        "Cairo", "Beijing"}
        Dim data As ServiceReference.WeatherData() = client.GetWeatherData(localities)
        Dim source As New BindingSource()
        source.DataSource = data
        DataGridView1.DataSource = source
    End Sub
End Class

