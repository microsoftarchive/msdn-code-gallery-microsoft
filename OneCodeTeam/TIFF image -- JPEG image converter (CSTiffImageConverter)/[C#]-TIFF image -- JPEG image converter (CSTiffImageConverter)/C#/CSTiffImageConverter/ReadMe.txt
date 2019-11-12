=============================================================================
       WINDOWS APPLICATION : CSTiffImageConverter Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary: 

This sample demonstrates how to convert JPEG images into TIFF images and vice 
versa. This sample also allows to create single multipage TIFF images from 
selected JPEG images.

TIFF (originally standing for Tagged Image File Format) is a flexible, 
adaptable file format for handling images and data within a single file, 
by including the header tags (size, definition, image-data arrangement, 
applied image compression) defining the image's geometry. For example, a 
TIFF file can be a container holding compressed (lossy) JPEG and (lossless) 
PackBits compressed images. A TIFF file also can include a vector-based 
clipping path (outlines, croppings, image frames). 


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the TIFF image converter
sample.

Step 1: Build and run the sample solution in Visual Studio 2010

Step 2: Check the checkbox "check to create multipage tiff (single) file" if 
multipage tiff file is to be created.

// Jpeg -> Tiff 
Step 3: Click on the "Convert to Tiff" button to browse the jpeg images. 
(Multiselect supported.)

Step 4: Click "Ok" after selecting the jpeg images, which will initiate the 
conversion process.

// Tiff -> Jpeg
Step 5: Click on the "Convert to Jpeg" button to browse the tiff image.

Step 6: Click "Ok" after selecting the tiff image, which will initiate the 
conversion process.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Converting TIFF to JPEG
(See: TiffImageConverter.ConvertTiffToJpeg)

The basic code logic is as follows:

  1. load the TIFF image with Image
  2. get the number of frames in the TIFF image.
  3. select each frame, and save it as a new jpg image file.

        public static string[] ConvertTiffToJpeg(string fileName)
        {
            using (Image imageFile = Image.FromFile(fileName))
            {
                FrameDimension frameDimensions = new FrameDimension(
                    imageFile.FrameDimensionsList[0]);

                // Gets the number of pages from the tiff image (if multipage)
                int frameNum = imageFile.GetFrameCount(frameDimensions);
                string[] jpegPaths = new string[frameNum];

                for (int frame = 0; frame < frameNum; frame++)
                {
                    // Selects one frame at a time and save as jpeg.
                    imageFile.SelectActiveFrame(frameDimensions, frame);
                    using (Bitmap bmp = new Bitmap(imageFile))
                    {
                        jpegPaths[frame] = String.Format("{0}\\{1}{2}.jpg", 
                            Path.GetDirectoryName(fileName),
                            Path.GetFileNameWithoutExtension(fileName), 
                            frame);
                        bmp.Save(jpegPaths[frame], ImageFormat.Jpeg);
                    }
                }

                return jpegPaths;
            }
        }

B. Converting JPEG(s) to TIFF
(See: TiffImageConverter.ConvertJpegToTiff)

The basic code logic is as follows:

  1. if user checked "check to create multipage tiff (single) file".
    1) initialize the first frame of the multipage tiff using the first 
       selected jpeg file.
    2) add additional frames from the rest jpeg files.
    3) when it is the last frame, flush the resources and close it.
  2. if user did not check "check to create multipage tiff (single) file"
    1) load each jpeg file
    2) save it as a single-frame tiff file.

        public static string[] ConvertJpegToTiff(string[] fileNames, bool isMultipage)
        {
            EncoderParameters encoderParams = new EncoderParameters(1);
            ImageCodecInfo tiffCodecInfo = ImageCodecInfo.GetImageEncoders()
                .First(ie => ie.MimeType == "image/tiff");

            string[] tiffPaths = null; 
            if (isMultipage)
            {
                tiffPaths = new string[1];
                Image tiffImg = null;
                try
                {
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        if (i == 0)
                        {
                            tiffPaths[i] = String.Format("{0}\\{1}.tif",
                                Path.GetDirectoryName(fileNames[i]),
                                Path.GetFileNameWithoutExtension(fileNames[i]));

                            // Initialize the first frame of multipage tiff.
                            tiffImg = Image.FromFile(fileNames[i]);
                            encoderParams.Param[0] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                            tiffImg.Save(tiffPaths[i], tiffCodecInfo, encoderParams);
                        }
                        else
                        {
                            // Add additional frames.
                            encoderParams.Param[0] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                            using (Image frame = Image.FromFile(fileNames[i]))
                            {
                                tiffImg.SaveAdd(frame, encoderParams);
                            }
                        }

                        if (i == fileNames.Length - 1)
                        {
                            // When it is the last frame, flush the resources and closing.
                            encoderParams.Param[0] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.Flush);
                            tiffImg.SaveAdd(encoderParams);
                        }
                    }
                }
                finally
                {
                    if (tiffImg != null)
                    {
                        tiffImg.Dispose();
                        tiffImg = null;
                    }
                }
            }
            else
            {
                tiffPaths = new string[fileNames.Length];

                for (int i = 0; i < fileNames.Length; i++)
                {
                    tiffPaths[i] = String.Format("{0}\\{1}.tif",
                        Path.GetDirectoryName(fileNames[i]),
                        Path.GetFileNameWithoutExtension(fileNames[i]));

                    // Save as individual tiff files.
                    using (Image tiffImg = Image.FromFile(fileNames[i]))
                    {
                        tiffImg.Save(tiffPaths[i], ImageFormat.Tiff);
                    }
                }
            }

            return tiffPaths;
        }

/////////////////////////////////////////////////////////////////////////////
References:

Tagged Image File Format
http://en.wikipedia.org/wiki/Tagged_Image_File_Format


/////////////////////////////////////////////////////////////////////////////