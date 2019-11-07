// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "ImageBrowserViewModel.h"
#include "IPhoto.h"
#include "IMonthBlock.h"
#include "IYearGroup.h"
#include "IPhotoGroup.h"
#include "PhotoCache.h"
#include "DelegateCommand.h"
#include "ImageNavigationData.h"
#include "TaskExceptionsExtensions.h"
#include "Repository.h"

using namespace concurrency;
using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Navigation;

ImageBrowserViewModel::ImageBrowserViewModel(shared_ptr<Repository> repository, shared_ptr<ExceptionPolicy> exceptionPolicy) :  
    m_repository(repository),
    ViewModelBase(exceptionPolicy), 
    m_photoCache(std::make_shared<PhotoCache>()), 
    m_monthGroups(ref new Vector<IPhotoGroup^>()),
    m_yearGroups(ref new Vector<IYearGroup^>()),
    m_cancellationTokenSource(cancellation_token_source()),
    m_currentQueryId(1),
    m_currentMode(Mode::Pending),
    m_lastFileChangeTime(0ull),
    m_hasFileUpdateTask(false),
    m_runningMonthQuery(false), m_runningYearQuery(false)
{
    m_groupCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &ImageBrowserViewModel::NavigateToGroup), nullptr);
    m_cropImageCommand = ref new DelegateCommand(
        ref new ExecuteDelegate(this, &ImageBrowserViewModel::CropImage),
        ref new CanExecuteDelegate(this, &ImageBrowserViewModel::CanProcessImage));
    m_rotateImageCommand = ref new DelegateCommand(
        ref new ExecuteDelegate(this, &ImageBrowserViewModel::RotateImage),
        ref new CanExecuteDelegate(this, &ImageBrowserViewModel::CanProcessImage));
    m_cartoonizeImageCommand = ref new DelegateCommand(
        ref new ExecuteDelegate(this, &ImageBrowserViewModel::CartoonizeImage),
        ref new CanExecuteDelegate(this, &ImageBrowserViewModel::CanProcessImage));

    // Register with the repository to receive notifications of file system changes.
    auto wr = Platform::WeakReference(this);
    function<void()> callback = [wr] {
        auto vm = wr.Resolve<ImageBrowserViewModel>();
        if (nullptr != vm)
        {
            vm->OnDataChanged();
        }
    };
    m_repository->AddObserver(callback, PageType::Browse);
}

ImageBrowserViewModel::~ImageBrowserViewModel()
{
    if (nullptr != m_repository)
    {
        m_repository->RemoveObserver(PageType::Browse);
    }
}

#pragma region XAML Properties
ICommand^ ImageBrowserViewModel::GroupCommand::get()
{
    return m_groupCommand;
}

ICommand^ ImageBrowserViewModel::CropImageCommand::get()
{
    return m_cropImageCommand;
}

ICommand^ ImageBrowserViewModel::RotateImageCommand::get()
{
    return m_rotateImageCommand;
}

ICommand^ ImageBrowserViewModel::CartoonizeImageCommand::get()
{
    return m_cartoonizeImageCommand;
}

bool ImageBrowserViewModel::InProgress::get()
{
    return m_runningMonthQuery;
}

Object^ ImageBrowserViewModel::SelectedItem::get()
{
    return m_photo;
}

void ImageBrowserViewModel::SelectedItem::set(Object^ value)
{
    if (m_photo != value)
    {
        ViewModelBase::m_isAppBarOpen = false;
        m_photo = dynamic_cast<IPhoto^>(value);
        bool result = CanProcessImage(nullptr);
        m_rotateImageCommand->CanExecute(result);
        m_cropImageCommand->CanExecute(result);
        m_cartoonizeImageCommand->CanExecute(result);
        IsAppBarEnabled = value != nullptr;
        IsAppBarOpen = value != nullptr;
        IsAppBarSticky = value != nullptr;
        OnPropertyChanged("SelectedItem");
    }
}

IObservableVector<IPhotoGroup^>^ ImageBrowserViewModel::MonthGroups::get()
{
    assert(nullptr != m_monthGroups);
    return m_monthGroups;
}

IObservableVector<IYearGroup^>^ ImageBrowserViewModel::YearGroups::get()
{
    assert(nullptr != m_yearGroups);
    return m_yearGroups;
}

IPhoto^ ImageBrowserViewModel::GetPhotoForYearAndMonth(unsigned int year, unsigned int month)
{
    return m_photoCache->GetForYearAndMonth(year, month);
}  
#pragma endregion


#pragma region Regulate File Update Events
// To prevent UI flicker, the ImageBrowserViewMode limits the frequency of update events
// that occur when file system changes require the image brower to update the displayed items.

// This is minimum allowed time in milliseconds between successive file system change notifications. 
const ULONGLONG EVENTSPACING = 30000ul;

// This is the handler that is invoked whenever the file system notifies the image browser view model of a change in the
// previously submitted month query. The handler propagates updates at a maximum rate of one for each EVENTSPACING/1000 seconds.
void ImageBrowserViewModel::OnDataChanged()
{
    assert(IsMainThread());

    auto currentTime = GetTickCount64();

    if (m_hasFileUpdateTask || m_currentMode == Mode::Pending)
    {
        // An update is already scheduled to occur. Ignore the current change notification.
    }
    else if (m_lastFileChangeTime == 0l || currentTime > (m_lastFileChangeTime + EVENTSPACING))
    {
        // If it's the first change, or more than EVENTSPACING/1000 seconds have elapsed since the last update, observe the change immediately.
        m_lastFileChangeTime = currentTime;
        ObserveFileChange();
    }
    else
    {
        // Otherwise, schedule the update to occur EVENTSPACING/1000 seconds from the previous update.
        auto timeSinceUpdate = (currentTime > m_lastFileChangeTime ? currentTime - m_lastFileChangeTime : 0ul);
        unsigned int timeToWait = (timeSinceUpdate < EVENTSPACING ? EVENTSPACING - (unsigned int)timeSinceUpdate : EVENTSPACING);
        m_lastFileChangeTime = currentTime + timeToWait;
        m_hasFileUpdateTask = true;

        // Use weak reference to avoid inadvertently extending object lifetime.
        auto weakThis = Platform::WeakReference(this);

        // Observe the update after waiting the specified amount of time.
        create_task([timeToWait]() {
            assert(IsBackgroundThread());
            ::wait(timeToWait);
        }).then([weakThis]() {
            assert(IsMainThread());
            auto vm = weakThis.Resolve<ImageBrowserViewModel>();
            if (nullptr != vm)
            {
                vm->ObserveFileChange();
                vm->m_hasFileUpdateTask = false;
            }
        }, task_continuation_context::use_current()).then(ObserveException<void>(m_exceptionPolicy));
    }
}
#pragma endregion

void ImageBrowserViewModel::NavigateToGroup(Object^ parameter)
{
    auto group = dynamic_cast<IPhotoGroup^>(parameter);
    assert(group != nullptr);
    if (group->Items->Size > 0)
    {
        auto photo = dynamic_cast<IPhoto^>(group->Items->GetAt(0));
        assert(photo != nullptr);
        ImageNavigationData imageData(photo);
        ViewModelBase::GoToPage(PageType::Image, imageData.SerializeToString());
    }
}

void ImageBrowserViewModel::CropImage(Object^ parameter)
{
    ImageNavigationData data(m_photo);
    ViewModelBase::GoToPage(PageType::Crop, data.SerializeToString());
}

void ImageBrowserViewModel::RotateImage(Object^ parameter)
{
    ImageNavigationData data(m_photo);
    ViewModelBase::GoToPage(PageType::Rotate, data.SerializeToString());
}

void ImageBrowserViewModel::CartoonizeImage(Object^ parameter)
{
    ImageNavigationData data(m_photo);
    ViewModelBase::GoToPage(PageType::Cartoonize, data.SerializeToString());
}

bool ImageBrowserViewModel::CanProcessImage(Object^ parameter)
{
    return (m_photo != nullptr && !m_photo->IsInvalidThumbnail);
}

#pragma region ImageViewModelStateMachine

// The functions in this region implement a finite state machine that handles
// all possible interleavings of ImageBrowserViewModel actions. These include
// user requests, file system update notifications and completions of in-flight 
// async operations.

// Possible modes or states:
//    Default (0, 0, 0): no pending data changes, not running query, view inactive  
//    Pending (1, 0, 0): pending data changes, not running query, view inactive 
//    Active  (0, 0, 1): no pending data changes, not running query, view active 
//    Running (0, 1, 1): no pending data changes, running query, view active   

// Transitions between modes:
//    OnNavigatedFrom - occurs when view becomes inactive
//    OnNavigatedTo - occurs when becomes active
//    ObserveFileChange - occurs when file system changes invalidate the result of the current query
//    FinishMonthAndYearQueries - occurs when current month and year queries are both complete

// State transition occurs when view model becomes inactive (no longer visible).
void ImageBrowserViewModel::OnNavigatedFrom(NavigationEventArgs^ e)
{
    assert(IsMainThread());
    switch (m_currentMode)
    {
    case Mode::Default:
        assert(false);       // Can't navigate from an inactive view model
        break;

    case Mode::Pending:
        assert(false);       // Can't navigate from an inactive view model
        break;

    case Mode::Active:
        m_currentMode = Mode::Default;
        break;

    case Mode::Running:
        CancelMonthAndYearQueries();
        m_currentMode = Mode::Pending;
        break;
    }
}

// State transition occurs when view model becomes active (visible).
void ImageBrowserViewModel::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
    assert(IsMainThread());
    switch (m_currentMode)
    {
    case Mode::Default:
        m_currentMode = Mode::Active;
        break;

    case Mode::Pending:
        StartMonthAndYearQueries();
        m_currentMode = Mode::Running;
        break;

    case Mode::Active:
        assert(false);   // Can't navigate to already active view model
        break;

    case Mode::Running:
        assert(false);   // Can't navigate to already active view model
        break;
    }
}

// State transition occurs when file system changes invalidate the result of the current query.
void ImageBrowserViewModel::ObserveFileChange()
{
    assert(IsMainThread());

    switch (m_currentMode)
    {
    case Mode::Default:
        m_currentMode = Mode::Pending;
        break;

    case Mode::Pending:
        m_currentMode = Mode::Pending;
        break;

    case Mode::Active:
        StartMonthAndYearQueries();
        m_currentMode = Mode::Running;
        break;

    case Mode::Running:
        CancelMonthAndYearQueries();
        StartMonthAndYearQueries();
        m_currentMode = Mode::Running;
        break;
    }
}

// State transition occurs when both month and year queries have finished running. 
void ImageBrowserViewModel::FinishMonthAndYearQueries()
{
    assert(m_runningMonthQuery == false);
    assert(m_runningYearQuery == false);
    assert(m_currentMode == Mode::Running);

    switch (m_currentMode)
    {
    case Mode::Default:
        assert(false);  // There is no query in flight in the Default mode.
        break;

    case Mode::Pending:
        assert(false);  // There is no query in flight in the Pending mode.
        break;

    case Mode::Active:
        assert(false);  // There is no query in flight in the Active mode.
        break;

    case Mode::Running:
        m_currentMode = Mode::Active;
        m_currentQueryId++;
        break;
    }
}
#pragma endregion

#pragma region YearAndMonthQueries
void ImageBrowserViewModel::CancelMonthAndYearQueries()
{
    assert(m_currentMode == Mode::Running);

    if (m_runningMonthQuery)
    {
        m_runningMonthQuery = false;
        OnPropertyChanged("InProgress");
    }
    m_runningYearQuery = false;
    m_currentQueryId++;
    m_cancellationTokenSource.cancel();
}

void ImageBrowserViewModel::StartMonthAndYearQueries()
{
    assert(IsMainThread());
    assert(!m_runningMonthQuery);
    assert(!m_runningYearQuery);
    assert(m_currentMode == Mode::Active || m_currentMode == Mode::Running || m_currentMode == Mode::Pending);

    m_cancellationTokenSource = cancellation_token_source();
    auto token = m_cancellationTokenSource.get_token();
    StartMonthQuery(m_currentQueryId, token);
    StartYearQuery(m_currentQueryId, token);
}

void ImageBrowserViewModel::StartMonthQuery(int queryId, cancellation_token token)
{
    m_runningMonthQuery = true;
    OnPropertyChanged("InProgress");
    m_photoCache->Clear();
    run_async_non_interactive([this, queryId, token]()
    {
        // if query is obsolete, don't run it.
        if (queryId != m_currentQueryId) return;

        m_repository->GetMonthGroupedPhotosWithCacheAsync(m_photoCache, token).then([this, queryId](task<IVectorView<IPhotoGroup^>^> priorTask)
        {
            assert(IsMainThread());
            if (queryId != m_currentQueryId)  
            {
                // Query is obsolete. Propagate exception and quit.
                priorTask.get();
                return;
            }

            m_runningMonthQuery = false;
            OnPropertyChanged("InProgress");
            if (!m_runningYearQuery)
            {
                FinishMonthAndYearQueries();
            }
            try
            {     
                // Update display with results.
                m_monthGroups->Clear();                   
                for (auto group : priorTask.get())
                {  
                    m_monthGroups->Append(group);
                }
                OnPropertyChanged("MonthGroups");
            }
            // On exception (including cancellation), remove any partially computed results and rethrow.
            catch (...)
            {
                m_monthGroups = ref new Vector<IPhotoGroup^>();
                throw;
            }
        }, task_continuation_context::use_current()).then(ObserveException<void>(m_exceptionPolicy));
    });
}

void ImageBrowserViewModel::StartYearQuery(int queryId, cancellation_token token)
{
    assert(!m_runningYearQuery);
    assert(m_currentMode == Mode::Active || m_currentMode == Mode::Running || m_currentMode == Mode::Pending);

    m_runningYearQuery = true;
    run_async_non_interactive([this, queryId, token]()
    {
        assert(IsMainThread());

        // If query is obsolete, don't run it.
        if (queryId != m_currentQueryId) return;

        m_repository->GetYearGroupedMonthsAsync(token).then([this, queryId](task<IVectorView<IYearGroup^>^> priorTask)
        {
            assert(IsMainThread());
            if (queryId != m_currentQueryId)
            {
                // Query is obsolete. Rethrow any exception and exit.
                priorTask.get();
                return;
            }

            m_runningYearQuery = false;
            if (!m_runningMonthQuery)
            {
                FinishMonthAndYearQueries();
            }

            try
            {
                // Update display with results.
                m_yearGroups->Clear();  
                for (auto group : priorTask.get())
                {  
                    m_yearGroups->Append(group);
                }
                OnPropertyChanged("YearGroups");
            }
            // On exception (including cancellation), remove any partially computed results and rethrow.
            catch (...)
            {
                m_yearGroups = ref new Vector<IYearGroup^>();
                throw;
            }
        }, task_continuation_context::use_current()).then(ObserveException<void>(m_exceptionPolicy));
    });
}
#pragma endregion



