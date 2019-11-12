'***************************************************************************
'
'    Copyright (c) Microsoft Corporation. All rights reserved.
'    This code is licensed under the Visual Studio SDK license terms.
'    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'***************************************************************************

' Copyright (c) Microsoft Corporation
' All rights reserved

Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Formatting
Imports Microsoft.VisualStudio.Utilities

Namespace CaretFisheye


	''' <summary>
	''' This class implements a connector that produces the CaretFisheye LineTransformSourceProvider.
	''' </summary>
    <Export(GetType(ILineTransformSourceProvider)),
    ContentType("text"),
    TextViewRole(PredefinedTextViewRoles.Interactive)>
    Friend NotInheritable Class CaretFisheyeLineTransformSourceProvider
        Implements ILineTransformSourceProvider

        Public Function Create(ByVal textView As IWpfTextView) As ILineTransformSource Implements ILineTransformSourceProvider.Create

            Return CaretFisheyeLineTransformSource.Create(textView)

        End Function

    End Class

End Namespace

