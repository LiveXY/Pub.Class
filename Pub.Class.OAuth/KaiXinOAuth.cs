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
using System.Security.Cryptography;

namespace Pub.Class {
    /// <summary>
    /// KaiXin 授权登录
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class KaiXinOAuth : IOAuth {
        /// <summary>
        /// request_token
        /// </summary>
        public static readonly string request_token = "http://api.kaixin001.com/oauth/request_token";
        /// <summary>
        /// authorize
        /// </summary>
        public static readonly string authorize = "http://api.kaixin001.com/oauth/authorize";
        /// <summary>
        /// access_token
        /// </summary>
        public static readonly string access_token = "http://api.kaixin001.com/oauth/access_token";
        /// <summary>
        /// user_info
        /// </summary>
        public static readonly string user_info = "http://api.kaixin001.com/users/me.json";
        /// <summary>
        /// friends_list
        /// </summary>
        public static readonly string friends_list = "http://api.kaixin001.com/friends/me.json";
        /// <summary>
        /// add
        /// </summary>
        public static readonly string add = "http://api.kaixin001.com/records/add.json";
        /// <summary>
        /// qq app 配置信息
        /// </summary>
        public static readonly ConfigInfo config = OAuthConfig.GetConfigInfo(OAuthEnum.kaixin);
        /// <summary>
        /// 取授权登录URL
        /// </summary>
        /// <returns>登录URL</returns>
        public string GetAuthUrl() {
            //http://wiki.open.kaixin001.com/index.php?id=OAuth%e6%96%87%e6%a1%a3
            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("oauth_callback", config.RedirectUrl.UrlEncode2()));
            param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
            param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
            param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
            param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
            param.Add(new UrlParameter("oauth_version", "1.0"));
            param.Add(new UrlParameter("scope", "create_records"));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign = new StringBuilder().Append("GET&")
                .Append(request_token.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, "", sbSign.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());

            string data = HttpHelper.SendGet(new StringBuilder().Append(request_token).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());

            string token = data.GetMatchingValues("oauth_token=(.+?)&", "oauth_token=", "&").FirstOrDefault() ?? "";
            string tokenSecret = data.GetMatchingValues("oauth_token_secret=(.+?)&", "oauth_token_secret=", "&").FirstOrDefault() ?? "";
            Session2.Set("oauth_token", token);
            Session2.Set("oauth_token_secret", tokenSecret);
            return authorize + "?oauth_token=" + token;
        }
        /// <summary>
        /// 取登录账号信息
        /// </summary>
        /// <returns>取登录账号信息</returns>
        public UserInfo GetUserInfo() {
            UserInfo user = new UserInfo();
            string openid = Request2.GetQ("openid");
            string openkey = Request2.GetQ("openkey");

            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
            param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
            param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
            param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
            param.Add(new UrlParameter("oauth_token", Request2.GetQ("oauth_token")));
            param.Add(new UrlParameter("oauth_verifier", Request2.GetQ("oauth_verifier")));
            param.Add(new UrlParameter("oauth_version", "1.0"));
            //param.Add(new UrlParameter("scope", "create_records"));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign = new StringBuilder().Append("GET&")
                .Append(access_token.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, Session2.Get("oauth_token_secret"), sbSign.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());
            string data = HttpHelper.SendGet(new StringBuilder().Append(access_token).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString()) + "&";

            user.Token = data.GetMatchingValues("oauth_token=(.+?)&", "oauth_token=", "&").FirstOrDefault() ?? "";
            user.Secret = data.GetMatchingValues("oauth_token_secret=(.+?)&", "oauth_token_secret=", "&").FirstOrDefault() ?? "";

            param.Clear();
            param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
            param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
            param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
            param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
            param.Add(new UrlParameter("oauth_token", user.Token));
            param.Add(new UrlParameter("oauth_version", "1.0"));
            //param.Add(new UrlParameter("scope", "create_records"));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign2 = new StringBuilder().Append("GET&")
                .Append(user_info.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, user.Secret, sbSign2.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());
            data = HttpHelper.SendGet(new StringBuilder().Append(user_info).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());
            
            user.UserID = data.GetMatchingValues("\"uid\":\"(.+?)\"", "\"uid\":\"", "\"").FirstOrDefault() ?? "";
            user.Email = data.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
            user.Name = (data.GetMatchingValues("\"name\":\"(.+?)\"", "\"name\":\"", "\"").FirstOrDefault() ?? "").Ascii2Native();
            user.Sex = (data.GetMatchingValues("\"gender\":\"(.+?)\"", "\"gender\":\"", "\"").FirstOrDefault() ?? "") == "0" ? 1 : 0;
            user.Header = data.GetMatchingValues("\"logo50\":\"(.+?)\"", "\"logo50\":\"", "\"").FirstOrDefault() ?? "";

            //{"uid":"45908241","name":"\u534e\u6625","gender":"0","logo50":"http:\/\/img.kaixin001.com.cn\/i\/50_0_0.gif"}
            //Msg.WriteEnd(GetFriendsInfo(user.Token, user.Secret).ToJson());
            //SendText(user.Token, user.Secret, "测试数据");
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
            bool isTrue = true; int count = 10; int page = 1;

            while (isTrue) {
                List<UrlParameter> param = new List<UrlParameter>();
                param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
                param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
                param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
                param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
                param.Add(new UrlParameter("oauth_token", accessToken));
                param.Add(new UrlParameter("oauth_version", "1.0"));
                param.Add(new UrlParameter("fields", ""));
                param.Add(new UrlParameter("num", count));
                param.Add(new UrlParameter("start", count * (page-1)));
                param.Sort(new UrlParameterCompre());

                StringBuilder sbSign = new StringBuilder().Append("GET&")
                    .Append(friends_list.UrlEncode2())
                    .Append("&")
                    .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

                param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, accessSecret, sbSign.ToString()).UrlEncode2()));
                param.Sort(new UrlParameterCompre());
                string data = "";
                try {
                    data = HttpHelper.SendGet(new StringBuilder().Append(friends_list).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());
                    data = data.Substring(1, data.Length - 2);
                } catch {}
                IList<string> userlist = data.GetMatchingValues("{(.+?)}", "{", "}");

                foreach (string info in userlist) {
                    UserInfo user = new UserInfo();
                    user.UserID = info.GetMatchingValues("\"uid\":\"(.+?)\"", "\"uid\":\"", "\"").FirstOrDefault() ?? "";
                    user.Email = info.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
                    user.Name = (info.GetMatchingValues("\"name\":\"(.+?)\"", "\"name\":\"", "\"").FirstOrDefault() ?? "").Ascii2Native();
                    user.Sex = (info.GetMatchingValues("\"gender\":\"(.+?)\"", "\"gender\":\"", "\"").FirstOrDefault() ?? "") == "0" ? 1 : 0;
                    user.Address = info.GetMatchingValues("\"location\":\"(.+?)\"", "\"location\":\"", "\"").FirstOrDefault() ?? "";
                    user.Header = info.GetMatchingValues("\"logo50\":\"(.+?)\"", "\"logo50\":\"", "\"").FirstOrDefault() ?? "";
                    list.Add(user);
                }

                if (userlist.IsNull() || userlist.Count == 0) isTrue = false;
                page++;
            };

            //{"users":[
            //{"uid":"33941855","name":"\u65b0\u534e\u793e\u7535\u89c6","gender":"0","logo50":"http:\/\/logo.kaixin001.com.cn\/logo\/94\/18\/50_33941855_3.jpg"}],
            //"prev":"-1","next":"-1","total":"1"}
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
            param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
            param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
            param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
            param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
            param.Add(new UrlParameter("oauth_token", accessToken));
            param.Add(new UrlParameter("oauth_version", "1.0"));
            param.Add(new UrlParameter("content", text.SubString(270, "").UrlUpperEncode()));
            //param.Add(new UrlParameter("access_token", accessToken));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign = new StringBuilder().Append("POST&")
                .Append(add.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, accessSecret, sbSign.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());
            HttpHelper.SendPost(add, OAuthCommon.GetUrlParameter(param), "application/x-www-form-urlencoded");
        }
    }
}
