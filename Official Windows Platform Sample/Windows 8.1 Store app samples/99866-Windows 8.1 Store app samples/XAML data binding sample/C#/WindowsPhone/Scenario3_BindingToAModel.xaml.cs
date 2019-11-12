//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.ComponentModel;

namespace DataBinding
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        Employee _employee;

        public Scenario3()
        {
            this.InitializeComponent();
            Scenario3Reset(null, null);
        }
        
        private void Scenario3Reset(object sender, RoutedEventArgs e)
        {
            _employee = new Employee();
            _employee.Name = "Jane Doe";
            _employee.Organization = "Contoso";
            _employee.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(employeeChanged);
                        
            rootPage.DataContext = _employee;
            tbBoundDataModelStatus.Text = "";
        }

        private void employeeChanged(object sender, PropertyChangedEventArgs e)
        {
            tbBoundDataModelStatus.Text = "The property:'" + e.PropertyName + "' was changed";
        }

    }
}
