Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.Administration
Imports WCF_CustomServiceApplication.Server
Imports WCF_CustomServiceApplication.Client

Namespace Features

    <GuidAttribute("eedc5939-6c90-4375-9ee1-4a2c7630c0cb")> _
    Public Class Feature1EventReceiver
        Inherits SPFeatureReceiver

        Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
            'install the service
            Dim service As DayNamerService = SPFarm.Local.Services.GetValue(Of DayNamerService)()
            If service Is Nothing Then
                service = New DayNamerService(SPFarm.Local)
                service.Update()
            End If

            'install the service proxy
            Dim serviceProxy As DayNamerServiceProxy = SPFarm.Local.ServiceProxies.GetValue(Of DayNamerServiceProxy)()
            If serviceProxy Is Nothing Then
                serviceProxy = New DayNamerServiceProxy(SPFarm.Local)
                serviceProxy.Update(True)
            End If

            'with service added to the farm, install instance
            Dim serviceInstance As DayNamerServiceInstance = New DayNamerServiceInstance(SPServer.Local, service)
            serviceInstance.Update(True)
        End Sub

        Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
            'uninstall the instance
            Dim serviceInstance As DayNamerServiceInstance = SPFarm.Local.Services.GetValue(Of DayNamerServiceInstance)()
            If serviceInstance IsNot Nothing Then
                SPServer.Local.ServiceInstances.Remove(serviceInstance.Id)
            End If
            'uninstall the service proxy
            Dim serviceProxy As DayNamerServiceProxy = SPFarm.Local.ServiceProxies.GetValue(Of DayNamerServiceProxy)()
            If serviceProxy IsNot Nothing Then
                SPFarm.Local.ServiceProxies.Remove(serviceProxy.Id)
            End If
            'uninstall the service
            Dim service As DayNamerService = SPFarm.Local.Services.GetValue(Of DayNamerService)()
            If service IsNot Nothing Then
                SPFarm.Local.Services.Remove(service.Id)
            End If
        End Sub

    End Class

End Namespace
