using System;
using System.Runtime.InteropServices;

namespace CommonUtils.Core
{
    /// <summary>
    /// 指针操作
    /// </summary>
    public static class PointerUtil
    {
        public static IntPtr GetIDispatchPointer(object o)
        => Marshal.GetIDispatchForObject(o);
    }
}
