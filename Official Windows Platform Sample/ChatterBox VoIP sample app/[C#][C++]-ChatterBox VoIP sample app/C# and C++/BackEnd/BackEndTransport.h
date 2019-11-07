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
#include "windows.h"

namespace PhoneVoIPApp
{
    namespace BackEnd
    {
        namespace TransportMessageType
        {
            enum Value
            {
                Audio = 0,
                Video = 1
            };
        }

        public delegate void MessageReceivedEventHandler(Windows::Storage::Streams::IBuffer ^pBuffer, UINT64 hnsPresentationTime, UINT64 hnsSampleDuration);

        /// <summary>
        /// This is an abstraction of a network transport class
        /// which does not actually send data over the network.
        /// </summary>
        public ref class BackEndTransport sealed
        {
        public:
            // Constructor
            BackEndTransport();

            // Destructor
            virtual ~BackEndTransport();

            void WriteAudio(BYTE* bytes, int byteCount);
            void WriteVideo(BYTE* bytes, int byteCount, UINT64 hnsPresentationTime, UINT64 hnsSampleDuration);

            event MessageReceivedEventHandler^ AudioMessageReceived;
            event MessageReceivedEventHandler^ VideoMessageReceived;

        private:
            void Write(BYTE* bytes, int byteCount, TransportMessageType::Value dataType, UINT64 hnsPresentationTime, UINT64 hnsSampleDurationTime);
        };
    }
}
