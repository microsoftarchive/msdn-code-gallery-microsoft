'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel.Design
Imports System.Diagnostics

Imports Microsoft.VisualStudio.Shell
Imports Microsoft.Samples.VisualStudio.Services.Interfaces

Namespace Microsoft.Samples.VisualStudio.Services
    ''' <summary>
    ''' This is the package that exposes the Visual Studio services.
    ''' In order to expose a service a package must implement the IServiceProvider interface (the one 
    ''' defined in the Microsoft.VisualStudio.OLE.Interop.dll interop assembly, not the one defined in the
    ''' .NET Framework) and notify the shell that it is exposing the services.
    ''' The implementation of the interface can be somewhat difficult and error prone because it is not 
    ''' designed for managed clients, but using the Managed Package Framework (MPF) we don’t really need
    ''' to write any code: if our package derives from the Package class, then it will get for free the 
    ''' implementation of IServiceProvider from the base class.
    ''' The notification to the shell about the exported service is done using the IProfferService interface
    ''' exposed by the SProfferService service; this service keeps a list of the services exposed globally 
    ''' by the loaded packages and allows the shell to find the service even if the service provider that 
    ''' exposes it is not inside the currently active chain of providers. If we simply use this service, 
    ''' then the service will be available for all the clients when the package is loaded, but the service
    ''' will be not usable when the package is not loaded. To avoid this problem and tell the shell that 
    ''' it has to make sure that this package is loaded when the service is queried, we have to register 
    ''' the service and package inside the services section of the registry. The MPF exposes the 
    ''' ProvideServiceAttribute registration attribute to add the information needed inside the registry, 
    ''' so that all we have to do is to use it in the definition of the class that implements the package.
    ''' </summary>
    <PackageRegistration(UseManagedResourcesOnly:=True), _
         InstalledProductRegistration("#112", "#113", "1.0", IconResourceID:=400), _
         ProvideService(GetType(SMyGlobalService)), _
         System.Runtime.InteropServices.Guid("0b6d8794-a92d-492e-bf6e-a8fbfddaf0dc")> _
    Public NotInheritable Class ServicesPackage
        Inherits Package
        ''' <summary>
        ''' Standard constructor for the package.
        ''' </summary>
        Public Sub New()
            ' Here we update the list of the provided services with the ones specific for this package.
            ' Notice that we set to true the boolean flag about the service promotion for the global:
            ' to promote the service is actually to proffer it globally using the SProfferService service.
            ' For performance reasons we don’t want to instantiate the services now, but only when and 
            ' if some client asks for them, so we here define only the type of the service and a function
            ' that will be called the first time the package will receive a request for the service. 
            ' This callback function is the one responsible for creating the instance of the service 
            ' object.
            Dim serviceContainer As IServiceContainer = TryCast(Me, IServiceContainer)
            Dim callback As New ServiceCreatorCallback(AddressOf CreateService)
            serviceContainer.AddService(GetType(SMyGlobalService), callback, True)
            serviceContainer.AddService(GetType(SMyLocalService), callback)
        End Sub

        ''' <summary>
        ''' This is the function that will create a new instance of the services the first time a client
        ''' will ask for a specific service type. It is called by the base class's implementation of
        ''' IServiceProvider.
        ''' </summary>
        ''' <param name="container">The IServiceContainer that needs a new instance of the service.
        '''                         This must be this package.</param>
        ''' <param name="serviceType">The type of service to create.</param>
        ''' <returns>The instance of the service.</returns>
        Private Function CreateService(ByVal container As IServiceContainer, ByVal serviceType As Type) As Object
            ' Check if the IServiceContainer is this package.
            If Not container Is Me Then
                Debug.WriteLine("ServicesPackage.CreateService called from an unexpected service container.")
                Return Nothing
            End If

            ' Find the type of the requested service and create it.
            If GetType(SMyGlobalService) Is serviceType Then
                ' Build the global service using this package as its service provider.
                Return New MyGlobalService(Me)
            End If
            If GetType(SMyLocalService) Is serviceType Then
                ' Build the local service using this package as its service provider.
                Return New MyLocalService(Me)
            End If

            ' If we are here the service type is unknown, so write a message on the debug output
            ' and return null.
            Debug.WriteLine("ServicesPackage.CreateService called for an unknown service type.")
            Return Nothing
        End Function
    End Class
End Namespace
