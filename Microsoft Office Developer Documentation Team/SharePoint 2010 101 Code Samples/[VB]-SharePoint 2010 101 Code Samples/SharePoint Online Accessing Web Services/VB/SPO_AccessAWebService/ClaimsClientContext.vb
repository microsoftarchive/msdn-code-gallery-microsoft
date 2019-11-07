Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports Microsoft.SharePoint.Client

Public Class ClaimsClientContext
    'Displays a pop up to login the user. An authentication Cookie is returned if the user is sucessfully authenticated.
    Public Shared Function GetAuthenticatedCookies(ByVal targetSiteUrl As String, ByVal popUpWidth As Integer, ByVal popUpHeight As Integer) As CookieCollection
        Dim authCookie As CookieCollection = Nothing
        Using webAuth As ClaimsWebAuth = New ClaimsWebAuth(targetSiteUrl, popUpWidth, popUpHeight)
            authCookie = webAuth.Show()
        End Using
        Return authCookie
    End Function

    'Override for for displaying pop. Default width and height values are used for the pop up window.
    Public Shared Function GetAuthenticatedContext(ByVal targetSiteUrl As String) As ClientContext
        Return GetAuthenticatedContext(targetSiteUrl, 0, 0)
    End Function

    'This method will return a ClientContext object with the authentication cookie set.
    'The ClientContext should be disposed of as any other IDisposable
    Public Shared Function GetAuthenticatedContext(ByVal targetSiteUrl As String, ByVal popUpWidth As Integer, ByVal popUpHeight As Integer) As ClientContext
        Dim cookies As CookieCollection = Nothing
        cookies = ClaimsClientContext.GetAuthenticatedCookies(targetSiteUrl, popUpWidth, popUpHeight)
        If cookies Is Nothing Then
            Return Nothing
        End If
        Dim context As ClientContext = New ClientContext(targetSiteUrl)
        Try
            AddHandler context.ExecutingWebRequest, Function(sender As Object, e As WebRequestEventArgs)
                                                        e.WebRequestExecutor.WebRequest.CookieContainer = New CookieContainer()
                                                        For Each cookie As Cookie In cookies
                                                            e.WebRequestExecutor.WebRequest.CookieContainer.Add(cookie)
                                                        Next
                                                    End Function
        Catch ex As Exception
            If Not context Is Nothing Then
                context.Dispose()
            End If
            Throw
        End Try
        Return context
    End Function


End Class
