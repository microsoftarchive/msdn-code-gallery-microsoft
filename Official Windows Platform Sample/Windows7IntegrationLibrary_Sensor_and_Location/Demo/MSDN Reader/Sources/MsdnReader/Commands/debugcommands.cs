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

namespace MsdnReader
{
#if DEBUG   
    /// <summary>
    /// Debug-only commands 
    /// </summary>
    public sealed class DebugCommands
    {
        public static GarbageCollectCommand GarbageCollectCommand
        {
            get { return _garbageCollectCommand; } 
        }

        private static GarbageCollectCommand _garbageCollectCommand = new GarbageCollectCommand();
    }

    public abstract class DebugCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return CanExecuteInternal(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            ExecuteInternal(parameter);
        }

        #endregion


        protected virtual void ExecuteInternal(object parameter)
        {
        }

        protected virtual bool CanExecuteInternal(object parameter)
        {
            return true;
        }
    }

    /// <summary>
    /// Forces garbage collection
    /// </summary>
    public sealed class GarbageCollectCommand : DebugCommand
    {
        protected override void ExecuteInternal(object parameter)
        {
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
        }
    }
#endif
}