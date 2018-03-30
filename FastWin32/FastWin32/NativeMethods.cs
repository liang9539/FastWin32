using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FastWin32
{
    /// <summary>
    /// 本地方法
    /// </summary>
    internal static class NativeMethods
    {
        #region Constant
        /// <summary>
        /// 最大模块名长度
        /// </summary>
        public const uint MAX_MODULE_NAME32 = 255;

        /// <summary>
        /// 最大路径长度
        /// </summary>
        public const uint MAX_PATH = 260;

        /// <summary>
        /// 表示当前进程的伪句柄
        /// </summary>
        public static readonly IntPtr CURRENT_PROCESS = (IntPtr)(-1);

        public const uint INFINITE = 0xFFFFFFFF;

        public const uint LIST_MODULES_DEFAULT = 0x0;

        public const uint LIST_MODULES_32BIT = 0x1;

        public const uint LIST_MODULES_64BIT = 0x2;

        public const uint LIST_MODULES_ALL = 0x3;

        #region Hook Id
        /// <summary>
        /// Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box, message box, menu, or scroll bar. For more information, see the MessageProc hook procedure.
        /// </summary>
        public const uint WH_MSGFILTER = unchecked((uint)-1);

        /// <summary>
        /// Installs a hook procedure that records input messages posted to the system message queue. This hook is useful for recording macros. For more information, see the JournalRecordProc hook procedure.
        /// </summary>
        public const uint WH_JOURNALRECORD = 0;

        /// <summary>
        /// Installs a hook procedure that posts messages previously recorded by a WH_JOURNALRECORD hook procedure. For more information, see the JournalPlaybackProc hook procedure.
        /// </summary>
        public const uint WH_JOURNALPLAYBACK = 1;

        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure.
        /// </summary>
        public const uint WH_KEYBOARD = 2;

        /// <summary>
        /// Installs a hook procedure that monitors messages posted to a message queue. For more information, see the GetMsgProc hook procedure.
        /// </summary>
        public const uint WH_GETMESSAGE = 3;

        /// <summary>
        /// Installs a hook procedure that monitors messages before the system sends them to the destination window procedure. For more information, see the CallWndProc hook procedure.
        /// </summary>
        public const uint WH_CALLWNDPROC = 4;

        /// <summary>
        /// Installs a hook procedure that receives notifications useful to a CBT application. For more information, see the CBTProc hook procedure.
        /// </summary>
        public const uint WH_CBT = 5;

        /// <summary>
        /// Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box, message box, menu, or scroll bar. The hook procedure monitors these messages for all applications in the same desktop as the calling thread. For more information, see the SysMsgProc hook procedure.
        /// </summary>
        public const uint WH_SYSMSGFILTER = 6;

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure.
        /// </summary>
        public const uint WH_MOUSE = 7;

        /// <summary>
        /// 当调用 GetMessage 或 PeekMessage 来从消息队列种查询非鼠标、键盘消息时
        /// </summary>
        public const uint WH_HARDWARE = 8;

        /// <summary>
        /// Installs a hook procedure useful for debugging other hook procedures. For more information, see the DebugProc hook procedure.
        /// </summary>
        public const uint WH_DEBUG = 9;

        /// <summary>
        /// Installs a hook procedure that receives notifications useful to shell applications. For more information, see the ShellProc hook procedure.
        /// </summary>
        public const uint WH_SHELL = 10;

        /// <summary>
        /// Installs a hook procedure that will be called when the application's foreground thread is about to become idle. This hook is useful for performing low priority tasks during idle time. For more information, see the ForegroundIdleProc hook procedure.
        /// </summary>
        public const uint WH_FOREGROUNDIDLE = 11;

        /// <summary>
        /// Installs a hook procedure that monitors messages after they have been processed by the destination window procedure. For more information, see the CallWndRetProc hook procedure.
        /// </summary>
        public const uint WH_CALLWNDPROCRET = 12;

        /// <summary>
        /// Installs a hook procedure that monitors low-level keyboard input events. For more information, see the LowLevelKeyboardProc hook procedure.
        /// </summary>
        public const uint WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Installs a hook procedure that monitors low-level mouse input events. For more information, see the LowLevelMouseProc hook procedure.
        /// </summary>
        public const uint WH_MOUSE_LL = 14;
        #endregion

        #region Memory
        public const uint PAGE_NOACCESS = 0x01;

        public const uint PAGE_READONLY = 0x02;

        public const uint PAGE_READWRITE = 0x04;

        public const uint PAGE_WRITECOPY = 0x08;

        public const uint PAGE_EXECUTE = 0x10;

        public const uint PAGE_EXECUTE_READ = 0x20;

        public const uint PAGE_EXECUTE_READWRITE = 0x40;

        public const uint PAGE_EXECUTE_WRITECOPY = 0x80;

        public const uint PAGE_GUARD = 0x100;

        public const uint PAGE_NOCACHE = 0x200;

        public const uint PAGE_WRITECOMBINE = 0x400;

        public const uint PAGE_REVERT_TO_FILE_MAP = 0x80000000;

        public const uint PAGE_ENCLAVE_THREAD_CONTROL = 0x80000000;

        public const uint PAGE_TARGETS_NO_UPDATE = 0x40000000;

        public const uint PAGE_TARGETS_INVALID = 0x40000000;

        public const uint PAGE_ENCLAVE_UNVALIDATED = 0x20000000;

        public const uint MEM_COMMIT = 0x00001000;

        public const uint MEM_RESERVE = 0x00002000;

        public const uint MEM_DECOMMIT = 0x00004000;

        public const uint MEM_RELEASE = 0x00008000;

        public const uint MEM_FREE = 0x00010000;

        public const uint MEM_PRIVATE = 0x00020000;

        public const uint MEM_MAPPED = 0x00040000;

        public const uint MEM_RESET = 0x00080000;

        public const uint MEM_TOP_DOWN = 0x00100000;

        public const uint MEM_WRITE_WATCH = 0x00200000;

        public const uint MEM_PHYSICAL = 0x00400000;

        public const uint MEM_ROTATE = 0x00800000;

        public const uint MEM_DIFFERENT_IMAGE_BASE_OK = 0x00800000;

        public const uint MEM_RESET_UNDO = 0x01000000;

        public const uint MEM_LARGE_PAGES = 0x20000000;

        public const uint MEM_4MB_PAGES = 0x80000000;

        public const uint MEM_64K_PAGES = MEM_LARGE_PAGES | MEM_PHYSICAL;

        public const uint SEC_64K_PAGES = 0x00080000;

        public const uint SEC_FILE = 0x00800000;

        public const uint SEC_IMAGE = 0x01000000;

        public const uint SEC_PROTECTED_IMAGE = 0x02000000;

        public const uint SEC_RESERVE = 0x04000000;

        public const uint SEC_COMMIT = 0x08000000;

        public const uint SEC_NOCACHE = 0x10000000;

        public const uint SEC_WRITECOMBINE = 0x40000000;

        public const uint SEC_LARGE_PAGES = 0x80000000;

        public const uint SEC_IMAGE_NO_EXECUTE = SEC_IMAGE | SEC_NOCACHE;

        public const uint MEM_IMAGE = SEC_IMAGE;

        public const uint WRITE_WATCH_FLAG_RESET = 0x01;

        public const uint MEM_UNMAP_WITH_TRANSIENT_BOOST = 0x01;
        #endregion

        #region Process Security and Access Rights
        /// <summary>
        /// Required to delete the object.
        /// </summary>
        public const uint DELETE = 0x00010000;

        /// <summary>
        /// Required to read information in the security descriptor for the object, not including the information in the SACL.
        /// To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right.
        /// For more information, see SACL Access Right.
        /// </summary>
        public const uint READ_CONTROL = 0x00020000;

        /// <summary>
        /// The right to use the object for synchronization.
        /// This enables a thread to wait until the object is in the signaled state.
        /// </summary>
        public const uint SYNCHRONIZE = 0x00100000;

        /// <summary>
        /// Required to modify the DACL in the security descriptor for the object.
        /// </summary>
        public const uint WRITE_DAC = 0x00040000;

        /// <summary>
        /// Required to change the owner in the security descriptor for the object.
        /// </summary>
        public const uint WRITE_OWNER = 0x00080000;

        /// <summary>
        /// Standard Rights Required
        /// </summary>
        public const uint STANDARD_RIGHTS_REQUIRED = DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER;

        /// <summary>
        /// Required to create a process.
        /// </summary>
        public const uint PROCESS_CREATE_PROCESS = 0x0080;

        /// <summary>
        /// Required to create a thread.
        /// </summary>
        public const uint PROCESS_CREATE_THREAD = 0x0002;

        /// <summary>
        /// Required to duplicate a handle using DuplicateHandle.
        /// </summary>
        public const uint PROCESS_DUP_HANDLE = 0x0040;

        /// <summary>
        /// Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken).
        /// </summary>
        public const uint PROCESS_QUERY_INFORMATION = 0x0400;

        /// <summary>
        /// Required to retrieve certain information about a process (see GetExitCodeProcess, GetPriorityClass, IsProcessInJob, QueryFullProcessImageName).
        /// A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION.
        /// </summary>
        public const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        /// <summary>
        /// Required to set certain information about a process, such as its priority class (see SetPriorityClass).
        /// </summary>
        public const uint PROCESS_SET_INFORMATION = 0x0200;

        /// <summary>
        /// Required to set memory limits using SetProcessWorkingSetSize.
        /// </summary>
        public const uint PROCESS_SET_QUOTA = 0x0100;

        /// <summary>
        /// Required to suspend or resume a process.
        /// </summary>
        public const uint PROCESS_SUSPEND_RESUME = 0x0800;

        /// <summary>
        /// Required to terminate a process using TerminateProcess.
        /// </summary>
        public const uint PROCESS_TERMINATE = 0x0001;

        /// <summary>
        /// Required to perform an operation on the address space of a process (see VirtualProtectEx and O:WriteProcessMemory).
        /// </summary>
        public const uint PROCESS_VM_OPERATION = 0x0008;

        /// <summary>
        /// Required to read memory in a process using O:ReadProcessMemory.
        /// </summary>
        public const uint PROCESS_VM_READ = 0x0010;

        /// <summary>
        /// Required to write to memory in a process using O:WriteProcessMemory.
        /// </summary>
        public const uint PROCESS_VM_WRITE = 0x0020;

        /// <summary>
        /// All possible access rights for a process object.
        /// </summary>
        public const uint PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF;
        #endregion

        #region Thread Creation
        /// <summary>
        /// 线程被创建为挂起状态
        /// </summary>
        public const uint CREATE_SUSPENDED = 0x00000004;

        /// <summary>
        /// 指定堆栈大小
        /// </summary>
        public const uint STACK_SIZE_PARAM_IS_A_RESERVATION = 0x00010000;
        #endregion

        #region Thread Security and Access Rights
        /// <summary>
        /// Required to read certain information from the thread object, such as the exit code (see GetExitCodeThread).
        /// </summary>
        public const uint THREAD_QUERY_INFORMATION = 0x0040;
        #endregion

        #region Window Messages
        public const uint WM_NULL = 0x0000;

        public const uint WM_KEYDOWN = 0x0100;

        public const uint WM_KEYUP = 0x0101;

        public const uint WM_SYSKEYDOWN = 0x0104;

        public const uint WM_SYSKEYUP = 0x0105;

        public const uint WM_USER = 0x0400;

        public const uint WM_APP = 0x8000;
        #endregion
        #endregion

        #region Structures
        #region Hook Structures
        /// <summary>
        /// Contains information about a low-level keyboard input event.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct KBDLLHOOKSTRUCT
        {
            /// <summary>
            /// A virtual-key code.
            /// The code must be a value in the range 1 to 254.
            /// </summary>
            public uint vkCode;

            /// <summary>
            /// A hardware scan code for the key.
            /// </summary>
            public uint scanCode;

            /// <summary>
            /// The extended-key flag, event-injected flags, context code, and transition-state flag.
            /// This member is specified as follows.
            /// An application can use the following values to test the keystroke flags.
            /// Testing LLKHF_INJECTED (bit 4) will tell you whether the event was injected.
            /// If it was, then testing LLKHF_LOWER_IL_INJECTED (bit 1) will tell you whether or not the event was injected from a process running at lower integrity level.
            /// </summary>
            public uint flags;

            /// <summary>
            /// The time stamp for this message, equivalent to what GetMessageTime would return for this message.
            /// </summary>
            public uint time;

            /// <summary>
            /// Additional information associated with the message.
            /// </summary>
            public IntPtr dwExtraInfo;
        }
        #endregion

        #region ImageHlp Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct IMAGE_EXPORT_DIRECTORY
        {
            public uint Characteristics;

            public uint TimeDateStamp;

            public ushort MajorVersion;

            public ushort MinorVersion;

            public uint Name;

            public uint Base;

            public uint NumberOfFunctions;

            public uint NumberOfNames;

            public uint AddressOfFunctions;

            public uint AddressOfNames;

            public uint AddressOfNameOrdinals;
        }
        #endregion

        #region Memory Management Structures
        /// <summary>
        /// Contains information about a range of pages in the virtual address space of a process.
        /// The VirtualQuery and VirtualQueryEx functions use this structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MEMORY_BASIC_INFORMATION
        {
            /// <summary>
            /// A pointer to the base address of the region of pages.
            /// </summary>
            public IntPtr BaseAddress;

            /// <summary>
            /// A pointer to the base address of a range of pages allocated by the VirtualAlloc function.
            /// The page pointed to by the BaseAddress member is contained within this allocation range.
            /// </summary>
            public IntPtr AllocationBase;

            /// <summary>
            /// The memory protection option when the region was initially allocated.
            /// This member can be one of the memory protection constants or 0 if the caller does not have access.
            /// </summary>
            public uint AllocationProtect;

            /// <summary>
            /// The size of the region beginning at the base address in which all pages have identical attributes, in bytes.
            /// </summary>
            public IntPtr RegionSize;

            /// <summary>
            /// The state of the pages in the region.
            /// </summary>
            public uint State;

            /// <summary>
            /// The access protection of the pages in the region. This member is one of the values listed for the AllocationProtect member.
            /// </summary>
            public uint Protect;

            /// <summary>
            /// The type of pages in the region.
            /// </summary>
            public uint Type;

            /// <summary>
            /// 结构体在非托管内存中大小
            /// </summary>
            public static readonly uint UnmanagedSize = (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
        }
        #endregion

        #region Message Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MSG
        {
            public IntPtr hwnd;

            public uint message;

            public IntPtr wParam;

            public IntPtr lParam;

            public uint time;

            public POINT pt;
        }
        #endregion

        #region Rectangle Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct POINT
        {
            public int x;

            public int y;
        }
        #endregion

        #region Shell Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHELLEXECUTEINFO
        {
            public uint cbSize;

            public uint fMask;

            public IntPtr hwnd;

            public string lpVerb;

            public string lpFile;

            public string lpParameters;

            public string lpDirectory;

            public int nShow;

            public IntPtr hInstApp;

            public IntPtr lpIDList;

            public string lpClass;

            public IntPtr hkeyClass;

            public uint dwHotKey;

            public IntPtr hMonitor;

            public IntPtr hProcess;

            public static readonly uint UnmanagedSize = (uint)Marshal.SizeOf(typeof(SHELLEXECUTEINFO));
        }
        #endregion
        #endregion

        #region Callback
        /// <summary>
        /// 回调函数 要继续遍历,返回true;要停止遍历,返回false
        /// </summary>
        /// <param name="hWnd">子级窗口的句柄</param>
        /// <param name="lParam">EnumWindows或EnumDesktopWindows中给出的应用程序定义值</param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// 回调函数 要继续遍历,返回true;要停止遍历,返回false
        /// </summary>
        /// <param name="hWnd">顶级窗口的句柄</param>
        /// <param name="lParam">EnumWindows或EnumDesktopWindows中给出的应用程序定义值</param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// HookProc 回调函数
        /// </summary>
        /// <param name="nCode">钩子代码传递给当前的钩子过程。下一个钩子过程使用此代码来确定如何处理挂钩信息。</param>
        /// <param name="wParam">此参数的含义取决于与当前钩链相关联的钩子类型。</param>
        /// <param name="lParam">此参数的含义取决于与当前钩链相关联的钩子类型。</param>
        /// <returns></returns>
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        #endregion

        #region Functions
        #region Debugging Functions
        #region ReadProcessMemory
        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, out uint lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out byte lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out bool lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out char lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out short lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out ushort lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out int lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out uint lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out long lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out ulong lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out float lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out double lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out IntPtr lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesRead);
        #endregion

        #region WriteProcessMemory
        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, out uint lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref bool lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref byte lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref char lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref short lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref ushort lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref int lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref uint lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref long lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref ulong lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref float lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref double lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref IntPtr lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint nSize, uint* lpNumberOfBytesWritten);
        #endregion
        #endregion

        #region Dynamic-Link Library Functions
        /// <summary>
        /// 获取模块所在路径
        /// </summary>
        /// <param name="hModule">模块句柄</param>
        /// <param name="lpFilename">文件路径</param>
        /// <param name="nSize">缓冲区大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetModuleFileNameW", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, uint nSize);

        /// <summary>
        /// 获取当前进程中符合条件的模块句柄
        /// </summary>
        /// <param name="lpModuleName">模块名</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// 获取指定模块中导出函数的地址
        /// </summary>
        /// <param name="hModule">模块句柄</param>
        /// <param name="lpProcName">函数名</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        /// <summary>
        /// 加载DLL
        /// </summary>
        /// <param name="lpFileName">DLL路径</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
        #endregion

        #region Hook Functions
        /// <summary>
        /// 安装Windows消息钩子
        /// </summary>
        /// <param name="idHook">将安装的钩子的类型</param>
        /// <param name="lpfn">回调函数</param>
        /// <param name="hMod">回调函数所在模块的句柄。如果 dwThreadId 指定的线程由当前进程创建并且回调函数在当前进程中，参数必须设置为 IntPtr.Zero</param>
        /// <param name="dwThreadId">与回调函数关联的线程ID，0为全局钩子</param>
        /// <returns>返回钩子的句柄</returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetWindowsHookExW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(uint idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// 卸载Windows消息钩子
        /// </summary>
        /// <param name="hhk">要卸载的钩子的句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "UnhookWindowsHookEx", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// 将钩子信息传递给当前钩子链中的下一个钩子过程。挂钩过程可以在处理挂钩信息之前或之后调用此函数。
        /// </summary>
        /// <param name="hhk">当前钩子的句柄，可以不填写</param>
        /// <param name="nCode">钩子代码传递给当前的钩子过程。下一个钩子过程使用此代码来确定如何处理挂钩信息。</param>
        /// <param name="wParam">所述的wParam传递给当前挂钩过程值。此参数的含义取决于与当前钩链相关联的钩子类型。</param>
        /// <param name="lParam">所述的lParam传递给当前挂钩过程值。此参数的含义取决于与当前钩链相关联的钩子类型。</param>
        /// <returns>该值由链中的下一个钩子过程返回。当前的钩子过程也必须返回此值。返回值的含义取决于钩子类型。有关详细信息，请参阅各个挂钩过程的说明。</returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CallNextHookEx", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        #endregion

        #region Keyboard Input Functions
        /// <summary>
        /// 将虚拟键的状态拷贝到缓冲区
        /// </summary>
        /// <param name="lpKeyState">指向一个256字节的数组，数组用于接收每个虚拟键的状态。</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetKeyboardState", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        /// <summary>
        /// 获取虚拟键状态
        /// </summary>
        /// <param name="nVirtKey"></param>
        /// <returns>高位为1，表示按下，为0表示未按下。低位为1，表示虚拟键被切换。比如按下Caps Lock键，低位为1，反之低位为0</returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetKeyState", ExactSpelling = true, SetLastError = true)]
        public static extern short GetKeyState(int nVirtKey);

        /// <summary>
        /// 该函数将指定的虚拟键码和键盘状态翻译为相应的字符或字符串。该函数使用由给定的键盘布局句柄标识的物理键盘布局和输入语言来翻译代码。
        /// </summary>
        /// <param name="uVirtKey">指定要翻译的虚拟键码。</param>
        /// <param name="uScanCode">定义被翻译键的硬件扫描码。若该键处于Up状态，则该值的最高位被设置。</param>
        /// <param name="lpKeyState">指向包含当前键盘状态的一个256字节数组。数组的每个成员包含一个键的状态。若某字节的最高位被设置，则该键处于Down状态。若最低位被设置，则表明该键被触发。在此函数中，仅有Caps Lock键的触发位是相关的。Num Lock和Scroll Lock键的触发状态将被忽略。</param>
        /// <param name="lpChar">指向接受翻译所得字符或字符串的缓冲区。</param>
        /// <param name="uFlags">定义一个菜单是否处于激活状态。若一菜单是活动的，则该参数为1，否则为0。</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ToAscii", ExactSpelling = true, SetLastError = true)]
        public static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, out char lpChar, uint uFlags);
        #endregion

        #region Memory Management Functions
        /// <summary>
        /// 在当前进程中分配内存
        /// </summary>
        /// <param name="lpAddress">指定一个地址用于分配内存（如果为IntPtr.Zero则自动分配）</param>
        /// <param name="dwSize">要分配内存的大小</param>
        /// <param name="flAllocationType">内存分配选项</param>
        /// <param name="flProtect">内存保护选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "VirtualAlloc", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, [MarshalAs(UnmanagedType.SysUInt)] uint dwSize, uint flAllocationType, uint flProtect);

        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpAddress">指定一个地址用于分配内存（如果为IntPtr.Zero则自动分配）</param>
        /// <param name="dwSize">要分配内存的大小</param>
        /// <param name="flAllocationType">内存分配选项</param>
        /// <param name="flProtect">内存保护选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "VirtualAllocEx", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, [MarshalAs(UnmanagedType.SysUInt)] uint dwSize, uint flAllocationType, uint flProtect);

        /// <summary>
        /// 在当前进程中释放内存
        /// </summary>
        /// <param name="lpAddress">指定释放内存的地址</param>
        /// <param name="dwSize">要释放内存的大小</param>
        /// <param name="dwFreeType">内存释放选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "VirtualFree", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFree(IntPtr lpAddress, [MarshalAs(UnmanagedType.SysUInt)] uint dwSize, uint dwFreeType);

        /// <summary>
        /// 释放内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpAddress">指定释放内存的地址</param>
        /// <param name="dwSize">要释放内存的大小</param>
        /// <param name="dwFreeType">内存释放选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "VirtualFreeEx", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, [MarshalAs(UnmanagedType.SysUInt)] uint dwSize, uint dwFreeType);

        /// <summary>
        /// 查询地址空间中内存地址的信息
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpAddress">查询内存的地址</param>
        /// <param name="lpBuffer">内存页面信息</param>
        /// <param name="dwLength">MEMORY_BASIC_INFORMATION结构的大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "VirtualQueryEx", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, [MarshalAs(UnmanagedType.SysUInt)] uint dwLength);
        #endregion

        #region Message Functions
        /// <summary>
        /// 向线程发送消息
        /// </summary>
        /// <param name="idThread">线程ID</param>
        /// <param name="Msg">消息类型</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "PostThreadMessageW", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostThreadMessage(uint idThread, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 同步方法发送消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="Msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SendMessageW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        #endregion

        #region Process and Thread Functions
        /// <summary>
        /// 将一个线程的输入处理机制附加或分离到另一个线程的输入处理机制
        /// </summary>
        /// <param name="idAttach">附加线程的ID，不能是系统线程</param>
        /// <param name="idAttachTo">被附加线程的ID，不能是系统线程</param>
        /// <param name="fAttach">true为附加，false为分离</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "AttachThreadInput", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        /// <summary>
        /// 关闭句柄
        /// </summary>
        /// <param name="hObject">句柄</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CloseHandle", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// 创建远程线程
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpThreadAttributes"></param>
        /// <param name="dwStackSize">堆栈初始大小，如果为0，则使用系统默认</param>
        /// <param name="lpStartAddress">远程进程中线程的起始地址</param>
        /// <param name="lpParameter">指向要传递给线程函数的变量的指针</param>
        /// <param name="dwCreationFlags">线程创建的标志</param>
        /// <param name="lpThreadId">指向接收线程标识符的变量的指针</param>
        /// <returns>如果函数成功，返回值是新线程的句柄，否则返回值是IntPtr.Zero</returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateRemoteThread", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern IntPtr CreateRemoteThread(IntPtr hProcess, void* lpThreadAttributes, [MarshalAs(UnmanagedType.SysUInt)] uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, uint* lpThreadId);

        /// <summary>
        /// 获取当前进程ID
        /// </summary>
        /// <returns>线程ID</returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentProcessId", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetCurrentProcessId();

        /// <summary>
        /// 获取当前线程ID
        /// </summary>
        /// <returns>线程ID</returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentThreadId", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetCurrentThreadId();

        /// <summary>
        /// Retrieves the termination status of the specified thread.
        /// </summary>
        /// <param name="hThread">A handle to the thread.</param>
        /// <param name="lpExitCode">A pointer to a variable to receive the thread termination status.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetExitCodeThread", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

        /// <summary>
        /// 获取线程所在进程的ID
        /// </summary>
        /// <param name="Thread">线程句柄</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetProcessIdOfThread", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetProcessIdOfThread(IntPtr Thread);

        /// <summary>
        /// 获取指定的进程是否在WOW64下运行
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="Wow64Process">
        /// 32位进程运行在32位Windows下：false
        /// 32位进程运行在64位Windows下：true
        /// 64位进程运行在64位Windows下：false
        /// </param>
        /// <returns>返回值是函数是否执行成功，而不是是否为64位进程！！！</returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "IsWow64Process", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool Wow64Process);

        /// <summary>
        /// 打开进程
        /// </summary>
        /// <param name="dwDesiredAccess">权限</param>
        /// <param name="bInheritHandle">是否继承</param>
        /// <param name="dwProcessId">进程ID</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "OpenProcess", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        /// <summary>
        /// 打开线程
        /// </summary>
        /// <param name="dwDesiredAccess">权限</param>
        /// <param name="bInheritHandle">是否继承</param>
        /// <param name="dwThreadId">线程ID</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "OpenThread", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        /// <summary>
        /// 打开进程
        /// </summary>
        /// <param name="dwDesiredAccess">权限</param>
        /// <param name="bInheritHandle">是否继承</param>
        /// <param name="dwProcessId">进程ID</param>
        /// <returns></returns>
        public static SafeNativeHandle SafeOpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId) => OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId);

        /// <summary>
        /// 打开线程
        /// </summary>
        /// <param name="dwDesiredAccess">权限</param>
        /// <param name="bInheritHandle">是否继承</param>
        /// <param name="dwThreadId">线程ID</param>
        /// <returns></returns>
        public static SafeNativeHandle SafeOpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId) => OpenThread(dwDesiredAccess, bInheritHandle, dwThreadId);
        #endregion

        #region PSAPI Functions
        /// <summary>
        /// 遍历所有进程ID
        /// </summary>
        /// <param name="pProcessIds">进程ID</param>
        /// <param name="cb"></param>
        /// <param name="pBytesReturned"></param>
        /// <returns></returns>
        [DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "EnumProcesses", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcesses(ref uint pProcessIds, uint cb, out uint pBytesReturned);

        /// <summary>
        /// 遍历进程的所有模块
        /// </summary>
        /// <param name="hProcess">进程的句柄</param>
        /// <param name="lphModule">模块句柄</param>
        /// <param name="cb">储存模块句柄的字节数</param>
        /// <param name="lpcbNeeded">储存所有模块句柄所需的字节数</param>
        /// <returns></returns>
        [DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "EnumProcessModules", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool EnumProcessModules(IntPtr hProcess, IntPtr* lphModule, uint cb, out uint lpcbNeeded);

        /// <summary>
        /// 遍历进程的所有模块
        /// </summary>
        /// <param name="hProcess">进程的句柄</param>
        /// <param name="lphModule">模块句柄</param>
        /// <param name="cb">储存模块句柄的字节数</param>
        /// <param name="lpcbNeeded">储存所有模块句柄所需的字节数</param>
        /// <param name="dwFilterFlag">过滤条件</param>
        /// <returns></returns>
        [DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "EnumProcessModulesEx", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool EnumProcessModulesEx(IntPtr hProcess, IntPtr* lphModule, uint cb, out uint lpcbNeeded, uint dwFilterFlag);

        /// <summary>
        /// 获取模块名
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="hModule">模块句柄</param>
        /// <param name="lpBaseName">模块名</param>
        /// <param name="nSize">最大模块名长度</param>
        /// <returns>成功将返回非零整数</returns>
        [DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetModuleBaseNameW", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetModuleBaseName(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, uint nSize);

        /// <summary>
        /// 获取进程路径
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpImageFileName">进程路径</param>
        /// <param name="nSize">缓存区大小</param>
        /// <returns></returns>
        [DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetProcessImageFileNameW", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetProcessImageFileName(IntPtr hProcess, StringBuilder lpImageFileName, uint nSize);
        #endregion

        #region Shell Functions
        /// <summary>
        /// 启动程序
        /// </summary>
        /// <param name="pExecInfo">选项</param>
        /// <returns></returns>
        [DllImport("shell32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ShellExecuteExW", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO pExecInfo);
        #endregion

        #region Synchronization Functions
        /// <summary>
        /// 等待对象被关闭
        /// </summary>
        /// <param name="hHandle">对象的句柄</param>
        /// <param name="dwMilliseconds">最多等待多少毫秒</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WaitForSingleObject", ExactSpelling = true, SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        #endregion

        #region Window Functions
        /// <summary>
        /// 遍历所有子窗口
        /// </summary>
        /// <param name="hWndParent">父窗口句柄</param>
        /// <param name="lpEnumFunc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "EnumChildWindows", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// 遍历所有顶级窗口
        /// </summary>
        /// <param name="lpEnumFunc"></param>
        /// <param name="lParam">自定义参数</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "EnumWindows", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="lpClassName">窗口类名</param>
        /// <param name="lpWindowName">窗口标题</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindWindowW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="hWndParent">父窗口句柄</param>
        /// <param name="hWndChildAfter">从此窗口之后开始查找（此窗口必须为父窗口的直接子窗口）</param>
        /// <param name="lpszClass">窗口类名</param>
        /// <param name="lpszWindow">窗口标题</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindWindowExW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// 获取当前顶端窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetForegroundWindow", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 获取Program Manager的窗口句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetShellWindow", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetShellWindow();

        /// <summary>
        /// 获取某个窗口的创建者的线程ID和进程ID
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="lpdwProcessId">进程ID</param>
        /// <returns>线程ID</returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetWindowThreadProcessId", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern uint GetWindowThreadProcessId(IntPtr hWnd, uint* lpdwProcessId);

        /// <summary>
        /// 是否为有效窗口
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "IsWindow", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        ///// <summary>
        ///// 设置父窗口
        ///// </summary>
        ///// <param name="hWndChild">子窗口句柄</param>
        ///// <param name="hWndNewParent">新的父窗口的句柄</param>
        ///// <returns></returns>
        //[DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetParent", ExactSpelling = true, SetLastError = true)]
        //public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 设置窗口位置
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="hWndInsertAfter">窗口Z序</param>
        /// <param name="x">左上顶点x坐标</param>
        /// <param name="y">左上顶点y坐标</param>
        /// <param name="cx">长度</param>
        /// <param name="cy">高度</param>
        /// <param name="uFlags">选项</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetWindowPos", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        /// <summary>
        /// 激活窗口
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetActiveWindow", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetActiveWindow(IntPtr hWnd);

        /// <summary>
        /// 设置焦点
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetFocus", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        /// <summary>
        /// 窗口置顶
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetForegroundWindow", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        #region ntdll.dll
        /// <summary>
        /// 恢复进程
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns>If the function succeeds, the return value is the thread's previous suspend count.If the function fails, the return value is (DWORD) -1. To get extended error information, call GetLastError.</returns>
        [DllImport("ntdll.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ZwResumeProcess", ExactSpelling = true, SetLastError = true)]
        public static extern uint ZwResumeProcess(IntPtr hProcess);

        /// <summary>
        /// 暂停进程
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns>If the function succeeds, the return value is the thread's previous suspend count; otherwise, it is (DWORD) -1. To get extended error information, use the GetLastError function.</returns>
        [DllImport("ntdll.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ZwSuspendProcess", ExactSpelling = true, SetLastError = true)]
        public static extern uint ZwSuspendProcess(IntPtr hProcess);
        #endregion
        #endregion

        /// <summary>
        /// 获取所有不支持的本地方法，可以通过反射调用此方法
        /// <code>string[] notSupportedMethod = (string[])Assembly.Load("FastWin32").GetType("FastWin32.NativeMethods").GetMethod("SelfCheck", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);</code>
        /// </summary>
        /// <returns></returns>
        public static string[] SelfCheck()
        {
            List<string> methodList;
            Type dllImportType;
            DllImportAttribute[] dllImportAttributes;
            IntPtr moduleHandle;

            methodList = new List<string>();
            dllImportType = typeof(DllImportAttribute);
            foreach (MethodInfo methodInfo in typeof(NativeMethods).GetMethods())
            {
                dllImportAttributes = (DllImportAttribute[])methodInfo.GetCustomAttributes(dllImportType, false);
                if (dllImportAttributes == null || dllImportAttributes.Length == 0)
                    continue;
                moduleHandle = LoadLibrary(dllImportAttributes[0].Value);
                if (moduleHandle == IntPtr.Zero)
                    methodList.Add(dllImportAttributes[0].EntryPoint);
                if (GetProcAddress(moduleHandle, dllImportAttributes[0].EntryPoint) == IntPtr.Zero)
                    methodList.Add(dllImportAttributes[0].EntryPoint);
            }
            return methodList.ToArray();
        }
    }
}
