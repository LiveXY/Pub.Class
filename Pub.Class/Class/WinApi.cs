using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Pub.Class {
    /// <summary>
    /// WM_COPYDATA消息所要求的数据结构
    /// </summary>
    public struct CopyDataStruct {
        /// <summary>
        /// handler
        /// </summary>
        public IntPtr dwData;
        /// <summary>
        /// cb data
        /// </summary>
        public int cbData;
        /// <summary>
        /// lp data
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }

    /// <summary>
    /// 设置RichTextBox行距所需要的数据结构,PARAFORMAT2
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PARAFORMAT2 {
        /// <summary>
        /// cbSize
        /// </summary>
        public int cbSize;
        /// <summary>
        /// dwMask
        /// </summary>
        public uint dwMask;
        /// <summary>
        /// wNumbering
        /// </summary>
        public short wNumbering;
        /// <summary>
        /// wReserved
        /// </summary>
        public short wReserved;
        /// <summary>
        /// dxStartIndent
        /// </summary>
        public int dxStartIndent;
        /// <summary>
        /// dxRightIndent
        /// </summary>
        public int dxRightIndent;
        /// <summary>
        /// dxOffset
        /// </summary>
        public int dxOffset;
        /// <summary>
        /// wAlignment
        /// </summary>
        public short wAlignment;
        /// <summary>
        /// cTabCount
        /// </summary>
        public short cTabCount;
        /// <summary>
        /// rgxTabs
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] rgxTabs;
        /// <summary>
        /// dySpaceBefore
        /// </summary>
        public int dySpaceBefore;
        /// <summary>
        /// dySpaceAfter
        /// </summary>
        public int dySpaceAfter;
        /// <summary>
        /// dyLineSpacing
        /// </summary>
        public int dyLineSpacing;
        /// <summary>
        /// sStyle
        /// </summary>
        public short sStyle;
        /// <summary>
        /// bLineSpacingRule
        /// </summary>
        public byte bLineSpacingRule;
        /// <summary>
        /// bOutlineLevel
        /// </summary>
        public byte bOutlineLevel;
        /// <summary>
        /// wShadingWeight
        /// </summary>
        public short wShadingWeight;
        /// <summary>
        /// wShadingStyle
        /// </summary>
        public short wShadingStyle;
        /// <summary>
        /// wNumberingStart
        /// </summary>
        public short wNumberingStart;
        /// <summary>
        /// wNumberingStyle
        /// </summary>
        public short wNumberingStyle;
        /// <summary>
        /// wNumberingTab
        /// </summary>
        public short wNumberingTab;
        /// <summary>
        /// wBorderSpace
        /// </summary>
        public short wBorderSpace;
        /// <summary>
        /// wBorderWidth
        /// </summary>
        public short wBorderWidth;
        /// <summary>
        /// wBorders
        /// </summary>
        public short wBorders;
    }
    public static class WMessages {
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_SETTEXT = 0x000c;
        public const int WM_CLEAR = 0x303;
        public const int WM_CLOSE = 0x10;
        public const int WM_COMMAND = 0x111;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 260;
        public const int BN_CLICKED = 0x00F5;
        public const int KEYEVENTF_KEYUP = 2;
        public const int VK_CONTROL = 0x11;
        public const int VK_F5 = 0x74;
        public const int VK_MENU = 0x12;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        public const int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下
        public const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下
        public const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起 
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040; // 模拟鼠标中键抬起
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标
        public const int WM_GETTEXT = 0x000D;
        public const int WM_CLICK = 0x00F5;
        public const int WM_SHOWDROPDOWN = 0x014D;//在窗体中声明消息常量
        public const int WM_SETCURSOR = 0x14F;
        public const int CB_FINDSTRING = 0x14C;
        public const int LB_GETCOUNT = 0x18B;
        public const int LB_GETTEXT = 0x189;
        public const int LB_GETTEXTLEN = 0x18A;
        public const int AW_HOR_POSITIVE = 0x00000001;
        public const int AW_HOR_NEGATIVE = 0x00000002;
        public const int AW_VER_POSITIVE = 0x00000004;
        public const int AW_VER_NEGATIVE = 0x00000008;
        public const int AW_CENTER = 0x00000010;
        public const int AW_HIDE = 0x00010000;
        public const int AW_ACTIVATE = 0x00020000;
        public const int AW_SLIDE = 0x00040000;
        public const int AW_BLEND = 0x00080000;
    }
    /// <summary>
    /// WinApi
    /// </summary>
    /// <example>
    /// <code>
    /// IntPtr _hWndInvisibleWindow;
    /// if (_hWndInvisibleWindow != IntPtr.Zero) WinApi.SendMessage(_hWndInvisibleWindow, WinApi.WM_SYSCOMMAND, WinApi.SC_CLOSE, 0);  
    /// IntPtr hBuiltInDialog = WinApi.FindWindow("#32770", "");
    /// if (hBuiltInDialog != IntPtr.Zero) {
    ///     List&lt;IntPtr> childWindows = WinApi.EnumChildWindows(hBuiltInDialog); 
    ///     List&lt;string> childWindowNames = WinApi.GetWindowNames(childWindows);
    ///     if (!childWindowNames.Contains("Nur N&amp;ame")) return;
    ///     if (!childWindowNames.Contains("Mehr Spalten")) return;
    ///     _hWndInvisibleWindow = WinApi.CreateWindowEx(0, "Static", "X4UTrick", 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero); 
    ///     WinApi.SetParent(hBuiltInDialog, _hWndInvisibleWindow);
    ///     WinApi.SendMessage(hBuiltInDialog, WinApi.WM_SYSCOMMAND, WinApi.SC_CLOSE, 0);
    /// }
    /// </code>
    /// </example>
    [SuppressUnmanagedCodeSecurity]
    public class WinApi {
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        [DllImport("user32.dll")]
        public static extern int PostMessage(int hwnd, int msg, int wParam, int lParam);
        public static int MakeLParam(int p, int p_2) {
            return ((p_2 << 16) | (p & 0xFFFF));
        }
//#if !MONO40
        /// <summary>
        /// 鼠标左键单击
        /// </summary>
        /// <param name="handle">窗口句柄</param>
        /// <param name="x">位置</param>
        public static void MouseLeftClick(IntPtr handle, Point x) {
            PostMessage(handle, (uint)WMessages.WM_LBUTTONDOWN, 0, MakeLParam(x.X, x.Y));
            PostMessage(handle, (uint)WMessages.WM_LBUTTONUP, 0, MakeLParam(x.X, x.Y));
        }
        /// <summary>
        /// 按钮单击事件
        /// </summary>
        /// <param name="CurrnetFormHandle">窗口句柄</param>
        /// <param name="strWindow">按钮标题</param>
        public static void ClickButton(IntPtr CurrnetFormHandle, string strWindow) {
            ClickButton(CurrnetFormHandle, "Button", strWindow);
        }
        /// <summary>
        /// 按钮单击事件
        /// </summary>
        /// <param name="CurrnetFormHandle">窗口句柄</param>
        /// <param name="strClass">按钮类名</param>
        /// <param name="strWindow">按钮标题</param>
        public static void ClickButton(IntPtr CurrnetFormHandle, string strClass, string strWindow) {
            ClickButton(CurrnetFormHandle, IntPtr.Zero, strClass, strWindow);
        }
        /// <summary>
        /// 按钮单击事件
        /// </summary>
        /// <param name="hwdParent">窗口句柄</param>
        /// <param name="hwndChildAfter"></param>
        /// <param name="strClass">按钮类名</param>
        /// <param name="strWindow">按钮标题</param>
        public static void ClickButton(IntPtr hwdParent, IntPtr hwndChildAfter, string strClass, string strWindow) {
            IntPtr hWnd = FindWindowEx(hwdParent, hwndChildAfter, strClass, strWindow);
            SendMessage(hWnd, (uint)WMessages.WM_LBUTTONDOWN, 0, 0);
            SendMessage(hWnd, (uint)WMessages.WM_LBUTTONUP, 0, 0);
        }
        /// <summary>
        /// 给文本框赋值
        /// </summary>
        /// <example>
        /// <code>
        /// WinApi.SetTextBySendMessage(this.HWND, "测试数据");
        /// </code>
        /// </example>
        /// <param name="hWnd">句柄</param>
        /// <param name="lParam">要发送的内容</param>
        public static void SetTextBySendMessage(IntPtr hWnd, string lParam) {
            SendMessage(hWnd, (int)WMessages.WM_SETTEXT, IntPtr.Zero, lParam);
        }
        /// <summary>
        /// 获得窗口内容或标题
        /// </summary>
        /// <example>
        /// <code>
        /// WinApi.GetText(this.HWND)
        /// </code>
        /// </example>
        /// <param name="hWnd">句柄</param>
        /// <returns></returns>
        public static string GetWindowText(IntPtr hWnd) {
            StringBuilder result = new StringBuilder(128);
            GetWindowText(hWnd, result, result.Capacity);
            return result.ToString();
        }
        /// <summary>
        /// 获得窗口内容或标题
        /// </summary>
        /// <param name="windowHandles">句柄列表</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// List&lt;string> childWindowNames = WinApi.GetWindowTexts(childWindows);
        /// if (!childWindowNames.Contains("Nur N&amp;ame")) return;
        /// </code>
        /// </example>
        public static List<string> GetWindowTexts(List<IntPtr> windowHandles) {
            List<string> windowNameList = new List<string>();
            StringBuilder windowName = new StringBuilder(260);
            foreach (IntPtr hWnd in windowHandles) {
                int textLen = GetWindowText(hWnd, windowName, 260);
                windowNameList.Add(windowName.ToString());
            }
            return windowNameList;
        }
        /// <summary>
        /// 找类名
        /// </summary>
        /// <example>
        /// <code>
        /// WinApi.GetClassName(this.HWND);
        /// </code>
        /// </example>
        /// <param name="hWnd">句柄</param>
        /// <returns></returns>
        public static string GetClassName(IntPtr hWnd) {
            StringBuilder lpClassName = new StringBuilder(128);
            GetClassName(hWnd, lpClassName, lpClassName.Capacity);
            return lpClassName.ToString();
        }
        /// <summary>
        /// 窗口在屏幕位置
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <returns></returns>
        public static Rectangle GetWindowRect(IntPtr hWnd) {
            Rectangle result = default(Rectangle);
            GetWindowRect(hWnd, ref result);
            return result;
        }
        /// <summary>
        /// 窗口相对屏幕位置转换成父窗口位置
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle ScreenToClient(IntPtr hWnd, Rectangle rect) {
            Rectangle result = rect;
            ScreenToClient(hWnd, ref result);
            return result;
        }
        /// <summary>
        /// 窗口大小
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        public static Rectangle GetClientRect(IntPtr hWnd) {
            Rectangle result = default(Rectangle);
            GetClientRect(hWnd, ref result);
            return result;
        }
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        /// <summary>
        /// 模拟键盘输入
        /// </summary>
        /// <param name="key"></param>
        public static void KeyboardEvent(char key) {
            WinApi.keybd_event((byte)key, 0, 0, 0);
            System.Threading.Thread.Sleep(50);
            WinApi.keybd_event((byte)key, 0, 2, 0);
        }
        /// <summary>
        /// 模拟键盘输入
        /// </summary>
        /// <param name="key"></param>
        public static void KeyboardEvent(string keys) {
            foreach (char pchar in keys) {
                KeyboardEvent(pchar);
            }
        }
        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(int hWndParent, int lpEnumFunc, int lParam);
        /// <summary>
        /// 遍历所有子窗口
        /// </summary>
        /// <example>
        /// <code>
        /// List&lt;IntPtr> childWindows = WinApi.EnumChildWindows(hBuiltInDialog);
        /// </code>
        /// </example>
        public static List<IntPtr> EnumChildWindows(IntPtr hParentWnd) {
            List<IntPtr> childWindowHandles = new List<IntPtr>();

            GCHandle hChilds = GCHandle.Alloc(childWindowHandles);
            try {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(hParentWnd, childProc, GCHandle.ToIntPtr(hChilds));
            } finally {
                if (hChilds.IsAllocated) hChilds.Free();
            }
            return childWindowHandles;
        }
        /// <summary>
        /// 当前窗口
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetLocalWindow() {
            Point point;
            WinApi.GetCursorPos(out point);
            return WinApi.WindowFromPoint(point);
        }
        /// <summary>
        /// 取桌面上的所有父级窗口
        /// </summary>
        /// <returns></returns>
        public static IList<WindowInfo> GetAllDesktopWindows() {
            List<WindowInfo> wndList = new List<WindowInfo>();
            EnumWindows(delegate(IntPtr hWnd, int lParam) {
                WindowInfo wnd = new WindowInfo(); StringBuilder sb = new StringBuilder(256);
                wnd.hWnd = hWnd;
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                wndList.Add(wnd);
                return true;
            }, 0);
            return wndList;
        }
        /// <summary>
        /// 查找窗口句柄，如果找到多个匹配窗口，则返回顶层窗口句柄
        /// </summary>
        /// <param name="windowName">窗口标题</param>
        public static IntPtr FindWindowByText(string windowName) {
            return FindWindow(null, windowName);
        }
        /// <summary>
        /// 查找窗口句柄，如果找到多个匹配窗口，则返回顶层窗口句柄
        /// </summary>
        /// <param name="className">窗口类名</param>
        public static IntPtr FindWindowByClass(string className) {
            return FindWindow(null, className);
        }
        /// <summary>
        /// 查找窗口句柄，如果找到多个匹配窗口，则返回顶层窗口句柄
        /// </summary>
        /// <param name="handlerParent">父窗口句柄, 如果为0, 则以桌面窗口为父窗口, 查找桌面窗口的所有子窗口</param>
        /// <param name="handlerChildAfter">子窗口句柄,
        /// 1. 子窗口必须是父窗口的直接子窗口.
        /// 2. 如果子窗口为0,则从父窗口的第一个子窗口开始查找</param>
        /// <param name="childClassName">子窗口类名</param>
        /// <param name="childWindowName">子窗口标题</param>
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr handlerParent, IntPtr handlerChildAfter, string childClassName, string childWindowName);
        /// <summary>
        /// 查找窗口句柄，如果找到多个匹配窗口，则返回顶层窗口句柄
        /// </summary>
        /// <param name="handlerParent">父窗口句柄, 如果为0, 则以桌面窗口为父窗口, 查找桌面窗口的所有子窗口</param>
        /// <param name="childWindowName">子窗口标题</param>
        public static IntPtr FindWindowExByText(IntPtr handlerParent, string childWindowName) {
            return FindWindowEx(handlerParent, IntPtr.Zero, null, childWindowName);
        }
        /// <summary>
        /// 查找窗口句柄，如果找到多个匹配窗口，则返回顶层窗口句柄
        /// </summary>
        /// <param name="handlerParent">父窗口句柄, 如果为0, 则以桌面窗口为父窗口, 查找桌面窗口的所有子窗口</param>
        /// <param name="handlerChildAfter">子窗口句柄,
        /// 1. 子窗口必须是父窗口的直接子窗口.
        /// 2. 如果子窗口为0,则从父窗口的第一个子窗口开始查找</param>
        /// <param name="childWindowName">子窗口标题</param>
        public static IntPtr FindWindowExByText(IntPtr handlerParent, IntPtr handlerChildAfter, string childWindowName) {
            return FindWindowEx(handlerParent, handlerChildAfter, null, childWindowName);
        }
        /// <summary>
        /// 查找窗口句柄，如果找到多个匹配窗口，则返回顶层窗口句柄
        /// </summary>
        /// <param name="handlerParent">父窗口句柄, 如果为0, 则以桌面窗口为父窗口, 查找桌面窗口的所有子窗口</param>
        /// <param name="childClassName">子窗口类名</param>
        public static IntPtr FindWindowExByClass(IntPtr handlerParent, string childClassName) {
            return FindWindowEx(handlerParent, IntPtr.Zero, childClassName, null);
        }
        /// <summary>
        /// 查找窗口句柄，如果找到多个匹配窗口，则返回顶层窗口句柄
        /// </summary>
        /// <param name="handlerParent">父窗口句柄, 如果为0, 则以桌面窗口为父窗口, 查找桌面窗口的所有子窗口</param>
        /// <param name="handlerChildAfter">子窗口句柄,
        /// 1. 子窗口必须是父窗口的直接子窗口.
        /// 2. 如果子窗口为0,则从父窗口的第一个子窗口开始查找</param>
        /// <param name="childClassName">子窗口类名</param>
        public static IntPtr FindWindowExByClass(IntPtr handlerParent, IntPtr handlerChildAfter, string childClassName) {
            return FindWindowEx(handlerParent, handlerChildAfter, childClassName, null);
        }
        /// <summary>
        /// 全部子窗口句柄
        /// </summary>
        /// <param name="hwndParent">窗口句柄</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static List<IntPtr> FindWindowExByClasss(IntPtr hwndParent, string className) {
            List<IntPtr> resultList = new List<IntPtr>();
            for (IntPtr hwndClient = FindWindowEx(hwndParent, IntPtr.Zero, className, null);
                hwndClient != IntPtr.Zero; hwndClient = FindWindowEx(hwndParent, hwndClient, className, null)) {
                resultList.Add(hwndClient);
            }

            return resultList;
        }
        /// <summary>
        /// 全部子窗口句柄
        /// </summary>
        /// <param name="hwndParent">窗口句柄</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static List<IntPtr> FindWindowExByTexts(IntPtr hwndParent, string textName) {
            List<IntPtr> resultList = new List<IntPtr>();
            for (IntPtr hwndClient = FindWindowEx(hwndParent, IntPtr.Zero, null, textName);
                hwndClient != IntPtr.Zero; hwndClient = FindWindowEx(hwndParent, hwndClient, null, textName)) {
                resultList.Add(hwndClient);
            }

            return resultList;
        }
        /// <summary>
        /// 激活窗口
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        /// <summary>
        /// 激活窗口,并设置窗口状态 1默认 2最小化窗口 3最大化窗口
        /// </summary>
        /// <param name="handlerWindow">窗口句柄</param>
        /// <param name="status">窗口状态 1默认 2最小化窗口 3最大化窗口</param>
        public static void SetActiveWindow(IntPtr handlerWindow, int status = 1) {
            ShowWindowAsync(handlerWindow, (int)status);
            SetForegroundWindow(handlerWindow);
        }
        /// <summary>
        /// 显示或隐藏窗口
        /// </summary>
        /// <param name="handlerWindow">窗口句柄</param>
        /// <param name="cmd">命令 1显示 0隐藏</param>
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr handlerWindow, uint cmd);
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="handlerWindow">窗口句柄</param>
        public static bool ShowWindow(IntPtr handlerWindow) {
            return ShowWindow(handlerWindow, 1);
        }
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="handlerWindow">窗口句柄</param>
        public static bool HideWindow(IntPtr handlerWindow) {
            return ShowWindow(handlerWindow, 0);
        }
        /// <summary>
        /// 显示任务栏
        /// </summary>
        public static bool ShowTaskBar() {
            //获取任务栏窗口句柄
            IntPtr handler = FindWindow("Shell_TrayWnd", null);

            //显示窗口
            return ShowWindow(handler);
        }
        /// <summary>
        /// 隐藏任务栏
        /// </summary>
        public static bool HideTaskBar() {
            //获取任务栏窗口句柄
            IntPtr handler = FindWindow("Shell_TrayWnd", null);

            //隐藏窗口
            return HideWindow(handler);
        }
        /// <summary>
        /// 读取内存中的值
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static int ReadMemoryValue(int baseAddress, string processName) {
            try {
                byte[] buffer = new byte[4];
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); //获取缓冲区地址
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, Safe.GetPIDByProcessName(processName));
                ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, 4, IntPtr.Zero); //将制定内存中的值读入缓冲区
                CloseHandle(hProcess);
                return Marshal.ReadInt32(byteAddress);
            } catch {
                return 0;
            }
        }
        /// <summary>
        /// 将值写入指定内存地址中
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="processName"></param>
        /// <param name="value"></param>
        public static void WriteMemoryValue(int baseAddress, string processName, int value) {
            IntPtr hProcess = OpenProcess(0x1F0FFF, false, Safe.GetPIDByProcessName(processName)); //0x1F0FFF 最高权限
            WriteProcessMemory(hProcess, (IntPtr)baseAddress, new int[] { value }, 4, IntPtr.Zero);
            CloseHandle(hProcess);
        }
        /// <summary>
        /// 线程执行周期
        /// </summary>
        /// <returns></returns>
        public static long GetCycleCount() {
            if (SystemInfo.IsWindowsVistaOrHigher) { //vist以上
                ulong cycleCount = 0;
                QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
                return (long)cycleCount;
            }  else {
                long l;
                long kernelTime, userTimer;
                GetThreadTimes(GetCurrentThread(), out l, out l, out kernelTime, out userTimer);
                return kernelTime + userTimer;
            }
        }
        /// <summary>
        /// QueryThreadCycleTime kernel32.dll API
        /// </summary>
        /// <param name="threadHandle">句柄</param>
        /// <param name="cycleTime"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);
        /// <summary>
        /// GetCurrentThread kernel32.dll API
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);
//#endif
        [DllImport("user32.dll")]
        public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
        /// <summary>
        /// 获取父窗口句柄
        /// </summary>
        /// <example>
        /// <code>
        /// WinApi.GetParent(this.HWND)
        /// </code>
        /// </example>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetParent")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        /// <summary>
        /// 获取窗口位置和大小
        /// </summary>
        /// <example>
        /// <code>
        /// WinApi.GetWindowRect(this.HWND)
        /// </code>
        /// </example>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="rc">位置和大小</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowRect(IntPtr hwnd, ref System.Drawing.Rectangle rc);
        /// <summary>
        /// 获取工作区位置和大小
        /// </summary>
        /// <example>
        /// <code>
        /// Rectangle clientSize = WinApi.GetClientRect(this.HWND);
        /// </code>
        /// </example>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="rc">位置和大小</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClientRect(IntPtr hwnd, ref System.Drawing.Rectangle rc);
        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <example>
        /// <code>
        /// WinApi.MoveWindow(this.HWND, left, this.Top, this.Width, this.Height, true);
        /// </code>
        /// </example>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="nWidth">宽</param>
        /// <param name="nHeight">高</param>
        /// <param name="bRepaint"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <code>
        /// Rectangle clientPoint = WinApi.ScreenToClient(this.Parent.HWND, WinApi.GetWindowRect(this.HWND));
        /// </code>
        /// </example>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="rect"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ScreenToClient(IntPtr hWnd, ref System.Drawing.Rectangle rect);
        /// <summary>
        /// 设置窗口标题
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="lpString">标题</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);
        /// <summary>
        /// 模拟键盘按键
        /// keybd_event(179, 0, 0, 0);
        /// keybd_event(179, 0, 2, 0);  
        /// keybd_event((byte)Keys.CapsLock , 0, 0, 0);
        /// keybd_event((byte)Keys.CapsLock , 0, 2, 0);
        /// 模拟按下'ALT+F4'键
        /// keybd_event(18,0,0,0);
        /// keybd_event(115,0,0,0);
        /// keybd_event(115,0,KEYEVENTF_KEYUP,0);
        /// keybd_event(18,0,KEYEVENTF_KEYUP,0); 
        /// </summary>
        /// <param name="bVk">虚拟键值</param>
        /// <param name="bScan">一般为0</param>
        /// <param name="dwFlags">这里是整数类型 0 为按下，2为释放</param>
        /// <param name="dwExtraInfo">这里是整数类型 一般情况下设成为0</param>
        /// <summary>
        /// 锁定鼠标键盘
        /// </summary>
        /// <param name="Block">True:锁 False:解</param>
        [DllImport("user32.dll")]
        public static extern void BlockInput(bool Block);
        /// <summary>
        /// windows的系统锁定
        /// </summary>
        /// <returns></returns>
        [DllImport("user32 ")]
        public static extern bool LockWorkStation();
        /// <summary>
        /// 遍历所有子窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int EnumWindows(CallBack x, int y);
        /// <summary>
        /// 遍历所有子窗口
        /// </summary>
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWnd, EnumWindowProc callback, IntPtr userObject);
        public static bool EnumWindow(IntPtr hChildWindow, IntPtr pointer) {
            GCHandle hChilds = GCHandle.FromIntPtr(pointer);
            ((List<IntPtr>)hChilds.Target).Add(hChildWindow);

            return true;
        }
        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);
        public delegate bool CallBack(IntPtr hwnd, int lParam);
        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        /// <summary>
        /// 创建一个窗口
        /// </summary>
        /// <example>
        /// <code>
        /// IntPtr _hWndInvisibleWindow = WinApi.CreateWindowEx(0, "Static", "X4UTrick", 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero); 
        /// </code>
        /// </example>
        [DllImport("user32.dll")]
        public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("shell32.dll", EntryPoint = "ShellExecute")]
        public static extern int ShellExecuteA(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nIndex">指定要设定的值的信息</param>
        /// <param name="dwNewLong">新值</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern long SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        public static extern int SetFocus(int hWnd);
        [DllImportAttribute("gdi32.dll ")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, System.Int32 dwRop);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);
        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(HandleRef hWnd);
        [DllImport("user32.dll")]
        public static extern int GetLastActivePopup(int hWnd);
        [DllImport("user32.dll")]
        public static extern int AnyPopup();
        [DllImport("user32.dll")]
        public static extern int EnumThreadWindows(int dwThreadId, CallBack lpfn, int lParam);
        /// <summary>
        /// 窗体闪烁 FlashWindow(this.Handle, 5);
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int FlashWindow(IntPtr hwnd, int bInvert);
        /// <summary>
        /// 动画窗口
        /// AnimateWindow(this.Handle, 500, Win32.AW_VER_NEGATIVE);
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDlg">指定含有控制的对话框</param>
        /// <param name="nIDDlgItem">标识带有将被设置的标题和文本的控制。</param>
        /// <param name="lpString">指向一个以NULL结尾的字符串指针，该字符串指针包含了将被复制到控制的文本。</param>
        /// <returns></returns>
        [DllImport("user32", EntryPoint = "SetDlgItemText")]
        public static extern int SetDlgItemText(int hDlg, int nIDDlgItem, string lpString);
        public struct WindowInfo { public IntPtr hWnd; public string szWindowName; public string szClassName;    }
        [DllImport("user32.dll")]
        public static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        /// <summary>
        /// 查找窗口句柄，如果找到多个匹配窗口，则返回顶层窗口句柄
        /// </summary>
        /// <param name="className">窗口类名</param>
        /// <param name="windowName">窗口标题</param>
        /// <example>
        /// <code>
        /// IntPtr h = WinApi.FindWindow("#32770", "");
        /// if (h != IntPtr.Zero) { }
        /// </code>
        /// </example>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string className, string windowName);
        /// <summary>
        /// 设置活动窗口
        /// </summary>
        /// <param name="handlerWindow">窗口句柄</param>
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern void SetForegroundWindow(IntPtr handlerWindow);
        /// <summary>
        /// 激活窗口，设置窗口状态 1默认 2最小化窗口 3最大化窗口
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">窗口状态 1默认 2最小化窗口 3最大化窗口</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary>
        /// 向窗口发送Windows消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <example>
        /// <code>
        /// WinApi.SendMessage(hBuiltInDialog, WinApi.WM_SYSCOMMAND, WinApi.SC_CLOSE, 0); 
        /// </code>
        /// </example>
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        /// <summary>
        /// 向窗口发送Windows消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref CopyDataStruct lParam);
        /// <summary>
        /// 向窗口发送Windows消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref PARAFORMAT2 lParam);
        /// <summary>
        /// SendMessage(this.textBox1.Handle, WMessages.WM_SETTEXT, IntPtr.Zero, s);
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        /// <summary>
        /// 发送消息(要等处理程序处理完，发送程序才能继续执行)
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint msg, IntPtr wParam, string lParam);
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        [DllImport("User32.dll")]
        public static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);
        /// <summary>
        /// 获取当前焦点控件的窗口句柄
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();
        /// <summary>
        /// 获取窗口句柄的类名
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="lpClassName">获取的类名</param>
        /// <param name="nMaxCount">接收类名缓冲区的大小</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        /// <summary>
        /// 获取窗口句柄的标题
        /// </summary>
        /// <param name="handler">窗口句柄</param>
        /// <param name="text">获取的标题</param>
        /// <param name="size">接收标题缓冲区的大小</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr handler, StringBuilder text, int size);
        /// <summary>
        /// 显示或隐藏鼠标
        /// </summary>
        /// <param name="isShow">是否显示鼠标</param>
        [DllImport("User32")]
        public static extern void ShowCursor(bool isShow);
        /// <summary>
        /// 设置光标位置
        /// </summary>
        /// <param name="x">鼠标的X坐标</param>
        /// <param name="y">鼠标的Y坐标</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);
        /// <summary>
        /// 获取当前鼠标的位置
        /// </summary>
        /// <param name="point">鼠标的当前位置</param>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point point);
        /// <summary>
        /// 获取窗口句柄的屏幕坐标
        /// </summary>
        /// <param name="handler">窗口句柄</param>
        /// <param name="point">窗口句柄的位置</param>
        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr handler, out Point point);
        /// <summary>
        /// 通过屏幕坐标获取窗口句柄
        /// </summary>
        /// <param name="Point">屏幕的位置</param>
        [DllImport("user32")]
        public static extern IntPtr WindowFromPoint(Point Point);
        /// <summary>
        /// SetWindowPos(this.Handle, 100, Screen.PrimaryScreen.Bounds.Width - this.Width, Screen.PrimaryScreen.Bounds.Height - this.Height - 30, 50, 50, 1);
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="hWndInsertAfter">前一个的窗口句柄</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="flags">标识</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int width, int height, int flags);
        /// <summary>
        /// 模拟鼠标事件
        /// mouse_event(WMessages.MOUSEEVENTF_RIGHTDOWN, X , Y , 0, 0);
        /// keybd_event((byte)Keys.F11, 0, 0, 0);//按下F11
        /// keybd_event((byte)Keys.F11, 0, 0x2, 0);   //弹起F11
        /// </summary>
        /// <param name="dwFlags">鼠标事件</param>
        /// <param name="dx">X坐标</param>
        /// <param name="dy">Y坐标</param>
        /// <param name="cButtons"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        /// <summary>
        /// 读进程内存
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpBaseAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nSize"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <returns></returns>
        [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);
        /// <summary>
        /// 打开进程
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        [DllImportAttribute("kernel32.dll", EntryPoint = "OpenProcess")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern void CloseHandle(IntPtr hObject);
        /// <summary>
        /// 写内存
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpBaseAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nSize"></param>
        /// <param name="lpNumberOfBytesWritten"></param>
        /// <returns></returns>
        [DllImportAttribute("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, int[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
    }
}


//与键盘上各键对应的键值
//在软件开发的过程中我们经常与键盘打交道，以下是我查MSDN 所得希望对各位有帮助。
//可在代码中的任何地方用下列值代替键盘上的键：
//值 描述
//0x1 鼠标左键
//0x2 鼠标右键
//0x3 CANCEL 键
//0x4 鼠标中键
//0x8 BACKSPACE 键
//0x9 TAB 键
//0xC CLEAR 键
//0xD ENTER 键
//0x10 SHIFT 键
//0x11 CTRL 键
//0x12 MENU 键
//0x13 PAUSE 键
//0x14 CAPS LOCK 键
//0x1B ESC 键
//0x20 SPACEBAR 键
//0x21 PAGE UP 键
//0x22 PAGE DOWN 键
//0x23 END 键
//0x24 HOME 键
//0x25 LEFT ARROW 键
//0x26 UP ARROW 键
//0x27 RIGHT ARROW 键
//0x28 DOWN ARROW 键
//0x29 SELECT 键
//0x2A PRINT SCREEN 键
//0x2B EXECUTE 键
//0x2C SNAPSHOT 键
//0x2D INSERT 键
//0x2E DELETE 键
//0x2F HELP 键
//0x90 NUM LOCK 键 

//A 至 Z 键与 A - Z 字母的 ASCII 码相同：
//值 描述
//65 A 键
//66 B 键
//67 C 键
//68 D 键
//69 E 键
//70 F 键
//71 G 键
//72 H 键
//73 I 键
//74 J 键
//75 K 键
//76 L 键
//77 M 键
//78 N 键
//79 O 键
//80 P 键
//81 Q 键
//82 R 键
//83 S 键
//84 T 键
//85 U 键
//86 V 键
//87 W 键
//88 X 键
//89 Y 键
//90 Z 键 

//0 至 9 键与数字 0 - 9 的 ASCII 码相同：
//值 描述
//48 0 键
//49 1 键
//50 2 键
//51 3 键
//52 4 键
//53 5 键
//54 6 键
//55 7 键
//56 8 键
//57 9 键 

//下列常数代表数字键盘上的键：
//值 描述
//0x60 0 键
//0x61 1 键
//0x62 2 键
//0x63 3 键
//0x64 4 键
//0x65 5 键
//0x66 6 键
//0x67 7 键
//0x68 8 键
//0x69 9 键
//0x6A MULTIPLICATION SIGN (*) 键
//0x6B PLUS SIGN (+) 键
//0x6C ENTER 键
//0x6D MINUS SIGN (-) 键
//0x6E DECIMAL POINT (.) 键
//0x6F DIVISION SIGN (/) 键 

//下列常数代表功能键：
//值 描述
//0x70 F1 键
//0x71 F2 键
//0x72 F3 键
//0x73 F4 键
//0x74 F5 键
//0x75 F6 键
//0x76 F7 键
//0x77 F8 键
//0x78 F9 键
//0x79 F10 键
//0x7A F11 键
//0x7B F12 键
//0x7C F13 键
//0x7D F14 键
//0x7E F15 键
//0x7F F16 键

//public partial class Form1 : Form {
//    public Form1() {
//        InitializeComponent();
//    }

//    private void Form1_Load(object sender, EventArgs e) {

//    }

//    //启动无线阳光
//    private void btnGet_Click(object sender, EventArgs e) {
//        if (Helper.GetPidByProcessName(processName) == 0) {
//            MessageBox.Show("哥们启用之前游戏总该运行吧！");
//            return;
//        }
//        if (btnGet.Text == "启用-阳光无限") {
//            timer1.Enabled = true;
//            btnGet.Text = "关闭-阳光无限";
//        } else {
//            timer1.Enabled = false;
//            btnGet.Text = "启用-阳光无限";
//        }
//    }

//    private void timer1_Tick(object sender, EventArgs e) {

//        if (Helper.GetPidByProcessName(processName) == 0) {
//            timer1.Enabled = false;
//            btnGet.Text = "启用-阳光无限";
//        }
//        int address = ReadMemoryValue(baseAddress);             //读取基址(该地址不会改变)
//        address = address + 0x768;                              //获取2级地址
//        address = ReadMemoryValue(address);
//        address = address + 0x5560;                             //获取存放阳光数值的地址
//        WriteMemory(address, 0x1869F);                          //写入数据到地址（0x1869F表示99999）
//        timer1.Interval = 1000;
//    }

//    //启动无线金钱
//    private void btnMoney_Click(object sender, EventArgs e) {

//        if (Helper.GetPidByProcessName(processName) == 0) {
//            MessageBox.Show("哥们启用之前游戏总该运行吧！");
//            return;
//        }
//        if (btnMoney.Text == "启用-金钱无限") {
//            timer2.Enabled = true;
//            btnMoney.Text = "关闭-金钱无限";
//        } else {
//            timer2.Enabled = false;
//            btnMoney.Text = "启用-金钱无限";
//        }
//    }

//    private void timer2_Tick(object sender, EventArgs e) {
//        if (Helper.GetPidByProcessName(processName) == 0) {
//            timer2.Enabled = false;
//            btnMoney.Text = "启用-金钱无限";
//        }
//        int address = ReadMemoryValue(baseAddress);             //读取基址(该地址不会改变)
//        address = address + 0x82C;                              //获取2级地址
//        address = ReadMemoryValue(address);
//        address = address + 0x28;                               //得到金钱地址
//        WriteMemory(address, 0x1869F);                          //写入数据到地址（0x1869F表示99999）
//        timer2.Interval = 1000;
//    }

//    private void btnGo_Click(object sender, EventArgs e) {
//        if (Helper.GetPidByProcessName(processName) == 0) {
//            MessageBox.Show("哥们启用之前游戏总该运行吧！");
//            return;
//        }
//        int address = ReadMemoryValue(baseAddress);             //读取基址(该地址不会改变)
//        address = address + 0x82C;                              //获取2级地址
//        address = ReadMemoryValue(address);
//        address = address + 0x24;
//        int lev = 1;
//        try {
//            lev = int.Parse(txtLev.Text.Trim());
//        } catch {
//            MessageBox.Show("输入的关卡格式不真确！默认设置为1");
//        }

//        WriteMemory(address, lev);

//    }

//    //读取制定内存中的值
//    public int ReadMemoryValue(int baseAdd) {
//        return Helper.ReadMemoryValue(baseAdd, processName);
//    }

//    //将值写入指定内存中
//    public void WriteMemory(int baseAdd, int value) {
//        Helper.WriteMemoryValue(baseAdd, processName, value);
//    }

//    private int baseAddress = 0x006A9EC0;           //游戏内存基址
//    private string processName = "PlantsVsZombies"; //游戏进程名字
//}
