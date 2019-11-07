# Build Progress Bar - VS 2013
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Visual Studio 2013
## Topics
- Extensibility
## Updated
- 05/29/2014
## Description

<div id="longDesc">
<h1><span>Introduction</span></h1>
<p>A Visual Studio Package which provides a new tool window called &quot;Build Progress&quot; which displays a WPF ProgressBar indicating percentage completion of the current solution build.</p>
<h1><span>Getting Started</span></h1>
<p>To build and run this sample, you must have Visual Studio 2013 installed, as well as the Visual Studio SDK. Unzip the BuildProgressBar.zip file into your Visual Studio Projects directory (My Documents\Visual Studio 2013\Projects) and open the BuildProgressBar.sln
 solution.</p>
<h1><span>Building the Sample</span></h1>
<p>To build the sample, make sure the BuildProgressBar solution is open and then use the Build | Build Solution menu command.</p>
<h1><span>Running the Sample</span></h1>
<p>To run the sample, hit F5 or choose the Debug | Start Debugging menu command. A new instance of Visual Studio will launch under the experimental hive. The experimental hive is a special &quot;sandbox&quot; instance of Visual Studio that allows you to develop and test
 packages without affecting your main instance of Visual Studio. Once loaded, choose the View | Other Windows | Build Progress menu command. A new tool window called &quot;Build Progress&quot; will open, displaying a WPF ProgressBar control. You can move, resize, or
 dock this tool window however you like. Next, either open an existing buildable solution or create a new one by using the File | New | Project menu command. Once the solution has loaded, build the solution by running the Build | Build Solution menu command.
 You should see the progress bar's value and text change as each project in the solution is built.</p>
<h1><span>Source Code Overview</span></h1>
<p>The source code in this sample demonstrates several techniques you can use to write your own packages:</p>
<ul>
<li>How to add a menu command to the View | Other Windows menu group. </li><li>How to display WPF content on a tool window </li><li>How to monitor solution load/unload events </li><li>How to monitor solution build events </li><li>How to check and monitor the value of a Visual Studio Shell property (VisualEffectsAllowed)
</li></ul>
<p>Our Visual Studio Package implements the interfaces IVsShellPropertyEvents, IVsSolutionEvents, and IVsUpdateSolutionEvents2 so that it can receive notification of shell property changes, solution load/unload events, and solution build events. However, in
 order to receive calls to these interface methods, it must advise the appropriate service providers that we want to be notified, as demonstrated in the ProgressBarPackage's Initialize method.</p>
<p>We monitor the value of the VisualEffectsAllowed shell property in order to modify how the progress bar is displayed. We also monitor solution load/unload events to keep track of how many projects are currently loaded in the solution. Lastly, we monitor
 solution build events in order to update the value and text of the progress bar.</p>
<h1><span>Project Files</span></h1>
<ul>
<li>BuildProgressBar.vsct - Defines layout and type of commands in the package, namely the Build Progress menu command.
</li><li>BuildProgressToolWindow.cs - Implements the tool window functionality, and owns an instance of the ProgressBarControl.
</li><li>ProgressBarControl.xaml - Defines the XAML for the ProgressBarControl, which is the content hosted on the tool window.
</li><li>ProgressBarControl.xaml.cs - Defines the code-behind for the ProgressBarControl. This gives us greater control over the behavior of the ProgressBarControl.
</li><li>ProgressBarPackage.cs - Implements the Visual Studio Package, where we monitor events.&nbsp;
</li></ul>
</div>
