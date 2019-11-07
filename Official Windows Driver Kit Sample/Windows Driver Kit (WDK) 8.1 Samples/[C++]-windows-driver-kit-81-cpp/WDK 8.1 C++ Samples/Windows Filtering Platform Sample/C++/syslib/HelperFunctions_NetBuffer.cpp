////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      HelperFunctions_NetBuffer.cpp
//
//   Abstract:
//      This module contains kernel helper functions that assist with NET_BUFFER and NET_BUFFER_LIST.
//
//   Naming Convention:
//
//      <Module><Object><Action><Modifier>
//  
//      i.e.
//
//       KrnlHlprNBLGetRequiredRefCount
//
//       <Module>
//          KrnlHlpr           -       Function is located in syslib\ and applies to kernel mode.
//       <Object>
//          NBL                -       Function pertains to NET_BUFFER_LIST objects.
//       <Action>
//          {
//            Get              -       Function retrieves data.
//          }
//       <Modifier>
//          {
//            RequiredRefCount -       Function returns a refCount for the NBL / NBL chain.
//          }
//
//   Private Functions:
//
//   Public Functions:
//      KrnlHlprNBLGetRequiredRefCount(),
//
//   Author:
//      Dusty Harper      (DHarper)
//
//   Revision History:
//
//      [ Month ][Day] [Year] - [Revision]-[ Comments ]
//      May       01,   2010  -     1.0   -  Creation
//
////////////////////////////////////////////////////////////////////////////////////////////////////

#include "HelperFunctions_Include.h"     /// .
#include "HelperFunctions_NetBuffer.tmh" /// $(OBJ_PATH)\$(O)\

/**
 @kernel_helper_function="KrnlHlprNBLGetRequiredRefCount"
 
   Purpose:  Return a count of how many NBLs are within the NBL chain.                          <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
UINT32 KrnlHlprNBLGetRequiredRefCount(_In_ const NET_BUFFER_LIST* pNBL,
                                      _In_ BOOLEAN isChained)           /* FALSE */
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> KrnlHlprNBLGetRequiredRefCount()\n");

#endif /// DBG

   NT_ASSERT(pNBL);

   UINT32 requiredRefCount = 0;

   if(isChained)
   {
      for(NET_BUFFER_LIST* pCurrentNBL = (NET_BUFFER_LIST*)pNBL;
          pCurrentNBL;
          pCurrentNBL = NET_BUFFER_LIST_NEXT_NBL(pCurrentNBL))
      {
         requiredRefCount++;
      }
   }
   else
      requiredRefCount = 1;

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- KrnlHlprNBLGetRequiredRefCount() [refCount: %#d]\n",
              requiredRefCount);

#endif /// DBG
   
   return requiredRefCount;
}
