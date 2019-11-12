/****************************** Module Header ******************************\
Module Name:  Reg.h
Project:      CppShellExtDragDropHandler
Copyright (c) Microsoft Corporation.

The file declares reusable helper functions to register and unregister 
in-process COM components and shell drag-and-drop handlers in the registry.

RegisterInprocServer - register the in-process component in the registry.
UnregisterInprocServer - unregister the in-process component in the registry.
RegisterShellExtDragDropHandler - register the drag-and-drop handler.
UnregisterShellExtDragDropHandler - unregister the drag-and-drop handler.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MP.
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
//   FUNCTION: RegisterShellExtDragDropHandler
//
//   PURPOSE: Register the drag-and-drop handler.
//
//   PARAMETERS:
//   * clsid - Class ID of the component
//   * pszFriendlyName - Friendly name
//
//   NOTE: The function creates the following key in the registry.
//
//   HKCR
//   {
//      NoRemove Directory
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//      NoRemove Folder
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//      NoRemove Drive
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//   }
//
HRESULT RegisterShellExtDragDropHandler(const CLSID& clsid, PCWSTR pszFriendlyName);


//
//   FUNCTION: UnregisterShellExtDragDropHandler
//
//   PURPOSE: Unregister the drag-and-drop handler.
//
//   PARAMETERS:
//   * clsid - Class ID of the component
//
//   NOTE: The function removes the {<CLSID>} key under 
//   HKCR\Directory\shellex\DragDropHandlers, 
//   HKCR\Folder\shellex\DragDropHandlers 
//   HKCR\Drive\shellex\DragDropHandlers in the registry.
//
HRESULT UnregisterShellExtDragDropHandler(const CLSID& clsid);