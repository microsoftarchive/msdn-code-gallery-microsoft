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
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities

Namespace DiffClassifier

    <Export(GetType(IClassifierProvider)),
    ContentType("diff")>
    Friend Class DiffClassifierProvider
        Implements IClassifierProvider

        <Import()>
        Friend ClassificationRegistry As IClassificationTypeRegistryService = Nothing
        Private Shared diffClassifier As DiffClassifier

        Public Function GetClassifier(ByVal buffer As ITextBuffer) As IClassifier Implements IClassifierProvider.GetClassifier
            If diffClassifier Is Nothing Then
                diffClassifier = New DiffClassifier(ClassificationRegistry)
            End If

            Return diffClassifier
        End Function

    End Class
End Namespace
