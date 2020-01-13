using System;
using System.Runtime.InteropServices;

namespace CommonUtils
{
    /// <summary>
    /// 指针操作
    /// </summary>
    public static class PointerUtil
    {
        public static IntPtr GetFunctionPointer(Delegate d)
        => Marshal.GetFunctionPointerForDelegate(d);

        public static IntPtr GetIDispatchPointer(object o)
        => Marshal.GetIDispatchForObject(o);

        public static IntPtr GetIUnknownPointer(object o)
        => Marshal.GetIUnknownForObject(o);

        public static T GetObject<T>(IntPtr p)
        => Marshal.PtrToStructure<T>(p);

        public static byte[] GetBytes(IntPtr p, int length)
        {
            byte[] bytes = new byte[length];
            Marshal.Copy(p, bytes, 0, length);
            return bytes;
        }

        /// <summary>
        /// todo 指针互转测试
        /// </summary>
        public static void UseCase()
        {
            var bytes = new byte[] { 1, 2, 3 };

        }
    }
}
