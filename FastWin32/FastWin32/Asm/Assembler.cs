using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using LzmaSharp;
using SysProcess = System.Diagnostics.Process;

namespace FastWin32.Asm
{
    /// <summary>
    /// 汇编器，提供编译与反编译支持
    /// </summary>
    public static class Assembler
    {
        /// <summary>
        /// 参数b
        /// </summary>
        private static readonly string _argb = Environment.Is64BitProcess ? " -b 64 " : " -b 32 ";
        /// <summary>
        /// 参数f
        /// </summary>
        private static readonly string _argf = Environment.Is64BitProcess ? " -f win64 " : " -f in32 ";
        /// <summary>
        /// nasm文件夹
        /// </summary>
        internal static string _nasmDir;
        /// <summary>
        /// 汇编器路径
        /// </summary>
        internal static string _nasmPath;
        /// <summary>
        /// 反汇编器路径
        /// </summary>
        internal static string _ndisasmPath;

        /// <summary>
        /// 最后一次编译错误
        /// </summary>
        public static string LastCompileError { get; internal set; }

        /// <summary>
        /// 第一次访问时解压资源
        /// </summary>
        static Assembler()
        {
            Unpack();
            GC.Collect();
            //释放资源
        }

        /// <summary>
        /// 解压
        /// </summary>
        private static void Unpack()
        {
            byte[] decompressedBuffer;
            byte[] bytNasm;
            byte[] bytNdisasm;

            _nasmDir = Path.Combine(Path.GetTempPath(), "nasm");
            //最终文件解压在此文件夹
            if (!Directory.Exists(_nasmDir))
                //如果文件夹不存在，就创建
                Directory.CreateDirectory(_nasmDir);
            decompressedBuffer = LzmaCompressor.Decompress(Resources.NAsm);
            //解压
            _nasmPath = Path.Combine(_nasmDir, "nasm.exe");
            bytNasm = new byte[1124864];
            Buffer.BlockCopy(decompressedBuffer, 0, bytNasm, 0, bytNasm.Length);
            File.WriteAllBytes(_nasmPath, bytNasm);
            //nasm.exe
            _ndisasmPath = Path.Combine(_nasmDir, "ndisasm.exe");
            bytNdisasm = new byte[632832];
            Buffer.BlockCopy(decompressedBuffer, bytNasm.Length, bytNdisasm, 0, bytNdisasm.Length);
            File.WriteAllBytes(_ndisasmPath, bytNdisasm);
            //ndisasm.exe
        }

        /// <summary>
        /// 汇编指令转机器码
        /// </summary>
        /// <param name="opcodes">汇编指令</param>
        /// <returns></returns>
        public static byte[] OpcodesToBytes(string[] opcodes)
        {
            if (opcodes == null)
                throw new ArgumentNullException();
            if (opcodes.Length == 0)
                throw new ArgumentOutOfRangeException();

            ProcessStartInfo startInfo;
            SysProcess process;
            string[] output;
            string error;

            File.WriteAllLines(Path.Combine(_nasmDir, "asm"), opcodes);
            //写入字节数组到文件
            startInfo = new ProcessStartInfo
            {
                Arguments = _argf + "-l list asm",
                CreateNoWindow = true,
                FileName = _nasmPath,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = _nasmDir
            };
            process = new SysProcess
            {
                StartInfo = startInfo
            };
            process.Start();
            //启动编译器
            process.WaitForExit();
            //等待结束
            error = process.StandardError.ReadLine();
            //读取第一行错误
            if (string.IsNullOrEmpty(error))
            {
                //编译成功
                output = File.ReadAllLines(Path.Combine(_nasmDir, "list"));
                //读取list
                output = ListAnalyzer(output, 16, 18);
                //获取机器码
                return HexsToBytes(output);
            }
            else
            {
                //编译失败
                StringBuilder builder;

                builder = new StringBuilder();
                builder.Append("line" + error.Substring(4));
                while (true)
                {
                    error = process.StandardError.ReadLine();
                    //读取第下一行错误
                    if (string.IsNullOrEmpty(error))
                        break;
                    else
                        //添加到builder
                        builder.Append(Environment.NewLine + "line" + error.Substring(4));
                }
                LastCompileError = builder.ToString();
                throw new AsmCompilerException();
            }
        }

        /// <summary>
        /// 机器码转汇编指令
        /// </summary>
        /// <param name="bytes">机器码</param>
        /// <returns></returns>
        public static string[] BytesToOpcodes(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();
            if (bytes.Length == 0)
                throw new ArgumentOutOfRangeException();

            ProcessStartInfo startInfo;
            SysProcess process;
            string line;
            List<string> output;

            File.WriteAllBytes(Path.Combine(_nasmDir, "asm"), bytes);
            //写入字节数组到文件
            startInfo = new ProcessStartInfo
            {
                Arguments = _argb + "asm",
                CreateNoWindow = true,
                FileName = _ndisasmPath,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _nasmDir
            };
            process = new SysProcess
            {
                StartInfo = startInfo
            };
            process.Start();
            //启动反编译器
            process.WaitForExit();
            //等待结束
            output = new List<string>();
            while (true)
            {
                line = process.StandardOutput.ReadLine();
                //读取下一行
                if (string.IsNullOrEmpty(line))
                    break;
                else
                    output.Add(line);
            }
            return ListAnalyzer(output.ToArray(), 28, -1);
        }

        /// <summary>
        /// 解析器
        /// </summary>
        /// <param name="lines">所有行</param>
        /// <param name="startIndex">开始截取</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        private static string[] ListAnalyzer(string[] lines, int startIndex, int length)
        {
            List<string> result;

            result = new List<string>();
            if (length == -1)
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length > startIndex)
                        result.Add(lines[i].Substring(startIndex, lines[i].Length - startIndex).TrimEnd(' '));
                }
            else
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length > startIndex)
                        result.Add(lines[i].Substring(startIndex, length).TrimEnd(' '));
                }
            return result.ToArray();
        }

        /// <summary>
        /// 十六进制数组转换为字节数组
        /// </summary>
        /// <param name="hexs">十六进制数组</param>
        /// <returns></returns>
        private static byte[] HexsToBytes(string[] hexs)
        {
            if (hexs == null)
                throw new ArgumentNullException();

            List<byte> list;

            list = new List<byte>();
            foreach (string hex in hexs)
                for (int i = 0; i < hex.Length; i += 2)
                    list.Add(Convert.ToByte(hex.Substring(i, 2), 16));
            return list.ToArray();
        }
    }
}
