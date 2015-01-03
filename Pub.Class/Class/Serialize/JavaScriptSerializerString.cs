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
using System.Text;
#if !NET20
using System.Web.Script.Serialization;
#endif

namespace Pub.Class {
    /// <summary>
    /// json序列化和反序列化
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// User u1 = new User() { UserID = 1000, Name = "熊华春" };
    /// var serialize = new JavaScriptSerializerString();
    /// string s = serialize.Serialize(u1);
    /// serialize.Deserialize&lt;User>(s);
    /// </example>
    /// </code>
    /// </summary>
    public class JavaScriptSerializerString : ISerializeString {
        /// <summary>
        /// 序列成json
        /// </summary>
        /// <param name="o">对像</param>
        /// <returns>json</returns>
        public string Serialize<T>(T o) {
#if NET20
            return Json.ToJsonString(o);
#else
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            return serializer.Serialize(o);
#endif
        }
        /// <summary>
        /// json反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">xml</param>
        /// <returns>对像</returns>
        public T Deserialize<T>(string data) {
            //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //if (typeof(T) == typeof(DateTime)) {
            //    object obj = jsonSerializer.Deserialize<DateTime>(data).ToLocalTime();
            //    return (T)obj;
            //} else return jsonSerializer.Deserialize<T>(data);
#if NET20
            return Json.ToObject<T>(data);
#else
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Deserialize<T>(data);
#endif
        }
        /// <summary>
        /// 序列成json文件
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="fileName">文件名</param>
        public void SerializeFile<T>(T o, string fileName) {
            FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, Serialize(o));
        }
        /// <summary>
        /// json文件反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns>对像</returns>
        public T DeserializeFile<T>(string fileName) {
            string data = FileDirectory.FileReadAll(fileName, Encoding.UTF8);
            return Deserialize<T>(data);
        }
        /// <summary>
        /// 序列成XML后DES加密
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="key">加密KEY</param>
        /// <returns>XML密文</returns>
        public string SerializeEncode<T>(T o, string key = "") {
            return key.IsNullEmpty() ? Serialize(o) : Serialize(o).DESEncode(key);
        }
        /// <summary>
        /// DES解密后反序列成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">XML密文</param>
        /// <param name="key">解密KEY</param>
        /// <returns>对像</returns>
        public T DecodeDeserialize<T>(string data, string key = "") {
            return key.IsNullEmpty() ? Deserialize<T>(data) : Deserialize<T>(data.DESDecode(key));
        }
    }
}
