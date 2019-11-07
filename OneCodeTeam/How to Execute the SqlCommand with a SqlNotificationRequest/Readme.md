# How to Execute the SqlCommand with a SqlNotificationRequest
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- .NET
- ADO.NET
- Data Access
## Topics
- SqlNotificationRequest
- SqlCommand
## Updated
- 09/22/2016
## Description

<h1><img id="154422" src="154422-8171.onecodesampletopbanner.png" alt="" width="696" height="58"></h1>
<h1><span><span>如何执行带有 SqlNotificationRequest 的 </span><span>SqlCommand</span></span></h1>
<h2><span><span>简介</span></span></h2>
<p><span><span>在数据库上执行命令获取数据后，该数据可能已在其他客户端上发生更改。 如果应用程序需要最新数据，其需要一个来自服务器的通知。 在此应用程序中，我们将演示如何执行带有 Sql</span><span>NotificationRequest 的 SqlCommand：</span></span></p>
<p><span><span>1. 设置并执行带有 SqlNotificationRequest 的 SqlCommand；</span></span></p>
<p><span><span>2. 开始监视 SqlServer 中的队列；</span></span></p>
<p><span><span>3. 刷新数据。</span></span></p>
<h2><span><span>构建示例</span></span></h2>
<p><span><span>运行该示例之前，您需要完成以下步骤：</span></span></p>
<p><span><span>步骤 1. 请选择以下一种方式来构建数据库：</span></span></p>
<p><span><span>&bull;&nbsp;</span><span>将数据文件 MySchool.mdf（位于文件夹 _External_Dependecies 下）附加到 SQL Server</span><span>（2008 或更高版本）</span><span>数据库实例。</span><span> 附加后，请在 SqlServer 中执行以下脚本：</span></span></p>
<p><span><span>ALTER DATABASE MySchool SET ENABLE_BROKER;</span></span></p>
<p><span><span>&bull;&nbsp;</span><span>在 SQL Server（2008 或更高版本）数据库实例中运行 MySchool.sql 脚本（在文件夹 _External_Dependecies 下）。</span></span></p>
<p><span><span>步骤 2. 将&ldquo;项目属性-&gt;相应设置-&gt; MySchoolConnectionString&rdquo;中的连接字符串修改为您的 SQL Server（2008 或更高版本）</span><span>数据库实例名</span></span></p>
<h2><span><span>运行示例</span></span></h2>
<p><span><span>按 F5 运行示例。</span></span></p>
<p><span><span>1. 单击 </span><span>Get Data</span><span> 按钮获取所有数据：</span></span> <strong>
&nbsp;</strong><em>&nbsp;</em></p>
<h1><span><span><br>
</span></span></h1>
<p><img id="154423" src="154423-image.png" alt="" width="756" height="362"></p>
<p><em><span><span>2. </span></span></em>您可以单击 DataGridView 中的单元&#26684;选择一行，然后可以输入新的评分。 单击 Update 按钮后，DataGridView 中的&#20540;将更新<em><span><span>。</span></span></em></p>
<p><img id="154424" src="154424-11418413-0a54-4d87-be36-b17e716f23ecimage.png" alt=""></p>
<p><span><span>3. 您可以单击 </span><span>Stop</span><span> 按钮停止监视。</span></span></p>
<p><span><span>使用代码</span></span></p>
<p><span><span>1. 注册</span><span>SqlNotificationRequest</span></span></p>
<p><span><span>使用 SqlNotificationRequest 之前需要进行创建。</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">request = new SqlNotificationRequest();
request.UserData = new Guid().ToString();
request.Options = String.Format(&quot;Service={0};&quot;, serviceName);
request.Timeout = notificationTimeout;
</pre>
<div class="preview">
<pre class="csharp">request&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlNotificationRequest();&nbsp;
request.UserData&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Guid().ToString();&nbsp;
request.Options&nbsp;=&nbsp;String.Format(<span class="cs__string">&quot;Service={0};&quot;</span>,&nbsp;serviceName);&nbsp;
request.Timeout&nbsp;=&nbsp;notificationTimeout;&nbsp;
</pre>
</div>
</div>
</div>
<p><span><span>&nbsp;</span></span><span><span>2. 创建命令并绑定</span><span>SqlNotificationRequest</span></span></p>
<p><span><span>我们创建一个填写数据的命令并将 </span><span>SqlNotificationRequest</span><span> 绑定至该命令。</span></span><strong>&nbsp;</strong><em>&nbsp;</em></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">command.Notification = null;
command.Notification = notification.NotificationRequest;
using (SqlDataAdapter adapter = new SqlDataAdapter(command))
{
    adapter.Fill(dataToWatch, tableName);
    dgvWatch.DataSource = dataToWatch;
    dgvWatch.DataMember = tableName;
}
</pre>
<div class="preview">
<pre class="csharp">command.Notification&nbsp;=&nbsp;<span class="cs__keyword">null</span>;&nbsp;
command.Notification&nbsp;=&nbsp;notification.NotificationRequest;&nbsp;
<span class="cs__keyword">using</span>&nbsp;(SqlDataAdapter&nbsp;adapter&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlDataAdapter(command))&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;adapter.Fill(dataToWatch,&nbsp;tableName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;dgvWatch.DataSource&nbsp;=&nbsp;dataToWatch;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;dgvWatch.DataMember&nbsp;=&nbsp;tableName;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<p><span><span>3. 开始监视 Service Broker 队列</span></span></p>
<p><span><span>我们打开一个新的线程以监视 Service Broker 队列。</span></span> <strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void StartSqlNotification()
{
    listenTask = new Task(Listen);
    listenTask.Start();
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span><span class="cs__keyword">void</span>&nbsp;StartSqlNotification()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;listenTask&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Task(Listen);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;listenTask.Start();&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p><span><span>4. 正在监视 Service Broker 队列</span></span></p>
<p><span><span>如果从该队列获取到新消息，我们将刷新数据。</span></span></p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">private void Listen()
{
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (cmd = new SqlCommand(listenSql, conn))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.CommandTimeout = notificationTimeout &#43; 120;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i &lt;= reader.FieldCount - 1; i&#43;&#43;)
                            Debug.WriteLine(reader[i].ToString());
                    }
                }
            }
        }
    RegisterSqlNotificationRequest();
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Listen()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlConnection&nbsp;conn&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlConnection(connectionString))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(cmd&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlCommand(listenSql,&nbsp;conn))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(conn.State&nbsp;!=&nbsp;ConnectionState.Open)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;conn.Open();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.CommandTimeout&nbsp;=&nbsp;notificationTimeout&nbsp;&#43;&nbsp;<span class="cs__number">120</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlDataReader&nbsp;reader&nbsp;=&nbsp;cmd.ExecuteReader())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">while</span>&nbsp;(reader.Read())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">for</span>&nbsp;(<span class="cs__keyword">int</span>&nbsp;i&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;i&nbsp;&lt;=&nbsp;reader.FieldCount&nbsp;-&nbsp;<span class="cs__number">1</span>;&nbsp;i&#43;&#43;)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Debug.WriteLine(reader[i].ToString());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;RegisterSqlNotificationRequest();&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span><span>5. InvokeOnChanged 事件</span></span>
<p><span><span>如果数据已更改而 OnChanged 事件不为空，则调用该事件。</span></span></p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">if (OnChanged != null)
{
    OnChanged(this, null);
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">if</span>&nbsp;(OnChanged&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;OnChanged(<span class="cs__keyword">this</span>,&nbsp;<span class="cs__keyword">null</span>);&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p><span><span>更多信息</span></span></p>
<p><span><a href="http://msdn.microsoft.com/en-us/library/wd2x83zk(VS.110).aspx"><span>带有 SqlNotificationRequest 的 SqlCommand 执行</span></a></span></p>
<p><span><a href="http://msdn.microsoft.com/zh-cn/library/ms172133.aspx"><span>启用查询通知</span></a></span></p>
<p><span><a href="http://msdn.microsoft.com/query/dev11.query?appId=Dev11IDEF1&l=EN-US&k=k(%22System.Data.Sql.SqlNotificationRequest.%23ctor%22);k(TargetFrameworkMoniker-.NETFramework,Version%3Dv4.5);k(DevLang-csharp)&rd=true"><span>SqlNotificationRequest 构造函数</span></a><a name="_GoBack"></a></span>
<strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples
</span></p>
<p><img id="154425" src="154425-7771245f-b03b-44b4-9c65-fe5905f121b5image.png" alt=""><strong>&nbsp;</strong><em>&nbsp;</em></p>
