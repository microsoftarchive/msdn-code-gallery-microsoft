=============================================================================
                  CSASPNETIPtoLocation Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

This project illustrates how to get the geographical location from an IP
address via a db file named "Location.mdf". You need install Sqlserver 
Express for run the web applicaiton. The code-sample only supports Internet
Protocol version 4 (IPv4).

/////////////////////////////////////////////////////////////////////////////
Demo the Sample.

Step1: Browse the Default.aspx from the sample project and you can find your 
IP address displayed on the page. If you are running the sample locally, you 
may get "127.0.0.1" (or "::1" if IPv6 is enabled) as your client and the 
server is the same machine. When you deploy this demo to a host server, you 
will get your real IP address.

[Note]
If you get "::1" of client address, it's the IPv6 version of your IP address. 
If you want to disable or enable IPv6, please refer to this KB article: 
http://support.microsoft.com/kb/929852
[/Note]

Step2: Enter an IPv4 address (e.g. 207.46.131.43) in the TextBox and click 
the Submit button. You will get the basic geographical location information, 
including country code and country name for the specified IP address.


/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step1: Create a C# ASP.NET Empty Web Application in Visual Studio 2010.

Step2: Add a Default ASP.NET page into the application.

Step3: Add a Label, a TextBox and a Button control to the page. The Label
is used to show the client IP address. TextBox is for IP address inputting,
and then user can click the Button to get the location info based on that
input.

Step4: Write code to get the client IP address.

    string ipAddress;
    ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
    if (string.IsNullOrEmpty(ipAddress))
    {
        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
    }

Step5: Create a class and name it as "IPConvert", this class use to convert
IP address string to an IP number. The code like this:

    public static string ConvertToIPRange(string ipAddress)
       {
           try
           {
               string[] ipArray = ipAddress.Split('.');
               int number = ipArray.Length;
               double ipRange = 0;
               if (number != 4)
               {
                   return "error ipAddress";
               }
               for (int i = 0; i < 4; i++)
               {
                   int numPosition = int.Parse(ipArray[3 - i].ToString());
                   if (i == 4)
                   {
                       ipRange += numPosition;
                   }
                   else
                   {
                       ipRange += ((numPosition % 256) * (Math.Pow(256, (i))));
                   }
               }
               return ipRange.ToString();
           }
           catch (Exception)
           {
               return "error";
           }
       }

Step6: Write code to get the location info from the Location.mdf file
       
    // Get the IP address string and calculate IP number.
    string ipRange = IPConvert.ConvertToIPRange(ipAddress);
    DataTable tabLocation = new DataTable();

    // Create a connection to Sqlserver
    using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConectString"].ToString()))
    {
        string selectCommand = "select * from IPtoLocation where CAST(" + ipRange + " as bigint) between BeginingIP and EndingIP";
        SqlDataAdapter sqlAdapter = new SqlDataAdapter(selectCommand, sqlConnection);
        sqlConnection.Open();
        sqlAdapter.Fill(tabLocation);
    }

    // Store IP infomation into Location entity class
    if (tabLocation.Rows.Count == 1)
    {
        locationInfo.BeginIP = tabLocation.Rows[0][0].ToString();
        locationInfo.EndIP = tabLocation.Rows[0][1].ToString();
        locationInfo.CountryTwoCode = tabLocation.Rows[0][2].ToString();
        locationInfo.CountryThreeCode = tabLocation.Rows[0][3].ToString();
        locationInfo.CountryName = tabLocation.Rows[0][4].ToString();
    }
    else
    {
        Response.Write("<strong>Cannot find the location based on the IP address [" + ipAddress + "].</strong> ");
        return;
    }

Step7: Write code to display the info on the page.

/////////////////////////////////////////////////////////////////////////////
References:

# SQLServer Express
http://msdn.microsoft.com/en-us/library/dd981032.aspx

/////////////////////////////////////////////////////////////////////////////