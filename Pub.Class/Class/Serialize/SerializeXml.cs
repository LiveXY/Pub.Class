//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2011 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Text;
using System.Net.Mime;
using System.Net;

namespace Pub.Class {
    /// <summary>
    /// 写日志
    /// 
    /// 修改纪录
    ///     2013.02.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class SerializeXml {
        private readonly ISerializeString serializeString;
        /// <summary>
        /// 构造器 指定DLL文件和全类名
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public SerializeXml(string dllFileName, string className) {
            errorMessage = string.Empty;
            if (serializeString.IsNull()) {
                serializeString = (ISerializeString)dllFileName.LoadClass(className);
            }
        }
        /// <summary>
        /// 构造器 指定classNameDllName(SerializeXmlProviderName) 默认Pub.Class.JavaScriptSerializerString
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public SerializeXml(string classNameAndAssembly) {
            errorMessage = string.Empty;
            if (serializeString.IsNull()) {
                if (classNameAndAssembly.IsNullEmpty())
                    serializeString = Singleton<XmlSerializerString>.Instance();
                else
                    serializeString = (ISerializeString)classNameAndAssembly.LoadClass();
            }
        }
        /// <summary>
        /// 构造器 从Web.config中读SerializeXmlProviderName 默认Pub.Class.SimpleSerializeString,Pub.Class
        /// </summary>
        public SerializeXml() {
            errorMessage = string.Empty;
            if (serializeString.IsNull()) {
                string classNameAndAssembly = WebConfig.GetApp("SerializeXmlProviderName");
                if (classNameAndAssembly.IsNullEmpty())
                    serializeString = Singleton<XmlSerializerString>.Instance();
                else
                    serializeString = (ISerializeString)classNameAndAssembly.LoadClass();
            }
        }
        private string errorMessage = string.Empty;
        /// <summary>
        /// 出错消息
        /// </summary>
        public string ErrorMessage { get { return errorMessage; } }
        ///<summary>
        /// 序列化
        ///</summary>
        public string Serialize<T>(T o) {
            errorMessage = string.Empty;
            try {
                return serializeString.Serialize(o);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return null;
            }
        }
        ///<summary>
        /// 反序列化
        ///</summary>
        public T Deserialize<T>(string data) {
            errorMessage = string.Empty;
            try {
                return serializeString.Deserialize<T>(data);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return default(T);
            }
        }
        ///<summary>
        /// 序列化文件
        ///</summary>
        public void SerializeFile<T>(T o, string fileName) {
            errorMessage = string.Empty;
            try {
                serializeString.SerializeFile(o, fileName);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
            }
        }
        ///<summary>
        /// 反序列化文件
        ///</summary>
        public T DeserializeFile<T>(string fileName) {
            errorMessage = string.Empty;
            try {
                return serializeString.DeserializeFile<T>(fileName);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return default(T);
            }
        }
        ///<summary>
        /// 序列化 DES加密
        ///</summary>
        public string SerializeEncode<T>(T o, string key = "") {
            errorMessage = string.Empty;
            try {
                return serializeString.SerializeEncode(o, key);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return null;
            }
        }
        ///<summary>
        /// 反序列化 DES解密
        ///</summary>
        public T DecodeDeserialize<T>(string data, string key = "") {
            errorMessage = string.Empty;
            try {
                return serializeString.DecodeDeserialize<T>(data, key);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return default(T);
            }
        }

        private static ISerializeString s_serializeString;
        /// <summary>
        /// 使用外部插件
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public static void Use(string dllFileName, string className) {
            s_serializeString = (ISerializeString)dllFileName.LoadClass(className);
        }
        /// <summary>
        /// 使用外部插件
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public static void Use(string classNameAndAssembly) {
            if (classNameAndAssembly.IsNullEmpty())
                s_serializeString = Singleton<XmlSerializerString>.Instance();
            else
                s_serializeString = (ISerializeString)classNameAndAssembly.LoadClass();
        }
        /// <summary>
        /// 使用外部插件
        /// </summary>
        public static void Use<T>(T t) where T : ISerializeString, new() {
            s_serializeString = Singleton<T>.Instance();
        }

        ///<summary>
        /// 序列化
        ///</summary>
        public static string ToXml<T>(T o) {
            try {
                if (s_serializeString.IsNull()) {
                    string classNameAndAssembly = WebConfig.GetApp("SerializeXmlProviderName");
                    Use(classNameAndAssembly);
                }
                return s_serializeString.Serialize(o);
            } catch (Exception ex) {
                throw ex;
            }
        }
        ///<summary>
        /// 反序列化
        ///</summary>
        public static T FromXml<T>(string data) {
            try {
                if (s_serializeString.IsNull()) {
                    string classNameAndAssembly = WebConfig.GetApp("SerializeXmlProviderName");
                    Use(classNameAndAssembly);
                }
                return s_serializeString.Deserialize<T>(data);
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
