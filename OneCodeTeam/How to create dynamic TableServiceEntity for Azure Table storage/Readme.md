# How to create dynamic TableServiceEntity for Azure Table storage
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Azure
- Cloud
## Topics
- Microsoft Azure
## Updated
- 09/22/2016
## Description

<h1>
<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
</h1>
<h1>How to create dynamic TableServiceEntity for Azure Table storage</h1>
<h2>Introduction</h2>
<p>This sample shows how to define properties at the run time which will be added to the table when you insert the entities. Windows Azure table has flexible schema, so we needn't to define an entity class to serialize the entity.</p>
<h2>Building the Sample</h2>
<p>You should do the steps below before running the code sample.&nbsp;</p>
<p>Create a storage account&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;</p>
<p>&nbsp;1. Go to the&nbsp;<a href="https://manage.windowsazure.com/">Windows Azure Management Portal</a> and sign in.</p>
<p>&nbsp;2. Click &ldquo;New&rdquo; -&gt; &ldquo;data service&rdquo; -&gt; &ldquo;storage&rdquo;-&gt; &ldquo;quick create&rdquo;.</p>
<p>&nbsp;3. Click &ldquo;Manage Access Keys&rdquo; and get the storage account name and the primary access key</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; Open Program.cs file and Program.vb, replace Storage Account with the storage account name you got, and Primary Access Key with the primary access key you got.</p>
<p>&nbsp;<img id="136149" src="136149-qq.png" alt="" width="837" height="37"></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<h2>Running the Sample</h2>
<p>Press Ctrl&#43;F5 to run the sample.</p>
<p>You will get this:</p>
<p>&nbsp;<img id="136150" src="136150-11.png" alt="" width="574" height="375"></p>
<p>Then you can open your Table storage and you will see:</p>
<p>&nbsp;</p>
<p>Please notice that, we don't declare Id and IsRead properties in this project, but no error happens. This is because table storage is a flexible storage. If we want to use it more flexibly, define the property at the run time is a good choice.</p>
<h2>Using the Code</h2>
<ol>
<li>Create a console application, name it CSAzureDynamicTalbeEntity. </li><li>Manage the nugget to get the latest Azure.Storage package. </li><li>Create a new class: DynamicObjectTalbeEntity.cs. This entity inherits DynamicObject class and implements ITalbeEntity interface. So you can use it as an Azure table entity, and add properties at the run time.
</li></ol>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">#region ITableEntity properties

      // Summary:

      //     Gets or sets the entity's current ETag. Set this value to '*' in order to

      //     blindly overwrite an entity as part of an update operation.

      public string ETag { get; set; }

      //

      // Summary:

      //     Gets or sets the entity's partition key.

      public string PartitionKey { get; set; }

      //

      // Summary:

      //     Gets or sets the entity's row key.

      public string RowKey { get; set; }

      //

      // Summary:

      //     Gets or sets the entity's time stamp.

      public DateTimeOffset Timestamp { get; set; }

      #endregion

#region ITableEntity implementation

 

      public void ReadEntity(IDictionary&lt;string, EntityProperty&gt; properties, OperationContext operationContext)

      {

          this.properties = properties;

      }

 

      public IDictionary&lt;string, EntityProperty&gt; WriteEntity(OperationContext operationContext)

      {

          return this.properties;

      }

 

      #endregion

 </pre>
<pre class="hidden">#Region &quot;ITableEntity properties&quot;

    ' Summary:

    '     Gets or sets the entity's current ETag. Set this value to '*' in order to

    '     blindly overwrite an entity as part of an update operation.

    Public Property ETag() As String Implements ITableEntity.ETag

        Get

            Return m_ETag

        End Get

        Set(value As String)

            m_ETag = value

        End Set

    End Property

    Private m_ETag As String

    '

    ' Summary:

    '     Gets or sets the entity's partition key.

    Public Property PartitionKey() As String Implements ITableEntity.PartitionKey

        Get

            Return m_PartitionKey

        End Get

        Set(value As String)

            m_PartitionKey = value

        End Set

    End Property

    Private m_PartitionKey As String

    '

    ' Summary:

    '     Gets or sets the entity's row key.

    Public Property RowKey() As String Implements ITableEntity.RowKey

        Get

            Return m_RowKey

        End Get

        Set(value As String)

            m_RowKey = value

        End Set

    End Property

    Private m_RowKey As String

    '

    ' Summary:

    '     Gets or sets the entity's time stamp.

    Public Property Timestamp() As DateTimeOffset Implements ITableEntity.Timestamp

        Get

            Return m_Timestamp

        End Get

        Set(value As DateTimeOffset)

            m_Timestamp = value

        End Set

    End Property

    Private m_Timestamp As DateTimeOffset

#End Region

#Region &quot;ITableEntity implementation&quot;

    Public Sub ReadEntity(properties As IDictionary(Of String, EntityProperty), operationContext As OperationContext) Implements ITableEntity.ReadEntity

        Me.properties = properties

    End Sub

    Public Function WriteEntity(operationContext As OperationContext) As IDictionary(Of String, EntityProperty) Implements ITableEntity.WriteEntity

        Return Me.properties

    End Function

#End Region

--VB code snippet end--</pre>
<div class="preview">
<pre class="csharp"><span class="cs__preproc">#region&nbsp;ITableEntity&nbsp;properties</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Summary:</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Gets&nbsp;or&nbsp;sets&nbsp;the&nbsp;entity's&nbsp;current&nbsp;ETag.&nbsp;Set&nbsp;this&nbsp;value&nbsp;to&nbsp;'*'&nbsp;in&nbsp;order&nbsp;to</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;blindly&nbsp;overwrite&nbsp;an&nbsp;entity&nbsp;as&nbsp;part&nbsp;of&nbsp;an&nbsp;update&nbsp;operation.</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;ETag&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Summary:</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Gets&nbsp;or&nbsp;sets&nbsp;the&nbsp;entity's&nbsp;partition&nbsp;key.</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;PartitionKey&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Summary:</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Gets&nbsp;or&nbsp;sets&nbsp;the&nbsp;entity's&nbsp;row&nbsp;key.</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;RowKey&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Summary:</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Gets&nbsp;or&nbsp;sets&nbsp;the&nbsp;entity's&nbsp;time&nbsp;stamp.</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;DateTimeOffset&nbsp;Timestamp&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span><span class="cs__preproc">&nbsp;
&nbsp;
#region&nbsp;ITableEntity&nbsp;implementation</span>&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;ReadEntity(IDictionary&lt;<span class="cs__keyword">string</span>,&nbsp;EntityProperty&gt;&nbsp;properties,&nbsp;OperationContext&nbsp;operationContext)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.properties&nbsp;=&nbsp;properties;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;IDictionary&lt;<span class="cs__keyword">string</span>,&nbsp;EntityProperty&gt;&nbsp;WriteEntity(OperationContext&nbsp;operationContext)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">this</span>.properties;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>&nbsp;
&nbsp;
&nbsp;</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp; 4.<span style="font-size:10px">&nbsp;</span><span style="font-size:10px">The following code snippet overrides DynamicObject&rsquo;s Class.</span></div>
<p>&nbsp;</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">public override bool TryGetMember(GetMemberBinder binder, out object result)

     {

           if (!properties.ContainsKey(binder.Name))

               properties.Add(binder.Name, ConvertToEntityProperty(binder.Name, null));

           result = properties[binder.Name];

      return true;

     }

     public override bool TrySetMember(SetMemberBinder binder, object value)

     {

           EntityProperty property = ConvertToEntityProperty(binder.Name, value);

           if (properties.ContainsKey(binder.Name))

               properties[binder.Name] = property;

           else

               properties.Add(binder.Name, property);

          

      return true;

     }</pre>
<pre class="hidden">Public Overrides Function TryGetMember(binder As GetMemberBinder, ByRef result As Object) As Boolean

        If Not properties.ContainsKey(binder.Name) Then

            properties.Add(binder.Name, ConvertToEntityProperty(binder.Name, Nothing))

        End If

        result = properties(binder.Name)

        Return True

    End Function

    Public Overrides Function TrySetMember(binder As SetMemberBinder, value As Object) As Boolean

        Dim [property] As EntityProperty = ConvertToEntityProperty(binder.Name, value)

        If properties.ContainsKey(binder.Name) Then

            properties(binder.Name) = [property]

        Else

            properties.Add(binder.Name, [property])

        End If

        Return True

    End Function</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">override</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;TryGetMember(GetMemberBinder&nbsp;binder,&nbsp;<span class="cs__keyword">out</span>&nbsp;<span class="cs__keyword">object</span>&nbsp;result)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(!properties.ContainsKey(binder.Name))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;properties.Add(binder.Name,&nbsp;ConvertToEntityProperty(binder.Name,&nbsp;<span class="cs__keyword">null</span>));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;result&nbsp;=&nbsp;properties[binder.Name];&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">override</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;TrySetMember(SetMemberBinder&nbsp;binder,&nbsp;<span class="cs__keyword">object</span>&nbsp;<span class="cs__keyword">value</span>)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;EntityProperty&nbsp;property&nbsp;=&nbsp;ConvertToEntityProperty(binder.Name,&nbsp;<span class="cs__keyword">value</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(properties.ContainsKey(binder.Name))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;properties[binder.Name]&nbsp;=&nbsp;property;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;properties.Add(binder.Name,&nbsp;property);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<p>&nbsp;</p>
<p><strong>More Information</strong><strong> </strong></p>
<p>CloudStorageAccount Class&nbsp;&nbsp;&nbsp; <a href="http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.cloudstorageaccount.aspx">
http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.cloudstorageaccount.aspx</a>&nbsp;</p>
<p>CloudTableClient Class <a href="https://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storageclient.cloudtableclient(v=azure.95).aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-2">
https://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storageclient.cloudtableclient(v=azure.95).aspx?cs-save-lang=1&amp;cs-lang=csharp#code-snippet-2</a></p>
<p>CloudTable Class <a href="https://msdn.microsoft.com/en-us/library/azure/microsoft.windowsazure.storage.table.cloudtable.aspx">
https://msdn.microsoft.com/en-us/library/azure/microsoft.windowsazure.storage.table.cloudtable.aspx</a></p>
<p>CloudTable.Execute Method <a href="https://msdn.microsoft.com/en-us/library/azure/microsoft.windowsazure.storage.table.cloudtable.execute.aspx">
https://msdn.microsoft.com/en-us/library/azure/microsoft.windowsazure.storage.table.cloudtable.execute.aspx</a></p>
<p>ITableEntity Interface <a href="https://msdn.microsoft.com/en-us/library/azure/microsoft.windowsazure.storage.table.itableentity.aspx">
https://msdn.microsoft.com/en-us/library/azure/microsoft.windowsazure.storage.table.itableentity.aspx</a></p>
