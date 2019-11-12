# Share ASP.NET sessions between domains (CSASPNETShareSessionBetweenSubDomains)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Session Management
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>ASP.NET APPLICATION : CSASPNETShareSessionBetweenSubDomains Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
Session can be set to different modes (InProc, SqlServer, and StateServer).<br>
When using SqlServer/SateServer mode, Session will store in a specific <br>
SQL Server/Sate Server. If two ASP.NET Web Applications specify the same SQL Server
<br>
as Session Server, all Sessions store in the same database. All in all, if using <br>
SQL Server Session, it is possible to share Session between different ASP.NET <br>
Applications. Since ASP.NET stores Session Id to cookie to specify current Session,
<br>
so in order to share Session, it is necessary to share Session Id in the cookie.<br>
<br>
The CSASPNETShareSessionBetweenSubDomains sample demonstrates how to configure <br>
a SessionState Server and then create a SharedSessionModule module to achieve <br>
sharing Session between sub domain ASP.NET Web Applications.<br>
<br>
Two ASP.NET Web Applications need to run in the same Root Domain (can use <br>
different ports).<br>
</p>
<h3>Steps:</h3>
<p style="font-family:Courier New"><br>
1. Configure SQL Server to Store ASP.NET Session State.<br>
<br>
&nbsp; Run &quot;C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssadd&quot;<br>
&nbsp; to add Session State support to Sql Server Express [1].<br>
<br>
&nbsp; If you haven't added Session State to SQL Server, when you configure a web site to use SQL Server Mode<br>
&nbsp; Session State, <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Data.SqlClient.SqlException.aspx" target="_blank" title="Auto generated link to System.Data.SqlClient.SqlException">System.Data.SqlClient.SqlException</a> will be thrown saying <br>
&nbsp; &quot;Invalid object name 'tempdb.dbo.ASPStateTempSessions'.&quot;<br>
<br>
2. Configure ASP.NET Web Applications to Use SQL Server to Store Session and <br>
&nbsp; Use specific decryptionKey and validationKey.<br>
<br>
&nbsp; Add this settings to web.config file to use SQL Server Session State:<br>
&nbsp; &lt;configuration&gt;<br>
&nbsp; &nbsp;&lt;system.web&gt;<br>
&nbsp; &nbsp; &lt;sessionState <br>
&nbsp; &nbsp; &nbsp; &nbsp; mode=&quot;SQLServer&quot; <br>
&nbsp; &nbsp; &nbsp; &nbsp; sqlConnectionString=&quot;Data Source=localhost\sqlexpress;Integrated Security=True&quot; /&gt;<br>
&nbsp; &nbsp;&lt;/system.web&gt;<br>
&nbsp; &lt;/configuration&gt;<br>
<br>
&nbsp; Add this settings to web.config to use specific decryptionKey and validationKey:<br>
&nbsp; &lt;configuration&gt;<br>
&nbsp; &nbsp;&lt;system.web&gt;<br>
&nbsp; &nbsp; &lt;machineKey <br>
&nbsp; &nbsp; &nbsp; &nbsp; decryptionKey=&quot;EDCDA6DF458176504BBCC720A4E29348E252E652591179E2&quot;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; validationKey=&quot;CC482ED6B5D3569819B3C8F07AC3FA855B2FED7F0130F55D8405597C796457A2F5162D35C69B61F257DB5EFE6BC4F6CEBDD23A4118C4519F55185CB5EB3DFE61&quot;/&gt;<br>
&nbsp; &nbsp;&lt;/system.web&gt;<br>
&nbsp; &lt;/configuration&gt;<br>
<br>
&nbsp; If you host the applications in IIS, please run the Application Pool under
<br>
&nbsp; an account who can log into the database. Otherwise <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Data.SqlClient.SqlException.aspx" target="_blank" title="Auto generated link to System.Data.SqlClient.SqlException">System.Data.SqlClient.SqlException</a><br>
&nbsp; will be thrown saying &quot;Cannot open database 'ASPState' requested by the login. The login failed.&quot;<br>
<br>
3. Write SharedSessionModule Module to Achieve The Logic of Sharing Session<br>
<br>
&nbsp; a. Implement Init() method to set Application Id read from web.config.<br>
<br>
&nbsp; b. Implement PostRequestHandlerExecute Event to store Session Id to cookie with
<br>
&nbsp; &nbsp; &nbsp;the same domain and root path.<br>
<br>
4. Configure ASP.NET Web Applications to Use SharedSessionModule Module.<br>
&nbsp; <br>
&nbsp; Add this config to web.config to use SharedSessionModule Module:<br>
&nbsp; &lt;configuration&gt;<br>
&nbsp; &nbsp;&lt;system.web&gt;<br>
&nbsp; &nbsp; &lt;httpModules&gt;<br>
&nbsp; &nbsp; &nbsp; &lt;add <br>
&nbsp; &nbsp; &nbsp; &nbsp; name=&quot;SharedSessionModule&quot; <br>
&nbsp; &nbsp; &nbsp; &nbsp; type=&quot;CSASPNETShareSessionBetweenSubDomainsModule.SharedSessionModule, CSASPNETShareSessionBetweenSubDomainsModule, Version=1.0.0.0, Culture=neutral&quot;/&gt;<br>
&nbsp; &nbsp; &lt;/httpModules&gt;<br>
&nbsp; &nbsp;&lt;/system.web&gt;<br>
&nbsp; &nbsp;&lt;appSettings&gt;<br>
&nbsp; &nbsp; &nbsp;&lt;add key=&quot;ApplicationName&quot; value=&quot;MySampleWebSite&quot;/&gt;<br>
&nbsp; &nbsp; &nbsp;&lt;add key=&quot;RootDomain&quot; value=&quot;localhost&quot;/&gt;<br>
&nbsp; &nbsp;&lt;/appSettings&gt;<br>
&nbsp; &lt;/configuration&gt;<br>
<br>
&nbsp; If you run the applications in your own domains except localhost, <br>
&nbsp; please don't forget to change the value of RootDomain after publishing.<br>
<br>
5. Run and Test<br>
&nbsp; <br>
&nbsp; a. Add a new Web Page.<br>
&nbsp; b. Add two Buttons (used to Refresh the page and Set Session) and one Label for displaying<br>
&nbsp; &nbsp; &nbsp;Session value.<br>
&nbsp; c. On Page_PreRender() method, read Session and display it in Label. On Button Click<br>
&nbsp; &nbsp; &nbsp;Event, Set Value to Session.<br>
&nbsp; d. Create a new Web Site with the same config as Web Site 1, but set different value<br>
&nbsp; &nbsp; &nbsp;to Session<br>
&nbsp; e. Now open two sites in two tabs. Now if you set Session Value in site1,<br>
&nbsp; &nbsp; &nbsp;you can retrieve the same value in site2. So they use the same Session.<br>
<br>
[1] Remove Session State from Sql Server.<br>
&nbsp; &nbsp;Run &quot;C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssremove&quot;<br>
&nbsp; &nbsp;to remove Session State support from Sql Server.<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
ASP.NET Session State<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms972429.aspx">http://msdn.microsoft.com/en-us/library/ms972429.aspx</a><br>
<br>
ASP.NET SQL Server Registration Tool (Aspnet_regsql.exe) <br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms229862(VS.80).aspx">http://msdn.microsoft.com/en-us/library/ms229862(VS.80).aspx</a><br>
<br>
ASP.NET Cookies Overview<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms178194.aspx">http://msdn.microsoft.com/en-us/library/ms178194.aspx</a><br>
<br>
How To Create an ASP.NET HTTP Module Using Visual C# .NET<br>
<a target="_blank" href="http://support.microsoft.com/kb/307996">http://support.microsoft.com/kb/307996</a><br>
<br>
Application Pool Identities<br>
<a target="_blank" href="http://learn.iis.net/page.aspx/624/application-pool-identities/">http://learn.iis.net/page.aspx/624/application-pool-identities/</a><br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
