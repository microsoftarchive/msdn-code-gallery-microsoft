'***************************** Module Header ******************************\
' Module Name: Module1.vb
' Project:     VBAzureStartDeallocatedVM
' Copyright (c) Microsoft Corporation.
' 
' This sample will show you how to stop VM(Deallocated) and start it.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/
Imports System.Security.Cryptography.X509Certificates
Imports System.Xml
Imports System.Net
Imports System.IO

Module Module1

    Public SubscriptionID As String = "<Subscription ID>"

    ' A VM need a host service, you can create it in your Azure portal.
    Public VMName As String = "<VM Name>"

    ' You need to make sure this certificate is in your Azure management certificate pool.
    ' And it's also in your local computer personal certificate pool.
    Public CertificateThumbprint As String = "<Certificate Thumbprint>"
    Public Certificate As X509Certificate2
    Sub Main()
        Dim certificateStore As New X509Store(StoreName.My, StoreLocation.CurrentUser)
        certificateStore.Open(OpenFlags.[ReadOnly])
        Dim certs As X509Certificate2Collection = certificateStore.Certificates.Find(X509FindType.FindByThumbprint, CertificateThumbprint, False)
        If certs.Count = 0 Then
            Console.WriteLine("Can't find the certificate in your local computer.")
            Console.ReadKey()
            Return
        Else
            Certificate = certs(0)
        End If

        'StartVirtualMachine(SubscriptionID, Certificate, VMName, VMName, VMName);
        StopVirtualMachine(SubscriptionID, Certificate, VMName, VMName, VMName, False)

    End Sub

    Private Sub StartVirtualMachine(subscriptionID__1 As String, cer As X509Certificate2, serviceName As String, deploymentsName As String, vmName As String)
        Dim request As HttpWebRequest = DirectCast(HttpWebRequest.Create(New Uri("https://management.core.windows.net/" & SubscriptionID & "/services/hostedservices/" & serviceName & "/deployments/" & deploymentsName & "/roleinstances/" & vmName & "/Operations")), HttpWebRequest)

        request.Method = "POST"
        request.ClientCertificates.Add(Certificate)
        request.ContentType = "application/xml"
        request.Headers.Add("x-ms-version", "2013-06-01")

        ' Add body to the request
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load("..\..\StartVM.xml")

        Dim requestStream As Stream = request.GetRequestStream()
        Dim streamWriter As New StreamWriter(requestStream, System.Text.UTF8Encoding.UTF8)
        xmlDoc.Save(streamWriter)

        streamWriter.Close()
        requestStream.Close()
        Try
            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            response.Close()
            Console.WriteLine("Operation succeed!")
            Console.ReadKey()
        Catch ex As WebException

            Console.Write(ex.Response.Headers.ToString())
            Console.Read()
        End Try
    End Sub

    Private Sub StopVirtualMachine(subscriptionID As String, cer As X509Certificate2, serviceName As String, deploymentsName As String, vmName As String, Deallocated As Boolean)
        Dim request As HttpWebRequest = DirectCast(HttpWebRequest.Create(New Uri("https://management.core.windows.net/" & subscriptionID & "/services/hostedservices/" & serviceName & "/deployments/" & deploymentsName & "/roleinstances/" & vmName & "/Operations")), HttpWebRequest)

        request.Method = "POST"
        request.ClientCertificates.Add(cer)
        request.ContentType = "application/xml"
        request.Headers.Add("x-ms-version", "2013-06-01")

        ' Add body to the reqeust 
        Dim xmlDoc As New XmlDocument()
        If Deallocated Then
            xmlDoc.Load("..\..\StopVM_Deallocated.xml")
        Else
            xmlDoc.Load("..\..\StopVM.xml")
        End If

        Dim requestStream As Stream = request.GetRequestStream()
        Dim streamWriter As New StreamWriter(requestStream, System.Text.UTF8Encoding.UTF8)
        xmlDoc.Save(streamWriter)

        streamWriter.Close()
        requestStream.Close()
        Try
            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            response.Close()
            Console.WriteLine("Operation succeed!")
            Console.ReadKey()
        Catch ex As WebException

            Console.Write(ex.Response.Headers.ToString())
            Console.Read()
        End Try

    End Sub

End Module
