//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// CustomGridViewItemPresenter.h
// Declaration of the CustomGridViewItemPresenter class
//

#pragma once

#include "pch.h"

using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Shapes;
using namespace Windows::UI::Xaml::Media::Animation;

namespace SDKSample
{
	namespace ListViewSimple
	{
		public ref class CustomGridViewItemPresenter sealed : public ContentPresenter
		{
		public:
			CustomGridViewItemPresenter();

		protected:
			virtual bool GoToElementStateCore(Platform::String^ stateName, bool useTransitions) override;
			virtual void OnApplyTemplate() override;

		private:
			void StartPointerDownAnimation();
			void StopPointerDownAnimation();
			void ShowFocusVisuals();
			void HideFocusVisuals();
			void ShowPointerOverVisuals();
			void HidePointerOverVisuals();
			void CreatePointerDownStoryboard();
			void CreatePointerOverElements();
			void CreateFocusElements();
			bool FocusElementsAreCreated();
			bool PointerOverElementsAreCreated();


			// These are the only objects we need to show item's content and visuals for
			// focus and pointer over state. This is a huge reduction in total elements 
			// over the expanded GridViewItem template. Even better is that these objects
			// are only instantiated when they are needed instead of at startup!
			Grid^ _contentGrid ;
			Rectangle^ _pointerOverBorder ;
			Rectangle^ _focusVisual;			
			GridView^ _parentGridView;

			PointerDownThemeAnimation^ _pointerDownAnimation;
			Storyboard^ _pointerDownStoryboard;
		};
	}
}
