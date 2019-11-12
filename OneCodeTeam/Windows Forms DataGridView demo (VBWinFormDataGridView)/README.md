# Windows Forms DataGridView demo (VBWinFormDataGridView)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Forms
## Topics
- Data Binding
- DataGridView
## Updated
- 03/04/2012
## Description

<h1><span style="font-family:������">WINDOWS FORMS APPLICATION</span> (<span style="font-family:������">VBWinFormDataGridView</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal"><span style="">There are five samples in this solution to demonstrate DataGridView:
</span></p>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>CustomDataGridViewColumn<span style="">:</span></p>
<p class="MsoListParagraphCxSpMiddle">This sample demonstrates how to create a custom DataGridView column<span style="">.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>DataGridViewPaging<span style="">:</span></p>
<p class="MsoListParagraphCxSpMiddle">This sample demonstrates how to page data in the<span style="">&nbsp;
</span>DataGridView control<span style="">. </span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>EditingControlHosting<span style="">:</span></p>
<p class="MsoListParagraphCxSpMiddle">This sample demonstrates how to host a control in the current DataGridViewCell<span style="">&nbsp;
</span>for editing.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>JustInTimeDataLoading<span style="">:</span></p>
<p class="MsoListParagraphCxSpMiddle">If you are working with a very large table in a remote database, for example, you might want to avoid startup delays by retrieving only the data that is necessary for display and retrieving additional data only when the
 user scrolls<span style="">&nbsp; </span>new rows into view. If the client computers running your application have a<span style="">&nbsp;
</span>limited amount of memory available for storing data, you might also want to<span style="">&nbsp;
</span>discard unused data when retrieving new values from the database.</p>
<p class="MsoListParagraphCxSpMiddle">This sample demonstrates how to use virtual mode in the DataGridView control<span style="">&nbsp;
</span>with a data cache that loads data from a server only when it is needed.<span style="">&nbsp;
</span>This kind of data loading is called &quot;Just-in-time data loading&quot;.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>MultipleLayeredColumnHeader<span style="">:</span></p>
<p class="MsoListParagraphCxSpLast">This sample demonstrates how to display multiple layer column headers on the DataGridView contorl.<span style="">
</span></p>
<h2>Running the Sample</h2>
<p class="MsoListParagraph" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>CustomDataGridViewColumn.</p>
<p class="MsoNormal"><span style=""><img src="53670-image.png" alt="" width="576" height="407" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>DataGridViewPaging<span style="">.</span></p>
<p class="MsoNormal"><span style=""><img src="53671-image.png" alt="" width="576" height="512" align="middle">
</span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">EditingControlHosting. </span></p>
<p class="MsoNormal"><span style=""><img src="53672-image.png" alt="" width="576" height="428" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>JustInTimeDataLoading<span style="">. </span></p>
<p class="MsoNormal"><span style=""><img src="53673-image.png" alt="" width="576" height="460" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>MultipleLayeredColumnHeader<span style="">.</span></p>
<p class="MsoNormal"><span style=""><img src="53674-image.png" alt="" width="576" height="431" align="middle">
</span></p>
<p class="MsoNormal">. </p>
<h2>Using the Code</h2>
<p class="MsoListParagraph" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>CustomDataGridViewColumn<span style="">. </span></p>
<p class="MsoNormal" style="margin-left:36.0pt">There're six standard DataGridViewColumn types for use as follows:</p>
<p class="MsoNormal" style="margin-left:72.0pt"><span style=""><img src="53675-image.png" alt="" width="433" height="407" align="middle">
</span><span style="background:#D9D9D9"></span></p>
<p class="MsoNormal" style="margin-left:36.0pt">However, developers may want to use a different control for editing on the column,<span style="">
</span>e.g. MarkedTextBox, DateTimePicker etc. This feature can be achieved in two ways:</p>
<p class="MsoNormal" style="margin-left:36.0pt"><span style="">A</span>. Create a custom DataGridViewColumn;
</p>
<p class="MsoNormal" style="margin-left:36.0pt"><span style="">&nbsp;&nbsp; </span>
The code in this CustomDataGridViewColumn sample demonstrates how to do this;<span style="">
</span></p>
<p class="MsoNormal" style="margin-left:36.0pt"><span style="">B</span>. Place the editing control on the current cell when editing begins, and hide the editing control when the editing completes. For the details of this<span style="">
</span>approach, please refer to the <span style=""><span style="">&nbsp;&nbsp;</span></span>EditingControlHosting sample.<span style="">
</span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>DataGridViewPaging<span style=""> </span></p>
<p class="MsoNormal" style="margin-left:36.0pt"><span style="">A. Get total count of the rows in the table;
</span></p>
<p class="MsoNormal" style="margin-left:36.0pt"><span style="">B. Calculate total count of pages;
</span></p>
<p class="MsoNormal" style="margin-left:36.0pt"><span style="">C. Load each page on demand;
</span></p>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>EditingControlHosting<span style=""> </span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">A. Create an instance of the editing control, in this sample the editing control is MaskedTextBox.<span style="">&nbsp;&nbsp;&nbsp;
</span></span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">B. Specify a mask for the MaskedTextBox and add the MaskedTextBox to the control collection of the DataGridView;<span style="">&nbsp;&nbsp;
</span></span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">C. Hide the MaskedTextBox;
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">D. Handle the CellBeginEdit event to show the MaskedTextBox on the current editing cell;<span style="">&nbsp;&nbsp;
</span></span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">E. Handle the CellEndEdit event to hide the MaskedTextBox when editing completes;
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">F. Handle the Scroll event to adjust the location of the MaskedTextBox as it is showing when scrolling the DataGridView;
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">G. Handle the EditingControlShowing event to pass the focus to the MaskedTextBox when begin editing with keystrokes;
</span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>JustInTimeDataLoading</p>
<p class="MsoListParagraphCxSpLast"><span style="">A</span>.<span style="">&nbsp;
</span>Enable VirtualMode on the DataGridView control by setting the VirtualMode property to true:<span style="">&nbsp;&nbsp;&nbsp;
</span><span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
' Enable VirtualMode on the DataGridView
Me.dataGridView1.VirtualMode = True

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">B</span>.<span style="">&nbsp;
</span>Add columns to the DataGridView according to the data in the database;<span style="">
</span></p>
<p class="MsoListParagraphCxSpLast"><span style="">C</span>.<span style="">&nbsp;
</span>Retrieve the row count of the data in the database and set the RowCount<span style="">&nbsp;
</span>property for the DataGridView;<span style="">&nbsp;&nbsp;&nbsp; </span><span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
              ' Handle the CellValueNeeded event to retrieve the requested cell value
            ' from the data store or the Customer object currently in edit.
            ' This event occurs whenever the DataGridView control needs to paint a cell.


            ' Create a DataRetriever and use it to create a Cache object
            ' and to initialize the DataGridView columns and rows.
            Try
                Dim retriever As DataRetriever = New DataRetriever(connectionString, table)
                memoryCache = New Cache(retriever, 16)
                For Each column As DataColumn In retriever.Columns
                    dataGridView1.Columns.Add(column.ColumnName, column.ColumnName)
                Next
                Me.dataGridView1.RowCount = retriever.RowCount
            Catch ex As Exception
                MessageBox.Show(&quot;Connection could not be established. &quot; &#43; _
                    &quot;Verify that the connection string is valid.&quot;)
                Application.Exit()
            End Try

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast"><span style="">D</span>.<span style="">&nbsp;
</span>Handle the CellValueNeeded event to retrieve the requested cell value from the data store or the Customer object currently in edit.<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Private Sub dataGridView1_CellValueNeeded(ByVal sender As <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Object.aspx" target="_blank" title="Auto generated link to System.Object">System.Object</a>, ByVal e As <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.Forms.DataGridViewCellValueEventArgs.aspx" target="_blank" title="Auto generated link to System.Windows.Forms.DataGridViewCellValueEventArgs">System.Windows.Forms.DataGridViewCellValueEventArgs</a>) Handles dataGridView1.CellValueNeeded
    e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex)
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>MultipleLayeredColumnHeader</p>
<p class="MsoListParagraphCxSpLast"><span style="">A</span>.<span style="">&nbsp;
</span>Enable resizing on the column headers by setting the ColumnHeadersHeightSizeMode property as follows:<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
' Enable resizing on the column headers
          Me.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast"><span style="">B</span>.<span style="">&nbsp;
</span>Adjust the height for the column headers to make it wide enough for two layers;<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
' Adjust the height for the column headers
Me.dataGridView1.ColumnHeadersHeight = Me.dataGridView1.ColumnHeadersHeight * 2

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast"><span style="">C</span>.<span style="">&nbsp;
</span>Adjust the text alignment on the column headers to make the text display at the center of the bottom;<span style="">&nbsp;&nbsp;&nbsp;
</span><span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
             ' Adjust the text alignment on the column headers to make the text display
           ' at the center of the bottom
           Me.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter


           ' Handle the CellPainting event to draw text for each header cell

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast"><span style="">D</span>.<span style="">&nbsp;
</span>Handle the DataGridView.CellPainting event to draw text for each header<span style="">&nbsp;
</span>cell;<span style=""> </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Private Sub dataGridView1_CellPainting(ByVal sender As <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Object.aspx" target="_blank" title="Auto generated link to System.Object">System.Object</a>, ByVal e As <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.Forms.DataGridViewCellPaintingEventArgs.aspx" target="_blank" title="Auto generated link to System.Windows.Forms.DataGridViewCellPaintingEventArgs">System.Windows.Forms.DataGridViewCellPaintingEventArgs</a>) Handles dataGridView1.CellPainting
           If e.RowIndex = -1 AndAlso e.ColumnIndex &gt; -1 Then
               e.PaintBackground(e.CellBounds, False)
               Dim r2 As Rectangle = e.CellBounds
               r2.Y &#43;= e.CellBounds.Height / 2
               r2.Height = e.CellBounds.Height / 2
               e.PaintContent(r2)
               e.Handled = True
           End If
       End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast"><span style="">E</span>.<span style="">&nbsp;
</span>Handle the DataGridView.Paint event to draw &quot;merged&quot; header cells;<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Private Sub dataGridView1_Paint(ByVal sender As <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Object.aspx" target="_blank" title="Auto generated link to System.Object">System.Object</a>, ByVal e As <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.Forms.PaintEventArgs.aspx" target="_blank" title="Auto generated link to System.Windows.Forms.PaintEventArgs">System.Windows.Forms.PaintEventArgs</a>) Handles dataGridView1.Paint
           ' Data for the &quot;merged&quot; header cells
           Dim monthes As String() = {&quot;January&quot;, &quot;February&quot;, &quot;March&quot;}
           For j As Integer = 0 To Me.dataGridView1.ColumnCount - 1 Step 2
               ' Get the column header cell bounds
               Dim r1 As Rectangle = Me.dataGridView1.GetCellDisplayRectangle(j, -1, True)


               r1.X &#43;= 1
               r1.Y &#43;= 1
               r1.Width = r1.Width * 2 - 2
               r1.Height = r1.Height / 2 - 2


               Using br As SolidBrush = New SolidBrush(Me.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor)
                   e.Graphics.FillRectangle(br, r1)
               End Using


               Using p As Pen = New Pen(SystemColors.InactiveBorder)
                   e.Graphics.DrawLine(p, r1.X, r1.Bottom, r1.Right, r1.Bottom)
               End Using


               Using format As StringFormat = New StringFormat()
                   Using br As SolidBrush = New SolidBrush(Me.dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor)
                       format.Alignment = StringAlignment.Center
                       format.LineAlignment = StringAlignment.Center
                       e.Graphics.DrawString(monthes(j / 2), Me.dataGridView1.ColumnHeadersDefaultCellStyle.Font, _
                                             br, r1, format)
                   End Using
               End Using
           Next
       End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph"><span style=""></span></p>
<h2>More Information</h2>
<p class="MsoListParagraphCxSpFirst" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://windowsclient.net/blogs/faqs/archive/tags/Custom&#43;Designers/default.aspx">Windows Forms FAQs</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx">DataGridView Class</a><span style="">&nbsp;&nbsp;
</span></span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://msdn.microsoft.com/en-us/library/ms180996.aspx">DataGridView Custom Column Sample</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://msdn.microsoft.com/en-us/library/ms171624.aspx">Implementing Virtual Mode with Just-In-Time Data Loading in the Windows Forms DataGridView Control.</a><span style="">&nbsp;&nbsp;
</span></span></p>
<p class="MsoListParagraphCxSpLast"><span style="font-family:������"><span style="">&nbsp;&nbsp;
</span></span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
