' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Drawing.Drawing2D

Public Class BeadedScoreBoard
    Inherits System.Windows.Forms.UserControl

#Region "Variables and Enumerations"

    Private totalBeads As Integer = 10
    Private totalScore As Integer = 0
    Private beadDiameter As Integer = CInt(Me.Width / (totalBeads + 2))
    Private privateBeadColor As System.Drawing.Color = Color.Black
    Private outlineColor As System.Drawing.Color = Color.Black
    Private isClickable As Boolean = True
    Private hasOutline As Boolean = True

#End Region

#Region "Control Properties"
    ' The color property of the beads.
    Public Property BeadColor() As System.Drawing.Color
        Get
            Return privateBeadColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            privateBeadColor = value
            Me.Invalidate()
        End Set
    End Property

    Public Property BeadOutlineColor() As System.Drawing.Color
        Get
            Return outlineColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            outlineColor = value
            Me.Invalidate()
        End Set
    End Property

    Public Property Clickable() As Boolean
        Get
            Return isClickable
        End Get
        Set(ByVal value As Boolean)
            isClickable = value
            Me.Invalidate()
        End Set
    End Property

    Public Property Outline() As Boolean
        Get
            Return hasOutline
        End Get
        Set(ByVal value As Boolean)
            hasOutline = value
            Me.Invalidate()
        End Set
    End Property

    ' The number of beads on the control.
    Public Property BeadCount() As Integer
        Get
            Return totalBeads
        End Get
        Set(ByVal value As Integer)
            If value > 0 Then
                totalBeads = value
            Else
                totalBeads = 10
            End If
            beadDiameter = CInt(Me.Width / (totalBeads + 2))
            Me.Invalidate()
        End Set
    End Property

    ' The score displayed by the control.
    Public Property Score() As Integer
        Get
            Return totalScore
        End Get
        Set(ByVal value As Integer)
            If value >= 0 Then
                If value < totalBeads Then
                    totalScore = value
                Else
                    totalScore = totalBeads
                End If
            Else
                totalScore = 0
            End If
            Me.Invalidate()
        End Set
    End Property

#End Region

#Region "Drawing Functions"
    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)

        Dim rect As System.Drawing.Rectangle = e.ClipRectangle
        Dim g As Graphics = e.Graphics
        Dim mainPen As New Pen(Color.Black)

        ' Adjust the bead size depending on the chang in size of the control.
        beadDiameter = CInt(Me.Width / (totalBeads + 2))

        ' Draw Main Lines.
        mainPen.Width = CSng(System.Math.Ceiling(rect.Height / 100))
        g.DrawLine(mainPen, 0, CInt(rect.Height / 2), rect.Width, CInt(rect.Height / 2))
        mainPen.Width = 1
        g.DrawLine(mainPen, 0, CInt(rect.Height / 2 - beadDiameter / 2), 0, CInt(rect.Height / 2 + beadDiameter / 2))
        g.DrawLine(mainPen, rect.Width - 1, CInt(rect.Height / 2 - beadDiameter / 2), rect.Width - 1, CInt(rect.Height / 2 + beadDiameter / 2))

        ' Draw Left Beads.
        Dim i As Integer
        For i = 0 To totalScore - 1
            DrawBead(g, rect, CInt(i * beadDiameter + beadDiameter / 2))
        Next

        ' Draw Right Beads.
        For i = 0 To totalBeads - totalScore - 1
            DrawBead(g, rect, CInt(rect.Width - i * beadDiameter - beadDiameter / 2 - 1))
        Next

    End Sub

    ' Draws a bead on the line with x as the center.
    Private Sub DrawBead(ByVal g As Graphics, ByVal rect As System.Drawing.Rectangle, ByVal x As Integer)
        g.FillEllipse(New System.Drawing.Pen(privateBeadColor).Brush, x - CInt(beadDiameter / 2), CInt(rect.Height / 2 - beadDiameter / 2), beadDiameter, beadDiameter)
        If hasOutline Then
            g.DrawEllipse(New Pen(outlineColor), x - CInt(beadDiameter / 2), CInt(rect.Height / 2 - beadDiameter / 2), beadDiameter, beadDiameter)
        End If

    End Sub

#End Region

#Region "Event Handlers"

    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        If isClickable Then
            If e.Button = Windows.Forms.MouseButtons.Left Then
                Score += 1
            Else
                Score -= 1
            End If
        End If
    End Sub

#End Region

End Class
