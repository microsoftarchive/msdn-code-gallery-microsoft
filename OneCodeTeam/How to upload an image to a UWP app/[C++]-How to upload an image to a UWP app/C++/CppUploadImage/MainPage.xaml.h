//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"

namespace CppUploadImage
{
	
	public ref class MainPage sealed
	{
	public:
		MainPage();

	private:
		void UploadButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
