# How to use Microsoft Translator API
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- .NET
## Topics
- Translator API
## Updated
- 10/28/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="http://aka.ms/onecodesampletopbanner1" alt="">
</a></div>
<h1><a name="OLE_LINK1"><span style="font-size:13.5pt; font-family:&quot;Segoe UI Light&quot;,sans-serif; color:black; background:#FCFCFC">How to use Microsoft Translator API</span></a></h1>
<h2>Introduction</h2>
<p class="MsoNormal">This sample demonstrates how to disable script registered at code after the execution in ASP.NET.</p>
<h2>Running the sample</h2>
<p style="margin:0in; margin-bottom:.0001pt"><span style="font-size:10.0pt; font-family:&quot;Calibri&quot;,sans-serif">Do one of the following:</span></p>
<p style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in; text-indent:-.25in">
<span style="font-size:10.0pt; font-family:Symbol">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:10.0pt; font-family:&quot;Calibri&quot;,sans-serif">Click the Start Debugging button on the toolbar.</span></p>
<p style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in; text-indent:-.25in">
<span style="font-size:10.0pt; font-family:Symbol">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:10.0pt; font-family:&quot;Calibri&quot;,sans-serif">Click Start Debugging in the Debug menu.</span></p>
<p style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in; text-indent:-.25in">
<span style="font-size:10.0pt; font-family:Symbol">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:10.0pt; font-family:&quot;Calibri&quot;,sans-serif">Press F5.</span></p>
<p style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in">
<span style="font-size:10.0pt; font-family:&quot;Calibri&quot;,sans-serif">&nbsp;</span></p>
<p style="margin:0in; margin-bottom:.0001pt"><span style="font-size:10.0pt; font-family:&quot;Calibri&quot;,sans-serif">Enter any text in the first text box and click detect.</span></p>
<p style="margin:0in; margin-bottom:.0001pt"><span style="font-size:10.0pt; font-family:&quot;Calibri&quot;,sans-serif">Now, select the desired language in the dropdown list， and then click convert.</span></p>
<p class="MsoNormal"><img src="162497-image.png" alt="" width="576" height="399" align="middle"></p>
<p class="MsoNormal"><strong><span style="font-size:12.0pt; font-family:&quot;Calibri Light&quot;,sans-serif">Using the code</span></strong></p>
<p class="MsoNormal">Add service reference to your web application using the following URL -
<a href="http://api.microsofttranslator.com/V2/Soap.svc">http://api.microsofttranslator.com/V2/Soap.svc</a></p>
<p class="MsoNormal">Now, before you can invoke any of the exposed methods, you need to register your application on the portal -
<a href="https://datamarket.azure.com/developer/applications/">https://datamarket.azure.com/developer/applications/</a></p>
<p class="MsoNormal">Once registered, copy the ClientID and clientSecret keys&nbsp;to keep them handy.</p>
<p class="MsoNormal">In the web application, add the following code to generate the Authentication token to make the WCF call.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">[DataContract]&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;AdmAccessToken&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;[DataMember]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;access_token&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;[DataMember]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;token_type&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;[DataMember]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;expires_in&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;[DataMember]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;scope&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
}&nbsp;
&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;AdmAuthentication&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">readonly</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;DatamarketAccessUri&nbsp;=&nbsp;<span class="cs__string">&quot;https://datamarket.accesscontrol.windows.net/v2/OAuth2-13&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;clientId;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;clientSecret;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;request;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;AdmAccessToken&nbsp;token;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;System.Threading.Timer&nbsp;accessTokenRenewer;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Access&nbsp;token&nbsp;expires&nbsp;every&nbsp;10&nbsp;minutes.&nbsp;Renew&nbsp;it&nbsp;every&nbsp;9&nbsp;minutes&nbsp;only.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">const</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;RefreshTokenDuration&nbsp;=&nbsp;<span class="cs__number">9</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;AdmAuthentication(<span class="cs__keyword">string</span>&nbsp;clientId,&nbsp;<span class="cs__keyword">string</span>&nbsp;clientSecret)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.clientId&nbsp;=&nbsp;clientId;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.clientSecret&nbsp;=&nbsp;clientSecret;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//If&nbsp;clientid&nbsp;or&nbsp;client&nbsp;secret&nbsp;has&nbsp;special&nbsp;characters,&nbsp;encode&nbsp;before&nbsp;sending&nbsp;request</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.request&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Format(<span class="cs__string">&quot;grant_type=client_credentials&amp;client_id={0}&amp;client_secret={1}&amp;scope=http://api.microsofttranslator.com&quot;</span>,&nbsp;HttpUtility.UrlEncode(clientId),&nbsp;HttpUtility.UrlEncode(clientSecret));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.token&nbsp;=&nbsp;HttpPost(DatamarketAccessUri,&nbsp;<span class="cs__keyword">this</span>.request);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//renew&nbsp;the&nbsp;token&nbsp;every&nbsp;specified&nbsp;minutes</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;accessTokenRenewer&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Threading.Timer.aspx" target="_blank" title="Auto generated link to System.Threading.Timer">System.Threading.Timer</a>(<span class="cs__keyword">new</span>&nbsp;TimerCallback(OnTokenExpiredCallback),&nbsp;<span class="cs__keyword">this</span>,&nbsp;TimeSpan.FromMinutes(RefreshTokenDuration),&nbsp;TimeSpan.FromMilliseconds(-<span class="cs__number">1</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;AdmAccessToken&nbsp;GetAccessToken()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">this</span>.token;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;RenewAccessToken()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AdmAccessToken&nbsp;newAccessToken&nbsp;=&nbsp;HttpPost(DatamarketAccessUri,&nbsp;<span class="cs__keyword">this</span>.request);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//swap&nbsp;the&nbsp;new&nbsp;token&nbsp;with&nbsp;old&nbsp;one</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Note:&nbsp;the&nbsp;swap&nbsp;is&nbsp;thread&nbsp;unsafe</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.token&nbsp;=&nbsp;newAccessToken;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__keyword">string</span>.Format(<span class="cs__string">&quot;Renewed&nbsp;token&nbsp;for&nbsp;user:&nbsp;{0}&nbsp;is:&nbsp;{1}&quot;</span>,&nbsp;<span class="cs__keyword">this</span>.clientId,&nbsp;<span class="cs__keyword">this</span>.token.access_token));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnTokenExpiredCallback(<span class="cs__keyword">object</span>&nbsp;stateInfo)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RenewAccessToken();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__keyword">string</span>.Format(<span class="cs__string">&quot;Failed&nbsp;renewing&nbsp;access&nbsp;token.&nbsp;Details:&nbsp;{0}&quot;</span>,&nbsp;ex.Message));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">finally</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration),&nbsp;TimeSpan.FromMilliseconds(-<span class="cs__number">1</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__keyword">string</span>.Format(<span class="cs__string">&quot;Failed&nbsp;to&nbsp;reschedule&nbsp;the&nbsp;timer&nbsp;to&nbsp;renew&nbsp;access&nbsp;token.&nbsp;Details:&nbsp;{0}&quot;</span>,&nbsp;ex.Message));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;AdmAccessToken&nbsp;HttpPost(<span class="cs__keyword">string</span>&nbsp;DatamarketAccessUri,&nbsp;<span class="cs__keyword">string</span>&nbsp;requestDetails)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Prepare&nbsp;OAuth&nbsp;request</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WebRequest&nbsp;webRequest&nbsp;=&nbsp;WebRequest.Create(DatamarketAccessUri);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webRequest.ContentType&nbsp;=&nbsp;<span class="cs__string">&quot;application/x-www-form-urlencoded&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webRequest.Method&nbsp;=&nbsp;<span class="cs__string">&quot;POST&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">byte</span>[]&nbsp;bytes&nbsp;=&nbsp;Encoding.ASCII.GetBytes(requestDetails);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webRequest.ContentLength&nbsp;=&nbsp;bytes.Length;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(Stream&nbsp;outputStream&nbsp;=&nbsp;webRequest.GetRequestStream())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;outputStream.Write(bytes,&nbsp;<span class="cs__number">0</span>,&nbsp;bytes.Length);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(WebResponse&nbsp;webResponse&nbsp;=&nbsp;webRequest.GetResponse())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DataContractJsonSerializer&nbsp;serializer&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DataContractJsonSerializer(<span class="cs__keyword">typeof</span>(AdmAccessToken));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Get&nbsp;deserialized&nbsp;object&nbsp;from&nbsp;JSON&nbsp;stream</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AdmAccessToken&nbsp;token&nbsp;=&nbsp;(AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;token;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<p class="MsoNormal">Detect button click event:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Button1_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;AdmAccessToken&nbsp;admToken;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;headerValue;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Get&nbsp;Client&nbsp;Id&nbsp;and&nbsp;Client&nbsp;Secret&nbsp;from&nbsp;https://datamarket.azure.com/developer/applications/</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Refer&nbsp;obtaining&nbsp;AccessToken&nbsp;(http://msdn.microsoft.com/en-us/library/hh454950.aspx)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;AdmAuthentication&nbsp;admAuth&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;AdmAuthentication(&lt;&nbsp;clientID&nbsp;&gt;,&lt;&nbsp;clientSecret&nbsp;&gt;);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;admToken&nbsp;=&nbsp;admAuth.GetAccessToken();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DateTime&nbsp;tokenReceived&nbsp;=&nbsp;DateTime.Now;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Create&nbsp;a&nbsp;header&nbsp;with&nbsp;the&nbsp;access_token&nbsp;property&nbsp;of&nbsp;the&nbsp;returned</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;headerValue&nbsp;=&nbsp;<span class="cs__string">&quot;Bearer&nbsp;&quot;</span>&nbsp;&#43;&nbsp;admToken.access_token;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DetectMethod(headerValue);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;}&nbsp;
}&nbsp;
&nbsp;
<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;DetectMethod(<span class="cs__keyword">string</span>&nbsp;authToken)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;TranslatorService.LanguageServiceClient&nbsp;client&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;TranslatorService.LanguageServiceClient();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Set&nbsp;Authorization&nbsp;header&nbsp;before&nbsp;sending&nbsp;the&nbsp;request</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpRequestMessageProperty&nbsp;httpRequestProperty&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpRequestMessageProperty();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpRequestProperty.Method&nbsp;=&nbsp;<span class="cs__string">&quot;POST&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpRequestProperty.Headers.Add(<span class="cs__string">&quot;Authorization&quot;</span>,&nbsp;authToken);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(OperationContextScope&nbsp;scope&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;OperationContextScope(client.InnerChannel))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name]&nbsp;=&nbsp;httpRequestProperty;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Below&nbsp;line&nbsp;will&nbsp;return&nbsp;the&nbsp;code&nbsp;of&nbsp;the&nbsp;detected&nbsp;language.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>[]&nbsp;strDetectedLangCode&nbsp;=&nbsp;{&nbsp;client.Detect(<span class="cs__string">&quot;&quot;</span>,&nbsp;txtUser.Text)&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;detectedLangCode&nbsp;=&nbsp;strDetectedLangCode[<span class="cs__number">0</span>];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Fetch&nbsp;the&nbsp;name&nbsp;of&nbsp;the&nbsp;detected&nbsp;language&nbsp;using&nbsp;the&nbsp;code.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>[]&nbsp;strDetectedLang&nbsp;=&nbsp;client.GetLanguageNames(<span class="cs__string">&quot;&quot;</span>,&nbsp;<span class="cs__string">&quot;en&quot;</span>,&nbsp;strDetectedLangCode,&nbsp;<span class="cs__keyword">true</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lblDetectedText.Text&nbsp;=&nbsp;strDetectedLang[<span class="cs__number">0</span>];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Fetching&nbsp;the&nbsp;list&nbsp;of&nbsp;supported&nbsp;languages&nbsp;and&nbsp;binding&nbsp;to&nbsp;the&nbsp;dropdown&nbsp;list.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;languagesForTranslate&nbsp;=&nbsp;client.GetLanguagesForTranslate(<span class="cs__string">&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;allLang&nbsp;=&nbsp;client.GetLanguageNames(<span class="cs__string">&quot;&quot;</span>,&nbsp;<span class="cs__string">&quot;en&quot;</span>,&nbsp;languagesForTranslate,&nbsp;<span class="cs__keyword">true</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;drpAllLang.DataSource&nbsp;=&nbsp;allLang;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;drpAllLang.DataBind();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<p class="MsoNormal">Translate button click event</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Button2_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;AdmAccessToken&nbsp;admToken;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;headerValue;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Get&nbsp;Client&nbsp;Id&nbsp;and&nbsp;Client&nbsp;Secret&nbsp;from&nbsp;https://datamarket.azure.com/developer/applications/</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Refer&nbsp;obtaining&nbsp;AccessToken&nbsp;(http://msdn.microsoft.com/en-us/library/hh454950.aspx)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;AdmAuthentication&nbsp;admAuth&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;AdmAuthentication(&lt;&nbsp;clientID&nbsp;&gt;,&lt;&nbsp;clientSecret&nbsp;&gt;);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;admToken&nbsp;=&nbsp;admAuth.GetAccessToken();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DateTime&nbsp;tokenReceived&nbsp;=&nbsp;DateTime.Now;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Create&nbsp;a&nbsp;header&nbsp;with&nbsp;the&nbsp;access_token&nbsp;property&nbsp;of&nbsp;the&nbsp;returned</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;headerValue&nbsp;=&nbsp;<span class="cs__string">&quot;Bearer&nbsp;&quot;</span>&nbsp;&#43;&nbsp;admToken.access_token;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TranslateMethod(headerValue);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;}&nbsp;
}&nbsp;
&nbsp;
<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;TranslateMethod(<span class="cs__keyword">string</span>&nbsp;auToken)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;TranslatorService.LanguageServiceClient&nbsp;client&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;TranslatorService.LanguageServiceClient();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Set&nbsp;Authorization&nbsp;header&nbsp;before&nbsp;sending&nbsp;the&nbsp;request</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpRequestMessageProperty&nbsp;httpRequestProperty&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpRequestMessageProperty();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpRequestProperty.Method&nbsp;=&nbsp;<span class="cs__string">&quot;POST&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpRequestProperty.Headers.Add(<span class="cs__string">&quot;Authorization&quot;</span>,&nbsp;auToken);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Creates&nbsp;a&nbsp;block&nbsp;within&nbsp;which&nbsp;an&nbsp;OperationContext&nbsp;object&nbsp;is&nbsp;in&nbsp;scope.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(OperationContextScope&nbsp;scope&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;OperationContextScope(client.InnerChannel))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name]&nbsp;=&nbsp;httpRequestProperty;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;translationResult;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;languagesForTranslate&nbsp;=&nbsp;client.GetLanguagesForTranslate(<span class="cs__string">&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;detectedLangCode&nbsp;=&nbsp;client.Detect(<span class="cs__string">&quot;&quot;</span>,&nbsp;txtUser.Text);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;translationResult&nbsp;=&nbsp;client.Translate(<span class="cs__string">&quot;&quot;</span>,&nbsp;txtUser.Text,&nbsp;detectedLangCode,&nbsp;languagesForTranslate[drpAllLang.SelectedIndex],&nbsp;<span class="cs__string">&quot;text/html&quot;</span>,&nbsp;<span class="cs__string">&quot;general&quot;</span>,&nbsp;<span class="cs__string">&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;txtTranslated.Text&nbsp;=&nbsp;translationResult;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
