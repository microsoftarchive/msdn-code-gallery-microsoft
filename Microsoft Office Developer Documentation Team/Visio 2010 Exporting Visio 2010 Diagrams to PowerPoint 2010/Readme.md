# Visio 2010: Exporting Visio 2010 Diagrams to PowerPoint 2010
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Visual Studio 2010
- PowerPoint 2010
- Visio 2010
## Topics
- importing
## Updated
- 11/12/2012
## Description

<p><span style="font-size:small">This add-in for Visio 2010 demonstrates how to recreate Visio diagrams in PowerPoint 2010. The add-in reads positioning and formatting data from the Visio shapes, converts the data into values that PowerPoint can use, and then
 creates new slides and analog shapes in PowerPoint for each Visio shape. The add-in has two modes: one that translates a single Visio page into a single PowerPoint slide, and another that translates an entire Visio document into a PowerPoint presentation.</span></p>
<h1>Description of the &ldquo;Export Visio to PowerPoint&rdquo; add-in</h1>
<p><span style="font-size:small">The add-in includes a simple ribbon customization that includes a custom
<strong>Export</strong> tab with a single <strong>Export to PowerPoint</strong> group. The
<strong>Export to PowerPoint</strong> group includes two buttons: <strong>Export Page</strong> and
<strong>Export Diagram</strong>. The markup for the <strong>Export</strong> tab is contained in the Ribbon1.xml file. All of the ribbon callback methods are included in the Ribbon1.vb file.</span></p>
<h2>Figure 1. Export tab with a basic flowchart</h2>
<h2><img id="59829" src="59829-visio2010sample1.png" alt="" width="647" height="466"></h2>
<h2>Figure 2: Results of export operation</h2>
<p><img id="59830" src="59830-visio2010sample2.png" alt="" width="645" height="467"></p>
<p><span style="font-size:small">The code that translates Visio shape data to values that PowerPoint can read is included in the ShapeConversion.vb class file. The add-in consumes
<strong>ShapeConversion</strong> objects when creating new PowerPoint shapes. Static conversion data, like the mapping of Visio shape types to PowerPoint shape types, is contained in the ShapeConversion.xml file (which the
<strong>ShapeConversion</strong> class reads).</span></p>
<p><span style="font-size:small">This add-in also demonstrates how to control one application from another. The code in the ThisAddIn.vb file shows how to create an instance of PowerPoint from another application, add slides to the presentation, add and format
 shapes on the slides, and then how to connect the shapes together with connectors.&nbsp;</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">Visio 2010 (any SKU).</span> </li><li><span style="font-size:small">PowerPoint 2010.</span> </li><li><span style="font-size:small">Visual Studio 2010.</span> </li><li><span style="font-size:small">Basic familiarity with VB.NET, XML, and LINQ.</span>
</li></ul>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The &ldquo;Export Visio to PowerPoint&rdquo; add-in contains the following notable files:</span></p>
<ul>
<li><span style="font-size:small">The CodeSample_ExportVisio project, including:</span>
<ul>
<li><span style="font-size:small">ThisAddIn.vb code file</span> </li><li><span style="font-size:small">Ribbon1.vb code file</span> </li><li><span style="font-size:small">Ribbon1.xml file</span> </li><li><span style="font-size:small">ShapeConversion.vb code file</span> </li><li><span style="font-size:small">ShapeConversion.xml file</span> </li><li><span style="font-size:small">ExportVisioDiagram_32x32.png image</span> </li><li><span style="font-size:small">ExportVisioPage_32x32.png image</span> </li></ul>
</li></ul>
<h1>Configure the sample</h1>
<p><span style="font-size:small">You will need to install the Visual Studio Tools for Office Runtime and .NET Framework 4 to use the add-in.</span></p>
<h1>Build the sample</h1>
<p><span style="font-size:small">Choose the F5 key to build the add-in.</span></p>
<h1>Run and test the sample</h1>
<ol>
<li><span style="font-size:small">Choose the F5 key to build and deploy the add-in.</span><br>
<span style="font-size:small">An instance of Visio 2010 opens up, with an <strong>
Export</strong> tab added to the ribbon.</span> </li><li><span style="font-size:small">Create a new Visio diagram using the &ldquo;Basic Flowchart (US units) template.&rdquo;</span>
</li><li><span style="font-size:small">On the <strong>Export</strong> tab, in the <strong>
Export to PowerPoint</strong> group, choose either the <strong>Export Page</strong> or
<strong>Export Diagram</strong> button.</span><br>
<span style="font-size:small">A new PowerPoint 2010 presentation opens up. Slides and shapes will be added to the presentation as the add-in iterates over the Visio document.</span>
</li></ol>
<h1>Troubleshooting</h1>
<p><span style="font-size:small">If the add-in fails to load, check the list of disabled add-ins in Visio 2010 (<strong>File</strong> tab &gt;
<strong>Options</strong> &gt; <strong>Visio Options</strong> dialog box &gt; <strong>
Add-Ins</strong> tab).</span></p>
<p><span style="font-size:small">The &ldquo;Export Visio to PowerPoint&rdquo; add-in has the following known issues:</span></p>
<ul>
<li><span style="font-size:small">For best results, do not export a diagram that is larger than seven pages in Visio or uses a drawing size greater than 8.5 by 11 inches.</span>
</li><li><span style="font-size:small">Some PowerPoint shapes will not appear exactly similar to their Visio analogs.</span>
</li><li><span style="font-size:small">Callouts and containers in PowerPoint do not behave similarly to callouts and containers in Visio.</span>
</li><li><span style="font-size:small">Connectors in PowerPoint may not always connect at the same connections points as their Visio analogs.</span>
</li><li><span style="font-size:small">Connectors other than the <strong>Dynamic connector</strong> may not be connected to shapes in PowerPoint 2010.</span>
</li><li><span style="font-size:small">Hyperlinks applied to Visio shapes do not export to PowerPoint.</span>
</li><li><span style="font-size:small">Rich Text Format (RTF) is not preserved between Visio and PowerPoint. Bold, underline, italics, bullets, and numbering will be lost.</span>
</li><li><span style="font-size:small">Data graphic legends will not appear in PowerPoint as they do in Visio. Individual items within the legend will not be to scale.</span>
</li></ul>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small">Export Visio 2010 diagrams to PowerPoint 2010 (<a href="http://msdn.microsoft.com/library/42ddb852-b78d-4724-bbdb-a5e364944c25">http://msdn.microsoft.com/library/42ddb852-b78d-4724-bbdb-a5e364944c25</a>)</span>
</li><li><span style="font-size:small">Customizing the Ribbon in Visio 2010 by Using a Visual Studio 2010 Add-In (<a href="http://msdn.microsoft.com/en-us/library/gg617997.aspx">http://msdn.microsoft.com/en-us/library/gg617997.aspx</a>)</span>
</li><li><span style="font-size:small">Visio 2010 Automation Reference (<a href="http://msdn.microsoft.com/en-us/library/ee861526.aspx">http://msdn.microsoft.com/en-us/library/ee861526.aspx</a>)</span>
</li><li><span style="font-size:small">PowerPoint 2010 Primary Interop Assembly Reference (<a href="http://msdn.microsoft.com/en-us/library/ff759900.aspx">http://msdn.microsoft.com/en-us/library/ff759900.aspx</a>)</span>
</li></ul>
