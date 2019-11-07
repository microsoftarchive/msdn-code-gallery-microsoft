# How to add a table with rows to PowerPoint using open xml
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Office
- Office Development
## Topics
- PowerPoint
- Open XML
## Updated
- 06/16/2015
## Description

<h1>How to add a table with rows to PowerPoint using Open Xml (CSOpenXmlCreateTable)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to create a table with rows into PowerPoint document using Open XML SDK.</p>
<p class="MsoNormal">Using Open XML SDK 2.0, you can create document structure and content using strongly-typed classed that correspond to PresentationML elements, you can find these classes in the
<a href="http://msdn.microsoft.com/en-us/library/office/documentformat.openxml.presentation(v=office.14).aspx">
DocumentFormat.OpenXml.Presentation</a> namespace.</p>
<h2 class="MsoNormal">Video</h2>
<p><a href="http://channel9.msdn.com/Blogs/OneCode/How-to-add-a-table-with-rows-to-PowerPoint-using-OpenXML" target="_blank"><img id="139002" src="139002-how%20to%20add%20a%20table%20with%20rows%20to%20powerpoint%20using%20openxml%20%20%20channel%209.png" alt="" width="640" height="350" style="border:1px solid black"></a></p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before building the sample, please make sure you have installed
<a href="http://www.microsoft.com/en-us/download/details.aspx?id=5124">Open XML SDK 2.0 for Microsoft Office.</a></p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1. Open &quot;CSOpenXmlCreateTable.sln&quot; file and then click Ctrl&#43;F5 to run the project. You will see the following Windows Form:</p>
<p class="MsoNormal"><span><img src="83512-image.png" alt="" width="363" height="262" align="middle">
</span></p>
<p class="MsoNormal">Step2. Click &quot;Open&quot; button to choose an existing PowerPoint file on client machine. When click &quot;Open&quot; button, you will see the open dialog as below:</p>
<p class="MsoNormal"><span><img src="83513-image.png" alt="" width="576" height="465" align="middle">
</span></p>
<p class="MsoNormal">Step3. Choose an existing PowerPoint file and then the &quot;Insert Table&quot; button will be enable, you can click the button to insert a table with 2 rows in the last slide of the PowerPoint file.</p>
<p class="MsoNormal">Step4. If Insert operation is successful, you will get the successful message as below:</p>
<p class="MsoNormal"><span><img src="83514-image.png" alt="" width="576" height="253" align="middle">
</span></p>
<p class="MsoNormal">You also can open the PowerPoint file and check whether there is a table in the ppt file.</p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal"><span>&nbsp;</span><span> <img src="83515-image.png" alt="" width="576" height="416" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step1. Create a Windows Forms Application project in Visual Studio.</p>
<p class="MsoNormal">Step2. Add the Open Xml references to the project. To reference the Open XML, right click the project and click &quot;Add Reference&hellip;&quot; button. In the Add Reference dialog, navigate to the .Net tab, find DocumentFormat.OpenXml and WindowsBase,
 then click &quot;Ok&quot; button.</p>
<p class="MsoNormal">Step3. Import Open XML namespace in Program class.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using P14 = DocumentFormat.OpenXml.Office2010.PowerPoint;

</pre>
<pre id="codePreview" class="csharp">using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using P14 = DocumentFormat.OpenXml.Office2010.PowerPoint;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step4. Generate a table with rows using strongly-typed classed in Open XML SDK.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">/// &lt;summary&gt;
    /// Generate Table as below order:
    /// a:tbl(Table) -&gt;a:tr(TableRow)-&gt;a:tc(TableCell)
    /// We can return TableCell object with CreateTextCell method
    /// and Append the TableCell object to TableRow 
    /// &lt;/summary&gt;
    /// &lt;returns&gt;Table Object&lt;/returns&gt;
    private static A.Table GenerateTable()
    {
        string[,] tableSources=new string[,] {{&quot;name&quot;,&quot;age&quot;}, {&quot;Tom&quot;,&quot;25&quot;}};
       
        // Declare and instantiate table 
        A.Table table = new A.Table();


        // Specify the required table properties for the table
        A.TableProperties tableProperties = new A.TableProperties() { FirstRow = true, BandRow = true };
        A.TableStyleId tableStyleId = new A.TableStyleId();
        tableStyleId.Text = &quot;{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}&quot;;


        tableProperties.Append(tableStyleId);


        // Declare and instantiate tablegrid and colums
        A.TableGrid tableGrid1 = new A.TableGrid();
        A.GridColumn gridColumn1 = new A.GridColumn() { Width = 3048000L };
        A.GridColumn gridColumn2 = new A.GridColumn() { Width = 3048000L };


        tableGrid1.Append(gridColumn1);
        tableGrid1.Append(gridColumn2);
        table.Append(tableProperties);
        table.Append(tableGrid1);
        for (int row = 0; row &lt; tableSources.GetLength(0); row&#43;&#43;)
        {
            // Instantiate the table row
            A.TableRow tableRow = new A.TableRow() { Height = 370840L };
            for (int column = 0; column &lt; tableSources.GetLength(1); column&#43;&#43;)
            {               
                tableRow.Append(CreateTextCell(tableSources.GetValue(row, column).ToString()));
            }


            table.Append(tableRow);
        } 


        return table;
    }


    /// &lt;summary&gt;
    /// Create table cell with the below order:
    /// a:tc(TableCell)-&gt;a:txbody(TextBody)-&gt;a:p(Paragraph)-&gt;a:r(Run)-&gt;a:t(Text)
    /// &lt;/summary&gt;
    /// &lt;param name=&quot;text&quot;&gt;Inserted Text in Cell&lt;/param&gt;
    /// &lt;returns&gt;Return TableCell object&lt;/returns&gt;
    private static A.TableCell CreateTextCell(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            text = string.Empty;
        }


        // Declare and instantiate the table cell 
        // Create table cell with the below order:
        // a:tc(TableCell)-&gt;a:txbody(TextBody)-&gt;a:p(Paragraph)-&gt;a:r(Run)-&gt;a:t(Text)
        A.TableCell tableCell = new A.TableCell();


        //  Declare and instantiate the text body
        A.TextBody textBody = new A.TextBody();
        A.BodyProperties bodyProperties = new A.BodyProperties();
        A.ListStyle listStyle = new A.ListStyle();


        A.Paragraph paragraph = new A.Paragraph();
        A.Run run = new A.Run();
        A.RunProperties runProperties = new A.RunProperties() { Language = &quot;en-US&quot;, Dirty = false, SmartTagClean = false };
        A.Text text2 = new A.Text();
        text2.Text = text;
        run.Append(runProperties);
        run.Append(text2);
        A.EndParagraphRunProperties endParagraphRunProperties = new A.EndParagraphRunProperties() { Language = &quot;en-US&quot;, Dirty = false };


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

</pre>
<pre id="codePreview" class="csharp">/// &lt;summary&gt;
    /// Generate Table as below order:
    /// a:tbl(Table) -&gt;a:tr(TableRow)-&gt;a:tc(TableCell)
    /// We can return TableCell object with CreateTextCell method
    /// and Append the TableCell object to TableRow 
    /// &lt;/summary&gt;
    /// &lt;returns&gt;Table Object&lt;/returns&gt;
    private static A.Table GenerateTable()
    {
        string[,] tableSources=new string[,] {{&quot;name&quot;,&quot;age&quot;}, {&quot;Tom&quot;,&quot;25&quot;}};
       
        // Declare and instantiate table 
        A.Table table = new A.Table();


        // Specify the required table properties for the table
        A.TableProperties tableProperties = new A.TableProperties() { FirstRow = true, BandRow = true };
        A.TableStyleId tableStyleId = new A.TableStyleId();
        tableStyleId.Text = &quot;{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}&quot;;


        tableProperties.Append(tableStyleId);


        // Declare and instantiate tablegrid and colums
        A.TableGrid tableGrid1 = new A.TableGrid();
        A.GridColumn gridColumn1 = new A.GridColumn() { Width = 3048000L };
        A.GridColumn gridColumn2 = new A.GridColumn() { Width = 3048000L };


        tableGrid1.Append(gridColumn1);
        tableGrid1.Append(gridColumn2);
        table.Append(tableProperties);
        table.Append(tableGrid1);
        for (int row = 0; row &lt; tableSources.GetLength(0); row&#43;&#43;)
        {
            // Instantiate the table row
            A.TableRow tableRow = new A.TableRow() { Height = 370840L };
            for (int column = 0; column &lt; tableSources.GetLength(1); column&#43;&#43;)
            {               
                tableRow.Append(CreateTextCell(tableSources.GetValue(row, column).ToString()));
            }


            table.Append(tableRow);
        } 


        return table;
    }


    /// &lt;summary&gt;
    /// Create table cell with the below order:
    /// a:tc(TableCell)-&gt;a:txbody(TextBody)-&gt;a:p(Paragraph)-&gt;a:r(Run)-&gt;a:t(Text)
    /// &lt;/summary&gt;
    /// &lt;param name=&quot;text&quot;&gt;Inserted Text in Cell&lt;/param&gt;
    /// &lt;returns&gt;Return TableCell object&lt;/returns&gt;
    private static A.TableCell CreateTextCell(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            text = string.Empty;
        }


        // Declare and instantiate the table cell 
        // Create table cell with the below order:
        // a:tc(TableCell)-&gt;a:txbody(TextBody)-&gt;a:p(Paragraph)-&gt;a:r(Run)-&gt;a:t(Text)
        A.TableCell tableCell = new A.TableCell();


        //  Declare and instantiate the text body
        A.TextBody textBody = new A.TextBody();
        A.BodyProperties bodyProperties = new A.BodyProperties();
        A.ListStyle listStyle = new A.ListStyle();


        A.Paragraph paragraph = new A.Paragraph();
        A.Run run = new A.Run();
        A.RunProperties runProperties = new A.RunProperties() { Language = &quot;en-US&quot;, Dirty = false, SmartTagClean = false };
        A.Text text2 = new A.Text();
        text2.Text = text;
        run.Append(runProperties);
        run.Append(text2);
        A.EndParagraphRunProperties endParagraphRunProperties = new A.EndParagraphRunProperties() { Language = &quot;en-US&quot;, Dirty = false };


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

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step5. Append the Table object to the Slide of the PowerPoint file.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">/// &lt;summary&gt;
      /// Append Table into Last Slide
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;presentationDocument&quot;&gt;&lt;/param&gt;
      private static void CreateTableInLastSlide(PresentationDocument presentationDocument)
      {
          // Get the presentation Part of the presentation document
          PresentationPart presentationPart = presentationDocument.PresentationPart;


          // Get the Slide Id collection of the presentation document
          var slideIdList = presentationPart.Presentation.SlideIdList;


          // Get all Slide Part of the presentation document
          var list = slideIdList.ChildElements
                      .Cast&lt;SlideId&gt;()
                      .Select(x =&gt; presentationPart.GetPartById(x.RelationshipId))
                      .Cast&lt;SlidePart&gt;();


          // Get the last Slide Part of the presentation document
          var tableSlidePart = (SlidePart)list.Last();


          // Declare and instantiate the graphic Frame of the new slide
          GraphicFrame graphicFrame = tableSlidePart.Slide.CommonSlideData.ShapeTree.AppendChild(new GraphicFrame());
         
          // Specify the required Frame properties of the graphicFrame
          ApplicationNonVisualDrawingPropertiesExtension applicationNonVisualDrawingPropertiesExtension = new ApplicationNonVisualDrawingPropertiesExtension() { Uri = &quot;{D42A27DB-BD31-4B8C-83A1-F6EECF244321}&quot; };
          P14.ModificationId modificationId1 = new P14.ModificationId() { Val = 3229994563U };
          modificationId1.AddNamespaceDeclaration(&quot;p14&quot;, &quot;http://schemas.microsoft.com/office/powerpoint/2010/main&quot;);
          applicationNonVisualDrawingPropertiesExtension.Append(modificationId1);
          graphicFrame.NonVisualGraphicFrameProperties = new NonVisualGraphicFrameProperties
          (new NonVisualDrawingProperties() { Id = 5, Name = &quot;table 1&quot; },
          new NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoGrouping = true }),
          new ApplicationNonVisualDrawingProperties(new ApplicationNonVisualDrawingPropertiesExtensionList(applicationNonVisualDrawingPropertiesExtension)));


          graphicFrame.Transform = new Transform(new A.Offset() { X = 1650609L, Y = 4343400L }, new A.Extents() { Cx = 6096000L, Cy = 741680L });
          
          // Specify the Griaphic of the graphic Frame
          graphicFrame.Graphic = new A.Graphic(new A.GraphicData(GenerateTable()) { Uri = &quot;http://schemas.openxmlformats.org/drawingml/2006/table&quot; });
          presentationPart.Presentation.Save();
      }

</pre>
<pre id="codePreview" class="csharp">/// &lt;summary&gt;
      /// Append Table into Last Slide
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;presentationDocument&quot;&gt;&lt;/param&gt;
      private static void CreateTableInLastSlide(PresentationDocument presentationDocument)
      {
          // Get the presentation Part of the presentation document
          PresentationPart presentationPart = presentationDocument.PresentationPart;


          // Get the Slide Id collection of the presentation document
          var slideIdList = presentationPart.Presentation.SlideIdList;


          // Get all Slide Part of the presentation document
          var list = slideIdList.ChildElements
                      .Cast&lt;SlideId&gt;()
                      .Select(x =&gt; presentationPart.GetPartById(x.RelationshipId))
                      .Cast&lt;SlidePart&gt;();


          // Get the last Slide Part of the presentation document
          var tableSlidePart = (SlidePart)list.Last();


          // Declare and instantiate the graphic Frame of the new slide
          GraphicFrame graphicFrame = tableSlidePart.Slide.CommonSlideData.ShapeTree.AppendChild(new GraphicFrame());
         
          // Specify the required Frame properties of the graphicFrame
          ApplicationNonVisualDrawingPropertiesExtension applicationNonVisualDrawingPropertiesExtension = new ApplicationNonVisualDrawingPropertiesExtension() { Uri = &quot;{D42A27DB-BD31-4B8C-83A1-F6EECF244321}&quot; };
          P14.ModificationId modificationId1 = new P14.ModificationId() { Val = 3229994563U };
          modificationId1.AddNamespaceDeclaration(&quot;p14&quot;, &quot;http://schemas.microsoft.com/office/powerpoint/2010/main&quot;);
          applicationNonVisualDrawingPropertiesExtension.Append(modificationId1);
          graphicFrame.NonVisualGraphicFrameProperties = new NonVisualGraphicFrameProperties
          (new NonVisualDrawingProperties() { Id = 5, Name = &quot;table 1&quot; },
          new NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoGrouping = true }),
          new ApplicationNonVisualDrawingProperties(new ApplicationNonVisualDrawingPropertiesExtensionList(applicationNonVisualDrawingPropertiesExtension)));


          graphicFrame.Transform = new Transform(new A.Offset() { X = 1650609L, Y = 4343400L }, new A.Extents() { Cx = 6096000L, Cy = 741680L });
          
          // Specify the Griaphic of the graphic Frame
          graphicFrame.Graphic = new A.Graphic(new A.GraphicData(GenerateTable()) { Uri = &quot;http://schemas.openxmlformats.org/drawingml/2006/table&quot; });
          presentationPart.Presentation.Save();
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal">DocumentFormat.OpenXml.Presentation Namespace:</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/office/documentformat.openxml.presentation(v=office.14).aspx">http://msdn.microsoft.com/en-us/library/office/documentformat.openxml.presentation(v=office.14).aspx</a></p>
<p class="MsoNormal">Open XML for Office developers:</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/office/bb265236">http://msdn.microsoft.com/en-us/office/bb265236</a></p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal">&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
