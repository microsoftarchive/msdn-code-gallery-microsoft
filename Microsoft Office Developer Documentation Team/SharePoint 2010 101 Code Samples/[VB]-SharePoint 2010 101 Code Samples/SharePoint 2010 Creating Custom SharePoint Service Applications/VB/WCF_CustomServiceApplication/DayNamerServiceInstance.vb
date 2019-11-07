Imports Microsoft.SharePoint.Administration

Namespace Server

    'This class enables SharePoint to create instances of the 
    'custom service application on one or more application servers
    'in the farm. When a client calls the service application
    'they don't need to know which servers it runs on.
    Public Class DayNamerServiceInstance
        Inherits SPIisWebServiceInstance

#Region "Constructors"

        Public Sub New()

        End Sub

        Public Sub New(ByVal server As SPServer, ByVal service As DayNamerService)
            MyBase.New(server, service)
        End Sub

#End Region

#Region "Properties"

        Public Overrides ReadOnly Property DisplayName As String
            Get
                Return "Day Namer Service"
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String
            Get
                Return "Day Namer service application providing simple day names."
            End Get
        End Property

        Public Overrides ReadOnly Property TypeName As String
            Get
                Return "Day Namer Service"
            End Get
        End Property

#End Region

    End Class

End Namespace
