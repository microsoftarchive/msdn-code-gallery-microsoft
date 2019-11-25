# How to compare cells in excel and highlight the cells that are different
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Office
- Office Development
## Topics
- Highlight
- compare cells
## Updated
- 06/13/2013
## Description

<h1>How to compare cells in Excel and highlight the cells that are different (CSExcelCompareCells)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to compare cells in different columns of the same sheet in an excel file and compare cells in different sheets of the excel file.</p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before you run the sample, you should make sure Microsoft Office 2010 installed on your computer.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1. Open <span style="">&quot;</span>CSExcelCompareCells.sln&quot; to open the project and then press F5 on the keyboard to run the sample. The form resembles the following:</p>
<p class="MsoNormal"><span style=""><img src="84386-image.png" alt="" width="576" height="188" align="middle">
</span></p>
<p class="MsoNormal">Step2.<span style="">&nbsp; </span>Click &quot;Select&quot; button to choose an existing excel file which need to compare cells. Then you should input valid source column and target column. At last, you can click &quot;Compare Columns&quot;
 to compare cells in different columns of the same sheet<span style="">.</span> If the manipulation is successful, you will receive &quot;Compare Columns successfully, Please check in the excel file&quot; message.</p>
<p class="MsoNormal"><span style=""><img src="84387-image.png" alt="" width="576" height="272" align="middle">
</span></p>
<p class="MsoNormal">Step3. If you want to compare cells in different sheets of the excel file, you should input valid source sheet and target sheet, then Click &quot;Compare Sheets&quot; button to compare cells in different sheets. You also can receive the
 &quot;Compare Sheets successfully, Please check in the excel file&quot; message when the manipulation is successful.</p>
<p class="MsoNormal"><span style=""><img src="84388-image.png" alt="" width="576" height="299" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step1. Create Windows Forms Application from template of Visual Studio.</p>
<p class="MsoNormal">Step2. Add &quot;Microsoft.Office.Interop.Excel.dll&quot; reference to your project</p>
<p class="MsoNormal">Step3. Add a helper class to the project and named it as &quot;CompareHelper.cs&quot;. Then code the two functions by using the following code snippets:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
      /// Compare Cells in different columns in the same sheet of an excel file
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sourceCol&quot;&gt;Source Column&lt;/param&gt;
      /// &lt;param name=&quot;targetCol&quot;&gt;Taget Column&lt;/param&gt;
      /// &lt;param name=&quot;excelFile&quot;&gt;The Path of Excel File&lt;/param&gt;
      public void CompareColumns(string sourceCol, string targetCol, string excelFile)
      {
          try
          {
              // Create an instance of Microsoft Excel and make it invisible
              excelApp = new Excel.Application();
              excelApp.Visible = false;
             
              // open a Workbook and get the active Worksheet
              excelWorkbook = excelApp.Workbooks.Open(excelFile);
              excelWorkSheet1 = excelWorkbook.ActiveSheet;


              // maximum Row
              lastLine = excelWorkSheet1.UsedRange.Rows.Count;
              for (int row = 1; row &lt;= lastLine; row&#43;&#43;)
              {
                  // Compare cell value
                  if (excelWorkSheet1.Range[sourceCol &#43; row.ToString()].Value != excelWorkSheet1.Range[targetCol &#43; row.ToString()].Value)
                  {
                      excelWorkSheet1.Range[sourceCol &#43; row.ToString()].Interior.Color = 255;
                      excelWorkSheet1.Range[targetCol &#43; row.ToString()].Interior.Color = 5296274;
                  }
              }


              // Save WorkBook and close
              excelWorkbook.Save();
              excelWorkbook.Close();


              // Quit Excel Application
              excelApp.Quit();
          }
          catch
          {
              throw ;
          }
      }


      /// &lt;summary&gt;
      /// Compare Cells in different sheets of an excel file
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sourceSheetNum&quot;&gt;Source Sheet Number&lt;/param&gt;
      /// &lt;param name=&quot;targetSheetNum&quot;&gt;Target Sheet Number&lt;/param&gt;
      /// &lt;param name=&quot;excelFile&quot;&gt;Path of Excel File&lt;/param&gt;
      public void CompareSheets(int sourceSheetNum,int targetSheetNum,string excelFile)
      {
          try
          {
              // Create an instance of Microsoft Excel and make it invisible
              excelApp = new Excel.Application();
              excelApp.Visible = false;
              excelWorkbook = excelApp.Workbooks.Open(excelFile);
              excelWorkSheet1 = excelWorkbook.Sheets[sourceSheetNum];
              excelWorkSheet2 = excelWorkbook.Sheets[targetSheetNum];
              lastLine = Math.Max(
                  excelWorkSheet1.UsedRange.Rows.Count,
                  excelWorkSheet2.UsedRange.Rows.Count);
              for (int row = 1; row &lt;= lastLine; row&#43;&#43;)
              {
                  // maximum column 
                  int lastCol = Math.Max(
                      excelWorkSheet1.UsedRange.Columns.Count,
                      excelWorkSheet2.UsedRange.Columns.Count);
                  for (int column = 1; column &lt;= lastCol; column&#43;&#43;)
                  {
                      if (excelWorkSheet1.Cells[row, column].Value != excelWorkSheet2.Cells[row, column].Value)
                      {
                          ((Excel.Range)excelWorkSheet1.Cells[row, column]).Interior.Color = 255;
                          ((Excel.Range)excelWorkSheet2.Cells[row, column]).Interior.Color = 5296274;
                      }
                  }
              }


              // Save WorkBook and close
              excelWorkbook.Save();
              excelWorkbook.Close();


              // Quit Excel Application
              excelApp.Quit();
          }
          catch
          {
              throw;
          }
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step4. Design UI of main Form and then code the event handle by using the following code snippets:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
      /// Select Excel File
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
      /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
      private void btnSelect_Click(object sender, EventArgs e)
      {
          using (OpenFileDialog openFileDialog = new OpenFileDialog())
          {
              openFileDialog.Filter = &quot;Excel Files(*.xls;*.xlsx)|*.xls;*.xlsx&quot;;
              openFileDialog.Title = &quot;Select an excel file&quot;;
              if (openFileDialog.ShowDialog() == DialogResult.OK)
              {
                  txbExcelPath.Text = openFileDialog.FileName;
              }
          }
      }


      /// &lt;summary&gt;
      /// Compare Cells in different columns in the same sheet of an excel file
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
      /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
      private void btnCompareCol_Click(object sender, EventArgs e)
      {
          if (!File.Exists(txbExcelPath.Text))
          {
              MessageBox.Show(&quot;Please Select valid path of word document!&quot;, &quot;Error&quot;, MessageBoxButtons.OK, MessageBoxIcon.Error);
              return;
          }


          Regex reg = new Regex(&quot;^[A-Z]&#43;$&quot;);
          if (txbSourceCol.Text != string.Empty && txbTargetCol.Text != string.Empty)
          {
              if (reg.IsMatch(txbSourceCol.Text.ToUpper()) && reg.IsMatch(txbTargetCol.Text.ToUpper()))
              {
                  try
                  {
                      new CompareHelper().CompareColumns(
                          txbSourceCol.Text,
                          txbTargetCol.Text,
                          txbExcelPath.Text);


                      // Clean up the unmanaged Excel COM resources by explicitly
                      GC.Collect();
                      GC.WaitForPendingFinalizers();
                      GC.Collect();
                      GC.WaitForPendingFinalizers(); 
                      MessageBox.Show(&quot;Compare Columns successfully,Please check in the excel file&quot;);
                  }
                  catch(Exception ex)
                  {
                      MessageBox.Show(&quot;Exception occur, the Exception Message is: &quot; &#43; ex.Message);
                      return;
                  }
              }
              else
              {
                  MessageBox.Show(&quot;Source Column and Target Column must be letter&quot;);
                  return;
              }
          }
          else
          {
              MessageBox.Show(&quot;Please input Source Column and Target Column&quot;);
              return;
          }
      }


      /// &lt;summary&gt;
      /// Compare Cells in different sheets of an excel file
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
      /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
      private void btnCompareSheet_Click(object sender, EventArgs e)
      {
          if (!File.Exists(txbExcelPath.Text))
          {
              MessageBox.Show(&quot;Please Select valid path of word document!&quot;, &quot;Error&quot;, MessageBoxButtons.OK, MessageBoxIcon.Error);
              return;
          }


          Regex reg = new Regex(&quot;^[0-9]*$&quot;);
          if (txbSourceSheet.Text != string.Empty && txbTargetSheet.Text != string.Empty)
          {
              if (reg.IsMatch(txbSourceSheet.Text.ToUpper()) && reg.IsMatch(txbTargetSheet.Text.ToUpper()))
              {
                  try
                  {
                      new CompareHelper().CompareSheets(
                          int.Parse(txbSourceSheet.Text),
                          int.Parse(txbTargetSheet.Text),
                          txbExcelPath.Text);


                      // Clean up the unmanaged Excel COM resources by explicitly
                      GC.Collect();
                      GC.WaitForPendingFinalizers();
                      GC.Collect();
                      GC.WaitForPendingFinalizers(); 
                      MessageBox.Show(&quot;Compare sheets successfully,Please check in the excel file&quot;);
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(&quot;Exception occur, the Exception Message is: &quot; &#43; ex.Message);
                      return;
                  }
              }
              else
              {
                  MessageBox.Show(&quot;Source Sheet and Target Sheet must be Number&quot;);
                  return;
              }
          }
          else
          {
              MessageBox.Show(&quot;Please input Source Sheet and Target Sheet&quot;);
              return;
          }
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><span style=""><span style="">&nbsp;</span>Office Dev Center
</span></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/office/aa905340">http://msdn.microsoft.com/en-us/office/aa905340</a>
</p>
<p class="MsoNormal"><span style="">&nbsp;</span>Excel Object Model Overview</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/wss56bz7(v=vs.100).aspx">http://msdn.microsoft.com/en-us/library/wss56bz7(v=vs.100).aspx</a>
</p>
<p class="MsoNormal">Regex Class</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regex.aspx">http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regex.aspx</a>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
