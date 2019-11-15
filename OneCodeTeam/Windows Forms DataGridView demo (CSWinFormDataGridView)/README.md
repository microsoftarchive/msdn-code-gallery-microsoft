# Windows Forms DataGridView demo (CSWinFormDataGridView)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- Windows Forms
## Topics
- Data Binding
## Updated
- 02/03/2012
## Description

<p style="font-family:Courier New">&nbsp;</p>
<h2>WINDOWS FORMS APPLICATION : CSWinFormDataGridView Project Overview<br>
<br>
CustomDataGridViewColumn Sample</h2>
<p style="font-family:Courier New">&nbsp;</p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
This sample demonstrates how to create a custom DataGridView column.<br>
<br>
</p>
<h3>Remark:</h3>
<p style="font-family:Courier New"><br>
There're six standard DataGridViewColumn types for use as follows:<br>
<br>
DataGridViewTextBoxColumn<br>
DataGridViewCheckedBoxColumn<br>
DataGridViewComboBoxColumn<br>
DataGridViewLinkColumn<br>
DataGridViewButtonColumn<br>
DataGridViewImageColumn<br>
<br>
However, developers may want to use a different control for editing on the column,<br>
e.g. MarkedTextBox, DateTimePicker etc. This feature can be achieved in two ways:<br>
<br>
1. Create a custom DataGridViewColumn; <br>
<br>
&nbsp; The code in this CustomDataGridViewColumn sample demonstrates how to do this;<br>
<br>
2. Place the editing control on the current cell when editing begins, and hide<br>
&nbsp; the editing control when the editing completes. For the details of this<br>
&nbsp; approach, please refer to the EditingControlHosting sample.<br>
<br>
</p>
<h3>Creation:</h3>
<p style="font-family:Courier New"><br>
1. Create a MaskedTextBoxEditingControl class derive from MaskedTextBox class <br>
&nbsp; and IDataGridViewEditingControl class, see the code in the <br>
&nbsp; MaskedTextBoxEditingControl.cs file for the implementation details;<br>
<br>
2. Create a MaskedTextBoxCell class derive from DataGridViewTextBoxCell class,<br>
&nbsp; see the code in the MaskedTextBoxCell.cs file for the implementation details;<br>
<br>
3. Create a MaskedTextBoxColumn class derive from DataGridViewColumn class,<br>
&nbsp; see the code in the MaskedTextBoxColumn.cs file for the implementation details;<br>
<br>
4. Build the program;<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
1. DataGridView Custom Column Sample<br>
<a href="http://msdn.microsoft.com/en-us/library/ms180996.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/ms180996.aspx</a><br>
<br>
2. Windows Forms FAQs<br>
<a href="http://windowsclient.net/blogs/faqs/archive/tags/Custom&#43;Designers/default.aspx" target="_blank">http://windowsclient.net/blogs/faqs/archive/tags/Custom&#43;Designers/default.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
