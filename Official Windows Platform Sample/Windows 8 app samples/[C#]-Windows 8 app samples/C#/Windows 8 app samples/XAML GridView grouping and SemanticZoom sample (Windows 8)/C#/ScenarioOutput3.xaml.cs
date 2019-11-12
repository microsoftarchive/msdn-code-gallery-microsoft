// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;
using Expression.Blend.SampleData.SampleDataSource;

namespace GroupedGridView
{
    public sealed partial class ScenarioOutput3 : Page
    {

        StoreData _storeData = null;

        public ScenarioOutput3()
        {
            InitializeComponent();

            _storeData = new StoreData();

            List<GroupInfoList<object>> dataLetter = _storeData.GetGroupsByLetter();
            cvs2.Source = dataLetter;
            (semanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvs2.View.CollectionGroups;

        }

    }
}
