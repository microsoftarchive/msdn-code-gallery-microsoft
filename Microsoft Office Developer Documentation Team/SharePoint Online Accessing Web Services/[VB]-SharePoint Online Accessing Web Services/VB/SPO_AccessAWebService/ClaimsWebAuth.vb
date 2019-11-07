Imports System.Windows.Forms
Imports System.Net
Imports System.IO
Imports System.Net.Security
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Globalization


Public Class ClaimsWebAuth
    Implements IDisposable

#Region "Constructors"

    'Displays a pop up window to authenticate the user
    Public Sub New(ByVal targetSiteUrl As String, ByVal popUpWdith As Integer, ByVal popUpHeight As Integer)
        If String.IsNullOrEmpty(targetSiteUrl) Then
            Throw New ArgumentException(Constants.MSG_REQUIRED_SITE_URL)
        End If
        Me.fldTargetSiteUrl = targetSiteUrl
        'set login page url and success url from target site
        Me.GetClaimParams(Me.fldTargetSiteUrl, Me.fldLoginPageUrl, Me.fldNavigationEndUrl)

        Me.PopUpWidth = popUpWdith
        Me.PopUpHeight = popUpHeight

        Me.webBrowser = New WebBrowser()
        AddHandler Me.webBrowser.Navigated, AddressOf ClaimsWebBrowser_Navigated
        Me.webBrowser.ScriptErrorsSuppressed = True
        Me.webBrowser.Dock = DockStyle.Fill

    End Sub

#End Region

#Region "Private Fields"

    Private webBrowser As WebBrowser
    Private fldCookies As CookieCollection
    Private DisplayLoginForm As Form

#End Region

#Region "Public Properties"
    'Target site Url
    Private fldTargetSiteUrl As String = Nothing
    Public Property TargetSiteUrl() As String
        Get
            Return fldTargetSiteUrl
        End Get
        Set(ByVal value As String)
            fldTargetSiteUrl = value
        End Set
    End Property

    'Cookies returned from CLAIM server.
    Public ReadOnly Property AuthCookies() As CookieCollection
        Get
            Return fldCookies
        End Get
    End Property

    'Login form Url
    Private fldLoginPageUrl As String
    Public Property LoginPageUrl() As String
        Get
            Return fldLoginPageUrl
        End Get
        Set(ByVal value As String)
            fldLoginPageUrl = value
        End Set
    End Property

    'Success Url
    Private fldNavigationEndUrl As Uri
    Public Property NavigationEndUrl() As Uri
        Get
            Return fldNavigationEndUrl
        End Get
        Set(ByVal value As Uri)
            fldNavigationEndUrl = value
        End Set
    End Property

    'Width of Login Dialog
    Private fldPopUpWidth As Integer = 0
    Public Property PopUpWidth() As Integer
        Get
            Return fldPopUpWidth
        End Get
        Set(ByVal value As Integer)
            fldPopUpWidth = value
        End Set
    End Property

    'Height of Login Dialog
    Private fldPopUpHeight As Integer
    Public Property PopUpHeight() As Integer
        Get
            Return fldPopUpHeight
        End Get
        Set(ByVal value As Integer)
            fldPopUpHeight = value
        End Set
    End Property

    'Is set to true if the CLAIM site did not return the proper headers -- hence it's not an CLAIM site or does not support CLAIM style authentication
    Private fldIsCLAIMSite As Boolean = False
    Public ReadOnly Property IsClaimSite() As Boolean
        Get
            Return fldIsCLAIMSite
        End Get
    End Property

#End Region

#Region "Private Methods"

    Private Sub GetClaimParams(ByVal targetUrl As String, ByRef loginUrl As String, ByRef navigationEndUrl As Uri)
        Dim webRequest As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(targetUrl), HttpWebRequest)
        webRequest.Method = Constants.WR_METHOD_OPTIONS
#If DEBUG Then
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf IgnoreCertificateErrorHandler)
#End If
        Try
            Dim response As WebResponse = DirectCast(webRequest.GetResponse(), WebResponse)
            ExtraHeadersFromResponse(response, loginUrl, navigationEndUrl)
        Catch webEx As WebException
            ExtraHeadersFromResponse(webEx.Response, loginUrl, navigationEndUrl)
        End Try
    End Sub

    Private Function ExtraHeadersFromResponse(ByVal response As WebResponse, ByRef loginUrl As String, ByRef navigationEndUrl As Uri) As Boolean
        loginUrl = Nothing
        navigationEndUrl = Nothing
        Try
            navigationEndUrl = New Uri(response.Headers(Constants.CLAIM_HEADER_RETURN_URL))
            loginUrl = (response.Headers(Constants.CLAIM_HEADER_AUTH_REQUIRED))
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function ExtractAuthCookiesFromUrl(ByVal url As String) As CookieCollection
        Dim uriBase As Uri = New Uri(url)
        Dim uri = New Uri(uriBase, "/")
        'Call the WinInet.dll to get cookie
        Dim stringCookie As String = CookieReader.GetCookie(uri.ToString())
        If String.IsNullOrEmpty(stringCookie) Then
            Return Nothing
        End If
        stringCookie = stringCookie.Replace("; ", ",").Replace(";", ",")
        'use CookieContainer to parse the string cookie to CookieCollection
        Dim cookieContainer As CookieContainer = New CookieContainer()
        cookieContainer.SetCookies(uri, stringCookie)
        Return cookieContainer.GetCookies(uri)
    End Function

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Opens a Windows Forms Web Browser control to authenticate the user against an CLAIM site.
    ''' </summary>
    ''' <param name="popUpWidth"></param>
    ''' <param name="popUpHeight"></param>
    Public Function Show() As CookieCollection
        If String.IsNullOrEmpty(Me.LoginPageUrl) Then
            Throw New ApplicationException(Constants.MSG_NOT_CLAIM_SITE)
        End If
        'navigate to the login page url
        Me.webBrowser.Navigate(Me.LoginPageUrl)
        DisplayLoginForm = New Form()
        DisplayLoginForm.SuspendLayout()
        'size the form
        Dim dialogWidth As Integer = Constants.DEFAULT_POP_UP_WIDTH
        Dim dialogHeight As Integer = Constants.DEFAULT_POP_UP_HEIGHT
        If PopUpHeight <> 0 And PopUpWidth <> 0 Then
            dialogWidth = Convert.ToInt32(PopUpWidth)
            dialogHeight = Convert.ToInt32(PopUpHeight)
        End If

        DisplayLoginForm.Width = dialogWidth
        DisplayLoginForm.Height = dialogHeight
        DisplayLoginForm.Text = Me.fldTargetSiteUrl

        DisplayLoginForm.Controls.Add(Me.webBrowser)
        DisplayLoginForm.ResumeLayout(False)

        Application.Run(DisplayLoginForm)
        'See ClaimsWebBrowser_Navigated event
        Return Me.fldCookies
    End Function

#End Region

#Region "Private Events"

    Private Sub ClaimsWebBrowser_Navigated(ByVal sender As Object, ByVal e As WebBrowserNavigatedEventArgs)
        'check whether the url is same as the navigationEndUrl.
        If Not fldNavigationEndUrl Is Nothing And fldNavigationEndUrl.Equals(e.Url) Then
            Me.fldCookies = ExtractAuthCookiesFromUrl(Me.LoginPageUrl)
            Me.DisplayLoginForm.Close()
        End If
    End Sub

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not Me.webBrowser Is Nothing Then
                Me.webBrowser.Dispose()
            End If
            If Not Me.DisplayLoginForm Is Nothing Then
                Me.DisplayLoginForm.Dispose()
            End If
        End If
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "Utilities"

#If DEBUG Then
    Private Function IgnoreCertificateErrorHandler(ByVal sender As Object,
                                                   ByVal certificate As System.Security.Cryptography.X509Certificates.X509Certificate,
                                                   ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain,
                                                   ByVal sslPolicyErros As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function
#End If

#End Region

End Class
