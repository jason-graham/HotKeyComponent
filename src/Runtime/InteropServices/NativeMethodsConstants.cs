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

namespace System.Runtime.InteropServices
{
    public static partial class NativeMethods
    {
        /// <summary>
        /// Defines the kernel32.dll library name.
        /// </summary>
        public const string KERNEL32 = "kernel32.dll";
        /// <summary>
        /// Defines the user32.dll library name.
        /// </summary>
        public const string USER32 = "user32.dll";
        /// <summary>
        /// Defines the gdi32.dll library name.
        /// </summary>
        public const string GDI32 = "gdi32.dll";
        /// <summary>
        /// Defines the ntdll.dll library name.
        /// </summary>
        public const string NTDLL = "ntdll.dll";
    }
}