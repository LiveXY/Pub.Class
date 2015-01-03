//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;

namespace Pub.Class {
    /// <summary>
    /// 实体类描述
    /// 
    /// 修改纪录
    ///     2011.05.01 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class EntityInfo : Attribute {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 详细描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        public EntityInfo(string name) {
            this.Name = name;
            this.Description = string.Empty;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="desc">详细描述</param>
        public EntityInfo(string name, string desc) {
            this.Name = name;
            this.Description = desc;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">名称</param>
        /// <param name="desc">详细描述</param>
        public EntityInfo(string id = "", string name = "", string desc = "") {
            this.ID = id;
            this.Name = name;
            this.Description = desc;
        }
    }
}
