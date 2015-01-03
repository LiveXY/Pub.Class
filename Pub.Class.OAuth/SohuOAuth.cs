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
    /// Sohu 授权登录
    /// 
    /// 修改纪录
    ///     2011.12.02 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class SohuOAuth : IOAuth {
        /// <summary>
        /// request_token
        /// </summary>
        public static readonly string request_token = "http://api.t.sohu.com/oauth/request_token";
        /// <summary>
        /// authorize
        /// </summary>
        public static readonly string authorize = "http://api.t.sohu.com/oauth/authorize";
        /// <summary>
        /// access_token
        /// </summary>
        public static readonly string access_token = "http://api.t.sohu.com/oauth/access_token";
        /// <summary>
        /// user_info
        /// </summary>
        public static readonly string user_info = "http://api.t.sohu.com/account/verify_credentials.json";
        /// <summary>
        /// friends_list
        /// </summary>
        public static readonly string friends_list = "http://api.t.sohu.com/statuses/friends.json";
        /// <summary>
        /// add
        /// </summary>
        public static readonly string add = "http://api.t.sohu.com/statuses/update.json";
        /// <summary>
        /// qq app 配置信息
        /// </summary>
        public static readonly ConfigInfo config = OAuthConfig.GetConfigInfo(OAuthEnum.sohu);
        /// <summary>
        /// 取授权登录URL
        /// </summary>
        /// <returns>登录URL</returns>
        public string GetAuthUrl() {
            List<UrlParameter> param = new List<UrlParameter>();
            param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
            param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
            param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
            param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
            param.Add(new UrlParameter("oauth_version", "1.0"));
            param.Sort(new UrlParameterCompre());

            StringBuilder sbSign = new StringBuilder().Append("GET&")
                .Append(Rfc3986.Encode(request_token))
                .Append("&")
                .Append(Rfc3986.Encode(OAuthCommon.GetUrlParameter(param)));

            param.Add(new UrlParameter("oauth_signature", Rfc3986.Encode(OAuthCommon.GetHMACSHA1(Rfc3986.Encode(config.AppSecret), "", sbSign.ToString()))));
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
                .Append(Rfc3986.Encode(access_token))
                .Append("&")
                .Append(Rfc3986.Encode(OAuthCommon.GetUrlParameter(param)));

            param.Add(new UrlParameter("oauth_signature", Rfc3986.Encode(OAuthCommon.GetHMACSHA1(Rfc3986.Encode(config.AppSecret), Rfc3986.Encode(Session2.Get("oauth_token_secret")), sbSign.ToString()))));
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
                .Append(Rfc3986.Encode(user_info))
                .Append("&")
                .Append(Rfc3986.Encode(OAuthCommon.GetUrlParameter(param)));

            param.Add(new UrlParameter("oauth_signature", Rfc3986.Encode(OAuthCommon.GetHMACSHA1(Rfc3986.Encode(config.AppSecret), Rfc3986.Encode(user.Secret), sbSign2.ToString()))));
            param.Sort(new UrlParameterCompre());
            data = HttpHelper.SendGet(new StringBuilder().Append(user_info).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());

            user.UserID = data.GetMatchingValues("\"id\":\"(.+?)\"", "\"id\":\"", "\"").FirstOrDefault() ?? "";
            user.Email = data.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
            user.Name = data.GetMatchingValues("\"screen_name\":\"(.+?)\"", "\"screen_name\":\"", "\"").FirstOrDefault() ?? "";
            user.Sex = (data.GetMatchingValues("\"gender\":\"(.+?)\"", "\"gender\":\"", "\"").FirstOrDefault() ?? "") == "1" ? 1 : 0;
            user.Header = data.GetMatchingValues("\"profile_image_url\":\"(.+?)\"", "\"profile_image_url\":\"", "\"").FirstOrDefault() ?? "";
            user.Address = data.GetMatchingValues("\"location\":\"(.+?)\"", "\"location\":\"", "\"").FirstOrDefault() ?? "";

            //{"id":"268563401","screen_name":"livexy","name":"","location":"上海市,徐汇区","description":"","url":"","gender":"0",
            //"profile_image_url":"http://s4.cr.itc.cn/img/t/avt_48.jpg","protected":true,"followers_count":1,"profile_background_color":"",
            //"profile_text_color":"","profile_link_color":"","profile_sidebar_fill_color":"","profile_sidebar_border_color":"","friends_count":7,
            //"created_at":"Fri Dec 02 13:26:29 +0800 2011","favourites_count":0,"utc_offset":"","time_zone":"","profile_background_image_url":"",
            //"notifications":"","geo_enabled":false,"statuses_count":0,"following":true,"verified":false,"lang":"zh_cn","contributors_enabled":false}
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
            bool isTrue = true; int count = 5; int page = 1;

            while (isTrue) {
                List<UrlParameter> param = new List<UrlParameter>();
                param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
                param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
                param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
                param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
                param.Add(new UrlParameter("oauth_token", accessToken));
                param.Add(new UrlParameter("oauth_version", "1.0"));
                param.Add(new UrlParameter("page", page));
                param.Add(new UrlParameter("count", count));
                param.Sort(new UrlParameterCompre());

                StringBuilder sbSign = new StringBuilder().Append("GET&")
                    .Append(Rfc3986.Encode(friends_list))
                    .Append("&")
                    .Append(Rfc3986.Encode(OAuthCommon.GetUrlParameter(param)));

                param.Add(new UrlParameter("oauth_signature", Rfc3986.Encode(OAuthCommon.GetHMACSHA1(Rfc3986.Encode(config.AppSecret), Rfc3986.Encode(accessSecret), sbSign.ToString()))));
                param.Sort(new UrlParameterCompre());
                string data = "";
                try {
                    data = HttpHelper.SendGet(new StringBuilder().Append(friends_list).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());
                    data = data.Substring(1, data.Length - 2);
                } catch {}
                IList<string> userlist = data.GetMatchingValues("{\"id\":\"(.+?)}}", "{", "}}");

                foreach (string info in userlist) {
                    UserInfo user = new UserInfo();
                    user.UserID = info.GetMatchingValues("\"id\":\"(.+?)\"", "\"id\":\"", "\"").FirstOrDefault() ?? "";
                    user.Email = info.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
                    user.Name = info.GetMatchingValues("\"screen_name\":\"(.+?)\"", "\"screen_name\":\"", "\"").FirstOrDefault() ?? "";
                    user.Sex = (info.GetMatchingValues("\"gender\":\"(.+?)\"", "\"gender\":\"", "\"").FirstOrDefault() ?? "") == "1" ? 1 : 0;
                    user.Address = info.GetMatchingValues("\"location\":\"(.+?)\"", "\"location\":\"", "\"").FirstOrDefault() ?? "";
                    user.Header = info.GetMatchingValues("\"profile_image_url\":\"(.+?)\"", "\"profile_image_url\":\"", "\"").FirstOrDefault() ?? "";
                    list.Add(user);
                }

                if (userlist.IsNull() || userlist.Count == 0) isTrue = false;
                page++;
            };

            //"users":[
            //{"id":"8641996","screen_name":"新闻头条","name":"","location":"北京市,海淀区","description":"","url":"","gender":"1","profile_image_url":"http://s4.cr.itc.cn/mblog/icon/c7/4b/m_13119583882378.JPG","protected":true,"followers_count":5301900,"profile_background_color":"","profile_text_color":"","profile_link_color":"","profile_sidebar_fill_color":"","profile_sidebar_border_color":"","friends_count":1217,"created_at":"Tue Jun 29 14:35:44 +0800 2010","favourites_count":5,"utc_offset":"","time_zone":"","profile_background_image_url":"","notifications":"","geo_enabled":false,"statuses_count":18590,"following":true,"verified":true,"lang":"zh_cn","contributors_enabled":false,"status":{"created_at":"Fri Dec 09 13:05:40 +0800 2011","id":"2463351713","text":"【乌鲁木齐一女记者身中5刀顽强自救脱险 】12月6日晚11时30分，新疆都市报社会新闻女记者李娜下班回家，刚进入小区，突遭歹徒持刀抢劫，身中5刀，胸腔、腹腔、左臂、左腿等多处被捅，左肺部破裂。受伤后她淡定招呼出租车将其送往医院，并清醒报警。http://t.itc.cn/LHsfW","source":"搜狐微博","favorited":false,"truncated":"","in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","small_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_13/f_5772229398968603.jpg","middle_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_13/m_5772229398968603.jpg","original_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_13/5772229398968603.jpg"}},
            //{"id":"6376033","screen_name":"搜狐视频","name":"","location":"北京市,-","description":"搜狐视频tv.sohu.com 带给你每日最及时的新闻、电影、电视剧、纪录片、动画片等资讯，敬请关注！","url":"","gender":"1","profile_image_url":"http://s5.cr.itc.cn/mblog/icon/ac/39/m_13083074776832.jpg","protected":true,"followers_count":2540941,"profile_background_color":"","profile_text_color":"","profile_link_color":"","profile_sidebar_fill_color":"","profile_sidebar_border_color":"","friends_count":531,"created_at":"Tue Jun 08 15:16:04 +0800 2010","favourites_count":2,"utc_offset":"","time_zone":"","profile_background_image_url":"","notifications":"","geo_enabled":false,"statuses_count":6694,"following":true,"verified":true,"lang":"zh_cn","contributors_enabled":false,"status":{"created_at":"Fri Dec 09 12:03:09 +0800 2011","id":"2462806849","text":"#搜狐视频微视听#【何洁《爱过的你》MV首播】何洁全新专辑中，最后曝光同时也最受网友青睐的疗伤系主打情歌《爱过的你》MV正式首播。MV中何洁展现出的演技简直可以用出神入化来形容，每一种情绪，每一种状态都拿捏得特别到位，而且每场戏都是一遍通过…… http://t.itc.cn/Lu9HL","source":"搜狐微博","favorited":false,"truncated":"","in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","small_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_12/f_8629582797796467.jpg","middle_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_12/m_8629582797796467.jpg","original_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_12/8629582797796467.jpg"}},
            //{"id":"1323475","screen_name":"搜狐科学","name":"","location":"北京市,-","description":"浩瀚的宇宙+奇异的生物+绝美的景观+历史的痕迹＝搜狐科学频道。","url":"","gender":"1","profile_image_url":"http://s5.cr.itc.cn/mblog/icon/f7/db/m_12706074122238.jpg","protected":false,"followers_count":2246515,"profile_background_color":"","profile_text_color":"","profile_link_color":"","profile_sidebar_fill_color":"","profile_sidebar_border_color":"","friends_count":17,"created_at":"Wed Apr 07 10:26:38 +0800 2010","favourites_count":0,"utc_offset":"","time_zone":"","profile_background_image_url":"","notifications":"","geo_enabled":false,"statuses_count":943,"following":true,"verified":true,"lang":"zh_cn","contributors_enabled":false,"status":{"created_at":"Fri Dec 09 12:30:02 +0800 2011","id":"2463028113","text":"【瑞士发现癌细胞扩散蛋白质 或开启治疗新途径】瑞士科学家近期发现了一种促进肿瘤扩散和转移的“主力”蛋白质——成骨细胞特异因子-2。实验证明，控制这种蛋白质的数量能够有效抑制恶性肿瘤的活跃程度。这一发现将有望给癌症治疗开辟一条全新的途径。http://t.itc.cn/L3RsH","source":"皮皮精灵","favorited":false,"truncated":"","in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","small_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_12/f_8631221896284467.jpg","middle_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_12/m_8631221896284467.jpg","original_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_12/8631221896284467.jpg"}},
            //{"id":"1333002","screen_name":"财经头条","name":"","location":"北京市,-","description":"及时资讯、独家爆料、麻辣点评，以财经视角关注民生视野，一起来微博吧！","url":"","gender":"1","profile_image_url":"http://s4.cr.itc.cn/mblog/icon/29/c7/m_12931723592217.jpg","protected":false,"followers_count":6491415,"profile_background_color":"","profile_text_color":"","profile_link_color":"","profile_sidebar_fill_color":"","profile_sidebar_border_color":"","friends_count":1458,"created_at":"Thu Apr 08 16:39:36 +0800 2010","favourites_count":4,"utc_offset":"","time_zone":"","profile_background_image_url":"","notifications":"","geo_enabled":false,"statuses_count":15916,"following":true,"verified":true,"lang":"zh_cn","contributors_enabled":false,"status":{"created_at":"Fri Dec 09 12:07:59 +0800 2011","id":"2462845477","text":"【东星航空诉民航中南局一审败诉】2009年3月，民航中南局据武汉市政府一纸公函责令东星航空停飞。距离一审整整7个月，东星诉民航中南局这一内地民航第一“民告官”案一审结果终于出来了：广州白云区法院判东星航空败诉。@东星集团 新闻发言人@兰剑敏 称，东星集团将上诉到底。","source":"搜狐微博","favorited":false,"truncated":"","in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","small_pic":"http://s3.t.itc.cn/mblog/pic/201112_9_12/f_8629896712993467.jpg","middle_pic":"http://s3.t.itc.cn/mblog/pic/201112_9_12/m_8629896712993467.jpg","original_pic":"http://s3.t.itc.cn/mblog/pic/201112_9_12/8629896712993467.jpg"}},
            //{"id":"205207873","screen_name":"搜狐微博官方辟谣","name":"","location":"北京市,海淀区","description":"搜狐微博辟谣官方账号","url":"","gender":"1","profile_image_url":"http://s5.cr.itc.cn/mblog/icon/61/30/m_13153908401706.jpg","protected":true,"followers_count":2725279,"profile_background_color":"","profile_text_color":"","profile_link_color":"","profile_sidebar_fill_color":"","profile_sidebar_border_color":"","friends_count":0,"created_at":"Thu Sep 01 20:17:53 +0800 2011","favourites_count":0,"utc_offset":"","time_zone":"","profile_background_image_url":"","notifications":"","geo_enabled":false,"statuses_count":71,"following":true,"verified":true,"lang":"zh_cn","contributors_enabled":false,"status":{"created_at":"Fri Dec 09 10:41:30 +0800 2011","id":"2462125691","text":"近日，网传“最新最恐怖的拐卖妇女方式出炉”一文，经搜狐微博查证，纯属谣言。近日，多位博友发表微博称“最新最恐怖的拐卖妇女方式出炉”并配图一张，详细的表述了所谓的拐卖过程。经搜狐微博查证，早在今年4月，京、上、广三地皆有类似谣言传播，北京、广州警方均在微博上发表过辟谣声明，表示当地警方并未接到相关警情，且谣言中的地铁线路、站名等均有错误，（新闻链接 http://t.itc.cn/LvAXy）国内各...","source":"搜狐微博","favorited":false,"truncated":"","in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","small_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_10/f_5763578358261603.jpg","middle_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_10/m_5763578358261603.jpg","original_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_10/5763578358261603.jpg"}},
            //{"id":"16053","screen_name":"娱乐头条","name":"","location":"北京市,-","description":"搜狐娱乐频道微博，提供最新明星八卦、影视资讯、音乐试听等内容，每周7天每天24小时更新，敬请关注！","url":"","gender":"1","profile_image_url":"http://s5.cr.itc.cn/mblog/icon/09/50/m_12991200389544.jpg","protected":true,"followers_count":5411328,"profile_background_color":"","profile_text_color":"","profile_link_color":"","profile_sidebar_fill_color":"","profile_sidebar_border_color":"","friends_count":781,"created_at":"Thu Jan 07 17:34:26 +0800 2010","favourites_count":8,"utc_offset":"","time_zone":"","profile_background_image_url":"","notifications":"","geo_enabled":false,"statuses_count":17283,"following":true,"verified":true,"lang":"zh_cn","contributors_enabled":false,"status":{"created_at":"Fri Dec 09 11:20:41 +0800 2011","id":"2462471205","text":"【#Touch of Evil#】去年纽约时报做的年度策划是13位演员的表演课，今年的盘点新鲜出炉，主题是邪恶接触（touch of evil）且看布拉德-皮特(Brad Pitt)戏仿《#橡皮头#》中的杰克-南斯。好莱坞大牌如何演绎经典？且看皮特如何像经典致敬！http://t.itc.cn/LmnNU http://t.itc.cn/L3Pvp","source":"搜狐微博","favorited":false,"truncated":"","in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","small_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_11/f_5765897143974603.jpg","middle_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_11/m_5765897143974603.jpg","original_pic":"http://s2.t.itc.cn/mblog/pic/201112_9_11/5765897143974603.jpg"}},
            //{"id":"31963453","screen_name":"搜狐微博官方","name":"","location":"北京市,海淀区","description":"搜狐微博官方活动账号","url":"","gender":"0","profile_image_url":"http://s5.cr.itc.cn/mblog/icon/6a/98/m_13014576518865.jpg","protected":true,"followers_count":13601146,"profile_background_color":"","profile_text_color":"","profile_link_color":"","profile_sidebar_fill_color":"","profile_sidebar_border_color":"","friends_count":158,"created_at":"Fri Jan 07 16:21:46 +0800 2011","favourites_count":0,"utc_offset":"","time_zone":"","profile_background_image_url":"","notifications":"","geo_enabled":false,"statuses_count":2789,"following":true,"verified":true,"lang":"zh_cn","contributors_enabled":false,"status":{"created_at":"Fri Dec 09 09:55:01 +0800 2011","id":"2461704594","text":"看剧抽奖：每周五下午3:30来搜狐视频看 《生活大爆炸》http://t.itc.cn/L3ruv 以 #生活大爆炸#为关键词写下本集你认为最有趣的3个段子发一条微博，我们将抽选10名参与者获得有搜狐微博徽标的iPhone充电宝！由@搜狐视频 公布获奖名单。活动详情：http://t.itc.cn/L464r","source":"搜狐微博","favorited":false,"truncated":"","in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","small_pic":"http://s3.t.itc.cn/mblog/pic/201112_9_9/f_8621637448428467.jpg","middle_pic":"http://s3.t.itc.cn/mblog/pic/201112_9_9/m_8621637448428467.jpg","original_pic":"http://s3.t.itc.cn/mblog/pic/201112_9_9/8621637448428467.jpg"}}
            //],"cursor_id":17283996
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
                .Append(Rfc3986.Encode(add))
                .Append("&")
                .Append(Rfc3986.Encode(OAuthCommon.GetUrlParameter(param)));

            param.Add(new UrlParameter("oauth_signature", Rfc3986.Encode(OAuthCommon.GetHMACSHA1(Rfc3986.Encode(config.AppSecret), Rfc3986.Encode(accessSecret), sbSign.ToString()))));
            param.Sort(new UrlParameterCompre());
            HttpHelper.SendPost(add, OAuthCommon.GetUrlParameter(param), "application/x-www-form-urlencoded");
        }
    }
}
