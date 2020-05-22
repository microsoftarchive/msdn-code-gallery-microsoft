// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace MsdnReader
{
    /// <summary>
    /// Decorator that groups a subtree of UIElements without needing to sublclass them
    /// </summary>
    public class UIAutomationPane : ContentControl
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new UIAutomationDecoratorAutomationPeer(this);
        }

        private class UIAutomationDecoratorAutomationPeer : FrameworkElementAutomationPeer
        {
            public UIAutomationDecoratorAutomationPeer(UIAutomationPane owner)
                :
                base(owner)
            {
            }
            
            protected override AutomationControlType GetAutomationControlTypeCore() 
            {
                return AutomationControlType.Pane;
            }
            
            protected override string GetClassNameCore()
            {
                return "Frame";
            }
        }
    }
}