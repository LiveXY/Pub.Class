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

namespace Pub.Class {
    /// <summary>
    /// 序列化反序列化操作类
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    ///     using (SerializeHelper s = new SerializeHelper(SerializeEnum.json)) { 
    ///         s.Serialize(u);
    ///         s.Deserialize&lt;User>(s.Serialize(u))
    ///     }
    ///     using (SerializeHelper s = new SerializeHelper(SerializeEnum.xml)) { 
    ///         s.SerializeFile(u, "c:\test.txt");
    ///         s.DeserializeFile&lt;User>(s.SerializeFile(u, "c:\test.txt"))
    ///     }
    ///     using (SerializeHelper s = new SerializeHelper(SerializeEnum.binary)) { 
    ///         s.SerializeFile(u, "c:\test.txt");
    ///         s.DeserializeFile&lt;User>(s.SerializeFile(u, "c:\test.txt"))
    ///     }
    ///     using (SerializeHelper s = new SerializeHelper("json")) { 
    ///         s.SerializeEncode(u, "test");
    ///         s.DecodeDeserialize&lt;User>(s.SerializeEncode(u, "test"))
    ///     }
    ///     using (SerializeHelper s = new SerializeHelper("xml")) { 
    ///         s.SerializeEncode(u, "");
    ///         s.DecodeDeserialize&lt;User>(s.SerializeEncode(u, ""))
    ///     }
    ///     using (SerializeHelper s = new SerializeHelper("binary")) { 
    ///         s.SerializeEncode(u, "");
    ///         s.DecodeDeserialize&lt;User>(s.SerializeEncode(u, ""))
    ///     }
    /// </example>
    /// </code>
    /// </summary>
    public class SerializeHelper : Disposable {
        private SerializeEnum serializeEnum;
        private ISerialize serialize = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serializeEnum">序列化类型 string</param>
        public SerializeHelper(string serializeEnum) {
            this.serializeEnum = serializeEnum.ToEnum<SerializeEnum>();
            init();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serializeEnum">序列化类型 enum</param>
        public SerializeHelper(SerializeEnum serializeEnum) {
            this.serializeEnum = serializeEnum;
            init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void init() { 
            switch (this.serializeEnum) {
                case SerializeEnum.xml: this.serialize = new XmlSerialize(); break;
                case SerializeEnum.json: this.serialize = new JsonSerialize(); break;
                case SerializeEnum.binary: this.serialize = new BinarySerialize(); break;
                default: this.serialize = new JsonSerialize(); break;
            }
        }
        /// <summary>
        /// 用using 自动释放
        /// </summary>
        protected override void InternalDispose() {
            serialize = null;
            base.InternalDispose();
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="o">对像</param>
        /// <returns>字符串</returns>
        public string Serialize(object o) {
            return this.serialize.Serialize(o);
        }
        /// <summary>
        /// data反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">内容</param>
        /// <returns>对像</returns>
        public T Deserialize<T>(string data) {
            return this.serialize.Deserialize<T>(data);
        }
        /// <summary>
        /// 序列成文件
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="fileName">文件名</param>
        public void SerializeFile(string o, string fileName) {
            this.serialize.SerializeFile(o, fileName);
        }
        /// <summary>
        /// 文件反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns>对像</returns>
        public T DeserializeFile<T>(string fileName) {
            return this.serialize.DeserializeFile<T>(fileName);
        }
        /// <summary>
        /// 序列化后DES加密
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="key">加密KEY</param>
        /// <returns>密文</returns>
        public string SerializeEncode(object o, string key = "") {
            return this.serialize.SerializeEncode(o, key);
        }
        /// <summary>
        /// DES解密后反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">密文</param>
        /// <param name="key">解密KEY</param>
        /// <returns>对像</returns>
        public T DecodeDeserialize<T>(string data, string key = "") {
            return this.serialize.DecodeDeserialize<T>(data, key);
        }
    }
}
