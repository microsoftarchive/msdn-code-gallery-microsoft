' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' The encapsulates the authorization aspect of custom security.
''' </summary>
''' <remarks></remarks>
Public Class SampleIPrincipal : Implements System.Security.Principal.IPrincipal

    ''' <summary>
    ''' The identity associated with this principal
    ''' </summary>
    ''' <remarks></remarks>
    Private identityValue As System.Security.Principal.IIdentity

    ''' <summary>
    ''' Create a new SamplePrincipal given a username and password
    ''' </summary>
    ''' <param name="name">username to be provided to the SampleIdentity</param>
    ''' <param name="password">password to be provided to the SampleIdentity</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String, ByVal password As String)
        identityValue = New SampleIIdentity(name, password)
    End Sub

    ''' <summary>
    ''' Return the identity of this user
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public ReadOnly Property Identity() As System.Security.Principal.IIdentity Implements System.Security.Principal.IPrincipal.Identity
        Get
            Return identityValue
        End Get
    End Property

    ''' <summary>
    ''' Determines what roles the current user is in.
    ''' </summary>
    ''' <param name="role">Passing "DemoUser" will return true; otherwise false is returned.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsInRole(ByVal role As String) As Boolean Implements System.Security.Principal.IPrincipal.IsInRole
        If (role.Equals("DemoUser")) Then
            Return True
        Else
            Return False
        End If
    End Function
End Class



