# How to create a real-time chat UWP app using SignalR
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- SignalR
- ASP.NET SignalR
- OWIN self-hosting
- Universal Windows App Development
- Universal Windows Platform
- UWP
## Topics
- SignalR
- Universal Windows Platform
- UWP
## Updated
- 11/18/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><a name="OLE_LINK1"></a><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">How to create a real-time chat app in Universal Windows Platform using ASP.NET SignalR</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Introduction
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>This sample demonstrates how to create a real-time chat app in Universal Windows Platform using ASP.NET SignalR.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>SignalR allows bi-directional communication between server and client. Servers can now push the&nbsp;content to&nbsp;instantly connect clients&nbsp;when it becomes available.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>In this sample, we will create a self-host SignalR console app as the server and a UWP app as the client.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Building the sample</span></span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><span style="font-size:12pt"><span style="font-size:10pt">This sample should be run in Microsoft Visual Studio 2015 version and Windows 10.</span></span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><span style="font-size:12pt"><span style="font-size:10pt">Before you build
</span><span style="font-size:10pt">the project, make sure you have restored all</span><span>&nbsp;</span><span style="font-size:10pt">the</span><span style="font-size:10pt"> package</span><span style="font-size:10pt">s</span><span style="font-size:10pt"> in
 the project. </span></span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><span style="font-size:12pt"><span style="font-size:10pt"><img src="163665-image.png" alt="" width="609" height="527" align="middle">
</span><a name="_GoBack"></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt">&bull;&nbsp;</span><span style="font-size:10pt">Open solution in Visual Studio 2015.</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt">&bull;&nbsp;</span><span style="font-size:10pt">Right-click on the solution, and select Set Startup Projects.... Select the Multiple startup projects radio button, and set both projects' Action to Start.</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:12pt"><span style="font-size:10pt"><img src="163666-image.png" alt="" width="650" height="441" align="middle">
</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt">&bull;&nbsp;</span><span style="font-size:10pt">Then do one of the following:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Click the Start Debugging button on the toolbar.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Click Start Debugging in the Debug menu.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Press F5.</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt">&bull;&nbsp;</span><span style="font-size:10pt">Input user name and group name, then click
</span><span style="font-weight:bold; font-size:10pt">Join</span><span style="font-size:10pt"> button.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span><span><img src="163667-image.png" alt="" width="593" height="427" align="middle">
</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span>&bull;&nbsp;</span><span>In the chat page, input messages and then click
</span><span style="font-weight:bold">Send</span><span style="font-weight:bold">.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span><span><img src="163668-image.png" alt="" width="593" height="427" align="middle">
</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span>&bull;&nbsp;</span><span>Click </span><span style="font-weight:bold">Exit</span><span style="font-weight:bold">
</span><span>to exit the chat room.</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>CSSignalRHost project</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
ChatHub class</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">[HubName(<span class="cs__string">&quot;ChatHub&quot;</span>)]&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;ChatHub&nbsp;:&nbsp;Hub&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;add&nbsp;connection&nbsp;to&nbsp;group</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;groupName&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;returns&gt;&lt;/returns&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;Task&nbsp;JoinGroup(<span class="cs__keyword">string</span>&nbsp;groupName)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;Groups.Add(Context.ConnectionId,&nbsp;groupName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;remove&nbsp;connection&nbsp;from&nbsp;group</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;groupName&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;returns&gt;&lt;/returns&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;Task&nbsp;LeaveGroup(<span class="cs__keyword">string</span>&nbsp;groupName)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;Groups.Remove(Context.ConnectionId,&nbsp;groupName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;send&nbsp;message&nbsp;to&nbsp;the&nbsp;connections&nbsp;in&nbsp;the&nbsp;group.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;groupName&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;userName&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;message&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;sendTime&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;SendToGroup(<span class="cs__keyword">string</span>&nbsp;groupName,&nbsp;<span class="cs__keyword">string</span>&nbsp;userName,&nbsp;<span class="cs__keyword">string</span>&nbsp;message,&nbsp;DateTime&nbsp;sendTime)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Clients.Group(groupName).ReceiveMessage(userName,&nbsp;message,&nbsp;sendTime);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">Startup class</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">class</span>&nbsp;Startup&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Configuration(IAppBuilder&nbsp;app)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;app.UseCors(CorsOptions.AllowAll);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;app.MapSignalR();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">CSUWPSignalRClient</div>
<div class="endscriptcode">Invoke SignalR method in App()</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;SignalR()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Connect&nbsp;to&nbsp;the&nbsp;url</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;MyHubConnection&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HubConnection(<span class="cs__string">&quot;http://127.0.0.1:8080&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//ChatHub&nbsp;is&nbsp;the&nbsp;hub&nbsp;name&nbsp;defined&nbsp;in&nbsp;the&nbsp;host&nbsp;program.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;MyHubProxy&nbsp;=&nbsp;MyHubConnection.CreateHubProxy(<span class="cs__string">&quot;ChatHub&quot;</span>);&nbsp;
}<span class="cs__preproc">&nbsp;
&nbsp;
#endregion</span>&nbsp;
&nbsp;
<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;Initializes&nbsp;the&nbsp;singleton&nbsp;application&nbsp;object.&nbsp;&nbsp;This&nbsp;is&nbsp;the&nbsp;first&nbsp;line&nbsp;of&nbsp;authored&nbsp;code</span>&nbsp;
<span class="cs__com">///&nbsp;executed,&nbsp;and&nbsp;as&nbsp;such&nbsp;is&nbsp;the&nbsp;logical&nbsp;equivalent&nbsp;of&nbsp;main()&nbsp;or&nbsp;WinMain().</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
<span class="cs__keyword">public</span>&nbsp;App()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.InitializeComponent();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.Suspending&nbsp;&#43;=&nbsp;OnSuspending;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Initialize&nbsp;hub&nbsp;connection.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SignalR();&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">Connect to hub and join in the group&nbsp;</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;async&nbsp;<span class="cs__keyword">void</span>&nbsp;btnJoin_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;RoutedEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;btnJoin.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">false</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;tbxError.Text&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Empty;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;groupName&nbsp;=&nbsp;tbxGroup.Text.Trim();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;userName&nbsp;=&nbsp;tbxName.Text.Trim();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(groupName.Length&nbsp;==&nbsp;<span class="cs__number">0</span>&nbsp;||&nbsp;userName.Length&nbsp;==&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbxError.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Group&nbsp;&amp;&nbsp;user&nbsp;name&nbsp;can't&nbsp;be&nbsp;empty.&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;btnJoin.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Connect&nbsp;to&nbsp;hub</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;App&nbsp;myApp&nbsp;=&nbsp;(Application.Current&nbsp;<span class="cs__keyword">as</span>&nbsp;App);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(myApp.MyHubConnection.State&nbsp;!=&nbsp;ConnectionState.Connected)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;myApp.MyHubConnection.Start();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbxError.Text&nbsp;=&nbsp;$<span class="cs__string">&quot;Can't&nbsp;connect&nbsp;to&nbsp;server&nbsp;{myApp.MyHubConnection.Url}&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;btnJoin.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//join&nbsp;to&nbsp;group</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(myApp.MyHubConnection.State&nbsp;==&nbsp;Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;myApp.MyHubProxy.Invoke(<span class="cs__string">&quot;JoinGroup&quot;</span>,&nbsp;groupName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dynamic&nbsp;info&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;{&nbsp;groupName&nbsp;=&nbsp;groupName,&nbsp;userName&nbsp;=&nbsp;userName&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Frame.Navigate(<span class="cs__keyword">typeof</span>(ChatRoom),&nbsp;info);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbxError.Text&nbsp;=&nbsp;$<span class="cs__string">&quot;Can't&nbsp;connect&nbsp;to&nbsp;server&nbsp;{myApp.MyHubConnection.Url}&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;btnJoin.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">Receive message handler</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;async&nbsp;<span class="cs__keyword">void</span>&nbsp;ReceiveMessage(<span class="cs__keyword">string</span>&nbsp;userName,&nbsp;<span class="cs__keyword">string</span>&nbsp;message,DateTime&nbsp;sendTime)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,&nbsp;()&nbsp;=&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.lvwMessages.Items.Add($<span class="cs__string">&quot;{sendTime.ToString(&quot;</span>MM-dd&nbsp;HH:mm:ss<span class="cs__string">&quot;)}\n{userName}:&nbsp;{message}&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;});&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">More information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://www.asp.net/signalr/overview/getting-started"><span style="color:#0563c1; text-decoration:underline">ASP.NET SignalR</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
