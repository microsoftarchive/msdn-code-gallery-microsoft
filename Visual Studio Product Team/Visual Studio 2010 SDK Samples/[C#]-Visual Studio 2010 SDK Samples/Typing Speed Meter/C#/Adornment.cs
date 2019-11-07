using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using System.Windows;
using System.Timers;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;

namespace TypingSpeed
{

    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    class TypingSpeedMeter
    {

        private TypingSpeedControl _root;
        private IWpfTextView _view;
        private IAdornmentLayer _adornmentLayer;
        private int _curMax;
        DateTime _start;
       

        public TypingSpeedMeter(IWpfTextView view)
        {

            _view = view;
            _root = new TypingSpeedControl();

            _curMax = 0;

            _start = DateTime.UtcNow;
          
            //Grab a reference to the adornment layer that this adornment should be added to
            _adornmentLayer = view.GetAdornmentLayer("TypingSpeed");

            _view.ViewportHeightChanged += delegate { this.onSizeChange(); };
            _view.ViewportWidthChanged += delegate { this.onSizeChange(); };

            

        }

        public void updateBar(int typedChars)
        {
            int max = 1000;
            double curLevel = 0;

            DateTime now = DateTime.UtcNow;
            var interval = now.Subtract(_start).TotalMinutes;

            int speed = (int)(typedChars / interval);

            //speed
            _root.val.Content = speed;
            if (speed > _curMax)
            {
                _curMax = speed;
                _root.MaxVal.Content = "Max: " + _curMax.ToString();
            }

            if (speed >= max)
            {
                curLevel = 1;
            }
            else
            {
                curLevel = (double) speed/max;
            }

            _root.fill.Height = _root.bar.Height * curLevel;
            
        }



        public void onSizeChange()
        {
            //clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAdornment(_root);

            //Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(_root, _view.ViewportRight - 80);
            Canvas.SetTop(_root, _view.ViewportTop + 15);

            //add the image to the adornment layer and make it relative to the viewports
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _root, null);
        }


    }
}
