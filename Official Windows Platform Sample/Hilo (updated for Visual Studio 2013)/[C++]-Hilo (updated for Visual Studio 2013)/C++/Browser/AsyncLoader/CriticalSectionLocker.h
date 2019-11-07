//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

namespace Hilo 
{ 
    namespace AsyncLoader
    {
        //
        // class CriticalSectionLocker
        //
        // Helper class to ensure:
        // - EnterCriticalSection is called if a CRITICAL_SECTION pointer is valid.
        // - LeaveCriticalSection is always called at the end of the scope if 
        //   EnterCriticalSection() was called.
        //
        class CriticalSectionLocker
        {
            CRITICAL_SECTION* m_criticalSection;
        public:
            CriticalSectionLocker(CRITICAL_SECTION* cs)
            {
                m_criticalSection = cs;
                if ( nullptr != m_criticalSection )
                {
                    EnterCriticalSection(m_criticalSection);
                }
            }
            ~CriticalSectionLocker()
            {
                if ( nullptr != m_criticalSection )
                {
                    LeaveCriticalSection(m_criticalSection);
                    m_criticalSection = nullptr;
                }
            }
        };
    }
}
