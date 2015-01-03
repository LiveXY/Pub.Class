//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Pub.Class {
    /// <summary>
    /// 多语言类
    /// 
    /// 修改纪录
    ///     2006.05.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class WinLang {
        //#region WinLang
        /// <summary>
        /// 语言文件目录
        /// </summary>
        public static string LangPath = string.Empty;
        /// <summary>
        /// 初始化语言文件目录
        /// </summary>
        /// <param name="langPath">语言文件目录</param>
        public static void SetPath(string langPath) {
            LangPath = langPath.TrimEnd('\\') + "\\";
        }
        /// <summary>
        /// 返回所有语言文件列表 0为当前使用的语言
        /// </summary>
        /// <returns>返回所有语言文件列表 0为当前使用的语言</returns>
        public static string[] GetLangList() {
            DirectoryInfo Folder = new DirectoryInfo(LangPath);
            FileInfo[] subFiles = Folder.GetFiles();
            string fileList = string.Empty;

            string LangConfigFile = LangPath + "langConfig.inf";
            IniFile ini = new IniFile(LangConfigFile);
            string DefaultLang = ini.ReadValue("Language", "DefaultLang");
            string firstLang = string.Empty;

            for (int j = 0; j < subFiles.Length; j++) {
                if (subFiles[j].Extension.ToLower().Equals(".ini")) {
                    firstLang = subFiles[0].Name.Substring(0, subFiles[0].Name.Length - 4);
                    fileList += subFiles[j].Name.Substring(0, subFiles[j].Name.Length - 4) + "|";
                }
            }

            if (DefaultLang.Equals("Cookies")) {
                DefaultLang = Cookie2.Get("Lang", "Default").Trim().Base64Decode();
                if (!DefaultLang.Equals(string.Empty)) fileList = DefaultLang + "|" + fileList; else fileList = firstLang + "|" + fileList;
            } else fileList = DefaultLang + "|" + fileList;
            return fileList.TrimEnd('|').Split('|');
        }
        /// <summary>
        /// 修改使用的语言
        /// </summary>
        /// <param name="langName">语言</param>
        public static void SetLang(string langName) {
            string LangConfigFile = LangPath + "langConfig.inf";
            IniFile ini = new IniFile(LangConfigFile);
            ini.WriteValue("Language", "DefaultLang", langName);
        }
        /// <summary>
        /// 生成下拉选择语言列表
        /// </summary>
        /// <returns>生成下拉选择语言列表</returns>
        public static string cboLangList() {
            string LangConfigFile = LangPath + "langConfig.inf";
            IniFile ini = new IniFile(LangConfigFile);
            string DefaultLang = ini.ReadValue("Language", "DefaultLang");
            string CookiesDefaultLang = ini.ReadValue("Language", "CookiesDefaultLang");

            string _value = string.Empty;
            if (DefaultLang.Equals("Cookies")) {
                DefaultLang = Cookie2.Get("Lang", "Default").Trim().Base64Decode();
                if (DefaultLang.Trim().Equals(string.Empty)) DefaultLang = CookiesDefaultLang;
                _value = "<select name=\"cboLangList\" onchange=\"window.location='changeLang.aspx?lang=' + this.value\">";
                string[] LangArr = GetLangList();
                for (int i = 1; i < LangArr.Length; i++) {
                    _value += "<option value=\"" + LangArr[i] + "\" " + Selected(DefaultLang, LangArr[i]) + ">" + LangArr[i] + "</option>";
                }
                _value += "</select>";
            }
            return _value;
        }
        /// <summary>
        /// 返回selected
        /// </summary>
        /// <param name="str1">值1</param>
        /// <param name="str2">值2</param>
        /// <returns>返回selected</returns>
        public static string Selected(string str1, string str2) {
            if (str1 == str2) return "selected"; else return string.Empty;
        }
        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="section">section</param>
        /// <param name="key">KEY</param>
        /// <returns>返回消息</returns>
        public static string Msg(string section, string key) {
            string LangConfigFile = LangPath + "langConfig.inf";
            IniFile ini = new IniFile(LangConfigFile);
            string DefaultLang = ini.ReadValue("Language", "DefaultLang");
            string CookiesDefaultLang = ini.ReadValue("Language", "CookiesDefaultLang");

            if (DefaultLang.Equals("Cookies")) DefaultLang = Cookie2.Get("Lang", "Default").Trim().Base64Decode();
            if (DefaultLang.Trim().Equals(string.Empty)) {
                LangConfigFile = LangPath + CookiesDefaultLang + ".ini";
            } else LangConfigFile = LangPath + DefaultLang + ".ini";
            //Pub.Class.Msg.Write(LangConfigFile);
            ini = new IniFile(LangConfigFile);
            string msgValue = ini.ReadValue("system", "msgExit");
            return msgValue;
        }
        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="frmName">WINFORM</param>
        /// <param name="section">section</param>
        /// <param name="key">key</param>
        /// <returns>返回消息</returns>
        public static string Msg(string frmName, string section, string key) {
            if (frmName.Equals(string.Empty)) return string.Empty;
            string LangConfigFile = LangPath + "langConfig.inf";
            IniFile ini = new IniFile(LangConfigFile);
            string DefaultLang = ini.ReadValue("Language", "DefaultLang");
            string CookiesDefaultLang = ini.ReadValue("Language", "CookiesDefaultLang");

            if (DefaultLang.Equals("Cookies")) DefaultLang = Cookie2.Get("Lang", "Default").Trim().Base64Decode();
            if (DefaultLang.Trim().Equals(string.Empty)) {
                LangConfigFile = LangPath + CookiesDefaultLang + "\\" + frmName + ".ini";
            } else LangConfigFile = LangPath + DefaultLang + "\\" + frmName + ".ini";
            //Pub.Class.Msg.Write(LangConfigFile);
            ini = new IniFile(LangConfigFile);
            string msgValue = ini.ReadValue("system", "msgExit");
            return msgValue;
        }
        //#endregion
    }
}
