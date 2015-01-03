//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Web.Script.Serialization;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;

namespace Pub.Class {
    /// <summary>
    /// DataTable扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class DataTableExtensions {
        #region excel template
        private const string xlsTemplate = @"<?xml version=""1.0""?>
<?mso-application progid=""Excel.Sheet""?>
<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:o=""urn:schemas-microsoft-com:office:office""
 xmlns:x=""urn:schemas-microsoft-com:office:excel""
 xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:html=""http://www.w3.org/TR/REC-html40"">
 <DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">
  <Created>{0}T00:00:00Z</Created>
  <LastSaved>{0}T00:00:00Z</LastSaved>
  <Version>14.00</Version>
 </DocumentProperties>
 <OfficeDocumentSettings xmlns=""urn:schemas-microsoft-com:office:office"">
  <AllowPNG/>
  <RemovePersonalInformation/>
 </OfficeDocumentSettings>
 <ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">
  <WindowHeight>8010</WindowHeight>
  <WindowWidth>14805</WindowWidth>
  <WindowTopX>240</WindowTopX>
  <WindowTopY>105</WindowTopY>
  <ProtectStructure>False</ProtectStructure>
  <ProtectWindows>False</ProtectWindows>
 </ExcelWorkbook>
 <Styles>
  <Style ss:ID=""Default"" ss:Name=""Normal"">
   <Alignment ss:Vertical=""Bottom""/>
   <Borders/>
   <Font ss:FontName=""宋体"" x:CharSet=""134"" ss:Size=""11"" ss:Color=""#000000""/>
   <Interior/>
   <NumberFormat/>
   <Protection/>
  </Style>
  <Style ss:ID=""s62"">
   <Font ss:FontName=""宋体"" x:CharSet=""134"" ss:Size=""11"" ss:Color=""#000000""
    ss:Bold=""1""/>
  </Style>
 </Styles>
{1}
</Workbook>";
        private const string tableTemplate = @" <Worksheet ss:Name=""{0}"">
  <Table ss:ExpandedColumnCount=""{1}"" ss:ExpandedRowCount=""{2}"" x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultColumnWidth=""54"" ss:DefaultRowHeight=""13.5"">
{3}
  </Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <PageSetup>
    <Header x:Margin=""0.3""/>
    <Footer x:Margin=""0.3""/>
    <PageMargins x:Bottom=""0.75"" x:Left=""0.7"" x:Right=""0.7"" x:Top=""0.75""/>
   </PageSetup>
   <Print>
    <ValidPrinterInfo/>
    <PaperSizeIndex>0</PaperSizeIndex>
    <VerticalResolution>0</VerticalResolution>
    <NumberofCopies>0</NumberofCopies>
   </Print>
   <Selected/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>1</ActiveRow>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>";
        #endregion
        /// <summary>
        /// 最快 DataTable To 实体列表 IL
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").ToList&lt;UC_Member>();
        /// </code>
        /// </example>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns>实体列表</returns>
        public static List<TResult> ToList<TResult>(this DataTable dt) where TResult : class, new() {
            List<TResult> list = new List<TResult>();
            if (dt.IsNull() || dt.Rows.Count == 0) return list;
            DataTableEntityBuilder<TResult> eblist = DataTableEntityBuilder<TResult>.CreateBuilder(dt.Rows[0]);
            foreach (DataRow info in dt.Rows) list.Add(eblist.Build(info));
            dt.Dispose(); dt = null;
            return list;
        }
        /// <summary>
        /// 实体转换 IL
        /// </summary>
        /// <typeparam name="TResult">实体类</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns>实体类</returns>
        public static TResult ToEntity<TResult>(this DataTable dt) where TResult : class, new() {
            return dt.ToList<TResult>().FirstOrDefault();
        }
        /// <summary>
        /// DataTable To 实体列表 反射CACHE 
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").ToList2&lt;UC_Member>();
        /// </code>
        /// </example>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<TResult> ToList2<TResult>(this DataTable dt) where TResult : class, new() {
            //创建一个属性的列表   
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口   
            Type t = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表    
            Array.ForEach<PropertyInfo>(t.GetPropertiesCache(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合   
            List<TResult> oblist = new List<TResult>();

            foreach (DataRow row in dt.Rows) {
                //创建TResult的实例   
                TResult ob = new TResult();
                //找到对应的数据  并赋值   
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.   
                oblist.Add(ob);
            }
            return oblist;
        }
        /// <summary>
        /// DataTable To 实体列表 反射 
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").ToList3&lt;UC_Member>();
        /// </code>
        /// </example>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<TResult> ToList3<TResult>(this DataTable dt) where TResult : class, new() {
            List<TResult> list = new List<TResult>();
            if (dt.IsNull()) return list;
            int len = dt.Rows.Count;

            for (int i = 0; i < len; i++) {
                TResult info = new TResult();
                foreach (DataColumn dc in dt.Rows[0].Table.Columns) {
                    if (dt.Rows[i][dc.ColumnName].IsNull() || string.IsNullOrEmpty(dt.Rows[i][dc.ColumnName].ToString())) continue;
                    info.GetType().GetPropertyCache(dc.ColumnName).SetValue(info, dt.Rows[i][dc.ColumnName], null);
                }
                list.Add(info);
            }
            dt.Dispose(); dt = null;
            return list;
        }
        public static IList<T> ToList4<T>(this DataTable dt) where T : class,new() {
            List<T> list = new List<T>();
            if (dt == null || dt.Rows.Count <= 0) return list;
            PropertyInfo[] propertys = typeof(T).GetPropertiesCache(); //typeof(T).GetProperties();
            for (int j = 0; j < dt.Rows.Count; j++) {
                T _t = (T)Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo pi in propertys) {
                    if (pi.Name.Equals(dt.Columns[pi.Name].ColumnName)) {
                        if (dt.Rows[j][pi.Name] != DBNull.Value)
                            pi.SetValue(_t, dt.Rows[j][pi.Name], null);
                        //else
                        //    pi.SetValue(_t, null, null);
                    }
                }
                list.Add(_t);
            }
            return list;
        }
        /// <summary>
        /// DataTable 转JSON 速度最快
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").ToDataTableJson();
        /// </code>
        /// </example>
        /// <param name="dt">DataTable</param>
        /// <returns>JSON字符串</returns>
        public static string ToDataTableJson(this DataTable dt) {
            if (dt.IsNull()) return "[]";
            StringBuilder jsonHtml = new StringBuilder();
            jsonHtml.Append("[");

            JavaScriptSerializerString serializer = new JavaScriptSerializerString();
            foreach (DataRow row in dt.Rows) {
                jsonHtml.Append("{");
                foreach (DataColumn col in dt.Columns) jsonHtml.AppendFormat("\"{0}\":{1},", col.ColumnName, serializer.Serialize(row[col]));
                jsonHtml.Remove(jsonHtml.Length - 1, 1);
                jsonHtml.Append("},");
            }
            dt.Dispose(); dt = null;

            if (jsonHtml.Length > 1) jsonHtml.Remove(jsonHtml.Length - 1, 1);
            jsonHtml.Append("]");
            return jsonHtml.ToString();
        }
        /// <summary>
        /// DataTable 转JSON 速度最快
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").ToJson();
        /// </code>
        /// </example>
        /// <param name="dt">DataTable</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson(this DataTable dt) {
            return dt.ToDataTableJson();
        }
        /// <summary>
        /// DataTable 转JSON 速度最快
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="fields">字段列表</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson(this DataTable dt, params string[] fields) {
            return dt.ToJson(fields, null);
        }
        /// <summary>
        /// DataTable 转JSON 速度最快
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="fields">字段列表</param>
        /// <param name="alias">别名</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson(this DataTable dt, string[] fields, string[] alias) {
            if (dt.IsNull()) return "[]";
            if (alias.IsNull() || fields.Length != alias.Length) alias = fields;
            if (fields.IsNull() || fields.Length == 0) return dt.ToDataTableJson();

            JavaScriptSerializerString serializer = new JavaScriptSerializerString();
            StringBuilder jsonHtml = new StringBuilder();
            jsonHtml.Append("[");
            foreach (DataRow row in dt.Rows) {
                jsonHtml.Append("{");
                int i = 0;
                foreach (string col in fields) { jsonHtml.AppendFormat("\"{0}\":{1},", alias[i], serializer.Serialize(row[col])); i++; }
                jsonHtml.Remove(jsonHtml.Length - 1, 1);
                jsonHtml.Append("},");
            }
            dt.Dispose(); dt = null;

            if (jsonHtml.Length > 1) jsonHtml.Remove(jsonHtml.Length - 1, 1);
            jsonHtml.Append("]");
            return jsonHtml.ToString();
        }
        /// <summary>
        /// DataTable 转JSON 速度最快
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="fields">字段列表</param>
        /// <param name="alias">别名</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson(this DataTable dt, int[] fields, string[] alias) {
            if (dt.IsNull()) return "[]";
            if (alias.IsNull() || fields.Length != alias.Length) Msg.WriteEnd("参数错误！");
            if (fields.IsNull() || fields.Length == 0) return dt.ToDataTableJson();

            JavaScriptSerializerString serializer = new JavaScriptSerializerString();
            StringBuilder jsonHtml = new StringBuilder();
            jsonHtml.Append("[");
            foreach (DataRow row in dt.Rows) {
                jsonHtml.Append("{");
                int i = 0;
                foreach (int col in fields) { jsonHtml.AppendFormat("\"{0}\":{1},", alias[i], serializer.Serialize(row[col])); i++; }
                jsonHtml.Remove(jsonHtml.Length - 1, 1);
                jsonHtml.Append("},");
            }
            dt.Dispose(); dt = null;

            if (jsonHtml.Length > 1) jsonHtml.Remove(jsonHtml.Length - 1, 1);
            jsonHtml.Append("]");
            return jsonHtml.ToString();
        }
        /// <summary>
        /// DataTable 排序
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").OrderBy("MemberID desc");
        /// </code>
        /// </example>
        /// <param name="dt">DataTable</param>
        /// <param name="orderBy">orderBy</param>
        /// <returns>DataTable</returns>
        public static DataTable OrderBy(this DataTable dt, string orderBy) {
            dt.DefaultView.Sort = orderBy;
            return dt.DefaultView.ToTable();
        }
        /// <summary>
        /// DataTable 条件
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").Where("MemberID=1");
        /// </code>
        /// </example>
        /// <param name="dt">DataTable</param>
        /// <param name="where">where</param>
        /// <returns>DataTable</returns>
        public static DataTable Where(this DataTable dt, string where) {
            DataTable resultDt = dt.Clone();
            DataRow[] resultRows = dt.Select(where);
            foreach (DataRow dr in resultRows) resultDt.Rows.Add(dr.ItemArray);
            return resultDt;
        }
        /// <summary>
        /// DataTable 分页
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").ToPage(1,10,out totals);
        /// </code>
        /// </example>
        /// <param name="dt">DataTable</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每面大小</param>
        /// <param name="totalRecords">总记录数</param>
        /// <returns>DataTable</returns>
        public static DataTable ToPage(this DataTable dt, int pageIndex, int pageSize, out int totalRecords) {
            totalRecords = dt.Rows.Count;
            int startRow = (pageIndex - 1) * pageSize;
            int endRow = startRow + pageSize;
            if (startRow > totalRecords || startRow < 0) { startRow = 0; endRow = pageSize; }
            if (endRow > totalRecords + pageSize) { startRow = totalRecords - pageSize; endRow = totalRecords; }

            DataTable dt2 = dt.Clone();
            for (int i = startRow; i < endRow; i++) { if (i >= totalRecords) break; dt2.Rows.Add(dt.Rows[i].ItemArray); }

            return dt2;
        }
        /// <summary>
        /// DataTable 排序
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").OrderBy&lt;UC_Member>("MemberID desc");
        /// </code>
        /// </example>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <param name="orderBy">orderBy</param>
        /// <returns>实体列表</returns>
        public static List<TResult> OrderBy<TResult>(this DataTable dt, string orderBy) where TResult : class, new() {
            return dt.OrderBy(orderBy).ToList<TResult>();
        }
        /// <summary>
        /// DataTable 条件
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").Where&lt;UC_Member>("MemberID=1");
        /// </code>
        /// </example>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <param name="where">where</param>
        /// <returns>实体列表</returns>
        public static List<TResult> Where<TResult>(this DataTable dt, string where) where TResult : class, new() {
            return dt.Where(where).ToList<TResult>();
        }
        /// <summary>
        /// DataTable 分页
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").ToPage&lt;UC_Member>("MemberID desc");
        /// </code>
        /// </example>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每面大小</param>
        /// <param name="totalRecords">总记录数</param>
        /// <returns>实体列表</returns>
        public static List<TResult> ToPage<TResult>(this DataTable dt, int pageIndex, int pageSize, out int totalRecords) where TResult : class, new() {
            totalRecords = dt.Rows.Count;
            int startRow = (pageIndex - 1) * pageSize;
            int endRow = startRow + pageSize;
            if (startRow > totalRecords || startRow < 0) { startRow = 0; endRow = pageSize; }
            if (endRow > totalRecords + pageSize) { startRow = totalRecords - pageSize; endRow = totalRecords; }

            DataTable dt2 = dt.Clone();
            for (int i = startRow; i < endRow; i++) { if (i >= totalRecords) break; dt2.Rows.Add(dt.Rows[i].ItemArray); }

            return dt2.ToList<TResult>();
        }
        /// <summary>
        /// DataRow 取field列的值
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").Rows[0].Get&lt;int>("MemeberID")
        /// </code>
        /// </example>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="row">DataRow</param>
        /// <param name="field">字段名</param>
        /// <returns>值</returns>
        public static T Get<T>(this DataRow row, string field) { return row.Get<T>(field, default(T)); }
        /// <summary>
        /// DataRow 取field列的值
        /// </summary>
        /// <example>
        /// <code>
        /// Data.GetDataTable("Select MemberID,RealName from UC_Member").Rows[0].Get&lt;int>("MemeberID", 0)
        /// </code>
        /// </example>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="row">DataRow</param>
        /// <param name="field">字段名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        public static T Get<T>(this DataRow row, string field, T defaultValue) {
            var value = row[field];
            if (value == DBNull.Value) return defaultValue;
            return value.ConvertTo<T>(defaultValue);
        }
        /// <summary>
        /// DataRowView 取field列的值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="row">DataRowView</param>
        /// <param name="field">字段名</param>
        /// <returns>值</returns>
        public static T Get<T>(this DataRowView row, string field) { return row.Get<T>(field, default(T)); }
        /// <summary>
        /// DataRowView 取field列的值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="row">DataRowView</param>
        /// <param name="field">字段名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        public static T Get<T>(this DataRowView row, string field, T defaultValue) {
            var value = row[field];
            if (value == DBNull.Value) return defaultValue;
            return value.ConvertTo<T>(defaultValue);
        }
        /// <summary>
        /// 交换DataTable中的行列位置
        /// </summary>
        /// <param name="inputDT">要交换的DataTable</param>
        /// <returns>交换后的DataTable</returns>
        public static DataTable SwapDTCR(this DataTable inputDT) {
            DataTable outputDT = new DataTable();

            //标题的位置不变
            outputDT.Columns.Add(inputDT.Columns[0].ColumnName);

            foreach (DataRow inRow in inputDT.Rows) {
                string newColName = inRow[0].ToString();
                outputDT.Columns.Add(newColName);
            }

            for (int rCount = 1; rCount <= inputDT.Columns.Count - 1; rCount++) {
                DataRow newRow = outputDT.NewRow();

                newRow[0] = inputDT.Columns[rCount].ColumnName;
                for (int cCount = 0; cCount <= inputDT.Rows.Count - 1; cCount++) {
                    string colValue = inputDT.Rows[cCount][rCount].ToString();
                    newRow[cCount + 1] = colValue;
                }
                outputDT.Rows.Add(newRow);
            }
            return outputDT;
        }
        /// <summary>
        /// 通过DataTable获得CSV格式数据
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <param name="c">char</param>
        /// <returns>CSV字符串数据</returns>
        public static string ToCSV(this DataTable dataTable, char c = ',') {
            if (dataTable.IsNull()) return string.Empty;

            StringBuilder sb = new StringBuilder();
            // 写出表头
            foreach (DataColumn DataColumn in dataTable.Columns) sb.AppendFormat("{0}{1}", DataColumn.ColumnName.ToString(), c);
            sb.RemoveLastChar(c.ToString());
            sb.Append("\n");

            // 写出数据
            foreach (DataRowView dataRowView in dataTable.DefaultView) {
                foreach (DataColumn DataColumn in dataTable.Columns) sb.AppendFormat("{0}{1}", dataRowView[DataColumn.ColumnName].ToString(), c);
                sb.RemoveLastChar(c.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }
        /// <summary>
        /// 通过DataTable获得CSV格式数据
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <returns>CSV字符串数据</returns>
        public static string ToCSV(this DataTable dataTable) { return dataTable.ToCSV(','); }
        /// <summary>
        /// DataTable导出CSV文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="fileName">CSV文件路径</param>
        public static void ToCSV(this DataTable dt, string fileName) {
            FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, dt.ToCSV(','));
        }
        /// <summary>
        /// DataTable转换为EXCEL文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="fileName">excel文件路径</param>
        public static void ToExcel(this DataTable dt, string fileName) {
            FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, xlsTemplate.FormatWith(DateTime.Now.ToString("yyyy-MM-dd"), toExcel(dt)));
        }
        //public static void ToExcel(this DataTable dt, string fileName) {
        //    System.Web.UI.WebControls.DataGrid grid = new System.Web.UI.WebControls.DataGrid();
        //    grid.HeaderStyle.Font.Bold = true;
        //    grid.DataSource = dt;
        //    grid.DataMember = dt.TableName;
        //    grid.DataBind();
        //    using (StreamWriter sw = new StreamWriter(fileName)) {
        //        using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw)) {
        //            grid.RenderControl(hw);
        //        }
        //    }
        //}
        private static string toExcel(this DataTable dt, int i = 1) {
            StringBuilder sbXML = new StringBuilder();
            int cols = dt.Columns.Count, rows = dt.Rows.Count;
            if (cols == 0) return string.Empty;
            string tableName = (dt.TableName.IsNullEmpty() ? "Sheet" + i : dt.TableName).TrimEnd('$');

            sbXML.AppendLine("   <Row>");
            for (int j = 0; j < cols; j++) {
                sbXML.AppendLine("    <Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">{0}</Data></Cell>".FormatWith(dt.Columns[i].ColumnName));
            }
            sbXML.AppendLine("   </Row>");
            if (rows > 0) {
                for (int k = 0; k < rows; k++) {
                    sbXML.AppendLine("   <Row>");
                    for (int j = 0; j < cols; j++) {
                        sbXML.AppendLine("    <Cell><Data ss:Type=\"String\">{0}</Data></Cell>".FormatWith(dt.Rows[k][j].ToString()));
                    }
                    sbXML.AppendLine("   </Row>");
                }
            }
            return tableTemplate.FormatWith(tableName, cols, rows + 1, sbXML.ToString());
        }
        /// <summary>
        /// 取DataTable行和列值
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="row">行</param>
        /// <param name="column">列</param>
        /// <returns></returns>
        public static object Cell(this DataTable dt, int row, int column) {
            return dt.Rows[row][column];
        }
        /// <summary>
        /// DataSet 转 Json [[],[]]
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson(this DataSet ds) {
            StringBuilder sbJson = new StringBuilder();
            if (ds.IsNull() || ds.Tables.Count == 0) return "[]";
            sbJson.Append("[");
            foreach (DataTable table in ds.Tables) {
                sbJson.Append(table.ToJson());
                sbJson.Append(",");
            }
            sbJson.Remove(sbJson.Length - 1, 1);
            sbJson.Append("]");
            return sbJson.ToString();
        }
        /// <summary>
        /// 通过DataSet获得CSV格式数据
        /// </summary>
        /// <param name="dataSet">数据集</param>
        /// <returns>CSV字符串数据</returns>
        public static string ToCSV(this DataSet dataSet) {
            StringBuilder StringBuilder = new StringBuilder();
            foreach (DataTable dataTable in dataSet.Tables) {
                StringBuilder.Append(dataTable.ToCSV());
            }
            return StringBuilder.ToString();
        }
        /// <summary>
        /// DataSet导出CSV文件
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="fileName">CSV文件路径</param>
        public static void ToCSV(this DataSet ds, string fileName) {
            FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, ds.ToCSV());
        }
        /// <summary>
        /// DataSet转EXCEL
        /// </summary>
        /// <param name="source">DataSet</param>
        /// <param name="fileName">文件名</param>
        public static void ToExcel(this DataSet source, string fileName) {
            StringBuilder sbXML = new StringBuilder();
            source.Tables.Do((p, i) => sbXML.Append(toExcel((DataTable)p, i)));
            FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, xlsTemplate.FormatWith(DateTime.Now.ToString("yyyy-MM-dd"), sbXML.ToString()));
            source.Dispose();
            source = null;
        }
        /// <summary>
        /// 修改列名
        /// </summary>
        /// <param name="source">DataTable</param>
        /// <param name="list">字典</param>
        /// <param name="isFilter">是否过滤list中不存在的数据</param>
        public static DataTable ChangeColumnName(this DataTable source, Dictionary<string, string> list, bool isFilter = false) {
            DataTable output = new DataTable();
            if (isFilter) {
                list.Keys.Do(p => output.Columns.Add(list[p]));
                foreach (DataRow info in source.Rows) {
                    DataRow newRow = output.NewRow();
                    foreach (var key in list.Keys) newRow[list[key]] = info[key];
                    output.Rows.Add(newRow);
                }
            } else {
                foreach (DataColumn col in source.Columns) {
                    var colName = col.ColumnName;
                    if (list.ContainsKey(colName)) colName = list[colName];
                    output.Columns.Add(colName);
                }
                foreach (DataRow row in source.Rows) {
                    DataRow newRow = output.NewRow();
                    foreach (DataColumn col in source.Columns) {
                        var colName = col.ColumnName;
                        if (list.ContainsKey(colName)) colName = list[colName];
                        newRow[colName] = row[col.ColumnName];
                    }
                    output.Rows.Add(newRow);
                }
            }
            return output;
        }
        /// <summary>
        /// 修改列名
        /// </summary>
        /// <param name="source">DataTable</param>
        /// <param name="list">字典</param>
        public static DataTable ChangeColumnName(this DataTable source, Dictionary<string, string> list) { return ChangeColumnName(source, list); }
        /// <summary>
        /// 添加列
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static DataTable AddColumn<T>(this DataTable dt, string columnName) {
            dt.Columns.Add(columnName, typeof(T));
            return dt;
        }
        /// <summary>
        /// 添加行数据
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public static DataTable AddRow(this DataTable dt, params object[] values) {
            if (values.Length > dt.Columns.Count) return dt;
            lock (dt) {
                dt.BeginLoadData();
                DataRow dr = dt.NewRow();
                int i = 0;
                foreach (var val in values) { dr[i] = val; i++; }
                dt.Rows.Add(dr);
                dt.EndLoadData();
            }
            return dt;
        }
        /// <summary>
        /// 列修改名称
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="oldCol">老列名</param>
        /// <param name="newCol">新列名</param>
        /// <returns></returns>
        public static DataTable ColumnRename(this DataTable dt, string oldCol, string newCol) {
            int i = 0;
            foreach (DataColumn col in dt.Columns) {
                if (col.ColumnName == oldCol) dt.Columns[i].ColumnName = newCol;
                i++;
            }
            return dt;
        }
    }
}
