//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;


namespace DataBinding
{
    public sealed partial class Scenario10 : Page
    {
        Employee _employee;
        MainPage rootpage = MainPage.Current;

        public Scenario10()
        {
            this.InitializeComponent();

            _employee = new Employee();
            Output.DataContext = _employee;

            ScenarioReset(null, null);
            _employee.PropertyChanged += employeeChanged;
            ShowEmployeeInfo(EmployeeDataModel.Text);
        }

        private void ScenarioReset(object sender, RoutedEventArgs e)
        {

            _employee.Name = "Jane Doe";
            _employee.Organization = "Contoso";
            _employee.Age = null;

            //To reset Bindings with NullTargetValue and FallbackValue is necessary to reassign the Bindings
            BindingExpression ageBindingExp = AgeTextBox.GetBindingExpression(TextBox.TextProperty);
            Binding ageBinding = ageBindingExp.ParentBinding;
            AgeTextBox.SetBinding(TextBox.TextProperty, ageBinding);

            BindingExpression salaryBindingExp = SalaryTextBox.GetBindingExpression(TextBox.TextProperty);
            Binding salaryBinding = salaryBindingExp.ParentBinding;
            SalaryTextBox.SetBinding(TextBox.TextProperty, salaryBinding);

            rootpage.NotifyUser("", NotifyType.StatusMessage);

        }

        private void ShowEmployeeInfo(string content)
        {
            content += "\nName: " + _employee.Name;
            content += "\nOrganization: " + _employee.Organization;
            if (_employee.Age == null)
            {
                content += "\nAge: Null";
            }
            else
            {
                content += "\nAge: " + _employee.Age;
            }
            rootpage.NotifyUser(content, NotifyType.StatusMessage);
        }

        private void employeeChanged(object sender, PropertyChangedEventArgs e)
        {
            string text = "The property '" + e.PropertyName + "' changed.";
            text += "\n\nNew values are:\n";
            ShowEmployeeInfo(text);
        }

        private void AgeTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            int age = 0;
            if (int.TryParse(AgeTextBox.Text, out age))
            {
                _employee.Age = age;
            }

        }
    }
}
