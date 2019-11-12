# App tiles and badges sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Controls
- universal app
## Updated
- 05/28/2014
## Description

<div id="mainSection">
<p><img src="111788-image.png" alt="" align="middle"></p>
<p>This sample shows you how to use an app tile, which is the representation and launch point for your app on the Start screen. The sample also shows you how to use a badge with that tile, which lets app relay status information to the user when the app is
 not running&mdash;the number of unread mails is the generic example.</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info
 about how to build apps that target Windows and Windows Phone with Visual Studio, see
<a href="http://msdn.microsoft.com/library/windows/apps/dn609832">Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>An app tile can be a static image, but it can also be a <em>live tile</em>, which can show the user updated news, photos, events, or other content related to that app. Notification updates, based on a Windows-supplied set of templates, provide that material
 (which can come from either a local, network, or web source), keeping the tile content fresh.</p>
<p>The sample demonstrates the following scenarios:</p>
<ul>
<li>Sending new text content to the tile </li><li>Using a local image (from the app's package or local storage) in the tile </li><li>Using a web image (http or https protocol) in the tile </li><li>Updating a badge on the tile </li><li>Updating a tile through a push notification, using Azure Mobile Services </li><li>Exercising the full catalog of tile templates </li><li>Enabling the notification queue, either regardless of tile size or selectively for specific tile sizes, and using tags to selectively replace notifications in the queue.
</li><li>Setting an expiration time for a notification so that it is removed from the tile
</li><li>Accessing images through different protocols and by using a pre-defined root path (baseUri)
</li><li>Globalization, including localization, scaling of images, and accessibility </li><li>Optimizing images before you send your notification, including scaling and cropping (JavaScript only)
</li><li>Avoiding duplication of content in tiles that show multiple stories </li></ul>
<p>&nbsp;</p>
<p>Most of the functionality in this sample requires that the tile can receive notifications. Tile notifications can be disabled by a user for a single app or for all apps, or by a system administrator using group policy.</p>
<p>This sample code has declared the &quot;Internet (Client)&quot; capability in its app's manifest. You must do this in your own code whenever you plan to use non-local content. In the Microsoft Visual Studio&nbsp;2013 manifest editor, you can find this option under
 the <strong>Capabilities</strong> tab.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the
<a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><strong>Overviews</strong> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh779724">Tiles overview</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh779719">Badges overview</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh781199">Using the notification queue (overview)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761491">The tile template catalog</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465403">Guidelines and checklist for tiles and badges</a>
</dt><dt><strong>JavaScript/HTML tutorials</strong> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465439">Quickstart: Sending a tile update</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700418">Quickstart: Sending a badge update</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465429">How to use the notification queue</a>
</dt><dt><strong>C#/C&#43;&#43;/XAML tutorials</strong> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868253">Quickstart: Sending a tile update</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868225">Quickstart: Sending a badge update</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868234">How to use the notification queue</a>
</dt><dt><strong>Reference pages</strong> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208661"><strong>Windows.UI.Notifications namespace</strong></a>
</dt></dl>
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
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p>&nbsp;</p>
<ol>
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <strong>File</strong> &gt;
<strong>Open</strong> &gt; <strong>Project/Solution</strong>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <strong>Tiles.Windows</strong> in <strong>Solution Explorer</strong>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <strong>Build</strong> &gt; <strong>Build Solution</strong>, or use
<strong>Build</strong> &gt; <strong>Build Tiles.Windows</strong>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <strong>Tiles.WindowsPhone</strong> in <strong>Solution Explorer</strong>.
</li><li>Press Ctrl&#43;Shift&#43;B or use <strong>Build</strong> &gt; <strong>Build Solution</strong> or use
<strong>Build</strong> &gt; <strong>Build Tiles.WindowsPhone</strong>. </li></ol>
</li></ul>
</li></ol>
<p>&nbsp;</p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><strong>Deploying the sample</strong></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <strong>Tiles.Windows</strong> in <strong>Solution Explorer</strong>. </li><li>Use <strong>Build</strong> &gt; <strong>Deploy Solution</strong> or <strong>Build</strong> &gt;
<strong>Deploy Tiles.Windows</strong>. </li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <strong>Tiles.WindowsPhone</strong> in <strong>Solution Explorer</strong>.
</li><li>Use <strong>Build</strong> &gt; <strong>Deploy Solution</strong> or <strong>Build</strong> &gt;
<strong>Deploy Tiles.WindowsPhone</strong>. </li></ol>
</li></ul>
<p><strong>Deploying and running the sample</strong></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <strong>Tiles.Windows</strong> in <strong>Solution Explorer</strong> and select
<strong>Set as StartUp Project</strong>. </li><li>To debug the sample and then run it, press F5 or use <strong>Debug</strong> &gt;
<strong>Start Debugging</strong>. To run the sample without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <strong>Tiles.WindowsPhone</strong> in <strong>Solution Explorer</strong> and select
<strong>Set as StartUp Project</strong>. </li><li>To debug the sample and then run it, press F5 or use <strong>Debug</strong> &gt;
<strong>Start Debugging</strong>. To run the sample without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>. </li><li>Give it several seconds to launch in the emulator (it takes over the full screen, so if you're still seeing your Start screen tiles, the sample hasn't launched yet), after which you can find the sample in the Apps list. Add the tile's sample to the Start
 screen so that you can see the result of the action that you've taken in the sample. A tile must be pinned to the Start screen to receive notifications.
</li></ol>
</li></ul>
<h2><a id="How_to_use_the_sample"></a><a id="how_to_use_the_sample"></a><a id="HOW_TO_USE_THE_SAMPLE"></a>How to use the sample</h2>
<p>After you send a tile or badge notification from one of the scenarios, switch to your Start screen to see the notification appear on the sample's app tile. Click the tile again to return to the sample.</p>
<p>A tile can display only those notifications that contain a binding for the size that the tile currently uses. Otherwise, nothing happens. Therefore, you might have to resize a tile to see the notification that you've sent. For instance, in scenario 6, which
 allows you to exercise all of the tile templates, your tile on the Start screen must be wide if you are sending a wide tile template. The sample app shows you the notification's XML payload, including the tile size or sizes that the payload contains. To resize
 your tile on the Start screen, right-click (or press and hold) the tile to be presented with the option to resize. Choose medium, wide, or large as needed. Note that the small size is not a live tile and Windows Phone does not support the large tile size.</p>
<p>Note that scenario 5 (&quot;Send push notifications from Mobile Services&quot;) is strictly informational and performs no action on the tile. However, in the Windows Phone emulator, you can experiment with push notifications through its
<strong>Additional Tools</strong> flyout.</p>
</div>
