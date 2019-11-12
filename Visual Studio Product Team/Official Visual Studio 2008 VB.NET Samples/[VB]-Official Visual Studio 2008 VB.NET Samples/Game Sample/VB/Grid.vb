' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' This class represents the grid of blocks. It handles most of the game play.
''' </summary>
''' <remarks></remarks>
Public Class Grid
    ' The grids is 12 columns and 15 rows of Block objects.
    Dim matrix(11, 14) As Block

    ''' <summary>
    ''' Creates a few rows of blocks to start the game. Game starts with Red, Blue, and Green blocks.
    ''' </summary>
    ''' <param name="nrows">Number of rows of blocks to create to start the game.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal nrows As Integer)
        If nrows > matrix.GetLength(0) Then
            Throw New Exception("Must start with " & matrix.GetLength(0) & " or fewer rows.")
        End If
        Dim row As Integer
        Dim column As Integer
        For row = 0 To nrows - 1
            For column = 0 To matrix.GetLength(1) - 1
                matrix(row, column) = New Block(New Color() {Color.Red, Color.Blue, Color.Green})
            Next
        Next
        For row = nrows To matrix.GetLength(0) - 1
            For column = 0 To matrix.GetLength(1) - 1
                matrix(row, column) = Nothing
            Next
        Next
    End Sub

    ''' <summary>
    ''' A new row may be added at any time. New rows have Gray blocks in addition
    ''' to Red, Blue, and Green. This makes the game more difficult.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AddRow()
        Dim column As Integer
        ' Add a new block to each column.
        For column = 0 To matrix.GetLength(1) - 1
            Dim newBlock As New Block(New Color() {Color.Red, Color.Blue, Color.Green, Color.Gray})
            ' Add the new block at the botttom of the column, and push the rest of the
            ' blocks up one column.
            For row As Integer = matrix.GetLength(0) - 1 To 1 Step -1
                matrix(row, column) = matrix(row - 1, column)
            Next
            matrix(0, column) = newBlock
        Next
    End Sub

    ''' <summary>
    ''' Draw the grid of blocks
    ''' </summary>
    ''' <param name="graphics"></param>
    ''' <param name="backColor"></param>
    ''' <remarks></remarks>
    Public Sub Draw(ByVal graphics As Graphics, ByVal backColor As Color)
        graphics.Clear(backColor)
        Dim row As Integer
        Dim column As Integer
        Dim theBlock As Block
        For row = 0 To matrix.GetLength(0) - 1
            For column = 0 To matrix.GetLength(1) - 1
                theBlock = matrix(row, column)
                If Not theBlock Is Nothing Then
                    Dim pointA As New Point(column * Block.BlockSize, row * Block.BlockSize)
                    matrix(row, column).Draw(graphics, pointA)
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' This method responds to a click event in the UI.
    ''' </summary>
    ''' <param name="point"></param>
    ''' <returns>The number of blocks removed from the grid.</returns>
    ''' <remarks></remarks>
    Public Function Click(ByVal point As Point) As Integer
        ' Figure out row and column.
        Dim total As Integer
        Dim transPt As Point = PointTranslator.TranslateToTL(point)
        Dim selectedRow As Integer = transPt.Y \ Block.BlockSize
        Dim selectedColumn As Integer = transPt.X \ Block.BlockSize
        Dim selectedBlock As Block = matrix(selectedRow, selectedColumn)
        If Not selectedBlock Is Nothing Then
            selectedBlock.MarkedForDeletion = True
            ' Determine if any of the neighboring blocks are the same color.
            FindSameColorNeighbors(selectedRow, selectedColumn)
            ' Determine how many blocks would be eliminated.
            total = Me.CalculateScore()
            If total > 1 Then
                Me.CollapseBlocks()
            Else
                Me.ClearMarkedForDeletion()
            End If
        End If
        Return total
    End Function

    Private Sub ClearMarkedForDeletion()
        Dim row As Integer
        Dim column As Integer
        For column = matrix.GetLength(1) - 1 To 0 Step -1
            ' If column is completely empty, then move everthing down one.
            For row = 0 To matrix.GetLength(0) - 1
                If Not matrix(row, column) Is Nothing Then
                    matrix(row, column).MarkedForDeletion = False
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' Find out how many blocks will be eliminated.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CalculateScore() As Integer
        Dim row As Integer
        Dim column As Integer
        Dim total As Integer = 0
        For column = matrix.GetLength(1) - 1 To 0 Step -1
            ' If column is completely empty, then move everthing down one.
            For row = 0 To matrix.GetLength(0) - 1
                If Not matrix(row, column) Is Nothing Then
                    If matrix(row, column).MarkedForDeletion Then
                        total += 1
                    End If
                End If
            Next
        Next
        Return total
    End Function
    ''' <summary>
    ''' After the blocks are removed from the columns, there may be
    ''' columns that are empty. Move columns from right to left to
    ''' fill in the empty columns.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CollapseColumns()
        Dim row As Integer
        Dim column As Integer
        For column = matrix.GetLength(1) - 1 To 0 Step -1
            ' If column is completely empty, then all the columns over one.
            Dim noBlocks As Boolean = True
            For row = 0 To matrix.GetLength(0) - 1
                If Not matrix(row, column) Is Nothing Then
                    noBlocks = False
                End If
            Next

            If noBlocks Then
                Dim newcol As Integer
                For newcol = column To matrix.GetLength(1) - 2
                    For row = 0 To matrix.GetLength(0) - 1
                        matrix(row, newcol) = matrix(row, newcol + 1)
                    Next
                Next
                newcol = matrix.GetLength(1) - 1
                For row = 0 To matrix.GetLength(0) - 1
                    matrix(row, newcol) = Nothing
                Next
            End If
        Next

    End Sub

    ''' <summary>
    ''' Remove all the blocks from the grid.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CollapseBlocks()
        Dim theBlock As Block
        Dim column As Integer
        Dim row As Integer
        Dim aRow As Integer

        ' First remove the blocks from each column.
        For column = 0 To matrix.GetLength(1) - 1
            For row = matrix.GetLength(0) - 1 To 0 Step -1
                theBlock = matrix(row, column)
                If (Not theBlock Is Nothing) Then
                    If theBlock.MarkedForDeletion Then
                        For aRow = row To matrix.GetLength(0) - 2
                            matrix(aRow, column) = matrix(aRow + 1, column)
                        Next
                        matrix(matrix.GetLength(0) - 1, column) = Nothing
                    End If
                End If
            Next
        Next

        ' Reset the MarkedForDeletion flags.
        For row = 0 To matrix.GetLength(0) - 1
            For column = 0 To matrix.GetLength(1) - 1
                theBlock = matrix(row, column)
                If Not theBlock Is Nothing Then
                    theBlock.MarkedForDeletion = False
                End If
            Next
        Next

        ' Remove any columns that are now empty.
        CollapseColumns()
    End Sub

    ''' <summary>
    ''' Provides access into the grid.
    ''' </summary>
    ''' <param name="row"></param>
    ''' <param name="column"></param>
    ''' <value></value>
    ''' <remarks></remarks>
    Default Public Property Item(ByVal row As Integer, ByVal column As Integer) As Block
        Get
            Return matrix(row, column)
        End Get
        Set(ByVal Value As Block)
            matrix(row, column) = Value
        End Set
    End Property


    Private blocksToExamine As ArrayList
    ''' <summary>
    ''' Set MarkedForDeletion to True for each neighboring block
    ''' of the same color.
    ''' </summary>
    ''' <param name="row"></param>
    ''' <param name="column"></param>
    ''' <remarks></remarks>
    Private Sub FindSameColorNeighbors(ByVal row As Integer, ByVal column As Integer)
        Dim color As Color = matrix(row, column).Color
        blocksToExamine = New ArrayList
        blocksToExamine.Add(New Point(row, column))
        matrix(row, column).MarkedForDeletion = True

        ' Each time you find a neighbor, mark it for deletion, and 
        ' add it to the list of blocks to look for neighbors. After you
        ' examine it, remove it from the list. Keep doing this
        ' until there are no more blocks to look at.
        While blocksToExamine.Count > 0
            FindNeighbors()
        End While
    End Sub

    ''' <summary>
    ''' Look to the blocks on each side.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FindNeighbors()
        ' Take the first block out of the arraylist and examine it.
        Dim location As Point = CType(blocksToExamine(0), Point)
        Dim currentBlock As Block = matrix(location.X, location.Y)
        Dim row As Integer = location.X
        Dim column As Integer = location.Y
        blocksToExamine.RemoveAt(0)

        Dim nextRow As Integer
        Dim nextCol As Integer
        Dim selected As Block

        ' look up
        If row < matrix.GetLength(0) - 1 Then
            nextRow = row + 1
            selected = matrix(nextRow, column)
            ExamineNeighbor(selected, nextRow, column, currentBlock.Color)
        End If

        ' look down
        If row > 0 Then
            nextRow = row - 1
            selected = matrix(nextRow, column)
            ExamineNeighbor(selected, nextRow, column, currentBlock.Color)
        End If


        ' look left
        If column > 0 Then
            nextCol = column - 1
            selected = matrix(row, nextCol)
            ExamineNeighbor(selected, row, nextCol, currentBlock.Color)
        End If

        ' look right
        If column < matrix.GetLength(1) - 1 Then
            nextCol = column + 1
            selected = matrix(row, nextCol)
            ExamineNeighbor(selected, row, nextCol, currentBlock.Color)
        End If
    End Sub

    ''' <summary>
    ''' If the neighbor is the same color, add it to the blocks
    ''' to examine.
    ''' </summary>
    ''' <param name="selected"></param>
    ''' <param name="row"></param>
    ''' <param name="column"></param>
    ''' <param name="color"></param>
    ''' <remarks></remarks>
    Private Sub ExamineNeighbor(ByVal selected As Block, ByVal row As Integer, ByVal column As Integer, ByVal color As Color)
        If Not selected Is Nothing Then
            If selected.Color.Equals(color) Then
                If Not selected.MarkedForDeletion Then
                    selected.MarkedForDeletion = True
                    blocksToExamine.Add(New Point(row, column))
                End If
            End If
        End If
    End Sub



End Class

