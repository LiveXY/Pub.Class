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
    /// 序列化接口
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface ISerializeString {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="o">对像</param>
        /// <returns>字符串</returns>
        string Serialize<T>(T o);
        /// <summary>
        /// data反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">内容</param>
        /// <returns>对像</returns>
        T Deserialize<T>(string data);
        /// <summary>
        /// 序列成文件
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="fileName">文件名</param>
        void SerializeFile<T>(T o, string fileName);
        /// <summary>
        /// 文件反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns>对像</returns>
        T DeserializeFile<T>(string fileName);
        /// <summary>
        /// 序列化后DES加密
        /// </summary>
        /// <param name="o">对像</param>
        /// <param name="key">加密KEY</param>
        /// <returns>密文</returns>
        string SerializeEncode<T>(T o, string key = "");
        /// <summary>
        /// DES解密后反序列化成对像
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="data">密文</param>
        /// <param name="key">解密KEY</param>
        /// <returns>对像</returns>
        T DecodeDeserialize<T>(string data, string key = "");
    }
}
