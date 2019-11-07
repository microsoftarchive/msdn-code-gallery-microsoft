# How to use Microsoft Translator API
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- .NET
## Topics
- Translator API
## Updated
- 05/14/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<h1><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"></a><span class="info-text"><strong><span style="font-size:10.0pt; line-height:105%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">How to use Microsoft Translator API in VB.NET application</span></strong></span><span class="info-text"><strong><span style="line-height:105%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">
</span></strong></span></h1>
<p><span style="font-size:x-small"><strong><span class="info-text"><span style="color:black; line-height:105%; font-family:&quot;Segoe UI&quot;,sans-serif">Requirement</span><span style="color:black; line-height:105%; font-family:&quot;Segoe UI&quot;,sans-serif">&nbsp;</span><span style="color:black; line-height:105%; font-family:&quot;Segoe UI&quot;,sans-serif">:
</span></span></strong></span><span class="info-text"><span style="font-size:10.0pt; line-height:105%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">To use Microsoft Translator API.
</span></span></p>
<p><span style="font-size:x-small"><strong>Technology:</strong> </span>VB, ASP.NET, Visual Studio 2012</p>
<p>&nbsp;</p>
<p>The sample demonstrates how to <span class="info-text"><span style="font-size:10.0pt; line-height:105%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">disable the script registered at the code later after execution in ASP.NET.
</span></span></p>
<p><span class="info-text"><span style="font-size:10.0pt; line-height:105%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black"><br>
</span></span></p>
<p><span style="font-size:x-small"><strong>Running&nbsp;the sample</strong></span></p>
<p><span style="font-size:x-small"><strong>Steps are listed at&nbsp;below:</strong></span></p>
<p>Add service reference to your web application using the following URL - <span lang="EN">
<a href="http://api.microsofttranslator.com/V2/Soap.svc">http://api.microsofttranslator.com/V2/Soap.svc</a>
</span></p>
<p>Now, before you invoke any of the exposed methods, you need to register your application via this portal:
<span style="font-size:9.5pt; line-height:105%; font-family:Consolas; color:green; background:white">
<a href="https://datamarket.azure.com/developer/applications/">https://datamarket.azure.com/developer/applications/</a></span><span style="font-size:9.5pt; line-height:105%; font-family:Consolas; color:green">
</span></p>
<p>Once registered, copy the <span class="SpellE"><span style="background:yellow">ClientID</span></span><span style="background:yellow"> and
<span class="SpellE">clientSecret</span></span> keys and keep them handy.</p>
<p>In the web application, add the following code to generate the Authentication token to make the WCF call.<span style="line-height:normal; font-family:&quot;Times New Roman&quot;; font-size:7pt; font-style:normal; font-variant:normal">&nbsp;</span></p>
<p><span style="line-height:normal; font-family:&quot;Times New Roman&quot;; font-size:7pt; font-style:normal; font-variant:normal">&nbsp;</span><span style="text-indent:-0.25in">1.Create the
</span><span class="SpellE" style="text-indent:-0.25in">DataContract</span><span style="text-indent:-0.25in"> to exchange data with the Microsoft Translator Service.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">&lt;DataContract&gt; _
Public Class AdmAccessToken
    &lt;DataMember&gt; _
    Public Property access_token() As String
        Get
            Return m_access_token
        End Get
        Set(value As String)
            m_access_token = value
        End Set
    End Property
    Private m_access_token As String
    &lt;DataMember&gt; _
    Public Property token_type() As String
        Get
            Return m_token_type
        End Get
        Set(value As String)
            m_token_type = value
        End Set
    End Property
    Private m_token_type As String
    &lt;DataMember&gt; _
    Public Property expires_in() As String
        Get
            Return m_expires_in
        End Get
        Set(value As String)
            m_expires_in = value
        End Set
    End Property
    Private m_expires_in As String
    &lt;DataMember&gt; _
    Public Property scope() As String
        Get
            Return m_scope
        End Get
        Set(value As String)
            m_scope = value
        End Set
    End Property
    Private m_scope As String
End Class</pre>
<div class="preview">
<pre class="vb">&lt;DataContract&gt;&nbsp;_&nbsp;
<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Class</span>&nbsp;AdmAccessToken&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;DataMember&gt;&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Property</span>&nbsp;access_token()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;m_access_token&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>(value&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;m_access_token&nbsp;=&nbsp;value&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Property</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;m_access_token&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;DataMember&gt;&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Property</span>&nbsp;token_type()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;m_token_type&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>(value&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;m_token_type&nbsp;=&nbsp;value&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Property</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;m_token_type&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;DataMember&gt;&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Property</span>&nbsp;expires_in()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;m_expires_in&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>(value&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;m_expires_in&nbsp;=&nbsp;value&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Property</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;m_expires_in&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;DataMember&gt;&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Property</span>&nbsp;scope()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;m_scope&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>(value&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;m_scope&nbsp;=&nbsp;value&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Property</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;m_scope&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Class</span></pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp; &nbsp; &nbsp; &nbsp;2.</span></span></span>Create a class for fetching access token to invoke the
<span class="SpellE">TranslatorService</span> API.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">Public Class AdmAuthentication
    Public Shared ReadOnly DatamarketAccessUri As String = &quot;https://datamarket.accesscontrol.windows.net/v2/OAuth2-13&quot;
    Private clientId As String
    Private clientSecret As String
    Private request As String
    Private token As AdmAccessToken
 
    Public Sub New(clientId As String, clientSecret As String)
        Me.clientId = clientId
        Me.clientSecret = clientSecret
        'If clientid or client secret has special characters, encode before sending request
        Me.request = String.Format(&quot;grant_type=client_credentials&amp;client_id={0}&amp;client_secret={1}&amp;scope=http://api.microsofttranslator.com&quot;, HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret))
        Me.token = HttpPost(DatamarketAccessUri, Me.request)
    End Sub
 
    Public Function GetAccessToken() As AdmAccessToken
        Return Me.token
    End Function
  
 
    Private Function HttpPost(DatamarketAccessUri As String, requestDetails As String) As AdmAccessToken
        'Prepare OAuth request
        Dim webRequest__1 As WebRequest = WebRequest.Create(DatamarketAccessUri)
        webRequest__1.ContentType = &quot;application/x-www-form-urlencoded&quot;
        webRequest__1.Method = &quot;POST&quot;
        Dim bytes As Byte() = Encoding.ASCII.GetBytes(requestDetails)
        webRequest__1.ContentLength = bytes.Length
        Using outputStream As Stream = webRequest__1.GetRequestStream()
            outputStream.Write(bytes, 0, bytes.Length)
        End Using
        Using webResponse As WebResponse = webRequest__1.GetResponse()
            Dim serializer As New DataContractJsonSerializer(GetType(AdmAccessToken))
            'Get deserialized object from JSON stream
            Dim token As AdmAccessToken = DirectCast(serializer.ReadObject(webResponse.GetResponseStream()), AdmAccessToken)
            Return token
        End Using
    End Function
End Class</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Class</span>&nbsp;AdmAuthentication&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Shared</span>&nbsp;<span class="visualBasic__keyword">ReadOnly</span>&nbsp;DatamarketAccessUri&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;<span class="visualBasic__string">&quot;https://datamarket.accesscontrol.windows.net/v2/OAuth2-13&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;clientId&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;clientSecret&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;request&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;token&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AdmAccessToken&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;<span class="visualBasic__keyword">New</span>(clientId&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>,&nbsp;clientSecret&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.clientId&nbsp;=&nbsp;clientId&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.clientSecret&nbsp;=&nbsp;clientSecret&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'If&nbsp;clientid&nbsp;or&nbsp;client&nbsp;secret&nbsp;has&nbsp;special&nbsp;characters,&nbsp;encode&nbsp;before&nbsp;sending&nbsp;request</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.request&nbsp;=&nbsp;<span class="visualBasic__keyword">String</span>.Format(<span class="visualBasic__string">&quot;grant_type=client_credentials&amp;client_id={0}&amp;client_secret={1}&amp;scope=http://api.microsofttranslator.com&quot;</span>,&nbsp;HttpUtility.UrlEncode(clientId),&nbsp;HttpUtility.UrlEncode(clientSecret))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.token&nbsp;=&nbsp;HttpPost(DatamarketAccessUri,&nbsp;<span class="visualBasic__keyword">Me</span>.request)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;GetAccessToken()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AdmAccessToken&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;<span class="visualBasic__keyword">Me</span>.token&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;HttpPost(DatamarketAccessUri&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>,&nbsp;requestDetails&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AdmAccessToken&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Prepare&nbsp;OAuth&nbsp;request</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;webRequest__1&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;WebRequest&nbsp;=&nbsp;WebRequest.Create(DatamarketAccessUri)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webRequest__1.ContentType&nbsp;=&nbsp;<span class="visualBasic__string">&quot;application/x-www-form-urlencoded&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webRequest__1.Method&nbsp;=&nbsp;<span class="visualBasic__string">&quot;POST&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;bytes&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Byte</span>()&nbsp;=&nbsp;Encoding.ASCII.GetBytes(requestDetails)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webRequest__1.ContentLength&nbsp;=&nbsp;bytes.Length&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;outputStream&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Stream&nbsp;=&nbsp;webRequest__1.GetRequestStream()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;outputStream.Write(bytes,&nbsp;<span class="visualBasic__number">0</span>,&nbsp;bytes.Length)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;webResponse&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;WebResponse&nbsp;=&nbsp;webRequest__1.GetResponse()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;serializer&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;DataContractJsonSerializer(<span class="visualBasic__keyword">GetType</span>(AdmAccessToken))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Get&nbsp;deserialized&nbsp;object&nbsp;from&nbsp;JSON&nbsp;stream</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;token&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AdmAccessToken&nbsp;=&nbsp;<span class="visualBasic__keyword">DirectCast</span>(serializer.ReadObject(webResponse.GetResponseStream()),&nbsp;AdmAccessToken)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;token&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Class</span></pre>
</div>
</div>
</div>
<p><strong><span style="line-height:105%; font-size:x-small">Detect button click event:
</span></strong></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim admToken As AdmAccessToken
        Dim headerValue As String
        'Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
        'Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx)           
        Dim admAuth As New AdmAuthentication(&quot;Your Client ID&quot;, &quot;Client Secret Key&quot;)
        Try
            admToken = admAuth.GetAccessToken()
            Dim tokenReceived As DateTime = DateTime.Now
            ' Create a header with the access_token property of the returned
            headerValue = &quot;Bearer &quot; &#43; admToken.access_token
            DetectMethod(headerValue)
        Catch ex As Exception
        End Try
    End Sub
 
 
 
    Private Sub DetectMethod(authToken As String)
        Dim client As New ServiceReference1.LanguageServiceClient()
 
        Dim httpRequestProperty As New HttpRequestMessageProperty()
        httpRequestProperty.Method = &quot;POST&quot;
        httpRequestProperty.Headers.Add(&quot;Authorization&quot;, authToken)
        Using scope As New OperationContextScope(client.InnerChannel)
            OperationContext.Current.OutgoingMessageProperties(HttpRequestMessageProperty.Name) = httpRequestProperty
            'Below line will return the code of the detected language.
            Dim strDetectedLangCode As String() = {client.Detect(&quot;&quot;, txtUser.Text)}
            detectedLangCode = strDetectedLangCode(0)
            'Fetch the name of the detected language using the code.
            Dim strDetectedLang As String() = client.GetLanguageNames(&quot;&quot;, &quot;en&quot;, strDetectedLangCode, True)
            lblDetectedText.Text = strDetectedLang(0)
 
            'Fetching the list of supported languages and binding to the dropdown list.
            languagesForTranslate = client.GetLanguagesForTranslate(&quot;&quot;)
            allLang = client.GetLanguageNames(&quot;&quot;, &quot;en&quot;, languagesForTranslate, True)
            drpAllLang.DataSource = allLang
            drpAllLang.DataBind()
        End Using
 
    End Sub</pre>
<div class="preview">
<pre class="vb">&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Protected</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;Button1_Click(sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Object</span>,&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;EventArgs)&nbsp;<span class="visualBasic__keyword">Handles</span>&nbsp;Button1.Click&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;admToken&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AdmAccessToken&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;headerValue&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Get&nbsp;Client&nbsp;Id&nbsp;and&nbsp;Client&nbsp;Secret&nbsp;from&nbsp;https://datamarket.azure.com/developer/applications/</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Refer&nbsp;obtaining&nbsp;AccessToken&nbsp;(http://msdn.microsoft.com/en-us/library/hh454950.aspx)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;admAuth&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;AdmAuthentication(<span class="visualBasic__string">&quot;Your&nbsp;Client&nbsp;ID&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;Client&nbsp;Secret&nbsp;Key&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;admToken&nbsp;=&nbsp;admAuth.GetAccessToken()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;tokenReceived&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;DateTime&nbsp;=&nbsp;DateTime.Now&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Create&nbsp;a&nbsp;header&nbsp;with&nbsp;the&nbsp;access_token&nbsp;property&nbsp;of&nbsp;the&nbsp;returned</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;headerValue&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Bearer&nbsp;&quot;</span>&nbsp;&#43;&nbsp;admToken.access_token&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DetectMethod(headerValue)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Catch</span>&nbsp;ex&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Exception&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;DetectMethod(authToken&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;client&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ServiceReference1.LanguageServiceClient()&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;httpRequestProperty&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;HttpRequestMessageProperty()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpRequestProperty.Method&nbsp;=&nbsp;<span class="visualBasic__string">&quot;POST&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpRequestProperty.Headers.Add(<span class="visualBasic__string">&quot;Authorization&quot;</span>,&nbsp;authToken)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;scope&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;OperationContextScope(client.InnerChannel)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OperationContext.Current.OutgoingMessageProperties(HttpRequestMessageProperty.Name)&nbsp;=&nbsp;httpRequestProperty&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Below&nbsp;line&nbsp;will&nbsp;return&nbsp;the&nbsp;code&nbsp;of&nbsp;the&nbsp;detected&nbsp;language.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;strDetectedLangCode&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>()&nbsp;=&nbsp;{client.Detect(<span class="visualBasic__string">&quot;&quot;</span>,&nbsp;txtUser.Text)}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;detectedLangCode&nbsp;=&nbsp;strDetectedLangCode(<span class="visualBasic__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Fetch&nbsp;the&nbsp;name&nbsp;of&nbsp;the&nbsp;detected&nbsp;language&nbsp;using&nbsp;the&nbsp;code.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;strDetectedLang&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>()&nbsp;=&nbsp;client.GetLanguageNames(<span class="visualBasic__string">&quot;&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;en&quot;</span>,&nbsp;strDetectedLangCode,&nbsp;<span class="visualBasic__keyword">True</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lblDetectedText.Text&nbsp;=&nbsp;strDetectedLang(<span class="visualBasic__number">0</span>)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Fetching&nbsp;the&nbsp;list&nbsp;of&nbsp;supported&nbsp;languages&nbsp;and&nbsp;binding&nbsp;to&nbsp;the&nbsp;dropdown&nbsp;list.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;languagesForTranslate&nbsp;=&nbsp;client.GetLanguagesForTranslate(<span class="visualBasic__string">&quot;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;allLang&nbsp;=&nbsp;client.GetLanguageNames(<span class="visualBasic__string">&quot;&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;en&quot;</span>,&nbsp;languagesForTranslate,&nbsp;<span class="visualBasic__keyword">True</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;drpAllLang.DataSource&nbsp;=&nbsp;allLang&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;drpAllLang.DataBind()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<p class="MsoNormal"><span style="font-size:x-small"><strong><span style="line-height:105%">Translate Button click event
</span></strong></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim admToken As AdmAccessToken
        Dim headerValue As String
        'Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
        'Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx)           
        Dim admAuth As New AdmAuthentication(&quot;Your Client ID &quot;, &quot; Client Secret Key &quot;)
        Try
            admToken = admAuth.GetAccessToken()
            Dim tokenReceived As DateTime = DateTime.Now
            ' Create a header with the access_token property of the returned
            headerValue = &quot;Bearer &quot; &#43; admToken.access_token
            TranslateMethod(headerValue)
        Catch ex As Exception
        End Try
    End Sub
 
 
 Private Sub TranslateMethod(auToken As String)
        Dim client As New ServiceReference1.LanguageServiceClient()
        'Set Authorization header before sending the request
        Dim httpRequestProperty As New HttpRequestMessageProperty()
        httpRequestProperty.Method = &quot;POST&quot;
        httpRequestProperty.Headers.Add(&quot;Authorization&quot;, auToken)
        ' Creates a block within which an OperationContext object is in scope.
        Using scope As New OperationContextScope(client.InnerChannel)
            OperationContext.Current.OutgoingMessageProperties(HttpRequestMessageProperty.Name) = httpRequestProperty
            Dim translationResult As String
            languagesForTranslate = client.GetLanguagesForTranslate(&quot;&quot;)
            detectedLangCode = client.Detect(&quot;&quot;, txtUser.Text)
            translationResult = client.Translate(&quot;&quot;, txtUser.Text, detectedLangCode, languagesForTranslate(drpAllLang.SelectedIndex), &quot;text/html&quot;, &quot;general&quot;, _
                &quot;&quot;)
            txtTranslated.Text = translationResult
        End Using
    End Sub</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Protected</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;Button2_Click(sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Object</span>,&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;EventArgs)&nbsp;<span class="visualBasic__keyword">Handles</span>&nbsp;Button2.Click&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;admToken&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AdmAccessToken&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;headerValue&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Get&nbsp;Client&nbsp;Id&nbsp;and&nbsp;Client&nbsp;Secret&nbsp;from&nbsp;https://datamarket.azure.com/developer/applications/</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Refer&nbsp;obtaining&nbsp;AccessToken&nbsp;(http://msdn.microsoft.com/en-us/library/hh454950.aspx)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;admAuth&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;AdmAuthentication(<span class="visualBasic__string">&quot;Your&nbsp;Client&nbsp;ID&nbsp;&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;&nbsp;Client&nbsp;Secret&nbsp;Key&nbsp;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;admToken&nbsp;=&nbsp;admAuth.GetAccessToken()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;tokenReceived&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;DateTime&nbsp;=&nbsp;DateTime.Now&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Create&nbsp;a&nbsp;header&nbsp;with&nbsp;the&nbsp;access_token&nbsp;property&nbsp;of&nbsp;the&nbsp;returned</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;headerValue&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Bearer&nbsp;&quot;</span>&nbsp;&#43;&nbsp;admToken.access_token&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TranslateMethod(headerValue)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Catch</span>&nbsp;ex&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Exception&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;TranslateMethod(auToken&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;client&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ServiceReference1.LanguageServiceClient()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Set&nbsp;Authorization&nbsp;header&nbsp;before&nbsp;sending&nbsp;the&nbsp;request</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;httpRequestProperty&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;HttpRequestMessageProperty()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpRequestProperty.Method&nbsp;=&nbsp;<span class="visualBasic__string">&quot;POST&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpRequestProperty.Headers.Add(<span class="visualBasic__string">&quot;Authorization&quot;</span>,&nbsp;auToken)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Creates&nbsp;a&nbsp;block&nbsp;within&nbsp;which&nbsp;an&nbsp;OperationContext&nbsp;object&nbsp;is&nbsp;in&nbsp;scope.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;scope&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;OperationContextScope(client.InnerChannel)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OperationContext.Current.OutgoingMessageProperties(HttpRequestMessageProperty.Name)&nbsp;=&nbsp;httpRequestProperty&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;translationResult&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;languagesForTranslate&nbsp;=&nbsp;client.GetLanguagesForTranslate(<span class="visualBasic__string">&quot;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;detectedLangCode&nbsp;=&nbsp;client.Detect(<span class="visualBasic__string">&quot;&quot;</span>,&nbsp;txtUser.Text)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;translationResult&nbsp;=&nbsp;client.Translate(<span class="visualBasic__string">&quot;&quot;</span>,&nbsp;txtUser.Text,&nbsp;detectedLangCode,&nbsp;languagesForTranslate(drpAllLang.SelectedIndex),&nbsp;<span class="visualBasic__string">&quot;text/html&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;general&quot;</span>,&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__string">&quot;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;txtTranslated.Text&nbsp;=&nbsp;translationResult&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<p class="MsoNormal"><strong><span style="text-decoration:underline"><span style="font-size:12.0pt; line-height:105%">Screenshot:
</span></span></strong></p>
<p class="MsoNormal" style="margin-left:.5in"><span><img src="173094-image.png" alt="" width="624" height="438" align="middle">
</span><strong>&nbsp;</strong></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
