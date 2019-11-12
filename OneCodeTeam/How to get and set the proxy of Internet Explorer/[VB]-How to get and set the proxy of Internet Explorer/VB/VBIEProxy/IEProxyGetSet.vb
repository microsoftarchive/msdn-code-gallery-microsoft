Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Imports System.ComponentModel

Public Class IEProxyGetSet

    Private Const INTERNET_OPTION_SETTINGS_CHANGED As Integer = 39
    ' Causes the proxy data to be reread from the registry for a handle.
    Private Const INTERNET_OPTION_REFRESH As Integer = 37
    ' Sets or retrieves an INTERNET_PROXY_INFO structure that contains the proxy data for an existing InternetOpen handle.
    Private Const INTERNET_OPTION_PROXY As Integer = 38
    Private Const RegistryKeyPath As String = "HKEY_CURRENT_USER\Software\Microsoft\" + "Windows\CurrentVersion\Internet Settings"
    ' Initializes an application's use of the WinINet functions.
    <DllImport("wininet.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function InternetOpen(lpszAgent As String, dwAccessType As Integer, lpszProxyName As String, lpszProxyBypass As String, dwFlags As Integer) As IntPtr
    End Function
    ' Closes Internet handle.
    <DllImport("wininet.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function InternetCloseHandle(hInternet As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    ' Queries an Internet option on the specified handle.
    <DllImport("wininet.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Private Shared Function InternetQueryOption(hInternet As IntPtr, dwOption As UInteger, lpBuffer As IntPtr, ByRef lpdwBufferLength As Integer) As Boolean
    End Function
    ' Sets an Internet option.
    <DllImport("wininet.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Private Shared Function InternetSetOption(hInternet As IntPtr, dwOption As Integer, lpBuffer As IntPtr, dwBufferLength As Integer) As Boolean
    End Function

    Enum InternetOpenType
        INTERNET_OPEN_TYPE_PRECONFIG = 0
        INTERNET_OPEN_TYPE_DIRECT = 1
        INTERNET_OPEN_TYPE_PROXY = 3
    End Enum

    Structure INTERNET_PROXY_INFO
        Public Property dwAccessType() As InternetOpenType
            Get
                Return m_dwAccessType
            End Get
            Set(value As InternetOpenType)
                m_dwAccessType = value
            End Set
        End Property
        Private m_dwAccessType As InternetOpenType
        Public Property lpszProxy() As String
            Get
                Return m_lpszProxy
            End Get
            Set(value As String)
                m_lpszProxy = value
            End Set
        End Property
        Private m_lpszProxy As String
        Public Property lpszProxyBypass() As String
            Get
                Return m_lpszProxyBypass
            End Get
            Set(value As String)
                m_lpszProxyBypass = value
            End Set
        End Property
        Private m_lpszProxyBypass As String
    End Structure

    Private Sub btnGetProxy_Click(sender As Object, e As EventArgs) Handles btnGetProxy.Click

        Dim bufferLength As Integer = 0
        Dim buffer As IntPtr = IntPtr.Zero

        InternetQueryOption(IntPtr.Zero, INTERNET_OPTION_PROXY, IntPtr.Zero, bufferLength)
        Try
            buffer = Marshal.AllocHGlobal(bufferLength)
            If InternetQueryOption(IntPtr.Zero, INTERNET_OPTION_PROXY, buffer, bufferLength) Then
                ' Converting structure to IntPtr.
                Dim proxyInfo As INTERNET_PROXY_INFO = DirectCast(Marshal.PtrToStructure(buffer, GetType(INTERNET_PROXY_INFO)), INTERNET_PROXY_INFO)
                ' Getting the proxy details.
                Select Case proxyInfo.dwAccessType.ToString()
                    Case "INTERNET_OPEN_TYPE_PRECONFIG"
                        cmbAccessType.SelectedIndex = 0
                        Exit Select
                    Case "INTERNET_OPEN_TYPE_DIRECT"
                        cmbAccessType.SelectedIndex = 1
                        Exit Select
                    Case "INTERNET_OPEN_TYPE_PROXY"
                        cmbAccessType.SelectedIndex = 2
                        Exit Select
                    Case Else
                        Exit Select
                End Select
                tbProxyServer.Text = proxyInfo.lpszProxy
                tbProxyByPass.Text = proxyInfo.lpszProxyBypass
                If Registry.GetValue(RegistryKeyPath, "ProxyEnable", "").ToString() = "1" Then
                    cmbProxyStatusInfo.SelectedIndex = 1
                Else
                    cmbProxyStatusInfo.SelectedIndex = 0
                End If
                MessageBox.Show("Successfully retrieved Internet Explorer proxy settings!", "Proxy Modifier", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Throw New Win32Exception()
            End If
        Catch exception As Exception
            MessageBox.Show(exception.Message, "Proxy Modifier", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        Finally
            If buffer <> IntPtr.Zero Then
                Marshal.FreeHGlobal(buffer)
            End If
        End Try

    End Sub

    Private Sub IEProxyGetSet_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim hInternet As IntPtr = InternetOpen("Browser", CInt(InternetOpenType.INTERNET_OPEN_TYPE_DIRECT), Nothing, Nothing, 0)
        If IntPtr.Zero = hInternet Then
            MessageBox.Show("InternetOpen returned null.", "Proxy Modifier", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Return
        End If

    End Sub


    Private Sub btnSetProxy_Click(sender As Object, e As EventArgs) Handles btnSetProxy.Click

        ' Setting the proxy details.
        Registry.SetValue(RegistryKeyPath, "ProxyServer", tbProxyServer.Text)
        If cmbProxyStatusInfo.SelectedIndex = 0 Then
            Registry.SetValue(RegistryKeyPath, "ProxyEnable", 0, RegistryValueKind.DWord)
        Else
            Registry.SetValue(RegistryKeyPath, "ProxyEnable", 1, RegistryValueKind.DWord)
        End If
        Registry.SetValue(RegistryKeyPath, "ProxyOverride", tbProxyByPass.Text)

        ' Forcing the OS to refresh the IE settings to reflect new proxy settings.
        InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0)
        InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0)
        MessageBox.Show("Internet Explorer proxy settings updated!", "Proxy Modifier", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub
End Class