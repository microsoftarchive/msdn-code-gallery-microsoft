// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace MsdnReader
{
    /// <summary>
    /// Subclass of TextBox that exposes a CommitCommand and CommitCommandParameter. This class registers class-level 
    /// command bindings for the Enter key that invokes the CommitCommand. The AcceptsReturn property on this class is meaningless because
    /// the Enter key always invokes the command. Intended for use in Search control
    /// </summary>
    public class CommandTextBox : TextBox
    {
        /// <summary>
        /// The DependencyProperty for CommitCommand
        /// </summary>
        public static readonly DependencyProperty CommitCommandProperty =
                DependencyProperty.Register(
                        "CommitCommand",
                        typeof(ICommand),
                        typeof(CommandTextBox),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for the CommitCommandParameter
        /// </summary>
        public static readonly DependencyProperty CommitCommandParameterProperty =
                DependencyProperty.Register(
                        "CommitCommandParameter",
                        typeof(object),
                        typeof(CommandTextBox),
                        new FrameworkPropertyMetadata((object)null));


        /// <summary>
        /// Get or set the Command property
        /// </summary>
        public ICommand CommitCommand
        {
            get
            {
                return (ICommand)GetValue(CommitCommandProperty);
            }
            set
            {
                SetValue(CommitCommandProperty, value);
            }
        }

        /// <summary>
        /// Reflects the parameter to pass to the CommitCommandProperty upon execution.
        /// </summary>
        public object CommitCommandParameter
        {
            get
            {
                return GetValue(CommitCommandParameterProperty);
            }
            set
            {
                SetValue(CommitCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// When the Enter key is pressed, invoke CommitCommand
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    switch (e.Key)
                    {
                        case Key.Enter:
                            e.Handled = true;
                            ExecuteCommitCommand();
                            break;
                    }
                }
            }

            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        /// Execute the CommitCommand
        /// </summary>
        protected virtual void ExecuteCommitCommand()
        {
            if (CommitCommand != null && CommitCommand.CanExecute(CommitCommandParameter))
            {
                CommitCommand.Execute(CommitCommandParameter);
            }
        }
    }
}