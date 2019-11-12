================================================================================
       Windows APPLICATION: CSRichTextBoxSyntaxHighlighting  Overview                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to format XML and highlight the elements in 
RichTextBoxControl.

RichTextBoxControl can process RTF(Rich Text Format) file, which is a proprietary
document file format with published specification developed by Microsoft Corporation.

A simple RTF file is like 

{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe2052
{\fonttbl{\f0\fnil Courier New;}}
{\colortbl ;\red0\green0\blue255;\red139\green0\blue0;\red255\green0\blue0;\red0\green0\blue0;}
\viewkind4\uc1\pard\cf1\f0\fs24 
<?\cf2 xml \cf3 version\cf1 =\cf0 "\cf1 1.0\cf0 " \cf3 encoding\cf1 =\cf0 "\cf1 utf-8\cf0 "\cf1 ?>\par
<\cf2 html\cf1 >\par
    <\cf2 head\cf1 >\par
        <\cf2 title\cf1 >\par
            \cf4 My home page\par
        \cf1 </\cf2 title\cf1 >\par
    </\cf2 head\cf1 >\par
    <\cf2 body \cf3 bgcolor\cf1 =\cf0 "\cf1 000000\cf0 " \cf3 text\cf1 =\cf0 "\cf1 ff0000\cf0 " \cf1 >\par
        \cf4 Hello World!\par
    \cf1 </\cf2 body\cf1 >\par
</\cf2 html\cf1 >\par
}

It contains 2 parts:Header and Content.The colortbl in header includes all the color 
definitions used in the file. \cfN means the Foreground color and \par means a new 
paragraph.


////////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Build this project in VS2010. 

Step2. Run CSRichTextBoxSyntaxHighlighting .exe.

Step3. Paste following Xml script to the RichTextBox in the UI.

	   <?xml version="1.0" encoding="utf-8" ?><html><head><title>My home page</title></head><body bgcolor="000000" text="ff0000">Hello World!</body></html>


Step4. Click the "Process" button, then the text in the RichTextBox will be changed to 
       following XML with colors.

	   <?xml version="1.0" encoding="utf-8"?>
	   <html>
			<head>
				<title>
					My home page
				</title>
			</head>
			<body bgcolor="000000" text="ff0000" >
				Hello World!
			</body>
	   </html>


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Design a class XMLViewerSettings that defines the colors used in the XmlViewer, and 
   some constants that specify the color order in the RTF.

2. Design a class XMLViewer that inherits System.Windows.Forms.RichTextBox. It is used 
   to display an Xml in a specified format. 
   
   RichTextBox uses the Rtf format to show the test. The XMLViewer will convert the Xml
   to Rtf with some formats specified in the XMLViewerSettings, and then set the Rtf 
   property to the value.
       
3. The CharacterEncoder class supplies a static(Shared) method to encode some 
   special characters in Xml and Rtf, such as '<', '>', '"', '&', ''', '\',
   '{' and '}' .

/////////////////////////////////////////////////////////////////////////////
References:

RichTextBox Class
http://msdn.microsoft.com/en-us/library/system.windows.forms.richtextbox.aspx

Rich Text Format (RTF) Specification, version 1.6
http://msdn.microsoft.com/en-us/library/aa140277(office.10).aspx


/////////////////////////////////////////////////////////////////////////////
