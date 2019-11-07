'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports Microsoft.Xna.Framework.Audio

Public Class CapabilityPlaceholder

    '  In Windows Phone applications that use the CaptureSource class, 
    '  you must also use the Microsoft.Devices.Camera, Microsoft.Devices.PhotoCamera, 
    '  or Microsoft.Xna.Framework.Audio.Microphone class to enable audio capture 
    '  and accurate capability detection in the application.

    '  Since this sample does not need any of these classes, this unused
    '  class prompts the Marketplace capability detection process to add the 
    '  ID_CAP_MICROPHONE capability to the application capabilities list upon ingestion. 

    '  For more information about capability detection, see: http://go.microsoft.com/fwlink/?LinkID=204620

    Dim unusedMic As Microphone = Nothing

    Private Function unusedMethod() As String
        Return unusedMic.ToString()
    End Function

End Class
