Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Documents
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media

Public NotInheritable Class BasicCustomControl
    Inherits Control
    Public Sub New()
        Me.DefaultStyleKey = GetType(BasicCustomControl)
    End Sub
End Class
