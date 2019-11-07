' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Drawing.Drawing2D

''' <summary>
''' This class represents one of the balls in the game grid.
''' </summary>
''' <remarks></remarks>
Public Class Block
    Public Const BlockSize As Integer = 25
    Private colorValue As Color
    Private deletionValue As Boolean = False
    Private Shared rand As New Random

    Public Property Color() As Color
        Get
            Return colorValue
        End Get
        Set(ByVal Value As Color)
            colorValue = Value
        End Set
    End Property

    Public Property MarkedForDeletion() As Boolean
        Get
            Return deletionValue
        End Get
        Set(ByVal Value As Boolean)
            deletionValue = Value
        End Set
    End Property

    Public Sub New(ByVal newColor As Color)
        colorValue = newColor
    End Sub

    Public Sub New(ByVal colors() As Color)
        Dim ncolors As Integer = colors.Length
        Dim pickedColor As Integer
        pickedColor = rand.Next(0, ncolors)
        colorValue = colors(pickedColor)
    End Sub

    Public Sub Draw(ByVal graphics As Graphics, ByVal point As Point)
        Dim brush As System.Drawing.Drawing2D.LinearGradientBrush = CreateTheBrush(point)
        DrawTheCircle(graphics, brush, point)
    End Sub

    Private Sub DrawTheCircle(ByVal graphics As Graphics, ByVal brush As LinearGradientBrush, ByVal location As Point)
        Dim topleft As Point = location
        Dim bottomright As Point = New Point(location.X + BlockSize, location.Y + BlockSize)
        Dim transTopLeft As Point = PointTranslator.TranslateToBL(topleft)
        Dim transBottomRight As Point = PointTranslator.TranslateToBL(bottomright)
        Dim transwidth As Integer = transBottomRight.X - transTopLeft.X
        Dim transheight As Integer = transBottomRight.Y - transTopLeft.Y
        graphics.FillEllipse(brush, New Rectangle(transTopLeft, New Size(transwidth, transheight)))
    End Sub

    Private Function CreateTheBrush(ByVal location As Point) As LinearGradientBrush
        Dim transLocation As Point = PointTranslator.TranslateToBL(location)
        Dim brushpt1 As Point = transLocation
        Dim brushpt2 As New Point(transLocation.X + Block.BlockSize + 4, transLocation.Y - BlockSize - 4)
        Dim brush As New LinearGradientBrush(brushpt1, brushpt2, Me.Color, System.Drawing.Color.White)
        Return brush
    End Function
End Class


