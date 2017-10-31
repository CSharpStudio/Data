using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Css.Wpf.UI.Native {
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLibraryHandle() : base(true) 
        {}

        protected override bool ReleaseHandle() 
        {
            return UnsafeNativeMethods.FreeLibrary(handle);
        }
    }
}