/****************************** Module Header ******************************\
*   Module Name:  <ShutdownBehavior.cs>
*   Project:	<CSWPFMVVMPractice>
* Copyright (c) Microsoft Corporation.
* 
* The ShutdownBehavior class is an attached behavior that is used to shut down the application.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/  

using System;
using System.Windows;
using System.Windows.Controls;

namespace CSWPFMVVMPractice
{  
    public class ShutdownBehavior:DependencyObject
    {
        public static Nullable<bool> GetForceShutdown(DependencyObject obj)
        {
            return (Nullable<bool>)obj.GetValue(ForceShutdownProperty);
        }

        public static void SetForceShutdown(DependencyObject obj, Nullable<bool> value)
        {
            obj.SetValue(ForceShutdownProperty, value);
        }
                
        // Using a DependencyProperty as the backing store for ForceShutdown.  This enables 
        // animation, styling, binding, etc...        
        public static readonly DependencyProperty ForceShutdownProperty =
            DependencyProperty.RegisterAttached("ForceShutdown", typeof(Nullable<bool>), 
            typeof(ShutdownBehavior), 
            new UIPropertyMetadata(null, (sender,e)=>OnPropertyChangedCallBack(sender,e)));

        /// <summary>
        /// The ForceShutdown property's PropertyChangedCallback method
        /// In this method, get the MenuItem the attached property is set on and subscribe to the
        /// Click event of the MenuItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPropertyChangedCallBack(DependencyObject sender, 
            DependencyPropertyChangedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            if (mnu == null)
            {
                return;
            }
         
            // This ensures the Click event on the MenuItem is subscribed to only once            
            mnu.Click-=new RoutedEventHandler(mnu_Click);           
            mnu.Click += new RoutedEventHandler(mnu_Click);          

        }

        /// <summary>
        /// MenuItem's Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void mnu_Click(object sender, RoutedEventArgs e)
        {       
            // If the ForceShutdown attached proerty value set on the MenuItem is true, exit the 
            // application immediately         
            if (GetForceShutdown(sender as DependencyObject).Value == true)
            {
                Application.Current.Shutdown();
            }
            
            // If the ForceShutdown attached proerty value set on the MenuItem is false, show a 
            // messagebox before exiting the application            
            else if (GetForceShutdown(sender as DependencyObject).Value == false)
            {
                if (MessageBox.Show("Are you sure to exit the application?", "Exit", 
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
