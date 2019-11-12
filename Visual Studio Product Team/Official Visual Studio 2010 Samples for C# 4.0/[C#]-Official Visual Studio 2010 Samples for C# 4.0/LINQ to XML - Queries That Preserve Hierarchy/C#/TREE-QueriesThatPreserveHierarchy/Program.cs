using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace TREE_QueriesThatPreserveHierarchy
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.Q13();  // Prepare a (nested) table of contents for Book1, listing all the sections and their titles. Preserve the original attributes of each <section> element, if any.
            //samples.Q14();  // Prepare a (flat) figure list for Book1, listing all the figures and their titles. Preserve the original attributes of each <figure> element, if any.
            //samples.Q15();  // How many sections are in Book1, and how many figures?
            //samples.Q16();  // How many top-level sections are in Book1?
            //samples.Q17();  // Make a flat list of the section elements in Book1. In place of its original attributes, each section element should have two attributes, containing the title of the section and the number of figures immediately contained in the section.
            //samples.Q18();  // Make a nested list of the section elements in Book1, preserving their original attributes and hierarchy. Inside each section element, include the title of the section and an element that includes the number of figures immediately contained in the section.
        }

        private class LinqSamples
        {
            [Category("TREE - Queries that preserve hierarchy")]
            [Description(@"Prepare a (nested) table of contents for Book1, listing all the sections and their titles. Preserve the original attributes of each <section> element, if any.")]
            public void Q13()
            {
                XDocument book = XDocument.Load("book.xml");

                var result = new XElement("toc",
                                 TableOfContents(book.Element("book")));

                Console.WriteLine(result);

                // Solution in XQuery:
                // declare function local:toc($book-or-section as element()) as element()*
                // {
                //    for $section in $book-or-section/section
                //    return
                //    <section>
                //        { $section/@* , $section/title , local:toc($section) }                 
                //    </section>
                // };
                // <toc>
                // {
                //    for $s in doc(""book.xml"")/book return local:toc($s)
                // }
                // </toc>
            }

            public static IEnumerable<XElement> TableOfContents(XElement bookOrSection)
            {
                return from s in bookOrSection.Elements("section")
                       select new XElement("section",
                                  s.Attributes(),
                                  s.Element("title"),
                                  TableOfContents(s));
            }


            [Category("TREE - Queries that preserve hierarchy")]
            [Description(@"Prepare a (flat) figure list for Book1, listing all the figures and their titles. Preserve the original attributes of each <figure> element, if any.")]
            public void Q14()
            {
                XDocument book = XDocument.Load("book.xml");

                var result = new XElement("figlist",
                                 from f in book.Descendants("figure")
                                 select new XElement("figure",
                                            f.Attributes(),
                                            f.Elements("title")));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <figlist>
                // {
                //    for $f in doc(""book.xml"")//figure
                //    return
                //        <figure>
                //            { $f/@* }
                //            { $f/title }
                //        </figure>
                // }
                // </figlist>
            }


            [Category("TREE - Queries that preserve hierarchy")]
            [Description(@"How many sections are in Book1, and how many figures?")]
            public void Q15()
            {
                XDocument book = XDocument.Load("book.xml");

                var result = new XElement("results",
                                 new XElement("section_count",
                                     book.Descendants("section").Count()),
                                 new XElement("figure_count",
                                     book.Descendants("figure")
                                         .Count()));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <section_count>{ count(doc(""book.xml"")//section) }</section_count>, 
                // <figure_count>{ count(doc(""book.xml"")//figure) }</figure_count>
            }


            [Category("TREE - Queries that preserve hierarchy")]
            [Description(@"How many top-level sections are in Book1?")]
            public void Q16()
            {
                XDocument book = XDocument.Load("book.xml");

                var result = new XElement("top_section_count",
                                 book.Element("book")
                                     .Elements("section")
                                     .Count());

                Console.WriteLine(result);

                // Solution in XQuery:
                // <top_section_count>
                // { 
                //    count(doc(""book.xml"")/book/section) 
                // }
                // </top_section_count>
            }


            [Category("TREE - Queries that preserve hierarchy")]
            [Description(@"Make a flat list of the section elements in Book1. In place of its original attributes, each section element should have two attributes, containing the title of the section and the number of figures immediately contained in the section.")]
            public void Q17()
            {
                XDocument book = XDocument.Load("book.xml");

                var result = new XElement("section_list",
                                 from s in book.Descendants("section")
                                 select new XElement("section",
                                            new XAttribute("title",
                                                s.Element("title").Value),
                                            new XAttribute("figcount",
                                                s.Elements("figure").Count())));

                Console.WriteLine(result);

                // Solution in XQuery:
                // <section_list>
                // {
                //   for $s in doc(""book.xml"")//section
                //   let $f := $s/figure
                //   return
                //       <section title=""{ $s/title/text() }"" figcount=""{ count($f) }""/>
                // }
                // </section_list>
            }


            [Category("TREE - Queries that preserve hierarchy")]
            [Description(@"Make a nested list of the section elements in Book1, preserving their original attributes and hierarchy. Inside each section element, include the title of the section and an element that includes the number of figures immediately contained in the section.")]
            public void Q18()
            {
                XDocument book = XDocument.Load("book.xml");

                var result = new XElement("toc",
                                 SectionSummary(book.Element("book")
                                                    .Elements("section")));

                Console.WriteLine(result);

                // Solution in XQuery:
                // declare function local:section-summary($book-or-section as element()*)
                //  as element()*
                // {
                //  for $section in $book-or-section
                //  return
                //    <section>
                //       { $section/@* }
                //       { $section/title }       
                //       <figcount>         
                //         { count($section/figure) }
                //       </figcount>                
                //       { local:section-summary($section/section) }                      
                //    </section>
                // };
                // <toc>
                //  {
                //    for $s in doc(""book.xml"")/book/section
                //    return local:section-summary($s)
                //  }
                // </toc>
            }

            public static IEnumerable<XElement> SectionSummary(IEnumerable<XElement> bookOrSection)
            {
                return from section in bookOrSection
                       select new XElement("section",
                                  section.Attributes(),
                                  section.Element("title"),
                                  new XElement("figcount",
                                      section.Elements("figure").Count()),
                                      SectionSummary(section.Elements("section")));
            }
        }
    }
}
