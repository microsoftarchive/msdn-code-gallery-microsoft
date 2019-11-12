=========================================================================================
    WINDOWS FORMS APPLICATION : VBWinFormExAeroToClient Project Overview
=========================================================================================

/////////////////////////////////////////////////////////////////////////////////////////
Summary:

VBWinFormExAeroToClient example demonstrates how to create a WinFrom application with
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

Step 2. Run VBWinFormExAeroToClient.exe

Step 3. You will see the main form. It will show whether the aero is enabled/supported.

Step 4. If the Aero is enabled, set the parameters and click the "Apply" button to show a 
        Demo Form with the Aero effect. 

        You can also change the parameters and click the "Apply" button to change the Aero
        effect of the Demo Form.

/////////////////////////////////////////////////////////////////////////////////////////
Code Logic:

A. Determine whether Aero effect is enabled.

            If Environment.OSVersion.Version.Major >= 6 Then
                ' Make sure the Glass is enabled by the user.
                NativeMethods.DwmIsCompositionEnabled(isDWMEnable)
            End If

B. Extend the frame to achieve the Aero effect.
   
   1. Extend the frame using the DwmExtendFrameIntoClientArea method. The margins parameter
      of this method defines the margins of the form, in other words, the width and height
      of the frame.
       
   2. Make the region in the margins transparent. 
   
   Because the region belongs to the frame now, so it will have the Aero effect.
      
          NativeMethods.DwmExtendFrameIntoClientArea(Me.Handle, glassMargins)
      
          ' Make the region in the margins transparent. 
          _marginRegion = New Region(Me.ClientRectangle)

          ' If the glassMargins contains a negative value, or the values are not valid,
          ' then make the whole form transparent.
          If Me.GlassMargins.IsNegativeOrOverride(Me.ClientSize) Then
              e.Graphics.FillRegion(transparentBrush, _marginRegion)

              ' By default, exlucde the region of the client area.
          Else
              _marginRegion.Exclude(New Rectangle(
                                    Me.GlassMargins.cxLeftWidth,
                                    Me.GlassMargins.cyTopHeight,
                                    Me.ClientSize.Width - Me.GlassMargins.cxLeftWidth - Me.GlassMargins.cxRightWidth,
                                    Me.ClientSize.Height - Me.GlassMargins.cyTopHeight - Me.GlassMargins.cyBottomHeight))
              e.Graphics.FillRegion(transparentBrush, _marginRegion)
          End If


C. Enable the blur effect on the form to achieve the Aero effect.

   1. Make the region transparent.
   
   2. Enable the blur effect on the form using the DwmEnableBlurBehindWindow method.

   Then the region will have a Aero effect.
   
      Dim bbh As New NativeMethods.DWM_BLURBEHIND()
      bbh.dwFlags = NativeMethods.DWM_BLURBEHIND.DWM_BB_ENABLE
      bbh.fEnable = True
      bbh.hRegionBlur = Me.BlurRegion.GetHrgn(graphics)
      NativeMethods.DwmEnableBlurBehindWindow(Me.Handle, bbh)

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