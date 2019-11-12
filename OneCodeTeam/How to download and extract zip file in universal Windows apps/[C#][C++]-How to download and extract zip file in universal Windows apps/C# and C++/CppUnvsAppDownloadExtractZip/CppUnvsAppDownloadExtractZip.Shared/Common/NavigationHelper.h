//
// NavigationHelper.h
// Declaration of the NavigationHelper and associated classes
//

#pragma once

#include "RelayCommand.h"

namespace CppUnvsAppDownloadExtractZip
{
	namespace Common
	{
		/// <summary>
		/// Class used to hold the event data required when a page attempts to load state.
		/// </summary>
		public ref class LoadStateEventArgs sealed
		{
		public:

			/// <summary>
			/// The parameter value passed to <see cref="Frame->Navigate(Type, Object)"/> 
			/// when this page was initially requested.
			/// </summary>
			property Platform::Object^ NavigationParameter
			{
				Platform::Object^ get();
			}

			/// <summary>
			/// A dictionary of state preserved by this page during an earlier
			/// session.  This will be null the first time a page is visited.
			/// </summary>
			property Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ PageState
			{
				Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ get();
			}

		internal:
			LoadStateEventArgs(Platform::Object^ navigationParameter, Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState);

		private:
			Platform::Object^ _navigationParameter;
			Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ _pageState;
		};

		/// <summary>
		/// Represents the method that will handle the <see cref="NavigationHelper->LoadState"/>event
		/// </summary>
		public delegate void LoadStateEventHandler(Platform::Object^ sender, LoadStateEventArgs^ e);

		/// <summary>
		/// Class used to hold the event data required when a page attempts to save state.
		/// </summary>
		public ref class SaveStateEventArgs sealed
		{
		public:

			/// <summary>
			/// An empty dictionary to be populated with serializable state.
			/// </summary>
			property Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ PageState
			{
				Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ get();
			}

		internal:
			SaveStateEventArgs(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState);

		private:
			Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ _pageState;
		};

		/// <summary>
		/// Represents the method that will handle the <see cref="NavigationHelper->SaveState"/>event
		/// </summary>
		public delegate void SaveStateEventHandler(Platform::Object^ sender, SaveStateEventArgs^ e);

		/// <summary>
		/// NavigationHelper aids in navigation between pages.  It provides commands used to 
		/// navigate back and forward as well as registers for standard mouse and keyboard 
		/// shortcuts used to go back and forward in Windows and the hardware back button in
		/// Windows Phone.  In addition it integrates SuspensionManger to handle process lifetime
		/// management and state management when navigating between pages.
		/// </summary>
		/// <example>
		/// To make use of NavigationHelper, follow these two steps or
		/// start with a BasicPage or any other Page item template other than BlankPage.
		/// 
		/// 1) Create an instance of the NavigationHelper somewhere such as in the 
		///		constructor for the page and register a callback for the LoadState and 
		///		SaveState events.
		/// <code>
		///		MyPage::MyPage()
		///		{
		///			InitializeComponent();
		///			auto navigationHelper = ref new Common::NavigationHelper(this);
		///			navigationHelper->LoadState += ref new Common::LoadStateEventHandler(this, &MyPage::LoadState);
		///			navigationHelper->SaveState += ref new Common::SaveStateEventHandler(this, &MyPage::SaveState);
		///		}
		///		
		///		void MyPage::LoadState(Object^ sender, Common::LoadStateEventArgs^ e)
		///		{ }
		///		void MyPage::SaveState(Object^ sender, Common::SaveStateEventArgs^ e)
		///		{ }
		/// </code>
		/// 
		/// 2) Register the page to call into the NavigationHelper whenever the page participates 
		///		in navigation by overriding the <see cref="Windows::UI::Xaml::Controls::Page::OnNavigatedTo"/> 
		///		and <see cref="Windows::UI::Xaml::Controls::Page::OnNavigatedFrom"/> events.
		/// <code>
		///		void MyPage::OnNavigatedTo(NavigationEventArgs^ e)
		///		{
		///			NavigationHelper->OnNavigatedTo(e);
		///		}
		///
		///		void MyPage::OnNavigatedFrom(NavigationEventArgs^ e)
		///		{
		///			NavigationHelper->OnNavigatedFrom(e);
		///		}
		/// </code>
		/// </example>
		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class NavigationHelper sealed
		{
		public:
			/// <summary>
			/// <see cref="RelayCommand"/> used to bind to the back Button's Command property
			/// for navigating to the most recent item in back navigation history, if a Frame
			/// manages its own navigation history.
			/// 
			/// The <see cref="RelayCommand"/> is set up to use the virtual method <see cref="GoBack"/>
			/// as the Execute Action and <see cref="CanGoBack"/> for CanExecute.
			/// </summary>
			property RelayCommand^ GoBackCommand
			{
				RelayCommand^ get();
			}

			/// <summary>
			/// <see cref="RelayCommand"/> used for navigating to the most recent item in 
			/// the forward navigation history, if a Frame manages its own navigation history.
			/// 
			/// The <see cref="RelayCommand"/> is set up to use the virtual method <see cref="GoForward"/>
			/// as the Execute Action and <see cref="CanGoForward"/> for CanExecute.
			/// </summary>
			property RelayCommand^ GoForwardCommand
			{
				RelayCommand^ get();
			}

		internal:
			NavigationHelper(Windows::UI::Xaml::Controls::Page^ page,
				RelayCommand^ goBack = nullptr,
				RelayCommand^ goForward = nullptr);

			bool CanGoBack();
			void GoBack();
			bool CanGoForward();
			void GoForward();

			void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e);
			void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e);

			event LoadStateEventHandler^ LoadState;
			event SaveStateEventHandler^ SaveState;

		private:
			Platform::WeakReference _page;

			RelayCommand^ _goBackCommand;
			RelayCommand^ _goForwardCommand;

#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP
			Windows::Foundation::EventRegistrationToken _backPressedEventToken;

			void HardwareButton_BackPressed(Platform::Object^ sender,
				Windows::Phone::UI::Input::BackPressedEventArgs^ e);
#else
			bool _navigationShortcutsRegistered;
			Windows::Foundation::EventRegistrationToken _acceleratorKeyEventToken;
			Windows::Foundation::EventRegistrationToken _pointerPressedEventToken;

			void CoreDispatcher_AcceleratorKeyActivated(Windows::UI::Core::CoreDispatcher^ sender,
				Windows::UI::Core::AcceleratorKeyEventArgs^ e);
			void CoreWindow_PointerPressed(Windows::UI::Core::CoreWindow^ sender,
				Windows::UI::Core::PointerEventArgs^ e);
#endif

			Platform::String^ _pageKey;
			Windows::Foundation::EventRegistrationToken _loadedEventToken;
			Windows::Foundation::EventRegistrationToken _unloadedEventToken;
			void OnLoaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OnUnloaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

			~NavigationHelper();
		};
	}
}
