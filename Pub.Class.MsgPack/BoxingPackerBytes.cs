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
using MsgPack;

namespace Pub.Class.MsgPack {
    /// <summary>
    /// MsgPack序列化
    /// 
    /// 修改纪录
    ///     2013.02.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class BoxingPackerBytes : ISerializeBytes {
        private readonly static BoxingPacker packer = new BoxingPacker();
        public void RegisterTypes(params Type[] types) { }
        /// <summary>
        /// 序列成bytes
        /// </summary>
        /// <param name="o">对像</param>
        /// <returns>bytes</returns>
        public byte[] Serialize<T>(T o) {
            return packer.Pack(o);
        }
        /// <summary>
        /// bytes反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">bytes</param>
        /// <returns>对像</returns>
        public T Deserialize<T>(byte[] data) {
            return (T)packer.Unpack(data);
        }
        /// <summary>
        /// 序列成bytes文件
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="fileName">文件名</param>
        public void SerializeFile<T>(T o, string fileName) {
            FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, Serialize(o).ToUTF8());
        }
        /// <summary>
        /// bytes文件反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns>对像</returns>
        public T DeserializeFile<T>(string fileName) {
            byte[] data = FileDirectory.FileReadAll(fileName, Encoding.UTF8).FromBase64();
            return Deserialize<T>(data);
        }
        /// <summary>
        /// 序列成bytes后DES加密
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="key">加密KEY</param>
        /// <returns>bytes密文</returns>
        public byte[] SerializeEncode<T>(T o, string key = "") {
            return key.IsNullEmpty() ? Serialize(o) : Serialize(o).ToUTF8().DESEncode(key).FromBase64();
        }
        /// <summary>
        /// DES解密后反序列成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">bytes密文</param>
        /// <param name="key">解密KEY</param>
        /// <returns>对像</returns>
        public T DecodeDeserialize<T>(byte[] data, string key = "") {
            return key.IsNullEmpty() ? Deserialize<T>(data) : Deserialize<T>(data.ToUTF8().DESDecode(key).FromBase64());
        }
    }
}
