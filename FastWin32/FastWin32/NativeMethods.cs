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
        #region const
        /// <summary>
        /// 模块名最大长度
        /// </summary>
        public const uint MAX_MODULE_NAME32 = 255;
        public const uint DELETE = 0x00010000;
        public const uint READ_CONTROL = 0x00020000;
        public const uint WRITE_DAC = 0x00040000;
        public const uint WRITE_OWNER = 0x00080000;
        public const uint SYNCHRONIZE = 0x00100000;
        /// <summary>
        /// 标准权限
        /// </summary>
        public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        /// <summary>
        /// 无效句柄
        /// </summary>
        public static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);
        /// <summary>
        /// 表示当前进程的伪句柄
        /// </summary>
        public static readonly IntPtr CURRENT_PROCESS = (IntPtr)(-1);
        #region message
        public const uint WM_USER = 0x0400;
        public const uint LVM_FIRST = 0x1000;
        public const uint LVM_GETITEMCOUNT = LVM_FIRST + 4;
        public const uint LVM_DELETEITEM = LVM_FIRST + 8;
        public const uint LVM_SETITEMPOSITION = LVM_FIRST + 15;
        public const uint LVM_GETITEMPOSITION = LVM_FIRST + 16;
        public const uint LVM_REDRAWITEMS = LVM_FIRST + 21;
        public const uint LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        public const uint LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55;
        public const uint DTM_FIRST = 0x1000;
        public const uint DTM_GETSYSTEMTIME = DTM_FIRST + 1;
        public const uint DTM_SETSYSTEMTIME = DTM_FIRST + 2;
        public const uint GDT_ERROR = unchecked((uint)-1);
        public const uint GDT_VALID = 0;
        public const uint GDT_NONE = 1;
        #endregion
        #endregion

        #region enum
        /// <summary>
        /// 访问权限
        /// </summary>
        [Flags]
        public enum ProcAccessFlags : uint
        {
            /// <summary>
            /// 使用TerminateProcess结束进程
            /// </summary>
            PROCESS_TERMINATE = 0x0001,

            /// <summary>
            /// 创建线程
            /// </summary>
            PROCESS_CREATE_THREAD = 0x0002,

            /// <summary>
            /// 不明
            /// </summary>
            PROCESS_SET_SESSIONID = 0x0004,

            /// <summary>
            /// 操作进程的内存
            /// </summary>
            PROCESS_VM_OPERATION = 0x0008,

            /// <summary>
            /// 使用ReadProcessMemory读取内存
            /// </summary>
            PROCESS_VM_READ = 0x0010,

            /// <summary>
            /// 使用WriteProcessMemory写入内存
            /// </summary>
            PROCESS_VM_WRITE = 0x0020,

            /// <summary>
            /// 使用DuplicateHandle复制句柄
            /// </summary>
            PROCESS_DUP_HANDLE = 0x0040,

            /// <summary>
            /// 创建进程
            /// </summary>
            PROCESS_CREATE_PROCESS = 0x0080,

            /// <summary>
            /// 使用SetProcessWorkingSetSize设置进程实际使用内存大小
            /// </summary>
            PROCESS_SET_QUOTA = 0x0100,

            /// <summary>
            /// 设置进程相关信息，比如使用SetPriorityClass设置进程优先级
            /// </summary>
            PROCESS_SET_INFORMATION = 0x0200,

            /// <summary>
            /// Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken).
            /// </summary>
            PROCESS_QUERY_INFORMATION = 0x0400,

            /// <summary>
            /// 不明，看起来是暂停/继续
            /// </summary>
            PROCESS_SUSPEND_RESUME = 0x0800,

            /// <summary>
            /// Required to retrieve certain information about a process (see GetExitCodeProcess, GetPriorityClass, IsProcessInJob, QueryFullProcessImageName).
            /// A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION.
            /// </summary>
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,

            /// <summary>
            /// 不明
            /// </summary>
            PROCESS_SET_LIMITED_INFORMATION = 0x2000,

            /// <summary>
            /// 所有权限（不建议使用，据说容易被报毒）
            /// </summary>
            PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF,
        }

        /// <summary>
        /// 枚举进程模块过滤标志
        /// </summary>
        public enum EnumModulesFilterFlag : uint
        {
            /// <summary>
            /// 默认
            /// </summary>
            DEFAULT = 0x0,

            /// <summary>
            /// 列出32位模块
            /// </summary>
            X86 = 0x1,

            /// <summary>
            /// 列出64位模块
            /// </summary>
            X64 = 0x2,

            /// <summary>
            /// 列出所有模块
            /// </summary>
            ALL = 0x3
        }

        /// <summary>
        /// 内存分配选项（直接Google翻译了，自己翻译挺麻烦的）
        /// </summary>
        [Flags]
        public enum MemoryAllocationFlags : uint
        {
            #region must contain
            /// <summary>
            /// 为指定的预留内存页面分配内存费用（从磁盘上的内存和分页文件的总体大小）。该函数还保证当调用者稍后初次访问存储器时，内容将为零。除非/直到虚拟地址被实际访问，否则实际物理页面不会被分配。
            /// 为了保存和提交一步到位的网页，呼吁 VirtualAllocEx来用 MEM_COMMIT | MEM_RESERVE。
            /// 尝试通过指定MEM_COMMIT而不使用 MEM_RESERVE和非NULL lpAddress来提交特定的地址范围，除非已经保留了整个范围。生成的错误代码为ERROR_INVALID_ADDRESS。
            /// 尝试提交已经提交的页面不会导致该功能失败。这意味着您可以在不首先确定每个页面的当前承诺状态的情况下提交页面。
            /// 如果lpAddress指定了一个地址，flAllocationType必须是MEM_COMMIT。
            /// </summary>
            MEM_COMMIT = 0x00001000,

            /// <summary>
            /// 保留进程的虚拟地址空间的范围，而不会在内存中或磁盘上的分页文件中分配任何实际物理存储。
            /// 您通过使用MEM_COMMIT再次 调用VirtualAllocEx来提交保留的页面 。为了保存和提交一步到位的网页，呼吁 VirtualAllocEx来用 。MEM_COMMIT | MEM_RESERVE
            /// 其他内存分配功能（如malloc和 localAlloc）在释放之前不能使用保留的内存。
            /// </summary>
            MEM_RESERVE = 0x00002000,

            /// <summary>
            /// 表示由lpAddress和 dwSize指定的内存范围内的数据不再受关注。页面不应从页面文件读取或写入页面文件。但是，内存块将在以后再次被使用，所以不应该被分解。该值不能与任何其他值一起使用。
            /// 使用此值不能保证用MEM_RESET操作的范围 将包含零。如果要使范围包含零，请解除内存，然后重新提交。
            /// 当您使用MEM_RESET时， VirtualAllocEx函数将忽略fProtect的值 。但是，您仍然必须将fProtect设置为有效的保护值，例如PAGE_NOACCESS。
            /// 如果使用MEM_RESET并且将内存范围映射到文件，VirtualAllocEx将返回错误 。共享视图只有在映射到页面文件时才可以接受。
            /// </summary>
            MEM_RESET = 0x00080000,

            /// <summary>
            /// MEM_RESET_UNDO只应在哪个地址范围被称为 MEM_RESET成功之前应用。它指示由lpAddress和dwSize指定的指定内存范围内的数据 对调用者感兴趣，并尝试反转MEM_RESET的影响。如果功能成功，则表示指定地址范围内的所有数据都是完整的。如果函数失败，则地址范围中的至少一些数据已被替换为零。
            /// 该值不能与任何其他值一起使用。如果MEM_RESET_UNDO在一个地址范围上被调用，这个地址范围不是早于MEM_RESET，行为是未定义的。指定MEM_RESET时， VirtualAllocEx函数将忽略flProtect的值 。但是，您仍然必须将flProtect设置为有效的保护值，例如PAGE_NOACCESS。
            /// 在Windows Server 2008 R2，Windows 7中，在Windows Server 2008，Windows Vista中，Windows Server 2003和Windows XP中：  该MEM_RESET_UNDO标志之前，不支持Windows 8和Windows Server 2012中。
            /// </summary>
            MEM_RESET_UNDO = 0x1000000,
            #endregion

            #region optional
            /// <summary>
            /// 使用大页面支持分配内存。
            /// 大小和对齐方式必须是大页面最小值的倍数。要获取此值，请使用 GetLargePageMinimum函数。
            /// 如果指定此值，还必须指定MEM_RESERVE和MEM_COMMIT。
            /// </summary>
            MEM_LARGE_PAGES = 0x20000000,

            /// <summary>
            /// 保留可用于映射地址窗口扩展（AWE）页面的地址范围 。
            /// 该值必须与MEM_RESERVE一起使用，不能使用其他值。
            /// </summary>
            MEM_PHYSICAL = 0x00400000,

            /// <summary>
            /// 以尽可能高的地址分配内存。这可能比常规分配更慢，特别是当有很多分配时。
            /// </summary>
            MEM_TOP_DOWN = 0x00100000
            #endregion
        }

        /// <summary>
        /// 内存保护选项（直接Google翻译了，自己翻译挺麻烦的）
        /// </summary>
        [Flags]
        public enum MemoryProtectionFlags : uint
        {
            #region must contain
            /// <summary>
            /// 启用对页面的提交区域的执行访问。写入承诺区域的尝试导致访问冲突。
            /// CreateFileMapping函数不支持此标志 。
            /// </summary>
            PAGE_EXECUTE = 0x10,

            /// <summary>
            /// 启用对已提交的页面区域的执行或只读访问。写入承诺区域的尝试导致访问冲突。
            /// </summary>
            PAGE_EXECUTE_READ = 0x20,

            /// <summary>
            /// 启用对已提交的页面区域的执行，只读或读/写访问。
            /// </summary>
            PAGE_EXECUTE_READWRITE = 0x40,

            /// <summary>
            /// 对文件映射对象的映射视图启用执行，只读或写入后访问。写入已提交的写时写入页面的尝试将导致为进程创建的页面的私有副本。私人页面标记为PAGE_EXECUTE_READWRITE，更改将写入新页面。
            /// VirtualAlloc或 VirtualAllocEx函数不支持此标志。
            /// </summary>
            PAGE_EXECUTE_WRITECOPY = 0x80,

            /// <summary>
            /// 禁用对承诺的页面区域的所有访问。尝试读取，写入或执行承诺的区域导致访问冲突。
            /// CreateFileMapping函数不支持此标志 。
            /// </summary>
            PAGE_NOACCESS = 0x01,

            /// <summary>
            /// 启用对提交的页面区域的只读访问。写入承诺区域的尝试导致访问冲突。如果 启用了数据执行保护，则尝试在提交的区域中执行代码会导致访问冲突。
            /// </summary>
            PAGE_READONLY = 0x02,

            /// <summary>
            /// 启用对提交的页面区域的只读或读/写访问。如果 启用了数据执行保护功能，则尝试在提交的区域中执行代码会导致访问冲突。
            /// </summary>
            PAGE_READWRITE = 0x04,

            /// <summary>
            /// 启用对文件映射对象的映射视图的只读或写时访问。写入已提交的写时写入页面的尝试将导致为进程创建的页面的私有副本。私人页面标记为PAGE_READWRITE，更改将写入新页面。如果启用了数据执行保护功能，则尝试在提交的区域中执行代码会导致访问冲突。
            /// VirtualAlloc或 VirtualAllocEx函数不支持此标志。
            /// </summary>
            PAGE_WRITECOPY = 0x08,
            #endregion

            #region optional
            /// <summary>
            /// 该地区的页面变成了保护页面。任何访问保护页面的尝试都会导致系统引发 STATUS_GUARD_PAGE_VIOLATION异常并关闭保护页面状态。因此，保护​​页作为一次性访问报警。有关详细信息，请参阅 创建Guard页面。
            /// 当访问尝试导致系统关闭保护页面状态时，底层页面保护将接管。
            /// 如果在系统服务期间发生保护页异常，则该服务通常返回故障状态指示符。
            /// 此值不能与PAGE_NOACCESS一起使用。
            /// CreateFileMapping函数不支持此标志 。
            /// </summary>
            PAGE_GUARD = 0x100,

            /// <summary>
            /// 将所有页面设置为不可高速缓存。应用程序不应该使用此属性，除非显式要求设备。使用与SEC_NOCACHE映射的内存的联锁功能 可能导致 EXCEPTION_ILLEGAL_INSTRUCTION异常。
            /// 所述PAGE_NOCACHE标志不能与使用PAGE_GUARD，PAGE_NOACCESS，或PAGE_WRITECOMBINE标志。
            /// 所述PAGE_NOCACHE标志只能与分配专用存储器时使用的VirtualAlloc，VirtualAllocEx来，或VirtualAllocExNuma功能。要为共享内存启用非缓存内存访问，请在调用CreateFileMapping函数时指定SEC_NOCACHE标志。
            /// </summary>
            PAGE_NOCACHE = 0x200,

            /// <summary>
            /// 设置要组合的所有页面。
            /// 应用程序不应该使用此属性，除非显式要求设备。将联锁功能与映射为写入组合的内存结合使用会导致EXCEPTION_ILLEGAL_INSTRUCTION异常。
            /// 该PAGE_WRITECOMBINE标志不能与指定PAGE_NOACCESS，PAGE_GUARD，和PAGE_NOCACHE标志。
            /// 所述PAGE_WRITECOMBINE标志只能与分配专用存储器时使用的VirtualAlloc，VirtualAllocEx来，或VirtualAllocExNuma功能。要启用共享内存的写入组合内存访问，请在调用CreateFileMapping函数时指定SEC_WRITECOMBINE标志。
            /// </summary>
            PAGE_WRITECOMBINE = 0x400
            #endregion
        }

        /// <summary>
        /// 内存释放选项（直接Google翻译了，自己翻译挺麻烦的）
        /// </summary>
        public enum MemoryFreeFlag : uint
        {
            /// <summary>
            /// 取消提交的页面的指定区域。操作完成后，页面处于保留状态。
            /// 如果您尝试解除未提交的页面，该功能不会失败。这意味着您可以在不首先确定其当前承诺状态的情况下解除一系列页面。
            /// 所述MEM_DECOMMIT当不支持值lpAddress参数提供的基址的飞地。
            /// </summary>
            MEM_DECOMMIT = 0x4000,

            /// <summary>
            /// 释放指定的页面区域。操作完成后，页面处于空闲状态。
            /// 如果指定此值，则dwSize必须为0（零），并且lpAddress必须指向当该区域被保留时VirtualAllocEx函数返回的基址 。如果不符合这些条件之一，则该函数将失败。
            /// 如果该区域中的任何页面都被提交，则该函数首先被解除，然后释放它们。
            /// 如果尝试发布不同状态的页面，某些保留和某些提交的页面，该函数不会失败。这意味着您可以在不首先确定当前承诺状态的情况下释放一系列页面。
            /// </summary>
            MEM_RELEASE = 0x8000
        }

        /// <summary>
        /// 内存页面的状态
        /// </summary>
        public enum PageState : uint
        {
            /// <summary>
            /// 表示已分配物理存储的已提交页面，位于内存中或磁盘上的页面文件中。
            /// </summary>
            MEM_COMMIT = 0x1000,

            /// <summary>
            /// 无法访问的可用页面，可以被分配。
            /// </summary>
            MEM_FREE = 0x10000,

            /// <summary>
            /// 
            /// </summary>
            MEM_RESERVE = 0x2000
        }

        /// <summary>
        /// 页面类型
        /// </summary>
        public enum PageType : uint
        {
            /// <summary>
            /// Indicates that the memory pages within the region are mapped into the view of an image section.
            /// </summary>
            MEM_IMAGE = 0x1000000,

            /// <summary>
            /// Indicates that the memory pages within the region are mapped into the view of a section.
            /// </summary>
            MEM_MAPPED = 0x40000,

            /// <summary>
            /// Indicates that the memory pages within the region are private (that is, not shared by other processes).
            /// </summary>
            MEM_PRIVATE = 0x20000
        }

        /// <summary>
        /// 处理器架构
        /// </summary>
        public enum PROCESSOR_ARCHITECTURE : uint
        {
            /// <summary>
            /// AMD64
            /// </summary>
            PROCESSOR_ARCHITECTURE_AMD64 = 9,

            /// <summary>
            /// ARM
            /// </summary>
            PROCESSOR_ARCHITECTURE_ARM = 5,

            /// <summary>
            /// IA64
            /// </summary>
            PROCESSOR_ARCHITECTURE_IA64 = 6,

            /// <summary>
            /// Intel
            /// </summary>
            PROCESSOR_ARCHITECTURE_INTEL = 0,

            /// <summary>
            /// 未知
            /// </summary>
            PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF
        }

        /// <summary>
        /// 控制线程创建的标志
        /// </summary>
        public enum ThreadCreationFlags : uint
        {
            /// <summary>
            /// 创建后立即运行
            /// </summary>
            Zero = 0,

            /// <summary>
            /// 线程被创建为挂起状态
            /// </summary>
            CREATE_SUSPENDED = 0x00000004,

            STACK_SIZE_PARAM_IS_A_RESERVATION = 0x00010000
        }
        #endregion

        #region struct
        /// <summary>
        /// 内存页面信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            /// <summary>
            /// 区域基地址
            /// </summary>
            public IntPtr BaseAddress;

            /// <summary>
            /// 分配基地址
            /// </summary>
            public IntPtr AllocationBase;

            /// <summary>
            /// 区域被初次保留时赋予的保护属性
            /// </summary>
            public MemoryProtectionFlags AllocationProtect;

            /// <summary>
            /// 区域大小（以字节为计量单位）
            /// </summary>
            public IntPtr RegionSize;

            /// <summary>
            /// 状态
            /// </summary>
            public PageState State;

            /// <summary>
            /// 保护属性
            /// </summary>
            public MemoryProtectionFlags Protect;

            /// <summary>
            /// 类型
            /// </summary>
            public PageType Type;

            /// <summary>
            /// 结构体在非托管内存中大小
            /// </summary>
            public static readonly uint Size = (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
        }

        /// <summary>
        /// 包含有关当前计算机系统的信息。这包括处理器的架构和类型，系统中的处理器数量，页面大小以及其他此类信息。
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            /// <summary>
            /// 指定系统中的中央处理器的体系结构。
            /// </summary>
            public PROCESSOR_ARCHITECTURE wProcessorArchitecture;

            /// <summary>
            /// 保留供将来使用。
            /// </summary>
            public ushort wReserved;

            /// <summary>
            /// 指定页面的大小和页面保护和委托的颗粒。这是被 VirtualAlloc 函数使用的页大小。
            /// </summary>
            public uint dwPageSize;

            /// <summary>
            /// 指向应用程序和动态链接库(DLL)可以访问的最低内存地址。
            /// </summary>
            public IntPtr lpMinimumApplicationAddress;

            /// <summary>
            /// 指向应用程序和动态链接库(DLL)可以访问的最高内存地址。
            /// </summary>
            public IntPtr lpMaximumApplicationAddress;

            /// <summary>
            /// 指定一个用来代表这个系统中装配了的中央处理器的掩码。二进制0位是处理器0；31位是处理器31。
            /// </summary>
            public uint dwActiveProcessorMask;

            /// <summary>
            /// 指定系统中的处理器的数目。
            /// </summary>
            public uint dwNumberOfProcessors;

            /// <summary>
            /// 指定系统中中央处理器的类型。（已废弃）
            /// </summary>
            public uint dwProcessorType;

            /// <summary>
            /// 指定已经被分配的虚拟内存空间的粒度。例如，如果使用VirtualAlloc函数请求分配1byte内存空间，那么将会保留由dwAllocationGranularity指定大小byte的地址空间。在过去，这个值被定为64K并固化在硬件中，但是其它的硬件体系结构可能需要另外的值。
            /// </summary>
            public uint dwAllocationGranularity;

            /// <summary>
            /// 指定系统体系结构依赖的处理器级别。
            /// </summary>
            public ushort wProcessorLevel;

            /// <summary>
            /// 指定系统体系结构依赖的处理器修订版本号。下表显示了对于每一种处理器体系，处理器的修订版本号是如何构成的。
            /// </summary>
            public ushort wProcessorRevision;
        }
        #endregion

        #region method
        #region process thread handle
        /// <summary>
        /// 打开进程
        /// </summary>
        /// <param name="dwDesiredAccess">权限</param>
        /// <param name="bInheritHandle">是否继承</param>
        /// <param name="dwProcessId">进程ID</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr OpenProcess(
            ProcAccessFlags dwDesiredAccess,
            bool bInheritHandle,
            uint dwProcessId);

        /// <summary>
        /// 关闭句柄
        /// </summary>
        /// <param name="hObject">句柄</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool CloseHandle(
            IntPtr hObject);

        /// <summary>
        /// 获取某个窗口的创建者的线程ID和进程ID
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="lpdwProcessId">进程ID</param>
        /// <returns>线程ID</returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(
            IntPtr hWnd,
            out uint lpdwProcessId);

        /// <summary>
        /// 获取指定的进程是否在WOW64下运行
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="Wow64Process">
        /// 32位进程运行在32位Windows下：False
        /// 32位进程运行在64位Windows下：True
        /// 64位进程运行在64位Windows下：False
        /// </param>
        /// <returns>返回值是函数是否执行成功，而不是是否为64位进程！！！</returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool IsWow64Process(
            IntPtr hProcess,
            out bool Wow64Process);

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
        [DllImport("kernel32.dll")]
        public static unsafe extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            ThreadCreationFlags dwCreationFlags,
            uint* lpThreadId);
        #endregion

        #region module
        /// <summary>
        /// 枚举进程的所有模块
        /// </summary>
        /// <param name="hProcess">进程的句柄</param>
        /// <param name="lphModule">模块句柄</param>
        /// <param name="cb">储存模块句柄的字节数</param>
        /// <param name="lpcbNeeded">储存所有模块句柄所需的字节数</param>
        /// <param name="dwFilterFlag">过滤条件</param>
        /// <returns></returns>
        [DllImport("psapi.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool EnumProcessModulesEx(
            IntPtr hProcess,
            IntPtr* lphModule,
            uint cb,
            ref uint lpcbNeeded,
            EnumModulesFilterFlag dwFilterFlag);

        /// <summary>
        /// 获取模块名
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="hModule">模块句柄</param>
        /// <param name="lpBaseName">模块名</param>
        /// <param name="nSize">最大模块名长度</param>
        /// <returns>成功将返回非零整数</returns>
        [DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetModuleBaseName(
            IntPtr hProcess,
            IntPtr hModule,
            StringBuilder lpBaseName,
            uint nSize);

        /// <summary>
        /// 获取当前进程中符合条件的模块句柄
        /// </summary>
        /// <param name="lpModuleName">模块名</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(
            string lpModuleName);
        #endregion

        #region memory
        #region readmemory
        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            void* lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out byte lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out bool lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out char lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out short lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out ushort lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out int lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out uint lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out long lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out ulong lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out float lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);

        /// <summary>
        /// 读取内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要读取的内容</param>
        /// <param name="nSize">读取内容的大小</param>
        /// <param name="lpNumberOfBytesRead">实际读取大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out double lpBuffer,
            uint nSize,
            void* lpNumberOfBytesRead);
        #endregion

        #region writememory
        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            void* lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref bool lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref byte lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref char lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref short lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref ushort lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref int lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref uint lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref long lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref ulong lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref float lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);

        /// <summary>
        /// 写入内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">地址</param>
        /// <param name="lpBuffer">要写入的内容</param>
        /// <param name="nSize">写入内容的大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref double lpBuffer,
            uint nSize,
            void* lpNumberOfBytesWritten);
        #endregion

        #region virtual
        /// <summary>
        /// 在当前进程中分配内存
        /// </summary>
        /// <param name="lpAddress">指定一个地址用于分配内存（如果为IntPtr.Zero则自动分配）</param>
        /// <param name="dwSize">要分配内存的大小</param>
        /// <param name="flAllocationType">内存分配选项</param>
        /// <param name="flProtect">内存保护选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualAlloc(
            IntPtr lpAddress,
            uint dwSize,
            MemoryAllocationFlags flAllocationType,
            MemoryProtectionFlags flProtect);

        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpAddress">指定一个地址用于分配内存（如果为IntPtr.Zero则自动分配）</param>
        /// <param name="dwSize">要分配内存的大小</param>
        /// <param name="flAllocationType">内存分配选项</param>
        /// <param name="flProtect">内存保护选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            MemoryAllocationFlags flAllocationType,
            MemoryProtectionFlags flProtect);

        /// <summary>
        /// 在当前进程中释放内存
        /// </summary>
        /// <param name="lpAddress">指定释放内存的地址</param>
        /// <param name="dwSize">要释放内存的大小</param>
        /// <param name="dwFreeType">内存释放选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool VirtualFree(
            IntPtr lpAddress,
            uint dwSize,
            MemoryFreeFlag dwFreeType);

        /// <summary>
        /// 释放内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpAddress">指定释放内存的地址</param>
        /// <param name="dwSize">要释放内存的大小</param>
        /// <param name="dwFreeType">内存释放选项</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            MemoryFreeFlag dwFreeType);

        /// <summary>
        /// 查询地址空间中内存地址的信息
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpAddress">查询内存的地址</param>
        /// <param name="lpBuffer">内存页面信息</param>
        /// <param name="dwLength">MEMORY_BASIC_INFORMATION结构的大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern uint VirtualQueryEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer,
            uint dwLength);
        #endregion
        #endregion

        #region window
        /// <summary>
        /// 设置父窗口
        /// </summary>
        /// <param name="hWndChild">子窗口句柄</param>
        /// <param name="hWndNewParent">新的父窗口的句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SetParent(
            IntPtr hWndChild,
            IntPtr hWndNewParent);

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
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint uFlags);

        /// <summary>
        /// 是否为有效窗口
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool IsWindow(
            IntPtr hWnd);

        /// <summary>
        /// 同步方法发送消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="Msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint SendMessage(
            IntPtr hWnd,
            uint Msg,
            uint wParam,
            uint lParam);

        /// <summary>
        /// 异步方法发送消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="Msg">消息</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool PostMessage(
            IntPtr hWnd,
            uint Msg,
            uint wParam,
            uint lParam);

        /// <summary>
        /// 获取桌面的窗口句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// 获取Program Manager的窗口句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetShellWindow();

        /// <summary>
        /// 获取任务栏的窗口句柄（任务栏的一部分）
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetTaskmanWindow();

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="lpClassName">窗口类名</param>
        /// <param name="lpWindowName">窗口标题</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindWindow(
            string lpClassName,
            string lpWindowName);

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="hWndParent">父窗口句柄</param>
        /// <param name="hWndChildAfter">从此窗口之后开始查找（此窗口必须为父窗口的直接子窗口）</param>
        /// <param name="lpszClass">窗口类名</param>
        /// <param name="lpszWindow">窗口标题</param>
        /// <returns></returns>
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindWindowEx(
            IntPtr hWndParent,
            IntPtr hWndChildAfter,
            string lpszClass,
            string lpszWindow);
        #endregion

        #region dll
        /// <summary>
        /// 获取指定模块中导出函数的地址
        /// </summary>
        /// <param name="hModule">模块句柄</param>
        /// <param name="lpProcName">函数名</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string lpProcName);
        #endregion

        #region system
        /// <summary>
        /// 检索有关当前系统的信息。
        /// </summary>
        /// <param name="lpSystemInfo">接收信息的SYSTEM_INFO结构</param>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
        public static extern void GetSystemInfo(
            out SYSTEM_INFO lpSystemInfo);

        /// <summary>
        /// 检索有关当前系统的信息到运行在WOW64下的应用程序 。如果该功能是从64位应用程序调用的，则等同于 GetSystemInfo函数。
        /// </summary>
        /// <param name="lpSystemInfo">接收信息的SYSTEM_INFO结构</param>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
        public static extern void GetNativeSystemInfo(
            out SYSTEM_INFO lpSystemInfo);
        #endregion
        #endregion
    }
}
