# Use SMTP to send email (VBSMTPSendEmail)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Network
## Topics
- SMTP
## Updated
- 03/11/2012
## Description

<h1>CONSOLE <span class="GramE">APPLICATION(</span> <span class="SpellE">VBSMTPSendEmail</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal"><span class="SpellE">VBSMTPSendEmail</span> demonstrates sending email with attachment and embedded image in the message body using SMTP server from a VB.NET program.</p>
<p class="MsoNormal">With the introduction of .NET 2.0, the classes for sending email are packed in the
<span class="SpellE">System.Net.Mail</span> namespace. In the example, we use the
<span class="SpellE">MailMessage</span> and the <span class="SpellE">SmtpClient</span> classes.</p>
<h2>Using the Code</h2>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Build an email object and set the basic email properties.<span style="">
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span class="GramE"><span style="">Represents an e-mail message that can be sent using the
<span class="SpellE">SmtpClient</span> class.</span></span><span style=""> Instances of the
<span class="SpellE">MailMessage</span> class are used to construct e-mail messages that are transmitted to an SMTP server for delivery using the
<span class="SpellE">SmtpClient</span> class. </span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">The sender, recipient, subject, and body of an e-mail message may be specified as parameters when a
<span class="SpellE">MailMessage</span> is used to initialize a <span class="SpellE">
MailMessage</span> object. These parameters may also be set or accessed using properties on the
<span class="SpellE">MailMessage</span> object. </span></p>
<p class="MsoListParagraphCxSpLast"><span style=""><span style="">&nbsp;</span>The primary mail message headers and elements for the message may be set using the following properties of the
<span class="SpellE">MailMessage</span> class. </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
     Dim mail As New MailMessage()
     mail.To.Add(&quot;anyreceiver@anydomain.com&quot;)
     mail.From = New MailAddress(&quot;anyaddress@anydomain.com&quot;)

</pre>
<pre id="codePreview" class="vb">
     Dim mail As New MailMessage()
     mail.To.Add(&quot;anyreceiver@anydomain.com&quot;)
     mail.From = New MailAddress(&quot;anyaddress@anydomain.com&quot;)

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Add an attachment of the email.<span style=""> </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
    mail.Attachments.Add(New Attachment(attachedFile))

</pre>
<pre id="codePreview" class="vb">
    mail.Attachments.Add(New Attachment(attachedFile))

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Embed an image in the message body.<span style=""> </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
    mail.Body, null, &quot;text/html&quot;);
    LinkedResource imgLink = new LinkedResource(imgFile, &quot;image/jpg&quot;);
    imgLink.ContentId = &quot;image1&quot;;
    imgLink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
    htmlView.LinkedResources.Add(imgLink);
    mail.AlternateViews.Add(htmlView);


</pre>
<pre id="codePreview" class="vb">
    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
    mail.Body, null, &quot;text/html&quot;);
    LinkedResource imgLink = new LinkedResource(imgFile, &quot;image/jpg&quot;);
    imgLink.ContentId = &quot;image1&quot;;
    imgLink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
    htmlView.LinkedResources.Add(imgLink);
    mail.AlternateViews.Add(htmlView);


</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Configure the SMTP client.<span style=""> </span></p>
<p class="MsoListParagraphCxSpMiddle"><span class="GramE"><span style="">Allows applications to send e-mail by using the Simple Mail Transfer Protocol (SMTP).</span></span><span style=""> The
<span class="SpellE">SmtpClient</span> class is used to send e-mail to an SMTP server for delivery. The SMTP protocol is defined in RFC 2821, which is available at
<a href="http://www.ietf.org/">http://www.ietf.org</a>. </span></p>
<p class="MsoListParagraphCxSpLast"><span style="">The classes shown in the following table are used to construct e-mail messages that can be sent using
<span class="SpellE">SmtpClient</span>. </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
    Dim smtp As New SmtpClient()
    smtp.Host = &quot;smtp.live.com&quot;
    smtp.Credentials = New NetworkCredential(&quot;myaccount@live.com&quot;, &quot;mypassword&quot;)
    smtp.EnableSsl = True

</pre>
<pre id="codePreview" class="vb">
    Dim smtp As New SmtpClient()
    smtp.Host = &quot;smtp.live.com&quot;
    smtp.Credentials = New NetworkCredential(&quot;myaccount@live.com&quot;, &quot;mypassword&quot;)
    smtp.EnableSsl = True

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Send the email.<span style=""> </span></p>
<p class="MsoListParagraphCxSpMiddle"><span class="GramE"><span style="">Sends the specified message to an SMTP server for delivery.</span></span><span style=""> This method blocks while the e-mail is transmitted. You can specify a time-out value using
 the Timeout property to ensure that the method returns after a specified amount of time elapses.
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">Before calling this method, the Host and Port properties must be set either through the configuration files by setting the relevant properties, or by passing this information into the
<span class="SpellE"><span class="GramE">SmtpClient</span></span><span class="GramE">(</span>String, Int32) constructor.
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">You cannot call this method if there is a message being sent asynchronously.
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">If the SMTP host requires credentials, you must set them before calling this method. To specify credentials, use the
<span class="SpellE">UseDefaultCredentials</span> or Credentials properties. </span>
</p>
<p class="MsoListParagraphCxSpMiddle"><span style="">If you receive <span class="GramE">
an</span> <span class="SpellE">SmtpException</span> exception, check the <span class="SpellE">
StatusCode</span> property to find the reason the operation failed. The <span class="SpellE">
SmtpException</span> can also contain an inner exception that indicates the reason the operation failed.
</span></p>
<p class="MsoListParagraphCxSpLast"><span style="">When sending e-mail using Send to multiple recipients and the SMTP server accepts some recipients as valid and rejects others, Send sends e-mail to the accepted recipients and then a
<span class="SpellE">SmtpFailedRecipientsException</span> is thrown. The exception will contain a listing of the recipients that were rejected.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
    smtp.Send(mail)

</pre>
<pre id="codePreview" class="vb">
    smtp.Send(mail)

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><span style="font-family:������">MSDN: SmtpClient Class </span>
</p>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.aspx">http://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.aspx</a>
</span></p>
<p class="MsoNormal"><span style="font-family:������">Sending Emails from C# Application using default SMTP
</span></p>
<p class="MsoNormal"><span style=""><a href="http://www.codeproject.com/KB/cs/Sending_Mails_From_C_.aspx">http://www.codeproject.com/KB/cs/Sending_Mails_From_C_.aspx</a></span><span style="font-family:������">
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
