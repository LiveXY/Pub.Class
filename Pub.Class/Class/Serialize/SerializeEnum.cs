//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2011 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Data;

namespace Pub.Class {
    /// <summary>
    /// 序列化Enum
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public enum SerializeEnum {
        /// <summary>
        /// xml
        /// </summary>
        xml,
        /// <summary>
        /// json
        /// </summary>
        json,
        /// <summary>
        /// binary
        /// </summary>
        binary
    }
}
