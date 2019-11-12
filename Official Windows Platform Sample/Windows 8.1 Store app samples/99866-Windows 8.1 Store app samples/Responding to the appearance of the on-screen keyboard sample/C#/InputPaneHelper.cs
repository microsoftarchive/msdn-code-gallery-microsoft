using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

namespace KeyboardEventsSampleCS
{
    public delegate void InputPaneShowingHandler(object sender, InputPaneVisibilityEventArgs e);
    public delegate void InputPaneHidingHandler(InputPane input, InputPaneVisibilityEventArgs e);
    public class InputPaneHelper
    {
        private Dictionary<UIElement, InputPaneShowingHandler> handlerMap;
        private UIElement lastFocusedElement = null;
        private InputPaneHidingHandler hidingHandlerDelegate = null;

        public InputPaneHelper()
        {
            handlerMap = new Dictionary<UIElement, InputPaneShowingHandler>();
        }

        public void SubscribeToKeyboard(bool subscribe)
        {
            InputPane input = InputPane.GetForCurrentView();
            if (subscribe)
            {
                input.Showing += ShowingHandler;
                input.Hiding += HidingHandler;
            }
            else
            {
                input.Showing -= ShowingHandler;
                input.Hiding -= HidingHandler;
            }
        }

        public void AddShowingHandler(UIElement element, InputPaneShowingHandler handler)
        {
            if (handlerMap.ContainsKey(element))
            {
                throw new System.Exception("A handler is already registered!");
            }
            else
            {
                handlerMap.Add(element, handler);
                element.GotFocus += GotFocusHandler;
                element.LostFocus += LostFocusHandler;
            }
        }

        // It's important to tell which element got focus in order to decide if
        // the keyboard handler should be called
        private void GotFocusHandler(object sender, RoutedEventArgs e)
        {
            lastFocusedElement = (UIElement)sender;
        }

        private void LostFocusHandler(object sender, RoutedEventArgs e)
        {
            if (lastFocusedElement == (UIElement)sender)
            {
                lastFocusedElement = null;
            }
        }

        private void ShowingHandler(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            if (lastFocusedElement != null && handlerMap.Count > 0)
            {
                handlerMap[lastFocusedElement](lastFocusedElement, e);
            }
            lastFocusedElement = null;
        }

        private void HidingHandler(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            if (hidingHandlerDelegate != null)
            {
                hidingHandlerDelegate(sender, e);
            }
            lastFocusedElement = null;
        }

        public void SetHidingHandler(InputPaneHidingHandler handler)
        {
            this.hidingHandlerDelegate = handler;
        }

        public void RemoveShowingHandler(UIElement element)
        {
            handlerMap.Remove(element);
            element.GotFocus -= GotFocusHandler;
            element.LostFocus -= LostFocusHandler;
        }
    }
}
