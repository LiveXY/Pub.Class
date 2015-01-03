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
    /// QQ视频
    /// 
    /// 修改纪录
    ///     2011.10.18 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class VQQShare : IVideoShare {
        /// <summary>
        /// 取指定URL的视频资料
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>分享视频实体</returns>
        public VideoInfo GetVideoInfo(string url) {
            #region 测试数据
            //http://v.qq.com/cover/7/72rfnl00qd0bml4.html
            //http://static.video.qq.com/TPout.swf?vid=84EU5iAW7G1
            #endregion

            if (url.IndexOf(".swf?") != -1) return null;

            string vid = url.IndexOf("vid=") != -1 ? url.Substring(url.IndexOf("vid=") + 4) : "";
            string data = (Net2.GetRemoteHtmlCode4(url, Encoding.UTF8) ?? "").ReplaceRN();
            string extTitle = string.Empty, extImg = string.Empty;
            vid = vid.Split('_')[0].Trim();
            //Msg.WriteEnd(vid);
            if (vid.Length != 11) {
                vid = data.GetMatchingValues("vid:\"(.+?)\"", "vid:\"", "\"").FirstOrDefault() ?? "";
                vid = vid.Trim().Split('|')[0];
                vid = vid.Trim().Split(' ')[0];
                if (vid.Length != 11) {
                    string data2 = data.GetMatchingValues("<div class=\"tab_cont\" id=\"mod_videolist\">(.+?)</div>", "<div class=\"tab_cont\" id=\"mod_videolist\">", "</div>").FirstOrDefault() ?? "";
                    string li = data2.GetMatchingValues("<li>(.+?)</li>", "<li>", "</li>").FirstOrDefault() ?? "";
                    extTitle = li.GetMatchingValues("title=\"(.+?)\"", "title=\"", "\"").FirstOrDefault() ?? "";
                    vid = li.GetMatchingValues("id=\"(.+?)\"", "id=\"", "\"").FirstOrDefault() ?? "";
                }
            };
            if (vid.Length != 11) return null;
            string li2 = data.GetMatchingValues("<li id=\"li_{0}\">(.+?)</li>".FormatWith(vid), "<li id=\"li_{0}\">".FormatWith(vid), "</li>").FirstOrDefault() ?? "";
            if (!li2.Trim().IsNullEmpty()) {
                extImg = li2.GetMatchingValues("src=\"(.+?)\"", "src=\"", "\"").FirstOrDefault() ?? "";
                extTitle = li2.GetMatchingValues("\">(.+?)</a>", "\">", "</a>").FirstOrDefault() ?? "";
            }

            string title = extTitle.Trim().IsNullEmpty() ? (data.GetMatchingValues("coverTitle:\"(.+?)\"", "coverTitle:\"", "\"").FirstOrDefault() ?? "").Trim() : extTitle;
            string img = extImg.Trim().IsNullEmpty() ? data.GetMatchingValues("coverPic:\"(.+?)\"", "coverPic:\"", "\"").FirstOrDefault() ?? "" : extImg;
            string flv = "http://static.video.qq.com/TPout.swf?vid={0}".FormatWith(vid);

            return new VideoInfo() { PicUrl = img, Title = title, Url = flv };
        }
    }
}
