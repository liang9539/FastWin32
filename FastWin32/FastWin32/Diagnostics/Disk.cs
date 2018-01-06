using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 磁盘
    /// </summary>
    public static class Disk
    {
        /// <summary>
        /// 枚举所有卷（只枚举有盘符的卷），若枚举失败，返回<see langword="null"/>
        /// </summary>
        /// <returns></returns>
        public static IList<DriveInfo> EnumAllVolumes()
        {
            string[] letters;
            IList<DriveInfo> driveInfosList;

            letters = Environment.GetLogicalDrives();
            driveInfosList = new List<DriveInfo>(letters.Length);
            foreach (string letter in letters)
                driveInfosList.Add(new DriveInfo(letter));
            return driveInfosList;
        }

        /// <summary>
        /// 枚举所有卷，返回卷的Guid，若枚举失败，返回<see langword="null"/>
        /// </summary>
        /// <returns></returns>
        public static IList<string> EnumAllVolumesByVolumeName()
        {
            IntPtr hFindVolume;
            StringBuilder volumeName;
            IList<string> volumeNamesList;

            volumeName = new StringBuilder(60);
            hFindVolume = FindFirstVolume(volumeName, 60);
            if (hFindVolume == IntPtr.Zero)
                return null;
            volumeNamesList = new List<string>();
            do
            {
                volumeNamesList.Add(volumeName.ToString());
            } while (FindNextVolume(hFindVolume, volumeName, 60));
            return volumeNamesList;
        }

        /// <summary>
        /// 获取卷Guid路径对应的路径（用盘符表示，例如C:\），若无盘符，返回<see langword="null"/>
        /// </summary>
        /// <param name="volumeName">用Guid表示的分区根目录</param>
        /// <returns></returns>
        public static unsafe string GetVolumePath(string volumeName)
        {
            if (string.IsNullOrEmpty(volumeName))
                throw new ArgumentNullException(nameof(volumeName) + "不能为空");

            StringBuilder path;
            uint nChars;

            path = new StringBuilder(10);
            if (GetVolumePathNamesForVolumeName(volumeName, path, 10, &nChars) && nChars > 1)
                return path.ToString();
            else
                return null;
        }
    }
}
