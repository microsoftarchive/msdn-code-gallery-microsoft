'**************************** Module Header ******************************\
' Module Name:  CallbackMethods.vb
' Project:      VBO365ExportContacts
' Copyright (c) Microsoft Corporation.
' 
' Outlook Web App (OWA) allows us to import multiple contacts in a very simple 
' way. However, it does not allow to export contacts. In this application, we 
' will demonstrate how to export contacts from Office 365 Exchange Online.
' This file includes the methods used as the callback methods.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Security.Cryptography.X509Certificates

Namespace VBO365ExportContacts
    Public NotInheritable Class CallbackMethods
        Private Sub New()
        End Sub
        Public Shared Function RedirectionUrlValidationCallback(ByVal redirectionUrl As String) As Boolean
            ' The default for the validation callback is to reject the URL.
            Dim result As Boolean = False

            Dim redirectionUri As New Uri(redirectionUrl)

            ' Validate the contents of the redirection URL. In this simple validation
            ' callback, the redirection URL is considered valid if it is using HTTPS
            ' to encrypt the authentication credentials. 
            If redirectionUri.Scheme = "https" Then
                result = True
            End If
            Return result
        End Function


        Public Shared Function CertificateValidationCallBack(ByVal sender As Object,
            ByVal certificate As System.Security.Cryptography.X509Certificates.X509Certificate,
            ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain,
            ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
            'return true;
            ' If the certificate is a valid, signed certificate, return true.
            If sslPolicyErrors = System.Net.Security.SslPolicyErrors.None Then
                Return True
            End If

            ' If there are errors in the certificate chain, look at each error to determine the cause.
            If (sslPolicyErrors And System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) <> 0 Then
                If chain IsNot Nothing AndAlso chain.ChainStatus IsNot Nothing Then
                    For Each status As System.Security.Cryptography.X509Certificates.X509ChainStatus In chain.ChainStatus
                        If (certificate.Subject = certificate.Issuer) AndAlso
                            (status.Status =
                System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot) Then
                            ' Self-signed certificates with an untrusted root are valid. 
                            Continue For
                        Else
                            If status.Status <>
                System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError Then
                                ' If there are any other errors in the certificate chain, the certificate is invalid,
                                ' so the method returns false.
                                Return False
                            End If
                        End If
                    Next status
                End If

                ' When processing reaches this line, the only errors in the certificate chain are 
                ' untrusted root errors for self-signed certificates. These certificates are valid
                ' for default Exchange server installations, so return true.
                Return True
            Else
                ' In all other cases, return false.
                Return False
            End If
        End Function
    End Class
End Namespace
