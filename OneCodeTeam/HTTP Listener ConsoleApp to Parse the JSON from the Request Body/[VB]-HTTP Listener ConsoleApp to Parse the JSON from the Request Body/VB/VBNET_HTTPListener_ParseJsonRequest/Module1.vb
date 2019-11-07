'//DISCLAIMER

'//The sample scripts are Not supported under any Microsoft standard support program Or service.
'//The sample scripts are provided AS Is without warranty of any kind.Microsoft further disclaims all implied warranties including, without limitation, 
'//any implied warranties of merchantability Or of fitness for a particular purpose.The entire risk arising out of the use Or performance of the sample 
'//scripts And documentation remains with you. In no event shall Microsoft, its authors, Or anyone else involved in the creation, production, Or delivery of 
'//the scripts be liable for any damages whatsoever (including without limitation, damages for loss of business profits, business interruption, loss of business 
'//information, Or other pecuniary loss) arising out of the use of Or inability to use the sample scripts Or documentation, even if Microsoft has been advised of
'//the possibility of such damages.

Imports System
Imports System.Web
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading.Tasks
Imports System.Net
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Data
Imports System.Data.SqlClient






Module Module1

    Sub Main()


        ' Create an instance of HTTP Listener class

        Dim httpListener As New HttpListener


        httpListener.Prefixes.Add("http://+:80/")
        httpListener.Start()
        Dim context = httpListener.GetContext()
        Dim request = context.Request


        ' Waiting synchronously till the request is received from the client for HTTP over port 80
        While True

            Dim body As System.IO.Stream = request.InputStream
            Dim encoding As System.Text.Encoding = request.ContentEncoding
            Dim reader As New System.IO.StreamReader(body, encoding)
            If request.ContentType IsNot Nothing Then
                Console.WriteLine("Client data content type {0}", request.ContentType)
            End If
            Console.WriteLine("Client data content length {0}", request.ContentLength64)

            Console.WriteLine("Start of client JSON data:")
            ' Convert the data to a string and display it on the console.
            Dim s As String = reader.ReadToEnd()
            Console.WriteLine(s)




            Console.WriteLine("End of client data:")

            Console.WriteLine("Parsing the JSON Request Body.....")

            '***********************You may have to modify the below based on your JSON in Request Body **

            '  Dim jsonObj = JObject.Parse(s)
            Dim jsonObj = JObject.Parse(s)
            Console.WriteLine(("Country : " + CType(jsonObj("country_name"), String)))
            Console.WriteLine(("Country Code : " + CType(jsonObj("country_code"), String)))
            Console.WriteLine(("Region Code : " + CType(jsonObj("region_code"), String)))
            Console.WriteLine(("City : " + CType(jsonObj("city"), String)))
            Console.WriteLine(("Zip Code :" + CType(jsonObj("zipcode"), String)))
            Console.WriteLine(("Latitude :" + CType(jsonObj("latitude"), String)))

            '***********************SQL server DB call goes below ************************
            '                 * 
            '                 * 
            '                 *
            'Dim connetionString As String = "Data Source=ServerName;Initial Catalog=DatabaseName;User ID=UserName;Password=Password"
            'Dim con As SqlConnection = New SqlConnection(connetionString)
            'Dim sqlReader As SqlDataReader
            'con.Open()


            'sqlReader = New SqlCommand("select * from Country where city=" + CType(jsonObj("city"),String), con).ExecuteReader()

            '               If (sqlReader.HasRows) Then
            '               {
            '                    While (sqlReader.Read())
            '                    {
            '                        Console.WriteLine("CountryName | CountryCode | ZipCode \n {0}  |   {1}  |   {2}", sqlReader.GetInt32(0),
            '                        sqlReader.GetString(1), sqlReader.GetInt32(2))
            '                    }
            '                }
            '                Else
            '                {
            '                    Console.WriteLine("No rows found.")
            '                }
            '                sqlReader.Close()
            '                *
            '                * 
            '                * 
            '                * 
            '                * 
            '                * 
            '                ---------------------------------------------------------------------------



            Console.ReadLine()
        End While
    End Sub

End Module
