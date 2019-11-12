Imports Microsoft.Office.Interop
Imports Microsoft.Office.Core
Public Class Form1

    Dim objPPT As PowerPoint.Application
    Dim objPres As PowerPoint.Presentation

    Private Sub cmdStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStart.Click
        StartPowerPoint()
    End Sub
    Private Sub cmdCreatePresentation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCreatePresentation.Click
        EnsurePowerPointIsRunning(False, False)
        'Add Presentation
        objPres = objPPT.Presentations.Add(MsoTriState.msoTrue)
    End Sub
    Private Sub cmdAddSlide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddSlide.Click
        Dim objSlide As PowerPoint.Slide
        Dim objCustomLayout As PowerPoint.CustomLayout
        EnsurePowerPointIsRunning(True)
        'Create a custom layout based on the first layout in the slide master.
        'This is used simply for creating the slide
        objCustomLayout = objPres.SlideMaster.CustomLayouts.Item(1)
        'Create slide
        objSlide = objPres.Slides.AddSlide(1, objCustomLayout)
        'Set the layout
        objSlide.Layout = PowerPoint.PpSlideLayout.ppLayoutText
        'Clean up
        objCustomLayout.Delete()
        objCustomLayout = Nothing
        objSlide = Nothing
    End Sub
    Private Sub cmdRemoveSlide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRemoveSlide.Click
        EnsurePowerPointIsRunning(True)
        If objPres.Slides.Count > 0 Then
            objPres.Slides(1).Delete()
        Else
            MsgBox("No slides to remove", MsgBoxStyle.Information)
        End If
    End Sub
    Private Sub cmdSetTitleText_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSetTitleText.Click
        Dim i As Integer
        EnsurePowerPointIsRunning(True, True)
        'Add Text to slide title. Find first textbox in shape collection
        'If none exists, then do nothing
        objPres.Slides(1).Select()
        For i = 1 To objPres.Slides(1).Shapes.Count
            If objPres.Slides(1).Shapes(i).HasTextFrame Then
                objPres.Slides(1).Shapes(i).TextFrame.TextRange.Text = Me.txtTitle.Text
                Exit For
            End If
        Next i
    End Sub


    Private Sub cmdAddChart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddChart.Click
        Dim ds As New ShipmentSchema, dt As ShipmentSchema.ShipmentDataTable
        EnsurePowerPointIsRunning(True, True)
        '
        'Load data from the xml file distributed with
        'the sample
        ds.ReadXml(My.Application.Info.DirectoryPath & "\ShipmentData.xml")
        dt = ds.Tables("Shipment")
        '
        'Start excel, populate a sheet with the XML data, create a chart in Excel
        'and copy to poerpoint
        Dim objExcel As Excel.Application
        Dim objWorkbook As Excel.Workbook
        Dim objSheet As Excel.Worksheet
        Dim objChart As Excel.Chart
        objExcel = New Excel.Application
        objExcel.Visible = True
        objWorkbook = objExcel.Workbooks.Add
        objSheet = objWorkbook.Sheets("Sheet1")
        DataTableToExcelSheet(dt, objSheet, 1, 1)
        objSheet.Range("A1:B4").Select()
        objChart = objExcel.Charts.Add()
        With objChart
            '3D pie chart
            .ChartType = Excel.XlChartType.xl3DPie
            'chart style is numeric - you can find the list by hovering
            'over the chart styles gallery in Excel
            .ChartStyle = 10
            'Turning off autoscaling allows us to resize the chart
            .AutoScaling = False
            'Increasing elevation tilts the pie chart towards the user
            .Elevation = 30
            .Select()
        End With
        Application.DoEvents()
        'width and height are set in pixels
        objExcel.Selection.width = 300
        objExcel.Selection.Height = 300
        'Copy picture of chart to clipboard
        objChart.CopyPicture(Excel.XlPictureAppearance.xlPrinter, Excel.XlCopyPictureFormat.xlPicture, Excel.XlPictureAppearance.xlPrinter)
        'paste into PowerPoint
        objPPT.Activate()
        Dim objSlide As PowerPoint.Slide
        Dim objShape As PowerPoint.Shape
        objSlide = objPres.Slides(1)
        objSlide.Select()
        objSlide.Layout = PowerPoint.PpSlideLayout.ppLayoutTitleOnly
        objSlide.Shapes.Paste()
        objShape = objSlide.Shapes(2)
        objShape.ZOrder(MsoZOrderCmd.msoSendToBack)
        objShape.Left = 400
        objShape.Top = 100
        'Clean up
        objWorkbook.Close(False)
        objExcel.Quit()
        objExcel = Nothing
    End Sub


    Private Sub cmdAddTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddTable.Click
        Dim objShape As PowerPoint.Shape
        Dim objTable As PowerPoint.Table
        EnsurePowerPointIsRunning(True, True)
        '
        'Load a data table from the XML file distributed with the 
        'sample application. This table will be used to populate the
        'PowerPoint table
        Dim ds As New ShipmentSchema, dt As ShipmentSchema.ShipmentDataTable
        ds.ReadXml(My.Application.Info.DirectoryPath & "\ShipmentData.xml")
        dt = ds.Tables("Shipment")
        '
        'Add a table into the first slide in the presentation
        objPres.Slides(1).Select()
        objShape = objPres.Slides(1).Shapes.AddTable(5, 2, 50, 100, 300)
        objTable = objShape.Table
        '
        'Populate the table from the dataset
        With objShape.Table
            .Cell(1, 1).Shape.TextFrame.TextRange.Text = dt.Columns.Item(0).ColumnName
            .Cell(1, 2).Shape.TextFrame.TextRange.Text = dt.Columns.Item(1).ColumnName
            'Apply a table style, using the GUID of the style
            .ApplyStyle("{B301B821-A1FF-4177-AEE7-76D212191A09}", False)
            Dim nRow As Integer, nCol As Integer
            For nRow = 0 To dt.Rows.Count - 1
                For nCol = 0 To dt.Columns.Count - 1
                    .Cell(2 + nRow, 1 + nCol).Shape.TextFrame.TextRange.Text = dt.Rows(nRow).Item(nCol)
                Next nCol
            Next nRow
        End With
        '
        'Clean up
        objTable = Nothing
        objShape = Nothing
        dt = Nothing
        ds = Nothing
    End Sub
    Private Sub cmdAddTextbox_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddTextbox.Click
        Dim objShape As PowerPoint.Shape
        Dim strText As String = "Tacoma shipments increase 10%" & vbCrLf & "Seattle shipments steady"
        EnsurePowerPointIsRunning(True, True)
        objPres.Slides(1).Select()
        objShape = objPres.Slides(1).Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 50, 300, 300, 300)
        objShape.TextFrame.AutoSize = PowerPoint.PpAutoSize.ppAutoSizeShapeToFitText
        objShape.TextFrame.TextRange.Text = strText
        objShape.TextEffect.FontSize = 20
        objShape.TextEffect.FontBold = MsoTriState.msoTrue
        '
        'Clean up
        objShape = Nothing
    End Sub
    Sub StartPowerPoint()
        objPPT = New PowerPoint.Application
        objPPT.Visible = MsoTriState.msoTrue
        objPPT.WindowState = PowerPoint.PpWindowState.ppWindowMaximized
    End Sub
    Sub EnsurePowerPointIsRunning(Optional ByVal blnAddPresentation As Boolean = False, Optional ByVal blnAddSlide As Boolean = False)
        Dim strName As String
        '
        'Try accessing the name property. If it causes an exception then 
        'start a new instance of PowerPoint
        Try
            strName = objPPT.Name
        Catch ex As Exception
            StartPowerPoint()
        End Try
        '
        'blnAddPresentation is used to ensure there is a presentation loaded
        If blnAddPresentation = True Then
            Try
                strName = objPres.Name
            Catch ex As Exception
                objPres = objPPT.Presentations.Add(MsoTriState.msoTrue)
            End Try
        End If
        '
        'BlnAddSlide is used to ensure there is at least one slide in the 
        'presentation
        If blnAddSlide Then
            Try
                strName = objPres.Slides(1).Name
            Catch ex As Exception
                Dim objSlide As PowerPoint.Slide
                Dim objCustomLayout As PowerPoint.CustomLayout
                objCustomLayout = objPres.SlideMaster.CustomLayouts.Item(1)
                objSlide = objPres.Slides.AddSlide(1, objCustomLayout)
                objSlide.Layout = PowerPoint.PpSlideLayout.ppLayoutText
                objCustomLayout = Nothing
                objSlide = Nothing
            End Try
        End If
    End Sub
    Private Sub cmdQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdQuit.Click
        Try
            objPres.Close()
            objPres = Nothing
        Catch
        End Try
        Try
            objPPT.Quit()
            objPPT = Nothing
        Catch ex As Exception
        End Try
        System.GC.Collect()
    End Sub
    Sub DataTableToExcelSheet(ByVal dt As DataTable, ByVal objSheet As Excel.Worksheet, ByVal nStartRow As Integer, ByVal nStartCol As Integer)
        Dim nRow As Integer, nCol As Integer
        'copy a datatable into an excel sheet
        For nRow = 0 To dt.Rows.Count - 1
            For nCol = 0 To dt.Columns.Count - 1
                objSheet.Cells(nStartRow + nRow, nStartCol + nCol) = dt.Rows(nRow).Item(nCol)
            Next nCol
        Next nRow
    End Sub
End Class
