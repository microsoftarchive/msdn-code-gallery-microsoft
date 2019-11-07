/****************************** Module Header ******************************\
* Module Name:  CSOpenXmlInsertImageToPPT.cs
* Project:      CSOpenXmlInsertImageToPPT
* Copyright(c)  Microsoft Corporation.
* 
* The Class is used to Insert Image into PowerPoint using Open XML SDK.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace CSOpenXmlInsertImageToPPT
{
    public class InsertImage
    {
        /// <summary>
        /// Insert a new Slide into PowerPoint
        /// </summary>
        /// <param name="presentationPart">Presentation Part</param>
        /// <param name="layoutName">Layout of the new Slide</param>
        /// <returns>Slide Instance</returns>
        public Slide InsertSlide(PresentationPart presentationPart, string layoutName)
        {
            UInt32 slideId = 256U;

            // Get the Slide Id collection of the presentation document
            var slideIdList = presentationPart.Presentation.SlideIdList;

            if (slideIdList == null)
            {
                throw new NullReferenceException("The number of slide is empty, please select a ppt with a slide at least again");
            }

            slideId += Convert.ToUInt32(slideIdList.Count());

            // Creates a Slide instance and adds its children.
            Slide slide = new Slide(new CommonSlideData(new ShapeTree()));

            SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();
            slide.Save(slidePart);
            
            // Get SlideMasterPart and SlideLayoutPart from the existing Presentation Part
            SlideMasterPart slideMasterPart = presentationPart.SlideMasterParts.First();
            SlideLayoutPart slideLayoutPart = slideMasterPart.SlideLayoutParts.SingleOrDefault
                (sl => sl.SlideLayout.CommonSlideData.Name.Value.Equals(layoutName, StringComparison.OrdinalIgnoreCase));
            if (slideLayoutPart == null)
            {
                throw new Exception("The slide layout " + layoutName + " is not found");
            }

            slidePart.AddPart<SlideLayoutPart>(slideLayoutPart);

            slidePart.Slide.CommonSlideData = (CommonSlideData)slideMasterPart.SlideLayoutParts.SingleOrDefault(
                sl => sl.SlideLayout.CommonSlideData.Name.Value.Equals(layoutName)).SlideLayout.CommonSlideData.Clone();

            // Create SlideId instance and Set property
            SlideId newSlideId = presentationPart.Presentation.SlideIdList.AppendChild<SlideId>(new SlideId());
            newSlideId.Id = slideId;
            newSlideId.RelationshipId = presentationPart.GetIdOfPart(slidePart);

            return GetSlideByRelationShipId(presentationPart, newSlideId.RelationshipId);
        }
        
        /// <summary>
        /// Get Slide By RelationShip ID
        /// </summary>
        /// <param name="presentationPart">Presentation Part</param>
        /// <param name="relationshipId">Relationship ID</param>
        /// <returns>Slide Object</returns>
        private static Slide GetSlideByRelationShipId(PresentationPart presentationPart, StringValue relationshipId)
        {
            // Get Slide object by Relationship ID
            SlidePart slidePart = presentationPart.GetPartById(relationshipId) as SlidePart;
            if (slidePart != null)
            {
                return slidePart.Slide;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Insert Image into Slide
        /// </summary>
        /// <param name="filePath">PowerPoint Path</param>
        /// <param name="imagePath">Image Path</param>
        /// <param name="imageExt">Image Extension</param>
        public void InsertImageInLastSlide(Slide slide, string imagePath, string imageExt)
        {
            // Creates a Picture instance and adds its children.
            P.Picture picture = new P.Picture();
            string embedId = string.Empty;
            embedId = "rId" + (slide.Elements<P.Picture>().Count() + 915).ToString();
            P.NonVisualPictureProperties nonVisualPictureProperties = new P.NonVisualPictureProperties(
                new P.NonVisualDrawingProperties() { Id = (UInt32Value)4U, Name = "Picture 5" },
                new P.NonVisualPictureDrawingProperties(new A.PictureLocks() { NoChangeAspect = true }),
                new ApplicationNonVisualDrawingProperties());

            P.BlipFill blipFill = new P.BlipFill();
            Blip blip = new Blip() { Embed = embedId };

            // Creates a BlipExtensionList instance and adds its children
            BlipExtensionList blipExtensionList = new BlipExtensionList();
            BlipExtension blipExtension = new BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };

            UseLocalDpi useLocalDpi = new UseLocalDpi() { Val = false };
            useLocalDpi.AddNamespaceDeclaration("a14",
                "http://schemas.microsoft.com/office/drawing/2010/main");

            blipExtension.Append(useLocalDpi);
            blipExtensionList.Append(blipExtension);
            blip.Append(blipExtensionList);

            Stretch stretch = new Stretch();
            FillRectangle fillRectangle = new FillRectangle();
            stretch.Append(fillRectangle);

            blipFill.Append(blip);
            blipFill.Append(stretch);

            // Creates a ShapeProperties instance and adds its children.
            P.ShapeProperties shapeProperties = new P.ShapeProperties();

            A.Transform2D transform2D = new A.Transform2D();
            A.Offset offset = new A.Offset() { X = 457200L, Y = 1524000L };
            A.Extents extents = new A.Extents() { Cx = 8229600L, Cy = 5029200L };

            transform2D.Append(offset);
            transform2D.Append(extents);

            A.PresetGeometry presetGeometry = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
            A.AdjustValueList adjustValueList = new A.AdjustValueList();

            presetGeometry.Append(adjustValueList);

            shapeProperties.Append(transform2D);
            shapeProperties.Append(presetGeometry);

            picture.Append(nonVisualPictureProperties);
            picture.Append(blipFill);
            picture.Append(shapeProperties);

            slide.CommonSlideData.ShapeTree.AppendChild(picture);

            // Generates content of imagePart.
            ImagePart imagePart = slide.SlidePart.AddNewPart<ImagePart>(imageExt, embedId);
            FileStream fileStream = new FileStream(imagePath, FileMode.Open);
            imagePart.FeedData(fileStream);
            fileStream.Close();
        }
    }
}
