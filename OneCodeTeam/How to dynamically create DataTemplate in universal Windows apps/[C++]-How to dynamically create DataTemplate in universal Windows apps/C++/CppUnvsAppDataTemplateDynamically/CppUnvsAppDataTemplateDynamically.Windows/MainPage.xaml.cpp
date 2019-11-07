/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cpp
* Project:      CppUnvsAppDataTemplateDynamically.Windows
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to create DataTemplate dynamically.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#include "pch.h"
#include "MainPage.xaml.h"

using namespace CppUnvsAppDataTemplateDynamically;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;


MainPage::MainPage()
{
	InitializeComponent();

	Windows::Foundation::Collections::IObservableVector<Book^>^  books = Book::GetBooks();
	BookGridView->ItemsSource = books;

	Platform::String^ str = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">";
	str = String::Concat(str, "<Grid Width=\"200\" Height=\"100\">");
	str = String::Concat(str, "<StackPanel>");
	str = String::Concat(str, "<StackPanel Orientation=\"Horizontal\" Margin=\"10,3,0,3\"><TextBlock Text=\"Name:\" Style=\"{StaticResource AppBodyTextStyle}\" Margin=\"0,0,5,0\"/>\
			   			   <TextBlock Text=\"{Binding Name}\" Style=\"{StaticResource AppBodyTextStyle}\"/></StackPanel>");
	str = String::Concat(str, "<StackPanel Orientation=\"Horizontal\" Margin=\"10,3,0,3\"><TextBlock Text=\"Price:\" Style=\"{StaticResource AppBodyTextStyle}\" Margin=\"0,0,5,0\"/>\
			   			   <TextBlock Text=\"{Binding Price}\" Style=\"{StaticResource AppBodyTextStyle}\"/></StackPanel>");
	str = String::Concat(str, "<StackPanel Orientation=\"Horizontal\" Margin=\"10,3,0,3\"><TextBlock Text=\"Author:\" Style=\"{StaticResource AppBodyTextStyle}\" Margin=\"0,0,5,0\"/>\
			   			   <TextBlock Text=\"{Binding Author}\" Style=\"{StaticResource AppBodyTextStyle}\"/></StackPanel>");
	str = String::Concat(str, "</StackPanel>");
	str = String::Concat(str, "</Grid>");
	str = String::Concat(str, "</DataTemplate>");
	
	DataTemplate^ datatemplate = (DataTemplate^)Windows::UI::Xaml::Markup::XamlReader::Load(str);
	BookGridView->ItemTemplate = datatemplate;
}


void MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri(((HyperlinkButton^)sender)->Tag->ToString()));
}


void MainPage::Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
	if (e->NewSize.Width <= 800)
	{
		VisualStateManager::GoToState(this, "MinimalLayout", true);
	}
	else if (e->NewSize.Width < e->NewSize.Height)
	{
		VisualStateManager::GoToState(this, "PortraitLayout", true);
	}
	else
	{
		VisualStateManager::GoToState(this, "DefaultLayout", true);
	}
}
