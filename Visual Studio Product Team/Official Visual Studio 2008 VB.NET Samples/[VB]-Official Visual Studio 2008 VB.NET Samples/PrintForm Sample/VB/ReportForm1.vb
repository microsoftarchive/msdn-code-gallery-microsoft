' This sample application demonstrates the use of the PrintForm
' component to print a simple report without the need for a 
' PrintDocument component.


Public Class Report

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Format the labels, picture boxes and button
        Label1.Text = My.Resources.CostOfGoods
        Label2.Text = My.Resources.OperatingExpense
        Label3.Text = My.Resources.Profit
        PictureBox1.BackColor = Color.Khaki
        PictureBox2.BackColor = Color.CadetBlue
        PictureBox3.BackColor = Color.Green
        Button1.Text = My.Resources.PrintButton
    End Sub

    Private Sub Form1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Dim drawFormat As New StringFormat()

        ' Draw the report title
        Using formGraphics As Graphics = Me.CreateGraphics(), _
            drawFont As New System.Drawing.Font("Arial", 24), _
            drawBrush As New SolidBrush(Color.Black)

            formGraphics.DrawString(My.Resources.ReportTitle, drawFont, drawBrush, _
                30.0, 20.0, drawFormat)
        End Using
        ' Call the method that draws a pie chart
        DrawPieChartHelper()

        ' Draw the values on the chart
        Using formGraphics As Graphics = Me.CreateGraphics(), _
            drawFont As New System.Drawing.Font("Arial", 12), _
            drawBrush As New SolidBrush(Color.Red)

            formGraphics.DrawString(My.Resources.COGValue, drawFont, drawBrush, _
                90.0, 140.0, drawFormat)
            formGraphics.DrawString(My.Resources.OEValue, drawFont, drawBrush, _
                150.0, 250.0, drawFormat)
            formGraphics.DrawString(My.Resources.PValue, drawFont, drawBrush, _
                210.0, 210.0, drawFormat)
        End Using

    End Sub
    ' Shows how to call the DrawPieChart method
    Public Sub DrawPieChartHelper()
        Dim percents() As Integer = {10, 20, 70}
        Dim colors() As Color = {Color.Green, Color.CadetBlue, Color.Khaki}
        Dim graphics As Graphics = Me.CreateGraphics
        Dim location As Point = New Point(30, 70)
        Dim size As Size = New Size(250, 250)
        DrawPieChart(percents, colors, graphics, location, size)
    End Sub


    ' Draws a pie chart.
    Public Sub DrawPieChart(ByVal percents() As Integer, ByVal colors() As Color, _
    ByVal surface As Graphics, ByVal location As Point, ByVal pieSize As Size)
        ' Check if sections add up to 100.
        Dim sum As Integer = 0
        For Each percent As Integer In percents
            sum += percent
        Next

        If sum <> 100 Then
            Throw New ArgumentException("Percentages do not add up to 100.")
        End If

        If percents.Length <> colors.Length Then
            Throw New ArgumentException("There must be the same number of percents and colors.")
        End If

        Dim percentTotal As Integer = 0
        For percent As Integer = 0 To percents.Length() - 1
            surface.FillPie( _
                New SolidBrush(colors(percent)), _
                New Rectangle(location, pieSize), CType(percentTotal * 360 / 100, Single), CType(percents(percent) * 360 / 100, Single))
            percentTotal += percents(percent)
        Next
        Return
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ' Hide the print button
        Button1.Visible = False
        ' Set the PrintAction to display a Print Preview dialog
        PrintForm1.PrintAction = Printing.PrintAction.PrintToPreview
        ' Print a copy of the form
        PrintForm1.Print()
        ' Restore the print button
        Button1.Visible = True
    End Sub
End Class
