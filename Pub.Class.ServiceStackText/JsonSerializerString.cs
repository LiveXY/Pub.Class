//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ServiceStack.Text;

namespace Pub.Class.ServiceStackText {
    /// <summary>
    /// ServiceStackText序列化
    /// 
    /// 修改纪录
    ///     2013.02.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class JsonSerializerString : ISerializeString {
        /// <summary>
        /// 序列成json
        /// </summary>
        /// <param name="o">对像</param>
        /// <returns>json</returns>
        public string Serialize<T>(T o) {
            return JsonSerializer.SerializeToString(o);
        }
        /// <summary>
        /// json反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">xml</param>
        /// <returns>对像</returns>
        public T Deserialize<T>(string data) {
            return JsonSerializer.DeserializeFromString<T>(data);
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
