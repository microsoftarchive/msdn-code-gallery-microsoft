================================================================================
				Windows APPLICATION: CSManipulateImagesInWordDocument                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to export, delete or replace the images in a word document
using Open XML SDK. 

/////////////////////////////////////////////////////////////////////////////
Prerequisite

Open XML SDK 2.0

You can download it in the following link:
http://www.microsoft.com/downloads/en/details.aspx?FamilyId=C6E744E5-36E9-45F5-8D8C-331DF206E0D0&displaylang=en


////////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Open this project in  Visual Studio 2010. 
        
Step2. Build the solution. 

Step3. Run CSManipulateImagesInWordDocument.exe.

Step4. Click "Open the word doc" button, and in the OpenFileDialog , select a Word 2007/2010
       document(*.docx file). The listbox will show all images reference ID.

Step5. Click an item in the listbox, you will see the image in the right panel.

Step6. Click the "Export" button, it will show a SaveFileDialog and you can save it 
       to a local file.

Step7. Click the "Delete" button, it will show an alert. If you confirm it, this application
       will delete the image. Close this application, and open the document in Word, you 
       will find that the image has been deleted.

Step7. Run Step3, Step4 and Step5 again. Click the "Replace" button, it will show an
       OpenFileDialog. Choose a local image and confirm the alert, this application
       will replace the image. Close this application, and open the document in Word, you 
       will find that the image has been replaced.

/////////////////////////////////////////////////////////////////////////////
Code Logic:

The image data in a document are stored as a ImagePart, and the Blip element
in a Drawing element will refers to the ImagePart, like following structure

<w:drawing>
  <wp:inline>  
    <a:graphic>
      <a:graphicData>
        <pic:pic>
          <pic:blipFill>
            <a:blip r:embed="rId7">
              <a:extLst>
                <a:ext uri="{28A0092B-C50C-407E-A947-70E740481C1C}">
                  <a14:useLocalDpi val="0" />
                </a:ext>
              </a:extLst>
            </a:blip>
          </pic:blipFill>
        </pic:pic>
      </a:graphicData>
    </a:graphic>
  </wp:inline>
</w:drawing>

A. To list all images in the document, we can get all Drawing elements first, and then get the Blip
   element in the Drawing element.

         public IEnumerable<Blip> GetAllImages()
         {
        
             // Get the drawing elements in the document.
             var drawingElements = from run in Document.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>()
                                   where run.Descendants<Drawing>().Count() != 0
                                   select run.Descendants<Drawing>().First();
        
             // Get the blip elements in the drawing elements.
             var blipElements = from drawing in drawingElements
                                where drawing.Descendants<Blip>().Count() > 0
                                select drawing.Descendants<Blip>().First();           
        
             return blipElements;
         }

B. To delete the image, we can delete the Drawing element that contains the Blip element.
        public void DeleteImage(Blip blipElement)
        {
            OpenXmlElement parent = blipElement.Parent;
            while (parent != null && !(parent is DocumentFormat.OpenXml.Wordprocessing.Drawing))
            {
                parent = parent.Parent;
            }

            if (parent != null)
            {
                Drawing drawing = parent as Drawing;
                drawing.Parent.RemoveChild<Drawing>(drawing);

                // Raise the ImagesChanged event.
                this.OnImagesChanged();

            }
        }


C. To replace an image in a document, we have to add an ImagePart to the document first,
   and then edit the Blip element to refer to the new ImagePart.

        public void ReplaceImage(Blip blipElement, FileInfo newImg)
        {
            string rid = AddImagePart(newImg);
            blipElement.Embed.Value = rid;
            this.OnImagesChanged();
        }

        string AddImagePart(FileInfo newImg)
        {
            ImagePartType type = ImagePartType.Bmp ;
            switch(newImg.Extension.ToLower())
            {
                case ".jpeg":
                case ".jpg":
                    type = ImagePartType.Jpeg;
                    break;
                case ".png":
                    type = ImagePartType.Png;
                    break;
                default:
                    type = ImagePartType.Bmp;
                    break;
            }

            ImagePart newImgPart = Document.MainDocumentPart.AddImagePart(type);
            using (FileStream stream = newImg.OpenRead())
            {
                newImgPart.FeedData(stream);
            }

            string rId = Document.MainDocumentPart.GetIdOfPart(newImgPart);
            return rId;
        }

D. Because we have set the image as the Image property of the PictureBox, we can just use
   the Image.Save method to export the image to local file.

    picView.Image.Save(dialog.FileName, ImageFormat.Jpeg);

/////////////////////////////////////////////////////////////////////////////
References:

Welcome to the Open XML SDK 2.0 for Microsoft Office
http://msdn.microsoft.com/en-us/library/bb448854.aspx

WordprocessingDocument Class
http://msdn.microsoft.com/en-us/library/documentformat.openxml.packaging.wordprocessingdocument.aspx

ImagePart Class
http://msdn.microsoft.com/en-us/library/documentformat.openxml.packaging.imagepart.aspx
/////////////////////////////////////////////////////////////////////////////