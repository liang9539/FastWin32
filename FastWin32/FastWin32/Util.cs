using System;
using static FastWin32.NativeMethods;

namespace FastWin32
{
    /// <summary>
    /// 常用方法
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// 打开进程（读写）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static IntPtr OpenProcessRW(uint processId)
        {
            return OpenProcess(ProcAccessFlags.PROCESS_VM_OPERATION | ProcAccessFlags.PROCESS_VM_READ | ProcAccessFlags.PROCESS_VM_WRITE, false, processId);
        }

        /// <summary>
        /// 打开进程（读写+查询）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static IntPtr OpenProcessRWQuery(uint processId)
        {
            return OpenProcess(ProcAccessFlags.PROCESS_VM_OPERATION | ProcAccessFlags.PROCESS_VM_READ | ProcAccessFlags.PROCESS_VM_WRITE | ProcAccessFlags.PROCESS_QUERY_INFORMATION, false, processId);
        }
    }
}
