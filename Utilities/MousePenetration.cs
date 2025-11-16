using System;
using System.Runtime.InteropServices;

namespace BongoCat_Like.Utilities
{
    public static class MousePenetration
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int LWA_ALPHA = 0x2;
        private const int LWA_COLORKEY = 0x1;

        /// <summary>
        /// 启用鼠标穿透功能
        /// </summary>
        /// <param name="handle">窗口句柄</param>
        public static void Enable(IntPtr handle)
        {
            // 获取当前扩展样式
            int extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);

            // 添加分层和透明样式
            _ = SetWindowLong(handle, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            // 可选：设置透明度（这里设置为完全透明，但保留鼠标穿透特性）
            SetLayeredWindowAttributes(handle, 0, 255, LWA_ALPHA);
        }

        /// <summary>
        /// 禁用鼠标穿透功能
        /// </summary>
        /// <param name="handle">窗口句柄</param>
        public static void Disable(IntPtr handle)
        {
            // 获取当前扩展样式
            int extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);

            // 移除透明样式（保留分层样式以支持透明度调整）
            _ = SetWindowLong(handle, GWL_EXSTYLE, (extendedStyle & ~WS_EX_TRANSPARENT) | WS_EX_LAYERED);

            // 恢复完全不透明状态
            SetLayeredWindowAttributes(handle, 0, 255, LWA_ALPHA);
        }

        /// <summary>
        /// 检查当前窗口是否启用了鼠标穿透
        /// </summary>
        /// <param name="handle">窗口句柄</param>
        /// <returns>是否启用穿透</returns>
        public static bool IsEnabled(IntPtr handle)
        {
            int extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);
            return (extendedStyle & WS_EX_TRANSPARENT) == WS_EX_TRANSPARENT;
        }
    }
}
