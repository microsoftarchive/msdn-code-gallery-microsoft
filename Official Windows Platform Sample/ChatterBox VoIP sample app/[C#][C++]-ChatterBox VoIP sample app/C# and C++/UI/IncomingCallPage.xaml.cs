/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Windows.Controls;

namespace PhoneVoIPApp.UI
{
    public partial class IncomingCallPage : BasePage
    {
        public IncomingCallPage()
            : base(new IncomingCallViewModel())
        {
            InitializeComponent();
        }

        private void CallerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((IncomingCallViewModel)this.ViewModel).OnCallerNameChanged(this.CallerNameTextBox.Text);
        }

        private void SimulateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((IncomingCallViewModel)this.ViewModel).Simulate();
        }
    }
}
