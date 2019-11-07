# How to get SOAP Service method and parameter from wsdl address
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- WCF
- .NET
- Services
## Topics
- WCF
- .NET
- Services
## Updated
- 10/28/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to get SOAP service method and parameter from WSDL address</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Introduction
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>When accessing the WCF service, users can download the service description xml WSDL file from service address. Sometimes, users might&nbsp;need to use SOAP way to invoke service method. However, users can&rsquo;t post WSDL format to service address.
 Instead, they have to convert the WSDL format xml to SOAP format. This sample demonstrates how to get SOAP service method and parameter from WSDL address.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Visual studio 2015</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Open</span><span> the solution file in archive</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Download the target WSDL file from WCF service address, for example, if your service address is
</span><a href="http://localhost:6742/Service1.svc" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://localhost:6742/Service1.svc</span></a><span> then you can download the WSDL file from
</span><a href="http://localhost:6742/Service1.svc?singleWsdl" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://localhost:6742/Service1.svc?singleWsdl</span></a><span>
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Change the WSDL file path in
</span><span>Program.cs</span><span> line 18</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="162696-image.png" alt="" width="575" height="145" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Then run or debug the project, you will get the dictionary output which contains all the SOAP format method definition</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>When customers need to invoke SOAP style service on WCF service, they can make the use of sample code to generate SOAP formation content to do invocation.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>The sample implements </span><span style="font-weight:bold">WsdlToSoap</span><span style="font-weight:bold">
</span><span>utility to do the tedious&nbsp;work&nbsp;of converting WSDL to SOAP. You can make the use of it as you like.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>The below code snippets demonstrate how to do SOAP invocation on WCF service making the use of the convert class.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">static void Main(string[] args)
{
	// Read the wcf service wsdl xml and convert to soap xml format per method
	// The output is a dicionary, each entry is the soap xml format of that method.
	var output = new WsdlToSoap(File.ReadAllText(&quot;test.svc&quot;)).SoapMethodDictionary;

	// The test.svc wsdl contains a method &quot;GetData&quot;, please change the method name here according to your WCF service
	var soapXmlStringGetData = output[&quot;GetData&quot;];

	WebClient client = new WebClient();

	// the Content-Type needs to be set to XML
	client.Headers.Add(&quot;Content-Type&quot;, &quot;text/xml;charset=utf-8&quot;);

	// The SOAPAction header indicates which method you would like to invoke
	// and could be seen in the WSDL: &lt;soap:operation soapAction=&quot;...&quot; /&gt; element
	client.Headers.Add(&quot;SOAPAction&quot;, &quot;\&quot;http://tempuri.org/IService1/GetData\&quot;&quot;);

	// Please enter the your wcf service address here
	var response = client.UploadString(&quot;http://localhost:6742/Service1.svc&quot;, soapXmlStringGetData);

	// get the response
	Console.WriteLine(response);
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Main(<span class="cs__keyword">string</span>[]&nbsp;args)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Read&nbsp;the&nbsp;wcf&nbsp;service&nbsp;wsdl&nbsp;xml&nbsp;and&nbsp;convert&nbsp;to&nbsp;soap&nbsp;xml&nbsp;format&nbsp;per&nbsp;method</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;The&nbsp;output&nbsp;is&nbsp;a&nbsp;dicionary,&nbsp;each&nbsp;entry&nbsp;is&nbsp;the&nbsp;soap&nbsp;xml&nbsp;format&nbsp;of&nbsp;that&nbsp;method.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;output&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;WsdlToSoap(File.ReadAllText(<span class="cs__string">&quot;test.svc&quot;</span>)).SoapMethodDictionary;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;The&nbsp;test.svc&nbsp;wsdl&nbsp;contains&nbsp;a&nbsp;method&nbsp;&quot;GetData&quot;,&nbsp;please&nbsp;change&nbsp;the&nbsp;method&nbsp;name&nbsp;here&nbsp;according&nbsp;to&nbsp;your&nbsp;WCF&nbsp;service</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;soapXmlStringGetData&nbsp;=&nbsp;output[<span class="cs__string">&quot;GetData&quot;</span>];&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;WebClient&nbsp;client&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;WebClient();&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;the&nbsp;Content-Type&nbsp;needs&nbsp;to&nbsp;be&nbsp;set&nbsp;to&nbsp;XML</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;client.Headers.Add(<span class="cs__string">&quot;Content-Type&quot;</span>,&nbsp;<span class="cs__string">&quot;text/xml;charset=utf-8&quot;</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;The&nbsp;SOAPAction&nbsp;header&nbsp;indicates&nbsp;which&nbsp;method&nbsp;you&nbsp;would&nbsp;like&nbsp;to&nbsp;invoke</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;and&nbsp;could&nbsp;be&nbsp;seen&nbsp;in&nbsp;the&nbsp;WSDL:&nbsp;&lt;soap:operation&nbsp;soapAction=&quot;...&quot;&nbsp;/&gt;&nbsp;element</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;client.Headers.Add(<span class="cs__string">&quot;SOAPAction&quot;</span>,&nbsp;<span class="cs__string">&quot;\&quot;http://tempuri.org/IService1/GetData\&quot;&quot;</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Please&nbsp;enter&nbsp;the&nbsp;your&nbsp;wcf&nbsp;service&nbsp;address&nbsp;here</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;response&nbsp;=&nbsp;client.UploadString(<span class="cs__string">&quot;http://localhost:6742/Service1.svc&quot;</span>,&nbsp;soapXmlStringGetData);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;get&nbsp;the&nbsp;response</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(response);&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">More information</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>WSDL definition:
</span><a href="https://www.w3.org/TR/wsdl" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://www.w3.org/TR/wsdl</span></a><span>
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>SOAP definition:
</span><a href="https://en.wikipedia.org/wiki/SOAP" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://en.wikipedia.org/wiki/SOAP</span></a><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
