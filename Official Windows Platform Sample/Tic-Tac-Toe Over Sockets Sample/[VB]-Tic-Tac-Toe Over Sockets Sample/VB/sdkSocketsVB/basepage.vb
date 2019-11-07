'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Public Class basepage
    Inherits PhoneApplicationPage
    Private _isNewPageInstance As Boolean = False

    Public Sub New()
        _isNewPageInstance = True

    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As System.Windows.Navigation.NavigationEventArgs)
        ' If _isNewPageInstance is true, the page constuctor has been called, so
        ' state may need to be restored
        If _isNewPageInstance Then

            AddHandler TryCast(Application.Current, App).ApplicationDataObjectChanged, AddressOf basepage_ApplicationDataObjectChanged

            ' if the application member variable is not empty,
            ' set the page's data object from the application member variable.
            If (TryCast(Application.Current, App)).HostName IsNot Nothing Then
                _updateDependencyProperties()
            Else
                TryCast(Application.Current, App).GetDataAsync()

            End If
        End If

        ' Set _isNewPageInstance to false. If the user navigates back to this page
        ' and it has remained in memory, this value will continue to be false.
        _isNewPageInstance = False
    End Sub

    Private Sub basepage_ApplicationDataObjectChanged(ByVal sender As Object, ByVal e As EventArgs)
        ' Update the settings DependencyProperties.
        ' Note: this approach is brute-force since only one event is 
        ' used to signal a change in the settings, so all settings have to be
        ' updated. An alternative would be to have a changed event per setting
        ' and only update those settings that have truely changed.
        If System.Windows.Deployment.Current.Dispatcher.CheckAccess() Then
            _updateDependencyProperties()
        Else
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(Sub() _updateDependencyProperties())
        End If

    End Sub

    Private Sub _updateDependencyProperties()
        ServerName = (TryCast(Application.Current, App)).HostName
        PortNumber = (TryCast(Application.Current, App)).PortNumber
        PlayAsX = (TryCast(Application.Current, App)).PlayAsX
        PlayAsO = Not (TryCast(Application.Current, App)).PlayAsX
    End Sub

    Public Shared ReadOnly ServerNameProperty As DependencyProperty = DependencyProperty.RegisterAttached("ServerName", GetType(String), GetType(String), New PropertyMetadata(String.Empty))
    Public Property ServerName() As String
        Get
            Return CStr(GetValue(ServerNameProperty))
        End Get
        Set(ByVal value As String)
            SetValue(ServerNameProperty, value)
        End Set
    End Property

    Public Shared ReadOnly PortNumberProperty As DependencyProperty = DependencyProperty.RegisterAttached("PortNumber", GetType(Integer), GetType(Integer), New PropertyMetadata(0))
    Public Property PortNumber() As Integer
        Get
            Return CInt(Fix(GetValue(PortNumberProperty)))
        End Get
        Set(ByVal value As Integer)
            SetValue(PortNumberProperty, value)
        End Set
    End Property

    Public Shared ReadOnly PlayAsXProperty As DependencyProperty = DependencyProperty.RegisterAttached("PlayAsX", GetType(Boolean), GetType(Boolean), New PropertyMetadata(True))
    Public Property PlayAsX() As Boolean
        Get
            Return CBool(GetValue(PlayAsXProperty))
        End Get
        Set(ByVal value As Boolean)
            SetValue(PlayAsXProperty, value)
        End Set
    End Property


    ' This property is not actually stored in the application. It is faked here using the PlayAsXproperty
    ' The reason for this that this always the opposite of the PlayAsX value, so there is no need to store it
    Public Shared ReadOnly PlayAsOProperty As DependencyProperty = DependencyProperty.RegisterAttached("PlayAsO", GetType(Boolean), GetType(Boolean), New PropertyMetadata(True))
    Public Property PlayAsO() As Boolean
        Get
            Return Not CBool(GetValue(PlayAsXProperty))
        End Get
        Set(ByVal value As Boolean)
            SetValue(PlayAsOProperty, value)
        End Set
    End Property

End Class
