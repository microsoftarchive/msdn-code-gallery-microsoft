/****************************** Module Header ******************************\
 * Module Name:  ViewModelLocator.cs
 * Project:      CSWindowsStoreAppFlightDataFilter
 * Copyright (c) Microsoft Corporation.
 * 
 * ViewModelLocator
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

namespace FlightDataFilter.ViewModel
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        static ViewModelLocator()
        {
            _main = new MainViewModel();
        }

        public static MainViewModel _main;
        public MainViewModel Main
        {
            get
            {
                return _main;
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
