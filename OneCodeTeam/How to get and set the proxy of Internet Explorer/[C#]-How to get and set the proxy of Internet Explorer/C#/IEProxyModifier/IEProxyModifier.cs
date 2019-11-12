using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace IEProxyModifier
{
    public partial class IEProxyModifierForm : Form
    {
        // Notifies the system that the registry settings have been changed so that it
        // verifies the settings on the next call to InternetConnect.
        private const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        // Causes the proxy data to be reread from the registry for a handle.
        // No buffer is required.
        private const int INTERNET_OPTION_REFRESH = 37;
        // Sets or retrieves an INTERNET_PROXY_INFO structure that contains the
        // proxy data for an existing InternetOpen handle.
        private const int INTERNET_OPTION_PROXY = 38;
        private const string RegistryKeyPath = "HKEY_CURRENT_USER\\Software\\Microsoft\\"
                                           + "Windows\\CurrentVersion\\Internet Settings";
        // Initializes an application's use of the WinINet functions.
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr InternetOpen(string lpszAgent, int dwAccessType,
                    string lpszProxyName, string lpszProxyBypass, int dwFlags);
        // Closes a single Internet handle.
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool InternetCloseHandle(IntPtr hInternet);
        // Queries an Internet option on the specified handle.
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetQueryOption(IntPtr hInternet, uint dwOption,
                                    IntPtr lpBuffer, ref int lpdwBufferLength);
        // Sets an Internet option.
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetSetOption(IntPtr hInternet, int dwOption,
                                            IntPtr lpBuffer, int dwBufferLength);
    /// <summary>
    /// Access types supported by InternetOpen function.
    /// </summary>        
    enum InternetOpenType
    {
        INTERNET_OPEN_TYPE_PRECONFIG = 0,
        INTERNET_OPEN_TYPE_DIRECT = 1,
        INTERNET_OPEN_TYPE_PROXY = 3,
    }

    /// <summary>
    /// Contains information that is supplied with the INTERNET_OPTION_PROXY value
    /// to get or set proxy information on a handle obtained from a call to 
    /// the InternetOpen function.
    /// </summary>
    struct INTERNET_PROXY_INFO
    {
        public InternetOpenType dwAccessType { get; set; }
        public string lpszProxy { get; set; }
        public string lpszProxyBypass { get; set; }
    }

        public IEProxyModifierForm()
        {
            InitializeComponent();
        }

    private void btnGetProxy_Click(object sender, EventArgs e)
    {
        int bufferLength = 0;
        IntPtr buffer = IntPtr.Zero;

        InternetQueryOption(IntPtr.Zero, INTERNET_OPTION_PROXY, IntPtr.Zero,
                            ref bufferLength);
        try
        {
            buffer = Marshal.AllocHGlobal(bufferLength);
            if (InternetQueryOption(IntPtr.Zero, INTERNET_OPTION_PROXY, buffer,
                                    ref bufferLength))
            {
                INTERNET_PROXY_INFO proxyInfo = (INTERNET_PROXY_INFO)
                    // Converting structure to IntPtr.
                Marshal.PtrToStructure(buffer, typeof(INTERNET_PROXY_INFO));
                // Getting the proxy details.
                switch (proxyInfo.dwAccessType.ToString())
                {
                    case "INTERNET_OPEN_TYPE_PRECONFIG":
                        cmbAccessType.SelectedIndex = 0;
                        break;
                    case "INTERNET_OPEN_TYPE_DIRECT":
                        cmbAccessType.SelectedIndex = 1;
                        break;
                    case "INTERNET_OPEN_TYPE_PROXY":
                        cmbAccessType.SelectedIndex = 2;
                        break;
                    default:
                        break;
                }
                tbProxyServer.Text = proxyInfo.lpszProxy;
                tbProxyByPass.Text = proxyInfo.lpszProxyBypass;
                if (Registry.GetValue(RegistryKeyPath, "ProxyEnable", "").
                    ToString() == "1")
                {
                    cmbProxyStatusInfo.SelectedIndex = 1;
                }
                else
                {
                    cmbProxyStatusInfo.SelectedIndex = 0;
                }
                MessageBox.Show("Internet Explorer proxy settings has been retrieved.",
                "Proxy Modifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                throw new Win32Exception();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Proxy Modifier", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
        finally
        {
            if (buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }

        private void IEProxyModifierForm_Load(object sender, EventArgs e)
        {
            IntPtr hInternet = InternetOpen("Browser",
                (int)InternetOpenType.INTERNET_OPEN_TYPE_DIRECT, null, null, 0);
            if (IntPtr.Zero == hInternet)
            {
                MessageBox.Show("InternetOpen returned null.", "Proxy Modifier",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnSetProxy_Click(object sender, EventArgs e)
        {
            // Setting the proxy details.
            Registry.SetValue(RegistryKeyPath, "ProxyServer", tbProxyServer.Text);
            if (cmbProxyStatusInfo.SelectedIndex == 0)
            {
                Registry.SetValue(RegistryKeyPath, "ProxyEnable", 0,
                    RegistryValueKind.DWord);
            }
            else
            {
                Registry.SetValue(RegistryKeyPath, "ProxyEnable", 1,
                    RegistryValueKind.DWord);
            }
            Registry.SetValue(RegistryKeyPath, "ProxyOverride", tbProxyByPass.Text);

            // Forcing the OS to refresh the IE settings to reflect new proxy settings.
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED,
                                IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            MessageBox.Show("Internet Explorer proxy settings has been changed.",
                "Proxy Modifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}