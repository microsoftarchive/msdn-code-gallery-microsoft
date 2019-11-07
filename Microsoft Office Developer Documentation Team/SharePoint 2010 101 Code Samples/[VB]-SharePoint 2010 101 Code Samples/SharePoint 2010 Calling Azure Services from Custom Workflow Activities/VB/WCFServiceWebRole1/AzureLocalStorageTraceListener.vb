Imports System
Imports System.Diagnostics
Imports System.IO
Imports Microsoft.WindowsAzure.Diagnostics
Imports Microsoft.WindowsAzure.ServiceRuntime

Public Class AzureLocalStorageTraceListener
    Inherits XmlWriterTraceListener

    Public Sub New()
        MyBase.New(Path.Combine(AzureLocalStorageTraceListener.GetLogDirectory().Path, "WCFServiceWebRole1.svclog"))
    End Sub

    Public Shared Function GetLogDirectory() As DirectoryConfiguration
        Dim directory As DirectoryConfiguration = New DirectoryConfiguration
        directory.Container = "wad-tracefiles"
        directory.DirectoryQuotaInMB = 10
        directory.Path = RoleEnvironment.GetLocalResource("WCFServiceWebRole1.svclog").RootPath
        Return directory
    End Function

End Class
