# Media Play To sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Audio and video
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates the Play To API. </p>
<p>The Play To sample demonstrates how you can expand your media application and add the capability to stream video, audio, and images to other devices on your local network. For example, a user that is watching a video in your application can stream that video
 to their TV for everyone in the room to view. This sample demonstrates how to stream a media file and how to stream pictures as a slide show.</p>
<p>You can use Windows Media Player as a Play To target device. To do this, open Windows Media Player on a separate computer, and then expand the Stream menu and select &quot;Allow remote control of my Player&quot;. You can then add Windows Media Player on the other
 computer as a Digital media renderer in the Devices and Printers control panel.</p>
<p>If you use the code from the C# version of this sample in your own app, please be aware of the following known issues:</p>
<table>
<tbody>
<tr>
<th>Known issue</th>
<th>Workaround</th>
</tr>
<tr>
<td>Callback handlers for the <a href="http://msdn.microsoft.com/library/windows/apps/br242926">
<b>MediaElement</b></a> element cannot reference the UI thread directly. If you attempt to reference a
<b>MediaElement</b> element directly, you will encounter an RPC_E_WRONG_THREAD exception for invoking a call to the element on the wrong thread.</td>
<td>Implement a dispatcher to handle RPC calls for the <a href="http://msdn.microsoft.com/library/windows/apps/br242926">
<b>MediaElement</b></a> callback handler as seen in this example from ScenarioOutput1.xaml.cs.
<div class="code"><span>
<table>
<tbody>
<tr>
<th>C#</th>
</tr>
<tr>
<td>
<pre>void receiver_VolumeChangeRequested(Windows.Media.PlayTo.PlayToReceiver sender,
                                    VolumeChangeRequestedEventArgs args)
{
    PlayToReceiver.Dispatcher.Helper.BeginInvoke(() =&gt;
    {
        if (dmrVideo != null)
        {
            dmrVideo.Volume = args.Volume;
        }
    });
}
</pre>
</td>
</tr>
</tbody>
</table>
</span></div>
</td>
</tr>
</tbody>
</table>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Roadmaps</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465134">Adding multimedia</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh767284">Designing UX for apps</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for apps using C# and Visual Basic</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700360">Roadmap for apps using C&#43;&#43;</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465037">Roadmap for apps using JavaScript</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Tasks</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465184">Quickstart: Using Play To in applications (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465191">Quickstart: Using Play To in applications (C#)</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207025"><b>Windows.Media.PlayTo</b></a>
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
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</div>
