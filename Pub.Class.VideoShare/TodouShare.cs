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
    /// todou.com视频资料
    /// 
    /// 修改纪录
    ///     2011.10.18 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class TodouShare : IVideoShare {
        /// <summary>
        /// 取指定URL的视频资料
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>分享视频实体</returns>
        public VideoInfo GetVideoInfo(string url) {
            #region 测试数据
            //http://www.tudou.com/playlist/p/l13328532i103653275.html
            //http://www.tudou.com/playlist/p/a67602i103588337.html
            //http://www.tudou.com/playlist/p/l13844631.html
            //http://www.tudou.com/playlist/p/a67554.html
            //http://www.tudou.com/playlist/p/a67615i106685029.html
            //http://www.tudou.com/programs/view/tQZEb2YCQ8M/
            //http://www.tudou.com/programs/view/akg0GtypUC8/
            //http://www.tudou.com/programs/view/T9Jg5KRoMKU/
            //http://www.tudou.com/programs/view/ma3EfG-d3Ys/
            //http://www.tudou.com/programs/view/DiZGd71W2Q4/

            //http://i1.tdimg.com/106/450/394/p.jpg
            //http://www.tudou.com/v/DiZGd71W2Q4/v.swf
            //http://v.t.sina.com.cn/share/share.php?c=spr_web_bd_tudou_weibo
            //&url=http%3A%2F%2Fwww.tudou.com%2Fplaylist%2Fp%2Fl13328532i103653275.html
            //&title=%E3%80%8A%E3%80%8A%E9%85%B1%E8%82%98%E5%AD%90%E3%80%8B%E3%80%90%E5%8C%97%E4%BA%AC%E7%94%B5%E5%BD%B1%E5%AD%A6%E9%99%A2%E5%AF%BC%E6%BC%94%E7%B3%BB%E5%AD%A6%E7%94%9F%E4%BD%9C%E5%93%81%E3%80%91%E3%80%8B++%EF%BC%88%E5%8F%AF%E9%80%89%E5%8E%9F%E7%94%BB%E6%B8%85%E6%99%B0%E5%BA%A6%EF%BC%89
            //&source=%E5%9C%9F%E8%B1%86%E7%BD%91
            //&sourceUrl=http%3A%2F%2Fwww.tudou.com%2F
            //&content=gb2312
            //&pic=http%3A%2F%2Fi2.tdimg.com%2F103%2F653%2F275%2Fp.jpg
            //&appkey=2043051649
            //&ralateUid=1697524603
            #endregion
            string apiUrl = "http://api.tudou.com/v3/gw?method=item.info.get&appKey=7ee7a34f14a4c74b&format=json&itemCodes={0}";
            if (url.EndsWith("/v.swf")) {
                string sid = url.GetMatchingValues("/v/(.+?)/v.swf", "/v/", "/v.swf").FirstOrDefault() ?? "";
                if (sid.IsNullEmpty()) return null;
                url = "http://www.tudou.com/programs/view/{0}/".FormatWith(sid);
            }
            string itemcode = url.GetMatchingValues("/view/(.+?)/", "/view/", "/").FirstOrDefault() ?? "";
            if (itemcode.IsNullEmpty()) return null;

            string data = Net2.GetRemoteHtmlCode4(apiUrl.FormatWith(itemcode), Encoding.UTF8) ?? "";

            string title = data.GetMatchingValues("\"title\":\"(.+?)\"", "\"title\":\"", "\"").FirstOrDefault() ?? "";
            string img = data.GetMatchingValues("\"picUrl\":\"(.+?)\"", "\"picUrl\":\"", "\"").FirstOrDefault() ?? "";
            string flv = "http://www.tudou.com/v/{0}/v.swf".FormatWith(itemcode);

            return new VideoInfo() { PicUrl = img, Title = title, Url = flv };
        }
    }
}
