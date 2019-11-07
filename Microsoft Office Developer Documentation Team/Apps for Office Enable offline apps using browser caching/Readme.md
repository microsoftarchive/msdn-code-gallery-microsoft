# Apps for Office: Enable offline apps using browser caching
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Word 2013
- apps for Office
- PowerPoint 2013
## Topics
- data and storage
## Updated
- 03/08/2013
## Description

<p><span style="font-size:small">The CodeSample_OfflineCalculatorApp.appcache file contains the AppCache manifest. The AppCache manifest declares that the following resources will be stored locally and referenced if the browser is in an offline state:</span></p>
<ul>
<li><span style="font-size:small">CodeSample_OfflineCalculator.html</span> </li><li><span style="font-size:small">App.css</span> </li><li><span style="font-size:small">Office.css</span> </li><li><span style="font-size:small">CodeSample_OfflineCalculatorApp.js</span> </li><li><span style="font-size:small">Office.js</span> </li><li><span style="font-size:small">MicrosoftAjax.js</span> </li><li><span style="font-size:small">jquery-1.7.1.js</span> </li><li><span style="font-size:small">toast.js</span> </li></ul>
<p><span style="font-size:small">The manifest file is referenced in the <em>manifest
</em>attribute of the <strong>html</strong> tag of the CodeSample_OfflineCalculatorApp.html file.</span></p>
<p><span style="font-size:small">In addition, the sample demonstrates how to get data from a document, how to store and retrieve data from HTML5 web storage (localStorage), and how to insert data back into the document. It also demonstrates best practices for
 surfacing errors to the user through using dynamically generated content.</span></p>
<p><span style="font-size:small">For more information about the HTML5 Application Cache API, see
<a href="http://msdn.microsoft.com/en-us/library/ie/hh673545(v=vs.85).aspx">Application Cache API (&ldquo;AppCache&rdquo;)</a>. For more information about HTML5 Web Storage, see
<a href="http://msdn.microsoft.com/en-us/library/cc197062(VS.85).aspx">Introduction to Web Storage</a>. For more information about working with data using the JavaScript APIs for apps for Office, see the
<a href="http://msdn.microsoft.com/library/b27e70c3-d87d-4d27-85e0-103996273298">
JavaScript API for Office</a>.</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">Word 2013 or PowerPoint 2013.</span> </li><li><span style="font-size:small">Visual Studio 2012, apps for Office project templates.</span>
</li><li><span style="font-size:small">Basic familiarity with HTML5, JavaScript, CSS3, and jQuery.</span>
</li></ul>
<p><span style="font-size:small"><strong>Note</strong>: To test the offline capabilities, Windows 8 and Internet Explorer 10 Preview are also required.</span></p>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The <em>Apps for Office: Enable offline apps using browser caching</em> sample app contains the following important files:</span></p>
<ul>
<li><span style="font-size:small">CodeSample_OfflineCalculator project, including:</span>
<ul>
<li><span style="font-size:small">CodeSample_OfflineCalculatorApp.xml manifest</span>
</li><li><span style="font-size:small">CodeSample_OfflineCalculatorApp.appcache manifest</span>
</li><li><span style="font-size:small">CodeSample_OfflineCalculatorApp.js file</span> </li><li><span style="font-size:small">CodeSample_OfflineCalculatorApp.html file</span>
</li><li><span style="font-size:small">toast.js</span> </li><li><span style="font-size:small">App.css file</span> </li></ul>
</li></ul>
<h1>Configure the sample</h1>
<p><span style="font-size:small">No configuration is necessary to run the sample and operate the calculator.</span></p>
<p><span style="font-size:small">To test the offline capabilities of the app, follow these steps:</span></p>
<ol>
<li><span style="font-size:small">Download and insert the app into Word 2013 or PowerPoint 2013.</span>
</li><li><span style="font-size:small">Disconnect the Windows 8 computer from all networks.</span>
</li><li><span style="font-size:small">Open Word 2013 or PowerPoint 2013&nbsp;and insert the app again.</span>
</li></ol>
<h1>Build the sample</h1>
<p><span style="font-size:small">Choose the F5 key in Visual Studio 2012 to build and deploy the app.</span></p>
<h1>Run and test the sample</h1>
<ol>
<li><span style="font-size:small">Choose the F5 key to build and deploy the app.</span>
</li><li><span style="font-size:small">Use the app&rsquo;s interface to get and set data from the Word document, perform number computation tasks, and store data.</span>
</li></ol>
<h1>Troubleshooting</h1>
<p><span style="font-size:small">If the app fails to install, ensure that the <strong>
SourceLocation</strong> element in the CodeSample_OfflineCalculator.xml file has the correct URL value for the
<em>DefaultValue </em>attribute.</span></p>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/ie/hh673545(v=vs.85).aspx">Application Cache API (&ldquo;AppCache&rdquo;)</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/cc197062(VS.85).aspx">Introduction to Web Storage</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/library/f85ad02c-64f0-4b73-87f6-7f521b3afd69">Document.getSelectedDataAsync function</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/library/998f38dc-83bd-4659-a759-4758c632a6ef">Document.setSelectedDataAsync function</a></span>
</li></ul>
