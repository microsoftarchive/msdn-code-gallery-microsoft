# Get geographical location from visitor IP in ASP.NET (CSASPNETIPtoLocation)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- IP Address
## Updated
- 05/26/2011
## Description

<p style="font-family:Courier New"></p>
<h2>CSASPNETIPtoLocation Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
This project illustrates how to get the geographical location from an IP<br>
address via a db file named &quot;Location.mdf&quot;. You need install Sqlserver <br>
Express for run the web applicaiton. The code-sample only supports Internet<br>
Protocol version 4 (IPv4).<br>
<br>
Demo the Sample.<br>
<br>
Step1: Browse the Default.aspx from the sample project and you can find your <br>
IP address displayed on the page. If you are running the sample locally, you <br>
may get &quot;127.0.0.1&quot; (or &quot;::1&quot; if IPv6 is enabled) as your client and the
<br>
server is the same machine. When you deploy this demo to a host server, you <br>
will get your real IP address.<br>
<br>
[Note]<br>
If you get &quot;::1&quot; of client address, it's the IPv6 version of your IP address.
<br>
If you want to disable or enable IPv6, please refer to this KB article: <br>
<a target="_blank" href="http://support.microsoft.com/kb/929852">http://support.microsoft.com/kb/929852</a><br>
[/Note]<br>
<br>
Step2: Enter an IPv4 address (e.g. 207.46.131.43) in the TextBox and click <br>
the Submit button. You will get the basic geographical location information, <br>
including country code and country name for the specified IP address.<br>
<br>
</p>
<h3>Code Logical:</h3>
<p style="font-family:Courier New"><br>
Step1: Create a C# ASP.NET Empty Web Application in Visual Studio 2010.<br>
<br>
Step2: Add a Default ASP.NET page into the application.<br>
<br>
Step3: Add a Label, a TextBox and a Button control to the page. The Label<br>
is used to show the client IP address. TextBox is for IP address inputting,<br>
and then user can click the Button to get the location info based on that<br>
input.<br>
<br>
Step4: Write code to get the client IP address.<br>
<br>
&nbsp; &nbsp;string ipAddress;<br>
&nbsp; &nbsp;ipAddress = Request.ServerVariables[&quot;HTTP_X_FORWARDED_FOR&quot;];<br>
&nbsp; &nbsp;if (string.IsNullOrEmpty(ipAddress))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;ipAddress = Request.ServerVariables[&quot;REMOTE_ADDR&quot;];<br>
&nbsp; &nbsp;}<br>
<br>
Step5: Create a class and name it as &quot;IPConvert&quot;, this class use to convert<br>
IP address string to an IP number. The code like this:<br>
<br>
&nbsp; &nbsp;public static string ConvertToIPRange(string ipAddress)<br>
&nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; try<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; string[] ipArray = ipAddress.Split('.');<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; int number = ipArray.Length;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; double ipRange = 0;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; if (number != 4)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return &quot;error ipAddress&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; for (int i = 0; i &lt; 4; i&#43;&#43;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; int numPosition = int.Parse(ipArray[3 - i].ToString());<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; if (i == 4)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ipRange &#43;= numPosition;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ipRange &#43;= ((numPosition % 256) * (Math.Pow(256, (i))));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return ipRange.ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; catch (Exception)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return &quot;error&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; }<br>
<br>
Step6: Write code to get the location info from the Location.mdf file<br>
&nbsp; &nbsp; &nbsp; <br>
&nbsp; &nbsp;// Get the IP address string and calculate IP number.<br>
&nbsp; &nbsp;string ipRange = IPConvert.ConvertToIPRange(ipAddress);<br>
&nbsp; &nbsp;DataTable tabLocation = new DataTable();<br>
<br>
&nbsp; &nbsp;// Create a connection to Sqlserver<br>
&nbsp; &nbsp;using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[&quot;ConectString&quot;].ToString()))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;string selectCommand = &quot;select * from IPtoLocation where CAST(&quot; &#43; ipRange &#43; &quot; as bigint) between BeginingIP and EndingIP&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp;SqlDataAdapter sqlAdapter = new SqlDataAdapter(selectCommand, sqlConnection);<br>
&nbsp; &nbsp; &nbsp; &nbsp;sqlConnection.Open();<br>
&nbsp; &nbsp; &nbsp; &nbsp;sqlAdapter.Fill(tabLocation);<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;// Store IP infomation into Location entity class<br>
&nbsp; &nbsp;if (tabLocation.Rows.Count == 1)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;locationInfo.BeginIP = tabLocation.Rows[0][0].ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp;locationInfo.EndIP = tabLocation.Rows[0][1].ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp;locationInfo.CountryTwoCode = tabLocation.Rows[0][2].ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp;locationInfo.CountryThreeCode = tabLocation.Rows[0][3].ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp;locationInfo.CountryName = tabLocation.Rows[0][4].ToString();<br>
&nbsp; &nbsp;}<br>
&nbsp; &nbsp;else<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;Response.Write(&quot;&lt;strong&gt;Cannot find the location based on the IP address [&quot; &#43; ipAddress &#43; &quot;].&lt;/strong&gt; &quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp;return;<br>
&nbsp; &nbsp;}<br>
<br>
Step7: Write code to display the info on the page.<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
# SQLServer Express<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/dd981032.aspx">http://msdn.microsoft.com/en-us/library/dd981032.aspx</a><br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
