# How to use SqlDependency to get the notification in Entity Framework
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- Entity Framework
- .NET Development
## Topics
- Entity Frameowork
- Sqldependency
- automatically update
## Updated
- 08/17/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="http://bit.ly/onecodesampletopbanner" alt="">
</a></div>
<h1>How to Automatically Update Data by Sqldependency in Entity Framework (VBEFAutoUpdate)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">We can use the Sqldependency to get the notification when the data is changed in database, but EF doesn't have the same feature. In this sample, we will demonstrate how to automatically update by Sqldependency in Entity Framework.</p>
<p class="MsoNormal">In this sample, we will demonstrate two ways that use SqlDependency to get the change notification to auto update data:</p>
<p class="MsoNormal">1. Get the notification of changes immediately.</p>
<p class="MsoNormal">2. Get the notification of changes regularly.</p>
<p class="MsoNormal"><strong>Note</strong> For C# version, you can go to&nbsp;<a href="https://code.msdn.microsoft.com/How-to-use-SqlDependency-5c0da0b3">https://code.msdn.microsoft.com/How-to-use-SqlDependency-5c0da0b3</a> to download.</p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before you run the sample, you need to finish the following step:</p>
<p class="MsoNormal">Step1. Modify the connection string in the App.config-&gt; &lt;configuration&gt;-&gt; &lt;connectionStrings&gt; to your SQL Server 2008 database instance.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to run the sample, the following is the result.</p>
<p class="MsoNormal"><span><img src="101154-image.png" alt="" width="756" height="452" align="middle">
</span></p>
<p class="MsoNormal"><strong>1. Immediately Update </strong></p>
<p class="MsoNormal"><span>First, input the range of price to get the products, or you can directly click the
<strong><em>Get Data</em></strong> button to get all the products. </span></p>
<p class="MsoNormal"><span>&nbsp;</span><span> <img src="101155-image.png" alt="" width="756" height="452" align="middle">
</span></p>
<p class="MsoNormal">Second, you can click the cell in DataGridView to select the product or just input the id in
<strong><em>Product Id</em></strong>, and then input the new price. After click the
<strong><em>Update</em></strong> button, the value in DataGridView will update.</p>
<p class="MsoNormal"><span><img src="101156-image.png" alt="" width="756" height="452" align="middle">
</span></p>
<p class="MsoNormal">At finally, you can click <strong><em>Stop</em></strong> button to stop the Update.</p>
<p class="MsoNormal"><strong>2. Regularly Update </strong></p>
<p class="MsoNormal">First, input the range of price to get the products and input the seconds to set the period of update.</p>
<p class="MsoNormal"><span><img src="101157-image.png" alt="" width="756" height="452" align="middle">
</span></p>
<p class="MsoNormal">Then you can input the new price and update. The value will be updated at the end of the period.</p>
<p class="MsoNormal"><span><img src="101158-image.png" alt="" width="756" height="452" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
0.<strong> Start/Stop the monitor</strong></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
We need a global location, to start/stop the sql dependency monitor activity for each DbContext. In this sample, we make use of form &quot;On Load&quot; and &quot;Form Closed&quot; event to put the codes.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB Script</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vbs</span>

<div class="preview">
<pre class="vbs"><span class="vbScript__keyword">Private</span>&nbsp;<span class="vbScript__keyword">Sub</span>&nbsp;AutoUpdate_Load(<span class="vbScript__keyword">ByVal</span>&nbsp;sender&nbsp;<span class="vbScript__keyword">As</span>&nbsp;<span class="vbScript__keyword">Object</span>,&nbsp;<span class="vbScript__keyword">ByVal</span>&nbsp;e&nbsp;<span class="vbScript__keyword">As</span>&nbsp;EventArgs)&nbsp;<span class="vbScript__keyword">Handles</span>&nbsp;<span class="vbScript__keyword">MyBase</span>.Load&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;btnGetData.Enabled&nbsp;=&nbsp;CanRequestNotifications()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;CreateDatabaseIfNotExist()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="vbScript__com">'&nbsp;start&nbsp;notification&nbsp;monitor</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ImmediateNotificationRegister(<span class="vbScript__keyword">Of</span>&nbsp;Product).StartMonitor(warehouse)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;RegularlyNotificationRegister(<span class="vbScript__keyword">Of</span>&nbsp;Product).StartMonitor(warehouse)&nbsp;
<span class="vbScript__keyword">End</span>&nbsp;<span class="vbScript__keyword">Sub</span>&nbsp;
&nbsp;
<span class="vbScript__keyword">Private</span>&nbsp;<span class="vbScript__keyword">Sub</span>&nbsp;AutoUpdate_FormClosed(sender&nbsp;<span class="vbScript__keyword">As</span>&nbsp;<span class="vbScript__keyword">Object</span>,&nbsp;e&nbsp;<span class="vbScript__keyword">As</span>&nbsp;FormClosedEventArgs)&nbsp;<span class="vbScript__keyword">Handles</span>&nbsp;<span class="vbScript__keyword">MyBase</span>.FormClosed&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="vbScript__com">'&nbsp;stop&nbsp;notification&nbsp;monitor</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ImmediateNotificationRegister(<span class="vbScript__keyword">Of</span>&nbsp;Product).StopMonitor(warehouse)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;RegularlyNotificationRegister(<span class="vbScript__keyword">Of</span>&nbsp;Product).StopMonitor(warehouse)&nbsp;
<span class="vbScript__keyword">End</span>&nbsp;<span class="vbScript__keyword">Sub</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<div class="endscriptcode">1. <strong>Get the ObjectQuery</strong></div>
<p>&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
We need the connection string, command string and parameters to create the SqlDependency, so we need to get the ObjectQuery. If we use DbQuery to query, we first convert the DbQuery to ObjectQuery.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">Public Shared Function GetObjectQuery(Of TEntity As Class)(ByVal Context As DbContext, ByVal query As IQueryable) As ObjectQuery
&nbsp;&nbsp;&nbsp; If TypeOf query Is ObjectQuery Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Return TryCast(query, ObjectQuery)
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; If Context Is Nothing Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Throw New ArgumentException(&quot;Paramter cannot be null&quot;, &quot;context&quot;)
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; ' Use the DbContext to create the ObjectContext
&nbsp;&nbsp;&nbsp; Dim objectContext As ObjectContext = (CType(Context, IObjectContextAdapter)).ObjectContext
&nbsp;&nbsp;&nbsp; ' Use the DbSet to create the ObjectSet and get the appropriate provider.
&nbsp;&nbsp;&nbsp; Dim iqueryable As IQueryable = TryCast(objectContext.CreateObjectSet(Of TEntity)(), IQueryable)
&nbsp;&nbsp;&nbsp; Dim provider As IQueryProvider = iqueryable.Provider


&nbsp;&nbsp;&nbsp; ' Use the provider and expression to create the ObjectQuery.
&nbsp;&nbsp;&nbsp; Return TryCast(provider.CreateQuery(query.Expression), ObjectQuery)
End Function

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. <strong>Immediately Update</strong></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
Before start the SqlDependency, stop all the SqlDependency.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">Private Sub BeginSqlDependency()
&nbsp;&nbsp;&nbsp; ' Before start the SqlDependency, stop all the SqlDependency.
&nbsp;&nbsp;&nbsp; SqlDependency.Stop(QueryExtension.GetConnectionString(objectQuery))
&nbsp;&nbsp;&nbsp; SqlDependency.Start(QueryExtension.GetConnectionString(objectQuery))


&nbsp;&nbsp;&nbsp; RegisterSqlDependency()
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
Then register the SqlDependency.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">Private Sub RegisterSqlDependency()
&nbsp;&nbsp;&nbsp; If Command Is Nothing OrElse Connection Is Nothing Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Throw New ArgumentException(&quot;command and connection cannot be null&quot;)
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; ' Make sure the command object does not already have
&nbsp;&nbsp;&nbsp; ' a notification object associated with it.
&nbsp;&nbsp;&nbsp; cmd.Notification = Nothing


&nbsp;&nbsp;&nbsp; ' Create and bind the SqlDependency objectto the command object.
&nbsp;&nbsp;&nbsp; dependency = New SqlDependency(cmd)
&nbsp;&nbsp;&nbsp; AddHandler dependency.OnChange, AddressOf DependencyOnChange


&nbsp;&nbsp;&nbsp; ' After register SqlDependency, the SqlCommand must be executed, or we can't 
&nbsp;&nbsp;&nbsp;&nbsp;' get the notification.
&nbsp;&nbsp;&nbsp; RegisterSqlCommand()
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
When data was changed, the even handler will be fired.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">Private Sub DependencyOnChange(ByVal sender As Object, ByVal e As SqlNotificationEventArgs)
&nbsp;&nbsp;&nbsp; ' Move the original SqlDependency event handler.
&nbsp;&nbsp;&nbsp; Dim dependency As SqlDependency = CType(sender, SqlDependency)
&nbsp;&nbsp;&nbsp; RemoveHandler dependency.OnChange, AddressOf DependencyOnChange


&nbsp;&nbsp;&nbsp; RaiseEvent OnChanged(Me, Nothing)


&nbsp;&nbsp;&nbsp; ' We re-register the SqlDependency.
&nbsp;&nbsp;&nbsp; RegisterSqlDependency()
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
3. <strong>Regularly Update </strong></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
When register the SqlDependency, we create a Threading.Timer and set the delegate, state,delay time, period, and then run it.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">Private Sub RegisterSqlDependency()
&nbsp;&nbsp;&nbsp; If Connection Is Nothing OrElse Command Is Nothing Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Throw New ArgumentException(&quot;command and connection cannot be null&quot;)
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; ' Make sure the command object does not already have
&nbsp;&nbsp;&nbsp; ' a notification object associated with it.
&nbsp;&nbsp;&nbsp; cmd.Notification = Nothing


&nbsp;&nbsp;&nbsp; ' Create and bind the SqlDependency object
&nbsp;&nbsp;&nbsp; ' to the command object.
&nbsp;&nbsp;&nbsp; dependency = New SqlDependency(cmd)
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;Id of sqldependency:{0}&quot;, dependency.Id)


&nbsp;&nbsp;&nbsp; RegisterSqlCommand()


&nbsp;&nbsp;&nbsp; timer = New Timer(AddressOf CheckChange, Nothing, 0, interval)
&nbsp;&nbsp;&nbsp; timer.Change(0, interval)
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
Then at the end of period, the delegate will be fired.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">Private Sub CheckChange(ByVal state As Object)
&nbsp;&nbsp;&nbsp; If dependency IsNot Nothing AndAlso dependency.HasChanges Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RaiseEvent OnChanged(Me, Nothing)
&nbsp;&nbsp;&nbsp; End If
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/query/dev11.query?appId=Dev11IDEF1&l=EN-US&k=k(<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Data.SqlClient.SqlDependency.aspx" target="_blank" title="Auto generated link to System.Data.SqlClient.SqlDependency">System.Data.SqlClient.SqlDependency</a>);k(TargetFrameworkMoniker-.NETFramework,Version%3Dv4.5);k(DevLang-csharp)&rd=true">SqlDependency
 Class</a> </span></p>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/a52dhwx7.aspx">Using SqlDependency in a Windows Application</a>
</span></p>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/System.Threading.Timer.aspx">Timer Class</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
