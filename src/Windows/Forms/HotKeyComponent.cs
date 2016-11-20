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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.Enums;
    using System.Security.Permissions;
    using System.Threading;

    [DefaultEvent("HotKeyActivated")]
    public partial class HotKeyComponent : Component
    {
        #region InternalHotKeyState Class
        /// <summary>
        /// Defines properties identifying the hot key, ID, timestamp and callback.
        /// </summary>
        private sealed class InternalHotKeyState : HotKeyState
        {
            /// <summary>
            /// Defines a callback to the a method.
            /// </summary>
            public EventHandler<HotKeyActivatedEventArgs> Callback { get; private set; }
            /// <summary>
            /// Defines the hot key id.
            /// </summary>
            public int ID { get; private set; }
            /// <summary>
            /// Defines the last time the hot key was pressed.
            /// </summary>
            public DateTime TimeStamp { get; set; }

            #region Constructors
            /// <summary>
            /// Initializes the hot key state with an owner, id, key, modifier, repeat options, state and callback.
            /// </summary>
            /// <param name="owner">The owner of the state.</param>
            /// <param name="id">The hot key id.</param>
            /// <param name="key">The hot key.</param>
            /// <param name="allow_repeat">true to allow repeating; otherwise false.</param>
            /// <param name="state">The state of the hot key.</param>
            /// <param name="callback">A callback to notify when the hot key is pressed.</param>
            public InternalHotKeyState(HotKeyComponent owner, int id, HotKey hotkey, bool allow_repeat, object state, EventHandler<HotKeyActivatedEventArgs> callback)
                : base(owner, hotkey, allow_repeat, state)
            {
                ID = id;
                Callback = callback;
                TimeStamp = DateTime.MinValue;
            }

            /// <summary>
            /// Initializes the hot key state with an owner, id, key, modifier, repeat options, state and callback.
            /// </summary>
            /// <param name="owner">The owner of the state.</param>
            /// <param name="id">The hot key id.</param>
            /// <param name="key">The hot key.</param>
            /// <param name="modifier">The hot key modifier.</param>
            /// <param name="allow_repeat">true to allow repeating; otherwise false.</param>
            /// <param name="state">The state of the hot key.</param>
            /// <param name="callback">A callback to notify when the hot key is pressed.</param>
            public InternalHotKeyState(HotKeyComponent owner, int id, Keys key, Keys modifier, bool allow_repeat, object state, EventHandler<HotKeyActivatedEventArgs> callback)
                : this(owner, id, new HotKey(key, modifier), allow_repeat, state, callback)
            {
            }
            #endregion
        }
        #endregion

        #region Constants
        /// <summary>
        /// Defines the maximum id number for RegisterHoeyKey.
        /// </summary>
        private const int MAXIMUM_REGISTERHOTKEY_ID = 0xBFFF; 
        #endregion

        #region Fields
        /// <summary>
        /// An event to handle when a hot key is activated.
        /// </summary>
        public event EventHandler<HotKeyActivatedEventArgs> HotKeyActivated;

        /// <summary>
        /// A dummy window to receive messages.
        /// </summary>
        private readonly NativeHotKeySink window = new NativeHotKeySink();
        /// <summary>
        /// A collection of hot keys with IDs as a key.
        /// </summary>
        private readonly Dictionary<int, InternalHotKeyState> localHotKeys = new Dictionary<int, InternalHotKeyState>();
        #endregion

        #region Static Fields
        /// <summary>
        /// Defines the last hot key id used or -1.
        /// </summary>
        private static int HotKeyID = -1;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new HotKeyComponent.
        /// </summary>
        public HotKeyComponent()
        {
            InitializeComponent();

            Init();
        }

        /// <summary>
        /// Creates a new <see cref="HotKeyComponent"/> with the specified container.
        /// </summary>
        /// <param name="container">An <see cref="System.ComponentModel.IContainer"/> that represents the container for the hot key component.</param>
        public HotKeyComponent(IContainer container)
        {
            container.Add(this);
            InitializeComponent();

            Init();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Returns the keyboard key repeat speed, which is a value from 0 - 31.
        /// </summary>
        /// <returns>An 32-bit unsigned integer value indicating the keyboard key repeat speed.</returns>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static uint GetKeyboardSpeed()
        {
            uint repeatDelay = 0;
            if (!NativeMethods.SystemParametersInfo(SPICommands.SPI_GETKEYBOARDSPEED, 0, ref repeatDelay, 0))
                return 31;

            return repeatDelay;
        }

        /// <summary>
        /// Returns the keyboard key repeat delay, which is a value from 0-3.
        /// </summary>
        /// <returns>An 32-bit unsigned integer value indicating the keyboard key repeat delay.</returns>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static uint GetKeyboardRepeatDelay()
        {
            uint delay = 0;
            if (!NativeMethods.SystemParametersInfo(SPICommands.SPI_GETKEYBOARDDELAY, 0, ref delay, 0))
                return 3;

            return delay;
        }

        /// <summary>
        /// Sets up the hot key sink and initializes synchronization control.
        /// </summary>
        private void Init()
        {
            //calculate the key repeat rate and repeat delay
            int repeatRate = (int)Math.Ceiling(1000M / (27.5M * (GetKeyboardSpeed() / 31) + 2.5M));
            int repeatDelay = (int)Math.Ceiling(750M * (GetKeyboardRepeatDelay() / 3M) + 250M);

            //account for lag:
            repeatRate += 100;
            repeatDelay += 100;

            //use the larger of the delays, unfortunately, there is no hot key released event
            int minimumInterval = Math.Max(repeatRate, repeatDelay);

            //handles hot key notifications
            window.HotKeyPressed += (sender, e) =>
            {
                InternalHotKeyState hotkey = null;

                if (localHotKeys.TryGetValue(e.HotKeyID, out hotkey))
                {
                    //utc timestamps for better performance
                    bool invoke = hotkey.AllowRepeat || DateTime.UtcNow.Subtract(hotkey.TimeStamp).TotalMilliseconds > minimumInterval;
                    hotkey.TimeStamp = DateTime.UtcNow; //update timestamp

                    if (invoke)
                    {
                        HotKeyActivatedEventArgs args = new HotKeyActivatedEventArgs(hotkey);

                        if (HotKeyActivated != null)
                            HotKeyActivated(this, args);

                        if (hotkey.Callback != null)
                            hotkey.Callback(this, args);
                    }
                }
            };
        }
        #endregion

        #region Add Hot Key
        /// <summary>
        /// Attempts to create a new system-wide hot key with repeat options.
        /// </summary>
        /// <param name="key">The hot key to register.</param>
        /// <param name="modifier">The key modifier to associate with the key.</param>
        /// <param name="allow_repeat">true to allow repeating key events; otherwise, false.</param>
        /// <param name="result">When this method returns, contains an instance of a new state representing the hot key registration or null if the registration failed.</param>
        /// <returns>true if a new system-wide hot key was registered; otherwise, false.</returns>
        public bool TryAdd(Keys key, Keys modifier, bool allow_repeat, out HotKeyState result)
        {
            return TryAdd(key, modifier, allow_repeat, (object)null, out result);
        }

        /// <summary>
        /// Attempts to create a new system-wide hot key with repeat options and a user state.
        /// </summary>
        /// <param name="key">The hot key to register.</param>
        /// <param name="modifier">The key modifier to associate with the key.</param>
        /// <param name="allow_repeat">true to allow repeating key events; otherwise, false.</param>
        /// <param name="state">An optional user state.</param>
        /// <param name="result">When this method returns, contains an instance of a new state representing the hot key registration or null if the registration failed.</param>
        /// <returns>true if a new system-wide hot key was registered; otherwise, false.</returns>
        public bool TryAdd(Keys key, Keys modifier, bool allow_repeat, object state, out HotKeyState result)
        {
            return TryAdd(key, modifier, allow_repeat, state, null, out result);
        }

        /// <summary>
        /// Attempts to create a new system-wide hot key with repeat options and a callback.
        /// </summary>
        /// <param name="key">The hot key to register.</param>
        /// <param name="modifier">The key modifier to associate with the key.</param>
        /// <param name="allow_repeat">true to allow repeating key events; otherwise, false.</param>
        /// <param name="callback">A callback to invoke when the hot key is pressed.</param>
        /// <param name="result">When this method returns, contains an instance of a new state representing the hot key registration or null if the registration failed.</param>
        /// <returns>true if a new system-wide hot key was registered; otherwise, false.</returns>
        public bool TryAdd(Keys key, Keys modifier, bool allow_repeat, EventHandler<HotKeyActivatedEventArgs> callback, out HotKeyState result)
        {
            return TryAdd(key, modifier, allow_repeat, null, callback, out result);
        }

        /// <summary>
        /// Attempts to create a new system-wide hot key with repeat options, user state and a callback.
        /// </summary>
        /// <param name="key">The hot key to register.</param>
        /// <param name="modifier">The key modifier to associate with the key.</param>
        /// <param name="allow_repeat">true to allow repeating key events; otherwise, false.</param>
        /// <param name="state">An optional user state.</param>
        /// <param name="callback">A callback to invoke when the hot key is pressed.</param>
        /// <param name="result">When this method returns, contains an instance of a new state representing the hot key registration or null if the registration failed.</param>
        /// <returns>true if a new system-wide hot key was registered; otherwise, false.</returns>
        public bool TryAdd(Keys key, Keys modifier, bool allow_repeat, object state, EventHandler<HotKeyActivatedEventArgs> callback, out HotKeyState result)
        {
            return TryAdd(new HotKey(key, modifier), allow_repeat, state, callback, out result);
        }

        /// <summary>
        /// Attempts to create a new system-wide hot key with repeat options, user state and a callback.
        /// </summary>
        /// <param name="hotkey">The hot key to register.</param>
        /// <param name="allow_repeat">true to allow repeating key events; otherwise, false.</param>
        /// <param name="state">An optional user state.</param>
        /// <param name="callback">A callback to invoke when the hot key is pressed.</param>
        /// <param name="result">When this method returns, contains an instance of a new state representing the hot key registration or null if the registration failed.</param>
        /// <returns>true if a new system-wide hot key was registered; otherwise, false.</returns>
        public bool TryAdd(HotKey hotkey, bool allow_repeat, object state, EventHandler<HotKeyActivatedEventArgs> callback, out HotKeyState result)
        {
            int id = ++HotKeyID; //get next hotkey id

            if (id > MAXIMUM_REGISTERHOTKEY_ID) //max value id can be
                throw new NotImplementedException($"HotKeyComponent cannot support identification numbers greater-than {MAXIMUM_REGISTERHOTKEY_ID}; previously unregistered hotkey ids should be reused.");

            //create internal state for timestamps, state and callback
            InternalHotKeyState key_state = new InternalHotKeyState(this, id, hotkey, allow_repeat, state, callback);

            if (RegisterHotKey(key_state)) //register hot key with system
            {
                localHotKeys.Add(id, key_state); //add to collection
                result = key_state; //set state
                return true;
            }

            result = null;
            return false;
        }
        #endregion

        #region Remove HotKey
        /// <summary>
        /// Removes the specified hot key.
        /// </summary>
        /// <param name="state">The hot key to remove.</param>
        /// <returns>true if the hot key was removed successfully; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">state is null</exception>
        /// <exception cref="ArgumentException">state was created by a different HotKeyComponent instance</exception>
        /// <exception cref="ArgumentException">state is invalid type</exception>
        public bool Remove(HotKeyState state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            if (state.Owner != this)
                throw new ArgumentException("The specified state was created by a different HotKeyComponent instance.", "state");

            InternalHotKeyState hotkey = state as InternalHotKeyState; //cast state

            if (hotkey == null) //validate state
                throw new ArgumentException("The specified state is an invalid type.", "state");

            if (localHotKeys.Remove(hotkey.ID)) //attempt to remove the hotkey
            {
                UnregisterHotKey(hotkey); //unregister the hotkey
                return true;
            }
            else
                return false;
        }
        #endregion

        #region Register / Unregister
        /// <summary>
        /// Registers the specified hot key.
        /// </summary>
        /// <param name="hotkey">The hot key to register.</param>
        /// <returns>true if the hot key was registered; otherwise false.</returns>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool RegisterHotKey(InternalHotKeyState hotkey)
        {
            KeyModifiers modifiers = hotkey.HotKey.NativeModifier;

            if (!hotkey.AllowRepeat)
                modifiers |= KeyModifiers.MOD_NOREPEAT; //not supported on Windows Vista and Windows XP/2000

            return NativeMethods.RegisterHotKey(window.Handle, hotkey.ID, modifiers, hotkey.HotKey.Key);
        }

        /// <summary>
        /// Unregisters the specified hot key.
        /// </summary>
        /// <param name="hotkey">The hot key to unregister.</param>
        /// <returns>true if the hot key was unregistered; otherwise false.</returns>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool UnregisterHotKey(InternalHotKeyState hotkey)
        {
            return NativeMethods.UnregisterHotKey(window.Handle, hotkey.ID);
        }
        #endregion
    }
}
