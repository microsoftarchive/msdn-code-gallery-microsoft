Imports Microsoft.Office.Interop.Visio
Imports Microsoft.VisualBasic.Interaction
Imports System
Imports System.Collections.Generic
Imports PowerPoint = Microsoft.Office.Interop.PowerPoint
Imports Office = Microsoft.Office.Core
Imports System.Diagnostics
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.Xml
Imports System.Reflection
Imports System.IO

Public Class ThisAddIn
#Region "Class fields"
    ' Declare new pptSlide as PowerPoint slide
    Private pptSlide As PowerPoint.Slide

    ' Declare a variable for the Visio page to be exported.
    Private vsoPage As Visio.Page

#End Region

    Public Sub Main()

        Try
            ' Save value of DiagramServicesEnabled property and turn services off.
            Dim diagramSvcs As Integer = vsoPage.Document.DiagramServicesEnabled
            vsoPage.Document.DiagramServicesEnabled = CInt(VisDiagramServices.visServiceNone)

            ' Capture a reference to the shapes on the Visio drawing page.
            Dim vsoShapes As Visio.Shapes = vsoPage.Shapes
            Dim vsoShape As Visio.Shape
            Dim shpCount As Integer = vsoShapes.Count

            ' Iterate over all of the shapes on the page.
            For x = 1 To shpCount

                vsoShape = vsoShapes(x)
                Dim shpConv As ShapeConversion = New ShapeConversion(vsoShape)

                ' Determine how to convert vsoShape.
                Select Case shpConv.pptShapeType
                    Case 0
                        ' Shape is a connector; create a PPT connector.
                        ConstructPPTConnector(shpConv)

                    Case -1
                        ' Shape is not in ShapeConversion.xml; copy the Visio shape to PPT as an enhanced metafile.
                        CopyPasteVisioToPPT(x, shpConv)

                    Case Else
                        ' Create a PPT analog of the Visio shape
                        ConstructPPTShape(shpConv)

                End Select
            Next x

            ' Get the connections on the Visio page
            Dim vsoConnects As Visio.Connects = vsoPage.Connects
            Dim cnctCount As Integer = vsoConnects.Count

            ' Check to see if there are connections on the page.
            If cnctCount > 0 Then

                ' Iterate through connection points on Visio page. 
                For c As Integer = 1 To cnctCount

                    ' Connect the matching connector and shape on the PPT slide.
                    ConnectShapes(vsoConnects(c))

                Next c
            End If

            ' Restore previous diagram services setting.
            vsoPage.Document.DiagramServicesEnabled = diagramSvcs

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Public Sub ExportAllPages()

        ' Create a new PowerPoint presentation.
        Dim pptPreso As PowerPoint.Presentation = OpenPPTSession()
        Dim vsoDoc As Visio.Document = Globals.ThisAddIn.Application.ActiveDocument
        Dim numPages = vsoDoc.Pages.Count

        ' Iterate through each page in the Visio document.
        For x As Integer = 1 To numPages

            vsoPage = vsoDoc.Pages.Item(x)
            pptSlide = AddPPTSlide(pptPreso)
            Main()

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        Next

    End Sub
    Public Sub ExportPage()

        ' Create new PowerPoint presentation with a new slide.
        Dim pptPreso As PowerPoint.Presentation = OpenPPTSession()
        pptSlide = AddPPTSlide(pptPreso)

        ' Export shapes on Visio page to PowerPoint.
        vsoPage = Application.ActivePage
        Main()

    End Sub

#Region "PowerPoint application methods"
    Private Function OpenPPTSession() As PowerPoint.Presentation

        ' This sub creates a new PowerPoint file and defines the pptSlide global variable.
        ' Create a PowerPoint application object.
        Dim pptApp As PowerPoint.Application = New PowerPoint.Application
        pptApp.Visible = vbHidden

        ' Create a new PowerPoint file.
        Dim pptPreso As PowerPoint.Presentation = pptApp.Presentations.Add()

        Return pptPreso

    End Function
    Private Function AddPPTSlide(ByVal pptPreso As PowerPoint.Presentation) As PowerPoint.Slide
        ' This function adds a slide to the presentation passed in as an argument
        ' and returns   the new slide.

        ' Try to define pptLayout as "Blank" layout of default presentation template.
        ' If another template is set as default, select the first layout.
        Dim pptLayout As PowerPoint.CustomLayout
        If IsNothing(pptPreso.SlideMaster.CustomLayouts(7)) Then
            pptLayout = pptPreso.SlideMaster.CustomLayouts(1)
        Else
            pptLayout = pptPreso.SlideMaster.CustomLayouts(7)
        End If

        ' Create newSlide using pptLayout.
        Dim slide As PowerPoint.Slide = pptPreso.Slides.AddSlide((pptPreso.Slides.Count + 1), pptLayout)

        Return slide

    End Function
#End Region
#Region "Shape building methods"
    Sub CopyPasteVisioToPPT(ByVal shpInt As Integer, _
                            ByVal shpConv As ShapeConversion)

        Dim pptPic As PowerPoint.ShapeRange
        Dim pptShape As PowerPoint.Shape
        Dim rtfBox As RichTextBox = New RichTextBox

        Dim vsoShape As Visio.Shape = vsoPage.Shapes(shpInt)
        Dim vsoChars As Visio.Characters = vsoShape.Characters

        ' Remove text from shape to exclude it from picture.
        If (shpConv.hasText = True) Then
            vsoChars.Cut()
            rtfBox.Paste()
            vsoShape.CellsU("Para.Bullet").FormulaU = 0
        End If

        ' Paste the picture onto the PowerPoint slide and capture a reference to it.
        vsoShape.Copy()
        pptPic = pptSlide.Shapes.PasteSpecial(PowerPoint.PpPasteDataType.ppPasteEnhancedMetafile)
        'pptShape = pptPic.Item(1)
        pptShape = pptSlide.Shapes(pptSlide.Shapes.Count)

        ' Adjust the dimensions of the shape and change the name.
        With pptShape
            .Name = shpConv.pptShapeName
            .Width = shpConv.pptShapeWidth
            .Left = shpConv.pptShapeLeft
            .Top = shpConv.pptShapeTop
        End With

        If shpConv.pptShapeWidth = 0 Then
            pptShape.Height = shpConv.pptShapeHeight
        End If

        ' Create a text box over the shape with the shape's text, if any.
        If (shpConv.hasText) Then
            AddTextOverlay(shpConv)

            ' Cut and paste RTF from text box back to Visio shape
            rtfBox.SelectAll()
            rtfBox.Cut()
            vsoShape.Characters.Paste()
            rtfBox.Dispose()
        End If

        vsoShape = Nothing

        ' Free up memory.
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub
    Sub ConstructPPTShape(ByVal shpConv As ShapeConversion)

        ' Create a PowerPoint shape on pptSlide.
        Dim pptShape As PowerPoint.Shape = _
            pptSlide.Shapes.AddShape(shpConv.pptShapeType, _
                                          shpConv.pptShapeLeft, _
                                          shpConv.pptShapeTop, _
                                          shpConv.pptShapeWidth, _
                                          shpConv.pptShapeHeight)

        ' Apply fill color and transparency and change the name of the shape.
        With pptShape
            .Name = shpConv.pptShapeName
            .Rotation = shpConv.pptShapeRotation
            .Fill.Solid()
            .Fill.ForeColor.RGB = shpConv.pptShapeFillColor
            .Fill.Transparency = shpConv.pptShapeFillTrans
        End With

        ' Apply line formatting or remove line if the Visio shape has none.
        If shpConv.pptShapeLinePattern = 0 Then
            pptShape.Line.Visible = vbFalse
        Else
            With pptShape
                .Line.ForeColor.RGB = shpConv.pptShapeLineColor
                .Line.DashStyle = shpConv.pptShapeLinePattern
                .Line.Weight = shpConv.pptShapeLineWeight
            End With
        End If

        ' Apply double line border to org chart shapes.
        If pptShape.Name.Contains("Executive") Or pptShape.Name.Contains("Manager") Then
            pptShape.Line.Style = Microsoft.Office.Core.MsoLineStyle.msoLineThinThin
            pptShape.Line.Weight = 3.0
        End If

        ' Add shape text, font, font color, and font size.
        If Not String.IsNullOrEmpty(shpConv.pptShapeText) Then
            With pptShape
                .TextFrame.TextRange.Text = shpConv.pptShapeText
                .TextFrame.TextRange.Font.Name = shpConv.pptShapeTextFont
                .TextFrame.TextRange.Font.Color.RGB = shpConv.pptShapeTextColor
                .TextFrame.TextRange.Font.Size = shpConv.pptShapeTextFontSize
                .TextFrame.AutoSize = PowerPoint.PpAutoSize.ppAutoSizeNone
                .TextFrame.VerticalAnchor = shpConv.pptShapeTextVAlign
                .TextFrame.TextRange.ParagraphFormat.Alignment = shpConv.pptShapeTextHAlign
                .TextFrame.Orientation = shpConv.pptShapeTextOrientation
            End With
        End If

        ' Check the resizing of the PPT shape with autosizing
        If pptShape.Height < shpConv.pptShapeHeight Then
            ' Resize the shape if it is too small
            pptShape.Height = shpConv.pptShapeHeight
            pptShape.TextFrame.AutoSize = PowerPoint.PpAutoSize.ppAutoSizeNone
            pptShape.Top = shpConv.pptShapeTop
        End If

        ' Apply shadow to PPT shape, if any.
        If shpConv.pptShapeShadow Then
            With pptShape.Shadow
                .Visible = Microsoft.Office.Core.MsoTriState.msoTrue
                .OffsetX = shpConv.pptShapeShadowOffsetX
                .OffsetY = shpConv.pptShapeShadowOffsetY
                .Transparency = shpConv.pptShapeShadowTrans
                .ForeColor.RGB = shpConv.pptShapeShadowColor
                .Size = shpConv.pptShapeShadowSize
            End With
        End If

    End Sub
    Sub AddTextOverlay(ByVal shpConv As ShapeConversion)

        ' Creates a text box on top of shape with text, centered on shape.
        Dim pptShpText As PowerPoint.Shape = _
            pptSlide.Shapes.AddTextbox( _
                shpConv.pptShapeTextOrientation, _
                shpConv.pptShapeTextOverlayX, _
                shpConv.pptShapeTextOverlayY, _
                shpConv.pptShapeTextOverlayWidth, _
                shpConv.pptShapeTextOverlayHeight)

        ' Add and format the text in the textbox.
        With pptShpText
            .TextFrame.AutoSize = PowerPoint.PpAutoSize.ppAutoSizeNone
            .TextFrame.TextRange.Text = shpConv.pptShapeText
            .TextFrame.TextRange.Font.Name = shpConv.pptShapeTextFont
            .TextFrame.TextRange.Font.Color.RGB = shpConv.pptShapeTextColor
            .TextFrame.TextRange.Font.Size = shpConv.pptShapeTextFontSize
            .TextFrame.VerticalAnchor = shpConv.pptShapeTextVAlign
            .TextFrame.TextRange.ParagraphFormat.Alignment = shpConv.pptShapeTextHAlign
            pptShpText.Rotation = shpConv.pptShapeRotation
        End With

        ' Change background of textbox to match slide background if the shape is a connector.
        If shpConv.pptShapeType = 0 Then
            pptShpText.TextFrame.TextRange.ParagraphFormat.Alignment = _
                Microsoft.Office.Core.MsoParagraphAlignment.msoAlignCenter
            pptShpText.Fill.ForeColor = pptSlide.Background.Fill.ForeColor
        End If

        ' Adjust the location of the text overlay.
        With pptShpText
            .Left = shpConv.pptShapeTextOverlayX
            .Top = shpConv.pptShapeTextOverlayY
        End With
    End Sub
#End Region
#Region "Connector building methods"
    Sub ConstructPPTConnector(ByVal shpConv As ShapeConversion)

        ' Create the PowerPoint connector shape.
        Dim pptConnector As PowerPoint.Shape
        pptConnector = pptSlide.Shapes.AddConnector( _
            shpConv.pptConnectorType, _
            shpConv.pptConnectorBeginX, _
            shpConv.pptConnectorBeginY, _
            shpConv.pptConnectorEndX, _
            shpConv.pptConnectorEndY)

        ' Apply line formatting to connector.
        With pptConnector
            .Name = shpConv.pptShapeName
            .Line.ForeColor.RGB = shpConv.pptShapeLineColor
            .Line.Weight = shpConv.pptShapeLineWeight
            .Line.DashStyle = shpConv.pptShapeLinePattern
            .Line.BeginArrowheadStyle = shpConv.pptShapeLineBeginArrow
            .Line.EndArrowheadStyle = shpConv.pptShapeLineEndArrow
        End With

        ' Add text box to shape with shape text, if any.
        If shpConv.hasText Then
            AddTextOverlay(shpConv)
        End If

    End Sub
    Sub ConnectShapes(ByRef vsoConnect As Visio.Connect)

        ' Get names of connected shapes on Visio page.
        Dim cnctName As String = vsoConnect.FromSheet.Name
        Dim shpName As String = vsoConnect.ToSheet.Name

        ' Check that connector shape is a standard connector.
        If (cnctName.ToLower.Contains("connector")) Then

            ' Get analog shapes on PPT slide.
            Dim pptConnector As PowerPoint.Shape = pptSlide.Shapes(cnctName)
            Dim pptShape As PowerPoint.Shape = pptSlide.Shapes(shpName)

            ' Get the relationship of the connection point to the connector and shape.
            Dim cnctSite As Integer = DetermineConnectionSite(pptConnector, pptShape)
            Dim cnctEnd As Integer = vsoConnect.FromPart

            If (cnctEnd = 9) Then
                ' The Visio connector is connected at beginning; connect PPT connector at beginning.
                pptConnector.ConnectorFormat.BeginConnect(pptShape, cnctSite)

            Else
                'The Visio connector is connected at end; connect PPT connector at end.
                pptConnector.ConnectorFormat.EndConnect(pptShape, cnctSite)
            End If
        End If
    End Sub
    Function DetermineConnectionSite(ByVal pptConnector As PowerPoint.Shape, _
                                     ByVal pptShape As PowerPoint.Shape) As Integer
        ' This method determines where to connect a connector to a shape, based upon
        ' the spatial relationship of the connector to the shape.

        ' Determine the center focal point of a connector.
        Dim cnctCenterX As Single = pptConnector.Left + (0.5 * pptConnector.Width)
        Dim cnctCenterY As Single = pptConnector.Top + (0.5 * pptConnector.Height)

        ' Establish the slope and constant for a line, using equation 'y = mx + k', that includes 
        ' the top left and bottom right corner points of the pptShape's bounding box.
        Dim aTest As Boolean
        Dim aSlope As Single = pptShape.Height / pptShape.Width
        Dim aConstant As Single = pptShape.Top - (pptShape.Left * aSlope)

        ' Determine the relationship of pptConnector's focal point to the line,
        ' using a line that runs parallel to the line through the pptShape.
        If cnctCenterY > (cnctCenterX * aSlope) + aConstant Then

            ' pptConnector is below and/or to the left of pptShape. 
            aTest = True

        Else
            ' pptConnector is above and/or to the right of pptShape.
            aTest = False

        End If

        ' Establish the slope and constant for a line, using equation 'y = mx + k', that includes 
        ' the top right and bottom left corner points of the pptShape's bounding box.
        Dim bTest As Boolean
        Dim bSlope As Single = 0 - (pptShape.Height / pptShape.Width)
        Dim bConstant As Single = (pptShape.Top + pptShape.Height) _
                                                - (pptShape.Left * bSlope)

        ' Determine the relationship of pptConnector's focal point to line,
        ' using a line that runs parallel to the line through pptShape.
        If cnctCenterY > (cnctCenterX * bSlope) + bConstant Then
            bTest = True
        Else
            bTest = False
        End If

        ' Determine quadrant where connector is located in relation to the shape.
        Dim cnctCount As Integer = pptShape.ConnectionSiteCount
        Dim topConnection, lftConnection, btmConnection, rgtConnection As Integer

        ' Account for the different possible total connection sites.
        Select Case cnctCount
            Case 4
                topConnection = 1
                lftConnection = 2
                btmConnection = 3
                rgtConnection = 4
            Case 5
                topConnection = 1
                lftConnection = 2
                btmConnection = 3
                rgtConnection = 5
            Case 6
                topConnection = 1
                lftConnection = 3
                btmConnection = 5
                rgtConnection = 6
            Case 8
                topConnection = 1
                lftConnection = 3
                btmConnection = 5
                rgtConnection = 7
            Case Else
                topConnection = System.Math.Floor(1 * (cnctCount / 4))
                lftConnection = System.Math.Floor(2 * (cnctCount / 4))
                btmConnection = System.Math.Floor(3 * (cnctCount / 4))
                rgtConnection = System.Math.Floor(4 * (cnctCount / 4))
        End Select

        If aTest.Equals(True) And bTest.Equals(True) Then
            ' Connector is below the shape; glue connector to bottom connection point.
            Return btmConnection

        ElseIf aTest.Equals(True) And bTest.Equals(False) Then
            ' Connector is to the left of the shape; glue connector to left connection point.
            Return lftConnection

        ElseIf aTest.Equals(False) And bTest.Equals(True) Then
            ' Connector is to the right of the shape; glue connector to the right connection point.
            Return rgtConnection

        Else
            ' Connector is above the shape; glue connector to the top connection point.
            Return topConnection
        End If
    End Function
#End Region

    Protected Overrides Function CreateRibbonExtensibilityObject() As Microsoft.Office.Core.IRibbonExtensibility
        Return New Ribbon1()
    End Function

End Class


