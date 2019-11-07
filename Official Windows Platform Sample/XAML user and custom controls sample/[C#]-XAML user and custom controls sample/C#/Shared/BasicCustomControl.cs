using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace UserAndCustomControls
{
    public sealed class BasicCustomControl : Control
    {
        public BasicCustomControl()
        {
            this.DefaultStyleKey = typeof(BasicCustomControl);
        }
    }
}
