' Copyright (c) Microsoft Corporation. All rights reserved.
<Serializable()> _
Public Class Pixel
    Private XLocation As Integer
    Private YLocation As Integer
    Private PixelColor As Color

    Public Property X() As Integer
        Get
            Return XLocation
        End Get
        Set(ByVal value As Integer)
            XLocation = value
        End Set
    End Property

    Public Property Y() As Integer
        Get
            Return YLocation
        End Get
        Set(ByVal value As Integer)
            YLocation = value
        End Set
    End Property

    Public Property Color() As Color
        Get
            Return PixelColor
        End Get
        Set(ByVal value As Color)
            PixelColor = value
        End Set
    End Property

    Public Sub New()
        X = 0
        Y = 0
        Color = System.Drawing.Color.Black
    End Sub
End Class
