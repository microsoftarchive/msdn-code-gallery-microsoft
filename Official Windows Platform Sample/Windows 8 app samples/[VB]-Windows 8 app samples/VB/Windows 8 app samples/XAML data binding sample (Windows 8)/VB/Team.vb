Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DataBinding
Imports Windows.UI
Imports Windows.UI.Xaml.Media

#Region "Scenario4/5/6 DataModel"
Public Class Team
    'Has a custom string indexer
    Private _propBag As Dictionary(Of String, Object)
    Public Sub New()
        _propBag = New Dictionary(Of String, Object)()
    End Sub

    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
    Public Property City() As String
        Get
            Return m_City
        End Get
        Set(value As String)
            m_City = value
        End Set
    End Property
    Private m_City As String
    Public Property Color() As SolidColorBrush
        Get
            Return m_Color
        End Get
        Set(value As SolidColorBrush)
            m_Color = value
        End Set
    End Property
    Private m_Color As SolidColorBrush
    Default Public Property Item(indexer As String) As Object
        Get
            Return _propBag(indexer)
        End Get
        Set(value As Object)
            _propBag(indexer) = value
        End Set
    End Property

    Public Sub Insert(key As String, value As Object)
        _propBag.Add(key, value)
    End Sub

End Class

<System.Runtime.InteropServices.ComVisible(False)>
Public Class Teams
    Inherits List(Of Team)
    Public Sub New()
        Add(New Team() With { _
            .Name = "The Reds", _
            .City = "Liverpool", _
            .Color = New SolidColorBrush(Colors.Green) _
        })
        Add(New Team() With { _
            .Name = "The Red Devils", _
            .City = "Manchester", _
            .Color = New SolidColorBrush(Colors.Yellow) _
        })
        Add(New Team() With { _
            .Name = "The Blues", _
            .City = "London", _
            .Color = New SolidColorBrush(Colors.Orange) _
        })
        Dim _t As New Team() With { _
            .Name = "The Gunners", _
            .City = "London", _
            .Color = New SolidColorBrush(Colors.Red) _
        }
        _t("Gaffer") = "le Professeur"
        _t("Skipper") = "Mr Gooner"

        Add(_t)
    End Sub

End Class


#End Region
