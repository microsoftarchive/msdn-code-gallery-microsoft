'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Automation
Imports System.Collections.ObjectModel

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        DataBoundList.ItemsSource = DataHelper.GeneratePersonNamesSource()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class

#Region "CustomListView"
Public Class MyList
    Inherits ListView
    Protected Overrides Sub PrepareContainerForItemOverride(element As DependencyObject, item As Object)
        MyBase.PrepareContainerForItemOverride(element, item)
        Dim source As FrameworkElement = TryCast(element, FrameworkElement)
        source.SetBinding(AutomationProperties.AutomationIdProperty, New Binding With {.Path = New PropertyPath("AutomationId")})
        source.SetBinding(AutomationProperties.NameProperty, New Binding With {.Path = New PropertyPath("AutomationName")})
    End Sub
End Class
#End Region

#Region "PersonClass example code"
Public Class Person
    Private m_firstName As String
    Private m_lastName As String
    Private m_automationName As String
    Private _age As Integer
    Private m_automationId As String

    Public Sub New()
    End Sub

    Public Sub New(firstName__1 As String, lastName__2 As String, id As String, age__3 As Integer, name As String)
        FirstName = firstName__1
        LastName = lastName__2
        AutomationId = id
        Age = age__3
        AutomationName = name
    End Sub

    Public Property FirstName() As String
        Get
            Return m_firstName
        End Get
        Set(value As String)
            m_firstName = value
        End Set
    End Property

    Public Property LastName() As String
        Get
            Return m_lastName
        End Get
        Set(value As String)
            m_lastName = value
        End Set
    End Property

    Public Property Age() As Integer
        Get
            Return _age
        End Get
        Set(value As Integer)
            _age = value
        End Set
    End Property

    Public Property AutomationId() As String
        Get
            Return m_automationId
        End Get
        Set(value As String)
            m_automationId = value
        End Set
    End Property
    Public Property AutomationName() As String
        Get
            Return m_automationName
        End Get
        Set(value As String)
            m_automationName = value
        End Set
    End Property
End Class
#End Region

#Region "DataHelperClass"
Public Class DataHelper
    Public Shared Function GeneratePersonNamesSource() As ObservableCollection(Of Person)
        Dim ds = New ObservableCollection(Of Person)
        ds.Add(New Person("George", "Washington", "ListItemId1", 25, "ListItemName1"))
        ds.Add(New Person("John", "Adams", "ListItemId2", 30, "ListItemName2"))
        ds.Add(New Person("Thomas", "Jefferson", "ListItemId3", 45, "ListItemName3"))
        ds.Add(New Person("James", "Madison", "ListItemId4", 55, "ListItemName4"))
        ds.Add(New Person("James", "Monroe", "ListItemId5", 30, "ListItemName5"))
        ds.Add(New Person("John", "Adams", "ListItemId6", 25, "ListItemName6"))
        ds.Add(New Person("Andrew", "Jackson", "ListItemId7", 55, "ListItemName7"))
        ds.Add(New Person("Martin", "Van Buren", "ListItemId8", 56, "ListItemName8"))
        ds.Add(New Person("William", "Harrison", "ListItemId9", 40, "ListItemName9"))
        ds.Add(New Person("John", "Tyler", "ListItemId10", 42, "ListItemName10"))
        ds.Add(New Person("James", "Polk", "ListItemId11", 60, "ListItemName11"))
        ds.Add(New Person("Zachary", "Taylor", "ListItemId12", 65, "ListItemName12"))
        ds.Add(New Person("Millard", "Fillmore", "ListItemId13", 25, "ListItemName13"))
        ds.Add(New Person("Franklin", "Pierce", "ListItemId14", 35, "ListItemName14"))
        ds.Add(New Person("James", "Buchanan", "ListItemId15", 43, "ListItemName15"))
        ds.Add(New Person("Abraham", "Lincoln", "ListItemId16", 23, "ListItemName16"))
        ds.Add(New Person("Andrew", "Johnson", "ListItemId17", 21, "ListItemName17"))
        ds.Add(New Person("Rutherford", "Hayes", "ListItemId18", 25, "ListItemName18"))
        ds.Add(New Person("James", "Garfield", "ListItemId19", 30, "ListItemName19"))
        ds.Add(New Person("Chester", "Arthur", "ListItemId20", 34, "ListItemName20"))
        ds.Add(New Person("Grover", "Cleveland", "ListItemId21", 55, "ListItemName21"))
        Return ds
    End Function
End Class
#End Region
