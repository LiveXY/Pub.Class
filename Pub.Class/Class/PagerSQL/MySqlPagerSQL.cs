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
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// MySql分页  支持mysql
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// PagerSql pagerSql = new MySqlPagerSQL().GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email");
    /// </example>
    /// </code>
    /// </summary>
    public class MySqlPagerSQL : IPagerSQL {
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
        public PagerSql GetSQL(int pageIndex, int pageSize, string tableName, string pk = "*", string fieldList = "*", string where = "", string groupBy = "", string orderBy = "") {
            PagerSql sql = new PagerSql();
            StringBuilder strSql = new StringBuilder();

            strSql.Append("select ");
            strSql.AppendFormat("count({0}) as total ", pk);
            if (!tableName.IsNullEmpty()) strSql.AppendFormat("from {0} ", tableName);
            if (!where.IsNullEmpty()) strSql.AppendFormat("where {0} ", where);
            if (!groupBy.IsNullEmpty()) strSql.AppendFormat("group by {0} ", groupBy);
            sql.CountSql = strSql.ToString();

            strSql.Clear();
            strSql.Append("select ");
            //if (distinct) strSql.Append("distinct ");
            strSql.AppendFormat("{0} ", fieldList);
            if (!tableName.IsNullEmpty()) strSql.AppendFormat("from {0} ", tableName);
            if (!where.IsNullEmpty()) strSql.AppendFormat("where {0} ", where);
            if (!groupBy.IsNullEmpty()) strSql.AppendFormat("group by {0} ", groupBy);
            if (!orderBy.IsNullEmpty()) strSql.AppendFormat("order by {0} ", orderBy);
            strSql.AppendFormat("limit {0},{1} ", (pageIndex - 1) * pageSize, pageSize);
            sql.DataSql = strSql.ToString();

            return sql;
        }
    }
}
