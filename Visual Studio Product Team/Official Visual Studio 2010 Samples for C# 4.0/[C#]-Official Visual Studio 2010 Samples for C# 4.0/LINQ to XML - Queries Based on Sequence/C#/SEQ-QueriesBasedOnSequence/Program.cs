using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace SEQ_QueriesBasedOnSequence
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.Q19();  // In the Procedure section of Report1, what Instruments were used in the second Incision?
            //samples.Q20();  // In the Procedure section of Report1, what are the first two Instruments to be used?
            //samples.Q21();  // In Report1, what Instruments were used in the first two Actions after the second Incision?
            //samples.Q22();  // In Report1, find ""Procedure"" sections where no Anesthesia element occurs before the first Incision
            //samples.Q23();  // In Report1, what happened between the first Incision and the second Incision?
        }

        private class LinqSamples
        {
            [Category("SEQ - Queries based on Sequence")]
            [Description(@"In the Procedure section of Report1, what Instruments were used in the second Incision?")]
            public void Q19()
            {
                XDocument report = XDocument.Load("report1.xml");

                var result = report.Descendants("section")
                                   .Where(s => s.Element("section.title").Value == "Procedure")
                                   .Descendants("incision")
                                   .ElementAt(1)
                                   .Elements("instrument");

                foreach (XElement e in result)
                    Console.WriteLine(e);

                // Solution in XQuery:
                // for $s in doc(""report1.xml"")//section[section.title = ""Procedure""]
                // return ($s//incision)[2]/instrument
            }


            [Category("SEQ - Queries based on Sequence")]
            [Description(@"In the Procedure section of Report1, what are the first two Instruments to be used?")]
            public void Q20()
            {
                XDocument report = XDocument.Load("report1.xml");

                var result = report.Descendants("section")
                                   .Where(s => s.Element("section.title").Value == "Procedure")
                                   .Descendants("instrument")
                                   .Take(2);

                foreach (XElement e in result)
                    Console.WriteLine(e);

                // Solution in XQuery:
                // for $s in doc(""report1.xml"")//section[section.title = ""Procedure""]
                // return ($s//instrument)[position()<=2]
            }


            [Category("SEQ - Queries based on Sequence")]
            [Description(@"In Report1, what Instruments were used in the first two Actions after the second Incision?")]
            public void Q21()
            {
                XDocument report = XDocument.Load("report1.xml");

                var i2 = report.Descendants("incision")
                               .ElementAt(1);

                var result = report.Descendants()
                                   .SkipWhile(e => !e.Equals(i2))
                                   .Where(s => s.Name == "action")
                                   .Take(2)
                                   .Descendants("instrument");

                foreach (XElement e in result)
                    Console.WriteLine(e);

                // Solution in XQuery:
                // let $i2 := (doc(""report1.xml"")//incision)[2]
                // for $a in (doc(""report1.xml"")//action)[. >> $i2][position()<=2]
                // return $a//instrument
            }


            [Category("SEQ - Queries based on Sequence")]
            [Description(@"In Report1, find ""Procedure"" sections where no Anesthesia element occurs before the first Incision")]
            public void Q22()
            {
                XDocument report = XDocument.Load("report1.xml");

                var result = from p in report.Descendants("section")
                             let e = p.Descendants()
                                      .TakeWhile(s => s.Name != "incision")
                             where p.Element("section.title").Value == "Procedure" &&
                                   !e.Any(a => a.Name == "anesthesia")
                             select p;

                foreach (XElement e in result)
                    Console.WriteLine(e);

                // Solution in XQuery:
                // for $p in doc(""report1.xml"")//section[section.title = ""Procedure""]
                // where not(some $a in $p//anesthesia satisfies
                //        $a << ($p//incision)[1] )
                // return $p
            }


            [Category("SEQ - Queries based on Sequence")]
            [Description(@"In Report1, what happened between the first Incision and the second Incision?")]
            public void Q23()
            {

                XDocument report = XDocument.Load("report1.xml");

                var proc = (from p in report.Descendants("section")
                            where p.Element("section.title").Value == "Procedure"
                            select p).ElementAt(0);

                var i1 = proc.Descendants("incision")
                             .ElementAt(0);
                var i2 = proc.Descendants("incision")
                             .ElementAt(1);

                var result = new XElement("critical_sequence",
                                 proc.DescendantNodes()
                                     .Except(i1.DescendantNodes())
                                     .SkipWhile<XNode>(e => e != i1)
                                     .Skip<XNode>(1)
                                     .TakeWhile<XNode>(e => e != i2));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <critical_sequence>
                // {
                //  let $proc := doc(""report1.xml"")//section[section.title=""Procedure""][1],
                //      $i1 :=  ($proc//incision)[1],
                //      $i2 :=  ($proc//incision)[2]
                //  for $num in $proc//node() except $i1//node()
                //  where $num >> $i1 and $num << $i2
                //  return $num 
                // }
                // </critical_sequence>
            }
        }
    }
}
