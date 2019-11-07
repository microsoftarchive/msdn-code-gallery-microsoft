# How to add birthday or anniversary reminder in Office 365
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Exchange Online
- Office 365
## Topics
- Officer 365
- birthday reminder
## Updated
- 09/21/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"></a><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em></em>
</div>
<h1>How to Add Birthday or Anniversary Reminder for the Contacts in Office 365 Exchange Online (CSOffice365SetReminder)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">In this sample, we will demonstrate how to add birthday or anniversary reminder for the contacts.</p>
<p class="MsoNormal">We can search a contact folder to find the contacts that have birthdays or anniversaries, and then create the reminders for them. Additionally, we can import a comma-separated values (CSV) file for creating the reminders.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to run the sample, the following is the result.</p>
<p class="MsoNormal"><span><img src="113485-image.png" alt="" width="394" height="140" align="middle">
</span></p>
<p class="MsoNormal">First, we use our account to connect the Exchange Online.</p>
<p class="MsoNormal"><span><img src="113486-image.png" alt="" width="560" height="120" align="middle">
</span><span>&nbsp;</span></p>
<p class="MsoNormal">Then we get the contacts that have the Birthday or WeddingAnniversary properties.</p>
<p class="MsoNormal">We will create the appointments for the contacts.</p>
<p class="MsoNormal"><span><img src="113487-image.png" alt="" width="520" height="79" align="middle">
</span></p>
<p class="MsoNormal">We can also import a CSV file that contains the contacts.</p>
<p class="MsoNormal">After import the file, we create the appointments for the contacts that have the Birthday or WeddingAnniversary properties.</p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Create the reminders for the contacts folder.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
First, we get the folder basing the folder path.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">Folder contactsFolder = null;


// Searching the folder starts from the Contacts folder.
SearchFilter.RelationalFilter searchFilter =
new SearchFilter.IsEqualTo(FolderSchema.DisplayName, &quot;Contacts&quot;);
contactsFolder = GetFolder(service, searchFilter);


path = path.TrimStart('\\').TrimEnd('\\');


if (String.IsNullOrWhiteSpace(path))
{
&nbsp;&nbsp;&nbsp; return contactsFolder;
}
else
{
&nbsp;&nbsp;&nbsp; String[] pathList = path.Split('\\');
&nbsp;&nbsp;&nbsp; const Int32 pageSize = 10;


&nbsp;&nbsp;&nbsp; foreach (String name in pathList)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; searchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, name);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; FolderView folderView = new FolderView(pageSize);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;folderView.PropertySet = propertySet;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; FindFoldersResults folderResults = null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; do
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; folderResults = contactsFolder.FindFolders(searchFilter, folderView);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; folderView.Offset &#43;= pageSize;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // If the folder we find is the part of the parth, we will set the folder
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // as parent folder and search the next node in it.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (folderResults.TotalCount == 1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contactsFolder = folderResults.Folders[0];
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; } while (folderResults.MoreAvailable);
&nbsp;&nbsp;&nbsp; }
}

</pre>
<pre class="csharp" id="codePreview">Folder contactsFolder = null;


// Searching the folder starts from the Contacts folder.
SearchFilter.RelationalFilter searchFilter =
new SearchFilter.IsEqualTo(FolderSchema.DisplayName, &quot;Contacts&quot;);
contactsFolder = GetFolder(service, searchFilter);


path = path.TrimStart('\\').TrimEnd('\\');


if (String.IsNullOrWhiteSpace(path))
{
&nbsp;&nbsp;&nbsp; return contactsFolder;
}
else
{
&nbsp;&nbsp;&nbsp; String[] pathList = path.Split('\\');
&nbsp;&nbsp;&nbsp; const Int32 pageSize = 10;


&nbsp;&nbsp;&nbsp; foreach (String name in pathList)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; searchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, name);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; FolderView folderView = new FolderView(pageSize);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;folderView.PropertySet = propertySet;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; FindFoldersResults folderResults = null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; do
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; folderResults = contactsFolder.FindFolders(searchFilter, folderView);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; folderView.Offset &#43;= pageSize;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // If the folder we find is the part of the parth, we will set the folder
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // as parent folder and search the next node in it.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (folderResults.TotalCount == 1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contactsFolder = folderResults.Folders[0];
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; } while (folderResults.MoreAvailable);
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Second, we get the contacts that have the have the Birthday or WeddingAnniversary properties from the folder.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">List&lt;Contact&gt; contacts = new List&lt;Contact&gt;();


SearchFilter.SearchFilterCollection filters = 
&nbsp;&nbsp;&nbsp;&nbsp;new SearchFilter.SearchFilterCollection(LogicalOperator.And);


if (!String.IsNullOrWhiteSpace(name))
{
&nbsp;&nbsp;&nbsp; SearchFilter searchFilter = new SearchFilter.ContainsSubstring(ContactSchema.DisplayName, name);
&nbsp;&nbsp;&nbsp; filters.Add(searchFilter);
}


if (schemas != null)
{
&nbsp;&nbsp;&nbsp; foreach (PropertyDefinition schema in schemas)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SearchFilter searchFilter = new SearchFilter.Exists(schema);
&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;filters.Add(searchFilter);
&nbsp;&nbsp;&nbsp; }
}


const Int32 pageSize = 10;
ItemView itemView = new ItemView(pageSize);
PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly, schemas);
propertySet.Add(ContactSchema.DisplayName);
itemView.PropertySet = propertySet;


FindItemsResults&lt;Item&gt; findResults = null;
do
{
&nbsp;&nbsp;&nbsp; findResults = contactsFolder.FindItems(filters, itemView);
&nbsp;&nbsp;&nbsp; itemView.Offset &#43;= pageSize;


&nbsp;&nbsp;&nbsp; contacts.AddRange(findResults.Cast&lt;Contact&gt;());
} while (findResults.MoreAvailable);

</pre>
<pre class="csharp" id="codePreview">List&lt;Contact&gt; contacts = new List&lt;Contact&gt;();


SearchFilter.SearchFilterCollection filters = 
&nbsp;&nbsp;&nbsp;&nbsp;new SearchFilter.SearchFilterCollection(LogicalOperator.And);


if (!String.IsNullOrWhiteSpace(name))
{
&nbsp;&nbsp;&nbsp; SearchFilter searchFilter = new SearchFilter.ContainsSubstring(ContactSchema.DisplayName, name);
&nbsp;&nbsp;&nbsp; filters.Add(searchFilter);
}


if (schemas != null)
{
&nbsp;&nbsp;&nbsp; foreach (PropertyDefinition schema in schemas)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SearchFilter searchFilter = new SearchFilter.Exists(schema);
&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;filters.Add(searchFilter);
&nbsp;&nbsp;&nbsp; }
}


const Int32 pageSize = 10;
ItemView itemView = new ItemView(pageSize);
PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly, schemas);
propertySet.Add(ContactSchema.DisplayName);
itemView.PropertySet = propertySet;


FindItemsResults&lt;Item&gt; findResults = null;
do
{
&nbsp;&nbsp;&nbsp; findResults = contactsFolder.FindItems(filters, itemView);
&nbsp;&nbsp;&nbsp; itemView.Offset &#43;= pageSize;


&nbsp;&nbsp;&nbsp; contacts.AddRange(findResults.Cast&lt;Contact&gt;());
} while (findResults.MoreAvailable);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">At last, we create the appointments for the contacts.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">DateTime? date = getDate(contact);


if (date == null)
{
&nbsp;&nbsp;&nbsp; return false;
}


String appointmentSubject = subject &#43; &quot; &quot; &#43; contact.DisplayName;
// Check if there's the duplicate appointment.
if (HaveDuplicateAppointment(service, appointmentSubject))
{
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;There's a duplicate appointment of &quot; &#43; contact.DisplayName);
&nbsp;&nbsp;&nbsp; return false;
}


Appointment appointment = new Appointment(service);
appointment.Subject = appointmentSubject;
appointment.LegacyFreeBusyStatus = LegacyFreeBusyStatus.Free;
appointment.IsAllDayEvent = true;


Recurrence recurrence = new Recurrence.YearlyPattern(date.Value, 
&nbsp;&nbsp;&nbsp;&nbsp;(Month)date.Value.Month, date.Value.Day);
appointment.Recurrence = recurrence;


appointment.Save(SendInvitationsMode.SendToNone);

</pre>
<pre class="csharp" id="codePreview">DateTime? date = getDate(contact);


if (date == null)
{
&nbsp;&nbsp;&nbsp; return false;
}


String appointmentSubject = subject &#43; &quot; &quot; &#43; contact.DisplayName;
// Check if there's the duplicate appointment.
if (HaveDuplicateAppointment(service, appointmentSubject))
{
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;There's a duplicate appointment of &quot; &#43; contact.DisplayName);
&nbsp;&nbsp;&nbsp; return false;
}


Appointment appointment = new Appointment(service);
appointment.Subject = appointmentSubject;
appointment.LegacyFreeBusyStatus = LegacyFreeBusyStatus.Free;
appointment.IsAllDayEvent = true;


Recurrence recurrence = new Recurrence.YearlyPattern(date.Value, 
&nbsp;&nbsp;&nbsp;&nbsp;(Month)date.Value.Month, date.Value.Day);
appointment.Recurrence = recurrence;


appointment.Save(SendInvitationsMode.SendToNone);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">2. Create the reminders for the CSV file.</p>
<p class="MsoNormal">First, we need to import the CSV file that contains the contacts.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">// Get the DataTable that contains the value of the contacts.
DataTable contactsTable = new DataTable();
ImportCSVFile(contactsTable, filepath);


var properties = new { 
&nbsp;&nbsp;&nbsp;&nbsp;FirstName = &quot;First Name&quot;, 
&nbsp;&nbsp;&nbsp;&nbsp;LastName = &quot;Last Name&quot;, 
&nbsp;&nbsp;&nbsp;&nbsp;Anniversary = &quot;Anniversary&quot;, 
&nbsp;&nbsp;&nbsp;&nbsp;Birthday = &quot;Birthday&quot; };


foreach (DataRow row in contactsTable.Rows)
{
&nbsp;&nbsp;&nbsp; Contact contact = new Contact(service);


&nbsp;&nbsp;&nbsp; if (contactsTable.Columns.Contains(properties.FirstName))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.GivenName = row[properties.FirstName].ToString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.DisplayName = contact.GivenName;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.FileAs = contact.DisplayName;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (contactsTable.Columns.Contains(properties.LastName))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.Surname = row[properties.LastName].ToString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!String.IsNullOrWhiteSpace(contact.GivenName))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.DisplayName = contact.GivenName &#43; &quot; &quot; &#43; contact.Surname;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.FileAs = contact.DisplayName;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; CultureInfo provider = new CultureInfo(&quot;en-US&quot;);
&nbsp;&nbsp;&nbsp; DateTime date;
&nbsp;&nbsp;&nbsp; if (contactsTable.Columns.Contains(properties.Anniversary))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.WeddingAnniversary = DateTime.TryParseExact(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; row[properties.Anniversary].ToString(), &quot;d&quot;, provider, DateTimeStyles.None, out date) ? 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(DateTime?)date : null;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (contactsTable.Columns.Contains(properties.Birthday))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.Birthday = DateTime.TryParseExact(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; row[properties.Birthday].ToString(), &quot;d&quot;, provider, DateTimeStyles.None, out date) ? 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(DateTime?)date : null;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; contact.Save();

</pre>
<pre class="csharp" id="codePreview">// Get the DataTable that contains the value of the contacts.
DataTable contactsTable = new DataTable();
ImportCSVFile(contactsTable, filepath);


var properties = new { 
&nbsp;&nbsp;&nbsp;&nbsp;FirstName = &quot;First Name&quot;, 
&nbsp;&nbsp;&nbsp;&nbsp;LastName = &quot;Last Name&quot;, 
&nbsp;&nbsp;&nbsp;&nbsp;Anniversary = &quot;Anniversary&quot;, 
&nbsp;&nbsp;&nbsp;&nbsp;Birthday = &quot;Birthday&quot; };


foreach (DataRow row in contactsTable.Rows)
{
&nbsp;&nbsp;&nbsp; Contact contact = new Contact(service);


&nbsp;&nbsp;&nbsp; if (contactsTable.Columns.Contains(properties.FirstName))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.GivenName = row[properties.FirstName].ToString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.DisplayName = contact.GivenName;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.FileAs = contact.DisplayName;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (contactsTable.Columns.Contains(properties.LastName))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.Surname = row[properties.LastName].ToString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!String.IsNullOrWhiteSpace(contact.GivenName))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.DisplayName = contact.GivenName &#43; &quot; &quot; &#43; contact.Surname;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.FileAs = contact.DisplayName;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; CultureInfo provider = new CultureInfo(&quot;en-US&quot;);
&nbsp;&nbsp;&nbsp; DateTime date;
&nbsp;&nbsp;&nbsp; if (contactsTable.Columns.Contains(properties.Anniversary))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.WeddingAnniversary = DateTime.TryParseExact(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; row[properties.Anniversary].ToString(), &quot;d&quot;, provider, DateTimeStyles.None, out date) ? 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(DateTime?)date : null;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (contactsTable.Columns.Contains(properties.Birthday))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contact.Birthday = DateTime.TryParseExact(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; row[properties.Birthday].ToString(), &quot;d&quot;, provider, DateTimeStyles.None, out date) ? 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(DateTime?)date : null;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; contact.Save();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Second, we store the contacts in the list and load all the properties that we will use when creating the appointments.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">contactsList.Add(contact);


PropertySet propertySet = new PropertySet(ContactSchema.DisplayName, 
&nbsp;&nbsp;&nbsp;&nbsp;ContactSchema.Birthday, ContactSchema.WeddingAnniversary);
contact.Load(propertySet);

</pre>
<pre class="csharp" id="codePreview">contactsList.Add(contact);


PropertySet propertySet = new PropertySet(ContactSchema.DisplayName, 
&nbsp;&nbsp;&nbsp;&nbsp;ContactSchema.Birthday, ContactSchema.WeddingAnniversary);
contact.Load(propertySet);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
At last, we creating the appointments just like the last step of &quot;Creating the reminders for the contacts folder&quot;.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
3. Check the duplicate appointments</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
Before we create the appointments for the contacts, we also need to check if there's the duplicate appointment basing the Subject.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">SearchFilter.RelationalFilter searchFilter =
new SearchFilter.IsEqualTo(FolderSchema.DisplayName, &quot;Calendar&quot;);
Folder calendar = GetFolder(service, searchFilter);


const Int32 pageSize = 10;
ItemView itemView = new ItemView(pageSize);
PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly);
itemView.PropertySet = propertySet;


searchFilter = new SearchFilter.IsEqualTo(AppointmentSchema.Subject, appointmentSubject);


FindItemsResults&lt;Item&gt; findResults = calendar.FindItems(searchFilter, itemView);


if (findResults.TotalCount &gt; 0)
{
&nbsp;&nbsp;&nbsp; return true;
}

</pre>
<pre class="csharp" id="codePreview">SearchFilter.RelationalFilter searchFilter =
new SearchFilter.IsEqualTo(FolderSchema.DisplayName, &quot;Calendar&quot;);
Folder calendar = GetFolder(service, searchFilter);


const Int32 pageSize = 10;
ItemView itemView = new ItemView(pageSize);
PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly);
itemView.PropertySet = propertySet;


searchFilter = new SearchFilter.IsEqualTo(AppointmentSchema.Subject, appointmentSubject);


FindItemsResults&lt;Item&gt; findResults = calendar.FindItems(searchFilter, itemView);


if (findResults.TotalCount &gt; 0)
{
&nbsp;&nbsp;&nbsp; return true;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/dd633709(v=exchg.80).aspx">EWS Managed API 2.0</a></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/dd634368(v=exchg.80).aspx"><span class="SpellE">PropertyDefinition</span> Class</a><span class="MsoHyperlink"><span style="color:windowtext; text-decoration:none">
</span></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
