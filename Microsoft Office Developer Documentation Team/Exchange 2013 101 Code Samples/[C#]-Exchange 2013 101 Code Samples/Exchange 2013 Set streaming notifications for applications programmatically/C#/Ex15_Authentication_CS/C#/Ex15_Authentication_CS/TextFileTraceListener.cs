using System;
using System.IO;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
  // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
  public class TraceListener : ITraceListener
  {
    public void Trace(string traceType, string traceMessage)
    {
      CreateXMLTextFile(traceType, traceMessage);
    }

    private void CreateXMLTextFile(string fileName, string traceContent)
    {
      try
      {
        if (!Directory.Exists(@"..\\TraceOutput"))
        {
          Directory.CreateDirectory(@"..\\TraceOutput");
        }

        System.IO.File.WriteAllText(@"..\\TraceOutput\\" + fileName + DateTime.Now.Ticks + ".txt", traceContent);
      }
      catch (IOException ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}
