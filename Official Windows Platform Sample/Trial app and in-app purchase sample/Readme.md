# Trial app and in-app purchase sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Store
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to perform in-app feature or product purchases operations and use the licensing API provided by the Windows Store to determine the license status of an app or a feature that is enabled by an in-app purchase.
</p>
<p>The sample app shows the different ways the licensing API can be used to perform the following tasks for an app.</p>
<ul>
<li>Check the current license status of an app </li><li>Check the expiration date of a trial period </li><li>Check if an app's feature has been purchased through an in-app purchase </li><li>Perform an in-app purchase to buy the app </li><li>Perform an in-app purchase to buy an app feature or product </li><li>Perform an in-app purchase transaction using the Windows Store commerce platform.
</li><li>Perform an in-app purchase to buy an app feature or product from a large purchase catalog.
</li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/?LinkID=303894">Windows&nbsp;8.1 Feature Guide for Developers: Store updates</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh694065">How to create a trial version of your app</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh694067">How to support in-app purchases from your app</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br225197"><b>Windows.ApplicationModel.Store</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br225197">Windows.ApplicationModel.Store</a>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1 </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p>This sample is designed for Windows&nbsp;8.1. It will not build or run on earlier versions of Windows.</p>
<p>You can build and deploy both the retail and the debug versions of this app; however, only the retail version will pass the Windows App Certification Kit.</p>
<h2>Run the sample</h2>
<p>To run this sample app after you have built it successfully, deploy it to your computer. You can do this in Visual Studio&nbsp;2013. From
</p>
<ol>
<li>In the <b>Solution Explorer</b>, select the <b>Store </b>project. </li><li>From the <b>Build </b>menu, select <b>Build Solution</b>. </li><li>If the solution builds without errors or warnings, from the <b>Build</b> menu, select
<b>Deploy Solution</b>. </li><li>You should see the sample app in your <b>Start</b> screen. To run the app, click its tile in the
<b>Start</b> screen. </li></ol>
<p></p>
<p>This app simulates the license server in the Windows Store by using configuration files. Some of the examples illustrated in this app do not work more than one time. As such, it might be necessary to restart the program to see those examples work again.</p>
</div>
