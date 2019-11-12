Imports System.Collections.Generic
Imports LinqToTerraServer.TerraServerReference

Friend Module WebServiceHelper
    Private numResults As Integer = 200
    Private mustHaveImage As Boolean = False

    Friend Function GetPlacesFromTerraServer(ByVal locations As List(Of String)) As Place()
        ' Limit the total number of Web service calls.
        If locations.Count > 5 Then
            Dim s = "This query requires more than five separate calls to the Web service. Please decrease the number of places."
            Throw New InvalidQueryException(s)
        End If

        Dim allPlaces As New List(Of Place)

        ' For each location, call the Web service method to get data.
        For Each location In locations
            Dim places = CallGetPlaceListMethod(location)
            allPlaces.AddRange(places)
        Next

        Return allPlaces.ToArray()
    End Function

    Private Function CallGetPlaceListMethod(ByVal location As String) As Place()

        Dim client As New TerraServiceSoapClient()
        Dim placeFacts() As PlaceFacts

        Try
            ' Call the Web service method "GetPlaceList".
            placeFacts = client.GetPlaceList(location, numResults, mustHaveImage)

            ' If we get exactly 'numResults' results, they are probably truncated.
            If (placeFacts.Length = numResults) Then
                Dim s = "The results have been truncated by the Web service and would not be complete. Please try a different query."
                Throw New Exception(s)
            End If

            ' Create Place objects from the PlaceFacts objects returned by the Web service.
            Dim places(placeFacts.Length - 1) As Place
            For i = 0 To placeFacts.Length - 1
                places(i) = New Place(placeFacts(i).Place.City, _
                                      placeFacts(i).Place.State, _
                                      placeFacts(i).PlaceTypeId)
            Next

            ' Close the WCF client.
            client.Close()

            Return places
        Catch timeoutException As TimeoutException
            client.Abort()
            Throw
        Catch communicationException As System.ServiceModel.CommunicationException
            client.Abort()
            Throw
        End Try
    End Function
End Module
