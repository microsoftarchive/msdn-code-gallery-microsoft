'****************************** Module Header ******************************\
' Module Name:  CreateWordChart.vb
' Project:      VBOpenXmlCreateChartInWord
' Copyright(c)  Microsoft Corporation.
' 
' The Class is used to Create Chart into Word.
' Microsoft Word *.docx is an Open XML document combining texts, stytle,grapyhics 
' and so on into a single ZIP archive. 
' The Class uses Open XML SDK API to insert Pie Chart into Word automatically.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Drawing.Charts
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports System.Collections.Generic
Imports d = DocumentFormat.OpenXml.Drawing
Imports dc = DocumentFormat.OpenXml.Drawing.Charts
Imports dw = DocumentFormat.OpenXml.Drawing.Wordprocessing

Public Class CreateWordChart
    Implements IDisposable

    ' Specify whether the instance is disposed.
    Private disposed As Boolean = False

    ' The Word package
    Private document As WordprocessingDocument
    Public Sub New(wordpath As String)
        If String.IsNullOrEmpty(wordpath) OrElse Not System.IO.File.Exists(wordpath) Then
            Throw New Exception("The file is invalid. Please select an existing file again!")
        End If

        Me.document = WordprocessingDocument.Create(wordpath, WordprocessingDocumentType.Document)
    End Sub

    ' Create chart into Word document
    Public Sub CreateChart(chartList As List(Of ChartSubArea))
        ' Get MainDocumentPart of Document
        Dim mainPart As MainDocumentPart = document.AddMainDocumentPart()
        mainPart.Document = New Document(New Body())

        ' Create ChartPart object in Word Document
        Dim chartPart As ChartPart = mainPart.AddNewPart(Of ChartPart)("rId110")

        ' the root element of chartPart 
        Dim chartSpace As New dc.ChartSpace()
        chartSpace.Append(New dc.EditingLanguage() With {.Val = "en-us"})

        ' Create Chart 
        Dim chart As New dc.Chart()
        chart.Append(New dc.AutoTitleDeleted() With {.Val = True})

        ' Define the 3D view
        Dim view3D As New dc.View3D()
        view3D.Append(New dc.RotateX() With { _
             .Val = 30 _
        })
        view3D.Append(New dc.RotateY() With { _
             .Val = 0 _
        })

        ' Intiliazes a new instance of the PlotArea class
        Dim plotArea As New dc.PlotArea()
        plotArea.Append(New dc.Layout())

        ' the type of Chart 
        Dim pie3DChart As New dc.Pie3DChart()
        pie3DChart.Append(New dc.VaryColors() With { _
             .Val = True _
        })
        Dim pieChartSers As New dc.PieChartSeries()
        pieChartSers.Append(New dc.Index() With { _
            .Val = 0UI _
        })
        pieChartSers.Append(New dc.Order() With { _
             .Val = 0UI _
        })
        Dim seriesText As New dc.SeriesText()
        seriesText.Append(New dc.NumericValue() With { _
             .Text = "Series" _
        })

        Dim rowcount As UInteger = 0
        Dim count As UInteger = UInt32.Parse(chartList.Count.ToString())
        Dim endCell As String = (count + 1).ToString()
        Dim chartShapePros As New dc.ChartShapeProperties()

        ' Define cell for lable information
        Dim cateAxisData As New dc.CategoryAxisData()
        Dim stringRef As New dc.StringReference()
        stringRef.Append(New dc.Formula() With { _
             .Text = "Main!$A$2:$A$" & endCell _
        })
        Dim stringCache As New dc.StringCache()
        stringCache.Append(New dc.PointCount() With { _
             .Val = count _
        })

        ' Define cells for value information
        Dim values As New dc.Values()
        Dim numRef As New dc.NumberReference()
        numRef.Append(New dc.Formula() With { _
             .Text = "Main!$B$2:$B$" & endCell _
        })

        Dim numCache As New dc.NumberingCache()
        numCache.Append(New dc.FormatCode() With { _
             .Text = "General" _
        })
        numCache.Append(New dc.PointCount() With { _
             .Val = count _
        })

        ' Fill data for chart
        For Each item As ChartSubArea In chartList
            If count = 0 Then
                chartShapePros.Append(New d.SolidFill(New d.SchemeColor() With { _
                     .Val = item.Color _
                }))
                pieChartSers.Append(chartShapePros)
            Else
                Dim dataPoint As New dc.DataPoint()
                dataPoint.Append(New dc.Index() With { _
                     .Val = rowcount _
                })
                chartShapePros = New dc.ChartShapeProperties()
                chartShapePros.Append(New d.SolidFill(New d.SchemeColor() With { _
                     .Val = item.Color _
                }))
                dataPoint.Append(chartShapePros)
                pieChartSers.Append(dataPoint)
            End If

            Dim stringPoint As New dc.StringPoint() With { _
                 .Index = rowcount _
            }
            stringPoint.Append(New dc.NumericValue() With { _
                 .Text = item.Label _
            })
            stringCache.Append(stringPoint)

            Dim numericPoint As New dc.NumericPoint() With { _
                 .Index = rowcount _
            }
            numericPoint.Append(New dc.NumericValue() With { _
                 .Text = item.Value _
            })
            numCache.Append(numericPoint)
            rowcount += 1
        Next

        ' Create c:cat and c:val element 
        stringRef.Append(stringCache)
        cateAxisData.Append(stringRef)
        numRef.Append(numCache)
        values.Append(numRef)

        ' Append c:cat and c:val to the end of c:ser element
        pieChartSers.Append(cateAxisData)
        pieChartSers.Append(values)

        ' Append c:ser to the end of c:pie3DChart element
        pie3DChart.Append(pieChartSers)

        ' Append c:pie3DChart to the end of s:plotArea element
        plotArea.Append(pie3DChart)

        ' create child elements of the c:legend element
        Dim legend As New dc.Legend()
        legend.Append(New dc.LegendPosition() With { _
             .Val = LegendPositionValues.Right
        })
        Dim overlay As New dc.Overlay() With { _
             .Val = False _
        }
        legend.Append(Overlay)

        Dim textPros As dc.TextProperties = New TextProperties()
        textPros.Append(New d.BodyProperties())
        textPros.Append(New d.ListStyle())

        Dim paragraph As New d.Paragraph()
        Dim paraPros As New d.ParagraphProperties()
        Dim defaultParaPros As New d.DefaultParagraphProperties()
        defaultParaPros.Append(New d.LatinFont() With { _
             .Typeface = "Arial", _
             .PitchFamily = 34, _
             .CharacterSet = 0 _
        })
        defaultParaPros.Append(New d.ComplexScriptFont() With { _
             .Typeface = "Arial", _
             .PitchFamily = 34, _
             .CharacterSet = 0 _
        })
        paraPros.Append(defaultParaPros)
        paragraph.Append(paraPros)
        paragraph.Append(New d.EndParagraphRunProperties() With { _
             .Language = "en-Us" _
        })

        textPros.Append(paragraph)
        legend.Append(textPros)

        ' Append c:view3D, c:plotArea and c:legend elements to the end of c:chart element
        chart.Append(view3D)
        chart.Append(plotArea)
        chart.Append(legend)

        ' Append the c:chart element to the end of c:chartSpace element
        chartSpace.Append(chart)

        ' Create c:spPr Elements and fill the child elements of it
        chartShapePros = New dc.ChartShapeProperties()
        Dim outline As New d.Outline()
        outline.Append(New d.NoFill())
        chartShapePros.Append(outline)

        ' Append c:spPr element to the end of c:chartSpace element
        chartSpace.Append(chartShapePros)

        chartPart.ChartSpace = chartSpace

        ' Generate content of the MainDocumentPart
        GeneratePartContent(mainPart)
    End Sub

    ' Generate content of the MainDocumentPart
    Public Sub GeneratePartContent(mainPart As MainDocumentPart)
        Dim paragraph As New Paragraph() With { _
             .RsidParagraphAddition = "00C75AEB", _
             .RsidRunAdditionDefault = "000F3EFF" _
        }

        ' Create a new run that has an inline drawing object
        Dim run As New Run()
        Dim drawing As New Drawing()

        Dim inline As New dw.Inline()
        inline.Append(New dw.Extent() With { _
             .Cx = 5274310L, _
             .Cy = 3076575L _
        })
        Dim docPros As New dw.DocProperties() With { _
             .Id = CType(1UI, UInt32Value), _
             .Name = "Chart 1" _
        }
        inline.Append(docPros)

        Dim g As New d.Graphic()
        Dim graphicData As New d.GraphicData() With { _
             .Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" _
        }
        Dim chartReference As dc.ChartReference = New ChartReference() With { _
             .Id = "rId110" _
        }
        graphicData.Append(ChartReference)
        g.Append(graphicData)
        inline.Append(g)
        drawing.Append(inline)
        run.Append(drawing)
        Paragraph.Append(run)

        mainPart.Document.Body.Append(Paragraph)
    End Sub

#Region "IDisposable interface"

    Public Sub Dispose()
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        ' Protect from being called multiple times.
        If disposed Then
            Return
        End If

        If disposing Then
            ' Clean up all managed resources.
            If Me.document IsNot Nothing Then
                Me.document.Dispose()
            End If
        End If

        disposed = True
    End Sub
#End Region

    Public Sub Dispose1() Implements IDisposable.Dispose

    End Sub
End Class
