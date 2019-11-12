Imports Microsoft.VisualBasic.PowerPacks
Imports System.Drawing.Drawing2D

' This application demonstrates the use of Line and Shape controls to draw a picture.
Public Class PicForm

    Private i As Integer = 4
    Private w As Integer = 0
    Private river As Region
    Private wavec As Color = Color.RoyalBlue
    Private r As New Random

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        ' This procedure creates the smoke from the chimney in response to a timer.
        i = i + 1
        If i > 7 Then
            i = 4
            Me.OvalShape5.Visible = False
            Me.OvalShape6.Visible = False
            Me.OvalShape7.Visible = False
        End If

        If i = 5 Then
            Me.OvalShape5.Visible = True
        End If
        If i = 6 Then
            Me.OvalShape6.Visible = True
        End If
        If i = 7 Then
            Me.OvalShape7.Visible = True
        End If

    End Sub

    Private Sub ShapeContainer1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles ShapeContainer1.Paint
        ' These procedures create the irregular shaped objects using graphics paths.
        ' Creates the roof of the house.
        Using gp As GraphicsPath = New GraphicsPath()
            gp.AddPolygon(New Point() {Me.LineShape17.StartPoint, Me.LineShape17.EndPoint, Me.LineShape18.StartPoint})
            Dim brush As Brush = New HatchBrush(HatchStyle.Shingle, Color.Black, Color.Maroon)

            e.Graphics.FillPath(brush, gp)
        End Using
        ' Creates the chimney.
        Using gp As GraphicsPath = New GraphicsPath()
            gp.AddPolygon(New Point() {Me.LineShape20.StartPoint, Me.LineShape20.EndPoint, Me.LineShape22.EndPoint, Me.LineShape22.StartPoint})
            Dim brush As Brush = New HatchBrush(HatchStyle.HorizontalBrick, Color.BurlyWood, Color.Peru)
            e.Graphics.FillPath(brush, gp)
        End Using
        ' Creates the sidewalk.
        Using gp As GraphicsPath = New GraphicsPath()
            gp.AddPolygon(New Point() {Me.LineShape26.StartPoint, Me.LineShape26.EndPoint, Me.LineShape28.EndPoint, Me.LineShape30.EndPoint, Me.LineShape32.EndPoint, Me.LineShape33.EndPoint, Me.LineShape33.StartPoint, Me.LineShape31.StartPoint, Me.LineShape29.StartPoint, Me.LineShape27.StartPoint})
            Dim brush As Brush = New HatchBrush(HatchStyle.LargeConfetti, Color.Gray, Color.SaddleBrown)
            e.Graphics.FillPath(brush, gp)
        End Using
        ' Creates the mountains.
        Using gp As GraphicsPath = New GraphicsPath()
            gp.AddPolygon(New Point() {Me.LineShape4.StartPoint, Me.LineShape4.EndPoint, Me.LineShape3.EndPoint, Me.LineShape1.EndPoint, Me.LineShape2.EndPoint, Me.LineShape1.StartPoint})

            Dim rec As RectangleF = New RectangleF(207, 113, 471, 428)
            Dim brush As Brush = New LinearGradientBrush(rec, Color.ForestGreen, Color.YellowGreen, LinearGradientMode.Vertical)
            e.Graphics.FillPath(brush, gp)
        End Using
        ' Creates the river.
        Using gp As GraphicsPath = New GraphicsPath()
            gp.AddPolygon(New Point() {Me.LineShape5.StartPoint, Me.LineShape5.EndPoint, Me.LineShape7.EndPoint, Me.LineShape9.EndPoint, Me.LineShape11.EndPoint, Me.LineShape16.EndPoint, Me.LineShape16.StartPoint, Me.LineShape15.StartPoint, Me.LineShape14.StartPoint, Me.LineShape13.StartPoint})
            If river Is Nothing Then
                river = New Region(gp)
            End If
            Dim brush As Brush = New HatchBrush(HatchStyle.Wave, wavec, Color.CornflowerBlue)
            e.Graphics.FillPath(brush, gp)
        End Using

    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        ' This procedure alternates the color of the river.
        If wavec = Color.RoyalBlue Then
            wavec = Color.White
        Else
            wavec = Color.RoyalBlue
        End If
        ' Call Invalidate to force the controls to paint.
        Me.ShapeContainer1.Invalidate(river)

    End Sub

   

    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        ' This procedure moves the clouds in response to a timer.
        Dim s1 As Integer
        Dim s2 As Integer
        Dim s3 As Integer

        s1 = r.Next(-10, 10)
        s2 = r.Next(-10, 10)
        s3 = r.Next(-10, 10)

        Dim pt As Point = Me.OvalShape1.Location
        pt.X += s1
        Me.OvalShape1.Location = pt

        pt = Me.OvalShape2.Location
        pt.X += s2
        Me.OvalShape2.Location = pt

        pt = Me.OvalShape3.Location
        pt.X += s3
        Me.OvalShape3.Location = pt
    End Sub
End Class