using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

using System.Data;
using System.Data.SqlClient;

// desktop CLR
// using System.Runtime.Hosting;

namespace Fabrikam
{
    [Guid(EnterpriseServer.InterfaceId), ComVisible(true)]
    public interface IFoo   
    {
        IList<String> TestMethod(String input);
        event EventHandler<string> PeriodicEvent;
        IAsyncOperation<int> FindElementAsync(int input);
        string[] RetrieveData();
    }

    [Guid(EnterpriseServer.ClassId), ComVisible(true)]
    public sealed class EnterpriseServer : IFoo
    {
        internal const string ClassId = "cc998934-4d63-481e-b7fa-fe65396c373c";
        internal const string InterfaceId = "96f6f9c8-253d-47f3-8d1e-55f9d5a42a10";

        public EnterpriseServer()
        {
            FirePeriodicEvent();
        }

        public IList<String> TestMethod(String input)
        {
            var list = new List<String>();
            list.Add("Foo");
            list.Add("Bar");
            return list;
        }

        public IAsyncOperation<int> FindElementAsync(int input)
        {
            return Task<int>.Run(async () =>
            {
                var random = new System.Random();
                int retval = random.Next(10 * input);

                // make this API take 2 seconds to complete, so must be async, 
                // e.g. simulate calculating nth digit of pi
                var twoSecondSpan = new TimeSpan(0, 0, 2);  
                await Task.Delay(twoSecondSpan);

                return retval;
            }).AsAsyncOperation<int>();
        }

        /// <summary>
        /// This is a trivial example that uses System.Data.DataSet to iterate over XML data.
        /// A real enterprise desktop server would use System.Data over a data source
        /// such as SQL Server.  
        /// </summary>
        /// <returns></returns>
        public string[] RetrieveData()
        {
            string myXMLfile = "C:\\test\\CustomerData.xml";
            DataSet ds = new DataSet();
            var list = new List<string>();
            try
            {
                ds.ReadXml(myXMLfile);

                foreach (DataTable table in ds.Tables)
                {
                    if (table.TableName != "Customer")
                    {
                        continue;
                    }
                    foreach (DataRow row in table.Rows)
                    {
                        list.Add(row[0].ToString());    // CustomerName
                    }
                }

            }
            catch (Exception ex)
            {
                list.Add(ex.ToString());
            }
            string[] results = new string[list.Count];
            list.CopyTo(results);
            return results;
        }

        private void FirePeriodicEvent()
        {
            Task.Run(async () => 
            {
                var fiveSecondSpan = new TimeSpan(0, 0, 5);
                await DoPeriodicWorkAsync(fiveSecondSpan, fiveSecondSpan, 100);
            });
        }

        private async Task DoPeriodicWorkAsync(TimeSpan dueTime, TimeSpan interval, int count)
        {
           // wait the initial delay
           if (dueTime > TimeSpan.Zero)
           {
               await Task.Delay(dueTime);
           }

           // Repeat this loop until 'count' iterations have been done
           while (--count > 0)
           {
               PeriodicEvent(this, DateTime.Now.ToString());

               // Wait to repeat again.
               if (interval > TimeSpan.Zero)
               {
                   await Task.Delay(interval);       
               }
            }
        }

        public event EventHandler<string> PeriodicEvent;
    }
}
