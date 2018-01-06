using System;
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
        /// 无效句柄
        /// </summary>
        public static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);

        /// <summary>
        /// 表示当前进程的伪句柄
        /// </summary>
        public static readonly IntPtr CURRENT_PROCESS = (IntPtr)(-1);

        /// <summary>
        /// 无限超时等待
        /// </summary>
        public const uint INFINITE = 0xFFFFFFFF;

        /// <summary>
        /// 枚举进程模块过滤标志
        /// </summary>
        public static class EnumModulesFilterFlag
        {
            /// <summary>
            /// 默认
            /// </summary>
            public const uint DEFAULT = 0x0;

            /// <summary>
            /// 列出32位模块
            /// </summary>
            public const uint X86 = 0x1;

            /// <summary>
            /// 列出64位模块
            /// </summary>
            public const uint X64 = 0x2;

            /// <summary>
            /// 列出所有模块
            /// </summary>
            public const uint ALL = 0x3;
        }

        #region Date Time
        public const uint DTM_FIRST = 0x1000;

        public const uint DTM_GETSYSTEMTIME = DTM_FIRST + 1;

        public const uint DTM_SETSYSTEMTIME = DTM_FIRST + 2;

        public const uint GDT_ERROR = unchecked((uint)-1);

        public const uint GDT_VALID = 0;

        public const uint GDT_NONE = 1;
        #endregion

        #region File Share Mode
        public const uint FILE_SHARE_DELETE = 0x00000004;

        public const uint FILE_SHARE_READ = 0x00000001;

        public const uint FILE_SHARE_WRITE = 0x00000002;
        #endregion

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

        #region List View
        public const uint LVM_FIRST = 0x1000;

        public const uint LVM_GETITEMCOUNT = LVM_FIRST + 4;

        public const uint LVM_DELETEITEM = LVM_FIRST + 8;

        public const uint LVM_SETITEMPOSITION = LVM_FIRST + 15;

        public const uint LVM_GETITEMPOSITION = LVM_FIRST + 16;

        public const uint LVM_REDRAWITEMS = LVM_FIRST + 21;

        public const uint LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;

        public const uint LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55;

        public const uint LVM_GETITEM = LVM_FIRST + 75;

        public const uint LVM_GETITEMTEXT = LVM_FIRST + 115;

        /// <summary>
        /// The pszText member is valid or must be set.
        /// </summary>
        public const uint LVIF_TEXT = 0x00000001;

        /// <summary>
        /// The iImage member is valid or must be set.
        /// </summary>
        public const uint LVIF_IMAGE = 0x00000002;

        /// <summary>
        /// The lParam member is valid or must be set.
        /// </summary>
        public const uint LVIF_PARAM = 0x00000004;

        /// <summary>
        /// The state member is valid or must be set.
        /// </summary>
        public const uint LVIF_STATE = 0x00000008;

        /// <summary>
        /// The iIndent member is valid or must be set.
        /// </summary>
        public const uint LVIF_INDENT = 0x00000010;

        /// <summary>
        /// The control will not generate LVN_GETDISPINFO to retrieve text information if it receives an LVM_GETITEM message. Instead, the pszText member will contain LPSTR_TEXTCALLBACK.
        /// </summary>
        public const uint LVIF_NORECOMPUTE = 0x00000800;

        /// <summary>
        /// The iGroupId member is valid or must be set. If this flag is not set when an LVM_INSERTITEM message is sent, the value of iGroupId is assumed to be I_GROUPIDCALLBACK.
        /// </summary>
        public const uint LVIF_GROUPID = 0x00000100;

        /// <summary>
        /// The cColumns member is valid or must be set.
        /// </summary>
        public const uint LVIF_COLUMNS = 0x00000200;

        /// <summary>
        /// The piColFmt member is valid or must be set. If this flag is used, the cColumns member is valid or must be set.
        /// </summary>
        public const uint LVIF_COLFMT = 0x00010000;
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
        /// A handle that has the <see cref="PROCESS_QUERY_INFORMATION"/> access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION.
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
        /// Required to perform an operation on the address space of a process (see VirtualProtectEx and <see cref="O:WriteProcessMemory"/>).
        /// </summary>
        public const uint PROCESS_VM_OPERATION = 0x0008;

        /// <summary>
        /// Required to read memory in a process using <see cref="O:ReadProcessMemory"/>.
        /// </summary>
        public const uint PROCESS_VM_READ = 0x0010;

        /// <summary>
        /// Required to write to memory in a process using <see cref="O:WriteProcessMemory"/>.
        /// </summary>
        public const uint PROCESS_VM_WRITE = 0x0020;

        /// <summary>
        /// All possible access rights for a process object.
        /// </summary>
        public const uint PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF;
        #endregion

        #region Thread Creation
        /// <summary>
        /// 创建后立即运行
        /// </summary>
        public const uint CREATE_RUNNING = 0;

        /// <summary>
        /// 线程被创建为挂起状态
        /// </summary>
        public const uint CREATE_SUSPENDED = 0x00000004;

        /// <summary>
        /// 指定堆栈大小
        /// </summary>
        public const uint STACK_SIZE_PARAM_IS_A_RESERVATION = 0x00010000;
        #endregion

        #region Volume Management
        /// <summary>
        /// The drive type cannot be determined.
        /// </summary>
        public const uint DRIVE_UNKNOWN = 0;

        /// <summary>
        /// The root path is invalid; for example, there is no volume mounted at the specified path.
        /// </summary>
        public const uint DRIVE_NO_ROOT_DIR = 1;

        /// <summary>
        /// The drive has removable media; for example, a floppy drive, thumb drive, or flash card reader.
        /// </summary>
        public const uint DRIVE_REMOVABLE = 2;

        /// <summary>
        /// The drive has fixed media; for example, a hard disk drive or flash drive.
        /// </summary>
        public const uint DRIVE_FIXED = 3;

        /// <summary>
        /// The drive is a remote (network) drive.
        /// </summary>
        public const uint DRIVE_REMOTE = 4;

        /// <summary>
        /// The drive is a CD-ROM drive.
        /// </summary>
        public const uint DRIVE_CDROM = 5;

        /// <summary>
        /// The drive is a RAM disk.
        /// </summary>
        public const uint DRIVE_RAMDISK = 6;
        #endregion

        #region Window Messages
        public const uint WM_KEYDOWN = 0x100;

        public const uint WM_KEYUP = 0x101;

        public const uint WM_SYSKEYDOWN = 0x104;

        public const uint WM_SYSKEYUP = 0x105;

        public const uint WM_USER = 0x0400;
        #endregion
        #endregion

        #region Structures
        #region Authorization Structures
        /// <summary>
        /// The <see cref="SECURITY_ATTRIBUTES"/> structure contains the security descriptor for an object and specifies whether the handle retrieved by specifying this structure is inheritable.
        /// This structure provides security settings for objects created by various functions, such as <see cref="CreateFile"/>, CreatePipe, CreateProcess, RegCreateKeyEx, or RegSaveKeyEx.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SECURITY_ATTRIBUTES
        {
            /// <summary>
            /// The size, in bytes, of this structure.
            /// Set this value to the size of the <see cref="SECURITY_ATTRIBUTES"/> structure.
            /// </summary>
            public uint nLength;

            /// <summary>
            /// A pointer to a SECURITY_DESCRIPTOR structure that controls access to the object.
            /// If the value of this member is NULL, the object is assigned the default security descriptor associated with the access token of the calling process.
            /// This is not the same as granting access to everyone by assigning a NULL discretionary access control list (DACL).
            /// By default, the default DACL in the access token of a process allows access only to the user represented by the access token.
            /// </summary>
            public IntPtr lpSecurityDescriptor;

            /// <summary>
            /// A <see cref="bool"/> value that specifies whether the returned handle is inherited when a new process is created.
            /// If this member is true, the new process inherits the handle.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public bool bInheritHandle;
        }
        #endregion

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

        #region Memory Management Structures
        /// <summary>
        /// Contains information about a range of pages in the virtual address space of a process.
        /// The <see cref="VirtualQuery"/> and <see cref="VirtualQueryEx"/> functions use this structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MEMORY_BASIC_INFORMATION
        {
            /// <summary>
            /// A pointer to the base address of the region of pages.
            /// </summary>
            public IntPtr BaseAddress;

            /// <summary>
            /// A pointer to the base address of a range of pages allocated by the <see cref="VirtualAlloc"/> function.
            /// The page pointed to by the <see cref="BaseAddress"/> member is contained within this allocation range.
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
            /// The access protection of the pages in the region. This member is one of the values listed for the <see cref="AllocationProtect"/> member.
            /// </summary>
            public uint Protect;

            /// <summary>
            /// The type of pages in the region.
            /// </summary>
            public uint Type;

            /// <summary>
            /// 结构体在非托管内存中大小
            /// </summary>
            public static readonly uint Size = (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
        }
        #endregion

        #region Synchronization Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OVERLAPPED
        {
            public IntPtr Internal;

            public IntPtr InternalHigh;

            public Anonymous_7416d31a_1ce9_4e50_b1e1_0f2ad25c0196 Union1;

            public IntPtr hEvent;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Anonymous_7416d31a_1ce9_4e50_b1e1_0f2ad25c0196
        {
            [FieldOffset(0)]
            public Anonymous_ac6e4301_4438_458f_96dd_e86faeeca2a6 Struct1;

            [FieldOffset(0)]
            public IntPtr Pointer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Anonymous_ac6e4301_4438_458f_96dd_e86faeeca2a6
        {
            public uint Offset;

            public uint OffsetHigh;
        }
        #endregion
        #endregion

        #region Callback
        /// <summary>
        /// 回调函数 要继续枚举,返回true;要停止枚举,返回false
        /// </summary>
        /// <param name="hwnd">子级窗口的句柄</param>
        /// <param name="lParam">EnumWindows或EnumDesktopWindows中给出的应用程序定义值</param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool EnumChildProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// 回调函数 要继续枚举,返回true;要停止枚举,返回false
        /// </summary>
        /// <param name="hwnd">顶级窗口的句柄</param>
        /// <param name="lParam">EnumWindows或EnumDesktopWindows中给出的应用程序定义值</param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// <see cref="HookProc"/> 回调函数
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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out byte lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out bool lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out char lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out short lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out ushort lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out int lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out uint lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out long lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out ulong lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out float lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out double lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

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
        public static unsafe extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out IntPtr lpBuffer, uint nSize, uint* lpNumberOfBytesRead);
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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref bool lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref byte lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref char lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref short lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref ushort lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref int lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref uint lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref long lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref ulong lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref float lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref double lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref IntPtr lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

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
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);
        #endregion
        #endregion

        #region Device Management Functions
        /// <summary>
        /// Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.
        /// </summary>
        /// <param name="hDevice">A handle to the device on which the operation is to be performed. The device is typically a volume, directory, file, or stream. To retrieve a device handle, use the <see cref="CreateFile"/> function. For more information, see Remarks.</param>
        /// <param name="dwIoControlCode">The control code for the operation. This value identifies the specific operation to be performed and the type of device on which to perform it.</param>
        /// <param name="lpInBuffer">A pointer to the input buffer that contains the data required to perform the operation. The format of this data depends on the value of the dwIoControlCode parameter.</param>
        /// <param name="nInBufferSize">The size of the input buffer, in bytes.</param>
        /// <param name="lpOutBuffer">A pointer to the output buffer that is to receive the data returned by the operation. The format of this data depends on the value of the dwIoControlCode parameter.</param>
        /// <param name="nOutBufferSize">The size of the output buffer, in bytes.</param>
        /// <param name="lpBytesReturned">A pointer to a variable that receives the size of the data stored in the output buffer, in bytes.</param>
        /// <param name="lpOverlapped">A pointer to an OVERLAPPED structure.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "DeviceIoControl", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, byte[] lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, uint* lpBytesReturned, OVERLAPPED* lpOverlapped);
        #endregion

        #region Dynamic-Link Library Functions
        /// <summary>
        /// 获取指定模块中导出函数的地址
        /// </summary>
        /// <param name="hModule">模块句柄</param>
        /// <param name="lpProcName">函数名</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        /// <summary>
        /// 获取当前进程中符合条件的模块句柄
        /// </summary>
        /// <param name="lpModuleName">模块名</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        #region File Management Functions
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="lpFileName">文件名</param>
        /// <param name="dwDesiredAccess">权限</param>
        /// <param name="dwShareMode">共享模式</param>
        /// <param name="lpSecurityAttributes">指向SECURITY_ATTRIBUTES结构的指针</param>
        /// <param name="dwCreationDisposition">指定文件不存在时如何处理</param>
        /// <param name="dwFlagsAndAttributes">文件或设备属性和标志</param>
        /// <param name="hTemplateFile">具有GENERIC_READ访问权限的模板文件的有效句柄</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        #endregion

        #region Hook Functions
        /// <summary>
        /// 安装Windows消息钩子
        /// </summary>
        /// <param name="idHook">将安装的钩子的类型</param>
        /// <param name="lpfn">回调函数</param>
        /// <param name="hMod">回调函数所在模块的句柄。如果 dwThreadId 指定的线程由当前进程创建并且回调函数在当前进程中，参数必须设置为 <see cref="IntPtr.Zero"/></param>
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
        /// 该函数将指定的虚拟键码和键盘状态翻译为相应的字符或字符串。该函数使用由给定的键盘布局句柄标识的物理键盘布局和输入语言来翻译代码。
        /// </summary>
        /// <param name="uVirtKey">指定要翻译的虚拟键码。</param>
        /// <param name="uScanCode">定义被翻译键的硬件扫描码。若该键处于Up状态，则该值的最高位被设置。</param>
        /// <param name="lpKeyState">指向包含当前键盘状态的一个256字节数组。数组的每个成员包含一个键的状态。若某字节的最高位被设置，则该键处于Down状态。若最低位被设置，则表明该键被触发。在此函数中，仅有Caps Lock键的触发位是相关的。Num Lock和Scroll Lock键的触发状态将被忽略。</param>
        /// <param name="lpChar">指向接受翻译所得字符或字符串的缓冲区。</param>
        /// <param name="uFlags">定义一个菜单是否处于激活状态。若一菜单是活动的，则该参数为1，否则为0。</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ToAscii", ExactSpelling = true, SetLastError = true)]
        public static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, byte[] lpChar, uint uFlags);
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
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

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
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        /// <summary>
        /// 在当前进程中释放内存
        /// </summary>
        /// <param name="lpAddress">指定释放内存的地址</param>
        /// <param name="dwSize">要释放内存的大小</param>
        /// <param name="dwFreeType">内存释放选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "VirtualFree", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

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
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        /// <summary>
        /// 在当前进程中查询地址空间中内存地址的信息
        /// </summary>
        /// <param name="lpAddress">查询内存的地址</param>
        /// <param name="lpBuffer">内存页面信息</param>
        /// <param name="dwLength">MEMORY_BASIC_INFORMATION结构的大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "VirtualQuery", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualQuery(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        /// <summary>
        /// 查询地址空间中内存地址的信息
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpAddress">查询内存的地址</param>
        /// <param name="lpBuffer">内存页面信息</param>
        /// <param name="dwLength">MEMORY_BASIC_INFORMATION结构的大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "VirtualQueryEx", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
        #endregion

        #region Message Functions
        /// <summary>
        /// 同步方法发送消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="Msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SendMessageW", ExactSpelling = true, SetLastError = true)]
        public static extern uint SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 异步方法发送消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="Msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "PostMessageW", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        #endregion

        #region Process and Thread Functions
        /// <summary>
        /// 将一个线程的输入处理机制附加或分离到另一个线程的输入处理机制
        /// </summary>
        /// <param name="idAttach">附加线程的ID，不能是系统线程</param>
        /// <param name="idAttachTo">被附加线程的ID，不能是系统线程</param>
        /// <param name="fAttach">true为附加，false为分离</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "AttachThreadInput", ExactSpelling = true, SetLastError = true)]
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
        public static unsafe extern IntPtr CreateRemoteThread(IntPtr hProcess, SECURITY_ATTRIBUTES* lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, uint* lpThreadId);

        /// <summary>
        /// 获取当前线程ID
        /// </summary>
        /// <returns>线程ID</returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentThreadId", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetCurrentThreadId();

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
        #endregion

        #region PSAPI Functions
        /// <summary>
        /// 枚举进程的所有模块
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

        #region Synchronization Functions
        /// <summary>
        /// 等待对象
        /// </summary>
        /// <param name="hHandle">句柄</param>
        /// <param name="dwMilliseconds">超时间隔，以毫秒为单位</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WaitForSingleObject", ExactSpelling = true, SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        #endregion

        #region Volume Management Functions
        /// <summary>
        /// Retrieves the name of a volume on a computer.
        /// <see cref="FindFirstVolume"/> is used to begin scanning the volumes of a computer.
        /// </summary>
        /// <param name="lpszVolumeName">A pointer to a buffer that receives a null-terminated string that specifies a volume <see cref="Guid"/> path for the first volume that is found.</param>
        /// <param name="cchBufferLength">The length of the buffer to receive the volume <see cref="Guid"/> path.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindFirstVolumeW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr FindFirstVolume(StringBuilder lpszVolumeName, uint cchBufferLength);

        /// <summary>
        /// Continues a volume search started by a call to the <see cref="FindFirstVolume"/> function.
        /// <see cref="FindNextVolume"/> finds one volume per call.
        /// </summary>
        /// <param name="hFindVolume">The volume search handle returned by a previous call to the <see cref="FindFirstVolume"/> function.</param>
        /// <param name="lpszVolumeName">A pointer to a string that receives the volume <see cref="Guid"/> path that is found.</param>
        /// <param name="cchBufferLength">The length of the buffer that receives the volume <see cref="Guid"/> path, in TCHARs.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindNextVolumeW", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindNextVolume(IntPtr hFindVolume, StringBuilder lpszVolumeName, uint cchBufferLength);

        /// <summary>
        /// 获取驱动器类型
        /// </summary>
        /// <param name="lpRootPathName">驱动器根目录名</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetDriveTypeW", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetDriveType(string lpRootPathName);

        /// <summary>
        /// 检索装入指定路径的卷装入点
        /// </summary>
        /// <param name="lpszFileName">指向输入路径字符串的指针。绝对和相对文件和目录名称，例如“..”，都可以在此路径中使用</param>
        /// <param name="lpszVolumePathName">指向接收输入路径的卷装载点的字符串的指针</param>
        /// <param name="cchBufferLength">输出缓冲区的长度</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetVolumePathNameW", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetVolumePathName(string lpszFileName, StringBuilder lpszVolumePathName, uint cchBufferLength);

        /// <summary>
        /// 检索指定卷的驱动器号和已装入文件夹路径的列表
        /// </summary>
        /// <param name="lpszVolumeName">卷的卷Guid路径。卷Guid路径的格式为"\\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\"</param>
        /// <param name="lpszVolumePathNames">指向缓冲区的指针，该缓冲区接收驱动器号和挂载的文件夹路径列表。该列表是由一个额外的NULL字符终止的以空字符结尾的字符串的数组。如果缓冲区不足以保存完整列表，则缓冲区将尽可能多地保留列表。</param>
        /// <param name="cchBufferLength">TCHAR中的lpszVolumePathNames缓冲区 的长度，包括所有NULL字符。</param>
        /// <param name="lpcchReturnLength">如果调用成功，则此参数是复制到lpszVolumePathNames缓冲区的TCHAR数量。否则，这个参数是TCHARs中保存完整列表所需的缓冲区的大小。</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetVolumePathNamesForVolumeNameW", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool GetVolumePathNamesForVolumeName(string lpszVolumeName, StringBuilder lpszVolumePathNames, uint cchBufferLength,  uint* lpcchReturnLength);
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
        /// 获取整个屏幕的窗口句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetDesktopWindow", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();

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
        /// 获取任务栏的窗口句柄（任务栏的一部分）
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetTaskmanWindow", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetTaskmanWindow();

        /// <summary>
        /// 获取某个窗口的创建者的线程ID和进程ID
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="lpdwProcessId">进程ID</param>
        /// <returns>线程ID</returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetWindowThreadProcessId", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// 是否为有效窗口
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "IsWindow", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// 设置父窗口
        /// </summary>
        /// <param name="hWndChild">子窗口句柄</param>
        /// <param name="hWndNewParent">新的父窗口的句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetParent", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

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
    }
}
