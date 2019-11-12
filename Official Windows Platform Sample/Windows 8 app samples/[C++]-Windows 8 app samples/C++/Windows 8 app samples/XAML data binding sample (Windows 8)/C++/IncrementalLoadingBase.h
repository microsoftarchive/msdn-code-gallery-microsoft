// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// IncrementalLoadingBase.h
// Declaration of the IncrementalLoadingBase class
//

#pragma once

#include <ppltasks.h>
#include <collection.h>

namespace SDKSample
{
    namespace DataBinding
    {
		// This class can used as a jumpstart for implementing ISupportIncrementalLoading. 
    	// Implementing the ISupportIncrementalLoading interfaces allows you to create a list that loads
    	//  more data automatically when the user scrolls to the end of of a GridView or ListView.
		ref class IncrementalLoadingBase
    		: Windows::UI::Xaml::Interop::IBindableObservableVector, 
    		Windows::UI::Xaml::Data::ISupportIncrementalLoading
    	{
    #pragma region Windows::UI::Xaml::Data::ISupportIncrementalLoading

    	internal:
    		Concurrency::task<Windows::UI::Xaml::Data::LoadMoreItemsResult> LoadMoreItemsAsync(Concurrency::cancellation_token c, unsigned int count)
    		{
    			try
    			{
    				return Concurrency::task<void>([=]() {})
    					.then([=]() {
    						return LoadMoreItemsOverride(c, count);
    				})
    					.then([=](Windows::Foundation::Collections::IVector<Platform::Object^>^ items) -> Windows::UI::Xaml::Data::LoadMoreItemsResult {
    						auto baseIndex = _storage->Size;

    						for(unsigned int i=0; i<items->Size; i++)
    						{
    							_storage->Append(items->GetAt(i));
    						}

    						Windows::UI::Xaml::Data::LoadMoreItemsResult result;
    						result.Count = items->Size;
    						return result;
    				}, Concurrency::task_continuation_context::use_current());
    			}
    			catch(Platform::Exception^ e)
    			{
    				throw e;
    			}
    		}
    		property Platform::Object^ default[int]
    		{
    			Platform::Object^ get(int index)
    			{
    				return _storage->GetAt(index);
    			}
    			void set(int index, Platform::Object^ value)
    			{
    				_storage->SetAt(index, value);
    			}
    		}

    #pragma region Overridable methods

    		virtual Concurrency::task<Windows::Foundation::Collections::IVector<Platform::Object^>^> LoadMoreItemsOverride(Concurrency::cancellation_token c, unsigned int count)
    		{
    			return Concurrency::task<Windows::Foundation::Collections::IVector<Platform::Object^>^>(
					[=]() -> Windows::Foundation::Collections::IVector<Platform::Object^>^ {
    					auto items = ref new Platform::Collections::Vector<Platform::Object^>();
    					return items;
    			});
    		}
    		virtual bool HasMoreItemsOverride()
    		{
    			return false;
    		}

    #pragma endregion 

    		IncrementalLoadingBase()
    		{
    			_storage = ref new Platform::Collections::Vector<Platform::Object^>();
    			_storage->VectorChanged += ref new Windows::Foundation::Collections::VectorChangedEventHandler<Platform::Object^>(this, &IncrementalLoadingBase::_storageVectorChanged);
    			_busy = false;
    			_isVectorChangedObserved = false;
    		}

    	public:

    		virtual Windows::Foundation::IAsyncOperation<Windows::UI::Xaml::Data::LoadMoreItemsResult>^ LoadMoreItemsAsync(unsigned int count)
    		{
    			if (_busy)
    			{
    				throw ref new Platform::FailureException("Only one operation in flight at a time");
    			}

    			_busy = true;

    			return Concurrency::create_async([=](Concurrency::cancellation_token c) {
    				return LoadMoreItemsAsync(c, count)
    					.then([=](Windows::UI::Xaml::Data::LoadMoreItemsResult result) -> Windows::UI::Xaml::Data::LoadMoreItemsResult {
    						_busy = false;
    						return result;
    				});
    			});
    		}

    		property bool HasMoreItems
    		{
    			virtual bool get() { return HasMoreItemsOverride(); }
    		}

    #pragma endregion 

    #pragma region IBindableObservableVector

    		virtual event Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ VectorChanged
    		{
    			virtual Windows::Foundation::EventRegistrationToken add(Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ e)
    			{
    				_isVectorChangedObserved = true;
    				return _privateVectorChanged += e;
    			}

    			virtual void remove(Windows::Foundation::EventRegistrationToken t)
    			{
    				_privateVectorChanged -= t;
    			}
    			virtual void raise(Windows::UI::Xaml::Interop::IBindableObservableVector^ vector, Platform::Object^ e)
    			{
    				if(_isVectorChangedObserved)
    				{
    					_privateVectorChanged(vector, e);
    				}
    			}
    		}

    #pragma endregion 

    #pragma region Windows::UI::Xaml::Interop::IBindableIterator

    		virtual Windows::UI::Xaml::Interop::IBindableIterator^ First()
    		{
    			return dynamic_cast<Windows::UI::Xaml::Interop::IBindableIterator^>(_storage->First());
    		}

    #pragma endregion

    #pragma region Windows::UI::Xaml::Interop::IBindableVector 

    		virtual void Append(Platform::Object^ value)
    		{
    			_storage->Append(value);
    		}

    		virtual void Clear()
    		{
    			_storage->Clear();
    		}

    		virtual Platform::Object^ GetAt(unsigned int index)
    		{
    			return _storage->GetAt(index);
    		}

    		virtual Windows::UI::Xaml::Interop::IBindableVectorView^ GetView()
    		{
    			return safe_cast<Windows::UI::Xaml::Interop::IBindableVectorView^>(_storage->GetView());
    		}

    		virtual bool IndexOf(Platform::Object^ value, unsigned int* index)
    		{
    			return _storage->IndexOf(value, index);
    		}

    		virtual void InsertAt(unsigned int index, Platform::Object^ value)
    		{
    			_storage->InsertAt(index, value);
    		}

    		virtual void RemoveAt(unsigned int index)
    		{
    			_storage->RemoveAt(index);
    		}

    		virtual void RemoveAtEnd()
    		{
    			_storage->RemoveAtEnd();
    		}

    		virtual void SetAt(unsigned int index, Platform::Object^ value)
    		{
    			_storage->SetAt(index, value);
    		}

    		virtual property unsigned int Size
    		{
    			unsigned int get() { return _storage->Size; }
    		}


    #pragma endregion

    #pragma region State

    	private:
    		Platform::Collections::Vector<Platform::Object^>^ _storage;
    		bool _busy;
    		bool _isVectorChangedObserved;
    		event Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ _privateVectorChanged;

    		/// <summary>
    		/// 
    		/// </summary>
    		/// <param name="e">IVectorChangedEventArgs</param>
    		void _storageVectorChanged(Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ sender, Windows::Foundation::Collections::IVectorChangedEventArgs^ e)
    		{
    			if (_isVectorChangedObserved)
    			{
    				VectorChanged(this, e);
    			}
    		}

    #pragma endregion 
    	};

    }
}