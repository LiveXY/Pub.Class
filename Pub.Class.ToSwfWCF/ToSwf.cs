//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using Pub.Class;
using System.Diagnostics;
using System.ServiceModel;

namespace Pub.Class.ToSwfWCF {
    /// <summary>
    /// 文档视频转换实现
    /// 
    /// 修改纪录
    ///     2010.02.15 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class ToSwfService : IToSwf {
        /// <summary>
        /// 添加到转换列表
        /// </summary>
        /// <param name="filePath">相对文件名</param>
        public void AddToList(ToSwfData data) {
            ToSwfBase.AddToList(data);
        }
    }
}
