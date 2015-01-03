//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;

namespace Pub.Class.Excel.NPOI {
    /// <summary>
    /// COM组件写Excel11
    /// 
    /// 修改纪录
    ///     2012.03.19 版本：1.0 livexy 创建此类
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
            MemoryStream ms = RenderDataSetToExcel(ds) as MemoryStream;
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            byte[] data = ms.ToArray();

            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();

            data = null;
            ms = null;
            fs = null;
        }
        private Stream RenderDataSetToExcel(DataSet ds) {
            MemoryStream ms = new MemoryStream();
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet;
            HSSFRow headerRow;

            //for (int k = ds.Tables.Count - 1, len = 0; len <= k; k--) {
            //    DataTable dt = ds.Tables[k];
            foreach(DataTable dt in ds.Tables) {
                sheet = (HSSFSheet)workbook.CreateSheet(dt.TableName);
                headerRow = (HSSFRow)sheet.CreateRow(0);

                foreach (DataColumn column in dt.Columns)
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

                int rowIndex = 1;
                foreach (DataRow row in dt.Rows) {
                    HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);

                    foreach (DataColumn column in dt.Columns) {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }

                    rowIndex++;
                }
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            sheet = null;
            headerRow = null;
            workbook = null;
            return ms;
        }
        /// <summary>
        /// DataTable导出EXCEL文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        public void ToExcel(System.Data.DataTable dt) {
            MemoryStream ms = RenderDataTableToExcel(dt) as MemoryStream;
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            byte[] data = ms.ToArray();

            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();

            data = null;
            ms = null;
            fs = null;
        }
        private Stream RenderDataTableToExcel(DataTable dt) {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(dt.TableName);
            HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);

            foreach (DataColumn column in dt.Columns)
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

            int rowIndex = 1;
            foreach (DataRow row in dt.Rows) {
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);

                foreach (DataColumn column in dt.Columns) {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }

                rowIndex++;
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            sheet = null;
            headerRow = null;
            workbook = null;
            return ms;
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
            //doc = null;
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
            //cells.Add(row, column, value);
        }
        /// <summary>
        /// 保存修改
        /// </summary>
        public void Save() {
            //doc.Save(fileName, true);
        }
        /// <summary>
        /// 打开指定的工作薄
        /// </summary>
        /// <param name="workSheets">第N个工作薄</param>
        public void OpenWorkSheets(int workSheets) {
            //if (workSheets <= 0) return; //工作薄编号必须从1开始
            //cells = doc.Workbook.Worksheets[workSheets].Cells;
        }
    }
}
