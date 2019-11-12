/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockStatusBar : IVsStatusbar
    {
        public class ProgressArgs : EventArgs
        {
            public readonly uint Cookie;
            public readonly int InProgress;
            public readonly string Label;
            public readonly uint Complete;
            public readonly uint Total;
            public ProgressArgs(uint cookie, int inProgress, string label, uint complete, uint total)
            {
                Cookie = cookie;
                InProgress = inProgress;
                Label = label;
                Complete = complete;
                Total = total;
            }
        }
        public event EventHandler<ProgressArgs> OnProgress;

        #region IVsStatusbar Members

        public int Animation(int fOnOff, ref object pvIcon)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int FreezeOutput(int fFreeze)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetFreezeCount(out int plCount)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetText(out string pszText)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IsCurrentUser(IVsStatusbarUser pUser, ref int pfCurrent)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IsFrozen(out int pfFrozen)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Progress(ref uint pdwCookie, int fInProgress, string pwszLabel, uint nComplete, uint nTotal)
        {
            if (OnProgress != null)
            {
                OnProgress(this, new ProgressArgs(pdwCookie, fInProgress, pwszLabel, nComplete, nTotal));
            }
            return VSConstants.S_OK;
        }

        public int SetColorText(string pszText, uint crForeColor, uint crBackColor)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetInsMode(ref object pvInsMode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetLineChar(ref object pvLine, ref object pvChar)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetLineColChar(ref object pvLine, ref object pvCol, ref object pvChar)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetSelMode(ref object pvSelMode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetText(string pszText)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetXYWH(ref object pvX, ref object pvY, ref object pvW, ref object pvH)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
