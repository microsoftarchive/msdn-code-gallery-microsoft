/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cpp
* Project:      CppUnivsAppHtmlBinding.WindowsPhone
* Copyright (c) Microsoft Corporation.
*
* This code sample shows how to bind HTML from a data model to a WebView.
* For more details, please refer to:
* http://blogs.msdn.com/b/wsdevsol/archive/2013/09/26/binding-html-to-a-webview-with-attached-properties.aspx
*
* MainPage.xaml.h
* Declaration of the MainPage class.
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

using namespace CppUnvsAppHtmlBinding;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Platform::Collections;


MainPage::MainPage()
{
	InitializeComponent();

	HTMLStrings = ref new Vector<HTMLData^>();

	HTMLStrings->Append(ref new HTMLData("Item One", 
		"<!DOCTYPE html><html><body><h1>Sample HTML One</h1><p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla quis leo lobortis, aliquam diam ut, sagittis elit. Aliquam scelerisque dictum lectus, vitae aliquet velit facilisis vitae. Mauris hendrerit, lacus vitae sollicitudin dignissim, purus felis laoreet leo, non sollicitudin nulla libero bibendum lorem. Donec at tellus suscipit tortor aliquam  pretium a et neque. Pellentesque id lobortis nisl. Cras laoreet luctus diam, non viverra turpis commodo a. Phasellus non enim vestibulum, convallis enim a, molestie est. Etiam at placerat tortor. </p>    </body></html>"));
	HTMLStrings->Append(ref new HTMLData("Item Two", 
		"<!DOCTYPE html><html><body><h1>Sample HTML Two</h1><p>Nullam vestibulum elit nec nunc tincidunt tristique. Praesent et ante eget mauris sodales egestas eu eu felis. Morbi tincidunt sem sit amet hendrerit imperdiet. Duis aliquam tortor odio, sit amet sollicitudin ante consectetur et. Nullam feugiat vulputate erat, ac feugiat nibh blandit porttitor. Morbi lorem arcu, tincidunt vitae egestas sed, faucibusullamcorper orci. Integer commodo rhoncus neque, ac iaculis nibh laoreet ac. Sed sapien dui, pretium in arcu vel, lobortis rhoncus felis. Curabiturluctus sem a laoreet porttitor. Nunc id nunc varius orci tristique pharetra sit amet ac lorem.</p></body></html>"));
	HTMLStrings->Append(ref new HTMLData("Item Three", 
		"<!DOCTYPE html><html><body><h1>Sample HTML Three</h1><p>Nunc facilisis gravida vulputate. Sed in hendrerit velit. Vivamus vel libero eu nisi sodales viverra. Nullam id eros suscipit, semper dolor non, fringilla nunc. Mauris quis nisl vel mauris egestas laoreet sit amet eu velit. Etiam vel placerat nisl. Pellentesque non dolor diam. Etiam lectus elit, pellentesque eu tempus non, bibendum a odio. Fusce ac nunc cursus, iaculis tortor eu, hendrerit leo.</p></body></html>"));
	HTMLStrings->Append(ref new HTMLData("Item Four", 
		"<!DOCTYPE html><html><body><h1>Sample HTML Four</h1><p>Ut volutpat eget nunc ut fringilla. Vestibulum ac neque nunc. Morbi accumsan, nisi eu commodo imperdiet, sem justo ultrices lorem, sed tempor dui est eget metus. Mauris at turpis id urna rhoncustincidunt. Nam malesuada risus in sapien placerat, vitae rhoncus sem volutpat. Aenean imperdiet, eros at congue bibendum, felis neque vulputate est, pharetra aliquam lacus arcu vel turpis. Mauris faucibusmetus adipiscing, convallis lectus condimentum, rhoncus massa. Donec nibh mauris, auctor et molestie et, vulputate ut tortor. Maecenasfermentum consectetur purus vitae vestibulum. Aenean id posuere ipsum.Quisque molestie sodales libero nec vulputate. Aenean dui nisl,convallis eu lorem a, tempus pharetra diam. Suspendisse tempor nequeeros, ut venenatis nisi laoreet ut. Vestibulum eleifend sem eu mollispellentesque. Sed sed luctus ipsum. Morbi vitae luctus sem.</p></body></html>"));
	this->HtmlSource->Source = HTMLStrings;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	(void) e;	// Unused parameter

	// TODO: Prepare page for display here.

	// TODO: If your application contains multiple pages, ensure that you are
	// handling the hardware Back button by registering for the
	// Windows::Phone::UI::Input::HardwareButtons.BackPressed event.
	// If you are using the NavigationHelper provided by some templates,
	// this event is handled for you.
}


void MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri("http://blogs.msdn.com/b/onecode"));
}
