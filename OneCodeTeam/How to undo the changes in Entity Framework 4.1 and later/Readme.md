# How to undo the changes in Entity Framework 4.1 and later
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
## Topics
- Undo Changes
- Entity Framwork
- ObjectContext
- DbContext
## Updated
- 03/22/2013
## Description

<h1>Undo the Changes in EF (CSEFUndoChanges)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This sample demonstrates how to undo the changes in Entity Framework.</p>
<p class="MsoNormal">When we make the changes to the entities, we can use the SaveChanges Method to update the entities in the database. But sometimes some of the changes are wrong, and we need to rollback these changes. In this sample, we demonstrate how
 to use ObjectContext and DbContext to undo the changes in different levels.</p>
<p class="MsoNormal">1. Context Level;</p>
<p class="MsoNormal">2. Entity Set Level;</p>
<p class="MsoNormal">3. Entity Level;</p>
<p class="MsoNormal">4. Property Level.</p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before you run the sample, you need to finish the following steps:</p>
<p class="MsoNormal">Step1. Attach the database file MySchool.mdf under the folder _External_Dependecies to your SQL Server 2008 database instance.</p>
<p class="MsoNormal">Step2. Modify the connection string in the App.config file according to your SQL Server 2008 database instance name.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to run the sample, the following is the result.</p>
<p class="MsoNormal"><span style=""><img src="78796-image.png" alt="" width="640" height="395" align="middle">
</span></p>
<p class="MsoNormal">It will show the results of undoing the changes in the different levels of DbContext and ObjectContext.</p>
<h2>Using the Code</h2>
<p class="MsoNormal"><b style="">1. Undo the changes of DbContext </b></p>
<p class="MsoNormal">a. Undo the Changes in the Context level</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
It undoes the changes of the all entries.<span style="font-size:9.5pt; font-family:Consolas; color:green">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
// Undo the changes of the all entries.
foreach (DbEntityEntry entry in context.ChangeTracker.Entries())
{
&nbsp;&nbsp;&nbsp; switch (entry.State)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Under the covers, changing the state of an entity from 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// Modified to Unchanged first sets the values of all 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// properties to the original values that were read from 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// the database when it was queried, and then marks the 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// entity as Unchanged. This will also reject changes to 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// FK relationships since the original value of the FK 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// will be restored.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Modified:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Unchanged;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Added:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Detached;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // If the EntityState is the Deleted, reload the date from the database.&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;case EntityState.Deleted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.Reload();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default: break;
&nbsp;&nbsp;&nbsp; }
}

</pre>
<pre id="codePreview" class="csharp">
// Undo the changes of the all entries.
foreach (DbEntityEntry entry in context.ChangeTracker.Entries())
{
&nbsp;&nbsp;&nbsp; switch (entry.State)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Under the covers, changing the state of an entity from 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// Modified to Unchanged first sets the values of all 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// properties to the original values that were read from 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// the database when it was queried, and then marks the 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// entity as Unchanged. This will also reject changes to 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// FK relationships since the original value of the FK 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// will be restored.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Modified:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Unchanged;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Added:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Detached;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // If the EntityState is the Deleted, reload the date from the database.&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;case EntityState.Deleted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.Reload();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default: break;
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">b. Undo the Changes in the DbEntities level</p>
<p class="MsoNormal">It undoes the changes of the T type entries.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
foreach (DbEntityEntry&lt;T&gt; entry in context.ChangeTracker.Entries&lt;T&gt;())
{
&nbsp;&nbsp;&nbsp; switch (entry.State)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Modified:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Unchanged;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Deleted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.Reload();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Added:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Detached;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default: break;


&nbsp;&nbsp;&nbsp; }
}

</pre>
<pre id="codePreview" class="csharp">
foreach (DbEntityEntry&lt;T&gt; entry in context.ChangeTracker.Entries&lt;T&gt;())
{
&nbsp;&nbsp;&nbsp; switch (entry.State)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Modified:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Unchanged;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Deleted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.Reload();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Added:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Detached;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default: break;


&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">c. Undo the Changes in the DbEntity level</p>
<p class="MsoNormal">It will first get the entry of the entity, and then undoes the changes.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
DbEntityEntry entry = context.Entry(entity);
if (entry != null)
{
&nbsp;&nbsp;&nbsp; switch (entry.State)
&nbsp;&nbsp;&nbsp; {
&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;case EntityState.Modified:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Unchanged;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Deleted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.Reload();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Added:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Detached;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default: break;
&nbsp;&nbsp;&nbsp; }
}

</pre>
<pre id="codePreview" class="csharp">
DbEntityEntry entry = context.Entry(entity);
if (entry != null)
{
&nbsp;&nbsp;&nbsp; switch (entry.State)
&nbsp;&nbsp;&nbsp; {
&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;case EntityState.Modified:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Unchanged;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Deleted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.Reload();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case EntityState.Added:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; entry.State = EntityState.Detached;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default: break;
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">d. Undo the Change in the DbEntity Property level</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
DbEntityEntry entry = context.Entry(entity);
if (entry.State == EntityState.Added || entry.State == EntityState.Detached)
{
&nbsp;&nbsp;&nbsp; return;
}


// Get and Set the Property value by the Property Name.
object propertyValue = entry.OriginalValues.GetValue&lt;object&gt;(propertyName);
entry.Property(propertyName).CurrentValue = entry.Property(propertyName).OriginalValue;

</pre>
<pre id="codePreview" class="csharp">
DbEntityEntry entry = context.Entry(entity);
if (entry.State == EntityState.Added || entry.State == EntityState.Detached)
{
&nbsp;&nbsp;&nbsp; return;
}


// Get and Set the Property value by the Property Name.
object propertyValue = entry.OriginalValues.GetValue&lt;object&gt;(propertyName);
entry.Property(propertyName).CurrentValue = entry.Property(propertyName).OriginalValue;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><b style="">2. Undo the Changes of the ObjectContext </b></p>
<p class="MsoNormal">a. Undo the Changes in the Context level</p>
<p class="MsoNormal">If the states of the entities are Modified or Deleted, refresh the date from the database.</p>
<p class="MsoNormal">If the states of the entities are Added, detach these new entities.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
IEnumerable&lt;object&gt; collection = from e in context.ObjectStateManager.GetObjectStateEntries
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (System.Data.EntityState.Modified | System.Data.EntityState.Deleted)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select e.Entity;
context.Refresh(RefreshMode.StoreWins, collection);


IEnumerable&lt;object&gt; AddedCollection = from e in context.ObjectStateManager.GetObjectStateEntries
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (System.Data.EntityState.Added)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select e.Entity;
foreach (object addedEntity in AddedCollection)
{
&nbsp;&nbsp;&nbsp; context.Detach(addedEntity);
}

</pre>
<pre id="codePreview" class="csharp">
IEnumerable&lt;object&gt; collection = from e in context.ObjectStateManager.GetObjectStateEntries
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (System.Data.EntityState.Modified | System.Data.EntityState.Deleted)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select e.Entity;
context.Refresh(RefreshMode.StoreWins, collection);


IEnumerable&lt;object&gt; AddedCollection = from e in context.ObjectStateManager.GetObjectStateEntries
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (System.Data.EntityState.Added)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select e.Entity;
foreach (object addedEntity in AddedCollection)
{
&nbsp;&nbsp;&nbsp; context.Detach(addedEntity);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">b. Undo the Changes in the DbEntities level</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
IEnumerable&lt;T&gt; collection = from o in objectSets.AsEnumerable()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; where o.EntityState == EntityState.Modified || 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;o.EntityState == EntityState.Deleted
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select o;
context.Refresh(RefreshMode.StoreWins, collection);


IEnumerable&lt;T&gt; AddedCollection = (from e in context.ObjectStateManager.GetObjectStateEntries
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (System.Data.EntityState.Added)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select e.Entity).ToList().OfType&lt;T&gt;();
foreach (T entity in AddedCollection)
{
&nbsp;&nbsp;&nbsp; context.Detach(entity);
}

</pre>
<pre id="codePreview" class="csharp">
IEnumerable&lt;T&gt; collection = from o in objectSets.AsEnumerable()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; where o.EntityState == EntityState.Modified || 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;o.EntityState == EntityState.Deleted
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select o;
context.Refresh(RefreshMode.StoreWins, collection);


IEnumerable&lt;T&gt; AddedCollection = (from e in context.ObjectStateManager.GetObjectStateEntries
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; (System.Data.EntityState.Added)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select e.Entity).ToList().OfType&lt;T&gt;();
foreach (T entity in AddedCollection)
{
&nbsp;&nbsp;&nbsp; context.Detach(entity);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">c. Undo the Changes in the DbEntity level</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
if (entity.EntityState == EntityState.Modified || entity.EntityState == EntityState.Deleted)
{
&nbsp;&nbsp;&nbsp; context.Refresh(RefreshMode.StoreWins, entity);
}
else if (entity.EntityState == EntityState.Added)
{
&nbsp;&nbsp;&nbsp; context.Detach(entity);
}

</pre>
<pre id="codePreview" class="csharp">
if (entity.EntityState == EntityState.Modified || entity.EntityState == EntityState.Deleted)
{
&nbsp;&nbsp;&nbsp; context.Refresh(RefreshMode.StoreWins, entity);
}
else if (entity.EntityState == EntityState.Added)
{
&nbsp;&nbsp;&nbsp; context.Detach(entity);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">d. Undo the Change in the <span class="SpellE">DbEntity</span> Property level</p>
<p class="MsoNormal">It will first get the entry from the entity, so we can get the original values. And then we use the reflection to set the property value of the entity.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
ObjectStateEntry entry = context.ObjectStateManager.GetObjectStateEntry(entity);
if (entry.State != EntityState.Added && entry.State != EntityState.Detached)
{
&nbsp;&nbsp;&nbsp; object propertyValue = entry.OriginalValues[propertyName];
&nbsp;&nbsp;&nbsp; PropertyInfo propertyInfo = entity.GetType().GetProperty(propertyName);
&nbsp;&nbsp;&nbsp; propertyInfo.SetValue(entity, propertyValue, null);


}

</pre>
<pre id="codePreview" class="csharp">
ObjectStateEntry entry = context.ObjectStateManager.GetObjectStateEntry(entity);
if (entry.State != EntityState.Added && entry.State != EntityState.Detached)
{
&nbsp;&nbsp;&nbsp; object propertyValue = entry.OriginalValues[propertyName];
&nbsp;&nbsp;&nbsp; PropertyInfo propertyInfo = entity.GetType().GetProperty(propertyName);
&nbsp;&nbsp;&nbsp; propertyInfo.SetValue(entity, propertyValue, null);


}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><a href="http://blog.oneunicorn.com/2011/04/03/rejecting-changes-to-entities-in-ef-4-1/"><span style="">Rejecting changes to entities in EF 4.1</span></a><span style="">
</span></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.DATA.OBJECTS.OBJECTSTATEENTRY.ORIGINALVALUES)%3bk(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22)%3bk(DevLang-CSHARP)&rd=true"><span class="SpellE">ObjectStateEntry.OriginalValues</span>
 Property</a></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.DATA.ENTITY.DBCONTEXT.CHANGETRACKER);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true"><span class="SpellE">DbContext.ChangeTracker</span>
 Property</a></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.DATA.ENTITY.INFRASTRUCTURE.DBENTITYENTRY.ORIGINALVALUES);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true"><span class="SpellE">DbEntityEntry.OriginalValues</span>
 Property </a><span class="MsoHyperlink"></span></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.DATA.OBJECTS.OBJECTCONTEXT.OBJECTSTATEMANAGER);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true"><span class="SpellE">ObjectContext.ObjectStateManager</span>
 Property</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
