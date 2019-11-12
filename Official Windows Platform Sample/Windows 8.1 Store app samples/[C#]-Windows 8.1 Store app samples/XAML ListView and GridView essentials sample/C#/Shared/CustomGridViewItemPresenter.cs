using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;

namespace ListViewSimple
{
    class CustomGridViewItemPresenter : ContentPresenter
    {
        // These are the only objects we need to show item's content and visuals for
        // focus and pointer over state. This is a huge reduction in total elements 
        // over the expanded GridViewItem template. Even better is that these objects
        // are only instantiated when they are needed instead of at startup!
        Grid _contentGrid = null;
        Rectangle _pointerOverBorder = null;
        Rectangle _focusVisual = null;

        PointerDownThemeAnimation _pointerDownAnimation = null;
        Storyboard _pointerDownStoryboard = null;

        GridView _parentGridView;

        public CustomGridViewItemPresenter() : base() 
        {}

        protected override bool GoToElementStateCore(string stateName, bool useTransitions)
        {
            base.GoToElementStateCore(stateName, useTransitions);

            // change the visuals shown based on the state the item is going to
            switch (stateName)
            {
                case "Normal":
                    HidePointerOverVisuals();
                    HideFocusVisuals();
                    if (useTransitions)
                    {
                        StopPointerDownAnimation();
                    }
                    break;

                case "Focused":
                case "PointerFocused":
                    ShowFocusVisuals();
                    break;                

                case "Unfocused":
                    HideFocusVisuals();
                    break;

                case "PointerOver":
                    ShowPointerOverVisuals();
                    if (useTransitions)
                    {
                        StopPointerDownAnimation();
                    }
                    break;

                case "Pressed":
                case "PointerOverPressed":
                    if (useTransitions)
                    {
                        StartPointerDownAnimation();
                    }
                    break;

                // this sample does not deal with the DataAvailable, NotDragging, NoReorderHint, NoSelectionHint,
                // Unselected, SelectedUnfocused, or UnselectedPointerOver states
                default:break;
            }

            return true ;
        }

        private void StartPointerDownAnimation()
        {
            // create the storyboard for the pointer down animation if it doesn't exist 
            if (_pointerDownStoryboard == null)
            {
                CreatePointerDownStoryboard();
            }

            // start the storyboard for the pointer down animation 
            _pointerDownStoryboard.Begin();
        }

        private void StopPointerDownAnimation()
        {
            // stop the pointer down animation
            if (_pointerDownStoryboard != null)
            {
                _pointerDownStoryboard.Stop();
            }
        }

        private void ShowFocusVisuals()
        {
            // create the elements necessary to show focus visuals if they have
            // not been created yet.       
            if (!FocusElementsAreCreated())
            {
                CreateFocusElements();
            }

            // make sure the elements necessary to show focus visuals are opaque
            _focusVisual.Opacity = 1;
        }
        
        private void HideFocusVisuals()
        {
            // hide the elements that visualize focus if they have been created
            if (FocusElementsAreCreated())
            {
                _focusVisual.Opacity = 0;
            }
        }

        private void ShowPointerOverVisuals()
        {
            // create the elements necessary to show pointer over visuals if they have
            // not been created yet.       
            if (!PointerOverElementsAreCreated())
            {
                CreatePointerOverElements();
            }

            // make sure the elements necessary to show pointer over visuals are opaque
            _pointerOverBorder.Opacity = 1;
        }
        
        private void HidePointerOverVisuals()
        {
            // hide the elements that visualize pointer over if they have been created
            if (PointerOverElementsAreCreated())
            {
                _pointerOverBorder.Opacity = 0;
            }
        }

        private void CreatePointerDownStoryboard()
        {
            _pointerDownAnimation = new PointerDownThemeAnimation();
            Storyboard.SetTarget(_pointerDownAnimation, _contentGrid);

            _pointerDownStoryboard = new Storyboard();
            _pointerDownStoryboard.Children.Add(_pointerDownAnimation);
        }

        private void CreatePointerOverElements()
        {
            // create the "border" which is really a Rectangle with the correct attributes
            _pointerOverBorder = new Rectangle();
            _pointerOverBorder.IsHitTestVisible = false;
            _pointerOverBorder.Opacity = 0;
            // note that this uses a statically declared brush and will not respond to changes in high contrast
            _pointerOverBorder.Fill = (SolidColorBrush)_parentGridView.Resources["PointerOverBrush"];

            // add the pointer over visuals on top of all children of _InnerDragContent
            _contentGrid.Children.Insert(_contentGrid.Children.Count, _pointerOverBorder);
        }

        private void CreateFocusElements()
        {
            // create the focus visual which is a Rectangle with the correct attributes
            _focusVisual = new Rectangle();
            _focusVisual.IsHitTestVisible = false;
            _focusVisual.Opacity = 0;
            _focusVisual.StrokeThickness = 2;
            // note that this uses a statically declared brush and will not respond to changes in high contrast
            _focusVisual.Stroke = (SolidColorBrush)_parentGridView.Resources["FocusBrush"];

            // add the focus elements behind all children of _InnerDragContent
            _contentGrid.Children.Insert(0, _focusVisual);
        }

        private bool FocusElementsAreCreated()
        {
            return _focusVisual != null;
        }

        private bool PointerOverElementsAreCreated()
        {
            return _pointerOverBorder != null;
        }
        
        protected override void OnApplyTemplate()
        {
            // call the base method
            base.OnApplyTemplate();

            var obj = VisualTreeHelper.GetParent(this);
            while(!(obj is GridView))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }
            _parentGridView = (GridView)obj;

            _contentGrid = (Grid)VisualTreeHelper.GetChild(this,0);
        }
    }
}
