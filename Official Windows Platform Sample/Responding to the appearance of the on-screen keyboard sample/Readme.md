# Responding to the appearance of the on-screen keyboard sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- User Interface
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p class="CCE_Message">[This documentation is preliminary and is subject to change.]</p>
<p>This sample shows how to listen for and respond to the appearance of the onscreen soft keyboard. When focus is given to an element that requires text input on a device that does not have a dedicated hardware keyboard, the soft keyboard is invoked. You want
 to make sure that the soft keyboard does not cover the element into which the user is entering text. This sample demonstrates the use of the
<a href="http://msdn.microsoft.com/library/windows/apps/br242257"><b>EnsuredFocusedElementInView</b></a> property, which specifies that the application has taken steps to ensure that the input pane doesn't cover the UI element that has focus.
</p>
<p>The sample demonstrates the following scenario: </p>
<ul>
<li>Adjusting the position of onscreen elements so that they are not obscured by the soft keyboard
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
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242257"><b>Windows.UI.ViewManagement.InputPaneVisibilityEventArgs.ensuredFocusedElementInView</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 Windows Store app samples</a>
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
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File &gt; Open &gt; Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.
</li><li>Press F7 or use <b>Build &gt; Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug &gt; Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug &gt; Start Without Debugging</b>.</p>
<h2><a id="How_to_use_the_sample"></a><a id="how_to_use_the_sample"></a><a id="HOW_TO_USE_THE_SAMPLE"></a>How to use the sample</h2>
<p>Click the <b>Show keyboard behavior</b> button, which displays a colorful screen full of many elements arranged in three columns.</p>
<p>When you select the box labeled <b>Tap here to see default keyboard handling </b>
(standing in for an input field), the soft keyboard appears on the screen, moving the entire screen contents upward, but still obscuring some of the lowermost elements.</p>
<p>Close the soft keyboard through the keyboard itself.</p>
<p>Select the box labeled <b>Tap here to see custom keyboard handling</b>. Now the elements directly above the input field (&quot;Tap here...&quot;) boxes have moved upwards, but the columns to the left and right have remained stationary.</p>
</div>
