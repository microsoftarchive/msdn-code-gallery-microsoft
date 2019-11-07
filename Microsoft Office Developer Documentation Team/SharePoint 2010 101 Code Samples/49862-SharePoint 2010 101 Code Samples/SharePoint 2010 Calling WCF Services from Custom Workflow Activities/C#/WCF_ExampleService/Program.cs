using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is the main WCF namespace
using System.ServiceModel;
//This namespace is used for self-hosting the service
using System.ServiceModel.Description;

namespace WCF_ExampleService
{
    //Define the service contract
    [ServiceContract(Name="http://WCF_ExampleService")]
    public interface IDayNamer
    {
        [OperationContract]
        string TodayIs();
        [OperationContract]
        string TodayAdd(int daysToAdd);
    }

    //This class implements the WCF service
    public class DayNamerService : IDayNamer
    {
        //This simple method returns today's name
        public string TodayIs()
        {
            //Find out what today is
            DayOfWeek today = DateTime.Today.DayOfWeek;
            //output to the console
            Console.WriteLine("Received a TodayIs call.");
            Console.WriteLine("Returned {0}", today.ToString());
            //Return today's name
            return today.ToString();
        }

        //This method adds the requested number of days to today and returns the name of that day
        public string TodayAdd(int dayToAdd)
        {
            //Add the requested number to today
            DateTime requestedDateTime = DateTime.Today.AddDays(dayToAdd);
            DayOfWeek requestedDay = requestedDateTime.DayOfWeek;
            //output to the console
            Console.WriteLine("Received a TodayAdd call.:");
            Console.WriteLine("Days to add: {0}", dayToAdd.ToString());
            Console.WriteLine("Returned {0}", requestedDay.ToString());
            //Return today's name
            return requestedDay.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //This example WCF service is self-hosted in
            //the command console. This procedure runs the
            //service until the user presses a key

            //This is the address for the WCF service
            Uri baseAddress = new Uri("http://localhost:8088/WCF_ExampleService/Service");
            ServiceHost selfHost = new ServiceHost(typeof(DayNamerService), baseAddress);
            try
            {
                //Create an endpoint
                selfHost.AddServiceEndpoint(typeof(IDayNamer), new WSHttpBinding(), "DayNamerService");
                //Enable the service to exchange its metadata
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);
                //Open the service and tell the user
                selfHost.Open();
                Console.WriteLine("The Day Namer Service is ready");
                Console.WriteLine("Press any key to close the service");
                //Wait for the user to press a key
                Console.ReadKey();
                //Close the service
                selfHost.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("A communication exception occurred: {0}", e.Message);
                selfHost.Abort();
            }
        }
    }
}
