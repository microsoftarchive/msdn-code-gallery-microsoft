//
// Scenario10.xaml.cpp
// Implementation of the Scenario10 class
//

#include "pch.h"
#include "Scenario10_FallbackValueAndTargetNullValue.xaml.h"
#include "Employee.h"

using namespace SDKSample;
using namespace SDKSample::DataBinding;

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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

Scenario10::Scenario10()
{
	InitializeComponent();

	_employee = ref new Employee();
	Output->DataContext = _employee;

	_employee->PropertyChanged += ref new PropertyChangedEventHandler(this, &Scenario10::EmployeeChanged);
}

void Scenario10::OnNavigatedTo(NavigationEventArgs^ e)
{
	// A pointer back to the main page.  This is needed if you want to call methods in MainPage such
	// as NotifyUser()
	rootPage = MainPage::Current;
	ScenarioReset(nullptr, nullptr);
	ShowEmployeeInfo(EmployeeDataModel->Text);
}

void SDKSample::DataBinding::Scenario10::ScenarioReset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	_employee->Name = "Jane Doe";
	_employee->Organization = "Contoso";
	_employee->Age = nullptr;

	//To reset Bindings with NullTargetValue and FallbackValue is necessary to reassign the Bindings
	auto ageBindingExp = AgeTextBox->GetBindingExpression(TextBox::TextProperty);
	auto ageBinding = ageBindingExp->ParentBinding;
	AgeTextBox->SetBinding(TextBox::TextProperty, ageBinding);

	auto salaryBindingExp = SalaryTextBox->GetBindingExpression(TextBox::TextProperty);
	auto salaryBinding = salaryBindingExp->ParentBinding;
	SalaryTextBox->SetBinding(TextBox::TextProperty, salaryBinding);

	rootPage->NotifyUser("", NotifyType::StatusMessage);

}

void Scenario10::EmployeeChanged(Object^ sender, PropertyChangedEventArgs^ e)
{
	String^ text = "The property '" + e->PropertyName + "' changed.";
	text += "\n\nNew values are:\n";
	ShowEmployeeInfo(text);
}

void Scenario10::ShowEmployeeInfo(String^ content)
{

	content += "\nName: " + _employee->Name;
	content += "\nOrganization: " + _employee->Organization;
	if (_employee->Age == nullptr)
	{
		content += "\nAge: Null";
	}
	else
	{
		content += "\nAge: " + _employee->Age;
	}
	rootPage->NotifyUser(content, NotifyType::StatusMessage);
}


void SDKSample::DataBinding::Scenario10::AgeTextBoxLostFocus(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

	int age = _wtoi(AgeTextBox->Text->Data());

	if (age > 0)
	{
		_employee->Age = age;
	}
}
