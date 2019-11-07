/****************************** Module Header ******************************\
Module Name:  Reg.h
Project:      CppShellExtPreviewHandler
Copyright (c) Microsoft Corporation.

The file declares reusable helper functions to register and unregister 
in-process COM components and shell preview handlers in the registry.

RegisterInprocServer - register the in-process component in the registry.
UnregisterInprocServer - unregister the in-process component in the registry.
RegisterShellExtPreviewHandler - register the preview handler.
UnregisterShellExtPreviewHandler - unregister the preview handler.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>


//
//   FUNCTION: RegisterInprocServer
//
//   PURPOSE: Register the in-process component in the registry.
//
//   PARAMETERS:
//   * pszModule - Path of the module that contains the component
//   * clsid - Class ID of the component
//   * pszFriendlyName - Friendly name
//   * pszThreadModel - Threading model
//   * appId - AppID for Dll surrogate
//
//   NOTE: The function creates the HKCR\CLSID\{<CLSID>} key in the registry.
// 
//   HKCR
//   {
//      NoRemove CLSID
//      {
//          ForceRemove {<CLSID>} = s '<Friendly Name>'
//          {
//              val AppID = s '{<AppID>}'
//              InprocServer32 = s '%MODULE%'
//              {
//                  val ThreadingModel = s '<Thread Model>'
//              }
//          }
//      }
//      NoRemove AppID
//      {
//          ForceRemove {<AppID>}
//          {
//              val DllSurrogate = s '%SystemRoot%\system32\prevhost.exe'
//          }
//      }
//   }
//
HRESULT RegisterInprocServer(PCWSTR pszModule, const CLSID& clsid, 
    PCWSTR pszFriendlyName, PCWSTR pszThreadModel, const GUID& appId);


//
//   FUNCTION: UnregisterInprocServer
//
//   PURPOSE: Unegister the in-process component in the registry.
//
//   PARAMETERS:
//   * clsid - Class ID of the component
//   * appId - AppID for Dll surrogate
//
//   NOTE: The function deletes the HKCR\CLSID\{<CLSID>} key and the 
//   HKCR\AppID\{<AppID>} key in the registry.
//
HRESULT UnregisterInprocServer(const CLSID& clsid, const GUID& appId);


//
//   FUNCTION: RegisterShellExtPreviewHandler
//
//   PURPOSE: Register the shell preview handler.
//
//   PARAMETERS:
//   * pszFileType - The file type that the context menu handler is 
//     associated with. For example, '.txt' means all .txt files. The 
//     parameter must not be NULL.
//   * clsid - Class ID of the component
//   * pszDescription - The description of the preview handler. 
//
//   NOTE: The function creates the following key in the registry.
//
//   HKCR
//   {
//      NoRemove <File Type>
//      {
//          NoRemove shellex
//          {
//              {8895b1c6-b41f-4c1c-a562-0d564250836f} = s '{<CLSID>}'
//          }
//      }
//   }
//   HKCU
//   {
//      NoRemove SOFTWARE
//      {
//          NoRemove Microsoft
//          {
//              NoRemove Windows
//              {
//                  NoRemove CurrentVersion
//                  {
//                      NoRemove
//                      {
//                          PreviewHandlers
//                          {
//                              val {<CLSID>} = s '<Description>'
//                          }
//                      }
//                  }
//              }
//          }
//      }
//   }
//
HRESULT RegisterShellExtPreviewHandler(PCWSTR pszFileType, 
    const CLSID& clsid, PCWSTR pszDescription);


//
//   FUNCTION: UnregisterShellExtPreviewHandler
//
//   PURPOSE: Unregister the preview handler.
//
//   PARAMETERS:
//   * pszFileType - The file type that the context menu handler is 
//     associated with. For example, '.txt' means all .txt files. The 
//     parameter must not be NULL.
//   * clsid - Class ID of the component
//
//   NOTE: The function removes the registry key
//   HKCR\<File Type>\shellex\{8895b1c6-b41f-4c1c-a562-0d564250836f},
//   and remove the {<CLSID>} value from the preview handler list:
//   HKCU\Software\Microsoft\Windows\CurrentVersion\PreviewHandlers
//
HRESULT UnregisterShellExtPreviewHandler(PCWSTR pszFileType, 
    const CLSID& clsid);