Imports System.Runtime.Serialization
Imports System.ServiceModel

Namespace WCF_ServiceHostedInSharePoint

    'Define the service contract for the WCF service
    <ServiceContract()> _
    Public Interface ISharePointAnnouncementAdder

        'One method that adds an item to the Announcements list
        <OperationContract()> _
        Function AddAnnouncement(ByVal Title As String, ByVal Body As String) As Boolean

    End Interface

End Namespace
