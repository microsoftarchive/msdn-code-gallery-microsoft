/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

#include "stdafx.h"

#include "MenuAndCommandsPackage.h"

void MenuAndCommandsPackage::MenuVisibilityCallback(CommandHandler* pSender, DWORD /*flags*/, VARIANT* /*pIn*/, VARIANT* /*pOut*/)
{
	// Check the input parameter.
	VSL_CHECKPOINTER_DEFAULT(pSender);

	// Check that the caller is actually one of the menu items with dynamic visibility.
	const CommandId& id = pSender->GetId();
	VSL_CHECKBOOLEAN_EX(CLSID_MenuAndCommandsCmdSet == id.GetGuid(), E_UNEXPECTED, IDS_E_MENUVISIBILITYCALLBACK_UNEXPECTED);
	VSL_CHECKBOOLEAN_EX((cmdidDynVisibility1 == id.GetId()) || (cmdidDynVisibility2 == id.GetId()), E_UNEXPECTED, IDS_E_MENUVISIBILITYCALLBACK_UNEXPECTED);

	// Now we want to change the visibility of the two commands; the one that is visible is the pSender,
	// so first we have to get the other one.
	// Notice that we can not use the GetCommand function, because if the command is not found, then 
	// IOleCommandTarget::Exec will return OLECMDERR_E_NOTSUPPORTED and Visual Studio will think that
	// this package is not handling the command and will keep searching for one that that handles it.
	// What we need to do is to return an error so that the shell will know that we handle the command,
	// but the execution has found an error.
	DWORD dwOtherId = id.GetId()==cmdidDynVisibility1 ? cmdidDynVisibility2 : cmdidDynVisibility1;
	CommandHandler* pOtherCommand = TryToGetCommand(CommandId(CLSID_MenuAndCommandsCmdSet, dwOtherId));
	VSL_CHECKBOOL_EX(NULL != pOtherCommand, E_UNEXPECTED, IDS_E_MENUVISIBILITYCALLBACK_NOCOMMANDHANDLER);

	// Now we have the two commands, so it is possible to change the visibility.
	pSender->SetVisible(false);
	// Disable warning 6011 because prefast does not realize that VSL_CHECKBOOL will throw if
	// pOtherCommand is null.
	#pragma warning(push)
	#pragma warning(disable:6011)
	pOtherCommand->SetVisible(true);
	#pragma warning(pop)
}
