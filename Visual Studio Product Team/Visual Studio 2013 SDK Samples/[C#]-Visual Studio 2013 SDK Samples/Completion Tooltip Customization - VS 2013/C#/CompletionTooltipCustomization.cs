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

using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;

namespace CompletionTooltipCustomization
{
    internal class CompletionTooltipCustomization : TextBlock
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region MEF Exports

        [Export(typeof(IUIElementProvider<Completion, ICompletionSession>))]
        [Name("SampleCompletionTooltipCustomization")]
        [Order()]
        [ContentType("text")]
        internal class CompletionTooltipCustomizationProvider : IUIElementProvider<Completion, ICompletionSession>
        {
            public UIElement GetUIElement(Completion itemToRender, ICompletionSession context, UIElementType elementType)
            {
                if (elementType == UIElementType.Tooltip)
                {
                    return new CompletionTooltipCustomization(itemToRender);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Constructors

        internal CompletionTooltipCustomization(Completion completion)
        {
            this.Text = string.Format(CultureInfo.CurrentCulture, "{0}: {1}", completion.DisplayText, completion.Description);
            this.FontSize = 24;
            this.FontStyle = FontStyles.Italic;
        }

        #endregion
    }
}
