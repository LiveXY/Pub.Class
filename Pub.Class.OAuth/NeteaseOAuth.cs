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
    /// Netease 授权登录
    /// 
    /// 修改纪录
    ///     2011.12.02 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class NeteaseOAuth : IOAuth {
        /// <summary>
        /// request_token
        /// </summary>
        public static readonly string request_token = "http://api.t.163.com/oauth/request_token";
        /// <summary>
        /// authorize
        /// </summary>
        public static readonly string authorize = "http://api.t.163.com/oauth/authenticate";
        /// <summary>
        /// access_token
        /// </summary>
        public static readonly string access_token = "http://api.t.163.com/oauth/access_token";
        /// <summary>
        /// user_info
        /// </summary>
        public static readonly string user_info = "http://api.t.163.com/account/verify_credentials.json";
        /// <summary>
        /// friends_list
        /// </summary>
        public static readonly string friends_list = "http://api.t.163.com/statuses/friends.json";
        /// <summary>
        /// add
        /// </summary>
        public static readonly string add = "http://api.t.163.com/statuses/update.json";
        /// <summary>
        /// qq app 配置信息
        /// </summary>
        public static readonly ConfigInfo config = OAuthConfig.GetConfigInfo(OAuthEnum.netease);
        /// <summary>
        /// 取授权登录URL
        /// </summary>
        /// <returns>登录URL</returns>
        public string GetAuthUrl() {
            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("oauth_callback", config.RedirectUrl.UrlEncode2()));
            param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
            param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
            param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
            param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
            param.Add(new UrlParameter("oauth_version", "1.0"));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign = new StringBuilder().Append("GET&")
                .Append(request_token.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, "", sbSign.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());

            string data = HttpHelper.SendGet(new StringBuilder().Append(request_token).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString()) + "&";
            string token = data.GetMatchingValues("oauth_token=(.+?)&", "oauth_token=", "&").FirstOrDefault() ?? "";
            string tokenSecret = data.GetMatchingValues("oauth_token_secret=(.+?)&", "oauth_token_secret=", "&").FirstOrDefault() ?? "";

            Session2.Set("oauth_token", token);
            Session2.Set("oauth_token_secret", tokenSecret);
            return authorize + "?oauth_token=" + token + "&oauth_callback=" + config.RedirectUrl;
        }
        /// <summary>
        /// 取登录账号信息
        /// </summary>
        /// <returns>取登录账号信息</returns>
        public UserInfo GetUserInfo() {
            UserInfo user = new UserInfo();

            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
            param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
            param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
            param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
            param.Add(new UrlParameter("oauth_token", Request2.GetQ("oauth_token")));
            param.Add(new UrlParameter("oauth_verifier", Request2.GetQ("oauth_verifier")));
            param.Add(new UrlParameter("oauth_version", "1.0"));
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
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign2 = new StringBuilder().Append("GET&")
                .Append(user_info.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, user.Secret, sbSign2.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());

            data = HttpHelper.SendGet(new StringBuilder().Append(user_info).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());

            user.UserID = data.GetMatchingValues("\"id\":\"(.+?)\"", "\"id\":\"", "\"").FirstOrDefault() ?? "";
            user.Email = data.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
            user.Name = data.GetMatchingValues("\"name\":\"(.+?)\"", "\"name\":\"", "\"").FirstOrDefault() ?? "";
            user.Sex = (data.GetMatchingValues("\"gender\":\"(.+?)\"", "\"gender\":\"", "\"").FirstOrDefault() ?? "") == "1" ? 1 : 0;
            user.Header = data.GetMatchingValues("\"profile_image_url\":\"(.+?)\"", "\"profile_image_url\":\"", "\"").FirstOrDefault() ?? "";
            user.Address = data.GetMatchingValues("\"location\":\"(.+?)\"", "\"location\":\"", "\"").FirstOrDefault() ?? "";

            //{"status":null,"following":false,"blocking":false,"followed_by":false,"name":"cexo255","location":"上海市,徐汇区",
            //"id":"959281886828269520","description":"","email":"cexo255@163.com","gender":"0","verified":false,"url":"","screen_name":"cexo255",
            //"profile_image_url":"http://oimagea3.ydstatic.com/image?w=48&h=48&url=http%3A%2F%2Fimg1.cache.netease.com%2Ft%2Fimg%2Fdefault80.png",
            //"created_at":"Wed Apr 21 13:19:53 +0800 2010","darenRec":null,"favourites_count":"0","followers_count":"0","friends_count":"0",
            //"geo_enable":false,"icorp":"0","realName":null,"statuses_count":"0","sysTag":null,"userTag":null,"in_groups":[]}
            //Msg.Write(GetFriendsInfo(user.Token, user.Secret).ToJson());
            //SendText(user.Token, user.Secret, "测试数据2");
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
                param.Add(new UrlParameter("cursor", count * (page-1)));
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
                IList<string> userlist = data.GetMatchingValues("{\"status\":(.+?)]}", "{", "]}");

                foreach (string info in userlist) {
                    UserInfo user = new UserInfo();
                    user.UserID = info.GetMatchingValues("\"id\":\"(.+?)\"", "\"id\":\"", "\"").FirstOrDefault() ?? "";
                    user.Email = info.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
                    user.Name = info.GetMatchingValues("\"name\":\"(.+?)\"", "\"name\":\"", "\"").FirstOrDefault() ?? "";
                    user.Sex = (info.GetMatchingValues("\"gender\":\"(.+?)\"", "\"gender\":\"", "\"").FirstOrDefault() ?? "") == "1" ? 1 : 0;
                    user.Address = info.GetMatchingValues("\"location\":\"(.+?)\"", "\"location\":\"", "\"").FirstOrDefault() ?? "";
                    user.Header = info.GetMatchingValues("\"profile_image_url\":\"(.+?)\"", "\"profile_image_url\":\"", "\"").FirstOrDefault() ?? "";
                    list.Add(user);
                }

                if (userlist.IsNull() || userlist.Count == 0) isTrue = false;
                page++;
            };

            //"users":[
            //{"status":{"id":"-3707542760529240330","source":"网易微博","text":"","in_reply_to_screen_name":null,"in_reply_to_status_id":null,"in_reply_to_user_id":null,"in_reply_to_user_name":null,"truncated":false,"videoInfos":[{"title":"美国宪法和总统","flashUrl":"","coverUrl":"","shortUrl":"http://163.fm/EIPA0CD"}],"musicInfos":null},"following":true,"blocking":false,"followed_by":false,"name":"标尺的鲁克","location":",","id":"261087702224684492","description":"","email":"lukepost@yeah.net","gender":"1","verified":false,"url":"http://dongxi.net/column/bc","screen_name":"lukepost","profile_image_url":"http://oimagec2.ydstatic.com/image?w=48&h=48&url=http%3A%2F%2F126.fm%2F2wLbCY","created_at":"Fri Sep 03 21:31:19 +0800 2010","darenRec":"标尺网专栏作者，美国问题专家","favourites_count":"31","followers_count":"426486","friends_count":"337","geo_enable":false,"icorp":"0","realName":null,"statuses_count":"2723","sysTag":["财经专栏"],"userTag":null,"in_groups":[]},
            //{"status":{"id":"5655595159751579198","source":"网易微博","text":"","created_at":"Fri Dec 09 11:18:44 +0800 2011","in_reply_to_screen_name":null,"in_reply_to_status_id":null,"in_reply_to_user_id":null,"in_reply_to_user_name":null,"truncated":false,"videoInfos":null,"musicInfos":null},"following":true,"blocking":false,"followed_by":false,"name":"连岳","location":",","id":"2264175753142745453","description":"","email":"rosu@163.com","gender":"1","verified":false,"url":"","screen_name":"lianyue","profile_image_url":"http://oimagea6.ydstatic.com/image?w=48&h=48&url=http%3A%2F%2F126.fm%2F3k99W4","created_at":"Tue Mar 30 12:13:35 +0800 2010","darenRec":"情感专家，著名专栏作家","favourites_count":"1","followers_count":"1006455","friends_count":"81","geo_enable":false,"icorp":"0","realName":null,"statuses_count":"2527","sysTag":["专栏作家","思想","情感"],"userTag":null,"in_groups":[]},
            //{"status":{"id":"7703093847719120921","source":"网易微博","text":"","created_at":"Fri Dec 09 11:27:01 +0800 2011","in_reply_to_screen_name":"lianyue","in_reply_to_status_id":"-423057201922397552","in_reply_to_user_id":"2264175753142745453","in_reply_to_user_name":"连岳","truncated":false,"videoInfos":null,"musicInfos":null},"following":true,"blocking":false,"followed_by":false,"name":"郑旭光","location":"北京市,昌平区","id":"-5757539258355407223","description":"","email":"zhengxuguang@126.com","gender":"1","verified":false,"url":"","screen_name":"zhengxuguang","profile_image_url":"http://oimagea3.ydstatic.com/image?w=48&h=48&url=http%3A%2F%2F126.fm%2F3TZTVb","created_at":"Tue Sep 28 15:41:53 +0800 2010","darenRec":"经济学达人","favourites_count":"1","followers_count":"1197810","friends_count":"993","geo_enable":false,"icorp":"0","realName":"心证自由","statuses_count":"15898","sysTag":["财经专栏"],"userTag":["政治经济学","哈耶克","米塞斯","司马迁","经济学","亚当斯密","门格尔","奥地利学派","杨朱"],"in_groups":[]}]
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
            param.Add(new UrlParameter("status", text.SubString(270, "").UrlUpperEncode()));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign = new StringBuilder().Append("POST&")
                .Append(add.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, accessSecret, sbSign.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());
            string data = HttpHelper.SendPost(add, OAuthCommon.GetUrlParameter(param), "application/x-www-form-urlencoded");
            Msg.WriteEnd(data);
        }
    }
}
