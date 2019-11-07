// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// GeneratorIncrementalLoadingClass.h
// Declaration of the GeneratorIncrementalLoadingClass class
//

#pragma once

#include <ppltasks.h>
#include <collection.h>
#include "IncrementalLoadingBase.h"
#include "Employee.h"

namespace SDKSample
{
    namespace DataBinding
    {	
        // This class implements IncrementalLoadingBase. 
        // To create your own Infinite List, you can create a class like this one that doesn't have 'generator' or 'maxcount', 
        //  and instead downloads items from a live data source in LoadMoreItemsOverrideAsync.
        ref class GeneratorIncrementalLoadingClass : public DataBinding::IncrementalLoadingBase
        {
#pragma region State
        internal:
            unsigned int _count;
            unsigned int _maxCount;
#pragma endregion

        internal:
            GeneratorIncrementalLoadingClass(unsigned int maxCount)
            {
                _maxCount = maxCount;
                _count = 0;
            }

            virtual Concurrency::task<Windows::Foundation::Collections::IVector<Platform::Object^>^> LoadMoreItemsOverride(Concurrency::cancellation_token c, unsigned int count) override
            {
                return Concurrency::task<Windows::Foundation::Collections::IVector<Platform::Object^>^>(
                    [=]() -> Windows::Foundation::Collections::IVector<Platform::Object^>^ {
                        unsigned int toGenerate = _maxCount - _count;

                        ///
                        ///  Replace this section with your code to load more items
                        ///
                        Concurrency::wait(10);

                        if (count < toGenerate)
                        {
                            toGenerate = count;
                        }

                        /// Note that Platform::Collections::Vector<Platform::Object^> can be returned as Windows::Foundation::Collections::IVector<Platform::Object^>
                        auto items = ref new Platform::Collections::Vector<Platform::Object^>();
                        for(unsigned int i=0; i < toGenerate; i++)
                        {
                            auto item = this->Generator(this->_count++);
                            items->Append(item);
                        }

                        return items;

                        ///
                        /// end Replace this section with your code to load more items
                        ///
                    }
                );
            }

            virtual bool HasMoreItemsOverride() override
            {
                return _count < _maxCount;
            }

        private:
            Platform::Object^ Generator(int count)
            {
                auto employee = ref new Employee();
                employee->Name = "Name" + count;
                employee->Organization = "Organization" + count;
                return employee;
            }
        };
    }
}
