Imports System.ServiceModel

Namespace Server

    'This interface defines the WCF service contract for
    'the custom service application. 
    <ServiceContract()> _
    Public Interface IDayNamerContract

        <OperationContract()> _
        Function TodayIs() As String

        <OperationContract()> _
        Function TodayAdd(ByVal daysToAdd As Integer) As String

    End Interface

End Namespace
