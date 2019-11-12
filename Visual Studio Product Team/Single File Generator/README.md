# Single File Generator
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Visual Studio 2010 SDK
## Topics
- Visual Studio Project System
- MSDBuild
- VSX
## Updated
- 02/28/2011
## Description

<p><span id="ctl00_ctl00_Content_TabContentPanel_Content_wikiSourceLabel">&nbsp;</span></p>
<h2>Introduction</h2>
<p>This sample demonstrates how to build a single file generator.<br>
<br>
</p>
<ul>
<li>Shows how to implement a single file generator. </li><li>Shows how to use CodeDom to generate source code in Visual Basic, C# </li></ul>
<p>&nbsp;</p>
<h2><strong><strong>Quick Navigation</strong></strong></h2>
<table border="0" style="font-size:12px">
<tbody>
<tr>
<td>1 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#Req">Requirements</a></td>
</tr>
<tr>
<td>2 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#downloadAndInstall">
Download and Install</a></td>
</tr>
<tr>
<td>3 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#BuildAndRun">Building and Running</a></td>
</tr>
<tr>
<td>4 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#GettingStarted">
Getting Started</a></td>
</tr>
<tr>
<td>4.1 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#ProjFiles">Project Files</a></td>
</tr>
<tr>
<td>4.2 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#UnitTests">Unit Tests</a></td>
</tr>
<tr>
<td>4.3 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#FuncTests">Functional Tests</a></td>
</tr>
<tr>
<td>4.4 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#Status">Status</a></td>
</tr>
<tr>
<td>4.5 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#History">History</a></td>
</tr>
<tr>
<td>4.6 <a href="http://code.msdn.microsoft.com/SingleFileGenerator#AddResx">Additional Resources</a></td>
</tr>
</tbody>
</table>
<p><span id="ctl00_ctl00_Content_TabContentPanel_Content_wikiSourceLabel"><br>
</span></p>
<h2>Requirements</h2>
<p><a class="externalLink" href="http://msdn.com/vstudio">Visual Studio 2010 and Visual Studio 2010 SDK</a><br>
<br>
</p>
<h2>Download and Install</h2>
<ul>
<li>Go to the &quot;Downloads tab&quot; and download the zip file associated with this sample
</li><li>Unzip the sample to your machine </li><li>Double click on the .sln file to launch the solution </li></ul>
<p>&nbsp;</p>
<h2>Building and Running</h2>
<p>To build and execute the sample, press F5 after the sample is loaded. This will launch the experimental hive which will demonstrate the sample's function.<br>
<br>
</p>
<h2>Getting Started</h2>
<p>The sample shows how to implement a single file generator. It demonstrates leveraging CodeDom to generate source code in Visual Basic, C#. It also demonstrates how to validate a XML document against a schema and communicate errors through the Error List.<br>
<br>
</p>
<h2>Project Files</h2>
<p>&nbsp;</p>
<table border="1" style="font-size:12px; border:1px solid black">
<tbody>
<tr>
<th>File Name </th>
<th>Description </th>
</tr>
<tr>
<td><strong>AssemblyInfo</strong></td>
<td>This file contains assembly custom attributes.</td>
</tr>
<tr>
<td><strong>BaseCodeGenerator</strong></td>
<td>Abstract class that implements the IVsSingleFileGenerator interface.</td>
</tr>
<tr>
<td><strong>BaseCodeGeneratorWithSite</strong></td>
<td>Abstract class that inherits from BaseCodeGenerator and implements the IObjectWithSite interface.</td>
</tr>
<tr>
<td><strong>XmlClassGenerator</strong></td>
<td>The single file generator class.</td>
</tr>
<tr>
<td><strong>SourceCodeGenerator</strong></td>
<td>Static class that contains the CodeDom code used to generate source code.</td>
</tr>
<tr>
<td><strong>Support</strong></td>
<td>Static class containing support functions.</td>
</tr>
<tr>
<td><strong>XmlClassGeneratorSchema.xsd</strong></td>
<td>Schema for XML documents that the generator knows how to convert into source code.</td>
</tr>
<tr>
<td><strong>Strings.resx</strong></td>
<td>Resource strings (localizable).</td>
</tr>
<tr>
<td><strong>ClassDiagram.cd</strong></td>
<td>Class diagram for all types defined in this project.</td>
</tr>
</tbody>
</table>
<p><span id="ctl00_ctl00_Content_TabContentPanel_Content_wikiSourceLabel"><br>
</span></p>
<h2>Unit Tests</h2>
<p>&nbsp;</p>
<table border="1" style="font-size:12px; border:1px solid black">
<tbody>
<tr>
<th>Description </th>
</tr>
<tr>
<td>Create generator instance</td>
</tr>
<tr>
<td>Verify that QI for IVsSingleFileGenerator succeeds</td>
</tr>
<tr>
<td>Verify that a single file generator registration attribute is created</td>
</tr>
<tr>
<td>Verify that Register method gets appropiately invoked</td>
</tr>
<tr>
<td>Verify that Un-Register method gets appropiately invoked</td>
</tr>
</tbody>
</table>
<p><span id="ctl00_ctl00_Content_TabContentPanel_Content_wikiSourceLabel"><br>
</span></p>
<h3>Functional Tests</h3>
<p>&nbsp;</p>
<table border="1" style="font-size:12px; border:1px solid black">
<tbody>
<tr>
<th>Description </th>
</tr>
<tr>
<td>Load the sample</td>
</tr>
<tr>
<td>Verify the sample builds in Debug Configuration</td>
</tr>
<tr>
<td>Verify the sample builds in Release Configuration</td>
</tr>
<tr>
<td>Verify that the generator is properly registered when building</td>
</tr>
<tr>
<td>Verify that the generator works (C#).</td>
</tr>
<tr>
<td>Verify that the generator works (Visual Basic).</td>
</tr>
<tr>
<td>Verify that the generator works (J#).</td>
</tr>
<tr>
<td>Verify that the generator fails for bad XML (C#).</td>
</tr>
<tr>
<td>Verify that the generator fails for bad XML (Visual Basic).</td>
</tr>
<tr>
<td>Verify that the generator fails for bad XML (J#).</td>
</tr>
</tbody>
</table>
<p><span id="ctl00_ctl00_Content_TabContentPanel_Content_wikiSourceLabel"><br>
</span></p>
<h2>Status</h2>
<p>&nbsp;</p>
<table border="1" style="font-size:12px; border:1px solid black">
<tbody>
<tr>
<th>Function </th>
<th>Status </th>
</tr>
<tr>
<td>Demonstrates Accessibility</td>
<td>Yes</td>
</tr>
<tr>
<td>Includes Architecture Diagram</td>
<td>Yes</td>
</tr>
<tr>
<td>Demonstrates Error Handling</td>
<td>No</td>
</tr>
<tr>
<td>Follows SDK Coding Standards</td>
<td>Yes</td>
</tr>
<tr>
<td>Demonstrates Localization</td>
<td>Yes</td>
</tr>
<tr>
<td>Implements Functional Tests</td>
<td>Yes</td>
</tr>
<tr>
<td>Sample supported by Microsoft</td>
<td>Yes</td>
</tr>
<tr>
<td>Implements Unit Tests</td>
<td>Yes</td>
</tr>
</tbody>
</table>
<p><span id="ctl00_ctl00_Content_TabContentPanel_Content_wikiSourceLabel"><br>
</span></p>
<h2>History</h2>
<p>&nbsp;</p>
<table border="1" style="font-size:12px; border-width:1px">
<tbody>
<tr>
<th>Date </th>
<th>Activity </th>
</tr>
<tr>
<td>2005-10-04</td>
<td>Created this sample for the Visual Studio 2005 SDK</td>
</tr>
<tr>
<td>2010-03-05</td>
<td>Ported this sample to work in Visual Studio 2010</td>
</tr>
</tbody>
</table>
<p><span id="ctl00_ctl00_Content_TabContentPanel_Content_wikiSourceLabel"><br>
</span></p>
<h4>Additional Resources</h4>
<p>&nbsp;</p>
<p><a id="112" href="/site/view/file/112/0/Single_File_Generator_Sample.docx">Single_File_Generator_Sample.docx</a></p>
<p><span id="ctl00_ctl00_Content_TabContentPanel_Content_wikiSourceLabel"><a class="externalLink" href="http://msdn.microsoft.com/en-us/library/bb166508%28VS.100%29.aspx">Implementing and Registering Single File Generators</a></span></p>
