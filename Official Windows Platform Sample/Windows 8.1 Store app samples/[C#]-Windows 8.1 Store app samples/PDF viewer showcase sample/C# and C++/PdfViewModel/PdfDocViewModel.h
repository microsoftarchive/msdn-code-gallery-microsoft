// Copyright (c) Microsoft Corporation. All rights reserved

//
// PdfDocViewModel.h
// Declaration of the PdfDocViewModel class
//
#pragma once
#include "PdfViewContext.h"

namespace PdfViewModel
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PdfDocViewModel sealed : Windows::UI::Xaml::Interop::IBindableObservableVector
    {
    public:
        PdfDocViewModel(_In_ Windows::Data::Pdf::PdfDocument^ pdfDocument, _In_ Windows::Foundation::Size pageSize, _In_ SurfaceType surfaceType);

#pragma region IBindableObservableVector

        virtual event Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ VectorChanged
        {
            virtual Windows::Foundation::EventRegistrationToken add(_In_ Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ e);
            virtual void remove(_In_ Windows::Foundation::EventRegistrationToken t);
        protected:
            void raise(_In_ Windows::UI::Xaml::Interop::IBindableObservableVector^ vector, _In_ Platform::Object^ e);
        }

#pragma endregion

#pragma region Windows::UI::Xaml::Interop::IBindableIterator

        virtual Windows::UI::Xaml::Interop::IBindableIterator^ First();

#pragma endregion

#pragma region Windows::UI::Xaml::Interop::IBindableVector

        virtual void Append(_In_ Platform::Object^ value);
        virtual void Clear();
        virtual Platform::Object^ GetAt(_In_ unsigned int index);
        virtual Windows::UI::Xaml::Interop::IBindableVectorView^ GetView();
        virtual bool IndexOf(_In_ Platform::Object^ value, _In_ unsigned int* index);
        virtual void InsertAt(_In_ unsigned int index, _In_ Platform::Object^ value);
        virtual void RemoveAt(_In_ unsigned int index);
        virtual void RemoveAtEnd();
        virtual void SetAt(_In_ unsigned int index, _In_ Platform::Object^ value);
        virtual property unsigned int Size 
        {
            unsigned int get(); 
        }

#pragma endregion

        void Trim();
        void UpdatePages(_In_ float currentZoomFactor);

    private:
        Object^ CreateEmptyItem(_In_ unsigned int index);
        void RealizeItemIfNotAvailable(_In_ unsigned int index, _In_ Platform::Object^ item);

#pragma region State

    private:
        Platform::Collections::Vector<Platform::Object^>^ storage;
        bool isVectorChangedObserved;
        event Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ privateVectorChanged;
        void StorageVectorChanged(_In_ Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ sender, _In_ Windows::Foundation::Collections::IVectorChangedEventArgs^ e);
        PdfViewContext^ pdfViewContext;
        Windows::Data::Pdf::PdfDocument^ pdfDocument;

#pragma endregion
    };
}
