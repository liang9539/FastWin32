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
            IList<string> guids;
            StringBuilder letter;
            IList<DriveInfo> volumesList;

            guids = EnumAllVolumesByVolumeName();
            if (guids == null)
                return null;
            letter = new StringBuilder(60);
            volumesList = new List<DriveInfo>(guids.Count);
            foreach (string guid in guids)
            {
                //volumesList.Add(new DriveInfo(letter.ToString()));
            }
            return volumesList;
            //string[] letters;
            //IList<DriveInfo> driveInfosList;

            //letters = Environment.GetLogicalDrives();
            //driveInfosList = new List<DriveInfo>(letters.Length);
            //foreach (string letter in letters)
            //    driveInfosList.Add(new DriveInfo(letter));
            //return driveInfosList;
        }

        /// <summary>
        /// 枚举所有卷，返回卷的Guid，若枚举失败，返回<see langword="null"/>
        /// </summary>
        /// <returns></returns>
        public static IList<string> EnumAllVolumesByVolumeName()
        {
            IntPtr hFindVolume;
            StringBuilder volume;
            IList<string> volumesList;

            volume = new StringBuilder(60);
            hFindVolume = FindFirstVolume(volume, 60);
            if (hFindVolume == IntPtr.Zero)
                return null;
            volumesList = new List<string>();
            do
            {
                volumesList.Add(volume.ToString());
            } while (FindNextVolume(hFindVolume, volume, 60));
            return volumesList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="volumeName">用Guid表示的分区根目录</param>
        /// <returns></returns>
        public static string TryGetVolumePath(string volumeName)
        {

        }
    }
}
