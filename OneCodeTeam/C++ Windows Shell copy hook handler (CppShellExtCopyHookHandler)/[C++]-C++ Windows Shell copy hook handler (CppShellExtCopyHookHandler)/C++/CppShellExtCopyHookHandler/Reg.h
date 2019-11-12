/****************************** Module Header ******************************\
Module Name:  Reg.h
Project:      CppShellExtCopyHookHandler
Copyright (c) Microsoft Corporation.

The file declares reusable helper functions to register and unregister 
in-process COM components and shell copy hook handlers in the registry.

RegisterInprocServer - register the in-process component in the registry.
UnregisterInprocServer - unregister the in-process component in the registry.
RegisterShellExtFolderCopyHookHandler - register the copy hook handler.
UnregisterShellExtFolderCopyHookHandler - unregister the copy hook handler.

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
//
//   NOTE: The function creates the HKCR\CLSID\{<CLSID>} key in the registry.
// 
//   HKCR
//   {
//      NoRemove CLSID
//      {
//          ForceRemove {<CLSID>} = s '<Friendly Name>'
//          {
//              InprocServer32 = s '%MODULE%'
//              {
//                  val ThreadingModel = s '<Thread Model>'
//              }
//          }
//      }
//   }
//
HRESULT RegisterInprocServer(PCWSTR pszModule, const CLSID& clsid, 
    PCWSTR pszFriendlyName, PCWSTR pszThreadModel);


//
//   FUNCTION: UnregisterInprocServer
//
//   PURPOSE: Unegister the in-process component in the registry.
//
//   PARAMETERS:
//   * clsid - Class ID of the component
//
//   NOTE: The function deletes the HKCR\CLSID\{<CLSID>} key in the registry.
//
HRESULT UnregisterInprocServer(const CLSID& clsid);


//
//   FUNCTION: RegisterShellExtFolderCopyHookHandler
//
//   PURPOSE: Register the folder copy hook handler.
//
//   PARAMETERS:
//   * pszName - The name of the copy hook handler
//   * clsid - Class ID of the component
//
//   NOTE: The function creates the following key in the registry.
//
//   HKCR
//   {
//      NoRemove Directory
//      {
//          NoRemove shellex
//          {
//              NoRemove CopyHookHandlers
//              {
//                  <Name> = s '{<CLSID>}'
//              }
//          }
//      }
//   }
//
HRESULT RegisterShellExtFolderCopyHookHandler(PCWSTR pszName, 
    const CLSID& clsid);


//
//   FUNCTION: UnregisterShellExtFolderCopyHookHandler
//
//   PURPOSE: Unregister the folder copy hook handler.
//
//   PARAMETERS:
//   * pszName - The name of the copy hook handler
//
//   NOTE: The function removes the <Name> key under 
//   HKCR\Directory\shellex\CopyHookHandlers in the registry.
//
HRESULT UnregisterShellExtFolderCopyHookHandler(PCWSTR pszName);