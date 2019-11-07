' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' Form coordinates have the top, left as (0,0). For the game grid,
''' it is easier to have the bottom left of the grid as (0,0). This
''' translates the points.
''' </summary>
''' <remarks></remarks>
Public Class PointTranslator

    Private Shared graphicsValue As Graphics
    Private Shared height As Integer

    Public Shared Property Graphics() As Graphics
        Get
            Return graphicsValue
        End Get
        Set(ByVal Value As Graphics)
            graphicsValue = Value
            height = CInt(graphicsValue.VisibleClipBounds.Height())
        End Set
    End Property

    ' Translates an (X,Y) point from the top left to an (X, Y) point from the bottom left.
    Public Shared Function TranslateToBL(ByVal topleft As Point) As Point
        Dim newPoint As Point
        newPoint.X = topleft.X
        newPoint.Y = height - topleft.Y
        Return newPoint
    End Function

    Public Shared Function TranslateToTL(ByVal bottomleft As Point) As Point
        Dim newPoint As Point
        newPoint.X = bottomleft.X
        newPoint.Y = height - bottomleft.Y
        Return newPoint
    End Function
End Class
