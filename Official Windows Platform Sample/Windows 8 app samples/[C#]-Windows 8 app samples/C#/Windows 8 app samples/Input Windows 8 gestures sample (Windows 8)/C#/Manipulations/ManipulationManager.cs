//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

namespace GesturesApp.Manipulations
{
    #region FilterManipulation

    public delegate void FilterManipulation(object sender, FilterManipulationEventArgs args);

    public class FilterManipulationEventArgs
    {
        internal FilterManipulationEventArgs(Windows.UI.Input.ManipulationUpdatedEventArgs args)
        {
            Delta = args.Delta;
            Pivot = args.Position;
        }

        public Windows.UI.Input.ManipulationDelta Delta
        {
            get;
            set;
        }

        public Windows.Foundation.Point Pivot
        {
            get;
            set;
        }
    }

    #endregion

    public sealed class ManipulationManager : InputProcessor
    {
        private bool _handlersRegistered;

        private Windows.UI.Xaml.Media.MatrixTransform _initialTransform;
        private Windows.UI.Xaml.Media.MatrixTransform _previousTransform;
        private Windows.UI.Xaml.Media.CompositeTransform _deltaTransform;
        private Windows.UI.Xaml.Media.TransformGroup _transform;

        /// <summary>
        /// Gets or sets the filter that is applied to each manipulation update.
        /// </summary>
        /// <remarks>
        /// OnFilterManipulation is called every time the manipulation is updated, before the update is applied.
        /// The filter can change the manipulation's pivots and deltas and it can be used for example to force
        /// the manipulated object to remain inside a region, or to make sure the scaling factor does not become
        /// too big or too small.
        /// </remarks>
        /// <seealso cref="ManipulationFilter"/>
        public FilterManipulation OnFilterManipulation
        {
            get;
            set;
        }

        public ManipulationManager(Windows.UI.Xaml.FrameworkElement element, Windows.UI.Xaml.Controls.Canvas parent)
            : base(element, parent)
        {
            this._handlersRegistered = false;
            this.InitialTransform = this._target.RenderTransform;
            this.ResetManipulation();
        }

        public Windows.UI.Xaml.Media.Transform InitialTransform
        {
            get { return this._initialTransform; }
            set
            {
                // Save initial transform, for resetting
                this._initialTransform = new Windows.UI.Xaml.Media.MatrixTransform()
                {
                    Matrix = new Windows.UI.Xaml.Media.TransformGroup()
                    {
                        Children = { value }
                    }.Value
                };
            }
        }

        /// <summary>
        /// Configures the manipulations that are enabled.
        /// </summary>
        /// <param name="scale">Boolean value that indicates if the manipulation target can be scaled.</param>
        /// <param name="rotate">Boolean value that indicates if the manipulation target can be rotated.</param>
        /// <param name="translate">Boolean value that indicates if the manipulation target can be translated.</param>
        /// <param name="inertia">Boolean value that indicates if manipulation inertia is enabled after rotate/translate.</param>
        public void Configure(bool scale, bool rotate, bool translate, bool inertia)
        {
            var settings = new Windows.UI.Input.GestureSettings();

            if (scale)
            {
                settings |= Windows.UI.Input.GestureSettings.ManipulationScale;
                if (inertia)
                {
                    settings |= Windows.UI.Input.GestureSettings.ManipulationScaleInertia;
                }
            }
            if (rotate)
            {
                settings |= Windows.UI.Input.GestureSettings.ManipulationRotate;
                if (inertia)
                {
                    settings |= Windows.UI.Input.GestureSettings.ManipulationRotateInertia;
                }
            }
            if (translate)
            {
                settings |= Windows.UI.Input.GestureSettings.ManipulationTranslateX |
                    Windows.UI.Input.GestureSettings.ManipulationTranslateY;
                if (inertia)
                {
                    settings |= Windows.UI.Input.GestureSettings.ManipulationTranslateInertia;
                }
            }
            this._gestureRecognizer.GestureSettings = settings;

            this.ConfigureHandlers(scale || rotate || translate);
        }

        public void ResetManipulation()
        {
            // Reset previous transform to the initial transform of the element
            this._previousTransform = new Windows.UI.Xaml.Media.MatrixTransform()
            {
                Matrix = this._initialTransform.Matrix
            };

            // Recreate delta transfrom. This way it is initalized to the identity transform.
            this._deltaTransform = new Windows.UI.Xaml.Media.CompositeTransform();

            // The actual transform is obtained as the composition of the delta transform
            // with the previous transform
            this._transform = new Windows.UI.Xaml.Media.TransformGroup()
            {
                Children = { this._previousTransform, this._deltaTransform }
            };

            // Set the element's transform
            this._target.RenderTransform = this._transform;
        }

        private void OnManipulationUpdated(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.ManipulationUpdatedEventArgs args)
        {
            // Because of the way we process pointer events, all coordinates are expressed
            // in the coordinate system of the reference of the manipulation target.
            // args.Position stores the position of the pivot of this manipulation
            // args.Delta stores the deltas (Translation, Rotation in degrees, and Scale)

            var filteredArgs = new FilterManipulationEventArgs(args);
            if (OnFilterManipulation != null)
            {
                OnFilterManipulation(this, filteredArgs);
            }

            // Update the transform
            // filteredArgs.Pivot indicates the position of the pivot of this manipulation
            // filteredArgs.Delta indicates the deltas (Translation, Rotation in degrees, and Scale)
            this._previousTransform.Matrix = _transform.Value;
            this._deltaTransform.CenterX = filteredArgs.Pivot.X;
            this._deltaTransform.CenterY = filteredArgs.Pivot.Y;
            this._deltaTransform.Rotation = filteredArgs.Delta.Rotation;
            this._deltaTransform.ScaleX = _deltaTransform.ScaleY = filteredArgs.Delta.Scale;
            this._deltaTransform.TranslateX = filteredArgs.Delta.Translation.X;
            this._deltaTransform.TranslateY = filteredArgs.Delta.Translation.Y;
        }

        private void ConfigureHandlers(bool register)
        {
            if (register && !_handlersRegistered)
            {
                this._gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;

                this._handlersRegistered = true;
            }
            else if (!register && _handlersRegistered)
            {
                this._gestureRecognizer.ManipulationUpdated -= OnManipulationUpdated;

                this._handlersRegistered = false;
            }
        }
    }
}
