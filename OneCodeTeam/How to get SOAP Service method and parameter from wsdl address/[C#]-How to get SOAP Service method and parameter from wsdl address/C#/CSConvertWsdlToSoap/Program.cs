using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Net;

namespace CSConvertWsdlToSoap
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the wcf service wsdl xml and convert to soap xml format per method
            // The output is a dicionary, each entry is the soap xml format of that method.
            var output = new WsdlToSoap(File.ReadAllText("test.svc")).SoapMethodDictionary;

            // The test.svc wsdl contains a method "GetData", please change the method name here according to your WCF service
            var soapXmlStringGetData = output["GetData"];

            WebClient client = new WebClient();

            // the Content-Type needs to be set to XML
            client.Headers.Add("Content-Type", "text/xml;charset=utf-8");

            // The SOAPAction header indicates which method you would like to invoke
            // and could be seen in the WSDL: <soap:operation soapAction="..." /> element
            client.Headers.Add("SOAPAction", "\"http://tempuri.org/IService1/GetData\"");

            // Please enter the your wcf service address here
            var response = client.UploadString("http://localhost:6742/Service1.svc", soapXmlStringGetData);

            // get the response
            Console.WriteLine(response);
        }
    }
}
