// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "RotateImageView.xaml.h"
#include "ImageBrowserViewModel.h"

using namespace Hilo;
using namespace Windows::Foundation;
using namespace Windows::UI::Input;
using namespace Windows::UI::Xaml::Input;

// See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

RotateImageView::RotateImageView() 
{
    InitializeComponent();
    m_viewModel = dynamic_cast<Hilo::RotateImageViewModel^>(DataContext);
}

void RotateImageView::OnManipulationDelta(ManipulationDeltaRoutedEventArgs^ e)
{
    m_viewModel->RotationAngle += e->Delta.Rotation;
}

void RotateImageView::OnManipulationCompleted(ManipulationCompletedRoutedEventArgs^ e)
{
    m_viewModel->EndRotation();
}
