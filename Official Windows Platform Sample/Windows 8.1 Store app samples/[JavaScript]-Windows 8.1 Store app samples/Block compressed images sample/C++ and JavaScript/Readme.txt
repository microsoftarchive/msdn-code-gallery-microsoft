=============================
About block compressed images
=============================

Block compression is a technique for reducing GPU memory consumption of image assets; see:
http://go.microsoft.com/fwlink/?LinkId=317333

Any Visual Studio C++ project may be used to generate block compressed images, by adding the
ImageContentTask build customization to the project; see:
http://go.microsoft.com/fwlink/?LinkId=317334

Windows Store JavaScript apps support DDS files using the following block compression formats:
- BC1_UNORM (for opaque or 1 bit transparency images)
- BC2_UNORM (for images with transparency)
- BC3_UNORM (for images with transparency that is spatially correlated)

In addition, the following restrictions MUST be followed:
- Select "Yes" for the "Convert to pre-multiplied alpha format" option in Visual Studio.
- Select "No" for the "Generate Mips" option in Visual Studio.
- The image must be a multiple of 4 pixels in width and height.

======================
How to use this sample
======================

The BlockCompressedImages JavaScript project uses block compressed (DDS) images that are generated
by the ImageContentPipeline C++ project.

Before running BlockCompressedImages, you must first perform the following steps:
1. In Solution Explorer, right click on the ImageContentPipeline C++ project > Build.
   The ImageContentPipeline project has the ImageContentTask custom build step set up to convert
   guitar-transparent.png and oldWood4_nt.jpg (in the OriginalAssets folder) to block compressed (DDS)
   form in the BlockCompressedAssets folder.

1a.You can view the ImageContentTask settings by right clicking on one of the images > Properties.

2. In Solution Explorer, right click on the BlockCompressedAssets folder > Add > Existing Item.

3. Navigate to the BlockCompressedAssets folder and add the two DDS images.
   (guitar-transparent.dds and oldWood4_nt.dds)

==========================================
Notes for generating your own image assets
==========================================

The ImageContentPipeline project has been customized to make DDS asset creation more convenient:
- By default, any new images that are set as Item Type = Image Content Pipeline will use
  premultiplied alpha and will not generate mipmaps. This matches the requirements of Windows Store JavaScript apps.
  NOTE: You still must select a valid compression mode (BC1_UNORM, BC2_UNORM, or BC3_UNORM) and
  ensure that your image is a multiple of 4 pixels in width and height.
- The project output directory is set to BlockCompressedAssets regardless of platform or configuration.