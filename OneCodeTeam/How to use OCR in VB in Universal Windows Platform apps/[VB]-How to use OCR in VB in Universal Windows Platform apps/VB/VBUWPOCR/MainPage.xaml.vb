' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Public Shared Current As MainPage

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Current = Me
        SampleTitle.Text = FEATURE_NAME
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ScenarioControl.ItemsSource = Scenarios
        If (Window.Current.Bounds.Width < 640) Then
            ScenarioControl.SelectedIndex = -1
        Else
            ScenarioControl.SelectedIndex = 0
        End If
    End Sub


    Private Sub ScenarioControl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        NotifyUser(String.Empty, NotifyType.StatusMessage)
        Dim scenarioListBox As ListBox = sender
        Dim s As Scenario = scenarioListBox.SelectedItem
        If (s IsNot Nothing) Then
            ScenarioFrame.Navigate(s.ClassType)
            If (Window.Current.Bounds.Width < 640) Then
                Splitter.IsPaneOpen = False
            End If
        End If
    End Sub

    Public Sub NotifyUser(_strMessage As String, _type As NotifyType)
        If (_type = NotifyType.StatusMessage) Then
            StatusBorder.Background = New SolidColorBrush(Windows.UI.Colors.Green)
        ElseIf (_type = NotifyType.ErrorMessage) Then
            StatusBorder.Background = New SolidColorBrush(Windows.UI.Colors.Green)
        End If
        StatusBlock.Text = _strMessage
        If (StatusBlock.Text <> String.Empty) Then
            StatusBlock.Visibility = Visibility.Visible
            StatusPanel.Visibility = Visibility.Visible
        Else
            StatusBlock.Visibility = Visibility.Collapsed
            StatusPanel.Visibility = Visibility.Collapsed
        End If
    End Sub

    Async Sub Footer_Click(sender As Object, e As RoutedEventArgs)
        Dim button As HyperlinkButton = sender
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(button.Tag.ToString()))
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Splitter.IsPaneOpen = Not Splitter.IsPaneOpen
    End Sub

End Class

Public Enum NotifyType
    StatusMessage
    ErrorMessage
End Enum

Public Class ScenarioBindingConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
        Dim s As Scenario = value
        Return (MainPage.Current.Scenarios.IndexOf(s) + 1).ToString() + ")" + s.Title
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
        Return True
    End Function
End Class