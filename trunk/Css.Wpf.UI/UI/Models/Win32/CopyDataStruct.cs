using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Css.Wpf.UI.Models.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CopyDataStruct
    {
        public IntPtr dwData;
        public int cbData;//字符串长度
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;//字符串
    }
}
