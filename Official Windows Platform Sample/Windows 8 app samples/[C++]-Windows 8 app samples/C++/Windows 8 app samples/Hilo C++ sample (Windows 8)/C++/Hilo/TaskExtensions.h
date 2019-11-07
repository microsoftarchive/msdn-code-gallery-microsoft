// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo 
{
    // Creates a task that completes with the provided result.
    template <typename Result>
    inline concurrency::task<Result> create_task_from_result(Result result)
    {
        return concurrency::create_task([result]() -> Result { return result; });
    }

    // Creates a task that completes with a void result.
    inline concurrency::task<void> create_empty_task()
    {
        return concurrency::create_task([]{});
    }

    // Runs a function in the main thread with non-interactive priority.
    void run_async_non_interactive(std::function<void ()>&& action);
}

#ifndef NDEBUG

#ifdef  __cplusplus
extern "C" {
#endif

    // Called at startup to remember the main thread id for debug assertion checking
    void RecordMainThread(void);

    // Called in debug mode only for thread context assertion checks
    bool IsMainThread(void);

    // Called in debug mode only for thread context assertion checks
    bool IsBackgroundThread(void);

#ifdef  __cplusplus
}
#endif

#endif /* not NDEBUG */