//---------------------------------------------------------------------------- 
//
//  Copyright (C) CSharp Labs.  All rights reserved.
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
    using System.Security.Permissions;

    /// <summary>
    /// The <see cref="NativeHotKeySink"/> class is a target for hot key notifications.
    /// </summary>
    public sealed class NativeHotKeySink : NativeWindow, IDisposable
    {
        /// <summary>
        /// Event raised when an hot key is pressed.
        /// </summary>
        public event EventHandler<HotKeyPressedEventArgs> HotKeyPressed;

        /// <summary>
        /// Initializes the <see cref="NativeHotKeySink"/> class.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public NativeHotKeySink()
        {
            CreateHandle(new CreateParams());
        }

        /// <summary>
        /// This method is called when a window message is sent to the handle of the window.
        /// </summary>
        /// <param name="m">The Windows <see cref="System.Windows.Forms.Message"/> to process.</param>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WindowsMessages.WM_HOTKEY)
            {
                if (HotKeyPressed != null)
                    HotKeyPressed(this, new HotKeyPressedEventArgs(m.WParam.ToInt32()));
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Disposes the hot key sink.
        /// </summary>
        ~NativeHotKeySink()
        {
            Disposing(false);
        }

        /// <summary>
        /// Disposes the hot key sink.
        /// </summary>
        public void Dispose()
        {
            Disposing(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes unmanaged and optionally, managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private void Disposing(bool disposing)
        {
            if (disposing)
                if (Handle != IntPtr.Zero)
                    DestroyHandle();
        }
    }
}
