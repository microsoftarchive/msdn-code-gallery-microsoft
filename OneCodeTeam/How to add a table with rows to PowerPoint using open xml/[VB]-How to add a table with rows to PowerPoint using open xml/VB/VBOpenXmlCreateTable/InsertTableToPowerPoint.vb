'/****************************** Module Header ******************************\
' Module Name:  InsertTableToPowerPoint.vb
' Project:      VBOpenXmlCreateTable
' Copyright(c)  Microsoft Corporation.
' 
' The Class is used to Create Table with rows in PowerPoint using Open XML SDK.
' We can get the xml struct of the PowerPoint file using Open XML SDK 2.0 Productivity Tool  
' Using Productivity Tool, We can find that we can create table as the below order:
' a:tbl(Table)->a:tr(TableRow)->a:tc(TableCell)->a:txbody(TextBody)->a:p(Paragraph)->a:r(Run)->a:t(Text)
' 
' By using the sample code you can add a new slide to an existing presentation
' Firstly,We can select an existing ppt file with at least one slide
' Secondly, Click "Insert table" button to insert table into last slide of the PowerPoint file
' At last, Open PowerPoint file on client machine and check whether there is a table with rows 
' If there is no error occured, we can see a new table with two rows and two columns in last slide.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/


Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Presentation
Imports A = DocumentFormat.OpenXml.Drawing
Imports P14 = DocumentFormat.OpenXml.Office2010.PowerPoint

Public Class InsertTableToPowerPoint

    ''' <summary>
    ''' Append Table into Last Slide
    ''' </summary>
    ''' <param name="presentationDocument"></param>
    Public Shared Sub CreateTableInLastSlide(presentationDocument As PresentationDocument)
        ' Get the presentation Part of the presentation document
        Dim presentationPart As PresentationPart = presentationDocument.PresentationPart

        ' Get the Slide Id collection of the presentation document
        Dim slideIdList = presentationPart.Presentation.SlideIdList

        If slideIdList Is Nothing Then
            Throw New NullReferenceException("The number of slide is empty, please select a ppt with a slide at least again")
        End If

        ' Get all Slide Part of the presentation document
        Dim list = slideIdList.ChildElements.Cast(Of SlideId)().[Select](Function(x) presentationPart.GetPartById(x.RelationshipId)).Cast(Of SlidePart)()

        ' Get the last Slide Part of the presentation document
        Dim tableSlidePart = DirectCast(list.Last(), SlidePart)

        ' Declare and instantiate the graphic Frame of the new slide
        Dim graphicFrame As GraphicFrame = tableSlidePart.Slide.CommonSlideData.ShapeTree.AppendChild(New GraphicFrame())

        ' Specify the required Frame properties of the graphicFrame
        Dim applicationNonVisualDrawingPropertiesExtension As New ApplicationNonVisualDrawingPropertiesExtension() With { _
            .Uri = "{D42A27DB-BD31-4B8C-83A1-F6EECF244321}"
        }
        Dim modificationId1 As New P14.ModificationId() With { _
         .Val = 3229994563UI
        }

        modificationId1.AddNamespaceDeclaration("p14", "http://schemas.microsoft.com/office/powerpoint/2010/main")
        applicationNonVisualDrawingPropertiesExtension.Append(modificationId1)
        graphicFrame.NonVisualGraphicFrameProperties = New NonVisualGraphicFrameProperties(New NonVisualDrawingProperties() With { _
         .Id = 5, _
         .Name = "table 1" _
        }, New NonVisualGraphicFrameDrawingProperties(New A.GraphicFrameLocks() With { _
         .NoGrouping = True _
        }), New ApplicationNonVisualDrawingProperties(New ApplicationNonVisualDrawingPropertiesExtensionList(applicationNonVisualDrawingPropertiesExtension)))

        graphicFrame.Transform = New Transform(New A.Offset() With { _
         .X = 1650609L, _
         .Y = 4343400L _
        }, New A.Extents() With { _
         .Cx = 6096000L, _
         .Cy = 741680L _
        })

        ' Specify the Griaphic of the graphic Frame
        graphicFrame.Graphic = New A.Graphic(New A.GraphicData(GenerateTable()) With { _
         .Uri = "http://schemas.openxmlformats.org/drawingml/2006/table" _
        })
        presentationPart.Presentation.Save()
    End Sub

    ''' <summary>
    ''' Generate Table as below order:
    ''' a:tbl(Table) ->a:tr(TableRow)->a:tc(TableCell)
    ''' We can return TableCell object with CreateTextCell method
    ''' and Append the TableCell object to TableRow 
    ''' </summary>
    ''' <returns>Table Object</returns>
    Private Shared Function GenerateTable() As A.Table
        Dim tableSources As String(,) = New String(,) {{"name", "age"}, {"Tom", "25"}}

        ' Declare and instantiate table 
        Dim table As New A.Table()

        ' Specify the required table properties for the table
        Dim tableProperties As New A.TableProperties() With { _
          .FirstRow = True, _
          .BandRow = True _
        }

        Dim tableStyleId As New A.TableStyleId()
        tableStyleId.Text = "{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}"

        tableProperties.Append(tableStyleId)

        ' Declare and instantiate tablegrid and colums
        Dim tableGrid1 As New A.TableGrid()
        Dim gridColumn1 As New A.GridColumn() With { _
         .Width = 3048000L _
        }
        Dim gridColumn2 As New A.GridColumn() With { _
         .Width = 3048000L _
        }

        tableGrid1.Append(gridColumn1)
        tableGrid1.Append(gridColumn2)
        table.Append(tableProperties)
        table.Append(tableGrid1)
        For row As Integer = 0 To tableSources.GetLength(0) - 1
            ' Instantiate the table row
            Dim tableRow As New A.TableRow() With { _
              .Height = 370840L _
            }
            For column As Integer = 0 To tableSources.GetLength(1) - 1
                tableRow.Append(CreateTextCell(tableSources.GetValue(row, column).ToString()))
            Next

            table.Append(tableRow)
        Next

        Return table
    End Function

    ''' <summary>
    ''' Create table cell with the below order:
    ''' a:tc(TableCell)->a:txbody(TextBody)->a:p(Paragraph)->a:r(Run)->a:t(Text)
    ''' </summary>
    ''' <param name="text">Inserted Text in Cell</param>
    ''' <returns>Return TableCell object</returns>
    Private Shared Function CreateTextCell(text As String) As A.TableCell
        If String.IsNullOrEmpty(text) Then
            text = String.Empty
        End If

        ' Declare and instantiate the table cell 
        ' Create table cell with the below order:
        ' a:tc(TableCell)->a:txbody(TextBody)->a:p(Paragraph)->a:r(Run)->a:t(Text)
        Dim tableCell As New A.TableCell()

        '  Declare and instantiate the text body
        Dim textBody As New A.TextBody()
        Dim bodyProperties As New A.BodyProperties()
        Dim listStyle As New A.ListStyle()

        Dim paragraph As New A.Paragraph()
        Dim run As New A.Run()
        Dim runProperties As New A.RunProperties() With { _
          .Language = "en-US", _
          .Dirty = False, _
          .SmartTagClean = False _
        }
        Dim text2 As New A.Text()
        text2.Text = text
        run.Append(runProperties)
        run.Append(text2)
        Dim endParagraphRunProperties As New A.EndParagraphRunProperties() With { _
          .Language = "en-US", _
          .Dirty = False _
        }

        paragraph.Append(run)
        paragraph.Append(endParagraphRunProperties)
        textBody.Append(bodyProperties)
        textBody.Append(listStyle)
        textBody.Append(paragraph)

        Dim tableCellProperties As New A.TableCellProperties()
        tableCell.Append(textBody)
        tableCell.Append(tableCellProperties)

        Return tableCell
    End Function

End Class
