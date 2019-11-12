//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AppUIBasics.Data;
using Windows.UI.Xaml.Controls.Primitives;

namespace AppUIBasics.Navigation
{
    public abstract class NavUserControl : UserControl
    {
        protected List<PageInfo> pageInfoList = new List<PageInfo>();
        protected int currentPageSize = 1;

        protected FlipView NavFlipView = null;
        protected ListBox PagingIndicatorListBox = null;

        public NavUserControl()
        {
            this.Loaded += NavUserControl_Loaded;
            this.SizeChanged += NavControl_SizeChanged;
            this.DataContextChanged += NavControl_DataContextChanged;
        }

        protected void NavUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            NavFlipView = (FlipView)this.FindName("NavFlipView");
            PagingIndicatorListBox = (ListBox)this.FindName("PagingIndicatorListBox");

            GetPageInfo();
            LayoutNavControl(new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height));
        }

        protected void NavControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (NavFlipView == null) return;
            if (e.NewSize.Width != e.PreviousSize.Width)
            {
                this.LayoutNavControl(e.NewSize);
            }
        }

        protected void NavControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (NavFlipView == null) return;
            GetPageInfo();
            LayoutNavControl(new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height));
        }

        protected int GetPageSize(double buttonWidth)
        {
            double widthAdjustment = NavFlipView.Padding.Left + NavFlipView.Padding.Right;
            return Convert.ToInt32(Math.Floor(((Window.Current.Bounds.Width - widthAdjustment) / buttonWidth)));
        }

        protected int GetPageCount(int buttonCount, int pageSize)
        {
            return Convert.ToInt32(Math.Ceiling((double)buttonCount / pageSize));
        }

        protected virtual List<PageInfo> ConvertDataContextToList(object context)
        {
            List<PageInfo> infoList = new List<PageInfo>();

            if (context.GetType() == typeof(ObservableCollection<ControlInfoDataGroup>))
            {
                var data = context as ObservableCollection<ControlInfoDataGroup>;
                for (int i = 0; i < data.Count; i++)
                {
                    ControlInfoDataGroup group = data.ElementAt(i);

                    infoList.Add(new PageInfo(group.Title, typeof(SectionPage), group.UniqueId, group.Items));
                }
            }
            else if (context.GetType() == typeof(ControlInfoDataGroup))
            {
                var data = context as ControlInfoDataGroup;
                for (int i = 0; i < data.Items.Count; i++)
                {
                    ControlInfoDataItem item = data.Items[i];

                    infoList.Add(new PageInfo(item.Title, typeof(ItemPage), item.UniqueId));
                }
            }
            else if (context.GetType() == typeof(ObservableCollection<ControlInfoDataItem>))
            {
                var data = context as ObservableCollection<ControlInfoDataItem>;
                for (int i = 0; i < data.Count; i++)
                {
                    ControlInfoDataItem item = data.ElementAt(i);

                    infoList.Add(new PageInfo(item.Title, typeof(ItemPage), item.UniqueId));
                }
            }

            return infoList;
        }

        protected virtual void NavButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                // If the button links to a page, navigate to the page.
                PageInfo pageInfo = (PageInfo)b.DataContext;
                if (pageInfo == null) return;

                // If the button links to a page, navigate to the page.
                if (pageInfo.PageType != typeof(Type))
                {
                    if (pageInfo.Data == null && NavigationRootPage.RootFrame.CurrentSourcePageType != pageInfo.PageType)
                    {
                        NavigationRootPage.RootFrame.Navigate(pageInfo.PageType);
                        NavigationRootPage.Current.TopAppBar.IsOpen = false;
                    }
                    else if (pageInfo.Data != null)
                    {
                        NavigationRootPage.RootFrame.Navigate(pageInfo.PageType, pageInfo.Data.ToString());
                        NavigationRootPage.Current.TopAppBar.IsOpen = false;
                    }
                }
            }
        }

        protected void PagingIndicator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Make sure that the navigation buttons are updated by forcing focus to the FlipView
            NavFlipView.Focus(Windows.UI.Xaml.FocusState.Pointer);
        }

        protected void LayoutNavControl(Size size)
        {
            double buttonWidth = (double)this.Resources["NavButtonWidth"];
            int pageSize = GetPageSize(buttonWidth);

            currentPageSize = pageSize;

            NavFlipView.Items.Clear();
            PagingIndicatorListBox.Items.Clear();

            int pageCount = GetPageCount(pageInfoList.Count, pageSize);
            if (pageCount == 1)
            {
                this.PagingIndicatorListBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.PagingIndicatorListBox.Visibility = Visibility.Visible;
            }

            this.GetButtons(pageSize, pageCount);
        }

        protected virtual void GetButtons(int pageSize, int pageCount)
        {
            int currentButtonInfo = 0;
            for (int currentPage = 0; currentPage < pageCount; currentPage++)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;

                for (int currentPanel = 0; currentPanel < pageSize; currentPanel++)
                {
                    if (currentButtonInfo >= pageInfoList.Count)
                    {
                        break;
                    }

                    PageInfo pageInfo = pageInfoList[currentButtonInfo];
                    sp.Children.Add(GetButton(pageInfo));

                    currentButtonInfo++;
                }

                // ListBoxItem must be added before the StackPanel is added
                // to the FlipView so that the selected index values match.
                this.PagingIndicatorListBox.Items.Add(new ListBoxItem());
                this.NavFlipView.Items.Add(sp);
            }
        }

        protected virtual Button GetButton(PageInfo pageInfo)
        {
            Button newButton = new Button();
            newButton.DataContext = pageInfo;
            newButton.Content = pageInfo.Title;
            newButton.Click += NavButton_Click;
            newButton.Style = (Style)this.Resources["NavButtonStyle"];

            return newButton;
        }

        protected virtual void GetPageInfo()
        {
            pageInfoList.Clear();

            if (this.DataContext != null)
            {
                pageInfoList = this.ConvertDataContextToList(this.DataContext);

                // Insert 'Home' button.
                // pageInfoList.Insert(0, new PageInfo("Home", typeof(HubPage)));
            }

            // To populate the navigation control manually, instead of from the DataContext,
            // delete the previous code and add your page info as shown here. 
            // Use the fully qualified class name for each page.

            // pageInfoList.Add(new PageInfo("Home", typeof(HubPage)));
            // pageInfoList.Add(new PageInfo("Page 2", typeof(Page2)));
            // pageInfoList.Add(new PageInfo("Page 3", typeof(Page3)));
            // pageInfoList.Add(new PageInfo("Button", typeof(ItemPage), "Button"));       
        }
    }

    public class PageInfo
    {
        public PageInfo(string title, Type pageType)
        {
            this.Title = title;
            this.PageType = pageType;
            this.Data = null;
        }

        public PageInfo(string title, Type pageType, object data)
        {
            this.Title = title;
            this.PageType = pageType;
            this.Data = data;
            this.ChildData = null;
        }

        public PageInfo(string title, Type pageType, object data, object childData)
        {
            this.Title = title;
            this.PageType = pageType;
            this.Data = data;
            this.ChildData = childData;
        }

        public string Title { get; set; }
        public Type PageType { get; set; }
        public object Data { get; set; }
        public object ChildData { get; set; }
    }

    internal class NavToggleButton : Button
    {
        public ToggleButton Toggle { get; set; }


        protected override void OnApplyTemplate()
        {
            Toggle = this.GetTemplateChild("L2Toggle") as ToggleButton;

            base.OnApplyTemplate();
        }

    }
}
