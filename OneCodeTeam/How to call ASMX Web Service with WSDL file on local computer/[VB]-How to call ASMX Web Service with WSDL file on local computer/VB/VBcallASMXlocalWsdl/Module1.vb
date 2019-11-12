Module Module1

    '****************************** Module Header ******************************\
    'Module Name:    Program.cs
    'Project:        CScallASMXlocalWsdl
    'Copyright (c) Microsoft Corporation

    ' The project illustrates how to check whether a file is in use or not.

    'This source is subject to the Microsoft Public License.
    'See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
    'All other rights reserved.

    '*****************************************************************************/

    Sub Main()
        Using webService = New WebReference.WebService1()

            'invoke the web method
            'pass different parameter (integer) to test the application
            Dim result = webService.GetData(10)

            Console.WriteLine(result)

        End Using
        Console.ReadLine()
    End Sub

End Module
