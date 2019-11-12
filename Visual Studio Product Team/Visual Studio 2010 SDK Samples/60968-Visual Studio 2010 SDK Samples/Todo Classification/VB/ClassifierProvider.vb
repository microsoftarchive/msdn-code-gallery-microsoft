Imports System.Text
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Text

Namespace ToDoGlyphFactory

    <Export(GetType(IClassifierProvider)),
    ContentType("code")>
    Friend Class ToDoClassifierProvider
        Implements IClassifierProvider

        <Export(GetType(ClassificationTypeDefinition)), Name("todo")>
        Friend ToDoClassificationType As ClassificationTypeDefinition = Nothing

        <Import()>
        Friend ClassificationRegistry As IClassificationTypeRegistryService = Nothing

        <Import()>
        Friend _tagAggregatorFactory As IBufferTagAggregatorFactoryService = Nothing

        Public Function GetClassifier(ByVal buffer As ITextBuffer) As IClassifier Implements IClassifierProvider.GetClassifier
            Dim classificationType As IClassificationType = ClassificationRegistry.GetClassificationType("todo")

            Dim tagAggregator = _tagAggregatorFactory.CreateTagAggregator(Of ToDoTag)(buffer)
            Return New ToDoClassifier(tagAggregator, classificationType)
        End Function

    End Class

    <Export(GetType(EditorFormatDefinition)),
    ClassificationType(ClassificationTypeNames:="todo"),
    Name("ToDoText"),
    UserVisible(True),
    Order(After:=Priority.High)>
    Friend NotInheritable Class ToDoFormat
        Inherits ClassificationFormatDefinition

        Public Sub New()
            Me.DisplayName = "ToDo Text" 'human readable version of the name
            Me.BackgroundOpacity = 1
            Me.BackgroundColor = Colors.Orange
            Me.ForegroundColor = Colors.MidnightBlue
        End Sub

    End Class
End Namespace
