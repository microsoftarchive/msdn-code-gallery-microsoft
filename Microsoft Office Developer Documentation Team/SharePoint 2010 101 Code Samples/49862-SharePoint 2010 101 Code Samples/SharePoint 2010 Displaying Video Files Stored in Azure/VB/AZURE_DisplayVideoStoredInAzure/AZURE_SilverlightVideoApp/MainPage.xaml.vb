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
        'Display the video's Source Uri
        txtLocation.Text = mediaVideo.Source.ToString()
    End Sub

End Class
