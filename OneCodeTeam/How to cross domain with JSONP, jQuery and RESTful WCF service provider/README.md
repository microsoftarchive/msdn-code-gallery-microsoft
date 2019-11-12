# How to cross domain with JSONP, jQuery and RESTful WCF service provider
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
- Networking
- .NET Development
- Windows Desktop App Development
- Windows Communication Framework (WCF)
## Topics
- jQuery
- Javascript
- JSONP
- Cross Domain
- REST WCF
## Updated
- 06/13/2013
## Description

<h1>How to cross domain with JSONP, jQuery and RESTful WCF Service provider in JavaScript (JSCrossDomainWithJSONP)</h1>
<h2>Introduction</h2>
<p class="MsoNormal"><span style="">This article and the attached code sample demonstrate how to use jQuery or original JavaScript to cross domain. JSONP is a convention used to invoke cross-domain scripts by generating script tags in the current script.
 The result is returned in a specified callback function.</span> <span style="">JSONP is based on the idea that tags such as &lt;script src=&quot;http://...&quot; &gt; can evaluate scripts from any domain and the script retrieved by those tags is evaluated
 within a scope in which other functions may already be defined. You can find the answers for all the following questions in the code sample:
</span></p>
<p class="MsoListBulletCxSpFirst" style=""><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">How to use $.getJSON to read a JSONP request?
</span></p>
<p class="MsoListBulletCxSpLast" style=""><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">How to use original JavaScript to read a JSONP request?
</span></p>
<h2>Building the Sample</h2>
<p class="MsoNormal"><span style="">To build this sample </span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Open JSCrossDomainWithJSONP.sln solution file in Visual Studio 2010
</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:5.0pt"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Build<span style="">&nbsp; </span>entire solution </span>
</p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style=""><br>
After opening the sample project in Visual Studio 2010, you will find two projects:
<b style="">JSCrossDomainASPNETClient</b> and <b style="">JSCrossDomainWCFProvider</b>. Build entire solution and follow below steps:
</span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Right click <b style="">JSCrossDomainWCFProvider
</b>project in the<b style=""> </b>Solution Explorer </span></p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:5.0pt"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Select &quot;Debug&quot; item </span></p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:5.0pt"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Click &quot;Start new instance&quot; </span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:5.0pt"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Click &quot;Agent.aspx&quot; in the<span style="">&nbsp;
</span>&quot;Directory Listing&quot; of IE </span></p>
<p class="MsoNormal"><span style="">You will get the JSON data from WCF Service:
</span></p>
<p class="MsoNormal"><span style=""><img src="84418-image.png" alt="" width="765" height="166" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"><span style="">The Agent.aspx is an agent page to accept user's request and then sent it to WCF Service to get data. Now we can open the client
<b style="">Default.htm</b> page (like: <a href="http://localhost:50505/Default.htm">
http://localhost:50505/Default.htm</a>) as below: </span></p>
<p class="MsoNormal"><span style=""><img src="84419-image.png" alt="" width="763" height="410" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"><span style="">&quot;<b style="">Get Data By jQuery</b>&quot; button sends the request with jQuery method ($getJSON) and &quot;<b style="">Get Data By Original JavaScript</b>&quot; button with original JavaScript. For More information,
 please refer to below code.<br>
OK, let's click &quot;Get Data By jQuery&quot; to request the data through Agent.aspx.
</span></p>
<p class="MsoListBullet" style=""><span style=""><img src="84420-image.png" alt="" width="765" height="432" align="middle">
</span><span style=""></span></p>
<h2>Using the Code</h2>
<p class="MsoNormal"><span style="">The code sample provides the following reusable functions for using jQuery or original JavaScript to read a JSONP request.</span>
<span style=""></span></p>
<p class="MsoNormal"><b style=""><span style="">How to use $.getJSON to read a JSONP request?
</span></b></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>

<pre id="codePreview" class="js">
var url = &quot;http://localhost:50500/Agent.aspx?callBack=&quot;;


//Get data by using $.getJSON
function getAllUsersByJQuery(event) {
&nbsp;&nbsp;&nbsp; event && event.preventDefault();


&nbsp;&nbsp;&nbsp; $.getJSON(url &#43; &quot;?&quot;, jsonpCallBack);
}


//Call back function
function jsonpCallBack(users) {
&nbsp;&nbsp;&nbsp; data = users
&nbsp;&nbsp;&nbsp; pageCount = data.length % pageNum == 0 ? data.length / pageNum : parseInt(data.length / pageNum) &#43; 1;


&nbsp;&nbsp;&nbsp; $(&quot;#datacount&quot;).text(data.length);
&nbsp;&nbsp;&nbsp; $(&quot;#pagecount&quot;).text(pageCount);
&nbsp;&nbsp;&nbsp; $(&quot;#pageindex&quot;).text(pageIndex);
&nbsp;&nbsp;&nbsp; $(&quot;#error&quot;).empty();
&nbsp;&nbsp;&nbsp; fillView();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoNormal"><b style=""><span style="">How to use original JavaScript to read a JSONP request?
</span></b></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>

<pre id="codePreview" class="js">
var url = &quot;http://localhost:50500/Agent.aspx?callBack=&quot;;


//Get data by using original JavaScript with &quot;createElement&quot;
function getAllUsersByOriginalJS(event) {
&nbsp;&nbsp;&nbsp; event && event.preventDefault();


&nbsp;&nbsp;&nbsp; var jsonpScript = document.createElement(&quot;script&quot;);
&nbsp;&nbsp;&nbsp; jsonpScript.type = &quot;text/javascript&quot;;
&nbsp;&nbsp;&nbsp; jsonpScript.src = url &#43; &quot;jsonpCallBack&quot;;
&nbsp;&nbsp;&nbsp; document.head.appendChild(jsonpScript);
}


//Call back function
function jsonpCallBack(users) {
&nbsp;&nbsp;&nbsp; data = users
&nbsp;&nbsp;&nbsp; pageCount = data.length % pageNum == 0 ? data.length / pageNum : parseInt(data.length / pageNum) &#43; 1;


&nbsp;&nbsp;&nbsp; $(&quot;#datacount&quot;).text(data.length);
&nbsp;&nbsp;&nbsp; $(&quot;#pagecount&quot;).text(pageCount);
&nbsp;&nbsp;&nbsp; $(&quot;#pageindex&quot;).text(pageIndex);
&nbsp;&nbsp;&nbsp; $(&quot;#error&quot;).empty();
&nbsp;&nbsp;&nbsp; fillView();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<h2>More Information</h2>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
