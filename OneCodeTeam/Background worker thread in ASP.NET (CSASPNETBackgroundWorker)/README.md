# Background worker thread in ASP.NET (CSASPNETBackgroundWorker)
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- threading
- backgroundworker
## Updated
- 12/09/2012
## Description

<h1>Background worker thread in ASP.NET (CSASPNETBackgroundWorker)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">Sometimes we need to do some time-consuming operations in web page. The web page will be freezed until the operation is finished. In this case, we want the operation to run in the background, and also we may want to display the progress
 of the running operation in&nbsp; web page. <span style="">And sometimes</span>, we may want to schedule some operations (send email/report ect.). In these cases, we hope the operations can be ran at the specific time.
</p>
<p class="MsoNormal">This project creates a class named &quot;BackgroundWorker&quot; to achieve these goals. It<span style="">
</span>creates a page named &quot;Default.aspx&quot; to run the long time operation. And it creates a Background Worker to do the schedule when application starts up, then<span style="">
</span>it uses &quot;GlobalBackgroundWorker.aspx&quot; page to check the progress.<span style="">
</span></p>
<h2>Running the Sample<span style=""> </span></h2>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.25in"><span style=""><span style="">Step 1:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Open the CSASPNETBackgroundWorker.sln.<span style=""> </span>
</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.25in"><span style=""><span style="">Step 2:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Expand the CSASPNETBackgroundWorker web application and press Ctrl &#43; F5 to show the Default.aspx.
</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.25in"><span style=""><span style="">Step 3:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">Session based Background Worker.<br>
<br>
a. Open Default.aspx, and then click the button to run the operation in background.<span style="">
<img src="72280-image.png" alt="" width="784" height="320" align="middle">
</span><br>
b. Open Default.aspx in two browsers then click the buttons at the same time. You will see that two Background Workers work independently.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.25in"><span style=""><span style="">Step 4:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">Application Level Background Worker.<br>
<br>
a. Open GlobalBackgroundWorker.aspx. You will see that the Background Worker is running.<span style="">
<img src="72281-image.png" alt="" width="863" height="138" align="middle">
</span><br>
b. Close the browser, wait for 10 seconds and then open the page again. . You will see that the Background Worker is still running even we closed the browser.</span></p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.25in"><span style=""><span style="">Step 5:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Validation finished.</p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.25in"><span style=""><span style="">Step 1:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2012 or Visual Web Developer 2012. Name it as &quot;CSASPNETBackgroundWorker&quot;.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.25in"><span style=""><span style="">Step 2:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">&nbsp;</span>Add two web forms in the root directory, name them as &quot;Default.aspx&quot;, &quot;<span style=""> GlobalBackgroundWorker</span>.aspx&quot;.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.25in"><span style=""><span style="">Step 3:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">Create a class file and name it as &quot;BackgroundWorker&quot;. It starts an operation (method) in a separated thread.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
_innerThread = new Thread(() =&gt;
&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; _progress = 0;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; DoWork.Invoke(ref _progress, ref _result, arguments);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; _progress = 100;
&nbsp;&nbsp; });
&nbsp;&nbsp; _innerThread.Start();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:.25in"><span style=""><span style="">Step 4:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>In the page named &quot;Default.aspx&quot;. It uses UpdatePanel to achieve partial<span style=""> refreshing. And it uses Timer control to update the operation progress.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;!-- UpdateUpanel let the progress can be updated without updating the whole page (partial update). --&gt;
&nbsp;&nbsp;&nbsp; &lt;asp:UpdatePanel ID=&quot;UpdatePanel1&quot; runat=&quot;server&quot;&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;ContentTemplate&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;!-- The timer which used to update the progress. --&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;asp:Timer ID=&quot;Timer1&quot; runat=&quot;server&quot; Interval=&quot;100&quot; Enabled=&quot;false&quot; ontick=&quot;Timer1_Tick&quot;&gt;&lt;/asp:Timer&gt;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;!-- The Label which used to display the progress and the result --&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;asp:Label ID=&quot;lbProgress&quot; runat=&quot;server&quot; Text=&quot;&quot;&gt;&lt;/asp:Label&gt;<br>


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;!-- Start the operation by inputting value and clicking the button --&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Input a parameter: &lt;asp:TextBox ID=&quot;txtParameter&quot; runat=&quot;server&quot; Text=&quot;Hello World&quot;&gt;&lt;/asp:TextBox&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;asp:Button ID=&quot;btnStart&quot; runat=&quot;server&quot; Text=&quot;Click to Start the Background Worker&quot; onclick=&quot;btnStart_Click&quot; /&gt;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/ContentTemplate&gt;
&nbsp;&nbsp;&nbsp; &lt;/asp:UpdatePanel&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">In btnStart_Click() method which handles button click event. It creates a Background Worker and saves it to Session State. So that the Background Worker is bound to current Session.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
BackgroundWorker worker = new BackgroundWorker();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; worker.DoWork &#43;= new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; worker.RunWorker(txtParameter.Text);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // It needs Session Mode is &quot;InProc&quot;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // to keep the Background Worker working.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Session[&quot;worker&quot;] = worker;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:.25in"><span style=""><span style="">Step 5:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>In the Global class, you will find Application_Start() method creates a<span style=""> Background Worker and then saves it to Application State. Therefore, the Background Worker will keep running in background and it is shared by all users.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
BackgroundWorker worker = new BackgroundWorker();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; worker.DoWork &#43;= new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; worker.RunWorker(null);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // This Background Worker is Applicatoin Level,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // so it will keep working and it is shared by all users.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Application[&quot;worker&quot;] = worker;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:.25in"><span style=""><span style="">Step 6:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Build the application and you can debug it<span style="">.</span></p>
<h2>More Information</h2>
<p class="MsoNormal">Using Threads and Threading<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/e1dx6b2h.aspx">http://msdn.microsoft.com/en-us/library/e1dx6b2h.aspx</a><span style=""><br>
</span>UpdatePanel Control Overview<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/bb386454.aspx">http://msdn.microsoft.com/en-us/library/bb386454.aspx</a><span style=""><br>
</span>Timer Control Overview<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/bb398865.aspx">http://msdn.microsoft.com/en-us/library/bb398865.aspx</a><span style=""><br>
</span>Events (C# Programming Guide)<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/awbftdfh.aspx">http://msdn.microsoft.com/en-us/library/awbftdfh.aspx</a><span style="">
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
