# How to retrieve output parameter from stored procedure in Entity Framework
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- Entity Framework
- .NET Development
## Topics
- Entity Framework
## Updated
- 06/05/2013
## Description

<h1><span style="">How to get output parameter in stored procedure from Entity Framework</span> (<span style="">CSEFOutputParameter</span>)</h1>
<h2>Introduction<span style=""> </span></h2>
<p class="MsoNormal"><span style="">This sample demonstrates how to use ObjectParameter instance to get the value of output parameter in Entity Framework.
</span></p>
<h2>Building the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style="margin-bottom:0in; margin-bottom:.0001pt; text-indent:-.25in; line-height:12.75pt">
<span style="color:black"><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="color:black">Start Visual Studio 2012 and select File &gt; Open &gt; Project/Solution.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0in; margin-bottom:.0001pt; text-indent:-.25in; line-height:12.75pt">
<span style="color:black"><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="color:black">Go to the directory in which you download the sample. Go to the directory named for<span style="">&nbsp;&nbsp;
</span>the sample, and double-click the Microsoft Visual Studio Solution (.<span class="SpellE">sln</span>) file.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Attach the database file <span class="SpellE">
EFDemoDB.mdf</span> under the folder _<span class="SpellE">External_Dependencies</span> to your SQL Server 2008R2 database instance.<b>
</b></span></p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Modify the connection string in the <span class="SpellE">
App.config</span> according to your SQL Server 2008R2 database instance name.<b> </b>
</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in; line-height:12.75pt">
<span style="color:black"><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="color:black">Press F7 or use Build &gt; Build Solution to build the sample.
</span></p>
<h2>Running the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Right click the solution and built it. </span>
</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Press F5 to run the project, a console window will appear.
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style=""><img src="83618-image.png" alt="" width="676" height="144" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Follow the prompt to input person information.
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal">Stored Procedure</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>SQL</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">mysql</span>

<pre id="codePreview" class="mysql">
ALTER PROCEDURE [dbo].[InsertPerson]   
@Name varchar(50),   
@Description varchar(200),     
@ID int OUT   
AS   
INSERT INTO Person(Name,Description) VALUES(@Name,@Description)   
SET @ID = SCOPE_IDENTITY()

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal"><span style="">The code below demonstrates how to get the value of output parameter.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
// Create a ObjectParameter instance to retrieve output parameter from stored procedure
                   ObjectParameter Output = new ObjectParameter(&quot;ID&quot;, typeof(Int32));
                   context.InsertPerson(Name, Description, Output);


                   Console.Write(&quot;ID: {0}&quot;, Output.Value);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<h2>More Information</h2>
<p class="MsoNormal"><span class="SpellE"><span style="">ObjectParameter</span></span><span style=""> Class
</span></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/system.data.objects.objectparameter.aspx">http://msdn.microsoft.com/en-us/library/system.data.objects.objectparameter.aspx</a></p>
<p class="MsoNormal"><span style="">Stored Procedures in the Entity Framework </span>
</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/data/gg699321.aspx">http://msdn.microsoft.com/en-us/data/gg699321.aspx</a></p>
<p class="MsoNormal"><span style=""></span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
