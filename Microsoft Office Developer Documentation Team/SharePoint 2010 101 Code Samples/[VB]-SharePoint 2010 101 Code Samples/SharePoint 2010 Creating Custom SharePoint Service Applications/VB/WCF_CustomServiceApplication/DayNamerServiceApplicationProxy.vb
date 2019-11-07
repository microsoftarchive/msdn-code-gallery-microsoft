Imports Microsoft.SharePoint.Administration
Imports System.Configuration
Imports System.ServiceModel
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Utilities
Imports System.ServiceModel.Configuration
Imports System.ServiceModel.Channels
Imports WCF_CustomServiceApplication.Server

Namespace Client

    'This class sets up the SharePoint load balancer and 
    'the client configuration.
    <System.Runtime.InteropServices.Guid("2FB8D9CE-3630-4342-9E48-A59670800838")> _
    Public Class DayNamerServiceApplicationProxy
        Inherits SPIisWebServiceApplicationProxy

        Private _channelFactory As ChannelFactory(Of IDayNamerContract)
        Private _channelFactoryLock As Object = New Object()
        Private _endpointConfigName As String
        Private _result As String

        <Persisted()> _
        Private _loadBalancer As SPServiceLoadBalancer

        Public Sub New()

        End Sub

        Public Sub New(ByVal name As String, _
                       ByVal proxy As DayNamerServiceProxy, _
                       ByVal serviceAddress As Uri)

            MyBase.New(name, proxy, serviceAddress)
            ' create instance of a new load balancer
            _loadBalancer = New SPRoundRobinServiceLoadBalancer(serviceAddress)

        End Sub

#Region "Service App Proxy Plumbing"

        Private Function CreateChannelFactory(Of T)(ByVal endpointConfigName As String) _
            As ChannelFactory(Of T)

            'Open the client.config
            Dim clientConfigPath As String = SPUtility.GetGenericSetupPath("WebClients\DayNamer")
            Dim clientConfig As Configuration = OpenClientConfiguration(clientConfigPath)
            Dim factory As ConfigurationChannelFactory(Of T) = New ConfigurationChannelFactory(Of T)(endpointConfigName, clientConfig, Nothing)

            'configure the channel factory
            factory.ConfigureCredentials(SPServiceAuthenticationMode.Claims)

            Return factory

        End Function

        Friend Delegate Sub CodeToRunOnApplicationProxy(ByVal appProxy As DayNamerServiceApplicationProxy)

        Friend Shared Sub Invoke(ByVal serviceContext As SPServiceContext, ByVal codeBlock As CodeToRunOnApplicationProxy)
            If serviceContext Is Nothing Then
                Throw New ArgumentNullException("serviceContext")
            End If

            'Get service app proxy from the context
            Dim proxy As DayNamerServiceApplicationProxy = DirectCast(serviceContext.GetDefaultProxy(GetType(DayNamerServiceApplicationProxy)), DayNamerServiceApplicationProxy)
            If proxy Is Nothing Then
                Throw New InvalidOperationException("Unable to obtain object reference to the day namer service application proxy.")
            End If

            'Run the code block on the proxy
            Using New SPServiceContextScope(serviceContext)
                codeBlock(proxy)
            End Using

        End Sub

        Private Function GetEndpointConfigName(ByVal address As Uri) As String
            Dim configName As String

            'Get the the config name for the provided address
            If address.Scheme = Uri.UriSchemeHttp Then
                configName = "http"
            ElseIf address.Scheme = Uri.UriSchemeHttps Then
                configName = "https"
            Else
                Throw New NotSupportedException("Unsupported endpoint address.")
            End If

            Return configName

        End Function

        Private Function GetChannel(ByVal address As Uri) As IDayNamerContract
            Dim endpointConfig As String = GetEndpointConfigName(address)

            'If there's a cached channel, use that
            If (_channelFactory Is Nothing) OrElse (endpointConfig <> _endpointConfigName) Then
                SyncLock [_channelFactoryLock]
                    'Create a channel factory using the endpoint name
                    _channelFactory = CreateChannelFactory(Of IDayNamerContract)(endpointConfig)
                    'Cache the created channel
                    _endpointConfigName = endpointConfig
                End SyncLock
            End If

            Dim channel As IDayNamerContract

            'Create a channel that acts as the logged on user when authenticating with the service
            Dim endPointAddress = New EndpointAddress(address)
            channel = _channelFactory.CreateChannelActingAsLoggedOnUser(endPointAddress)

            Return channel

        End Function

        Private Delegate Sub CodeToRunOnChannel(ByVal contract As IDayNamerContract)

        Private Sub ExecuteOnChannel(ByVal operationName As String, _
                                     ByVal codeBlock As CodeToRunOnChannel)

            Dim loadBalancerContext As SPServiceLoadBalancerContext = _loadBalancer.BeginOperation()

            Try
                'Get a channel to the service app endpoint
                Dim channel As IChannel = DirectCast(GetChannel(loadBalancerContext.EndpointAddress), IChannel)
                Try
                    'Execute the code block
                    codeBlock(DirectCast(channel, IDayNamerContract))
                    channel.Close()
                Catch te As TimeoutException
                    loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed
                    Throw
                Catch endpointex As EndpointNotFoundException
                    loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed
                    Throw
                Finally
                    If channel.State <> CommunicationState.Closed Then
                        channel.Abort()
                    End If
                End Try
            Finally
                loadBalancerContext.EndOperation()
            End Try

        End Sub
         
#End Region

        'assign the custom app proxy type name
        Public Overrides ReadOnly Property TypeName As String
            Get
                Return "Day Namer Service Application"
            End Get
        End Property

        'Provisioning the app proxy requires creating a new load balancer
        Public Overrides Sub Provision()
            _loadBalancer.Provision()
            MyBase.Provision()
            Me.Update()
        End Sub

        'Unprovisioning the app proxy requires deleting the load balancer
        Public Overrides Sub Unprovision(ByVal deleteData As Boolean)
            _loadBalancer.Unprovision()
            MyBase.Unprovision(deleteData)
            Me.Update()
        End Sub

#Region "Service Application Methods"

        'These methods match those in the service application service contract
        Public Function TodayIs() As String
            _result = String.Empty

            'Execute the call against the service app
            ExecuteOnChannel("TodayIs", AddressOf todayIsCallback)

            Return _result
        End Function

        Private Sub todayIsCallback(ByVal channel As IDayNamerContract)
            _result = channel.TodayIs()
        End Sub

        Public Function TodayAdd(ByVal daysToAdd As Integer) As String
            _result = String.Empty

            'Execute the call against the service app
            ExecuteOnChannel("TodayAdd", AddressOf todayAddCallback)

            Return _result
        End Function

        Private Sub todayAddCallback(ByVal channel As IDayNamerContract)
            _result = channel.TodayAdd(10)
        End Sub


#End Region

    End Class

End Namespace
