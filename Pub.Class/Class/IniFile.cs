//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text;
using System.Runtime.InteropServices;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Collections.Generic;

namespace Pub.Class {
    /// <summary>
    /// INI文件操作 ini文件第一行必须为空行
    /// 
    /// 修改纪录
    ///     2007.03.04 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// IniFile ini = new IniFile("c:\\test.ini");
    /// ini.ReadValue("system", "winsize");
    /// ini.WriteValue("system", "winsize", "1024x768");
    /// ini.Remove("system");
    /// </code>
    /// </example>
    /// </summary>
    public class IniFile {
        //#region 私有成员
        private readonly string path;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, byte[] retVal, int size, string filePath);
        //#endregion
        //#region 构造器
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="iniPath">文件名  ini文件第一行必须为空行，文件编码必须是ANSI或Unicode不能为UTF8</param>
        public IniFile(string iniPath) { path = iniPath; }
        //#endregion
        //#region WriteValue
        /// <summary>
        /// 向INI文件写数据
        /// </summary>
        /// <param name="section">结点</param>
        /// <param name="key">名称</param>
        /// <param name="value">值</param>
        public IniFile WriteValue(string section, string key, string value) {
            if (section.IsNullEmpty() || key.IsNullEmpty() || value.IsNullEmpty()) return this;
            WritePrivateProfileString(section, key, value, this.path);
            return this;
        }
        //#endregion
        //#region ReadValue
        /// <summary>
        /// 向INI文件读数据
        /// </summary>
        /// <param name="section">结点</param>
        /// <param name="key">名称</param>
        /// <returns>数据</returns>
        public string ReadValue(string section, string key) {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
            return temp.ToString();
        }
        /// <summary>
        /// 返回指定section下的所有key和value
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public IDictionary<string, string> ReadValues(string section) {
            IDictionary<string, string> list = new Dictionary<string, string>();
            foreach (var key in ReadKeys(section)) list.Add(key, ReadValue(section, key));
            return list;
        }
        /// <summary>
        /// 返回所有section、key和value
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, IDictionary<string, string>> ReadValues() {
            IDictionary<string, IDictionary<string, string>> list = new Dictionary<string, IDictionary<string, string>>();
            foreach (var key in ReadSections()) list.Add(key, ReadValues(key));
            return list;
        }
        /// <summary>
        /// 读所有section
        /// </summary>
        /// <returns>读所有数据</returns>
        public IList<string> ReadSections() {
            return ByteToString(ReadKeys(null, null));
        }
        /// <summary>
        /// 读section下的所有key
        /// </summary>
        /// <param name="section">结点</param>
        /// <returns>读section下的所有数据</returns>
        public IList<string> ReadKeys(string section) {
            return ByteToString(ReadKeys(section, null));
        }
        /// <summary>
        /// 读section下的所有数据
        /// </summary>
        /// <param name="section">结点</param>
        /// <param name="key">名称</param>
        /// <returns>读section下的所有数据</returns>
        public byte[] ReadKeys(string section, string key) {
            byte[] retVal = new byte[0xff];
            GetPrivateProfileString(section, key, "", retVal, 0xff, this.path);
            return retVal;
        }
        /// <summary>
        /// ByteToString
        /// </summary>
        /// <param name="sectionByte">结点</param>
        /// <returns>读section下的所有数据</returns>
        private IList<string> ByteToString(byte[] sectionByte) {
            return new ASCIIEncoding().GetString(sectionByte).Split(new char[1]).ToList<string>().Where(p => p.Trim().Length > 0).Distinct().ToList();
        }
        //#endregion
        //#region Remove
        /// <summary>
        /// 删除所有
        /// </summary>
        public IniFile RemoveAll() {
            foreach (var info in ReadSections()) Remove(info);
            return this;
        }
        /// <summary>
        /// 删除section
        /// </summary>
        /// <param name="section">结点</param>
        public IniFile Remove(string section) {
            if (section.IsNullEmpty()) return this;
            WritePrivateProfileString(section, null, null, this.path);
            return this;
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="section">结点</param>
        /// <param name="key">key</param>
        public IniFile Remove(string section, string key) {
            if (section.IsNullEmpty() || key.IsNullEmpty()) return this;
            WritePrivateProfileString(section, key, null, this.path);
            return this;
        }
        //#endregion
        /// <summary>
        /// 数组属性 使用ini["system","win_width"] = 0;
        /// </summary>
        /// <param name="section">结点</param>
        /// <param name="key">名称</param>
        /// <returns></returns>
        public string this[string section, string key] {
            get { return ReadValue(section, key); }
            set { WriteValue(section, key, value); }
        }
    }

}