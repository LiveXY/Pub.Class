//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// 生成分页SQL代码
    /// 
    /// 修改纪录
    ///     2006.05.03 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class GetPager {
        //#region 私有成员
        private string _tblName;
        private string _fldKey;
        private string _fldFields;
        private int _PageSize;
        private int _PageIndex;
        private string _strWhere;
        private string _strOrder;
        private int _OrderType = -1;
        private int _doCount;
        private int _State = 0;
        //#endregion
        //#region 属性
        /// <summary>
        /// 表名
        /// </summary>
        public string tblName { set { _tblName = value; } }
        /// <summary>
        /// 主键
        /// </summary>
        public string fldKey { set { _fldKey = value; } }
        /// <summary>
        /// 字段名
        /// </summary>
        public string fldFields { set { _fldFields = value; } }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { set { _PageSize = value; } }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { set { _PageIndex = value; } }
        /// <summary>
        /// 条件
        /// </summary>
        public string strWhere { set { _strWhere = value; } }
        /// <summary>
        /// 排序字段 注意：如果使用OrderType时 strOrder只能使用一个字段排序
        /// </summary>
        public string strOrder { set { _strOrder = value; } }
        /// <summary>
        /// 排序类型 注意：如果strOrder支持多个字段排序时 不能使用OrderType属性
        /// </summary>
        public int OrderType { set { _OrderType = value; } }
        /// <summary>
        /// 不等0时是否输出记录数 
        /// </summary>
        public int doCount { set { _doCount = value; } }
        /// <summary>
        /// 返回操作状态
        /// </summary>
        public int State { get { return _State; } }
        //#endregion
        //#region 构造器
        /// <summary>
        /// 构造器
        /// </summary>
        public GetPager() {
            this._tblName = "";
            this._fldKey = "";
            this._fldFields = "*";
            this._PageSize = 10;
            this._PageIndex = 1;
            this._strWhere = "";
            this._strOrder = "";
            this._doCount = 0;
            this._OrderType = -1;
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="fldKey">主键</param>
        /// <param name="fldFields">字段名</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrder">排序</param>
        /// <param name="orderType">1ASC/2DESC排序类型</param>
        /// <param name="doCount">不等0时是否输出记录数 </param>
        public GetPager(string tblName, string fldKey, string fldFields, int PageSize, int PageIndex, string strWhere, string strOrder, int orderType, int doCount) {
            this._tblName = tblName;
            this._fldKey = fldKey;
            this._fldFields = fldFields;
            this._PageSize = PageSize;
            this._PageIndex = PageIndex;
            this._strWhere = strWhere;
            this._strOrder = strOrder;
            this._OrderType = orderType;
            this._doCount = doCount;
        }
        //#endregion
        //#region 生成SQL分页字符串
        /// <summary>
        /// 生成SQL字符串 
        /// 注意：如果使用OrderType!=-1时 strOrder只能使用一个字段排序,并且调用strSqlSingleOrder方法
        ///       否则调用strSqlUseNotIn方法
        /// </summary>
        /// <returns>生成SQL字符串</returns>
        public string StrSql() {
            if (_OrderType == -1) return StrSqlUseNotIn(); else return StrSqlSingleOrder();
        }
        /// <summary>
        /// 使用not in
        /// </summary>
        /// <returns>使用not in SQL分页字符串</returns>
        public string StrSqlUseNotIn() {
            string _strSQL = "";

            if (_doCount != 0) {
                if (_strWhere != "")
                    _strSQL = "select count(" + this._fldKey + ") as Total from " + _tblName + " where " + _strWhere;
                else
                    _strSQL = "select count(" + this._fldKey + ") as Total from " + _tblName;
            } else {
                if (_strOrder != "") _strOrder = " order by " + _strOrder + " ";
                if (_PageIndex == 1) {
                    if (_strWhere != "")
                        _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + " from " + _tblName + " where " + _strWhere + _strOrder;
                    else
                        _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + " from " + _tblName + " " + _strOrder;
                } else {
                    if (_strWhere != "")
                        _strSQL = "select top " + _PageSize + " " + _fldFields + " from " + _tblName + " where ( " + _fldKey + " not in (select top " + (_PageSize * (_PageIndex - 1)) + " " +
                            _fldKey + " from " + _tblName + " where " + _strWhere + _strOrder + ")) and (" + _strWhere + ") " + _strOrder;
                    else
                        _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + " from " + _tblName + " where ( " + _fldKey + " not in (select top " + (_PageSize * (_PageIndex - 1)) + " " +
                            _fldKey + " from " + _tblName + " " + _strOrder + ")) " + _strOrder;
                }
            }
            return _strSQL;
        }
        /// <summary>
        /// 只支持单个字段排序
        /// </summary>
        /// <returns>SQL分页字符串</returns>
        public string StrSqlSingleOrder() {
            string _strSQL = "", _strTmp = "", __strOrder, ___strOrder;

            if (_doCount != 0) {
                if (_strWhere != "")
                    _strSQL = "select count(" + this._fldKey + ") as Total from " + _tblName + " where " + _strWhere;
                else
                    _strSQL = "select count(" + this._fldKey + ") as Total from " + _tblName;
            } else {
                ___strOrder = _fldKey;
                if (_fldKey.Split('.').Length > 1) ___strOrder = _fldKey.Split('.')[1];
                if (_OrderType != 0) {
                    _strTmp = "<(select min";
                    __strOrder = " order by " + _strOrder + " desc";
                } else {
                    _strTmp = ">(select max";
                    __strOrder = " order by " + _strOrder + " asc";
                }
                if (_PageIndex == 1) {
                    if (_strWhere != "")
                        _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + "  from " + _tblName + " where " +
                            _strWhere + " " + __strOrder;
                    else
                        _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + "  from " + _tblName + " " + __strOrder;
                } else {
                    _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + "  from " + _tblName +
                        " where " + _fldKey + _strTmp + "(" + ___strOrder + ") from (select top " +
                        ((_PageIndex - 1) * _PageSize) + " " + _fldKey + " from " + _tblName + " " + __strOrder + ") as tblTmp) " + __strOrder;
                    if (_strWhere != "")
                        _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + "  from " + _tblName +
                            " where " + _fldKey + _strTmp + "(" + ___strOrder + ") from (select top " +
                            ((_PageIndex - 1) * _PageSize) + " " + _fldKey + " from " + _tblName + " where " + _strWhere + " " +
                            __strOrder + ") as tblTmp) and " + _strWhere + " " + __strOrder;
                }
            }
            return _strSQL;
        }
        /// <summary>
        /// 没使用not in
        /// </summary>
        /// <returns>SQL分页字符串</returns>
        public string StrSqlNoUseNotIn() {
            string _strSQL = "", __strOrder1 = "", __strOrder2 = "";

            if (_doCount != 0) {
                if (_strWhere != "")
                    _strSQL = "select count(" + this._fldKey + ") as Total from " + _tblName + " where " + _strWhere;
                else
                    _strSQL = "select count(" + this._fldKey + ") as Total from " + _tblName;
            } else {
                if (_strOrder != "") {
                    string[] OrderArr = _strOrder.Split(',');
                    for (int i = 0; i <= OrderArr.Length - 1; i++) {
                        string[] strOrderArr1 = OrderArr[i].Trim().Split(' ');
                        string[] strOrderArr2 = OrderArr[i].Trim().Split('.');
                        string __strOrder3 = (strOrderArr1.Length == 1) ? strOrderArr1[0] + " desc" : strOrderArr1[0] + (strOrderArr1[strOrderArr1.Length - 1] == "desc" ? " asc" : " desc");
                        string[] strOrderArr3 = __strOrder3.Trim().Split('.');
                        __strOrder1 = (strOrderArr3.Length == 1) ? __strOrder1 + strOrderArr3[0] : __strOrder1 + strOrderArr3[1] + ",";
                        __strOrder2 = (strOrderArr2.Length == 1) ? __strOrder2 + strOrderArr2[0] : __strOrder2 + strOrderArr2[1] + ",";
                    }
                    __strOrder1 = " order by " + __strOrder1.TrimEnd(',');
                    __strOrder2 = " order by " + __strOrder2.TrimEnd(',');
                    _strOrder = " order by " + _strOrder;
                }
                if (_PageIndex == 1) {
                    if (_strWhere != "")
                        _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + " from " + _tblName + " where " + _strWhere + _strOrder;
                    else
                        _strSQL = "select top " + _PageSize.ToString() + " " + _fldFields + " from " + _tblName + " " + _strOrder;
                } else {
                    if (_strWhere != "") _strWhere = " where " + _strWhere;
                    _strSQL = "select * from ( " +
                                "	SELECT TOP " + _PageSize.ToString() + " * from ( " +
                                "		select top " + (_PageSize * _PageIndex) + " " + _fldFields + " " +
                                "			from " + _tblName + " " +
                                "			" + _strWhere + " " +
                                "			" + _strOrder + "  " +
                                "	) as _a " + __strOrder1 + " " +
                                ") as _b " + __strOrder2 + " ";
                }
            }
            return _strSQL;
        }
        /// <summary>
        /// MySql分页
        /// </summary>
        /// <returns>SQL分页字符串</returns>
        public string StrMySql() {
            string _strSQL = "";

            if (_doCount != 0) {
                if (_strWhere != "")
                    _strSQL = "select count(" + this._fldKey + ") as Total from " + _tblName + " where " + _strWhere;
                else
                    _strSQL = "select count(" + this._fldKey + ") as Total from " + _tblName;
            } else {
                if (_strOrder != "") _strOrder = " order by " + _strOrder + " ";
                if (_PageIndex == 1) {
                    if (_strWhere != "")
                        _strSQL = "select " + _fldFields + " from " + _tblName + " where " + _strWhere + _strOrder + " limit 0," + _PageSize.ToString();
                    else
                        _strSQL = "select " + _fldFields + " from " + _tblName + " " + _strOrder + " limit 0," + _PageSize.ToString();
                } else {
                    if (_strWhere != "")
                        _strSQL = "select " + _fldFields + " from " + _tblName + " where " + _strWhere + _strOrder + " limit " + ((_PageIndex - 1) * _PageSize) + "," + _PageSize.ToString();
                    else
                        _strSQL = "select " + _fldFields + " from " + _tblName + " " + _strOrder + " limit " + ((_PageIndex - 1) * _PageSize) + "," + _PageSize.ToString();
                }
            }
            return _strSQL;
        }
        //#endregion
    }
}
