# Implement auto-complete textbox in WPF (CSWPFAutoCompleteTextBox)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- WPF
## Topics
- Controls
- Auto-complete
## Updated
- 04/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>WPF APPLICATION : CSWPFAutoCompleteTextBox Project Overview<br>
<br>
AutoCompleteTextBox Sample<br>
</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
Provide an easy implementation of AutoCompleteTextBox in WPF<br>
&nbsp; <br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
&nbsp; 1. Retemplate ComboBox to make it looks like TextBox.<br>
&nbsp; 2. Extend ComboBoxItem so that we can hightlight the already entered part<br>
&nbsp; &nbsp; &nbsp;in the dropdown list.<br>
&nbsp; 3. Get reference to the TextBox part of the ComboBox, and hook up <br>
&nbsp; &nbsp; &nbsp;TextBox.TextChanged event.<br>
&nbsp; 4. In the TextBox.TextChanged event handler, we filter the underlying <br>
&nbsp; &nbsp; &nbsp;datasource and create new list source with our customized ComboBox<br>
&nbsp; &nbsp; &nbsp;Items.<br>
&nbsp; <br>
<br>
<br>
&nbsp; </p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
