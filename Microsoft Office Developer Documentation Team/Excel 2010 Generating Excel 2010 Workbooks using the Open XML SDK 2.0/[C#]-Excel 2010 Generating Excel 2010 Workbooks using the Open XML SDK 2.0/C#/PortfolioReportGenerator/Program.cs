using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            PortfolioReport report = new PortfolioReport("Steve");
            report.CreateReport();
            report = new PortfolioReport("Kelly");
            report.CreateReport();
            Console.WriteLine("Reports created!");
            Console.WriteLine("Press ENTER to quit.");
            Console.ReadLine();
        }
    }
}
