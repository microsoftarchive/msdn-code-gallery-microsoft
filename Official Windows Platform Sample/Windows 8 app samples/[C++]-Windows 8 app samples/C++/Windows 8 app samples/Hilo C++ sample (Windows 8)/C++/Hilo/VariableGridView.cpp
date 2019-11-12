// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "VariableGridView.h"
#include "IResizable.h"

using namespace Hilo;
using namespace Platform;
using namespace Windows::UI::Xaml::Controls;

void VariableGridView::PrepareContainerForItemOverride(DependencyObject^ element, Object^ item)
{
    auto model = dynamic_cast<IResizable^>(item);

    if (model != nullptr)
    {
        element->SetValue(VariableSizedWrapGrid::ColumnSpanProperty, model->ColumnSpan);
        element->SetValue(VariableSizedWrapGrid::RowSpanProperty, model->RowSpan);
    }

    GridView::PrepareContainerForItemOverride(element, item);
}
