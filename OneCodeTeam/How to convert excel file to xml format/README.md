# How to convert excel file to xml format
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Office
- Office Development
## Topics
- Excel
- XML
- Convert
## Updated
- 04/12/2015
## Description

<h1>How to Convert excel file to xml format (CSOpenXmlExcelToXml)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to convert excel file to xml format using Open XML SDK.</p>
<h2 class="MsoNormal">Video</h2>
<p><a href="http://channel9.msdn.com/Blogs/OneCode/How-to-convert-an-Excel-file-to-XML-format" target="_blank"><img id="136308" src="136308-how%20to%20convert%20an%20excel%20file%20to%20xml%20format%20%20%20channel%209.png" alt="" width="640" height="350" style="border:1px solid black"></a></p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before building the sample, please make sure you have installed
<a href="http://www.microsoft.com/en-us/download/details.aspx?id=5124">Open XML SDK 2.0 for Microsoft Office</a>.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step 1. Open &quot;CSOpenXmlExcelToXml.sln&quot; and click Ctrl&#43;F5 to run this project. You will see the following form:</p>
<p class="MsoNormal"><span><img src="84391-image.png" alt="" width="399" height="298" align="middle">
</span></p>
<p class="MsoNormal">Step 2. <span>You can click &quot;Browser&quot; button to select an existing excel file and then click
</span><span>&quot;</span><span>Convert</span><span>&quot; </span><span>button to convert the excel document to xml format string. If this operation is successful, the xml string will show in textbox control on form. At this moment, the &quot;Save as&quot; button will be enabled.
</span></p>
<p class="MsoNormal"><span><img src="84392-image.png" alt="" width="406" height="331" align="middle">
</span></p>
<p class="MsoNormal">Step 3. Click &quot;Save as&quot; button to save xml string as xml file on client. When click &quot;Save as&quot; button, there is &quot;Saveas&quot; dialog pops up as below:</p>
<p class="MsoNormal"><span><img src="84393-image.png" alt="" width="576" height="222" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step 1. Create &quot;WinForm Application&quot; project via Visual Studio.</p>
<p class="MsoNormal">Step 2. <span>After you have installed the Open XML SDK 2.0, add the Open xml reference to the project. To reference the Open XML, right click the project file and click the &quot;Add Reference&quot; button. In the Add Reference dialog, navigate
 to the .Net tab, find DocumentFormat.OpenXml and WindowsBase, click Ok button. </span>
</p>
<p class="MsoNormal"><span>Step 3. Create ConvertExcelToXml class to convert excel file using Open XML API.
</span></p>
<p class="MsoNormal"><span>Step 4. Import Open XML namespace into the class. </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 5. Read the data in excel to DataTable object.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">/// &lt;summary&gt;
       ///  Read Data from selected excel file into DataTable
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;filename&quot;&gt;Excel File Path&lt;/param&gt;
       /// &lt;returns&gt;&lt;/returns&gt;
       private DataTable ReadExcelFile(string filename)
       {
           // Initialize an instance of DataTable
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
                       int columnIndex = 0;
                       foreach (Cell cell in row.Descendants&lt;Cell&gt;())
                       {
                           // Get Cell Column Index
                           int cellColumnIndex = GetColumnIndex(GetColumnName(cell.CellReference));


                           if (columnIndex &lt; cellColumnIndex)
                           {
                               do
                               {
                                   temprow[columnIndex] = string.Empty;
                                   columnIndex&#43;&#43;;
                               }


                               while (columnIndex &lt; cellColumnIndex);
                           }


                           temprow[columnIndex] = GetValueOfCell(spreadsheetDocument, cell);
                           columnIndex&#43;&#43;;
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
           catch (IOException ex)
           {
               throw new IOException(ex.Message);
           }
       }


       /// &lt;summary&gt;
       ///  Get Value of Cell 
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;spreadsheetdocument&quot;&gt;SpreadSheet Document Object&lt;/param&gt;
       /// &lt;param name=&quot;cell&quot;&gt;Cell Object&lt;/param&gt;
       /// &lt;returns&gt;The Value in Cell&lt;/returns&gt;
       private static string GetValueOfCell(SpreadsheetDocument spreadsheetdocument, Cell cell)
       {
           // Get value in Cell
           SharedStringTablePart sharedString = spreadsheetdocument.WorkbookPart.SharedStringTablePart;
           if (cell.CellValue == null)
           {
               return string.Empty;
           }


           string cellValue = cell.CellValue.InnerText;
           
           // The condition that the Cell DataType is SharedString
           if (cell.DataType != null &amp;&amp; cell.DataType.Value == CellValues.SharedString)
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
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal">Step 6. Get Xml format string from DataTable object.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">/// &lt;summary&gt;
        /// Convert DataTable to Xml format
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;filename&quot;&gt;Excel File Path&lt;/param&gt;
        /// &lt;returns&gt;Xml format string&lt;/returns&gt;
        public string GetXML(string filename)
        {
  using (DataSet ds = new DataSet())
  {
      ds.Tables.Add(this.ReadExcelFile(filename));
      return ds.GetXml();
  }
        }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 7. Click &quot;Browse&quot; button to choose the existing excel file on client and then click convert button to convert the data in excel to xml format string and show the string in textbox control.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">/// &lt;summary&gt;
        ///  Open an dialog to let users select Excel file
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            // Initializes a OpenFileDialog instance 
            using (OpenFileDialog openfileDialog = new OpenFileDialog())
            {
                openfileDialog.RestoreDirectory = true;
                openfileDialog.Filter = &quot;Excel files(*.xlsx;*.xls)|*.xlsx;*.xls&quot;;


                if (openfileDialog.ShowDialog() == DialogResult.OK)
                {
                    txbExcelName.Text = openfileDialog.FileName;
                }
            }
        }


        /// &lt;summary&gt;
        ///  Convert Excel file to Xml format and view in Listbox control
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
        private void btnConvert_Click(object sender, EventArgs e)
        {
            txbXmlView.Clear();
            string excelfileName = txbExcelName.Text;


            if (string.IsNullOrEmpty(excelfileName) || !File.Exists(excelfileName))
            {
                MessageBox.Show(&quot;The Excel file is invalid! Please select a valid file.&quot;);
                return;
            }


            try
            {
                string xmlFormatstring = new ConvertExcelToXml().GetXML(excelfileName);
                if (string.IsNullOrEmpty(xmlFormatstring))
                {
                    MessageBox.Show(&quot;The content of Excel file is Empty!&quot;);
                    return;
                }


                txbXmlView.Text = xmlFormatstring;


                // If txbXmlView has text, set btnSaveAs button to be enable
                btnSaveAs.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(&quot;There is error occurs! The error message is: &quot; &#43;ex.Message);
            }
        }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 8. Click &quot;Save as&quot; button to save xml string in textbox control as xml file on client.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">/// &lt;summary&gt;
       ///  Save the XMl format string as Xml file
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
       private void btnSaveAs_Click(object sender, EventArgs e)
       {
           // Initiializes a SaveFileDialog instance 
           using (SaveFileDialog savefiledialog = new SaveFileDialog())
           {
               savefiledialog.RestoreDirectory = true;
               savefiledialog.DefaultExt = &quot;xml&quot;;
               savefiledialog.Filter = &quot;All Files(*.xml)|*.xml&quot;;
               if (savefiledialog.ShowDialog() == DialogResult.OK)
               {
                   Stream filestream = savefiledialog.OpenFile();
                   StreamWriter streamwriter = new StreamWriter(filestream);
                   streamwriter.Write(&quot;&lt;?xml version='1.0'?&gt;&quot; &#43;
                       Environment.NewLine &#43; txbXmlView.Text);
                   streamwriter.Close();
               }
           }
       }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><span><a href="http://msdn.microsoft.com/en-us/library/documentformat.openxml.spreadsheet(v=office.14).aspx">Spreadsheet Namespace</a>
</span></p>
<p class="MsoNormal"><span><a href="http://msdn.microsoft.com/en-us/office/bb265236.aspx">Open XML for Office Developers</a>
</span></p>
<p class="MsoNormal">&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
