//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace Pub.Class.Excel.COM {
    /// <summary>
    /// COM组件读Excel11
    /// 
    /// 修改纪录
    ///     2011.02.15 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Excel11Reader : IExcelReader {
        private DataSet ds = new DataSet();
        /// <summary>
        /// 打开excel文件
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        public void Open(string excelPath) {
            Application xlsApp = new Application();
            Workbook workbook = xlsApp.Workbooks.Open(excelPath,
                Type.Missing, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing
            );

            foreach (Worksheet sheet in workbook.Worksheets) {
                if (ds.Tables.IndexOf(sheet.Name) == -1) {
                    ds.Tables.Add(toDataTable(sheet));
                }
            }
        }
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="sheet">工作表</param>
        /// <param name="endRow">行</param>
        /// <param name="endCol">列</param>
        /// <param name="startRow">开始行</param>
        /// <param name="startCol">开始列</param>
        /// <returns>DataTable</returns>
        public System.Data.DataTable toDataTable(Worksheet sheet, int endRow, int endCol, int startRow = 1, int startCol = 1) {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.TableName = sheet.Name;
            endRow = endRow + startRow;
            Range excelRange = null;
            int cols = 1;
            for (int col = startCol; col <= endCol; col++) {
                excelRange = sheet.Cells[startRow, col] as Range;
                string val = excelRange.Text.ToString();
                if (string.IsNullOrEmpty(val)) break;
                dt.Columns.Add(val);
                cols++;
            }

            int nulls = 1, rownulls = 1;
            for (int row = startRow + 1; row <= endRow; row++) {
                DataRow dataRow = dt.NewRow();
                nulls = 1;
                for (int col = startCol; col < cols; col++) {
                    excelRange = sheet.Cells[row, col] as Range;
                    string val = excelRange.Text.ToString();
                    if (string.IsNullOrEmpty(val)) nulls++;
                    dataRow[col - 1] = val;
                }
                dt.Rows.Add(dataRow);
                if (nulls == cols) rownulls++;
                if (rownulls > 10) break;
            }
            int count = dt.Rows.Count;
            if (count > 10 && rownulls > 10) {
                int start = count - 10;
                for (int i = 0; i < 10; i++) dt.Rows[start].Delete();
            }
            if (excelRange != null) {
                Marshal.ReleaseComObject(excelRange);
                excelRange = null;
            }
            return dt;
        }
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="sheet">工作表</param>
        /// <param name="endRow">行</param>
        /// <returns>DataTable</returns>
        public System.Data.DataTable toDataTable(Worksheet sheet, int endRow) {
            return toDataTable(sheet, endRow, 50);
        }
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="sheet">工作表</param>
        /// <returns>DataTable</returns>
        public System.Data.DataTable toDataTable(Worksheet sheet) {
            return toDataTable(sheet, 1000, 50);
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
        public System.Data.DataTable ToDataTable(string table) {
            int i = -1, index = 0;
            foreach (System.Data.DataTable dt in ds.Tables) {
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
        public System.Data.DataTable ToDataTable(int i) {
            int count = ds.Tables.Count;
            return i < count ? ds.Tables[i] : null;
        }
        /// <summary>
        /// excel转DataTable 第0个
        /// </summary>
        /// <returns>DataTable</returns>
        public System.Data.DataTable ToDataTable() {
            return ToDataTable(0);
        }
        ///// <summary>
        ///// 取第i个工作表的row,column位置的数据
        ///// </summary>
        ///// <param name="i">第i个工作表</param>
        ///// <param name="row">行</param>
        ///// <param name="column">列</param>
        ///// <returns>值</returns>
        //public object Cells(string table, int row, int column) {
        //    System.Data.DataTable dt = ToDataTable(table);
        //    return dt.IsNull() ? null : dt.Rows[row][column];
        //}
        ///// <summary>
        ///// 取table工作表的row,column位置的数据
        ///// </summary>
        ///// <param name="table">工作表名</param>
        ///// <param name="row">行</param>
        ///// <param name="column">列</param>
        ///// <returns>值</returns>
        //public object Cells(int i, int row, int column) {
        //    System.Data.DataTable dt = ToDataTable(i);
        //    return dt.IsNull() ? null : dt.Rows[row][column];
        //}
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() {
            ds.Dispose();
            Safe.KillProcess("EXCEL");
        }
    }
}
