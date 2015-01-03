//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Pub.Class;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Data.Common;

namespace Pub.Class {
    /// <summary>
    /// 用户信息实体
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class UserInfo {
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo() {
            this.Name = null;
            this.Sex = 0;
            this.Email = null;
            this.Token = null;
            this.UserID = null;
            this.Secret = null;
            this.Header = null;
            this.Address = null;
        }
        /// <summary>
        /// UserID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别 1男，0女
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Header
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Secret
        /// </summary>
        public string Secret { get; set; }
    }
    /// <summary>
    /// 配置信息实体
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class ConfigInfo { 
        /// <summary>
        /// 授权信息
        /// </summary>
        public ConfigInfo() {
            this.AppKey = null;
            this.AppSecret = null;
            this.RedirectUrl = null;
        }
        /// <summary>
        /// appSecret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// app key
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}
