# How to determine which Control caused PostBack
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Postback
## Updated
- 07/05/2013
## Description

<h1>Determine which control causes the PostBack event on an ASP.NET page (CSASPNETControlCausePostback)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample code demonstrates how to create a web application that can determine which control causes the postback event on an Asp.net page. Sometimes, we need to perform some specific actions based on the specific control which causes
 the postback. For example, we can get controls' id property that and do some operations, such as set TextBox's text with ViewState variable. In this sample, we can also transfer some data through postbacks.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Please follow these demonstration steps below. </p>
<p class="MsoNormal">Step 1:&nbsp;Open the CSASPNETControlCausePostback.sln. Expand the
<a name="OLE_LINK2"></a><a name="OLE_LINK1"><span style="">CSASPNETControlCausePostback
</span></a>web application and press Ctrl &#43; F5 to show the Default.aspx. </p>
<p class="MsoNormal">Step 2: We will see only some html controls on the page, two buttons, a checkbox and a Dropdownlist. In this page, we have added JavaScript functions that can cause postback event, so just click them (or select an item in the Dropdownlist
 control) for a try.</p>
<p class="MsoNormal"><span style=""><img src="91827-image.png" alt="" width="533" height="484" align="middle">
</span></p>
<p class="MsoNormal">Step 3:<span style="">&nbsp; </span>When you click the Button A, you can find the page is refreshed and some information has been added at the bottom of the page.</p>
<p class="MsoNormal"><span style=""><img src="91828-image.png" alt="" width="533" height="484" align="middle">
</span></p>
<p class="MsoNormal">Step 4: You can click other controls, the application can find which control causes the postback event and then display transfer data, for example, if you click CheckBox C:<span style="">
</span></p>
<p class="MsoNormal"><span style=""><img src="91829-image.png" alt="" width="533" height="484" align="middle">
</span></p>
<p class="MsoNormal">Step 5: View the Default2.aspx page with above steps, too. The Default2 web page shows how to make this function working with Server controls (Asp.net controls).</p>
<p class="MsoNormal">Step 6: Validation finished.</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Code Logical: </p>
<p class="MsoNormal">Step 1. Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2008 or Visual Web Developer 2008. Name it as &quot;CSASPNETControlCausePostback &quot;. Add two webform pages and name them as &quot;Default.aspx&quot; and
 &quot;Default2.aspx.</p>
<p class="MsoNormal">Step 2. We need to use JavaScript functions to cause postback function with two parameters, __EVENTTARGET and __EVENTARGUMENT, the former one is controls' id property, and the latter one is postback data, you can add the message as you
 like when you try to call this function. The Default page and Default2 page are different, Default page includes some HTML controls that we have to add __doPostBack function by ourselves, and we also need to manager the control's state. HTML control will lost
 their states while crossing postback events, for example, the CheckBox's checked property will recover to initial state if you do not handle their state.</p>
<h3>The following code shows how to persist controls' states when page is loading.</h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>
<pre class="hidden">
&lt;script type=&quot;text/javascript&quot;&gt;
    function load() {
        var checkbox = document.getElementById(&quot;chkC&quot;);
        var hiddenCheckBox = document.getElementById(&quot;checkbox&quot;);
        if (hiddenCheckBox.value == &quot;true&quot;) {
            checkbox.checked = true;
        }
        else if (hiddenCheckBox.value == &quot;false&quot;) {
            checkbox.checked = false;
        }
        else {
            checkbox.checked = false;
        }
        var dropdownlist = document.getElementById(&quot;ddlD&quot;);
        var hiddenDropdownlist = document.getElementById(&quot;dropdownlist&quot;);
        var index = parseInt(hiddenDropdownlist.value);
        dropdownlist.selectedIndex = index;
    }
&lt;/script&gt;

</pre>
<pre id="codePreview" class="js">
&lt;script type=&quot;text/javascript&quot;&gt;
    function load() {
        var checkbox = document.getElementById(&quot;chkC&quot;);
        var hiddenCheckBox = document.getElementById(&quot;checkbox&quot;);
        if (hiddenCheckBox.value == &quot;true&quot;) {
            checkbox.checked = true;
        }
        else if (hiddenCheckBox.value == &quot;false&quot;) {
            checkbox.checked = false;
        }
        else {
            checkbox.checked = false;
        }
        var dropdownlist = document.getElementById(&quot;ddlD&quot;);
        var hiddenDropdownlist = document.getElementById(&quot;dropdownlist&quot;);
        var index = parseInt(hiddenDropdownlist.value);
        dropdownlist.selectedIndex = index;
    }
&lt;/script&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h3>The following code shows how to cause postback event and save control's current state.</h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>
<pre class="hidden">
&lt;script type=&quot;text/javascript&quot;&gt;
      var theForm = document.forms['form1'];
      if (!theForm) {
          theForm = document.form1;


          function PostBack(controlName, postbackData) {
              var name = controlName;
              var data = postbackData;
              var checkbox = document.getElementById(&quot;chkC&quot;);
              var hiddenCheckBox = document.getElementById(&quot;checkbox&quot;);
              var dropdownlist = document.getElementById(&quot;ddlD&quot;);
              var hiddenDropdownlist = document.getElementById(&quot;dropdownlist&quot;);
              if (checkbox.checked) {
                  hiddenCheckBox.value = &quot;true&quot;;
              }
              else {
                  hiddenCheckBox.value = &quot;false&quot;;
              }
              hiddenDropdownlist.value = dropdownlist.selectedIndex;
              __doPostBack(name, data);
          }




      }
      function __doPostBack(eventTarget, eventArgument) {
          theForm.__EVENTTARGET.value = eventTarget;
          theForm.__EVENTARGUMENT.value = eventArgument;
          theForm.submit();
      }
  &lt;/script&gt;

</pre>
<pre id="codePreview" class="js">
&lt;script type=&quot;text/javascript&quot;&gt;
      var theForm = document.forms['form1'];
      if (!theForm) {
          theForm = document.form1;


          function PostBack(controlName, postbackData) {
              var name = controlName;
              var data = postbackData;
              var checkbox = document.getElementById(&quot;chkC&quot;);
              var hiddenCheckBox = document.getElementById(&quot;checkbox&quot;);
              var dropdownlist = document.getElementById(&quot;ddlD&quot;);
              var hiddenDropdownlist = document.getElementById(&quot;dropdownlist&quot;);
              if (checkbox.checked) {
                  hiddenCheckBox.value = &quot;true&quot;;
              }
              else {
                  hiddenCheckBox.value = &quot;false&quot;;
              }
              hiddenDropdownlist.value = dropdownlist.selectedIndex;
              __doPostBack(name, data);
          }




      }
      function __doPostBack(eventTarget, eventArgument) {
          theForm.__EVENTTARGET.value = eventTarget;
          theForm.__EVENTARGUMENT.value = eventArgument;
          theForm.submit();
      }
  &lt;/script&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal"><b style="">[NOTE] </b></p>
<p class="MsoNormal"><b style="">The former part of JavaScript functions can be added in the &lt;Head&gt; tag because page's onload event will call this function, so you need to write the &lt;body&gt; HTML code like this: &lt;body onload=&quot;load();&quot;&gt;.
</b></p>
<p class="MsoNormal"><b style="">The latter part of JavaScript functions must be added after the &lt;form&gt; tag because the JavaScript function will get form and invoke form.submit() function, if you add these JavaScript code in the &lt;Head&gt; tag, the
 theForm variable will be null value. </b></p>
<p class="MsoNormal">Step 3. Then you can add HTML controls on the page like sample's Default.aspx, after that, we can write some C# code in code-behind file for retrieving control id and postback data.</p>
<h3>The following code is use to get JavaScript arguments across postback event. </h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
protected void Page_Load(object sender, EventArgs e)
{
    if (Page.IsPostBack)
    {
        StringBuilder builder = new StringBuilder();
        if (!String.IsNullOrEmpty(Request[&quot;__EVENTTARGET&quot;]) && !String.IsNullOrEmpty(Request[&quot;__EVENTARGUMENT&quot;]))
        {
            string target = Request[&quot;__EVENTTARGET&quot;] as string;
            string argument = Request[&quot;__EVENTARGUMENT&quot;] as string;
            builder.Append(&quot;Cause postback control:&quot;);
            builder.Append(&quot;<br>&quot;);
            builder.Append(target);
            builder.Append(&quot;<br>&quot;);
            builder.Append(&quot;<br>&quot;);
            builder.Append(&quot;Postback data:&quot;);
            builder.Append(&quot;<br>&quot;);
            builder.Append(argument);
            lbMessage.Text = builder.ToString();
        }


    }
}

</pre>
<pre id="codePreview" class="csharp">
protected void Page_Load(object sender, EventArgs e)
{
    if (Page.IsPostBack)
    {
        StringBuilder builder = new StringBuilder();
        if (!String.IsNullOrEmpty(Request[&quot;__EVENTTARGET&quot;]) && !String.IsNullOrEmpty(Request[&quot;__EVENTARGUMENT&quot;]))
        {
            string target = Request[&quot;__EVENTTARGET&quot;] as string;
            string argument = Request[&quot;__EVENTARGUMENT&quot;] as string;
            builder.Append(&quot;Cause postback control:&quot;);
            builder.Append(&quot;<br>&quot;);
            builder.Append(target);
            builder.Append(&quot;<br>&quot;);
            builder.Append(&quot;<br>&quot;);
            builder.Append(&quot;Postback data:&quot;);
            builder.Append(&quot;<br>&quot;);
            builder.Append(argument);
            lbMessage.Text = builder.ToString();
        }


    }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></p>
<p class="MsoNormal">Step 4: In Default2 web page, we use Server buttons instead of HTML controls, the page will generate __doPostBack function automatically, so you can use __doPostBack directly and needn't to maintain the control's state.</p>
<p class="MsoNormal">Step 5. Build the application and you can debug it.</p>
<p class="MsoNormal"></p>
<h2>More Information</h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.page.ispostback.aspx">Page.IsPostBack Property</a></p>
<p class="MsoNormal"></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
