//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// CustomGridViewItemPresenter.cpp
// Implementation of the CustomGridViewItemPresenter class
//

#include "pch.h"
#include "CustomGridViewItemPresenter.h"

using namespace SDKSample::ListViewSimple;

using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Animation;

CustomGridViewItemPresenter::CustomGridViewItemPresenter()
{
	_contentGrid = nullptr;
	_pointerOverBorder = nullptr;
	_focusVisual = nullptr;
	_pointerDownAnimation = nullptr;
	_pointerDownStoryboard = nullptr;
}

bool CustomGridViewItemPresenter::GoToElementStateCore(Platform::String^ stateName, bool useTransitions)
{
	FrameworkElement::GoToElementStateCore(stateName, useTransitions);

	// change the visuals shown based on the state the item is going to
	if (stateName->Equals("Normal"))
	{
		HidePointerOverVisuals();
		HideFocusVisuals();
		if (useTransitions)
		{
			StopPointerDownAnimation();
		}
	}
	else
	{
		if (stateName->Equals("Focused") || stateName->Equals("PointerFocused"))
		{
			ShowFocusVisuals();
		}
		else
		{
			if (stateName->Equals("Unfocused"))
			{
				HideFocusVisuals();
			}
			else
			{
				if (stateName->Equals("PointerOver"))
				{
					ShowPointerOverVisuals();
					if (useTransitions)
					{
						StopPointerDownAnimation();
					}
				}
				else
				{
					if (stateName->Equals("Pressed") || stateName->Equals("PointerOverPressed"))
					{
						if (useTransitions)
						{
							StartPointerDownAnimation();
						}
						// this sample does not deal with the DataAvailable, NotDragging, NoReorderHint, NoSelectionHint,
						// Unselected, SelectedUnfocused, or UnselectedPointerOver states
					}
				}
			}
		}
	}
	return true;
}

void CustomGridViewItemPresenter::StartPointerDownAnimation()
{
	// create the storyboard for the pointer down animation if it doesn't exist 
	if (_pointerDownStoryboard == nullptr)
	{
		CreatePointerDownStoryboard();
	}

	// start the storyboard for the pointer down animation 
	_pointerDownStoryboard->Begin();
}

void CustomGridViewItemPresenter::StopPointerDownAnimation()
{
	// stop the pointer down animation
	if (_pointerDownStoryboard != nullptr)
	{
		_pointerDownStoryboard->Stop();
	}
}

void CustomGridViewItemPresenter::ShowFocusVisuals()
{
	// create the elements necessary to show focus visuals if they have
	// not been created yet.       
	if (!FocusElementsAreCreated())
	{
		CreateFocusElements();
	}

	// make sure the elements necessary to show focus visuals are opaque
	_focusVisual->Opacity = 1;
}

void CustomGridViewItemPresenter::HideFocusVisuals()
{
	// hide the elements that visualize focus if they have been created
	if (FocusElementsAreCreated())
	{
		_focusVisual->Opacity = 0;
	}
}

void CustomGridViewItemPresenter::ShowPointerOverVisuals()
{
	// create the elements necessary to show pointer over visuals if they have
	// not been created yet.       
	if (!PointerOverElementsAreCreated())
	{
		CreatePointerOverElements();
	}

	// make sure the elements necessary to show pointer over visuals are opaque
	_pointerOverBorder->Opacity = 1;
}

void CustomGridViewItemPresenter::HidePointerOverVisuals()
{
	// hide the elements that visualize pointer over if they have been created
	if (PointerOverElementsAreCreated())
	{
		_pointerOverBorder->Opacity = 0;
	}
}

void CustomGridViewItemPresenter::CreatePointerDownStoryboard()
{
	_pointerDownAnimation = ref new PointerDownThemeAnimation();
	Storyboard::SetTarget(_pointerDownAnimation, _contentGrid);

	_pointerDownStoryboard = ref new Storyboard();
	_pointerDownStoryboard->Children->Append(_pointerDownAnimation);
}

void CustomGridViewItemPresenter::CreatePointerOverElements()
{
	// create the "border" which is really a Rectangle with the correct attributes
	_pointerOverBorder = ref new Rectangle();
	_pointerOverBorder->IsHitTestVisible = false;
	_pointerOverBorder->Opacity = 0;
	// note that this uses a statically declared brush and will not respond to changes in high contrast
	_pointerOverBorder->Fill = (SolidColorBrush^) _parentGridView->Resources->Lookup("PointerOverBrush");

	// add the pointer over visuals on top of all children of _InnerDragContent
	_contentGrid->Children->InsertAt(_contentGrid->Children->Size, _pointerOverBorder);
}

void CustomGridViewItemPresenter::CreateFocusElements()
{
	// create the focus visual which is a Rectangle with the correct attributes
	_focusVisual = ref new Rectangle();
	_focusVisual->IsHitTestVisible = false;
	_focusVisual->Opacity = 0;
	_focusVisual->StrokeThickness = 2;
	// note that this uses a statically declared brush and will not respond to changes in high contrast
	_focusVisual->Stroke = (SolidColorBrush^) _parentGridView->Resources->Lookup("FocusBrush");

	// add the focus elements behind all children of _InnerDragContent
	_contentGrid->Children->InsertAt(0, _focusVisual);
}

bool CustomGridViewItemPresenter::FocusElementsAreCreated()
{
	return _focusVisual != nullptr;
}

bool CustomGridViewItemPresenter::PointerOverElementsAreCreated()
{
	return _pointerOverBorder != nullptr;
}

void CustomGridViewItemPresenter::OnApplyTemplate()
{
	// call the base method
	ContentPresenter::OnApplyTemplate();

	auto obj = VisualTreeHelper::GetParent(this);
	GridView^ gv = dynamic_cast<GridView^>(obj);
	while(gv == nullptr)
	{
		obj = VisualTreeHelper::GetParent(obj);
		gv = dynamic_cast<GridView^>(obj);
	}
	_parentGridView = gv;

	_contentGrid = (Grid^) VisualTreeHelper::GetChild(this, 0);
}
