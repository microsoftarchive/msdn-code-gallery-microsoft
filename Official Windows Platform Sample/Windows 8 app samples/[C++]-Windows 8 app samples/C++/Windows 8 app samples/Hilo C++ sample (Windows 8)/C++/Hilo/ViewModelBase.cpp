// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "ViewModelBase.h"
#include "ExceptionPolicy.h"

using namespace Hilo;
using namespace Platform;
using namespace std;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Navigation;

ViewModelBase::ViewModelBase(shared_ptr<ExceptionPolicy> exceptionPolicy) : m_exceptionPolicy(exceptionPolicy), m_isAppBarEnabled(false),
    m_isAppBarOpen(false), m_isAppBarSticky(false)
{
}

bool ViewModelBase::IsAppBarEnabled::get()
{
    return m_isAppBarEnabled;
}

void ViewModelBase::IsAppBarEnabled::set(bool value)
{
    if (m_isAppBarEnabled != value)
    {
        m_isAppBarEnabled = value;
        OnPropertyChanged("IsAppBarEnabled");
    }
}

bool ViewModelBase::IsAppBarOpen::get()
{
    return m_isAppBarOpen;
}

void ViewModelBase::IsAppBarOpen::set(bool value)
{
    if (m_isAppBarOpen != value)
    {
        m_isAppBarOpen = value;
        OnPropertyChanged("IsAppBarOpen");
    }
}

bool ViewModelBase::IsAppBarSticky::get()
{
    return m_isAppBarSticky;
}

void ViewModelBase::IsAppBarSticky::set(bool value)
{
    if (m_isAppBarSticky != value)
    {
        m_isAppBarSticky = value;
        OnPropertyChanged("IsAppBarSticky");
    }
}

void ViewModelBase::OnNavigatedTo(NavigationEventArgs^ e)
{
}

void ViewModelBase::OnNavigatedFrom(NavigationEventArgs^ e)
{
}

void ViewModelBase::LoadState(IMap<String^, Object^>^ stateMap)
{
}

void ViewModelBase::SaveState(IMap<String^, Object^>^ stateMap)
{
}

void ViewModelBase::GoBack()
{
    NavigateBack();
}

void ViewModelBase::GoHome()
{
    NavigateHome();
}


void ViewModelBase::GoToPage(PageType page, Object^ parameter)
{
    NavigateToPage(page, parameter);
}