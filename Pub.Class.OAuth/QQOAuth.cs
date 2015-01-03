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
    /// QQ 授权登录
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class QQOAuth : IOAuth {
        /// <summary>
        /// request_token
        /// </summary>
        public static readonly string request_token = "https://open.t.qq.com/cgi-bin/request_token";
        /// <summary>
        /// authorize
        /// </summary>
        public static readonly string authorize = "https://open.t.qq.com/cgi-bin/authorize";
        /// <summary>
        /// access_token
        /// </summary>
        public static readonly string access_token = "https://open.t.qq.com/cgi-bin/access_token";
        /// <summary>
        /// user_info
        /// </summary>
        public static readonly string user_info = "http://open.t.qq.com/api/user/info";
        /// <summary>
        /// friends_list
        /// </summary>
        public static readonly string friends_list = "http://open.t.qq.com/api/friends/idollist";
        /// <summary>
        /// add
        /// </summary>
        public static readonly string add = "http://open.t.qq.com/api/t/add";
        /// <summary>
        /// qq app 配置信息
        /// </summary>
        public static readonly ConfigInfo config = OAuthConfig.GetConfigInfo(OAuthEnum.qq);
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
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign = new StringBuilder().Append("GET&")
                .Append(access_token.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, Session2.Get("oauth_token_secret"), sbSign.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());
            string data = HttpHelper.SendGet(new StringBuilder().Append(access_token).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());

            user.Token = data.GetMatchingValues("oauth_token=(.+?)&", "oauth_token=", "&").FirstOrDefault() ?? "";
            user.Secret = data.GetMatchingValues("oauth_token_secret=(.+?)&", "oauth_token_secret=", "&").FirstOrDefault() ?? "";
            user.UserID = data.Substring(data.IndexOf("&name=") + 6);

            param.Clear();
            param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
            param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
            param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
            param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
            param.Add(new UrlParameter("oauth_token", user.Token));
            param.Add(new UrlParameter("oauth_version", "1.0"));
            param.Add(new UrlParameter("format", "json"));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign2 = new StringBuilder().Append("GET&")
                .Append(user_info.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, user.Secret, sbSign2.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());
            data = HttpHelper.SendGet(new StringBuilder().Append(user_info).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());
            
            user.Email = data.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
            user.Name = data.GetMatchingValues("\"nick\":\"(.+?)\"", "\"nick\":\"", "\"").FirstOrDefault() ?? "";
            user.Sex = (data.GetMatchingValues("\"sex\":(.+?),", "\"sex\":", ",").FirstOrDefault() ?? "") == "1" ? 1 : 0;
            user.Address = data.GetMatchingValues("\"location\":\"(.+?)\"", "\"location\":\"", "\"").FirstOrDefault() ?? "";

            //{"data":{"birth_day":31,"birth_month":3,"birth_year":1984,"city_code":"9","country_code":"1","edu":null,"email":"30133499@qq.com",
            //"fansnum":59,"head":"","idolnum":25,"introduction":"","isent":0,"isvip":0,"location":"未知","name":"cexo255","nick":"熊","openid":"",
            //"province_code":"31","sex":1,"tag":null,"tweetnum":40,"verifyinfo":""},"errcode":0,"msg":"ok","ret":0}
            //Msg.WriteEnd(GetFriendsInfo(user.Token, user.Secret).ToJson());
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
                param.Add(new UrlParameter("format", "json"));
                param.Add(new UrlParameter("reqnum", count));
                param.Add(new UrlParameter("startindex", count * (page-1)));
                param.Sort(new UrlParameterCompre());

                StringBuilder sbSign = new StringBuilder().Append("GET&")
                    .Append(friends_list.UrlEncode2())
                    .Append("&")
                    .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

                param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, accessSecret, sbSign.ToString()).UrlEncode2()));
                param.Sort(new UrlParameterCompre());
                string data = HttpHelper.SendGet(new StringBuilder().Append(friends_list).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());
                IList<string> userlist = data.GetMatchingValues("{\"city_code\"(.+?)}]}", "{\"city_code\"", "}]}");

                foreach (string info in userlist) {
                    UserInfo user = new UserInfo();
                    user.UserID = info.GetMatchingValues("\"name\":\"(.+?)\"", "\"name\":\"", "\"").FirstOrDefault() ?? "";
                    user.Email = info.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
                    user.Name = info.GetMatchingValues("\"nick\":\"(.+?)\"", "\"nick\":\"", "\"").FirstOrDefault() ?? "";
                    user.Sex = (info.GetMatchingValues("\"sex\":(.+?),", "\"sex\":", ",").FirstOrDefault() ?? "") == "1" ? 1 : 0;
                    user.Address = info.GetMatchingValues("\"location\":\"(.+?)\"", "\"location\":\"", "\"").FirstOrDefault() ?? "";
                    user.Header = info.GetMatchingValues("\"head\":\"(.+?)\"", "\"head\":\"", "\"").FirstOrDefault() ?? "";
                    list.Add(user);
                }

                if (userlist.IsNull() || userlist.Count == 0) isTrue = false;
                page++;
            };

            //{"data":{"hasnext":0,"info":[
            //{"city_code":"1","country_code":"1","fansnum":1517049,"head":"http://app.qlogo.cn/mbloghead/a234df2a8474d1e853be","idolnum":25,"isidol":true,"isvip":1,"location":"未知","name":"tncmayun","nick":"TNC马云","openid":"","province_code":"33","tag":null,"tweet":[{"from":"腾讯微博","id":"18650095129418","text":"看着家人的眼泪，听见同事们疲惫委屈的声音，心悴了，真累了，真想放弃。心里无数次责问自己：我们为了什么？凭啥去承担如此的责任？也许商人赚了钱就该过舒适生活，或象别人一样移民，社会好坏和我们有啥关系？昨晚上网听见那批人高奏纳粹军歌，呼喊＂消灭一切，摧毁一切＂伤害着无辜。亲，淘宝人！！","timestamp":1318479818}]},
            //{"city_code":"","country_code":"1","fansnum":100,"head":"http://app.qlogo.cn/mbloghead/51d6d26e67012a6e069c","idolnum":136,"isidol":true,"isvip":0,"location":"未知","name":"pandorcai","nick":"蔡日","openid":"","province_code":"31","tag":[{"id":"77846971042806542","name":"旅游"},{"id":"595526330566555944","name":"逛论坛"},{"id":"3116172981967911833","name":"爱睡觉"},{"id":"3428083006598920604","name":"上网"},{"id":"4762518206506141882","name":"游戏"},{"id":"7632343735443733612","name":"听音乐"},{"id":"8143061372111998179","name":"看书"},{"id":"9393533470027694381","name":"看电影"},{"id":"12117796803083473608","name":"美食"},{"id":"16511128160049158514","name":"程序员"}],"tweet":[{"from":"QQ签名","id":"71615048757565","text":"两个月六张罚单的人伤不起呀","timestamp":1322884522}]},
            //{"city_code":"","country_code":"1","fansnum":69,"head":"http://app.qlogo.cn/mbloghead/a024e15a9310b3f5179c","idolnum":35,"isidol":true,"isvip":0,"location":"未知","name":"willyjl","nick":"杨洁丽","openid":"","province_code":"31","tag":null,"tweet":[{"from":"腾讯微博","id":"73020113642003","text":"凭我的姿色，竟然只需交这么点税收！‘地球形象税收局’的审美观真是令我不敢恭维。快来看看你要纳多少#形象税#：http://url.cn/3YFsGb ","timestamp":1310006202}]},
            //{"city_code":"","country_code":"1","fansnum":12,"head":"http://app.qlogo.cn/mbloghead/7d5c58b2cdd733963f32","idolnum":37,"isidol":true,"isvip":0,"location":"未知","name":"nina20101012","nick":"珠珠","openid":"","province_code":"31","tag":null,"tweet":[{"from":"腾讯微博","id":"78580001467265","text":"今天看新闻了，那个叫悦悦的小女孩儿太可怜了，几分钟被4个轮子压过去，本来可以避免那么严重的后果的，为什么路人都那么漠视，那是生命啊！，太气人了，鄙视你们，会有报应的","timestamp":1318943847}]},
            //{"city_code":"1","country_code":"1","fansnum":168,"head":"http://app.qlogo.cn/mbloghead/57875e17a4bce1fbb702","idolnum":284,"isidol":true,"isvip":0,"location":"未知","name":"killmyleon","nick":"乐乐","openid":"","province_code":"41","tag":[{"id":"3296752990863134558","name":"恋爱ING"},{"id":"3428083006598920604","name":"上网"},{"id":"8486326060299489387","name":"邪恶"},{"id":"9393533470027694381","name":"看电影"},{"id":"12093106934468067613","name":"ubuntu"},{"id":"13717765964038710262","name":"努力ING"},{"id":"14438978826104116590","name":"debian"},{"id":"14847358170114098914","name":"linux"},{"id":"16469568389185112236","name":"各种宅"}],"tweet":[{"from":"QQ签名","id":"52611047553060","text":"坚持就是胜利！","timestamp":1322091149}]},
            //{"city_code":"","country_code":"","fansnum":229,"head":"http://app.qlogo.cn/mbloghead/ad7b87d35771ebd0bcd0","idolnum":125,"isidol":true,"isvip":0,"location":"","name":"iamyanghua","nick":"杨华","openid":"","province_code":"","tag":[{"id":"77846971042806542","name":"旅游"},{"id":"4762518206506141882","name":"游戏"}],"tweet":[{"from":"","id":"0","text":"","timestamp":0}]},
            //{"city_code":"","country_code":"","fansnum":1253,"head":"http://app.qlogo.cn/mbloghead/21663a093b620c1150d8","idolnum":6,"isidol":true,"isvip":0,"location":"未知","name":"MEIZU_SH","nick":"魅族_上海","openid":"","province_code":"","tag":[{"id":"3314035123865995316","name":"魅族徐家汇旗舰店"},{"id":"4766081530055180763","name":"魅族上海"},{"id":"5840235084298680107","name":"魅族手机"},{"id":"9912118391828821150","name":"MEIZU"},{"id":"13082348043929536259","name":"魅族"},{"id":"13609997272425797546","name":"魅族专卖店"},{"id":"14986941257412550705","name":"M9"},{"id":"15053646515186054983","name":"魅族旗舰店"},{"id":"15236338144929393964","name":"上海心意"}],"tweet":[{"from":"腾讯微博","id":"19218047161270","text":"#MX最新消息#魅族MX试玩视频 http://url.cn/3MYnA4   大家一睹为快 http://url.cn/3yjgiY ","timestamp":1323313998}]},
            //{"city_code":"","country_code":"1","fansnum":3160,"head":"http://app.qlogo.cn/mbloghead/09694c818d98c2b440a6","idolnum":10,"isidol":true,"isvip":0,"location":"未知","name":"lovemeizu","nick":"魅族迷","openid":"","province_code":"31","tag":[{"id":"399381935516400475","name":"m9"},{"id":"1433216300070607690","name":"80后"},{"id":"7120965338382079114","name":"舌头可以舔到鼻子"},{"id":"12149833612181773251","name":"android"},{"id":"13082348043929536259","name":"魅族"},{"id":"14193650147949730220","name":"数码"},{"id":"14743408212463651736","name":"m8"},{"id":"17382413701540968475","name":"没生过病"}],"tweet":[{"from":"腾讯微博","id":"38016132218810","text":"魅族M9手机上市时可能会有每月赠送88元的186套餐供选择。合约机价格与裸机价一样，同为8GB版2499元、16GB版2699元。  　　这是 J.Wong 首次论及M9与联通合约套餐方案的信息，同时披露16GB版M9的价格为2699元。 但随后称目前套餐方案还未定，并提到，“如果有合作套餐的话届时可以直接在专卖店办理。”","timestamp":1290763523}]},
            //{"city_code":"","country_code":"1","fansnum":9362,"head":"http://app.qlogo.cn/mbloghead/32f1c38a5d2ef62b918e","idolnum":0,"isidol":true,"isvip":0,"location":"未知","name":"meizutech","nick":"魅族资讯","openid":"","province_code":"31","tag":[{"id":"77846971042806542","name":"旅游"},{"id":"595526330566555944","name":"逛论坛"},{"id":"797311093297244621","name":"爬山"},{"id":"3428083006598920604","name":"上网"},{"id":"7632343735443733612","name":"听音乐"},{"id":"9393533470027694381","name":"看电影"},{"id":"13082348043929536259","name":"魅族"},{"id":"14728618028410374988","name":"摄影"},{"id":"14986941257412550705","name":"M9"},{"id":"17168216440080056340","name":"爱狗"}],"tweet":[{"from":"腾讯微博","id":"71120012656231","text":"魅族的壁纸单张的版权费就几百美金，不知道商业授权要多少钱。肯定很吓人。","timestamp":1323324978}]},
            //{"city_code":"1","country_code":"1","fansnum":18,"head":"http://app.qlogo.cn/mbloghead/029e58a18b7fc410a238","idolnum":80,"isidol":true,"isvip":0,"location":"未知","name":"z580019","nick":"zxm","openid":"","province_code":"41","tag":null,"tweet":[{"from":"QQ空间说说","id":"220052593834","text":"[em]e300[/em]v","timestamp":1323041442}]}],
            //"timestamp":1323327658},"errcode":0,"msg":"ok","ret":0}
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
            param.Add(new UrlParameter("format", "json"));
            param.Add(new UrlParameter("content", text.SubString(270, "").UrlUpperEncode()));
            //param.Add(new UrlParameter("clientip", "127.0.0.1"));
            //param.Add(new UrlParameter("jing", ""));
            //param.Add(new UrlParameter("wei", ""));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign = new StringBuilder().Append("POST&")
                .Append(add.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());

            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, accessSecret, sbSign.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());
            HttpHelper.SendPost(add, OAuthCommon.GetUrlParameter(param));
        }
    }
}
