' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    'When navigateed to this page from OnActivated of app.cs
    'set the uri info to UI
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim uri As Uri = e.Parameter

        Me.Query.Text = "this is a test"

        If uri IsNot Nothing Then
            Me.Scheme.Text = uri.Scheme
            Me.Host.Text = uri.Host
            Me.LocalPath.Text = uri.LocalPath
            Me.Query.Text = uri.Query
        End If
    End Sub

End Class
