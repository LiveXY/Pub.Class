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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

namespace Pub.Class {
    /// <summary>
    /// binary序列化和反序列化
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// User u1 = new User() { UserID = 1000, Name = "熊华春" };
    /// var serialize = new BinarySerializeString();
    /// string s = serialize.Serialize(u1);
    /// serialize.Deserialize&lt;User>(s);
    /// </example>
    /// </code>
    /// </summary>
    public class BinaryFormatterString : ISerializeString {
        /// <summary>
        /// 序列成16进制字符串
        /// </summary>
        /// <param name="o">对像</param>
        /// <returns>16进制字符串</returns>
        public string Serialize<T>(T o) {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                formatter.Serialize(ms, o);
                return ms.ToArray().ToUTF8();
            }
        }
        /// <summary>
        /// 16进制字符串反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">16进制字符串</param>
        /// <returns>对像</returns>
        public T Deserialize<T>(string data) {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data.FromBase64())) return (T)formatter.Deserialize(ms);
        }
        /// <summary>
        /// 序列成16进制字符串文件
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="fileName">文件名</param>
        public void SerializeFile<T>(T o, string fileName) {
            FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, Serialize(o));
        }
        /// <summary>
        /// 16进制字符串文件反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns>对像</returns>
        public T DeserializeFile<T>(string fileName) {
            string data = FileDirectory.FileReadAll(fileName, Encoding.UTF8);
            return Deserialize<T>(data);
        }
        /// <summary>
        /// 序列成16进制字符串后DES加密
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="key">加密KEY</param>
        /// <returns>16进制字符串密文</returns>
        public string SerializeEncode<T>(T o, string key = "") {
            return key.IsNullEmpty() ? Serialize(o) : Serialize(o).DESEncode(key);
        }
        /// <summary>
        /// DES解密后反序列成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">16进制字符串密文</param>
        /// <param name="key">解密KEY</param>
        /// <returns>对像</returns>
        public T DecodeDeserialize<T>(string data, string key = "") {
            return key.IsNullEmpty() ? Deserialize<T>(data) : Deserialize<T>(data.DESDecode(key));
        }
    }
}
