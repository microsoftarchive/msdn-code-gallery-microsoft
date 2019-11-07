Imports LinqToTerraServer

Module Module1

    Sub Main()
        Dim terraPlaces As New QueryableTerraServerData(Of Place)

        ' First query.
        Dim query = From place In terraPlaces _
                    Where place.Name = "Redmond" _
                    Select place.State

        Console.WriteLine("States that have a place named ""Redmond"":")
        For Each state In query
            Console.WriteLine(state)
        Next

        ' Second query.
        Dim count = (From place In terraPlaces _
                    Where place.Name.StartsWith("Lond") _
                    Select place).Count()

        Console.WriteLine(vbCrLf & "Number of places that start with ""Lond"": " & count & vbCrLf)

        ' Third query.
        Dim places = New String() {"Johannesburg", "Yachats", "Seattle"}

        Dim query3 = From place In terraPlaces _
                    Where places.Contains(place.Name) _
                    Order By place.State _
                    Select place.Name, place.State

        Console.WriteLine("Places, ordered by state, whose name is either ""Johannesburg"", ""Yachats"", or ""Seattle"":")
        For Each obj In query3
            Console.WriteLine(obj)
        Next

        Console.ReadLine()
    End Sub

End Module
