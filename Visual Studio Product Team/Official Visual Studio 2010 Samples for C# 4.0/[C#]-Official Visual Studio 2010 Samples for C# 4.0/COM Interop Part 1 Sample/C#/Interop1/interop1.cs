// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// interop1.cs
// Build with "csc /R:QuartzTypeLib.dll interop1.cs"
using System;
class MainClass 
{ 
	/************************************************************ 
	Abstract: This method collects the filename of an AVI to show
	then creates an instance of the Quartz COM object.
	To show the AVI, the program calls RenderFile and Run on 
	IMediaControl. Quartz uses its own thread and window to display 
	the AVI.The main thread blocks on a ReadLine until the user presses
	Enter.
		Input Parameters: the location of the avi file it is going to display
		Returns: void
	**************************************************************/ 
	public static void Main(string[] args) 
	{ 
		// Check to see if the user passed in a filename 
		if (args.Length != 1)
		{ 
			DisplayUsage();
			return;
		} 

		if (args[0] == "/?")
		{ 
			DisplayUsage();
			return;
		} 

		string filename = args[0]; 

		// Check to see if the file exists
		if (!System.IO.File.Exists(filename))
		{
			Console.WriteLine("File " + filename + " not found.");
			DisplayUsage();
			return;
		}
    
		// Create instance of Quartz
		// (Calls CoCreateInstance(E436EBB3-524F-11CE-9F53-0020AF0BA770,
		// NULL, CLSCTX_ALL, IID_IUnknown, &graphManager).): 

		try
		{
			QuartzTypeLib.FilgraphManager graphManager = 
				new QuartzTypeLib.FilgraphManager();

			// QueryInterface for the IMediaControl interface:
			QuartzTypeLib.IMediaControl mc =
				(QuartzTypeLib.IMediaControl)graphManager;

			// Call some methods on a COM interface 
			// Pass in file to RenderFile method on COM object. 
			mc.RenderFile(filename);

			// Show file. 
			mc.Run();
		}
		catch(Exception ex)
		{
			Console.WriteLine("Unexpected COM exception: " + ex.Message);
		}

		// Wait for completion.
		Console.WriteLine("Press Enter to continue."); 
		Console.ReadLine();
	}
    
	private static void DisplayUsage() 
	{ 
		// User did not provide enough parameters. 
		// Display usage: 
		Console.WriteLine("VideoPlayer: Plays AVI files."); 
		Console.WriteLine("Usage: VIDEOPLAYER.EXE filename"); 
		Console.WriteLine("where filename is the full path and"); 
		Console.WriteLine("file name of the AVI to display."); 
	} 
}

