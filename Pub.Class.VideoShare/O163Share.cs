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
    /// open.163.com视频资料
    /// 
    /// 修改纪录
    ///     2011.10.18 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class O163Share : IVideoShare {
        /// <summary>
        /// 取指定URL的视频资料
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>分享视频实体</returns>
        public VideoInfo GetVideoInfo(string url) {
            #region 测试数据
            //http://v.163.com/movie/2011/8/R/S/M7BN25K5V_M7BPHVERS.html
            //http://swf.ws.126.net/movieplayer/-0-2_M7BN25K5V_M7BPHVERS-vimg1_ws_126_net//image/snapshot_movie/2011/8/H/F/M7BPHVDHF-.swf
            //http://swf.ws.126.net/movieplayer/-0-2_M7BN25K5V_M7BPHVERS-.swf
            #endregion
            if (url.EndsWith(".swf")) return null;

            string[] list = url.Split('/');
            string code = list[list.Length - 1];
            code = code.Left(code.Length - 5);
            string data = (Net2.GetRemoteHtmlCode4(url) ?? "").ReplaceRN();
            string on = data.GetMatchingValues("<li class=\"on\">                 	   <a class=\"picLink\"(.+?)<div class=\"nowplay\">正在播放...</div>                 	</li>").FirstOrDefault() ?? "";
            //Msg.WriteEnd(on);
            string title = (data.GetMatchingValues("<title>(.+?)</title>", "<title>", "</title>").FirstOrDefault() ?? "").Trim();
            string img = on.GetMatchingValues("<img height=\"90\" width=\"120\" src=\"(.+?)\" />", "<img height=\"90\" width=\"120\" src=\"", "\" />").FirstOrDefault() ?? "";
            string flv = "http://swf.ws.126.net/movieplayer/-0-2_{0}-.swf".FormatWith(code);
            if (title.IndexOf("_网易") != -1) title = title.Left(title.IndexOf("_网易")).Trim().TrimEnd('_').Trim();

            return new VideoInfo() { PicUrl = img, Title = title, Url = flv };
        }
    }
}
