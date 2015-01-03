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
    /// ku6.com视频资料
    /// 
    /// 修改纪录
    ///     2011.10.18 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Ku6Share : IVideoShare {
        /// <summary>
        /// 取指定URL的视频资料
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>分享视频实体</returns>
        public VideoInfo GetVideoInfo(string url) {
            #region 测试数据
            //http://v.ku6.com/show/FHC32qF5cQqwOcSA.html
            //http://service.weibo.com/share/share.php?c=spr_web_bd_ku6_weibo
            //&url=http%3A%2F%2Fv.ku6.com%2Fshow%2FFHC32qF5cQqwOcSA.html
            //&appkey=612700343
            //&ralateUid=1829627032
            //&title=%E6%95%91%E4%BA%BA%E9%98%BF%E5%A7%A8%E8%8E%B75%E4%B8%87%E5%85%83%E5%A5%96%E9%87%91%20%E8%A1%A8%E7%A4%BA%E6%8A%8A%E9%83%A8%E5%88%86%E5%A5%96%E9%87%91%E6%8D%90%E7%BB%99%E5%A5%B3%E7%AB%A5
            //&source=%E9%85%B76%E7%BD%91
            //&sourceUrl=http%3A%2F%2Fwww.ku6.com%2F
            //&content=gbk
            //&pic=http%3A%2F%2Fi3.ku6img.com%2Fencode%2Fpicpath%2F2011%2F10%2F18%2F9%2F1322073942415_8090181_8090181%2F5.jpg

            //title: "\u6551\u4eba\u963f\u59e8\u83b75\u4e07\u5143\u5956\u91d1\u0020\u8868\u793a\u628a\u90e8\u5206\u5956\u91d1\u6350\u7ed9\u5973\u7ae5"
            //cover: "http://i3.ku6img.com/encode/picpath/2011/10/18/9/1322073942415_8090181_8090181/5.jpg"
            //http://player.ku6.com/refer/FHC32qF5cQqwOcSA/v.swf
            #endregion
            if (url.EndsWith("/v.swf")) {
                string sid = url.GetMatchingValues("/refer/(.+?)/v.swf", "/refer/", "/v.swf").FirstOrDefault() ?? "";
                if (sid.IsNullEmpty()) return null;
                url = "http://v.ku6.com/show/{0}.html".FormatWith(sid);
            }

            string[] list = url.Split('/');
            string code = list[list.Length - 1].GetMatchingValues("(.+?).html", " ", ".html").FirstOrDefault() ?? "";
            if (code.IsNullEmpty()) return null;

            string data = Net2.GetRemoteHtmlCode4(url, Encoding.UTF8) ?? "";

            string title = (data.GetMatchingValues("title: \"(.+?)\"", "title: \"", "\"").FirstOrDefault() ?? "");
            if (title.IsNullEmpty()) title = (data.GetMatchingValues("title : \"(.+?)\"", "title: \"", "\"").FirstOrDefault() ?? "");
            title = title.Ascii2Native();
            string img = data.GetMatchingValues("cover: \"(.+?)\"", "cover: \"", "\"").FirstOrDefault() ?? "";
            if (img.IsNullEmpty()) img = data.GetMatchingValues("cover : \"(.+?)\"", "cover : \"", "\"").FirstOrDefault() ?? "";
            string flv = "http://player.ku6.com/refer/{0}/v.swf".FormatWith(code);

            return new VideoInfo() { PicUrl = img, Title = title, Url = flv };
        }
    }
}
