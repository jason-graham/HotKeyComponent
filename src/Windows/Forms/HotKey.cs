//---------------------------------------------------------------------------- 
//
//  Copyright (C) Jason Graham.  All rights reserved.
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// 
// History
//  06/20/13    Created 
//
//---------------------------------------------------------------------------

namespace System.Windows.Forms
{
    using System.Runtime.InteropServices.Enums;

    /// <summary>
    /// The HotKey structure represents a key and modifier.
    /// </summary>
    public struct HotKey
    {
        #region Fields
        /// <summary>
        /// Defines the key to press.
        /// </summary>
        private Keys key;
        /// <summary>
        /// Defines key modifiers.
        /// </summary>
        private Keys modifier;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Keys Key
        {
            get
            {
                return key;
            }
            private set
            {
                key = value;
            }
        }

        /// <summary>
        /// Gets or sets the modifier.
        /// </summary>
        public Keys Modifier
        {
            get
            {
                return modifier;
            }
            private set
            {
                if (modifier != value)
                {
                    modifier = Keys.None;

                    if (HasControlFlag(value))
                        modifier |= Keys.Control;

                    if (HasShiftFlag(value, true))
                        modifier |= Keys.Shift;

                    if (HasAltFlag(value))
                        modifier |= Keys.Alt;
                }
            }
        }

        /// <summary>
        /// Gets or sets the modifier as a native modifier.
        /// </summary>
        public KeyModifiers NativeModifier
        {
            get
            {
                KeyModifiers native = KeyModifiers.NONE;

                if (modifier.HasFlag(Keys.Control))
                    native |= KeyModifiers.MOD_CONTROL;

                if (modifier.HasFlag(Keys.Shift))
                    native |= KeyModifiers.MOD_SHIFT;

                if (modifier.HasFlag(Keys.Alt))
                    native |= KeyModifiers.MOD_ALT;

                return native;
            }
            private set
            {
                modifier = Keys.None;

                if (value.HasFlag(KeyModifiers.MOD_CONTROL))
                    modifier |= Keys.Control;

                if (value.HasFlag(KeyModifiers.MOD_SHIFT))
                    modifier |= Keys.Shift;

                if (value.HasFlag(KeyModifiers.MOD_ALT))
                    modifier |= Keys.Alt;
            }
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> contains a control flag.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>true if any control flag is set; otherwise, false.</returns>
        private bool HasControlFlag(Keys value)
        {
            return value.HasFlag(Keys.Control) || value.HasFlag(Keys.ControlKey) || value.HasFlag(Keys.LControlKey) || value.HasFlag(Keys.RControlKey);
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> contains a shift flag.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="shiftKey">Determines if the <see cref="Keys.ShiftKey"/> flag should be checked.</param>
        /// <returns>true if any shift flag is set; otherwise, false.</returns>
        private bool HasShiftFlag(Keys value, bool shiftKey)
        {
            return value.HasFlag(Keys.Shift) || value.HasFlag(Keys.LShiftKey) || value.HasFlag(Keys.RShiftKey) || (shiftKey && value.HasFlag(Keys.ShiftKey));
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> contains an alt flag.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>true if any alt flag is set; otherwise, false.</returns>
        private bool HasAltFlag(Keys value)
        {
            return value.HasFlag(Keys.Alt) || value.HasFlag(Keys.Menu) || value.HasFlag(Keys.LMenu) || value.HasFlag(Keys.RMenu);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the hot key with a key and modifier.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifier">The modifier.</param>
        public HotKey(Keys key, Keys modifier)
            : this()
        {
            if (HasControlFlag(key))
                throw new ArgumentException("Cannot specify control flag in key parameter.", "key");

            if (HasShiftFlag(key, false))
                throw new ArgumentException("Cannot specify shift flag in key parameter.", "key");

            if (HasAltFlag(key))
                throw new ArgumentException("Cannot specify alt flag in key parameter.", "key");

            Key = key;
            Modifier = modifier;
        }

        /// <summary>
        /// Initializes the hot key with a key.
        /// </summary>
        /// <param name="key">The key.</param>
        public HotKey(Keys key)
            : this(key, Keys.None)
        { }
        #endregion

        #region Operator Overrides
        /// <summary>
        /// Compares hotkeys and returns a value indicating their equality.
        /// </summary>
        /// <param name="left">First hot key.</param>
        /// <param name="right">Second hot key</param>
        /// <returns>true if both have same key and modifier; otherwise false.</returns>
        public static bool operator ==(HotKey left, HotKey right)
        {
            return left.Key == right.Key && left.Modifier == right.Modifier;
        }

        /// <summary>
        /// Compares hotkeys and returns a value indicating their inverse-equality.
        /// </summary>
        /// <param name="left">First hot key.</param>
        /// <param name="right">Second hot key</param>
        /// <returns>false if both hotkeys have the same key and modifier; otherwise true.</returns>
        public static bool operator !=(HotKey left, HotKey right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines if the specified object is equal to this.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>true if the objects are both hotkeys and have the same values.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HotKey))
                return false;

            return (HotKey)obj == this;
        }

        /// <summary>
        /// Returns a hash code for the key and modifier.
        /// </summary>
        /// <returns>A key and modifier hash code.</returns>
        public override int GetHashCode()
        {
            int hash = 71;
            hash = hash * 83 + key.GetHashCode();
            hash = hash * 83 + modifier.GetHashCode();
            return hash;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns a string which represents the hot key.
        /// </summary>
        /// <returns>A string which represents the hot key.</returns>
        public override string ToString()
        {
            if (key == Keys.None)
            {
                if (modifier == Keys.None)
                    return "None";
                else
                    return modifier.ToString();
            }
            else
            {
                if (modifier == Keys.None)
                    return key.ToString();
                else
                    return String.Format("{0} + {1}", modifier, key);
            }
        }
        #endregion
    }
}
