Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace PortfolioReportGenerator
    Class Program
        Public Shared Sub Main(ByVal args As String())
            Dim report As New PortfolioReport("Steve")
            report.CreateReport()
            report = New PortfolioReport("Kelly")
            report.CreateReport()
            Console.WriteLine("Reports created!")
            Console.WriteLine("Press ENTER to quit.")
            Console.ReadLine()
        End Sub
    End Class
End Namespace