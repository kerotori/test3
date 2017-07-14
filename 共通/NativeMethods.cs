﻿using System;
using System.Runtime.InteropServices;

namespace 共通
{
    public class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(
        IntPtr hWnd,
        [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(
        IntPtr hWnd,
        out WINDOWPLACEMENT lpwndpl);
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;

        /// <summary>
        /// ウインドウの状態(最小化・最大化・通常)
        /// </summary>
        public SW showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        
        /// <summary>
        /// 通常状態のウインドウの位置とサイズ
        /// </summary>
        public RECT normalPosition;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    public enum SW
    {
        HIDE = 0,
        SHOWNORMAL = 1,
        SHOWMINIMIZED = 2,
        SHOWMAXIMIZED = 3,
        SHOWNOACTIVATE = 4,
        SHOW = 5,
        MINIMIZE = 6,
        SHOWMINNOACTIVE = 7,
        SHOWNA = 8,
        RESTORE = 9,
        SHOWDEFAULT = 10,
    }
}
