' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' Sample implementation of the System.Security.Principal.IIdentity interface. This class encapsulates
''' the user identity attached to the current thread.  This is just a sample of how the IPrincipal and
''' IIdentity are related; the algorithm is by no means secure.
''' </summary>
''' <remarks></remarks>
Public Class SampleIIdentity : Implements System.Security.Principal.IIdentity

    Private nameValue As String
    Private authenticated As Boolean

    ''' <summary>
    ''' Create a new identity using the name and password provided.  
    ''' </summary>
    ''' <param name="name">name of the user</param>
    ''' <param name="password">password for the user to be used during authentication</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String, ByVal password As String)
        nameValue = name
        If password.Equals("password") Then
            authenticated = True
        Else
            authenticated = False
        End If
    End Sub

    ''' <summary>
    ''' Returns the current authentication mechanism.  Here, we're just comparing the password to "password", but
    ''' a more reasonable AuthenticationType might be "SqlDatabase", "LDAP", etc.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public ReadOnly Property AuthenticationType() As String Implements System.Security.Principal.IIdentity.AuthenticationType
        Get
            Return "An Insecure Sample Authentication Mechanism"
        End Get
    End Property

    ''' <summary>
    ''' Has this user been authenticated?  Returns true if the password provided matches "password", this will return true.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public ReadOnly Property IsAuthenticated() As Boolean Implements System.Security.Principal.IIdentity.IsAuthenticated
        Get
            Return authenticated
        End Get
    End Property

    ''' <summary>
    ''' The name of the the user associated with this identity.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public ReadOnly Property Name() As String Implements System.Security.Principal.IIdentity.Name
        Get
            Return nameValue
        End Get
    End Property
End Class

