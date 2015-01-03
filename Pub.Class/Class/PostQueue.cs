//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif

namespace Pub.Class {

    /// <summary>
    /// 不能重复操作实体
    /// 
    /// 修改纪录
    ///     2008.01.23 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    [Serializable]
    [EntityInfo("不能重复操作实体")]
    public class PostQueueInfo {
        /// <summary>
        /// 时间
        /// </summary>
        [EntityInfo("时间")]
        public DateTime Time { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        [EntityInfo("IP")]
        public string IP { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        [EntityInfo("操作")]
        public string Op { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [EntityInfo("内容")]
        public string Content { get; set; }
    }

    /// <summary>
    /// 不能重复操作队列
    /// 
    /// 修改纪录
    ///     2008.01.23 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// PostQueue.Length = 50; //队列最大长度
    /// PostQueue.MaxSecond = 5; //限制多少秒内不能相同操作
    /// PostQueue.MaxPosts = 0; //限制相同操作次数 默认0 0表示不限制
    /// PostQueue.Clear(); //清空队列
    /// PostQueue.RunContent("test"); //同一个IP不能提交相同的内容
    /// PostQueue.RunContent("insert course reply","test"); //课程评论时同一个IP不能提交相同的内容
    /// PostQueue.Run(); //同一个IP在5秒内不能提交相同的重复操作
    /// PostQueue.Run("insert course reply"); //同一个IP在5秒内不能回复课程重复操作
    /// </code>
    /// </example>
    /// </summary>
    public class PostQueue {
        /// <summary>
        /// 队列最大长度 默认50
        /// </summary>
        public static int Length = 50;
        /// <summary>
        /// 限制多少秒内不能相同操作 默认5
        /// </summary>
        public static int MaxSecond = 5;
        /// <summary>
        /// 限制相同操作次数 MaxSecond秒内最多允许提交MaxPosts次 默认0 0表示不限制
        /// </summary>
        public static int MaxPosts = 0;
        /// <summary>
        /// 队列数据
        /// </summary>
        public static IList<PostQueueInfo> list = new List<PostQueueInfo>();
        /// <summary>
        /// 限制用户操作 限制多少秒内不能相同操作
        /// </summary>
        /// <param name="op">操作 不能为空</param>
        public static void Run(string op) {
            if (MaxPosts == 0) return;

            op = op.IsNullEmpty() ? "_sys_op_" : op;
            DateTime now = DateTime.Now;
            string ip = Request2.GetIP();

            int len = list.Where(p => p.Time.GetTimeSpan(now).Seconds < MaxSecond && ip == p.IP && p.Op == op).Count();
            if (len >= MaxPosts) Msg.WriteEnd("限制{0}秒内不能相同操作".FormatWith(MaxSecond));

            list.Add(new PostQueueInfo() { IP = ip, Time = now, Op = op });
            if (list.Count() > Length) list.RemoveAt(0);
        }
        /// <summary>
        /// 限制用户操作 限制多少秒内不能相同操作
        /// </summary>
        public static void Run() { Run(string.Empty); }
        /// <summary>
        /// 限制用户操作 不能提交相同的内容
        /// </summary>
        /// <param name="op">操作 不能为空</param>
        /// <param name="content">内容</param>
        public static void RunContent(string op, string content) {
            op = op.IsNullEmpty() ? "_sys_op_" : op;
            DateTime now = DateTime.Now;
            string ip = Request2.GetIP();

            int len = list.Where(p => ip == p.IP && p.Op == op && p.Content == content).Count();
            if (len > 0) Msg.WriteEnd("不能提交相同的内容");

            list.Add(new PostQueueInfo() { IP = ip, Time = now, Op = op, Content = content });
            if (list.Count() > Length) list.RemoveAt(0);
        }
        /// <summary>
        /// 限制用户操作 不能提交相同的内容
        /// </summary>
        /// <param name="content">内容</param>
        public static void RunContent(string content) {
            RunContent(string.Empty, content);
        }
        /// <summary>
        /// 清空队列
        /// </summary>
        public static void Clear() { list.Clear(); }
    }
}


