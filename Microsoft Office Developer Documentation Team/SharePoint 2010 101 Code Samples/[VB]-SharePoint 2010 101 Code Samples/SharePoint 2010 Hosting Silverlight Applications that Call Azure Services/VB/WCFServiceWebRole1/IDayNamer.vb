
'This is the interface that defines the WCF service contract
<ServiceContract()>
Public Interface IDayNamer

    'Two simple operation contracts that return strings
    <OperationContract()> _
    Function TodayIs() As String

    <OperationContract()> _
    Function TodayAdd(ByVal daysToAdd As Integer) As String

End Interface



