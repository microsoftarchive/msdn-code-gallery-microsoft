# Designer View Over XML Editor
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- VSX
- Visual Studio SDK
## Topics
- Visual Studio 2010 Shell
## Updated
- 03/01/2011
## Description

<h1><span style="font-size:large">Inroduction</span></h1>
<p>Demonstrates how to create an extension with a WPF-based Visual Designer for editing XML files with a specific schema (XSD) in coordination with the Visual Studio XML Editor.</p>
<p>In this sample we implement a basic view for a .vstemplate file.</p>
<h1><br>
<span style="font-size:large">Requirements</span></h1>
<ul>
<li><a class="externalLink" href="http://www.microsoft.com/visualstudio/en-us/try/default.mspx#download">Visual Studio 2010
</a></li><li><a class="externalLink" href="http://www.microsoft.com/downloads/details.aspx?FamilyID=cb82d35c-1632-4370-acfb-83c01c2ece24&displaylang=en">Visual Studio 2010 SDK
</a></li></ul>
<h1><span style="font-size:large">Getting Started</span></h1>
<ol>
<li>Download and unzip the sample </li><li>Open the solution file </li><li>Build the solution </li><li>Open the Visual Studio experimental instance by pressing F5 </li></ol>
<h1><br>
<span style="font-size:large">To test the sample functionality</span></h1>
<ol>
<li>On the File menu, click Open </li><li>Browse to the TestTemplates sub-directory within the solution </li><li>Select and Open one of the files </li><li>A new tab opens with the contents of the file laid out in the fields of a WPF form
</li><li>On the View menu, click Code </li><li>An additional tab opens with the contents of the file formatted by the XmlEditor
</li></ol>
<h1><span style="font-size:large">Files </span></h1>
<ul>
<li>VsTemplateDesignerPackage.cs &ndash; registers the designer, via ProvideXmlEditorChooserDesignerView, as the preferred editor view for files with the .vstemplate extension and indicated schema
</li><li>EditorFactory.cs &ndash; determines if the document to be edited already exists (was already opened in the Xml Editor view), rather than assuming it must be created; creates the designer&rsquo;s EditorPane as the new Editor
</li><li>EditorPane.cs &ndash; creates the sited designer control and associated XmlModel for the file and text buffer
</li><li>IViewModel.cs &ndash; expresses the interface needed to bind the designer controls to the XmlSchema
</li><li>ViewModel.cs &ndash; implements IViewModel and manages the events needed to synchronize data between the fields in the designer and the underlying XML document, which may also be seen in the XML Editor
</li><li>VsDesignerControl.xaml[.cs] &ndash; implements the WPF controls expressing the designer interface and binds them to the ViewModel
</li><li>VsTemplateSchema.cs &ndash; XML schema file generated via xsd.exe vstemplate.xsd /classes /e /n:MyNameSpace
</li></ul>
