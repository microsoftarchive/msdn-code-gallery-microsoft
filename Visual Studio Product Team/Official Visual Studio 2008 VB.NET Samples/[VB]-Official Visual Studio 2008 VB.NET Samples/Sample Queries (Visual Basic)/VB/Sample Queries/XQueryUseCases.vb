' Copyright (c) Microsoft Corporation.  All rights reserved.

' XQuery Solutions within this file are excerpted from "XML Query Use Cases" (http://www.w3.org/TR/xquery-use-cases/)
' Copyright (c) 2007 W3C (R) (MIT, ERCIM, Keio).  All Rights Reserved.  W3C liability, trademark and document use rules apply. 
' The Status section of "XML Query Use Cases" states as follows:
' Status of this Document
' This section describes the status of this document at the time of its publication. Other documents may supersede this document. A list of current W3C publications and the latest revision of this technical report can be found in the W3C technical reports index at http://www.w3.org/TR/.
' This is the W3C Working Group Note of "XML Query (XQuery) Use Cases", produced by the W3C XML Query Working Group, part of the XML Activity. This document is being published as a Working Group Note to persistently record the Use Cases that guided the development of XQuery 1.0: An XML Query Language and its associated specifications as W3C Recommendations.
' Please submit comments about this document using W3C's public Bugzilla system (instructions can be found at http://www.w3.org/XML/2005/04/qt-bugzilla). If access to that system is not feasible, you may send your comments to the W3C XSLT/XPath/XQuery public comments mailing list, public-qt-comments@w3.org. It will be very helpful if you include the string [XQRecUseCases] in the subject line of your report, whether made in Bugzilla or in email. Each Bugzilla entry and email message should contain only one comment. Archives of the comments and responses are available at http://lists.w3.org/Archives/Public/public-qt-comments/.
' Publication as a Working Group Note does not imply endorsement by the W3C Membership. At the time of publication, work on this document was considered complete and no further revisions are anticipated. It is a stable document and may be used as reference material or cited from another document. However, this document may be updated, replaced, or made obsolete by other documents at any time.
' This document was produced by a group operating under the 5 February 2004 W3C Patent Policy. W3C maintains a public list of any patent disclosures made in connection with the deliverables of the group; that page also includes instructions for disclosing a patent. An individual who has actual knowledge of a patent which the individual believes contains Essential Claim(s) must disclose the information in accordance with section 6 of the W3C Patent Policy .

Option Infer On

Imports SampleQueries.SampleSupport
Imports System.IO
Imports System.Linq
Imports System.Xml.Linq
Imports System.Runtime.CompilerServices

<Title("Bonus: XQuery Use Cases"), _
Prefix("Q")> _
Public Class XQueryUseCases
    Inherits SampleHarness

    Private ReadOnly dataPath As String = Path.GetFullPath(System.Windows.Forms.Application.StartupPath & "\..\..\SampleData\")

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q1"), _
    Description("List books published by Addison-Wesley after 1991, including their year and title.")> _
    Public Sub Q1()
        'Solution in XQuery:    
        '<bib>
        '    {
        '    for $b in doc(""http://bstore1.example.com/bib.xml"")/bib/book
        '    where $b/publisher = ""Addison-Wesley"" and $b/@year > 1991
        '        Return
        '    <book year=""{ $b/@year }"">
        '        { $b/title }
        '    </book>
        '    }    
        '</bib>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim result = <bib>
                         <%= From book In bib.<bib>.<book> _
                             Where book.<publisher>.Value = "Addison-Wesley" AndAlso book.@year > 1991 _
                             Select _
                             <book year=<%= book.@year %>>
                                 <%= book.<title> %>
                             </book> %>
                     </bib>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q2"), _
    Description("Create a flat list of all the title-author pairs, with each pair enclosed in a ""result"" element.")> _
    Public Sub Q2()
        'Solution in XQuery:
        '<results>
        '  {
        '    for $b in doc(""http://bstore1.example.com/bib.xml"")/bib/book,
        '        $t in $b/title,
        '        $a in $b/author
        '        Return
        '        <result>
        '            { $t }    
        '            { $a }
        '        </result>
        '  }
        '</results>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim result = <results>
                         <%= From book In bib.<bib>.<book> _
                             From title In book.<title> _
                             From author In book.<author> _
                             Select _
                             <result>
                                 <%= title %>
                                 <%= author %>
                             </result> %>
                     </results>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q3"), _
    Description("For each book in the bibliography, list the title and authors, grouped inside a ""result"" element.")> _
    Public Sub Q3()
        'Solution in XQuery:
        '<results>
        '    {
        '        for $b in doc(""http://bstore1.example.com/bib.xml"")/bib/book
        '            Return
        '            <result>
        '                { $b/title }
        '                { $b/author  }
        '            </result>
        '    }
        '</results>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim result = <results>
                         <%= From book In bib.<bib>.<book> _
                             Select _
                             <result>
                                 <%= book.<title> %>
                                 <%= book.<author> %>
                             </result> %>
                     </results>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q4"), _
    Description("For each author in the bibliography, list the author's name and the titles of all books by that author, grouped inside a ""result"" element.")> _
    Public Sub Q4()
        'Solution in XQuery:
        '<results>
        '  {
        '    let $a := doc(""http://bstore1.example.com/bib/bib.xml"")//author
        '    for $last in distinct-values($a/last),
        '        $first in distinct-values($a[last=$last]/first)
        '    order by $last, $first
        '    Return
        '        <result>
        '            <author>
        '               <last>{ $last }</last>
        '               <first>{ $first }</first>
        '            </author>
        '            {
        '                for $b in doc(""http://bstore1.example.com/bib.xml"")/bib/book
        '                where some $ba in $b/author 
        '                      satisfies ($ba/last = $last and $ba/first=$first)
        '                return $b/title
        '            }
        '        </result>
        '  }
        '</results>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim result = <results>
                         <%= From book In bib.<bib>.<book> _
                             From author In book.<author> _
                             Group book By author.<last>.Value, author.<first>.Value _
                             Into Group _
                             Order By last, first _
                             Select _
                             <result>
                                 <author>
                                     <last><%= last %></last>
                                     <first><%= first %></first>
                                 </author>
                                 <%= From book In Group Select book.<title> %>
                             </result> %>
                     </results>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q5"), _
    Description("For each book found at both bstore1.example.com and bstore2.example.com, list the title of the book and its price from each source.")> _
    Public Sub Q5()
        'Solution in XQuery:
        '    <books-with-prices>
        '      {
        '        for $b in doc(""http://bstore1.example.com/bib.xml"")//book,
        '            $a in doc(""http://bstore2.example.com/reviews.xml"")//entry
        '        where $b/title = $a/title
        '            Return
        '            <book-with-prices>
        '                { $b/title }
        '                <price-bstore2>{ $a/price/text() }</price-bstore2>
        '                <price-bstore1>{ $b/price/text() }</price-bstore1>
        '            </book-with-prices>
        '      }
        '    </books-with-prices>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")
        Dim reviews = XDocument.Load(dataPath & "reviews.xml")

        Dim result = <books-with-prices>
                         <%= From book In bib...<book> _
                             Join review In reviews...<entry> _
                             On book.<title>.Value Equals review.<title>.Value _
                             Select _
                             <book-with-prices>
                                 <%= book.<title> %>
                                 <price-bstore2><%= review.<price>.Value %></price-bstore2>
                                 <price-bstore1><%= book.<price>.Value %></price-bstore1>
                             </book-with-prices> %>
                     </books-with-prices>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q6"), _
    Description("For each book that has at least one author, list the title and first two authors, and an empty ""et-al"" element if the book has additional authors.")> _
    Public Sub Q6()
        'Solution in XQuery:
        '<bib>
        '  {
        '    for $b in doc(""http://bstore1.example.com/bib.xml"")//book
        '    where count($b/author) > 0
        '        Return
        '        <book>
        '            { $b/title }
        '            {
        '                for $a in $b/author[position()<=2]  
        '                return $a
        '            }
        '            {
        '                if (count($b/author) > 2)
        '                 then <et-al/>
        '                 else ()
        '            }
        '        </book>
        '  }
        '</bib>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim result = <bib>
                         <%= From book In bib...<book> _
                             Where book.<author>.Any _
                             Select _
                             <book>
                                 <%= book.<title> %>
                                 <%= book.<author>.Take(2) %>
                                 <%= If(book.<author>.Count > 2, <et-al/>, Nothing) %>
                             </book> %>
                     </bib>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q7"), _
    Description("List the titles and years of all books published by Addison-Wesley after 1991, in alphabetic order.")> _
    Public Sub Q7()
        'Solution in XQuery:
        '<bib>
        '  {
        '    for $b in doc(""http://bstore1.example.com/bib.xml"")//book
        '    where $b/publisher = ""Addison-Wesley"" and $b/@year > 1991
        '    order by $b/title
        '        Return
        '        <book>
        '            { $b/@year }
        '            { $b/title }
        '        </book>
        '  }
        '</bib>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim result = <bib>
                         <%= From book In bib...<book> _
                             Where book.<publisher>.Value = "Addison-Wesley" AndAlso book.@year > 1991 _
                             Order By book.<title>.Value _
                             Select _
                             <book year=<%= book.@year %>>
                                 <%= book.<title> %>
                             </book> _
                         %>
                     </bib>

        Console.WriteLine(result)
    End Sub


    <Category("XMP - Experiences and Exemplars"), _
    Title("Q8"), _
    Description("Find books in which the name of some element ends with the string ""or"" and the same element contains the string ""Suciu"" somewhere in its content. For each such book, return the title and the qualifying element.")> _
    Public Sub Q8()
        'Solution in XQuery:
        'for $b in doc(""http://bstore1.example.com/bib.xml"")//book
        'let $e := $b/*[contains(string(.), ""Suciu"") 
        '               and ends-with(local-name(.), ""or"")
        'where exists($e)
        'return
        '    <book>
        '        { $b/title }
        '        { $e }
        '    </book>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim result = <results>
                         <%= From book In bib...<book> _
 _
                             Let e = _
                             (From element In book.Elements() _
                             Where element.Value.Contains("Suciu") _
                             AndAlso element.Name.LocalName.EndsWith("or") _
                             Select element) _
 _
                             Where e.Any _
                             Select _
                             <book>
                                 <%= book.<title> %>
                                 <%= e %>
                             </book> %>
                     </results>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q9"), _
    Description("In the document ""books.xml"", find all section or chapter titles that contain the word ""XML"", regardless of the level of nesting.")> _
    Public Sub Q9()
        'Solution in XQuery:
        '<results>
        '  {
        '    for $t in doc(""books.xml"")//(chapter | section)/title
        '    where contains($t/text(), ""XML"")
        '    return $t
        '  }
        '</results>

        'Solution in VB
        Dim bookDoc = XDocument.Load(dataPath & "books.xml")
        Dim chaptersAndSections = From book In bookDoc.Descendants() _
                                  Where book.Name = "chapter" OrElse book.Name = "section" _
                                  Select book

        Dim result = <results>
                         <%= From title In chaptersAndSections.<title> _
                             Where title.Value.Contains("XML") _
                             Select title %>
                     </results>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q10"), _
    Description("In the document ""prices.xml"", find the minimum price for each book, in the form of a ""minprice"" element with the book title as its title attribute.")> _
    Public Sub Q10()
        'Solution in XQuery:
        '<results>
        '  {
        '    let $doc := doc(""prices.xml"")
        '    for $t in distinct-values($doc//book/title)
        '    let $p := $doc//book[title = $t]/price
        '        Return
        '      <minprice title=""{ $t }"">
        '        <price>{ min($p) }</price>
        '      </minprice>
        '  }
        '</results>

        'Solution in VB
        Dim prices = XDocument.Load(dataPath & "prices.xml")

        Dim result = <results>
                         <%= From book In prices...<book> _
                             Group book.<price> By book.<title>.Value _
                             Into Group _
                             Select _
                             <minprice title=<%= title %>>
                                 <price>
                                     <%= Aggregate price In Group Into Min(CDbl(price.Value)) %>
                                 </price>
                             </minprice> %>
                     </results>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q11"), _
    Description("For each book with an author, return the book with its title and authors. For each book with an editor, return a reference with the book title and the editor's affiliation.")> _
    Public Sub Q11()
        'Solution in XQuery:
        '<bib>
        '{
        '        for $b in doc(""http://bstore1.example.com/bib.xml"")//book[author]
        '        Return
        '            <book>
        '                { $b/title }
        '                { $b/author }
        '            </book>
        '}
        '{
        '        for $b in doc(""http://bstore1.example.com/bib.xml"")//book[editor]
        '            Return
        '          <reference>
        '            { $b/title }
        '            {$b/editor/affiliation}
        '          </reference>
        '}
        '</bib>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim result = <bib>
                         <%= From book In bib...<book> _
                             Where book.<author>.Any _
                             Select _
                             <book>
                                 <%= book.<title> %>
                                 <%= book.<author> %>
                             </book> %>
                         <%= From book In bib...<book> _
                             Where book.<editor>.Any _
                             Select _
                             <reference>
                                 <%= book.<title> %>
                                 <%= book.<editor>.<affiliation> %>
                             </reference> %>
                     </bib>

        Console.WriteLine(result)
    End Sub

    <Category("XMP - Experiences and Exemplars"), _
    Title("Q12"), _
    Description("Find pairs of books that have different titles but the same set of authors (possibly in a different order).")> _
    Public Sub Q12()
        'Solution in XQuery:
        '<bib>
        '{
        '    for $book1 in doc(""http://bstore1.example.com/bib.xml"")//book,
        '        $book2 in doc(""http://bstore1.example.com/bib.xml"")//book
        '    let $aut1 := for $a in $book1/author 
        '                 order by $a/last, $a/first
        '                 return $a
        '    let $aut2 := for $a in $book2/author 
        '                 order by $a/last, $a/first
        '                 return $a
        '    where $book1 << $book2
        '    and not($book1/title = $book2/title)
        '    and deep-equal($aut1, $aut2) 
        '        Return
        '        <book-pair>
        '            { $book1/title }
        '            { $book2/title }
        '        </book-pair>
        '}
        '</bib>

        'Solution in VB
        Dim bib = XDocument.Load(dataPath & "bib.xml")

        Dim books = bib...<book>.Select(Function(book, index) _
                                            New With {.title = book.<title>, _
                                                      index, _
                                                      .authors = From author In book.<author> _
                                                                 Order By author.<last>.Value, author.<first>.Value _
                                                                 Select author})

        Dim result = <bib>
                         <%= From book1 In books _
                             From book2 In books _
                             Where book1.index < book2.index _
                             AndAlso book1.authors.SequenceEqual(book2.authors) _
                             AndAlso book1.title.Value <> book2.title.Value _
                             Select _
                             <book-pair>
                                 <%= book1.title %>
                                 <%= book2.title %>
                             </book-pair> %>
                     </bib>

        Console.WriteLine(result)
    End Sub

    <Category("TREE - Queries that preserve hierarchy"), _
    Title("Q1"), _
    LinkedFunction("GetTableOfContents"), _
    Description("Prepare a (nested) table of contents for Book1, listing all the sections and their titles. Preserve the original attributes of each <section> element, if any.")> _
    Public Sub Q13()
        'Solution in XQuery:
        'declare function local:toc($book-or-section as element()) as element()*
        '{
        '    for $section in $book-or-section/section
        '        Return
        '    <section>
        '        { $section/@* , $section/title , local:toc($section) }                 
        '    </section>
        '};

        '<toc>
        '{
        '    for $s in doc(""book.xml"")/book return local:toc($s)
        '}
        '</toc>

        'Solution in VB
        Dim book = XDocument.Load(dataPath & "book.xml")

        Dim result = <toc>
                         <%= GetTableOfContents(book.<book>(0)) %>
                     </toc>

        Console.WriteLine(result)
    End Sub
    Public Function GetTableOfContents(ByVal bookOrSection As XElement) As IEnumerable(Of XElement)
        Return _
            From section In bookOrSection.<section> _
            Select <section <%= section.Attributes() %>>
                       <%= section.<title> %>
                       <%= GetTableOfContents(section) %>
                   </section>
    End Function

    <Category("TREE - Queries that preserve hierarchy"), _
    Title("Q2"), _
    Description("Prepare a (flat) figure list for Book1, listing all the figures and their titles. Preserve the original attributes of each <figure> element, if any.")> _
    Public Sub Q14()
        'Solution in XQuery:
        '    <figlist>
        '    {
        '        for $f in doc(""book.xml"")//figure
        '            Return
        '            <figure>
        '                { $f/@* }
        '                { $f/title }
        '            </figure>
        '    }
        '    </figlist>

        'Solution in VB
        Dim book = XDocument.Load(dataPath & "book.xml")

        Dim result = <figlist>
                         <%= From figure In book...<figure> _
                             Select _
                             <figure <%= figure.Attributes() %>>
                                 <%= figure.<title> %>
                             </figure> %>
                     </figlist>

        Console.WriteLine(result)
    End Sub

    <Category("TREE - Queries that preserve hierarchy"), _
    Title("Q3"), _
    Description("How many sections are in Book1, and how many figures?")> _
    Public Sub Q15()
        'Solution in XQuery:
        '<section_count>{ count(doc(""book.xml"")//section) }</section_count>, 
        '<figure_count>{ count(doc(""book.xml"")//figure) }</figure_count>"

        'Solution in VB
        Dim book = XDocument.Load(dataPath & "book.xml")

        Dim result = <results>
                         <section_count><%= book...<section>.Count() %></section_count>
                         <figure_count><%= book...<figure>.Count() %></figure_count>
                     </results>

        Console.WriteLine(result)
    End Sub

    <Category("TREE - Queries that preserve hierarchy"), _
    Title("Q4"), _
    Description("How many top-level sections are in Book1?")> _
    Public Sub Q16()
        'Solution in XQuery:
        '<top_section_count>
        '{ 
        '    count(doc(""book.xml"")/book/section) 
        '}
        '</top_section_count>

        'Solution in VB
        Dim books = XDocument.Load(dataPath & "book.xml")

        Dim result = <top_section_count>
                         <%= books.<book>.<section>.Count() %>
                     </top_section_count>

        Console.WriteLine(result)
    End Sub

    <Category("TREE - Queries that preserve hierarchy"), _
    Title("Q5"), _
    Description("Make a flat list of the section elements in Book1. In place of its original attributes, each section element should have two attributes, containing the title of the section and the number of figures immediately contained in the section.")> _
    Public Sub Q17()
        'Solution in XQuery:
        '<section_list>
        '{
        '   for $s in doc(""book.xml"")//section
        '   let $f := $s/figure
        '   Return
        '      <section title=""{ $s/title/text() }"" figcount=""{ count($f) }""/>
        '<section_list>

        'Solution in VB
        Dim book = XDocument.Load(dataPath & "book.xml")

        Dim result = <section_list>
                         <%= From section In book...<section> _
                             Select _
                             <section title=<%= section.<title>.Value %> figcount=<%= section.<figure>.Count() %>/> %>
                     </section_list>

        Console.WriteLine(result)
    End Sub

    <Category("TREE - Queries that preserve hierarchy"), _
    Title("Q6"), _
    LinkedFunction("GetSectionSummary"), _
    Description("Make a nested list of the section elements in Book1, preserving their original attributes and hierarchy. Inside each section element, include the title of the section and an element that includes the number of figures immediately contained in the section")> _
    Public Sub Q18()
        'Solution in XQuery:
        'declare function local:section-summary($book-or-section as element()*)
        'as element()*
        '{
        '  for $section in $book-or-section
        '        Return
        '    <section>
        '       { $section/@* }
        '       { $section/title }       
        '       <figcount>         
        '         { count($section/figure) }
        '       </figcount>                
        '       { local:section-summary($section/section) }                      
        '    </section>
        '};

        '<toc>
        '  {
        '    for $s in doc(""book.xml"")/book/section
        '    return local:section-summary($s)
        '  }
        '</toc>

        'Solution in VB
        Dim book = XDocument.Load(dataPath & "book.xml")

        Dim result = <toc>
                         <%= GetSectionSummary(book.<book>.<section>) %>
                     </toc>

        Console.WriteLine(result)
    End Sub
    Function GetSectionSummary(ByVal booksOrSections As IEnumerable(Of XElement)) As IEnumerable(Of XElement)
        Return _
            From section In booksOrSections _
            Select <section <%= section.Attributes() %>>
                       <%= section.<title> %>
                       <figcount>
                           <%= section.<figure>.Count() %>
                       </figcount>
                       <%= GetSectionSummary(section.<section>) %>
                   </section>
    End Function

    <Category("SEQ - Queries based on Sequence"), _
    Title("Q1"), _
    Description("In the Procedure section of Report1, what Instruments were used in the second Incision?")> _
    Public Sub Q19()
        'Solution in XQuery:
        'for $s in doc(""report1.xml"")//section[section.title = ""Procedure""]
        'return ($s//incision)[2]/instrument

        'Solution in VB
        Dim report = XDocument.Load(dataPath & "report1.xml")

        Dim result = From section In report...<section> _
                     Where section.<section.title>.Value = "Procedure" _
                     From incision In section...<incision>(1).<instrument> _
                     Select incision

        For Each e In result
            Console.WriteLine(e)
        Next
    End Sub

    <Category("SEQ - Queries based on Sequence"), _
    Title("Q2"), _
    Description("In the Procedure section of Report1, what are the first two Instruments to be used?")> _
    Public Sub Q20()
        'Solution in XQuery:
        'for $s in doc(""report1.xml"")//section[section.title = ""Procedure""]
        'return ($s//instrument)[position()<=2]

        'Solution in VB
        Dim report = XDocument.Load(dataPath & "report1.xml")

        Dim result = From section In report...<section> _
                     Where section.<section.title>.Value = "Procedure" _
                     From instrument In section...<instrument> _
                     Take 2 _
                     Select instrument

        For Each e In result
            Console.WriteLine(e)
        Next
    End Sub

    <Category("SEQ - Queries based on Sequence"), _
    Title("Q3"), _
    Description("In Report1, what Instruments were used in the first two Actions after the second Incision?")> _
    Public Sub Q21()
        'Solution in XQuery:
        'let $i2 := (doc(""report1.xml"")//incision)[2]
        'for $a in (doc(""report1.xml"")//action)[. >> $i2][position()<=2]
        'return $a//instrument

        'Solution in VB
        Dim report = XDocument.Load(dataPath & "report1.xml")

        Dim i2 = report...<incision>(1)

        Dim result = From action In report.Descendants _
                     Skip While Not action.Equals(i2) _
                     Where action.Name = "action" _
                     Take 2 _
                     From instrument In action...<instrument> _
                     Select instrument

        For Each instrument In result
            Console.WriteLine(instrument)
        Next
    End Sub

    <Category("SEQ - Queries based on Sequence"), _
    Title("Q4"), _
    Description("In Report1, find ""Procedure"" sections where no Anesthesia element occurs before the first Incision")> _
    Public Sub Q22()
        'Solution in XQuery:
        'for $p in doc(""report1.xml"")//section[section.title = ""Procedure""]
        'where not(some $a in $p//anesthesia satisfies
        '        $a << ($p//incision)[1] )
        'return $p

        'Solution in VB
        Dim report = XDocument.Load(dataPath & "report1.xml")

        Dim result = From procedure In report...<section> _
                     Let e = (From incision In procedure.Descendants() _
                              Take While incision.Name <> "incision") _
                     Where procedure.<section.title>.Value = "Procedure" _
                           AndAlso Not e...<anesthesia>.Any _
                     Select procedure

        For Each e In result
            Console.WriteLine(e)
        Next
    End Sub

    <Category("SEQ - Queries based on Sequence"), _
    Title("Q5"), _
    Description("In Report1, what happened between the first Incision and the second Incision?")> _
    Public Sub Q23()
        'Solution in XQuery:
        '<critical_sequence>
        ' {
        '  let $proc := doc("report1.xml")//section[section.title="Procedure"][1],
        '      $i1 :=  ($proc//incision)[1],
        '      $i2 :=  ($proc//incision)[2]
        '  for $n in $proc//node() except $i1//node()
        '  where $n >> $i1 and $n << $i2
        '  return $n 
        ' }
        '</critical_sequence>

        'Solution in VB
        Dim report = XDocument.Load(dataPath & "report1.xml")
        Dim proc = (From procedure In report...<section> _
                    Where procedure.<section.title>.Value = "Procedure" _
                    Select procedure).First

        Dim i1 = proc...<incision>(0)
        Dim i2 = proc...<incision>(1)

        Dim result = <critical_sequence>
                         <%= proc.DescendantNodes() _
                             .Except(i1.DescendantNodes) _
                             .SkipWhile(Function(n) n IsNot i1) _
                             .Skip(1) _
                             .TakeWhile(Function(n) n IsNot i2) %>
                     </critical_sequence>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q1"), _
    Description("List the item number and description of all bicycles that currently have an auction in progress, ordered by item number.")> _
    Public Sub Q24()
        'Solution in XQuery:
        '<result>
        '{
        '    for $i in doc(""items.xml"")//item_tuple
        '    where $i/start_date <= current-date()
        '    and $i/end_date >= current-date() 
        '    and contains($i/description, ""Bicycle"")
        '    order by $i/itemno
        '        Return
        '        <item_tuple>
        '            { $i/itemno }
        '            { $i/description }
        '        </item_tuple>
        '}
        '</result>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")

        Dim result = <result>
                         <%= From item In items...<item_tuple> _
                             Where item.<start_date>.Value <= #1/31/1999# _
                             AndAlso item.<end_date>.Value >= #1/31/1999# _
                             AndAlso item.<description>.Value.Contains("Bicycle") _
                             Order By item.<itemno>.Value _
                             Select _
                             <item_tuple>
                                 <%= item.<itemno> %>
                                 <%= item.<description> %>
                             </item_tuple> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q2"), _
    LinkedClass("MyExtensions"), _
    Description("For all bicycles, list the item number, description, and highest bid (if any), ordered by item number.")> _
    Public Sub Q25()
        'Solution in XQuery:
        '<result>
        '{
        '    for $i in doc(""items.xml"")//item_tuple
        '    let $b := doc(""bids.xml"")//bid_tuple[itemno = $i/itemno]
        '    where contains($i/description, ""Bicycle"")
        '    order by $i/itemno
        '        Return
        '        <item_tuple>
        '            { $i/itemno }
        '            { $i/description }
        '            <high_bid>{ max($b/bid) }</high_bid>
        '        </item_tuple>
        '}
        '</result>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")
        Dim bids = XDocument.Load(dataPath & "bids.xml")

        Dim result = <result>
                         <%= From item In items...<item_tuple> _
 _
                             Let bid = _
                             (From e In bids...<bid_tuple> _
                             Where e.<itemno>.Value = item.<itemno>.Value() _
                             Select e) _
 _
                             Where item.<description>.Value.Contains("Bicycle") _
                             Order By item.<itemno>.Value _
                             Select _
                             <item_tuple>
                                 <%= item.<itemno> %>
                                 <%= item.<description> %>
                                 <high_bid>
                                     <%= Aggregate e In bid.<bid> Into Max(CType(e.Value, Double?)) %>
                                 </high_bid>
                             </item_tuple> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q3"), _
    Description("Find cases where a user with a rating worse (alphabetically, greater) than ""C"" is offering an item with a reserve price of more than 1000.")> _
    Public Sub Q26()
        'Solution in XQuery:
        '<result>
        '{
        '    for $u in doc("users.xml")//user_tuple
        '    for $i in doc("items.xml")//item_tuple
        '    where $u/rating > "C" 
        '    and $i/reserve_price > 1000 
        '    and $i/offered_by = $u/userid
        '            Return
        '        <warning>
        '            { $u/name }
        '            { $u/rating }
        '            { $i/description }
        '            { $i/reserve_price }
        '        </warning>
        '}
        '</result>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")
        Dim users = XDocument.Load(dataPath & "users.xml")

        Dim result = <result>
                         <%= From user In users...<user_tuple> _
                             Join item In items...<item_tuple> _
                             On user.<userid>.Value Equals item.<offered_by>.Value _
                             Where String.Compare(user.<rating>.Value, "C") > 0 _
                             AndAlso item.<reserve_price>.Value > 1000 _
                             Order By item.<itemno>.Value _
                             Select _
                             <warning>
                                 <%= user.<name> %>
                                 <%= user.<rating> %>
                                 <%= item.<description> %>
                                 <%= item.<reserve_price> %>
                             </warning> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q4"), _
    Description("List item numbers and descriptions of items that have no bids.")> _
    Public Sub Q27()
        'Solution in XQuery:
        '<result>
        '{
        '    for $i in doc(""items.xml"")//item_tuple
        '    where empty(doc(""bids.xml"")//bid_tuple[itemno = $i/itemno])
        '        Return
        '        <no_bid_item>
        '            { $i/itemno }
        '            { $i/description }
        '        </no_bid_item>
        '}
        '</result>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")
        Dim bids = XDocument.Load(dataPath & "bids.xml")

        Dim result = <result>
                         <%= From item In items...<item_tuple> _
                             Group Join bid In bids...<bid_tuple> _
                             On item.<itemno>.Value Equals bid.<itemno>.Value _
                             Into itemBids = Group _
                             Where Not itemBids.Any _
                             Select _
                             <no_bid_item>
                                 <%= item.<itemno> %>
                                 <%= item.<description> %>
                             </no_bid_item> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q5"), _
    Description("For bicycle(s) offered by Tom Jones that have received a bid, list the item number, description, highest bid, and name of the highest bidder, ordered by item number.")> _
    Public Sub Q28()
        'Solution in XQuery:
        '<result>
        '{
        '    for $seller in doc(""users.xml"")//user_tuple,
        '        $buyer in  doc(""users.xml"")//user_tuple,
        '        $item in  doc(""items.xml"")//item_tuple,
        '        $highbid in  doc(""bids.xml"")//bid_tuple
        '    where $seller/name = ""Tom Jones""
        '    and $seller/userid  = $item/offered_by
        '    and contains($item/description , ""Bicycle"")
        '    and $item/itemno  = $highbid/itemno
        '    and $highbid/userid  = $buyer/userid
        '    and $highbid/bid = max(
        '                            doc(""bids.xml"")//bid_tuple
        '                                [itemno = $item/itemno]/bid
        '                        )
        '    order by ($item/itemno)
        '        Return
        '        <jones_bike>
        '            { $item/itemno }
        '            { $item/description }
        '            <high_bid>{ $highbid/bid }</high_bid>
        '            <high_bidder>{ $buyer/name }</high_bidder>
        '        </jones_bike>
        '}
        '</result>

        'Solution in VB
        Dim users = XDocument.Load(dataPath & "users.xml")
        Dim items = XDocument.Load(dataPath & "items.xml")
        Dim bids = XDocument.Load(dataPath & "bids.xml")

        Dim jonesBikes = From seller In users...<user_tuple> _
                         Join item In items...<item_tuple> _
                         On seller.<userid>.Value Equals item.<offered_by>.Value _
                         Where seller.<name>.Value = "Tom Jones" _
                         AndAlso item.<description>.Value.Contains("Bicycle") _
                         Select item

        Dim bikeTuple = From jonesBike In jonesBikes _
                        Group Join bidTuple In bids...<bid_tuple> _
                        On jonesBike.<itemno>.Value Equals bidTuple.<itemno>.Value _
                        Into allBids = Group _
                        Where allBids.Any _
                        Let highBid = (From b In allBids _
                                       Where b.<bid>.Value = (Aggregate e In allBids.<bid> Into Max(CDbl(e.Value))) _
                                       Select b).First _
                        Let highBidder = (From buyer In users...<user_tuple> _
                                          Where highBid.<userid>.Value = buyer.<userid>.Value _
                                          Select buyer).First _
                        Order By jonesBike.<itemno>.Value _
                        Select jonesBike, highBid, highBidder

        Dim result = <result>
                         <%= From bb In bikeTuple _
                             Select _
                             <jones_bike>
                                 <%= bb.jonesBike.<itemno> %>
                                 <%= bb.jonesBike.<description> %>
                                 <high_bid><%= bb.highBid.<bid> %></high_bid>
                                 <high_bidder><%= bb.highBidder.<name> %></high_bidder>
                             </jones_bike> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q6"), _
    LinkedClass("MyExtensions"), _
    Description("For each item whose highest bid is more than twice its reserve price, list the item number, description, reserve price, and highest bid.")> _
    Public Sub Q29()
        'Solution in XQuery:
        '<result>
        '{
        '    for $item in doc(""items.xml"")//item_tuple
        '    let $b := doc(""bids.xml"")//bid_tuple[itemno = $item/itemno]
        '    let $z := max($b/bid)
        '    where $item/reserve_price * 2 < $z
        '        Return
        '        <successful_item>
        '            { $item/itemno }
        '            { $item/description }
        '            { $item/reserve_price }
        '            <high_bid>{$z }</high_bid>
        '        </successful_item>
        '}
        '</result>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")
        Dim bids = XDocument.Load(dataPath & "bids.xml")

        Dim result = <result>
                         <%= From item In items...<item_tuple> _
                             Group Join bid In bids...<bid_tuple> _
                             On item.<itemno>.Value Equals bid.<itemno>.Value _
                             Into itemBids = Group _
                             Let maxBid = Aggregate e In itemBids.<bid> Into Max(CType(e.Value, Double?)) _
                             Where item.<reserve_price>.Value * 2 < maxBid _
                             Select _
                             <successful_item>
                                 <%= item.<itemno> %>
                                 <%= item.<description> %>
                                 <%= item.<reserve_price> %>
                                 <high_bid><%= maxBid %></high_bid>
                             </successful_item> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q7"), _
    LinkedClass("MyExtensions"), _
    Description("Find the highest bid ever made for a bicycle or tricycle.")> _
    Public Sub Q30()
        'Solution in XQuery:
        'let $allbikes := doc(""items.xml"")//item_tuple
        '                [contains(description, ""Bicycle"") 
        '                or contains(description, ""Tricycle"")]
        'let $bikebids := doc(""bids.xml"")//bid_tuple[itemno = $allbikes/itemno]
        'Return
        '    <high_bid>
        '    { 
        '        max($bikebids/bid) 
        '    }
        '    </high_bid>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")
        Dim bids = XDocument.Load(dataPath & "bids.xml")

        Dim maxBid = Aggregate item In items...<item_tuple> _
                     Join bid In bids...<bid_tuple> _
                     On item.<itemno>.Value Equals bid.<itemno>.Value _
                     Where item.<description>.Value.Contains("Bicycle") _
                     OrElse item.<description>.Value.Contains("Tricycle") _
                     From e In bid.<bid> _
                     Into Max(CDbl(e.Value))

        Dim result = <high_bid>
                         <%= maxBid %>
                     </high_bid>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q8"), _
    Description("How many items were actioned (auction ended) in March 1999?")> _
    Public Sub Q31()
        'Solution in XQuery:
        '    let $item := doc(""items.xml"")//item_tuple
        '[end_date >= xs:date(""1999-03-01"") and end_date <= xs:date(""1999-03-31"")]()
        'Return
        '    <item_count>
        '    { 
        '        count($item) 
        '    }
        '    </item_count>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")

        Dim itemQuery = From item In items...<item_tuple> _
                        Where item.<end_date>.Value >= #3/1/1999# _
                        AndAlso item.<end_date>.Value <= #3/31/1999# _
                        Select item

        Dim result = <item_count>
                         <%= itemQuery.Count() %>
                     </item_count>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q9"), _
    Description("List the number of items auctioned each month in 1999 for which data is available, ordered by month.")> _
    Public Sub Q32()
        'Solution in XQuery:
        '<result>
        '{
        '    let $end_dates := doc(""items.xml"")//item_tuple/end_date
        '    for $m in distinct-values(for $e in $end_dates 
        '                            return month-from-date($e))
        '    let $item := doc(""items.xml"")
        '        //item_tuple[year-from-date(end_date) = 1999 
        '                    and month-from-date(end_date) = $m]
        '    order by $m
        '        Return
        '        <monthly_result>
        '            <month>{ $m }</month>
        '            <item_count>{ count($item) }</item_count>
        '        </monthly_result>
        '}
        '</result>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")

        Dim result = <result>
                         <%= From item In items...<item_tuple> _
                             Where CDate(item.<end_date>.Value).Year = 1999 _
                             Group item By month = CDate(item.<end_date>.Value).Month _
                             Into Group _
                             Order By month _
                             Select _
                             <monthly_result>
                                 <month><%= month %></month>
                                 <item_count><%= Group.Count %></item_count>
                             </monthly_result> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q10"), _
    LinkedClass("MyExtensions"), _
    Description("For each item that has received a bid, list the item number, the highest bid, and the name of the highest bidder, ordered by item number.")> _
    Public Sub Q33()
        'Solution in XQuery:
        '<result>
        '{
        '    for $highbid in doc(""bids.xml"")//bid_tuple,
        '        $user in doc(""users.xml"")//user_tuple
        '    where $user/userid = $highbid/userid 
        '    and $highbid/bid = max(doc(""bids.xml"")//bid_tuple[itemno=$highbid/itemno]/bid)
        '    order by $highbid/itemno
        '        Return
        '        <high_bid>
        '            { $highbid/itemno }
        '            { $highbid/bid }
        '            <bidder>{ $user/name/text() }</bidder>
        '        </high_bid>
        '}
        '</result>

        'Solution in VB
        Dim bids = XDocument.Load(dataPath & "bids.xml")
        Dim users = XDocument.Load(dataPath & "users.xml")

        Dim result = <result>
                         <%= From bid In bids...<bid_tuple> _
                             Join user In users...<user_tuple> _
                             On bid.<userid>.Value Equals user.<userid>.Value _
                             Group By itemNo = bid.<itemno>.Value _
                             Into Group _
                             Order By itemNo _
                             From highBid In Group _
                             Let maxBid = Aggregate m In Group Into Max(CDbl(m.bid.<bid>.Value)) _
                             Where highBid.bid.<bid>.Value = maxBid _
                             Select _
                             <high_bid>
                                 <itemno><%= itemNo %></itemno>
                                 <%= highBid.bid.<bid> %>
                                 <bidder><%= highBid.user.<name>.Value %></bidder>
                             </high_bid> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q11"), _
    LinkedClass("MyExtensions"), _
    Description("List the item number and description of the item(s) that received the highest bid ever recorded, and the amount of that bid.")> _
    Public Sub Q34()
        'Solution in XQuery:
        'let $highbid := max(doc(""bids.xml"")//bid_tuple/bid)
        '<result>
        '{
        'for $item in doc(""items.xml"")//item_tuple,
        '        $b in doc(""bids.xml"")//bid_tuple[itemno = $item/itemno]
        'where $b/bid = $highbid
        'Return
        '        <expensive_item>
        '            { $item/itemno }
        '            { $item/description }
        '            <high_bid>{ $highbid }</high_bid>
        '        </expensive_item>
        '}
        '</result>

        'Solution in VB
        Dim bids = XDocument.Load(dataPath & "bids.xml")
        Dim items = XDocument.Load(dataPath & "items.xml")

        Dim highBid = Aggregate bid In bids...<bid_tuple>.<bid> Into Max(CDbl(bid.Value))

        Dim result = <result>
                         <%= From item In items...<item_tuple> _
                             Join bid In bids...<bid_tuple> _
                             On item.<itemno>.Value Equals bid.<itemno>.Value _
                             Where bid.<bid>.Value = highBid _
                             Select _
                             <expensive_item>
                                 <%= item.<itemno> %>
                                 <%= item.<description> %>
                                 <high_bid><%= highBid %></high_bid>
                             </expensive_item> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q12"), _
    Description("List the item number and description of the item(s) that received the largest number of bids, and the number of bids it (or they) received.")> _
    Public Sub Q35()
        'Solution in XQuery:
        'declare function local:bid_summary()
        'as element()*
        '{
        '    for $i in distinct-values(doc(""bids.xml"")//itemno)
        '    let $b := doc(""bids.xml"")//bid_tuple[itemno = $i]
        '        Return
        '        <bid_count>
        '            <itemno>{ $i }</itemno>
        '            <nbids>{ count($b) }</nbids>
        '        </bid_count>
        '};

        '<result>
        '{
        '    let $bid_counts := local:bid_summary(),
        '        $maxbids := max($bid_counts/nbids),
        '        $maxitemnos := $bid_counts[nbids = $maxbids]
        '    for $item in doc(""items.xml"")//item_tuple,
        '        $bc in $bid_counts
        '    where $bc/nbids =  $maxbids and $item/itemno = $bc/itemno
        '            Return
        '        <popular_item>
        '            { $item/itemno }
        '            { $item/description }
        '            <bid_count>{ $bc/nbids/text() }</bid_count>
        '        </popular_item>
        '}
        '</result>

        'Solution in VB
        Dim items = XDocument.Load(dataPath & "items.xml")
        Dim bids = XDocument.Load(dataPath & "bids.xml")

        Dim bidCounts = From item In (From e In bids...<itemno> _
                                      Select e.Value _
                                      Distinct) _
                        Group Join b In bids...<bid_tuple> _
                              On item Equals b.<itemno>.Value _
                        Into itemBids = Group _
                        Select itemNo = item, nbids = itemBids.Count()

        Dim maxBids = Aggregate bc In bidCounts Into Max(bc.nbids)

        Dim result = <result>
                         <%= From item In items...<item_tuple> _
                             Join bc In bidCounts _
                             On item.<itemno>.Value Equals bc.itemNo _
                             Where bc.nbids = maxBids _
                             Select _
                             <popular_item>
                                 <%= item.<itemno> %>
                                 <%= item.<description> %>
                                 <bid_count><%= bc.nbids %></bid_count>
                             </popular_item> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q13"), _
    LinkedClass("MyExtensions"), _
    Description("For each user who has placed a bid, give the userid, name, number of bids, and average bid, in order by userid.")> _
    Public Sub Q36()
        'Solution in XQuery:
        '<result>
        '    '{
        '    for $uid in distinct-values(doc(""bids.xml"")//userid),
        '        $u in doc(""users.xml"")//user_tuple[userid = $uid]
        '    let $b := doc(""bids.xml"")//bid_tuple[userid = $uid]
        '    order by $u/userid
        '        Return
        '        <bidder>
        '            { $u/userid }
        '            { $u/name }
        '            <bidcount>{ count($b) }</bidcount>
        '            <avgbid>{ avg($b/bid) }</avgbid>
        '        </bidder>
        '}
        '</result>

        'Solution in VB
        Dim users = XDocument.Load(dataPath & "users.xml")
        Dim bids = XDocument.Load(dataPath & "bids.xml")

        Dim result = <result>
                         <%= From uid In _
 _
                             (From e In bids...<userid> _
                             Select e.Value Distinct) _
 _
                             Join user In users...<user_tuple> _
                             On uid Equals user.<userid>.Value _
                             Group Join bid In bids...<bid_tuple> _
                             On uid Equals bid.<userid>.Value _
                             Into itemBids = Group _
                             Order By user.<userid>.Value _
                             Select _
                             <bidder>
                                 <%= user.<userid> %>
                                 <%= user.<name> %>
                                 <bid_count><%= itemBids.Count() %></bid_count>
                                 <avgbid><%= Aggregate item In itemBids Into Average(CDbl(item.<bid>.Value)) %></avgbid>
                             </bidder> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q14"), _
    LinkedClass("MyExtensions"), _
    Description("List item numbers and average bids for items that have received three or more bids, in descending order by average bid.")> _
    Public Sub Q37()
        'Solution in XQuery:
        '<result>
        '{
        '    for $i in distinct-values(doc(""bids.xml"")//itemno)
        '    let $b := doc(""bids.xml"")//bid_tuple[itemno = $i]
        '    let $avgbid := avg($b/bid)
        '    where count($b) >= 3
        '    order by $avgbid descending
        '        Return
        '        <popular_item>
        '            <itemno>{ $i }</itemno>
        '            <avgbid>{ $avgbid }</avgbid>
        '        </popular_item>
        '}
        '</result>

        'Solution in VB
        Dim bids = XDocument.Load(dataPath & "bids.xml")

        Dim result = <result>
                         <%= From item In _
 _
                             (From e In bids...<itemno> _
                             Select e.Value Distinct) _
 _
                             Group Join b In bids...<bid_tuple> _
                             On item Equals b.<itemno>.Value _
                             Into itemBids = Group _
                             Let avgBid = (Aggregate b In itemBids Into Average(CDbl(b.<bid>.Value))) _
                             Where itemBids.Count() >= 3 _
                             Order By avgBid Descending _
                             Select _
                             <popular_item>
                                 <itemno><%= item %></itemno>
                                 <avgbid><%= avgBid %></avgbid>
                             </popular_item> _
                         %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q15"), _
    Description("List names of users who have placed multiple bids of at least $100 each.")> _
    Public Sub Q38()
        'Solution in XQuery:
        '<result>
        '{
        '    for $u in doc(""users.xml"")//user_tuple
        '    let $b := doc(""bids.xml"")//bid_tuple[userid=$u/userid and bid>=100]
        '    where count($b) > 1
        '        Return
        '        <big_spender>{ $u/name/text() }</big_spender>
        '}
        '</result>

        'Solution in VB
        Dim bids = XDocument.Load(dataPath & "bids.xml")
        Dim users = XDocument.Load(dataPath & "users.xml")

        Dim result = <result>
                         <%= From user In users...<user_tuple> _
                             Group Join bid In _
 _
                             (From item In bids...<bid_tuple> _
                             Where CDbl(item.<bid>.Value) >= 100 _
                             Select item) _
 _
                             On user.<userid>.Value Equals bid.<userid>.Value _
                             Into itemBids = Group _
                             Where itemBids.Count() > 1 _
                             Select _
                             <big_spender><%= user.<name>.Value %></big_spender> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q16"), _
    LinkedFunction("GetStatus"), _
    Description("List all registered users in order by userid; for each user, include the userid, name, and an indication of whether the user is active (has at least one bid on record) or inactive (has no bid on record)")> _
    Public Sub Q39()
        'Solution in XQuery:
        '<result>
        '{
        '    for $u in doc(""users.xml"")//user_tuple
        '    let $b := doc(""bids.xml"")//bid_tuple[userid = $u/userid]
        '    order by $u/userid
        '        Return
        '        <user>
        '            { $u/userid }
        '            { $u/name }
        '            {
        '                if (empty($b))
        '                then <status>inactive</status>
        '                else <status>active</status>
        '            }
        '        </user>
        '}
        '</result>

        'Solution in VB
        Dim bids = XDocument.Load(dataPath & "bids.xml")
        Dim users = XDocument.Load(dataPath & "users.xml")

        Dim result = <result>
                         <%= From user In users...<user_tuple> _
                             Group Join b In bids...<bid_tuple> _
                             On user.<userid>.Value Equals b.<userid>.Value _
                             Into itemBids = Group _
                             Order By user.<userid>.Value _
                             Select _
                             <user>
                                 <%= user.<userid> %>
                                 <%= user.<name> %>
                                 <status><%= If(itemBids.Any, "active", "inactive") %></status>
                             </user> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q17"), _
    Description("List the names of users, if any, who have bid on every item")> _
    Public Sub Q40()
        'Solution in XQuery:
        '<frequent_bidder>
        '{
        '    for $u in doc(""users.xml"")//user_tuple
        '    where 
        '    every $item in doc(""items.xml"")//item_tuple satisfies 
        '        some $b in doc(""bids.xml"")//bid_tuple satisfies 
        '        ($item/itemno = $b/itemno and $u/userid = $b/userid)
        '        Return
        '        $u/name
        '}
        '</frequent_bidder>

        'Solution in VB
        Dim bids = XDocument.Load(dataPath & "bids.xml")
        Dim users = XDocument.Load(dataPath & "users.xml")
        Dim items = XDocument.Load(dataPath & "items.xml")

        Dim result = <frequent_bidder>
                         <%= From user In users...<user_tuple> _
                             Group Join b In bids...<bid_tuple> _
                             On user.<userid>.Value Equals b.<userid>.Value _
                             Into itemBids = Group _
 _
                             Let itemNos = _
                             (From item In itemBids _
                             Select itemNo = item.<itemno>.Value Distinct Order By itemNo) _
 _
                             Where itemNos.SequenceEqual _
                             (From item In items...<itemno> _
                             Select itemNo = item.Value Order By itemNo) _
 _
                             Select user.<name> %>
                     </frequent_bidder>

        Console.WriteLine(result)
    End Sub

    <Category("R - Access to Relational Data"), _
    Title("Q18"), _
    Description("List all users in alphabetic order by name. For each user, include descriptions of all the items (if any) that were bid on by that user, in alphabetic order.")> _
    Public Sub Q41()
        'Solution in XQuery:
        '<result>
        '{
        '    for $u in doc(""users.xml"")//user_tuple
        '    order by $u/name
        '        Return
        '        <user>
        '            { $u/name }
        '            {
        '                for $b in distinct-values(doc(""bids.xml"")//bid_tuple
        '                                            [userid = $u/userid]/itemno)
        '                for $i in doc(""items.xml"")//item_tuple[itemno = $b]
        '                let $descr := $i/description/text()
        '                order by $descr
        '                Return
        '                    <bid_on_item>{ $descr }</bid_on_item>
        '            }
        '        </user>
        '}
        '</result>

        'Solution in VB
        Dim bids = XDocument.Load(dataPath & "bids.xml")
        Dim users = XDocument.Load(dataPath & "users.xml")
        Dim items = XDocument.Load(dataPath & "items.xml")

        Dim result = <result>
                         <%= From user In users...<user_tuple> _
                             Join bid In bids...<bid_tuple> _
                             On user.<userid>.Value Equals bid.<userid>.Value _
                             Join item In items...<item_tuple> _
                             On bid.<itemno>.Value Equals item.<itemno>.Value _
                             Group item.<description>.Value By user.<name>.Value Into Group _
                             Order By name _
                             Select _
                             <user>
                                 <name><%= name %></name>
                                 <%= From item In Group _
                                     Distinct _
                                     Order By item _
                                     Select <bid_on_item><%= item %></bid_on_item> %>
                             </user> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q1"), _
    Description("Locate all paragraphs in the report (all ""para"" elements occurring anywhere within the ""report"" element).")> _
    Public Sub Q42()
        'Solution in XQuery:
        '<result>
        '{ 
        '    doc(""sgml.xml"")//report//para 
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= sgml...<report>...<para> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q2"), _
    Description("Locate all paragraph elements in an introduction (all ""para"" elements directly contained within an ""intro"" element).")> _
    Public Sub Q43()
        'Solution in XQuery:
        '<result>
        '{ 
        '    doc(""sgml.xml"")//intro/para 
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= sgml...<intro>.<para> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q3"), _
    Description("Locate all paragraphs in the introduction of a section that is in a chapter that has no introduction (all ""para"" elements directly contained within an ""intro"" element directly contained in a ""section"" element directly contained in a ""chapter"" element. The ""chapter"" element must not directly contain an ""intro"" element).")> _
    Public Sub Q44()
        'Solution in XQuery:
        '<result>
        '{
        '    for $c in doc(""sgml.xml"")//chapter
        '    where empty($c/intro)
        '    return $c/section/intro/para
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= From chapter In sgml...<chapter> _
                             Where Not chapter.<intro>.Any _
                             Select chapter.<section>.<intro>.<para> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q4"), _
    Description("Locate the second paragraph in the third section in the second chapter (the second ""para"" element occurring in the third ""section"" element occurring in the second ""chapter"" element occurring in the ""report"").")> _
    Public Sub Q45()
        'Solution in XQuery:
        '<result>
        '{
        '    (((doc(""sgml.xml"")//chapter)[2]//section)[3]//para)[2]
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= sgml...<chapter>(1)...<section>(2)...<para>(1) %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q5"), _
    Description("Locate all classified paragraphs (all ""para"" elements whose ""security"" attribute has the value ""c"").")> _
    Public Sub Q46()
        'Solution in XQuery:
        '<result>
        '{
        '    doc(""sgml.xml"")//para[@security = ""c""]
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= From paragraph In sgml...<para> _
                             Where paragraph.@security = "c" _
                             Select paragraph %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q6"), _
    Description("List the short titles of all sections (the values of the ""shorttitle"" attributes of all ""section"" elements, expressing each short title as the value of a new element.)")> _
    Public Sub Q47()
        'Solution in XQuery:
        '<result>
        '{
        '    for $s in doc(""sgml.xml"")//section/@shorttitle
        '    return <stitle>{ $s }</stitle>]
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= From section In sgml...<section>.Attributes("shorttitle") _
                             Select <stitle <%= section %>/> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q7"), _
    Description("Locate the initial letter of the initial paragraph of all introductions (the first character in the content [character content as well as element content] of the first ""para"" element contained in an ""intro"" element).")> _
    Public Sub Q48()
        'Solution in XQuery:
        '<result>
        '{
        '    for $i in doc(""sgml.xml"")//intro/para[1]
        '        Return
        '        <first_letter>{ substring(string($i), 1, 1) }</first_letter>
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= From intro In sgml...<intro> _
                             Select <first_letter><%= intro.<para>(0).Value(0) %></first_letter> %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q8a"), _
    Description("Locate all sections with a title that has ""is SGML"" in it. The string may occur anywhere in the descendants of the title element, and markup boundaries are ignored")> _
    Public Sub Q49()
        'Solution in XQuery:
        '<result>
        '{
        '    doc(""sgml.xml"")//section[.//title[contains(., ""is SGML"")]]
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= From section In sgml...<section> _
 _
                             Where _
                             (From title In section...<title> _
                             Where title.Value.Contains("is SGML") _
                             Select title).Any _
 _
                             Select section %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q8b"), _
    Description("Same as (Q8a), but the string ""is SGML"" cannot be interrupted by sub-elements, and must appear in a single text node.")> _
    Public Sub Q50()
        'Solution in XQuery:
        '<result>
        '{
        '    doc(""sgml.xml"")//section[.//title/text()[contains(., ""is SGML"")]]
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= From section In sgml...<section> _
 _
                             Where _
                             (From node In section...<title>.Nodes() _
                             Where TypeOf (node) Is XText AndAlso CType(node, XText).Value.Contains("is SGML") _
                             Select node).Any _
 _
                             Select section %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q9"), _
    Description("Locate all the topics referenced by a cross-reference anywhere in the report (all the ""topic"" elements whose ""topicid"" attribute value is the same as an ""xrefid"" attribute value of any ""xref"" element).")> _
    Public Sub Q51()
        'Solution in XQuery:
        '<result>
        '{
        '    for $id in doc(""sgml.xml"")//xref/@xrefid
        '    return doc(""sgml.xml"")//topic[@topicid = $id]
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim result = <result>
                         <%= From id In sgml...<xref> _
                             Join topic In sgml...<topic> _
                             On id.@xrefid Equals topic.@topicid _
                             Select topic %>
                     </result>

        Console.WriteLine(result)
    End Sub

    <Category("SGML - Standard Generalized Markup Language"), _
    Title("Q10"), _
    Description("Locate the closest title preceding the cross-reference (""xref"") element whose ""xrefid"" attribute is ""top4"" (the ""title"" element that would be touched last before this ""xref"" element when touching each element in document order).")> _
    Public Sub Q52()
        'Solution in XQuery:
        '<result>
        '{
        '    let $x := doc(""sgml.xml"")//xref[@xrefid = ""top4""],
        '        $t := doc(""sgml.xml"")//title[. << $x]
        '    return $t[last()]
        '}
        '</result>

        'Solution in VB
        Dim sgml = XDocument.Load(dataPath & "sgml.xml")

        Dim idQuery = From id In sgml...<xref> _
                      Where id.@xrefid = "top4" _
                      Select id

        Dim titleQuery = From title In sgml...<title> _
                         Where title.IsBefore(idQuery(0)) _
                         Select title

        Dim result = <result>
                         <%= If(titleQuery.Count - 1 > 0, titleQuery.Last(), titleQuery.First()) %>
                     </result>

        Console.WriteLine(result)
    End Sub


    <Category("STRING - String Search"), _
    Title("Q1"), _
    Description("Find the titles of all news items where the string ""Foobar Corporation"" appears in the title.")> _
    Public Sub Q53()
        'Solution in XQuery:
        'doc(""string.xml"")//news_item/title[contains(., ""Foobar Corporation"")]

        'Solution in VB
        Dim str = XDocument.Load(dataPath & "string.xml")

        Dim result = From s In str...<news_item> _
                     Where s.<title>.Value.Contains("Foobar Corporation") _
                     Select s.<title>.First()

        For Each t In result
            Console.WriteLine(t)
        Next
    End Sub

    <Category("STRING - String Search"), _
    Title("Q2"), _
    LinkedFunction("GetPartners"), _
    Description("Find news items where the Foobar Corporation and one or more of its partners are mentioned in the same paragraph and/or title. List each news item by its title and date.")> _
    Public Sub Q54()
        'Solution in XQuery:
        'declare function local:partners($company as xs:string) as element()*
        '{
        '    let $c := doc(""company-data.xml"")//company[name = $company]
        '    return $c//partner
        '};

        'let $foobar_partners := local:partners(""Foobar Corporation"")

        'for $item in doc(""string.xml"")//news_item
        'where
        'some $t in $item//title satisfies
        '    (contains($t/text(), ""Foobar Corporation"")
        '    and (some $partner in $foobar_partners satisfies
        '    contains($t/text(), $partner/text())))
        'or (some $par in $item//par satisfies
        '(contains(string($par), ""Foobar Corporation"")
        '    and (some $partner in $foobar_partners satisfies
        '        contains(string($par), $partner/text())))) 
        '        Return
        '    <news_item>
        '        { $item/title }
        '        { $item/date }
        '    </news_item>

        'Solution in VB
        Dim str = XDocument.Load(dataPath & "string.xml")
        Dim foobarPartners = GetPartners("Foobar Corporation")

        Dim result = From item In str...<news_item> _
 _
                     Let paragraph = _
                       (From para In item...<par> _
                        From part In foobarPartners _
                        Where para.Value.Contains("Foobar Corporation") _
                        AndAlso foobarPartners.Any(Function(x) para.Value.Contains(x.Value)) _
                        Select para) _
 _
                     Where paragraph.Any _
                     OrElse _
                     ( _
                       (From i In item...<title> _
                        Where i.Value.Contains("Foobar Corporation") _
                        Select i).Any _
 _
                        AndAlso foobarPartners.Any(Function(x) item.<title>.Value.Contains(x.Value)) _
                     ) _
                     Select _
                     <news_item>
                         <%= item.<title> %><%= item.<date> %>
                     </news_item>

        For Each e In result
            Console.WriteLine(e)
        Next
    End Sub
    Function GetPartners(ByVal company As String) As IEnumerable(Of XElement)
        Dim compDoc = XDocument.Load(dataPath & "company-data.xml")
        Dim companyQuery = From comp In compDoc...<company> _
                           Where comp.<name>.Value = company _
                           Select comp
        Return companyQuery...<partner>
    End Function

    <Category("STRING - String Search"), _
    Title("Q3"), _
    LinkedFunction("GetPartners"), _
    Description("Find news items where a company and one of its partners is mentioned in the same news item and the news item is not authored by the company itself.")> _
    Public Sub Q55()
        'Solution in XQuery:
        'declare function local:partners($company as xs:string) as element()*
        '{
        '    let $c := doc(""company-data.xml"")//company[name = $company]
        '    return $c//partner
        '};

        'for $item in doc(""string.xml"")//news_item,
        '    $c in doc(""company-data.xml"")//company
        'let $partners := local:partners($c/name)
        'where contains(string($item), $c/name)
        'and (some $p in $partners satisfies
        '    contains(string($item), $p) and $item/news_agent != $c/name)
        '        Return
        '    $item

        'Solution in VB
        Dim str = XDocument.Load(dataPath & "string.xml")
        Dim comp = XDocument.Load(dataPath & "company-data.xml")

        Dim result = From item In str...<news_item> _
                     From c In comp...<company> _
                     Let part = GetPartners(c.<name>.Value) _
                     Where item.Value.Contains(c.<name>.Value) _
                     AndAlso part.Any(Function(p) _
                                        item.Value.Contains(p.Value) _
                                        AndAlso item.<news_agent>.Value <> c.<name>.Value) _
                     Select item

        For Each e In result
            Console.WriteLine(e)
        Next
    End Sub

    <Category("STRING - String Search"), _
    Title("Q4"), _
    Description("For each news item that is relevant to the Gorilla Corporation, create an ""item summary"" element. The content of the item summary is the content of the title, date, and first paragraph of the news item, separated by periods. A news item is relevant if the name of the company is mentioned anywhere within the content of the news item.")> _
    Public Sub Q56()
        'Solution in XQuery:
        'for $item in doc(""string.xml"")//news_item
        'where contains(string($item/content), ""Gorilla Corporation"")
        '    Return
        '    <item_summary>
        '        { concat($item/title,"". "") }
        '        { concat($item/date,"". "") }
        '        { string(($item//par)[1]) }
        '    </item_summary>

        'Solution in VB
        Dim str = XDocument.Load(dataPath & "string.xml")

        Dim result = From item In str...<news_item> _
                     Where item.<content>.Value.Contains("Gorilla Corporation") _
                     Select _
                     <item_summary>
                         <%= item.<title>.Value & ". " %>
                         <%= item.<date>.Value & ". " %>
                         <%= item...<par>(0).Value %>
                     </item_summary>

        For Each e In result
            Console.WriteLine(e)
        Next
    End Sub

    <Category("PARTS - Recursive Parts Explosion"), _
    Title("Q1"), _
    LinkedFunction("GetOneLevel"), _
    Description("Convert the sample document from ""partlist"" format to ""parttree"" format. In the result document, part containment is represented by containment of one <part> element inside another. Each part that is not part of any other part should appear as a separate top-level element in the output document.")> _
    Public Sub Q57()
        'Solution in XQuery:
        'declare function local:one_level($p as element()) as element()
        '{
        '    <part partid=""{ $p/@partid }""
        '          name=""{ $p/@name }"" >
        '        {
        '            for $s in doc(""partlist.xml"")//part
        '            where $s/@partof = $p/@partid
        '            return local:one_level($s)
        '        }
        '    </part>
        '};

        '<parttree>
        '  {
        '    for $p in doc(""partlist.xml"")//part[empty(@partof)]
        '    return local:one_level($p)
        '  }
        '</parttree>

        'Solution in VB
        Dim partlist = XDocument.Load(dataPath & "partlist.xml")

        Dim result = <parttree>
                         <%= From part In partlist...<part> _
                             Where part.@partof Is Nothing _
                             Select GetOneLevel(part) %>
                     </parttree>

        Console.WriteLine(result)
    End Sub
    Function GetOneLevel(ByVal p As XElement) As XElement
        Dim partlist = XDocument.Load(dataPath & "partlist.xml")
        Return <part partid=<%= p.@partid %> name=<%= p.@name %>>
                   <%= From s In partlist...<part> _
                       Where s.@partof IsNot Nothing AndAlso s.@partof = p.@partid _
                       Select GetOneLevel(s) %>
               </part>
    End Function
End Class
