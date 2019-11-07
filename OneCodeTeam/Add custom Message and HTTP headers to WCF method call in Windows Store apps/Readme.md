# Add custom Message and HTTP headers to WCF method call in Windows Store apps
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows 8
- Windows Store app Development
- .NET Development
- Windows Communication Framework (WCF)
## Topics
- WCF
- Header
## Updated
- 09/22/2016
## Description

<hr>
<div></div>
<h1><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></h1>
<h1>How to add Custom Message headers and HTTP headers to a WCF method call in a Windows Store app<span>
</span></h1>
<h2><span>Introduction </span></h2>
<p class="MsoNormal">​This code sample demonstrates how to add custom Message headers as well as HTTP headers to outgoing WCF requests. One of the reasons you may want to add these extra headers could be that you may be having a service that requires you
 to send either of these headers and the call may fail with an error if you do not add them to the request. This code sample just shows how to do it. A
<span class="SpellE">MessageHeader</span> is a SOAP Header that gets added to the outgoing SOAP Envelope of the WCF request. A SOAP Envelope is the entity body part of the HTTP request. Depending on whether your service implementation requires a Message Header
 or a HTTP header, you can change your WCF Client to add that information accordingly.</p>
<p class="MsoNormal">This sample implements a WCF Service implementation and a client project in the form of a Windows Store app. The WCF Service is a simple Calculator service. The client side project only calls the &quot;Add&quot; method, but it shows how to add
 the <span class="SpellE">MessageHeader</span> and HTTP headers to the outgoing requests.</p>
<p class="MsoNormal">Please note: This sample is written based on the following blog:</p>
<p class="MsoNormal"><span><a href="http://blogs.msdn.com/b/wsdevsol/archive/2014/02/07/adding-custom-messageheader-and-http-header-to-a-wcf-method-call.aspx">http://blogs.msdn.com/b/wsdevsol/archive/2014/02/07/adding-custom-messageheader-and-http-header-to-a-wcf-method-call.aspx</a>
</span></p>
<h2><span>Running the Sample </span></h2>
<p class="MsoNormal" style="margin-left:.5in"><span>(Note: if you want to debug the service in Visual Studio, you have to run Visual Studio as Administrator.)
</span></p>
<p class="MsoNormal" style="margin-left:.5in"><span><img src="117356-image.png" alt="" width="576" height="60" align="middle">
</span><span>&nbsp;</span></p>
<p class="MsoNormal" style="margin-left:.5in"><span><span>&nbsp;</span> <img src="117357-image.png" alt="" width="576" height="238" align="middle">
</span><span>&nbsp;</span></p>
<p class="MsoNormal" style="margin-left:.5in"><span>&nbsp;</span></p>
<p class="MsoNormal" style="margin-left:.5in">&nbsp;</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span><span>5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>Validation is completed.</span><span> </span></p>
<h2><span>Using the Code </span></h2>
<p class="MsoNormal"><span>The most important part of adding a <span class="SpellE">
MessageHeader</span> or a HTTP Header to the outgoing request is to gain access to the
<span class="SpellE">OperationContext</span> of the currently executing request. To summarize the two operations<span class="GramE">:</span><br>
a.) To add a <span class="SpellE">MessageHeader</span>, we access <span class="SpellE">
OperationContext.Current.OutgoingMessageHeaders</span> property and call the Add(&hellip;) method.<br>
b.) To add a HTTP Header, we access the <span class="GramE">OperationContext.Current.OutgoingMessageProperties[</span>HttpRequestMessageProperty.Name] property and assign a
<span class="SpellE">HttpRequestMessageProperty</span> to it. </span></p>
<p class="MsoNormal"><span>The code section below shows how to retrieve the <span class="SpellE">
OperationContext</span> and assign these properties: </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">CalculatorServiceClient client = new CalculatorServiceClient();
using (new OperationContextScope(client.InnerChannel))
{
&nbsp;&nbsp;&nbsp; //....we will add code here
}

</pre>
<pre class="csharp" id="codePreview">CalculatorServiceClient client = new CalculatorServiceClient();
using (new OperationContextScope(client.InnerChannel))
{
&nbsp;&nbsp;&nbsp; //....we will add code here
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span>In the code above, <span class="SpellE">CalculatorServiceClient</span> is the auto-generated class of the Calculator service that was generated when you did &quot;<strong>Add Service Reference</strong>&quot;.
</span></p>
<p class="MsoNormal"><span>Once you create the <span class="SpellE">OperationContextScope</span>, you can add a
<span class="SpellE"><strong>MessageHeader</strong></span> to the outgoing request by accessing the
<span class="SpellE"><strong>OperationContext.Current.OutgoingMessageHeaders</strong></span> property and then calling the Add method to add the
<span class="SpellE">MessageHeader</span>. We are using a custom object of type
<span class="SpellE">UserInfo</span> that will automatically get serialized. We are using a custom object to show how you can not only pass in simple types &ndash; such as Strings or Integers, but also add complex types to the outgoing
<span class="SpellE">MessageHeader</span>. Please refer to the code below: </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">using(new OperationContextScope(client.InnerChannel)) 
{
&nbsp;&nbsp;&nbsp; // We will use a custom class called UserInfo to be passed in as a MessageHeader
&nbsp;&nbsp;&nbsp; UserInfo userInfo = new UserInfo();
&nbsp;&nbsp;&nbsp; userInfo.FirstName = &quot;John&quot;;
&nbsp;&nbsp;&nbsp; userInfo.LastName = &quot;Doe&quot;;
&nbsp;&nbsp;&nbsp; userInfo.Age = 30;
 
&nbsp;&nbsp;&nbsp;&nbsp;// Add a SOAP Header to an outgoing request
&nbsp;&nbsp;&nbsp; MessageHeader aMessageHeader = MessageHeader.CreateHeader(&quot;UserInfo&quot;, &quot;http://tempuri.org&quot;, userInfo);
&nbsp;&nbsp;&nbsp; OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);
 
}

</pre>
<pre class="csharp" id="codePreview">using(new OperationContextScope(client.InnerChannel)) 
{
&nbsp;&nbsp;&nbsp; // We will use a custom class called UserInfo to be passed in as a MessageHeader
&nbsp;&nbsp;&nbsp; UserInfo userInfo = new UserInfo();
&nbsp;&nbsp;&nbsp; userInfo.FirstName = &quot;John&quot;;
&nbsp;&nbsp;&nbsp; userInfo.LastName = &quot;Doe&quot;;
&nbsp;&nbsp;&nbsp; userInfo.Age = 30;
 
&nbsp;&nbsp;&nbsp;&nbsp;// Add a SOAP Header to an outgoing request
&nbsp;&nbsp;&nbsp; MessageHeader aMessageHeader = MessageHeader.CreateHeader(&quot;UserInfo&quot;, &quot;http://tempuri.org&quot;, userInfo);
&nbsp;&nbsp;&nbsp; OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);
 
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span>Similarly we add an HTTP header to the outgoing request by accessing the
<span class="GramE"><strong>OperationContext.Current.OutgoingMessageProperties[</strong></span><strong>HttpRequestMessageProperty.Name]</strong> property and assigning an
<span class="SpellE">HttpRequestMessageProperty</span>. The code below shows you how to do this:
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">CalculatorServiceClient client = new CalculatorServiceClient();
using(new OperationContextScope(client.InnerChannel)) 
{
&nbsp;&nbsp;&nbsp; // Add an HTTP Header to an outgoing request
&nbsp;&nbsp;&nbsp; HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
&nbsp;&nbsp;&nbsp; requestMessage.Headers[&quot;MyHttpHeader&quot;] = &quot;MyHttpHeaderValue&quot;;
&nbsp;&nbsp;&nbsp; OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
}

</pre>
<pre class="csharp" id="codePreview">CalculatorServiceClient client = new CalculatorServiceClient();
using(new OperationContextScope(client.InnerChannel)) 
{
&nbsp;&nbsp;&nbsp; // Add an HTTP Header to an outgoing request
&nbsp;&nbsp;&nbsp; HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
&nbsp;&nbsp;&nbsp; requestMessage.Headers[&quot;MyHttpHeader&quot;] = &quot;MyHttpHeaderValue&quot;;
&nbsp;&nbsp;&nbsp; OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span>Putting both these things together, we will end up with the code below that adds both &ndash; a
<span class="SpellE">MessageHeader</span> as well as a HTTP Header to the outgoing HTTP request and then we call the
<span class="SpellE"><span class="GramE">AddAsync</span></span><span class="GramE">(</span>..) method to add two numbers. The approach below shows you the way of calling the asynchronous WCF method in Windows Store apps.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">CalculatorServiceClient client = new CalculatorServiceClient();
using(new OperationContextScope(client.InnerChannel)) 
{
&nbsp;&nbsp;&nbsp; // We will use a custom class called UserInfo to be passed in as a MessageHeader
&nbsp;&nbsp;&nbsp; UserInfo userInfo = new UserInfo();
&nbsp;&nbsp;&nbsp; userInfo.FirstName = &quot;John&quot;;
&nbsp;&nbsp;&nbsp; userInfo.LastName = &quot;Doe&quot;;
&nbsp;&nbsp;&nbsp; userInfo.Age = 30;
 
&nbsp;&nbsp;&nbsp;&nbsp;// Add a SOAP Header to an outgoing request
&nbsp;&nbsp;&nbsp; MessageHeader aMessageHeader = MessageHeader.CreateHeader(&quot;UserInfo&quot;, &quot;http://tempuri.org&quot;, userInfo);
&nbsp;&nbsp;&nbsp; OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);
 
&nbsp;&nbsp;&nbsp;&nbsp;// Add an HTTP Header to an outgoing request
&nbsp;&nbsp;&nbsp; HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
&nbsp;&nbsp;&nbsp; requestMessage.Headers[&quot;MyHttpHeader&quot;] = &quot;MyHttpHeaderValue&quot;;
&nbsp;&nbsp;&nbsp; OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
 
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double result = await client.AddAsync(20, 40);
&nbsp;&nbsp;&nbsp; txtOut.Text = &quot;Add result: &quot; &#43; result.ToString();
}

</pre>
<pre class="csharp" id="codePreview">CalculatorServiceClient client = new CalculatorServiceClient();
using(new OperationContextScope(client.InnerChannel)) 
{
&nbsp;&nbsp;&nbsp; // We will use a custom class called UserInfo to be passed in as a MessageHeader
&nbsp;&nbsp;&nbsp; UserInfo userInfo = new UserInfo();
&nbsp;&nbsp;&nbsp; userInfo.FirstName = &quot;John&quot;;
&nbsp;&nbsp;&nbsp; userInfo.LastName = &quot;Doe&quot;;
&nbsp;&nbsp;&nbsp; userInfo.Age = 30;
 
&nbsp;&nbsp;&nbsp;&nbsp;// Add a SOAP Header to an outgoing request
&nbsp;&nbsp;&nbsp; MessageHeader aMessageHeader = MessageHeader.CreateHeader(&quot;UserInfo&quot;, &quot;http://tempuri.org&quot;, userInfo);
&nbsp;&nbsp;&nbsp; OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);
 
&nbsp;&nbsp;&nbsp;&nbsp;// Add an HTTP Header to an outgoing request
&nbsp;&nbsp;&nbsp; HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
&nbsp;&nbsp;&nbsp; requestMessage.Headers[&quot;MyHttpHeader&quot;] = &quot;MyHttpHeaderValue&quot;;
&nbsp;&nbsp;&nbsp; OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
 
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double result = await client.AddAsync(20, 40);
&nbsp;&nbsp;&nbsp; txtOut.Text = &quot;Add result: &quot; &#43; result.ToString();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span>If you collect a <strong>Network/ Fiddler</strong> trace of the call, you should see the following HTTP request that shows what the added information looks like.
<br>
Notice the <strong>&lt;<span class="SpellE">s<span class="GramE">:Header</span></span>&gt;&hellip;&lt;/<span class="SpellE">s:Header</span>&gt;</strong> section for the
<span class="SpellE"><strong>MessageHeader</strong></span> and the <span class="SpellE">
<strong>MyHttpHeader</strong></span> HTTP header in the request below.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>
<pre class="hidden">POST / HTTP/1.1
Content-Type: text/xml; charset=utf-8
MyHttpHeader: MyHttpHeaderValue
SOAPAction: &quot;http://tempuri.org/ICalculatorService/Add&quot;
Host: &lt;yourServer&gt;:8001
Content-Length: 544
Expect: 100-continue
Accept-Encoding: gzip, deflate
Connection: Keep-Alive
 


&nbsp;&nbsp;&nbsp; &lt;UserInfo xmlns=&quot;http://tempuri.org&quot; xmlns:i=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Age xmlns=&quot;http://schemas.datacontract.org/2004/07/CustomMessageHeader&quot;&gt;30&lt;/Age&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;FirstName xmlns=&quot;http://schemas.datacontract.org/2004/07/CustomMessageHeader&quot;&gt;John&lt;/FirstName&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;LastName xmlns=&quot;http://schemas.datacontract.org/2004/07/CustomMessageHeader&quot;&gt;Doe&lt;/LastName&gt;
&nbsp;&nbsp;&nbsp; &lt;/UserInfo&gt;


&nbsp;&nbsp;&nbsp; &lt;Add xmlns=&quot;http://tempuri.org/&quot;&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;n1&gt;20&lt;/n1&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;n2&gt;40&lt;/n2&gt;
&nbsp;&nbsp;&nbsp; &lt;/Add&gt;



</pre>
<pre class="html" id="codePreview">POST / HTTP/1.1
Content-Type: text/xml; charset=utf-8
MyHttpHeader: MyHttpHeaderValue
SOAPAction: &quot;http://tempuri.org/ICalculatorService/Add&quot;
Host: &lt;yourServer&gt;:8001
Content-Length: 544
Expect: 100-continue
Accept-Encoding: gzip, deflate
Connection: Keep-Alive
 


&nbsp;&nbsp;&nbsp; &lt;UserInfo xmlns=&quot;http://tempuri.org&quot; xmlns:i=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Age xmlns=&quot;http://schemas.datacontract.org/2004/07/CustomMessageHeader&quot;&gt;30&lt;/Age&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;FirstName xmlns=&quot;http://schemas.datacontract.org/2004/07/CustomMessageHeader&quot;&gt;John&lt;/FirstName&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;LastName xmlns=&quot;http://schemas.datacontract.org/2004/07/CustomMessageHeader&quot;&gt;Doe&lt;/LastName&gt;
&nbsp;&nbsp;&nbsp; &lt;/UserInfo&gt;


&nbsp;&nbsp;&nbsp; &lt;Add xmlns=&quot;http://tempuri.org/&quot;&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;n1&gt;20&lt;/n1&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;n2&gt;40&lt;/n2&gt;
&nbsp;&nbsp;&nbsp; &lt;/Add&gt;



</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span class="SpellE"><span>MessageHeader</span></span><span> Class:&nbsp;<br>
<a href="http://msdn.microsoft.com/en-us/library/system.servicemodel.channels.messageheader.aspx">http://msdn.microsoft.com/en-us/library/system.servicemodel.channels.messageheader.aspx&nbsp;</a><br>
<span class="SpellE"><span>OperationContext.Current</span></span> Property :&nbsp;<br>
<a href="http://msdn.microsoft.com/en-us/library/system.servicemodel.operationcontext.current.aspx">http://msdn.microsoft.com/en-us/library/system.servicemodel.operationcontext.current.aspx&nbsp;</a><br>
<span class="SpellE"><span>OperationContext.OutgoingMessageHeaders</span></span> Property :&nbsp;<br>
<a href="http://msdn.microsoft.com/en-us/library/system.servicemodel.operationcontext.outgoingmessageheaders.aspx">http://msdn.microsoft.com/en-us/library/system.servicemodel.operationcontext.outgoingmessageheaders.aspx&nbsp;</a><br>
<span class="SpellE"><span>OperationContext.OutgoingMessageProperties</span></span> Property:&nbsp;<br>
<a href="http://msdn.microsoft.com/en-us/library/system.servicemodel.operationcontext.outgoingmessageproperties.aspx">http://msdn.microsoft.com/en-us/library/system.servicemodel.operationcontext.outgoingmessageproperties.aspx</a>
</span></p>
<p class="MsoNormal">&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
