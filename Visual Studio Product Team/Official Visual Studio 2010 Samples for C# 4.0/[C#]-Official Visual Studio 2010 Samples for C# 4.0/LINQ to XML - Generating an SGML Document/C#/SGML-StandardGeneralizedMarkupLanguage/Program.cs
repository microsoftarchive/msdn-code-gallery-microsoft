using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.ComponentModel;

namespace SGML_StandardGeneralizedMarkupLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.Q42();  // Locate all paragraphs in the report (all ""para"" elements occurring anywhere within the ""report"" element).
            //samples.Q43();  // Locate all paragraph elements in an introduction (all ""para"" elements directly contained within an ""intro"" element).
            //samples.Q44();  // Locate all paragraphs in the introduction of a section that is in a chapter that has no introduction (all ""para"" elements directly contained within an ""intro"" element directly contained in a ""section"" element directly contained in a ""chapter"" element. The ""chapter"" element must not directly contain an ""intro"" element).
            //samples.Q45();  // Locate the second paragraph in the third section in the second chapter (the second ""para"" element occurring in the third ""section"" element occurring in the second ""chapter"" element occurring in the ""report"").
            //samples.Q46();  // Locate all classified paragraphs (all ""para"" elements whose ""security"" attribute has the value ""c"").
            //samples.Q47();  // List the short titles of all sections (the values of the ""shorttitle"" attributes of all ""section"" elements, expressing each short title as the value of a new element.)
            //samples.Q48();  // Locate the initial letter of the initial paragraph of all introductions (the first character in the content [character content as well as element content] of the first ""para"" element contained in an ""intro"" element).
            //samples.Q49();  // Locate all sections with a title that has ""is SGML"" in it. The string may occur anywhere in the descendants of the title element, and markup boundaries are ignored.
            //samples.Q50();  // Same as (Q8a), but the string ""is SGML"" cannot be interrupted by sub-elements, and must appear in a single text node.
            //samples.Q51();  // Locate all the topics referenced by a cross-reference anywhere in the report (all the ""topic"" elements whose ""topicid"" attribute value is the same as an ""xrefid"" attribute value of any ""xref"" element).
            //samples.Q52();  // Locate the closest title preceding the cross-reference (""xref"") element whose ""xrefid"" attribute is ""top4"" (the ""title"" element that would be touched last before this ""xref"" element when touching each element in document order).
        }

        private class LinqSamples
        {
            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate all paragraphs in the report (all ""para"" elements occurring anywhere within the ""report"" element).")]
            public void Q42()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 sgml.Descendants("report")
                                     .Descendants("para"));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // { 
                //    doc(""sgml.xml"")//report//para 
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate all paragraph elements in an introduction (all ""para"" elements directly contained within an ""intro"" element).")]
            public void Q43()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 sgml.Descendants("intro")
                                     .Elements("para"));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // { 
                //    doc(""sgml.xml"")//intro/para 
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate all paragraphs in the introduction of a section that is in a chapter that has no introduction (all ""para"" elements directly contained within an ""intro"" element directly contained in a ""section"" element directly contained in a ""chapter"" element. The ""chapter"" element must not directly contain an ""intro"" element).")]
            public void Q44()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 from c in sgml.Descendants("chapter")
                                 where !c.Elements("intro")
                                         .Any()
                                 select c.Elements("section")
                                         .Elements("intro")
                                         .Elements("para"));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    for $c in doc(""sgml.xml"")//chapter
                //    where empty($c/intro)
                //    return $c/section/intro/para
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate the second paragraph in the third section in the second chapter (the second ""para"" element occurring in the third ""section"" element occurring in the second ""chapter"" element occurring in the ""report"").")]
            public void Q45()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 sgml.Descendants("chapter")
                                     .ElementAt(1)
                                     .Descendants("section")
                                     .ElementAt(2)
                                     .Descendants("para")
                                     .ElementAt(1));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    (((doc(""sgml.xml"")//chapter)[2]//section)[3]//para)[2]
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate all classified paragraphs (all ""para"" elements whose ""security"" attribute has the value ""c"").")]
            public void Q46()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 from p in sgml.Descendants("para")
                                 where (string)p.Attribute("security") == "c"
                                 select p);

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    doc(""sgml.xml"")//para[@security = ""c""]
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"List the short titles of all sections (the values of the ""shorttitle"" attributes of all ""section"" elements, expressing each short title as the value of a new element.)")]
            public void Q47()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                XElement result = new XElement("result",
                                      from s in sgml.Descendants("section")
                                                    .Attributes("shorttitle")
                                      select new XElement("stitle", s));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    for $s in doc(""sgml.xml"")//section/@shorttitle
                //    return <stitle>{ $s }</stitle>
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate the initial letter of the initial paragraph of all introductions (the first character in the content [character content as well as element content] of the first ""para"" element contained in an ""intro"" element).")]
            public void Q48()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 from i in sgml.Descendants("intro")
                                 let p = i.Elements("para")
                                          .ElementAt(0)
                                 select new XElement("first_letter",
                                            p.Value[0]));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    for $i in doc(""sgml.xml"")//intro/para[1]
                //    return
                //        <first_letter>{ substring(string($i), 1, 1) }</first_letter>
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate all sections with a title that has ""is SGML"" in it. The string may occur anywhere in the descendants of the title element, and markup boundaries are ignored.")]
            public void Q49()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 from s in sgml.Descendants("section")
                                 where (from t in s.Descendants("title")
                                        where t.Value.Contains("is SGML")
                                        select t).Any()
                                 select s);

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    doc(""sgml.xml"")//section[.//title[contains(., ""is SGML"")]]
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Same as (Q8a), but the string ""is SGML"" cannot be interrupted by sub-elements, and must appear in a single text node.")]
            public void Q50()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 from s in sgml.Descendants("section")
                                 where (from n in s.Descendants("title").Nodes()
                                        where n is XText && ((XText)n).Value.Contains("is SGML")
                                        select n).Any()
                                 select s);

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    doc(""sgml.xml"")//section[.//title/text()[contains(., ""is SGML"")]]
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate all the topics referenced by a cross-reference anywhere in the report (all the ""topic"" elements whose ""topicid"" attribute value is the same as an ""xrefid"" attribute value of any ""xref"" element).")]
            public void Q51()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var result = new XElement("result",
                                 from id in sgml.Descendants("xref")
                                                .Attributes("xrefid")
                                 join t in sgml.Descendants("topic")
                                        on (string)id equals
                                           (string)t.Attribute("topicid")
                                 select t);

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    for $id in doc(""sgml.xml"")//xref/@xrefid
                //    return doc(""sgml.xml"")//topic[@topicid = $id]
                // }
                // </result>
            }


            [Category("SGML - Standard Generalized Markup Language")]
            [Description(@"Locate the closest title preceding the cross-reference (""xref"") element whose ""xrefid"" attribute is ""top4"" (the ""title"" element that would be touched last before this ""xref"" element when touching each element in document order).")]
            public void Q52()
            {
                XDocument sgml = null;
                // XDocument load by default does not resolve external entities so passing in our
                // own reader with the correct setting.
                XmlReaderSettings rs = new XmlReaderSettings();
                rs.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader r = XmlReader.Create("sgml.xml", rs))
                {
                    sgml = XDocument.Load(r);
                }

                var x = from id in sgml.Descendants("xref")
                        where id.Attribute("xrefid").Value == "top4"
                        select id;

                var t = from e in sgml.Descendants("title")
                        where e.IsBefore(x.ElementAt(0))
                        select e;

                var result = new XElement("result",
                    t.ElementAt(t.Count() - 1 > 0 ? t.Count() - 1 : 0));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <result>
                // {
                //    let $x := doc(""sgml.xml"")//xref[@xrefid = ""top4""],
                //        $t := doc(""sgml.xml"")//title[. << $x]
                //    return $t[last()]
                // }
                // </result>
            }
        }
    }
}
