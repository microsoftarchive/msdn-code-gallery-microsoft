// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// printversion.cs
// compile with: /unsafe
using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Give this assembly a version number:
[assembly:AssemblyVersion("4.3.2.1")]

public class Win32Imports 
{
	[DllImport("version.dll")]
	public static extern bool GetFileVersionInfo (string sFileName,
		int handle, int size, byte[] infoBuffer);
	[DllImport("version.dll")]
	public static extern int GetFileVersionInfoSize (string sFileName,
		out int handle);
   
	// The 3rd parameter - "out string pValue" - is automatically
	// marshaled from Ansi to Unicode:
	[DllImport("version.dll")]
	unsafe public static extern bool VerQueryValue (byte[] pBlock,
		string pSubBlock, out string pValue, out uint len);
	// This VerQueryValue overload is marked with 'unsafe' because 
	// it uses a short*:
	[DllImport("version.dll")]
	unsafe public static extern bool VerQueryValue (byte[] pBlock,
		string pSubBlock, out short *pValue, out uint len);
}

public class C 
{
	// Main is marked with 'unsafe' because it uses pointers:
	unsafe public static int Main () 
	{
		try 
		{
			int handle = 0;
			// Figure out how much version info there is:
			int size =
				Win32Imports.GetFileVersionInfoSize("printversion.exe",
				out handle);

			if (size == 0) return -1;

			byte[] buffer = new byte[size];

			if (!Win32Imports.GetFileVersionInfo("printversion.exe", handle, size, buffer))
			{
				Console.WriteLine("Failed to query file version information.");
				return 1;
			}

			short *subBlock = null;
			uint len = 0;
			// Get the locale info from the version info:
			if (!Win32Imports.VerQueryValue (buffer, @"\VarFileInfo\Translation", out subBlock, out len))
			{
				Console.WriteLine("Failed to query version information.");
				return 1;
			}

			string spv = @"\StringFileInfo\" + subBlock[0].ToString("X4") + subBlock[1].ToString("X4") + @"\ProductVersion";

			byte *pVersion = null;
			// Get the ProductVersion value for this program:
			string versionInfo;
			
			if (!Win32Imports.VerQueryValue (buffer, spv, out versionInfo, out len))
			{
				Console.WriteLine("Failed to query version information.");
				return 1;
			}

			Console.WriteLine ("ProductVersion == {0}", versionInfo);
		}
		catch (Exception e) 
		{
			Console.WriteLine ("Caught unexpected exception " + e.Message);
		}
      
		return 0;
	}
}

