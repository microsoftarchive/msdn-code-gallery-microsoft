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
using System.Collections.ObjectModel;


namespace DataBinding
{
    public sealed partial class Scenario8 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private GeneratorIncrementalLoadingClass<Employee> employees;

        public Scenario8()
        {
            this.InitializeComponent();

            Scenario8Reset(null, null);
        }

        private void Scenario8Reset(object sender, RoutedEventArgs e)
        {
            if (employees != null)
            {
                employees.CollectionChanged -= _employees_CollectionChanged;
            }

            employees = new GeneratorIncrementalLoadingClass<Employee>(1000, (count) => {
                return new Employee() { Name = "Name" + count, Organization = "Organization" + count };
            });
            employees.CollectionChanged += _employees_CollectionChanged;
                        
            employeesCVS.Source = employees;
            
            tbCollectionChangeStatus.Text = String.Empty;
        }

        void _employees_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            tbCollectionChangeStatus.Text = String.Format("Collection was changed. Count = {0}", employees.Count);
        }
    }
}
