// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "HubPhotoGroup.h"
#include "TaskExceptionsExtensions.h"
#include "IPhoto.h"
#include "IResizable.h"
#include "Repository.h"

using namespace concurrency;
using namespace Hilo;
using namespace std;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;

const unsigned int MaxNumberOfPictures = 6;

HubPhotoGroup::HubPhotoGroup(String^ title, String^ emptyTitle, shared_ptr<Repository> repository, shared_ptr<ExceptionPolicy> exceptionPolicy) : 
    m_title(title), m_emptyTitle(emptyTitle), m_retrievedPhotos(false), m_repository(repository), m_exceptionPolicy(exceptionPolicy),
    m_receivedChangeWhileRunning(false), m_runningQuery(false), m_hasFileUpdateTask(false), m_lastFileChangeTime(0ull)
{
    auto wr = Platform::WeakReference(this);
    function<void()> callback = [wr] {
        auto vm = wr.Resolve<HubPhotoGroup>();
        if (nullptr != vm)
        {
            vm->OnDataChanged();
        }
    };
    m_repository->AddObserver(callback, PageType::Hub);   
}

HubPhotoGroup::~HubPhotoGroup()
{
    if (nullptr != m_repository)
    {
        m_repository->RemoveObserver(PageType::Hub);
    }
}

#pragma region Regulate File Update Events
// To prevent UI flicker, the HubPhotoGroup limits the frequency of update events
// that occur when file system changes require the hub photo group to update the displayed items.

// This is minimum allowed time in milliseconds between successive file system change notifications. 
const ULONGLONG EVENTSPACING = 30000ul;

// This is the handler that is invoked whenever the file system notifies the hub photo group of a change in the
// previously submitted query. The handler propagates updates at a maximum rate of one for each EVENTSPACING/1000 seconds.
void HubPhotoGroup::OnDataChanged()
{
    assert(IsMainThread());

    auto currentTime = GetTickCount64();

    if (m_hasFileUpdateTask)
    {
        // An update is already scheduled to occur. Ignore the current change notification.
    }
    else if (m_lastFileChangeTime == 0ull || currentTime > (m_lastFileChangeTime + EVENTSPACING))
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
            auto obj = weakThis.Resolve<HubPhotoGroup>();
            if (nullptr != obj)
            {
                obj->ObserveFileChange();
                obj->m_hasFileUpdateTask = false;
            }
        }, task_continuation_context::use_current()).then(ObserveException<void>(m_exceptionPolicy));
    }
}
#pragma endregion

void HubPhotoGroup::OnPropertyChanged(String^ propertyName)
{
    assert(IsMainThread());
    PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs(propertyName));
}

IObservableVector<IPhoto^>^ HubPhotoGroup::Items::get()
{
    assert(IsMainThread());
    if (m_photos == nullptr)
    {
        OnDataChanged();
    }
    return m_photos;
}

String^ HubPhotoGroup::Title::get()
{
    if (!m_retrievedPhotos)
    {
        return "";
    }

    if (m_photos->Size == 0)
    {
        return m_emptyTitle;
    }
    return m_title;
}

task<void> HubPhotoGroup::QueryPhotosAsync()
{
    // only for unit testing
    if (m_photos == nullptr)
    {
        m_photos = ref new Vector<IPhoto^>();
    }

    auto t = m_repository->GetPhotosForPictureHubGroupAsync(this, MaxNumberOfPictures);
    return t.then([this](IVectorView<IPhoto^>^ photos)
    {
        assert(IsMainThread());
        m_retrievedPhotos = true;
        m_photos->Clear();
        bool firstPhoto = true;
        for (auto photo : photos)
        {
            if (firstPhoto)
            {
                IResizable^ resizable = dynamic_cast<IResizable^>(photo);
                if (nullptr != resizable)
                {
                    resizable->ColumnSpan = 2;
                    resizable->RowSpan = 2;
                }
                firstPhoto = false;
            }
            m_photos->Append(photo);
        }
    });
}

void HubPhotoGroup::ObserveFileChange()
{
    assert(IsMainThread());
    if (m_runningQuery)
    {
        m_receivedChangeWhileRunning = true;
    }

    if (!m_runningQuery)
    {          
        if (m_photos == nullptr)
        {
            m_photos = ref new Vector<IPhoto^>();
        }
        m_runningQuery = true;
        QueryPhotosAsync().then([this]
        {
            assert(IsMainThread());
            if (!m_receivedChangeWhileRunning)
            {
                OnPropertyChanged("Items");
                OnPropertyChanged("Title");
            }
            m_runningQuery = false;
            if (m_receivedChangeWhileRunning)
            {
                m_receivedChangeWhileRunning = false;
                OnDataChanged();
            }
        }).then(ObserveException<void>(m_exceptionPolicy));
 
    }
}