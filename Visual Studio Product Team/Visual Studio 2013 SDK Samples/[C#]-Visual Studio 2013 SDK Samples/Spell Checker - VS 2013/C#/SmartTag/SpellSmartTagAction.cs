//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Language.Spellchecker
{
    /// <summary>
    /// Smart tag actions for spelling suggestions.
    /// </summary>
    internal class SpellSmartTagAction : ISmartTagAction
    {
        #region private data
        private ITrackingSpan _span;
        private string _replaceWith;
        #endregion

        /// <summary>
        /// Constructor for spelling suggestions smart tag actions.
        /// </summary>
        /// <param name="span">Word to replace.</param>
        /// <param name="replaceWith">Text to replace misspelled word with.</param>
        /// <param name="enabled">Enable/disable this action.</param>
        public SpellSmartTagAction(ITrackingSpan span, string replaceWith, bool enabled)
        {
            _span = span;
            _replaceWith = replaceWith;
        }
        #region ISmartTagAction
        /// <summary>
        /// Display text.
        /// </summary>
        public string DisplayText
        {
            get { return _replaceWith; }
        }

        /// <summary>
        /// Icon to place next to the display text.
        /// </summary>
        public System.Windows.Media.ImageSource Icon
        {
            get { return null; }
        }

        /// <summary>
        /// This method is executed when action is selected in the context menu.
        /// </summary>
        public void Invoke()
        {
            _span.TextBuffer.Replace(_span.GetSpan(_span.TextBuffer.CurrentSnapshot), _replaceWith);
        }

        /// <summary>
        /// Enable/disable this action.
        /// </summary>
        public bool IsEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Action set to make sub menus.
        /// </summary>
        public ReadOnlyCollection<SmartTagActionSet> ActionSets
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}
