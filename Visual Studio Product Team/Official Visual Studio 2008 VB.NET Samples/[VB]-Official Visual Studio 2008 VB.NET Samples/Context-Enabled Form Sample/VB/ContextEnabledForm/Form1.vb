Imports Microsoft.Ink
Imports System.Reflection

Public Class Form1
    Dim myPenInputPanel As PenInputPanel
    Dim factoidNameValues = New SortedList(Of String, String)

    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'Attach the programmable PenInputPanel to the control
        myPenInputPanel = New PenInputPanel(textBox1)

        'Add all supported Factoids to the checked list box
        AddAllFactoids(listBox1.Items)
    End Sub

    Private Sub AddAllFactoids(ByVal objectCollection As ListBox.ObjectCollection)
        'Factoids are defined as static, public string constant fields in the Factoid object
        Dim factoidType As Type = GetType(Factoid)
        Dim field As FieldInfo

        For Each field In factoidType.GetFields(BindingFlags.Static Or BindingFlags.Public)
            'Retrieve the name of the Factoid
            Dim fieldName As String = field.Name

            '"WordList" factoid is not supported for PenInputPanel
            If fieldName = "WordList" Then
                Continue For
            End If

            'Retrieve the value of the Factoid (generally, but not always, the Name in uppercase
            Dim fieldValue As String = CType(field.GetValue(factoidType), String)

            'Add the name to the checked list box
            objectCollection.Add(fieldName)

            'Store the value so it can be retrieved when the list box is manipulated
            factoidNameValues.Add(fieldName, fieldValue)
        Next
    End Sub

    Sub ListBoxSelectedIndexChangedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedItems.Count = 0 Then
            'If none selected, set Factoid to default
            myPenInputPanel.Factoid = Factoid.Default
        End If

        'Get selected name of Factoid
        Dim factoidName As String = CType(ListBox1.SelectedItem, String)

        'Retrieve factoid value
        Dim factoidValue As String = factoidNameValues(factoidName)

        Try
            'Set context
            myPenInputPanel.Factoid = factoidValue
        Catch
            'Not all systems support all factoids
            MessageBox.Show("Factoid not supported on this system")

            'Set the factoid to default
            myPenInputPanel.Factoid = Factoid.Default
            ListBox1.SelectedIndex = 1
            ListBox1.Items.Remove(factoidName)
            ListBox1.Invalidate()
        End Try
    End Sub

    Sub CheckBoxChangedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            ListBox1.Enabled = True
        Else
            ListBox1.Enabled = False
            myPenInputPanel.Factoid = Factoid.Default
        End If
    End Sub

End Class
