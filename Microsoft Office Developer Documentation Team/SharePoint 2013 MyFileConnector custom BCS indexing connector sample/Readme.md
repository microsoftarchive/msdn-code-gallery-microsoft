# SharePoint 2013: MyFileConnector custom BCS indexing connector sample
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- C#
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- Search
- data and storage
## Updated
- 03/19/2013
## Description

<p><span style="font-size:small">The MyFileConnector sample demonstrates how to create a basic &#65279;Business Connectivity Services (BCS) indexing connector that crawls all files and folders within a file share on a Windows file system. This sample is designed to
 show you how to create custom BCS indexing connectors. For more information about creating custom indexing connectors, see
<a href="http://msdn.microsoft.com/library/38560a3b-69c6-4a56-97ca-3625bbd5755e.aspx">
Search connector framework in SharePoint 2013</a>.</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">Visual Studio</span> </li><li><span style="font-size:small">SharePoint Server 2013</span> </li></ul>
<h1><span style="font-size:medium">Key components of the MyFileConnector sample</span></h1>
<p><span style="font-size:small">The MyFileConnector sample contains the following:</span></p>
<ul>
<li><span style="font-size:small">MyFile.cs, which defines the file and folder external content types, and provides the method implementations for the
<strong>Finder</strong> and <strong>SpecificFinder</strong> operations</span> </li><li><span style="font-size:small">MyFileConnector.cs, which derives from the <strong>
StructuredRepositorySystemUtility</strong> class, which implements the <a href="http://msdn.microsoft.com/library/Microsoft.BusinessData.Runtime.ISystemUtility">
ISystemUtility</a>&nbsp;interface.</span> </li><li><span style="font-size:small">MyFileLobUri.cs, which derives from the <a href="http://msdn.microsoft.com/library/Microsoft.Office.Server.Search.Connector.BDC.LobUri">
LobUri</a> class, which maps the URLs as they are passed from Search to BCS. </span>
</li><li><span style="font-size:small">MyFileNamingContainer.cs, which implements the <a href="http://msdn.microsoft.com/library/Microsoft.Office.Server.Search.Connector.BDC.INamingContainer">
INamingContainer</a> interface, and maps the URLs as they are passed from BCS to Search.</span>
</li><li><span style="font-size:small">MyFileModel.xml, which is the BDC model file for the MyFileConnector sample.</span>
</li></ul>
<h1>Configure the sample</h1>
<ol>
<li><span style="font-size:small">Open the MyFileConnector project in Visual Studio.</span>
</li><li><span style="font-size:small">In <strong>Solution Explorer</strong>, expand the
<strong>References</strong> folder, and then restore any missing project references.&nbsp; The sample includes references to the following SharePoint Server 2013 assemblies:</span>
</li></ol>
<p style="padding-left:60px"><span style="font-size:small">&bull;&nbsp;<strong>Microsoft.BusinessData</strong></span><br>
<span style="font-size:small">&bull;&nbsp;<strong>Microsoft.SharePoint</strong></span><br>
<span style="font-size:small">&bull;&nbsp;<strong>Microsoft.Office.Server.Search.Connector</strong></span></p>
<h1>Run and test the sample</h1>
<p><span style="font-size:small">To deploy the MyFileConnector sample:</span></p>
<ol>
<li><span style="font-size:small">On the application server, add the sample assembly (MyFileConnector.dll) to the global assembly cache. For more information, see
<a href="http://msdn.microsoft.com/en-us/library/dkkx7f79(v=VS.110).aspx" target="_blank">
How to: Install an Assembly into the Global Assembly Cache</a>.</span> </li><li><span style="font-size:small">Copy MyFileModel.xml to the application server.</span>
</li><li><span style="font-size:small">Open the SharePoint Management Shell. </span></li><li><span style="font-size:small">At the command prompt, do the following:<br>
</span><br>
<span style="font-size:small">&bull;&nbsp;Type the following command, and then run it.</span><br>
<span style="font-size:small"><br>
&nbsp;&nbsp;<strong>$searchapp = Get-SPEnterpriseSearchServiceApplication</strong></span><br>
<span style="font-size:small"><br>
&bull;&nbsp;Type the following command, and then run it.</span><br>
<span style="font-size:small"><br>
&nbsp;&nbsp;<strong>New-SPEnterpriseSearchCrawlCustomConnector -SearchApplication $searchapp -protocol myfile -ModelFilePath &quot;<a href="file://\\ServerName\FolderName\MyFileModel.xml">\\ServerName\FolderName\MyFileModel.xml</a>&quot; -Name myfile</strong></span>
</li><li><span style="font-size:small">Add the following registry subkey to the server, and then set the value to
<strong>OSearch15.ConnectorProtocolHandler.1</strong>:</span><br>
<span style="font-size:small">[HKEY_LOCAL_MACHINE]\ SOFTWARE\Microsoft\Office Server\15.0\Search\Setup\ProtocolHandlers\myfile</span>
</li><li><span style="font-size:small">At the command prompt, do the following:</span><br>
<span style="font-size:small">&bull; Type the following command, and then run it.&nbsp;</span><br>
<span style="font-size:small"><br>
&nbsp; <strong>net stop osearch15</strong></span><br>
<span style="font-size:small"><br>
&bull; Type the following command, and then run it.&nbsp;</span><br>
<span style="font-size:small"><br>
&nbsp; <strong>net start osearch15</strong></span> </li><li><span style="font-size:small">On the Search Administration page, click <strong>
Content Sources</strong>, and then click <strong>New Content Source</strong>.</span>
</li><li><span style="font-size:small">Specify a name for the content source, and in <strong>
Content Source Type</strong>, click <strong>Custom Repository</strong>.</span> </li><li><span style="font-size:small">In <strong>Type of Repository</strong>, click <strong>
myfile</strong>.</span> </li><li><span style="font-size:small">In <strong>Start Addresses</strong>, type the following:</span><br>
<strong><span style="font-size:small">myfile://FileServerName/FileShareName/</span>
</strong></li></ol>
<h1>Change log</h1>
<p><span style="font-size:small">First version:&nbsp;July 16, 2012</span></p>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/38560a3b-69c6-4a56-97ca-3625bbd5755e.aspx" href="http://msdn.microsoft.com/library/38560a3b-69c6-4a56-97ca-3625bbd5755e.aspx">Search connector framework</a></span>
</li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/3c67b1cf-5fca-4805-a1b5-c9ac1ff8aede.aspx" href="http://msdn.microsoft.com/library/3c67b1cf-5fca-4805-a1b5-c9ac1ff8aede.aspx">Enhancing the BDC Model File for Search</a></span>
</li></ul>
