//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text;
using System.Web;
using System.IO;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Management;
#if !MONO40
using Microsoft.VisualBasic.Devices;
#endif

namespace Pub.Class {
    /// <summary>
    /// 系统信息类
    /// 
    /// 修改纪录
    ///     2012.05.31 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class SystemInfo {
        class _ {
            /// <summary>
            /// 机器名
            /// </summary>
            public static string MachineName {
                get {
                    return Environment.MachineName;
                }
            }
#if !MONO40
            private static string baseBoard;
            /// <summary>
            /// 主板序列号
            /// </summary>
            public static string BaseBoard {
                get {
                    if (baseBoard == null) {
                        baseBoard = GetInfo("Win32baseBoard", "SerialNumber");
                        if (string.IsNullOrEmpty(baseBoard)) baseBoard = GetInfo("Win32baseBoard", "Product");
                        baseBoard = GetInfo("Win32baseBoard", "Product") + ";" + baseBoard;
                    }
                    return baseBoard;
                }
            }
            private static string processors;
            /// <summary>
            /// 处理器序列号
            /// </summary>
            public static string Processors {
                get {
                    if (processors == null) {
                        processors = GetInfo("Win32_Processor", "Caption") + ";" + GetInfo("Win32_Processor", "MaxClockSpeed") + ";" + GetInfo("Win32_Processor", "ProcessorId");
                    }
                    return processors;
                }
            }
            private static Int64? _Memory;
            /// <summary>
            /// 内存总量
            /// </summary>
            public static Int64 Memory {
                get {
                    if (_Memory == null) {
                        _Memory = (Int64)new ComputerInfo().TotalPhysicalMemory;
                        //_Memory = Convert.ToInt64(GetInfo("Win32_LogicalMemoryConfiguration", "TotalPhysicalMemory"));
                    }
                    return _Memory.Value;
                }
            }
            private static string disk;
            /// <summary>
            /// 磁盘名称
            /// </summary>
            public static string Disk {
                get {
                    if (disk == null) disk = GetInfo("Win32_DiskDrive", "Model");
                    return disk;
                    //上面的方式取驱动器序列号会取得包括U盘和网络映射驱动器的序列号，实际只要当前所在盘就可以了
                    //return Volume;
                }
            }
            private static string diskSerial = string.Empty;
            /// <summary>
            /// 磁盘序列号
            /// </summary>
            public static string DiskSerial {
                get {
                    if (string.IsNullOrEmpty(diskSerial)) diskSerial = GetInfo("Win32_DiskDrive", "SerialNumber");
                    return diskSerial;
                }
            }
            private static string volume;
            /// <summary>
            /// 驱动器序列号
            /// </summary>
            public static string Volume {
                get {
                    //if (string.IsNullOrEmpty(_Volume)) _Volume = GetInfo("Win32_DiskDrive", "Model");
                    //磁盘序列号不够明显，故使用驱动器序列号代替
                    string id = AppDomain.CurrentDomain.BaseDirectory.Substring(0, 2);
                    if (volume == null) volume = GetInfo("Win32_LogicalDisk Where DeviceID=\"" + id + "\"", "VolumeSerialNumber");
                    return volume;
                }
            }
            private static string macs;
            /// <summary>
            /// 网卡地址序列号
            /// </summary>
            public static string Macs {
                get {
                    if (macs != null) return macs;
                    //return GetInfo("Win32_NetworkAdapterConfiguration", "MacAddress");
                    ManagementClass cimobject = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection moc = cimobject.GetInstances();
                    List<string> bbs = new List<string>();
                    foreach (ManagementObject mo in moc) {
                        if (mo != null &&
                            mo.Properties != null &&
                            mo.Properties["MacAddress"] != null &&
                            mo.Properties["MacAddress"].Value != null &&
                            mo.Properties["IPEnabled"] != null &&
                            (bool)mo.Properties["IPEnabled"].Value) {
                            //bbs.Add(mo.Properties["MacAddress"].Value.ToString());
                            string s = mo.Properties["MacAddress"].Value.ToString();
                            if (!bbs.Contains(s)) bbs.Add(s);
                        }
                    }
                    bbs.Sort();
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in bbs) {
                        if (sb.Length > 0) sb.Append(",");
                        sb.Append(s);
                    }
                    macs = sb.ToString().Trim();
                    return Macs;
                }
            }
            private static string ips;
            /// <summary>
            /// IP地址
            /// </summary>
            public static string IPs {
                get {
                    if (ips != null) return ips;
                    //return null;
                    ManagementClass cimobject = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection moc = cimobject.GetInstances();
                    List<string> bbs = new List<string>();
                    foreach (ManagementObject mo in moc) {
                        if (mo != null &&
                            mo.Properties != null &&
                            mo.Properties["IPAddress"] != null &&
                            mo.Properties["IPAddress"].Value != null &&
                            mo.Properties["IPEnabled"] != null &&
                            (bool)mo.Properties["IPEnabled"].Value) {
                            string[] ss = (string[])mo.Properties["IPAddress"].Value;
                            if (ss != null) {
                                foreach (string s in ss)
                                    if (!bbs.Contains(s)) bbs.Add(s);
                            }
                            //bbs.Add(mo.Properties["IPAddress"].Value.ToString());
                        }
                    }
                    bbs.Sort();
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in bbs) {
                        if (sb.Length > 0) sb.Append(",");
                        sb.Append(s);
                    }
                    ips = sb.ToString().Trim();
                    return ips;
                }
            }
            public static string GetInfo(string path, string property) {
                string wql = string.Format("Select {0} From {1}", property, path);
                ManagementObjectSearcher cimobject = new ManagementObjectSearcher(wql);
                ManagementObjectCollection moc = cimobject.Get();
                List<string> bbs = new List<string>();
                try {
                    foreach (ManagementObject mo in moc) {
                        if (mo != null &&
                            mo.Properties != null &&
                            mo.Properties[property] != null &&
                            mo.Properties[property].Value != null)
                            bbs.Add(mo.Properties[property].Value.ToString());
                    }
                } catch (Exception ex) {
                    //Msg.WriteEnd("获取{0} {1}硬件信息失败\r\n{2}", path, property, ex);
                }
                bbs.Sort();
                StringBuilder sb = new StringBuilder();
                foreach (string s in bbs) {
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(s);
                }
                return sb.ToString().Trim();
            }
#endif
        }
        /// <summary>
        /// win7以上版本
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows7OrHigher {
            get {
                return Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1 ? true : false;
            }
        }
        /// <summary>
        /// Vista版本
        /// </summary>
        /// <returns></returns>
        public static bool IsWindowsVista {
            get {
                return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0 ? true : false;
            }
        }
        /// <summary>
        /// Vista以上版本
        /// </summary>
        /// <returns></returns>
        public static bool IsWindowsVistaOrHigher {
            get {
                return Environment.OSVersion.Version.Major >= 6 ? true : false;
            }
        }
        private SystemInfo() {
        }
#if !MONO40
        private string machineName;
        /// <summary>机器名</summary>
        public string MachineName {
            get { return machineName; }
            set { machineName = value; }
        }
        private string baseBoard;
        /// <summary>主板</summary>
        public string BaseBoard {
            get { return baseBoard; }
            set { baseBoard = value; }
        }
        private string processors;
        /// <summary>处理器</summary>
        public string Processors {
            get { return processors; }
            set { processors = value; }
        }
        private string disk;
        /// <summary>磁盘</summary>
        public string Disk {
            get { return disk; }
            set { disk = value; }
        }
        private string diskSerial;
        /// <summary>磁盘序列号</summary>
        public string DiskSerial {
            get { return diskSerial; }
            set { diskSerial = value; }
        }
        private string volume;
        /// <summary>驱动器序列号</summary>
        public string Volume {
            get { return volume; }
            set { volume = value; }
        }
        private string macs;
        /// <summary>网卡</summary>
        public string Macs {
            get { return macs; }
            set { macs = value; }
        }
        private string ips;
        /// <summary>IP地址</summary>
        public string IPs {
            get { return ips; }
            set { ips = value; }
        }
        private long memory;
        /// <summary>内存</summary>
        public long Memory {
            get { return memory; }
            set { memory = value; }
        }
        private string osVersion;
        /// <summary>系统版本</summary>
        public string OSVersion {
            get { return osVersion; }
            set { osVersion = value; }
        }
        private Int32 screenWidth;
        /// <summary>屏幕宽</summary>
        public Int32 ScreenWidth {
            get { return screenWidth; }
            set { screenWidth = value; }
        }
        private Int32 screenHeight;
        /// <summary>屏幕高</summary>
        public Int32 ScreenHeight {
            get { return screenHeight; }
            set { screenHeight = value; }
        }
        private Int64 diskSize;
        /// <summary>磁盘大小</summary>
        public Int64 DiskSize {
            get { return diskSize; }
            set { diskSize = value; }
        }
        private void GetLocal() {
            MachineName = _.MachineName;
            BaseBoard = _.BaseBoard;
            Processors = _.Processors;
            Disk = _.Disk;
            DiskSerial = _.DiskSerial;
            Volume = _.Volume;
            Macs = _.Macs;
            IPs = _.IPs;
            OSVersion = Environment.OSVersion.ToString();
            Memory = _.Memory;
            string str = _.GetInfo("Win32_DesktopMonitor", "ScreenWidth");
            Int32 m = 0;
            if (Int32.TryParse(str, out m)) ScreenWidth = m;
            str = _.GetInfo("Win32_DesktopMonitor", "ScreenHeight");
            if (Int32.TryParse(str, out m)) ScreenHeight = m;

            str = _.GetInfo("Win32_DiskDrive", "Size");
            Int64 n = 0;
            if (Int64.TryParse(str, out n)) DiskSize = n;
            if (DiskSize <= 0) {
                DriveInfo[] drives = DriveInfo.GetDrives();
                if (drives != null && drives.Length > 0) {
                    foreach (DriveInfo item in drives) {
                        if (item.DriveType == DriveType.CDRom ||
                            item.DriveType == DriveType.Network ||
                            item.DriveType == DriveType.NoRootDirectory) continue;

                        DiskSize += item.TotalSize;
                    }
                }
            }
        }
        private static SystemInfo current;
        /// <summary>当前机器硬件信息</summary>
        public static SystemInfo Current {
            get {
                if (current != null) return current;
                lock (typeof(SystemInfo)) {
                    if (current != null) return current;

                    try {
                        current = new SystemInfo();
                        current.GetLocal();
                    } catch (Exception ex) {
                        //Msg.WriteEnd(ex);
                    }
                    return current;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToExtend() {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["MachineName"] = MachineName;
            data["BaseBoard"] = BaseBoard;
            data["Processors"] = Processors;
            data["Disk"] = Disk;
            data["DiskSerial"] = DiskSerial;
            data["Volume"] = Volume;
            data["Macs"] = Macs;
            data["IPs"] = IPs;
            data["OSVersion"] = OSVersion;
            data["Memory"] = Memory.ToString();
            data["ScreenWidth"] = ScreenWidth.ToString();
            data["ScreenHeight"] = ScreenHeight.ToString();
            data["DiskSize"] = DiskSize.ToString();
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SystemInfo FromExtend(Dictionary<string, string> data) {
            SystemInfo entity = new SystemInfo();
            entity.MachineName = data["MachineName"];
            entity.BaseBoard = data["BaseBoard"];
            entity.Processors = data["Processors"];
            entity.Disk = data["Disk"];
            entity.DiskSerial = data["DiskSerial"];
            entity.Volume = data["Volume"];
            entity.Macs = data["Macs"];
            entity.IPs = data["IPs"];
            entity.OSVersion = data["OSVersion"];
            entity.Memory = data["Memory"].ToBigInt();
            entity.ScreenWidth = data["ScreenWidth"].ToInt();
            entity.ScreenHeight = data["ScreenHeight"].ToInt();
            entity.DiskSize = data["DiskSize"].ToBigInt();
            return entity;
        }
        /// <summary>
        /// 导出XML
        /// </summary>
        /// <returns></returns>
        public virtual string ToXml() {
            return ToExtend().ToXml();
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static SystemInfo FromXml(string xml) {
            if (!string.IsNullOrEmpty(xml)) xml = xml.Trim();
            return FromExtend(xml.FromXml<Dictionary<string, string>>());
        }
#endif
    }
}
