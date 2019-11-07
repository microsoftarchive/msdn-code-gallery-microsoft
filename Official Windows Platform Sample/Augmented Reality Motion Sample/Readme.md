# Augmented Reality Motion Sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Phone 7.5
## Topics
- Sensors
## Updated
- 03/05/2013
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>This sample shows how to use the camera and the Motion API to create an augmented reality app. Windows&nbsp;Phone supports multiple sensors including a compass, which tracks the phone’s position relative to the earth’s magnetic field, and the accelerometer, which
 measures the force acting on each of the phone’s three axes, including the force of gravity. The Windows&nbsp;Phone Motion API takes the data provided by these sensors and performs a set of calculations to determine the phone’s current orientation in 3-D space.
 This sample app uses this 3-D orientation to map points on the device’s screen to points in 3-D space. This is done using XNA Viewport APIs in a manner similar to the way that games can determine if the point you click on the 2-D screen correlates to an object
 in a 3-D environment. For a walkthrough of how to create this app, see <a href="http://go.microsoft.com/fwlink/?LinkId=219427">
How to: Use the Combined Motion API for Windows Phone</a>.</p>
<p>You need to install Windows&nbsp;Phone&nbsp;SDK&nbsp;7.1 to run this sample. To get started, go to the
<a href="http://go.microsoft.com/fwlink/?LinkID=259204">Windows Phone Dev Center</a>.</p>
<div class="alert">
<table width="100%" cellspacing="0" cellpadding="0">
<tbody>
<tr>
<th align="left"><b>Caution:</b> </th>
</tr>
<tr>
<td>
<p>The Motion API requires that a compass sensor be present on the device in order to function properly. Running this sample on Windows Phone Emulator or a device without a compass will cause it to fail gracefully, presenting a message to the user that the
 Motion API is not supported.</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
</div>
