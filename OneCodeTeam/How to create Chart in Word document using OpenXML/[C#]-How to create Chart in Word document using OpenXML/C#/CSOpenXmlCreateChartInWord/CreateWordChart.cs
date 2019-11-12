/****************************** Module Header ******************************\
* Module Name:  CreateWordChart.cs
* Project:      CSOpenXmlCreateChartInWord
* Copyright(c)  Microsoft Corporation.
* 
* The Class is used to Create Chart in Word.
* Microsoft Word *.docx is an Open XML document combining texts, stytle,grapyhics 
* and so on into a single ZIP archive. 
* The Class uses Open XML SDK API to insert Pie Chart into Word automatically. 
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using d = DocumentFormat.OpenXml.Drawing;
using dc = DocumentFormat.OpenXml.Drawing.Charts;
using dw = DocumentFormat.OpenXml.Drawing.Wordprocessing;


namespace CSOpenXmlCreateChartInWord
{
    public class CreateWordChart:IDisposable
    {
        // Specify whether the instance is disposed.
        private bool disposed = false;

        // The Word package
        private WordprocessingDocument document;
        public CreateWordChart(string wordpath)
        {
            if (string.IsNullOrEmpty(wordpath) || !System.IO.File.Exists(wordpath))
            {
                throw new Exception("The file is invalid. Please select an existing file again!");
            }

            this.document = WordprocessingDocument.Create(wordpath,WordprocessingDocumentType.Document);
        }

        // Create Chart in Word document
        public void CreateChart(List<ChartSubArea> chartList)
        {
            // Get MainDocumentPart of Document
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());

            // Create ChartPart object in Word Document
            ChartPart chartPart = mainPart.AddNewPart<ChartPart>("rId110");

            // the root element of chartPart 
            dc.ChartSpace chartSpace = new dc.ChartSpace();
            chartSpace.Append(new dc.EditingLanguage() { Val = "en-us" });
            
            // Create Chart 
            dc.Chart chart = new dc.Chart();
            chart.Append(new dc.AutoTitleDeleted() { Val=true });

            // Define the 3D view
            dc.View3D view3D = new dc.View3D();
            view3D.Append(new dc.RotateX() { Val = 30 });
            view3D.Append(new dc.RotateY() { Val = 0 });
            
            // Intiliazes a new instance of the PlotArea class
            dc.PlotArea plotArea = new dc.PlotArea();
            plotArea.Append(new dc.Layout());

            // the type of Chart 
            dc.Pie3DChart pie3DChart = new dc.Pie3DChart();
            pie3DChart.Append(new dc.VaryColors() { Val = true });
            dc.PieChartSeries pieChartSers = new dc.PieChartSeries();
            pieChartSers.Append(new dc.Index() { Val=0U});
            pieChartSers.Append(new dc.Order() { Val = 0U });
            dc.SeriesText seriesText = new dc.SeriesText();
            seriesText.Append(new dc.NumericValue() { Text = "Series" });

            uint rowcount = 0;
            uint count = UInt32.Parse(chartList.Count.ToString());
            string endCell = (count + 1).ToString();
            dc.ChartShapeProperties chartShapePros = new dc.ChartShapeProperties();
           
            // Define cell for lable information
            dc.CategoryAxisData cateAxisData = new dc.CategoryAxisData();
            dc.StringReference stringRef = new dc.StringReference();
            stringRef.Append(new dc.Formula() { Text = "Main!$A$2:$A$" + endCell });
            dc.StringCache stringCache = new dc.StringCache();
            stringCache.Append(new dc.PointCount() { Val = count });

            // Define cells for value information
            dc.Values values = new dc.Values();
            dc.NumberReference numRef = new dc.NumberReference();
            numRef.Append(new dc.Formula() { Text = "Main!$B$2:$B$" + endCell });

            dc.NumberingCache numCache = new dc.NumberingCache();
            numCache.Append(new dc.FormatCode() { Text = "General" });
            numCache.Append(new dc.PointCount() { Val = count });

            // Fill data for chart
            foreach (var item in chartList)
            {
                if (count == 0)
                {
                    chartShapePros.Append(new d.SolidFill(new d.SchemeColor() { Val = item.Color }));
                    pieChartSers.Append(chartShapePros);
                }
                else
                {         
                    dc.DataPoint dataPoint = new dc.DataPoint();
                    dataPoint.Append(new dc.Index() { Val = rowcount });
                    chartShapePros = new dc.ChartShapeProperties();
                    chartShapePros.Append(new d.SolidFill(new d.SchemeColor() { Val = item.Color }));
                    dataPoint.Append(chartShapePros);
                    pieChartSers.Append(dataPoint);
                }

                dc.StringPoint stringPoint = new dc.StringPoint() { Index = rowcount };
                stringPoint.Append(new dc.NumericValue() { Text = item.Label });
                stringCache.Append(stringPoint);

                dc.NumericPoint numericPoint = new dc.NumericPoint() { Index = rowcount };
                numericPoint.Append(new dc.NumericValue() { Text = item.Value });
                numCache.Append(numericPoint);
                rowcount++;
            }

            // Create c:cat and c:val element 
            stringRef.Append(stringCache);
            cateAxisData.Append(stringRef);
            numRef.Append(numCache);
            values.Append(numRef);

            // Append c:cat and c:val to the end of c:ser element
            pieChartSers.Append(cateAxisData);
            pieChartSers.Append(values);

            // Append c:ser to the end of c:pie3DChart element
            pie3DChart.Append(pieChartSers);

            // Append c:pie3DChart to the end of s:plotArea element
            plotArea.Append(pie3DChart);

            // create child elements of the c:legend element
            dc.Legend legend = new dc.Legend();
            legend.Append(new dc.LegendPosition() { Val = LegendPositionValues.Right });
            dc.Overlay overlay = new dc.Overlay() { Val = false };
            legend.Append(overlay);

            dc.TextProperties textPros = new TextProperties();
            textPros.Append(new d.BodyProperties());
            textPros.Append(new d.ListStyle());

            d.Paragraph paragraph = new d.Paragraph();
            d.ParagraphProperties paraPros = new d.ParagraphProperties();
            d.DefaultParagraphProperties defaultParaPros = new d.DefaultParagraphProperties();
            defaultParaPros.Append(new d.LatinFont() { Typeface = "Arial", PitchFamily = 34, CharacterSet = 0 });
            defaultParaPros.Append(new d.ComplexScriptFont() { Typeface = "Arial", PitchFamily = 34, CharacterSet = 0 });
            paraPros.Append(defaultParaPros);
            paragraph.Append(paraPros);
            paragraph.Append(new d.EndParagraphRunProperties() { Language="en-Us"});

            textPros.Append(paragraph);
            legend.Append(textPros);

            // Append c:view3D, c:plotArea and c:legend elements to the end of c:chart element
            chart.Append(view3D);
            chart.Append(plotArea);
            chart.Append(legend);

            // Append the c:chart element to the end of c:chartSpace element
            chartSpace.Append(chart);

            // Create c:spPr Elements and fill the child elements of it
            chartShapePros = new dc.ChartShapeProperties();
            d.Outline outline = new d.Outline();
            outline.Append(new d.NoFill());
            chartShapePros.Append(outline);

            // Append c:spPr element to the end of c:chartSpace element
            chartSpace.Append(chartShapePros);

            chartPart.ChartSpace = chartSpace;
            
            // Generate content of the MainDocumentPart
            GeneratePartContent(mainPart);
        }

        // Generate content of the MainDocumentPart
        public void GeneratePartContent(MainDocumentPart mainPart)
        {
            Paragraph paragraph = new Paragraph() { RsidParagraphAddition = "00C75AEB", RsidRunAdditionDefault = "000F3EFF" };

            // Create a new run that has an inline drawing object
            Run run = new Run();
            Drawing drawing = new Drawing();
          
            dw.Inline inline = new dw.Inline();
            inline.Append(new dw.Extent() { Cx = 5274310L, Cy = 3076575L });
            dw.DocProperties docPros = new dw.DocProperties() { Id = (UInt32Value)1U, Name = "Chart 1" };
            inline.Append(docPros);

            d.Graphic g = new d.Graphic();
            d.GraphicData graphicData = new d.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" };
            dc.ChartReference chartReference = new ChartReference() { Id = "rId110" };
            graphicData.Append(chartReference);
            g.Append(graphicData);
            inline.Append(g);
            drawing.Append(inline);
            run.Append(drawing);
            paragraph.Append(run);

            mainPart.Document.Body.Append(paragraph);
        }

        #region IDisposable interface

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Protect from being called multiple times.
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Clean up all managed resources.
                if (this.document != null)
                {
                    this.document.Dispose();
                }
            }

            disposed = true;
        }
        #endregion
    }
}
