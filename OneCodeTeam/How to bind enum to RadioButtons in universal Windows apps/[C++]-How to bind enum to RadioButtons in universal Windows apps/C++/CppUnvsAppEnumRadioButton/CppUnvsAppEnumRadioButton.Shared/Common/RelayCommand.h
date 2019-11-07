//
// NavigationHelper.cpp
// Decleration of the NavigationHelper and associated classes
//

#pragma once

// <summary>
// A command whose sole purpose is to relay its functionality 
// to other objects by invoking delegates. 
// The default return value for the CanExecute method is 'true'.
// <see cref="RaiseCanExecuteChanged"/> needs to be called whenever
// <see cref="CanExecute"/> is expected to return a different value.
// </summary>


namespace CppUnvsAppEnumRadioButton
{
	namespace Common
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class RelayCommand sealed :[Windows::Foundation::Metadata::Default] Windows::UI::Xaml::Input::ICommand
		{
		public:
			virtual event Windows::Foundation::EventHandler<Object^>^ CanExecuteChanged;
			virtual bool CanExecute(Object^ parameter);
			virtual void Execute(Object^ parameter);
			virtual ~RelayCommand();

		internal:
			RelayCommand(std::function<bool(Platform::Object^)> canExecuteCallback,
				std::function<void(Platform::Object^)> executeCallback);
			void RaiseCanExecuteChanged();

		private:
			std::function<bool(Platform::Object^)> _canExecuteCallback;
			std::function<void(Platform::Object^)> _executeCallback;
		};
	}
}