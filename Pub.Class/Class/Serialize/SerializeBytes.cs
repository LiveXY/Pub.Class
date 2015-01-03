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
    public class SerializeBytes {
        private readonly ISerializeBytes serializeBytes;
        /// <summary>
        /// 构造器 指定DLL文件和全类名
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public SerializeBytes(string dllFileName, string className) {
            errorMessage = string.Empty;
            if (serializeBytes.IsNull()) {
                serializeBytes = (ISerializeBytes)dllFileName.LoadClass(className);
            }
        }
        /// <summary>
        /// 构造器 指定classNameDllName(SerializeBytesProviderName) 默认Pub.Class.JavaScriptSerializerString
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public SerializeBytes(string classNameAndAssembly) {
            errorMessage = string.Empty;
            if (serializeBytes.IsNull()) {
                if (classNameAndAssembly.IsNullEmpty())
                    serializeBytes = Singleton<BinaryFormatterBytes>.Instance();
                else
                    serializeBytes = (ISerializeBytes)classNameAndAssembly.LoadClass();
            }
        }
        /// <summary>
        /// 构造器 从Web.config中读SerializeBytesProviderName 默认Pub.Class.SimpleSerializeBytes,Pub.Class
        /// </summary>
        public SerializeBytes() {
            errorMessage = string.Empty;
            if (serializeBytes.IsNull()) {
                string classNameAndAssembly = WebConfig.GetApp("SerializeBytesProviderName");
                if (classNameAndAssembly.IsNullEmpty())
                    serializeBytes = Singleton<BinaryFormatterBytes>.Instance();
                else
                    serializeBytes = (ISerializeBytes)classNameAndAssembly.LoadClass();
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
        public byte[] Serialize<T>(T o) {
            errorMessage = string.Empty;
            try {
                return serializeBytes.Serialize(o);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return null;
            }
        }
        ///<summary>
        /// 反序列化
        ///</summary>
        public T Deserialize<T>(byte[] data) {
            errorMessage = string.Empty;
            try {
                return serializeBytes.Deserialize<T>(data);
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
                serializeBytes.SerializeFile(o, fileName);
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
                return serializeBytes.DeserializeFile<T>(fileName);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return default(T);
            }
        }
        ///<summary>
        /// 序列化 DES加密
        ///</summary>
        public byte[] SerializeEncode<T>(T o, string key = "") {
            errorMessage = string.Empty;
            try {
                return serializeBytes.SerializeEncode(o, key);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return null;
            }
        }
        ///<summary>
        /// 反序列化 DES解密
        ///</summary>
        public T DecodeDeserialize<T>(byte[] data, string key = "") {
            errorMessage = string.Empty;
            try {
                return serializeBytes.DecodeDeserialize<T>(data, key);
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return default(T);
            }
        }

        private static ISerializeBytes s_serializeBytes;
        /// <summary>
        /// 使用外部插件
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public static void Use(string dllFileName, string className) {
            s_serializeBytes = (ISerializeBytes)dllFileName.LoadClass(className);
        }
        /// <summary>
        /// 使用外部插件
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public static void Use(string classNameAndAssembly) {
            if (classNameAndAssembly.IsNullEmpty())
                s_serializeBytes = Singleton<BinaryFormatterBytes>.Instance();
            else
                s_serializeBytes = (ISerializeBytes)classNameAndAssembly.LoadClass();
        }
        /// <summary>
        /// 使用外部插件
        /// </summary>
        public static void Use<T>(T t) where T : ISerializeBytes, new() {
            s_serializeBytes = Singleton<T>.Instance();
        }

        ///<summary>
        /// 序列化
        ///</summary>
        public static byte[] ToBytes<T>(T o) {
            try {
                if (s_serializeBytes.IsNull()) {
                    string classNameAndAssembly = WebConfig.GetApp("SerializeBytesProviderName");
                    Use(classNameAndAssembly);
                }
                return s_serializeBytes.Serialize(o);
            } catch (Exception ex) {
                throw ex;
            }
        }
        ///<summary>
        /// 反序列化
        ///</summary>
        public static T FromBytes<T>(byte[] data) {
            try {
                if (s_serializeBytes.IsNull()) {
                    string classNameAndAssembly = WebConfig.GetApp("SerializeBytesProviderName");
                    Use(classNameAndAssembly);
                }
                return s_serializeBytes.Deserialize<T>(data);
            } catch (Exception ex) {
                throw ex;
            }
        }

        public static void RegisterTypes(params Type[] types) { 
            try {
                if (s_serializeBytes.IsNull()) {
                    string classNameAndAssembly = WebConfig.GetApp("SerializeBytesProviderName");
                    Use(classNameAndAssembly);
                }
                s_serializeBytes.RegisterTypes(types);
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
