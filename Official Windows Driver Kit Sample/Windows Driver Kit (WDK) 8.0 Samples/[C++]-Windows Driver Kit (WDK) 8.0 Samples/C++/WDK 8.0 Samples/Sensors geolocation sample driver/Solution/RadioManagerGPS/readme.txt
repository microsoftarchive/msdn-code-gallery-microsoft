Sample Radio Manager
====================
Demonstrates how to structure a Radio Manager for use with the Windows 8 Radio Management APIs.

On Windows 8 and later, the operating system contains a set of APIs which are used as a software mechanism
to control the various radios found on the machine. The APIs work by communicating with a Radio Manager, 
which is a COM object that relays commands from the APIs to turn the radio on or off, and reports back
radio information to the APIs. This feature is designed in such a way that a separate Radio Manager is 
required for each radio media type. For example, a WLAN radio will be controlled by a different Radio
Manager than a GPS radio. If there are 2 WLAN radios and a GPS radio, the 2 WLAN radios will be controlled
by one Radio Manager, and the GPS radio will be controlled by a different Radio Manager. It is important to note
the Radio Manager MUST NOT power on/off a device, but rather it must only turn radio transmission on/off.


Sample Language Implementations
===============================
This sample is available in the following language implementations:
     C++


Installation Files
==================
install.cmd
- Installation script. Copies and registers the dll and executes SampleRM.reg

SampleRM.reg
- script to install the Sample Radio Manager into the registry


Source Files
============
SampleRM.sln 
- The Visual Studio 2010 solution file for building the Sample Radio Manager dll

sampleRM.idl
- The interface definition for the Sample Radio Manager

RadioMgr.idl
- The interface definition for a Windows Radio Manager

SampleRadioManager.h
- Header file for the functions required for a Radio Manager

SampleRadioInstance.h
- Header file for the functions required for a Radio Instance

SampleInstanceCollection.h
- Header file for the functions required for a Collection of Radio Instances

precomp.h
- Common header file

InternalInterfaces.h
- Header file for internal interface used for this sample

dllmain.cpp
- Standard dllmain

SampleRadioManager.cpp
- Implementation details for the Sample Radio Manager. Important concepts include:
	- Utilizing IMediaRadioManagerNotifySink for radio instance events
	- Adding/Removing radio instances
	- Queuing and deploying worker jobs for system events

SampleRadioInstance.cpp
- Implementation details for the Sample Radio Instance. Important concepts include:
	- Accessors & Modifiers for radio information
	- Instance change functions

SampleInstanceCollection.cpp
- Implementation details for the Sample Instance Collection. Important concepts include:
	- Radio Instance discovery and retrieval

RadioMgr_interface.cpp
- Helper source file to include the MIDL-generated files.


Prerequisites for viewing and building solution
===============================================
1. Windows 7 
2. Visual Studio 2010


To build the sample using Visual Studio 2010 (preferred method):
===============================================================
1. Open File Explorer and navigate to the directory.
2. Double-click the icon for the SampleRM.sln (solution) file to open the file in Visual Studio.
3. In the Build menu, select Build Solution. The dll will be built in the default \Debug or \Release directory


Prerequisites for installing Radio Manager
==========================================
1. Windows 8
2. The VC++ 2010 Redistributable package must be installed for this dll to load.


Installing Radio Manager
========================
1. Copy the install.cmd, SampleRM.reg and SampleRM.dll files to a directory
2. Run install.cmd elevated
