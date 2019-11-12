' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Drawing.Drawing2D

Public Class DigitalScoreBoard

#Region "Variables and Enumerations"

    Private numberCount As Integer = 10
    Private hasOutline As Boolean = True
    Private outlineColorMember As System.Drawing.Color = Color.Black
    Private fillColor As System.Drawing.Color = Color.Black
    Private totalScore As Long = 0
    Private showLeadingZeros As Boolean = True
    Private numberSpacing As SpaceSize = SpaceSize.Medium

    Private numberWidth As Double
    Private blockSize As Double
    Private e As PaintEventArgs = Nothing

    ' Enumeration for each setting of the number spacing.
    Public Enum SpaceSize As Integer
        Small = 16
        Medium = 18
        Large = 20
    End Enum

#End Region

#Region "Control Properties"
    Public Property Outline() As Boolean
        Get
            Return hasOutline
        End Get
        Set(ByVal value As Boolean)
            hasOutline = value
            Me.Invalidate()
        End Set
    End Property

    Public Property Spacing() As SpaceSize
        Get
            Return numberSpacing
        End Get
        Set(ByVal value As SpaceSize)
            numberSpacing = value
            Me.Invalidate()
        End Set
    End Property

    Public Property Digits() As Integer
        Get
            Return numberCount
        End Get
        Set(ByVal value As Integer)
            If value > 17 Then
                numberCount = 17
            ElseIf value < 1 Then
                numberCount = 1
            Else
                numberCount = value
            End If
            Me.Invalidate()
        End Set
    End Property

    Public Property OutlineColor() As System.Drawing.Color
        Get
            Return outlineColorMember
        End Get
        Set(ByVal value As System.Drawing.Color)
            outlineColorMember = value
            Me.Invalidate()
        End Set
    End Property

    Public Property NumberColor() As System.Drawing.Color
        Get
            Return fillColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            fillColor = value
            Me.Invalidate()
        End Set
    End Property

    Public Property Score() As Long
        Get
            Return totalScore
        End Get
        Set(ByVal value As Long)
            If Math.Log10(value) + 1 > 17 Then
                totalScore = 99999999999999999
            Else
                totalScore = value
            End If
            Me.Invalidate()
        End Set
    End Property

    Public Property LeadingZeros() As Boolean
        Get
            Return showLeadingZeros
        End Get
        Set(ByVal value As Boolean)
            showLeadingZeros = value
            Me.Invalidate()
        End Set
    End Property
#End Region

#Region "Drawing Functions"

    ' Draws the score on the scoreboard.
    Private Sub DisplayScore()

        Dim tempScore As Long
        Dim position As Integer = numberCount - 1
        If Math.Floor(Math.Log10(totalScore) + 1) > numberCount Then
            tempScore = CLng(10 ^ numberCount - 1)
            totalScore = tempScore
        ElseIf totalScore < 0 Then
            tempScore = 0
            totalScore = 0
        Else
            tempScore = totalScore
        End If

        If tempScore = 0 Then
            DrawNumber(0, position)
            position -= 1
        Else
            While tempScore > 0
                DrawNumber(CInt(tempScore Mod 10), position)
                tempScore = CLng(Math.Floor(tempScore / 10))
                position -= 1
            End While
        End If

        If showLeadingZeros Then
            While position >= 0
                DrawNumber(0, position)
                position -= 1
            End While
        End If

    End Sub

    ' Defines what to paint when the control need to be repainted.
    Protected Overrides Sub OnPaint(ByVal ea As System.Windows.Forms.PaintEventArgs)

        numberWidth = Me.Width / numberCount
        blockSize = numberWidth / numberSpacing

        If Me.Height / 24 < blockSize Then
            blockSize = Me.Height / 24
        End If

        e = ea
        DisplayScore()

    End Sub

    ' Draws a number in a specific position on the scoreboard.
    Private Sub DrawNumber(ByVal number As Integer, ByVal position As Integer)

        Select Case number
            Case 0
                DrawZero(position)
                Exit Select
            Case 1
                DrawOne(position)
                Exit Select
            Case 2
                DrawTwo(position)
                Exit Select
            Case 3
                DrawThree(position)
                Exit Select
            Case 4
                DrawFour(position)
                Exit Select
            Case 5
                DrawFive(position)
                Exit Select
            Case 6
                DrawSix(position)
                Exit Select
            Case 7
                DrawSeven(position)
                Exit Select
            Case 8
                DrawEight(position)
                Exit Select
            Case 9
                DrawNine(position)
                Exit Select
            Case Else
                DrawZero(position)
        End Select

    End Sub

    ' Draws a zero on the scoreboard in the specified position.
    Private Sub DrawZero(ByVal position As Integer)
        DrawVerticalLine(1 + numberSpacing * position, 2, -1, 0)
        DrawVerticalLine(1 + numberSpacing * position, 12, -1, 2)
        DrawHorizontalLine(2 + numberSpacing * position, 1, 0, -1)
        DrawHorizontalLine(2 + numberSpacing * position, 21, 0, 3)
        DrawVerticalLine(11 + numberSpacing * position, 2, 1, 0)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    ' Draws a one on the scoreboard in the specified position.
    Private Sub DrawOne(ByVal position As Integer)
        DrawVerticalLine(11 + numberSpacing * position, 2, 1, 0)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    ' Draws a two on the scoreboard in the specified position.
    Private Sub DrawTwo(ByVal position As Integer)

        DrawVerticalLine(1 + numberSpacing * position, 12, -1, 2)
        DrawHorizontalLine(2 + numberSpacing * position, 1, 0, -1)
        DrawHorizontalLine(2 + numberSpacing * position, 11, 0, 1)
        DrawHorizontalLine(2 + numberSpacing * position, 21, 0, 3)
        DrawVerticalLine(11 + numberSpacing * position, 2, 1, 0)
    End Sub

    ' Draws a three on the scoreboard in the specified position.
    Private Sub DrawThree(ByVal position As Integer)
        DrawHorizontalLine(2 + numberSpacing * position, 1, 0, -1)
        DrawHorizontalLine(2 + numberSpacing * position, 11, 0, 1)
        DrawHorizontalLine(2 + numberSpacing * position, 21, 0, 3)
        DrawVerticalLine(11 + numberSpacing * position, 2, 1, 0)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    ' Draws a four on the scoreboard in the specified position.
    Private Sub DrawFour(ByVal position As Integer)
        DrawVerticalLine(1 + numberSpacing * position, 2, -1, 0)
        DrawHorizontalLine(2 + numberSpacing * position, 11, 0, 1)
        DrawVerticalLine(11 + numberSpacing * position, 2, 1, 0)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    ' Draws a five on the scoreboard in the specified position.
    Private Sub DrawFive(ByVal position As Integer)
        DrawVerticalLine(1 + numberSpacing * position, 2, -1, 0)
        DrawHorizontalLine(2 + numberSpacing * position, 1, 0, -1)
        DrawHorizontalLine(2 + numberSpacing * position, 11, 0, 1)
        DrawHorizontalLine(2 + numberSpacing * position, 21, 0, 3)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    ' Draws a six on the scoreboard in the specified position.
    Private Sub DrawSix(ByVal position As Integer)
        DrawVerticalLine(1 + numberSpacing * position, 2, -1, 0)
        DrawVerticalLine(1 + numberSpacing * position, 12, -1, 2)
        DrawHorizontalLine(2 + numberSpacing * position, 1, 0, -1)
        DrawHorizontalLine(2 + numberSpacing * position, 11, 0, 1)
        DrawHorizontalLine(2 + numberSpacing * position, 21, 0, 3)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    ' Draws a seven on the scoreboard in the specified position.
    Private Sub DrawSeven(ByVal position As Integer)
        DrawHorizontalLine(2 + numberSpacing * position, 1, 0, -1)
        DrawVerticalLine(11 + numberSpacing * position, 2, 1, 0)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    ' Draws a eight on the scoreboard in the specified position.
    Private Sub DrawEight(ByVal position As Integer)
        DrawVerticalLine(1 + numberSpacing * position, 2, -1, 0)
        DrawVerticalLine(1 + numberSpacing * position, 12, -1, 2)
        DrawHorizontalLine(2 + numberSpacing * position, 1, 0, -1)
        DrawHorizontalLine(2 + numberSpacing * position, 11, 0, 1)
        DrawHorizontalLine(2 + numberSpacing * position, 21, 0, 3)
        DrawVerticalLine(11 + numberSpacing * position, 2, 1, 0)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    ' Draws a nine on the scoreboard in the specified position.
    Private Sub DrawNine(ByVal position As Integer)
        DrawVerticalLine(1 + numberSpacing * position, 2, -1, 0)
        DrawHorizontalLine(2 + numberSpacing * position, 1, 0, -1)
        DrawHorizontalLine(2 + numberSpacing * position, 11, 0, 1)
        DrawHorizontalLine(2 + numberSpacing * position, 21, 0, 3)
        DrawVerticalLine(11 + numberSpacing * position, 2, 1, 0)
        DrawVerticalLine(11 + numberSpacing * position, 12, 1, 2)
    End Sub

    Private Sub DrawVerticalLine(ByVal x As Integer, ByVal y As Integer, ByVal deltaX As Integer, ByVal deltaY As Integer)

        Dim rect As System.Drawing.Rectangle = e.ClipRectangle
        Dim g As System.Drawing.Graphics = e.Graphics

        Dim path As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath()

        Dim points() As Point = {New Point(CInt(blockSize * (0 + x) + deltaX), CInt(blockSize * (1 + y) + deltaY)), New Point(CInt(blockSize * (1 + x) + deltaX), CInt(blockSize * (0 + y) + deltaY)), New Point(CInt(blockSize * (2 + x) + deltaX), CInt(blockSize * (1 + y) + deltaY)), New Point(CInt(blockSize * (2 + x) + deltaX), CInt(blockSize * (9 + y) + deltaY)), New Point(CInt(blockSize * (1 + x) + deltaX), CInt(blockSize * (10 + y) + deltaY)), New Point(CInt(blockSize * (0 + x) + deltaX), CInt(blockSize * (9 + y) + deltaY))}

        path.AddPolygon(points)

        g.FillPath(New Pen(fillColor).Brush, path)
        If hasOutline Then
            g.DrawPath(New Pen(OutlineColor), path)
        End If

    End Sub

    Private Sub DrawHorizontalLine(ByVal x As Integer, ByVal y As Integer, ByVal deltaX As Integer, ByVal deltaY As Integer)

        Dim rect As System.Drawing.Rectangle = e.ClipRectangle
        Dim g As System.Drawing.Graphics = e.Graphics

        Dim path As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath()

        Dim points() As Point = {New Point(CInt(blockSize * (0 + x) + deltaX), CInt(blockSize * (1 + y) + deltaY)), New Point(CInt(blockSize * (1 + x) + deltaX), CInt(blockSize * (0 + y) + deltaY)), New Point(CInt(blockSize * (9 + x) + deltaX), CInt(blockSize * (0 + y) + deltaY)), New Point(CInt(blockSize * (10 + x) + deltaX), CInt(blockSize * (1 + y) + deltaY)), New Point(CInt(blockSize * (9 + x) + deltaX), CInt(blockSize * (2 + y) + deltaY)), New Point(CInt(blockSize * (1 + x) + deltaX), CInt(blockSize * (2 + y) + deltaY))}

        path.AddPolygon(points)

        g.FillPath(New Pen(fillColor).Brush, path)

        If hasOutline Then
            g.DrawPath(New Pen(OutlineColor), path)
        End If

    End Sub

#End Region

End Class
