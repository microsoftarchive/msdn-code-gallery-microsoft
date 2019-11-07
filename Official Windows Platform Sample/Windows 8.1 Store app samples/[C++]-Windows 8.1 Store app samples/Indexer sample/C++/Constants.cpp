//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::Indexer;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Add item to the index using the ContentIndexer",             "SDKSample.Indexer.S1_AddWithAPI" }, 
    { "Update and delete indexed items using the ContentIndexer",   "SDKSample.Indexer.S2_UpdateAndDeleteWithAPI" },
    { "Retrieve indexed items added using the ContentIndexer",      "SDKSample.Indexer.S3_RetrieveWithAPI" },
    { "Check the index revision number",                            "SDKSample.Indexer.S4_CheckIndexRevision" }, 
    { "Add indexed items by using appcontent-ms files",             "SDKSample.Indexer.S5_AddWithAppContent" },
    { "Delete indexed appcontent-ms files",                         "SDKSample.Indexer.S6_DeleteWithAppContent" },
    { "Retrieve indexed properties from appcontent-ms files",       "SDKSample.Indexer.S7_RetrieveWithAppContent" }
};