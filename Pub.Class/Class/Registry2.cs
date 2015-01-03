//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using Microsoft.Win32;

namespace Pub.Class {
    /// <summary>
    /// 注册表根键枚举
    /// 
    /// 修改纪录
    ///     2012.01.06 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public enum RegistryEnum { 
        /// <summary>
        /// LocalMachine HKEY_LOCAL_MACHINE
        /// </summary>
        LocalMachine,
        /// <summary>
        /// ClassesRoot HKEY_CLASSES_ROOT
        /// </summary>
        ClassesRoot,
        /// <summary>
        /// CurrentConfig HKEY_CURRENT_CONFIG
        /// </summary>
        CurrentConfig,
        /// <summary>
        /// CurrentUser HKEY_CURRENT_USER
        /// </summary>
        CurrentUser,
        /// <summary>
        /// DynData
        /// </summary>
        DynData,
        /// <summary>
        /// PerformanceData
        /// </summary>
        PerformanceData,
        /// <summary>
        /// Users HKEY_USERS
        /// </summary>
        Users
    }
    /// <summary>
    /// 注册表操作
    /// 
    /// 修改纪录
    ///     2012.01.06 版本：1.1 livexy 修改加入RegistryEnum
    ///     2007.04.29 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// string path = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    /// string key = "ToSwfFlv";
    /// Msg.Write(Registry2.Exists(path, "360Disabled"));
    /// string str = Registry2.Read(path, key).ToStr();
    /// if (str.IsNullEmpty()) Registry2.Write(path, key, "ToSwfFlv.exe");
    /// Msg.Write(str);
    /// Registry2.Delete(path, key);
    /// </code>
    /// </example>
    /// </summary>
    public static class Registry2 {
        /// <summary>
        /// 读注册表
        /// </summary>
        /// <param name="registryEnum">注册表根键枚举</param>
        /// <param name="path">路径</param>
        /// <param name="valueName">key</param>
        /// <returns>值</returns>
        public static object Read(RegistryEnum registryEnum, string path, string valueName) {
            RegistryKey key = null;
            switch (registryEnum){
                case RegistryEnum.LocalMachine: key = Registry.LocalMachine; break;
                case RegistryEnum.ClassesRoot: key = Registry.ClassesRoot; break;
                case RegistryEnum.CurrentConfig: key = Registry.CurrentConfig; break;
                case RegistryEnum.CurrentUser: key = Registry.CurrentUser; break;
                case RegistryEnum.DynData: key = Registry.DynData; break;
                case RegistryEnum.PerformanceData: key = Registry.PerformanceData; break;
                case RegistryEnum.Users: key = Registry.Users; break;
            }

            string[] parts = path.Trim('\\').Split('\\');
            if (parts.IsNull() || parts.Length == 0) return null;
            for (int x = 0; x < parts.Length; x++) {
                key = key.OpenSubKey(parts[x]);
                if (key.IsNull()) return null;
                if (x == parts.Length - 1) return key.GetValue(valueName, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
            }
            return null;
        }
        /// <summary>
        /// 读注册表
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="valueName">key</param>
        /// <returns>值</returns>
        public static object Read(string path, string valueName) {
            return Read(RegistryEnum.LocalMachine, path, valueName);
        }
        /// <summary>
        /// 写注册表
        /// </summary>
        /// <param name="registryEnum">注册表根键枚举</param>
        /// <param name="path">路径</param>
        /// <param name="valueName">key</param>
        /// <param name="value">value</param>
        public static void Write(RegistryEnum registryEnum, string path, string valueName, object value) {
            RegistryKey key = null;
            switch (registryEnum){
                case RegistryEnum.LocalMachine: key = Registry.LocalMachine; break;
                case RegistryEnum.ClassesRoot: key = Registry.ClassesRoot; break;
                case RegistryEnum.CurrentConfig: key = Registry.CurrentConfig; break;
                case RegistryEnum.CurrentUser: key = Registry.CurrentUser; break;
                case RegistryEnum.DynData: key = Registry.DynData; break;
                case RegistryEnum.PerformanceData: key = Registry.PerformanceData; break;
                case RegistryEnum.Users: key = Registry.Users; break;
            }
            RegistryKey lastKey = key;
            string[] parts = path.Trim('\\').Split('\\');
            if (parts.IsNull() || parts.Length == 0) return;

            for (int x = 0; x < parts.Length; x++) {
                key = key.OpenSubKey(parts[x], true);
                if (key.IsNull()) key = lastKey.CreateSubKey(parts[x]);
                if (x == parts.Length - 1) {
                    if (value is string) {
                        key.SetValue(valueName, value.ToString());
                    } else if (value is uint || value is int || value.GetType().IsEnum) {
                        object o = key.GetValue(valueName, null);

                        if (o.IsNull()) {
                            key.SetValue(valueName, value, RegistryValueKind.DWord);
                        } else {
                            RegistryValueKind kind = key.GetValueKind(valueName);

                            if (kind == RegistryValueKind.DWord) {
                                key.SetValue(valueName, value, RegistryValueKind.DWord);
                            } else if (kind == RegistryValueKind.Binary) {
                                uint num = (uint)value;

                                byte[] b = new byte[4];
                                b[0] = (byte)((num & 0x000000FF) >> 0);
                                b[1] = (byte)((num & 0x0000FF00) >> 1);
                                b[2] = (byte)((num & 0x00FF0000) >> 2);
                                b[3] = (byte)((num & 0xFF000000) >> 3);

                                b[0] = (byte)((num & 0x000000FF) >> 0);
                                b[1] = (byte)((num & 0x0000FF00) >> 8);
                                b[2] = (byte)((num & 0x00FF0000) >> 16);
                                b[3] = (byte)((num & 0xFF000000) >> 24);

                                key.SetValue(valueName, b, RegistryValueKind.Binary);
                            } else if (kind == RegistryValueKind.String) {
                                key.SetValue(valueName, "x" + ((uint)value).ToString("X8"));
                            }
                        }
                    } else if (value is Guid) {
                        key.SetValue(valueName, ((Guid)value).ToString("B"));
                    }
                }

                lastKey = key;
            }

            if (key.IsNotNull()) key.Close();
        }
        /// <summary>
        /// 写注册表 Registry.LocalMachine HKEY_LOCAL_MACHINE 
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="valueName">key</param>
        /// <param name="value">value</param>
        public static void Write(string path, string valueName, object value) {
            Write(RegistryEnum.LocalMachine, path, valueName, value);
        }
        /// <summary>
        /// 删除注册表
        /// </summary>
        /// <param name="registryEnum">注册表根键枚举</param>
        /// <param name="path">路径</param>
        /// <param name="valueName">key</param>
        public static void Delete(RegistryEnum registryEnum, string path, string valueName) {
            RegistryKey key = null;
            switch (registryEnum){
                case RegistryEnum.LocalMachine: key = Registry.LocalMachine; break;
                case RegistryEnum.ClassesRoot: key = Registry.ClassesRoot; break;
                case RegistryEnum.CurrentConfig: key = Registry.CurrentConfig; break;
                case RegistryEnum.CurrentUser: key = Registry.CurrentUser; break;
                case RegistryEnum.DynData: key = Registry.DynData; break;
                case RegistryEnum.PerformanceData: key = Registry.PerformanceData; break;
                case RegistryEnum.Users: key = Registry.Users; break;
            }

            string[] parts = path.Trim('\\').Split('\\');
            if (parts.IsNull() || parts.Length == 0) return;
            for (int x = 0; x < parts.Length; x++) {
                key = key.OpenSubKey(parts[x], true);
                if (key.IsNull()) return;
                if (x == parts.Length - 1) key.DeleteValue(valueName, false);
            }
        }
        /// <summary>
        /// 删除注册表
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="valueName">key</param>
        public static void Delete(string path, string valueName) {
            Delete(RegistryEnum.LocalMachine, path, valueName);
        }
        /// <summary>
        /// 判断一个注册表项是否存在
        /// </summary>
        /// <param name="registryEnum">注册表根键枚举</param>
        /// <param name="path">注册表子项</param>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public static bool Exists(RegistryEnum registryEnum, string path, string key) {
            bool returnValue = false; 
            RegistryKey registryKey = null;
            switch (registryEnum){
                case RegistryEnum.LocalMachine: registryKey = Registry.LocalMachine.OpenSubKey(path, false); break;
                case RegistryEnum.ClassesRoot: registryKey = Registry.ClassesRoot.OpenSubKey(path, false); break;
                case RegistryEnum.CurrentConfig: registryKey = Registry.CurrentConfig.OpenSubKey(path, false); break;
                case RegistryEnum.CurrentUser: registryKey = Registry.CurrentUser.OpenSubKey(path, false); break;
                case RegistryEnum.DynData: registryKey = Registry.DynData.OpenSubKey(path, false); break;
                case RegistryEnum.PerformanceData: registryKey = Registry.PerformanceData.OpenSubKey(path, false); break;
                case RegistryEnum.Users: registryKey = Registry.Users.OpenSubKey(path, false); break;
            }
            
            string[] SubKeyNames = registryKey.GetSubKeyNames();
            for (int i = 0; i < SubKeyNames.Length; i++) {
                //Msg.Write(SubKeyNames[i]);
                if (key.Equals(SubKeyNames[i])) {
                    returnValue = true;
                    break;
                }
            }
            return returnValue;
        }
        /// <summary>
        /// 判断一个注册表项是否存在 Registry.LocalMachine HKEY_LOCAL_MACHINE
        /// </summary>
        /// <param name="path">注册表子项</param>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string path, string key) {
            return Exists(RegistryEnum.LocalMachine, path, key);
        }
        public static string[] GetAllSubKey(RegistryEnum registryEnum, string path) { 
            RegistryKey key = null;
            switch (registryEnum){
                case RegistryEnum.LocalMachine: key = Registry.LocalMachine; break;
                case RegistryEnum.ClassesRoot: key = Registry.ClassesRoot; break;
                case RegistryEnum.CurrentConfig: key = Registry.CurrentConfig; break;
                case RegistryEnum.CurrentUser: key = Registry.CurrentUser; break;
                case RegistryEnum.DynData: key = Registry.DynData; break;
                case RegistryEnum.PerformanceData: key = Registry.PerformanceData; break;
                case RegistryEnum.Users: key = Registry.Users; break;
            }

            string[] parts = path.Trim('\\').Split('\\');
            if (parts.IsNull() || parts.Length == 0) return null;
            for (int x = 0; x < parts.Length; x++) {
                key = key.OpenSubKey(parts[x]);
                if (key.IsNull()) return null;
                if (x == parts.Length - 1) return key.GetSubKeyNames();
            }
            return new string[0];
        }
    }
}
