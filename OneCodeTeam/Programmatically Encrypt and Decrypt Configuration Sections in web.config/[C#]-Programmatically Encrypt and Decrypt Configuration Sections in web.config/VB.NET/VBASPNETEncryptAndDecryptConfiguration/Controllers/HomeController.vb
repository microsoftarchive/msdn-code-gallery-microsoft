Imports System.Web.Mvc
Imports System.Configuration
Imports System.Web.Configuration

Namespace Controllers
    Public Class HomeController
        Inherits Controller

        Private Const provider As String = "RSAProtectedConfigurationProvider"
        'Use RSA Provider to encrypt configuration 

        ' GET: Home
        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost> _
        Public Function EncryptConfig(sectionName As String) As ActionResult
            If String.IsNullOrEmpty(sectionName) Then
                Return Nothing
            End If

            Dim config As Configuration = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath)
            Dim section As ConfigurationSection = config.GetSection(sectionName)
            section.SectionInformation.ProtectSection(Provider)
            config.Save()
            Return Content("Success")
        End Function

        <HttpPost> _
        Public Function DecryptConfig(sectionName As String) As ActionResult
            If String.IsNullOrEmpty(sectionName) Then
                Return Nothing
            End If
            Dim config As Configuration = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath)
            Dim section As ConfigurationSection = config.GetSection(sectionName)
            If section.SectionInformation.IsProtected Then
                section.SectionInformation.UnprotectSection()
                config.Save()
            End If
            Return Content("Success")
        End Function
    End Class
End Namespace