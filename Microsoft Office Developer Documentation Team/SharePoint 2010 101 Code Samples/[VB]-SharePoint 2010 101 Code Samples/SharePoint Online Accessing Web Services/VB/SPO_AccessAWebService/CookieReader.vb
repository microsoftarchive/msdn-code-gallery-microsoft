Imports System.Text
Imports System.Runtime.InteropServices

'WinInet.dll wrapper
Friend NotInheritable Class CookieReader
    'Enables the retrieval of cookies that are marked as "HTTPOnly". 
    'Do not use this flag if you expose a scriptable interface, 
    'because this has security implications. It is imperative that 
    'you use this flag only if you can guarantee that you will never 
    'expose the cookie to third-party code by way of an 
    'extensibility mechanism you provide. 
    'Version:  Requires Internet Explorer 8.0 or later.
    Private Const INTERNET_COOKIE_HTTPONLY As Integer = 8192

    <DllImport("wininet.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Private Shared Function InternetGetCookieEx(ByVal url As String, _
                                                ByVal cookieName As String, _
                                                ByVal cookieData As StringBuilder, _
                                                ByRef size As Integer, _
                                                ByVal flags As Integer, _
                                                ByVal pReserved As IntPtr) As Boolean

    End Function

    'Returns cookie contents as a string
    Public Shared Function GetCookie(ByVal url As String) As String
        Dim size As String = 512
        Dim sb As StringBuilder = New StringBuilder(size)
        If Not InternetGetCookieEx(url, Nothing, sb, size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero) Then
            If size < 0 Then
                Return Nothing
            End If
            sb = New StringBuilder(size)
            If Not InternetGetCookieEx(url, Nothing, sb, size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero) Then
                Return Nothing
            End If
        End If
        Return sb.ToString()
    End Function

End Class
