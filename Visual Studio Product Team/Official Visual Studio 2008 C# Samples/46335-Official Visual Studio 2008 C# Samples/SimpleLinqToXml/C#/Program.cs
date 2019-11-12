//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Xml;
using System.Xml.Linq;

// See the Readme.html file for additional information
namespace Samples
{
    public class Program
    {
        public static void Main()
        {
            // construct the predefined document
            // using the XML DOM-like approach
            XDocument document = CreateDocumentVerbose();

            // display the document in the console
            Console.WriteLine(document);

            // dump all nodes to the console 
            DumpNode(document);

            // construct the predefined document again
            // with the functional approach
            document = CreateDocumentConcise();
            
            // display the document in the console
            Console.WriteLine(document);

            // dump all nodes to the console
            DumpNode(document);

            Console.ReadLine();
        }

        // <?xml version="1.0"?>
        // <?order alpha ascending?>
        // <art xmlns='urn:art-org:art'>
        //   <period name='Renaissance' xmlns:a='urn:art-org:artists'>
        //     <a:artist>Leonardo da Vinci</a:artist>
        //     <a:artist>Michelangelo</a:artist>
        //     <a:artist><![CDATA[Donatello]]></a:artist>
        //   </period>
        //   <!-- insert period here -->
        // </art>
        public static XDocument CreateDocumentVerbose()
        {
            XNamespace nsArt = "urn:art-org:art";
            XNamespace nsArtists = "urn:art-org:artists";

            // create the document
            XDocument document = new XDocument();

            // create the xml declaration and
            // set on the document
            document.Declaration = new XDeclaration("1.0", null, null);

            // create the art element and
            // add to the document 
            XElement art = new XElement(nsArt + "art");
            document.Add(art);

            // create the order processing instruction and
            // add before the art element 
            XProcessingInstruction pi = new XProcessingInstruction("order", "alpha ascending");
            art.AddBeforeSelf(pi);

            // create the period element and
            // add to the art element
            XElement period = new XElement(nsArt + "period");
            art.Add(period);

            // add the name attribute to the period element 
            period.SetAttributeValue("name", "Renaissance");

            // create the namespace declaration xmlns:a and
            // add to the period element 
            XAttribute nsdecl = new XAttribute(XNamespace.Xmlns + "a", nsArtists);
            period.Add(nsdecl);

            // create the artists elements and
            // the underlying text nodes
            period.SetElementValue(nsArtists + "artist", "Michelangelo");

            XElement artist = new XElement(nsArtists + "artist", "Leonardo ", "da ", "Vinci");
            period.AddFirst(artist);

            artist = new XElement(nsArtists + "artist");
            period.Add(artist);
            XText cdata = new XText("Donatello");
            artist.Add(cdata);

            // create the comment and
            // add to the art element
            XComment comment = new XComment("insert period here");
            art.Add(comment);

            return document;
        }

        // <?xml version="1.0"?>
        // <?order alpha ascending?>
        // <art xmlns='urn:art-org:art'>
        //   <period name='Renaissance' xmlns:a='urn:art-org:artists'>
        //     <a:artist>Leonardo da Vinci</a:artist>
        //     <a:artist>Michelangelo</a:artist>
        //     <a:artist><![CDATA[Donatello]]></a:artist>
        //   </period>
        //   <!-- insert period here -->
        // </art>
        public static XDocument CreateDocumentConcise()
        {
            XNamespace nsArt = "urn:art-org:art";
            XNamespace nsArtists = "urn:art-org:artists";

            // create the document all at once
            return new XDocument(
                        new XDeclaration("1.0", null, null),
                        new XProcessingInstruction("order", "alpha ascending"),
                        new XElement(nsArt + "art",
                            new XElement(nsArt + "period",
                                new XAttribute("name", "Renaissance"),
                                new XAttribute(XNamespace.Xmlns + "a", nsArtists),
                                new XElement(nsArtists + "artist", "Leonardo da Vinci"),
                                new XElement(nsArtists + "artist", "Michelangelo"),
                                new XElement(nsArtists + "artist", 
                                    new XText("Donatello"))),
                            new XComment("insert period here")));                        
        }

        public static void DumpNode(XNode node) 
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Document:
                    XDocument document = (XDocument)node;
                    Console.WriteLine("StartDocument");
                    XDeclaration declaration = document.Declaration;
                    if (declaration != null)
                    {
                        Console.WriteLine("XmlDeclaration: {0} {1} {2}", declaration.Version, declaration.Encoding, declaration.Standalone);
                    }
                    foreach (XNode n in document.Nodes())
                    {
                        DumpNode(n);
                    }
                    Console.WriteLine("EndDocument");
                    break;
                case XmlNodeType.Element:
                    XElement element = (XElement)node;
                    Console.WriteLine("StartElement: {0}", element.Name);
                    if (element.HasAttributes) 
                    {
                        foreach (XAttribute attribute in element.Attributes())
                        {
                            Console.WriteLine("Attribute: {0} = {1}", attribute.Name, attribute.Value);
                        }
                    }
                    if (!element.IsEmpty)
                    {
                        foreach (XNode n in element.Nodes())
                        {
                            DumpNode(n);
                        }
                    }
                    Console.WriteLine("EndElement: {0}", element.Name);
                    break;
                case XmlNodeType.Text:
                    XText text = (XText)node;
                    Console.WriteLine("Text: {0}", text.Value); 
                    break;
                case XmlNodeType.ProcessingInstruction:
                    XProcessingInstruction pi = (XProcessingInstruction)node;
                    Console.WriteLine("ProcessingInstruction: {0} {1}", pi.Target, pi.Data);
                    break;
                case XmlNodeType.Comment:
                    XComment comment = (XComment)node;
                    Console.WriteLine("Comment: {0}", comment.Value);
                    break;
                case XmlNodeType.DocumentType:
                    XDocumentType documentType = (XDocumentType)node;
                    Console.WriteLine("DocumentType: {0} {1} {2} {3}", documentType.Name, documentType.PublicId, documentType.SystemId, documentType.InternalSubset);
                    break;
                default:
                    break;
            }
        }
    }
}
