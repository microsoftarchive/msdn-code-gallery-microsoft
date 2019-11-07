// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="FocusStateChangedEventArgs.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace CameraSampleCS.ViewModels
{
    using System;
    using System.Windows;

    /// <summary>
    /// Contains information about the camera focus state.
    /// </summary>
    public class FocusStateChangedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="focusState">Focus state.</param>
        public FocusStateChangedEventArgs(FocusState focusState)
            : this(focusState, false, null)
        {
            // Do nothing.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="focusState">Focus state.</param>
        /// <param name="continuous">Whether the currnet event is raised by the continuous auto-focus operation.</param>
        public FocusStateChangedEventArgs(FocusState focusState, bool continuous)
            : this(focusState, continuous, null)
        {
            // Do nothing.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="focusState">Focus state.</param>
        /// <param name="continuous">Whether the currnet event is raised by the continuous auto-focus operation.</param>
        /// <param name="focusPoint">Point of interest.</param>
        public FocusStateChangedEventArgs(FocusState focusState, bool continuous, Point? focusPoint)
        {
            this.FocusState = focusState;
            this.Continuous = continuous;
            this.FocusPoint = focusPoint;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the focus state.
        /// </summary>
        public FocusState FocusState { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this event is raised by the continuous auto-focus operation.
        /// </summary>
        public bool Continuous { get; private set; }

        /// <summary>
        /// Gets the point of interest.
        /// </summary>
        public Point? FocusPoint { get; private set; }

        #endregion // Properties
    }
}
