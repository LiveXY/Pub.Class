//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif

namespace Pub.Class {
    /// <summary>
    /// 运算符号
    /// 
    /// 修改纪录
    ///     2011.07.10 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public enum Operator {
        /// <summary>
        /// Like 模糊查询
        /// </summary>
        Like,
        /// <summary>
        /// ＝ is equal to 等于号 
        /// </summary>
        Equal,
        /// <summary>
        /// &lt;> (≠) is not equal to 不等于号
        /// </summary>
        NotEqual,
        /// <summary>
        /// ＞ is more than 大于号
        /// </summary>
        MoreThan,
        /// <summary>
        /// ＜ is less than 小于号 
        /// </summary>
        LessThan,
        /// <summary>
        /// ≥ is more than or equal to 大于或等于号 
        /// </summary>
        MoreThanOrEqual,
        /// <summary>
        /// ≤ is less than or equal to 小于或等于号
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 在某个值的中间，拆成两个符号 >= 和 &lt;=
        /// </summary>
        Between,
        /// <summary>
        /// 在某个字符串值中
        /// </summary>
        In,
        /// <summary>
        /// And操作
        /// </summary>
        And,
        /// <summary>
        /// Or操作
        /// </summary>
        Or,
        /// <summary>
        /// 值为字段
        /// </summary>
        Field,
        /// <summary>
        /// is null
        /// </summary>
        Is,
        /// <summary>
        /// is not null
        /// </summary>
        IsNot
    }

    /// <summary>
    /// 查询信息实体类
    /// 
    /// 修改纪录
    ///     2011.07.10 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public partial class SearchInfo {
        /// <summary>
        /// 查询信息实体类
        /// </summary>
        public SearchInfo() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="andor">字段的and/or操作符号</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="type">是否有开始和结束括号</param>
        public SearchInfo(Operator andor, string fieldName, object fieldValue, Operator oper, int type) : this(andor, fieldName, fieldValue, oper, type, false) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="andor">字段的and/or操作符号</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <param name="type">是否有开始和结束括号</param>
        public SearchInfo(Operator andor, string fieldName, object fieldValue, Operator oper, int type, bool where) {
            this.andor = andor;
            this.fieldName = fieldName;
            this.fieldValue = fieldValue;
            this.oper = oper;
            this.where = where;
            this.type = type;
        }
        private Operator andor;
        /// <summary>
        /// 字段的and/or操作符号
        /// </summary>
        public Operator AndOr { get { return andor; } set { andor = value; } }
        private string fieldName;
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get { return fieldName; } set { fieldName = value; } }
        private object fieldValue;
        /// <summary>
        /// 字段的值
        /// </summary>
        public object FieldValue { get { return fieldValue; } set { fieldValue = value; } }
        private Operator oper;
        /// <summary>
        /// 字段的操作符号
        /// </summary>
        public Operator Operator { get { return oper; } set { oper = value; } }
        private bool where = false;
        /// <summary>
        /// 查询条件
        /// </summary>
        public bool Where { get { return where; } set { where = value; } }
        private int type = 0;
        /// <summary>
        /// 类型 0没有开始和结束 1开始 2结束
        /// </summary>
        public int Type { get { return type; } set { type = value; } }
    }

    /// <summary>
    /// Where操作
    /// 
    /// 修改纪录
    ///     2011.07.10 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    ///     string where = new Where()
    ///         .And("UserName", "test1001", Operator.Equal)
    ///         .AndBegin("MemberID", 1000, Operator.LessThan)
    ///         .And("Score", 90, Operator.MoreThan)
    ///         .And("Account", "test%", Operator.Like)
    ///         .AndEnd("Status", "(1,2)", Operator.In)
    ///         .And("CreateTime", "'2011-01-01' and '2011-12-12'", Operator.Between)
    ///         .And("LastLoginTime", "2011-01-01", Operator.MoreThanOrEqual)
    ///         .And("LastLoginTime", "2011-12-12", Operator.LessThanOrEqual)
    ///         .And("UserName", "", Operator.Equal, true)
    ///         .Or("MemberID", 999, Operator.NotEqual)
    ///         .ToSql().TrimStart(3);
    ///     Msg.WriteEnd(where);
    /// </example>
    /// </code>
    /// </summary>
    public partial class Where {
        private IList<SearchInfo> conditions = new List<SearchInfo>();
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where And(string fieldName, object fieldValue, Operator oper) {
            conditions.Add(new SearchInfo(Operator.And, fieldName, fieldValue, oper, 0));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where AndP(string fieldName, object fieldValue, Operator oper) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.And, "@" + fieldName, fieldValue, oper, 0));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where AndBegin(string fieldName, object fieldValue, Operator oper) {
            conditions.Add(new SearchInfo(Operator.And, fieldName, fieldValue, oper, 1));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where AndBeginP(string fieldName, object fieldValue, Operator oper) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.And, "@" + fieldName, fieldValue, oper, 1));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where AndEnd(string fieldName, object fieldValue, Operator oper) {
            conditions.Add(new SearchInfo(Operator.And, fieldName, fieldValue, oper, 2));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where AndEndP(string fieldName, object fieldValue, Operator oper) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.And, "@" + fieldName, fieldValue, oper, 2));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where Or(string fieldName, object fieldValue, Operator oper) {
            conditions.Add(new SearchInfo(Operator.Or, fieldName, fieldValue, oper, 0));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where OrP(string fieldName, object fieldValue, Operator oper) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.Or, "@" + fieldName, fieldValue, oper, 0));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where OrBegin(string fieldName, object fieldValue, Operator oper) {
            conditions.Add(new SearchInfo(Operator.Or, fieldName, fieldValue, oper, 1));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where OrBeginP(string fieldName, object fieldValue, Operator oper) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.Or, "@" + fieldName, fieldValue, oper, 1));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where OrEnd(string fieldName, object fieldValue, Operator oper) {
            conditions.Add(new SearchInfo(Operator.Or, fieldName, fieldValue, oper, 2));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <returns></returns>
        public Where OrEndP(string fieldName, object fieldValue, Operator oper) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.Or, "@" + fieldName, fieldValue, oper, 2));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where And(string fieldName, object fieldValue, Operator oper, bool where) {
            conditions.Add(new SearchInfo(Operator.And, fieldName, fieldValue, oper, 0, where));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where AndP(string fieldName, object fieldValue, Operator oper, bool where) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.And, "@" + fieldName, fieldValue, oper, 0, where));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where AndBegin(string fieldName, object fieldValue, Operator oper, bool where) {
            conditions.Add(new SearchInfo(Operator.And, fieldName, fieldValue, oper, 1, where));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where AndBeginP(string fieldName, object fieldValue, Operator oper, bool where) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.And, "@" + fieldName, fieldValue, oper, 1, where));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where AndEnd(string fieldName, object fieldValue, Operator oper, bool where) {
            conditions.Add(new SearchInfo(Operator.And, fieldName, fieldValue, oper, 2, where));
            return this;
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where AndEndP(string fieldName, object fieldValue, Operator oper, bool where) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.And, "@" + fieldName, fieldValue, oper, 2, where));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where Or(string fieldName, object fieldValue, Operator oper, bool where) {
            conditions.Add(new SearchInfo(Operator.Or, fieldName, fieldValue, oper, 0, where));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where OrP(string fieldName, object fieldValue, Operator oper, bool where) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.Or, "@" + fieldName, fieldValue, oper, 0, where));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where OrBegin(string fieldName, object fieldValue, Operator oper, bool where) {
            conditions.Add(new SearchInfo(Operator.Or, fieldName, fieldValue, oper, 1, where));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where OrBeginP(string fieldName, object fieldValue, Operator oper, bool where) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.Or, "@" + fieldName, fieldValue, oper, 1, where));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where OrEnd(string fieldName, object fieldValue, Operator oper, bool where) {
            conditions.Add(new SearchInfo(Operator.Or, fieldName, fieldValue, oper, 2, where));
            return this;
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="oper">字段的操作符号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Where OrEndP(string fieldName, object fieldValue, Operator oper, bool where) {
            int index = fieldName.IndexOf(".");
            if (index != -1) fieldName = fieldName.Substring(index + 1);
            conditions.Add(new SearchInfo(Operator.Or, "@" + fieldName, fieldValue, oper, 2, where));
            return this;
        }
        /// <summary>
        /// 转换为SQL字符串
        /// </summary>
        /// <returns>SQL字符串</returns>
        public string ToSql() {
            StringBuilder sbSql = new StringBuilder();
            foreach (SearchInfo info in conditions) {
                //如果选择Where为True,并且该字段为空值的话,跳过
                if (info.Where && info.FieldValue.IsNull()) continue;

                if (info.Operator == Operator.Like) {
                    sbSql.AppendFormat("{3}{0}{1}'{2}'{4}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue, OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""), info.Type == 2 ? ")" : "");
                } else if (info.Operator == Operator.In || info.Operator == Operator.Between) {
                    sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                } else if (info.Operator == Operator.Field) {
                    sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                } else if (info.Operator == Operator.Is || info.Operator == Operator.IsNot) {
                    sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                } else {
                    sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), ValueToStr(info.FieldValue) + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                }
            }
            return sbSql.ToString().Trim();
        }
        /// <summary>
        /// 转换为SQL字符串
        /// </summary>
        /// <returns>SQL字符串</returns>
        public Parameters ToParameters(string dbkey = "") {
            Database db = Data.Pool(dbkey);
            StringBuilder sbSql = new StringBuilder();
            IList<DbParameter> pars = new List<DbParameter>();
            Parameters com = new Parameters();

            foreach (SearchInfo info in conditions) {
                //如果选择Where为True,并且该字段为空值的话,跳过
                if (info.Where && info.FieldValue.IsNull()) continue;

                if (info.FieldName.IndexOf("@") == -1) {
                    if (info.Operator == Operator.Like) {
                        sbSql.AppendFormat("{3}{0}{1}'{2}'{4}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue, OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""), info.Type == 2 ? ")" : "");
                    } else if (info.Operator == Operator.In || info.Operator == Operator.Between) {
                        sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                    } else if (info.Operator == Operator.Field) {
                        sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                    } else if (info.Operator == Operator.Is || info.Operator == Operator.IsNot) {
                        sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                    } else {
                        sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), ValueToStr(info.FieldValue) + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                    }
                } else {
                    info.FieldName = info.FieldName.Trim('@');
                    if (info.Operator == Operator.Like) {
                        sbSql.AppendFormat("{3}{0}{1}{2}{4}", info.FieldName, OperatorToStr(info.Operator), db.GetParamIdentifier() + info.FieldName, OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""), info.Type == 2 ? ")" : "");
                        pars.AddUnique(db.MakeParam(db.GetParamIdentifier() + info.FieldName, info.FieldValue));
                    } else if (info.Operator == Operator.In || info.Operator == Operator.Between) {
                        sbSql.AppendFormat("{3}{0}{1}({2})", info.FieldName, OperatorToStr(info.Operator), db.GetParamIdentifier() + info.FieldName + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                        pars.AddUnique(db.MakeParam(db.GetParamIdentifier() + info.FieldName, info.FieldValue));
                    } else if (info.Operator == Operator.Field) {
                        sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                    } else if (info.Operator == Operator.Is || info.Operator == Operator.IsNot) {
                        sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), info.FieldValue + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                    } else {
                        sbSql.AppendFormat("{3}{0}{1}{2}", info.FieldName, OperatorToStr(info.Operator), db.GetParamIdentifier() + info.FieldName + (info.Type == 2 ? ")" : ""), OperatorToStr(info.AndOr) + (info.Type == 1 ? "(" : ""));
                        pars.AddUnique(db.MakeParam(db.GetParamIdentifier() + info.FieldName, info.FieldValue));
                    }
                }
            }
            com.CommandText = sbSql.ToString().Trim();
            com.ParameterList = pars.ToArray();

            sbSql.Clear(); sbSql = null;
            pars.Clear(); pars = null;
            return com;
        }
        /// <summary>
        /// 转换为SQL字符串
        /// </summary>
        /// <returns>SQL字符串</returns>
        public override string ToString() {
            return ToSql();
        }
        /// <summary>
        /// 转换枚举类型为对应的语句操作符号
        /// </summary>
        /// <param name="oper">SqlOperator枚举对象</param>
        /// <returns><![CDATA[对应的Sql语句操作符号（如 ">" "<>" ">=")]]></returns>
        private string OperatorToStr(Operator oper) {
            switch (oper) {
                case Operator.Equal: return " = ";
                case Operator.LessThan: return " < ";
                case Operator.LessThanOrEqual: return " <= ";
                case Operator.Like: return " like ";
                case Operator.MoreThan: return " > ";
                case Operator.MoreThanOrEqual: return " >= ";
                case Operator.NotEqual: return " <> ";
                case Operator.In: return " in ";
                case Operator.Between: return " between ";
                case Operator.And: return " and ";
                case Operator.Or: return " or ";
                case Operator.Is: return " is ";
                case Operator.IsNot: return " is not ";
                default: return "=";
            }
        }
        /// <summary>
        /// 值转换为可用的字符或数字
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns></returns>
        public static string ValueToStr(object value) {
            if (value.IsNull()) return string.Empty;
            switch (value.GetType().ToString()) {
                case "System.Int16": return value.ToString();
                case "System.UInt16": return value.ToString();
                case "System.Single": return value.ToString();
                case "System.UInt32": return value.ToString();
                case "System.Int32": return value.ToString();
                case "System.UInt64": return value.ToString();
                case "System.Int64": return value.ToString();
                case "System.String": return "'{0}'".FormatWith(value.ToString().SafeSql());
                case "System.Double": return value.ToString();
                case "System.Decimal": return value.ToString();
                case "System.Byte": return value.ToString();
                case "System.Boolean": return value.ToString().ToLower() == "true" ? "1" : "0";
                case "System.DateTime": return "'{0}'".FormatWith(value.ToString());
                case "System.Guid": return "'{0}'".FormatWith(value.ToString());
                default: return value.ToString();
            }
        }
    }
}