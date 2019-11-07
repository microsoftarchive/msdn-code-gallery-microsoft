# How to use RESTful WCF Service with HTTP requests like "Get/Post/Put/Delete" [
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- .NET Development
- Windows Communication Framework (WCF)
## Topics
- REST
- HTTP Request
- Get Post Put Delete
## Updated
- 06/13/2013
## Description

<h1>How to use RESTful WCF Service with HTTP requests like &quot;Get/Post/Put/Delete&quot; in VB (VBRESTfulWCFService)</h1>
<h2>Introduction</h2>
<p class="MsoNormal"><span style="">This article and the attached code sample demonstrate how to use RESTful WCF Service.</span>
<span style="">REST defines an architectural style based on a set of constraints for building things the &quot;Web&quot; way. REST is not tied to any particular technology or platform – it's simply a way to design things to work like the &quot;<b style="">Web</b>&quot;.
 People often refer to services that follow this philosophy as &quot;RESTful services.&quot; In this sample, we'll cover the fundamental REST design principles and show you how to build a RESTful service with Windows Communication Foundation (WCF), and with
 &quot;<b style="">Get/Post/Put/Delete</b>&quot; request in the ASPNET MVC client. You can find the answers for all the following questions in the code sample:
</span></p>
<p class="MsoListBulletCxSpFirst" style=""><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">How to define a RESTful WCF service with &quot;<b style="">Get/Post/Put/Delete</b>&quot;?
</span></p>
<p class="MsoListBulletCxSpLast" style=""><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">How to use HttpWebRequest to invoke a RESTful WCF service?
</span></p>
<h2>Building the Sample</h2>
<p class="MsoNormal"><span style="">To build this sample </span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Open VBRESTfulWCFService.sln solution file in Visual Studio 2012
</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Build<span style="">&nbsp; </span>entire solution </span>
</p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style=""><br>
After opening the sample project in Visual Studio 2012, you will find two projects:
<b style="">VBRESTfulWCFServiceASPNETClient </b>and <b style="">VBRESTfulWCFServiceProvider</b>.
<b style="">VBRESTfulWCFServiceASPNETClient </b>is a client project to use HttpWebRequest to invoke the RESTful WCF Service and do operations like: Get/Create/Update/Delete users.
<b style="">VBRESTfulWCFServiceProvider </b>is a WCF Service to provide Get/Post/Put/Delete methods for client invoker. Build entire solution and press F5 to start debugging.
</span></p>
<p class="MsoNormal"><span style="">The initialization will get all users with a &quot;<b style="">Get</b>&quot; method from Restful WCF Service provider. You can find the page like below:
</span></p>
<p class="MsoNormal"><span style=""><img src="84610-image.png" alt="" width="763" height="333" align="middle">
</span><span style=""></span></p>
<p class="MsoListBulletCxSpFirst" style=""><span style=""></span></p>
<p class="MsoListBulletCxSpMiddle" style=""><span style="">We can use &quot;<b style="">Create</b>
<b style="">User</b>&quot;,&quot;<b style="">Edit</b>&quot; or &quot;<b style="">Delete</b>&quot; buttons to operate user list. If you want to create a user, click &quot;<b style="">Create User</b>&quot; button. You can find the page like below:
</span></p>
<p class="MsoListBulletCxSpMiddle" style=""><span style=""></span></p>
<p class="MsoListBulletCxSpMiddle" style=""><span style=""><img src="84611-image.png" alt="" width="763" height="493" align="middle">
</span><span style=""></span></p>
<p class="MsoListBulletCxSpMiddle" style=""><span style=""></span></p>
<p class="MsoListBulletCxSpMiddle" style=""><span style="">After typing user information and clicking &quot;<b style="">Create</b>&quot; button, we will find a new user is created.
</span></p>
<p class="MsoListBulletCxSpMiddle" style=""><span style=""></span></p>
<p class="MsoListBulletCxSpMiddle" style=""><span style=""><img src="84612-image.png" alt="" width="767" height="358" align="middle">
</span><span style=""></span></p>
<p class="MsoListBulletCxSpLast" style="margin-left:0in; text-indent:0in"><span style=""></span></p>
<h2>Using the Code</h2>
<p class="MsoNormal"><span style="">The code sample provides the following reusable functions for creating and using RESTfule WCF Service.
</span></p>
<p class="MsoNormal"><b style=""><span style="">How to define a RESTful WCF service with &quot;Get/Post/Put/Delete&quot;?
</span></b></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
''' &lt;summary&gt;
''' WCF Service interface to define operations
''' &lt;/summary&gt;
&lt;ServiceContract(), DataContractFormat()&gt;
Friend Interface IUserService


&nbsp;&nbsp;&nbsp; ''' &lt;summary&gt;
&nbsp;&nbsp;&nbsp; ''' Definde operation contract
&nbsp;&nbsp;&nbsp; ''' &lt;/summary&gt;
&nbsp;&nbsp;&nbsp; ''' &lt;param name=&quot;user&quot;&gt;&lt;/param&gt;
&nbsp;&nbsp;&nbsp; ''' &lt;remarks&gt;&lt;/remarks&gt;
&nbsp;&nbsp;&nbsp; &lt;OperationContract(),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WebInvoke(Method:=&quot;POST&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; UriTemplate:=&quot;/User/Create&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RequestFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ResponseFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BodyStyle:=WebMessageBodyStyle.Bare)&gt;
&nbsp;&nbsp;&nbsp; Sub CreateUser(ByVal user As User)


&nbsp;&nbsp;&nbsp; &lt;WebInvoke(Method:=&quot;DELETE&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;UriTemplate:=&quot;/User/Delete/{Id}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RequestFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ResponseFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BodyStyle:=WebMessageBodyStyle.Bare),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; OperationContract()&gt;
&nbsp;&nbsp;&nbsp; Sub DeleteUser(ByVal id As String)


&nbsp;&nbsp;&nbsp; &lt;OperationContract(),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WebGet(UriTemplate:=&quot;/User/All&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RequestFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ResponseFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BodyStyle:=WebMessageBodyStyle.Bare)&gt;
&nbsp;&nbsp;&nbsp; Function GetAllUsers() As List(Of User)


&nbsp;&nbsp;&nbsp; &lt;OperationContract(),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WebInvoke(Method:=&quot;PUT&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; UriTemplate:=&quot;/User/Edit&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RequestFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ResponseFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BodyStyle:=WebMessageBodyStyle.Bare)&gt;
&nbsp;&nbsp;&nbsp; Sub UpdateUser(ByVal user As User)
End Interface

</pre>
<pre id="codePreview" class="vb">
''' &lt;summary&gt;
''' WCF Service interface to define operations
''' &lt;/summary&gt;
&lt;ServiceContract(), DataContractFormat()&gt;
Friend Interface IUserService


&nbsp;&nbsp;&nbsp; ''' &lt;summary&gt;
&nbsp;&nbsp;&nbsp; ''' Definde operation contract
&nbsp;&nbsp;&nbsp; ''' &lt;/summary&gt;
&nbsp;&nbsp;&nbsp; ''' &lt;param name=&quot;user&quot;&gt;&lt;/param&gt;
&nbsp;&nbsp;&nbsp; ''' &lt;remarks&gt;&lt;/remarks&gt;
&nbsp;&nbsp;&nbsp; &lt;OperationContract(),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WebInvoke(Method:=&quot;POST&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; UriTemplate:=&quot;/User/Create&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RequestFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ResponseFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BodyStyle:=WebMessageBodyStyle.Bare)&gt;
&nbsp;&nbsp;&nbsp; Sub CreateUser(ByVal user As User)


&nbsp;&nbsp;&nbsp; &lt;WebInvoke(Method:=&quot;DELETE&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;UriTemplate:=&quot;/User/Delete/{Id}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RequestFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ResponseFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BodyStyle:=WebMessageBodyStyle.Bare),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; OperationContract()&gt;
&nbsp;&nbsp;&nbsp; Sub DeleteUser(ByVal id As String)


&nbsp;&nbsp;&nbsp; &lt;OperationContract(),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WebGet(UriTemplate:=&quot;/User/All&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RequestFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ResponseFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BodyStyle:=WebMessageBodyStyle.Bare)&gt;
&nbsp;&nbsp;&nbsp; Function GetAllUsers() As List(Of User)


&nbsp;&nbsp;&nbsp; &lt;OperationContract(),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WebInvoke(Method:=&quot;PUT&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; UriTemplate:=&quot;/User/Edit&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RequestFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ResponseFormat:=WebMessageFormat.Json,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BodyStyle:=WebMessageBodyStyle.Bare)&gt;
&nbsp;&nbsp;&nbsp; Sub UpdateUser(ByVal user As User)
End Interface

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoNormal"><b style=""><span style="">How to use HttpWebRequest to invoke a RESTful WCF service?
</span></b></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
''' &lt;summary&gt;
''' Send the request to WCF Service
''' &lt;/summary&gt;
''' &lt;param name=&quot;template&quot;&gt;Object template like: User&lt;/param&gt;
''' &lt;param name=&quot;action&quot;&gt;Action like: Delete&lt;/param&gt;
''' &lt;param name=&quot;method&quot;&gt;Request method&lt;/param&gt;
''' &lt;param name=&quot;t&quot;&gt;Object like: User&lt;/param&gt;
Private Sub SendRequest(ByVal template As String,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal action As String,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal method As HttpMethod,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal t As T)
&nbsp;&nbsp;&nbsp; Dim jsonData As String = JsonHelp.JsonSerialize(Of T)(t)
&nbsp;&nbsp;&nbsp; If String.IsNullOrEmpty(jsonData) Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Return
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; Dim data As Byte() = UnicodeEncoding.UTF8.GetBytes(jsonData)


&nbsp;&nbsp;&nbsp; httpRequest = DirectCast(WebRequest.Create(String.Format(basicUrl, template, action)), HttpWebRequest)
&nbsp;&nbsp;&nbsp; httpRequest.Method = method.ToString()
&nbsp;&nbsp;&nbsp; httpRequest.ContentType = &quot;application/json&quot;
&nbsp;&nbsp;&nbsp; httpRequest.ContentLength = data.Length


&nbsp;&nbsp;&nbsp; Try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using dataStream = httpRequest.GetRequestStream()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; dataStream.Write(data, 0, data.Length)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp; Catch we As WebException
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; StrMessage = we.Message
&nbsp;&nbsp;&nbsp; End Try
End Sub




''' &lt;summary&gt;
''' Get the response from WCF Service
''' &lt;/summary&gt;
''' &lt;param name=&quot;template&quot;&gt;Object template like: User&lt;/param&gt;
''' &lt;param name=&quot;action&quot;&gt;Action like: Delete&lt;/param&gt;
''' &lt;param name=&quot;method&quot;&gt;Request method&lt;/param&gt;
''' &lt;returns&gt;Return the result from WCF Service&lt;/returns&gt;
Private Function GetResponse(ByVal template As String,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal action As String,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal method As HttpMethod) As String
&nbsp;&nbsp;&nbsp; Dim responseData As String = String.Empty


&nbsp;&nbsp;&nbsp; httpRequest = DirectCast(WebRequest.Create(String.Format(basicUrl, template, action)), HttpWebRequest)
&nbsp;&nbsp;&nbsp; httpRequest.Method = method.ToString()


&nbsp;&nbsp;&nbsp; Try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using httpResponse = TryCast(httpRequest.GetResponse(), HttpWebResponse)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; dataStream = httpResponse.GetResponseStream()


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using streamReader = New StreamReader(dataStream)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; responseData = streamReader.ReadToEnd()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp; Catch we As WebException
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; StrMessage = we.Message
&nbsp;&nbsp;&nbsp; Catch pve As ProtocolViolationException
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; StrMessage = pve.Message
&nbsp;&nbsp;&nbsp; End Try


&nbsp;&nbsp;&nbsp; Return responseData
End Function

</pre>
<pre id="codePreview" class="vb">
''' &lt;summary&gt;
''' Send the request to WCF Service
''' &lt;/summary&gt;
''' &lt;param name=&quot;template&quot;&gt;Object template like: User&lt;/param&gt;
''' &lt;param name=&quot;action&quot;&gt;Action like: Delete&lt;/param&gt;
''' &lt;param name=&quot;method&quot;&gt;Request method&lt;/param&gt;
''' &lt;param name=&quot;t&quot;&gt;Object like: User&lt;/param&gt;
Private Sub SendRequest(ByVal template As String,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal action As String,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal method As HttpMethod,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal t As T)
&nbsp;&nbsp;&nbsp; Dim jsonData As String = JsonHelp.JsonSerialize(Of T)(t)
&nbsp;&nbsp;&nbsp; If String.IsNullOrEmpty(jsonData) Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Return
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; Dim data As Byte() = UnicodeEncoding.UTF8.GetBytes(jsonData)


&nbsp;&nbsp;&nbsp; httpRequest = DirectCast(WebRequest.Create(String.Format(basicUrl, template, action)), HttpWebRequest)
&nbsp;&nbsp;&nbsp; httpRequest.Method = method.ToString()
&nbsp;&nbsp;&nbsp; httpRequest.ContentType = &quot;application/json&quot;
&nbsp;&nbsp;&nbsp; httpRequest.ContentLength = data.Length


&nbsp;&nbsp;&nbsp; Try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using dataStream = httpRequest.GetRequestStream()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; dataStream.Write(data, 0, data.Length)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp; Catch we As WebException
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; StrMessage = we.Message
&nbsp;&nbsp;&nbsp; End Try
End Sub




''' &lt;summary&gt;
''' Get the response from WCF Service
''' &lt;/summary&gt;
''' &lt;param name=&quot;template&quot;&gt;Object template like: User&lt;/param&gt;
''' &lt;param name=&quot;action&quot;&gt;Action like: Delete&lt;/param&gt;
''' &lt;param name=&quot;method&quot;&gt;Request method&lt;/param&gt;
''' &lt;returns&gt;Return the result from WCF Service&lt;/returns&gt;
Private Function GetResponse(ByVal template As String,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal action As String,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ByVal method As HttpMethod) As String
&nbsp;&nbsp;&nbsp; Dim responseData As String = String.Empty


&nbsp;&nbsp;&nbsp; httpRequest = DirectCast(WebRequest.Create(String.Format(basicUrl, template, action)), HttpWebRequest)
&nbsp;&nbsp;&nbsp; httpRequest.Method = method.ToString()


&nbsp;&nbsp;&nbsp; Try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using httpResponse = TryCast(httpRequest.GetResponse(), HttpWebResponse)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; dataStream = httpResponse.GetResponseStream()


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using streamReader = New StreamReader(dataStream)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; responseData = streamReader.ReadToEnd()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp; Catch we As WebException
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; StrMessage = we.Message
&nbsp;&nbsp;&nbsp; Catch pve As ProtocolViolationException
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; StrMessage = pve.Message
&nbsp;&nbsp;&nbsp; End Try


&nbsp;&nbsp;&nbsp; Return responseData
End Function

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<h2>More Information</h2>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
