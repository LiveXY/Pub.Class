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
    /// 分页SQL接口
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface IPagerSQL {
        /// <summary>
        /// 分页SQL调用方法
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="tableName">表名称</param>
        /// <param name="pk">主键</param>
        /// <param name="fieldList">字段列表</param>
        /// <param name="where">where条件 and or 开始</param>
        /// <param name="groupBy">分组条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns>分页SQL</returns>
        PagerSql GetSQL(int pageIndex, int pageSize, string tableName, string pk = "*", string fieldList = "*", string where = "", string groupBy = "", string orderBy = "");
    }

    /// <summary>
    /// 分页SQL实体类
    /// </summary>
    [Serializable]
    [EntityInfo("分页SQL实体类")]
    public class PagerSql {
        /// <summary>
        /// 统计记录数SQL
        /// </summary>
        [EntityInfo("统计记录数SQL")]
        public string CountSql { set; get; }
        /// <summary>
        /// 取数据SQL
        /// </summary>
        [EntityInfo("取数据SQL")]
        public string DataSql { set; get; }
    }
}
