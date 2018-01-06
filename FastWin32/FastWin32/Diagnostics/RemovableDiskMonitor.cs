using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 可移动磁盘插入/弹出
    /// </summary>
    /// <param name="volumeName">用Guid表示的分区根目录</param>
    public delegate void RemovableDiskEventHandler(string volumeName);

    /// <summary>
    /// 可移动磁盘拔插监视器
    /// </summary>
    public static class RemovableDiskMonitor
    {
        /// <summary>
        /// 是否启动
        /// </summary>
        private static bool _isStarted;

        /// <summary>
        /// 分区搜索句柄
        /// </summary>
        private static IntPtr _hFindVolume;

        /// <summary>
        /// 监视器线程
        /// </summary>
        private static Thread _monitorThread;

        /// <summary>
        /// 可移动磁盘插入事件
        /// </summary>
        public static event RemovableDiskEventHandler RemovableDiskArrivaled;

        /// <summary>
        /// 可移动磁盘弹出事件
        /// </summary>
        public static event RemovableDiskEventHandler RemovableDiskMoveCompleted;

        /// <summary>
        /// 启动监视器
        /// </summary>
        public static void Start()
        {
            if (_isStarted)
                return;
            _monitorThread = new Thread(MonitorLoop)
            {
                IsBackground = true
            };
            _monitorThread.Start();
            _isStarted = true;
        }

        /// <summary>
        /// 监视循环
        /// </summary>
        private static void MonitorLoop()
        {
            IList<string> oldRemovableDisks;

            oldRemovableDisks = null;
            while (true)
            {
                Monitor(ref oldRemovableDisks);
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// 监视
        /// </summary>
        /// <param name="oldRemovableVolumes">旧的可移动磁盘列表</param>
        private static void Monitor(ref IList<string> oldRemovableVolumes)
        {
            StringBuilder volumeBuilder;
            string volume;
            IList<string> newRemovableVolumes;

            volumeBuilder = new StringBuilder(60);
            _hFindVolume = FindFirstVolume(volumeBuilder, 60);
            if (_hFindVolume == IntPtr.Zero)
                return;
            newRemovableVolumes = new List<string>();
            do
            {
                volume = volumeBuilder.ToString();
                if (GetDriveType(volume) == DRIVE_REMOVABLE)
                    newRemovableVolumes.Add(volume);
                //添加可移动设备路径到列表
            } while (FindNextVolume(_hFindVolume, volumeBuilder, 60));
            if (oldRemovableVolumes == null)
            {
                oldRemovableVolumes = newRemovableVolumes;
                return;
            }
            else
            {
                foreach (string str in newRemovableVolumes.Except(oldRemovableVolumes))
                    RemovableDiskArrivaled?.Invoke(str);
                //求差集new - old，输出插入的可移动磁盘
                foreach (string str in oldRemovableVolumes.Except(newRemovableVolumes))
                    RemovableDiskMoveCompleted?.Invoke(str);
                //求差集old - new，输出拔出的可移动磁盘
                oldRemovableVolumes = newRemovableVolumes;
            }
        }

        /// <summary>
        /// 停止监视器
        /// </summary>
        public static void Stop()
        {
            if (!_isStarted)
                return;
            _monitorThread.Abort();
            _isStarted = false;
        }
    }
}
