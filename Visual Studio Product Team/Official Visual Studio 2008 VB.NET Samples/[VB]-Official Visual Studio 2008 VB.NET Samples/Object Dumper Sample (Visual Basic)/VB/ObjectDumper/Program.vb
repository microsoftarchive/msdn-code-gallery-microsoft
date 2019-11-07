Module Program

    Sub Main()
        'The following code shows the variety of objects the object dumper can
        'handle. From all value types to collections and anonymous types, the 
        'object dumper adeptly shows all objects in their correct formats

        Sample1()
        Sample2()
        Sample3()
        Sample4()
        Sample5()
        Sample6()
        Sample7()
    End Sub

    Public Sub Sample1()
        'Writes a nullable type
        Dim i As Integer? = 1
        ObjectDumper.Write(i)
    End Sub

    Public Sub Sample2()
        'Writes a string
        Dim str As String = "This is a sample"
        ObjectDumper.Write(str)
    End Sub

    Public Sub Sample3()
        'Writes a value type
        Dim i As Decimal = 9.0
        ObjectDumper.Write(i)
    End Sub

    Public Class C4
        Private mem1 As Short
        Public Property Member1() As Short
            Get
                Return mem1
            End Get
            Set(ByVal value As Short)
                mem1 = value
            End Set
        End Property
    End Class
    Public Sub Sample4()
        'Writes a class and its members
        Dim iC4 = New C4()
        ObjectDumper.Write(iC4)
    End Sub

    Public Sub Sample5()
        'Writes an anonymous type
        Dim iAnon = New With {.name = "This is an anonymous type", .value = "First of its kind"}
        ObjectDumper.Write(iAnon)
    End Sub

    Public Sub Sample6()
        'Writes an array
        Dim intArr = New Integer() {1, 2, 3, 4, 5}
        ObjectDumper.Write(intArr)
    End Sub

    Public Structure Country
        Dim name As String
        Dim continent As String
    End Structure
    Public Sub Sample7()
        'Writes out the result of queries
        Dim countries = New Country() { _
            New Country With {.name = "USA", .continent = "North America"}, _
            New Country With {.name = "Pakistan", .continent = "Asia"}, _
            New Country With {.name = "Italy", .continent = "Europe"}, _
            New Country With {.name = "Canada", .continent = "North America"}}
        ObjectDumper.Write(From country In countries Where country.continent = "Asia" Select country.name, country.continent)
    End Sub
End Module
