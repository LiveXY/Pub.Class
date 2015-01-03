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
    /// 使用Top Top 分页  支持ACCESS MSSQL2000+  只允许主键排序
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// PagerSql pagerSql = new TopTopPagerSQL().GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email");
    /// </example>
    /// </code>
    /// </summary>
    public class TopTopPagerSQL : IPagerSQL {
        /// <summary>
        /// 分页SQL调用方法 只允许主键排序
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
            //select * from Student
            //where Id in (
            //select top 10 Id
            //from(select top 3200010 Id from Student order by Id)t
            //order by Id desc)
            //order by Id

            //SELECT * FROM (
            //SELECT TOP 页面容量 * FROM (
            //SELECT TOP 页面容量*当前页码 * FROM
            //表 WHERE 条件 ORDER BY 字段A ASC
            //)AS  TEMPTABLE1 ORDER BY 字段A DESC
            //) AS TEMPTABLE2 ORDER BY 字段A ASC
            StringBuilder orderByExt = new StringBuilder();
            foreach (string order in orderBy.Split(',')) {
                string order2 = order.Trim();
                if (order2.EndsWith(" desc", true, null)) orderByExt.AppendFormat("{0} {1},", order2.Left(order2.Length - 5), "asc");
                else orderByExt.AppendFormat("{0} {1},", order2.EndsWith(" asc", true, null) ? order2.Left(order2.Length - 4) : order2, "desc");
            }
            orderByExt.RemoveLastChar(",");

            strSql.Clear();
            strSql.Append("select ");
            //if (distinct) strSql.Append("distinct ");
            if (pageSize == 1) {
                strSql.AppendFormat("top {0} ", pageSize);
                strSql.AppendFormat("{0} ", fieldList);
                if (!tableName.IsNullEmpty()) strSql.AppendFormat("from {0} ", tableName);
                if (!where.IsNullEmpty()) strSql.AppendFormat("where {0} ", where);
                if (!groupBy.IsNullEmpty()) strSql.AppendFormat("group by {0} ", groupBy);
                if (!orderBy.IsNullEmpty()) strSql.AppendFormat("order by {0} ", orderBy);
            } else {
                if (!tableName.IsNullEmpty()) strSql.AppendFormat("{1} from (select top {0} {1} from (select top {3} {1} from {2} ", pageSize, fieldList, tableName, pageSize * pageIndex);
                if (!where.IsNullEmpty()) strSql.AppendFormat("where {0} ", where);
                if (!groupBy.IsNullEmpty()) strSql.AppendFormat("group by {0} ", groupBy);
                if (!orderBy.IsNullEmpty()) strSql.AppendFormat("order by {0} ", orderBy);
                strSql.AppendFormat(") as Top1 {0}) as Top2 {1} ", "order by " + orderByExt, "order by " + orderBy);
            }
            sql.DataSql = strSql.ToString();

            return sql;
        }
    }
}
