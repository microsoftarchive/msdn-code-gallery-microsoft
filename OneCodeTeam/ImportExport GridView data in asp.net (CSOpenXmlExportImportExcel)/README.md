# Import/Export GridView data in asp.net (CSOpenXmlExportImportExcel)
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Office Development
## Topics
- GridView
- Export/Import
## Updated
- 12/25/2012
## Description

<h1>How to Import/Export GridView data in Asp.NET (CSOpenXmlExportImportExcel)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to export Data in GridView control to Excel and import excel data to GridView using Open XML SDK.
</p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before building the sample, please make sure you have installed
<a href="http://www.microsoft.com/en-us/download/details.aspx?id=5124">Open XML SDK 2.0 for Microsoft Office</a>.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step 1. Open &quot;CSOpenXmlExportImportExcel.sln&quot; and click Ctrl&#43;F5 to run this project. You will see the following web form:</p>
<p class="MsoNormal"><span style=""><img src="73491-image.png" alt="" width="417" height="314" align="middle">
</span></p>
<p class="MsoNormal">Step 2. <span style="">You can click &quot;Browser&quot; button to select an existing excel file and then click
</span><span style="">&quot;</span><span style="">Import</span><span style="">&quot;
</span><span style="">button to import the data to GridView control. If this operation is successful, the GridView control will fill the data in excel file. At this moment, the &quot;Export&quot; button will be enable.
</span></p>
<p class="MsoNormal"><span style=""><img src="73492-image.png" alt="" width="419" height="314" align="middle">
</span></p>
<p class="MsoNormal">Step 3. Click &quot;Export&quot; button to export the Data in GridView control to excel file. The file will save in the root of current project.</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step 1. Create &quot;Asp.NET Empty Web Application&quot; project</p>
<p class="MsoNormal">Step 2. <span style="">After you have installed the Open XML SDK 2.0, add the Open xml reference to the project. To reference the Open XML, right click the project file and click the &quot;Add Reference&quot; button. In the Add Reference
 dialog, navigate to the .Net tab, find DocumentFormat.OpenXml and WindowsBase, click Ok button.
</span></p>
<p class="MsoNormal"><span style="">Step 3. Create CreateSpreadSheetProvider class to create excel file using Open XML API.
</span></p>
<p class="MsoNormal"><span style="">Step 4. Import Open XML namespace into the class.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 5. Define ExportToExcel method to export the data in GridView control to Excel file.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
      ///  Generate an excel file with data in GridView control
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;datatable&quot;&gt;DataTable object&lt;/param&gt;
      /// &lt;param name=&quot;filepath&quot;&gt;The Path of exported excel file&lt;/param&gt;
      public void ExportToExcel(DataTable datatable, string filepath)
      {
          // Initialize an instance of  SpreadSheet Document 
          using(SpreadsheetDocument spreadsheetDocument =SpreadsheetDocument.Create(filepath,SpreadsheetDocumentType.Workbook))
          {
              CreateExcelFile(spreadsheetDocument,datatable);
          }
      }


      /// &lt;summary&gt;
      ///  Create SpreadSheet Document and Fill datas
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;spreadsheetdoc&quot;&gt;SpreadSheet Document&lt;/param&gt;
      /// &lt;param name=&quot;table&quot;&gt;DataTable Object&lt;/param&gt;
      private void CreateExcelFile(SpreadsheetDocument spreadsheetdoc, DataTable table)
      {
          // Initialize an instance of WorkbookPart
          WorkbookPart workBookPart = spreadsheetdoc.AddWorkbookPart();


          // Create WorkBook 
          CreateWorkBookPart(workBookPart);


          // Add WorkSheetPart into WorkBook
          WorksheetPart worksheetPart1 = workBookPart.AddNewPart&lt;WorksheetPart&gt;(&quot;rId1&quot;);
          CreateWorkSheetPart(worksheetPart1, table);


          // Add SharedStringTable Part into WorkBook
          SharedStringTablePart sharedStringTablePart = workBookPart.AddNewPart&lt;SharedStringTablePart&gt;(&quot;rId2&quot;);
          CreateSharedStringTablePart(sharedStringTablePart, table);


          // Add WorkbookStyles Part into Workbook
          WorkbookStylesPart workbookStylesPart = workBookPart.AddNewPart&lt;WorkbookStylesPart&gt;(&quot;rId3&quot;);
          CreateWorkBookStylesPart(workbookStylesPart);


          // Save workbook
          workBookPart.Workbook.Save();
      }


      /// &lt;summary&gt;
      /// Create an Workbook instance and add its children
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;workbookPart&quot;&gt;WorkbookPart Object&lt;/param&gt;
      private void CreateWorkBookPart(WorkbookPart workbookPart)
      {
          Workbook workbook = new Workbook();
          Sheets sheets = new Sheets();


          // Initilize an instance of Sheet Object
          Sheet sheet1 = new Sheet()
          {
              Name = &quot;Sheet1&quot;,
              SheetId = Convert.ToUInt32(1),
              Id = &quot;rId1&quot;
          };


          // Add the sheet into sheets collection
          sheets.Append(sheet1);


          CalculationProperties calculationProperties1 = new CalculationProperties()
          {
              CalculationId = (UInt32Value)111222U
          };


          // Add elements into workbook
          workbook.Append(sheets);
          workbook.Append(calculationProperties1);
          workbookPart.Workbook = workbook;
      }


      /// &lt;summary&gt;
      ///  Generates content of worksheetPart
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;worksheetPart&quot;&gt;WorksheetPart Object&lt;/param&gt;
      /// &lt;param name=&quot;table&quot;&gt;DataTable Object&lt;/param&gt;
      private void CreateWorkSheetPart(WorksheetPart worksheetPart, DataTable table)
      {
          // Initialize worksheet and set the properties
          Worksheet worksheet1 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = &quot;x14ac&quot; } };
          worksheet1.AddNamespaceDeclaration(&quot;r&quot;, &quot;http://schemas.openxmlformats.org/officeDocument/2006/relationships&quot;);
          worksheet1.AddNamespaceDeclaration(&quot;mc&quot;, &quot;http://schemas.openxmlformats.org/markup-compatibility/2006&quot;);
          worksheet1.AddNamespaceDeclaration(&quot;x14ac&quot;, &quot;http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac&quot;);
          
          SheetViews sheetViews1 = new SheetViews();


          // Initialize an instance of the sheetview class
          SheetView sheetView1 = new SheetView()
          {
              WorkbookViewId = (UInt32Value)0U
          };


          Selection selection = new Selection() { ActiveCell = &quot;A1&quot; };
          sheetView1.Append(selection);


          sheetViews1.Append(sheetView1);
          SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties()
          {
              DefaultRowHeight = 15D,
              DyDescent = 0.25D
          };


          SheetData sheetData1 = new SheetData();
          UInt32Value rowIndex = 1U;
          PageMargins pageMargins1 = new PageMargins() 
          {
              Left = 0.7D,
              Right = 0.7D, 
              Top = 0.75D,
              Bottom = 0.75D, 
              Header = 0.3D, 
              Footer = 0.3D 
          };
   
          Row row1 = new Row() 
          { 
              RowIndex = rowIndex&#43;&#43;, 
              Spans = new ListValue&lt;StringValue&gt;() { InnerText = &quot;1:3&quot; },
              DyDescent = 0.25D 
          };


          // Add columns in DataTable to columns collection of SpreadSheet Document 
          for (int columnindex = 0; columnindex &lt; table.Columns.Count; columnindex&#43;&#43;)
          {
              Cell cell = new Cell() 
              { 
                  CellReference = new CreateSpreadSheetProvider().Get(columnindex, (Convert.ToInt32((UInt32)rowIndex) - 2)), DataType = CellValues.String 
              };


              // Get Value of DataTable and append the value to cell of spreadsheet document
              CellValue cellValue = new CellValue();
              cellValue.Text = table.Columns[columnindex].ColumnName.ToString();
              cell.Append(cellValue);


              row1.Append(cell);
          }


          // Add row to sheet
          sheetData1.Append(row1);


          // Add rows in DataTable to rows collection of SpreadSheet Document 
          for (int rIndex = 0; rIndex &lt; table.Rows.Count; rIndex&#43;&#43;)
          {
              Row row = new Row()
              {
                  RowIndex = rowIndex&#43;&#43;,
                  Spans = new ListValue&lt;StringValue&gt;() { InnerText = &quot;1:3&quot; },
                  DyDescent = 0.25D
              };


              for (int cIndex = 0; cIndex &lt; table.Columns.Count; cIndex&#43;&#43;)
              {
                  Cell cell = new Cell() 
                  { 
                      CellReference = new CreateSpreadSheetProvider().Get(cIndex, (Convert.ToInt32((UInt32)rowIndex) - 2)), DataType = CellValues.String
                  };


                  CellValue cellValue = new CellValue();
                  cellValue.Text = table.Rows[rIndex][cIndex].ToString();
                  cell.Append(cellValue);
                  row.Append(cell);
              }


              // Add row to Sheet Data
              sheetData1.Append(row);
          }


          // Add elements to worksheet
          worksheet1.Append(sheetViews1);
          worksheet1.Append(sheetFormatProperties1);
          worksheet1.Append(sheetData1);
          worksheet1.Append(pageMargins1);


          worksheetPart.Worksheet = worksheet1;
      }


      /// &lt;summary&gt;
      /// Generates content of sharedStringTablePart
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sharedStringTablePart&quot;&gt;SharedStringTablePart Object&lt;/param&gt;
      /// &lt;param name=&quot;table&quot;&gt;DataTable Object&lt;/param&gt;
      private void CreateSharedStringTablePart(SharedStringTablePart sharedStringTablePart, DataTable table)
      {
          UInt32Value stringCount = Convert.ToUInt32(table.Rows.Count) &#43; Convert.ToUInt32(table.Columns.Count);


          // Initialize an instance of SharedString Table
          SharedStringTable sharedStringTable = new SharedStringTable()
          {
              Count = stringCount,
              UniqueCount = stringCount
          };


          // Add columns of DataTable to sharedString iteam
          for (int columnIndex = 0; columnIndex &lt; table.Columns.Count; columnIndex&#43;&#43;)
          {
              SharedStringItem sharedStringItem = new SharedStringItem();
              Text text = new Text();
              text.Text = table.Columns[columnIndex].ColumnName;
              sharedStringItem.Append(text);


              // Add sharedstring item to sharedstring Table
              sharedStringTable.Append(sharedStringItem);
          }


          // Add rows of DataTable to sharedString iteam
          for (int rowIndex = 0; rowIndex &lt; table.Rows.Count; rowIndex&#43;&#43;)
          {
              SharedStringItem sharedStringItem = new SharedStringItem();
              Text text = new Text();
              text.Text = table.Rows[rowIndex][0].ToString();
              sharedStringItem.Append(text);
              sharedStringTable.Append(sharedStringItem);
          }


          sharedStringTablePart.SharedStringTable = sharedStringTable;
      }
    
      /// &lt;summary&gt;
      /// Generates content of workbookStylesPart
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;workbookStylesPart&quot;&gt;WorkbookStylesPart Object&lt;/param&gt;
      private void CreateWorkBookStylesPart(WorkbookStylesPart workbookStylesPart)
      {
          // Define Style of Sheet in workbook
          Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = &quot;x14ac&quot; } };
          stylesheet1.AddNamespaceDeclaration(&quot;mc&quot;, &quot;http://schemas.openxmlformats.org/markup-compatibility/2006&quot;);
          stylesheet1.AddNamespaceDeclaration(&quot;x14ac&quot;, &quot;http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac&quot;);


          // Initialize  an instance of fonts
          Fonts fonts = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };


          // Initialize  an instance of font,fontsize,color
          Font font = new Font();     
          FontSize fontSize = new FontSize() { Val = 14D };
          Color color = new Color() { Theme = (UInt32Value)1U };
          FontName fontName = new FontName() { Val = &quot;Calibri&quot; };
          FontFamilyNumbering fontFamilyNumbering = new FontFamilyNumbering() { Val = 2 };
          FontScheme fontScheme = new FontScheme() { Val = FontSchemeValues.Minor };


          // Add elements to font
          font.Append(fontSize);
          font.Append(color);
          font.Append(fontName);
          font.Append(fontFamilyNumbering);
          font.Append(fontScheme);


          fonts.Append(font);


          // Define the StylesheetExtensionList Class. When the object is serialized out as xml, its qualified name is x:extLst
          StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();


          // Define the StylesheetExtension Class
          StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = &quot;{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}&quot; };
          stylesheetExtension1.AddNamespaceDeclaration(&quot;x14&quot;, &quot;http://schemas.microsoft.com/office/spreadsheetml/2009/9/main&quot;);
          DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles slicerStyles1 = new DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles() { DefaultSlicerStyle = &quot;SlicerStyleLight1&quot; };
          
          stylesheetExtension1.Append(slicerStyles1);
          stylesheetExtensionList1.Append(stylesheetExtension1);


          // Add elements to stylesheet
          stylesheet1.Append(fonts);
          stylesheet1.Append(stylesheetExtensionList1);
       
          // Set the style of workbook
          workbookStylesPart.Stylesheet = stylesheet1;
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 6. Add Web Form to this project and user can use the control in this Web form to import or export.</p>
<p class="MsoNormal">Step 7. Click &quot;Browser…&quot; button to choose the existing excel file on client and then click import button to import the data in excel to GridView.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
       ///  Import Excel Data into GridView Control 
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
       protected void btnImport_Click(object sender, EventArgs e)
       {
           // The condition that FileUpload control contains a file 
           if (FileUpload1.HasFile)
           {
               // Get selected file name
               string filename = FileUpload1.PostedFile.FileName;


               // Get the extension of the selected file
               string fileExten = Path.GetExtension(filename);


               // The condition that the extension is not xlsx
               if (!fileExten.Equals(&quot;.xlsx&quot;, StringComparison.OrdinalIgnoreCase))
               {
                   Response.Write(&quot;&lt;script language=\&quot;javascript\&quot;&gt;alert('The extension of selected file is incorrect ,please select again');&lt;/script&gt;&quot;);
                   return;
               }


               // Read Data in excel file
               try
               {
                   DataTable dt = ReadExcelFile(filename);


                   if (dt.Rows.Count == 0)
                   {
                       Response.Write(&quot;&lt;script language=\&quot;javascript\&quot;&gt;alert('The first sheet is empty.');&lt;/script&gt;&quot;);
                       return;
                   }


                   // Bind Datasource
                   this.gridViewTest.DataSource = dt;
                   this.gridViewTest.DataBind();


                   // Enable Export button
                   this.btnExport.Enabled = true;
               }
               catch (IOException ex)
               {
                   string exceptionmessage = ex.Message;
                   Response.Write(&quot;&lt;script language=\&quot;javascript\&quot;&gt;alert(\&quot;&quot;&#43;exceptionmessage&#43;&quot;\&quot;);&lt;/script&gt;&quot;);
               }
           }
           else
           {
               Response.Write(&quot;&lt;script language=\&quot;javascript\&quot;&gt;alert('You did not specify a file to import');&lt;/script&gt;&quot;);
           }
       }


       /// &lt;summary&gt;
       ///  Read Data from selected excel file on client
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;filename&quot;&gt;File Path&lt;/param&gt;
       /// &lt;returns&gt;&lt;/returns&gt;
       private DataTable ReadExcelFile(string filename)
       {
           // Initializate an instance of DataTable
           DataTable dt = new DataTable();


           try
           {
               // Use SpreadSheetDocument class of Open XML SDK to open excel file
               using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filename, false))
               {
                   // Get Workbook Part of Spread Sheet Document
                   WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;


                   // Get all sheets in spread sheet document 
                   IEnumerable&lt;Sheet&gt; sheetcollection = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild&lt;Sheets&gt;().Elements&lt;Sheet&gt;();


                   // Get relationship Id
                   string relationshipId = sheetcollection.First().Id.Value;


                   // Get sheet1 Part of Spread Sheet Document
                   WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(relationshipId);


                   // Get Data in Excel file
                   SheetData sheetData = worksheetPart.Worksheet.Elements&lt;SheetData&gt;().First();
                   IEnumerable&lt;Row&gt; rowcollection = sheetData.Descendants&lt;Row&gt;();


                   if (rowcollection.Count() == 0)
                   {
                       return dt;
                   }


                   // Add columns
                   foreach (Cell cell in rowcollection.ElementAt(0))
                   {
                       dt.Columns.Add(GetValueOfCell(spreadsheetDocument, cell));
                   }


                   // Add rows into DataTable
                   foreach (Row row in rowcollection)
                   {
                       DataRow temprow = dt.NewRow();
                       for (int i = 0; i &lt; row.Descendants&lt;Cell&gt;().Count(); i&#43;&#43;)
                       {
                           temprow[i] = GetValueOfCell(spreadsheetDocument, row.Descendants&lt;Cell&gt;().ElementAt(i));
                       }


                       // Add the row to DataTable
                       // the rows include header row
                       dt.Rows.Add(temprow);
                   }
               }


               // Here remove header row
               dt.Rows.RemoveAt(0);
               return dt;
           }
           catch(IOException ex)
           {
               throw new IOException(ex.Message);
           }
       }


       /// &lt;summary&gt;
       ///  Get Value in Cell 
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;spreadsheetdocument&quot;&gt;SpreadSheet Document&lt;/param&gt;
       /// &lt;param name=&quot;cell&quot;&gt;Cell in SpreadSheet Document&lt;/param&gt;
       /// &lt;returns&gt;The value in cell&lt;/returns&gt;
       private static string GetValueOfCell(SpreadsheetDocument spreadsheetdocument, Cell cell)
       {
           // Get value in Cell
           SharedStringTablePart sharedString =spreadsheetdocument.WorkbookPart.SharedStringTablePart;
           if (cell.CellValue == null)
           {
               return string.Empty;
           }


           string cellValue = cell.CellValue.InnerText;
           
           // The condition that the Cell DataType is SharedString
           if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
           {
               return sharedString.SharedStringTable.ChildElements[int.Parse(cellValue)].InnerText;
           }
           else
           {
               return cellValue;
           }
       }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 8. Click &quot;Export&quot; button to export data in GridView control to excel file.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
     ///  Export Data in GridView control to Excel file
     /// &lt;/summary&gt;
     /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
     /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
     protected void btnExport_Click(object sender, EventArgs e)
     {
         // Initialize an instance of DataTable
         DataTable dt = CreateDataTable(this.gridViewTest);


         // Save the exported file 
         string appPath = Request.PhysicalApplicationPath;
         string filename = Guid.NewGuid().ToString() &#43; &quot;.xlsx&quot;;
         string filePath = appPath&#43; filename;


         new CreateSpreadSheetProvider().ExportToExcel(dt, filePath);


         string savefilepath = &quot;Export Excel file successfully, the exported excel file is placed in: &quot; &#43; filePath;
         Response.Write(&quot;&lt;script language='javascript'&gt;alert('&quot;&#43;savefilepath.Replace(&quot;\\&quot;,&quot;\\\\&quot;)&#43;&quot;');&lt;/script&gt;&quot;);
     }


     /// &lt;summary&gt;
     ///  Create DataTable from GridView Control
     /// &lt;/summary&gt;
     /// &lt;param name=&quot;girdViewtest&quot;&gt;GridView Control&lt;/param&gt;
     /// &lt;returns&gt;An instance of DataTable Object&lt;/returns&gt;
     private DataTable CreateDataTable(GridView girdViewtest)
     {
         DataTable dt = new DataTable();


         // Get columns from GridView
         // Add value of columns to DataTable 
         for (int i = 0; i &lt; gridViewTest.HeaderRow.Cells.Count; i&#43;&#43;)
         {
             dt.Columns.Add(gridViewTest.HeaderRow.Cells[i].Text);
         }


         // Get rows from GridView
         foreach (GridViewRow row in gridViewTest.Rows)
         {
             DataRow datarow = dt.NewRow();
             for (int i = 0; i &lt; row.Cells.Count; i&#43;&#43;)
             {
                 datarow[i] = row.Cells[i].Text.Replace(&quot;&nbsp;&quot;, &quot; &quot;);
             }


             // Add rows to DataTable
             dt.Rows.Add(datarow);
         }


         return dt;
     }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<h2>More Information</h2>
<p class="MsoNormal" style=""><span style=""><a href="http://msdn.microsoft.com/en-us/library/documentformat.openxml.spreadsheet(v=office.14).aspx">Spreadsheet Namespace</a>
</span></p>
<p class="MsoNormal" style=""><span style=""><a href="http://msdn.microsoft.com/en-us/office/bb265236.aspx">Open XML for Office Developers</a>
</span></p>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.fileupload.aspx">FileUpload Class</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
