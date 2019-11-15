# How to send email on a particular DateTime
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
- .NET
- Web App Development
## Topics
- Scheduled Task
## Updated
- 09/21/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src="https://aka.ms/onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"></a></div>
<h2><span style="font-size:14.0pt; line-height:115%">How to send email at particular date and time (VBEmailScheduler)</span><span>
</span></h2>
<h2><span>Introduction </span></h2>
<p class="MsoNormal"><strong><span>VBEmailScheduler</span></strong> enables you to send email at particular date and time; it also shows how to send emails in synchronous and asynchronous manner.<span>
</span></p>
<h2><span>Building the Sample </span></h2>
<p class="MsoNormal">To build this sample</p>
<ol>
<li><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,serif">&nbsp;</span><span style="text-indent:-0.25in">Open VBEmailScheduler.sln file in Visual Studio 2012</span>
</li><li><span style="text-indent:-0.25in">Build the Solution</span> </li></ol>
<h2><span>Running the Sample </span></h2>
<p class="MsoNormal">For running this sample, please follow the steps below:</p>
<p class="MsoListParagraph" style="margin-left:58.5pt; text-indent:-.25in"><span><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Add SMTP configuration details in app.config file. Some of the popular SMTP settings can be found here</p>
<table class="MsoNormalTable" border="1" cellspacing="0" cellpadding="0" style="margin-left:.5in; border-collapse:collapse; border:none">
<tbody>
<tr>
<td width="61" valign="top" style="width:45.6pt; border:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">&nbsp;</p>
</td>
<td width="113" valign="top" style="width:84.5pt; border:solid windowtext 1.0pt; border-left:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">SMTP Server</p>
</td>
<td width="144" valign="top" style="width:1.5in; border:solid windowtext 1.0pt; border-left:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Username</p>
</td>
<td width="78" valign="top" style="width:58.5pt; border:solid windowtext 1.0pt; border-left:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Password</p>
</td>
<td width="216" valign="top" style="width:2.25in; border:solid windowtext 1.0pt; border-left:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Port</p>
</td>
<td width="72" valign="top" style="width:.75in; border:solid windowtext 1.0pt; border-left:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">TLS/SSL</p>
</td>
</tr>
<tr>
<td width="61" valign="top" style="width:45.6pt; border:solid windowtext 1.0pt; border-top:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Hotmail</p>
</td>
<td width="113" valign="top" style="width:84.5pt; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">smtp.live.com</p>
</td>
<td width="144" valign="top" style="width:1.5in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal"><a href="mailto:me@hotmail.com">me@hotmail.com</a> or <a href="mailto:me@live.com">
me@live.com</a></p>
</td>
<td width="78" valign="top" style="width:58.5pt; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Hotmail password</p>
</td>
<td width="216" valign="top" style="width:2.25in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">587</p>
</td>
<td width="72" valign="top" style="width:.75in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Yes</p>
</td>
</tr>
<tr>
<td width="61" valign="top" style="width:45.6pt; border:solid windowtext 1.0pt; border-top:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Gmail</p>
</td>
<td width="113" valign="top" style="width:84.5pt; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">smtp.gmail.com</p>
</td>
<td width="144" valign="top" style="width:1.5in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal"><a href="mailto:me@gmail.com">me@gmail.com</a></p>
</td>
<td width="78" valign="top" style="width:58.5pt; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Gmail Password</p>
</td>
<td width="216" valign="top" style="width:2.25in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Port for TLS/STARTTLS: 587<br>
<span>&nbsp;</span>Port for SSL: 465</p>
</td>
<td width="72" valign="top" style="width:.75in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Yes</p>
</td>
</tr>
</tbody>
</table>
<p class="MsoNormal" style="margin-left:.5in">&nbsp;</p>
<p class="MsoListParagraph" style="margin-left:58.5pt; text-indent:-.25in"><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>In MainModule.vb, you should replace the receiver's E-Mail address with yours.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.0pt; font-family:&quot;Courier New&quot;"><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span style="color:green">// Receiver's E-Mail address. </span></span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.0pt; font-family:&quot;Courier New&quot;"><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>mailMessage.To.Add(<span style="color:#a31515">&quot;onecode@microsoft.com&quot;</span>);
</span></p>
<p class="MsoListParagraph" style="margin-left:58.5pt">&nbsp;</p>
<p class="MsoListParagraph" style="margin-left:58.5pt; text-indent:-.25in"><span><span>3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a task in Windows Task Scheduler. For creating a Schedule a Task, please refer:
<a href="http://technet.microsoft.com/en-us/library/cc748993.aspx">http://technet.microsoft.com/en-us/library/cc748993.aspx</a> article.</p>
<h2><span>&nbsp;</span></h2>
<h2><span>Using the Code </span></h2>
<p class="MsoNormal">The following code snippet shows the key code for sending email.</p>
<p class="MsoNormal">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Class</span>&nbsp;EmailSender&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;Send&nbsp;Email&nbsp;to&nbsp;a&nbsp;list&nbsp;of&nbsp;recipients.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;param&nbsp;name=&quot;recipientList&quot;&gt;A&nbsp;List&nbsp;of&nbsp;MailMessage&nbsp;object,&nbsp;that&nbsp;contains&nbsp;the&nbsp;list&nbsp;of&nbsp;Email&nbsp;Message&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;returns&gt;Returns&nbsp;True,&nbsp;if&nbsp;e-mail&nbsp;is&nbsp;sent&nbsp;successfully&nbsp;otherwise&nbsp;false&lt;/returns&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;SendEmail(recipientList&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;MailMessage))&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Boolean</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;smtpClient&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;SmtpClient&nbsp;=&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;config&nbsp;=&nbsp;ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;mailSettings&nbsp;=&nbsp;<span class="visualBasic__keyword">TryCast</span>(config.GetSectionGroup(<span class="visualBasic__string">&quot;system.net/mailSettings&quot;</span>),&nbsp;MailSettingsSectionGroup)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;status&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Boolean</span>&nbsp;=&nbsp;<span class="visualBasic__keyword">False</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;SMTP&nbsp;settings&nbsp;are&nbsp;defined&nbsp;in&nbsp;app.config&nbsp;file</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;smtpClient&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;SmtpClient()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;mailMessage&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;MailMessage&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;recipientList&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;smtpClient.Send(mailMessage)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span class="visualBasic__keyword">True</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Catch</span>&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Exception&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;errorMessage&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;<span class="visualBasic__keyword">String</span>.Empty&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">While</span>&nbsp;e&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;errorMessage&nbsp;&#43;=&nbsp;e.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;e&nbsp;=&nbsp;e.InnerException&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">While</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span class="visualBasic__keyword">False</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Finally</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;smtpClient&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;smtpClient.Dispose()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;status&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;Asynchronously&nbsp;send&nbsp;Email&nbsp;to&nbsp;a&nbsp;list&nbsp;of&nbsp;recipients.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;param&nbsp;name=&quot;recipientList&quot;&gt;A&nbsp;List&nbsp;of&nbsp;MailMessage&nbsp;object,&nbsp;that&nbsp;contains&nbsp;the&nbsp;list&nbsp;of&nbsp;Email&nbsp;Message&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;SendEmailAsync(recipientList&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;MailMessage))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;smtpClient&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;SmtpClient&nbsp;=&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;config&nbsp;=&nbsp;ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;mailSettings&nbsp;=&nbsp;<span class="visualBasic__keyword">TryCast</span>(config.GetSectionGroup(<span class="visualBasic__string">&quot;system.net/mailSettings&quot;</span>),&nbsp;MailSettingsSectionGroup)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;userState&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;SMTP&nbsp;settings&nbsp;are&nbsp;defined&nbsp;in&nbsp;app.config&nbsp;file</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;smtpClient&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;SmtpClient()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;mailMessage&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;MailMessage&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;recipientList&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;userState&nbsp;=&nbsp;mailMessage&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">AddHandler</span>&nbsp;smtpClient.SendCompleted,&nbsp;<span class="visualBasic__keyword">AddressOf</span>&nbsp;smtpClient_SendCompleted&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;smtpClient.SendAsync(mailMessage,&nbsp;userState)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Catch</span>&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Exception&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;errorMessage&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;<span class="visualBasic__keyword">String</span>.Empty&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">While</span>&nbsp;e&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;errorMessage&nbsp;&#43;=&nbsp;e.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;e&nbsp;=&nbsp;e.InnerException&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">While</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Finally</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;smtpClient&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;smtpClient.Dispose()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;smtpClient_SendCompleted(sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Object</span>,&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.ComponentModel.AsyncCompletedEventArgs.aspx" target="_blank" title="Auto generated link to System.ComponentModel.AsyncCompletedEventArgs">System.ComponentModel.AsyncCompletedEventArgs</a>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Get&nbsp;the&nbsp;Original&nbsp;MailMessage&nbsp;object</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;mailMessage&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;MailMessage&nbsp;=&nbsp;<span class="visualBasic__keyword">DirectCast</span>(e.UserState,&nbsp;MailMessage)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;subject&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;mailMessage.Subject&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Write&nbsp;custom&nbsp;logging&nbsp;code&nbsp;here.&nbsp;Currently&nbsp;it&nbsp;is&nbsp;showing&nbsp;error&nbsp;on&nbsp;console.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;e.Cancelled&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="visualBasic__string">&quot;Send&nbsp;canceled&nbsp;for&nbsp;[{0}]&nbsp;with&nbsp;subject&nbsp;[{1}]&nbsp;at&nbsp;[{2}].&quot;</span>,&nbsp;mailMessage.[<span class="visualBasic__keyword">To</span>],&nbsp;subject,&nbsp;DateTime.Now.ToString())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;e.[<span class="visualBasic__keyword">Error</span>]&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="visualBasic__string">&quot;An&nbsp;error&nbsp;{1}&nbsp;occurred&nbsp;when&nbsp;sending&nbsp;mail&nbsp;[{0}]&nbsp;to&nbsp;[{2}]&nbsp;at&nbsp;[{3}]&nbsp;&quot;</span>,&nbsp;subject,&nbsp;e.[<span class="visualBasic__keyword">Error</span>].ToString(),&nbsp;mailMessage.[<span class="visualBasic__keyword">To</span>],&nbsp;DateTime.Now.ToString())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="visualBasic__string">&quot;Message&nbsp;[{0}]&nbsp;is&nbsp;sent&nbsp;to&nbsp;[{1}]&nbsp;at&nbsp;[{2}].&quot;</span>,&nbsp;subject,&nbsp;mailMessage.[<span class="visualBasic__keyword">To</span>],&nbsp;DateTime.Now.ToString())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Class</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<h2><span>More Information </span></h2>
<p class="MsoListParagraph" style="text-indent:-.25in"><span>1.</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,serif">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Schedule a Task: <a href="http://technet.microsoft.com/en-us/library/cc748993.aspx">
http://technet.microsoft.com/en-us/library/cc748993.aspx</a></p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span>2.</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,serif">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>SmtpClient Class: <a href="http://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.aspx">
http://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.aspx</a></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
