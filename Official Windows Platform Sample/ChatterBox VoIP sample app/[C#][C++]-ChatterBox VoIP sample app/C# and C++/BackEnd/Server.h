/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#pragma once
#include <windows.h>
#include "Globals.h"

namespace PhoneVoIPApp
{
    namespace BackEnd
    {
        namespace OutOfProcess
        {
            // A remotely activatable class that is used by the UI process and managed code within
            // the VoIP background process to get access to native objects that exist in the VoIP background process.
            public ref class Server sealed
            {
            public:
                // Constructor
                Server()
                {
                }

                // Destructor
                virtual ~Server()
                {
                }

                // Called by the UI process to get the call controller object
                property CallController^ CallController
                {
                    PhoneVoIPApp::BackEnd::CallController^ get()
                    {
                        return Globals::Instance->CallController;
                    };
                }

                // Add methods and properties to get other objects here, as required.
            };
        }
    }
}
