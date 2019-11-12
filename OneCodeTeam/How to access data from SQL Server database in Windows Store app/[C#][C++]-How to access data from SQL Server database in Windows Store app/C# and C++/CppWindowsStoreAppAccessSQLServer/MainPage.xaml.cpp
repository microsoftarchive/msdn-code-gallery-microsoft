/******************************* Module Header ******************************\
* Module Name:  MainPage.xaml.cpp
* Project:      CppWindowsStoreAppAccessSQLServer
* Copyright (c) Microsoft Corporation.
*
* ​The sample demonstrates how to access data from SQL Server database in Windows Store app. 
* We cannot access SQL Server Database from Windows Store app directly. We have to 
* create a Service layer to access the database. 
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
****************************************************************************/

#include "pch.h"
#include "MainPage.xaml.h"

using namespace CPPWindowsStoreAppAccessSQLServer;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Web::Http;
using namespace Windows::Data::Json;
using namespace Windows::Data::Xml::Dom;

using namespace concurrency;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

MainPage::MainPage()
{
	InitializeComponent();
	SetValue(_defaultViewModelProperty, ref new Map<String^,Object^>(std::less<String^>()));
	auto navigationHelper = ref new Common::NavigationHelper(this);
	SetValue(_navigationHelperProperty, navigationHelper);
	navigationHelper->LoadState += ref new Common::LoadStateEventHandler(this, &MainPage::LoadState);
	navigationHelper->SaveState += ref new Common::SaveStateEventHandler(this, &MainPage::SaveState);
	Window::Current->SizeChanged+=ref new Windows::UI::Xaml::WindowSizeChangedEventHandler(this, &CPPWindowsStoreAppAccessSQLServer::MainPage::OnSizeChanged);
}

DependencyProperty^ MainPage::_defaultViewModelProperty =
	DependencyProperty::Register("DefaultViewModel",
		TypeName(IObservableMap<String^,Object^>::typeid), TypeName(MainPage::typeid), nullptr);

/// <summary>
/// used as a trivial view model.
/// </summary>
IObservableMap<String^, Object^>^ MainPage::DefaultViewModel::get()
{
	return safe_cast<IObservableMap<String^, Object^>^>(GetValue(_defaultViewModelProperty));
}

DependencyProperty^ MainPage::_navigationHelperProperty =
	DependencyProperty::Register("NavigationHelper",
		TypeName(Common::NavigationHelper::typeid), TypeName(MainPage::typeid), nullptr);

/// <summary>
/// Gets an implementation of <see cref="NavigationHelper"/> designed to be
/// used as a trivial view model.
/// </summary>
Common::NavigationHelper^ MainPage::NavigationHelper::get()
{
	return safe_cast<Common::NavigationHelper^>(GetValue(_navigationHelperProperty));
}

#pragma region Navigation support

/// The methods provided in this section are simply used to allow
/// NavigationHelper to respond to the page's navigation methods.
/// 
/// Page specific logic should be placed in event handlers for the  
/// <see cref="NavigationHelper::LoadState"/>
/// and <see cref="NavigationHelper::SaveState"/>.
/// The navigation parameter is available in the LoadState method 
/// in addition to page state preserved during an earlier session.

void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	NavigationHelper->OnNavigatedTo(e);
}

void MainPage::OnNavigatedFrom(NavigationEventArgs^ e)
{
	NavigationHelper->OnNavigatedFrom(e);
}

#pragma endregion

/// <summary>
/// Populates the page with content passed during navigation. Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="sender">
/// The source of the event; typically <see cref="NavigationHelper"/>
/// </param>
/// <param name="e">Event data that provides both the navigation parameter passed to
/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
/// a dictionary of state preserved by this page during an earlier
/// session. The state will be null the first time a page is visited.</param>
void MainPage::LoadState(Object^ sender, Common::LoadStateEventArgs^ e)
{
	(void) sender;	// Unused parameter
	(void) e;	// Unused parameter
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
/// <param name="e">Event data that provides an empty dictionary to be populated with
/// serializable state.</param>
void MainPage::SaveState(Object^ sender, Common::SaveStateEventArgs^ e){
	(void) sender;	// Unused parameter
	(void) e; // Unused parameter
}

// Get data from SQL Server by WCF Service
void CPPWindowsStoreAppAccessSQLServer::MainPage::GetButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

	// Clear Error message
	this->NotifyUser("");

	// Create httpClient object
	HttpClient^ httpClient = ref new HttpClient();
	Uri^ uri = ref new Uri("http://localhost:42920/Service.svc/querySql");

	// async send "get" request to get response string form service
	IAsyncOperationWithProgress<String^, HttpProgress> ^accessSQLOp= httpClient->GetStringAsync(uri);
	auto operationTask = create_task(accessSQLOp);
	operationTask.then([this](String^ response){	
		Vector<Article^>^ vector = ref new Vector<Article^>();

		// Parse json string to object
		JsonValue^ jsonValue = JsonValue::Parse(response);
		if (jsonValue->GetObject()->GetNamedBoolean("queryParam") == true)
		{
			// Convert xml string to vector<Article^> object
			String^ resulttxt = jsonValue->GetObject()->GetNamedString("querySqlResult");
			XmlDocument^ xdoc = ref new XmlDocument();
			xdoc->LoadXml(resulttxt);
			XmlNodeList^  nodelist = xdoc->GetElementsByTagName("Table");
			for each (auto var in nodelist)
			{
				Article^ item = ref new Article();
				item->Title = var->ChildNodes->Item(0)->InnerText;
				item->Text = var->ChildNodes->Item(1)->InnerText;

				// Insert an item
				vector->Append(item);
			}

			// Set ItemsSource of ListView control
			lvDataTemplates->ItemsSource = vector;
		}
		else
		{
			NotifyUser(L"Error occurs. Please make sure the database has been attached to SQL Server!");
		}
	});
}

// The event handler for the click event of the link in the footer. 
void CPPWindowsStoreAppAccessSQLServer::MainPage::HyperlinkButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri("http://blogs.msdn.com/b/onecode"));
}

void CPPWindowsStoreAppAccessSQLServer::MainPage::NotifyUser(Platform::String^ message)
{
	StatusBlock->Text = message;
}

// Handle Size change event
void CPPWindowsStoreAppAccessSQLServer::MainPage::OnSizeChanged(Platform::Object ^sender, Windows::UI::Core::WindowSizeChangedEventArgs ^e)
{
	if (e->Size.Width <= 500)
	{
		VisualStateManager::GoToState(this, "MinimalLayout", true);
	}
	else if (e->Size.Width < e->Size.Height)
	{
		VisualStateManager::GoToState(this, "PortraitLayout", true);
	}
	else
	{
		VisualStateManager::GoToState(this, "DefaultLayout", true);
	}
}
