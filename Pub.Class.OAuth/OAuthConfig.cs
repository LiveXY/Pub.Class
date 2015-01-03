//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;

namespace Pub.Class {
    /// <summary>
    /// OAuthConfig
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class OAuthConfig {
        /// <summary>
        /// 取OAuth.webconfig配置信息
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <returns>取OAuth.webconfig配置信息</returns>
        public static ConfigInfo GetConfigInfo(OAuthEnum authEnum) {
            Xml2 xml = new Xml2("~/oauth.config".GetMapPath());
            string _enum = authEnum.ToString();
            return new ConfigInfo() { 
                AppKey = xml.GetNodeText("/configuration/" + _enum + "/appKey"),
                AppSecret = xml.GetNodeText("/configuration/" + _enum + "/appSecret"),
                RedirectUrl = xml.GetNodeText("/configuration/" + _enum + "/redirectUrl"),
            };
        }
    }

}

