==============================================================================
 ASP.NET APPLICATION : CSASPNETShareSessionBetweenSubDomains Project Overview
==============================================================================

//////////////////////////////////////////////////////////////////////////////
Summary:

Session can be set to different modes (InProc, SqlServer, and StateServer).
When using SqlServer/SateServer mode, Session will store in a specific 
SQL Server/Sate Server. If two ASP.NET Web Applications specify the same SQL Server 
as Session Server, all Sessions store in the same database. All in all, if using 
SQL Server Session, it is possible to share Session between different ASP.NET 
Applications. Since ASP.NET stores Session Id to cookie to specify current Session, 
so in order to share Session, it is necessary to share Session Id in the cookie.

The CSASPNETShareSessionBetweenSubDomains sample demonstrates how to configure 
a SessionState Server and then create a SharedSessionModule module to achieve 
sharing Session between sub domain ASP.NET Web Applications.

Two ASP.NET Web Applications need to run in the same Root Domain (can use 
different ports).

//////////////////////////////////////////////////////////////////////////////
Steps:

1. Configure SQL Server to Store ASP.NET Session State.

   Run "C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssadd"
   to add Session State support to Sql Server Express [1].

   If you haven't added Session State to SQL Server, when you configure a web site to use SQL Server Mode
   Session State, System.Data.SqlClient.SqlException will be thrown saying 
   "Invalid object name 'tempdb.dbo.ASPStateTempSessions'."

2. Configure ASP.NET Web Applications to Use SQL Server to Store Session and 
   Use specific decryptionKey and validationKey.

   Add this settings to web.config file to use SQL Server Session State:
   <configuration>
    <system.web>
     <sessionState 
         mode="SQLServer" 
         sqlConnectionString="Data Source=localhost\sqlexpress;Integrated Security=True" />
    </system.web>
   </configuration>

   Add this settings to web.config to use specific decryptionKey and validationKey:
   <configuration>
    <system.web>
     <machineKey 
         decryptionKey="EDCDA6DF458176504BBCC720A4E29348E252E652591179E2" 
         validationKey="CC482ED6B5D3569819B3C8F07AC3FA855B2FED7F0130F55D8405597C796457A2F5162D35C69B61F257DB5EFE6BC4F6CEBDD23A4118C4519F55185CB5EB3DFE61"/>
    </system.web>
   </configuration>

   If you host the applications in IIS, please run the Application Pool under 
   an account who can log into the database. Otherwise System.Data.SqlClient.SqlException
   will be thrown saying "Cannot open database 'ASPState' requested by the login. The login failed."

3. Write SharedSessionModule Module to Achieve The Logic of Sharing Session

   a. Implement Init() method to set Application Id read from web.config.

   b. Implement PostRequestHandlerExecute Event to store Session Id to cookie with 
      the same domain and root path.

4. Configure ASP.NET Web Applications to Use SharedSessionModule Module.
   
   Add this config to web.config to use SharedSessionModule Module:
   <configuration>
    <system.web>
     <httpModules>
       <add 
         name="SharedSessionModule" 
         type="CSASPNETShareSessionBetweenSubDomainsModule.SharedSessionModule, CSASPNETShareSessionBetweenSubDomainsModule, Version=1.0.0.0, Culture=neutral"/>
     </httpModules>
    </system.web>
    <appSettings>
      <add key="ApplicationName" value="MySampleWebSite"/>
      <add key="RootDomain" value="localhost"/>
    </appSettings>
   </configuration>

   If you run the applications in your own domains except localhost, 
   please don't forget to change the value of RootDomain after publishing.

5. Run and Test
   
   a. Add a new Web Page.
   b. Add two Buttons (used to Refresh the page and Set Session) and one Label for displaying
      Session value.
   c. On Page_PreRender() method, read Session and display it in Label. On Button Click
      Event, Set Value to Session.
   d. Create a new Web Site with the same config as Web Site 1, but set different value
      to Session
   e. Now open two sites in two tabs. Now if you set Session Value in site1,
      you can retrieve the same value in site2. So they use the same Session.

[1] Remove Session State from Sql Server.
    Run "C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssremove"
    to remove Session State support from Sql Server.

//////////////////////////////////////////////////////////////////////////////
References:

ASP.NET Session State
http://msdn.microsoft.com/en-us/library/ms972429.aspx

ASP.NET SQL Server Registration Tool (Aspnet_regsql.exe) 
http://msdn.microsoft.com/en-us/library/ms229862(VS.80).aspx

ASP.NET Cookies Overview
http://msdn.microsoft.com/en-us/library/ms178194.aspx

How To Create an ASP.NET HTTP Module Using Visual C# .NET
http://support.microsoft.com/kb/307996

Application Pool Identities
http://learn.iis.net/page.aspx/624/application-pool-identities/