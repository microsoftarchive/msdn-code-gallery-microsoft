# Visual Studio Debug Engine Sample
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Visual Studio 2012
- Visual Studio 2013
- Debugger Engine
## Topics
- Debugger Extensibility
## Updated
- 10/14/2014
## Description

<h1>Introduction</h1>
<p><em>This sample shows how to build a Debug Engine for Visual Studio. A Debug Engine is the back end of the Visual Studio debugger - it provides execution control (ex: breakpoints, stepping, exceptions) and inspection (ex: watch window, modules, etc) for
 some new target environment.</em></p>
<p><em>Debug engines implement IDebugEngine2, IDebugProgram2, IDebugThread2 and many more interfaces. This family of interfaces is known as AD7 (Active Debugging 7) and has been arround and sample since Visual Studio 2002. This sample shows how to implement
 many of the AD7 interfaces.<br>
</em></p>
<p><em>Note that a Debug Engine is not the only way to provide a new back end to Visual Studio. A Debug Engine is the recommended technique when (1) interop debugging with the Microsoft Debug Engine isn't an&nbsp;important scenario -or- isn't something supported
 by the underlying target environment -and- (2) there are not significant assets from the Microsoft Debug Engine that you would like to reuse; one example where such reuse is often possible is when trying to provide native debugging for binaries compiled with
 the Microsoft C/C&#43;&#43; compilers in a new execution environment. If either of these cases apply to your scenario, you want to build an component which plugins into the Microsoft Debug Engine (codename Concord).<br>
</em></p>
<h1><span>Building the Sample</span></h1>
<p><em>This sample builds in Visual Studio 2013. It requires the Visual Studio 2013 SDK. Detailed instructions are found in the Walk through documents found in the sample.</em></p>
<h1><span>Top level files and directories</span></h1>
<ul>
<li><em><em>WalkThrough1.docx - provides a basic walk through of the sample, explaining how to build, how to get started, etc.</em></em>
</li><li><em>WalkThrough2.docx - provides an overview of how modules and threads work</em>
</li><li><em>WalkThrough3.docx - describes how breakpoints and stopping events work in the debugger</em>
</li><li><em>ProjectLauncher - This is a Visual Studio package which (1) integrates with the VSIX related targets to provide VSIX deployment of the sample -and- (2) adds a new menu command (Tools-&gt;ProjectLauncher) to launch a project under the sample debug engine.</em>
</li><li><em>Microsoft.VisualStudio.Debugger.SampleEngine - this is the heart of the sample. This provides the implementation of the AD7 interfaces for the sample.</em>
</li><li><em>Microsoft.VisualStudio.Debugger.SampleEngineWorker - this is an example backend for the sample. When producing your real debug engine, this code will be thrown out and replaced with your own debugger.</em>
</li></ul>
