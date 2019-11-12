Imports <xmlns='urn:art-org:art'>
Imports <xmlns:a='urn:art-org:artists'>
Imports System.Xml

Module Module1

    Sub Main()
        ' construct the predefined document
        ' using the XML Literals approach

        Dim document = CreateDocumentLiteral()
        Console.WriteLine(document)

        ' dump all nodes to the console 
        DumpNode(document)

        ' construct the predefined document again
        ' using the XML Literals approach with imported XML Namespaces
        document = CreateDocumentLiteralWithImportedNamespaces()
        Console.WriteLine(document)

        ' dump all nodes to the console 
        DumpNode(document)

        ' construct the predefined document again
        ' using the XML DOM-like approach
        document = CreateDocumentVerbose()

        ' display the document in the console
        Console.WriteLine(document)

        ' dump all nodes to the console 
        DumpNode(document)

        ' find all elements named 'artist' using Elements property
        FindChildren(document)

        ' find all elements named 'artist' using Descendants property
        FindDescendants(document)

        ' find attribute named 'name' using Attribute property
        FindAttribute(document)

        Console.ReadLine()
    End Sub
    ' <?xml version="1.0"?>
    ' <?order alpha ascending?>
    ' <art xmlns='urn:art-org:art'>
    '   <period name='Renaissance' xmlns:a='urn:art-org:artists'>
    '     <a:artist>Leonardo da Vinci</a:artist>
    '     <a:artist>Michelangelo</a:artist>
    '     <a:artist><![CDATA[Donatello]]></a:artist>
    '   </period>
    '   <!-- insert period here -->
    ' </art>
    Function CreateDocumentLiteral() As XDocument
        Return <?xml version="1.0"?>
               <?order alpha ascending?>
               <art xmlns='urn:art-org:art'>
                   <period name='Renaissance' xmlns:a='urn:art-org:artists'>
                       <a:artist>Leonardo da Vinci</a:artist>
                       <a:artist>Michelangelo</a:artist>
                       <a:artist><![CDATA[Donatello]]></a:artist>
                   </period>
                   <!-- insert period here -->
               </art>
    End Function

    ' <?xml version="1.0"?>
    ' <?order alpha ascending?>
    ' <art xmlns='urn:art-org:art'>
    '   <period name='Renaissance' xmlns:a='urn:art-org:artists'>
    '     <a:artist>Leonardo da Vinci</a:artist>
    '     <a:artist>Michelangelo</a:artist>
    '     <a:artist><![CDATA[Donatello]]></a:artist>
    '   </period>
    '   <!-- insert period here -->
    ' </art>
    Function CreateDocumentLiteralWithImportedNamespaces() As XDocument
        Return <?xml version="1.0"?>
               <?order alpha ascending?>
               <art>
                   <period name='Renaissance'>
                       <a:artist>Leonardo da Vinci</a:artist>
                       <a:artist>Michelangelo</a:artist>
                       <a:artist><![CDATA[Donatello]]></a:artist>
                   </period>
                   <!-- insert period here -->
               </art>
    End Function

    Function CreateDocumentVerbose() As XDocument
        Dim nsArt As XNamespace = "urn:art-org:art"
        Dim nsArtists As XNamespace = "urn:art-org:artists"

        ' create the document
        Dim document = New XDocument()

        ' create the xml declaration and
        ' set on the document
        document.Declaration = New XDeclaration("1.0", Nothing, Nothing)

        ' create the art element and
        ' add to the document 
        Dim art = New XElement(nsArt + "art")
        document.Add(art)

        ' create the order processing instruction and
        ' add before the art element 
        Dim pi = New XProcessingInstruction("order", "alpha ascending")
        art.AddBeforeSelf(pi)

        ' create the period element and
        ' add to the art element
        Dim period = New XElement(nsArt + "period")
        art.Add(period)

        ' add the name attribute to the period element 
        period.SetAttributeValue("name", "Renaissance")

        ' create the namespace declaration xmlns:a and
        ' add to the period element 
        Dim nsdecl = New XAttribute(XNamespace.Xmlns + "a", nsArtists)
        period.Add(nsdecl)

        ' create the artists elements and
        ' the underlying text nodes
        period.SetElementValue(nsArtists + "artist", "Michelangelo")

        Dim artist = New XElement(nsArtists + "artist", "Leonardo ", "da ", "Vinci")
        period.AddFirst(artist)

        artist = New XElement(nsArtists + "artist")
        period.Add(artist)
        Dim cdata = New XText("Donatello")
        artist.Add(cdata)

        ' create the comment and
        ' add to the art element
        Dim comment = New XComment("insert period here")
        art.Add(comment)

        Return document
    End Function
    

    Sub DumpNode(ByVal node As XNode)
        Select Case node.NodeType
            Case XmlNodeType.Document
                Dim document = CType(node, XDocument)
                Console.WriteLine("StartDocument")
                Dim declaration = document.Declaration
                If declaration IsNot Nothing Then
                    Console.WriteLine("XmlDeclaration: {0} {1} {2}", declaration.Version, declaration.Encoding, declaration.Standalone)
                End If
                For Each n In document.Nodes()
                    DumpNode(n)
                Next
                Console.WriteLine("EndDocument")
            Case XmlNodeType.Element
                Dim element = CType(node, XElement)
                Console.WriteLine("StartElement: {0}", element.Name)
                If element.HasAttributes Then
                    For Each attribute In element.Attributes()
                        Console.WriteLine("Attribute: {0} = {1}", attribute.Name, attribute.Value)
                    Next
                End If
                If Not element.IsEmpty Then
                    For Each n In element.Nodes()
                        DumpNode(n)
                    Next
                    End If
                Console.WriteLine("EndElement: {0}", element.Name)
            Case XmlNodeType.Text
                Dim text = CType(node, XText)
                Console.WriteLine("Text: {0}", text.Value)
            Case XmlNodeType.ProcessingInstruction
                Dim pi = CType(node, XProcessingInstruction)
                Console.WriteLine("ProcessingInstruction: {0} {1}", pi.Target, pi.Data)
            Case XmlNodeType.Comment
                Dim comment = CType(node, XComment)
                Console.WriteLine("Comment: {0}", comment.Value)
            Case XmlNodeType.DocumentType
                Dim documentType = CType(node, XDocumentType)
                Console.WriteLine("DocumentType: {0} {1} {2} {3}", documentType.Name, documentType.PublicId, documentType.SystemId, documentType.InternalSubset)
        End Select
    End Sub

    Sub FindChildren(ByVal document As XDocument)
        ' use Elements property on an XDocument object to retrive all elements named 'artist'
        Dim results = document.<art>.<period>.<a:artist>
        For Each element In results
            Console.WriteLine("Child 'artist' Element: " & element.Value)
        Next

        ' use Elements property on an XElement object to retrive all elements named 'artist'
        Dim periodElement = document.<art>.<period>(0)
        results = periodElement.<a:artist>
        For Each element In results
            Console.WriteLine("Child 'artist' Element: " & element.Value)
        Next
    End Sub

    Sub FindDescendants(ByVal document As XDocument)
        ' use Descendants property on an XDocument object to retrive all elements named 'artist'
        Dim results = document...<a:artist>
        For Each element In results
            Console.WriteLine("Descendant 'artist' Element: " & element.Value)
        Next

        ' use Descendants property on an XElement object to retrive all elements named 'artist'
        Dim artElement = document.<art>(0)
        results = artElement...<a:artist>
        For Each element In results
            Console.WriteLine("Descendant 'artist' Element: " & element.Value)
        Next
    End Sub

    Sub FindAttribute(ByVal document As XDocument)
        ' use Attribute property as well as Elements property to retrive first attribute named 'name' 
        ' inside element named 'period' within an XDocument object
        Dim result = document.<art>.<period>.@name
        Console.WriteLine("'name' Attribute: " & result)

        ' use Attribute property as well as Descendants property to retrive first attribute named 'name'
        ' inside element named 'period' within an XDocument object
        result = document...<period>.@name
        Console.WriteLine("'name' Attribute: " & result)

        ' use Attribute property as well as Descendants property to retrive first attribute named 'name'
        ' inside element named 'period' within an XElement object
        Dim periodElement = document...<period>(0)
        result = periodElement.@name
        Console.WriteLine("'name' Attribute: " & result)
    End Sub
End Module
