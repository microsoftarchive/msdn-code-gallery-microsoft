Imports AZURE_ClientSilverlightApp.DayNamerServiceReference

'This is the Silverlight application. Before you run this 
'project, remove the DayNamerServiceReference Service Reference
'and add a service reference to wherever you deployed the 
'WCF service. E.g. http://http://YourHostedServiceName.cloudapp.net/DayInfoService.svc
Partial Public Class MainPage
    Inherits UserControl

    Private privateSiteUrl As String

    Public Property SiteUrl() As String
        Get
            Return privateSiteUrl
        End Get
        Set(ByVal value As String)
            privateSiteUrl = value
        End Set
    End Property


    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub buttonGetToday_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles buttonGetToday.Click
        'Create the client proxy object
        Dim client As DayNamerClient = New DayNamerClient()
        'Call the TodayIs method asynchronously
        client.TodayIsAsync()
        'Handle its completed event
        AddHandler client.TodayIsCompleted, AddressOf client_TodayIsCompleted
    End Sub

    Private Sub client_TodayIsCompleted(ByVal sender As Object, ByVal args As TodayIsCompletedEventArgs)
        'Display the result
        resultsLabel.Content = args.Result.ToString()
    End Sub
End Class
