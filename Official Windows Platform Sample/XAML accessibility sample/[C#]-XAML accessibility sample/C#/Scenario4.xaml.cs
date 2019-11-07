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
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Data;
using System.Collections.ObjectModel;

namespace Accessibility
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario4()
        {
            this.InitializeComponent();
            DataBoundList.ItemsSource = DataHelper.GeneratePersonNamesSource();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }



    }
    #region CustomListView
    public class MyList : ListView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            FrameworkElement source = element as FrameworkElement;

            source.SetBinding(AutomationProperties.AutomationIdProperty, new Binding
            {
                Path = new PropertyPath("Content.AutomationId"),
                RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.Self }
            });

            source.SetBinding(AutomationProperties.NameProperty, new Binding
            {
                Path = new PropertyPath("Content.AutomationName"),
                RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.Self }
            });
        }
    }
    #endregion CustomListView

    #region PersonClass example code
    public class Person
    {
        private string firstName;
        private string lastName;
        private string automationName;
        private int _age;
        private string automationId;

        public Person()
        {
        }

        public Person(string firstName, string lastName, string id, int age, string name)
        {
            FirstName = firstName;
            LastName = lastName;
            AutomationId = id;
            Age = age;
            AutomationName = name;
        }
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
            }
        }
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
            }
        }
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
            }
        }

        public string AutomationId
        {
            get
            {
                return automationId;
            }
            set
            {
                automationId = value;
            }
        }
        public string AutomationName
        {
            get
            {
                return automationName;
            }
            set
            {
                automationName = value;
            }
        }
    }
    #endregion PersonClass

    #region DataHelperClass
    public class DataHelper
    {
        public static ObservableCollection<Person> GeneratePersonNamesSource()
        {
            var ds = new ObservableCollection<Person>();
            ds.Add(new Person("George", "Washington", "ListItemId1", 25, "ListItemName1"));
            ds.Add(new Person("John", "Adams", "ListItemId2", 30, "ListItemName2"));
            ds.Add(new Person("Thomas", "Jefferson", "ListItemId3", 45, "ListItemName3"));
            ds.Add(new Person("James", "Madison", "ListItemId4", 55, "ListItemName4"));
            ds.Add(new Person("James", "Monroe", "ListItemId5", 30, "ListItemName5"));
            ds.Add(new Person("John", "Adams", "ListItemId6", 25, "ListItemName6"));
            ds.Add(new Person("Andrew", "Jackson", "ListItemId7", 55, "ListItemName7"));
            ds.Add(new Person("Martin", "Van Buren", "ListItemId8", 56, "ListItemName8"));
            ds.Add(new Person("William", "Harrison", "ListItemId9", 40, "ListItemName9"));
            ds.Add(new Person("John", "Tyler", "ListItemId10", 42, "ListItemName10"));
            ds.Add(new Person("James", "Polk", "ListItemId11", 60, "ListItemName11"));
            ds.Add(new Person("Zachary", "Taylor", "ListItemId12", 65, "ListItemName12"));
            ds.Add(new Person("Millard", "Fillmore", "ListItemId13", 25, "ListItemName13"));
            ds.Add(new Person("Franklin", "Pierce", "ListItemId14", 35, "ListItemName14"));
            ds.Add(new Person("James", "Buchanan", "ListItemId15", 43, "ListItemName15"));
            ds.Add(new Person("Abraham", "Lincoln", "ListItemId16", 23, "ListItemName16"));
            ds.Add(new Person("Andrew", "Johnson", "ListItemId17", 21, "ListItemName17"));
            ds.Add(new Person("Rutherford", "Hayes", "ListItemId18", 25, "ListItemName18"));
            ds.Add(new Person("James", "Garfield", "ListItemId19", 30, "ListItemName19"));
            ds.Add(new Person("Chester", "Arthur", "ListItemId20", 34, "ListItemName20"));
            ds.Add(new Person("Grover", "Cleveland", "ListItemId21", 55, "ListItemName21"));
            return ds;
        }
    }
    #endregion
    
}
