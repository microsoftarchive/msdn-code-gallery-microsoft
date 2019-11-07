//
// Scenario9.xaml.cpp
// Implementation of the Scenario9 class
//
#include "pch.h"
#include "Scenario9_UpdateSourceTrigger.xaml.h"
#include "Employee.h"

using namespace SDKSample::DataBinding;
using namespace SDKSample;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;

Scenario9::Scenario9()
{
	InitializeComponent();
	_employee = ref new Employee();
	Output->DataContext = _employee;
	_employee->PropertyChanged += ref new Windows::UI::Xaml::Data::PropertyChangedEventHandler(this, &Scenario9::EmployeeChanged);
}

void Scenario9::OnNavigatedTo(NavigationEventArgs^ e)
{
	ScenarioReset(nullptr,nullptr);
}

void SDKSample::DataBinding::Scenario9::ScenarioReset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	_employee->Name = "Jane Doe";
	_employee->Organization = "Contoso";
	LogMessage("");
}

void Scenario9::EmployeeChanged(Object^ sender, Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e)
{
	if (e->PropertyName->Equals("Name"))
	{
		LogMessage("The property '" + e->PropertyName + "' was changed." + "\n\nNew value: " + _employee->Name);
	}
}

void Scenario9::UpdateDataBtnClick(Object^ sender, Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e)
{
	auto expression = NameTxtBox->GetBindingExpression(TextBox::TextProperty);
	expression->UpdateSource();
}

void SDKSample::DataBinding::Scenario9::LogMessage(String^ message)
{
	if (message->Length() > 0)
	{
		LogBorder->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
	else
	{
		LogBorder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
	Log->Text = message;
}
