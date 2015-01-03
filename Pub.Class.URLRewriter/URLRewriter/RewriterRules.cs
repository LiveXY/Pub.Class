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
using System.Collections;

namespace Pub.Class {
    /// <summary>
    /// URL重写规则集合
    /// 
    /// 修改纪录
    ///     2011.07.09 版本：1.0 livexy 创建此类
    ///     
    /// </summary>
    [Serializable()]
    public class RewriterRules : CollectionBase {
        /// <summary>
        /// 向集合中添加新规则
        /// </summary>
        /// <param name="r"></param>
        public virtual void Add(RewriterRule r) { this.InnerList.Add(r); }
        /// <summary>
        /// 获取或设置项
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RewriterRule this[int index] { get { return (RewriterRule)this.InnerList[index]; } set { this.InnerList[index] = value; } }
    }
}
