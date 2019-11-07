# How to stop/start deallocated VM using Windows Azure Service management REST API
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Azure
- Windows Azure Storage
## Topics
- Azure
## Updated
- 09/21/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"></a><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
<h1><span lang="EN-US">How to stop/start <span class="SpellE">deallocated</span> VM in Windows Azure using Windows Azure Service management REST API (<span class="SpellE">CSAzureStartDeallocatedVM</span>)</span></h1>
<h2><span lang="EN-US">Introduction</span></h2>
<p class="MsoNormal"><span lang="EN-US">VM (Deallocated) is a new feature in Azure VM. It's a very nice function. Many developers may want to use it by using REST API.
</span></p>
<p class="MsoNormal"><span lang="EN-US">You can find the answers to all the following questions in the code sample:</span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span lang="EN-US" style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span lang="EN-US">How to stop VM (Deallocated).</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:5.0pt"><span lang="EN-US" style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span lang="EN-US">How to start VM (Deallocated).</span></p>
<h2><span lang="EN-US">Running the Sample</span></h2>
<p class="MsoNormal"><span lang="EN-US">You should have a VM in your Azure VMs.</span></p>
<p class="MsoNormal"><span lang="EN-US">You should have a valid certificate for Azure. Please refer to
<a href="http://msdn.microsoft.com/en-us/library/windowsazure/gg551722.aspx">Create and Upload a Management Certificate for Windows Azure</a></span></p>
<h2><span lang="EN-US">Using the Code</span></h2>
<p class="MsoNormal"><span lang="EN-US">The code sample provides the following reusable functions to operate Azure VMS.</span></p>
<h3><span lang="EN-US">How to stop VM (Deallocated) </span></h3>
<p class="MsoNormal"><span lang="EN-US">&nbsp; </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">static void StopVirtualMachine(string subscriptionID, X509Certificate2 cer, string serviceName,string deploymentsName,string vmName, bool Deallocated)
       {
           HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(&quot;https://management.core.windows.net/&quot; &#43; subscriptionID
               &#43; &quot;/services/hostedservices/&quot; &#43; serviceName &#43; &quot;/deployments/&quot; &#43; deploymentsName &#43; &quot;/roleinstances/&quot; &#43; vmName &#43; &quot;/Operations&quot;));

           request.Method = &quot;POST&quot;;
           request.ClientCertificates.Add(cer);
           request.ContentType = &quot;application/xml&quot;;
           request.Headers.Add(&quot;x-ms-version&quot;, &quot;2013-06-01&quot;);

           //Add body to the reqeust 
           XmlDocument xmlDoc = new XmlDocument();
           if (Deallocated)
           {
               xmlDoc.Load(&quot;..\\..\\StopVM_Deallocated.xml&quot;);
           }
           else
           {
               xmlDoc.Load(&quot;..\\..\\StopVM.xml&quot;);  
           }

           Stream requestStream = request.GetRequestStream();
           StreamWriter streamWriter = new StreamWriter(requestStream, System.Text.UTF8Encoding.UTF8);
           xmlDoc.Save(streamWriter);

           streamWriter.Close();
           requestStream.Close();
           try
           {
               HttpWebResponse response = (HttpWebResponse)request.GetResponse();
               response.Close();
               Console.WriteLine(&quot;Operation Success!&quot;);
               Console.ReadKey();
           }
           catch (WebException ex)
           {
               
                Console.Write(ex.Response.Headers.ToString());
               Console.Read();
           }

       }
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;StopVirtualMachine(<span class="cs__keyword">string</span>&nbsp;subscriptionID,&nbsp;X509Certificate2&nbsp;cer,&nbsp;<span class="cs__keyword">string</span>&nbsp;serviceName,<span class="cs__keyword">string</span>&nbsp;deploymentsName,<span class="cs__keyword">string</span>&nbsp;vmName,&nbsp;<span class="cs__keyword">bool</span>&nbsp;Deallocated)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpWebRequest&nbsp;request&nbsp;=&nbsp;(HttpWebRequest)HttpWebRequest.Create(<span class="cs__keyword">new</span>&nbsp;Uri(<span class="cs__string">&quot;https://management.core.windows.net/&quot;</span>&nbsp;&#43;&nbsp;subscriptionID&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#43;&nbsp;<span class="cs__string">&quot;/services/hostedservices/&quot;</span>&nbsp;&#43;&nbsp;serviceName&nbsp;&#43;&nbsp;<span class="cs__string">&quot;/deployments/&quot;</span>&nbsp;&#43;&nbsp;deploymentsName&nbsp;&#43;&nbsp;<span class="cs__string">&quot;/roleinstances/&quot;</span>&nbsp;&#43;&nbsp;vmName&nbsp;&#43;&nbsp;<span class="cs__string">&quot;/Operations&quot;</span>));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Method&nbsp;=&nbsp;<span class="cs__string">&quot;POST&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.ClientCertificates.Add(cer);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.ContentType&nbsp;=&nbsp;<span class="cs__string">&quot;application/xml&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Headers.Add(<span class="cs__string">&quot;x-ms-version&quot;</span>,&nbsp;<span class="cs__string">&quot;2013-06-01&quot;</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Add&nbsp;body&nbsp;to&nbsp;the&nbsp;reqeust&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;XmlDocument&nbsp;xmlDoc&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;XmlDocument();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(Deallocated)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc.Load(<span class="cs__string">&quot;..\\..\\StopVM_Deallocated.xml&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc.Load(<span class="cs__string">&quot;..\\..\\StopVM.xml&quot;</span>);&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Stream&nbsp;requestStream&nbsp;=&nbsp;request.GetRequestStream();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;StreamWriter&nbsp;streamWriter&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;StreamWriter(requestStream,&nbsp;System.Text.UTF8Encoding.UTF8);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc.Save(streamWriter);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;streamWriter.Close();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;requestStream.Close();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpWebResponse&nbsp;response&nbsp;=&nbsp;(HttpWebResponse)request.GetResponse();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;response.Close();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Operation&nbsp;Success!&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.ReadKey();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(WebException&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.Write(ex.Response.Headers.ToString());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.Read();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<h3><span lang="EN-US">How to start VM (Deallocated)</span></h3>
<p class="MsoNormal"><span lang="EN-US">&nbsp; </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">static void StartVirtualMachine(string subscriptionID, X509Certificate2 cer, string serviceName, string deploymentsName, string vmName)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(&quot;https://management.core.windows.net/&quot; &#43; SubscriptionID
            &#43; &quot;/services/hostedservices/&quot; &#43; serviceName &#43; &quot;/deployments/&quot; &#43; deploymentsName &#43; &quot;/roleinstances/&quot; &#43; vmName &#43; &quot;/Operations&quot;));

            request.Method = &quot;POST&quot;;
            request.ClientCertificates.Add(Certificate);
            request.ContentType = &quot;application/xml&quot;;
            request.Headers.Add(&quot;x-ms-version&quot;, &quot;2013-06-01&quot;);

            // Add body to the request
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(&quot;..\\..\\StartVM.xml&quot;);

            Stream requestStream = request.GetRequestStream();
            StreamWriter streamWriter = new StreamWriter(requestStream, System.Text.UTF8Encoding.UTF8);
            xmlDoc.Save(streamWriter);

            streamWriter.Close();
            requestStream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                Console.WriteLine(&quot;Operation Success!&quot;);
                Console.ReadKey();
            }
            catch (WebException ex)
            {

                Console.Write(ex.Response.Headers.ToString());
                Console.Read();
            }
        }
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;StartVirtualMachine(<span class="cs__keyword">string</span>&nbsp;subscriptionID,&nbsp;X509Certificate2&nbsp;cer,&nbsp;<span class="cs__keyword">string</span>&nbsp;serviceName,&nbsp;<span class="cs__keyword">string</span>&nbsp;deploymentsName,&nbsp;<span class="cs__keyword">string</span>&nbsp;vmName)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpWebRequest&nbsp;request&nbsp;=&nbsp;(HttpWebRequest)HttpWebRequest.Create(<span class="cs__keyword">new</span>&nbsp;Uri(<span class="cs__string">&quot;https://management.core.windows.net/&quot;</span>&nbsp;&#43;&nbsp;SubscriptionID&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#43;&nbsp;<span class="cs__string">&quot;/services/hostedservices/&quot;</span>&nbsp;&#43;&nbsp;serviceName&nbsp;&#43;&nbsp;<span class="cs__string">&quot;/deployments/&quot;</span>&nbsp;&#43;&nbsp;deploymentsName&nbsp;&#43;&nbsp;<span class="cs__string">&quot;/roleinstances/&quot;</span>&nbsp;&#43;&nbsp;vmName&nbsp;&#43;&nbsp;<span class="cs__string">&quot;/Operations&quot;</span>));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Method&nbsp;=&nbsp;<span class="cs__string">&quot;POST&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.ClientCertificates.Add(Certificate);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.ContentType&nbsp;=&nbsp;<span class="cs__string">&quot;application/xml&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Headers.Add(<span class="cs__string">&quot;x-ms-version&quot;</span>,&nbsp;<span class="cs__string">&quot;2013-06-01&quot;</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Add&nbsp;body&nbsp;to&nbsp;the&nbsp;request</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;XmlDocument&nbsp;xmlDoc&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;XmlDocument();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc.Load(<span class="cs__string">&quot;..\\..\\StartVM.xml&quot;</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Stream&nbsp;requestStream&nbsp;=&nbsp;request.GetRequestStream();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;StreamWriter&nbsp;streamWriter&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;StreamWriter(requestStream,&nbsp;System.Text.UTF8Encoding.UTF8);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc.Save(streamWriter);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;streamWriter.Close();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;requestStream.Close();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpWebResponse&nbsp;response&nbsp;=&nbsp;(HttpWebResponse)request.GetResponse();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;response.Close();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Operation&nbsp;Success!&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.ReadKey();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(WebException&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.Write(ex.Response.Headers.ToString());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.Read();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<h2><span lang="EN-US">More Information</span></h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span lang="EN-US" style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span lang="EN-US"><a href="http://msdn.microsoft.com/en-us/library/windowsazure/gg551722.aspx">MSDN:<strong><span style="font-size:25.0pt; line-height:115%; font-family:&quot;Segoe UI&quot;,&quot;sans-serif&quot;">
</span></strong>Create and Upload a Management Certificate for Windows Azure</a></span></p>
<p class="MsoListParagraphCxSpLast"><span lang="EN-US">&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
