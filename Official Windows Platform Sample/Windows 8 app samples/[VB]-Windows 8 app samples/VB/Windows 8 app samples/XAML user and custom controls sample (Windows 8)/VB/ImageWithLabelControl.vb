Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Documents
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media

' The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

Public NotInheritable Class ImageWithLabelControl
    Inherits Control
    Public Sub New()
        Me.DefaultStyleKey = GetType(ImageWithLabelControl)
    End Sub

    ' you can get help for these properties using the propdp code snippet in C# and Visual Basic
    Public Property ImagePath() As ImageSource
        Get
            Return DirectCast(GetValue(ImagePathProperty), ImageSource)
        End Get
        Set(value As ImageSource)
            SetValue(ImagePathProperty, value)
        End Set
    End Property

    ' Using a DependencyProperty as the backing store for ImagePath.  This enables animation, styling, binding, etc...
    Public Shared ReadOnly ImagePathProperty As DependencyProperty = DependencyProperty.Register("ImagePath", GetType(ImageSource), GetType(ImageWithLabelControl), New PropertyMetadata(Nothing))

    Public Property Label() As String
        Get
            Return DirectCast(GetValue(LabelProperty), String)
        End Get
        Set(value As String)
            SetValue(LabelProperty, value)
        End Set
    End Property

    ' Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register("Label", GetType(String), GetType(ImageWithLabelControl), New PropertyMetadata(Nothing))


End Class
