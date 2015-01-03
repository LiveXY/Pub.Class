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
    /// youku.com视频资料
    /// 
    /// 修改纪录
    ///     2011.12.12 版本：1.1 livexy 修改正则
    ///     2011.10.18 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class YoukuShare : IVideoShare {
        /// <summary>
        /// 取指定URL的视频资料
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>分享视频实体</returns>
        public VideoInfo GetVideoInfo(string url) {
            #region 测试数据
            //http://v.youku.com/v_playlist/f16352267o1p2.html
            //http://v.youku.com/v_show/id_XMzEzNzAxNzky.html
            //http://player.youku.com/player.php/Type/Folder/Fid/16352267/Ob/1/Pt/3/sid/XMzEyMjE4Nzk2/v.swf <input type="text" id="link2" value="http://player.youku.com/player.php/sid/XMzEzNzAxNzky/v.swf" />
            //http://v.t.sina.com.cn/share/share.php?appkey=2684493555
            //&url=http://v.youku.com/v_show/id_XMzEyMTg4NDg4.html
            //&title=%E4%B8%AD%E5%9B%BD%E7%BD%91%E5%95%86%E7%BB%B4%E6%9D%83%E5%8D%8F%E4%BC%9A%E3%80%90%E4%BB%8A%E6%97%A5%E4%B8%80%E7%BA%BF%E6%9B%9D%E5%85%89%E6%B7%98%E5%AE%9D%E5%95%86%E5%9F%8E%E5%9C%88%E9%92%B1%E4%B8%8A%E5%B8%82%EF%BC%8C%E4%B8%8D%E9%A1%BE%E4%B8%AD%E5%B0%8F%E5%8D%96%E5%AE%B6%E7%9A%84%E5%88%A9%E7%9B%8A%E3%80%91
            //&ralateUid=1642904381
            //&source=%E4%BC%98%E9%85%B7%E7%BD%91
            //&sourceUrl=http%3A%2F%2Fwww.youku.com%2F
            //&content=utf8
            //&pic=http://g4.ykimg.com/0100641F464E95CD3FFA0D017E2DA67B689550-EC9D-F339-9651-6D2988EF2CBA

            //http://v.t.sina.com.cn/share/share.php?appkey=2684493555
            //&url=http://v.youku.com/v_show/id_XMzEzMzI3NzI0.html
            //&ralateUid=1642904381
            //&title=%E6%96%AF%E8%92%82%E8%8A%AC%C2%B7%E9%87%91%E6%81%90%E6%80%96%E5%B0%8F%E8%AF%B4%E6%94%B9%E7%BC%96%E3%80%8A%E5%B0%B8%E9%AA%A8%E8%A2%8B%E3%80%8B%E6%8B%8D%E6%91%84%E8%8A%B1%E7%B5%AE
            //&source=%E4%BC%98%E9%85%B7%E7%BD%91
            //&sourceUrl=http%3A%2F%2Fwww.youku.com%2F
            //&content=utf8
            //&pic=http://g2.ykimg.com/0100641F464E9ACEF9F2A700FC8CA4584613FE-429E-B3BD-B7F4-87ECF311C25D" target="_blank"><img src="http://static.youku.com/v1.0.0706/v/img/ico_sina.gif" />新浪微博</a>
            #endregion
            if (url.EndsWith("/v.swf")) {
                string sid = url.GetMatchingValues("sid/(.+?)/v.swf", "sid/", "/v.swf").FirstOrDefault() ?? "";
                if (sid.IsNullEmpty()) return null;
                url = "http://v.youku.com/v_show/id_{0}.html".FormatWith(sid);
            }
            string data = Net2.GetRemoteHtmlCode4(url, Encoding.UTF8) ?? "";

            //string[] sina = (data.GetMatchingValues("charset=\"400-03-10\" id=\"s_sina\" href=\"(.+?)\" target=\"_blank\"><img src=\"http://static.youku.com/v1.0.0706/v/img/ico_sina.gif\"", "charset=\"400-03-10\" id=\"s_sina\" href=\"", "\" target=\"_blank\"><img src=\"http://static.youku.com/v1.0.0706/v/img/ico_sina.gif\"").FirstOrDefault() ?? "").Split('&');
            string[] sina = (data.GetMatchingValues("charset=\"400-03-10\" id=\"s_sina\" href=\"(.+?)\" target=\"_blank\"><img", "charset=\"400-03-10\" id=\"s_sina\" href=\"", "\" target=\"_blank\"><img").FirstOrDefault() ?? "").Split('&');
            string img = "", title = "";
            if (sina.Length == 8) {
                img = sina[7].Right(sina[7].Length - 4);
                title = sina[2].Right(sina[2].Length - 6).UrlDecode();
            } else {
                //sina = (data.GetMatchingValues("charset=\"400-03-10\" id=\"s_sina\" href=\"(.+?)\" target=\"_blank\"><img src=\"http://static.youku.com/v1.0.0706/v/img/ico_sina.gif\"", "charset=\"400-03-10\" id=\"s_sina\" href=\"", "\" target=\"_blank\"><img src=\"http://static.youku.com/v1.0.0706/v/img/ico_sina.gif\"").FirstOrDefault() ?? "").Split('&');
                sina = (data.GetMatchingValues("charset=\"400-03-10\" id=\"s_sina\" href=\"(.+?)\" target=\"_blank\"><img", "charset=\"400-03-10\" id=\"s_sina\" href=\"", "\" target=\"_blank\"><img").FirstOrDefault() ?? "").Split('&');
                if (sina.Length == 8) {
                    img = sina[7].Right(sina[7].Length - 4);
                    title = sina[3].Right(sina[3].Length - 6).UrlDecode();
                }
            }

            string flv = data.GetMatchingValues("<input type=\"text\" id=\"link2\" value=\"(.+?)\" />", "<input type=\"text\" id=\"link2\" value=\"", "\" />").FirstOrDefault() ?? "";
            if (title.IndexOf("优酷") != -1) title = title.Left(title.IndexOf("优酷")).Trim().TrimEnd('-').Trim();
            return new VideoInfo() { PicUrl = img, Title = title, Url = flv };
        }
    }
}
