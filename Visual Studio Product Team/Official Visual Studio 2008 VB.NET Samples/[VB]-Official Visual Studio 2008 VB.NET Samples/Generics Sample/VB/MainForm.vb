' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Collections.Generic

Public Class MainForm
    Inherits System.Windows.Forms.Form

    ' This class member will tell us the type of data that is currently loaded
    ' within the source data listbox.
    Dim genericDataType As Type

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Check the radioQueue radio button.  This will ensure that a default generic data 
        ' structure is created.
        genericDataType = GetType(String)
        radioQueue.Checked = True
    End Sub

    Private Sub DataStructure_CheckChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioSortedDictionary.CheckedChanged, radioDictionary.CheckedChanged, radioStack.CheckedChanged, radioList.CheckedChanged, radioQueue.CheckedChanged

        ' Clear the target listbox.
        listTargetData.Items.Clear()

        ' We have one event handler for all of the checkchanged events for the data structure
        ' radio buttons.  Now we have to determine which one was checked so we can create
        ' and fill the appropriate data structure with data.

        If radioQueue.Checked Then
            CreateQueue()
        ElseIf radioList.Checked Then
            CreateList()
        ElseIf radioStack.Checked Then
            CreateStack()
        ElseIf radioDictionary.Checked Then
            CreateDictionary()
        ElseIf radioSortedDictionary.Checked Then
            CreateSortedDictionary()
        End If
    End Sub

#Region "Data Generation Methods"
    Private Sub radioString_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioString.CheckedChanged
        If radioString.Checked Then
            ' Clear the source and target list boxes.
            listSourceData.Items.Clear()
            listTargetData.Items.Clear()

            ' Add some string data to the source data listbox.
            listSourceData.Items.Add("NeKeta Argrow")
            listSourceData.Items.Add("Adam Barr")
            listSourceData.Items.Add("Bonnie L. Skelly")
            listSourceData.Items.Add("Rob Caron")
            listSourceData.Items.Add("Angela Barbariol")

            ' Set the data type that we are currently using.
            genericDataType = GetType(String)

            ' Load the existing data structure by simulating a CheckChanged event.
            DataStructure_CheckChanged(sender, e)
        End If
    End Sub

    Private Sub radioLong_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioLong.CheckedChanged
        If radioLong.Checked Then
            ' Clear the source and target list boxes.
            listSourceData.Items.Clear()
            listTargetData.Items.Clear()

            ' Add some long data to the source data listbox.
            Dim l As LongClass
            l = New LongClass(6)
            listSourceData.Items.Add(l)
            l = New LongClass(447)
            listSourceData.Items.Add(l)
            l = New LongClass(780812)
            listSourceData.Items.Add(l)
            l = New LongClass(99)
            listSourceData.Items.Add(l)
            l = New LongClass(-1)
            listSourceData.Items.Add(l)

            ' Set the data type that we are currently using.
            genericDataType = GetType(LongClass)

            ' Load the existing data structure by simulating a CheckChanged event.
            DataStructure_CheckChanged(sender, e)
        End If
    End Sub

    Private Sub radioObject_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioObject.CheckedChanged
        If radioObject.Checked Then
            ' Clear the source and target list boxes.
            listSourceData.Items.Clear()
            listTargetData.Items.Clear()

            ' Add some long data to the source data listbox.
            Dim c As Customer
            c = New Customer("Stefan Hesse", 1)
            listSourceData.Items.Add(c)
            c = New Customer("George Jiang", 2)
            listSourceData.Items.Add(c)
            c = New Customer("Catherine Boeger", 3)
            listSourceData.Items.Add(c)
            c = New Customer("Ivo William Salmre", 4)
            listSourceData.Items.Add(c)
            c = New Customer("Kim Ralls", 5)
            listSourceData.Items.Add(c)

            ' Set the data type that we are currently using.
            genericDataType = GetType(Customer)

            ' Load the existing data structure by simulating a CheckChanged event.
            DataStructure_CheckChanged(sender, e)
        End If
    End Sub
#End Region

#Region "Queue Data Structure Methods"

    Dim stringQueue As Queue(Of String)
    Dim longQueue As Queue(Of LongClass)
    Dim customerQueue As Queue(Of Customer)

    Private Sub QueueEnqueue(Of ItemType)(ByVal queue As Queue(Of ItemType))
        For i As Integer = 0 To listSourceData.Items.Count - 1
            queue.Enqueue(CType(listSourceData.Items(i), ItemType))
        Next
    End Sub

    Private Sub QueueDisplay(Of ItemType)(ByVal queue As Queue(Of ItemType))
        listTargetData.Items.Clear()
        For Each item As ItemType In queue
            listTargetData.Items.Add(item)
        Next
    End Sub

    Private Sub QueueReverse(Of ItemType)(ByVal queue As Queue(Of itemtype))
        ' Dump the queue to an array.
        Dim o_array(queue.Count - 1) As ItemType
        o_array = queue.ToArray()

        ' Remove the existing items from the queue.
        queue.Clear()

        ' Enqueue the items in the opposite order that they were in the queue.
        For i As Integer = o_array.Length - 1 To 0 Step -1
            queue.Enqueue(o_array(i))
        Next

        ' Load items from the queue to the target list box using the existing methods.
        LoadQueue(False)
    End Sub

    Private Sub QueueSort(Of ItemType)(ByVal queue As Queue(Of ItemType))
        ' Dump the queue to an array.
        Dim o_array(queue.Count - 1) As ItemType
        o_array = queue.ToArray()

        ' Sort the array.
        Array.Sort(o_array)

        ' Remove the existing items from the queue.
        queue.Clear()

        ' Add the items back to the queue.
        For i As Integer = 0 To o_array.Length - 1
            queue.Enqueue(o_array(i))
        Next

        ' Load items from the queue to the target list box using the existing methods.
        LoadQueue(False)
    End Sub

    Private Sub CreateQueue()
        Select Case genericDataType.Name
            Case "String"
                stringQueue = New System.Collections.Generic.Queue(Of String)
                QueueEnqueue(stringQueue)
            Case "LongClass"
                longQueue = New System.Collections.Generic.Queue(Of LongClass)
                QueueEnqueue(longQueue)
            Case "Customer"
                customerQueue = New System.Collections.Generic.Queue(Of Customer)
                QueueEnqueue(customerQueue)
        End Select
    End Sub

    Private Sub ReverseQueue()
        Select Case genericDataType.Name
            Case "String"
                QueueReverse(stringQueue)
            Case "LongClass"
                QueueReverse(longQueue)
            Case "Customer"
                QueueReverse(customerQueue)
        End Select
    End Sub

    Private Sub LoadQueue(Optional ByVal recreate As Boolean = True)
        ' We'll re-create the Queue here just in case it was emptied.
        If recreate Then
            CreateQueue()
        End If

        Select Case genericDataType.Name
            Case "String"
                QueueDisplay(stringQueue)
            Case "LongClass"
                QueueDisplay(longQueue)
            Case "Customer"
                QueueDisplay(customerQueue)
        End Select
    End Sub

    Private Sub EmptyQueue()
        Select Case genericDataType.Name
            Case "String"
                stringQueue.Clear()
            Case "LongClass"
                longQueue.Clear()
            Case "Customer"
                customerQueue.Clear()
        End Select
    End Sub

    Private Sub SortQueue()
        Select Case genericDataType.Name
            Case "String"
                QueueSort(stringQueue)
            Case "LongClass"
                QueueSort(longQueue)
            Case "Customer"
                QueueSort(customerQueue)
        End Select
    End Sub

#End Region

#Region "List Data Structure Methods"

    Dim stringList As List(Of String)
    Dim longList As List(Of LongClass)
    Dim customerList As List(Of Customer)

    Private Sub ListAdd(Of ItemType)(ByVal list As List(Of ItemType))
        ' Populate the List with the data from the source list box.
        For i As Integer = 0 To listSourceData.Items.Count - 1
            list.Add(CType(listSourceData.Items(i), ItemType))
        Next
    End Sub

    Private Sub ListDisplay(Of ItemType)(ByVal list As List(Of ItemType))
        For i As Integer = 0 To list.Count - 1
            listTargetData.Items.Add(list(i))
        Next
    End Sub

    Private Sub ListReverse(Of ItemType)(ByVal list As List(Of ItemType))
        list.Reverse()
    End Sub

    Private Sub ListSort(Of ItemType)(ByVal list As List(Of ItemType))
        list.Sort()
    End Sub

    Private Sub CreateList()
        Select Case genericDataType.Name
            Case "String"
                stringList = New System.Collections.Generic.List(Of String)
                ListAdd(stringList)
            Case "LongClass"
                longList = New System.Collections.Generic.List(Of LongClass)
                ListAdd(longList)
            Case "Customer"
                customerList = New System.Collections.Generic.List(Of Customer)
                ListAdd(customerList)
        End Select
    End Sub

    Private Sub LoadList(Optional ByVal recreate As Boolean = True)
        ' We'll re-create the List here just in case it was emptied.
        If recreate Then
            CreateList()
        End If

        Select Case genericDataType.Name
            Case "String"
                ListDisplay(stringList)
            Case "LongClass"
                ListDisplay(longList)
            Case "Customer"
                ListDisplay(customerList)
        End Select
    End Sub

    Private Sub EmptyList()
        Select Case genericDataType.Name
            Case "String"
                stringList.Clear()
            Case "LongClass"
                longList.Clear()
            Case "Customer"
                customerList.Clear()
        End Select
    End Sub

    Private Sub ReverseList()
        Select Case genericDataType.Name
            Case "String"
                ListReverse(stringList)
            Case "LongClass"
                ListReverse(longList)
            Case "Customer"
                ListReverse(customerList)
        End Select

        ' Load items from the queue to the target list box using the existing methods.
        LoadList(False)
    End Sub

    Private Sub SortList()
        Select Case genericDataType.Name
            Case "String"
                ListSort(stringList)
            Case "LongClass"
                ListSort(longList)
            Case "Customer"
                ListSort(customerList)
        End Select

        ' Load items from the queue to the target list box using the existing methods.
        LoadList(False)
    End Sub

#End Region

#Region "Stack Data Structure Methods"

    Dim stringStack As Stack(Of String)
    Dim longStack As Stack(Of LongClass)
    Dim customerStack As Stack(Of Customer)

    Private Sub StackPush(Of ItemType)(ByVal stack As Stack(Of ItemType))
        ' Populate the Stack with the data from the source list box.
        For i As Integer = 0 To listSourceData.Items.Count - 1
            stack.Push(CType(listSourceData.Items(i), ItemType))
        Next
    End Sub

    Private Sub StackDisplay(Of ItemType)(ByVal stack As Stack(Of ItemType))
        listTargetData.Items.Clear()
        For Each item As Object In stack
            listTargetData.Items.Add(CType(item, ItemType))
        Next
    End Sub

    Private Sub StackReverse(Of ItemType)(ByVal stack As Stack(Of ItemType))
        ' Dump the stack to an array.
        Dim o_array(stack.Count - 1) As ItemType
        o_array = stack.ToArray()

        ' Remove the existing items from the stack.
        stack.Clear()

        ' Push the items onto the stack opposite order that they were on it.
        For i As Integer = 0 To o_array.Length - 1
            stack.Push(o_array(i))
        Next

        ' Load items from the stack to the target list box using the existing methods.
        LoadStack(False)
    End Sub

    Private Sub StackSort(Of ItemType)(ByVal stack As Stack(Of ItemType))
        ' Dump the stack to an array.
        Dim o_array(stack.Count - 1) As ItemType
        o_array = stack.ToArray()

        ' Sort the array.
        Array.Sort(o_array)

        ' Remove the existing items from the stack.
        stack.Clear()

        ' Add the items back to the queue.
        For i As Integer = o_array.Length - 1 To 0 Step -1
            stack.Push(o_array(i))
        Next

        ' Load items from the stack to the target list box using the existing methods.
        LoadStack(False)
    End Sub

    Private Sub CreateStack()
        Select Case genericDataType.Name
            Case "String"
                stringStack = New System.Collections.Generic.Stack(Of String)
                StackPush(stringStack)
            Case "LongClass"
                longStack = New System.Collections.Generic.Stack(Of LongClass)
                StackPush(longStack)
            Case "Customer"
                customerStack = New System.Collections.Generic.Stack(Of Customer)
                StackPush(customerStack)
        End Select

    End Sub

    Private Sub LoadStack(Optional ByVal recreate As Boolean = True)
        ' We'll re-create the Stack here just in case it was emptied.
        If recreate Then
            CreateStack()
        End If

        Select Case genericDataType.Name
            Case "String"
                StackDisplay(stringStack)
            Case "LongClass"
                StackDisplay(longStack)
            Case "Customer"
                StackDisplay(customerStack)
        End Select
    End Sub

    Private Sub EmptyStack()
        Select Case genericDataType.Name
            Case "String"
                stringStack.Clear()
            Case "LongClass"
                longStack.Clear()
            Case "Customer"
                customerStack.Clear()
        End Select
    End Sub

    Private Sub ReverseStack()
        Select Case genericDataType.Name
            Case "String"
                StackReverse(stringStack)
            Case "LongClass"
                StackReverse(longStack)
            Case "Customer"
                StackReverse(customerStack)
        End Select
    End Sub

    Private Sub SortStack()
        Select Case genericDataType.Name
            Case "String"
                StackSort(stringStack)
            Case "LongClass"
                StackSort(longStack)
            Case "Customer"
                StackSort(customerStack)
        End Select
    End Sub

#End Region

#Region "Dictionary Data Structure Methods"

    Dim stringDictionary As Dictionary(Of String, String)
    Dim longDictionary As Dictionary(Of LongClass, LongClass)
    Dim customerDictionary As Dictionary(Of Customer, Customer)

    Private Sub DictionaryAdd(Of ItemType)(ByVal dictionary As Dictionary(Of ItemType, ItemType))
        ' Populate the Stack with the data from the source list box.
        For i As Integer = 0 To listSourceData.Items.Count - 1
            ' Since each item in our source list box is unique we can use the .ToString() method
            ' to get a string value that will act as the key for that item.
            dictionary.Add( _
                CType(listSourceData.Items(i), ItemType), _
                CType(listSourceData.Items(i), ItemType))
        Next
    End Sub

    Private Sub DictionaryDisplay(Of ItemType)(ByVal dictionary As Dictionary(Of ItemType, ItemType))
        listTargetData.Items.Clear()

        '  Add each item in the dictionary to the target list box.
        For Each item As ItemType In dictionary.Keys
            listTargetData.Items.Add(dictionary(item))
        Next
    End Sub

    Private Sub DictionarySort(Of ItemType)(ByVal dictionary As Dictionary(Of ItemType, ItemType))
        ' Items are stored within the dictionary in the order that they were added.
        ' To reverse the dictionary we'll copy the contents to an array and then add
        ' them back to the dictionary in reverse order.

        ' Dump the dictionary to an array.
        Dim o_array(dictionary.Keys.Count - 1) As Object
        Dim i As Integer
        For Each key As ItemType In dictionary.Keys
            o_array(i) = dictionary(key)
            i = i + 1
        Next

        ' Sort the array.
        Array.Sort(o_array)

        ' Remove the existing items from the dictionary.
        dictionary.Clear()

        ' Enqueue the items in the order that are in the sorted array.
        For i = 0 To o_array.Length - 1
            dictionary.Add( _
                CType(o_array(i), ItemType), _
                CType(o_array(i), ItemType))
        Next
    End Sub

    Private Sub DictionaryReverse(Of ItemType)(ByVal dictionary As Dictionary(Of ItemType, ItemType))
        ' Items are stored within the dictionary in the order that they were added.
        ' To reverse the dictionary we'll copy the contents to an array and then add
        ' them back to the dictionary in reverse order.

        ' Dump the dictionary to an array.
        Dim o_array(dictionary.Keys.Count - 1) As Object
        Dim i As Integer
        For Each item As ItemType In dictionary.Keys
            o_array(i) = dictionary(item)
            i = i + 1
        Next

        ' Remove the existing items from the dictionary.
        dictionary.Clear()

        ' Enqueue the items in the opposite order that they were in the dictionary.
        For i = o_array.Length - 1 To 0 Step -1
            dictionary.Add( _
            CType(o_array(i), ItemType), _
            CType(o_array(i), ItemType))
        Next
    End Sub

    Private Sub CreateDictionary()
        ' For generics that use keys to access the data structure members you have to provide
        ' bot the data structure type and the type of the key.  For this sample all keys
        ' are strings.
        Select Case genericDataType.Name
            Case "String"
                stringDictionary = New Dictionary(Of String, String)
                DictionaryAdd(stringDictionary)
            Case "LongClass"
                longDictionary = New Dictionary(Of LongClass, LongClass)
                DictionaryAdd(longDictionary)
            Case "Customer"
                customerDictionary = New Dictionary(Of Customer, Customer)
                DictionaryAdd(customerDictionary)
        End Select
    End Sub

    Private Sub LoadDictionary(Optional ByVal recreate As Boolean = True)
        ' We'll re-create the dictionary here just in case it was emptied.
        If recreate Then
            CreateDictionary()
        End If

        Select Case genericDataType.Name
            Case "String"
                DictionaryDisplay(stringDictionary)
            Case "LongClass"
                DictionaryDisplay(longDictionary)
            Case "Customer"
                DictionaryDisplay(customerDictionary)
        End Select
    End Sub

    Private Sub EmptyDictionary()
        Select Case genericDataType.Name
            Case "String"
                stringDictionary.Clear()
            Case "LongClass"
                longDictionary.Clear()
            Case "Customer"
                customerDictionary.Clear()
        End Select
    End Sub

    Private Sub ReverseDictionary()
        Select Case genericDataType.Name
            Case "String"
                DictionaryReverse(stringDictionary)
            Case "LongClass"
                DictionaryReverse(longDictionary)
            Case "Customer"
                DictionaryReverse(customerDictionary)
        End Select

        ' Load items from the dictionary to the target list box using the existing methods.
        LoadDictionary(False)
    End Sub

    Private Sub SortDictionary()
        Select Case genericDataType.Name
            Case "String"
                DictionarySort(stringDictionary)
            Case "LongClass"
                DictionarySort(longDictionary)
            Case "Customer"
                DictionarySort(customerDictionary)
        End Select

        ' Load items from the dictionary to the target list box using the existing methods.
        LoadDictionary(False)
    End Sub

#End Region

#Region "SortedList Data Structure Methods"

    Dim stringSorted As SortedList(Of String, String)
    Dim longSorted As SortedList(Of LongClass, LongClass)
    Dim customerSorted As SortedList(Of Customer, Customer)

    Private Sub SortedAdd(Of ItemType)(ByVal sorted As SortedList(Of ItemType, ItemType))
        ' Populate the Stack with the data from the source list box.
        For i As Integer = 0 To listSourceData.Items.Count - 1
            ' Since each item in our source list box is unique we can use the .ToString() method
            ' to get a string value that will act as the key for that item.
            sorted.Add( _
                CType(listSourceData.Items(i), ItemType), _
                CType(listSourceData.Items(i), ItemType))
        Next
    End Sub

    Private Sub SortedDisplay(Of ItemType)(ByVal sorted As SortedList(Of ItemType, ItemType))
        For Each pair As KeyValuePair(Of ItemType, ItemType) In sorted
            listTargetData.Items.Add(pair.Value)
        Next
    End Sub

    Private Sub SortedReverse(Of ItemType)(ByVal sorted As SortedList(Of ItemType, ItemType))
        ' Items are sorted when inserted into a SortedDictionary so you cannot modify
        ' the order of items stored within the SortedDictionary. You cannot store the 
        ' items in the SortedDictionary in the reverse order.

        ' You can, however, display them in reverse order in the list box control.
        Dim keys As IList(Of ItemType) = sorted.Keys
        For index As Integer = keys.Count - 1 To 0 Step -1
            listTargetData.Items.Add(CType(sorted(keys(index)), ItemType))
        Next
    End Sub

    Private Sub CreateSortedDictionary()
        ' For generics that use keys to access the data structure members you have to provide
        ' bot the data structure type and the type of the key.  For this sample all keys
        ' are strings.

        Select Case genericDataType.Name
            Case "String"
                stringSorted = New SortedList(Of String, String)
                SortedAdd(stringSorted)
            Case "LongClass"
                longSorted = New SortedList(Of LongClass, LongClass)
                SortedAdd(longSorted)
            Case "Customer"
                customerSorted = New SortedList(Of Customer, Customer)
                SortedAdd(customerSorted)
        End Select
    End Sub

    Private Sub LoadSortedDictionary(Optional ByVal recreate As Boolean = True)
        ' We'll re-create the SortedDictionary here just in case it was emptied.
        If recreate Then
            CreateSortedDictionary()
        End If

        Select Case genericDataType.Name
            Case "String"
                SortedDisplay(stringSorted)
            Case "LongClass"
                SortedDisplay(longSorted)
            Case "Customer"
                SortedDisplay(customerSorted)
        End Select
    End Sub

    Private Sub EmptySortedDictionary()
        Select Case genericDataType.Name
            Case "String"
                stringSorted.Clear()
            Case "LongClass"
                longSorted.Clear()
            Case "Customer"
                customerSorted.Clear()
        End Select
    End Sub

    Private Sub ReverseSortedDictionary()
        Select Case genericDataType.Name
            Case "String"
                SortedReverse(stringSorted)
            Case "LongClass"
                SortedReverse(longSorted)
            Case "Customer"
                SortedReverse(customerSorted)
        End Select
    End Sub

    Private Sub SortSortedDictionary()
        ' Since items are inserted in order within a Sorted Dictionary there is no implementation
        ' necessary for sorting the contents of this data structure.

        ' Load items from the dictionary to the target list box using the existing methods.
        LoadSortedDictionary(False)
    End Sub

#End Region

#Region "Button click events"
    Private Sub cmdLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoad.Click
        ' Empty the target list box.
        listTargetData.Items.Clear()

        ' When the Copy button is clicked we want to copy the data from the generic data
        ' structure that we are currently using, to the target list box.
        If radioQueue.Checked Then
            LoadQueue()
        ElseIf radioList.Checked Then
            LoadList()
        ElseIf radioStack.Checked Then
            LoadStack()
        ElseIf radioDictionary.Checked Then
            LoadDictionary()
        ElseIf radioSortedDictionary.Checked Then
            LoadSortedDictionary()
        End If

    End Sub


    Private Sub cmdEmpty_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEmpty.Click
        ' When the empty button is clicked we want to empty the generic data structure.
        If radioQueue.Checked Then
            EmptyQueue()
        ElseIf radioList.Checked Then
            EmptyList()
        ElseIf radioStack.Checked Then
            EmptyStack()
        ElseIf radioDictionary.Checked Then
            EmptyDictionary()
        ElseIf radioSortedDictionary.Checked Then
            EmptySortedDictionary()
        End If

        ' Empty the target list box.
        listTargetData.Items.Clear()
    End Sub


    Private Sub cmdReverse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReverse.Click
        ' Empty the target list box.
        listTargetData.Items.Clear()

        ' When the empty button is clicked we want to empty the generic data structure.
        If radioQueue.Checked Then
            ReverseQueue()
        ElseIf radioList.Checked Then
            ReverseList()
        ElseIf radioStack.Checked Then
            ReverseStack()
        ElseIf radioDictionary.Checked Then
            ReverseDictionary()
        ElseIf radioSortedDictionary.Checked Then
            ReverseSortedDictionary()
        End If
    End Sub


    Private Sub cmdSort_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSort.Click
        ' In order for this procedure to work, the items contained in the array
        ' must support comparsion. This is accomplished by objects by implementing
        ' the IComparable interface.
        ' See the Customer object definition in Customer.vb for more information.

        ' Empty the target list box.
        listTargetData.Items.Clear()

        ' When the empty button is clicked we want to empty the generic data structure.
        If radioQueue.Checked Then
            SortQueue()
        ElseIf radioList.Checked Then
            SortList()
        ElseIf radioStack.Checked Then
            SortStack()
        ElseIf radioDictionary.Checked Then
            SortDictionary()
        ElseIf radioSortedDictionary.Checked Then
            SortSortedDictionary()
        End If
    End Sub
#End Region


End Class
