Imports System.Attribute
Imports System.ServiceModel.Activation

'This class is the implementation of the IDayInfo service
'contract. It defines two simple methods that return the 
'names of days as strings. When you have deployed this 
'WCF service to Windows Azure, you can call it from the 
'silverlight client application. 

'N.B the clientaccesspolicy.xml file in this project must be
'deployed with this service in order for Silverlight to be
'permitted to access the service in Azure. Use Package in the 
'AZURE_DayNamerService project and the file will be automatically 
'included in the service package file

'Before you deploy this service to Azure, you must have a 
'Azure account set up. Use Package in the AZURE_DayNamerService
'project to create the service package file and cloud service
'configuration file. Then, in the Azure Management Portal, create
'a new hosted service and specify these files. Once that's
'done you should be able to access the service at
'http://YourHostedServiceName.cloudapp.net/DayInfoService.svc

<ServiceBehavior(AddressFilterMode:=AddressFilterMode.Any)> _
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)> _
Public Class DayInfoService
    Implements IDayNamer

    Public Function TodayAdd(ByVal daysToAdd As Integer) As String Implements IDayNamer.TodayAdd
        'Add the requested number to today
        Dim requestedDateTime As DateTime = DateTime.Today.AddDays(daysToAdd)
        Dim requestedDay As DayOfWeek = requestedDateTime.DayOfWeek
        'Return today's name
        Return requestedDay.ToString()
    End Function

    Public Function TodayIs() As String Implements IDayNamer.TodayIs
        'Find out what today is
        Dim today As DayOfWeek = DateTime.Today.DayOfWeek
        'Return today's name
        Return today.ToString()
    End Function

End Class
