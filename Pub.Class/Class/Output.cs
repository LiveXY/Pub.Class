using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// 输出消息类 主要用在输出xml json时使用。
    /// </summary>
    public class Output {
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 返回的数据
        /// </summary>
        public object Data { get; set; }
    }
}
