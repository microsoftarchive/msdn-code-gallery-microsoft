/************ 
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++DISCLAIMER++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 
------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 
The sample scripts are not supported under any Microsoft standard support program or service.The sample scripts are provided AS IS without warranty 
of any kind. Microsoft further disclaims all implied warranties including, without limitation, any implied warranties of merchantability or of fitness for 
a particular purpose.The entire risk arising out of the use or performance of the sample scripts and documentation remains with you.In no event shall 
Microsoft, its authors, or anyone else involved in the creation, production, or delivery of the scripts be liable for any damages whatsoever(including, 
without limitation, damages for loss of business profits, business interruption, loss of business information, or other pecuniary loss) arising out of the use 
of or inability to use the sample scripts or documentation, even if Microsoft has been advised of the possibility of such damages 
------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 
************/

/*********** 
 
Code by: 
Mayur Patankar 
IIS/ASP.NET team 
Microsoft iGTSC 

***********/

//including C# libraries 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace HttpCustomClients
{
 //Sends a Http Web Request to a server and whatever response it gets it prints it on the console 
    class Program
    {
        static void Main(string[] args)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(new Uri("http://da-vinci/suraj.htm"));//Create a HttpWebRequest object 

            httpWebRequest.Method = "GET";//Set the Method 

            httpWebRequest.KeepAlive = true;//Set Keep Alive

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();//Get the Response

            Console.WriteLine(httpWebResponse.StatusCode);//Print Status Code 

            Console.WriteLine(httpWebResponse.ContentType);//Print the Content Type

            Stream responseStrem =(Stream)httpWebResponse.GetResponseStream();//Get the REsponse Stream

            StreamReader responseReader = new StreamReader(responseStrem);//Read Response

            Console.WriteLine("Response Strem Received");

            Console.WriteLine(responseReader.ReadToEnd());//Print Response Stream 

            httpWebResponse.Close();

            responseStrem.Close();

            Console.ReadLine();


        }
    }
}
