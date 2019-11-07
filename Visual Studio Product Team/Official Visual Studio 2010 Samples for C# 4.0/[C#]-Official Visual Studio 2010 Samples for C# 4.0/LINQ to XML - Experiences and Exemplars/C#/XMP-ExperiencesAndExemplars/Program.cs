using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace XMP_ExperiencesAndExemplars
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.Q1();   // List books published by Addison-Wesley after 1991, including their year and title.
            //samples.Q2();   // Create a flat list of all the title-author pairs, with each pair enclosed in a ""result"" element.
            //samples.Q3();   // For each book in the bibliography, list the title and authors, grouped inside a ""result"" element.
            //samples.Q4();   // For each author in the bibliography, list the author's name and the titles of all books by that author, grouped inside a ""result"" element.
            //samples.Q5();   // For each book found at both bstore1.example.com and bstore2.example.com, list the title of the book and its price from each source.
            //samples.Q6();   // For each book that has at least one author, list the title and first two authors, and an empty ""et-al"" element if the book has additional authors.
            //samples.Q7();   // List the titles and years of all books published by Addison-Wesley after 1991, in alphabetic order.
            //samples.Q8();   // Find books in which the name of some element ends with the string ""or"" and the same element contains the string ""Suciu"" somewhere in its content. For each such book, return the title and the qualifying element.
            //samples.Q9();   // In the document ""books.xml"", find all section or chapter titles that contain the word ""XML"", regardless of the level of nesting.
            //samples.Q10();  // In the document ""prices.xml"", find the minimum price for each book, in the form of a ""minprice"" element with the book title as its title attribute.
            //samples.Q11();  // For each book with an author, return the book with its title and authors. For each book with an editor, return a reference with the book title and the editor's affiliation.
            //samples.Q12();  // Find pairs of books that have different titles but the same set of authors (possibly in a different order).
        }

        private class LinqSamples
        {
            [Category("XMP - Experiences and Exemplars")]
            [Description(@"List books published by Addison-Wesley after 1991, including their year and title.")]
            public void Q1()
            {
                XDocument bib = XDocument.Load("bib.xml");

                var result = new XElement("bib",
                                 from b in bib.Element("bib").Elements("book")
                                 where (string)b.Element("publisher") == "Addison-Wesley" &&
                                       (int)b.Attribute("year") > 1991
                                 select new XElement("book",
                                            new XAttribute("year", b.Attribute("year").Value),
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
            [Description(@"Create a flat list of all the title-author pairs, with each pair enclosed in a ""result"" element.")]
            public void Q2()
            {
                XDocument bib = XDocument.Load("bib.xml");

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
            [Description(@"For each book in the bibliography, list the title and authors, grouped inside a ""result"" element.")]
            public void Q3()
            {
                XDocument bib = XDocument.Load("bib.xml");

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
            [Description(@"For each author in the bibliography, list the author's name and the titles of all books by that author, grouped inside a ""result"" element.")]
            public void Q4()
            {
                XDocument bib = XDocument.Load("bib.xml");

                var result = new XElement("results",
                                 from b in bib.Element("bib")
                                              .Elements("book")
                                 from a in b.Elements("author")
                                 group b by new { last = (string)a.Element("last"), first = (string)a.Element("first") } into g
                                 orderby g.Key.last, g.Key.first
                                 select new XElement("result",
                                            new XElement("author",
                                                new XElement("last", g.Key.last),
                                                new XElement("first", g.Key.first)),
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
            [Description(@"For each book found at both bstore1.example.com and bstore2.example.com, list the title of the book and its price from each source.")]
            public void Q5()
            {
                XDocument bib = XDocument.Load("bib.xml");
                XDocument reviews = XDocument.Load("reviews.xml");

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
            [Description(@"For each book that has at least one author, list the title and first two authors, and an empty ""et-al"" element if the book has additional authors.")]
            public void Q6()
            {
                XDocument bib = XDocument.Load("bib.xml");

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
            [Description(@"List the titles and years of all books published by Addison-Wesley after 1991, in alphabetic order.")]
            public void Q7()
            {
                XDocument bib = XDocument.Load("bib.xml");

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
            [Description(@"Find books in which the name of some element ends with the string ""or"" and the same element contains the string ""Suciu"" somewhere in its content. For each such book, return the title and the qualifying element.")]
            public void Q8()
            {
                XDocument bib = XDocument.Load("bib.xml");

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
            [Description(@"In the document ""books.xml"", find all section or chapter titles that contain the word ""XML"", regardless of the level of nesting.")]
            public void Q9()
            {
                XDocument book = XDocument.Load("books.xml");

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
            [Description(@"In the document ""prices.xml"", find the minimum price for each book, in the form of a ""minprice"" element with the book title as its title attribute.")]
            public void Q10()
            {
                XDocument prices = XDocument.Load("prices.xml");

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
            [Description(@"For each book with an author, return the book with its title and authors. For each book with an editor, return a reference with the book title and the editor's affiliation.")]
            public void Q11()
            {
                XDocument bib = XDocument.Load("bib.xml");

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
            [Description(@"Find pairs of books that have different titles but the same set of authors (possibly in a different order).")]
            public void Q12()
            {
                XDocument bib = XDocument.Load("bib.xml");

                var books = bib.Descendants("book")
                               .Select((book, index) =>
                                    new
                                    {
                                        title = book.Element("title"),
                                        index,
                                        authors = from a in book.Elements("author")
                                                  orderby a.Element("last").Value, a.Element("first").Value
                                                  select a
                                    });

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
        }
    }
}
