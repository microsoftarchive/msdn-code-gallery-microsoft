# Background worker thread in ASP.NET (CSASPNETBackgroundWorker)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- threading
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>ASP.NET APPLICATION : CSASPNETBackgroundWorker Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
Sometimes we do an operation which needs long time to complete. It will <br>
stop the response and the page is blank until the operation finished. <br>
In this case, We want the operation to run in the background, and in the page, <br>
we want to display the progress of the running operation. Therefore, the user <br>
can know the operation is running and can know the progress.<br>
<br>
On the other hand, we want to schedule some operations (send email/report ect.).<br>
We want the operations can be run at the specific time. <br>
<br>
This project creates a class named &quot;BackgroundWorker&quot; to achieve these goals. It<br>
creates a page named &quot;Default.aspx&quot; to run the long time operation. And it
<br>
creates a Background Worker to do the schedule when application starts up, then<br>
it uses &quot;GlobalBackgroundWorker.aspx&quot; page to check the progress.<br>
<br>
</p>
<h3>Demo the Sample:</h3>
<p style="font-family:Courier New"><br>
1. Session based Background Worker.<br>
&nbsp; &nbsp;a. Open Default.aspx, then click the button to run the operation in background.<br>
&nbsp; &nbsp;b. Open Default.aspx in two browsers, then click the buttons at the same time.<br>
&nbsp; &nbsp; &nbsp; You will see that two Background Workers work independently.<br>
<br>
2. Application Level Background Worker.<br>
&nbsp; &nbsp;a. Open GlobalBackgroundWorker.aspx. You will see that the Background Worker<br>
&nbsp; &nbsp; &nbsp; is running.<br>
&nbsp; &nbsp;b. Close the browser, wait for 10 seconds and then open the page again.
<br>
&nbsp; &nbsp; &nbsp; You will see that the Background Worker is still running even we closed
<br>
&nbsp; &nbsp; &nbsp; the browser.<br>
</p>
<h3>Code Logical:</h3>
<p style="font-family:Courier New"><br>
1. Open the class named &quot;BackgroundWorker&quot;. You will see that it starts an<br>
&nbsp; operation (method) in a separated thread.<br>
<br>
&nbsp; &nbsp;_innerThread = new Thread(() =&gt;<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;_progress = 0;<br>
&nbsp; &nbsp; &nbsp; &nbsp;DoWork.Invoke(ref _progress, ref _result, arguments);<br>
&nbsp; &nbsp; &nbsp; &nbsp;_progress = 100;<br>
&nbsp; &nbsp;});<br>
&nbsp; &nbsp;_innerThread.Start();<br>
<br>
2. In the page named &quot;Default.aspx&quot;. It uses UpdatePanel to achieve partial
<br>
&nbsp; refreshing. And it uses Timer control to update the operation progress.<br>
<br>
&nbsp; &nbsp;&lt;!-- UpdateUpanel let the progress can be updated without updating the whole page (partial update). --&gt;<br>
&nbsp; &nbsp;&lt;asp:UpdatePanel ID=&quot;UpdatePanel1&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;ContentTemplate&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;!-- The timer which used to update the progress. --&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Timer ID=&quot;Timer1&quot; runat=&quot;server&quot; Interval=&quot;100&quot; Enabled=&quot;false&quot; ontick=&quot;Timer1_Tick&quot;&gt;&lt;/asp:Timer&gt;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;!-- The Label which used to display the progress and the result --&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Label ID=&quot;lbProgress&quot; runat=&quot;server&quot; Text=&quot;&quot;&gt;&lt;/asp:Label&gt;&lt;br /&gt;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;!-- Start the operation by inputting value and clicking the button --&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Input a parameter: &lt;asp:TextBox ID=&quot;txtParameter&quot; runat=&quot;server&quot; Text=&quot;Hello World&quot;&gt;&lt;/asp:TextBox&gt;&lt;br /&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Button ID=&quot;btnStart&quot; runat=&quot;server&quot; Text=&quot;Click to Start the Background Worker&quot; onclick=&quot;btnStart_Click&quot; /&gt;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/ContentTemplate&gt;<br>
&nbsp; &nbsp;&lt;/asp:UpdatePanel&gt;<br>
<br>
&nbsp; &nbsp;In btnStart_Click() method which handles button click event. It creates<br>
&nbsp; &nbsp;a Background Worker and saves it to Session State.<br>
&nbsp; &nbsp;So that the Background Worker is bound to current Session.<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;BackgroundWorker worker = new BackgroundWorker();<br>
&nbsp; &nbsp; &nbsp; &nbsp;worker.DoWork &#43;= new BackgroundWorker.DoWorkEventHandler(worker_DoWork);<br>
&nbsp; &nbsp; &nbsp; &nbsp;worker.RunWorker(txtParameter.Text);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// It needs Session Mode is &quot;InProc&quot;<br>
&nbsp; &nbsp; &nbsp; &nbsp;// to keep the Background Worker working.<br>
&nbsp; &nbsp; &nbsp; &nbsp;Session[&quot;worker&quot;] = worker;<br>
<br>
3. In the Global class, you will find Application_Start() method creates a <br>
&nbsp; Background Worker and then saves it to Application State. Therefore, the<br>
&nbsp; Background Worker will keep running in background and it is shared by all<br>
&nbsp; users.<br>
&nbsp; &nbsp; &nbsp; &nbsp;BackgroundWorker worker = new BackgroundWorker();<br>
&nbsp; &nbsp; &nbsp; &nbsp;worker.DoWork &#43;= new BackgroundWorker.DoWorkEventHandler(worker_DoWork);<br>
&nbsp; &nbsp; &nbsp; &nbsp;worker.RunWorker(null);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// This Background Worker is Applicatoin Level,<br>
&nbsp; &nbsp; &nbsp; &nbsp;// so it will keep working and it is shared by all users.<br>
&nbsp; &nbsp; &nbsp; &nbsp;Application[&quot;worker&quot;] = worker;<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
Using Threads and Threading<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/e1dx6b2h.aspx">http://msdn.microsoft.com/en-us/library/e1dx6b2h.aspx</a><br>
<br>
UpdatePanel Control Overview<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/bb386454.aspx">http://msdn.microsoft.com/en-us/library/bb386454.aspx</a><br>
<br>
Timer Control Overview<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/bb398865.aspx">http://msdn.microsoft.com/en-us/library/bb398865.aspx</a><br>
<br>
Events (C# Programming Guide)<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/awbftdfh.aspx">http://msdn.microsoft.com/en-us/library/awbftdfh.aspx</a><br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
