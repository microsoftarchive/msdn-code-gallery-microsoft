# Show spinner when loading large data in ASP.NET (CSASPNETShowSpinnerImage)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- AJAX
- ASP.NET
## Topics
- Spinner Image
## Updated
- 07/12/2011
## Description

<p style="font-family:Courier New"></p>
<h2>CSASPNETShowSpinnerImage Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
This project illustrates how to show spinner image while retrieving huge <br>
amount of data. As we know, handle a time-consuming operate always requiring <br>
a long time, we need to show a spinner image for better user experience.<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
Please follow these demonstration steps below.<br>
<br>
Step 1: Open the CSASPNETShowSpinnerImage.sln.<br>
<br>
Step 2: Expand the CSASPNETShowSpinnerImage web application and press <br>
&nbsp; &nbsp; &nbsp; &nbsp;Ctrl &#43; F5 to show the Default.aspx.<br>
<br>
Step 3: You will see the date time and a button on Default.aspx page, please<br>
&nbsp; &nbsp; &nbsp; &nbsp;click the button to retrieve data from XML file.<br>
<br>
Step 4: The application will show a popup for displaying spinner image, after <br>
&nbsp; &nbsp; &nbsp; &nbsp;several seconds, you can find the data is been shown in GridView
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;control.<br>
<br>
Step 5: Validation finished.<br>
<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
Step 1. Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2010 or<br>
&nbsp; &nbsp; &nbsp; &nbsp;Visual Web Developer 2010. Name it as &quot;CSASPNETShowSpinnerImage&quot;.
<br>
<br>
Step 2. Add a web form in the root directory of application, name it as <br>
&nbsp; &nbsp; &nbsp; &nbsp;&quot;Default.aspx&quot;.<br>
<br>
Step 3. Add three folders, &quot;Image&quot;, &quot;UserControl&quot;, &quot;XMLFile&quot;. The &quot;Image&quot; folder<br>
&nbsp; &nbsp; &nbsp; &nbsp;includes image files that you want to show. The &quot;UserControl&quot; folder
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;includes User Controls. The XML file includes XML files as the data<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;source of GridView control of Default page.<br>
<br>
Step 4. The Default web form page includes an UpdatePanel control and an <br>
&nbsp; &nbsp; &nbsp; &nbsp;UpdateProgress control. UpdatePanel Control includes retrieve data button<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;and GridView, the UpdateProgeress control includes PopupProgress user<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;control. The HTML code of Default page will be like this:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;head id=&quot;Head1&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;title&gt;&lt;/title&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <style type="text/css"> <br>
          .modalBackground <br>
          { <br>
              background-color: Gray; <br>
              opacity: 0.7; <br>
          } <br>
       </style> <br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/head&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;body id=&quot;body1&quot; runat=&quot;server&quot; &gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &lt;form id=&quot;form1&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &lt;div&gt; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:ToolkitScriptManager ID=&quot;ToolkitScriptManagerPopup&quot; runat=&quot;server&quot; /&gt; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:UpdatePanel ID=&quot;updatePanel&quot; UpdateMode=&quot;Conditional&quot; runat=&quot;server&quot;&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;ContentTemplate&gt; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;%=DateTime.Now.ToString() %&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;br /&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Button ID=&quot;btnRefresh&quot; runat=&quot;server&quot; Text=&quot;Refresh Panel&quot;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; OnClick=&quot;btnRefresh_Click&quot;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; OnClientClick=&quot;document.getElementById('PopupProgressUserControl_btnLink').click();&quot; /&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;br /&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:GridView ID=&quot;gvwXMLData&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/asp:GridView&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/ContentTemplate&gt; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/asp:UpdatePanel&gt; <br>
&nbsp; &nbsp; &nbsp; <br>
&nbsp; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:UpdateProgress ID=&quot;updateProgress&quot; runat=&quot;server&quot; AssociatedUpdatePanelID=&quot;updatePanel&quot;&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;ProgressTemplate&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;uc1:PopupProgress ID=&quot;PopupProgressUserControl&quot; runat=&quot;server&quot; /&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/ProgressTemplate&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/asp:UpdateProgress&gt; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &lt;/div&gt; &nbsp; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &lt;/form&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/body&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[/code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
Step 5 &nbsp;The btnRefresh Click event code in Default.aspx.cs file.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp; &nbsp; &nbsp; &nbsp; protected void btnRefresh_Click(object sender, EventArgs e)<br>
&nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Here we use Thread.Sleep() to suspends the thread for 10 seconds for imitating<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// an expensive, time-consuming operate of retrieve data. (Such as connect network<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// database to retrieve mass data.)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// So in practice application, you can remove this line.
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Threading.Thread.Sleep.aspx" target="_blank" title="Auto generated link to System.Threading.Thread.Sleep">System.Threading.Thread.Sleep</a>(10000);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Retrieve data from XML file as sample data.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;XmlDocument xmlDocument = new XmlDocument();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory &#43; &quot;XMLFile/XMLData.xml&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataTable tabXML = new DataTable();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataColumn columnName = new DataColumn(&quot;Name&quot;, Type.GetType(&quot;System.String&quot;));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataColumn columnAge = new DataColumn(&quot;Age&quot;, Type.GetType(&quot;System.Int32&quot;));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataColumn columnCountry = new DataColumn(&quot;Country&quot;, Type.GetType(&quot;System.String&quot;));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataColumn columnComment = new DataColumn(&quot;Comment&quot;, Type.GetType(&quot;System.String&quot;));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;tabXML.Columns.Add(columnName);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;tabXML.Columns.Add(columnAge);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;tabXML.Columns.Add(columnCountry);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;tabXML.Columns.Add(columnComment);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;XmlNodeList nodeList = xmlDocument.SelectNodes(&quot;Root/Person&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;foreach (XmlNode node in nodeList)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataRow row = tabXML.NewRow();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;row[&quot;Name&quot;] = node.SelectSingleNode(&quot;Name&quot;).InnerText;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;row[&quot;Age&quot;] = node.SelectSingleNode(&quot;Age&quot;).InnerText;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;row[&quot;Country&quot;] = node.SelectSingleNode(&quot;Country&quot;).InnerText;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;row[&quot;Comment&quot;] = node.SelectSingleNode(&quot;Comment&quot;).InnerText;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;tabXML.Rows.Add(row);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;gvwXMLData.DataSource = tabXML;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;gvwXMLData.DataBind();<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;[/code]<br>
<br>
Step 6. The PopupPregress user control is used to show a popup by ASP.NET Ajax control<br>
&nbsp; &nbsp; &nbsp; &nbsp;ModalPopupExtender. The ModalPopupExtender can show a Panel when target button<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;has been clicked. The HTML code of PopupProgress control as shown below:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;%@ Register Assembly=&quot;AjaxControlToolkit&quot; Namespace=&quot;AjaxControlToolkit&quot; TagPrefix=&quot;asp&quot; %&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;script language=&quot;javascript&quot; type=&quot;text/javascript&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;% =LoadImage() %&gt;<br>
&nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp;// The JavaScript function can shows loaded image in Image control.<br>
&nbsp; &nbsp; &nbsp; &nbsp;var imgStep = 0;<br>
&nbsp; &nbsp; &nbsp; &nbsp;function slide()<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var img = document.getElementById(&quot;PopupProgressUserControl_imgProgress&quot;); &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (document.all)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;img.filters.blendTrans.apply();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;} <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;img.title=imgMessage[imgStep]; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;img.src=imgUrl[imgStep]; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (document.all)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;img.filters.blendTrans.play();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;imgStep = (imgStep &lt; (imgUrl.length-1)) ? (imgStep &#43; 1) : 0;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;(new Image()).src = imgUrl[imgStep];<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;setInterval(&quot;slide()&quot;,1000);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/script&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Panel ID=&quot;pnlProgress&quot; runat=&quot;server&quot; CssClass=&quot;modalpopup&quot;&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;div class=&quot;popupcontainerLoading&quot;&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;div class=&quot;popupbody&quot;&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;table width=&quot;100%&quot;&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;tr&gt; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;td align=&quot;center&quot;&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Image ID=&quot;imgProgress&quot; runat=&quot;server&quot; style=&quot;filter: blendTrans(duration=0.618)&quot; &nbsp;ImageUrl=&quot;~/Image/0.jpg&quot;/&gt; &nbsp; &nbsp;
 &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/td&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/tr&gt; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;tr&gt; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Button ID=&quot;btnCancel&quot; runat=&quot;server&quot; Text=&quot;Cancel&quot; /&gt;&lt;/td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/table&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &lt;/div&gt; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &lt;/div&gt; <br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/asp:Panel&gt; <br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:LinkButton ID=&quot;btnLink&quot; runat=&quot;server&quot; Text=&quot;&quot;&gt;&lt;/asp:LinkButton&gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:ModalPopupExtender ID=&quot;mpeProgress&quot; runat=&quot;server&quot; TargetControlID=&quot;btnLink&quot;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;X=&quot;500&quot; Y=&quot;0&quot; PopupControlID=&quot;pnlProgress&quot; BackgroundCssClass=&quot;modalBackground&quot; DropShadow=&quot;true&quot; &nbsp;CancelControlID=&quot;btnCancel&quot; &gt;
<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/asp:ModalPopupExtender&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[/code]<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;The PopupProgress.ascx.cs file:<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/// &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// This method is used to load images of customize files and
<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// register JavaScript code on User Control page.<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;returns&gt;&lt;/returns&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;public string LoadImage()<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;StringBuilder strbScript = new StringBuilder();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string imageUrl = &quot;&quot;;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strbScript.Append(&quot;var imgMessage = new Array();&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strbScript.Append(&quot;var imgUrl = new Array();&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string[] strs = new string[7];<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strs[0] = &quot;Image/0.jpg&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strs[1] = &quot;Image/1.jpg&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strs[2] = &quot;Image/2.jpg&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strs[3] = &quot;Image/3.jpg&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strs[4] = &quot;Image/4.jpg&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strs[5] = &quot;Image/5.jpg&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strs[6] = &quot;Image/6.jpg&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;for (int i = 0; i &lt; strs.Length; i&#43;&#43;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;imageUrl = strs[i];<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strbScript.Append(String.Format(&quot;imgUrl[{0}] = '{1}';&quot;, i, imageUrl));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strbScript.Append(String.Format(&quot;imgMessage[{0}] = '{1}';&quot;, i, imageUrl.Substring(imageUrl.LastIndexOf('.') - 1)));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strbScript.Append(&quot;for (var i=0; i&lt;imgUrl.length; i&#43;&#43;)&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;strbScript.Append(&quot;{ (new Image()).src = imgUrl[i]; }&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return strbScript.ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[/code]<br>
<br>
Step 7. Build the application and you can debug it.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: ModalPopupExtender experiences<br>
<a target="_blank" href="http://weblogs.asp.net/jgonzalez/archive/2007/03/02/modalpopupextender-experiences.aspx">http://weblogs.asp.net/jgonzalez/archive/2007/03/02/modalpopupextender-experiences.aspx</a><br>
<br>
MSDN: ASP.NET User Controls<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/y6wb1a0e.aspx">http://msdn.microsoft.com/en-us/library/y6wb1a0e.aspx</a><br>
<br>
MSDN: XmlDocument Class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.xml.xmldocument.aspx">http://msdn.microsoft.com/en-us/library/system.xml.xmldocument.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
