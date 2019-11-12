using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;
using ConsoleApplication1.ServiceReference1;
using ConsoleApplication1.ServiceReference2;
using WCFContract;

namespace ConsoleApplication1
{
    class Program
    {  
        static void Main(string[] args)
        {
            //Talk2WebRole();
            //Talk2WorkroleViaWebRole();
            Talk2Workrole();
        }

        static void Talk2WebRole()
        {
            ServiceReference1.ContractClient cc = new ServiceReference1.ContractClient();
            var result = cc.GetRoleInfo();
            Console.WriteLine(result);
            Console.ReadLine();
        }

        static void Talk2WorkroleViaWebRole()
        {
            ServiceReference2.ContractClient cc = new ServiceReference2.ContractClient();
            var result = cc.GetRoleInfo();
            Console.WriteLine(result);
            Console.ReadLine();
        }

        static void Talk2Workrole()
        {
            ChannelFactory<WCFContract.IContract> factory;
            WCFContract.IContract channel;
            
            // You need to modify the endpoint address to fit yours.
            EndpointAddress endpoint = new EndpointAddress("net.tcp://127.0.0.1:5117/External");

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, false);
            factory = new ChannelFactory<WCFContract.IContract>(binding);

            channel = factory.CreateChannel(endpoint);

            Console.WriteLine(channel.GetRoleInfo());
            Console.Read();
        }

    }
}
