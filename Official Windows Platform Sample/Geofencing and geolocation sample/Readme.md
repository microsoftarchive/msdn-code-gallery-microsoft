# Geofencing and geolocation sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Store app
## Topics
- Location
- Geolocation
## Updated
- 10/17/2013
## Description

<h1><span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:7.5pt 0in; line-height:normal"><strong><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:20pt">&nbsp;</span></strong></p>
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0in 7.5pt; line-height:normal"><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">The Geofencing and geolocation sample demonstrates how to use the Windows.Devices.Geolocation.Geofencing namespace to check-in at
 places of interest automatically when a user's device enters the area. </span></p>
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0in 7.5pt; line-height:normal"><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Specifically, this sample covers:
</span></p>
<span style="font-family:Times New Roman; font-size:small"></span>
<ul type="disc">
<span style="font-family:Times New Roman; font-size:small"></span>
<li style="margin:0in 0in 2.25pt; color:black; line-height:140%; font-style:normal">
<span style="line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Using the geofencing APIs</span>
<span style="font-family:Times New Roman; font-size:small"></span></li><li style="margin:0in 0in 2.25pt; color:black; line-height:140%; font-style:normal">
<span style="line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Using the Location background task type</span>
<span style="font-family:Times New Roman; font-size:small"></span></li><li style="margin:0in 0in 2.25pt; color:black; line-height:140%; font-style:normal">
<span style="line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Using the Geolocation APIs</span>
<span style="font-family:Times New Roman; font-size:small"></span></li></ul>
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0in 8pt; line-height:normal"><strong><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:10.5pt">Operating system requirements</span></strong></p>
<span style="font-family:Times New Roman; font-size:small"></span>
<table border="1" cellspacing="0" cellpadding="0" width="97%" style="margin:auto auto auto 3.75pt; border:currentColor; width:97%; border-collapse:collapse">
<span style="font-family:Times New Roman"></span>
<tbody>
<tr style="">
<span style="font-family:Times New Roman"></span>
<td valign="bottom" style="background:#cccccc; padding:3.75pt; border:1pt solid #cccccc #cccccc #c8cdde">
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:3.75pt 0in; line-height:normal"><strong><span style="color:#000066; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Client</span></strong></p>
<span style="font-family:Times New Roman; font-size:small"></span></td>
<span style="font-family:Times New Roman"></span>
<td valign="top" style="background:white; padding:3.75pt; border:1pt 1pt 1pt 0px solid solid solid none #d5d5d3 #d5d5d3 #d5d5d3 #000000">
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0.75pt 0pt; line-height:normal"><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Windows&nbsp;8.1 Preview
</span></p>
<span style="font-family:Times New Roman; font-size:small"></span></td>
<span style="font-family:Times New Roman"></span>
</tr>
<span style="font-family:Times New Roman"></span>
<tr style="">
<span style="font-family:Times New Roman"></span>
<td valign="bottom" style="background:#cccccc; padding:3.75pt; border:0px 1pt 1pt none solid solid #000000 #cccccc #c8cdde">
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0in 0pt; line-height:normal"><strong><span style="color:#000066; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Server</span></strong></p>
<span style="font-family:Times New Roman; font-size:small"></span></td>
<span style="font-family:Times New Roman"></span>
<td valign="top" style="background:white; padding:3.75pt; border:0px 1pt 1pt 0px none solid solid none #000000 #d5d5d3 #d5d5d3 #000000">
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0.75pt 0pt; line-height:normal"><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">None supported [Windows Store apps only]</span></p>
<span style="font-family:Times New Roman; font-size:small"></span></td>
<span style="font-family:Times New Roman"></span>
</tr>
<span style="font-family:Times New Roman"></span>
</tbody>
</table>
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0in 8pt; line-height:normal"><strong><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:10.5pt">Build the sample</span></strong></p>
<span style="font-family:Times New Roman; font-size:small"></span>
<ol style="list-style-type:decimal; direction:ltr">
<li style="color:black; font-style:normal">
<p style="color:#000000; line-height:140%; font-style:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Install the Bing Maps SDK for Windows 8.1 Store Apps from
<a href="http://go.microsoft.com/fwlink/?LinkID=327999"><span style=""><span style="color:#0563c1; font-family:Times New Roman">here</span></span></a>.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Get a Bing Maps Developer Key from
<a href="http://go.microsoft.com/fwlink/p/?linkid=187187"><span style=""><span style="color:#0563c1; font-family:Times New Roman">here</span></span></a>. Edit Geofencing4SqSample\Mainpage.xaml and replace YOUR_BING_MAPS_KEY with your Bing Maps Developer Key.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Reserve your app name in the Windows Store developer dashboard.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Start Visual Studio&nbsp;2013 and select
<strong>File</strong> &gt; <strong>Open</strong> &gt; <strong>Project/Solution</strong>.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;Solution (.sln) file.
</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Edit Geofencing4SqSample\Package.appxmanifest and replace the Package display name (Packaging tab) with your app name.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Get your app SID from the Windows Store developer dashboard. This is generated from your package display name. Edit Geofencing4SqSample\Constants.cs and update the
 AppSid constant with your app&rsquo;s SID.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Register your app at developer.foursquare.com. Set your redirect URL to ms-app://YOUR_APP_SID.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Edit Geofencing4SqSample\Constants.cs to replace YOUR_FOURSQUARE_CLIENT_ID with the Client Id that Foursquare has provisioned for your app. Repeat this step for Geofencing4SqSample\BackgroundTasks\Constants.cs.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Select an architecture and build your project.<span style="">&nbsp;
</span>&quot;AnyCPU&quot; is not supported by the Bing</span></p>
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Maps SDK.</span></p>
</li><li style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt; font-style:normal; font-weight:normal">
<p style="color:#000000; line-height:140%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt; font-style:normal; font-weight:normal; margin-top:0in; margin-bottom:2.25pt">
<span style="color:black; line-height:140%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">Press F7 or use
<strong>Build</strong> &gt; <strong>Build Solution</strong> to build the sample. </span>
</p>
</li></ol>
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0in 8pt; line-height:normal"><strong><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:10.5pt">Run the sample</span></strong></p>
<span style="font-family:Times New Roman; font-size:small"></span>
<p style="margin:0in 0in 7.5pt; line-height:normal"><span style="color:black; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:9.5pt">To debug the app and then run it, press F5 or use
<strong>Debug</strong> &gt; <strong>Start Debugging</strong>. To run the app without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>. </span></p>
<span style="font-family:Times New Roman; font-size:small"></span></h1>
<div id="_mcePaste" class="mcePaste" style="left:-10000px; top:0px; width:1px; height:1px; overflow:hidden">
</div>
