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
Imports System.Windows.Media
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities

Namespace DiffClassifier


    Friend NotInheritable Class DiffClassificationDefinitions

#Region "Content Type and File Extension Definitions"

        <Export(), Name("diff"), BaseDefinition("text")>
        Friend Shared diffContentTypeDefinition As ContentTypeDefinition = Nothing

        <Export(), FileExtension(".diff"), ContentType("diff")>
        Friend Shared diffFileExtensionDefinition As FileExtensionToContentTypeDefinition = Nothing

        <Export(), FileExtension(".patch"), ContentType("diff")>
        Friend Shared patchFileExtensionDefinition As FileExtensionToContentTypeDefinition = Nothing

#End Region

#Region "Classification Type Definitions"

        <Export(), Name("diff")>
        Friend Shared diffClassificationDefinition As ClassificationTypeDefinition = Nothing

        <Export(), Name("diff.added"), BaseDefinition("diff")>
        Friend Shared diffAddedDefinition As ClassificationTypeDefinition = Nothing

        <Export(), Name("diff.removed"), BaseDefinition("diff")>
        Friend Shared diffRemovedDefinition As ClassificationTypeDefinition = Nothing

        <Export(), Name("diff.changed"), BaseDefinition("diff")>
        Friend Shared diffChangedDefinition As ClassificationTypeDefinition = Nothing

        <Export(), Name("diff.infoline"), BaseDefinition("diff")>
        Friend Shared diffInfolineDefinition As ClassificationTypeDefinition = Nothing

        <Export(), Name("diff.patchline"), BaseDefinition("diff")>
        Friend Shared diffPatchLineDefinition As ClassificationTypeDefinition = Nothing

        <Export(), Name("diff.header"), BaseDefinition("diff")>
        Friend Shared diffHeaderDefinition As ClassificationTypeDefinition = Nothing

#End Region

#Region "Classification Format Productions"

        <Export(GetType(EditorFormatDefinition)),
        ClassificationType(ClassificationTypeNames:="diff.added"),
        Name("diff.added")>
        Friend NotInheritable Class DiffAddedFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.ForegroundColor = Colors.Blue
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition)),
        ClassificationType(ClassificationTypeNames:="diff.removed"),
        Name("diff.removed")>
        Friend NotInheritable Class DiffRemovedFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.ForegroundColor = Colors.Red
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition)),
        ClassificationType(ClassificationTypeNames:="diff.changed"),
        Name("diff.changed")>
        Friend NotInheritable Class DiffChangedFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.ForegroundColor = Color.FromRgb(&HCC, &H60, &H10)
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition)),
        ClassificationType(ClassificationTypeNames:="diff.infoline"),
        Name("diff.infoline")>
        Friend NotInheritable Class DiffInfolineFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.ForegroundColor = Color.FromRgb(&H44, &HBB, &HBB)
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition)),
        ClassificationType(ClassificationTypeNames:="diff.patchline"),
        Name("diff.patchline")>
        Friend NotInheritable Class DiffPatchLineFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.ForegroundColor = Colors.Goldenrod
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition)),
        ClassificationType(ClassificationTypeNames:="diff.header"),
        Name("diff.header")>
        Friend NotInheritable Class DiffHeaderFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.ForegroundColor = Color.FromRgb(0, &HBB, 0)
            End Sub
        End Class
#End Region

    End Class

End Namespace
