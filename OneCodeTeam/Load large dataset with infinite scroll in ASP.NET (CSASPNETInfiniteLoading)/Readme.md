# Load large dataset with infinite scroll in ASP.NET (CSASPNETInfiniteLoading)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- AJAX
- Infinite scroll
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>ASP.NET APPLICATION : CSASPNETInfiniteLoading Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
Infinite scroll, has also been called autopagerize, unpaginate, endless pages. <br>
But essentially it is pre-fetching content from a subsequent page and adding <br>
it directly to the user�s current page. &nbsp;The code sample demonstrates loading
<br>
a large number of data entries in an XML file. It support infinite scroll <br>
with the AJAX technology.<br>
<br>
<br>
Demo the Sample. <br>
<br>
Open the CSASPNETInfiniteLoading.sln directly, expand the web application <br>
node and press F5 to test the application.<br>
<br>
Step 1. &nbsp;View default.aspx in browser<br>
<br>
Step 2. &nbsp;By default, we could see a vertical scroll on the page, just drag it
<br>
&nbsp; &nbsp; &nbsp; &nbsp; scroll down, you will find the new content load infinitely meanwhile
<br>
&nbsp; &nbsp; &nbsp; &nbsp; the scroll bar becomes small and small.<br>
&nbsp;&nbsp;&nbsp;&nbsp; note: if there is no vertical scroll bar on page, just do appropriate<br>
&nbsp;&nbsp;&nbsp;&nbsp; scaling for the page till the vertical scroll bar appeared.<br>
<br>
<br>
</p>
<h3>Code Logical:</h3>
<p style="font-family:Courier New"><br>
Step 1. &nbsp;Create a C# ASP.NET Empty Web Application in Visual Studio 2010.<br>
<br>
<br>
Step 2. &nbsp;Create a new directory, &quot;Scripts&quot;. Right-click the directory and click<br>
&nbsp; &nbsp; &nbsp; &nbsp; Add -&gt; New Item -&gt; JScript File. We need to reference jquery javascript
<br>
&nbsp;&nbsp;&nbsp;&nbsp; library files jquery-1.4.1.min.js<br>
<br>
<br>
Step 3. &nbsp;Create a new directory, &quot;Styles&quot;. Right-click the directory and click<br>
&nbsp; &nbsp; &nbsp; &nbsp; Add -&gt; New Item -&gt; Style Sheet File. reference site.css.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <br>
<br>
Step 4. &nbsp;Open the Default.aspx,(If there is no Default.aspx, create one.)<br>
&nbsp; &nbsp; &nbsp; &nbsp; In the Head block, add javascript and style references like below.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp; [CODE] &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;link rel=&quot;stylesheet&quot; href=&quot;Styles/Site.css&quot; type=&quot;text/css&quot; /&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &lt;script type=&quot;text/javascript&quot; src=&quot;Scripts/jquery-1.4.1.min.js&quot;&gt;&lt;/script&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp; [/CODE]<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp; write the autocomplete javascript as below.<br>
&nbsp;&nbsp;&nbsp;&nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; $(document).ready(function () {<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;function lastPostFunc() {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;$('#divPostsLoader').html('&lt;img src=&quot;images/bigLoader.gif&quot;&gt;');<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;//send a query to server side to present new content<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;$.ajax({<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;type: &quot;POST&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;url: &quot;Default.aspx/Foo&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;data: &quot;{}&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;contentType: &quot;application/json; charset=utf-8&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;dataType: &quot;json&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;success: function (data) {<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (data != &quot;&quot;) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;$('.divLoadData:last').after(data.d);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;$('#divPostsLoader').empty();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;})<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;};<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;//When scroll down, the scroller is at the bottom with the function below and fire
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;the lastPostFunc function<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;$(window).scroll(function () {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if ($(window).scrollTop() == $(document).height() - $(window).height()) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;lastPostFunc();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;});<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; });<br>
&nbsp; &nbsp;<br>
&nbsp;&nbsp;&nbsp;&nbsp; [CODE]&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <br>
&nbsp;&nbsp;&nbsp;&nbsp; For more details, please refer to the Default.aspx in this sample.<br>
<br>
Step 7. &nbsp;Everything is ready, test the application by scrolling down the page to see what happens.
<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
<a target="_blank" href="http://www.webresourcesdepot.com/load-content-while-scrolling-with-jquery/">http://www.webresourcesdepot.com/load-content-while-scrolling-with-jquery/</a>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
