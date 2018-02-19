using System;
using System.Diagnostics;
using System.Threading;
using FastWin32.Memory;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            uint processId;
            Pointer pointer;
            int value;

            processId = (uint)Process.Start("Tutorial-i386.exe").Id;
            Console.WriteLine("Go to \"Step 6\" then continue");
            Console.ReadKey();
            pointer = new Pointer("Tutorial-i386.exe", 0x1FD630, 0);
            MemoryIO.ReadInt32(processId, pointer, out value);
            Console.WriteLine($"Current value:{value}. Now we lock it");
            while (true)
            {
                MemoryIO.WriteInt32(processId, pointer, 5000);
                Thread.Sleep(1);
            }
        }
    }
}
