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
    /// Oracle分页  支持oracle
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// PagerSql pagerSql = new OraclePagerSQL().GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email");
    /// </example>
    /// </code>
    /// </summary>
    public class OraclePagerSQL : IPagerSQL {
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
            //SELECT * FROM (
            //SELECT MY_TABLE.*,ROWNUM AS MY_ROWNUM FROM (
            // 括号里写实际的需要查询的SQL语句**/
            //) AS MY_TABLE WHERE ROWNUM <=200/**这里是一页中的最后一条记录**/
            //) WHERE MY_ROWNUM>=10 /**这里是一页中的第一条记录**/

            //select * from t_xiaoxi where rowid in(select rid from (select rownum rn,rid from(select rowid rid,cid from t_xiaoxi  order by cid desc) where rownum<10000) where rn>9980) order by cid desc;
            //select * from (select t.*,row_number() over(order by cid desc) rk from t_xiaoxi t) where rk<10000 and rk>9980;
            //select * from(select t.*,rownum rn from(select * from t_xiaoxi order by cid desc) t where rownum<10000) where rn>9980;
            strSql.Clear();
            strSql.Append("select ");
            strSql.AppendFormat("{0} ", fieldList);

            if (!tableName.IsNullEmpty()) strSql.AppendFormat("from {0} (select {1},rownum as my_rownum from (", tableName, fieldList);
            strSql.Append("select ");
            //if (distinct) strSql.Append("distinct ");
            strSql.AppendFormat("{0} ", fieldList);
            if (!tableName.IsNullEmpty()) strSql.AppendFormat("from {0} ", tableName);
            if (!where.IsNullEmpty()) strSql.AppendFormat("where {0} ", where);
            if (!groupBy.IsNullEmpty()) strSql.AppendFormat("group by {0} ", groupBy);
            if (!orderBy.IsNullEmpty()) strSql.AppendFormat("order by {0} ", orderBy);
            strSql.AppendFormat(") as my_table where rownum<{0}) where my_rownum>{1}", pageSize * (pageIndex - 1) + pageSize, pageSize * (pageIndex - 1));
            if (!orderBy.IsNullEmpty()) strSql.AppendFormat("order by {0} ", orderBy);
            sql.DataSql = strSql.ToString();

            return sql;
        }
    }
}
