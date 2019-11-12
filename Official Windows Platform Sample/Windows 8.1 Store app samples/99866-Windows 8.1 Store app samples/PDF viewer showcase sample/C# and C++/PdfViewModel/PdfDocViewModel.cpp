// Copyright (c) Microsoft Corporation. All rights reserved

//
// PdfDocViewModel.cpp
// Definition of the PdfDocViewModel class
//

#include "pch.h"
#include "PdfDocViewModel.h"
#include "ImageSourceVSIS.h"
#include "PdfViewContext.h"
#include "PdfPageViewModel.h"

using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Interop;
using namespace Platform;

namespace PdfViewModel
{
    PdfDocViewModel::PdfDocViewModel(_In_ Windows::Data::Pdf::PdfDocument^ pdfDoc, _In_ Windows::Foundation::Size pageSize, _In_ PdfViewModel::SurfaceType surfaceType)
        : pdfDocument(pdfDoc)
    {
        pdfViewContext = ref new PdfViewContext(pageSize, pdfDocument->GetPage(0)->Size, surfaceType);
        storage = ref new Collections::Vector<Object^>();
        storage->VectorChanged += ref new Windows::Foundation::Collections::VectorChangedEventHandler<Object^>(this, &PdfDocViewModel::StorageVectorChanged);
        isVectorChangedObserved = false;

        for (unsigned int pg = 0; pg < pdfDocument->PageCount; ++pg)
        {
            Append(CreateEmptyItem(pg));
        }
    }

#pragma region IBindableObservableVector

    Windows::Foundation::EventRegistrationToken PdfDocViewModel::VectorChanged::add(_In_ BindableVectorChangedEventHandler^ e)
    {
        isVectorChangedObserved = true;
        return privateVectorChanged += e;
    }

    void PdfDocViewModel::VectorChanged::remove(_In_ Windows::Foundation::EventRegistrationToken t)
    {
        privateVectorChanged -= t;
    }

    void PdfDocViewModel::VectorChanged::raise(_In_ IBindableObservableVector^ vector, _In_ Object^ e)
    {
        if (isVectorChangedObserved)
        {
            privateVectorChanged(vector, e);
        }
    }

#pragma endregion

#pragma region IBindableIterator

    IBindableIterator^ PdfDocViewModel::First()
    {
        return dynamic_cast<IBindableIterator^>(storage->First());
    }

#pragma endregion

#pragma region IBindableVector

    void PdfDocViewModel::Append(_In_ Object^ value)
    {
        storage->Append(value);
    }

    void PdfDocViewModel::Clear()
    {
        storage->Clear();
    }

    Object^ PdfDocViewModel::GetAt(_In_ unsigned int index)
    {
        Object^ item = storage->GetAt(index);
        if (item == nullptr)
        {
            item = CreateEmptyItem(index);
            SetAt(index, item);
        }
        RealizeItemIfNotAvailable(index, item);
        return item;
    }

    IBindableVectorView^ PdfDocViewModel::GetView()
    {
        return safe_cast<IBindableVectorView^>(storage->GetView());
    }

    bool PdfDocViewModel::IndexOf(_In_ Object^ value, _In_ unsigned int* index)
    {
        return storage->IndexOf(value, index);
    }

    void PdfDocViewModel::InsertAt(_In_ unsigned int index, _In_ Object^ value)
    {
        storage->InsertAt(index, value);
    }

    void PdfDocViewModel::RemoveAt(_In_ unsigned int index)
    {
        storage->RemoveAt(index);
    }

    void PdfDocViewModel::RemoveAtEnd()
    {
        storage->RemoveAtEnd();
    }

    void PdfDocViewModel::SetAt(_In_ unsigned int index, _In_ Object^ value)
    {
        storage->SetAt(index, value);
    }

    unsigned int PdfDocViewModel::Size::get()
    {
        return storage->Size;
    }

#pragma endregion

    /// <summary>
    /// This function is used to create empty PdfPageViewModel items
    /// </summary>
    Object^ PdfDocViewModel::CreateEmptyItem(_In_ unsigned int index)
    {
        auto obj = ref new PdfPageViewModel(pdfDocument, index);
        obj->SetViewContext(pdfViewContext);
        return obj;
    }

    /// <summary>
    /// This function should be used to realize the item (do some initialization tasks if required).
    /// </summary>
    void PdfDocViewModel::RealizeItemIfNotAvailable(_In_ unsigned int /*index*/, _In_ Object^ /*item*/)
    {
    }

    /// <summary>
    /// Call this method when the app suspends to hint to the driver that the app is entering an idle state
    /// and that its memory can be used temporarily for other apps.
    /// </summary>
    void PdfDocViewModel::Trim()
    {
        pdfViewContext->Renderer->Trim();
    }

    /// <summary>
    /// Function to update page based on current zoomFactor
    /// </summary>
    /// <param name="currentZoomFactor">Current zoom factor of scroll viewer</param>
    void PdfDocViewModel::UpdatePages(_In_ float currentZoomFactor)
    {
        if (pdfViewContext->ZoomFactor != currentZoomFactor)
        {
            pdfViewContext->ZoomFactor = currentZoomFactor;
            if (pdfViewContext->SurfaceType == SurfaceType::VirtualSurfaceImageSource)
            {
                for (unsigned int index = 0, len = Size; index < len; index++)
                {
                    auto pageData = dynamic_cast<PdfPageViewModel^>(storage->GetAt(index));
                    if ((pageData != nullptr) && !pageData->IsUnloaded())
                    {
                        pageData->UpdateSurface();
                    }
                }
            }
        }
    }

#pragma region State

    /// <summary>
    /// Event handler for store vector changed
    /// </summary>
    /// <param name="e">IVectorChangedEventArgs</param>
    void PdfDocViewModel::StorageVectorChanged(_In_ Windows::Foundation::Collections::IObservableVector<Object^>^ /*sender*/, _In_ Windows::Foundation::Collections::IVectorChangedEventArgs^ e)
    {
        if (isVectorChangedObserved)
        {
            VectorChanged(this, e);
        }
    }
#pragma endregion
}