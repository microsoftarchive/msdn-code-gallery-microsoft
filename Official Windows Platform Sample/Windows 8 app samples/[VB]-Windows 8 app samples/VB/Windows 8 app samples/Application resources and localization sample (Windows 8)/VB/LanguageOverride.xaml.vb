Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.Globalization

' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Partial Public NotInheritable Class LanguageOverride
    Inherits UserControl
    Private comboBoxValues As List(Of ComboBoxValue)

    Public Sub New()
        Me.InitializeComponent()
        AddHandler Loaded, AddressOf Control_Loaded
    End Sub

    Private Sub Control_Loaded(sender As Object, e As RoutedEventArgs)
        comboBoxValues = New List(Of ComboBoxValue)()

        ' First show the default setting
        comboBoxValues.Add(New ComboBoxValue() With { _
            .DisplayName = "Use language preferences (recommended)", _
            .LanguageTag = "" _
        })

        ' If there are other languages the user speaks that aren't the default show them first
        If ApplicationLanguages.PrimaryLanguageOverride <> "" OrElse ApplicationLanguages.Languages.Count > 1 Then
            For i = 0 To ApplicationLanguages.Languages.Count - 1
                If (ApplicationLanguages.PrimaryLanguageOverride = "" AndAlso i <> 0) OrElse (ApplicationLanguages.PrimaryLanguageOverride <> "" AndAlso i <> 1) Then
                    Me.LanguageOverrideComboBox_AddLanguage(New Windows.Globalization.Language(ApplicationLanguages.Languages(i)))
                End If
            Next
            comboBoxValues.Add(New ComboBoxValue() With { _
                .DisplayName = "——————", _
                .LanguageTag = "" _
            })
        End If

        ' Finally, add the rest of the languages the app supports
        Dim manifestLanguageObjects As New List(Of Windows.Globalization.Language)()
        For Each lang In ApplicationLanguages.ManifestLanguages
            manifestLanguageObjects.Add(New Windows.Globalization.Language(lang))
        Next
        Dim orderedManifestLanguageObjects As IEnumerable(Of Windows.Globalization.Language) = manifestLanguageObjects.OrderBy(Function(lang) lang.DisplayName)
        For Each lang In orderedManifestLanguageObjects
            Me.LanguageOverrideComboBox_AddLanguage(lang)
        Next

        LanguageOverrideComboBox.ItemsSource = comboBoxValues
        LanguageOverrideComboBox.SelectedIndex = comboBoxValues.FindIndex(AddressOf FindCurrent)
        AddHandler LanguageOverrideComboBox.SelectionChanged, AddressOf LanguageOverrideComboBox_SelectionChanged

    End Sub

    Private Sub LanguageOverrideComboBox_AddLanguage(lang As Windows.Globalization.Language)
        comboBoxValues.Add(New ComboBoxValue() With { _
            .DisplayName = lang.DisplayName, _
            .LanguageTag = lang.LanguageTag _
        })
    End Sub

    Private Shared Function FindCurrent(value As ComboBoxValue) As Boolean

        If value.LanguageTag = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride Then
            Return True
        End If
        Return False

    End Function

    Private Sub LanguageOverrideComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim combo As ComboBox = TryCast(sender, ComboBox)

        ' Set the persistent application language override
        Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = combo.SelectedValue.ToString
    End Sub
End Class

Public Class ComboBoxValue
    Public Property DisplayName() As String
        Get
            Return m_DisplayName
        End Get
        Set(value As String)
            m_DisplayName = value
        End Set
    End Property
    Private m_DisplayName As String
    Public Property LanguageTag() As String
        Get
            Return m_LanguageTag
        End Get
        Set(value As String)
            m_LanguageTag = value
        End Set
    End Property
    Private m_LanguageTag As String
End Class
