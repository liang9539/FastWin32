using System;
using System.Linq;
using FastWin32.Diagnostics;

namespace Example2
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = (new int[0]).Select(dummy => new { moduleHandle = default(IntPtr), moduleName = default(string), pFunction = default(IntPtr), functionName = default(string), ordinal = default(short) }).ToList();
            Module32.EnumModules(Process32.GetCurrentProcessId(), (IntPtr moduleHandle, string moduleName, string filePath) =>
            {
                list.Clear();
                Module32.EnumFunctions(Process32.GetCurrentProcessId(), moduleHandle, (IntPtr pFunction, string functionName, short ordinal) =>
                {
                    list.Add(new { moduleHandle, moduleName, pFunction, functionName, ordinal });
                    return true;
                });
                list = list.OrderBy(item => item.moduleName).ToList();
                list.ForEach(item => Console.WriteLine($"MH:{item.moduleHandle.ToString("X16")} MN:{item.moduleName} PF:{item.pFunction.ToString("X16")} FN:{item.functionName} OD:{item.ordinal.ToString()}"));
                return true;
            });
            Console.ReadKey();
        }
    }
}
