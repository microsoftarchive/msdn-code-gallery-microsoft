Imports Windows.Media.Ocr

Public Class WordOverlay
    Implements INotifyPropertyChanged

    Private Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    ''' <summary>
    ''' Left and Right properties of Thickess define word box position.
    ''' </summary>
    Private _word As OcrWord

    ''' <summary>
    ''' Scaled word box width.
    ''' </summary>
    Private _wordPosition As Thickness
    Public Property WordPosition() As Thickness
        Get
            Return _wordPosition
        End Get
        Private Set(value As Thickness)
            _wordPosition = value
        End Set
    End Property

    ''' <summary>
    ''' Scaled word box width.
    ''' </summary>
    Private _wordWidth As Double
    Public Property WordWidth() As Double
        Get
            Return _wordWidth
        End Get
        Private Set(value As Double)
            _wordWidth = value
        End Set
    End Property

    ''' <summary>
    ''' Scaled word box height.
    ''' </summary>
    Private _wordHeight As Double
    Public Property WordHeight() As Double
        Get
            Return _wordHeight
        End Get
        Private Set(value As Double)
            _wordHeight = value
        End Set
    End Property

    Public Sub New(ocrWord As OcrWord)
        _word = ocrWord
        UpdateProps(_word.BoundingRect)
    End Sub

    Public Sub Transform(scale As ScaleTransform)
        UpdateProps(scale.TransformBounds(_word.BoundingRect))
    End Sub

    Public Function CreateWordPositionBinding() As Binding
        Dim positionBinding As New Binding()
        positionBinding.Source = Me
        positionBinding.Path = New PropertyPath("WordPosition")
        Return positionBinding
    End Function

    Public Function CreateWordWidthBinding() As Binding
        Dim widthBinding As New Binding()
        widthBinding.Source = Me
        widthBinding.Path = New PropertyPath("WordWidth")
        Return widthBinding
    End Function

    Public Function CreateWordHeightBinding() As Binding
        Dim heightBinding As New Binding()
        heightBinding.Source = Me
        heightBinding.Path = New PropertyPath("WordHeight")
        Return heightBinding
    End Function

    Private Sub UpdateProps(wordBoundingBox As Rect)
        WordPosition = New Thickness(wordBoundingBox.Left, wordBoundingBox.Top, 0, 0)
        WordWidth = wordBoundingBox.Width
        WordHeight = wordBoundingBox.Height
        NotifyPropertyChanged("WordPosition")
        NotifyPropertyChanged("WordHeight")
        NotifyPropertyChanged("WordWidth")
    End Sub
End Class
