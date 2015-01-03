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
    /// 读Excel
    /// 
    /// 修改纪录
    ///     2011.07.04 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface IExcelReader: IAddIn {
        /// <summary>
        /// 打开excel文件
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        void Open(string excelPath);
        /// <summary>
        /// excel转DataSet
        /// </summary>
        /// <returns>DataSet</returns>
        DataSet ToDataSet();
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="table">DataTable名称</param>
        /// <returns>DataTable</returns>
        DataTable ToDataTable(string table);
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>DataTable</returns>
        DataTable ToDataTable(int i);
        /// <summary>
        /// excel转DataTable 第0个
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable ToDataTable();
        /// <summary>
        /// 释放资源
        /// </summary>
        void Dispose();
    }

    /// <summary>
    /// 读EXCEL
    /// 
    /// 修改纪录
    ///     2011.07.04 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    ///         ExcelReader excelReader = new ExcelReader("Pub.Class.Excel.OleDb.dll", "Pub.Class.Excel.OleDb.ExcelReader", "~/test.xls".GetMapPath());
    ///         ExcelReader excelReader = new ExcelReader("Pub.Class.Excel.COM.dll", "Pub.Class.Excel.COM.Excel11Reader", "~/test.xls".GetMapPath());
    ///         DataSet ds = excelReader.ToDataSet();
    ///         foreach (DataTable dt in ds.Tables) { 
    ///             Msg.Write(dt.TableName + "：" + dt.ToJson() + "<br /><br />");
    ///         }
    ///         Msg.Write(excelReader.ToDataTable("单选题").ToJson());
    ///         excelReader.Dispose();
    /// </code>
    /// </example>
    /// </summary>
    public class ExcelReader: Disposable {
        private readonly IExcelReader excelReader = null;
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="excelPath">excel文件路径</param>
        public ExcelReader(string dllFileName, string className, string excelPath) {
            if (excelReader.IsNull()) {
                excelReader = (IExcelReader)dllFileName.LoadClass(className);
                excelReader.Open(excelPath);
            }
        }
        /// <summary>
        /// 构造器 指定classNameDllName(ExcelReaderProviderName) 默认Pub.Class.Excel.OleDb.ExcelReader,Pub.Class.Excel.OleDb
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        /// <param name="excelPath">excel文件路径</param>
        public ExcelReader(string classNameAndAssembly, string excelPath) { 
            if (excelReader.IsNull()) {
                excelReader = (IExcelReader)classNameAndAssembly.IfNullOrEmpty("Pub.Class.Excel.OleDb.ExcelReader,Pub.Class.Excel.OleDb").LoadClass();
                excelReader.Open(excelPath);
            }
        }
        /// <summary>
        /// 构造器 从Web.config中读ExcelReaderProviderName 默认Pub.Class.Excel.OleDb.ExcelReader,Pub.Class.Excel.OleDb
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        public ExcelReader(string excelPath) { 
            if (excelReader.IsNull()) {
                excelReader = (IExcelReader)(WebConfig.GetApp("ExcelReaderProviderName") ?? "Pub.Class.Excel.OleDb.ExcelReader,Pub.Class.Excel.OleDb").LoadClass();
                excelReader.Open(excelPath);
            }
        }
        /// <summary>
        /// excel转DataSet
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet ToDataSet() {
            return excelReader.ToDataSet();
        }
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="table">DataTable名称</param>
        /// <returns>DataTable</returns>
        public DataTable ToDataTable(string table) {
            return excelReader.ToDataTable(table);
        }
        /// <summary>
        /// excel转DataTable
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>DataTable</returns>
        public DataTable ToDataTable(int i) {
            return excelReader.ToDataTable(i);
        }
        /// <summary>
        /// excel转DataTable 第0个
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable ToDataTable() {
            return excelReader.ToDataTable(0);
        }
        /// <summary>
        /// 用using 自动释放
        /// </summary>
        protected override void InternalDispose() {
            excelReader.Dispose();
            base.InternalDispose();
        }
    }
}
