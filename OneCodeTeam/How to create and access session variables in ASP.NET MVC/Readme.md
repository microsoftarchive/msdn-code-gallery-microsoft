# How to create and access session variables in ASP.NET MVC
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
- .NET
- ASP.NET MVC
- ASP.NET MVC 4
- Web App Development
## Topics
- Session
## Updated
- 09/21/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src=":-onecodesampletopbanner1" alt=""></a><span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to create and access session
 variables in ASP.NET MVC</span><span style="font-weight:bold; font-size:14pt"> (</span><span style="font-weight:bold; font-size:14pt">ASPNETMVCSession</span><span style="font-weight:bold; font-size:14pt">)</span></span></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">This sample demonstrates how to create and access session variable</span><span style="font-size:11pt">s</span><span style="font-size:11pt"> in ASP.NET MVC. In this
</span><span style="font-size:11pt">sample</span><span style="font-size:11pt">, </span>
<span style="font-size:11pt">we</span><span style="font-size:11pt"> will </span><span style="font-size:11pt">demo</span><span style="font-size:11pt"> two ways to achieve this. One is
</span><span style="font-size:11pt">to </span><span style="font-size:11pt">directly access
</span><span style="font-weight:bold">HttpContext.Current.Session</span><span style="font-size:11pt">. The
</span><span style="font-size:11pt">other is to create an extension method for </span>
<span style="font-weight:bold">HttpContextBase</span><span style="font-size:11pt">. We type some words</span><span style="font-size:11pt"> selected</span><span style="font-size:11pt"> from the page and submit it to the controller</span><span style="font-size:11pt">.</span><span style="font-size:11pt">
</span><span style="font-size:11pt">I</span><span style="font-size:11pt">n the controller, we save the text to session.</span><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-weight:bold">Building the Sample </span>
</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>For this sample </span><span>to </span><span>run, you must install the ASP.NET MVC 4 and if you use Visual Studio 2010, you must install Visual Studio 2010 SP1.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><a href="http://www.asp.net/mvc/mvc4" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://www.asp.net/mvc/mvc4</span></a></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><a href="http://www.microsoft.com/web/handlers/webpi.ashx?command=getinstallerredirect&appid=MVC4VS2010_Loc" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">ASP.NET
 MVC 4 for Visual Studio 2010 SP1</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>Open the </span><span>CSASPNETMVCSession.sln/VBASPNETMVCSession</span><span>.sln.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>Press </span><span style="font-weight:bold">F5</span><span> or
</span><span style="font-weight:bold">Ctrl &#43; F5</span><span> to run the application.
</span><span>You will see a </span><span>page as</span><span> </span><span>shown</span><span> below:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img width="359" src="117327-image.png" alt="" height="141" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>Type some text to the
</span><span>text box</span><span> and then click the submit button. </span><span>T</span><span>hen t</span><span>he page
</span><span>will be displayed as follows </span><span>(</span><span>Each submit button
</span><span>works for th</span><span>e text box </span><span>just above</span><span> it</span><span>)</span><span>:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img width="351" src="117328-image.png" alt="" height="198" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span>Validation finished</span><span>.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Create an </span>
<span style="font-weight:bold">ASP</span><span style="font-weight:bold">.</span><span style="font-weight:bold">NET MVC 4 Web Application</span><span style="font-size:11pt"> project in
</span><span style="font-weight:bold">Visual Studio</span><span style="font-size:11pt">.</span><span style="font-size:11pt">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">We can use the default </span>
<span style="font-weight:bold">template</span><span style="font-size:11pt"> and default &ldquo;</span><span style="font-weight:bold">View engine</span><span style="font-size:11pt">&rdquo;. In the demo project,
</span><span style="font-size:11pt">our</span><span style="font-size:11pt"> choice is the
</span><span style="font-weight:bold">Empty</span><span style="font-size:11pt"> template.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">In the </span>
<span style="font-weight:bold">Solution Explorer</span><span style="font-size:11pt">, browse to the
</span><span style="font-weight:bold">Controllers</span><span style="font-size:11pt"> folders and t</span><span style="font-size:11pt">hen double-click to add a class. Y</span><span style="font-size:11pt">ou can name it as
</span><span style="font-weight:bold">HomeController.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">We will save</span><span style="font-size:11pt"> and access</span><span style="font-size:11pt"> the</span><span style="font-size:11pt"> session in the
</span><span style="font-weight:bold">HomeController</span><span style="font-size:11pt"> class. We will use two ways to achieve this.
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Using</span><span style="font-size:11pt"> HttpContext.Current.Session:</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Save the session
</span><span style="color:#000000; font-size:9pt">variables:</span><span style="font-size:11pt">
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">System.Web.HttpContext.Current.Session[&quot;sessionString&quot;] = sessionValue;
</pre>
<pre class="hidden">System.Web.HttpContext.Current.Session(&quot;sessionString&quot;) = sessionValue
</pre>
<pre class="csharp" id="codePreview">System.Web.HttpContext.Current.Session[&quot;sessionString&quot;] = sessionValue;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Access the session variables:
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">ViewData(&quot;sessionString&quot;) = TryCast(System.Web.HttpContext.Current.Session(&quot;sessionString&quot;), [String])
</pre>
<pre class="hidden">ViewData[&quot;sessionString&quot;] = System.Web.HttpContext.Current.Session[&quot;sessionString&quot;] as String;
</pre>
<pre class="vb" id="codePreview">ViewData(&quot;sessionString&quot;) = TryCast(System.Web.HttpContext.Current.Session(&quot;sessionString&quot;), [String])
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Creating</span><span style="font-size:11pt"> an extension method for
</span><span style="font-size:11pt">HttpSessionStateBase</span><span style="font-size:11pt">:</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Firstly, write extension method to the HttpSessionStateBase. The code
</span><span style="font-size:11pt">snippet </span><a name="_GoBack"></a><span style="font-size:11pt">resembles the following:</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">public static class SessionExtensions
   {
       /// &lt;summary&gt;
       /// Get value.
       /// &lt;/summary&gt;
       /// &lt;typeparam name=&quot;T&quot;&gt;&lt;/typeparam&gt;
       /// &lt;param name=&quot;session&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;key&quot;&gt;&lt;/param&gt;
       /// &lt;returns&gt;&lt;/returns&gt;
       public static T GetDataFromSession&lt;T&gt;(this HttpSessionStateBase session, string key)
       {
           return (T)session[key];
       }
       /// &lt;summary&gt;
       /// Set value.
       /// &lt;/summary&gt;
       /// &lt;typeparam name=&quot;T&quot;&gt;&lt;/typeparam&gt;
       /// &lt;param name=&quot;session&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;key&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;value&quot;&gt;&lt;/param&gt;
       public static void SetDataToSession&lt;T&gt;(this HttpSessionStateBase session, string key, object value)
       {
           session[key] = value;
       }
   }
</pre>
<pre class="hidden">Public Module SessionExtensions
    Sub New()
    End Sub
    ''' &lt;summary&gt;
    ''' Get value.
    ''' &lt;/summary&gt;
    ''' &lt;typeparam name=&quot;T&quot;&gt;&lt;/typeparam&gt;
    ''' &lt;param name=&quot;session&quot;&gt;&lt;/param&gt;
    ''' &lt;param name=&quot;key&quot;&gt;&lt;/param&gt;
    ''' &lt;returns&gt;&lt;/returns&gt;
    &lt;System.Runtime.CompilerServices.Extension&gt; _
    Public Function GetDataFromSession(Of T)(session As HttpSessionStateBase, key As String) As T
        Return DirectCast(session(key), T)
    End Function
    ''' &lt;summary&gt;
    ''' Set value.
    ''' &lt;/summary&gt;
    ''' &lt;typeparam name=&quot;T&quot;&gt;&lt;/typeparam&gt;
    ''' &lt;param name=&quot;session&quot;&gt;&lt;/param&gt;
    ''' &lt;param name=&quot;key&quot;&gt;&lt;/param&gt;
    ''' &lt;param name=&quot;value&quot;&gt;&lt;/param&gt;
    &lt;System.Runtime.CompilerServices.Extension&gt; _
    Public Sub SetDataToSession(Of T)(session As HttpSessionStateBase, key As String, value As Object)
        session(key) = value
    End Sub
End Module
</pre>
<pre class="csharp" id="codePreview">public static class SessionExtensions
   {
       /// &lt;summary&gt;
       /// Get value.
       /// &lt;/summary&gt;
       /// &lt;typeparam name=&quot;T&quot;&gt;&lt;/typeparam&gt;
       /// &lt;param name=&quot;session&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;key&quot;&gt;&lt;/param&gt;
       /// &lt;returns&gt;&lt;/returns&gt;
       public static T GetDataFromSession&lt;T&gt;(this HttpSessionStateBase session, string key)
       {
           return (T)session[key];
       }
       /// &lt;summary&gt;
       /// Set value.
       /// &lt;/summary&gt;
       /// &lt;typeparam name=&quot;T&quot;&gt;&lt;/typeparam&gt;
       /// &lt;param name=&quot;session&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;key&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;value&quot;&gt;&lt;/param&gt;
       public static void SetDataToSession&lt;T&gt;(this HttpSessionStateBase session, string key, object value)
       {
           session[key] = value;
       }
   }
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Save the session variables by using</span><span style="font-size:11pt"> the extension method:</span><span style="font-size:11pt">
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">vb</span>
<pre class="hidden">Session.SetDataToSession&lt;string&gt;(&quot;key1&quot;, sessionValue);
</pre>
<pre class="hidden">Session.SetDataToSession(Of String)(&quot;key1&quot;, sessionValue)
</pre>
<pre class="vb" id="codePreview">Session.SetDataToSession&lt;string&gt;(&quot;key1&quot;, sessionValue);
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Access the session variables by
</span><span style="font-size:11pt">using </span><span style="font-size:11pt">the extension method:
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">string value = Session.GetDataFromSession&lt;string&gt;(&quot;key1&quot;);
          ViewData[&quot;sessionStringByExtensions&quot;] = value;
</pre>
<pre class="hidden">Dim value As String = Session.GetDataFromSession(Of String)(&quot;key1&quot;)
      ViewData(&quot;sessionStringByExtensions&quot;) = value
</pre>
<pre class="csharp" id="codePreview">string value = Session.GetDataFromSession&lt;string&gt;(&quot;key1&quot;);
          ViewData[&quot;sessionStringByExtensions&quot;] = value;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">The html code</span><span style="font-size:11pt"> snippet</span><span style="font-size:11pt"> resembles the following:</span><span style="font-size:11pt">
<br>
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span><span>VB</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span><span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">&lt;body&gt;
    <div>
        &lt;%Using (Html.BeginForm(&quot;SaveSession&quot;, &quot;Home&quot;, FormMethod.Post))
        %&gt;
        <p>&lt;%=ViewData(&quot;sessionString&quot;)%&gt; </p>
        &lt;fieldset&gt;
            <div>
                The text will be stored: &lt;%=Html.TextBox(&quot;sessionValue&quot;)%&gt;
            </div>
            <div>
                &lt;input type=&quot;submit&quot; id=&quot;submitToMethod1&quot; name=&quot;submit&quot; value=&quot;submit&quot; /&gt;
            </div>
        &lt;/fieldset&gt;
        &lt;%           
        End Using
        %&gt;
        &lt;%Using (Html.BeginForm(&quot;SaveSessionByExtensions&quot;, &quot;Home&quot;, FormMethod.Post))
        %&gt;
        <p>
            &lt;%=ViewData(&quot;sessionStringByExtensions&quot;)%&gt;
        </p>
        &lt;fieldset&gt;
            <div>
                The text will be stored: &lt;%=Html.TextBox(&quot;sessionValue&quot;)%&gt;
            </div>
            <div>
                &lt;input type=&quot;submit&quot; id=&quot;submitToMethod2&quot; name=&quot;submit&quot; value=&quot;submit&quot; /&gt;
            </div>
        &lt;/fieldset&gt;
        &lt;%           
        End Using
        %&gt;
    </div>
&lt;/body&gt;
</pre>
<pre class="hidden">&lt;body&gt;
    <div>
        @Using (Html.BeginForm(&quot;SaveSession&quot;, &quot;Home&quot;, FormMethod.Post))
            @<p>
                @ViewData(&quot;sessionString&quot;)
            </p>
            @&lt;fieldset&gt;
                <div>
                    The text will be stored: @Html.TextBox(&quot;sessionValue&quot;)
                </div>
                <div>
                    &lt;input type=&quot;submit&quot; id=&quot;submitToMethod1&quot; name=&quot;submit&quot; value=&quot;submit&quot; /&gt;
                </div>
            &lt;/fieldset&gt;
        End Using
        @Using (Html.BeginForm(&quot;SaveSessionByExtensions&quot;, &quot;Home&quot;, FormMethod.Post))
            @<p>
                @ViewData(&quot;sessionStringByExtensions&quot;)
            </p>
            @&lt;fieldset&gt;
                <div>
                    The text will be stored: @Html.TextBox(&quot;sessionValue&quot;)
                </div>
                <div>
                    &lt;input type=&quot;submit&quot; id=&quot;submitToMethod2&quot; name=&quot;submit&quot; value=&quot;submit&quot; /&gt;
                </div>
            &lt;/fieldset&gt;
        End Using
    </div>
&lt;/body&gt;
</pre>
<pre class="hidden">&lt;body&gt;
    <div>
        @using (Html.BeginForm(&quot;SaveSession&quot;, &quot;Home&quot;, FormMethod.Post))
        {
            <p>@ViewData[&quot;sessionString&quot;] </p>
            &lt;fieldset&gt;
                <div>
                    The text will be stored: @Html.TextBox(&quot;sessionValue&quot;)
                </div>
                <div>
                    &lt;input type=&quot;submit&quot; id=&quot;submitToMethod1&quot; name=&quot;submit&quot; value=&quot;submit&quot; /&gt;
                </div>
            &lt;/fieldset&gt;
        }
        @using (Html.BeginForm(&quot;SaveSessionByExtensions&quot;, &quot;Home&quot;, FormMethod.Post))
        {
            <p>@ViewData[&quot;sessionStringByExtensions&quot;]</p>
            &lt;fieldset&gt;
                <div>
                    The text will be stored: @Html.TextBox(&quot;sessionValue&quot;)
                </div>
                <div>
                    &lt;input type=&quot;submit&quot; id=&quot;submitToMethod2&quot; name=&quot;submit&quot; value=&quot;submit&quot; /&gt;
                </div>
            &lt;/fieldset&gt;
        }
    </div>
&lt;/body&gt;
</pre>
<pre class="html" id="codePreview">&lt;body&gt;
    <div>
        &lt;%Using (Html.BeginForm(&quot;SaveSession&quot;, &quot;Home&quot;, FormMethod.Post))
        %&gt;
        <p>&lt;%=ViewData(&quot;sessionString&quot;)%&gt; </p>
        &lt;fieldset&gt;
            <div>
                The text will be stored: &lt;%=Html.TextBox(&quot;sessionValue&quot;)%&gt;
            </div>
            <div>
                &lt;input type=&quot;submit&quot; id=&quot;submitToMethod1&quot; name=&quot;submit&quot; value=&quot;submit&quot; /&gt;
            </div>
        &lt;/fieldset&gt;
        &lt;%           
        End Using
        %&gt;
        &lt;%Using (Html.BeginForm(&quot;SaveSessionByExtensions&quot;, &quot;Home&quot;, FormMethod.Post))
        %&gt;
        <p>
            &lt;%=ViewData(&quot;sessionStringByExtensions&quot;)%&gt;
        </p>
        &lt;fieldset&gt;
            <div>
                The text will be stored: &lt;%=Html.TextBox(&quot;sessionValue&quot;)%&gt;
            </div>
            <div>
                &lt;input type=&quot;submit&quot; id=&quot;submitToMethod2&quot; name=&quot;submit&quot; value=&quot;submit&quot; /&gt;
            </div>
        &lt;/fieldset&gt;
        &lt;%           
        End Using
        %&gt;
    </div>
&lt;/body&gt;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">We create a</span><span style="font-size:11pt">n</span><span style="font-size:11pt"> enum named Al</span><span style="font-size:11pt">l</span><span style="font-size:11pt">ViewsNames to switch
 the engine mode be</span><span style="font-size:11pt">t</span><span style="font-size:11pt">ween Razor and ASPX.</span><span>
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">#region All Views' names
       enum AllViewsNames
       {
           RazorIndex,
           ASPXIndex
       }
       #endregion
       static AllViewsNames currentViewEnum = AllViewsNames.ASPXIndex;
       // Current view name;
       string strCurrentView = currentViewEnum == AllViewsNames.RazorIndex ? &quot;Index&quot; : &quot;TestPage&quot;;
</pre>
<pre class="csharp" id="codePreview">#region All Views' names
       enum AllViewsNames
       {
           RazorIndex,
           ASPXIndex
       }
       #endregion
       static AllViewsNames currentViewEnum = AllViewsNames.ASPXIndex;
       // Current view name;
       string strCurrentView = currentViewEnum == AllViewsNames.RazorIndex ? &quot;Index&quot; : &quot;TestPage&quot;;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">You can run and debug it.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">More Information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">HttpSessionStateBase Class</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.web.httpsessionstatebase.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.web.httpsessionstatebase.aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">HttpContext Class</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.web.httpcontext(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.web.httpcontext(v=vs.110).aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">HttpContext.Current Property</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.web.httpcontext.current(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.web.httpcontext.current(v=vs.110).aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">HttpContext.Session Property</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/vstudio/system.web.httpcontext.session(v=vs.100).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/vstudio/system.web.httpcontext.session(v=vs.100).aspx</span></a><span style="color:#0563c1; text-decoration:underline">
<br>
</span><span style="font-size:11pt">Controller.RedirectToAction Method</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.web.mvc.controller.redirecttoaction(v=vs.118).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.web.mvc.controller.redirecttoaction(v=vs.118).aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">FormExtensions.BeginForm Method</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.web.mvc.html.formextensions.beginform(v=vs.118).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.web.mvc.html.formextensions.beginform(v=vs.118).aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
