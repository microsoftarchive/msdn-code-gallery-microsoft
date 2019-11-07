========================================================================
         ASP.NET APPLICATION : CSASPNETChart Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

  The project illustrates how to use the new Chart control to create an chart
  in the web page.

/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step1. Create a C# ASP.NET Web Application in Visual Studio 2010 RC /
Visual Web Developer 2010 and name it as CSASPNETChart.

[NOTE] As Visual Studio 2010 has not been realsed, you can download its
express version from http://www.microsoft.com/express/Web/

Step2. Delete the following default folders and files created automatically 
by Visual Studio.

Account folder
Script folder
Style folder
About.aspx file
Default.aspx file
Global.asax file
Site.Master file

Step3. Add a new web form page to the website and name it as Default.aspx.

Step4. Add a Chart control into the page. You can find it in the Data 
category of the Toolbox.

[NOTE] When a Chart control is added into the page, such a Register Info will
be added to the same page automatically.

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, 
    Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

Also, a new reference /System.Web.DataVisualization/ will be added to the web
application as well.

Step5: Add two Series into the Chart tag as the sample below.

    <Series>
        <asp:Series Name="Series1">
        </asp:Series>
        <asp:Series Name="Series2">
        </asp:Series>
    </Series>

[NOTE] The Series collection property stores Series objects, which are used to 
store data that is to be displayed, along with attributes of that data.

Step6: Edit the two Series to add ChartType property which equals to Column and
ChartArea property with the value as ChartArea1.

[NOTE] The Series ChartType value that indicates the chart type that will be 
used to represent the series. For all items in this collectin, please refer 
to this link: http://msdn.microsoft.com/en-us/library/system.web.ui.datavisualization.charting.seriescharttype(VS.100).aspx
The ChartAreas collection property stores ChartArea objects, which are primarily 
used to draw one or more charts using one set of axes. You will finally find the 
HTML code looks like this.

<asp:Chart ID="Chart1" runat="server" Height="400px" Width="500px">
    <Series>
        <asp:Series Name="Series1" ChartType="Column" ChartArea="ChartArea1">
        </asp:Series>
        <asp:Series Name="Series2" ChartType="Column" ChartArea="ChartArea1">
        </asp:Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="ChartArea1">
        </asp:ChartArea>
    </ChartAreas>
</asp:Chart>

Step7: Create data source for the Chart control via DataTable in the behind
code. In this step, please directly follow the method CreateDataTable in 
Default.aspx.cs, as this is not what we are talking about in this project.

Step8: Bind the data source to the Chart control.

    Chart1.Series[0].YValueMembers = "Volume1";
    Chart1.Series[1].YValueMembers = "Volume2";
    Chart1.Series[0].XValueMember = "Date";

[NOTE] Series.YValueMembers property is used to get or set member columns of 
the chart data source used to bind data to the Y-values of the series. Alike,
Series.XValueMember property is for getting or setting the member column of 
the chart data source used to data bind to the X-value of the series.

Step9: Now, you can run the page to see the achievement we did before :-)

/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Chart Class
http://msdn.microsoft.com/en-us/library/system.web.ui.datavisualization.charting.chart(VS.100).aspx

MSDN: Chart Controls Tutorial
http://msdn.microsoft.com/en-us/library/dd489231(VS.100).aspx

ASP.NET: Chart Control
http://www.asp.net/learn/aspnet-4-quick-hit-videos/video-8770.aspx (Quick Hit Videl)

/////////////////////////////////////////////////////////////////////////////