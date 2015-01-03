//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Pub.Class;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Data.Common;

namespace Pub.Class {
    /// <summary>
    /// RenRen 授权登录
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class RenRenOAuth : IOAuth {
        /// <summary>
        /// authorize
        /// </summary>
        public static readonly string authorize = "https://graph.renren.com/oauth/authorize";
        /// <summary>
        /// access_token
        /// </summary>
        public static readonly string access_token = "https://graph.renren.com/oauth/token";
        /// <summary>
        /// session_key
        /// </summary>
        public static readonly string session_key = "https://graph.renren.com/renren_api/session_key";
        /// <summary>
        /// user_info
        /// </summary>
        public static readonly string server = "http://api.renren.com/restserver.do";
        /// <summary>
        /// RenRen app 配置信息
        /// </summary>
        public static readonly ConfigInfo config = OAuthConfig.GetConfigInfo(OAuthEnum.renren);
        /// <summary>
        /// 取授权登录URL
        /// </summary>
        /// <returns>登录URL</returns>
        public string GetAuthUrl() {
            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("client_id", config.AppKey));
            param.Add(new UrlParameter("scope", "publish_feed "));//%20wl.offline_access
            param.Add(new UrlParameter("response_type", "code"));
            param.Add(new UrlParameter("redirect_uri", config.RedirectUrl));
            param.Sort(new UrlParameterCompre());
            return authorize + "?" + OAuthCommon.GetUrlParameter(param);
        }
        /// <summary>
        /// 取登录账号信息
        /// </summary>
        /// <returns>取登录账号信息</returns>
        public UserInfo GetUserInfo() {
            string code = Request2.GetQ("code").Trim();
            UserInfo user = new UserInfo();

            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("client_id", config.AppKey));
            param.Add(new UrlParameter("redirect_uri", config.RedirectUrl));
            param.Add(new UrlParameter("client_secret", config.AppSecret));
            param.Add(new UrlParameter("code", code));
            param.Add(new UrlParameter("grant_type", "authorization_code"));
            param.Sort(new UrlParameterCompre());

            string data = HttpHelper.SendPost(access_token, OAuthCommon.GetUrlParameter(param), "application/x-www-form-urlencoded");
            user.Token = data.GetMatchingValues("\"access_token\":\"(.+?)\"", "\"access_token\":\"", "\"").FirstOrDefault() ?? "";
            user.Name = data.GetMatchingValues("\"name\":\"(.+?)\"", "\"name\":\"", "\"").FirstOrDefault() ?? "";

            data = HttpHelper.SendGet(session_key + "?oauth_token=" + user.Token);
            string _session_key = data.GetMatchingValues("\"session_key\":\"(.+?)\"", "\"session_key\":\"", "\"").FirstOrDefault() ?? "";
            user.Secret = _session_key;

            param.Clear();
            param.Add(new UrlParameter("method", "users.getInfo"));
            param.Add(new UrlParameter("api_key", config.AppKey));
            param.Add(new UrlParameter("v", "1.0"));
            param.Add(new UrlParameter("format", "JSON"));
            param.Add(new UrlParameter("session_key", _session_key));
            param.Add(new UrlParameter("access_token", user.Token));
            param.Sort(new UrlParameterCompre());
            string sign = OAuthCommon.MD5(OAuthCommon.GetUrlParameter2(param) + config.AppSecret);
            param.Add(new UrlParameter("sig", sign));

            data = HttpHelper.SendPost(server, OAuthCommon.GetUrlParameter(param), "application/x-www-form-urlencoded");
            user.UserID = data.GetMatchingValues("\"uid\":(.+?),", "\"uid\":", ",").FirstOrDefault() ?? "";
            user.Sex = (data.GetMatchingValues("\"sex\":(.+?),", "\"sex\":", ",").FirstOrDefault() ?? "") == "1" ? 1 : 0;
            user.Header = data.GetMatchingValues("\"headurl\":\"(.+?)\"", "\"headurl\":\"", "\"").FirstOrDefault() ?? "";
            //Msg.WriteEnd(GetFriendsInfo(user.Token, user.Secret).ToJson());
            //SendText(user.Token, user.Secret, "测试数据");
            //data = Net2.GetRemoteHtmlCode4(user_info + "?access_token=" + user.Token, Encoding.UTF8);

            //{"expires_in":2592943,"refresh_token":"171693|0.8FABtNFcYxY4k5EitG8rC4cHF5OimqIW.248357590",
            //"user":{"id":248357590,"name":"熊华春","avatar":[{"type":"avatar","url":"http:\/\/hd52.xiaonei.com\/photos\/hd52\/20080711\/23\/56\/head_6642f107.jpg"},
            //{"type":"tiny","url":"http:\/\/hd52.xiaonei.com\/photos\/hd52\/20080711\/23\/56\/tiny_6642f107.jpg"},
            //{"type":"main","url":"http:\/\/hd52.xiaonei.com\/photos\/hd52\/20080711\/23\/56\/main_6642f107.jpg"},
            //{"type":"large","url":"http:\/\/hd52.xiaonei.com\/photos\/hd52\/20080711\/23\/56\/large_6478m107.jpg"}]},
            //"access_token":"171693|6.5784313da61394f8b29af060af50e699.2592000.1326250800-248357590"}
            //{"renren_token":{"session_secret":"b7e69adb797a7d127e092bb4af60e1ab","expires_in":2592359,"session_key":"6.5784313da61394f8b29af060af50e699.2592000.1326250800-248357590"},
            //"oauth_token":"171693|6.5784313da61394f8b29af060af50e699.2592000.1326250800-248357590","user":{"id":248357590}}
            //[{"uid":248357590,"tinyurl":"http://hd52.xiaonei.com/photos/hd52/20080711/23/56/tiny_6642f107.jpg","vip":1,"sex":1,
            //"name":"熊华春","star":0,"headurl":"http://hd52.xiaonei.com/photos/hd52/20080711/23/56/head_6642f107.jpg","zidou":0}]
            return user;
        }
        /// <summary>
        /// 取登录账号好友信息
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <param name="accessSecret">Access Secret</param>
        /// <returns>取登录账号好友信息</returns>
        public IList<UserInfo> GetFriendsInfo(string accessToken, string accessSecret) {
            IList<UserInfo> list = new List<UserInfo>();

            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("method", "friends.getFriends"));
            param.Add(new UrlParameter("v", "1.0"));
            param.Add(new UrlParameter("format", "JSON"));
            param.Add(new UrlParameter("api_key", config.AppKey));
            param.Add(new UrlParameter("session_key", accessSecret));
            param.Add(new UrlParameter("access_token", accessToken));
            param.Add(new UrlParameter("count", "500"));
            param.Add(new UrlParameter("page", "1"));
            param.Sort(new UrlParameterCompre());
            string sign = OAuthCommon.MD5(OAuthCommon.GetUrlParameter2(param) + config.AppSecret);
            param.Add(new UrlParameter("sig", sign));

            string data = HttpHelper.SendPost(server, OAuthCommon.GetUrlParameter(param), "application/x-www-form-urlencoded");
            if (data.IsNullEmpty()) return list;
            IList<string> userlist = data.GetMatchingValues("{(.+?)}", "{", "}");

            foreach (string info in userlist) {
                UserInfo user = new UserInfo();
                user.UserID = info.GetMatchingValues("\"id\":(.+?),", "\"id\":", ",").FirstOrDefault() ?? "";
                user.Name = info.GetMatchingValues("\"name\":\"(.+?)\"", "\"name\":\"", "\"").FirstOrDefault() ?? "";
                user.Header = info.GetMatchingValues("\"headurl\":\"(.+?)\"", "\"headurl\":\"", "\"").FirstOrDefault() ?? "";
                list.Add(user);
            }
            //[{"id":231102615,"tinyurl":"http://hd11.xiaonei.com/photos/hd11/20070913/21/34/tiny_5567j172.jpg",
            //"name":"张乐乐","headurl":"http://hd11.xiaonei.com/photos/hd11/20070913/21/34/head_5567j172.jpg"}]
            return list;
        }
        /// <summary>
        /// 同步消息
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <param name="accessSecret">Access Secret</param>
        /// <param name="text">消息</param>
        public void SendText(string accessToken, string accessSecret, string text) {
            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("access_token", accessToken));
            param.Add(new UrlParameter("api_key", config.AppKey));
            param.Add(new UrlParameter("format", "JSON"));
            param.Add(new UrlParameter("method", "feed.publishTemplatizedAction"));
            param.Add(new UrlParameter("session_key", accessSecret));
            param.Add(new UrlParameter("template_id", "1"));
            param.Add(new UrlParameter("title_data", "{\"title\":\"t\"}"));
            param.Add(new UrlParameter("v", "1.0"));
            param.Add(new UrlParameter("body_data", "{\"content\":" + text.ReplaceRN().SubString(270, "").ToJson() + "}"));
            param.Sort(new UrlParameterCompre());
            string sign = OAuthCommon.MD5(OAuthCommon.GetUrlParameter2(param) + config.AppSecret);
            param.Add(new UrlParameter("sig", sign));
            param.Sort(new UrlParameterCompre());
            HttpHelper.SendRequest(server, OAuthCommon.GetUrlParameter(param), "POST", "application/x-www-form-urlencoded", "", null, null, null, null, Encoding.UTF8, Encoding.UTF8);
        }
    }
}
