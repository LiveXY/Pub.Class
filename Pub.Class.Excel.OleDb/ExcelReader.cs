//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

namespace Pub.Class.Excel.OleDb {
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    /// <summary>
    /// OleDb读Excel11
    /// 
    /// 修改纪录
    ///     2011.02.15 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class ExcelReader : IExcelReader {
        private DataSet ds = new DataSet();
        /// <summary>
        /// 打开excel文件
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        public void Open(string excelPath) {
            string connStr = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelPath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";

            OleDbConnection conn = new OleDbConnection(connStr); 
            conn.Open();
            DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            conn.Close(); conn.Dispose(); conn = null;

            Data.ResetDbProvider();
            Data.DBType = "OleDb";
            Data.ConnString = connStr;

            foreach (DataRow row in dt.Rows) {
                string name = row["TABLE_NAME"].ToString();
                dt = Data.GetDataTable("select * from [{0}]".FormatWith(name));
                name = name.Trim('\'').Trim('$');
                if (ds.Tables.IndexOf(name) == -1) {
                    ds.Tables.Add(dt);
                    dt.TableName = name;
                }
            }
        }
        /// <summary>
        /// excel转DataSet
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet ToDataSet() {
            return ds;
        }
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="table">DataTable名称</param>
        /// <returns>DataTable</returns>
        public DataTable ToDataTable(string table) {
            int i = -1, index = 0;
            foreach (DataTable dt in ds.Tables) {
                if (dt.TableName.TrimEnd('$').ToLower().Equals(table.TrimEnd('$').ToLower())) { i = index; break; }
                index++;
            }
            return i == -1 ? null : ToDataTable(i);
        }
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>DataTable</returns>
        public DataTable ToDataTable(int i) {
            int count = ds.Tables.Count;
            return i < count ? ds.Tables[i] : null;
        }
        /// <summary>
        /// excel转DataTable 第0个
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable ToDataTable() {
            return ToDataTable(0);
        }
        ///// <summary>
        ///// 取table工作表的row,column位置的数据
        ///// </summary>
        ///// <param name="table">工作表名</param>
        ///// <param name="row">行</param>
        ///// <param name="column">列</param>
        ///// <returns>值</returns>
        //public object Cells(string table, int row, int column) {
        //    DataTable dt = ToDataTable(table);
        //    return dt.IsNull() ? null : dt.Rows[row][column];
        //}
        ///// <summary>
        ///// 取第i个工作表的row,column位置的数据
        ///// </summary>
        ///// <param name="i">第i个工作表</param>
        ///// <param name="row">行</param>
        ///// <param name="column">列</param>
        ///// <returns>值</returns>
        //public object Cells(int i, int row, int column) {
        //    DataTable dt = ToDataTable(i);
        //    return dt.IsNull() ? null : dt.Rows[row][column];
        //}
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() {
            ds.Dispose();
        }
    }
}
