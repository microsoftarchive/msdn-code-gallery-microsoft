'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports System.Collections.ObjectModel


Public Class Cities
    Inherits ObservableCollection(Of City)
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Create a default list of cities and their latitudes and longitudes
    ''' </summary>
    Public Sub LoadDefaultData()
        App.cityList.Add(New City("Redmond, WA", "47.67", "-122.12"))
        App.cityList.Add(New City("Green Bay, WI", "44.5216", "-87.9898"))
        App.cityList.Add(New City("Tampa, FL", "27.959", "-82.4821"))
        App.cityList.Add(New City("Austin, TX", "30.267", "-97.743"))
        App.cityList.Add(New City("Santa Clara, CA", "37.3542", "-121.954"))
    End Sub

End Class

