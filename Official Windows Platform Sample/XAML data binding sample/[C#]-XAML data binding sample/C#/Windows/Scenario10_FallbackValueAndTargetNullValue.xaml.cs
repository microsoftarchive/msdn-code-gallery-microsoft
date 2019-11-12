//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;


namespace DataBinding
{
    public sealed partial class Scenario10 : Page
    {
        Employee _employee;

        public Scenario10()
        {
            this.InitializeComponent();

            _employee = new Employee();
            Output.DataContext = _employee;

            ScenarioReset(null, null);
            _employee.PropertyChanged += employeeChanged;
            ShowEmployeeInfo(EmployeeDataModel);
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

            tbBoundDataModelStatus.Text = "";
        }

        private void ShowEmployeeInfo(TextBlock textBlock)
        {
            textBlock.Text += "\nName: " + _employee.Name;
            textBlock.Text += "\nOrganization: " + _employee.Organization;
            if (_employee.Age == null)
            {
                textBlock.Text += "\nAge: Null";
            }
            else
            {
                textBlock.Text += "\nAge: " + _employee.Age;
            }
        }

        private void employeeChanged(object sender, PropertyChangedEventArgs e)
        {
            tbBoundDataModelStatus.Text = "The property '" + e.PropertyName + "' changed.";
            tbBoundDataModelStatus.Text += "\n\nNew values are:\n";
            ShowEmployeeInfo(tbBoundDataModelStatus);
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
