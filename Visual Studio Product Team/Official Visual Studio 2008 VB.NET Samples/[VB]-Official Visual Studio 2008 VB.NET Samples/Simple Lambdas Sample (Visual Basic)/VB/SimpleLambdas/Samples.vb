'Copyright (C) Microsoft Corporation.  All rights reserved.

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq

Public Class Samples
    Shared numbers As Integer() = New Integer() {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}
    Shared strings As String() = New String() {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

    Class Person
        Public Name As String
        Public Level As String
    End Class

    Shared persons As Person() = New Person() { _
        New Person With {.Name = "Jesper", .Level = 3}, _
        New Person With {.Name = "Lene", .Level = 3}, _
        New Person With {.Name = "Jonathan", .Level = 5}, _
        New Person With {.Name = "Sagiv", .Level = 3}, _
        New Person With {.Name = "Jacqueline", .Level = 3}, _
        New Person With {.Name = "Ellen", .Level = 3}, _
        New Person With {.Name = "Gustavo", .Level = 9}}


    Public Shared Sub Sample1()
        ' use Where() to filter out elements matching a particular condition       
        Dim fnums = numbers.Where(Function(n) n < 5)

        Console.WriteLine("Numbers < 5")
        For Each x As Integer In fnums
            Console.WriteLine(x)
        Next
    End Sub

    Public Shared Sub Sample2()
        ' use First() to find the one element matching a particular condition       
        Dim v As String = strings.First(Function(s) s(0) = "o"c)

        Console.WriteLine("string starting with 'o': {0}", v)
    End Sub

    Public Shared Sub Sample3()
        ' use Select() to convert each element into a new value
        Dim snums = numbers.Select(Function(n) strings(n))

        Console.WriteLine("Numbers")
        For Each s As String In snums
            Console.WriteLine(s)
        Next
    End Sub

    Public Shared Sub Sample4()
        ' use Anonymous Type constructors to construct multi-valued results on the fly
        Dim q = strings.Select(Function(s) New With {.Head = s.Substring(0, 1), .Tail = s.Substring(1)})
        For Each p In q
            Console.WriteLine("Head = {0}, Tail = {1}", p.Head, p.Tail)
        Next
    End Sub

    Public Shared Sub Sample5()
        ' Combine Select() and Where() to make a complete query
        Dim q = numbers.Where(Function(n) n < 5).Select(Function(n) strings(n))

        Console.WriteLine("Numbers < 5")
        For Each x In q
            Console.WriteLine(x)
        Next
    End Sub

    Public Shared Sub Sample6()
        ' use GroupBy() to construct group partitions out of similar elements
        Dim q = strings.GroupBy(Function(s) s(0)) ' <- group by first character of each string

        For Each g In q
            Console.WriteLine("Group: {0}", g.Key)
            For Each v As String In g
                Console.WriteLine(vbTab & "Value: {0}", v)
            Next
        Next
    End Sub

    Public Shared Sub Sample7()
        ' use GroupBy() and aggregates such as Count(), Min(), Max(), Sum(), Average() to compute values over a partition
        Dim q = strings.GroupBy(Function(s) s(0)).Select(Function(g) New With {.FirstChar = g.Key, .Count = g.Count()})

        For Each v In q
            Console.WriteLine("There are {0} string(s) starting with the letter {1}", v.Count, v.FirstChar)
        Next
    End Sub

    Public Shared Sub Sample8()
        ' use OrderBy()/OrderByDescending() to give order to your resulting sequence
        Dim q = strings.OrderBy(Function(s) s) ' order the strings by their name

        For Each s As String In q
            Console.WriteLine(s)
        Next
    End Sub

    Public Shared Sub Sample9()
        ' use ThenBy()/ThenByDescending() to provide additional ordering detail
        Dim q = persons.OrderBy(Function(p) p.Level).ThenBy(Function(p) p.Name)

        For Each p In q
            Console.WriteLine("{0}  {1}", p.Level, p.Name)
        Next
    End Sub

    Public Shared Sub Sample10()
        ' use query expressions to simplify
        Dim q = From p In persons _
                Order By p.Level, p.Name _
                Group p.Name By p.Level Into People = Group

        For Each g In q
            Console.WriteLine("Level: {0}", g.Level)
            For Each s In g.People
                Console.WriteLine("Person: {0}", s)
            Next
        Next
    End Sub
End Class
