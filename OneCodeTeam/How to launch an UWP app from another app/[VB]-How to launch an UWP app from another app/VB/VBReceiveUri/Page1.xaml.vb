' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.UI.Popups
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class Page1
    Inherits Page

    Public Sub New()
        InitializeComponent()
    End Sub

    'When navigateed to this page from OnActivated of app.cs
    'set the uri info to UI
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim uri As Uri = e.Parameter
        If uri IsNot Nothing Then
            Scheme.Text = uri.Scheme
            Host.Text = uri.Host
            LocalPath.Text = uri.LocalPath
            Query.Text = uri.Query
        End If
    End Sub

End Class
