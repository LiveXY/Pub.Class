//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Pub.Class {
    /// <summary>
    /// Servers类
    /// 
    /// 修改纪录
    ///     2006.05.10 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Server2 {
        //#region GetMapPath
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath) {
            if (HttpContext.Current.IsNotNull())
                return HttpContext.Current.Server.MapPath(strPath);
            else {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith(".\\")) strPath = strPath.Substring(2);
                strPath = strPath.TrimStart('~').TrimStart('\\');
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        //#endregion
    }
}
