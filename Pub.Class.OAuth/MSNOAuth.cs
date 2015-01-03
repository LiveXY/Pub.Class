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
    /// MSN 授权登录
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class MSNOAuth : IOAuth {
        /// <summary>
        /// authorize
        /// </summary>
        public static readonly string authorize = "https://oauth.live.com/authorize";
        /// <summary>
        /// access_token
        /// </summary>
        public static readonly string access_token = "https://oauth.live.com/token";
        /// <summary>
        /// user_info
        /// </summary>
        public static readonly string user_info = "https://apis.live.net/v5.0/me";
        /// <summary>
        /// friends_list
        /// </summary>
        public static readonly string friends_list = "https://apis.live.net/v5.0/me/contacts";
        /// <summary>
        /// add
        /// </summary>
        public static readonly string add = "https://apis.live.net/v5.0/me/share";
        /// <summary>
        /// msn app 配置信息
        /// </summary>
        public static readonly ConfigInfo config = OAuthConfig.GetConfigInfo(OAuthEnum.msn);
        /// <summary>
        /// 取授权登录URL
        /// </summary>
        /// <returns>登录URL</returns>
        public string GetAuthUrl() {
            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("client_id", config.AppKey));
            param.Add(new UrlParameter("scope", "wl.signin%20wl.basic%20wl.share"));//%20wl.offline_access
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

            data = Net2.GetRemoteHtmlCode4(user_info + "?access_token=" + user.Token, Encoding.UTF8);
            user.UserID = data.GetMatchingValues("\"id\": \"(.+?)\"", "\"id\": \"", "\"").FirstOrDefault() ?? "";
            user.Name = data.GetMatchingValues("\"name\": \"(.+?)\"", "\"name\": \"", "\"").FirstOrDefault() ?? "";
            user.Sex = (data.GetMatchingValues("\"gender\": \"(.+?)\"", "\"gender\": \"", "\"").FirstOrDefault() ?? "male") == "male" ? 1 : 0;

            string url = data.GetMatchingValues("\"link\": \"(.+?)\"", "\"link\": \"", "\"").FirstOrDefault() ?? "";
            if (!url.Trim().IsNullEmpty()) {
                data = Net2.GetRemoteHtmlCode4(url, Encoding.UTF8).ReplaceRN();
                url = data.GetMatchingValues("<img id=\"snsupermeic1_usertile\" errsrc=\"(.+?)\" onload=\"ic_onTL\\(this\\)\"", "<img id=\"snsupermeic1_usertile\" errsrc=\"", "\" onload=\"ic_onTL(this)\"").FirstOrDefault() ?? "";
                int index = url.IndexOf("src=\"");
                if (index != -1) user.Header = url.Substring(index + 5);
            }
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
            string data = Net2.GetRemoteHtmlCode4(friends_list + "?access_token=" + accessToken, Encoding.UTF8).ReplaceRN().Trim();
            if (data.IsNullEmpty()) return list;
            data = data.Substring(1, data.Length - 2);
            IList<string> userlist = data.GetMatchingValues("{(.+?)}", "{", "}");

            foreach (string info in userlist) {
                UserInfo user = new UserInfo();
                user.UserID = info.GetMatchingValues("\"user_id\": \"(.+?)\"", "\"user_id\": \"", "\"").FirstOrDefault() ?? "";
                user.Name = info.GetMatchingValues("\"name\": \"(.+?)\"", "\"name\": \"", "\"").FirstOrDefault() ?? "";
                user.Sex = (info.GetMatchingValues("\"gender\": \"(.+?)\"", "\"gender\": \"", "\"").FirstOrDefault() ?? "male") == "male" ? 1 : 0;
                bool is_friend = (info.GetMatchingValues("\"is_friend\": (.+?),", "\"is_friend\": ", ",").FirstOrDefault() ?? "false") == "true" ? true : false;
                user.Email = info.GetMatchingValues("\"email_hashes\": \\[             \"(.+?)\"", "\"email_hashes\": [             \"", "\"").FirstOrDefault() ?? "";
                if (user.Name.IsEmail()) {
                    user.Email = user.Name;
                    user.Name = user.Name.Split('@')[0];
                }
                if (is_friend) list.Add(user);
            }
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
            param.Add(new UrlParameter("method", "post"));
            param.Add(new UrlParameter("message", text.SubString(270, "").UrlUpperEncode()));
            param.Sort(new UrlParameterCompre());

            Net2.GetRemoteHtmlCode4(add + "?" + OAuthCommon.GetUrlParameter(param), Encoding.UTF8);
        }
    }
}
