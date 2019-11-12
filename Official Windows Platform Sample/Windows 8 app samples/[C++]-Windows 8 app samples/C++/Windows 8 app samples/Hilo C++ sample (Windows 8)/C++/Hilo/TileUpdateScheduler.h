// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    class ExceptionPolicy;
    class Repository;

    // See http://go.microsoft.com/fwlink/?LinkId=267275 for info about Hilo's implementation of tiles.
    
    // The TileUpdateScheduler class keeps Hilo's start tile up to date.
    class TileUpdateScheduler
    {
    public:
        TileUpdateScheduler();
        concurrency::task<void> TileUpdateScheduler::ScheduleUpdateAsync(std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy>  exceptionPolicy);

    private:
        void TileUpdateScheduler::UpdateTile(Windows::Foundation::Collections::IVector<Windows::Storage::StorageFile^>^ files);
    };
}
