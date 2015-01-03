//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace Pub.Class.Excel.COM {
    /// <summary>
    /// COM组件写Excel11
    /// 
    /// 修改纪录
    ///     2011.02.15 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Excel11Writer : IExcelWriter {
        private string fileName = string.Empty;
        private Application xlsApp = new Application();
        private Worksheet xlsSheet = null;
        private Workbook xlsBook = null;
        private Range range = null;
        /// <summary>
        /// 打开excel文件
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        public void Open(string excelPath) {
            fileName = excelPath;
            xlsApp = new Application();
            if (FileDirectory.FileExists(fileName)) {
                xlsBook = xlsApp.Workbooks.Open(fileName, Type.Missing, false, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, 0, true, false, false);
            }
        }
        /// <summary>
        /// DataSet导出EXCEL文件
        /// </summary>
        /// <param name="ds">DataSet</param>
        public void ToExcel(DataSet ds) {
            if (xlsBook.IsNotNull()) xlsBook.Close();
            xlsBook = xlsApp.Workbooks.Add(true);
            for (int k = ds.Tables.Count - 1, len = 0; len <= k; k--) toExcel(ds.Tables[k], k);
            (xlsBook.Worksheets.get_Item(ds.Tables.Count + 1) as Worksheet).Delete();
            FileDirectory.FileDelete(fileName);
            xlsBook.SaveAs(fileName, 56, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }
        /// <summary>
        /// DataTable导出EXCEL文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        public void ToExcel(System.Data.DataTable dt) {
            if (xlsBook.IsNotNull()) xlsBook.Close();
            xlsBook = xlsApp.Workbooks.Add(true);
            toExcel(dt, 1);
            (xlsBook.Worksheets.get_Item(2) as Worksheet).Delete();
            FileDirectory.FileDelete(fileName);
            xlsBook.SaveAs(fileName, 56, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }
        private void toExcel(System.Data.DataTable dt, int i = 1) {
            xlsSheet = (Worksheet)xlsBook.Worksheets.Add();
            int rowIndex = 1, columnIndex = 0, colstart = 0;
            string tableName = dt.TableName.IsNullEmpty() ? ("Sheet" + i.ToString()) : dt.TableName.Trim('$');
            xlsSheet.Name = tableName;
            foreach (DataColumn col in dt.Columns) {
                columnIndex++;
                Range cel = (Range)xlsSheet.Cells[rowIndex, columnIndex];
                cel.Font.Bold = true;
                xlsSheet.Cells[rowIndex, columnIndex] = col.ColumnName;
            }
            foreach (DataRow row in dt.Rows) {
                rowIndex++;
                foreach (DataColumn col in dt.Columns) {
                    colstart++;
                    Range cel = (Range)xlsSheet.Cells[rowIndex, colstart];
                    xlsSheet.Cells[rowIndex, colstart] = row[col.ColumnName].ToString();
                }
                colstart = 0;
            }
        }
        /// <summary>
        /// 删除工作表
        /// </summary>
        /// <param name="tableName">表名</param>
        public void Delete(string tableName) {
            int i = 1, index = 0;
            foreach (Worksheet ws in xlsBook.Worksheets) {
                if (ws.Name.ToLower() == tableName.ToLower()) index = i;
                i++;
            }
            if (index > 0) {
                (xlsBook.Worksheets[index] as Worksheet).Visible = XlSheetVisibility.xlSheetHidden;
                (xlsBook.Worksheets[index] as Worksheet).Delete();
                xlsBook.Save();
                //xlsBook.Application.Save(fileName);
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() {
            if (range != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
            range = null;

            if (xlsSheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(xlsSheet);
            xlsSheet = null;

            if (xlsBook != null) {
                xlsBook.Close(false, Type.Missing, Type.Missing); System.Runtime.InteropServices.Marshal.ReleaseComObject(xlsBook);
            }
            xlsBook = null;

            if (xlsApp != null) {
                xlsApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlsApp);
            }
            xlsApp = null;
            Safe.KillProcess("EXCEL");
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
            xlsSheet.Cells[row, column] = value;
        }
        /// <summary>
        /// 保存修改
        /// </summary>
        public void Save() {
            xlsBook.Save();
        }
        /// <summary>
        /// 打开指定的工作薄
        /// </summary>
        /// <param name="workSheets">第N个工作薄</param>
        public void OpenWorkSheets(int workSheets) {
            if (workSheets <= 0) return; //工作薄编号必须从1开始
            xlsSheet = (Worksheet)xlsBook.Worksheets[workSheets];
        }
    }
}
