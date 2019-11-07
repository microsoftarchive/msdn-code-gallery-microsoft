# How to send email at particular date and time (CSEmailScheduler)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- Scheduled Task
## Updated
- 04/25/2012
## Description

<h2><span class="spelle"><span style="font-size:14.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">How to send email at particular date and time (<span class="SpellE">CSEmailScheduler</span>&lt;u1:p&gt;&lt;/u1:p&gt;)</span></span><span style="">
</span></h2>
<h2><span style="">Introduction </span></h2>
<p class="MsoNormal"><span class="SpellE"><span class="spelle"><b><span style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">CSEmailScheduler</span></b></span></span> enables you to send email at particular date and time; it also shows how to send emails in synchronous
 and asynchronous manner.<span style=""> </span></p>
<h2><span style="">Building the Sample </span></h2>
<p class="MsoNormal">To build this sample</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style="">1.</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Open CSEmailScheduler.sln file in Visual Studio 2010</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style="">2.</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Build the Solution</p>
<h2><span style="">Running the Sample </span></h2>
<p class="MsoNormal">For running this sample, please follow the steps below:</p>
<p class="MsoListParagraph" style="margin-left:58.5pt; text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Add SMTP configuration details in <span class="SpellE"><span class="spelle"><span style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">app.config</span></span></span> file. Some of the popular SMTP settings can be found here
</p>
<table class="MsoNormalTable" border="1" cellspacing="0" cellpadding="0" style="margin-left:.5in; border-collapse:collapse; border:none">
<tbody>
<tr style="">
<td width="61" valign="top" style="width:45.6pt; border:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">&lt;u1:p&gt;&nbsp;&lt;/u1:p&gt; </p>
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
<tr style="">
<td width="61" valign="top" style="width:45.6pt; border:solid windowtext 1.0pt; border-top:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Hotmail</p>
</td>
<td width="113" valign="top" style="width:84.5pt; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">smtp.live.com</p>
</td>
<td width="144" valign="top" style="width:1.5in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal"><a href="mailto:me@hotmail.com">me@hotmail.com</a> or <a href="mailto:me@live.com">
me@live.com</a> </p>
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
<tr style="">
<td width="61" valign="top" style="width:45.6pt; border:solid windowtext 1.0pt; border-top:none; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Gmail</p>
</td>
<td width="113" valign="top" style="width:84.5pt; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">smtp.gmail.com </p>
</td>
<td width="144" valign="top" style="width:1.5in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal"><a href="mailto:me@gmail.com">me@gmail.com</a> </p>
</td>
<td width="78" valign="top" style="width:58.5pt; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Gmail Password</p>
</td>
<td width="216" valign="top" style="width:2.25in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Port for TLS/STARTTLS: 587<br>
<span style="">&nbsp;</span>Port for SSL: 465</p>
</td>
<td width="72" valign="top" style="width:.75in; border-top:none; border-left:none; border-bottom:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt; padding:0in 5.4pt 0in 5.4pt">
<p class="MsoNormal">Yes</p>
</td>
</tr>
</tbody>
</table>
<p class="MsoNormal" style="margin-left:.5in">&lt;u1:p&gt;&nbsp;&lt;/u1:p&gt; </p>
<p class="MsoListParagraph" style="margin-left:58.5pt; text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>In <span class="SpellE">Program.cs</span>, you should replace the receiver’s E-Mail address with yours.</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.0pt; font-family:&quot;Courier New&quot;"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span style="color:green">// Receiver’s E-Mail address. </span></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.0pt; font-family:&quot;Courier New&quot;"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE"><span class="GramE">mailMessage.To.Add</span></span><span class="GramE">(</span><span style="color:#A31515">&quot;pathakajay@live.com&quot;</span>);
</span></p>
<p class="MsoListParagraph" style="margin-left:58.5pt"></p>
<p class="MsoListParagraph" style="margin-left:58.5pt; text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a task in Windows Task Scheduler. For creating a Schedule a Task, please refer:
<a href="http://technet.microsoft.com/en-us/library/cc748993.aspx">http://technet.microsoft.com/en-us/library/cc748993.aspx</a> article.</p>
<h2><span style="">Using the Code </span></h2>
<p class="MsoNormal">The following code snippet shows the key code for sending email.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">&nbsp;
public class EmailSender
{
&nbsp;&nbsp;&nbsp; /// &lt;summary&gt;
&nbsp;&nbsp;&nbsp; /// Send Email to a list of recipients. 
&nbsp;&nbsp;&nbsp;&nbsp;/// &lt;/summary&gt;
&nbsp;&nbsp;&nbsp; /// &lt;param name=&quot;recipientList&quot;&gt;A List of MailMessage object, that contains the list of Email Message&lt;/param&gt;
&nbsp;&nbsp;&nbsp; /// &lt;returns&gt;Returns True, if e-mail is sent successfully otherwise false&lt;/returns&gt;
&nbsp;&nbsp;&nbsp; public bool SendEmail(List&lt;MailMessage&gt; recipientList)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SmtpClient smtpClient = null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var mailSettings = config.GetSectionGroup(&quot;system.net/mailSettings&quot;) as MailSettingsSectionGroup;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bool status = false;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // SMTP settings are defined in app.config file
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient = new SmtpClient();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (MailMessage mailMessage in recipientList)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient.Send(mailMessage);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; status = true;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; catch (Exception e)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string errorMessage = string.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; while (e != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; errorMessage &#43;= e.ToString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; e = e.InnerException;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; status = false;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; finally
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (smtpClient != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient.Dispose();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return status;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; /// &lt;summary&gt;
&nbsp;&nbsp;&nbsp; /// Asynchronously send Email to a list of recipients. 
&nbsp;&nbsp;&nbsp;&nbsp;/// &lt;/summary&gt;
&nbsp;&nbsp;&nbsp; /// &lt;param name=&quot;recipientList&quot;&gt;A List of MailMessage object, that contains the list of Email Message&lt;/param&gt;
&nbsp;&nbsp;&nbsp; public void SendEmailAsync(List&lt;MailMessage&gt; recipientList)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SmtpClient smtpClient = null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var mailSettings = config.GetSectionGroup(&quot;system.net/mailSettings&quot;) as MailSettingsSectionGroup;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; object userState = null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // SMTP settings are defined in app.config file
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;smtpClient = new SmtpClient();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (MailMessage mailMessage in recipientList)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; userState = mailMessage;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient.SendCompleted &#43;= new SendCompletedEventHandler(smtpClient_SendCompleted);
&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;smtpClient.SendAsync(mailMessage, userState);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; catch (Exception e)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string errorMessage = string.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; while (e != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; errorMessage &#43;= e.ToString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; e = e.InnerException;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; finally
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (smtpClient != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient.Dispose();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Get the Original MailMessage object
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MailMessage mailMessage = (MailMessage)e.UserState;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string subject = mailMessage.Subject;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Write custom logging code here. Currently it is showing error on console.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (e.Cancelled)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;Send canceled for [{0}] with subject [{1}] at [{2}].&quot;, mailMessage.To, subject, DateTime.Now.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (e.Error != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;An error {1} occurred when sending mail [{0}] to [{2}] at [{3}] &quot;, subject, e.Error.ToString(), mailMessage.To, DateTime.Now.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;Message [{0}] is sent to [{1}] at [{2}].&quot;, subject, mailMessage.To, DateTime.Now.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }
}

</pre>
<pre id="codePreview" class="csharp">&nbsp;
public class EmailSender
{
&nbsp;&nbsp;&nbsp; /// &lt;summary&gt;
&nbsp;&nbsp;&nbsp; /// Send Email to a list of recipients. 
&nbsp;&nbsp;&nbsp;&nbsp;/// &lt;/summary&gt;
&nbsp;&nbsp;&nbsp; /// &lt;param name=&quot;recipientList&quot;&gt;A List of MailMessage object, that contains the list of Email Message&lt;/param&gt;
&nbsp;&nbsp;&nbsp; /// &lt;returns&gt;Returns True, if e-mail is sent successfully otherwise false&lt;/returns&gt;
&nbsp;&nbsp;&nbsp; public bool SendEmail(List&lt;MailMessage&gt; recipientList)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SmtpClient smtpClient = null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var mailSettings = config.GetSectionGroup(&quot;system.net/mailSettings&quot;) as MailSettingsSectionGroup;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bool status = false;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // SMTP settings are defined in app.config file
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient = new SmtpClient();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (MailMessage mailMessage in recipientList)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient.Send(mailMessage);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; status = true;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; catch (Exception e)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string errorMessage = string.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; while (e != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; errorMessage &#43;= e.ToString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; e = e.InnerException;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; status = false;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; finally
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (smtpClient != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient.Dispose();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return status;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; /// &lt;summary&gt;
&nbsp;&nbsp;&nbsp; /// Asynchronously send Email to a list of recipients. 
&nbsp;&nbsp;&nbsp;&nbsp;/// &lt;/summary&gt;
&nbsp;&nbsp;&nbsp; /// &lt;param name=&quot;recipientList&quot;&gt;A List of MailMessage object, that contains the list of Email Message&lt;/param&gt;
&nbsp;&nbsp;&nbsp; public void SendEmailAsync(List&lt;MailMessage&gt; recipientList)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SmtpClient smtpClient = null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var mailSettings = config.GetSectionGroup(&quot;system.net/mailSettings&quot;) as MailSettingsSectionGroup;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; object userState = null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // SMTP settings are defined in app.config file
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;smtpClient = new SmtpClient();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (MailMessage mailMessage in recipientList)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; userState = mailMessage;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient.SendCompleted &#43;= new SendCompletedEventHandler(smtpClient_SendCompleted);
&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;smtpClient.SendAsync(mailMessage, userState);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; catch (Exception e)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string errorMessage = string.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; while (e != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; errorMessage &#43;= e.ToString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; e = e.InnerException;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; finally
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (smtpClient != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; smtpClient.Dispose();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Get the Original MailMessage object
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MailMessage mailMessage = (MailMessage)e.UserState;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string subject = mailMessage.Subject;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Write custom logging code here. Currently it is showing error on console.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (e.Cancelled)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;Send canceled for [{0}] with subject [{1}] at [{2}].&quot;, mailMessage.To, subject, DateTime.Now.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (e.Error != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;An error {1} occurred when sending mail [{0}] to [{2}] at [{3}] &quot;, subject, e.Error.ToString(), mailMessage.To, DateTime.Now.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;Message [{0}] is sent to [{1}] at [{2}].&quot;, subject, mailMessage.To, DateTime.Now.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<h2><span style="">More Information </span></h2>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style="">1.</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Schedule a Task: <a href="http://technet.microsoft.com/en-us/library/cc748993.aspx">
http://technet.microsoft.com/en-us/library/cc748993.aspx</a></p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style="">2.</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE"><span class="spelle"><span style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">SmtpClient</span></span></span> Class:
<a href="http://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.aspx">
http://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.aspx</a> </p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
