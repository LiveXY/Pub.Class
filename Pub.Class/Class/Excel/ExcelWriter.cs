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
    /// 写Excel
    /// 
    /// 修改纪录
    ///     2011.07.04 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface IExcelWriter: IAddIn {
        /// <summary>
        /// 打开excel文件
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        void Open(string excelPath);
        /// <summary>
        /// DataSet导出EXCEL文件
        /// </summary>
        /// <param name="ds">DataSet</param>
        void ToExcel(DataSet ds);
        /// <summary>
        /// DataTable导出EXCEL文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        void ToExcel(DataTable dt);
        /// <summary>
        /// 删除工作表
        /// </summary>
        /// <param name="tableName">表名</param>
        void Delete(string tableName);
        /// <summary>
        /// 释放资源
        /// </summary>
        void Dispose();
    }

    /// <summary>
    /// 写EXCEL
    /// 
    /// 修改纪录
    ///     2011.07.04 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    ///         ExcelWriter excelWriter = new ExcelWriter("Pub.Class.Excel.OleDb.dll", "Pub.Class.Excel.OleDb.ExcelWriter", "~/test2.xls".GetMapPath());
    ///         ExcelWriter excelWriter = new ExcelWriter("Pub.Class.Excel.COM.dll", "Pub.Class.Excel.COM.Excel11Writer", "~/test5.xls".GetMapPath());
    ///         excelWriter.ToExcel(ds);
    ///         excelWriter.Delete("判断题");
    ///         excelWriter.ToExcel(ds.Tables[1]);
    ///         excelWriter.Dispose();
    /// </code>
    /// </example>
    /// </summary>
    public class ExcelWriter: Disposable {
        private readonly IExcelWriter excelWriter = null;
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="excelPath">excel文件路径</param>
        public ExcelWriter(string dllFileName, string className, string excelPath) {
            if (excelWriter.IsNull()) {
                excelWriter = (IExcelWriter)dllFileName.LoadClass(className);
                excelWriter.Open(excelPath);
            }
        }
        /// <summary>
        /// 构造器 指定classNameDllName(ExcelWriterProviderName) 默认Pub.Class.Excel.OleDb.ExcelWriter,Pub.Class.Excel.OleDb
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        /// <param name="excelPath">excel文件路径</param>
        public ExcelWriter(string classNameAndAssembly, string excelPath) { 
            if (excelWriter.IsNull()) {
                excelWriter = (IExcelWriter)classNameAndAssembly.IfNullOrEmpty("Pub.Class.Excel.OleDb.ExcelWriter,Pub.Class.Excel.OleDb").LoadClass();
                excelWriter.Open(excelPath);
            }
        }
        /// <summary>
        /// 构造器 从Web.config中读ExcelWriterProviderName 默认Pub.Class.Excel.OleDb.ExcelWriter,Pub.Class.Excel.OleDb
        /// </summary>
        /// <param name="excelPath">excel文件路径</param>
        public ExcelWriter(string excelPath) { 
            if (excelWriter.IsNull()) {
                excelWriter = (IExcelWriter)(WebConfig.GetApp("ExcelWriterProviderName") ?? "Pub.Class.Excel.OleDb.ExcelWriter,Pub.Class.Excel.OleDb").LoadClass();
                excelWriter.Open(excelPath);
            }
        }
        /// <summary>
        /// DataSet导出EXCEL文件
        /// </summary>
        /// <param name="ds">DataSet</param>
        public ExcelWriter ToExcel(DataSet ds) {
            excelWriter.ToExcel(ds);
            return this;
        }
        /// <summary>
        /// DataTable导出EXCEL文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        public ExcelWriter ToExcel(DataTable dt) {
            excelWriter.ToExcel(dt);
            return this;
        }
        /// <summary>
        /// 删除工作表
        /// </summary>
        /// <param name="tableName">表名</param>
        public ExcelWriter Delete(string tableName) {
            excelWriter.Delete(tableName);
            return this;
        }
        /// <summary>
        /// 用using 自动释放
        /// </summary>
        protected override void InternalDispose() {
            excelWriter.Dispose();
            base.InternalDispose();
        }
    }
}
