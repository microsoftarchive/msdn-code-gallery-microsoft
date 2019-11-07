# How to use HttpClient to post Json data to WebService in universal Windows apps
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows Store app
- Windows Store app Development
- Windows Phone Development
- Windows 8.1
- Windows Phone 8.1
- universal windows app
## Topics
- JSON
- WebService
- universal app
## Updated
- 09/22/2016
## Description

<h1>
<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
</h1>
<h1>How to use HttpClient to post Json data to WebService in universal Windows apps</h1>
<h2>Introduction</h2>
<p>This sample demonstrates how to use the HttpClient and DataContractJsonSerializer&nbsp; class to post JSON data to a web service. It's easy to achieve this in&nbsp; WinJS realm.&nbsp; But there is no example that shows how to do this using HttpClient in
 the .NET applications. So we provide this sample hoping it will be helpful to you.</p>
<p>For Win10 UWP sample, please download from the link below:</p>
<p><a href="https://code.msdn.microsoft.com/How-to-post-json-data-2c86fe51">https://code.msdn.microsoft.com/How-to-post-json-data-2c86fe51</a></p>
<h2>Video</h2>
<p><a href="http://channel9.msdn.com/Blogs/OneCode/How-to-use-HttpClient-to-post-Json-data-to-WebService-in-universal-Windows-apps" target="_blank"><img id="138071" src="138071-how%20to%20use%20httpclient%20to%20post%20json%20data%20to%20webservice%20in%20universal%20windows%20apps%20%20%20channel%209.png" alt="" width="640" height="350" style="border:1px solid black"></a></p>
<h2>Building the Sample</h2>
<p>Open the solution file with Visual Studio 2013. Then right click the JSONWCFService project, select &ldquo;View&rdquo; and &ldquo;View in browser&rdquo; to start the web service. Then you can run the Windows Store or Phone app project.</p>
<h2>Running the Sample</h2>
<p>Input the &ldquo;Name&rdquo; and &ldquo;Age&rdquo; data, then click &ldquo;Start&rdquo; button to get data from web service.</p>
<p>After posting the &ldquo;Name&rdquo; and &ldquo;Age&rdquo; as JSON data, the TextBox below will show some text processed by the web service.</p>
<p>&nbsp;<img id="135032" src="135032-1.png" alt="" width="640" height="400" style="border:1px solid black"></p>
<h2>Using the Code</h2>
<p>1. Create a web service.</p>
<pre><div class="scriptcode"><div class="pluginEditHolder" pluginCommand="mceScriptCode"><div class="title"><span>C#</span></div><div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div><span class="hidden">csharp</span><pre class="hidden">[ServiceContract]
public interface IWCFService
{
 
        [OperationContract]
        [WebInvoke(UriTemplate = &quot;GetData&quot;, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetDataUsingDataContract(string Name, string Age);
}
public class WCFService : IWCFService
{
        public string GetDataUsingDataContract(string Name, string Age)
        {
            return &quot;Your input is: &quot; &#43; &quot;Name: &quot; &#43; Name &#43; &quot;  Age: &quot; &#43; Age;
        }
}</pre>
<div class="preview">
<pre class="csharp">[ServiceContract]&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">interface</span>&nbsp;IWCFService&nbsp;
{&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[OperationContract]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[WebInvoke(UriTemplate&nbsp;=&nbsp;<span class="cs__string">&quot;GetData&quot;</span>,&nbsp;RequestFormat&nbsp;=&nbsp;WebMessageFormat.Json,&nbsp;ResponseFormat&nbsp;=&nbsp;WebMessageFormat.Json,&nbsp;BodyStyle&nbsp;=&nbsp;WebMessageBodyStyle.Wrapped)]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;GetDataUsingDataContract(<span class="cs__keyword">string</span>&nbsp;Name,&nbsp;<span class="cs__keyword">string</span>&nbsp;Age);&nbsp;
}&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;WCFService&nbsp;:&nbsp;IWCFService&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;GetDataUsingDataContract(<span class="cs__keyword">string</span>&nbsp;Name,&nbsp;<span class="cs__keyword">string</span>&nbsp;Age)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__string">&quot;Your&nbsp;input&nbsp;is:&nbsp;&quot;</span>&nbsp;&#43;&nbsp;<span class="cs__string">&quot;Name:&nbsp;&quot;</span>&nbsp;&#43;&nbsp;Name&nbsp;&#43;&nbsp;<span class="cs__string">&quot;&nbsp;&nbsp;Age:&nbsp;&quot;</span>&nbsp;&#43;&nbsp;Age;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
</pre>
<p>&nbsp;</p>
<p>2. Configure the configuration file of the web service.</p>
<pre><div class="scriptcode"><div class="pluginEditHolder" pluginCommand="mceScriptCode"><div class="title"><span>XML</span></div><div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div><span class="hidden">xml</span><pre class="hidden">&lt;system.serviceModel&gt;
  &lt;!--Add Services--&gt;
&lt;services&gt;
    &lt;service name=&quot;JSONWCFService.WCFService&quot; behaviorConfiguration=&quot;ServiceBehaviour&quot;&gt;
      &lt;endpoint name=&quot;JsonEndPoint&quot; contract=&quot;JSONWCFService.IWCFService&quot; binding=&quot;webHttpBinding&quot; behaviorConfiguration=&quot;jsonbehavior&quot;/&gt;
    &lt;/service&gt;
  &lt;/services&gt;
  
  &lt;behaviors&gt;
    &lt;serviceBehaviors&gt;
      &lt;behavior name=&quot;ServiceBehaviour&quot;&gt;
        &lt;!-- To avoid disclosing metadata information, set the values below to false before deployment --&gt;
        &lt;serviceMetadata httpGetEnabled=&quot;true&quot; httpsGetEnabled=&quot;true&quot;/&gt;
        &lt;!-- To receive exception details in faults for debugging purposes, set the value below to true.  Set to false before deployment to avoid disclosing exception information --&gt;
        &lt;serviceDebug includeExceptionDetailInFaults=&quot;false&quot;/&gt;
      &lt;/behavior&gt;
    &lt;/serviceBehaviors&gt;
    &lt;!--Add EndPoint Behaviors--&gt;
    &lt;endpointBehaviors&gt;
      &lt;behavior name=&quot;jsonbehavior&quot;&gt;
        &lt;webHttp defaultBodyStyle=&quot;Wrapped&quot; defaultOutgoingResponseFormat=&quot;Json&quot;/&gt;
      &lt;/behavior&gt;
    &lt;/endpointBehaviors&gt;
  &lt;/behaviors&gt;
  
  &lt;protocolMapping&gt;
      &lt;add binding=&quot;basicHttpsBinding&quot; scheme=&quot;https&quot; /&gt;
  &lt;/protocolMapping&gt;    
  &lt;serviceHostingEnvironment aspNetCompatibilityEnabled=&quot;true&quot; multipleSiteBindingsEnabled=&quot;true&quot; /&gt;
&lt;/system.serviceModel&gt;
 </pre>
<div class="preview">
<pre class="xml"><span class="xml__tag_start">&lt;system</span>.serviceModel<span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;<span class="xml__comment">&lt;!--Add&nbsp;Services--&gt;</span>&nbsp;
<span class="xml__tag_start">&lt;services</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;service</span>&nbsp;<span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;JSONWCFService.WCFService&quot;</span>&nbsp;<span class="xml__attr_name">behaviorConfiguration</span>=<span class="xml__attr_value">&quot;ServiceBehaviour&quot;</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;endpoint</span>&nbsp;<span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;JsonEndPoint&quot;</span>&nbsp;<span class="xml__attr_name">contract</span>=<span class="xml__attr_value">&quot;JSONWCFService.IWCFService&quot;</span>&nbsp;<span class="xml__attr_name">binding</span>=<span class="xml__attr_value">&quot;webHttpBinding&quot;</span>&nbsp;<span class="xml__attr_name">behaviorConfiguration</span>=<span class="xml__attr_value">&quot;jsonbehavior&quot;</span><span class="xml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_end">&lt;/service&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_end">&lt;/services&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;behaviors</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;serviceBehaviors</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;behavior</span>&nbsp;<span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;ServiceBehaviour&quot;</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__comment">&lt;!--&nbsp;To&nbsp;avoid&nbsp;disclosing&nbsp;metadata&nbsp;information,&nbsp;set&nbsp;the&nbsp;values&nbsp;below&nbsp;to&nbsp;false&nbsp;before&nbsp;deployment&nbsp;--&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;serviceMetadata</span>&nbsp;<span class="xml__attr_name">httpGetEnabled</span>=<span class="xml__attr_value">&quot;true&quot;</span>&nbsp;<span class="xml__attr_name">httpsGetEnabled</span>=<span class="xml__attr_value">&quot;true&quot;</span><span class="xml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__comment">&lt;!--&nbsp;To&nbsp;receive&nbsp;exception&nbsp;details&nbsp;in&nbsp;faults&nbsp;for&nbsp;debugging&nbsp;purposes,&nbsp;set&nbsp;the&nbsp;value&nbsp;below&nbsp;to&nbsp;true.&nbsp;&nbsp;Set&nbsp;to&nbsp;false&nbsp;before&nbsp;deployment&nbsp;to&nbsp;avoid&nbsp;disclosing&nbsp;exception&nbsp;information&nbsp;--&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;serviceDebug</span>&nbsp;<span class="xml__attr_name">includeExceptionDetailInFaults</span>=<span class="xml__attr_value">&quot;false&quot;</span><span class="xml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_end">&lt;/behavior&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_end">&lt;/serviceBehaviors&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__comment">&lt;!--Add&nbsp;EndPoint&nbsp;Behaviors--&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;endpointBehaviors</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;behavior</span>&nbsp;<span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;jsonbehavior&quot;</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;webHttp</span>&nbsp;<span class="xml__attr_name">defaultBodyStyle</span>=<span class="xml__attr_value">&quot;Wrapped&quot;</span>&nbsp;<span class="xml__attr_name">defaultOutgoingResponseFormat</span>=<span class="xml__attr_value">&quot;Json&quot;</span><span class="xml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_end">&lt;/behavior&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_end">&lt;/endpointBehaviors&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_end">&lt;/behaviors&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;protocolMapping</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;add</span>&nbsp;<span class="xml__attr_name">binding</span>=<span class="xml__attr_value">&quot;basicHttpsBinding&quot;</span>&nbsp;<span class="xml__attr_name">scheme</span>=<span class="xml__attr_value">&quot;https&quot;</span>&nbsp;<span class="xml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_end">&lt;/protocolMapping&gt;</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;serviceHostingEnvironment</span>&nbsp;<span class="xml__attr_name">aspNetCompatibilityEnabled</span>=<span class="xml__attr_value">&quot;true&quot;</span>&nbsp;<span class="xml__attr_name">multipleSiteBindingsEnabled</span>=<span class="xml__attr_value">&quot;true&quot;</span>&nbsp;<span class="xml__tag_start">/&gt;</span>&nbsp;
&lt;/system.serviceModel&gt;&nbsp;
&nbsp;</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<br></pre>
<p>3. Use HttpClient class to post json data to a web service and get the callback result.</p>
<pre><div class="scriptcode"><div class="pluginEditHolder" pluginCommand="mceScriptCode"><div class="title"><span>C#</span></div><div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div><span class="hidden">csharp</span><pre class="hidden">private async void Start_Click(object sender, RoutedEventArgs e)
        {
            // Clear text of Output textbox 
            this.OutputField.Text = string.Empty;
            this.statusText.Text = string.Empty;
 
            this.StartButton.IsEnabled = false;
            httpClient = new HttpClient();
            try
            {
                string resourceAddress = &quot;http://localhost:4848/WCFService.svc/GetData&quot;;
                int age = Convert.ToInt32(this.Agetxt.Text);
                if (age &gt; 120 || age &lt; 0)
                {
                    throw new Exception(&quot;Age must be between 0 and 120&quot;);
                }
                Person p = new Person { Name = this.Nametxt.Text, Age = age };
                string postBody = JsonSerializer(p);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(&quot;application/json&quot;));
                HttpResponseMessage wcfResponse = await httpClient.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, &quot;application/json&quot;));
                await DisplayTextResult(wcfResponse, OutputField);
            }
            catch (HttpRequestException hre)
            {
                NotifyUser(&quot;Error:&quot; &#43; hre.Message);
            }
            catch (TaskCanceledException)
            {
                NotifyUser(&quot;Request canceled.&quot;);
            }
            catch (Exception ex)
            {
                NotifyUser(ex.Message);
            }
            finally
            {
                this.StartButton.IsEnabled = true;
                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }
            }
        }</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;async&nbsp;<span class="cs__keyword">void</span>&nbsp;Start_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;RoutedEventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Clear&nbsp;text&nbsp;of&nbsp;Output&nbsp;textbox&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.OutputField.Text&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Empty;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.statusText.Text&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Empty;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StartButton.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">false</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpClient&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpClient();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;resourceAddress&nbsp;=&nbsp;<span class="cs__string">&quot;http://localhost:4848/WCFService.svc/GetData&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;age&nbsp;=&nbsp;Convert.ToInt32(<span class="cs__keyword">this</span>.Agetxt.Text);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(age&nbsp;&gt;&nbsp;<span class="cs__number">120</span>&nbsp;||&nbsp;age&nbsp;&lt;&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;Exception(<span class="cs__string">&quot;Age&nbsp;must&nbsp;be&nbsp;between&nbsp;0&nbsp;and&nbsp;120&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Person&nbsp;p&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Person&nbsp;{&nbsp;Name&nbsp;=&nbsp;<span class="cs__keyword">this</span>.Nametxt.Text,&nbsp;Age&nbsp;=&nbsp;age&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;postBody&nbsp;=&nbsp;JsonSerializer(p);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpClient.DefaultRequestHeaders.Accept.Add(<span class="cs__keyword">new</span>&nbsp;MediaTypeWithQualityHeaderValue(<span class="cs__string">&quot;application/json&quot;</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpResponseMessage&nbsp;wcfResponse&nbsp;=&nbsp;await&nbsp;httpClient.PostAsync(resourceAddress,&nbsp;<span class="cs__keyword">new</span>&nbsp;StringContent(postBody,&nbsp;Encoding.UTF8,&nbsp;<span class="cs__string">&quot;application/json&quot;</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;DisplayTextResult(wcfResponse,&nbsp;OutputField);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(HttpRequestException&nbsp;hre)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyUser(<span class="cs__string">&quot;Error:&quot;</span>&nbsp;&#43;&nbsp;hre.Message);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(TaskCanceledException)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyUser(<span class="cs__string">&quot;Request&nbsp;canceled.&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyUser(ex.Message);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">finally</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StartButton.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpClient&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpClient.Dispose();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpClient&nbsp;=&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<br></pre>
<p>&nbsp;</p>
<h2>More Information</h2>
<p>HttpClient Class</p>
<p><a href="http://msdn.microsoft.com/en-us/library/system.net.http.httpclient.aspx">http://msdn.microsoft.com/en-us/library/system.net.http.httpclient.aspx</a></p>
<p>DataContractJsonSerializer Class</p>
<p><a href="http://msdn.microsoft.com/en-us/library/system.runtime.serialization.json.datacontractjsonserializer.aspx">http://msdn.microsoft.com/en-us/library/system.runtime.serialization.json.datacontractjsonserializer.aspx</a></p>
