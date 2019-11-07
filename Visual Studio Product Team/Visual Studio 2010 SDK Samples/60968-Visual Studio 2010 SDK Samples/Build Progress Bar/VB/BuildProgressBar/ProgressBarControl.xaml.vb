Imports System.Security.Permissions
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Animation

'''<summary>
'''  Interaction logic for ProgressBarControl.xaml
'''</summary>
Partial Public Class ProgressBarControl
    Inherits System.Windows.Controls.UserControl

    Private Shared brightGreen As Color = Color.FromArgb(&HFF, &H1, &HD3, &H28)
    Private aColor As Boolean = False

    Public Sub New()
        InitializeComponent()

        ' Animate from blue to bright green by default
        StartColor = Colors.Blue
        EndColor = brightGreen
    End Sub

    Public Property StartColor As Color
    Public Property EndColor As Color

    Public Property AnimateColor As Boolean
        Get
            Return aColor
        End Get
        Set(ByVal value As Boolean)
            aColor = value

            ' Create or remove a drop shadow effect
            If aColor And progressBar.Effect Is Nothing Then
                progressBar.Effect = New DropShadowEffect()
            ElseIf Not AnimateColor And Not progressBar.Effect Is Nothing Then
                progressBar.Effect = Nothing
            End If
        End Set
    End Property

    ' Get/set the progress bar value
    Public Property Value As Double
        Get
            Return progressBar.Value
        End Get
        Set(ByVal v As Double)
            If aColor Then
                progressBar.Foreground = New SolidColorBrush(Lerp(StartColor, EndColor, Value))
            Else
                progressBar.Foreground = New SolidColorBrush(brightGreen)
            End If

            progressBar.Value = v
        End Set
    End Property

    ' Get/set the progress bar text
    Public Property Text As String
        Get
            Return barText.Text
        End Get
        Set(ByVal value As String)
            barText.Text = value
        End Set
    End Property

    Private Function Lerp(ByVal first As Color, ByVal second As Color, ByVal percentage As Double) As Color
        Dim a As Byte = CType((CType(second.A, Double) - first.A) * percentage + first.A, Byte)
        Dim r As Byte = CType((CType(second.R, Double) - first.R) * percentage + first.R, Byte)
        Dim g As Byte = CType((CType(second.G, Double) - first.G) * percentage + first.G, Byte)
        Dim b As Byte = CType((CType(second.B, Double) - first.B) * percentage + first.B, Byte)

        Return Color.FromArgb(a, r, g, b)
    End Function

    Private Sub ProgressToolWindow_SizeChanged(ByVal sender As System.Object, ByVal e As System.Windows.SizeChangedEventArgs)
        AdjustSize()
    End Sub

    Private Sub ProgressToolWindow_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        AdjustSize()
    End Sub

    Private Sub AdjustSize()
        progressBar.Width = Me.ActualWidth - 24
        progressBar.Height = Math.Max(10, Math.Min(48, Me.ActualHeight - 24))
        viewbox.Width = progressBar.Width
    End Sub
End Class