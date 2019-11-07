Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities

Namespace OokLanguage

    Friend NotInheritable Class OrdinaryClassificationDefinition

        ''' <summary>
        ''' Defines the "ordinary" classification type.
        ''' </summary>
        <Export(GetType(ClassificationTypeDefinition)), Name("ook!")>
        Friend Shared ookExclamation As ClassificationTypeDefinition = Nothing

        ''' <summary>
        ''' Defines the "ordinary" classification type.
        ''' </summary>
        <Export(GetType(ClassificationTypeDefinition)), Name("ook?")>
        Friend Shared ookQuestion As ClassificationTypeDefinition = Nothing

        ''' <summary>
        ''' Defines the "ordinary" classification type.
        ''' </summary>
        <Export(GetType(ClassificationTypeDefinition)), Name("ook.")>
        Friend Shared ookPeriod As ClassificationTypeDefinition = Nothing

    End Class
End Namespace
