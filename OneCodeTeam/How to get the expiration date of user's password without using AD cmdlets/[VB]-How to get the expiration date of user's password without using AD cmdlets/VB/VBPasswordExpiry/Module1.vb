Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.DirectoryServices

Module Module1

    Sub Main()
        Console.WriteLine("Enter domain")
        Dim userdomain As String = Console.ReadLine()
        Console.WriteLine("Enter userid")
        Dim userid As String = Console.ReadLine()
        Dim timespn As TimeSpan = GetTimeRemainingUntilPasswordExpiration(userdomain, userid)
        Console.WriteLine("Password expires in : {0}", timespn.ToString())
        Console.WriteLine([String].Format("You must change your password within" + " {0} days", timespn.Days))
        Console.ReadLine()
    End Sub

    Private Function GetTimeRemainingUntilPasswordExpiration(ByVal domain As String, ByVal userName As String) As TimeSpan
        Using userEntry = New System.DirectoryServices.DirectoryEntry(String.Format("WinNT://{0}/{1},user", domain, userName))
            Dim maxPasswordAge = CInt(userEntry.Properties.Cast(Of System.DirectoryServices.PropertyValueCollection)().First(Function(p) p.PropertyName = "MaxPasswordAge").Value)
            Dim passwordAge = CInt(userEntry.Properties.Cast(Of System.DirectoryServices.PropertyValueCollection)().First(Function(p) p.PropertyName = "PasswordAge").Value)
            Return TimeSpan.FromSeconds(maxPasswordAge) - TimeSpan.FromSeconds(passwordAge)
        End Using
    End Function
End Module
