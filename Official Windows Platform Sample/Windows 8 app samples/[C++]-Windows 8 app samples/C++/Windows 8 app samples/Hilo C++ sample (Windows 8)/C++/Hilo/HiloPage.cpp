// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "HiloPage.h"
#include "Common\SuspensionManager.h"

using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation::Collections;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Navigation;

// See http://go.microsoft.com/fwlink/?LinkId=267274 for info about the Hilo app.

static TypeName hiloPageTypeName = { Hilo::HiloPage::typeid->FullName, TypeKind::Metadata };
static TypeName objectTypeName = { "Object", TypeKind::Primitive };
static Platform::String^ viewModelStateKey = "ViewModelState";

static TypeName rotateImageViewType = { "Hilo.RotateImageView", TypeKind::Metadata };
static TypeName cropImageViewType = { "Hilo.CropImageView", TypeKind::Metadata };
static TypeName cartoonizeImageViewType = { "Hilo.CartoonizeImageView", TypeKind::Metadata };
static TypeName imageViewType = { "Hilo.ImageView", TypeKind::Metadata };
static TypeName imageBrowserType =  { "Hilo.ImageBrowserView", TypeKind::Metadata };
static TypeName mainHubType = { "Hilo.MainHubView", TypeKind::Metadata };

map<PageType, TypeName> create_map()
{
    std::map<PageType, TypeName> pages;
    pages.insert(std::make_pair(PageType::Crop, cropImageViewType));
    pages.insert(std::make_pair(PageType::Rotate, rotateImageViewType));
    pages.insert(std::make_pair(PageType::Cartoonize, cartoonizeImageViewType));
    pages.insert(std::make_pair(PageType::Image, imageViewType));
    pages.insert(std::make_pair(PageType::Browse, imageBrowserType));   
    pages.insert(std::make_pair(PageType::Hub, mainHubType));
    return pages;
}
static map<PageType, TypeName> m_pages(create_map()); 

static DependencyProperty^ _hiloDataContextProperty =
    DependencyProperty::Register(
    "HiloDataContext",
    objectTypeName, 
    hiloPageTypeName, 
    ref new PropertyMetadata(
    nullptr, 
    ref new PropertyChangedCallback(
    &HiloPage::OnHiloDataContextPropertyChanged)
    ));

void HiloPage::OnHiloDataContextPropertyChanged(Windows::UI::Xaml::DependencyObject^ element, Windows::UI::Xaml::DependencyPropertyChangedEventArgs^ e)
{
    HiloPage^ page = static_cast<HiloPage^>(element);
    ViewModelBase^ oldViewModel = dynamic_cast<ViewModelBase^>(e->OldValue);
    page->DetachNavigationHandlers(oldViewModel);

    ViewModelBase^ newViewModel = dynamic_cast<ViewModelBase^>(e->NewValue);
    page->AttachNavigationHandlers(newViewModel);
}

DependencyProperty^ HiloPage::HiloDataContextProperty::get()
{
    return _hiloDataContextProperty;
}

HiloPage::HiloPage() : m_hasHandlers(false)
{
    if (Windows::ApplicationModel::DesignMode::DesignModeEnabled)
    {
        return;
    }

    // binding for data context changed
    SetBinding(HiloPage::HiloDataContextProperty, ref new Binding());
}


// See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

void HiloPage::NavigateBack()
{
    GoBack(nullptr, nullptr);
}

void HiloPage::NavigateHome()
{
    GoHome(nullptr, nullptr);
}

void HiloPage::NavigateToPage(PageType page, Platform::Object^ parameter)
{
    if (Frame != nullptr)
    {
        TypeName source = m_pages[page];
        Frame->Navigate(source, parameter);
    }
}

void HiloPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    
    ViewModelBase^ viewModel = dynamic_cast<ViewModelBase^>(DataContext);
    this->AttachNavigationHandlers(viewModel);
    if (viewModel != nullptr)
    {
        viewModel->OnNavigatedTo(e);
    }

    LayoutAwarePage::OnNavigatedTo(e);
}

// See http://go.microsoft.com/fwlink/?LinkId=267280 for more info on Hilo's implementation of suspend/resume.
void Hilo::HiloPage::OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
    ViewModelBase^ viewModel = dynamic_cast<ViewModelBase^>(DataContext);
    if (!HiloPage::IsSuspending) 
    {        
        // Since we never receive destructs, this is the only place to unsubscribe.
        DetachNavigationHandlers(viewModel);
        viewModel->OnNavigatedFrom(e);
    }

   LayoutAwarePage::OnNavigatedFrom(e);
}

// See http://go.microsoft.com/fwlink/?LinkId=267280 for more info on Hilo's implementation of suspend/resume.
void HiloPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
    auto vm = dynamic_cast<ViewModelBase^>(DataContext);
    if (vm != nullptr && pageState != nullptr)
    {
        IMap<String^, Object^>^ state = nullptr;
        state = dynamic_cast<IMap<String^, Object^>^>(pageState->Lookup(viewModelStateKey));

        vm->LoadState(state);
    }
}

// See http://go.microsoft.com/fwlink/?LinkId=267280 for more info on Hilo's implementation of suspend/resume.
void HiloPage::SaveState(IMap<String^, Object^>^ pageState)
{
    auto vm = dynamic_cast<ViewModelBase^>(DataContext);
    if (vm != nullptr)
    {
        auto vmStateMap = ref new Map<String^, Object^>();
        vm->SaveState(vmStateMap);

        pageState->Insert(viewModelStateKey, vmStateMap);
    }
}

void HiloPage::AttachNavigationHandlers(ViewModelBase^ viewModel) 
{
    if (nullptr == viewModel) return;

    // Only resusbcribe if our tokens are empty.

    if (!m_hasHandlers)
    {
        m_navigateBackEventToken = viewModel->NavigateBack::add(ref new NavigateEventHandler(this, &HiloPage::NavigateBack));
        m_navigateHomeEventToken = viewModel->NavigateHome::add(ref new NavigateEventHandler(this, &HiloPage::NavigateHome));
        m_navigateToPageEventToken = viewModel->NavigateToPage::add(ref new PageNavigateEventHandler(this, &HiloPage::NavigateToPage));
        m_hasHandlers = true;
    }
}

void HiloPage::DetachNavigationHandlers(ViewModelBase^ viewModel) 
{
    if (nullptr == viewModel) return;
    assert(!HiloPage::IsSuspending);

    if (m_hasHandlers)
    {
        viewModel->NavigateBack::remove(m_navigateBackEventToken);
        viewModel->NavigateHome::remove(m_navigateHomeEventToken);
        viewModel->NavigateToPage::remove(m_navigateToPageEventToken);
        m_hasHandlers = false;
    };
}