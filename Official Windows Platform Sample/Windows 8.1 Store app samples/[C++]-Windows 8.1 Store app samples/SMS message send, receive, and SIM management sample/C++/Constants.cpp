//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Sending a text message", "SDKSample.SmsSendReceive.SendMessage" },
    { "Receiving a text message", "SDKSample.SmsSendReceive.ReceiveMessage" },
    { "Sending a message in PDU format", "SDKSample.SmsSendReceive.SendPduMessage" },
    { "Reading a message from SIM", "SDKSample.SmsSendReceive.ReadMessage" },
    { "Deleting a message from SIM", "SDKSample.SmsSendReceive.DeleteMessage" }
};
