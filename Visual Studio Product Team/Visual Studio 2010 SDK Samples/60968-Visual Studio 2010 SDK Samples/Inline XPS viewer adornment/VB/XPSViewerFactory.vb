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

Imports System.ComponentModel.Composition
Imports System.IO
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Formatting
Imports Microsoft.VisualStudio.Utilities

Namespace InlineXPSViewer

    ''' <summary>
    ''' Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    ''' that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    ''' </summary>
    <Export(GetType(ILineTransformSourceProvider)),
    ContentType("code"),
    TextViewRole(PredefinedTextViewRoles.Document)>
    Friend NotInheritable Class XPSLineTransformSourceProvider
        Implements ILineTransformSourceProvider

        ''' <summary>
        ''' Defines the adornment layer for the adornment. This layer is ordered 
        ''' after the selection layer in the Z-order
        ''' </summary>
        <Export(GetType(AdornmentLayerDefinition)),
        Name("InlineXPSViewer"),
        Order(After:=PredefinedAdornmentLayers.Outlining, Before:=PredefinedAdornmentLayers.Selection),
        TextViewRole(PredefinedTextViewRoles.Document)>
        Public _xpsViewerLayer As AdornmentLayerDefinition = Nothing

        Public Function Create(ByVal textView As IWpfTextView) As ILineTransformSource Implements ILineTransformSourceProvider.Create
            Return textView.Properties.GetOrCreateSingletonProperty(Of lineXPSViewer)(Function()
                                                                                          Dim xpsViewerProvider As New lineXPSViewer(textView)
                                                                                          Return xpsViewerProvider
                                                                                      End Function)
        End Function

    End Class
End Namespace
