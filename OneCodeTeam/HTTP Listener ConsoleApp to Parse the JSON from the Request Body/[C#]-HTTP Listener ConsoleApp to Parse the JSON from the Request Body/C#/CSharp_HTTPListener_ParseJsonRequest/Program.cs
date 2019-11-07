//DISCLAIMER

//The sample scripts are not supported under any Microsoft standard support program or service.
//The sample scripts are provided AS IS without warranty of any kind.Microsoft further disclaims all implied warranties including, without limitation, 
//any implied warranties of merchantability or of fitness for a particular purpose.The entire risk arising out of the use or performance of the sample 
//scripts and documentation remains with you. In no event shall Microsoft, its authors, or anyone else involved in the creation, production, or delivery of 
//the scripts be liable for any damages whatsoever (including without limitation, damages for loss of business profits, business interruption, loss of business 
//information, or other pecuniary loss) arising out of the use of or inability to use the sample scripts or documentation, even if Microsoft has been advised of
//the possibility of such damages.


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;


namespace CSharp_HTTPListener_ParseJsonRequest
{
    class Program
    {
        static void Main(string[] args)
        {


            // Create an instance of HTTP Listener class
            var httpListener = new HttpListener
            {
                Prefixes = { "http://+:80/" },

            };

            httpListener.Start();
            var context = httpListener.GetContext();
            var request = context.Request;


            // Waiting synchronously till the request is received from the client for HTTP over port 80
            while (true)
            {

                System.IO.Stream body = request.InputStream;
                System.Text.Encoding encoding = request.ContentEncoding;
                System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
                if (request.ContentType != null)
                {
                    Console.WriteLine("Client data content type {0}", request.ContentType);
                }
                Console.WriteLine("Client data content length {0}", request.ContentLength64);

                Console.WriteLine("Start of client JSON data:");
                // Convert the data to a string and display it on the console.
                string s = reader.ReadToEnd();
                Console.WriteLine(s);




                Console.WriteLine("End of client data:");

                Console.WriteLine("Parsing the JSON Request Body.....");

                /************************You may have to modify the below based on your JSON in Request Body ***/
                var jsonObj = JObject.Parse(s);
                Console.WriteLine("Country : " + (string)jsonObj["country_name"]);
                Console.WriteLine("Country Code : " + (string)jsonObj["country_code"]);
                Console.WriteLine("Region Code : " + (string)jsonObj["region_code"]);
                Console.WriteLine("City : " + (string)jsonObj["city"]);
                Console.WriteLine("Zip Code :" + (string)jsonObj["zipcode"]);
                Console.WriteLine("Latitude :" + (string)jsonObj["latitude"]);


                /************************SQL server DB call goes below ************************
                 * 
                 * 
                 * 
                 * string  connetionString = "Data Source=ServerName;Initial Catalog=DatabaseName;User ID=UserName;Password=Password";
                 * SqlConnection con = new SqlConnection(connetionString);
                 * SqlDataReader sqlReader;
                 * con.Open();


                sqlReader = new SqlCommand("select * from Country where city=" + jsonObj["city"], con).ExecuteReader();

                if (sqlReader.HasRows)
                {
                    while (sqlReader.Read())
                    {
                        Console.WriteLine("CountryName | CountryCode | ZipCode \n {0}  |   {1}  |   {2}", sqlReader.GetInt32(0),
                        sqlReader.GetString(1), sqlReader.GetInt32(2));
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                sqlReader.Close();
                *
                * 
                * 
                * 
                * 
                * 
                ---------------------------------------------------------------------------*/

                Console.ReadLine();

            }
        }
    }
}
