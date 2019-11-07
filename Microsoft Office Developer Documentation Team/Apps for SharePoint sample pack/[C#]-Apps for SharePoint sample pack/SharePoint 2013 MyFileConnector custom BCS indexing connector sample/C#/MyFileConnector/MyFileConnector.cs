using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Office.Server.Search.Connector.BDC;

namespace MyFileConnector
{
    public class MyFileConnector : StructuredRepositorySystemUtility<MyFileProxy>
    {
        protected override MyFileProxy CreateProxy()
        {
            return new MyFileProxy();
        }

        protected override void DisposeProxy(MyFileProxy proxy)
        {
            proxy.Dispose();
        }

        protected override void SetConnectionString(MyFileProxy proxy, string connectionString)
        {
            Uri startAddress = new Uri(connectionString);
            proxy.connect(@"\\" + startAddress.Host + startAddress.AbsolutePath.Replace('/', '\\'));
        }

        protected override void SetCertificates(MyFileProxy proxy, System.Security.Cryptography.X509Certificates.X509CertificateCollection certifcates)
        {
            throw new NotImplementedException();
        }

        protected override void SetCookies(MyFileProxy proxy, System.Net.CookieContainer cookies)
        {
            throw new NotImplementedException();
        }

        protected override void SetCredentials(MyFileProxy proxy, string userName, string passWord)
        {
            throw new NotImplementedException();
        }

        protected override void SetProxyServerInfo(MyFileProxy proxy, string proxyServerName, string bypassList, bool bypassProxyForLocalAddress)
        {
            throw new NotImplementedException();
        }
    }
}
