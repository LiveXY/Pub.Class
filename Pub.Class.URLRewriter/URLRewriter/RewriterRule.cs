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

namespace Pub.Class {
    /// <summary>
    /// URL重写规则
    /// 
    /// 修改纪录
    ///     2011.07.09 版本：1.0 livexy 创建此类
    ///     
    /// </summary>
    [Serializable()]
    public class RewriterRule {
        private string match = string.Empty;
        /// <summary>
        /// 查找规则
        /// </summary>
        public string Match { get { return match; } set { match = value; } }
        private string action = string.Empty;
        /// <summary>
        /// 重写规则
        /// </summary>
        public string Action { get { return action; } set { action = value; } }
    }
}
