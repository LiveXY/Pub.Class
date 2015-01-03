//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

namespace Pub.Class.Excel.OleDb {
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Text;
    /// <summary>
    /// OleDb写Excel11
    /// 
    /// 修改纪录
    ///     2011.02.15 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class ExcelWriter : IExcelWriter {
        private string fileName = string.Empty;
        /// <summary>
        /// 打开excel文件
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        public void Open(string excelPath) {
            fileName = excelPath;
        }
        /// <summary>
        /// DataSet导出EXCEL文件
        /// </summary>
        /// <param name="ds">DataSet</param>
        public void ToExcel(DataSet ds) {
            ds.ToExcel(fileName);
        }
        /// <summary>
        /// DataTable导出EXCEL文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        public void ToExcel(DataTable dt) {
            dt.ToExcel(fileName);
        }
        /// <summary>
        /// 删除工作表
        /// </summary>
        /// <param name="tableName">表名</param>
        public void Delete(string tableName) {
            ExcelReader reader = new ExcelReader();
            reader.Open(fileName);
            DataSet ds = reader.ToDataSet();
            reader.Dispose();
            DataSet ds2 = new DataSet();
            ds.Tables.Do((p, i) => {
                string table = ((DataTable)p).TableName.Trim('$').ToLower();
                if (!tableName.Trim('$').ToLower().Equals(table)) {
                    ds2.Tables.Add((DataTable)p);
                }
            });
            ToExcel(ds2);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() {
            Safe.KillProcess("EXCEL");
        }
    }
}
