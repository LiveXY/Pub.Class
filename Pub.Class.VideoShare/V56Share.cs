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
    /// 56.com视频资料
    /// 
    /// 修改纪录
    ///     2011.10.18 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class V56Share : IVideoShare {
        /// <summary>
        /// 取指定URL的视频资料
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>分享视频实体</returns>
        public VideoInfo GetVideoInfo(string url) {
            #region 测试数据
            //http://www.56.com/u75/v_NjM2Nzk1OTI.html
            //http://player.56.com/v_NjM2Nzk1OTI.swf
            //http:\/\/img.v165.56.com\/images\/4\/19\/danny1103i56olo56i56.com_131844413759hd.jpg
            //,"Subject":"\u795e\u540c\u6b65\uff01\u8d85\u725b\u7537\u5973\u534f\u8c03\u624b\u4e2d\u821e\u8e48 ",
            //"Content":"\u8d85\u5f3a\u7684\u624b\u4e2d\u8fd0\u52a8\uff0c\u5728\u6781\u5feb\u7684\u901f\u5ea6\u4e0b\u8fbe\u5230\u60ca\u4eba\u7684\u4e00\u81f4\u3002 ",
            //"user_name":"danny1103 ",
            //"img":"http:\/\/img.v165.56.com\/images\/4\/19\/danny1103i56olo56i56.com_131844413759hd.jpg "
            //http://v.56.com/API/zhuantie.php?site=weibo&id=NjM2Nzk1OTI&pct=1
            #endregion
            if (url.EndsWith(".swf")) {
                string sid = url.GetMatchingValues("com/(.+?).swf", "com/", ".swf").FirstOrDefault() ?? "";
                if (sid.IsNullEmpty()) return null;
                string weibo_data = Net2.GetRemoteHtmlCode4("http://v.56.com/API/zhuantie.php?site=weibo&id={0}&pct=1".FormatWith(sid.Right(sid.Length - 2)), Encoding.UTF8) ?? "";
                string sina = weibo_data.GetMatchingValues("window.location='(.+?)'", "window.location='", "'").FirstOrDefault() ?? "";
                string sina_data = Net2.GetRemoteHtmlCode4(sina, Encoding.UTF8) ?? "";

                string _title = sina_data.GetMatchingValues("<textarea cols=\"20\" rows=\"5\" id=\"fw_content\">(.+?) http://", "<textarea cols=\"20\" rows=\"5\" id=\"fw_content\">", " http://").FirstOrDefault() ?? "";
                string _img = sina_data.GetMatchingValues("<img src=\"(.+?)\" alt=\"\" width=\"120\" height=\"80\" /", "<img src=\"", "\" alt=\"\" width=\"120\" height=\"80\" /").FirstOrDefault() ?? "";
                return new VideoInfo() { PicUrl = _img, Title = _title, Url = url };
            }

            string id = url.GetMatchingValues("/v_(.+?).html", "/v_", ".html").FirstOrDefault() ?? "";
            string data = Net2.GetRemoteHtmlCode4(url, Encoding.UTF8) ?? "";
            string title = (data.GetMatchingValues("\"Subject\":\"(.+?)\"", "\"Subject\":\"", "\"").FirstOrDefault() ?? "").Trim();
            string img = (data.GetMatchingValues("\"img\":\"(.+?)\"", "\"img\":\"", "\"").FirstOrDefault() ?? "").Replace("\\/", "/").Trim();
            string flv = "http://player.56.com/v_{0}.swf".FormatWith(id);
            title = title.Ascii2Native();

            return new VideoInfo() { PicUrl = img, Title = title, Url = flv };
        }
    }
}
