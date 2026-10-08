using System;
using System.Collections.Generic;

namespace SuryaHUD.Win32
{
    public sealed class HotkeyManager : IDisposable
    {
        private readonly Dictionary<int, Action> _actions = new();
        private IntPtr _windowHandle;
        private int _nextId = 1;

        public void Initialize(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }

        public bool Register(uint modifiers, uint virtualKey, Action action)
        {
            if (_windowHandle == IntPtr.Zero || action == null)
                return false;

            var id = _nextId++;
            if (!NativeMethods.RegisterHotKey(_windowHandle, id, modifiers, virtualKey))
                return false;

            _actions[id] = action;
            return true;
        }

        public void ProcessMessage(int message, IntPtr wParam, IntPtr lParam)
        {
            if (message != 0x0312)
                return;

            int id = wParam.ToInt32();
            if (_actions.TryGetValue(id, out var action))
                action();
        }

        public void Dispose()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                foreach (var id in _actions.Keys)
                    NativeMethods.UnregisterHotKey(_windowHandle, id);
            }

            _actions.Clear();
            _windowHandle = IntPtr.Zero;
        }
    }
}
