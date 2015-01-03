//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Linq.Expressions;
#endif
using System.Data.Common;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Pub.Class {
    /// <summary>
    /// 参数
    /// </summary>
    public partial class Parameters {
        /// <summary>
        /// SQL代码
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// 参数列表
        /// </summary>
        public IList<DbParameter> ParameterList { get; set; }
    }
    /// <summary>
    /// 动态生成SQL语句
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.1 livexy 添加允许插入null和允许修改null数据
    ///     2011.10.29 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    ///     string strSql = new SQL()
    ///         .Select("MemberID", "RealName", "CustomerName")
    ///         .Top(1)
    ///         .Distinct()
    ///         .From("UC_Member as a")
    ///         .InnerJoin("EC_Customer as b").On("a.CustomerID", "=", "b.CustomerID")
    ///         .Where("MemberID=1")
    ///         .Where(new Where().And("Account", "admin", Operator.Equal))
    ///         .GroupBy("MemberID", "RealName", "CustomerName")
    ///         .Having("MemberID=1")
    ///         .OrderBy("MemberID")
    ///         .OrderByDescending("RealName")
    ///         .ToString();
    ///     
    ///     string strSql = new SQL()
    ///         .Select(News_Category._CategoryName, News._NewsID)
    ///         .From(News._)
    ///         .InnerJoin(News_Category._).On(News._NCID, "=", News_Category._NCID)
    ///         //.Count(News._NewsID)
    ///         .Where(News._NewsID, "&lt;", "10")
    ///         .Where(new Where().And(News._NewsID, 3, Operator.Equal))
    ///         .Top(1)
    ///         .Distinct()
    ///         .GroupBy(News_Category._CategoryName, News._NewsID)
    ///         .Having(News._NewsID, "&lt;", "10")
    ///         .OrderByDescending(News._NewsID)
    ///         .ToString();
    ///     Msg.Write(Data.GetScalar(strSql).ToString() + "&lt;br />");
    ///     
    ///     string strSql = new SQL()
    ///         .Insert(News_Category._)
    ///         .Value(News_Category._CategoryName, "test")
    ///         .Value(News_Category._ParentID, 0)
    ///         .Value(News_Category._ExtUrl, "http://www.relaxlife.net")
    ///         .Value(News_Category._OrderNum, 0)
    ///         .ToString();
    ///     string strSql = new SQL()
    ///         .Insert(News_Category._, News_Category._CategoryName, News_Category._ParentID, News_Category._ExtUrl, News_Category._OrderNum)
    ///         .Values("test", 1, "http://www.relaxlife.net", 0)
    ///         .ToString();
    ///     string strSql = new SQL()
    ///         .Insert(News_Category._, News_Category._CategoryName, News_Category._ParentID, News_Category._ExtUrl, News_Category._OrderNum)
    ///         .Select(News_Category._CategoryName, News_Category._ParentID, News_Category._ExtUrl, News_Category._OrderNum)
    ///         .From(News_Category._)
    ///         .ToString();
    ///     Msg.Write(Data.ExecSql(strSql).ToString() + "&lt;br />");
    ///     
    ///     string strSql = new SQL()
    ///         .Update(News_Category._)
    ///         .Set("OrderNum=OrderNum+1")
    ///         .Set(News_Category._IsHide, false)
    ///         .Where(News_Category._NCID, "=", "1")
    ///     
    ///     string strSql = new SQL()
    ///         .Delete()
    ///         .From(News_Category._)
    ///         .Where(News_Category._NCID, "=", "1")
    ///         .ToString();
    ///     
    ///     string strSql = new SQL()
    ///         .Delete(News_Category._)
    ///         .Where(News_Category._NCID, "=", "1")
    ///         .ToString();
    ///     string strSql = new SQL()
    ///         .Delete(News_Category._)
    ///         .From(News_Category._)
    ///         .Where(News_Category._NCID, "=", "1")
    ///         .ToString();
    ///     
    ///     Msg.Write(Data.ExecSql(strSql).ToString() + "&lt;br />");
    ///     Msg.WriteEnd(strSql.Replace("\n", "&lt;br />"));
    /// </example>
    /// </code>
    /// </summary>
    public partial class SQL {
        private string _sql = string.Empty;
        /// <summary>
        /// 构造器
        /// </summary>
        public SQL() { }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="sql">sql</param>
        public SQL(string sql) {
            _sql = sql;
        }
        private Dictionary<string, DbParameter> pars = new Dictionary<string, DbParameter>();
        public IList<DbParameter> DbParameter {
            get {
                IList<DbParameter> list = new List<DbParameter>();
                foreach (var info in pars.Keys) list.Add(pars[info]);
                return list;
            }
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SQL AddParameter(string name, object value) {
            string key = db.GetParamIdentifier() + name.Trim('@');
            if (!pars.ContainsKey(key)) pars.Add(key, db.MakeParam(key, value));
            return this;
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SQL AddParameter(string name, DbType DbType, int Size, object value) {
            string key = db.GetParamIdentifier() + name.Trim('@');
            if (!pars.ContainsKey(key)) pars.Add(key, db.MakeInParam(key, DbType, Size, value));
            return this;
        }
        /// <summary>
        /// 输出参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public SQL OutputParameter(string name, DbType DbType, int Size) {
            string key = db.GetParamIdentifier() + name.Trim('@');
            if (!pars.ContainsKey(key)) pars.Add(key, db.MakeOutParam(key, DbType, Size));
            return this;
        }
        //#region Select
        private StringBuilder selectList = new StringBuilder();
        private Database db = Data.Pool();
        /// <summary>
        /// 设置DB
        /// </summary>
        /// <param name="dbkey">dbkey</param>
        /// <returns></returns>
        public SQL Database(string dbkey = "") {
            db = Data.Pool(dbkey);
            return this;
        }
        /// <summary>
        /// 设置DB
        /// </summary>
        /// <param name="dbkey">dbkey</param>
        /// <returns></returns>
        public SQL Database(string[] dbkey) {
            db = Data.Pool(dbkey);
            return this;
        }
        /// <summary>
        /// 查询SQL *
        /// </summary>
        /// <returns>this</returns>
        public SQL Select() { return Select("*"); }
        /// <summary>
        /// 查询SQL
        /// </summary>
        /// <param name="fields">字段列表</param>
        /// <returns>this</returns>
        public SQL Select(string fields) {
            if (fields.IsNullEmpty()) return Select("*");
            fields = fields.Trim(',');
            string[] _fields = fields.Split(',');
            foreach (string field in _fields) selectList.AppendFormat("{0},", field);
            return this;
        }
        /// <summary>
        /// 查询SQL
        /// </summary>
        /// <param name="fields">字段列表</param>
        /// <returns>this</returns>
        public SQL Select(params string[] fields) {
            if (fields.IsNull()) return Select("*");
            foreach (string field in fields) selectList.AppendFormat("{0},", field);
            return this;
        }
        /// <summary>
        /// 取总数
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL Count(string field) {
            if (field.IsNullEmpty()) return Count("*");
            selectList.Clear();
            selectList.AppendFormat("count({0})", field);
            return this;
        }
        /// <summary>
        /// max
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL Max(string field) {
            if (field.IsNullEmpty()) return this;
            //selectList.Clear();
            selectList.AppendFormat("max({0}),", field);
            return this;
        }
        /// <summary>
        /// min
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL Min(string field) {
            if (field.IsNullEmpty()) return this;
            //selectList.Clear();
            selectList.AppendFormat("min({0}),", field);
            return this;
        }
        /// <summary>
        /// sum
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL Sum(string field) {
            if (field.IsNullEmpty()) return this;
            //selectList.Clear();
            selectList.AppendFormat("sum({0}),", field);
            return this;
        }
        /// <summary>
        /// 取总数 字段*
        /// </summary>
        /// <returns>this</returns>
        public SQL Count() { return Count("*"); }
        //#endregion
        //#region Distinct
        private bool distinct = false;
        /// <summary>
        /// 去除重复
        /// </summary>
        /// <returns>this</returns>
        public SQL Distinct() {
            distinct = true;
            return this;
        }
        //#endregion
        //#region Top
        private int top = 0;
        /// <summary>
        /// 只取前N条记录
        /// </summary>
        /// <param name="n">前N条记录</param>
        /// <returns>this</returns>
        public SQL Top(int n) {
            this.top = n;
            return this;
        }
        private string limit = string.Empty;
        /// <summary>
        /// 取M记录之后N条记录
        /// </summary>
        /// <param name="offset">M</param>
        /// <param name="rows">N</param>
        /// <returns>this</returns>
        public SQL Limit(long offset, int rows) {
            limit = offset.ToString() + "," + rows.ToString();
            return this;
        }
        /// <summary>
        /// 只取前N条记录
        /// </summary>
        /// <param name="rows">N</param>
        /// <returns>this</returns>
        public SQL Limit(int rows) {
            limit = rows.ToString();
            return this;
        }
        //#endregion
        //#region From/InnerJoin/LeftJoin/RightJoin
        private StringBuilder fromList = new StringBuilder();
        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="tables">表名列表</param>
        /// <returns>this</returns>
        public SQL From(params string[] tables) {
            if (tables.IsNull()) return this;
            foreach (string table in tables) fromList.AppendFormat("{0},", table);
            return this;
        }
        /// <summary>
        /// 查询表 无锁
        /// </summary>
        /// <param name="tables">表名列表</param>
        /// <returns>this</returns>
        public SQL FromNoLock(params string[] tables) {
            if (tables.IsNull()) return this;
            foreach (string table in tables) fromList.AppendFormat("{0} with(nolock),", table);
            return this;
        }
        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL FromAlias(string table, string alias = "") {
            if (table.IsNull()) return this;
            fromList.AppendFormat("{0}{1},", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 查询表 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL FromAliasNoLock(string table, string alias = "") {
            if (table.IsNull()) return this;
            fromList.AppendFormat("{0}{1} with(nolock),", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 内联查询
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL InnerJoin(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(1);
            fromList.AppendFormat(" inner join {0}{1}", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 内联查询 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL InnerJoinNoLock(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(1);
            fromList.AppendFormat(" inner join {0}{1} with(nolock)", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 左联查询
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL LeftJoin(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" left join {0}{1}", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 左联查询 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL LeftJoinNoLock(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" left join {0}{1} with(nolock)", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 右联查询
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL RightJoin(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" right join {0}{1}", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 右联查询 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL RightJoinNoLock(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" right join {0}{1} with(nolock)", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// full联查询
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL FullJoin(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" full join {0}{1}", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// full联查询 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL FullJoinNoLock(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" full join {0}{1} with(nolock)", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 左外联查询
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL LeftOuterJoin(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" left outer join {0}{1}", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 左外联查询 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL LeftOuterJoinNoLock(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" left outer join {0}{1} with(nolock)", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 右外联查询
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL RightOuterJoin(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" right outer join {0}{1}", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 右外联查询 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL RightOuterJoinNoLock(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" right outer join {0}{1} with(nolock)", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// full外联查询
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL FullOuterJoin(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" full outer join {0}{1}", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// full外联查询 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL FullOuterJoinNoLock(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" full outer join {0}{1} with(nolock)", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 笛卡尔积
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL CrossJoin(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" cross join {0}{1}", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        /// <summary>
        /// 笛卡尔积 无锁
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>this</returns>
        public SQL CrossJoinNoLock(string table, string alias = "") {
            if (fromList.ToString().IsNullEmpty() || table.IsNullEmpty() || fromList.ToString().CharacterCount(',') > 1) return this;
            fromList.RemoveLastChar(',');
            fromList.AppendFormat(" cross join {0}{1} with(nolock)", table, alias.IsNullEmpty() ? "" : " as " + alias);
            return this;
        }
        //#endregion
        //#region On/AndOn/OrOn
        /// <summary>
        /// 联接查询条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL On(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                fromList.AppendFormat(" on {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                fromList.AppendFormat(" on {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        /// <summary>
        /// 联接查询条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL AndOn(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                fromList.AppendFormat(" on {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                fromList.AppendFormat(" and {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        /// <summary>
        /// 联接查询条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL OrOn(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                fromList.AppendFormat(" on {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                fromList.AppendFormat(" or {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        /// <summary>
        /// 联接查询条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>this</returns>
        public SQL On(Where where) {
            if (where.IsNull()) return this;
            Parameters command = where.ToParameters();
            if (command.CommandText.IsNullEmpty()) return this;
            fromList.AppendFormat("{0} ", command.CommandText.TrimStart(3).Trim());
            command.ParameterList.Do(p => {
                string key = p.ParameterName;
                if (!pars.ContainsKey(key)) pars.Add(key, p);
            });
            return this;
        }
        /// <summary>
        /// 联接查询条件
        /// </summary>
        /// <param name="command">条件</param>
        /// <returns>this</returns>
        public SQL On(Parameters command) {
            if (command.CommandText.IsNullEmpty()) return this;

            fromList.AppendFormat("{0} ", command.CommandText);
            command.ParameterList.Do(p => {
                string key = p.ParameterName;
                if (!pars.ContainsKey(key)) pars.Add(key, p);
            });
            return this;
        }
        /// <summary>
        /// 联接查询条件
        /// </summary>
        /// <param name="where">条件 默认1=1</param>
        /// <param name="addon">是否在前面加on 默认不加on</param>
        /// <returns>this</returns>
        public SQL On(string where = "", bool addon = false) {
            if (where.IsNullEmpty()) {
                fromList.AppendFormat("{0}1=1 ".FormatWith(addon ? " on " : ""));
                return this;
            }
            fromList.AppendFormat("{1}{0} ", where, addon ? " on " : "");
            return this;
        }
        //#endregion
        //#region GroupBy
        private StringBuilder groupList = new StringBuilder();
        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL GroupBy(string field) {
            if (field.IsNullEmpty()) return this;
            groupList.AppendFormat("{0},", field);
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="fields">字段数组</param>
        /// <returns>this</returns>
        public SQL GroupBy(params string[] fields) {
            if (fields.IsNull()) return this;
            foreach (string field in fields) groupList.AppendFormat("{0},", field);
            return this;
        }
        private StringBuilder computeList = new StringBuilder();
        /// <summary>
        /// 汇总
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL Compute(string field) {
            if (field.IsNullEmpty()) return this;
            computeList.AppendFormat("{0},", field);
            return this;
        }
        /// <summary>
        /// 汇总
        /// </summary>
        /// <param name="fields">字段数组</param>
        /// <returns>this</returns>
        public SQL Compute(params string[] fields) {
            if (fields.IsNull()) return this;
            foreach (string field in fields) computeList.AppendFormat("{0},", field);
            return this;
        }
        private bool isCube = false;
        /// <summary>
        /// CUBE 生成的结果集显示了所选列中值的所有组合的聚合
        /// </summary>
        /// <returns></returns>
        public SQL WithCube() {
            isCube = true;
            isRollUp = false;
            return this;
        }
        private bool isRollUp = false;
        /// <summary>
        /// ROLLUP 生成的结果集显示了所选列中值的某一层次结构的聚合
        /// </summary>
        /// <returns></returns>
        public SQL WithRollUp() {
            isRollUp = true;
            isCube = false;
            return this;
        }
        private StringBuilder havingWhere = new StringBuilder();
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>this</returns>
        public SQL Having(Where where) {
            if (where.IsNull()) return this;
            Parameters command = where.ToParameters();
            if (command.CommandText.IsNullEmpty()) return this;
            havingWhere.AppendFormat("{0} ", command.CommandText.TrimStart(3).Trim());
            command.ParameterList.Do(p => {
                string key = p.ParameterName;
                if (!pars.ContainsKey(key)) pars.Add(key, p);
            });
            return this;
        }
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="command">条件</param>
        /// <returns>this</returns>
        public SQL Having(Parameters command) {
            if (command.CommandText.IsNullEmpty()) return this;

            havingWhere.AppendFormat("{0} ", command.CommandText.TrimStart(3).Trim());
            command.ParameterList.Do(p => {
                string key = p.ParameterName;
                if (!pars.ContainsKey(key)) pars.Add(key, p);
            });
            return this;
        }
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>this</returns>
        public SQL Having(string where) {
            if (where.IsNullEmpty()) return this;
            havingWhere.AppendFormat("{0} ", where);
            return this;
        }
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL Having(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                havingWhere.AppendFormat(" {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                havingWhere.AppendFormat(" {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL AndHaving(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                havingWhere.AppendFormat(" and {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                havingWhere.AppendFormat(" and {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL OrHaving(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                havingWhere.AppendFormat(" or {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                havingWhere.AppendFormat(" or {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        //#endregion
        //#region OrderBy
        private StringBuilder orderList = new StringBuilder();
        /// <summary>
        /// 正序
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL OrderBy(string field) {
            if (field.IsNullEmpty()) return this;
            orderList.AppendFormat("{0},", field);
            return this;
        }
        /// <summary>
        /// 倒序
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL OrderByDescending(string field) {
            if (field.IsNullEmpty()) return this;
            orderList.AppendFormat("{0} desc,", field);
            return this;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public SQL Order(string orders) {
            if (orders.IsNullEmpty()) return this;
            foreach (string order in orders.Split(',')) orderList.AppendFormat("{0},", order);
            return this;
        }
        //#endregion
        //#region Where
        private StringBuilder whereList = new StringBuilder();
        private IList<DbParameter> pars2 = new List<DbParameter>();
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>this</returns>
        public SQL Where(Where where) {
            if (where.IsNull()) return this;
            Parameters command = where.ToParameters();
            if (command.CommandText.IsNullEmpty()) return this;
            whereList.AppendFormat("{0} ", whereList.IsNullEmpty() ? command.CommandText.TrimStart("and").TrimStart("or").Trim() : command.CommandText);
            command.ParameterList.Do(p => pars2.AddUnique(p));
            return this;
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="command">条件</param>
        /// <returns>this</returns>
        public SQL Where(Parameters command) {
            if (command.CommandText.IsNullEmpty()) return this;
            whereList.AppendFormat("{0} ", whereList.IsNullEmpty() ? command.CommandText.TrimStart("and").TrimStart("or").Trim() : command.CommandText);
            command.ParameterList.Do(p => pars2.AddUnique(p));
            return this;
        }
        /// <summary>
        /// 条件 
        /// </summary>
        /// <param name="where">条件 为空时是1=1</param>
        /// <returns>this</returns>
        public SQL Where(string where = "") {
            if (where.IsNullEmpty()) return this;
            whereList.AppendFormat("{0} ", where.IsNullEmpty() ? "1=1" : where);
            return this;
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL Where(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                whereList.AppendFormat(" {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                whereList.AppendFormat(" {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL AndWhere(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                whereList.AppendFormat(" and {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                whereList.AppendFormat(" and {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="op">操作符</param>
        /// <param name="right">右</param>
        /// <returns>this</returns>
        public SQL OrWhere(string left, string op, string right) {
            if (right.IndexOf("@") == 0) {
                string key = right.Trim('@');
                whereList.AppendFormat(" or {0} {1} {2} ".FormatWith(left, op, db.GetParamIdentifier() + key));
            } else {
                whereList.AppendFormat(" or {0} {1} {2} ".FormatWith(left, op, right));
            }
            return this;
        }
        //#endregion
        //#region Update
        private string updateTable = string.Empty;
        private IList<string> updateKeyEqValue = new List<string>();
        private Dictionary<string, object> updateList = new Dictionary<string, object>();
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns>this</returns>
        public SQL Update(string table) {
            updateTable = table;
            return this;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="keyEqValue">键=值</param>
        /// <returns>this</returns>
        public SQL Set(string keyEqValue) {
            updateKeyEqValue.Add(keyEqValue);
            return this;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="useNULL">是否允许更新为null的数据</param>
        /// <returns>this</returns>
        public SQL Set(string key, object value, bool useNULL) {
            if (value.IsNull() && !useNULL) return this;
            if (key.StartsWith(updateTable + ".")) key = key.Substring(updateTable.Length + 1);
            if (!updateList.ContainsKey(key)) updateList.Add(key, value); else updateList[key] = value;
            return this;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>this</returns>
        public SQL Set(string key, object value) {
            return Set(key, value, false);
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="useNULL">是否允许更新为null的数据</param>
        /// <returns>this</returns>
        public SQL SetP(string key, object value, bool useNULL = false) {
            if (key.StartsWith(updateTable + ".")) key = key.Substring(updateTable.Length + 1);
            return Set("@" + key, value, useNULL);
        }
        //#endregion
        //#region Delete
        private bool isDelete = false;
        private StringBuilder deleteList = new StringBuilder();
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tables">表</param>
        /// <returns>this</returns>
        public SQL Delete(params string[] tables) {
            foreach (string table in tables) deleteList.AppendFormat("{0},", table);
            isDelete = true;
            return this;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns>this</returns>
        public SQL Delete() {
            isDelete = true;
            return this;
        }
        //#endregion
        //#region Insert
        private string insertTable = string.Empty;
        private Dictionary<string, object> insertList = new Dictionary<string, object>();
        private IList<string> insertFields = new List<string>();
        private bool insertUseSelect = true;
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="table">表</param>
        /// <returns>this</returns>
        public SQL Insert(string table) {
            insertTable = table;
            return this;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="table">表</param>
        /// <returns>this</returns>
        public SQL InsertNoLock(string table) {
            insertTable = table + " with(nolock)";
            return this;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="fields">字段</param>
        /// <returns>this</returns>
        public SQL Insert(string table, params string[] fields) {
            insertTable = table;
            //insertFields = fields.ToList();
            foreach (string field in fields) {
                if (field.StartsWith(insertTable + ".")) insertFields.Add(field.Substring(insertTable.Length + 1));
                else insertFields.Add(field);
            }
            return this;
        }
        /// <summary>
        /// 值
        /// </summary>
        /// <param name="values">值</param>
        /// <returns>this</returns>
        public SQL Values(params object[] values) {
            insertUseSelect = false;
            if (insertFields.Count != values.Length) return this;
            int i = 0;
            foreach (string field in insertFields) {
                if (!insertList.ContainsKey(field)) insertList.Add(field, values[i]); else insertList[field] = values[i];
                i++;
            }
            return this;
        }
        /// <summary>
        /// 值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <param name="useNULL">是否插入为null的数据</param>
        /// <returns>this</returns>
        public SQL Value(string field, object value, bool useNULL) {
            if (value.IsNull() && !useNULL) return this;
            insertUseSelect = false;
            if (field.StartsWith(insertTable + ".")) field = field.Substring(insertTable.Length + 1);
            if (!insertList.ContainsKey(field)) insertList.Add(field, value); else insertList[field] = value;
            return this;
        }
        /// <summary>
        /// 值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>this</returns>
        public SQL Value(string field, object value) {
            return Value(field, value, false);
        }
        /// <summary>
        /// 值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>this</returns>
        public SQL ValueP(string field, object value, bool useNULL = false) {
            if (field.StartsWith(insertTable + ".")) field = field.Substring(insertTable.Length + 1);
            return Value("@" + field, value, useNULL);
        }
        //#endregion
        //#region ToString
        /// <summary>
        /// 生成查询SQL
        /// </summary>
        /// <returns>查询SQL代码</returns>
        public string ToSelectString() {
            StringBuilder strSql = new StringBuilder();
            if (selectList.Length > 0) {
                strSql.Append("select ");
                if (distinct) strSql.Append("distinct ");
                if (top > 0) strSql.AppendFormat("top {0} ", top);
                strSql.AppendFormat("{0} ", selectList.ToString().TrimEnd(','));
                if (fromList.Length > 0) strSql.AppendFormat("from {0} ", fromList.ToString().TrimEnd(','));
                if (limit.Length > 0) strSql.AppendFormat("limit {0} ", limit);
                if (whereList.Length > 0) strSql.AppendFormat("where {0} ", whereList.ToString());
                if (groupList.Length > 0) {
                    strSql.AppendFormat("group by {0} ", groupList.ToString().TrimEnd(','));
                    if (havingWhere.Length > 0) strSql.AppendFormat("having {0} ", havingWhere.ToString()); else strSql.Append(" ");
                    if (isCube) strSql.Append("with cube ");
                    if (isRollUp) strSql.Append("with rollup ");
                }
                if (orderList.Length > 0) strSql.AppendFormat("order by {0} ", orderList.ToString().TrimEnd(','));
                if (computeList.Length > 0) strSql.AppendFormat("compute {0} ", groupList.ToString().TrimEnd(','));
                if (pars2.Count > 0) {
                    pars2.Do(p => {
                        string key = p.ParameterName;
                        if (!pars.ContainsKey(key)) pars.Add(key, p);
                    });
                    pars2.Clear();
                }
            }
            if (sqlList.Count > 0) {
                foreach (string key in sqlList.Keys) {
                    sqlList[key].Do(p => {
                        strSql.Append(" ").Append(key).Append(" ").Append(p.ToString());
                        p.DbParameter.Do(d => {
                            string _key = d.ParameterName;
                            if (!pars.ContainsKey(_key)) pars.Add(_key, d);
                        });
                    });
                }
            }
            return strSql.ToString();
        }
        /// <summary>
        /// 生成插入SQL
        /// </summary>
        /// <returns>插入SQL代码</returns>
        public string ToInsertString() {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSql2 = new StringBuilder();
            if (!insertTable.IsNullEmpty()) {
                if (insertUseSelect && insertFields.Count > 0) {
                    strSql.AppendFormat("insert into {0}(", insertTable);
                    insertFields.Do(p => strSql.AppendFormat("{0},", p));
                    strSql.RemoveLastChar(1);
                    strSql.AppendFormat(") {0}", ToSelectString());
                } else if (!insertUseSelect && insertList.Count > 0) {
                    insertList.Do(p => {
                        if (p.Key.IndexOf("@") == 0) {
                            string key = p.Key.Trim('@');
                            strSql.AppendFormat("{0},", db.GetParamIdentifier() + key);
                            string _key = db.GetParamIdentifier() + key;
                            if (!pars.ContainsKey(_key)) pars.Add(_key, db.MakeParam(_key, p.Value.IsNull() ? DBNull.Value : p.Value));
                            strSql2.AppendFormat("{0},", key);
                        } else {
                            strSql.AppendFormat("{0},", p.Value.IsNull() ? "null" : Pub.Class.Where.ValueToStr(p.Value));
                            strSql2.AppendFormat("{0},", p.Key);
                        }
                    });
                    strSql2.Insert(0, "insert into {0}(".FormatWith(insertTable)).RemoveLastChar(1);
                    strSql2.Append(") values(");
                    strSql.Insert(0, strSql2.ToString()).RemoveLastChar(1);
                    strSql.Append(")");
                }
            }
            if (sqlList.Count > 0) {
                foreach (string key in sqlList.Keys) {
                    sqlList[key].Do(p => {
                        strSql.Append(" ").Append(key).Append(" ").Append(p.ToString());
                        p.DbParameter.Do(d => {
                            string _key = d.ParameterName;
                            if (!pars.ContainsKey(_key)) pars.Add(_key, d);
                        });
                    });
                }
            }
            return strSql.ToString();
        }
        /// <summary>
        /// 生成修改SQL
        /// </summary>
        /// <returns>修改SQL代码</returns>
        public string ToUpdateString() {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSql2 = new StringBuilder();
            if (!updateTable.IsNullEmpty()) {
                updateList.Do(p => {
                    if (p.Key.IndexOf("@") == 0) {
                        string key = p.Key.Trim('@');
                        strSql2.AppendFormat("{0}={1},", key, db.GetParamIdentifier() + key);
                        string _key = db.GetParamIdentifier() + key;
                        if (!pars.ContainsKey(_key)) pars.Add(_key, db.MakeParam(_key, p.Value.IsNull() ? DBNull.Value : p.Value));
                    } else {
                        strSql2.AppendFormat("{0}={1},", p.Key, p.Value.IsNull() ? "null" : Pub.Class.Where.ValueToStr(p.Value));
                    }
                });
                updateKeyEqValue.Do(p => strSql2.AppendFormat("{0},", p));
                if (strSql2.Length == 0) return string.Empty;
                strSql2.RemoveLastChar(",");

                strSql.AppendFormat("update {0} set {1} ", updateTable, strSql2.ToString());
                if (fromList.Length > 0) strSql.AppendFormat("from {0} ", fromList.ToString().TrimEnd(',')); else strSql.Append(" ");
                if (whereList.Length > 0) strSql.AppendFormat("where {0} ", whereList.ToString());
                if (pars2.Count > 0) {
                    pars2.Do(p => {
                        string _key = p.ParameterName;
                        if (!pars.ContainsKey(_key)) pars.Add(_key, p);
                    });
                    pars2.Clear();
                }
            }
            if (sqlList.Count > 0) {
                foreach (string key in sqlList.Keys) {
                    sqlList[key].Do(p => {
                        strSql.Append(" ").Append(key).Append(" ").Append(p.ToString());
                        p.DbParameter.Do(d => {
                            string _key = d.ParameterName;
                            if (!pars.ContainsKey(_key)) pars.Add(_key, d);
                        });
                    });
                }
            }
            return strSql.ToString();
        }
        /// <summary>
        /// 生成删除SQL
        /// </summary>
        /// <returns>删除SQL代码</returns>
        public string ToDeleteString() {
            StringBuilder strSql = new StringBuilder();
            if (isDelete) {
                strSql.Append("delete ");
                if (deleteList.Length > 0) {
                    deleteList.RemoveLastChar(",");
                    if (deleteList.ToString().CharacterCount(',') == 0 && fromList.Length == 0) strSql.Append("from ");
                    strSql.AppendFormat("{0} ", deleteList.ToString());
                }
                if (fromList.Length > 0) strSql.AppendFormat("from {0} ", fromList.ToString().TrimEnd(',')); else strSql.Append(" ");
                if (whereList.Length > 0) strSql.AppendFormat("where {0} ", whereList.ToString());
                if (pars2.Count > 0) {
                    pars2.Do(p => {
                        string _key = p.ParameterName;
                        if (!pars.ContainsKey(_key)) pars.Add(_key, p);
                    });
                    pars2.Clear();
                }
            }
            if (sqlList.Count > 0) {
                foreach (string key in sqlList.Keys) {
                    sqlList[key].Do(p => {
                        strSql.Append(" ").Append(key).Append(" ").Append(p.ToString());
                        p.DbParameter.Do(d => {
                            string _key = d.ParameterName;
                            if (!pars.ContainsKey(_key)) pars.Add(_key, d);
                        });
                    });
                }
            }
            return strSql.ToString();
        }
        /// <summary>
        /// 生成查询、插入、修改、删除SQL
        /// </summary>
        /// <returns>查询、插入、修改、删除SQL代码</returns>
        public override string ToString() {
            if (!_sql.IsNullEmpty()) return _sql;
            if (!insertTable.IsNullEmpty()) return ToInsertString();
            if (!updateTable.IsNullEmpty()) return ToUpdateString();
            if (isDelete) return ToDeleteString();
            if (selectList.Length > 0) return ToSelectString();
            return string.Empty;
        }
        /// <summary>
        /// 返回(SQL) as alias
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public string As(string alias) {
            return "({0}) as {1}".FormatWith(ToString(), alias);
        }
        private Dictionary<string, IList<SQL>> sqlList = new Dictionary<string, IList<SQL>>();
        /// <summary>
        /// Union
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public string UnionStr(SQL sql) {
            return "({0}) union ({1})".FormatWith(ToSelectString(), sql.ToSelectString());
        }
        /// <summary>
        /// UnionAll
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public string UnionAllStr(SQL sql) {
            return "({0}) union all ({1})".FormatWith(ToSelectString(), sql.ToSelectString());
        }
        /// <summary>
        /// UnionAll
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public SQL UnionAll(SQL sql) {
            if (sqlList.ContainsKey("union all")) {
                sqlList["union all"].Add(sql);
            } else {
                sqlList.Add("union all", new List<SQL>() { sql });
            }
            return this;
        }
        /// <summary>
        /// Union
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public SQL Union(SQL sql) {
            if (sqlList.ContainsKey("union")) {
                sqlList["union"].Add(sql);
            } else {
                sqlList.Add("union", new List<SQL>() { sql });
            }
            return this;
        }
        /// <summary>
        /// AddSQL
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public SQL AddSQL(SQL sql) {
            if (sqlList.ContainsKey(";")) {
                sqlList[";"].Add(sql);
            } else {
                sqlList.Add(";", new List<SQL>() { sql });
            }
            return this;
        }
        /// <summary>
        /// AddSQL
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public SQL AddSQL(string sql) {
            if (sqlList.ContainsKey(";")) {
                sqlList[";"].Add(sql.ToSQL());
            } else {
                sqlList.Add(";", new List<SQL>() { sql.ToSQL() });
            }
            return this;
        }
        /// <summary>
        /// Union
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public string Union(string sql) {
            return "({0}) union ({1})".FormatWith(ToSelectString(), sql);
        }
        /// <summary>
        /// UnionAll
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public string UnionAll(string sql) {
            return "({0}) union all ({1})".FormatWith(ToSelectString(), sql);
        }
        /// <summary>
        /// 如果存在
        /// </summary>
        /// <param name="trueExec">true 时执行SQL</param>
        /// <param name="falseExec">false 时执行SQL</param>
        /// <returns></returns>
        public string IfExistsStr(SQL trueExec, SQL falseExec = null) {
            return "if exists({0}) {1}{2}".FormatWith(ToString(), trueExec.ToString(), falseExec.IsNull() ? "" : " else " + falseExec.ToString());
        }
        /// <summary>
        /// 如果不存在
        /// </summary>
        /// <param name="trueExec">true 时执行SQL</param>
        /// <param name="falseExec">false 时执行SQL</param>
        /// <returns></returns>
        public string IfNotExistsStr(SQL trueExec, SQL falseExec = null) {
            return "if not exists({0}) {1}{2}".FormatWith(ToString(), trueExec.ToString(), falseExec.IsNull() ? "" : (" else " + falseExec.ToString()));
        }
        /// <summary>
        /// 如果存在
        /// </summary>
        /// <param name="trueExec">true 时执行SQL</param>
        /// <param name="falseExec">false 时执行SQL</param>
        /// <returns></returns>
        public SQL IfExists(SQL trueExec, SQL falseExec = null) {
            string trueSql = trueExec.ToString();
            trueExec.DbParameter.Do(p => {
                string _key = p.ParameterName;
                if (!pars.ContainsKey(_key)) pars.Add(_key, p);
            });
            string falseSql = falseExec.IsNull() ? "" : (" else " + falseExec.ToString());
            if (!falseSql.IsNullEmpty()) falseExec.DbParameter.Do(p => {
                string _key = p.ParameterName;
                if (!pars.ContainsKey(_key)) pars.Add(_key, p);
            });
            _sql = "if exists({0}) {1}{2}".FormatWith(ToString(), trueSql, falseSql);
            return this;
        }
        /// <summary>
        /// 如果不存在
        /// </summary>
        /// <param name="trueExec">true 时执行SQL</param>
        /// <param name="falseExec">false 时执行SQL</param>
        /// <returns></returns>
        public SQL IfNotExists(SQL trueExec, SQL falseExec = null) {
            string trueSql = trueExec.ToString();
            trueExec.DbParameter.Do(p => {
                string _key = p.ParameterName;
                if (!pars.ContainsKey(_key)) pars.Add(_key, p);
            });
            string falseSql = falseExec.IsNull() ? "" : (" else " + falseExec.ToString());
            if (!falseSql.IsNullEmpty()) falseExec.DbParameter.Do(p => {
                string _key = p.ParameterName;
                if (!pars.ContainsKey(_key)) pars.Add(_key, p);
            });
            _sql = "if not exists({0}) {1}{2}".FormatWith(ToString(), trueSql, falseSql);
            return this;
        }
        /// <summary>
        /// 如果存在
        /// </summary>
        /// <param name="trueExec">true 时执行SQL</param>
        /// <param name="falseExec">false 时执行SQL</param>
        /// <returns></returns>
        public SQL IfExistsBeginEnd(SQL trueExec, SQL falseExec = null) {
            string trueSql = trueExec.ToString();
            trueExec.DbParameter.Do(p => {
                string _key = p.ParameterName;
                if (!pars.ContainsKey(_key)) pars.Add(_key, p);
            });
            string falseSql = falseExec.IsNull() ? "" : (" else begin " + falseExec.ToString() + " end");
            if (!falseSql.IsNullEmpty()) falseExec.DbParameter.Do(p => {
                string _key = p.ParameterName;
                if (!pars.ContainsKey(_key)) pars.Add(_key, p);
            });
            _sql = "if exists({0}) begin {1} end {2}".FormatWith(ToString(), trueSql, falseSql);
            return this;
        }
        /// <summary>
        /// 返回数据列表
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public IList<T> ToList<T>(int cacheSeconds = 0, int timeout = 30) where T : class, new() {
            db.Timeout = timeout;
            return ToListByParameters<T>();
            //string sql = ToSelectString();
            //if (cacheSeconds < 1) return db.GetDbDataReader(sql).ToList<T>();
            //return Cache2.Get<IList<T>>(sql, cacheSeconds, () => {
            //    return db.GetDbDataReader(sql).ToList<T>();
            //});
        }
        /// <summary>
        /// 返回实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public T ToEntity<T>(int cacheSeconds = 0, int timeout = 30) where T : class, new() {
            db.Timeout = timeout;
            return ToEntityByParameters<T>(cacheSeconds);
            //string sql = ToSelectString();
            //if (cacheSeconds < 1) return db.GetDbDataReader(sql).ToEntity<T>();
            //return Cache2.Get<T>(sql, cacheSeconds, () => {
            //    return db.GetDbDataReader(sql).ToEntity<T>();
            //});
        }
        /// <summary>
        /// 返回第一行第一列数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public object ToScalar(int cacheSeconds = 0, int timeout = 30) {
            db.Timeout = timeout;
            return ToScalarByParameters(cacheSeconds);
            //string sql = ToSelectString();
            //if (cacheSeconds < 1) {
            //    object val = db.GetScalar(sql);
            //    if (val.IsAllNull()) return default(T);
            //    return (T)val;
            //}
            //return Cache2.Get<T>(sql, cacheSeconds, () => {
            //    object val = db.GetScalar(sql);
            //    if (val.IsAllNull()) return default(T);
            //    return (T)val;
            //});
        }
        /// <summary>
        /// 返回影响行数
        /// </summary>
        /// <returns></returns>
        public int ToExec(int timeout = 30) {
            db.Timeout = timeout;
            return ToExecByParameters();
            //string sql = ToString();
            //return db.ExecSql(sql);
        }
        /// <summary>
        /// 返回事务影响行数
        /// </summary>
        /// <returns></returns>
        public int ToExecTran(int timeout = 30, Action<Exception> ex = null) {
            db.Timeout = timeout;
            string sql = !_sql.IsNullEmpty() ? _sql : ToString();
            string par = ParametersToValues();
            if (par.IsNullEmpty()) {
                return db.ExecTran((tran) => {
                    return db.ExecSql(tran, CommandType.Text, sql);
                }, ex);
            } else {
                return db.ExecTran((tran) => {
                    if (sql.IndexOf(" ") == -1) {
                        return db.ExecSql(tran, CommandType.StoredProcedure, sql, DbParameter.ToArray());
                    } else {
                        return db.ExecSql(tran, CommandType.Text, sql, DbParameter.ToArray());
                    }
                }, ex);
            }
        }
        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public DbDataReader ToDataReader(int cacheSeconds = 0, int timeout = 30) {
            string sql = !_sql.IsNullEmpty() ? _sql : ToSelectString();
            string par = ParametersToValues();
            db.Timeout = timeout;
            if (par.IsNullEmpty()) {
                if (cacheSeconds < 1) return db.GetDbDataReader(sql);
                return Cache2.Get<DbDataReader>(sql, cacheSeconds, () => {
                    return db.GetDbDataReader(sql);
                });
            } else {
                if (cacheSeconds < 1) return db.GetDbDataReader(CommandType.Text, sql, DbParameter.ToArray());
                return Cache2.Get<DbDataReader>(sql + par, cacheSeconds, () => {
                    return db.GetDbDataReader(CommandType.Text, sql, DbParameter.ToArray());
                });
            }
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public DataTable ToDataTable(int cacheSeconds = 0, int timeout = 30) {
            string sql = !_sql.IsNullEmpty() ? _sql : ToSelectString();
            string par = ParametersToValues();
            db.Timeout = timeout;
            if (par.IsNullEmpty()) {
                if (cacheSeconds < 1) return db.GetDataTable(sql);
                return Cache2.Get<DataTable>(sql, cacheSeconds, () => {
                    return db.GetDataTable(sql);
                });
            } else {
                if (cacheSeconds < 1) return db.GetDataTable(CommandType.Text, sql, DbParameter.ToArray());
                return Cache2.Get<DataTable>(sql + par, cacheSeconds, () => {
                    return db.GetDataTable(CommandType.Text, sql, DbParameter.ToArray());
                });
            }
        }
        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public DataSet ToDataSet(int cacheSeconds = 0, int timeout = 30) {
            string sql = !_sql.IsNullEmpty() ? _sql : ToSelectString();
            string par = ParametersToValues();
            db.Timeout = timeout;
            if (par.IsNullEmpty()) {
                if (cacheSeconds < 1) return db.GetDataSet(sql);
                return Cache2.Get<DataSet>(sql, cacheSeconds, () => {
                    return db.GetDataSet(sql);
                });
            } else {
                if (cacheSeconds < 1) return db.GetDataSet(CommandType.Text, sql, DbParameter.ToArray());
                return Cache2.Get<DataSet>(sql + par, cacheSeconds, () => {
                    return db.GetDataSet(CommandType.Text, sql, DbParameter.ToArray());
                });
            }
        }
        /// <summary>
        /// 参数值列表
        /// </summary>
        /// <returns></returns>
        public string ParametersToValues() {
            StringBuilder sb = new StringBuilder();
            foreach (var info in pars) sb.Append(info.Value.ToString()).Append("_");
            return sb.RemoveLast().ToString();
        }
        /// <summary>
        /// 参数名列表
        /// </summary>
        /// <returns></returns>
        public string ParametersToNames() {
            StringBuilder sb = new StringBuilder();
            foreach (var info in pars.Keys) sb.Append(pars[info].ParameterName).Append("_");
            return sb.RemoveLast().ToString();
        }
        /// <summary>
        /// 返回数据列表
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <returns></returns>
        private IList<T> ToListByParameters<T>(int cacheSeconds = 0) where T : class, new() {
            string sql = !_sql.IsNullEmpty() ? _sql : ToSelectString();
            string par = ParametersToValues();
            if (par.IsNullEmpty()) {
                if (cacheSeconds < 1) return db.GetDbDataReader(sql).ToList<T>();
                return Cache2.Get<IList<T>>(sql, cacheSeconds, () => {
                    return db.GetDbDataReader(sql).ToList<T>();
                });
            } else {
                if (cacheSeconds < 1) return db.GetDbDataReader(CommandType.Text, sql, DbParameter.ToArray()).ToList<T>();
                return Cache2.Get<IList<T>>(sql + par, cacheSeconds, () => {
                    return db.GetDbDataReader(CommandType.Text, sql, DbParameter.ToArray()).ToList<T>();
                });
            }
        }
        /// <summary>
        /// 返回实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <returns></returns>
        private T ToEntityByParameters<T>(int cacheSeconds = 0) where T : class, new() {
            string sql = !_sql.IsNullEmpty() ? _sql : ToSelectString();
            string par = ParametersToValues();
            if (par.IsNullEmpty()) {
                if (cacheSeconds < 1) return db.GetDbDataReader(sql).ToEntity<T>();
                return Cache2.Get<T>(sql, cacheSeconds, () => {
                    return db.GetDbDataReader(sql).ToEntity<T>();
                });
            } else {
                if (cacheSeconds < 1) return db.GetDbDataReader(CommandType.Text, sql, DbParameter.ToArray()).ToEntity<T>();
                return Cache2.Get<T>(sql + par, cacheSeconds, () => {
                    return db.GetDbDataReader(CommandType.Text, sql, DbParameter.ToArray()).ToEntity<T>();
                });
            }
        }
        /// <summary>
        /// 返回第一行第一列数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="cacheSeconds">cache时间 秒*5</param>
        /// <returns></returns>
        private object ToScalarByParameters(int cacheSeconds = 0) {
            string sql = !_sql.IsNullEmpty() ? _sql : ToString();
            string par = ParametersToValues();
            //Msg.WriteEnd(sql + "|" + ParametersToNames() + "|" + par + "|" + cacheSeconds);
            if (par.IsNullEmpty()) {
                if (cacheSeconds < 1) return db.GetScalar(sql);
                return Cache2.Get<object>(sql, cacheSeconds, () => {
                    return db.GetScalar(sql);
                });
            } else {
                if (cacheSeconds < 1) return db.GetScalar(CommandType.Text, sql, DbParameter.ToArray());
                return Cache2.Get<object>(sql + par, cacheSeconds, () => {
                    return db.GetScalar(CommandType.Text, sql, DbParameter.ToArray());
                });
            }
        }
        /// <summary>
        /// 返回第一行第一列数据
        /// </summary>
        /// <returns></returns>
        private int ToExecByParameters() {
            string sql = !_sql.IsNullEmpty() ? _sql : ToString();
            string par = ParametersToValues();
            if (par.IsNullEmpty()) {
                return db.ExecSql(sql);
            } else {
                return db.ExecSql(CommandType.Text, sql, DbParameter.ToArray());
            }
        }
        //#endregion
    }
}