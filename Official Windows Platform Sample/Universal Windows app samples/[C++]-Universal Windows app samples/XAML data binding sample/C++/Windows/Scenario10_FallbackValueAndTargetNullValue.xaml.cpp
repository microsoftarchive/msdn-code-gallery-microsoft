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

	ScenarioReset(nullptr, nullptr);
	_employee->PropertyChanged += ref new PropertyChangedEventHandler(this, &Scenario10::EmployeeChanged);
	ShowEmployeeInfo(EmployeeDataModel);
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

	tbBoundDataModelStatus->Text = "";
}

void Scenario10::EmployeeChanged(Object^ sender, PropertyChangedEventArgs^ e)
{
	tbBoundDataModelStatus->Text = "The property '" + e->PropertyName + "' changed.";
	tbBoundDataModelStatus->Text += "\n\nNew values are:\n";
	ShowEmployeeInfo(tbBoundDataModelStatus);
}

void Scenario10::ShowEmployeeInfo(Windows::UI::Xaml::Controls::TextBlock^ textBlock)
{

	textBlock->Text += "\nName: " + _employee->Name;
	textBlock->Text += "\nOrganization: " + _employee->Organization;
	if (_employee->Age == nullptr)
	{
		textBlock->Text += "\nAge: Null";
	}
	else
	{
		textBlock->Text += "\nAge: " + _employee->Age;
	}
}


void SDKSample::DataBinding::Scenario10::AgeTextBoxLostFocus(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

	int age = _wtoi(AgeTextBox->Text->Data());

	if (age > 0)
	{
		_employee->Age = age;
	}
}
