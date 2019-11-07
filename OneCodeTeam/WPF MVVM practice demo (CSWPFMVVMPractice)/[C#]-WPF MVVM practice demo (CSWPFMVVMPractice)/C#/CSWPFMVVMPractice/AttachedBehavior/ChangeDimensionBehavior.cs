/****************************** Module Header ******************************\
*   Module Name:  <ChangeDimensionBehavior.cs>
*   Project:	<CSWPFMVVMPractice>
* Copyright (c) Microsoft Corporation.
* 
* The ChangeDimensionBehavior class is an attached behavior that is used to change the 
* dimension of the game.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/ 

using System.Windows;
using System.Windows.Controls;

namespace CSWPFMVVMPractice
{  
    public class ChangeDimensionBehavior:DependencyObject
    {
        public static int GetDimension(DependencyObject obj)
        {
            return (int)obj.GetValue(DimensionProperty);
        }

        public static void SetDimension(DependencyObject obj, int value)
        {
            obj.SetValue(DimensionProperty, value);
        }
     
        // Using a DependencyProperty as the backing store for Dimension.  This enables animation, 
        // styling, binding, etc...        
        public static readonly DependencyProperty DimensionProperty =
            DependencyProperty.RegisterAttached("Dimension", typeof(int), 
            typeof(ChangeDimensionBehavior),
            new UIPropertyMetadata(0, (sender, e) => OnPropertyChangedCallBack(sender, e)));      

        /// <summary>
        ///  Attached property Dimension's PropertyChangeCallback method
        ///  In this method, get the MenuItem the attached property is set on and subscribe the Click
        ///  on the MenuItem
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
       
            // this ensures that the Click event of the MenuItem is subscribed only once
            mnu.Click-=new RoutedEventHandler(mnu_Click);           
            mnu.Click += new RoutedEventHandler(mnu_Click);          

        }

        static void mnu_Click(object sender, RoutedEventArgs e)
        {          
            // get the value of the attached property set on the MenuItem         
            int dimension = GetDimension(sender as DependencyObject);          
            if (Application.Current.MainWindow.DataContext.GetType() == typeof(TicTacToeViewModel))
            {            
                // change the Dimension property of the TicTacToeViewModel by invoking the 
                // ChangeDimensionCommand on the TicTacToeViewModel             
                (Application.Current.MainWindow.DataContext as TicTacToeViewModel).
                    ChangeDimensionCommand.Execute(dimension);
        
            }       
        }
    }
}
