//Copyright (C) Microsoft Corporation.  All rights reserved.

// pinvoke.cs
// compile with: /addmodule:logfont.netmodule
// csc pinvoke.cs /addmodule:logfont.netmodule
using System;
using System.Runtime.InteropServices;

class PlatformInvokeTest
{   
	[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
	public static extern IntPtr CreateFontIndirect(
		[In, MarshalAs(UnmanagedType.LPStruct)]
		LOGFONT lplf   // characteristics
		);

	[DllImport("gdi32.dll")]
	public static extern bool DeleteObject(
		IntPtr handle
		);

	public static void Main() 
	{
		LOGFONT lf = new LOGFONT();
		lf.lfHeight = 9;
		lf.lfFaceName = "Arial";
		IntPtr handle = CreateFontIndirect(lf);

		if (IntPtr.Zero == handle)
		{
			Console.WriteLine("Can't creates a logical font.");
		}
		else
		{
			
			if (IntPtr.Size == 4)
				Console.WriteLine("{0:X}", handle.ToInt32());
			else
				Console.WriteLine("{0:X}", handle.ToInt64());		

			// delete the logical font created.
			if (!DeleteObject(handle))
				Console.WriteLine("Can't delete the logical font");

		}
		
	}
}

