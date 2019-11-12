' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Configuration
Imports System.ServiceModel
Imports System.ServiceModel.PeerResolvers

Namespace Microsoft.ServiceModel.Samples
    ' Multi-party chat application using Peer Channel (a multi-party channel)
    ' Chat service contract
    ' Applying [PeerBehavior] attribute on the service contract enables retrieval of PeerNode from IClientChannel.
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples", CallbackContract:=GetType(IChat))> _
    Public Interface IChat

        <OperationContract(IsOneWay:=True)> _
        Sub Join(ByVal member As String)
        <OperationContract(IsOneWay:=True)> _
        Sub Chat(ByVal member As String, ByVal msg As String)
        <OperationContract(IsOneWay:=True)> _
        Sub Leave(ByVal member As String)

    End Interface
    Public Interface IChatChannel
        Inherits IChat
        Inherits IClientChannel

    End Interface

End Namespace
