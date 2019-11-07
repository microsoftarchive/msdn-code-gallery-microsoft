' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

Imports Windows.UI.Popups
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    Private Async Sub RunPage1_Click(sender As Object, e As RoutedEventArgs)
        Await LaunchAppAsync("test-launchpage1://Page1/Path1?param1=This is param1&param1=This is param2")
    End Sub

    Private Async Sub RunMainPage_Click(sender As Object, e As RoutedEventArgs)
        Await LaunchAppAsync("test-launchmainpage://HostMainpage/Path1?param=This is param")
    End Sub

    Private Async Function LaunchAppAsync(uriStr As String) As Task
        Dim uri = New Uri(uriStr)
        Dim promptOptions = New Windows.System.LauncherOptions()
        promptOptions.TreatAsUntrusted = False

        Dim isSuccess As Boolean = Await Windows.System.Launcher.LaunchUriAsync(uri, promptOptions)

        If isSuccess = False Then
            Dim msg As String = "Launch failed"
            Await New MessageDialog(msg).ShowAsync()
        End If
    End Function
End Class
