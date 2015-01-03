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
using System.Xml.Serialization;
using System.IO;

namespace Pub.Class {
    /// <summary>
    /// XML序列化和反序列化
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// User u1 = new User() { UserID = 1000, Name = "熊华春" };
    /// var serialize = new XmlSerialize();
    /// string s = serialize.Serialize(u1);
    /// serialize.Deserialize&lt;User>(s);
    /// </example>
    /// </code>
    /// </summary>
    public class XmlSerializerString : ISerializeString {
        /// <summary>
        /// 序列成XML
        /// </summary>
        /// <param name="o">对像</param>
        /// <returns>XML</returns>
        public string Serialize<T>(T o) {
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            StringBuilder stringBuilder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(stringBuilder)) serializer.Serialize(textWriter, o);
            return stringBuilder.ToString();
        }
        /// <summary>
        /// XML反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">xml</param>
        /// <returns>对像</returns>
        public T Deserialize<T>(string data) {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader textReader = new StringReader(data)) return (T)serializer.Deserialize(textReader);
        }
        /// <summary>
        /// 序列成XML文件
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="fileName">文件名</param>
        public void SerializeFile<T>(T o, string fileName) {
            FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, Serialize(o));
        }
        /// <summary>
        /// XML文件反序列化成对像
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
