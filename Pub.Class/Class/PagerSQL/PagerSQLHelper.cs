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
    /// 分页SQL操作类
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// using (PagerSQLHelper s = new PagerSQLHelper(PagerSQLEnum.top_top)) {
    ///     PagerSql sql = s.GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email", "", "", "");
    ///     Msg.Write(sql.CountSql + "&lt;br />" + sql.DataSql);
    /// }
    /// using (PagerSQLHelper s = new PagerSQLHelper(PagerSQLEnum.not_in)) {
    ///     PagerSql sql = s.GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email", "", "", "");
    ///     Msg.Write(sql.CountSql + "&lt;br />" + sql.DataSql);
    /// }
    /// using (PagerSQLHelper s = new PagerSQLHelper(PagerSQLEnum.max_top)) {
    ///     PagerSql sql = s.GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email", "", "", "");
    ///     Msg.Write(sql.CountSql + "&lt;br />" + sql.DataSql);
    /// }
    /// using (PagerSQLHelper s = new PagerSQLHelper(PagerSQLEnum.row_number)) {
    ///     PagerSql sql = s.GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email", "", "", "");
    ///     Msg.Write(sql.CountSql + "&lt;br />" + sql.DataSql);
    /// }
    /// using (PagerSQLHelper s = new PagerSQLHelper(PagerSQLEnum.mysql)) {
    ///     PagerSql sql = s.GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email", "", "", "");
    ///     Msg.Write(sql.CountSql + "&lt;br />" + sql.DataSql);
    /// }
    /// using (PagerSQLHelper s = new PagerSQLHelper(PagerSQLEnum.oracle)) {
    ///     PagerSql sql = s.GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email", "", "", "");
    ///     Msg.Write(sql.CountSql + "&lt;br />" + sql.DataSql);
    /// }
    /// using (PagerSQLHelper s = new PagerSQLHelper(PagerSQLEnum.sqlite)) {
    ///     PagerSql sql = s.GetSQL(2, 10, "UC_Member", "MemberID", "MemberID,RealName,Email", "", "", "");
    ///     Msg.Write(sql.CountSql + "&lt;br />" + sql.DataSql);
    /// }
    /// </example>
    /// </code>
    /// </summary>
    public class PagerSQLHelper : Disposable {
        private PagerSQLEnum PagerSQLEnum;
        private IPagerSQL PagerSQL = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="PagerSQLEnum">PagerSQL 调用类型 Enum string</param>
        public PagerSQLHelper(string PagerSQLEnum) {
            this.PagerSQLEnum = PagerSQLEnum.ToEnum<PagerSQLEnum>();
            init();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="PagerSQLEnum">PagerSQL 调用类型 Enum</param>
        public PagerSQLHelper(PagerSQLEnum PagerSQLEnum) {
            this.PagerSQLEnum = PagerSQLEnum;
            init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void init() { 
            switch (this.PagerSQLEnum) {
                case PagerSQLEnum.not_in: this.PagerSQL = new NotInPagerSQL(); break;
                case PagerSQLEnum.max_top: this.PagerSQL = new MaxTopPagerSQL(); break;
                case PagerSQLEnum.top_top: this.PagerSQL = new TopTopPagerSQL(); break;
                case PagerSQLEnum.row_number: this.PagerSQL = new RowNumberPagerSQL(); break;
                case PagerSQLEnum.mysql: this.PagerSQL = new MySqlPagerSQL(); break;
                case PagerSQLEnum.sqlite: this.PagerSQL = new SqlitePagerSQL(); break;
                case PagerSQLEnum.oracle: this.PagerSQL = new OraclePagerSQL(); break;
                default: this.PagerSQL = new MaxTopPagerSQL(); break;
            }
        }
        /// <summary>
        /// 用using 自动释放
        /// </summary>
        protected override void InternalDispose() {
            PagerSQL = null;
            base.InternalDispose();
        }
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
            if (pk.IsNullEmpty() && !fieldList.IsNullEmpty() && fieldList.IndexOf(",") > 0) pk = fieldList.Split(',')[0];
            if (pk.IsNullEmpty() && !fieldList.Equals("*") && fieldList.IndexOf(",") == -1) pk = fieldList;
            if (fieldList.IsNullEmpty()) fieldList = "*";
            if (pk.IsNullEmpty()) pk = "*";
            pk = pk.Trim(','); fieldList = fieldList.Trim(',');
            if (orderBy.IsNullEmpty() && !pk.IsNullEmpty() && !pk.Equals("*")) orderBy = pk + " desc";
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            if (!where.IsNullEmpty()) where = "1=1 " + where;

            return PagerSQL.GetSQL(pageIndex, pageSize, tableName, pk, fieldList, where, groupBy, orderBy);
        }
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
        public PagerSql GetSQL(int pageIndex, int pageSize, string tableName, string pk = "*", string fieldList = "*", Where where = null, string groupBy = "", string orderBy = "") {
            if (pk.IsNullEmpty() && !fieldList.IsNullEmpty() && fieldList.IndexOf(",") > 0) pk = fieldList.Split(',')[0];
            if (pk.IsNullEmpty() && !fieldList.Equals("*") && fieldList.IndexOf(",") == -1) pk = fieldList;
            if (fieldList.IsNullEmpty()) fieldList = "*";
            if (pk.IsNullEmpty()) pk = "*";
            pk = pk.Trim(','); fieldList = fieldList.Trim(',');
            if (orderBy.IsNullEmpty() && !pk.IsNullEmpty() && !pk.Equals("*")) orderBy = pk + " desc";
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            string _where = "";
            if (!where.IsNull()) _where = "1=1 " + where.ToString();
            return PagerSQL.GetSQL(pageIndex, pageSize, tableName, pk, fieldList, _where, groupBy, orderBy);
        }
    }
}
