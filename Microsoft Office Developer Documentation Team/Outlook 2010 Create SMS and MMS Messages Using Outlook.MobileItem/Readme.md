# Outlook 2010: Create SMS and MMS Messages Using Outlook.MobileItem
## Requires
- 
## License
- Apache License, Version 2.0
## Technologies
- Outlook 2010
- Office 2010
## Topics
- Office 2010 101 code samples
- text messaging
## Updated
- 08/04/2011
## Description

<h1>Introduction</h1>
<p><span style="font-size:small">This sample shows how to create Short Message Service (SMS) text messages or multimedia message (MMS) messages from within Microsoft Outlook 2010.</span></p>
<p><span style="font-size:small">This code snippet is part of the Office 2010 101 code samples project. This sample, along with others, is offered here to incorporate directly in your code.</span></p>
<p><span style="font-size:small">Each code sample consists of approximately 5 to 50 lines of code demonstrating a distinct feature or feature set, in either VBA or both VB and C# (created in Visual Studio 2010). Each sample includes comments describing the
 sample, and setup code so that you can run the code with expected results or the comments will explain how to set up the environment so that the sample code runs.)</span></p>
<p><span style="font-size:small">Microsoft&reg; Office 2010 gives you the tools needed to create powerful applications. The Microsoft Visual Basic for Applications (VBA) code samples can assist you in creating your own applications that perform specific functions
 or as a starting point to create more complex solutions.</span></p>
<h1><span>Building the Sample</span></h1>
<p><span style="font-size:small">Use this sample to create an SMS message or MMS message from Outlook 2010.</span></p>
<p><span style="font-size:small">An Outlook MobileItem represents an SMS message or MMS message that you send to a mobile device like a phone.</span><br>
<br>
<span style="font-size:small">To actually send the message, you'll need to have configured Outlook with a Windows phone running 6.1 Update or 6.5.</span><br>
<br>
<span style="font-size:small">OR</span><br>
<br>
<span style="font-size:small">You can use an SMS service on the Internet.</span></p>
<p><span style="font-size:small">This example creates two messages: one SMS and one MMS. It then shows the Inspector for each item. You can then choose to provide a recipient or cancel the messages once the code completes execution.</span></p>
<p><span style="font-size:small">Open the VBA editor and paste this code into the existing ThisOutlookSession module. With the cursor inside the DemoMobileItem method, press F5.</span></p>
<p><span style="font-size:20px; font-weight:bold">Description</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">Sub DemoMobileItem()
    Dim app As Outlook.Application
    Set app = ThisOutlookSession
    
   ' Change the following path to refer to a
   ' small picture on your computer.
   Const PICTURE_PATH As String = _
    &quot;C:\Users\Public\Pictures\Sample Pictures\Desert.jpg&quot;

    ' Create an SMS message and provide a body text value.
    Dim sms As Outlook.MobileItem
    Set sms = app.CreateItem(olMobileItemSMS)
    sms.Subject = &quot;Test SMS&quot;
    sms.Body = &quot;Short Message Service (SMS) message from Outlook 2010.&quot;
    ' Display the Inspector non-modally.
    sms.Display False
    
    Dim mms As Outlook.MobileItem
    Set mms = app.CreateItem(olMobileItemMMS)
    mms.Subject = &quot;Test MMS with picture of the desert&quot;
    
    ' Attach the picture to the MMS.
    mms.Attachments.Add PICTURE_PATH
    ' Display the Inspector non-modally.
    mms.Display False
End Sub</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Sub</span>&nbsp;DemoMobileItem()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;app&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Outlook.Application&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;app&nbsp;=&nbsp;ThisOutlookSession&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Change&nbsp;the&nbsp;following&nbsp;path&nbsp;to&nbsp;refer&nbsp;to&nbsp;a</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;small&nbsp;picture&nbsp;on&nbsp;your&nbsp;computer.</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Const</span>&nbsp;PICTURE_PATH&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__string">&quot;C:\Users\Public\Pictures\Sample&nbsp;Pictures\Desert.jpg&quot;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Create&nbsp;an&nbsp;SMS&nbsp;message&nbsp;and&nbsp;provide&nbsp;a&nbsp;body&nbsp;text&nbsp;value.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;sms&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Outlook.MobileItem&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;sms&nbsp;=&nbsp;app.CreateItem(olMobileItemSMS)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;sms.Subject&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Test&nbsp;SMS&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;sms.Body&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Short&nbsp;Message&nbsp;Service&nbsp;(SMS)&nbsp;message&nbsp;from&nbsp;Outlook&nbsp;2010.&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Display&nbsp;the&nbsp;Inspector&nbsp;non-modally.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;sms.Display&nbsp;<span class="visualBasic__keyword">False</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;mms&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Outlook.MobileItem&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;mms&nbsp;=&nbsp;app.CreateItem(olMobileItemMMS)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mms.Subject&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Test&nbsp;MMS&nbsp;with&nbsp;picture&nbsp;of&nbsp;the&nbsp;desert&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Attach&nbsp;the&nbsp;picture&nbsp;to&nbsp;the&nbsp;MMS.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mms.Attachments.Add&nbsp;PICTURE_PATH&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Display&nbsp;the&nbsp;Inspector&nbsp;non-modally.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mms.Display&nbsp;<span class="visualBasic__keyword">False</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<h1><span>Source Code Files</span></h1>
<ul>
<li><span style="font-size:small"><em><em><a id="26107" href="/site/view/file/26107/1/Outlook.MobileItem.txt">Outlook.MobileItem.txt</a>&nbsp;- Download this sample only.<br>
</em></em></span></li><li><span style="font-size:small"><em><em><a id="26108" href="/site/view/file/26108/1/Office%202010%20101%20Code%20Samples.zip">Office 2010 101 Code Samples.zip</a>&nbsp;- Download all the samples.</em></em></span>
</li></ul>
<h1>More Information</h1>
<ul>
<li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/aa905455">Outlook Developer Center on MSDN</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/hh360994">101 Code Samples for Office 2010 Developers</a></span>
</li></ul>
