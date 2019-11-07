// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// XQuery Solutions within this file are excerpted from "XML Query Use Cases" (http://www.w3.org/TR/xquery-use-cases/)
// Copyright (c) 2007 W3C (R) (MIT, ERCIM, Keio).  All Rights Reserved.  W3C liability, trademark and document use rules apply. 
// The Status section of "XML Query Use Cases" states as follows:
// Status of this Document
// This section describes the status of this document at the time of its publication. Other documents may supersede this document. A list of current W3C publications and the latest revision of this technical report can be found in the W3C technical reports index at http://www.w3.org/TR/.
// This is the W3C Working Group Note of "XML Query (XQuery) Use Cases", produced by the W3C XML Query Working Group, part of the XML Activity. This document is being published as a Working Group Note to persistently record the Use Cases that guided the development of XQuery 1.0: An XML Query Language and its associated specifications as W3C Recommendations.
// Please submit comments about this document using W3C's public Bugzilla system (instructions can be found at http://www.w3.org/XML/2005/04/qt-bugzilla). If access to that system is not feasible, you may send your comments to the W3C XSLT/XPath/XQuery public comments mailing list, public-qt-comments@w3.org. It will be very helpful if you include the string [XQRecUseCases] in the subject line of your report, whether made in Bugzilla or in email. Each Bugzilla entry and email message should contain only one comment. Archives of the comments and responses are available at http://lists.w3.org/Archives/Public/public-qt-comments/.
// Publication as a Working Group Note does not imply endorsement by the W3C Membership. At the time of publication, work on this document was considered complete and no further revisions are anticipated. It is a stable document and may be used as reference material or cited from another document. However, this document may be updated, replaced, or made obsolete by other documents at any time.
// This document was produced by a group operating under the 5 February 2004 W3C Patent Policy. W3C maintains a public list of any patent disclosures made in connection with the deliverables of the group; that page also includes instructions for disclosing a patent. An individual who has actual knowledge of a patent which the individual believes contains Essential Claim(s) must disclose the information in accordance with section 6 of the W3C Patent Policy .

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using SampleSupport;

namespace SampleQueries {
    [Title("Bonus: XQuery Use Cases")]
    [Prefix("Q")]
    public class XQueryUseCases : SampleHarness {

        private readonly static string dataPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Data\"));
        // 1.1 Use Case "XMP": Experiences and Exemplars

        [Category("XMP - Experiences and Exemplars")]
        [Title("Q1")]
        [Description(@"List books published by Addison-Wesley after 1991, including their year and title.")]
        public void Q1()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var result = new XElement("bib",
                             from b in bib.Element("bib").Elements("book")
                             where (string)b.Element("publisher") == "Addison-Wesley" && 
                                   (int)b.Attribute("year") > 1991
                             select new XElement("book",
                                        new XAttribute("year",b.Attribute("year").Value),
                                        b.Element("title")));
            
            Console.WriteLine(result);

            //  Solution in XQuery:   
            //  <bib>
            //    {
            //    for $b in doc(""http://bstore1.example.com/bib.xml"")/bib/book
            //    where $b/publisher = ""Addison-Wesley"" and $b/@year > 1991
            //    return
            //    <book year=""{ $b/@year }"">
            //        { $b/title }
            //    </book>
            //    }    
            // </bib>
        }

        [Category("XMP - Experiences and Exemplars")]
        [Title("Q2")]
        [Description(@"Create a flat list of all the title-author pairs, with each pair enclosed in a ""result"" element.")]
        public void Q2()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var result = new XElement("results",
                             from b in bib.Element("bib").Elements("book")
                             from t in b.Elements("title")
                             from a in b.Elements("author")
                             select new XElement("result", t, a));
            
            Console.WriteLine(result);

            // Solution in XQuery:   
            //  <results>
            //  {
            //    for $b in doc(""http://bstore1.example.com/bib.xml"")/bib/book,
            //        $t in $b/title,
            //        $a in $b/author
            //    return
            //        <result>
            //            { $t }    
            //            { $a }
            //        </result>
            //  }
            // </results>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q3")]
        [Description(@"For each book in the bibliography, list the title and authors, grouped inside a ""result"" element.")]
        public void Q3()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var result = new XElement("results",
                             from b in bib.Element("bib")
                                          .Elements("book")
                             select new XElement("result",
                                        b.Elements("title"),
                                        b.Elements("author")));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <results>
            // {
            //    for $b in doc(""http://bstore1.example.com/bib.xml"")/bib/book
            //    return
            //        <result>
            //            { $b/title }
            //            { $b/author  }
            //        </result>
            // }
            // </results>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q4")]
        [Description(@"For each author in the bibliography, list the author's name and the titles of all books by that author, grouped inside a ""result"" element.")]
        public void Q4()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var result = new XElement("results",
                             from b in bib.Element("bib")
                                          .Elements("book")
                             from a in b.Elements("author")
                             group b by new{last = (string)a.Element("last"),first = (string)a.Element("first")} into g  
                             orderby g.Key.last, g.Key.first
                             select new XElement("result",
                                        new XElement("author", 
                                            new XElement("last",g.Key.last), 
                                            new XElement("first",g.Key.first)),
                                        from b in g select b.Element("title")));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <results>
            // {
            //    let $a := doc(""http://bstore1.example.com/bib/bib.xml"")//author
            //    for $last in distinct-values($a/last),
            //        $first in distinct-values($a[last=$last]/first)
            //    order by $last, $first
            //    return
            //        <result>
            //            <author>
            //               <last>{ $last }</last>
            //               <first>{ $first }</first>
            //            </author>
            //            {
            //                for $b in doc(""http://bstore1.example.com/bib.xml"")/bib/book
            //                where some $ba in $b/author 
            //                      satisfies ($ba/last = $last and $ba/first=$first)
            //                return $b/title
            //            }
            //        </result>
            // }
            // </results>
        }

        [Category("XMP - Experiences and Exemplars")]
        [Title("Q5")]
        [Description(@"For each book found at both bstore1.example.com and bstore2.example.com, list the title of the book and its price from each source.")]
        public void Q5()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            XDocument reviews = XDocument.Load(dataPath + "reviews.xml");
            
            var result = new XElement("books-with-prices",
                             from b in bib.Descendants("book")
                             join a in reviews.Descendants("entry") 
                                    on b.Element("title").Value equals 
                                       a.Element("title").Value
                             select new XElement("book-with-prices",
                                        b.Element("title"),
                                        new XElement("price-bstore2", 
                                            a.Element("price").Value),
                                        new XElement("price-bstore1", 
                                            b.Element("price").Value)));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <books-with-prices>
            // {
            //    for $b in doc(""http://bstore1.example.com/bib.xml"")//book,
            //        $a in doc(""http://bstore2.example.com/reviews.xml"")//entry
            //    where $b/title = $a/title
            //    return
            //        <book-with-prices>
            //            { $b/title }
            //            <price-bstore2>{ $a/price/text() }</price-bstore2>
            //            <price-bstore1>{ $b/price/text() }</price-bstore1>
            //        </book-with-prices>
            // }
            // </books-with-prices>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q6")]
        [Description(@"For each book that has at least one author, list the title and first two authors, and an empty ""et-al"" element if the book has additional authors.")]
        public void Q6()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var result = new XElement("bib",
                             from b in bib.Descendants("book")
                             where b.Elements("author")
                                    .Any()
                             select new XElement("book",
                                        b.Element("title"),
                                        b.Elements("author").Take(2),
                                        b.Elements("author").Count() > 2 ? new XElement("et-al") : null));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <bib>
            // {
            //  for $b in doc(""http://bstore1.example.com/bib.xml"")//book
            //  where count($b/author) > 0
            //  return
            //      <book>
            //          { $b/title }
            //          {
            //              for $a in $b/author[position()<=2]  
            //              return $a
            //          }
            //          {
            //              if (count($b/author) > 2)
            //               then <et-al/>
            //               else ()
            //          }
            //      </book>
            // }
            // </bib>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q7")]
        [Description(@"List the titles and years of all books published by Addison-Wesley after 1991, in alphabetic order.")]
        public void Q7()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var result = new XElement("bib",
                             from b in bib.Descendants("book")
                             where (string)b.Element("publisher") == "Addison-Wesley" && 
                                   (int)b.Attribute("year") > 1991
                             orderby (string)b.Element("title")
                             select new XElement("book",
                                        b.Attribute("year"),
                                        b.Element("title")));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <bib>
            // {
            //    for $b in doc(""http://bstore1.example.com/bib.xml"")//book
            //    where $b/publisher = ""Addison-Wesley"" and $b/@year > 1991
            //    order by $b/title
            //    return
            //        <book>
            //            { $b/@year }
            //            { $b/title }
            //        </book>
            // }
            // </bib>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q8")]
        [Description(@"Find books in which the name of some element ends with the string ""or"" and the same element contains the string ""Suciu"" somewhere in its content. For each such book, return the title and the qualifying element.")]
        public void Q8()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var result = new XElement("results",
                             from b in bib.Descendants("book")
                             let e = from x in b.Elements()
                                      where x.Value.Contains("Suciu") && 
                                            x.Name.LocalName.EndsWith("or")
                                      select x
                             where e.Any()
                             select new XElement("book", 
                                        b.Element("title"), 
                                        e));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // for $b in doc(""http://bstore1.example.com/bib.xml"")//book
            // let $e := $b/*[contains(string(.), ""Suciu"") 
            //               and ends-with(local-name(.), ""or"")]
            //    where exists($e)
            //    return
            //        <book>
            //            { $b/title }
            //            { $e }
            //        </book>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q9")]
        [Description(@"In the document ""books.xml"", find all section or chapter titles that contain the word ""XML"", regardless of the level of nesting.")]
        public void Q9()
        {
            XDocument book = XDocument.Load(dataPath + "books.xml");
            
            var ChaptersAndSections = from b in book.Descendants()
                                      where b.Name == "chapter" || 
                                            b.Name == "section"
                                      select b;
    
            var result = new XElement("results",
                             from t in ChaptersAndSections.Elements("title")
                             where t.Value.Contains("XML")
                             select t);
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <results>
            //  {
            //    for $t in doc(""books.xml"")//(chapter | section)/title
            //    where contains($t/text(), ""XML"")
            //    return $t
            //  }
            // </results>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q10")]
        [Description(@"In the document ""prices.xml"", find the minimum price for each book, in the form of a ""minprice"" element with the book title as its title attribute.")]
        public void Q10()
        {
            XDocument prices = XDocument.Load(dataPath + "prices.xml");
            
            var result = new XElement("results",
                             from b in prices.Descendants("book")
                             group b.Element("price") by (string)b.Element("title") into g 
                             select new XElement("minprice",
                                        new XAttribute("title", g.Key),
                                        new XElement("price", g.Min(x => (double)x))));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <results>
            // {
            //    let $doc := doc(""prices.xml"")
            //    for $t in distinct-values($doc//book/title)
            //    let $p := $doc//book[title = $t]/price
            //    return
            //      <minprice title=""{ $t }"">
            //        <price>{ min($p) }</price>
            //      </minprice>
            // }
            // </results>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q11")]
        [Description(@"For each book with an author, return the book with its title and authors. For each book with an editor, return a reference with the book title and the editor's affiliation.")]
        public void Q11()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var result = new XElement("bib",
                             from b in bib.Descendants("book")
                             where b.Elements("author").Any()
                             select new XElement("book",
                                        b.Elements("title"),
                                        b.Elements("author")),
                             
                             from b in bib.Descendants("book")
                             where b.Elements("editor").Any()
                             select new XElement("reference",
                                        b.Elements("title"),
                                        b.Elements("editor")
                                         .Elements("affiliation")));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <bib>
            // {
            //        for $b in doc(""http://bstore1.example.com/bib.xml"")//book[author]
            //        return
            //            <book>
            //                { $b/title }
            //                { $b/author }
            //            </book>
            // }
            // {
            //        for $b in doc(""http://bstore1.example.com/bib.xml"")//book[editor]
            //        return
            //          <reference>
            //            { $b/title }
            //            {$b/editor/affiliation}
            //          </reference>
            // }
            // </bib>
        }


        [Category("XMP - Experiences and Exemplars")]
        [Title("Q12")]
        [Description(@"Find pairs of books that have different titles but the same set of authors (possibly in a different order).")]
        public void Q12()
        {
            XDocument bib = XDocument.Load(dataPath + "bib.xml");
            
            var books = bib.Descendants("book")
                           .Select((book, index) =>
                                new {
                                    title = book.Element("title"),
                                    index,
                                    authors = from a in book.Elements("author")
                                              orderby a.Element("last").Value,  a.Element("first").Value 
                                              select a
                                } );
            
            var result = new XElement("bib", 
                             from b1 in books
                             from b2 in books
                             where b1.index < b2.index &&
                                   b1.title != b2.title &&
                                   b1.authors.SequenceEqual(b2.authors)
                             select new XElement("book-pair", 
                                        b1.title,
                                        b2.title));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <bib>
            // {
            //    for $book1 in doc(""http://bstore1.example.com/bib.xml"")//book,
            //        $book2 in doc(""http://bstore1.example.com/bib.xml"")//book
            //    let $aut1 := for $a in $book1/author 
            //                 order by $a/last, $a/first
            //                 return $a
            //    let $aut2 := for $a in $book2/author 
            //                 order by $a/last, $a/first
            //                 return $a
            //    where $book1 << $book2
            //    and not($book1/title = $book2/title)
            //    and deep-equal($aut1, $aut2) 
            //    return
            //        <book-pair>
            //            { $book1/title }
            //            { $book2/title }
            //        </book-pair>
            // }
            // </bib>
        }
            

        // 1.2 Use Case "TREE": Queries that preserve hierarchy

        [Category("TREE - Queries that preserve hierarchy")]
        [Title("Q1")]
        [LinkedMethod("TableOfContents")]
        [Description(@"Prepare a (nested) table of contents for Book1, listing all the sections and their titles. Preserve the original attributes of each <section> element, if any.")]
        public void Q13()
        {
            XDocument book = XDocument.Load(dataPath + "book.xml");

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
        [Title("Q2")]
        [Description(@"Prepare a (flat) figure list for Book1, listing all the figures and their titles. Preserve the original attributes of each <figure> element, if any.")]
        public void Q14()
        {
            XDocument book = XDocument.Load(dataPath + "book.xml");
    
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
        [Title("Q3")]
        [Description(@"How many sections are in Book1, and how many figures?")]
        public void Q15()
        {
            XDocument book = XDocument.Load(dataPath + "book.xml");

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
        [Title("Q4")]
        [Description(@"How many top-level sections are in Book1?")]
        public void Q16()
        {
            XDocument book = XDocument.Load(dataPath + "book.xml");

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
        [Title("Q5")]
        [Description(@"Make a flat list of the section elements in Book1. In place of its original attributes, each section element should have two attributes, containing the title of the section and the number of figures immediately contained in the section.")]
        public void Q17()
        {
            XDocument book = XDocument.Load(dataPath + "book.xml");
            
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
        [Title("Q6")]
        [LinkedMethod("SectionSummary")]
        [Description(@"Make a nested list of the section elements in Book1, preserving their original attributes and hierarchy. Inside each section element, include the title of the section and an element that includes the number of figures immediately contained in the section.")]
        public void Q18()
        {
            XDocument book = XDocument.Load(dataPath + "book.xml");

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



        // 1.3 Use Case "SEQ" - Queries based on Sequence

        [Category("SEQ - Queries based on Sequence")]
        [Title("Q1")]
        [Description(@"In the Procedure section of Report1, what Instruments were used in the second Incision?")]
        public void Q19()
        {
            XDocument report = XDocument.Load(dataPath + "report1.xml");
            
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
        [Title("Q2")]
        [Description(@"In the Procedure section of Report1, what are the first two Instruments to be used?")]
        public void Q20()
        {
            XDocument report = XDocument.Load(dataPath + "report1.xml");
            
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
        [Title("Q3")]
        [Description(@"In Report1, what Instruments were used in the first two Actions after the second Incision?")]
        public void Q21()
        {
            XDocument report = XDocument.Load(dataPath + "report1.xml");
            
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
        [Title("Q4")]
        [Description(@"In Report1, find ""Procedure"" sections where no Anesthesia element occurs before the first Incision")]
        public void Q22()
        {
            XDocument report = XDocument.Load(dataPath + "report1.xml");
            
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
        [Title("Q5")]
        [Description(@"In Report1, what happened between the first Incision and the second Incision?")]
        public void Q23()
        {

            XDocument report = XDocument.Load(dataPath + "report1.xml");
            
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

        // 1.4 Use Case "R" - Access to Relational Data

        [Category("R - Access to Relational Data")]
        [Title("Q1")]
        [Description(@"List the item number and description of all bicycles that currently have an auction in progress, ordered by item number.")]
        public void Q24()
        {
            XDocument items = XDocument.Load(dataPath + "items.xml");
            
            var result = new XElement("result",
                             from i in items.Descendants("item_tuple")
                             where (DateTime)i.Element("start_date") <= new DateTime(1999, 1, 31) &&
                                   (DateTime)i.Element("end_date") >= new DateTime(1999, 1, 31) &&
                                   i.Element("description")
                                    .Value
                                    .Contains("Bicycle")
                             orderby (int)i.Element("itemno")
                             select new XElement("item_tuple",
                                        i.Element("itemno"),
                                        i.Element("description")));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $i in doc(""items.xml"")//item_tuple
            //    where $i/start_date <= current-date()
            //    and $i/end_date >= current-date() 
            //    and contains($i/description, ""Bicycle"")
            //    order by $i/itemno
            //    return
            //        <item_tuple>
            //            { $i/itemno }
            //            { $i/description }
            //        </item_tuple>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q2")]
        [Description(@"For all bicycles, list the item number, description, and highest bid (if any), ordered by item number.")]
        public void Q25()
        {
            XDocument items = XDocument.Load(dataPath + "items.xml");
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            
            var result = new XElement("result",
                             from i in items.Descendants("item_tuple")
                             let b = from e in bids.Descendants("bid_tuple")
                                     where (int)e.Element("itemno") == (int)i.Element("itemno")
                                     select e
                             
                             where i.Element("description")
                                    .Value
                                    .Contains("Bicycle")
                             
                             orderby (int)i.Element("itemno")
                             
                             select new XElement("item_tuple",
                                        i.Element("itemno"),
                                        i.Element("description"),
                                        new XElement("high_bid", 
                                            b.Max(x => (double?)x.Element("bid"))))); 
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $i in doc(""items.xml"")//item_tuple
            //    let $b := doc(""bids.xml"")//bid_tuple[itemno = $i/itemno]
            //    where contains($i/description, ""Bicycle"")
            //    order by $i/itemno
            //    return
            //        <item_tuple>
            //            { $i/itemno }
            //            { $i/description }
            //            <high_bid>{ max($b/bid) }</high_bid>
            //        </item_tuple>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q3")]
        [Description(@"Find cases where a user with a rating worse (alphabetically, greater) than ""C"" is offering an item with a reserve price of more than 1000.")]
        public void Q26()
        {
            XDocument users = XDocument.Load(dataPath + "users.xml");
            XDocument items = XDocument.Load(dataPath + "items.xml");
            
            var result = new XElement("result",
                             from u in users.Descendants("user_tuple")
                             join i in items.Descendants("item_tuple") 
                                    on (string)u.Element("userid") equals                                        
                                       (string)i.Element("offered_by")
                             where String.Compare((string)u.Element("rating"), "C") > 0 &&
                                   (int)i.Element("reserve_price") > 1000
                             
                             orderby (int)i.Element("itemno")
                             
                             select new XElement("warning",
                                        u.Element("name"),
                                        u.Element("rating"),
                                        i.Element("description"),
                                        i.Element("reserve_price")));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $u in doc(""users.xml"")//user_tuple
            //    for $i in doc(""items.xml"")//item_tuple
            //    where $u/rating > ""C"" 
            //    and $i/reserve_price > 1000 
            //    and $i/offered_by = $u/userid
            //    return
            //        <warning>
            //            { $u/name }
            //            { $u/rating }
            //            { $i/description }
            //            { $i/reserve_price }
            //        </warning>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q4")]
        [Description(@"List item numbers and descriptions of items that have no bids.")]
        public void Q27()
        {
            XDocument items = XDocument.Load(dataPath + "items.xml");
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            
            var result = new XElement("result",
                             from i in items.Descendants("item_tuple")
                             join b in bids.Descendants("bid_tuple") 
                                    on (int)i.Element("itemno") equals 
                                       (int)b.Element("itemno")
                                    into itemBids
                             where !itemBids.Any()
                             select new XElement("no_bid_item",
                                        i.Element("itemno"),
                                        i.Element("description")));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $i in doc(""items.xml"")//item_tuple
            //    where empty(doc(""bids.xml"")//bid_tuple[itemno = $i/itemno])
            //    return
            //        <no_bid_item>
            //            { $i/itemno }
            //            { $i/description }
            //        </no_bid_item>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q5")]
        [Description(@"For bicycle(s) offered by Tom Jones that have received a bid, list the item number, description, highest bid, and name of the highest bidder, ordered by item number.")]
        public void Q28()
        {
            XDocument users = XDocument.Load(dataPath + "users.xml");
            XDocument items = XDocument.Load(dataPath + "items.xml");
            XDocument bids = XDocument.Load(dataPath + "bids.xml");

            var jonesBikes =                      
                     from seller in users.Descendants("user_tuple")
                     join item in items.Descendants("item_tuple") 
                               on (string)seller.Element("userid") equals 
                                  (string)item.Element("offered_by")
                     where (string)seller.Element("name") == "Tom Jones" &&
                           item.Element("description").Value.Contains("Bicycle")
                     select item;

            var bikeTuple =
                     from jonesBike in jonesBikes
                     join bidtuple in bids.Descendants("bid_tuple") 
                                   on (string)jonesBike.Element("itemno") equals 
                                      (string)bidtuple.Element("itemno") 
                                   into allBids
                     where allBids.Any()
                     let highBid = (from b in allBids
                                   where (double)b.Element("bid") == 
                                         allBids.Max(e=>(double)e.Element("bid"))
                                   select b).First()
                     let highBidder = (from buyer in users.Descendants("user_tuple") 
                                       where (string)highBid.Element("userid") ==
                                       (string)buyer.Element("userid")
                                       select buyer).First()
                     orderby (string)jonesBike.Element("itemno")
                     select new {jonesBike, highBid, highBidder};
            
            var result = 
                new XElement("result",
                             from bb in bikeTuple
                             select new XElement("jones_bike",
                                        bb.jonesBike.Element("itemno"),
                                        bb.jonesBike.Element("description"),
                                        new XElement("high_bid",
                                            bb.highBid.Element("bid")),
                                        new XElement("high_bidder",
                                            bb.highBidder.Element("name"))));
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $seller in doc(""users.xml"")//user_tuple,
            //        $buyer in  doc(""users.xml"")//user_tuple,
            //        $item in  doc(""items.xml"")//item_tuple,
            //        $highbid in  doc(""bids.xml"")//bid_tuple
            //    where $seller/name = ""Tom Jones""
            //    and $seller/userid  = $item/offered_by
            //    and contains($item/description , ""Bicycle"")
            //    and $item/itemno  = $highbid/itemno
            //    and $highbid/userid  = $buyer/userid
            //    and $highbid/bid = max(
            //                            doc(""bids.xml"")//bid_tuple
            //                                [itemno = $item/itemno]/bid
            //                        )
            //    order by ($item/itemno)
            //    return
            //        <jones_bike>
            //            { $item/itemno }
            //            { $item/description }
            //            <high_bid>{ $highbid/bid }</high_bid>
            //            <high_bidder>{ $buyer/name }</high_bidder>
            //        </jones_bike>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q6")]
        [Description(@"For each item whose highest bid is more than twice its reserve price, list the item number, description, reserve price, and highest bid.")]
        public void Q29()
        {
            XDocument items = XDocument.Load(dataPath + "items.xml");
            XDocument bids = XDocument.Load(dataPath + "bids.xml");

            var result = new XElement("result",
                             from item in items.Descendants("item_tuple")
                             join b in bids.Descendants("bid_tuple") 
                                    on (int)item.Element("itemno") equals 
                                       (int)b.Element("itemno")
                                    into itemBids
                             let maxBid = itemBids.Max(x => (double?)x.Element("bid"))
                             where (int)item.Element("reserve_price") * 2 < maxBid 
                             select new XElement("successful_item",
                                        item.Element("itemno"),
                                        item.Element("description"),
                                        item.Element("reserve_price"),
                                        new XElement("high_bid", maxBid)));

            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $item in doc(""items.xml"")//item_tuple
            //    let $b := doc(""bids.xml"")//bid_tuple[itemno = $item/itemno]
            //    let $z := max($b/bid)
            //    where $item/reserve_price * 2 < $z
            //    return
            //        <successful_item>
            //            { $item/itemno }
            //            { $item/description }
            //            { $item/reserve_price }
            //            <high_bid>{$z }</high_bid>
            //        </successful_item>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q7")]
        [Description(@"Find the highest bid ever made for a bicycle or tricycle.")]
        public void Q30()
        {
            XDocument items = XDocument.Load(dataPath + "items.xml");
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            
            var maxBid = (from i in items.Descendants("item_tuple")
                           join b in bids.Descendants("bid_tuple")
                                  on (int)i.Element("itemno") equals 
                                     (int)b.Element("itemno")
                           where i.Element("description").Value.Contains("Bicycle") ||
                                 i.Element("description").Value.Contains("Tricycle")
                           select (double)b.Element("bid")).Max();
            var result = new XElement("high_bid",
                                      maxBid);            
            Console.WriteLine(result);

            // Solution in XQuery:
            // let $allbikes := doc(""items.xml"")//item_tuple
            //                    [contains(description, ""Bicycle"") 
            //                    or contains(description, ""Tricycle"")]
            //    let $bikebids := doc(""bids.xml"")//bid_tuple[itemno = $allbikes/itemno]
            //    return
            //        <high_bid>
            //        { 
            //            max($bikebids/bid) 
            //        }
            //        </high_bid>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q8")]
        [Description(@"How many items were actioned (auction ended) in March 1999?")]
        public void Q31()
        {
            XDocument items = XDocument.Load(dataPath + "items.xml");
            
            var item = from i in items.Descendants("item_tuple")
                       where (DateTime)i.Element("end_date") >= new DateTime(1999, 3, 01) &&
                             (DateTime)i.Element("end_date") <= new DateTime(1999, 3, 31)
                       select i;
            
            var result = new XElement("item_count",
                             item.Count());
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // let $item := doc(""items.xml"")//item_tuple
            // [end_date >= xs:date(""1999-03-01"") and end_date <= xs:date(""1999-03-31"")]
            //    return
            //        <item_count>
            //        { 
            //            count($item) 
            //        }
            //        </item_count>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q9")]
        [Description(@"List the number of items auctioned each month in 1999 for which data is available, ordered by month.")]
        public void Q32()
        {
            XDocument items = XDocument.Load(dataPath + "items.xml");
            var result = new XElement("result",
                                         from i in items.Descendants("item_tuple")
                                         where ((DateTime)i.Element("end_date")).Year == 1999
                                         group i by ((DateTime)i.Element("end_date")).Month into g
                                         orderby g.Key
                                         select new XElement("monthly_result",
                                                    new XElement("month", g.Key),
                                                    new XElement("item_count", g.Count())));            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    let $end_dates := doc(""items.xml"")//item_tuple/end_date
            //    for $m in distinct-values(for $e in $end_dates 
            //                            return month-from-date($e))
            //    let $item := doc(""items.xml"")
            //        //item_tuple[year-from-date(end_date) = 1999 
            //                    and month-from-date(end_date) = $m]
            //    order by $m
            //    return
            //        <monthly_result>
            //            <month>{ $m }</month>
            //            <item_count>{ count($item) }</item_count>
            //        </monthly_result>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q10")]
        [Description(@"For each item that has received a bid, list the item number, the highest bid, and the name of the highest bidder, ordered by item number.")]
        public void Q33()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            XDocument users = XDocument.Load(dataPath + "users.xml");
            
            var result = new XElement("result",
                             from b in bids.Descendants("bid_tuple")
                             join u in users.Descendants("user_tuple")  
                                    on (string)b.Element("userid") equals
                                       (string)u.Element("userid")     
                             group new {b,u} by (string)b.Element("itemno") into g                             
                             orderby g.Key                             
                             select 
                                    from highBid in g 
                                    where g.Max(m => (double)m.b.Element("bid")) == (double)highBid.b.Element("bid")
                                    select new XElement("high_bid",
                                                new XElement("itemno",g.Key),
                                                highBid.b.Element("bid"), 
                                                new XElement("bidder", highBid.u.Element("name").Value)));            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $highbid in doc(""bids.xml"")//bid_tuple,
            //        $user in doc(""users.xml"")//user_tuple
            //    where $user/userid = $highbid/userid 
            //    and $highbid/bid = max(doc(""bids.xml"")//bid_tuple[itemno=$highbid/itemno]/bid)
            //    order by $highbid/itemno
            //    return
            //        <high_bid>
            //            { $highbid/itemno }
            //            { $highbid/bid }
            //            <bidder>{ $user/name/text() }</bidder>
            //        </high_bid>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q11")]
        [Description(@"List the item number and description of the item(s) that received the highest bid ever recorded, and the amount of that bid.")]
        public void Q34()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            XDocument items = XDocument.Load(dataPath + "items.xml");
            
            var highbid = bids.Descendants("bid_tuple")
                              .Select(b => b.Element("bid"))
                              .Max(b => (double)b);
    
            var result = new XElement("result",
                             from item in items.Descendants("item_tuple")
                             join b in bids.Descendants("bid_tuple")
                                    on (double)item.Element("itemno") equals
                                       (double)b.Element("itemno")                             
                             where (double)b.Element("bid") == highbid 
                             
                             select new XElement("expensive_item",
                                        item.Element("itemno"),
                                        item.Element("description"),
                                        new XElement("high_bid", highbid)));            
            Console.WriteLine(result);

            // Solution in XQuery:
            // let $highbid := max(doc(""bids.xml"")//bid_tuple/bid)
            // return
            //    <result>
            //    {
            //        for $item in doc(""items.xml"")//item_tuple,
            //            $b in doc(""bids.xml"")//bid_tuple[itemno = $item/itemno]
            //        where $b/bid = $highbid
            //        return
            //            <expensive_item>
            //                { $item/itemno }
            //                { $item/description }
            //                <high_bid>{ $highbid }</high_bid>
            //            </expensive_item>
            //    }
            //    </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q12")]
        [Description(@"List the item number and description of the item(s) that received the largest number of bids, and the number of bids it (or they) received.")]
        public void Q35()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            XDocument items = XDocument.Load(dataPath + "items.xml");
            
            var bidcounts = from i in bids.Descendants("itemno").Select(it => (int)it).Distinct()
                            join b in bids.Descendants("bid_tuple") 
                                   on i equals 
                                      (int)b.Element("itemno") 
                                   into itemBids                                    
                            select new {
                                          itemno = i,
                                          nbids = itemBids.Count() 
                                       };
                         
            var maxbids = bidcounts.Max(bc => bc.nbids);
    
            var result = new XElement("result",
                             from item in items.Descendants("item_tuple")
                             join bc in bidcounts
                                     on (int)item.Element("itemno") equals
                                        bc.itemno
                             where bc.nbids == maxbids                                    
                             select new XElement("popular_item",
                                        item.Element("itemno"),
                                        item.Element("description"),
                                        new XElement("bid_count", bc.nbids)));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // declare function local:bid_summary()
            // as element()*
            // {
            //    for $i in distinct-values(doc(""bids.xml"")//itemno)
            //    let $b := doc(""bids.xml"")//bid_tuple[itemno = $i]
            //    return
            //        <bid_count>
            //            <itemno>{ $i }</itemno>
            //            <nbids>{ count($b) }</nbids>
            //        </bid_count>
            // };
            // <result>
            // {
            //    let $bid_counts := local:bid_summary(),
            //        $maxbids := max($bid_counts/nbids),
            //        $maxitemnos := $bid_counts[nbids = $maxbids]
            //    for $item in doc(""items.xml"")//item_tuple,
            //        $bc in $bid_counts
            //    where $bc/nbids =  $maxbids and $item/itemno = $bc/itemno
            //    return
            //        <popular_item>
            //            { $item/itemno }
            //            { $item/description }
            //            <bid_count>{ $bc/nbids/text() }</bid_count>
            //        </popular_item>
            // }
            // </result>

        }


        [Category("R - Access to Relational Data")]
        [Title("Q13")]
        [Description(@"For each user who has placed a bid, give the userid, name, number of bids, and average bid, in order by userid.")]
        public void Q36()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            XDocument users = XDocument.Load(dataPath + "users.xml");
            
            var result = new XElement("result",
                             from uid in bids.Descendants("userid").Select(id=>(string)id).Distinct()
                             join u in users.Descendants("user_tuple")
                                    on (string)uid equals
                                       (string)u.Element("userid")
                             join b in bids.Descendants("bid_tuple")
                                    on (string)uid equals
                                       (string)b.Element("userid")                                       
                                    into itemBids
                             orderby (string)u.Element("userid")
                             select new XElement("bidder",
                                        u.Element("userid"),
                                        u.Element("name"),
                                        new XElement("bid_count", 
                                            itemBids.Count()),
                                        new XElement("avgbid", 
                                            itemBids.Select(e => (int)e.Element("bid"))
                                             .Average())));            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $uid in distinct-values(doc(""bids.xml"")//userid),
            //        $u in doc(""users.xml"")//user_tuple[userid = $uid]
            //    let $b := doc(""bids.xml"")//bid_tuple[userid = $uid]
            //    order by $u/userid
            //    return
            //        <bidder>
            //            { $u/userid }
            //            { $u/name }
            //            <bidcount>{ count($b) }</bidcount>
            //            <avgbid>{ avg($b/bid) }</avgbid>
            //        </bidder>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q14")]
        [Description(@"List item numbers and average bids for items that have received three or more bids, in descending order by average bid.")]
        public void Q37()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
                    
            var result = new XElement("result",
                             from i in bids.Descendants("itemno").Select(id=>(string)id).Distinct()
                             join b in bids.Descendants("bid_tuple")
                                    on i equals (string)b.Element("itemno")
                                    into itemBids
                             let avgbid = itemBids.Average(b=>(double)b.Element("bid"))
                             where itemBids.Count() >= 3
                             orderby avgbid descending
                             select new XElement("popular_item",
                                        new XElement("itemno", i),
                                        new XElement("avgbid", avgbid)));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $i in distinct-values(doc(""bids.xml"")//itemno)
            //    let $b := doc(""bids.xml"")//bid_tuple[itemno = $i]
            //    let $avgbid := avg($b/bid)
            //    where count($b) >= 3
            //    order by $avgbid descending
            //    return
            //        <popular_item>
            //            <itemno>{ $i }</itemno>
            //            <avgbid>{ $avgbid }</avgbid>
            //        </popular_item>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q15")]
        [Description(@"List names of users who have placed multiple bids of at least $100 each.")]
        public void Q38()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            XDocument users = XDocument.Load(dataPath + "users.xml");
                    
            var result = new XElement("result",
                             from u in users.Descendants("user_tuple")
                             join b in bids.Descendants("bid_tuple")
                                           .Where(e=>(int)e.Element("bid") >= 100)
                                    on (string)u.Element("userid") equals 
                                       (string)b.Element("userid")                              
                                    into itemBids
                             where itemBids.Count() > 1
                             select new XElement("big_spender",
                                        u.Element("name").Value));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $u in doc(""users.xml"")//user_tuple
            //    let $b := doc(""bids.xml"")//bid_tuple[userid=$u/userid and bid>=100]
            //    where count($b) > 1
            //    return
            //        <big_spender>{ $u/name/text() }</big_spender>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q16")]
        [Description(@"List all registered users in order by userid; for each user, include the userid, name, and an indication of whether the user is active (has at least one bid on record) or inactive (has no bid on record).")]
        public void Q39()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            XDocument users = XDocument.Load(dataPath + "users.xml");
                    
            var result = new XElement("result",
                             from u in users.Descendants("user_tuple")
                             join b in bids.Descendants("bid_tuple")
                                    on (string)u.Element("userid") equals 
                                       (string)b.Element("userid")
                                    into itemBids
                             orderby (string)u.Element("userid")
                             select new XElement("user",
                                        u.Element("userid"),
                                        u.Element("name"),
                                        new XElement("status", 
                                            itemBids.Any() ? "active" : "inactive")));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $u in doc(""users.xml"")//user_tuple
            //    let $b := doc(""bids.xml"")//bid_tuple[userid = $u/userid]
            //    order by $u/userid
            //    return
            //        <user>
            //            { $u/userid }
            //            { $u/name }
            //            {
            //                if (empty($b))
            //                then <status>inactive</status>
            //                else <status>active</status>
            //            }
            //        </user>
            // }
            // </result>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q17")]
        [Description(@"List the names of users, if any, who have bid on every item.")]
        public void Q40()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            XDocument users = XDocument.Load(dataPath + "users.xml");
            XDocument items = XDocument.Load(dataPath + "items.xml");
                    
            var result = new XElement("frequent_bidder",
                             from u in users.Descendants("user_tuple")
                             join b in bids.Descendants("bid_tuple")
                                          on (string)u.Element("userid") equals 
                                             (string)b.Element("userid")
                                          into itemBids
                             let inoss = itemBids.Select(i=>(string)i.Element("itemno"))
                                                 .Distinct().OrderBy(e => e)
                             
                             where inoss.SequenceEqual(items.Descendants("itemno")
                                                       .Select(e => (string)e).OrderBy(e => e))
                             select u.Element("name"));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // <frequent_bidder>
            // {
            //    for $u in doc(""users.xml"")//user_tuple
            //    where 
            //    every $item in doc(""items.xml"")//item_tuple satisfies 
            //        some $b in doc(""bids.xml"")//bid_tuple satisfies 
            //        ($item/itemno = $b/itemno and $u/userid = $b/userid)
            //    return
            //        $u/name
            // }
            // </frequent_bidder>
        }


        [Category("R - Access to Relational Data")]
        [Title("Q18")]
        [Description(@"List all users in alphabetic order by name. For each user, include descriptions of all the items (if any) that were bid on by that user, in alphabetic order.")]
        public void Q41()
        {
            XDocument bids = XDocument.Load(dataPath + "bids.xml");
            XDocument users = XDocument.Load(dataPath + "users.xml");
            XDocument items = XDocument.Load(dataPath + "items.xml");
                    
            var result = new XElement("result", 
                             from u in users.Descendants("user_tuple")
                             join b in bids.Descendants("bid_tuple")
                                    on (string)u.Element("userid") equals
                                       (string)b.Element("userid")
                             join i in items.Descendants("item_tuple")
                                    on (string)b.Element("itemno") equals 
                                       (string)i.Element("itemno") 
                             group (string)i.Element("description") by (string)u.Element("name") into g
                             orderby g.Key
                             
                             select new XElement("user",
                                                 new XElement("name",g.Key),
                                                 g.Distinct().OrderBy(b => b)
                                                             .Select(b=> new XElement("bid_on_item",b))));
          
            Console.WriteLine(result);

            // Solution in XQuery:
            // <result>
            // {
            //    for $u in doc(""users.xml"")//user_tuple
            //    order by $u/name
            //    return
            //        <user>
            //            { $u/name }
            //            {
            //                for $b in distinct-values(doc(""bids.xml"")//bid_tuple
            //                                            [userid = $u/userid]/itemno)
            //                for $i in doc(""items.xml"")//item_tuple[itemno = $b]
            //                let $descr := $i/description/text()
            //                order by $descr
            //                return
            //                    <bid_on_item>{ $descr }</bid_on_item>
            //            }
            //        </user>
            // }
            // </result>
        }


        // 1.5 Use Case "SGML": Standard Generalized Markup Language

        [Category("SGML - Standard Generalized Markup Language")]
        [Title("Q1")]
        [Description(@"Locate all paragraphs in the report (all ""para"" elements occurring anywhere within the ""report"" element).")]
        public void Q42()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q2")]
        [Description(@"Locate all paragraph elements in an introduction (all ""para"" elements directly contained within an ""intro"" element).")]
        public void Q43()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q3")]
        [Description(@"Locate all paragraphs in the introduction of a section that is in a chapter that has no introduction (all ""para"" elements directly contained within an ""intro"" element directly contained in a ""section"" element directly contained in a ""chapter"" element. The ""chapter"" element must not directly contain an ""intro"" element).")]
        public void Q44()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q4")]
        [Description(@"Locate the second paragraph in the third section in the second chapter (the second ""para"" element occurring in the third ""section"" element occurring in the second ""chapter"" element occurring in the ""report"").")]
        public void Q45()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q5")]
        [Description(@"Locate all classified paragraphs (all ""para"" elements whose ""security"" attribute has the value ""c"").")]
        public void Q46()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q6")]
        [Description(@"List the short titles of all sections (the values of the ""shorttitle"" attributes of all ""section"" elements, expressing each short title as the value of a new element.)")]
        public void Q47()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q7")]
        [Description(@"Locate the initial letter of the initial paragraph of all introductions (the first character in the content [character content as well as element content] of the first ""para"" element contained in an ""intro"" element).")]
        public void Q48()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q8a")]
        [Description(@"Locate all sections with a title that has ""is SGML"" in it. The string may occur anywhere in the descendants of the title element, and markup boundaries are ignored.")]
        public void Q49()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q8b")]
        [Description(@"Same as (Q8a), but the string ""is SGML"" cannot be interrupted by sub-elements, and must appear in a single text node.")]
        public void Q50()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q9")]
        [Description(@"Locate all the topics referenced by a cross-reference anywhere in the report (all the ""topic"" elements whose ""topicid"" attribute value is the same as an ""xrefid"" attribute value of any ""xref"" element).")]
        public void Q51()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
        [Title("Q10")]
        [LinkedMethod("Before")]
        [Description(@"Locate the closest title preceding the cross-reference (""xref"") element whose ""xrefid"" attribute is ""top4"" (the ""title"" element that would be touched last before this ""xref"" element when touching each element in document order).")]
        public void Q52()
        {
            XDocument sgml = null;
            // XDocument load by default does not resolve external entities so passing in our
            // own reader with the correct setting.
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader r = XmlReader.Create(dataPath + "sgml.xml", rs))
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
                t.ElementAt(t.Count()-1 > 0?t.Count()-1:0)); 
    
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


        // 1.6 Use Case "STRING": String Search

        [Category("STRING - String Search")]
        [Title("Q1")]
        [Description(@"Find the titles of all news items where the string ""Foobar Corporation"" appears in the title.")]
        public void Q53()
        {
            XDocument str = XDocument.Load(dataPath + "string.xml");
                   
            var result = from s in str.Descendants("news_item")
                         where s.Element("title")
                                .Value
                                .Contains("Foobar Corporation")
                         select s.Element("title");
            
            foreach (XElement e in result)
                Console.WriteLine(e);

            // Solution in XQuery:
            // doc(""string.xml"")//news_item/title[contains(., ""Foobar Corporation"")]
        }


        [Category("STRING - String Search")]
        [Title("Q2")]
        [LinkedMethod("Partners")]
        [Description(@"Find news items where the Foobar Corporation and one or more of its partners are mentioned in the same paragraph and/or title. List each news item by its title and date.")]
        public void Q54()
        {
            XDocument str = XDocument.Load(dataPath + "string.xml");
    
            var FoobarPartners = Partners("Foobar Corporation");
           
            var result = from item in str.Descendants("news_item")
                         let paragraph = from p in item.Descendants("par")
                                         from part in FoobarPartners
                                         where p.Value.Contains("Foobar Corporation") &&
                                               FoobarPartners.Any(x => p.Value.Contains(x.Value))
                                         select p
                         
                         where paragraph.Any() ||
                               ((from i in item.Descendants("title")
                                where i.Value.Contains("Foobar Corporation")
                                select i).Any()&&
                               FoobarPartners.Any(x => item.Element("title")
                                                           .Value
                                                           .Contains(x.Value)))
                         
                         select new XElement("news_item",
                                    item.Element("title"),
                                    item.Element("date"));
            
            foreach (XElement e in result)
                Console.WriteLine(e);

            // Solution in XQuery:
            // declare function local:partners($company as xs:string) as element()*
            // {
            //    let $c := doc(""company-data.xml"")//company[name = $company]
            //    return $c//partner
            // };

            // let $foobar_partners := local:partners(""Foobar Corporation"")

            // for $item in doc(""string.xml"")//news_item
            // where
            // some $t in $item//title satisfies
            //    (contains($t/text(), ""Foobar Corporation"")
            //    and (some $partner in $foobar_partners satisfies
            //    contains($t/text(), $partner/text())))
            // or (some $par in $item//par satisfies
            // (contains(string($par), ""Foobar Corporation"")
            //    and (some $partner in $foobar_partners satisfies
            //        contains(string($par), $partner/text())))) 
            // return
            //    <news_item>
            //        { $item/title }
            //        { $item/date }
            //    </news_item>
        }


        [Category("STRING - String Search")]
        [Title("Q4")]
        [LinkedMethod("Partners")]
        [Description(@"Find news items where a company and one of its partners is mentioned in the same news item and the news item is not authored by the company itself.")]
        public void Q55()
        {
            XDocument str = XDocument.Load(dataPath + "string.xml");
            XDocument comp = XDocument.Load(dataPath + "company-data.xml");
    
            var result = from item in str.Descendants("news_item")
                         from c in comp.Descendants("company")
                         let part = Partners(c.Element("name").Value)
                         where item.Value
                                   .Contains(c.Element("name").Value) &&
                               part.Any(p => item.Value.Contains(p.Value) && 
                                             (string)item.Element("news_agent") != (string)c.Element("name"))
                         select item;
            
            foreach (XElement e in result)
                Console.WriteLine(e);

            // Solution in XQuery:
            // declare function local:partners($company as xs:string) as element()*
            // {
            //    let $c := doc(""company-data.xml"")//company[name = $company]
            //    return $c//partner
            // };

            // for $item in doc(""string.xml"")//news_item,
            //    $c in doc(""company-data.xml"")//company
            // let $partners := local:partners($c/name)
            // where contains(string($item), $c/name)
            // and (some $p in $partners satisfies
            //    contains(string($item), $p) and $item/news_agent != $c/name)
            // return
            //    $item
        }

        public static IEnumerable<XElement> Partners(string company)
        {
            XDocument comp = XDocument.Load(dataPath + "company-data.xml");
            
            var e = from c in comp.Descendants("company")
                    where (string)c.Element("name") == company
                    select c;
            
            return e.ElementAt(0)
                    .Descendants("partner");
        }


        [Category("STRING - String Search")]
        [Title("Q5")]
        [LinkedMethod("Partners")]
        [Description(@"For each news item that is relevant to the Gorilla Corporation, create an ""item summary"" element. The content of the item summary is the content of the title, date, and first paragraph of the news item, separated by periods. A news item is relevant if the name of the company is mentioned anywhere within the content of the news item.")]
        public void Q56()
        {
            XDocument str = XDocument.Load(dataPath + "string.xml");
            
            var result = from item in str.Descendants("news_item")
                         where item.Element("content")
                                   .Value
                                   .Contains("Gorilla Corporation")
                         select new XElement("item_summary",
                                    item.Element("title").Value + ". ",
                                    item.Element("date").Value + ". ",
                                    item.Descendants("par")
                                        .ElementAt(0)
                                        .Value);
            
            foreach (XElement e in result)
                Console.WriteLine(e);

            // Solution in XQuery:
            // for $item in doc(""string.xml"")//news_item
            // where contains(string($item/content), ""Gorilla Corporation"")
            // return
            //    <item_summary>
            //        { concat($item/title,"". "") }
            //        { concat($item/date,"". "") }
            //        { string(($item//par)[1]) }
            //    </item_summary>
        }


        // 1.8 Use Case "PARTS" - Recursive Parts Explosion

        [Category("PARTS - Recursive Parts Explosion")]
        [Title("Q1")]
        [LinkedMethod("oneLevel")]
        [Description(@"Convert the sample document from ""partlist"" format to ""parttree"" format. In the result document, part containment is represented by containment of one <part> element inside another. Each part that is not part of any other part should appear as a separate top-level element in the output document.")]
        public void Q57()
        {
            XDocument partlist = XDocument.Load(dataPath + "partlist.xml");
    
            var result = new XElement("parttree", 
                             from p in partlist.Descendants("part")
                             where p.Attribute("partof") == null
                             select oneLevel(p));
            
            Console.WriteLine(result);

            // Solution in XQuery:
            // declare function local:one_level($p as element()) as element()
            // {
            //    <part partid=""{ $p/@partid }""
            //          name=""{ $p/@name }"" >
            //        {
            //            for $s in doc(""partlist.xml"")//part
            //            where $s/@partof = $p/@partid
            //            return local:one_level($s)
            //        }
            //    </part>
            // };

            // <parttree>
            //  {
            //    for $p in doc(""partlist.xml"")//part[empty(@partof)]
            //    return local:one_level($p)
            //  }
            // </parttree>
        }


        public static XElement oneLevel(XElement p)
        {
            XDocument partlist = XDocument.Load(dataPath + "partlist.xml");
    
            return new XElement("part",
                       new XAttribute("partid",(string)p.Attribute("partid")),
                       new XAttribute("name",(string)p.Attribute("name")),
                       from s in partlist.Descendants("part")
                       where s.Attribute("partof") != null &&
                             s.Attribute("partof").Value == p.Attribute("partid").Value
                       select oneLevel(s));
        }
    }
}

