# Call server side function without web service
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- ASP.NET
## Updated
- 06/13/2013
## Description

<h1>Call server side function without web service (VBASPNETStaticCodeByPageMethod)</h1>
<h2>Introduction</h2>
<p class="MsoNormal"><span style="">This sample code will demonstrate how to call server side function without web service. ASP.NET Ajax extensions came with full support for script services. But sometimes you don't want to build a web service for a small
 piece of code or for basic functionality. This is why the PageMethods feature was created. The PageMethods feature enables the using of code-behind page methods on the clientside. The PageMethods can only be added to the page itself so don't try to add them
 to user controls or custom controls - it won't work.</span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style="">Please follow the steps below. </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal">
<span style="">Step 1: Open the VBASPNETStaticCodeByPageMethod.sln file. </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal">
<span style="">Step 2: Press &quot;Ctrl&#43;F5&quot; to view Default page in browser.<br>
<span style=""><img src="84212-image.png" alt="" width="559" height="108" align="middle">
</span></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal">
<span style="">Type a name and then click the SayHello button you will see as shown below:<br>
<span style=""><img src="84213-image.png" alt="" width="514" height="271" align="middle">
</span><br style="">
<br style="">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal">
<span style=""><br>
Step 3: Click the getData button and then you will see shown below: </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal">
<span style=""><img src="84214-image.png" alt="" width="577" height="271" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal">
<span style=""><br>
Step 4: Validation is completed.</span></p>
<p class="MsoNormal"></p>
<h2>Using the Code</h2>
<p class="MsoNormal"><span style=""><span style="">&nbsp;</span>Code Logical: <span style="">
&nbsp;&nbsp;&nbsp; </span></span></p>
<p class="MsoNormal"><span style="">Step 1: Create a VB &quot;ASP.NET Web Application&quot; in Visual Studio 2012 or Visual Web Developer and name it as &quot;VBASPNETStaticCodeByPageMethod&quot;.
</span></p>
<p class="MsoNormal"><span style="">Step 2: Add a Jscript File to project by followed the steps below:
<br>
Right-click at the project file &gt;&gt;Add&gt;&gt;New Item…&gt;&gt;Web&gt;&gt;Jscript File
</span></p>
<p class="MsoNormal"><span style="">Have selected the Jscript template, then type script as &quot;name&quot; and then click Add button.<span style="">&nbsp;
</span>After that, </span><span style="color:#222222">paste this segment code to it.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>

<pre id="codePreview" class="js">
// Gets the session state value.
function getSayHello(src) {
    var name = document.getElementById(src.id).value;
    PageMethods.sayHello(name,
        OnSucceeded);
}


// Callback function invoked on successful 
// completion of the page method.
function OnSucceeded(result) {   
       alert(result);
    
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">Then add script reference of the script.js </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;asp:ScriptManager ID=&quot;ScriptManager1&quot; runat=&quot;server&quot; EnablePageMethods=&quot;true&quot;&gt;
      &lt;Scripts&gt;
          &lt;asp:ScriptReference Path=&quot;script.js&quot; /&gt;
      &lt;/Scripts&gt;
  &lt;/asp:ScriptManager&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">Create a static method at code-behind. </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
&lt;WebMethod()&gt; _
Public Shared Function sayHello(ByVal name As String) As String
        Return &quot;Hello,&quot; & name
    End Function

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><b style=""><span style="">[Note]</span></b><span style="">
<span style=""><span style="">&nbsp;</span>We need</span></span><span style=""> to</span><span style=""> add the System.Web.Services.WebMethod at top of the method.<br>
<span style="">&nbsp;</span>Then add a button at the page as below: </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;input type=&quot;button&quot; onclick=&quot;getSayHello(&lt;%=tbInput.ClientID %&gt;)&quot; value=&quot;SayHello&quot; /&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">This is a routine call: the passed parameter of the client side and server side is a string or number, etc..
</span></p>
<p class="MsoNormal"><span style="">Step 4: <span style="">PageMethod also have usage that is similar to the Default ModelBinder.<br>
First, add the following code in code-behind: </span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
&lt;WebMethod()&gt; _
   Public Shared Function getData(ByVal t As TestUser) As Object
       Return New With { _
        Key .Name = t.Name & &quot;-Test&quot;, _
        Key .Value = t.Phone _
       }
   End Function

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Public Class TestUser
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal value As String)
                m_Name = value
            End Set
        End Property
        Private m_Name As String
        Public Property BirthDate() As DateTime
            Get
                Return m_BirthDate
            End Get
            Set(ByVal value As DateTime)
                m_BirthDate = value
            End Set
        End Property
        Private m_BirthDate As DateTime
        Public Property Phone() As IList(Of String)
            Get
                Return m_Phone
            End Get
            Set(ByVal value As IList(Of String))
                m_Phone = value
            End Set
        End Property
        Private m_Phone As IList(Of String)
    End Class

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">Write a javascript to call PageMethod in aspx.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>

<pre id="codePreview" class="js">
&lt;script type=&quot;text/javascript&quot;&gt;
       function getData() {
           var data = {
               Name: &quot;TestUser&quot;,
               BirthDate: new Date(),
               Phone: [&quot;13612345678&quot;, &quot;02112345&quot;]
           };
           PageMethods.getData(data, function(returnValue) {
               alert(returnValue.Name &#43; &quot;:&quot; &#43; returnValue.Value);
           });
       }
       
    
   &lt;/script&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">Then add a button at the page as shown below:
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;input type=&quot;button&quot; onclick=&quot;getData()&quot; value=&quot;getData&quot; /&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;; color:red"><br>
</span><span style="font-family:&quot;Segoe UI&quot;,&quot;sans-serif&quot;; color:#676767"><span style="">&nbsp;</span></span><span style="">Step 5: You can test and debug it.</span></p>
<h2>More Information<br>
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Calling Static Methods in an ASP.NET Web Page
</span></h2>
<p class="MsoNormal"><span style=""><span style="">&nbsp;</span><a href="http://www.asp.net/Ajax/Documentation/Live/tutorials/ExposingWebServicesToAJAXTutorial.aspx">http://www.asp.net/Ajax/Documentation/Live/tutorials/ExposingWebServicesToAJAXTutorial.aspx</a><br>
</span><span lang="EN" style="">ASP.NET Ajax PageMethods<br>
</span><span style=""><a href="http://blogs.microsoft.co.il/blogs/gilf/archive/2008/10/04/asp-net-ajax-pagemethods.aspx">http://blogs.microsoft.co.il/blogs/gilf/archive/2008/10/04/asp-net-ajax-pagemethods.aspx</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
