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

namespace System.Runtime.InteropServices.Enums
{
    [Flags]
    public enum KeyModifiers : int
    {
        NONE = 0x0,
        /// <summary>
        /// Either ALT key must be held down.
        /// </summary>
        MOD_ALT = 0x0001,
        /// <summary>
        /// Either CTRL key must be held down.
        /// </summary>
        MOD_CONTROL = 0x0002,
        /// <summary>
        /// Either SHIFT key must be held down.
        /// </summary>
        MOD_SHIFT = 0x0004,
        /// <summary>
        /// Either WINDOWS key was held down. These keys are labeled with the Windows logo. Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system.
        /// </summary>
        MOD_WINKEY = 0x0008,
        /// <summary>
        /// Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications. 
        /// </summary>
        /// <remarks>Windows Vista and Windows XP/2000:  This flag is not supported.</remarks>
        MOD_NOREPEAT = 0x4000
    }
}
