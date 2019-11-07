=============================================================================
              Windows APPLICATION: CSSoftKeyboard Overview                        
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The sample demonstrates how to create a soft keyboard. It has the following 
features

1. It will not get focus when a key button clicked.

2. If the user presses the left mouse button within its nonclient area(such as the
   title bar), it will be activated. When the left mouse button is released, it will
   activate the previous foreground Window.

3 When user clicks a charactor on it, like "A" or "1", it will send the key to 
  the active application.

4 It supports special keys, like "WinKey" "Delete".

5 It supports the combination of keys, like "Ctrl+C".

NOTE: Ctrl+Alt+Del is not supported as it will cause security issue. 

/////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Build this project in VS2010. 

Step2. Open NotePad.exe and then run CSSoftKeyboard.exe. Make sure NotePad.exe 
       is the active application.


Demo a normal key button.

Step3. Click the "a" key on the keyboard, you will see that a letter "a" appears in the 
       NotePad.exe.


Demo a lock key button.

Step4. Click the "Caps" key, you will see the background of the key is changed to white.
       And the text of all the letter keys will be changed to UpperCase. 

Step5. Click the "a" key on the keyboard, you will see that a letter "A" appears in the 
       NotePad.exe.

Step6. Click the "Caps" key, you will see the background of the key is changed to black again.
       And the text of all the letter keys will be changed to LowerCase. 


Demo the Win key.

Step7. Click the "Win" key, you will see the background of the key is changed to white.

Step8. Click the "Win" key again, you will see the background of the key is changed to black again.
       And the Start menu is opened.


Demo shift key.

Step9. Click the left "Shift" key, you will see the background of this key and the right
       "Shift" key are changed to white. And the keys that support Shift will display the shift
	   text. For example, the key "=" will display as "+".

Step10. Click the key "+" on the keyboard, you will see that a letter "+" appears in the 
        NotePad.exe. 
	   
	    The background of the left and right "Shift" keys will be changed to black, and the keys
	    that support Shift will display the normal text. 


Demo the combination of keys.

Step11. Click the left "Ctrl" key, you will see the background of this key and the right
        "Ctrl" key are changed to white.

Step12. Click the "s" key, you will see that NotePad.exe will show a Save File dialog. 

/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Design a NoActivateWindow class to be inherited by the main KeyBoardForm.

   The NoActivateWindow class represents a form that will not be activated until the user
   presses the left mouse button within its nonclient area(such as the title bar, menu bar, 
   or window frame). When the left mouse button is released, this window will activate the
   previous foreground Window.

        /// <summary>
        /// Set the form style to WS_EX_NOACTIVATE so that it will not get focus. 
        /// </summary>
        protected override CreateParams CreateParams
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCLBUTTONDOWN:

                    // Get the current foreground window.
                    var foregroundWindow = UnsafeNativeMethods.GetForegroundWindow();

                    // If this window is not the current foreground window, then activate
                    // itself.
                    if (foregroundWindow != this.Handle)
                    {
                        UnsafeNativeMethods.SetForegroundWindow(this.Handle);

                        // Store the handle of previous foreground window.
                        if (foregroundWindow != IntPtr.Zero)
                        {
                            previousForegroundWindow = foregroundWindow;
                        }
                    }

                    break;
                case WM_NCMOUSEMOVE:

                    // Determine whether previous window still exist. If yes, then 
                    // activate it.
                    // Note: There is a scenario that the previous window is closed, but 
                    //       the same handle is assgined to a new window.
                    if ( UnsafeNativeMethods.IsWindow(previousForegroundWindow))
                    {
                        UnsafeNativeMethods.SetForegroundWindow(previousForegroundWindow);
                    }

                    break;
                default:
                    break;
            }

            base.WndProc(ref m);
        }

2. The KeyboardInput class wraps the SendInput method in User32.dll and supplies SendKey method to 
   simulate a normal key press event. It also supports the combination of keys, like "Ctrl+C".

   There are 3 scenarios:
   2.1. A single key is pressed, such as "A".
        
                var inputs = new NativeMethods.INPUT[1];
                inputs[0].type = NativeMethods.INPUT_KEYBOARD;
                inputs[0].inputUnion.ki.wVk = (short)key;
                UnsafeNativeMethods.SendInput(1, inputs, Marshal.SizeOf(inputs[0]));

   2.2. A key with modifier keys is pressed, such as "Ctrl+A".

                // To simulate this scenario, the inputs contains the toggling 
                // modifier keys, pressing the key and releasing modifier keys events.
                //
                // For example, to simulate Ctrl+C, we have to send 3 inputs:
                // 1. Ctrl is pressed.
                // 2. C is pressed. 
                // 3. Ctrl is released.
                var inputs = new NativeMethods.INPUT[modifierKeys.Count()*2 + 1];
                
                int i = 0;

                // Simulate the toggling the modifier keys.
                foreach (var modifierKey in modifierKeys)
                {
                    inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                    inputs[i].inputUnion.ki.wVk = (short)modifierKey;
                    i++;
                }

                // Simulate pressing the key.
                inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                inputs[i].inputUnion.ki.wVk = (short)key;
                i++;

                // Simulate releasing the modifier keys.
                foreach (var modifierKey in modifierKeys)
                {
                    inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                    inputs[i].inputUnion.ki.wVk = (short)modifierKey; 
                    inputs[i].inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;
                    i++;
                }
                
                UnsafeNativeMethods.SendInput((uint)inputs.Length, inputs,
                    Marshal.SizeOf(inputs[0]));

   2.3. A key that could be toggled is pressed, such as Caps Lock, Num Lock or
        Scroll Lock. 

                var inputs = new NativeMethods.INPUT[2];
             
                // Press the key.
                inputs[0].type = NativeMethods.INPUT_KEYBOARD;
                inputs[0].inputUnion.ki.wVk = (short)key;
             
                // Release the key.
                inputs[1].type = NativeMethods.INPUT_KEYBOARD;
                inputs[1].inputUnion.ki.wVk = (short)key;
                inputs[1].inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;
             
                UnsafeNativeMethods.SendInput(2, inputs, Marshal.SizeOf(inputs[0]));

       
3. The KeyBoardForm class is the main form of the keyboard. When the form is 
   being loaded, it will load the KeysMapping.xml to initialize the keyboard buttons. 


/////////////////////////////////////////////////////////////////////////////
References:

SendInput Function
http://msdn.microsoft.com/en-us/library/ms646310(VS.85).aspx

GetKeyState Function
http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx

Control.CreateParams Property
http://msdn.microsoft.com/en-us/library/system.windows.forms.control.createparams.aspx

GetForegroundWindow Function
http://msdn.microsoft.com/en-us/library/ms633505(VS.85).aspx

SetForegroundWindow Function
http://msdn.microsoft.com/en-us/library/ms633539(VS.85).aspx


/////////////////////////////////////////////////////////////////////////////
