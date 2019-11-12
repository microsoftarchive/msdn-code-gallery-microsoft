/****************************** Module Header ******************************\
 * Module Name:  KeyBoardForm.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the main form that represents a soft keyboard. When the form is 
 * being loaded, it will load the KeysMapping.xml to initialize the keyboard
 * buttons. 
 * 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using CSSoftKeyboard.Controls;

namespace CSSoftKeyboard
{
    public partial class KeyBoardForm : CSSoftKeyboard.NoActivate.NoActivateWindow
    {

        IEnumerable<KeyButton> keyButtonList = null;
        IEnumerable<KeyButton> KeyButtonList
        {
            get
            {
                if (keyButtonList == null)
                {
                    keyButtonList = this.Controls.OfType<KeyButton>();
                }
                return keyButtonList;
            }
        }


        List<int> pressedModifierKeyCodes = null;

        /// <summary>
        /// The pressed modifier keys.
        /// </summary>
        List<int> PressedModifierKeyCodes
        {
            get
            {
                if (pressedModifierKeyCodes == null)
                {
                    pressedModifierKeyCodes = new List<int>();
                }
                return pressedModifierKeyCodes;
            }
        }


        public KeyBoardForm()
        {
            InitializeComponent();
        }

        #region Handle key board event

        private void KeyBoardForm_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeKeyButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Register the key button click event.
            foreach (KeyButton btn in this.KeyButtonList)
            {
                btn.Click += new EventHandler(KeyButton_Click);
            }

            this.Activate();
        }

        /// <summary>
        /// Handle the key button click event.
        /// </summary>
        void KeyButton_Click(object sender, EventArgs e)
        {
            KeyButton btn = sender as KeyButton;
            if (btn == null)
            {
                return;
            }

            // Synchronize the key pairs, like LShiftKey and RShiftKey.
            SyncKeyPairs(btn);

            // Process the special key such as AppsKey.
            if (ProcessSpecialKey(btn))
            {
                return;
            }

            // Update the text of key buttons if the NumLock, ShiftKey or CapsLock is pressed.
            if (btn.Key == Keys.NumLock || btn.Key == Keys.ShiftKey || btn.Key == Keys.CapsLock)
            {
                UpdateKeyButtonsText(keyButtonLShift.IsPressed, keyButtonNumLock.IsPressed, keyButtonCapsLock.IsPressed);
            }

            // The CapsLock, NumLock or ScrollLock key is pressed.
            if (btn.IsLockKey)
            {
                UserInteraction.KeyboardInput.SendToggledKey(btn.KeyCode);
            }

            // A modifier key is pressed. 
            else if (btn.IsModifierKey)
            {
                // The modifier key is pressed twice.
                if (PressedModifierKeyCodes.Contains(btn.KeyCode))
                {
                    UserInteraction.KeyboardInput.SendToggledKey(btn.KeyCode);

                    // Clear the pressed state of all the modifier key buttons.
                    ResetModifierKeyButtons();
                }
                else
                {
                    PressedModifierKeyCodes.Add(btn.KeyCode);
                }
            }

            // A normal key is pressed.
            else
            {
                int btnKeyCode = btn.KeyCode;

                // If the key is a number pad key and the NumLock is not pressed, then use the 
                // UnNumLockKeyCode.
                if (btn.IsNumberPadKey && !keyButtonNumLock.IsPressed && btn.UnNumLockKeyCode > 0)
                {
                    btnKeyCode = btn.UnNumLockKeyCode;
                }

                UserInteraction.KeyboardInput.SendKey(PressedModifierKeyCodes, btnKeyCode);

                // Clear the pressed state of all the modifier key buttons.
                ResetModifierKeyButtons();
            }
        }

        /// <summary>
        /// Synchronize the key pairs, like LShiftKey and RShiftKey.
        /// </summary>
        void SyncKeyPairs(KeyButton btn)
        {
            if (btn == keyButtonLShift)
            {
                keyButtonRShift.IsPressed = keyButtonLShift.IsPressed;
            }
            if (btn == keyButtonRShift)
            {
                keyButtonLShift.IsPressed = keyButtonRShift.IsPressed;
            }

            if (btn == keyButtonLAlt)
            {
                keyButtonRAlt.IsPressed = keyButtonLAlt.IsPressed;
            }
            if (btn == keyButtonRAlt)
            {
                keyButtonLAlt.IsPressed = keyButtonRAlt.IsPressed;
            }

            if (btn == keyButtonLControl)
            {
                keyButtonRControl.IsPressed = keyButtonLControl.IsPressed;
            }
            if (btn == keyButtonRControl)
            {
                keyButtonLControl.IsPressed = keyButtonRControl.IsPressed;
            }
        }

        /// <summary>
        /// Process the special key such as AppsKey.
        /// </summary>
        bool ProcessSpecialKey(KeyButton btn)
        {
            bool handled = true;
            switch (btn.Key)
            {

                // Use Shift+F10 to simulate the Apps key. 
                case Keys.Apps:
                    UserInteraction.KeyboardInput.SendKey(
                                        new int[] { (int)Keys.ShiftKey },
                                        (int)Keys.F10);
                    break;
                default:
                    handled = false;
                    break;
            }
            return handled;
        }

        /// <summary>
        /// Initialize the key buttons.
        /// </summary>
        void InitializeKeyButtons()
        {
            short capsLockState = UserInteraction.UnsafeNativeMethods.GetKeyState((int)Keys.CapsLock);
            keyButtonCapsLock.IsPressed = (capsLockState & 0x0001) != 0;

            short numLockState = UserInteraction.UnsafeNativeMethods.GetKeyState((int)Keys.NumLock);
            keyButtonNumLock.IsPressed = (numLockState & 0x0001) != 0;


            short scrLockState = UserInteraction.UnsafeNativeMethods.GetKeyState((int)Keys.Scroll);
            keyButtonScrollLock.IsPressed = (scrLockState & 0x0001) != 0;

            var keysMappingDoc = XDocument.Load("Resources\\KeysMapping.xml");
            foreach (var key in keysMappingDoc.Root.Elements())
            {
                int keyCode = int.Parse(key.Element("KeyCode").Value);

                IEnumerable<KeyButton> btns = KeyButtonList.Where(btn => btn.KeyCode == keyCode);

                foreach (KeyButton btn in btns)
                {
                    btn.NormalText = key.Element("NormalText").Value;

                    if (key.Elements("ShiftText").Count() > 0)
                    {
                        btn.ShiftText = key.Element("ShiftText").Value;
                    }

                    if (key.Elements("UnNumLockText").Count() > 0)
                    {
                        btn.UnNumLockText = key.Element("UnNumLockText").Value;
                    }

                    if (key.Elements("UnNumLockKeyCode").Count() > 0)
                    {
                        btn.UnNumLockKeyCode =
                            int.Parse(key.Element("UnNumLockKeyCode").Value);
                    }

                    btn.UpdateDisplayText(false, keyButtonNumLock.IsPressed, keyButtonCapsLock.IsPressed);
                }
            }
        }

        /// <summary>
        /// Update the text of the key.
        /// </summary>
        void UpdateKeyButtonsText(bool isShiftKeyPressed, bool isNumLockPressed, bool isCapsLockPressed)
        {
            foreach (var btn in this.KeyButtonList)
            {
                btn.UpdateDisplayText(isShiftKeyPressed, isNumLockPressed, isCapsLockPressed);
            }
        }

        /// <summary>
        /// Clear the pressed state of all the modifier key buttons.
        /// </summary>
        void ResetModifierKeyButtons()
        {
            PressedModifierKeyCodes.Clear();

            keyButtonLShift.IsPressed = false;
            keyButtonRShift.IsPressed = false;
            keyButtonRControl.IsPressed = false;
            keyButtonLControl.IsPressed = false;
            keyButtonRAlt.IsPressed = false;
            keyButtonLAlt.IsPressed = false;
            keyButtonWin.IsPressed = false;

            foreach (var keybtn in KeyButtonList)
            {
                keybtn.UpdateDisplayText(false, keyButtonNumLock.IsPressed, keyButtonCapsLock.IsPressed);
            }
        }

        #endregion

    }
}
