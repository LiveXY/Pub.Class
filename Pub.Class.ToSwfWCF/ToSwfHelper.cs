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
    public class ToSwfHelper {
        /// <summary>
        /// 添加到转换列表
        /// </summary>
        /// <param name="ToSwfData">实体</param>
        public static void AddToList(ToSwfData data) {
            using (ChannelFactory<IToSwf> channel = new ChannelFactory<IToSwf>("Pub.Class.ToSwfWCF.ToSwfService")) { 
                IToSwf toSwf = channel.CreateChannel();
                using (toSwf as IDisposable) {
                    toSwf.AddToList(data);
                }
            }
        }
    }
}
