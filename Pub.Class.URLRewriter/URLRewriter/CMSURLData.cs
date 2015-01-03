//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Pub.Class;
using System.Web;
using System.Text.RegularExpressions;

namespace Pub.Class {
    /// <summary>
    /// CMSURL数据
    /// 
    /// 修改纪录
    ///     2011.07.13 版本：1.0 livexy 创建此类
    ///     
    /// </summary>
    public class CMSURLData {
        /// <summary>
        /// URL规则
        /// </summary>
        public static RewriterRules Rules = new RewriterRules();
    }
}
