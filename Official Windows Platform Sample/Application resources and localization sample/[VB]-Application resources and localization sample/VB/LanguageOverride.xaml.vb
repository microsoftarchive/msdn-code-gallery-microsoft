
Imports Windows.Globalization

' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Partial Public NotInheritable Class LanguageOverride
    Inherits UserControl

    Private comboBoxValues As List(Of ComboBoxValue)
    Private lastSelectionIndex As Integer

    Public Delegate Sub LanguageOverrideChangedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Event LanguageOverrideChanged As LanguageOverrideChangedEventHandler

    Public Sub New()
        Me.InitializeComponent()
        AddHandler Loaded, AddressOf Control_Loaded
    End Sub

    Private Sub Control_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        comboBoxValues = New List(Of ComboBoxValue)()

        ' First show the default setting
        comboBoxValues.Add(New ComboBoxValue() With {.DisplayName = "Use language preferences (recommended)", .LanguageTag = ""})


        ' If there are app languages that the user speaks, show them next

        ' Note: the first (non-override) language, if set as the primary language override
        ' would give the same result as not having any primary language override. There's
        ' still a difference, though: If the user changes their language preferences, the 
        ' default setting (no override) would mean that the actual primary app language
        ' could change. But if it's set as an override, then it will remain the primary
        ' app language after the user changes their language preferences.

        For i = 0 To ApplicationLanguages.Languages.Count - 1
            Me.LanguageOverrideComboBox_AddLanguage(New Windows.Globalization.Language(ApplicationLanguages.Languages(i)))
        Next i

        ' Now add a divider followed by all the app manifest languages (in case the user
        ' wants to pick a language not currently in their profile)

        ' NOTE: If an app is deployed using a bundle with resource packages, the following
        ' addition to the list may not be useful: The set of languages returned by 
        ' ApplicationLanguages.ManifestLanguages will consist of only those manifest 
        ' languages in the main app package or in the resource packages that are installed 
        ' and registered for the current user. Language resource packages get deployed for 
        ' the user if the language is in the user's profile. Therefore, the only difference 
        ' from the set returned by ApplicationLanguages.Languages above would depend on 
        ' which languages are included in the main app package.

        comboBoxValues.Add(New ComboBoxValue() With {.DisplayName = "——————", .LanguageTag = "-"})

        ' Create a List and sort it before adding items
        Dim manifestLanguageObjects As New List(Of Windows.Globalization.Language)()
        For Each lang In ApplicationLanguages.ManifestLanguages
            manifestLanguageObjects.Add(New Windows.Globalization.Language(lang))
        Next lang
        Dim orderedManifestLanguageObjects As IEnumerable(Of Windows.Globalization.Language) = manifestLanguageObjects.OrderBy(Function(lang) lang.DisplayName)
        For Each lang In orderedManifestLanguageObjects
            Me.LanguageOverrideComboBox_AddLanguage(lang)
        Next lang

        LanguageOverrideComboBox.ItemsSource = comboBoxValues
        LanguageOverrideComboBox.SelectedIndex = comboBoxValues.FindIndex(AddressOf FindCurrent)
        AddHandler LanguageOverrideComboBox.SelectionChanged, AddressOf LanguageOverrideComboBox_SelectionChanged

        lastSelectionIndex = LanguageOverrideComboBox.SelectedIndex
    End Sub

    Private Sub LanguageOverrideComboBox_AddLanguage(ByVal lang As Windows.Globalization.Language)
        comboBoxValues.Add(New ComboBoxValue() With {.DisplayName = lang.NativeName, .LanguageTag = lang.LanguageTag})
    End Sub

    Private Shared Function FindCurrent(ByVal value As ComboBoxValue) As Boolean

        If value.LanguageTag = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride Then
            Return True
        End If
        Return False

    End Function

    Private Sub LanguageOverrideComboBox_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim combo As ComboBox = TryCast(sender, ComboBox)

        ' Don't accept the list item for the divider (tag = "-")
        If combo.SelectedValue.ToString() = "-" Then
            combo.SelectedIndex = lastSelectionIndex
        Else
            lastSelectionIndex = combo.SelectedIndex

            ' Set the persistent application language override
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = combo.SelectedValue.ToString()

            RaiseEvent LanguageOverrideChanged(Me, New EventArgs())
        End If

    End Sub
End Class

Public Class ComboBoxValue
    Public Property DisplayName() As String
    Public Property LanguageTag() As String
End Class
