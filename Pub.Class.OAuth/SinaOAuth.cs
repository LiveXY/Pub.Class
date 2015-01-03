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
using System.Net;
using System.IO;

namespace Pub.Class {
    /// <summary>
    /// sina 授权登录
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class SinaOAuth : IOAuth {
        /// <summary>
        /// request_token
        /// </summary>
        public static readonly string request_token = "http://api.t.sina.com.cn/oauth/request_token";
        /// <summary>
        /// authorize
        /// </summary>
        public static readonly string authorize = "http://api.t.sina.com.cn/oauth/authorize";
        /// <summary>
        /// access_token
        /// </summary>
        public static readonly string access_token = "http://api.t.sina.com.cn/oauth/access_token";
        /// <summary>
        /// user_info
        /// </summary>
        public static readonly string user_info = "http://api.t.sina.com.cn/account/verify_credentials.xml";
        /// <summary>
        /// friends_list
        /// </summary>
        public static readonly string friends_list = "http://api.t.sina.com.cn/statuses/friends.json";
        /// <summary>
        /// add
        /// </summary>
        public static readonly string add = "http://api.t.sina.com.cn/statuses/update.json";
        /// <summary>
        /// sina app 配置信息
        /// </summary>
        public static readonly ConfigInfo config = OAuthConfig.GetConfigInfo(OAuthEnum.sina);
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
                .Append(request_token.UrlEncode2())
                .Append("&")
                .Append(OAuthCommon.GetUrlParameter(param).UrlEncode2());
            
            param.Add(new UrlParameter("oauth_signature", OAuthCommon.GetHMACSHA1(config.AppSecret, "", sbSign.ToString()).UrlEncode2()));
            param.Sort(new UrlParameterCompre());
            string data = HttpHelper.SendGet(new StringBuilder().Append(request_token).Append("?").Append(OAuthCommon.GetUrlParameter(param)).ToString());

            int intOTS = data.IndexOf("oauth_token=");
            int intOTSS = data.IndexOf("&oauth_token_secret=");
            string oauth_token = data.Substring(intOTS + 12, intOTSS - (intOTS + 12));
            string oauth_token_secret = data.Substring((intOTSS + 20), data.Length - (intOTSS + 20));
            Session2.Set("oauth_token", oauth_token);
            Session2.Set("oauth_token_secret", oauth_token_secret);
            return (authorize + "?oauth_token=" + oauth_token + "&oauth_callback=" + config.RedirectUrl);
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
            
            int intOTS = data.IndexOf("oauth_token=");
            int intOTSS = data.IndexOf("&oauth_token_secret=");
            int intUser = data.IndexOf("&user_id=");
            user.Token = data.Substring(intOTS + 12, intOTSS - (intOTS + 12));
            user.Secret = data.Substring((intOTSS + 20), intUser - (intOTSS + 20));
            user.UserID = data.Substring((intUser + 9), data.Length - (intUser + 9));

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
            
            user.Name = data.GetMatchingValues("<name>(.+?)</name>", "<name>", "</name>").FirstOrDefault() ?? "";
            user.Header = data.GetMatchingValues("<profile_image_url>(.+?)</profile_image_url>", "<profile_image_url>", "</profile_image_url>").FirstOrDefault() ?? "";
            user.Sex = (data.GetMatchingValues("<gender>(.+?)</gender>", "<gender>", "</gender>").FirstOrDefault() ?? "").ToLower().Equals("m") ? 1 : 0;
            user.Address = data.GetMatchingValues("<location>(.+?)</location>", "<location>", "</location>").FirstOrDefault() ?? "";

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
                param.Add(new UrlParameter("source", config.AppKey));
                param.Add(new UrlParameter("count", count));
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
                IList<string> userlist = data.GetMatchingValues("{\"id\":(.+?)}}", "{", "}}");

                foreach (string info in userlist) {
                    UserInfo user = new UserInfo();
                    user.UserID = info.GetMatchingValues("\"id\":(.+?),", "\"id\":", ",").FirstOrDefault() ?? "";
                    user.Email = info.GetMatchingValues("\"email\":\"(.+?)\"", "\"email\":\"", "\"").FirstOrDefault() ?? "";
                    user.Name = info.GetMatchingValues("\"name\":\"(.+?)\"", "\"name\":\"", "\"").FirstOrDefault() ?? "";
                    user.Sex = (info.GetMatchingValues("\"gender\":\"(.+?)\"", "\"gender\":\"", "\"").FirstOrDefault() ?? "") == "m" ? 1 : 0;
                    user.Address = info.GetMatchingValues("\"location\":\"(.+?)\"", "\"location\":\"", "\"").FirstOrDefault() ?? "";
                    user.Header = info.GetMatchingValues("\"profile_image_url\":\"(.+?)\"", "\"profile_image_url\":\"", "\"").FirstOrDefault() ?? "";
                    list.Add(user);
                }

                if (userlist.IsNull() || userlist.Count == 0) isTrue = false;
                page++;
            };

            //{"users":[
            //{"id":1972885037,"screen_name":"","name":"","province":"11","city":"8","location":"","description":"","url":"","profile_image_url":"","domain":"diandianteam","gender":"m","followers_count":74327,"friends_count":69,"statuses_count":430,"favourites_count":1,"created_at":"Wed Feb 16 00:00:00 +0800 2011","following":false,"allow_all_act_msg":true,"geo_enabled":false,"verified":true,"remark":"","status":{"created_at":"Thu Dec 08 13:35:46 +0800 2011","id":3388322699728824,"text":"工场的兄弟产品，帮推哦[好得意]","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388322699728824","retweeted_status":{"created_at":"Wed Dec 07 15:46:22 +0800 2011","id":3387993178396122,"text":"当你想找iPhone/iTouch/iPad上的软件或游戏的时候，同步推就是一个：可以帮你找到所有好玩好用还想要软件的方便小帮手。现在登陆AppStore，可以直接下载同步推啦。iTunes下载地址：http://t.cn/SqiARV","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","thumbnail_pic":"http://ww2.sinaimg.cn/thumbnail/77dc26f5gw1dnu2qpro98j.jpg","bmiddle_pic":"http://ww2.sinaimg.cn/bmiddle/77dc26f5gw1dnu2qpro98j.jpg","original_pic":"http://ww2.sinaimg.cn/large/77dc26f5gw1dnu2qpro98j.jpg","geo":null,"mid":"3387993178396122","user":{"id":2010916597,"screen_name":"同步推","name":"同步推","province":"35","city":"2","location":"福建 厦门","description":"一站式浏览、下载、安装海量游戏和应用，超百万iPhone/iPad/iPod Touch 用户正在使用的免费应用商店。","url":"http://tui.tongbu.com","profile_image_url":"http://tp2.sinaimg.cn/2010916597/50/5617123876/0","domain":"tongbutui","gender":"f","followers_count":7341,"friends_count":39,"statuses_count":566,"favourites_count":3,"created_at":"Mon Mar 07 00:00:00 +0800 2011","following":false,"allow_all_act_msg":true,"geo_enabled":true,"verified":true}}}},
            //{"id":2493118952,"screen_name":"花瓣网","name":"花瓣网","province":"33","city":"1","location":"浙江 杭州","description":"花瓣网，帮你收集，发现网络上你喜欢的事物。 http://www.huaban.com/","url":"http://www.huaban.com/","profile_image_url":"http://tp1.sinaimg.cn/2493118952/50/5615173111/0","domain":"huabanwang","gender":"f","followers_count":14452,"friends_count":341,"statuses_count":287,"favourites_count":18,"created_at":"Tue Oct 25 00:00:00 +0800 2011","following":false,"allow_all_act_msg":true,"geo_enabled":true,"verified":true,"remark":"","status":{"created_at":"Fri Dec 09 09:26:55 +0800 2011","id":3388622462153735,"text":"来看这个趣味的采集：【可爱的垃圾袋】日本启动的一个GARBAGE BAG ART WORK（垃圾袋艺术品计划），由设计工作室MAQ主导，试图使得街头常见的垃圾袋变得更为美观，让城市风景更加美好。环保是全球的责任，就应该从最小的细节做起！http://t.cn/SqDsVw [撒花]","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388622462153735","retweeted_status":{"created_at":"Fri Dec 09 08:59:12 +0800 2011","id":3388615487373188,"text":"【可爱的垃圾袋】行走于日本的街头，如果看见这些漂亮的袋子，你肯定会好奇是谁忍心把这些有趣的袋子随意丢在街头呢？其实这是日本启动的一个GARBAGE BAG ART WORK（垃圾袋艺术品计划），由设计工作室MAQ主导，试图... http://t.cn/SqDsVw  （分享自 @花瓣网）","source":"<a href=\"http://huaban.com/\" rel=\"nofollow\">花瓣网</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","thumbnail_pic":"http://ww3.sinaimg.cn/thumbnail/67f7a5a4jw1dnw282j5mej.jpg","bmiddle_pic":"http://ww3.sinaimg.cn/bmiddle/67f7a5a4jw1dnw282j5mej.jpg","original_pic":"http://ww3.sinaimg.cn/large/67f7a5a4jw1dnw282j5mej.jpg","geo":null,"mid":"3388615487373188","user":{"id":1744283044,"screen_name":"杭州小杰","name":"杭州小杰","province":"33","city":"1","location":"浙江 杭州","description":"眼泪眼屎，意守丹田。","url":"","profile_image_url":"http://tp1.sinaimg.cn/1744283044/50/1289142138/1","domain":"jf882736","gender":"m","followers_count":758,"friends_count":228,"statuses_count":1173,"favourites_count":11,"created_at":"Thu May 20 00:00:00 +0800 2010","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false}}}},
            //{"id":1662047260,"screen_name":"SinaAppEngine","name":"SinaAppEngine","province":"11","city":"8","location":"北京 海淀区","description":"Sina App Engine专注于提供高品质的应用云服务.您可以访问我们的网站了解更多信息 http://sae.sina.com.cn","url":"http://sae.sina.com.cn","profile_image_url":"http://tp1.sinaimg.cn/1662047260/50/1258613353/1","domain":"saet","gender":"m","followers_count":111018,"friends_count":114,"statuses_count":1867,"favourites_count":15,"created_at":"Thu Nov 19 00:00:00 +0800 2009","following":false,"allow_all_act_msg":true,"geo_enabled":true,"verified":true,"remark":"","status":{"created_at":"Fri Dec 09 10:02:38 +0800 2011","id":3388631445927062,"text":"恭喜，#线上活动#真欢乐！","source":"<a href=\"http://event.weibo.com/\" rel=\"nofollow\">微活动</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388631445927062","retweeted_status":{"created_at":"Thu Dec 08 16:25:46 +0800 2011","id":3388365481554324,"text":"O(∩_∩)O#线上活动#奔放上线!http://t.cn/Squlm0这哥们木有#有奖活动#一掷千金滴气派，也缺乏#同城活动#惊艳遇见滴可能，But!TA有颗迷恋新鲜滴心，TA热衷创意、流连灵感…TA具有#普通 文艺 2X青年#多重气质，令人感叹#看到它我就XX了#。TA！就是微活动家滴活宝，等你调戏http://t.cn/Sqn9hQ","source":"<a href=\"http://event.weibo.com/\" rel=\"nofollow\">微活动</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","thumbnail_pic":"http://ww4.sinaimg.cn/thumbnail/6f20c747tw1dnv9826m22j.jpg","bmiddle_pic":"http://ww4.sinaimg.cn/bmiddle/6f20c747tw1dnv9826m22j.jpg","original_pic":"http://ww4.sinaimg.cn/large/6f20c747tw1dnv9826m22j.jpg","geo":null,"mid":"3388365481554324","user":{"id":1864419143,"screen_name":"活动小队长","name":"活动小队长","province":"11","city":"8","location":"北京 海淀区","description":"新浪微博微活动平台官方微博。 靠谱活动推荐 惊喜你的生活！    <br />\n周一至周五 9：30-18：30 为您答疑解惑！<br />\n         <br />\n活动合作\\…","url":"http://event.weibo.com","profile_image_url":"http://tp4.sinaimg.cn/1864419143/50/5601946761/0","domain":"event","gender":"f","followers_count":877553,"friends_count":394,"statuses_count":2439,"favourites_count":8,"created_at":"Tue Nov 16 00:00:00 +0800 2010","following":false,"allow_all_act_msg":true,"geo_enabled":true,"verified":true},"annotations":[{"source":{"id":"294315","appid":"38","name":"#线上活动#欢乐上线！转发赢限量奖品","title":"#线上活动#欢乐上...","url":"http://event.weibo.com/294315"}}]}}},
            //{"id":1579058951,"screen_name":"课件培训-何佳瑾","name":"课件培训-何佳瑾","province":"11","city":"2","location":"北京 西城区","description":"不懂美工和IT，做出一流水准的专业高效PPT与快速课件。","url":"http://blog.sina.com.cn/kejianttt","profile_image_url":"http://tp4.sinaimg.cn/1579058951/50/5609489542/0","domain":"kejianttt","gender":"f","followers_count":1081,"friends_count":318,"statuses_count":206,"favourites_count":26,"created_at":"Sat Nov 07 00:00:00 +0800 2009","following":false,"allow_all_act_msg":true,"geo_enabled":false,"verified":false,"remark":"","status":{"created_at":"Fri Dec 09 08:53:52 +0800 2011","id":3388614145058479,"text":"#JJ学习#提升课程有效性的关键之四：课程是否让学员展示了理解程度？ 评估学员对课程内容的理解程度，将有助于学员将所学内容与已有知识整合起来。选择题或判断题只能显示一个人是否知道了某项事实。要衡量学员对所学知识的理解程度，应当是看他是否能把这些事实运用于决策。(何佳瑾整编自Articulate)","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388614145058479"}},{"id":1175961930,"screen_name":"俞小白童鞋","name":"俞小白童鞋","province":"31","city":"15","location":"上海 浦东新区","description":"曾经有一人，爱我如生命。","url":"http://blog.sina.com.cn/yamamototaro","profile_image_url":"http://tp3.sinaimg.cn/1175961930/50/5607261690/1","domain":"yamamototaro","gender":"m","followers_count":879,"friends_count":182,"statuses_count":6460,"favourites_count":109,"created_at":"Wed Mar 31 00:00:00 +0800 2010","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false,"remark":"","status":{"created_at":"Thu Dec 08 22:13:17 +0800 2011","id":3388452935175529,"text":"转发微博。","source":"<a href=\"http://weibo.com/mobile/iphone.php\" rel=\"nofollow\">iPhone客户端</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388452935175529","retweeted_status":{"created_at":"Thu Dec 08 21:43:10 +0800 2011","id":3388445355582747,"text":"有的恋爱像吃大的蛋糕，你要切下几刀后，才能开始嚐到它的甜。有的恋爱像吃小的蛋糕，才吃一口，就吃完了。有的恋爱像吃甜甜圈，你一边嚐到它的甜，一边清楚的看见：它的心是空的。。。。【 康永 - 给未知恋人的爱情短信 】...........................( 图 * 郑维 作品 )","source":"<a href=\"http://weibo.com/mobile/iphone.php\" rel=\"nofollow\">iPhone客户端</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","thumbnail_pic":"http://ww1.sinaimg.cn/thumbnail/4c69db7djw1dnvionwxohj.jpg","bmiddle_pic":"http://ww1.sinaimg.cn/bmiddle/4c69db7djw1dnvionwxohj.jpg","original_pic":"http://ww1.sinaimg.cn/large/4c69db7djw1dnvionwxohj.jpg","geo":null,"mid":"3388445355582747","user":{"id":1282005885,"screen_name":"蔡康永","name":"蔡康永","province":"71","city":"1000","location":"台湾","description":"** 工作邀約康永, 請洽經紀公司: ying@hte888.com // ( 北京傳真FAX )+86-10-65309417 // ( 台北傳真FAX )+886-2-27523638","url":"http://blog.sina.com.cn/caikangyong","profile_image_url":"http://tp2.sinaimg.cn/1282005885/50/5617303476/1","domain":"caikangyong","gender":"m","followers_count":12115806,"friends_count":413,"statuses_count":489,"favourites_count":1577,"created_at":"Fri Aug 28 00:00:00 +0800 2009","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":true},"annotations":[{"server_ip":"10.73.19.66"}]},"annotations":[]}},
            //{"id":1712092715,"screen_name":"eLearning实施","name":"eLearning实施","province":"31","city":"1000","location":"上海","description":"热爱e-Learning，关注e-Learning现状和发展趋势，专注于企业e-Learning实施。","url":"","profile_image_url":"http://tp4.sinaimg.cn/1712092715/50/1300094129/0","domain":"sumanelearning","gender":"f","followers_count":538,"friends_count":173,"statuses_count":73,"favourites_count":11,"created_at":"Wed Mar 31 00:00:00 +0800 2010","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false,"remark":"","status":{"created_at":"Sat Aug 13 17:49:25 +0800 2011","id":3345987150202697,"text":"每个人在不同的场合，都要扮演不同的角色，不分场合或混淆觉色都是不负责的。“活在当下”，在当下全情投入，演好我的角色。","source":"<a href=\"http://weibo.com/mobile/wap.php\" rel=\"nofollow\">新浪微博手机版</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3345987150202697","retweeted_status":{"created_at":"Sat Aug 13 16:41:33 +0800 2011","id":3345970070820733,"text":"上一分钟在咖啡馆接受记者的采访，大谈特谈网络文学和影视改编，下一分钟采访结束告别记者立马奔进超市，化身打算买菜回家做饭的欧巴桑。[害羞]","source":"<a href=\"http://weibo.com/mobile/iphone.php\" rel=\"nofollow\">iPhone客户端</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3345970070820733","user":{"id":1225419417,"screen_name":"匪我思存","name":"匪我思存","province":"42","city":"1","location":"湖北 武汉","description":"人懒、嘴馋、会拖稿、间歇刷屏、话痨、假装有文化、品味差。不看@，有事请写信：feiwsc@sohu.com","url":"http://www.feiwosicun.net/bbs/index.php","profile_image_url":"http://tp2.sinaimg.cn/1225419417/50/1279875696/0","domain":"fwsc","gender":"f","followers_count":388309,"friends_count":163,"statuses_count":3350,"favourites_count":6,"created_at":"Fri Aug 28 00:00:00 +0800 2009","following":false,"allow_all_act_msg":false,"geo_enabled":false,"verified":true},"annotations":[{"server_ip":"10.73.19.140"}]},"annotations":[]}},
            //{"id":2030979823,"screen_name":"林海lloyd","name":"林海lloyd","province":"31","city":"1000","location":"上海","description":"","url":"","profile_image_url":"http://tp4.sinaimg.cn/2030979823/50/5602042778/1","domain":"","gender":"m","followers_count":47,"friends_count":39,"statuses_count":21,"favourites_count":11,"created_at":"Thu Mar 17 00:00:00 +0800 2011","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false,"remark":"","status":{"created_at":"Tue Nov 29 12:00:29 +0800 2011","id":3385037228465654,"text":"這事怎麼會髮生，30顆螺絲啊，要死人的！！！[怒骂][怒骂][怒骂]","source":"<a href=\"http://weibo.com/mobile/android.php\" rel=\"nofollow\">Android客户端</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3385037228465654","retweeted_status":{"created_at":"Tue Nov 29 10:30:11 +0800 2011","id":3385014504838578,"text":"【法国航空公司客机在中国维修后发现少30颗螺钉】太让人虚惊了，法国航空公司一架空客A340客机在中国厦门维修后，继续飞行，几天後才发现飞机的一块保护板上竟然少了近30个螺丝钉。继“中国制造”之后，“中国维修”也要威震世界了！http://t.cn/SUrBIn","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","thumbnail_pic":"http://ww4.sinaimg.cn/thumbnail/71ced708tw1dnkknmmmxfj.jpg","bmiddle_pic":"http://ww4.sinaimg.cn/bmiddle/71ced708tw1dnkknmmmxfj.jpg","original_pic":"http://ww4.sinaimg.cn/large/71ced708tw1dnkknmmmxfj.jpg","geo":null,"mid":"3385014504838578","user":{"id":1909380872,"screen_name":"企业家智库","name":"企业家智库","province":"11","city":"1000","location":"北京","description":"微博中最具影响力的商界精英聚集地！欢迎关注分享。","url":"http://weibo.com/jingcaimingren","profile_image_url":"http://tp1.sinaimg.cn/1909380872/50/5617266792/1","domain":"nannvqushi","gender":"m","followers_count":184194,"friends_count":378,"statuses_count":5545,"favourites_count":1591,"created_at":"Mon Dec 27 00:00:00 +0800 2010","following":false,"allow_all_act_msg":true,"geo_enabled":true,"verified":false}},"annotations":[]}},
            //{"id":1842500341,"screen_name":"病态的栗子","name":"病态的栗子","province":"31","city":"1000","location":"上海","description":"呐喊——以拒绝同流合污的名义！","url":"","profile_image_url":"http://tp2.sinaimg.cn/1842500341/50/5615791638/1","domain":"","gender":"m","followers_count":98,"friends_count":74,"statuses_count":541,"favourites_count":2,"created_at":"Sat Oct 30 00:00:00 +0800 2010","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false,"remark":"","status":{"created_at":"Thu Dec 08 09:18:24 +0800 2011","id":3388257929528921,"text":"中国哲学的根基其实是思辩，有很强的逻辑，并非只重效而忽略理，恰恰相反。重效而略理往往欲速不达，好效一定有逻辑，不能因为你看不透而认为不重要，这是本末倒置。培训人做坏了培训业。 //@丶陈大钢:// @赵克欣 : 道，无形无相，如糖入水，视之不见，尝之有味。","source":"<a href=\"http://weibo.com/mobile/iphone.php\" rel=\"nofollow\">iPhone客户端</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388257929528921","retweeted_status":{"created_at":"Wed Dec 07 23:42:57 +0800 2011","id":3388113113705520,"text":"行动学习催化师需要对企业文化有充分的认识与理解，否则会陷到机械的流程与技术中，忘记学习本身就是适变过程。在系统和谐前提下，有效比有道理更重要。","source":"<a href=\"http://weibo.com/mobile/ipad.php\" rel=\"nofollow\">iPad客户端</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388113113705520","user":{"id":1738470153,"screen_name":"李珂nichole","name":"李珂nichole","province":"44","city":"3","location":"广东 深圳","description":"管理培训 培训师培训 培训系统","url":"http://blog.sina.com.cn/u/1738470153","profile_image_url":"http://tp2.sinaimg.cn/1738470153/50/5616159177/0","domain":"sznichole","gender":"f","followers_count":906,"friends_count":337,"statuses_count":1280,"favourites_count":123,"created_at":"Tue Jul 13 00:00:00 +0800 2010","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false}},"annotations":[]}},
            //{"id":1890959170,"screen_name":"江姜蒋","name":"江姜蒋","province":"31","city":"1000","location":"上海","description":"","url":"","profile_image_url":"http://tp3.sinaimg.cn/1890959170/50/5613749110/0","domain":"yvonne001","gender":"f","followers_count":90,"friends_count":98,"statuses_count":712,"favourites_count":26,"created_at":"Thu Dec 09 00:00:00 +0800 2010","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false,"remark":"","status":{"created_at":"Thu Dec 08 17:50:54 +0800 2011","id":3388386905931032,"text":"//@俞小白童鞋: @壹嗰餃釨","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388386905931032","retweeted_status":{"created_at":"Mon Nov 28 15:35:05 +0800 2011","id":3384728843254125,"text":"#YouTube闪闪亮#【坠机可怕，更可怕的是。。。】如此淡定的机长！[生病] 还有如此配合的空姐。。。！[闪电] http://t.cn/S4GtnR 你们淡定，我们蛋疼呀！！[抓狂]","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","thumbnail_pic":"http://ww3.sinaimg.cn/thumbnail/83fae389tw1dnjntnk65kj.jpg","bmiddle_pic":"http://ww3.sinaimg.cn/bmiddle/83fae389tw1dnjntnk65kj.jpg","original_pic":"http://ww3.sinaimg.cn/large/83fae389tw1dnjntnk65kj.jpg","geo":null,"mid":"3384728843254125","user":{"id":2214257545,"screen_name":"YouTube精选","name":"YouTube精选","province":"400","city":"1","location":"海外 美国","description":"国内要刷微博，国外也要刷微博，作为一个搬运工，最重要的是不怕河蟹，墙内墙外都是浮云，我是YouTube精选，我在weibo.com","url":"http://weibo.com/youtube","profile_image_url":"http://tp2.sinaimg.cn/2214257545/50/5604558022/0","domain":"youtube","gender":"f","followers_count":326225,"friends_count":327,"statuses_count":2312,"favourites_count":9,"created_at":"Sat Jul 02 00:00:00 +0800 2011","following":false,"allow_all_act_msg":true,"geo_enabled":true,"verified":false}}}},
            //{"id":1688983163,"screen_name":"keylogic王成","name":"keylogic王成","province":"11","city":"1","location":"北京 东城区","description":"KeyLogic公司创始人，秉承“赋能于人”的使命和“专业主义”的价值观创业，爱人才、喜战略、奉儒释道、求民主自由","url":"","profile_image_url":"http://tp4.sinaimg.cn/1688983163/50/5617696588/1","domain":"wangchengkeylogic","gender":"m","followers_count":1714,"friends_count":458,"statuses_count":969,"favourites_count":1,"created_at":"Tue Feb 02 00:00:00 +0800 2010","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false,"remark":"","status":{"created_at":"Thu Dec 08 23:45:38 +0800 2011","id":3388476173068478,"text":"越是乱时越需要静，静时方能心清:)","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","geo":null,"mid":"3388476173068478","retweeted_status":{"created_at":"Thu Dec 08 23:44:22 +0800 2011","id":3388475858817826,"text":"悟：人在忙时最容易出现心乱，心乱则事更乱。回顾师父 @济群法师 昨天晚霞的微博，让我体验到心静则国土净。越是乱时越需要静，静时方能心清。同时困难时候往往是提升的最佳时机与机遇。回到公司，静、定、思一下，心清净许多。过程全力以赴，果上随缘，无怨无悔！","source":"<a href=\"http://weibo.com\" rel=\"nofollow\">新浪微博</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","thumbnail_pic":"http://ww3.sinaimg.cn/thumbnail/68583a48gw1dnvm4bzfd3j.jpg","bmiddle_pic":"http://ww3.sinaimg.cn/bmiddle/68583a48gw1dnvm4bzfd3j.jpg","original_pic":"http://ww3.sinaimg.cn/large/68583a48gw1dnvm4bzfd3j.jpg","geo":null,"mid":"3388475858817826","user":{"id":1750612552,"screen_name":"菩提禅缘","name":"菩提禅缘","province":"33","city":"1","location":"浙江 杭州","description":"皈依　@济群法师　门下，于菩提书院修学道次第！随缘无我，如梦如幻！菩提大道，三学增上！阿里十年、风雨同舟！----淘宝禅缘　(陈瑜、觉贤居士）","url":"http://blog.sina.com.cn/putichanyuan","profile_image_url":"http://tp1.sinaimg.cn/1750612552/50/1279901586/1","domain":"putichanyuan","gender":"m","followers_count":11522,"friends_count":473,"statuses_count":1331,"favourites_count":30,"created_at":"Fri Jun 04 00:00:00 +0800 2010","following":false,"allow_all_act_msg":true,"geo_enabled":false,"verified":true}}}}
            //],"next_cursor":10,"previous_cursor":0}
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
            param.Add(new UrlParameter("source", config.AppKey));
            param.Add(new UrlParameter("status", text.SubString(270, "").UrlUpperEncode()));
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
