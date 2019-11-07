/****************************** Module Header ******************************\
 * Module Name:  KeyButton.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * This control represents a key board button.
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
using System.Drawing;
using System.Windows.Forms;

namespace CSSoftKeyboard.Controls
{
    public partial class KeyButton : Button
    {

        // The default styles.
        static Color normalBackColor = Color.Black;
        static Color mouseOverBackColor = Color.DimGray;
        static Color pressedBackColor = Color.White;

        static Color normalLabelForeColor = Color.White;
        static Color pressedlLabelForeColor = Color.Black;
     
        /// <summary>
        /// The key code of this key.
        /// </summary>
        public int KeyCode { get; set; }

        Keys key;
        public Keys Key
        {
            get
            {
                if (key == Keys.None)
                {
                    key = (Keys)KeyCode;
                }

                return key;
            }
        }

        /// <summary>
        /// The key code of the number pad key if NumLock key is not pressed.
        /// </summary>
        public int UnNumLockKeyCode { get; set; }

        /// <summary>
        /// The text of the number pad key if NumLock key is not pressed.
        /// </summary>
        public string UnNumLockText { get; set; }

        /// <summary>
        /// The normal text of the key.
        /// </summary>
        public string NormalText { get; set; }

        /// <summary>
        /// The text of the key when the Shift key is pressed.
        /// </summary>
        public string ShiftText { get; set; }

        /// <summary>
        /// Specify whether it is a modifier key.
        /// </summary>
        public bool IsModifierKey
        {
            get
            {
                return Key == Keys.ControlKey
                     || Key == Keys.ShiftKey
                     || Key == Keys.Menu
                     || Key == Keys.LWin
                     || Key == Keys.RWin;
            }
        }

       
        /// <summary>
        /// Specify whether it is a lock key.
        /// </summary>
        public bool IsLockKey
        {
            get
            {
                return Key == Keys.Capital
                    || Key == Keys.Scroll
                    || Key == Keys.NumLock;
            }
        }

        /// <summary>
        /// Specify whether it is a letter key.
        /// </summary>
        public bool IsLetterKey
        {
            get
            {
                return Key >= Keys.A && Key <= Keys.Z;
            }
        }

        /// <summary>
        /// Specify whether it is a number pad key.
        /// </summary>
        public bool IsNumberPadKey
        {
            get
            {
                return Key >= Keys.NumPad0 && Key <= Keys.NumPad9;
            }
        }

        bool isPressed;

        /// <summary>
        /// Specify whether the key is pressed.
        /// </summary>
        public bool IsPressed
        {
            get
            {
                return isPressed;
            }
            set
            {
                if (isPressed != value)
                {
                    isPressed = value;

                    this.OnIsPressedChange(EventArgs.Empty);
                }
            }

        }

        bool isMouseOver;

        /// <summary>
        /// Specify whether the mouse is over this key.
        /// </summary>
        bool IsMouseOver
        {
            get
            {
                return isMouseOver;
            }
            set
            {
                if (isMouseOver != value)
                {
                    isMouseOver = value;

                    this.OnIsMouseOverChange(EventArgs.Empty);
                }
            }
        }

        public KeyButton()
        {
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackColor = System.Drawing.Color.Black;
            this.ForeColor = System.Drawing.Color.White;
        }

        /// <summary>
        /// Update the text of the key.
        /// </summary>
        public void UpdateDisplayText(bool isShiftKeyPressed, bool isNumLockPressed, bool isCapsLockPressed)
        {
            if (this.IsLetterKey)
            {
                this.Text = (isShiftKeyPressed ^ isCapsLockPressed) ? ShiftText : NormalText;
            }
            else if (!string.IsNullOrEmpty(this.ShiftText))
            {
                this.Text = isShiftKeyPressed ? ShiftText : NormalText;
            }
            else if (this.IsNumberPadKey)
            {
                this.Text = isNumLockPressed ? NormalText : UnNumLockText;
            }

        }

        #region Update the style of the key board button.

        /// <summary>
        /// Handle the MouseDown event.
        /// Change the value of the IsPressed property, will cause the button to
        /// refresh.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            IsPressed = !IsPressed;
        }

        /// <summary>
        /// Handle the MouseUp event.
        /// If the key is not a modifier key or a lock key, set the IsPressed property 
        /// to false, which makes the button refresh.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!IsModifierKey && !IsLockKey)
            {
                IsPressed = false;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            IsMouseOver = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseOver = false;
        }


        protected virtual void OnIsMouseOverChange(EventArgs e)
        {
            ReDrawKeyButton();
        }

        protected virtual void OnIsPressedChange(EventArgs e)
        {
            ReDrawKeyButton();
        }

        protected virtual void ReDrawKeyButton()
        {
            if (IsPressed)
            {
                this.BackColor = KeyButton.pressedBackColor;
                this.ForeColor = KeyButton.pressedlLabelForeColor;
            }
            else if (IsMouseOver)
            {
                this.BackColor = KeyButton.mouseOverBackColor;
                this.ForeColor = KeyButton.normalLabelForeColor;
            }
            else
            {
                this.BackColor = KeyButton.normalBackColor;
                this.ForeColor = KeyButton.normalLabelForeColor;
            }
        }

        #endregion
    }
}
