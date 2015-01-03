//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using org.in2bits.MyXls;

namespace Pub.Class.Excel.MyXls {
    /// <summary>
    /// COM组件写Excel11
    /// 
    /// 修改纪录
    ///     2012.03.19 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class ExcelWriter : IExcelWriter {
        private string fileName = string.Empty;
        private XlsDocument doc = new XlsDocument();
        private Cells cells;

        /// <summary>
        /// 打开excel文件
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        public void Open(string excelPath) {
            fileName = excelPath;
            doc.FileName = fileName.GetFileName();
        }
        /// <summary>
        /// DataSet导出EXCEL文件
        /// </summary>
        /// <param name="ds">DataSet</param>
        public void ToExcel(DataSet ds) {
            ds.Tables.Do((p, i) => toExcel((System.Data.DataTable)p, i + 1));
            Save();
        }
        /// <summary>
        /// DataTable导出EXCEL文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        public void ToExcel(System.Data.DataTable dt) {
            toExcel(dt, 1);
            Save();
        }
        private void toExcel(System.Data.DataTable dt, int i = 1) {
            Worksheet sheet = doc.Workbook.Worksheets.AddNamed(dt.TableName.IfNullOrEmpty("Sheet" + i.ToString()).Trim("$"));
            cells = sheet.Cells;
            int rows = dt.Rows.Count, cols = dt.Columns.Count;
            for (int k = 1; k <= cols; k++) {
                cells.AddValueCell(1, k, dt.Columns[k - 1].Caption).Font.Bold = true;
            }
            for (int j = 2; j <= rows + 1 ; j++) {
                for (int k = 1; k <= cols; k++) {
                    cells.AddValueCell(j, k, dt.Rows[j - 2][k - 1]);
                }
            }
        }
        /// <summary>
        /// 删除工作表
        /// </summary>
        /// <param name="tableName">表名</param>
        public void Delete(string tableName) {
            
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() {
            System.GC.Collect();
        }
        /// <summary>
        /// 写第i个工作表的row,column位置的数据
        /// </summary>
        /// <param name="i">第i个工作表</param>
        /// <param name="row">行</param>
        /// <param name="column">列</param>
        /// <returns>值</returns>
        public void Cells(int row, int column, object value) {
            cells.AddValueCell(row, column, value);
        }
        /// <summary>
        /// 保存修改
        /// </summary>
        public void Save() {
            doc.Save(fileName.GetParentPath('\\'), true);
        }
        /// <summary>
        /// 打开指定的工作薄
        /// </summary>
        /// <param name="workSheets">第N个工作薄</param>
        public void OpenWorkSheets(int workSheets) {
            if (workSheets <= 0) return; //工作薄编号必须从1开始
            cells = doc.Workbook.Worksheets[workSheets].Cells;
        }
    }
}
