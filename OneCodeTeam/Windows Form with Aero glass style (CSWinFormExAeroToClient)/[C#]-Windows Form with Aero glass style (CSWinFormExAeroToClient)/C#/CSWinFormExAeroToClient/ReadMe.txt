=========================================================================================
    WINDOWS FORMS APPLICATION : CSWinFormExAeroToClient Project Overview
=========================================================================================

/////////////////////////////////////////////////////////////////////////////////////////
Summary:

CSWinFormExAeroToClient example demonstrates how to create a WinFrom application with
Aero effect.

There are 2 approaches to achieve this effect.

1. The frame of a Form has the Aero effect, and then we can extend the Frame to client
   area using the DwmExtendFrameIntoClientArea method.

2. Set the client area of a Form to transparent, and enable the blur effect on this window
   using DwmEnableBlurBehindWindow method.

NOTE:
This appication must run on Windows Vista or later version, and the DWM composition is 
enabled.

/////////////////////////////////////////////////////////////////////////////////////////
Demo:


Step 1. Build this project in VS2010. 

Step 2. Run CSWinFormExAeroToClient.exe

Step 3. You will see the main form. It will show whether the aero is enabled/supported.

Step 4. If the Aero is enabled, set the parameters and click the "Apply" button to show a 
        Demo Form with the Aero effect. 

        You can also change the parameters and click the "Apply" button to change the Aero
        effect of the Demo Form.

/////////////////////////////////////////////////////////////////////////////////////////
Code Logic:

A. Determine whether Aero effect is enabled.

     if (System.Environment.OSVersion.Version.Major >= 6)
     {
         NativeMethods.DwmIsCompositionEnabled(out isDWMEnable);
     }

B. Extend the frame to achieve the Aero effect.
   
   1. Extend the frame using the DwmExtendFrameIntoClientArea method. The margins parameter
      of this method defines the margins of the form, in other words, the width and height
      of the frame.
       
   2. Make the region in the margins transparent. 
   
   Because the region belongs to the frame now, so it will have the Aero effect.
      
          NativeMethods.DwmExtendFrameIntoClientArea(this.Handle, ref glassMargins);
      
          marginRegion = new Region(this.ClientRectangle);

          if (this.GlassMargins.IsNegativeOrOverride(this.ClientSize))
          {
              e.Graphics.FillRegion(transparentBrush, marginRegion);
          }
          else
          {
              marginRegion.Exclude(new Rectangle(
                  this.GlassMargins.cxLeftWidth,
                  this.GlassMargins.cyTopHeight,
                  this.ClientSize.Width - this.GlassMargins.cxLeftWidth - this.GlassMargins.cxRightWidth,
                  this.ClientSize.Height - this.GlassMargins.cyTopHeight - this.GlassMargins.cyBottomHeight));
              e.Graphics.FillRegion(transparentBrush, marginRegion);
          }


C. Enable the blur effect on the form to achieve the Aero effect.

   1. Make the region transparent.
   
   2. Enable the blur effect on the form using the DwmEnableBlurBehindWindow method.

   Then the region will have a Aero effect.
   
      NativeMethods.DWM_BLURBEHIND bbh = new NativeMethods.DWM_BLURBEHIND();
      bbh.dwFlags = NativeMethods.DWM_BLURBEHIND.DWM_BB_ENABLE;
      bbh.fEnable = true;
      bbh.hRegionBlur = this.BlurRegion.GetHrgn(graphics);
      NativeMethods.DwmEnableBlurBehindWindow(this.Handle, bbh);

D. The GlassForm supplies following properties to set the Aero effect.
       EnableExtendFrame
       GlassMargins
       EnableBlurBehindWindow 
       BlurRegion

E. The MainForm sets the properties of a GlassForm instance to demo the Aero effect. 

/////////////////////////////////////////////////////////////////////////////////////////
References:

DwmEnableBlurBehindWindow Function
http://msdn.microsoft.com/en-us/library/aa969508(VS.85).aspx

DwmExtendFrameIntoClientArea Function
http://msdn.microsoft.com/en-us/library/aa969512(VS.85).aspx

DwmIsCompositionEnabled Function
http://msdn.microsoft.com/en-us/library/aa969518(VS.85).aspx

Windows Vista for Developers – Part 3 – The Desktop Window Manager
http://weblogs.asp.net/kennykerr/archive/2006/08/10/Windows-Vista-for-Developers-_1320_-Part-3-_1320_-The-Desktop-Window-Manager.aspx

Create Special Effects With The Desktop Window Manager
http://msdn.microsoft.com/en-us/magazine/cc163435.aspx

DWM Functions
http://msdn.microsoft.com/en-us/library/aa969527(v=VS.85).aspx
/////////////////////////////////////////////////////////////////////////////////////////