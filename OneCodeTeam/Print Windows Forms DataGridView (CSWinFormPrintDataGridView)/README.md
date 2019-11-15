# Print Windows Forms DataGridView (CSWinFormPrintDataGridView)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Forms
## Topics
- DataGridView
- Printing
## Updated
- 03/12/2012
## Description

<h1>The code sample demonstrates how to print a DataGridView. (CSWinFormPrintDataGridView)</h1>
<h2>Introduction</h2>
<div>The code sample demonstrates how to print a DataGridView. The sample shows you the granularity as to print a single cell too.</div>
<div>&nbsp;</div>
<div>The Code contains:</div>
<div>&nbsp;</div>
<div>Two forms,</div>
<div>1. Main form which contains DataGridView.</div>
<div>2. One more form to give an option to customer if he wants to print any of the Columns</div>
<div>&nbsp;</div>
<div>The code is mostly in below events</div>
<div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;printDocument1_PrintPage(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;System.Drawing.Printing.PrintPageEventArgs&nbsp;e)</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;This contains most of the logic as to which row or column is to be printed.
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span><span class="cs__keyword">void</span>&nbsp;dataGridView1_CellContentClick(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;DataGridViewCellEventArgs&nbsp;e)</pre>
</div>
</div>
</div>
<p>This contains an important line of code which confirms which column is to be printed.</p>
<p>&nbsp;</p>
<h2>Running the Sample</h2>
<p>After executing the code or double clicking the application, you will get:</p>
<p><img src="54175-image001.png" alt=""></p>
<p>1. Select to print: After entering all the data, we can select which Row to print by selecting the check boxes.</p>
<p>2. Choose which Column to print: This will open one more form like below.</p>
<p>Here you can choose any one of the columns to be printed. If you want all of them select &ldquo;All of them&rdquo;</p>
<p><img src="54176-image002.png" alt=""></p>
<p>Please note if you select in &ldquo;Select to print&rdquo; only the first and second row and in the columns to print if you select all of them. It will print the first rows completely.</p>
<p>Let&rsquo;s say you just wanted to print first entry of Sunday .You need to select only the first row in &ldquo;Select to Print&rdquo; and Sundays in Choose Which column to print.</p>
<p>In any case you can select the entries you want to print, but you need to give an option in both Columns and Rows.</p>
<p>3. Confirm to print: This will open a print Dialog from which you can select a printer and print it.</p>
<p>&nbsp;</p>
<h2>Using the Code</h2>
<p>1. In printDocument1_PrintPage , I am first printing all the days using the below code</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__com">//&nbsp;Please&nbsp;note&nbsp;this&nbsp;is&nbsp;where&nbsp;I&nbsp;am&nbsp;Printing&nbsp;Sunday&nbsp;,&nbsp;Monday&nbsp;,&nbsp;Tuesday....&nbsp;We&nbsp;can&nbsp;also&nbsp;move&nbsp;rowCounter&nbsp;to&nbsp;</span><span class="cs__com">//&nbsp;the&nbsp;maxRowcounter&nbsp;loop&nbsp;where&nbsp;we&nbsp;are&nbsp;printing&nbsp;below&nbsp;</span><span class="cs__keyword">for</span>&nbsp;(z&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;z&nbsp;&lt;&nbsp;dataGridView1.Columns.Count&nbsp;-&nbsp;<span class="cs__number">1</span>;&nbsp;z&#43;&#43;)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;e.Graphics.FillRectangle(Brushes.AliceBlue,&nbsp;realwidth,&nbsp;realheight,&nbsp;width,&nbsp;height);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;e.Graphics.DrawRectangle(Pens.Black,&nbsp;realwidth,&nbsp;realheight,&nbsp;width,&nbsp;height);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;e.Graphics.DrawString(dataGridView1.Columns[z].HeaderText,&nbsp;dataGridView1.Font,&nbsp;Brushes.Black,&nbsp;realwidth,&nbsp;realheight);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;realwidth&nbsp;=&nbsp;realwidth&nbsp;&#43;&nbsp;width;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>Iterating through each day and drawing a rectangle and further what is the content in each column.</p>
<p>2. After this I move to printing the contents of the individual cells. I have used the below logic:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">while</span>&nbsp;(rowCounter&nbsp;&lt;&nbsp;dataGridView1.Rows.Count)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;realwidth&nbsp;=&nbsp;<span class="cs__number">100</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(columnsToPrint[rowCounter]&nbsp;==&nbsp;<span class="cs__keyword">true</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;((f.checkBox8.Checked&nbsp;==&nbsp;<span class="cs__keyword">true</span>)&nbsp;||&nbsp;(f.checkBox1.Checked))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(dataGridView1.Rows[rowCounter].Cells[<span class="cs__number">0</span>].Value&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dataGridView1.Rows[rowCounter].Cells[<span class="cs__number">0</span>].Value&nbsp;=&nbsp;<span class="cs__string">&quot;&quot;</span>;&nbsp;
</pre>
</div>
</div>
</div>
<p>The first while loop is for the count of no of rows and rowCounter is considered as an index further.</p>
<p>columnsToPrint[rowCounter] is an array of bools . As per which ever Row is selected in the first form at &ldquo;Select To Print&rdquo;, the loop will be entered for printing.</p>
<p>The drawing goes on for each row and further selecting which cell to print in each row.</p>
<p>Now the property for the checkboxes is checked if it&rsquo;s true or not. These check boxes are there in Form2. Based on which ever check box is selected it will start drawing.</p>
<p>&nbsp;</p>
<h2>More Information</h2>
<p><a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx">http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx</a></p>
<p><a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.printdialog.aspx">http://msdn.microsoft.com/en-us/library/system.windows.forms.printdialog.aspx</a></p>
<p><a href="http://msdn.microsoft.com/en-us/library/system.drawing.printing.printdocument.aspx">http://msdn.microsoft.com/en-us/library/system.drawing.printing.printdocument.aspx</a></p>
</div>
</div>
<div>&nbsp;</div>
<div></div>
<div><br>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt=""></a></div>
</div>
