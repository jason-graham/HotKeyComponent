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
    using System.Runtime.InteropServices.Enums;

    public static partial class NativeMethods
    {
        /// <summary>
        /// Retrieves or sets the value of one of the system-wide parameters. This function can also update the user profile while setting a parameter. see http://msdn.microsoft.com/en-us/library/ms724947(VS.85).aspx
        /// </summary>
        /// <param name="action">The system-wide parameter to be retrieved or set.</param>
        /// <param name="param">A parameter whose usage and format depends on the system parameter being queried or set. For more information about system-wide parameters, see the uiAction parameter. If not otherwise indicated, you must specify zero for this parameter.</param>
        /// <param name="vparam">A parameter whose usage and format depends on the system parameter being queried or set. For more information about system-wide parameters, see the uiAction parameter. If not otherwise indicated, you must specify null for this parameter.</param>
        /// <param name="init">If a system parameter is being set, specifies whether the user profile is to be updated, and if so, whether the WM_SETTINGCHANGE message is to be broadcast to all top-level windows to notify them of the change. </param>
        /// <returns>If the function succeeds, the return value is a nonzero value.</returns>
        [DllImport(USER32, EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfo(SPICommands action, uint param, ref uint vparam, uint init);
    }
}
