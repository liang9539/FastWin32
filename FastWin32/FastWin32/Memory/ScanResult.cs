using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存扫描结果
    /// </summary>
    public class ScanResult : IDisposable
    {
        private string _directory;
        private Thread _writeThread;

        /// <summary>
        /// 实例化
        /// </summary>
        public ScanResult()
        {
            _directory = GenTempDir();
            //生成临时文件夹
        }

        /// <summary>
        /// 生成临时文件夹
        /// </summary>
        /// <returns></returns>
        private string GenTempDir()
        {
            string tempPath;
            string result;

            tempPath = Path.GetTempPath();
            do
            {
                result = Path.Combine(tempPath, "MemoryScan-", Guid.NewGuid().ToString());
                //生成一个路径
            } while (File.Exists(result));
            Directory.CreateDirectory(result);
            //创建目录
            return result;
        }

        /// <summary>
        /// 清理扫描结果文件夹（务必在使用完后调用此方法）
        /// </summary>
        public void Clear()
        {
            Directory.Delete(_directory, true);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _writeThread.Abort();
                }
                Clear();
                disposedValue = true;
            }
        }

        /// <summary>
        /// 终结器
        /// </summary>
        ~ScanResult()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
