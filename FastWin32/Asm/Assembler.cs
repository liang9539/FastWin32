using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FastWin32.Memory;
using static FastWin32.NativeMethods;
using size_t = System.IntPtr;

namespace FastWin32.Asm
{
    /// <summary>
    /// 汇编器，提供编译与反编译支持
    /// </summary>
    public static class Assembler
    {
        /// <summary>
        /// 汇编器路径
        /// </summary>
        private static string _nasmPath;

        /// <summary>
        /// 反汇编器路径
        /// </summary>
        private static string _ndisasmPath;

        /// <summary>
        /// 汇编器路径
        /// </summary>
        public static string NasmPath
        {
            get => _nasmPath;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                value = Path.GetFullPath(value);
                if (!File.Exists(value))
                    throw new FileNotFoundException();
                _nasmPath = value;
            }
        }

        /// <summary>
        /// 反汇编器路径
        /// </summary>
        public static string NdisasmPath
        {
            get => _ndisasmPath;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                value = Path.GetFullPath(value);
                if (!File.Exists(value))
                    throw new FileNotFoundException();
                _ndisasmPath = value;
            }
        }

        /// <summary>
        /// 汇编指令转机器码
        /// </summary>
        /// <param name="opcodes">汇编指令</param>
        /// <param name="is64">是否使用64位汇编</param>
        /// <returns></returns>
        public static IList<AsmData> OpcodesToBytes(string[] opcodes, bool is64)
        {
            if (opcodes == null || opcodes.Length == 0)
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(_nasmPath))
                throw new FileNotFoundException("未设置编译器\"nasm.exe\"的路径");

            string[] output;
            string[] tokens;
            int n;
            List<AsmData> asmDataList;

            output = OpcodesToBytesRaw(opcodes, is64);
            n = 0;
            asmDataList = new List<AsmData>(opcodes.Length);
            foreach (string line in output)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                tokens = GetTokensNasm(line);
                if (int.Parse(tokens[0]) == n)
                {
                    //汇编指令太长，机器码用了2行或更长
                    for (int i = 0; i < tokens[1].Length; i += 2)
                        asmDataList[n - 1]._byteList.Add(Convert.ToByte(tokens[1].Substring(i, 2), 16));
                    //追加机器码到上一行
                }
                else
                {
                    //下一句汇编指令
                    asmDataList.Add(new AsmData(opcodes[n]));
                    for (int i = 0; i < tokens[1].Length; i += 2)
                        asmDataList[n]._byteList.Add(Convert.ToByte(tokens[1].Substring(i, 2), 16));
                    n++;
                }
            }
            foreach (AsmData asmData in asmDataList)
                asmData.AsReadOnly();
            return asmDataList;
        }

        /// <summary>
        /// 汇编指令转机器码保留原始数据
        /// </summary>
        /// <param name="opcodes">汇编指令</param>
        /// <param name="is64">是否使用64位汇编</param>
        /// <returns></returns>
        private static string[] OpcodesToBytesRaw(string[] opcodes, bool is64)
        {
            string tempFilePathOpcodes;
            string tempFilePathBytes;
            ProcessStartInfo startInfo;
            Process process;
            string error;
            StringBuilder stringBuilder;

            tempFilePathOpcodes = GetTempFileName();
            File.WriteAllLines(tempFilePathOpcodes, opcodes);
            tempFilePathBytes = GetTempFileName();
            startInfo = new ProcessStartInfo
            {
                Arguments = string.Format(" -f win{0} -l {1} {2}", is64 ? "64" : "32", Path.GetFileName(tempFilePathBytes), Path.GetFileName(tempFilePathOpcodes)),
                CreateNoWindow = true,
                FileName = _nasmPath,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetTempPath()
            };
            process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
            process.WaitForExit();
            error = process.StandardError.ReadLine();
            if (string.IsNullOrEmpty(error))
                return File.ReadAllLines(tempFilePathBytes);
            else
            {
                stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(error);
                while (true)
                {
                    error = process.StandardError.ReadLine();
                    if (string.IsNullOrEmpty(error))
                        break;
                    else
                        stringBuilder.AppendLine(error);
                }
                throw new AsmCompilerException(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// 获取nasm编译后生成list的Token
        /// </summary>
        /// <param name="line">list中的一行</param>
        /// <returns></returns>
        private static string[] GetTokensNasm(string line)
        {
            string[] tokens;

            tokens = new string[2];
            tokens[0] = line.Substring(0, 6).Trim();
            //第n条汇编指令
            tokens[1] = line.Substring(16, 18).Trim();
            //对应的机器码
            return tokens;
        }

        /// <summary>
        /// 获取<see cref="IList{AsmData}"/>中所有机器码（相当于将<see cref="GetAllBytesArray"/>的返回值拼接为一个数组）
        /// </summary>
        /// <param name="asmDataList"><see cref="AsmData"/>列表</param>
        /// <returns></returns>
        public static byte[] GetAllBytes(this IList<AsmData> asmDataList)
        {
            if (asmDataList == null)
                throw new ArgumentNullException();

            return asmDataList.SelectMany(asmData => asmData.Bytes).ToArray();
        }

        /// <summary>
        /// 获取<see cref="IList{AsmData}"/>中所有机器码数组
        /// </summary>
        /// <param name="asmDataList"><see cref="AsmData"/>列表</param>
        /// <returns></returns>
        public static byte[][] GetAllBytesArray(this IList<AsmData> asmDataList)
        {
            if (asmDataList == null)
                throw new ArgumentNullException();

            return asmDataList.Select(asmData => asmData.Bytes).ToArray();
        }

        /// <summary>
        /// 机器码转汇编指令
        /// </summary>
        /// <param name="bytes">机器码</param>
        /// <param name="is64">是否使用64位汇编</param>
        /// <returns></returns>
        public static IList<AsmData> BytesToOpcodes(byte[] bytes, bool is64)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(_ndisasmPath))
                throw new FileNotFoundException("未设置反编译器\"ndisasm.exe\"的路径");

            string[] output;
            string[] tokens;
            int n;
            List<AsmData> asmDataList;

            output = BytesToOpcodesRaw(bytes, is64);
            n = 0;
            asmDataList = new List<AsmData>(output.Length);
            foreach (string line in output)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                tokens = GetTokensNdisasm(line);
                if (tokens[1] == null)
                {
                    //汇编指令太长，机器码用了2行或更长
                    for (int i = 0; i < tokens[0].Length; i += 2)
                        asmDataList[n - 1]._byteList.Add(Convert.ToByte(tokens[0].Substring(i, 2), 16));
                    //追加机器码到上一行
                }
                else
                {
                    //下一句汇编指令
                    asmDataList.Add(new AsmData(tokens[1]));
                    for (int i = 0; i < tokens[0].Length; i += 2)
                        asmDataList[n]._byteList.Add(Convert.ToByte(tokens[0].Substring(i, 2), 16));
                    n++;
                }
            }
            foreach (AsmData asmData in asmDataList)
                asmData.AsReadOnly();
            return asmDataList;
        }

        /// <summary>
        /// 机器码转汇编指令保留原始数据
        /// </summary>
        /// <param name="bytes">机器码</param>
        /// <param name="is64">是否使用64位汇编</param>
        /// <returns></returns>
        private static string[] BytesToOpcodesRaw(byte[] bytes, bool is64)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(_ndisasmPath))
                throw new FileNotFoundException("未设置反编译器\"ndisasm.exe\"的路径");

            string tempFilePathBytes;
            ProcessStartInfo startInfo;
            Process process;
            string line;
            List<string> output;

            tempFilePathBytes = GetTempFileName();
            File.WriteAllBytes(tempFilePathBytes, bytes);
            startInfo = new ProcessStartInfo
            {
                Arguments = string.Format(" -b {0} {1}", is64 ? "64" : "32", Path.GetFileName(tempFilePathBytes)),
                CreateNoWindow = true,
                FileName = _ndisasmPath,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetTempPath()
            };
            process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
            process.WaitForExit();
            output = new List<string>();
            while (true)
            {
                line = process.StandardOutput.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;
                else
                    output.Add(line);
            }
            return output.ToArray();
        }

        /// <summary>
        /// 获取ndisasm反编译后生成list的Token
        /// </summary>
        /// <param name="line">list中的一行</param>
        /// <returns></returns>
        private static string[] GetTokensNdisasm(string line)
        {
            string[] tokens;

            tokens = new string[2];
            if (line[9] == '-')
            {
                tokens[0] = line.Substring(10).Trim();
                //机器码
            }
            else
            {
                tokens[0] = line.Substring(10, 16).Trim();
                //机器码
                tokens[1] = line.Substring(28).Trim();
                //汇编指令
            }
            return tokens;
        }

        /// <summary>
        /// 获取<see cref="IList{AsmData}"/>中所有汇编指令
        /// </summary>
        /// <param name="asmDataList"><see cref="AsmData"/>列表</param>
        /// <returns></returns>
        public static string[] GetAllOpcodes(this IList<AsmData> asmDataList)
        {
            if (asmDataList == null)
                throw new ArgumentNullException();

            return asmDataList.Select(asmData => asmData.Opcode).ToArray();
        }

        /// <summary>
        /// 获取临时文件
        /// </summary>
        /// <returns></returns>
        private static string GetTempFileName()
        {
            string path;

            path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            File.WriteAllText(path, null);
            return path;
        }

        /// <summary>
        /// 将机器码写入内存，返回函数指针。如果执行失败，返回<see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="bytes">机器码</param>
        /// <returns></returns>
        public static IntPtr GetFunctionPointer(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentNullException();

            IntPtr pAsm;

            pAsm = MemoryManagement.AllocMemoryInternal((size_t)bytes.Length, PAGE_EXECUTE_READ);
            //分配内存（可执行）
            if (MemoryIO.WriteBytesInternal(CURRENT_PROCESS, pAsm, bytes))
                return pAsm;
            else
                return IntPtr.Zero;
        }

        /// <summary>
        /// 将汇编指令写入内存，返回函数指针。如果执行失败，返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="opcodes">汇编指令</param>
        /// <param name="is64">是否使用64位汇编</param>
        /// <returns></returns>
        public static IntPtr GetFunctionPointer(string[] opcodes, bool is64)
        {
            if (opcodes == null || opcodes.Length == 0)
                throw new ArgumentNullException();

            return GetFunctionPointer(OpcodesToBytes(opcodes, is64).GetAllBytes());
        }

        /// <summary>
        /// 将机器码写入内存，返回对应的委托。如果执行失败，返回<see langword="null"/>
        /// </summary>
        /// <typeparam name="TDelegate">委托</typeparam>
        /// <param name="bytes">机器码</param>
        /// <returns></returns>
        public static TDelegate GetDelegate<TDelegate>(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentNullException();

            IntPtr pFunction;

            pFunction = GetFunctionPointer(bytes);
            if (pFunction == IntPtr.Zero)
                return default(TDelegate);
            else
                return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(pFunction, typeof(TDelegate));
        }

        /// <summary>
        /// 将机器码写入内存，返回对应的委托。如果执行失败，返回<see langword="null"/>
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="opcodes">汇编指令</param>
        /// <param name="is64">是否使用64位汇编</param>
        /// <returns></returns>
        public static TDelegate GetDelegate<TDelegate>(string[] opcodes, bool is64)
        {
            if (opcodes == null || opcodes.Length == 0)
                throw new ArgumentNullException();

            return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(GetFunctionPointer(opcodes, is64), typeof(TDelegate));
        }
    }
}
