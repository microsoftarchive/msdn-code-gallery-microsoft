# How to delete a row from Windows Azure Table storage without retrieving it first
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Azure
## Topics
- Azure
## Updated
- 05/05/2014
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to reduce the</span><span style="font-weight:bold; font-size:14pt"> times of</span><span style="font-weight:bold; font-size:14pt"> requests
</span><span style="font-weight:bold; font-size:14pt">sent </span><span style="font-weight:bold; font-size:14pt">to Azure Storage Service</span><span style="font-weight:bold; font-size:14pt"> in Windows Store apps</span><span style="font-weight:bold; font-size:14pt">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">When </span><span style="font-size:11pt">you
</span><span style="font-size:11pt">develop an app </span><span style="font-size:11pt">that
</span><span style="font-size:11pt">need</span><span style="font-size:11pt">s</span><span style="font-size:11pt"> connect</span><span style="font-size:11pt">ing</span><span style="font-size:11pt"> to Azure storage service</span><span style="font-size:11pt">,
 it </span><span style="font-size:11pt">will take a long time</span><span style="font-size:11pt"> to</span><span style="font-size:11pt"> s</span><span style="font-size:11pt">end request</span><span style="font-size:11pt">s</span><span style="font-size:11pt">
 and get response</span><span style="font-size:11pt">s</span><span style="font-size:11pt"> in the client application.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">This sample </span><span>demonstrate</span><span>s</span><span> how to reduce the
</span><span>times of </span><span>connection between client app and Azure storage service. It also shows how to handle general exception when</span><span> you</span><span> get the response.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Building the Sample</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:5pt; margin-bottom:5pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="color:#000000; font-size:11pt">&bull;&nbsp;</span><span style="color:#000000; font-size:11pt">Create a new</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="font-weight:bold; color:#000000; font-size:11pt">Storage
 Account</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="color:#000000; font-size:11pt">from the Windows Azure Management Portal.</span></span></p>
<p style="margin-left:30pt; margin-right:0pt; margin-top:5pt; margin-bottom:5pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:12pt"><span style="color:#000000; font-size:10pt">&nbsp;</span><span style="color:#000000; font-size:11pt">To do this, follow the instructions in</span><span style="color:#000000; font-size:11pt">&nbsp;</span><a href="http://www.windowsazure.com/en-us/manage/services/storage/how-to-create-a-storage-account/" style="text-decoration:none"><span style="color:#960bb4; font-size:11pt; text-decoration:underline">How
 To Create a Storage Account</span></a><span style="color:#000000; font-size:11pt">.</span></span></p>
<p style="margin-left:30pt; margin-right:0pt; margin-top:5pt; margin-bottom:5pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:12pt"><span style="color:#000000; font-size:10pt">&nbsp;</span><span style="color:#000000; font-size:11pt">Get the</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="font-weight:bold; color:#000000; font-size:11pt">Storage
 Account Keys</span><span style="color:#000000; font-size:11pt">. Browse to your storage account dashboard and click</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="font-weight:bold; color:#000000; font-size:11pt">Manage Access Keys</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="color:#000000; font-size:11pt">on
 the bottom bar.</span></span></p>
<p style="margin-left:30pt; margin-right:0pt; margin-top:5pt; margin-bottom:5pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:12pt"><span style="color:#000000; font-size:10pt">&nbsp;</span><span style="font-size:12pt"><img src="114042-image.png" alt="" width="201" height="57" align="middle">
</span></span></p>
<p style="margin-left:30pt; margin-right:0pt; margin-top:5pt; margin-bottom:5pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:12pt"><span style="color:#000000; font-size:11pt">Copy the</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="font-weight:bold; color:#000000; font-size:11pt">Storage Account Name</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="color:#000000; font-size:11pt">and</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="font-weight:bold; color:#000000; font-size:11pt">Primary
 Access Key</span><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="color:#000000; font-size:11pt">values.</span></span></p>
<p style="margin-left:30pt; margin-right:0pt; margin-top:5pt; margin-bottom:5pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:12pt"><span style="font-size:12pt"><img src="114043-image.png" alt="" width="546" height="459" align="middle">
</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp;<span style="font-size:11pt">Open this project, go to the
</span><span style="font-weight:bold">Solution-&gt;Project-&gt;DataSource-&gt;TableDataSource.cs
</span><span style="font-size:11pt">file</span><span>,</span><span style="font-size:11pt"> replace account name and account key you copied in the step 1.</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp;<span style="font-size:11pt">Build the project, download the class library from nugget.</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp;<span style="font-size:11pt">Press </span>
<span style="font-weight:bold">F5</span><span style="font-size:11pt"> </span><span style="font-size:11pt">to
</span><span style="font-size:11pt">start the app.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">This app only ha</span><span style="font-size:11pt">s one page view:</span><span style="font-size:11pt"> main page view.</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="114044-image.png" alt="" width="185" height="458" align="middle">
</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">You can Click</span><span style="font-size:11pt"> the</span><span style="font-size:11pt">
</span><span style="font-weight:bold">Delete</span><span style="font-size:11pt"> button first, then use
</span><span style="font-size:11pt">the </span><span style="font-weight:bold">Regenerate the Sample data
</span><span style="font-size:11pt">button</span><span style="font-size:11pt"> to</span><span style="font-size:11pt"> create the sample data.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">The code sample provide</span><span style="font-size:11pt">s</span><span style="font-size:11pt"> the following reusable functions to handle Azure storage transient fault error.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:26.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt; font-weight:bold"><span style="font-size:11pt; font-weight:bold">How to handle conflict error?</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">public class ConflictRetryPolicy:IRetryPolicy 
   { 
       int maxRetryAttemps = 10; 
       TimeSpan defaultRetryInterval = TimeSpan.FromSeconds(5); 
 
       public ConflictRetryPolicy(TimeSpan deltaBackoff, int retryAttempts) 
       { 
           maxRetryAttemps = retryAttempts; 
           defaultRetryInterval = deltaBackoff; 
       } 
 
       public IRetryPolicy CreateInstance() 
       { 
           return new ConflictRetryPolicy(TimeSpan.FromSeconds(5), 10); 
       } 
 
       public bool ShouldRetry(int currentRetryCount, int statusCode, Exception lastException, out TimeSpan retryInterval, OperationContext operationContext) 
       { 
           retryInterval = defaultRetryInterval; 
           if (currentRetryCount &gt;= maxRetryAttemps) 
           { 
               return false; 
           } 
           if (statusCode == 409) 
           { 
               return true; 
           } 
           else 
           { 
               return false; 
           } 
       } 
   } 
</pre>
<pre class="hidden">Public Class ConflictRetryPolicy
    Implements IRetryPolicy
    Private maxRetryAttemps As Integer = 10
    Private defaultRetryInterval As TimeSpan = TimeSpan.FromSeconds(5)
    Public Sub New(deltaBackoff As TimeSpan, retryAttempts As Integer)
        maxRetryAttemps = retryAttempts
        defaultRetryInterval = deltaBackoff
    End Sub
    Public Function CreateInstance() As IRetryPolicy Implements IRetryPolicy.CreateInstance
        Return New ConflictRetryPolicy(TimeSpan.FromSeconds(5), 10)
    End Function
    Public Function ShouldRetry(currentRetryCount As Integer, statusCode As Integer, lastException As Exception, ByRef retryInterval As TimeSpan, operationContext As Microsoft.WindowsAzure.Storage.OperationContext) As Boolean Implements IRetryPolicy.ShouldRetry
        retryInterval = defaultRetryInterval
        If currentRetryCount &gt;= maxRetryAttemps Then
            Return False
        End If
        If statusCode = 409 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
</pre>
<pre id="codePreview" class="csharp">public class ConflictRetryPolicy:IRetryPolicy 
   { 
       int maxRetryAttemps = 10; 
       TimeSpan defaultRetryInterval = TimeSpan.FromSeconds(5); 
 
       public ConflictRetryPolicy(TimeSpan deltaBackoff, int retryAttempts) 
       { 
           maxRetryAttemps = retryAttempts; 
           defaultRetryInterval = deltaBackoff; 
       } 
 
       public IRetryPolicy CreateInstance() 
       { 
           return new ConflictRetryPolicy(TimeSpan.FromSeconds(5), 10); 
       } 
 
       public bool ShouldRetry(int currentRetryCount, int statusCode, Exception lastException, out TimeSpan retryInterval, OperationContext operationContext) 
       { 
           retryInterval = defaultRetryInterval; 
           if (currentRetryCount &gt;= maxRetryAttemps) 
           { 
               return false; 
           } 
           if (statusCode == 409) 
           { 
               return true; 
           } 
           else 
           { 
               return false; 
           } 
       } 
   } 
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:26.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt; font-weight:bold"><span style="font-size:11pt; font-weight:bold">How to reduce request
</span><span style="font-size:11pt; font-weight:bold">sending</span><span style="font-size:11pt; font-weight:bold">
</span><span style="font-size:11pt; font-weight:bold">time</span><span style="font-size:11pt; font-weight:bold">s</span><span style="font-size:11pt; font-weight:bold">?</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">public static async Task&lt;bool&gt; DeleteEntity(DynamicTableEntity entity) 
       { 
           try 
           { 
               var table = client.GetTableReference(tableName); 
 
               TableOperation deleteOperation = TableOperation.Delete(entity); 
               var result = await table.ExecuteAsync(deleteOperation); 
 
               return true; 
           } 
           catch (Exception e) 
           { 
               //In windows store apps, StorageException is an internal class. 
               //You can't convert Exception to StorageException, so you should use  
               //RequestResult.TranslateFromExceptionMessage(e.Message) to get the HttpStatusCode. 
               var result = RequestResult.TranslateFromExceptionMessage(e.Message); 
 
               //We treat 404 as it has already been deleted. 
               if (result.HttpStatusCode == 404) 
               { 
                   return true; 
               } 
               else 
               { 
                   return false; 
                   //throw new WebException(result.HttpStatusMessage); 
               } 
           } 
 
       } 
</pre>
<pre class="hidden">Public Shared Async Function DeleteEntity(entity As DynamicTableEntity) As Task(Of Boolean)
     Try
         Dim table = client.GetTableReference(tableName)
         Dim deleteOperation As TableOperation = TableOperation.Delete(entity)
         Dim result = Await table.ExecuteAsync(deleteOperation)
         Return True
     Catch e As Exception
         ' In windows store app, StorageException is an internal class.
         ' You can't convert Exception to StorageException, so you should use 
         ' RequestResult.TranslateFromExceptionMessage(e.Message) get the HttpStatusCode.
         Dim result = RequestResult.TranslateFromExceptionMessage(e.Message)
         ' We treat 404 as it has already been deleted.
         If result.HttpStatusCode = 404 Then
             Return True
         Else
             ' Throw new WebException(result.HttpStatusMessage);
             Return False
         End If
     End Try
 End Function
</pre>
<pre id="codePreview" class="csharp">public static async Task&lt;bool&gt; DeleteEntity(DynamicTableEntity entity) 
       { 
           try 
           { 
               var table = client.GetTableReference(tableName); 
 
               TableOperation deleteOperation = TableOperation.Delete(entity); 
               var result = await table.ExecuteAsync(deleteOperation); 
 
               return true; 
           } 
           catch (Exception e) 
           { 
               //In windows store apps, StorageException is an internal class. 
               //You can't convert Exception to StorageException, so you should use  
               //RequestResult.TranslateFromExceptionMessage(e.Message) to get the HttpStatusCode. 
               var result = RequestResult.TranslateFromExceptionMessage(e.Message); 
 
               //We treat 404 as it has already been deleted. 
               if (result.HttpStatusCode == 404) 
               { 
                   return true; 
               } 
               else 
               { 
                   return false; 
                   //throw new WebException(result.HttpStatusMessage); 
               } 
           } 
 
       } 
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://www.windowsazure.com/en-us/documentation/articles/storage-dotnet-how-to-use-table-storage-20/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Azure table storage feature guide</span></a><span style="font-size:11pt">
 shows </span><span style="font-size:11pt">that it is not necessary to retrieve an entity before deletion
</span><span style="font-size:11pt">if you </span><span style="font-size:11pt">have</span><span style="font-size:11pt">
</span><span style="font-size:11pt">already retrieve</span><span style="font-size:11pt">d</span><span style="font-size:11pt"> it before. So we can delete that retrieve operation in code.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">If you send the same entity delete operation twice, you should catch the 404 error and ignore it as
</span><span style="font-size:11pt">the </span><span style="font-size:11pt">code above</span><span style="font-size:11pt"> does</span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">More Information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://www.windowsazure.com/en-us/documentation/articles/storage-dotnet-how-to-use-table-storage-20/#delete-entity" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://www.windowsazure.com/en-us/documentation/articles/storage-dotnet-how-to-use-table-storage-20/#delete-entity</span></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
