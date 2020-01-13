using System.Runtime.InteropServices;

namespace CommonUtils
{
    /// <summary>
    /// 访问鼠标硬件
    /// </summary>
    public class MouseUtil
    {
        /// <summary>
        /// 鼠标移动
        /// </summary>
        public static void Move(int stepX = 0, int stepY = 0)
        {
            mouse_event(MouseEventEnum.Move, stepX, stepY, 0, 0);
        }

        /// <summary>
        /// 鼠标飞到某个位置
        /// </summary>
        public static void MoveTo(int x, int y)
        {
            SetCursorPos(x, y);
        }


        //鼠标左键按下
        public static void LeftDown()
        {
            mouse_event(MouseEventEnum.LeftDown, 0, 0, 0, 0);
        }

        //鼠标左键松开
        public static void LeftUp()
        {
            mouse_event(MouseEventEnum.LeftUp, 0, 0, 0, 0);
        }

        //鼠标左键单击
        public static void LeftClick()
        {
            mouse_event(MouseEventEnum.LeftDown, 0, 0, 0, 0);
            mouse_event(MouseEventEnum.LeftUp, 0, 0, 0, 0);
        }

        //鼠标左键双击
        public static void LeftDoubleClick()
        {
            mouse_event(MouseEventEnum.LeftDown, 0, 0, 0, 0);
            mouse_event(MouseEventEnum.LeftUp, 0, 0, 0, 0);
            mouse_event(MouseEventEnum.LeftDown, 0, 0, 0, 0);
            mouse_event(MouseEventEnum.LeftUp, 0, 0, 0, 0);
        }

        //鼠标右键单击
        public static void RightDown()
        {
            mouse_event(MouseEventEnum.RightDown, 0, 0, 0, 0);
        }

        //鼠标右键单击
        public static void RightUp()
        {
            mouse_event(MouseEventEnum.RightUp, 0, 0, 0, 0);
        }

        //鼠标右键单击
        public static void RightClick()
        {
            mouse_event(MouseEventEnum.RightDown, 0, 0, 0, 0);
            mouse_event(MouseEventEnum.RightUp, 0, 0, 0, 0);
        }

        //鼠标右键单击
        public static void RightDoublieClick()
        {
            mouse_event(MouseEventEnum.RightDown, 0, 0, 0, 0);
            mouse_event(MouseEventEnum.RightUp, 0, 0, 0, 0);
            mouse_event(MouseEventEnum.RightDown, 0, 0, 0, 0);
            mouse_event(MouseEventEnum.RightUp, 0, 0, 0, 0);
        }

        #region 外部引用
        /// <summary>
        /// 设置鼠标位置
        /// </summary>
        [DllImport("user32")]
        private static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// 设置鼠标动作
        /// </summary>
        [DllImport("user32")]
        private static extern int mouse_event(MouseEventEnum mouseEvent, int stepX, int stepY, int zero1, int zero2);

        /// <summary>
        /// 设置鼠标动作的键值
        /// </summary>
        private enum MouseEventEnum
        {
            Move = 0x0001,               //发生移动
            LeftDown = 0x0002,           //鼠标按下左键
            LeftUp = 0x0004,             //鼠标松开左键
            RightDown = 0x0008,          //鼠标按下右键
            RightUp = 0x0010,            //鼠标松开右键
            MiddleDown = 0x0020,         //鼠标按下中键
            MiddleUp = 0x0040,           //鼠标松开中键
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,              //鼠标轮被移动
            VirtualDesk = 0x4000,        //虚拟桌面
            Absolute = 0x8000
        }
        #endregion
    }
}
