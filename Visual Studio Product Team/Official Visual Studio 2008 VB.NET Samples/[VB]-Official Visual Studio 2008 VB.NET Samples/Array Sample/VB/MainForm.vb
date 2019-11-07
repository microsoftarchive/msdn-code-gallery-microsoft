' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm
    Inherits System.Windows.Forms.Form

    ' Used to specify which listbox the DisplayArrayData
    ' method should use when loading data.
    Private Enum WhichListBox
        BoxOne
        BoxTwo
    End Enum

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the application form.
        Me.optValues.Checked = True
        Me.optName.Checked = True
    End Sub

    Private Sub cmdBinarySearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBinarySearch.Click
        Dim message As String
        Dim position As Integer
        Dim valueToFind As Integer
        Dim stringToFind As String = ""

        If Me.optValues.Checked OrElse Me.optId.Checked Then
            ' Looking for an integer search value.
            Try
                valueToFind = CInt(Me.txtBSearchFor.Text)
            Catch ex As Exception
                MessageBox.Show("Please enter an integer to search for.", "Binary Search", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Me.txtBSearchFor.Select()
                Exit Sub
            End Try
        Else
            ' Make sure there is a value to use a search criteria.
            If Me.txtBSearchFor.Text.Length = 0 Then
                MessageBox.Show("Please enter a value to search for", "Binary Search", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Me.txtBSearchFor.Select()
                Exit Sub
            End If
            stringToFind = Me.txtBSearchFor.Text
        End If

        Me.lstAfter.Items.Clear()


        If Me.optValues.Checked Then
            Dim valueArray() As Integer = MakeValueArray()
            DisplayArrayData(valueArray)
            ' Sort the array otherwise the BinarySearch method won't work.
            Array.Sort(valueArray)

            ' Show the data sorted.
            DisplayArrayData(valueArray)

            ' Perform search
            position = Array.BinarySearch(valueArray, valueToFind)

            If position >= 0 Then
                message = String.Format("The value {0} was found in the array at position {1}.", valueToFind, position.ToString())
            Else
                ' If the item is not found, a negative number is returned.
                ' This is the bitwise complement for the location where the
                ' item would have been *if* it existed.
                ' We use the Not operator to flip it back to a postive number.
                Dim intBWC As Integer = (Not position)
                message = String.Format("The value {0} was NOT found in the array. If it did exist it would be at position {1}.", valueToFind, intBWC)
            End If
        Else
            Dim objData() As Customer = MakeObjectArray()
            DisplayArrayData(objData)
            ' When searching an array of objects, we need to use
            ' a compatible object type. In this case we're using
            ' the value provided by the txtBSearchFor box as the 
            ' customer's name and providing a bogus Id value.
            ' The key to binary search working with objects is that
            ' the object must implement IComparable.
            Dim c As Customer

            If optId.Checked Then
                Customer.SetCompareKey(Customer.CompareField.Id)
                Array.Sort(objData)
                c = New Customer(valueToFind, "")
            Else
                Customer.SetCompareKey(Customer.CompareField.Name)
                Array.Sort(objData)
                c = New Customer(1, stringToFind)
            End If

            DisplayArrayData(objData)

            position = Array.BinarySearch(objData, c)

            If position >= 0 Then
                message = String.Format("The value {0} was found in the array at position {1}.", stringToFind, position.ToString())
            Else
                Dim intBWC As Integer = (Not position)
                message = String.Format("The value {0} was NOT found in the array. If it did exist it would be at position {1}.", stringToFind, intBWC)
            End If

        End If

        MessageBox.Show(message, "Binary Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub cmdCreateDynamic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCreateDynamic.Click
        ' This routine builds a dynamic array
        ' using the value specified in txtLength
        ' to set the number of buckets.
        If Me.optValues.Checked Then
            Dim dynamicValueData() As Integer
            ReDim dynamicValueData(CInt(Me.numberElements.Value) - 1)
            Dim i As Integer
            For i = 0 To dynamicValueData.Length - 1
                Try
                    dynamicValueData(i) = CType(InputBox("Enter a number", i.ToString(), "None " & i), Integer)
                Catch
                    dynamicValueData(i) = 0
                End Try
            Next
            DisplayArrayData(dynamicValueData)
        Else
            Dim dynamicObjectData() As Customer
            ReDim dynamicObjectData(CInt(Me.numberElements.Value) - 1)
            Dim i As Integer

            For i = 0 To dynamicObjectData.Length - 1
                dynamicObjectData(i) = New Customer
                dynamicObjectData(i).Id = ((i + 1) * 10)
                dynamicObjectData(i).Name = InputBox("Enter a string", ("Item " & (i + 1)), ("None " & i + 1))
            Next
            DisplayArrayData(dynamicObjectData)
        End If
        lstAfter.Items.Clear()
    End Sub

    Private Sub cmdCreateMatrix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCreateMatrix.Click
        ' This procedure uses the new initialization syntax to create a simple matrix array.
        Dim strMatrix(,) As String = {{"Red", "Green"}, {"Yellow", "Purple"}, {"Blue", "Orange"}}
        Me.DisplayArrayData(strMatrix)
        lstAfter.Items.Clear()
    End Sub

    Private Sub cmdCreateStatic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCreateStatic.Click
        ' This procedure uses the new initialization to build either a static
        ' array of strings or an array of Customer objects.
        ' Note that the arrary could of course be resized using the ReDim staetment.
        If Me.optValues.Checked Then
            Dim valueData() As Integer = MakeValueArray()
            DisplayArrayData(valueData)
        Else
            ' This command here takes advantage of the fact that objects in .NET
            ' can have parameterized constructors. This allows us to specify an
            ' array of objects in one line of code.
            Dim objData() As Customer = MakeObjectArray()
            DisplayArrayData(objData)
        End If
        lstAfter.Items.Clear()
    End Sub

    Private Sub cmdReverse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReverse.Click
        ' Reverse reorders the elements in an array in decending order as they were added.
        Me.lblDataAfter.Text = "Data After Reverse"
        If Me.optValues.Checked Then
            Dim valueArray() As Integer = MakeValueArray()
            DisplayArrayData(valueArray, WhichListBox.BoxOne)
            Array.Reverse(valueArray)
            DisplayArrayData(valueArray, WhichListBox.BoxTwo)
        Else
            Dim objData() As Customer = MakeObjectArray()
            DisplayArrayData(objData, WhichListBox.BoxOne)
            Array.Reverse(objData)
            DisplayArrayData(objData, WhichListBox.BoxTwo)
        End If

    End Sub

    Private Sub cmdSort_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSort.Click
        ' In order for this procedure to work, the items contained in the array
        ' must support comparsion. This is accomplished by objects by implementing
        ' the IComparable interface.
        ' See the Customer object definition below for more information.

        Me.lblDataAfter.Text = "Data After Sort"

        If Me.optValues.Checked Then
            Dim valueArray() As Integer = MakeValueArray()
            DisplayArrayData(valueArray, WhichListBox.BoxOne)
            Array.Sort(valueArray)
            DisplayArrayData(valueArray, WhichListBox.BoxTwo)
        Else
            Dim objData() As Customer = MakeObjectArray()
            DisplayArrayData(objData, WhichListBox.BoxOne)
            Array.Sort(objData)
            DisplayArrayData(objData, WhichListBox.BoxTwo)
        End If

    End Sub


    ' Start over when options are changed.
    Private Sub CompareKeyCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles optId.CheckedChanged, optName.CheckedChanged, optObjects.CheckedChanged, optValues.CheckedChanged

        Dim opt As RadioButton = CType(sender, RadioButton)
        ' Change the field used by the Customer object when sorts or searches 
        ' are applied.
        If opt.Name = "optId" Then
            ' SetCompareKey is a shared member that will affect all instances
            ' of the Customer type in this AppDomain.
            Customer.SetCompareKey(Customer.CompareField.Id)
        Else
            Customer.SetCompareKey(Customer.CompareField.Name)
        End If

        lstAfter.Items.Clear()
        lstArrayData.Items.Clear()
        lblDataAfter.Text = "No Data Displayed"
    End Sub

    Private Sub DataTypeCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optObjects.CheckedChanged, optValues.CheckedChanged
        ' Enabled the grpCompareField only when objects are being loaded
        ' into arrays.
        Dim opt As RadioButton = CType(sender, RadioButton)
        Me.grpCompareField.Enabled = Not (opt.Name = "optStrings")
        Me.lstArrayData.Items.Clear()
        Me.lstAfter.Items.Clear()
    End Sub

    Private Sub DisplayArrayData(ByVal arr As Array)
        ' Delegate to the next more complex version.
        Me.DisplayArrayData(arr, WhichListBox.BoxOne)
    End Sub

    Private Sub DisplayArrayData(ByVal arr As Array, ByVal ListBox As WhichListBox)

        Dim i As Integer
        Dim u As Integer = (arr.Length - 1)

        Dim lst As ListBox
        If ListBox = WhichListBox.BoxOne Then
            lst = Me.lstArrayData
        Else
            lst = Me.lstAfter
        End If

        lst.Items.Clear()

        ' Figure out how many dimensions (expressed as Rank)
        ' the passed in array has.
        Select Case arr.Rank
            Case 1
                For i = 0 To u
                    lst.Items.Add(String.Format("{0} = {1}", i, arr.GetValue(i).ToString()))
                Next
            Case 2
                Dim j As Integer
                For i = 0 To (arr.GetLength(0) - 1)
                    For j = 0 To (arr.GetLength(1) - 1)
                        lst.Items.Add(String.Format("({0}, {1}) = {2}", i, j, arr.GetValue(i, j).ToString()))
                    Next j
                Next i

            Case Else
                ' Sorry, we don't go beyond two dimensions
                lst.Items.Add(String.Format("The array received has too many dimensions ({0})", arr.Rank))

        End Select
    End Sub

    Private Function MakeValueArray() As Integer()
        Dim numbers() As Integer = {3423, 9348, 3581, 7642, 2985}
        Return numbers
    End Function

    Private Function MakeObjectArray() As Customer()
        Dim numbers() As Integer = MakeValueArray()
        Dim names() As String = {"Jonas Hasselberg", "Gigi Mathew", "Amy Rusko", "Pedro Gutierrez", "Kok-Ho Loh"}
        Dim objData() As Customer = { _
            New Customer(numbers(0), names(0)), _
            New Customer(numbers(1), names(1)), _
            New Customer(numbers(2), names(2)), _
            New Customer(numbers(3), names(3)), _
            New Customer(numbers(4), names(4))}
        Return objData
    End Function


    ' This code will close the form.
    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        ' Close the current form
        Me.Close()
    End Sub
End Class
