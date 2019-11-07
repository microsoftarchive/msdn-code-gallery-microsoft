//****************************** Module Header ******************************\
//Module Name:    Utility.cs
//Project:        EmbedExcelIntoWordDoc
//Copyright (c) Microsoft Corporation

// The project illustrates how to embed excel sheet into word document using using Open XML SDK

//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
//All other rights reserved.

//*****************************************************************************/
using System;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using v = DocumentFormat.OpenXml.Vml;
using ovml = DocumentFormat.OpenXml.Vml.Office;
using System.IO;
using System.Drawing;

class Utility
{
    public static void CreatePackage(string containingDocumentPath, string embeddedDocumentPath)
    {
        using (WordprocessingDocument package =
          WordprocessingDocument.Create(containingDocumentPath,
            WordprocessingDocumentType.Document))
        {
            AddParts(package, embeddedDocumentPath);
        }
    }

    private static void AddParts(WordprocessingDocument parent,
      string embeddedDocumentPath)
    {
        var mainDocumentPart = parent.AddMainDocumentPart();
        GenerateMainDocumentPart().Save(mainDocumentPart);

        var embeddedPackagePart =
          mainDocumentPart.AddNewPart<EmbeddedPackagePart>(
          "application/vnd.openxmlformats-" +
          "officedocument.spreadsheetml.sheet",
          "rId1");

        GenerateEmbeddedPackagePart(embeddedPackagePart,
          embeddedDocumentPath);

        var imagePart =
          mainDocumentPart.AddNewPart<ImagePart>(
          "image/x-emf", "rId2");

        GenerateImagePart(imagePart);
    }

    private static Document GenerateMainDocumentPart()
    {
        var element =
          new Document(
            new Body(
              new Paragraph(
                new Run(
                  new Text(
                    "This is the containing document."))),
              new Paragraph(
                new Run(
                  new Text(
                    "This is the embedded document: "))),
              new Paragraph(
                new Run(
                  new EmbeddedObject(
                    new v.Shape(
                      new v.ImageData()
                      {
                          Title = "",
                          RelationshipId = "rId2"
                      }
                    )
                    {
                        Id = "_x0000_i1025",
                        Style = "width:76.5pt;height:49.5pt",
                    },
                    new ovml.OleObject()
                    {
                        Type = ovml.OleValues.Embed,
                        ProgId = "Excel.Sheet.12",
                        ShapeId = "_x0000_i1025",
                        DrawAspect = ovml.OleDrawAspectValues.Icon,
                        ObjectId = "_1299573545",
                        Id = "rId1"
                    }
                  )
                )
              )
            )
          );

        return element;
    }

    public static void GenerateEmbeddedPackagePart(OpenXmlPart part,
      string embeddedDocumentPath)
    {
        byte[] embeddedDocumentBytes;

        // The following code will generate an exception if an invalid
        // filename is passed.
        using (FileStream fsEmbeddedDocument =
          File.OpenRead(embeddedDocumentPath))
        {
            embeddedDocumentBytes =
              new byte[fsEmbeddedDocument.Length];

            fsEmbeddedDocument.Read(embeddedDocumentBytes, 0,
              embeddedDocumentBytes.Length);
        }

        using (BinaryWriter writer =
          new BinaryWriter(part.GetStream()))
        {
            writer.Write(embeddedDocumentBytes);
            writer.Flush();
        }
    }

    public static void GenerateImagePart(OpenXmlPart part)
    {
        using (BinaryWriter writer = new BinaryWriter(part.GetStream()))
        {

            writer.Write(System.Convert.FromBase64String(EmbedExcelIntoWordDoc.Properties.Resource1.BASE64_STRING_EXCEL_ICON));

            writer.Flush();
        }
    }
}