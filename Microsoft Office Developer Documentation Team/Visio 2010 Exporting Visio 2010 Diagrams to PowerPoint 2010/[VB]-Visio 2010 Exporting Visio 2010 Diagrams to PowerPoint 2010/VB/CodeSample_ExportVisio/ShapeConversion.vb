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
Imports System.Xml.Linq

Public Class ShapeConversion

#Region "Class fields"
    ' Declare global variables - Visio shape, page height of Visio page, font.
    ' Declare a reusable variable for data conversion (as single).
    ' Declare a variable to store the conversion factor of Visio inches to PowerPoint points.
    Private pgHeight As Single
    Private vsoShape As Visio.Shape
    Private vsoFont As Visio.Font
    Private vsoCell As Visio.Cell
    Private sng As Single
    Private cf As Integer

    ' Declare private fields for setting class properties.
    Private shapeType As Integer
    Private shapeName As String
    Private shapeWidth As Integer
    Private shapeHeight As Integer
    Private shapeLeft As Integer
    Private shapeTop As Integer
    Private shapeRotation As Single

    Private shapeHasText As Boolean
    Private shapeText As String
    Private shapeTextFont As String
    Private shapeTextColor As Integer
    Private shapeTextFontSize As Single
    Private shapeTextVAlign As Integer
    Private shapeTextHAlign As Integer
    Private shapeTextOrient As Integer
    Private shapeTextOverlayX, shapeTextOverlayY As Integer
    Private shapeTextOverlayWidth, shapeTextOverlayHeight As Integer

    Private shapeFillColor As Integer
    Private shapeFillTrans As Single

    Private shapeLineColor As Integer
    Private shapeLineWeight As Single
    Private shapeLinePattern As Integer
    Private shapeLineBeginArrow As Integer
    Private shapeLineEndArrow As Integer

    Private shapeShadow As Boolean
    Private shapeShadowColor As Integer
    Private shapeShadowTrans As Single
    Private shapeShadowOffsetX, shapeShadowOffsetY As Short
    Private shapeShadowSize As Single

    Private connectorBeginX As Short
    Private connectorBeginY As Short
    Private connectorEndX As Short
    Private connectorEndY As Short
    Private connectorType As Short

#End Region

#Region "Class properties"
    ReadOnly Property pptShapeType() As Integer
        Get
            pptShapeType = shapeType
        End Get
    End Property
    ReadOnly Property pptShapeName() As String
        Get
            pptShapeName = shapeName
        End Get
    End Property
    ReadOnly Property pptShapeWidth() As Single
        Get
            pptShapeWidth = shapeWidth
        End Get
    End Property
    ReadOnly Property pptShapeHeight() As Single
        Get
            pptShapeHeight = shapeHeight
        End Get
    End Property
    ReadOnly Property pptShapeLeft() As Single
        Get
            pptShapeLeft = shapeLeft
        End Get
    End Property
    ReadOnly Property pptShapeTop() As Single
        Get
            pptShapeTop = shapeTop
        End Get
    End Property
    ReadOnly Property pptShapeRotation As Single
        Get
            pptShapeRotation = shapeRotation
        End Get
    End Property
    ReadOnly Property pptShapeText() As String
        Get
            pptShapeText = shapeText
        End Get
    End Property
    ReadOnly Property hasText() As Boolean
        Get
            hasText = shapeHasText
        End Get
    End Property
    ReadOnly Property pptShapeTextColor As Integer
        Get
            pptShapeTextColor = shapeTextColor
        End Get
    End Property
    ReadOnly Property pptShapeTextFont As String
        Get
            pptShapeTextFont = shapeTextFont
        End Get
    End Property
    ReadOnly Property pptShapeTextFontSize() As Single
        Get
            pptShapeTextFontSize = shapeTextFontSize
        End Get
    End Property
    ReadOnly Property pptShapeTextHAlign As Integer
        Get
            pptShapeTextHAlign = shapeTextHAlign
        End Get
    End Property
    ReadOnly Property pptShapeTextVAlign As Integer
        Get
            pptShapeTextVAlign = shapeTextVAlign
        End Get
    End Property
    ReadOnly Property pptShapeTextOrientation As Integer
        Get
            pptShapeTextOrientation = shapeTextOrient
        End Get
    End Property
    ReadOnly Property pptShapeTextOverlayX() As Integer
        Get
            pptShapeTextOverlayX = shapeTextOverlayX
        End Get
    End Property
    ReadOnly Property pptShapeTextOverlayY() As Integer
        Get
            pptShapeTextOverlayY = shapeTextOverlayY
        End Get
    End Property
    ReadOnly Property pptShapeTextOverlayWidth() As Integer
        Get
            pptShapeTextOverlayWidth = shapeTextOverlayWidth
        End Get
    End Property
    ReadOnly Property pptShapeTextOverlayHeight() As Integer
        Get
            pptShapeTextOverlayHeight = shapeTextOverlayHeight
        End Get
    End Property
    ReadOnly Property pptShapeFillColor As Integer
        Get
            pptShapeFillColor = shapeFillColor
        End Get
    End Property
    ReadOnly Property pptShapeFillTrans As Single
        Get
            pptShapeFillTrans = shapeFillTrans
        End Get
    End Property
    ReadOnly Property pptShapeLineColor As Integer
        Get
            pptShapeLineColor = shapeLineColor
        End Get
    End Property
    ReadOnly Property pptShapeLineWeight As Single
        Get
            pptShapeLineWeight = shapeLineWeight
        End Get
    End Property
    ReadOnly Property pptShapeLinePattern As Integer
        Get
            pptShapeLinePattern = shapeLinePattern
        End Get
    End Property
    ReadOnly Property pptShapeLineBeginArrow
        Get
            pptShapeLineBeginArrow = shapeLineBeginArrow
        End Get
    End Property
    ReadOnly Property pptShapeLineEndArrow
        Get
            pptShapeLineEndArrow = shapeLineEndArrow
        End Get
    End Property
    ReadOnly Property pptConnectorBeginX As Short
        Get
            pptConnectorBeginX = connectorBeginX
        End Get
    End Property
    ReadOnly Property pptConnectorBeginY As Short
        Get
            pptConnectorBeginY = connectorBeginY
        End Get
    End Property
    ReadOnly Property pptConnectorEndX As Short
        Get
            pptConnectorEndX = connectorEndX
        End Get
    End Property
    ReadOnly Property pptConnectorEndY As Short
        Get
            pptConnectorEndY = connectorEndY
        End Get
    End Property
    ReadOnly Property pptConnectorType As Short
        Get
            pptConnectorType = connectorType
        End Get
    End Property
    ReadOnly Property pptShapeShadow As Integer
        Get
            pptShapeShadow = shapeShadow

        End Get
    End Property
    ReadOnly Property pptShapeShadowColor As Integer
        Get
            pptShapeShadowColor = shapeShadowColor
        End Get
    End Property
    ReadOnly Property pptShapeShadowTrans As Single
        Get
            pptShapeShadowTrans = shapeShadowTrans
        End Get
    End Property
    ReadOnly Property pptShapeShadowOffsetX As Short
        Get
            pptShapeShadowOffsetX = shapeShadowOffsetX
        End Get
    End Property
    ReadOnly Property pptShapeShadowOffsetY As Short
        Get
            pptShapeShadowOffsetY = shapeShadowOffsetY
        End Get
    End Property
    ReadOnly Property pptShapeShadowSize As Single
        Get
            pptShapeShadowSize = shapeShadowSize
        End Get
    End Property
#End Region

    Sub New(ByVal shape As Visio.Shape)

        ' The ShapeConversion object converts Visio shape data into PowerPoint shape data.
        ' This constructor method has one parameter, shape, which is the Visio shape that is being converted to a PowerPoint shape.
        vsoShape = shape

        ' Define the pgHeight variable for size and location conversion.
        pgHeight = vsoShape.ContainingPage.PageSheet.Cells("PageHeight").Result(vbSingle)

        ' Create a conversion factor variable to translate Visio inches to PPT points.
        ' 1" (Visio) = 72 points (PowerPoint)
        'cf = 72 * (7.5 / pgHeight) 'To Scale conversion factor to account for difference in page dimensions.
        cf = 72

        ' Record the vsoShape's name and then lookup the equivalent PowerPoint shape.
        shapeName = vsoShape.Name
        shapeType = LookupVisioToPowerPoint()

        ' Check if the shape is 1-D (a connector).
        If Not (vsoShape.OneD = 0) Then
            ' Get and set the connector begin and end points, arrow types, and spatial relations.
            SetConnectorData()
        Else
            ' Get and set the shape width and height
            SetShapeLocationAndDimensions()
        End If

        ' Check if the shape is not a connector or to be copy/pasted
        If Not ((shapeType = -1) Or (shapeType = 0)) Then
            ' Set shapeFillColor variable from the Visio shape color.
            shapeFillColor = GetVisioColorFromCell("FillBkgnd")

            ' Set shapeFillTrans variable as percentage of transparency on Visio shape fill.
            shapeFillTrans = GetVisioSingleFromCell("FillBkgndTrans")
        End If

        ' Get the shape's text, if any.
        If vsoShape.Characters.Text = vbNullString Then
            shapeHasText = vbFalse
        Else
            shapeHasText = vbTrue
            SetShapeText()
        End If

        ' Get the shape's fill and line information.
        SetShapeLine()

        ' Get the shape's shadow.
        SetShapeShadow()

        vsoShape = Nothing
        vsoCell = Nothing
        vsoFont = Nothing

        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub
    Private Function LookupVisioToPowerPoint()

        ' Get the PPT equivalent of the Master shape used to create the Visio shape.
        ' Use the shape name to determine the name of the Master shape
        Dim shpMaster() As String = shapeName.Split(".")
        Dim shpName As String = shpMaster(0)

        ' Default value of PPTShapeType is -1, meaning that there is no equivalent.
        Dim shapeInt As Integer = -1

        ' If the shape is a connector, set the pptShapeType property to 0.
        If (shpName.ToLower.Contains("connector")) Then
            shapeInt = 0

        ElseIf (shpName.ToLower.Contains("sheet")) Then
            ' If the shape is a sheet, then there is no equivalent and to return the default value.
            Return shapeInt

        Else
            ' Otherwise, query the XML for any records 
            ' where the value for the id attribute is equal to shpName.
            shapeInt = QueryXML(shpName, "Shape")

        End If

        Return shapeInt

    End Function
    Private Sub SetConnectorData()

        ' This sub sets all of the connector-specific properties of the shape
        ' Set the begin and end points of the shape from the Visio connector
        connectorBeginX = CShort(cf * GetVisioSingleFromCell("BeginX"))
        connectorBeginY = CShort(cf * (pgHeight - GetVisioSingleFromCell("BeginY")))
        connectorEndX = CShort(cf * GetVisioSingleFromCell("EndX"))
        connectorEndY = CShort(cf * (pgHeight - GetVisioSingleFromCell("EndY")))

        ' Set shapeLineBeginArrow, shapeLineEndArrow as Microsoft.Office.Core.MsoArrowheadStyle enum (integer)
        ' Lookup values if they are not the default / most common values (0, 5).
        sng = GetVisioSingleFromCell("BeginArrow")
        If Not sng = 0 Then
            shapeLineBeginArrow = QueryXML(CStr(sng), "LineArrow", 1)
        Else
            shapeLineBeginArrow = 1
        End If

        sng = GetVisioSingleFromCell("EndArrow")
        If Not sng = 5 Then
            shapeLineEndArrow = QueryXML(CStr(sng), "LineArrow", 4)
        Else
            shapeLineEndArrow = 4
        End If

        ' Determine spatial relatations for One-D shapes using bounding box.
        Dim connectorLeft As Double
        Dim connectorBottom As Double
        Dim connectorRight As Double
        Dim connectorTop As Double

        vsoShape.BoundingBox(4, connectorLeft, connectorBottom, connectorRight, connectorTop)

        shapeWidth = CInt((connectorRight - connectorLeft) * cf)
        shapeHeight = CInt((connectorTop - connectorBottom) * cf)
        shapeLeft = CInt(connectorLeft * cf)
        shapeTop = CInt((pgHeight - connectorTop) * cf)

        ' Determine type of PPT connector to create.
        If ((shapeWidth < 7) Or (shapeHeight < 7)) Then
            ' Make a straight PPT connector
            connectorType = 1

        Else
            ' Make an elbow PPT connector
            connectorType = 2
        End If

    End Sub
    Private Sub SetShapeLocationAndDimensions()

        ' Get the height and width of the shape. 
        If CBool(vsoShape.CellExists("Width", 1)) Then
            shapeWidth = cf * GetVisioSingleFromCell("Width")
            shapeHeight = cf * GetVisioSingleFromCell("Height")
        Else
            shapeWidth = cf * GetVisioSingleFromCell("User.DefaultWidth")
            shapeHeight = cf * GetVisioSingleFromCell("User.DefaultHeight")
        End If

        ' Get the Visio shape's relationship to top-left corner of page.
        shapeLeft = (cf * GetVisioSingleFromCell("PinX")) - (0.5 * shapeWidth)
        shapeTop = (cf * (pgHeight - GetVisioSingleFromCell("PinY"))) - (0.5 * shapeHeight)

        ' Get the shape's rotation around the z-axis.
        Dim vsoAngleCell As Visio.Cell = vsoShape.CellsU("Angle")
        shapeRotation = 360 - (vsoAngleCell.Result(Visio.VisUnitCodes.visDegrees))

        ' Determine whether the shape has Data Graphics applied.
        If Not (IsNothing(vsoShape.DataGraphic)) Then

            Dim localLft As Double
            Dim localRgt As Double
            Dim localTop As Double
            Dim localBtm As Double
            Dim bBoxArgs As Integer = CInt(Visio.VisBoundingBoxArgs.visBBoxUprightWH) + CInt(Visio.VisBoundingBoxArgs.visBBoxIncludeDataGraphics)

            ' Include Data Graphics with dimensions of shape.
            vsoShape.BoundingBox(bBoxArgs, localLft, localBtm, localRgt, localTop)
            shapeWidth = cf * (localRgt - localLft)
            shapeHeight = cf * (localTop - localBtm)

        End If

    End Sub
    Private Sub SetShapeText()

        ' Get the shape's text.
        shapeText = vsoShape.Text

        ' Get the font size of the shape's text (converted).
        shapeTextFontSize = CSng(GetVisioStringFromCell("Char.Size").Replace(" pt", ""))

        ' Get the name of the font used by the shape text. 
        vsoFont = vsoShape.Document.Fonts.ItemFromID(GetVisioSingleFromCell("Char.Font"))
        shapeTextFont = vsoFont.Name

        ' Get the font color of the shape's text. 
        shapeTextColor = GetVisioColorFromCell("Char.Color")

        ' Get the horizontal alignment of the shape's text.
        shapeTextHAlign = CInt(GetVisioSingleFromCell("Para.HorzAlign")) + 1

        ' Get the vertical alignment of the shape's text.
        sng = GetVisioSingleFromCell("VerticalAlign")
        Select Case CInt(sng)
            Case 0
                shapeTextVAlign = 1
            Case 1
                shapeTextVAlign = 3
            Case 2
                shapeTextVAlign = 4
            Case Else
                shapeTextVAlign = 1
        End Select

        ' Get the orientation of the shape's text.
        sng = GetVisioSingleFromCell("TextDirection")
        Select Case CInt(sng)
            Case 0
                shapeTextOrient = 1
            Case 1
                shapeTextOrient = 3
            Case Else
                shapeTextOrient = 1
        End Select

        ' Get the spatial relationship and dimension of the shape's text block.
        If (vsoShape.CellExists("TxtWidth", 1)) Then

            ' Get the height and width of text block.
            shapeTextOverlayWidth = GetVisioSingleFromCell("TxtWidth") * (cf * 1.1)
            shapeTextOverlayHeight = GetVisioSingleFromCell("TxtHeight") * (cf * 1.1)

            ' Text overlay location is measured differently between connectors and shapes. 
            If (shapeType = 0) Then

                ' Get x-axis location of text box relative to center of connector.
                sng = GetVisioSingleFromCell("TxtPinX") - GetVisioSingleFromCell("TxtLocPinX")
                shapeTextOverlayX = CInt(((sng + GetVisioSingleFromCell("PinX")) - (GetVisioSingleFromCell("Width") * 0.5)) * cf)

                ' Get y-axis location of text box relative to center of connector
                sng = GetVisioSingleFromCell("PinY") + GetVisioSingleFromCell("TxtLocPinY")
                shapeTextOverlayY = (pgHeight - sng) * cf

            Else
                ' Text block distance on y-axis requires an additional constant of 10 pts.
                sng = (pgHeight - GetVisioSingleFromCell("PinY")) + GetVisioSingleFromCell("TxtLocPinY") - GetVisioSingleFromCell("TxtPinY")
                shapeTextOverlayY = CInt(sng * cf) + 10

                ' Get text block location relative to left edge of shape.
                sng = GetVisioSingleFromCell("TxtPinX") - GetVisioSingleFromCell("TxtLocPinX")
                shapeTextOverlayX = CInt(sng * cf) + shapeLeft

            End If
        Else
            ' The shape doesn't have a text block, so the overlay will be
            ' equivalent to the shapes dimensions.
            shapeTextOverlayWidth = shapeWidth
            shapeTextOverlayHeight = shapeHeight
            shapeTextOverlayX = shapeLeft
            shapeTextOverlayY = shapeTop

        End If
    End Sub
    Private Sub SetShapeLine()

        ' Set shapeLineColor variable from the Visio shape line color.
        shapeLineColor = GetVisioColorFromCell("LineColor")

        ' Set shapeLineWeight variable from the Visio shape border weight.
        shapeLineWeight = CSng(GetVisioStringFromCell("LineWeight").Replace(" pt", ""))

        ' Set shapeLinePattern as a value from the Microsoft.Office.Core.MsoLineDashStyle enum.
        ' Lookup value if it is not the default / most common value (1).
        sng = GetVisioSingleFromCell("LinePattern")
        If Not sng = 1 Then
            shapeLinePattern = QueryXML(CStr(sng), "LinePattern", 1)
        Else
            shapeLinePattern = 1
        End If

    End Sub
    Private Sub SetShapeShadow()

        ' Get pattern of Visio shape's shadow.
        If (CInt(GetVisioSingleFromCell("ShdwPattern")) > 0) Then
            shapeShadow = vbTrue
        Else
            shapeShadow = vbFalse
        End If

        ' Get Visio shadow color as PaletteEntry (integer).
        shapeShadowColor = GetVisioColorFromCell("ShdwForegnd")

        ' Get percentage of transparency for Visio shape's shadow.
        shapeShadowTrans = GetVisioSingleFromCell("ShdwForegndTrans")

        ' Get X-offset X and Y-offset of Visio shape's shadow, both singles.
        shapeShadowOffsetX = CShort(GetVisioSingleFromCell("ShapeShdwOffsetX") * 72)
        shapeShadowOffsetY = CShort(0 - (GetVisioSingleFromCell("ShapeShdwOffsetY") * 72))

        ' Get size of Visio shape's shadow as a single.
        shapeShadowSize = GetVisioSingleFromCell("ShapeShdwScaleFactor") * 100

    End Sub

#Region "Helper functions"
    Private Function QueryXML(ByVal id As String, ByVal element As String, Optional ByVal defaultVal As Integer = -1) As Integer

        Dim result As Integer = defaultVal

        ' Load the XML file into memory.
        Dim assmbly As Assembly = Assembly.GetExecutingAssembly()
        Dim stream As Stream = assmbly.GetManifestResourceStream( _
            "CodeSample_ExportVisio.ShapeConversion.xml")
        Dim xDoc As XDocument = XDocument.Load(stream)

        ' Define the query.
        Dim elements = _
            From el In xDoc.Descendants(element)
            Where (el.Attribute("id") = id)
            Select el.Value

        result = elements.DefaultIfEmpty(defaultVal).FirstOrDefault()

        ' Release the memory used for the xmlDocument object and return result.
        xDoc = Nothing
        Return result

    End Function
    Private Function GetVisioStringFromCell(ByVal cellName As String) As String

        ' Get data from a Visio cell and return as a string.
        vsoCell = vsoShape.Cells(cellName)
        Return vsoCell.ResultStrU(vbString)

    End Function
    Private Function GetVisioColorFromCell(ByVal cellName As String) As Integer

        Dim colorNum As Integer
        Dim vsoColor As Visio.Color

        ' Get data from a Visio cell containing color settings. 
        vsoCell = vsoShape.Cells(cellName)
        colorNum = vsoCell.Result(vbInteger)

        ' Gets a Visio.Color object from the document and returns as PaletteEntry.
        vsoColor = vsoShape.Document.Colors(colorNum)
        Return vsoColor.PaletteEntry

    End Function
    Private Function GetVisioSingleFromCell(ByVal cellName As String) As Single

        ' Get data from a Visio cell containing single data type.
        vsoCell = vsoShape.Cells(cellName)
        Return vsoCell.Result(vbSingle)

    End Function
#End Region

End Class
