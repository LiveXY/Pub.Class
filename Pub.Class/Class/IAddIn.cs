//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;

namespace Pub.Class {
    /// <summary>
    /// 插件接口
    /// 
    /// 修改纪录
    ///     2011.07.01 版本：1.0 livexy 创建此接口
    /// 
    /// </summary>
    public interface IAddIn {

    }
    /// <summary>
    /// 插件接口
    /// 
    /// 修改纪录
    ///     2012.01.10 版本：1.0 livexy 创建此接口
    /// 
    /// </summary>
    public interface IPlugin : IAddIn {
        /// <summary>
        /// 执行入口
        /// </summary>
        /// <param name="args">参数</param>
        void Main(params string[] args);
    }
}
