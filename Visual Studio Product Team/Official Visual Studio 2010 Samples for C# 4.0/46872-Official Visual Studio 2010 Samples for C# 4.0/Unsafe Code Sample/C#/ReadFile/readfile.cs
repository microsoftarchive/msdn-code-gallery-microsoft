// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// readfile.cs
// compile with: /unsafe
// arguments: readfile.txt

// Use the program to read and display a text file.
using System;
using System.Runtime.InteropServices;
using System.Text;

class FileReader
{
	const uint GENERIC_READ = 0x80000000;
	const uint OPEN_EXISTING = 3;
	IntPtr handle;

	[DllImport("kernel32", SetLastError=true)]
	static extern unsafe IntPtr CreateFile(
		string FileName,				// file name
		uint DesiredAccess,				// access mode
		uint ShareMode,					// share mode
		uint SecurityAttributes,		// Security Attributes
		uint CreationDisposition,		// how to create
		uint FlagsAndAttributes,		// file attributes
		int hTemplateFile				// handle to template file
		);



	[DllImport("kernel32", SetLastError=true)]
	static extern unsafe bool ReadFile(
		IntPtr hFile,					// handle to file
		void* pBuffer,				// data buffer
		int NumberOfBytesToRead,	// number of bytes to read
		int* pNumberOfBytesRead,		// number of bytes read
		int Overlapped				// overlapped buffer
		);


	[DllImport("kernel32", SetLastError=true)]
	static extern unsafe bool CloseHandle(
		IntPtr hObject   // handle to object
		);
	
	public bool Open(string FileName)
	{
		// open the existing file for reading
		
		handle = CreateFile(
			FileName,
			GENERIC_READ,
			0, 
			0, 
			OPEN_EXISTING,
			0,
			0);
	
		if (handle != IntPtr.Zero)
			return true;
		else
			return false;
	}

	public unsafe int Read(byte[] buffer, int index, int count) 
	{
		int n = 0;
		fixed (byte* p = buffer) 
		{
			if (!ReadFile(handle, p + index, count, &n, 0))
				return 0;
		}
		return n;
	}

	public bool Close()
	{
		//close file handle
		return CloseHandle(handle);
	}
}

class Test
{
	public static int Main(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("Usage : ReadFile <FileName>");
			return 1;
		}
		
		if (! System.IO.File.Exists(args[0]))
		{
			Console.WriteLine("File " + args[0] + " not found."); 
			return 1;
		}

		byte[] buffer = new byte[128];
		FileReader fr = new FileReader();
		
		if (fr.Open(args[0]))
		{
			
			// We are assuming that we are reading an ASCII file
			ASCIIEncoding Encoding = new ASCIIEncoding();
			
			int bytesRead;
			do 
			{
				bytesRead = fr.Read(buffer, 0, buffer.Length);
				string content = Encoding.GetString(buffer,0,bytesRead);
				Console.Write("{0}", content);
			}
			while ( bytesRead > 0);
			
			fr.Close();
			return 0;
		}
		else
		{
			Console.WriteLine("Failed to open requested file");
			return 1;
		}
	}
}
