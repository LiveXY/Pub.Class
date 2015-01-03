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
    /// 登录授权
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class OAuth {
        /// <summary>
        /// 取登录URL
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <returns>登录URL</returns>
        public static string GetAuthUrl(OAuthEnum authEnum) {
            IOAuth auth = null;
            switch (authEnum) {
                case OAuthEnum.msn: auth = new MSNOAuth(); break;
                case OAuthEnum.sina: auth = new SinaOAuth(); break;
                case OAuthEnum.qq: auth = new QQOAuth(); break;
                case OAuthEnum.netease: auth = new NeteaseOAuth(); break;
                case OAuthEnum.sohu: auth = new SohuOAuth(); break;
                case OAuthEnum.kaixin: auth = new KaiXinOAuth(); break;
                case OAuthEnum.renren: auth = new RenRenOAuth(); break;
            }
            return auth.IsNull() ? string.Empty : auth.GetAuthUrl();
        }
        /// <summary>
        /// 取登录URL
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <returns>登录URL</returns>
        public static string GetAuthUrl(string authEnum) {
            return GetAuthUrl(authEnum.ToEnum<OAuthEnum>());
        }
        /// <summary>
        /// 取登录账号信息
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <returns>取登录账号信息</returns>
        public static UserInfo GetUserInfo(OAuthEnum authEnum) {
            IOAuth auth = null;
            switch (authEnum) {
                case OAuthEnum.msn: auth = new MSNOAuth(); break;
                case OAuthEnum.sina: auth = new SinaOAuth(); break;
                case OAuthEnum.qq: auth = new QQOAuth(); break;
                case OAuthEnum.netease: auth = new NeteaseOAuth(); break;
                case OAuthEnum.sohu: auth = new SohuOAuth(); break;
                case OAuthEnum.kaixin: auth = new KaiXinOAuth(); break;
                case OAuthEnum.renren: auth = new RenRenOAuth(); break;
            }
            return auth.IsNull() ? null : auth.GetUserInfo();
        }
        /// <summary>
        /// 取登录账号信息
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <returns>取登录账号信息</returns>
        public static UserInfo GetUserInfo(string authEnum) {
            return GetUserInfo(authEnum.ToEnum<OAuthEnum>());
        }
        /// <summary>
        /// 取登录账号好友信息
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <param name="accessToken">Access Token</param>
        /// <param name="accessSecret">Access Secret</param>
        /// <returns>取登录账号好友信息</returns>
        public static IList<UserInfo> GetFriendsInfo(OAuthEnum authEnum, string accessToken, string accessSecret) {
            IOAuth auth = null;
            switch (authEnum) {
                case OAuthEnum.msn: auth = new MSNOAuth(); break;
                case OAuthEnum.sina: auth = new SinaOAuth(); break;
                case OAuthEnum.qq: auth = new QQOAuth(); break;
                case OAuthEnum.netease: auth = new NeteaseOAuth(); break;
                case OAuthEnum.sohu: auth = new SohuOAuth(); break;
                case OAuthEnum.kaixin: auth = new KaiXinOAuth(); break;
                case OAuthEnum.renren: auth = new RenRenOAuth(); break;
            }
            return auth.IsNull() ? null : auth.GetFriendsInfo(accessToken, accessSecret);
        }
        /// <summary>
        /// 取登录账号好友信息
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <param name="accessToken">Access Token</param>
        /// <param name="accessSecret">Access Secret</param>
        /// <returns>取登录账号好友信息</returns>
        public static IList<UserInfo> GetFriendsInfo(string authEnum, string accessToken, string accessSecret) {
            return GetFriendsInfo(authEnum.ToEnum<OAuthEnum>(), accessToken, accessSecret);
        }
        /// <summary>
        /// 同步消息
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <param name="accessToken">Access Token</param>
        /// <param name="accessSecret">Access Secret</param>
        /// <param name="text">消息</param>
        public static void SendText(OAuthEnum authEnum, string accessToken, string accessSecret, string text) { 
            IOAuth auth = null;
            switch (authEnum) {
                case OAuthEnum.msn: auth = new MSNOAuth(); break;
                case OAuthEnum.sina: auth = new SinaOAuth(); break;
                case OAuthEnum.qq: auth = new QQOAuth(); break;
                case OAuthEnum.netease: auth = new NeteaseOAuth(); break;
                case OAuthEnum.sohu: auth = new SohuOAuth(); break;
                case OAuthEnum.kaixin: auth = new KaiXinOAuth(); break;
                case OAuthEnum.renren: auth = new RenRenOAuth(); break;
            }
            if (!auth.IsNull()) auth.SendText(accessToken, accessSecret, text);
        }
        /// <summary>
        /// 同步消息
        /// </summary>
        /// <param name="authEnum">授权类型</param>
        /// <param name="accessToken">Access Token</param>
        /// <param name="accessSecret">Access Secret</param>
        /// <param name="text">消息</param>
        public static void SendText(string authEnum, string accessToken, string accessSecret, string text) { 
            SendText(authEnum.ToEnum<OAuthEnum>(), accessToken, accessSecret, text);
        }
    }
}
