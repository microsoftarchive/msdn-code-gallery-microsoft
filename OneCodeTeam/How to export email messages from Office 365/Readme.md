# How to export email messages from Office 365
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Exchange Online
- Office 365
## Topics
- Office 365
- export email
## Updated
- 09/21/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"></a><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong></strong><em></em>
</div>
<h1>How to Export Email Messages from Office 365 Exchange Online (CSOffice365ExportEmail)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">In this sample, we will demonstrate how to export the emails form the office 365.</p>
<p class="MsoNormal">If we export the emails, we can read them offline. We can follow these steps to implement it:</p>
<p class="MsoNormal">1. Create a search directory to collect the emails;</p>
<p class="MsoNormal">2. Get the directory;</p>
<p class="MsoNormal">3. Export the emails;</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to run the sample, the following is the result.</p>
<p class="MsoNormal"><span><img src="113514-image.png" alt="" width="636" height="173" align="middle">
</span></p>
<p class="MsoNormal">First, we use our account to connect the Exchange Online.</p>
<p class="MsoNormal">Before we create the search folder, we need to set the filters. In this sample, we set the filter basing the Subject of email. So we input the string that the email's subject contains.</p>
<p class="MsoNormal">And then we use the filter to create the search folder.</p>
<p class="MsoNormal"><span><img src="113515-image.png" alt="" width="629" height="100" align="middle">
</span><span>&nbsp;</span></p>
<p class="MsoNormal">We get the search folder basing the DisplayName of the folder.</p>
<p class="MsoNormal">At last we export all the emails to the <span class="GramE">
' .</span><span class="SpellE">eml</span>' files. In this sample, we will store the files in the application directory.</p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Create the search folder.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
a. set the filters</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
In this sample, we only search the nearest 30 days emails and set the other filters, such as the Subject.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">SearchFilter.SearchFilterCollection filterCollection = 
&nbsp;&nbsp;&nbsp;&nbsp;new SearchFilter.SearchFilterCollection(LogicalOperator.And);


DateTime startDate = DateTime.Now.AddDays(-30);
DateTime endDate = DateTime.Now;
SearchFilter startDateFilter = 
&nbsp;&nbsp;&nbsp;&nbsp;new SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.DateTimeCreated, startDate);
SearchFilter endDateFilter = 
&nbsp;&nbsp;&nbsp;&nbsp;new SearchFilter.IsLessThanOrEqualTo(EmailMessageSchema.DateTimeCreated, endDate);
filterCollection.Add(startDateFilter);
filterCollection.Add(endDateFilter);


SearchFilter itemClassFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.ItemClass, &quot;IPM.Note&quot;);
filterCollection.Add(itemClassFilter);


// Set the other filters.
if (filters != null)
{
&nbsp;&nbsp;&nbsp; foreach (PropertyDefinition property in filters.Keys)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SearchFilter searchFilter = new SearchFilter.ContainsSubstring(property, filters[property]);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; filterCollection.Add(searchFilter);
&nbsp;&nbsp;&nbsp; }
}

</pre>
<pre class="csharp" id="codePreview">SearchFilter.SearchFilterCollection filterCollection = 
&nbsp;&nbsp;&nbsp;&nbsp;new SearchFilter.SearchFilterCollection(LogicalOperator.And);


DateTime startDate = DateTime.Now.AddDays(-30);
DateTime endDate = DateTime.Now;
SearchFilter startDateFilter = 
&nbsp;&nbsp;&nbsp;&nbsp;new SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.DateTimeCreated, startDate);
SearchFilter endDateFilter = 
&nbsp;&nbsp;&nbsp;&nbsp;new SearchFilter.IsLessThanOrEqualTo(EmailMessageSchema.DateTimeCreated, endDate);
filterCollection.Add(startDateFilter);
filterCollection.Add(endDateFilter);


SearchFilter itemClassFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.ItemClass, &quot;IPM.Note&quot;);
filterCollection.Add(itemClassFilter);


// Set the other filters.
if (filters != null)
{
&nbsp;&nbsp;&nbsp; foreach (PropertyDefinition property in filters.Keys)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SearchFilter searchFilter = new SearchFilter.ContainsSubstring(property, filters[property]);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; filterCollection.Add(searchFilter);
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">b. Create the search folder</p>
<p class="MsoNormal">We first check if there's the duplicate folder. If there's no the duplicate folder, we will create a new search folder and save it; or we will update the search folder.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">Boolean isDuplicateFoler = true;
SearchFilter duplicateFilter=new SearchFilter.IsEqualTo(FolderSchema.DisplayName,displayName);
SearchFolder searchFolder = 
&nbsp;&nbsp;&nbsp;&nbsp;GetFolder(service, duplicateFilter, WellKnownFolderName.SearchFolders) as SearchFolder;


if (searchFolder == null)
{
&nbsp;&nbsp;&nbsp; searchFolder = new SearchFolder(service);
&nbsp;&nbsp;&nbsp; isDuplicateFoler = false;
}
searchFolder.SearchParameters.RootFolderIds.Add(folderId);
searchFolder.SearchParameters.Traversal = SearchFolderTraversal.Shallow;
searchFolder.SearchParameters.SearchFilter = filterCollection;


if (isDuplicateFoler)
{
&nbsp;&nbsp;&nbsp; searchFolder.Update();
}
else
{
&nbsp;&nbsp;&nbsp; searchFolder.DisplayName = displayName;


&nbsp;&nbsp;&nbsp; searchFolder.Save(WellKnownFolderName.SearchFolders);
}

</pre>
<pre class="csharp" id="codePreview">Boolean isDuplicateFoler = true;
SearchFilter duplicateFilter=new SearchFilter.IsEqualTo(FolderSchema.DisplayName,displayName);
SearchFolder searchFolder = 
&nbsp;&nbsp;&nbsp;&nbsp;GetFolder(service, duplicateFilter, WellKnownFolderName.SearchFolders) as SearchFolder;


if (searchFolder == null)
{
&nbsp;&nbsp;&nbsp; searchFolder = new SearchFolder(service);
&nbsp;&nbsp;&nbsp; isDuplicateFoler = false;
}
searchFolder.SearchParameters.RootFolderIds.Add(folderId);
searchFolder.SearchParameters.Traversal = SearchFolderTraversal.Shallow;
searchFolder.SearchParameters.SearchFilter = filterCollection;


if (isDuplicateFoler)
{
&nbsp;&nbsp;&nbsp; searchFolder.Update();
}
else
{
&nbsp;&nbsp;&nbsp; searchFolder.DisplayName = displayName;


&nbsp;&nbsp;&nbsp; searchFolder.Save(WellKnownFolderName.SearchFolders);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">2. Export the emails</p>
<p class="MsoNormal">a. get the specific search folder</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">SearchFilter filter= new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName);
SearchFolder searchFolder = GetFolder(service, filter, 
&nbsp;&nbsp;&nbsp;&nbsp;WellKnownFolderName.SearchFolders) as SearchFolder;

</pre>
<pre class="csharp" id="codePreview">SearchFilter filter= new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName);
SearchFolder searchFolder = GetFolder(service, filter, 
&nbsp;&nbsp;&nbsp;&nbsp;WellKnownFolderName.SearchFolders) as SearchFolder;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">b. export the emails</p>
<p class="MsoNormal">We find the results in the search folder and export all the email messages.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">String[] invalidStings = { &quot;\\&quot;,&quot;,&quot;,&quot;:&quot;,&quot;*&quot;,&quot;?&quot;,&quot;\&quot;&quot;,&quot;&lt;&quot;,&quot;&gt;&quot;,&quot;|&quot;};


PropertySet itemPorpertySet = new PropertySet(BasePropertySet.FirstClassProperties, 
&nbsp;&nbsp;&nbsp;&nbsp;EmailMessageSchema.MimeContent);


const Int32 pageSize = 50;
ItemView itemView = new ItemView(pageSize);


FindItemsResults&lt;Item&gt; findResults=null;
do
{
&nbsp;&nbsp;&nbsp; findResults = searchFolder.FindItems(itemView);


&nbsp;&nbsp;&nbsp; foreach (Item item in findResults.Items)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (item is EmailMessage)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; EmailMessage email = item as EmailMessage;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; email.Load(itemPorpertySet);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Byte[] content=email.MimeContent.Content;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; String fileName = email.Subject;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Replace all the invaild strings.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (String str in invalidStings)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileName = fileName.Replace(str,&quot;&quot;);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Export the emails to the .eml files.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileName = Path.Combine(filePath, fileName &#43; &quot;.eml&quot;);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; File.WriteAllBytes(fileName, content);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;Export the email:{0}&quot;,email.Subject);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; itemView.Offset &#43;= pageSize;
} while (findResults.MoreAvailable);

</pre>
<pre class="csharp" id="codePreview">String[] invalidStings = { &quot;\\&quot;,&quot;,&quot;,&quot;:&quot;,&quot;*&quot;,&quot;?&quot;,&quot;\&quot;&quot;,&quot;&lt;&quot;,&quot;&gt;&quot;,&quot;|&quot;};


PropertySet itemPorpertySet = new PropertySet(BasePropertySet.FirstClassProperties, 
&nbsp;&nbsp;&nbsp;&nbsp;EmailMessageSchema.MimeContent);


const Int32 pageSize = 50;
ItemView itemView = new ItemView(pageSize);


FindItemsResults&lt;Item&gt; findResults=null;
do
{
&nbsp;&nbsp;&nbsp; findResults = searchFolder.FindItems(itemView);


&nbsp;&nbsp;&nbsp; foreach (Item item in findResults.Items)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (item is EmailMessage)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; EmailMessage email = item as EmailMessage;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; email.Load(itemPorpertySet);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Byte[] content=email.MimeContent.Content;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; String fileName = email.Subject;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Replace all the invaild strings.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (String str in invalidStings)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileName = fileName.Replace(str,&quot;&quot;);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Export the emails to the .eml files.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileName = Path.Combine(filePath, fileName &#43; &quot;.eml&quot;);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; File.WriteAllBytes(fileName, content);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;Export the email:{0}&quot;,email.Subject);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; itemView.Offset &#43;= pageSize;
} while (findResults.MoreAvailable);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/dd633709(v=exchg.80).aspx">EWS Managed API 2.0</a></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/dd636112(v=exchg.80).aspx"><span class="SpellE">SearchFolder</span> Class</a><span class="MsoHyperlink"><span style="color:windowtext; text-decoration:none">
</span></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
