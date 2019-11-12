/****************************** Module Header ******************************\
* Module Name:  InsertTableToPowerPoint.cs
* Project:      CSOpenXmlCreateTable
* Copyright(c)  Microsoft Corporation.
* 
* The Class is used to Create Table with rows in PowerPoint using Open XML SDK.
* We can get the xml struct of the PowerPoint file using Open XML SDK 2.0 Productivity Tool  
* Using Productivity Tool, We can find that we can create table as the below order:
* a:tbl(Table)->a:tr(TableRow)->a:tc(TableCell)->a:txbody(TextBody)->a:p(Paragraph)->a:r(Run)->a:t(Text)
* 
* By using the sample code you can add a new slide to an existing presentation
* Firstly,We can select an existing ppt file with at least one slide
* Secondly, Click "Insert table" button to insert table into last slide of the PowerPoint file
* At last, Open PowerPoint file on client machine and check whether there is a table with rows 
* If there is no error occured, we can see a new table with two rows and two columns in last slide.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using P14 = DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace CSOpenXmlCreateTable
{
    public class InsertTableToPowerPoint
    {
        /// <summary>
        /// Append Table into Last Slide
        /// </summary>
        /// <param name="presentationDocument"></param>
        public static void CreateTableInLastSlide(PresentationDocument presentationDocument)
        {
            // Get the presentation Part of the presentation document
            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Get the Slide Id collection of the presentation document
            var slideIdList = presentationPart.Presentation.SlideIdList;

            if (slideIdList == null)
            {
                throw new NullReferenceException("The number of slide is empty, please select a ppt with a slide at least again");
            }

            // Get all Slide Part of the presentation document
            var list = slideIdList.ChildElements
                        .Cast<SlideId>()
                        .Select(x => presentationPart.GetPartById(x.RelationshipId))
                        .Cast<SlidePart>();

            // Get the last Slide Part of the presentation document
            var tableSlidePart = (SlidePart)list.Last();

            // Declare and instantiate the graphic Frame of the new slide
            GraphicFrame graphicFrame = tableSlidePart.Slide.CommonSlideData.ShapeTree.AppendChild(new GraphicFrame());

            // Specify the required Frame properties of the graphicFrame
            ApplicationNonVisualDrawingPropertiesExtension applicationNonVisualDrawingPropertiesExtension = new ApplicationNonVisualDrawingPropertiesExtension() { Uri = "{D42A27DB-BD31-4B8C-83A1-F6EECF244321}" };
            P14.ModificationId modificationId1 = new P14.ModificationId() { Val = 3229994563U };
            modificationId1.AddNamespaceDeclaration("p14", "http://schemas.microsoft.com/office/powerpoint/2010/main");
            applicationNonVisualDrawingPropertiesExtension.Append(modificationId1);
            graphicFrame.NonVisualGraphicFrameProperties = new NonVisualGraphicFrameProperties
            (new NonVisualDrawingProperties() { Id = 5, Name = "table 1" },
            new NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoGrouping = true }),
            new ApplicationNonVisualDrawingProperties(new ApplicationNonVisualDrawingPropertiesExtensionList(applicationNonVisualDrawingPropertiesExtension)));

            graphicFrame.Transform = new Transform(new A.Offset() { X = 1650609L, Y = 4343400L }, new A.Extents() { Cx = 6096000L, Cy = 741680L });

            // Specify the Griaphic of the graphic Frame
            graphicFrame.Graphic = new A.Graphic(new A.GraphicData(GenerateTable()) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/table" });
            presentationPart.Presentation.Save();
        }

        /// <summary>
        /// Generate Table as below order:
        /// a:tbl(Table) ->a:tr(TableRow)->a:tc(TableCell)
        /// We can return TableCell object with CreateTextCell method
        /// and Append the TableCell object to TableRow 
        /// </summary>
        /// <returns>Table Object</returns>
        private static A.Table GenerateTable()
        {
            string[,] tableSources = new string[,] { { "name", "age" }, { "Tom", "25" } };

            // Declare and instantiate table 
            A.Table table = new A.Table();

            // Specify the required table properties for the table
            A.TableProperties tableProperties = new A.TableProperties() { FirstRow = true, BandRow = true };
            A.TableStyleId tableStyleId = new A.TableStyleId();
            tableStyleId.Text = "{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}";

            tableProperties.Append(tableStyleId);

            // Declare and instantiate tablegrid and colums
            A.TableGrid tableGrid1 = new A.TableGrid();
            A.GridColumn gridColumn1 = new A.GridColumn() { Width = 3048000L };
            A.GridColumn gridColumn2 = new A.GridColumn() { Width = 3048000L };

            tableGrid1.Append(gridColumn1);
            tableGrid1.Append(gridColumn2);
            table.Append(tableProperties);
            table.Append(tableGrid1);
            for (int row = 0; row < tableSources.GetLength(0); row++)
            {
                // Instantiate the table row
                A.TableRow tableRow = new A.TableRow() { Height = 370840L };
                for (int column = 0; column < tableSources.GetLength(1); column++)
                {
                    tableRow.Append(CreateTextCell(tableSources.GetValue(row, column).ToString()));
                }

                table.Append(tableRow);
            }

            return table;
        }

        /// <summary>
        /// Create table cell with the below order:
        /// a:tc(TableCell)->a:txbody(TextBody)->a:p(Paragraph)->a:r(Run)->a:t(Text)
        /// </summary>
        /// <param name="text">Inserted Text in Cell</param>
        /// <returns>Return TableCell object</returns>
        private static A.TableCell CreateTextCell(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = string.Empty;
            }

            // Declare and instantiate the table cell 
            // Create table cell with the below order:
            // a:tc(TableCell)->a:txbody(TextBody)->a:p(Paragraph)->a:r(Run)->a:t(Text)
            A.TableCell tableCell = new A.TableCell();

            //  Declare and instantiate the text body
            A.TextBody textBody = new A.TextBody();
            A.BodyProperties bodyProperties = new A.BodyProperties();
            A.ListStyle listStyle = new A.ListStyle();

            A.Paragraph paragraph = new A.Paragraph();
            A.Run run = new A.Run();
            A.RunProperties runProperties = new A.RunProperties() { Language = "en-US", Dirty = false, SmartTagClean = false };
            A.Text text2 = new A.Text();
            text2.Text = text;
            run.Append(runProperties);
            run.Append(text2);
            A.EndParagraphRunProperties endParagraphRunProperties = new A.EndParagraphRunProperties() { Language = "en-US", Dirty = false };

            paragraph.Append(run);
            paragraph.Append(endParagraphRunProperties);
            textBody.Append(bodyProperties);
            textBody.Append(listStyle);
            textBody.Append(paragraph);

            A.TableCellProperties tableCellProperties = new A.TableCellProperties();
            tableCell.Append(textBody);
            tableCell.Append(tableCellProperties);

            return tableCell;
        }
    }
}
