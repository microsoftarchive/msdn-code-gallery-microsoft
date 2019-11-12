Imports VBUWPOCR

Partial Public Class MainPage
    Inherits Page

    Public Const FEATURE_NAME As String = "OCR VB sample"
    Private _scenarios As New List(Of Scenario)(New Scenario() {New Scenario("OCR image file", GetType(OcrFileImage)), New Scenario("OCR captured image", GetType(OcrCapturedImage))})

    Public ReadOnly Property Scenarios() As List(Of Scenario)
        Get
            Return _scenarios
        End Get
    End Property
End Class

Public Class Scenario

    Private _title As String
    Public Property Title() As String
        Get
            Return _title
        End Get
        Set(value As String)
            _title = value
        End Set
    End Property

    Private _classType As Type

    Public Property ClassType() As Type
        Get
            Return _classType
        End Get
        Set(value As Type)
            _classType = value
        End Set
    End Property

    Public Sub New(_myTitle As String, _myClassType As Type)
        _title = _myTitle
        _classType = _myClassType
    End Sub
End Class