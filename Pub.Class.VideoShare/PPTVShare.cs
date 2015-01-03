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
    /// pptv.com视频资料
    /// 
    /// 修改纪录
    ///     2011.12.12 版本：1.1 livexy 修改正则
    ///     2011.10.18 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class PPTVShare : IVideoShare {
        /// <summary>
        /// 取指定URL的视频资料
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>分享视频实体</returns>
        public VideoInfo GetVideoInfo(string url) {
            #region 测试数据
            //http://v.pptv.com/show/BFhJxibK6JGJ43EQ.html
            //http://player.pptv.com/v/BFhJxibK6JGJ43EQ.swf
            //分享到新浪微博
            //http://v.t.sina.com.cn/share/share.php?c=spr_web_bd_pplive_weibo
            //&url=http%3A%2F%2Fv.pptv.com%2Fshow%2FBFhJxibK6JGJ43EQ.html
            //&title=%E5%A8%B1%E4%B9%90%E6%92%AD%E6%8A%A5-20111019-%E4%BA%94%E6%98%9F%E6%83%8A%E7%8E%B0%E5%B1%B1%E5%AF%A8%E7%89%88%E7%9A%84%E8%A5%BF%E5%8D%95%E5%A5%B3%E5%AD%A9
            //&source=PPLive%E7%BD%91%E7%BB%9C%E7%94%B5%E8%A7%86
            //&sourceUrl=http%3A%2F%2Fwww.pptv.com
            //&content=utf-8
            //&pic=
            //&appkey=1938876518
            //<a target="_blank" title="分享到新浪微博" href="http://v.t.sina.com.cn/share/share.php?c=spr_web_bd_pplive_weibo&url=http%3A%2F%2Fv.pptv.com%2Fshow%2FBFhJxibK6JGJ43EQ.html&title=%E5%A8%B1%E4%B9%90%E6%92%AD%E6%8A%A5-20111019-%E4%BA%94%E6%98%9F%E6%83%8A%E7%8E%B0%E5%B1%B1%E5%AF%A8%E7%89%88%E7%9A%84%E8%A5%BF%E5%8D%95%E5%A5%B3%E5%AD%A9&source=PPLive%E7%BD%91%E7%BB%9C%E7%94%B5%E8%A7%86&sourceUrl=http%3A%2F%2Fwww.pptv.com&content=utf-8&pic=&appkey=1938876518" class="ico_2"></a>
            #endregion
            if (url.EndsWith(".swf")) {
                string sid = url.GetMatchingValues("/v/(.+?).swf", "/v/", ".swf").FirstOrDefault() ?? "";
                if (sid.IsNullEmpty()) return null;
                url = "http://v.pptv.com/show/{0}.html".FormatWith(sid);
            }
            string data = (Net2.GetRemoteHtmlCode4(url, Encoding.UTF8) ?? "").ReplaceRN();

            string title = data.GetMatchingValues("<h2>《(.+?)》评论</h2>", "<h2>《", "》评论</h2>").FirstOrDefault() ?? "";
            string sina = data.GetMatchingValues("<a target=\"_blank\" title=\"分享到新浪微博\" href=\"(.+?)\" class=\"ico_2\"></a>", "<a target=\"_blank\" title=\"分享到新浪微博\" href=\"", "\" class=\"ico_2\"></a>").FirstOrDefault() ?? "";
            string sina_code = Net2.GetRemoteHtmlCode4(sina, Encoding.UTF8) ?? "";
            string img = sina_code.GetMatchingValues("<img src=\"(.+?)\" alt=\"\" width=\"120\" height=\"80\" /", "<img src=\"", "\" alt=\"\" width=\"120\" height=\"80\" /").FirstOrDefault() ?? "";
            //string flv = data.GetMatchingValues("<input type=\"text\" class=\"txt\" readonly=\"readonly\" value=\"(.+?)\" id=\"fx_btn_txt2\" />", "<input type=\"text\" class=\"txt\" readonly=\"readonly\" value=\"", "\" id=\"fx_btn_txt2\" />").FirstOrDefault() ?? "";
            string flv = data.GetMatchingValues("<label>flash地址：</label> <input type=\"text\" class=\"txt\" readonly=\"readonly\" value=\"(.+?)\" id=\"fx_btn_txt2\" />", "<label>flash地址：</label> <input type=\"text\" class=\"txt\" readonly=\"readonly\" value=\"", "\" id=\"fx_btn_txt2\" />").FirstOrDefault() ?? "";
            return new VideoInfo() { PicUrl = img, Title = title, Url = flv };
        }
    }
}
