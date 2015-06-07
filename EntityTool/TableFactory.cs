using System;
using System.Collections.Generic;
using System.Text;
using Pub.Class;
using System.Data.Common;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif

namespace EntityTool {
	public class TableFactory {
		public static IList<TableEntity> GetTable() {
			IList<TableEntity> tabList = new List<TableEntity>();
			if (Data.DBType == "SqlServer") {
				string strSql = "select [name] as tabName,[type] from dbo.sysobjects where ([type] = 'u' or [type] = 'v') and [name]<>'dtproperties' order by [type] asc,[name] asc";
				DbDataReader dr = Data.GetDbDataReader(strSql);
				if (dr.IsNull()) return tabList;
				while (dr.Read()) {
					string tabName = dr["tabName"].ToString();
					string type = dr["type"].ToString().ToLower().Trim();
					if (tabName.ToLower().Trim() == "sysdiagrams") continue;

					TableEntity tabEntity = new TableEntity();
					tabEntity.isView = type == "u" ? false : true;
					tabEntity.Name = tabName;
					tabList.Add(tabEntity);
				}
				dr.Close(); dr.Dispose(); dr = null;
			} else if (Data.DBType == "Access" || Data.ConnString.ToLower().IndexOf(".mdb") > 0) {
				OleDbConnection conn = new OleDbConnection(Data.ConnString); conn.Open();
				DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
				foreach (DataRow dr in dt.Rows) {
					TableEntity tabEntity = new TableEntity();
					tabEntity.Name = dr["TABLE_NAME"].ToString();
					tabList.Add(tabEntity);
				}
				dt.Dispose(); dt = null;
				conn.Close(); conn.Dispose(); conn = null;
			} else if (Data.DBType == "MySql") {
				string[] arrs = Data.ConnString.Split(';');
				string database = "";
				foreach (string s in arrs) {
					if (s.Split('=')[0].ToLower() == "database" && s.Split('=').Length == 2) database = s.Split('=')[1];
				}
				string strSql = "SELECT table_name tabName FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = '" + database + "' ";
				DbDataReader dr = Data.GetDbDataReader(strSql);
				if (dr.IsNull()) return tabList;
				while (dr.Read()) {
					string tabName = dr["tabName"].ToString();

					TableEntity tabEntity = new TableEntity();
					tabEntity.isView = false;
					tabEntity.Name = tabName;
					tabList.Add(tabEntity);
				}
				dr.Close(); dr.Dispose(); dr = null;
			} else if (Data.DBType == "SQLite" || Data.DBType == "MonoSQLite") {
				string strSql = "select name from sqlite_master where type='table'";
				DbDataReader dr = Data.GetDbDataReader(strSql);
				if (dr.IsNull()) return tabList;
				while (dr.Read()) {
					string tabName = dr["name"].ToString();

					TableEntity tabEntity = new TableEntity();
					tabEntity.isView = false;
					tabEntity.Name = tabName;
					tabList.Add(tabEntity);
				}
				dr.Close(); dr.Dispose(); dr = null;
			}
			return tabList;
		}
	}

	public class TableStructureFactory {
		public static IList<TableStructureEntity> list = null;
		public static Config GetConfig() {
			return new Config() {
				ModelPath = string.IsNullOrEmpty(WebConfig.GetApp("ModelPath")) ? "" : WebConfig.GetApp("ModelPath").TrimEnd('/') + "/",
				DALPath = string.IsNullOrEmpty(WebConfig.GetApp("DALPath")) ? "" : WebConfig.GetApp("DALPath").TrimEnd('/') + "/",
				IDALPath = string.IsNullOrEmpty(WebConfig.GetApp("IDALPath")) ? "" : WebConfig.GetApp("IDALPath").TrimEnd('/') + "/",
				BLLPath = string.IsNullOrEmpty(WebConfig.GetApp("BLLPath")) ? "" : WebConfig.GetApp("BLLPath").TrimEnd('/') + "/",
				EntityPath = string.IsNullOrEmpty(WebConfig.GetApp("EntityPath")) ? "" : WebConfig.GetApp("EntityPath").TrimEnd('/') + "/",
				FactoryPath = string.IsNullOrEmpty(WebConfig.GetApp("FactoryPath")) ? "" : WebConfig.GetApp("FactoryPath").TrimEnd('/') + "/",
				AdminPath = string.IsNullOrEmpty(WebConfig.GetApp("AdminPath")) ? "" : WebConfig.GetApp("AdminPath").TrimEnd('/') + "/",
				Author = string.IsNullOrEmpty(WebConfig.GetApp("Author")) ? "LiveXY" : WebConfig.GetApp("Author"),
				CopyRight = string.IsNullOrEmpty(WebConfig.GetApp("CopyRight")) ? "LiveXY" : WebConfig.GetApp("CopyRight"),
				TemplateName = string.IsNullOrEmpty(WebConfig.GetApp("TemplateName")) ? "Model-DAL-BLL-SqlServer-Text" : WebConfig.GetApp("TemplateName"),
				DesignPattern = string.IsNullOrEmpty(WebConfig.GetApp("DesignPattern")) ? "Static" : WebConfig.GetApp("DesignPattern"),
				DesignPatternExtName = string.IsNullOrEmpty(WebConfig.GetApp("DesignPatternExtName")) ? "Helper" : WebConfig.GetApp("DesignPatternExtName"),
				UseOneProject = string.IsNullOrEmpty(WebConfig.GetApp("UseOneProject")) ? false : WebConfig.GetApp("UseOneProject").ToBool(false),
				CacheTime = (string.IsNullOrEmpty(WebConfig.GetApp("CacheTime")) ? "0" : WebConfig.GetApp("CacheTime")).ToInt(),
				PageSize = (string.IsNullOrEmpty(WebConfig.GetApp("PageSize"))) ? "15" : WebConfig.GetApp("PageSize"),
				PagerSqlEnum = (string.IsNullOrEmpty(WebConfig.GetApp("PagerSqlEnum"))) ? "Base.PagerSqlEnum" : WebConfig.GetApp("PagerSqlEnum"),
				ProjectStartDate = string.IsNullOrEmpty(WebConfig.GetApp("ProjectStartDate")) ? DateTime.Now.ToString("yyyy-MM-dd") : WebConfig.GetApp("ProjectStartDate"),
				OPList = new List<TableOperator>(),
				Project = string.IsNullOrEmpty(WebConfig.GetApp("Project")) ? "Test" : WebConfig.GetApp("Project")
			};
		}
		public static void CreateXML(string fileName, IList<TableOperator> oplist) {
			Xml2.Create(fileName, "", "", "utf-8", "<root></root>");
			Xml2 xml = new Xml2(fileName);
			foreach (var info in oplist) {
				xml.AddNode("root", "Table", "Name|Entity|Insert|Update|Delete|IsExistByID|SelectByID|SelectPageList|SelectListByFK|SelectListByAll|UpdateAndInsert", "{0}|{9}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{10}".FormatWith(
					info.Table, info.Insert.ToString().ToLower(), info.Update.ToString().ToLower(), info.DeleteByID.ToString().ToLower(),
					info.IsExistByID.ToString().ToLower(), info.SelectByID.ToString().ToLower(), info.SelectPageList.ToString().ToLower(),
					info.SelectListByFK.ToString().ToLower(), info.SelectListByAll.ToString().ToLower(), info.Entity.ToString().ToLower(),
					info.UpdateAndInsert.ToString().ToLower()
				));
			}
			xml.Save();
			xml.Close();
		}
		public static IList<TableOperator> LoadOPList(Config config, Action<TableEntity> process) {
			string xmlFile = "".GetMapPath() + config.Project + ".xml";
			bool xmlExist = FileDirectory.FileExists(xmlFile);
			IList<TableEntity> list = TableFactory.GetTable();
			foreach (TableEntity entity in list) {
				process(entity);
				if (!xmlExist) config.OPList.Add(new TableOperator() { Table = entity.Name });
				else {
					Xml2 xml = new Xml2(xmlFile);
					string[] attrs = xml.GetAttr("root//Table[@Name='" + entity.Name + "']", "Name|Insert|Update|Delete|IsExistByID|SelectByID|SelectPageList|SelectListByFK|SelectListByAll|Entity|UpdateAndInsert").Split('|');
					if (attrs[0].IsNullEmpty()) {
						config.OPList.Add(new TableOperator() { Table = entity.Name });
						var info = config.OPList[config.OPList.Count - 1];
						xml.AddNode("root", "Table", "Name|Entity|Insert|Update|Delete|IsExistByID|SelectByID|SelectPageList|SelectListByFK|SelectListByAll|UpdateAndInsert", "{0}|{9}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{10}".FormatWith(
							info.Table, info.Insert.ToString().ToLower(), info.Update.ToString().ToLower(), info.DeleteByID.ToString().ToLower(),
							info.IsExistByID.ToString().ToLower(), info.SelectByID.ToString().ToLower(), info.SelectPageList.ToString().ToLower(),
							info.SelectListByFK.ToString().ToLower(), info.SelectListByAll.ToString().ToLower(), info.Entity.ToString().ToLower(),
							info.UpdateAndInsert.ToString().ToLower()
						));
						xml.Save();
					} else {
						config.OPList.Add(new TableOperator() {
							Table = entity.Name,
							Insert = attrs[1] == "true" ? true : false,
							Update = attrs[2] == "true" ? true : false,
							DeleteByID = attrs[3] == "true" ? true : false,
							IsExistByID = attrs[4] == "true" ? true : false,
							SelectByID = attrs[5] == "true" ? true : false,
							SelectPageList = attrs[6] == "true" ? true : false,
							SelectListByFK = attrs[7] == "true" ? true : false,
							SelectListByAll = attrs[8] == "true" ? true : false,
							Entity = attrs[9] == "true" ? true : false,
							UpdateAndInsert = attrs[10] == "true" ? true : false,
						});
					}
					xml.Close();
				}
			}
			if (!xmlExist) TableStructureFactory.CreateXML(xmlFile, config.OPList);
			return config.OPList;
		}

		public static string DBTypeToCSType(string dbType) {
			switch (dbType) {
				case "integer":
				case "int": return "Int";
				case "tinyint": return "TinyInt";
				case "bigint": return "BigInt";
				case "float": return "Float";
				case "smallint": return "SmallInt";
				case "numeric": return "Decimal";
				case "decimal": return "Decimal";
				case "char": return "Char";
				case "nchar": return "NChar";
				case "ntext": return "NText";
				case "nvarchar": return "NVarChar";
				case "text": return "Text";
				case "varchar": return "VarChar";
				case "datetime": return "VarChar";
				case "smalldatetime": return "VarChar";
				case "bit": return "Bit";
				case "money": return "Money";
				case "uniqueidentifier": return "UniqueIdentifier";
			}
			return "";
		}
		public static string SQLiteDBTypeToCSType(string dbType) {
			switch (dbType) {
				case "integer":
				case "int": return "BigInt";
				case "tinyint": return "TinyInt";
				case "bigint": return "BigInt";
				case "float": return "Float";
				case "smallint": return "SmallInt";
				case "numeric": return "Decimal";
				case "decimal": return "Decimal";
				case "char": return "Char";
				case "nchar": return "NChar";
				case "ntext": return "NText";
				case "nvarchar": return "NVarChar";
				case "text": return "Text";
				case "varchar": return "VarChar";
				case "datetime": return "VarChar";
				case "smalldatetime": return "VarChar";
				case "bit": return "Bit";
				case "money": return "Money";
				case "uniqueidentifier": return "UniqueIdentifier";
			}
			return "";
		}

		public static string AccessDBTypeToCSType(string dbType) {
			switch (dbType) {
				case "int": return "Integer";
				case "tinyint": return "TinyInt";
				case "bigint": return "BigInt";
				case "float": return "Float";
				case "smallint": return "SmallInt";
				case "numeric": return "Decimal";
				case "decimal": return "Decimal";
				case "varchar": return "VarChar";
				case "text": return "LongVarChar";
				case "datetime": return "VarChar";
				case "smalldatetime": return "VarChar";
				case "bit": return "Boolean";
			}
			return "";
		}

		public static IList<TableStructureEntity> GetTableStructure(string tableName, out string title) {
			title = string.Empty;
			IList<TableStructureEntity> tabStructList = new List<TableStructureEntity>();
			if (Data.DBType == "SqlServer") {
				#region SQL
				string strSql = string.Format(@"SELECT (case when a.colorder=1 then d.name else '' end) TableName, 
	a.colorder ColOrder, 
	a.name ColumnName, 
	(case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then 1 else 0 end) IsIdentity, 
	(case when (SELECT count(*) FROM sysobjects WHERE (name in (SELECT name FROM sysindexes WHERE (id = a.id) AND (indid in (SELECT indid FROM sysindexkeys WHERE (id = a.id) AND (colid in (SELECT colid FROM syscolumns WHERE (id = a.id) AND (name = a.name))))))) AND (xtype = 'PK'))>0 then 1  else 0 end) PK, 
	(case when (select count(*) from sysforeignkeys as x,syscolumns as y, sysobjects as z where x.fkeyid=y.id and x.fkey=y.colid and x.fkeyid=z.id and z.name='{0}' and y.name=a.name)>0 then 1  else 0 end) FK, 
	b.name ColumnType, 
	a.length Bytes, 
	COLUMNPROPERTY(a.id,a.name,'PRECISION') as Length, 
	isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0) as Decimals, 
	(case when a.isnullable=1 then 1 else 0 end) _IsNull, 
	isnull(e.text,'') _Default, 
	isnull(g.[value],'') AS _Memo 
FROM syscolumns a left join systypes b on a.xtype=b.xusertype 
	inner join sysobjects d on a.id=d.id and (d.xtype='U' or d.xtype='V') and d.name<>'dtproperties' 
	left join syscomments e on a.cdefault=e.id 
	left join sys.extended_properties g on a.id=g.major_id AND a.colid = g.minor_id 
where d.name = '{0}'
order by a.id,a.colorder", tableName);
				DbDataReader dr = Data.GetDbDataReader(strSql);
				if (dr.IsNull()) return tabStructList;
				while (dr.Read()) {
					TableStructureEntity tabStatuctEntity = new TableStructureEntity();
					tabStatuctEntity.ColumnName = dr["ColumnName"].ToString().Trim();
					tabStatuctEntity.ColumnType = dr["ColumnType"].ToString().Trim();
					tabStatuctEntity.DBType = tabStatuctEntity.ColumnType;
					tabStatuctEntity.CSType = DBTypeToCSType(tabStatuctEntity.ColumnType);
					tabStatuctEntity.ColOrder = dr["ColOrder"].ToString().ToInt();
					tabStatuctEntity.IsIdentity = dr["IsIdentity"].ToString() == "1" ? true : false;
					tabStatuctEntity.IsPK = dr["PK"].ToString() == "1" ? true : false;
					tabStatuctEntity.IsFK = dr["FK"].ToString() == "1" ? true : false;
					tabStatuctEntity.Bytes = dr["Bytes"].ToString().ToInt();
					tabStatuctEntity.Length = dr["Length"].ToString().ToInt();
					tabStatuctEntity.Decimals = dr["Decimals"].ToString().ToInt();
					tabStatuctEntity.IsNull = dr["_IsNull"].ToString() == "1" ? true : false;
					tabStatuctEntity.Default = dr["_Default"].ToString().Trim();
					tabStatuctEntity.Memo = dr["_Memo"].ToString().Trim().ReplaceRN();

					if (tabStatuctEntity.IsPK) title = tabStatuctEntity.Memo.IndexOf("编号") >= 0 ? tabStatuctEntity.Memo.Replace("编号", "") : "";

					//if (!tabStatuctEntity.IsFK) {}
					if (tabStatuctEntity.Memo.Length >= 2) {
						if (tabStatuctEntity.Memo.Substring(0, 2).ToLower() == "fk") {
							tabStatuctEntity.IsFK = true;
							tabStatuctEntity.Memo = tabStatuctEntity.Memo.Substring(2).Trim();
						}
					}
					if (string.IsNullOrEmpty(tabStatuctEntity.Memo)) tabStatuctEntity.Memo = tabStatuctEntity.ColumnName;

					string columnType = tabStatuctEntity.ColumnType;
					string type = "," + columnType + ",";
					string def = tabStatuctEntity.Default.TrimEnd(')').TrimStart('(');
					if (",int,tinyint,bigint,float,smallint,numeric,decimal,money,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						if (columnType == "bigint") tabStatuctEntity.ColumnType = "Int64";
						else if (columnType == "float") tabStatuctEntity.ColumnType = "decimal";
						else if (columnType == "numeric") tabStatuctEntity.ColumnType = "decimal";
						else if (columnType == "decimal") tabStatuctEntity.ColumnType = "decimal";
						else if (columnType == "tinyint") tabStatuctEntity.ColumnType = "byte";
						else if (columnType == "smallint") tabStatuctEntity.ColumnType = "short";
						else if (columnType == "money") tabStatuctEntity.ColumnType = "decimal";
						else tabStatuctEntity.ColumnType = "int";
						if (tabStatuctEntity.ColumnType != "decimal") tabStatuctEntity.Length = tabStatuctEntity.Bytes;

						//System.Windows.Forms.MessageBox.Show(tabStatuctEntity.ColumnType + " " + tabStatuctEntity.ColumnName + " " + tabStatuctEntity.Default);
					} else if (",char,nchar,ntext,nvarchar,text,varchar,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						if (tabStatuctEntity.ColumnType == "text") tabStatuctEntity.Length = 0;
						if (tabStatuctEntity.ColumnType == "ntext") tabStatuctEntity.Length = 0;

						tabStatuctEntity.ColumnType = "string";
					} else if (",datetime,smalldatetime,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "DateTime";
						if (def.Trim().ToLower().IndexOf("getdate(") >= 0) tabStatuctEntity.IsIdentity = true;
						tabStatuctEntity.DBType = "string";
						tabStatuctEntity.Length = 23;
					} else if (",bit,".IndexOf(type) >= 0) {
						//tabStatuctEntity.Default = string.IsNullOrEmpty(def.Trim()) ? "= null" : def.Trim()=="0" ? "= false" : "= true";
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "bool";
					} else if (",uniqueidentifier,".IndexOf(type) >= 0) {
						if (tabStatuctEntity.Default == "(newid())") tabStatuctEntity.IsIdentity = true;
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "Guid";
						tabStatuctEntity.Length = 36;
						//tabStatuctEntity.DBType = "Guid";
					}
					//if (tabStatuctEntity.IsPK) tabStatuctEntity.IsIdentity = true;

					tabStructList.Add(tabStatuctEntity);
				}
				dr.Close(); dr.Dispose(); dr = null;
				#endregion
			} else if (Data.DBType == "Access" || Data.ConnString.ToLower().IndexOf(".mdb") > 0) {
				#region Access
				OleDbConnection conn = new OleDbConnection(Data.ConnString); conn.Open();
				string strSql = string.Format("select top 1 * from [{0}]", tableName);
				OleDbCommand cmd = new OleDbCommand();
				cmd.Connection = conn;
				cmd.CommandText = "SELECT * FROM [" + tableName + "]";
				OleDbDataReader dr = cmd.ExecuteReader(CommandBehavior.KeyInfo);
				if (dr.IsNull()) return tabStructList;
				DataTable schemaTable = dr.GetSchemaTable();
				dr.Close(); dr.Dispose(); dr = null;
				cmd.Dispose(); cmd = null;

				int i = 0;
				foreach (DataRow myField in schemaTable.Rows) {

					TableStructureEntity tabStatuctEntity = new TableStructureEntity();
					tabStatuctEntity.ColumnName = myField["ColumnName"].ToString().Trim();
					tabStatuctEntity.ColumnType = myField["DataType"].ToString().Trim();
					tabStatuctEntity.ColOrder = i;
					tabStatuctEntity.IsIdentity = string.Compare(myField["IsAutoIncrement"].ToString().Trim(), "true", true) == 0 ? true : false;
					tabStatuctEntity.IsPK = string.Compare(myField["IsKey"].ToString().Trim(), "true", true) == 0 ? true : false;
					tabStatuctEntity.IsFK = false;
					tabStatuctEntity.Length = myField["ColumnSize"].ToString().ToInt();
					tabStatuctEntity.IsNull = string.Compare(myField["AllowDBNull"].ToString().Trim(), "true", true) == 0 ? true : false;
					tabStatuctEntity.Bytes = myField["NumericPrecision"].ToString().ToInt();
					tabStatuctEntity.Decimals = myField["NumericScale"].ToString().ToInt();

					DataTable dtblFields = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new Object[] { null, null, tableName, tabStatuctEntity.ColumnName });
					if (dtblFields != null && dtblFields.Rows.Count > 0) {
						tabStatuctEntity.Default = dtblFields.Rows[0]["COLUMN_DEFAULT"].ToString();
						tabStatuctEntity.Memo = dtblFields.Rows[0]["DESCRIPTION"].ToString().ReplaceRN();
					} else {
						tabStatuctEntity.Memo = tabStatuctEntity.ColumnName.ReplaceRN();
					}

					if (tabStatuctEntity.IsPK) title = tabStatuctEntity.Memo.IndexOf("编号") >= 0 ? tabStatuctEntity.Memo.Replace("编号", "") : "";
					if (tabStatuctEntity.Memo.Length >= 2) {
						if (tabStatuctEntity.Memo.Substring(0, 2).ToLower() == "fk") {
							tabStatuctEntity.IsFK = true;
							tabStatuctEntity.Memo = tabStatuctEntity.Memo.Substring(2).Trim();
						}
					}
					if (string.IsNullOrEmpty(tabStatuctEntity.Memo)) tabStatuctEntity.Memo = tabStatuctEntity.ColumnName;

					string columnType = tabStatuctEntity.ColumnType;
					string type = "," + columnType + ",";
					string def = tabStatuctEntity.Default.TrimEnd(')').TrimStart('(');
					if (",System.Int32,System.Decimal,System.Byte,System.Double,System.Int64,System.Numeric,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						if (columnType == "System.Int64") { tabStatuctEntity.ColumnType = "Int64"; tabStatuctEntity.DBType = "bigint"; } else if (columnType == "System.Numeric") { tabStatuctEntity.ColumnType = "decimal"; tabStatuctEntity.DBType = "decimal"; } else if (columnType == "System.Double") { tabStatuctEntity.ColumnType = "decimal"; tabStatuctEntity.DBType = "decimal"; } else if (columnType == "System.Decimal") { tabStatuctEntity.ColumnType = "decimal"; tabStatuctEntity.DBType = "decimal"; } else if (columnType == "System.Byte") { tabStatuctEntity.ColumnType = "byte"; tabStatuctEntity.DBType = "tinyint"; } else { tabStatuctEntity.ColumnType = "int"; tabStatuctEntity.DBType = "int"; };

					} else if (",System.String,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "string";
						//if (tabStatuctEntity.Length==536870910) tabStatuctEntity.Length = 0; 
						tabStatuctEntity.DBType = "varchar";
						if (tabStatuctEntity.Length > 255) {
							tabStatuctEntity.DBType = "text";
							tabStatuctEntity.Length = -1;
						}
					} else if (",System.DateTime,System.Date,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "DateTime";
						if (def.Trim().ToLower().IndexOf("now(") >= 0) tabStatuctEntity.IsIdentity = true;
						tabStatuctEntity.Length = 23;
						tabStatuctEntity.DBType = "datetime";
					} else if (",System.Boolean,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "bool";
						tabStatuctEntity.DBType = "bit";
					}
					tabStatuctEntity.CSType = AccessDBTypeToCSType(tabStatuctEntity.DBType);
					tabStructList.Add(tabStatuctEntity);

					i++;
				}
				conn.Close(); conn.Dispose(); conn = null;
				#endregion
			} else if (Data.DBType == "MySql") {
				#region MYSQL
				string[] arrs = Data.ConnString.Split(';');
				string database = "";
				foreach (string s in arrs) {
					if (s.Split('=')[0].ToLower() == "database" && s.Split('=').Length == 2) database = s.Split('=')[1];
				}
				string strSql = string.Format(@"
SELECT COLUMN_NAME, DATA_TYPE, COLUMN_DEFAULT, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH, COLUMN_TYPE, COLUMN_KEY, EXTRA, COLUMN_COMMENT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE table_name = '{0}'
AND table_schema = '{1}'
", tableName, database);
				DbDataReader dr = Data.GetDbDataReader(strSql);
				if (dr.IsNull()) return tabStructList;
				while (dr.Read()) {
					TableStructureEntity tabStatuctEntity = new TableStructureEntity();
					tabStatuctEntity.ColumnName = dr["COLUMN_NAME"].ToString().Trim();
					tabStatuctEntity.ColumnType = dr["DATA_TYPE"].ToString().Trim();
					tabStatuctEntity.DBType = tabStatuctEntity.ColumnType;
					tabStatuctEntity.CSType = DBTypeToCSType(tabStatuctEntity.ColumnType);
					//tabStatuctEntity.ColOrder = dr["ColOrder"].ToString().ToInt();
					tabStatuctEntity.IsIdentity = dr["EXTRA"].IsDBNull() ? false : (dr["EXTRA"].ToString() == "auto_increment" ? true : false);
					tabStatuctEntity.IsPK = dr["COLUMN_KEY"].IsDBNull() ? false : (dr["COLUMN_KEY"].ToString() == "PRI" ? true : false);
					//tabStatuctEntity.IsFK = dr["FK"].ToString() == "1" ? true : false;
					string _colType = dr["COLUMN_TYPE"].ToString();
					if (_colType.IndexOf("(") != -1 && _colType.Split('(').Length == 2) {
						tabStatuctEntity.Bytes = _colType.Split('(')[1].Replace(")", "").Split(',')[0].ToInt();
					}
					tabStatuctEntity.Length = dr["CHARACTER_MAXIMUM_LENGTH"].ToString().ToInt();
					//tabStatuctEntity.Decimals = dr["Decimals"].ToString().ToInt();
					tabStatuctEntity.IsNull = dr["IS_NULLABLE"].ToString() == "YES" ? true : false;
					tabStatuctEntity.Default = dr["COLUMN_DEFAULT"].ToString().Trim();
					tabStatuctEntity.Memo = dr["COLUMN_COMMENT"].ToString().Trim().ReplaceRN();

					if (tabStatuctEntity.IsPK) title = tabStatuctEntity.Memo.IndexOf("编号") >= 0 ? tabStatuctEntity.Memo.Replace("编号", "") : "";

					//if (!tabStatuctEntity.IsFK) {}
					if (tabStatuctEntity.Memo.Length >= 2) {
						if (tabStatuctEntity.Memo.Substring(0, 2).ToLower() == "fk") {
							tabStatuctEntity.IsFK = true;
							tabStatuctEntity.Memo = tabStatuctEntity.Memo.Substring(2).Trim();
						}
					}
					if (string.IsNullOrEmpty(tabStatuctEntity.Memo)) tabStatuctEntity.Memo = tabStatuctEntity.ColumnName;

					string columnType = tabStatuctEntity.ColumnType;
					string type = "," + columnType + ",";
					string def = tabStatuctEntity.Default.TrimEnd(')').TrimStart('(');
					if (",int,tinyint,bigint,float,smallint,numeric,decimal,money,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						if (columnType == "bigint") tabStatuctEntity.ColumnType = "Int64";
						else if (columnType == "float") tabStatuctEntity.ColumnType = "float";
						else if (columnType == "numeric") tabStatuctEntity.ColumnType = "decimal";
						else if (columnType == "decimal") tabStatuctEntity.ColumnType = "decimal";
						else if (columnType == "tinyint") tabStatuctEntity.ColumnType = "bool";
						else if (columnType == "smallint") tabStatuctEntity.ColumnType = "short";
						else if (columnType == "money") tabStatuctEntity.ColumnType = "decimal";
						else tabStatuctEntity.ColumnType = "int";
						if (tabStatuctEntity.ColumnType != "decimal") tabStatuctEntity.Length = tabStatuctEntity.Bytes;

						//System.Windows.Forms.MessageBox.Show(tabStatuctEntity.ColumnType + " " + tabStatuctEntity.ColumnName + " " + tabStatuctEntity.Default);
					} else if (",char,nchar,ntext,nvarchar,text,varchar,longtext,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						if (tabStatuctEntity.ColumnType == "text") tabStatuctEntity.Length = 0;
						if (tabStatuctEntity.ColumnType == "ntext") tabStatuctEntity.Length = 0;

						tabStatuctEntity.ColumnType = "string";
					} else if (",datetime,smalldatetime,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "DateTime";
						if (def.Trim().ToLower().IndexOf("getdate(") >= 0) tabStatuctEntity.IsIdentity = true;
						tabStatuctEntity.DBType = "string";
						tabStatuctEntity.Length = 23;
					} else if (",bool,boolean,".IndexOf(type) >= 0) {
						//tabStatuctEntity.Default = string.IsNullOrEmpty(def.Trim()) ? "= null" : def.Trim()=="0" ? "= false" : "= true";
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "bool";
					} else if (",bit,".IndexOf(type) >= 0) {
						//tabStatuctEntity.Default = string.IsNullOrEmpty(def.Trim()) ? "= null" : def.Trim()=="0" ? "= false" : "= true";
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "UInt64";
					} else if (",uniqueidentifier,".IndexOf(type) >= 0) {
						if (tabStatuctEntity.Default == "(newid())") tabStatuctEntity.IsIdentity = true;
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "Guid";
						tabStatuctEntity.Length = 36;
						//tabStatuctEntity.DBType = "Guid";
					}
					//if (tabStatuctEntity.IsPK) tabStatuctEntity.IsIdentity = true;

					tabStructList.Add(tabStatuctEntity);
				}
				dr.Close(); dr.Dispose(); dr = null;
				#endregion
			} else if (Data.DBType == "SQLite" || Data.DBType == "MonoSQLite") {
				#region SQLite
				string strSql = string.Format("select sql from sqlite_master where type='table' and name='{0}'", tableName);
				IList<string> sqls = Data.GetScalar(strSql).ToString().Replace("\"", "").Replace("[", "").Replace("]", "").GetLines();
				if (sqls.IsNullEmpty() || sqls.Count < 3) return tabStructList;
				bool ispk = sqls[sqls.Count - 2].Trim(',').Trim().StartsWith("primary key", StringComparison.InvariantCultureIgnoreCase);
				string pks = ispk ? sqls[sqls.Count - 2].Trim(',').Trim().Substring(11) : "";
				for (int i = 1, len = sqls.Count - (ispk ? 2 : 1); i < len; i++) {
					string[] rows = sqls[i].Trim().Trim(',').Split(' ');
					bool rowAutoID = sqls[i].IndexOf("AUTOINCREMENT", StringComparison.InvariantCultureIgnoreCase) != -1;
					bool rowPKey = pks.IndexOf(rows[0], StringComparison.InvariantCultureIgnoreCase) != -1;
					if (!rowPKey) rowPKey = sqls[i].IndexOf("PRIMARY KEY ", StringComparison.InvariantCultureIgnoreCase) != -1;
					TableStructureEntity tabStatuctEntity = new TableStructureEntity();
					tabStatuctEntity.ColumnName = rows[0];
					tabStatuctEntity.ColumnType = rows[1].Trim().ToLower();
					if (rows[1].IndexOf("(") != -1) {
						tabStatuctEntity.ColumnType = rows[1].Trim().ToLower().Substring(0, rows[1].IndexOf("("));
						tabStatuctEntity.Length = rows[1].Trim().ToLower().Substring(rows[1].IndexOf("(")).Replace("(", "").Replace(")", "").ToInt();
					}
					tabStatuctEntity.DBType = tabStatuctEntity.ColumnType;
					tabStatuctEntity.CSType = SQLiteDBTypeToCSType(tabStatuctEntity.ColumnType);
					//tabStatuctEntity.ColOrder = dr["ColOrder"].ToString().ToInt();
					tabStatuctEntity.IsIdentity = rowAutoID;
					tabStatuctEntity.IsPK = rowPKey;
					//tabStatuctEntity.IsFK = dr["FK"].ToString() == "1" ? true : false;
					string _colType = rows[1];
					if (_colType.IndexOf("(") != -1 && _colType.Split('(').Length == 2) {
						tabStatuctEntity.Bytes = _colType.Split('(')[1].Replace(")", "").Split(',')[0].ToInt();
					}
					//tabStatuctEntity.Length = dr["CHARACTER_MAXIMUM_LENGTH"].ToString().ToInt();
					//tabStatuctEntity.Decimals = dr["Decimals"].ToString().ToInt();
					//tabStatuctEntity.IsNull = dr["IS_NULLABLE"].ToString() == "YES" ? true : false;
					//tabStatuctEntity.Default = dr["COLUMN_DEFAULT"].ToString().Trim();
					//tabStatuctEntity.Memo = dr["COLUMN_COMMENT"].ToString().Trim().ReplaceRN();

					if (tabStatuctEntity.IsPK) title = tabStatuctEntity.Memo.IndexOf("编号") >= 0 ? tabStatuctEntity.Memo.Replace("编号", "") : "";

					//if (!tabStatuctEntity.IsFK) {}
					if (tabStatuctEntity.Memo.Length >= 2) {
						if (tabStatuctEntity.Memo.Substring(0, 2).ToLower() == "fk") {
							tabStatuctEntity.IsFK = true;
							tabStatuctEntity.Memo = tabStatuctEntity.Memo.Substring(2).Trim();
						}
					}
					if (string.IsNullOrEmpty(tabStatuctEntity.Memo)) tabStatuctEntity.Memo = tabStatuctEntity.ColumnName;

					string columnType = tabStatuctEntity.ColumnType;
					string type = "," + columnType + ",";
					string def = tabStatuctEntity.Default.TrimEnd(')').TrimStart('(');
					if (",int,integer,tinyint,bigint,float,smallint,numeric,decimal,money,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						if (columnType == "bigint") tabStatuctEntity.ColumnType = "Int64";
						else if (columnType == "integer") tabStatuctEntity.ColumnType = "Int64";
						else if (columnType == "float") tabStatuctEntity.ColumnType = "float";
						else if (columnType == "numeric") tabStatuctEntity.ColumnType = "decimal";
						else if (columnType == "decimal") tabStatuctEntity.ColumnType = "decimal";
						else if (columnType == "tinyint") tabStatuctEntity.ColumnType = "bool";
						else if (columnType == "smallint") tabStatuctEntity.ColumnType = "short";
						else if (columnType == "money") tabStatuctEntity.ColumnType = "decimal";
						else tabStatuctEntity.ColumnType = "int";
						if (tabStatuctEntity.ColumnType != "decimal") tabStatuctEntity.Length = tabStatuctEntity.Bytes;

						//System.Windows.Forms.MessageBox.Show(tabStatuctEntity.ColumnType + " " + tabStatuctEntity.ColumnName + " " + tabStatuctEntity.Default);
					} else if (",char,nchar,ntext,nvarchar,text,varchar,longtext,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						if (tabStatuctEntity.ColumnType == "text") tabStatuctEntity.Length = 0;
						if (tabStatuctEntity.ColumnType == "ntext") tabStatuctEntity.Length = 0;

						tabStatuctEntity.ColumnType = "string";
					} else if (",datetime,smalldatetime,".IndexOf(type) >= 0) {
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "DateTime";
						if (def.Trim().ToLower().IndexOf("getdate(") >= 0) tabStatuctEntity.IsIdentity = true;
						tabStatuctEntity.DBType = "string";
						tabStatuctEntity.Length = 23;
					} else if (",bool,boolean,".IndexOf(type) >= 0) {
						//tabStatuctEntity.Default = string.IsNullOrEmpty(def.Trim()) ? "= null" : def.Trim()=="0" ? "= false" : "= true";
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "bool";
					} else if (",bit,".IndexOf(type) >= 0) {
						//tabStatuctEntity.Default = string.IsNullOrEmpty(def.Trim()) ? "= null" : def.Trim()=="0" ? "= false" : "= true";
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "bool";
					} else if (",uniqueidentifier,".IndexOf(type) >= 0) {
						if (tabStatuctEntity.Default == "(newid())") tabStatuctEntity.IsIdentity = true;
						tabStatuctEntity.Default = "= null";
						tabStatuctEntity.ColumnType = "Guid";
						tabStatuctEntity.Length = 36;
						//tabStatuctEntity.DBType = "Guid";
					}
					//if (tabStatuctEntity.IsPK) tabStatuctEntity.IsIdentity = true;

					tabStructList.Add(tabStatuctEntity);
				}
				#endregion
			}
			return tabStructList;
		}

		public static string GetTableStructCode(Config config, string tableName, string projectName, out string idalCode, out string dalCode, out string bllCode, out string sqlCode, bool isView) {
			string title = string.Empty;
			list = GetTableStructure(tableName, out title);

			VelocityHelper entity = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
			entity.Put("Project", projectName);
			entity.Put("ProjectStartDate", config.ProjectStartDate);
			entity.Put("CopyRight", config.CopyRight);
			entity.Put("TableName", tableName);
			entity.Put("TableNameCN", title);
			entity.Put("Now", DateTime.Now.ToString("yyyy-MM-dd"));
			entity.Put("Fields", list);
			entity.Put("helper", new helper());
			string code = entity.Display("model.tmp");

			dalCode = string.Empty; sqlCode = string.Empty;
			idalCode = string.Empty; bllCode = string.Empty;
			var pkInfo = list.Where(p => p.IsPK && p.IsIdentity).FirstOrDefault();
			var currTableOP = config.OPList.Where(p => p.Table == tableName).FirstOrDefault();
			if (!currTableOP.Insert && !currTableOP.Update && !currTableOP.DeleteByID && !currTableOP.SelectByID && !currTableOP.IsExistByID && !currTableOP.SelectListByAll && !currTableOP.SelectListByFK && !currTableOP.SelectPageList && !currTableOP.UpdateAndInsert) return code;

			VelocityHelper dal = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
			dal.Put("Project", projectName);
			dal.Put("ProjectStartDate", config.ProjectStartDate);
			dal.Put("CacheTime", config.CacheTime);
			dal.Put("TemplateName", config.TemplateName.Split('-')[3]);
			dal.Put("CopyRight", config.CopyRight);
			dal.Put("TableName", tableName);
			dal.Put("TableNameCN", title);
			dal.Put("Now", DateTime.Now.ToString("yyyy-MM-dd"));
			dal.Put("Fields", list);
			dal.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
			dal.Put("PKFields", list.Where(p => p.IsPK).ToList());
			dal.Put("FKFields", list.Where(p => p.IsFK).ToList());
			dal.Put("helper", new helper());
			dal.Put("currTableOP", currTableOP);
			dal.Put("PKInfo", pkInfo);
			dal.Put("isView", isView);
			dalCode = dal.Display("dal.tmp");

			VelocityHelper idal = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
			idal.Put("Project", projectName);
			idal.Put("ProjectStartDate", config.ProjectStartDate);
			idal.Put("CacheTime", config.CacheTime);
			idal.Put("TemplateName", config.TemplateName.Split('-')[3]);
			idal.Put("CopyRight", config.CopyRight);
			idal.Put("TableName", tableName);
			idal.Put("TableNameCN", title);
			idal.Put("Now", DateTime.Now.ToString("yyyy-MM-dd"));
			idal.Put("Fields", list);
			idal.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
			idal.Put("PKFields", list.Where(p => p.IsPK).ToList());
			idal.Put("FKFields", list.Where(p => p.IsFK).ToList());
			idal.Put("helper", new helper());
			idal.Put("currTableOP", currTableOP);
			idal.Put("PKInfo", pkInfo);
			idal.Put("isView", isView);
			idalCode = idal.Display("idal.tmp");

			VelocityHelper bll = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
			bll.Put("Project", projectName);
			bll.Put("ProjectStartDate", config.ProjectStartDate);
			bll.Put("CacheTime", config.CacheTime);
			bll.Put("TemplateName", config.TemplateName.Split('-')[3]);
			bll.Put("CopyRight", config.CopyRight);
			bll.Put("TableName", tableName);
			bll.Put("TableNameCN", title);
			bll.Put("Now", DateTime.Now.ToString("yyyy-MM-dd"));
			bll.Put("Fields", list);
			bll.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
			bll.Put("PKFields", list.Where(p => p.IsPK).ToList());
			bll.Put("FKFields", list.Where(p => p.IsFK).ToList());
			bll.Put("helper", new helper());
			bll.Put("currTableOP", currTableOP);
			bll.Put("PKInfo", pkInfo);
			bll.Put("isView", isView);
			bll.Put("UseOneProject", config.UseOneProject);
			bllCode = bll.Display("bll.tmp");

			//MessageBox.Show(list.Where(p => p.IsPK && p.IsIdentity).Count().ToString());

			string dbfile = "".GetMapPath() + ("\\templates\\{0}\\" + config.TemplateName.Split('-')[3] + "db.tmp").FormatWith(config.TemplateName);
			if (FileDirectory.FileExists(dbfile)) {
				VelocityHelper db = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
				db.Put("Project", projectName);
				db.Put("ProjectStartDate", config.ProjectStartDate);
				db.Put("CacheTime", config.CacheTime);
				//db.Put("DesignPatternExtName", config.DesignPatternExtName);
				db.Put("CopyRight", config.CopyRight);
				db.Put("TableName", tableName);
				db.Put("TableNameCN", title);
				db.Put("Now", DateTime.Now.ToString("yyyy-MM-dd"));
				db.Put("Fields", list);
				db.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
				db.Put("PKFields", list.Where(p => p.IsPK).ToList());
				db.Put("FKFields", list.Where(p => p.IsFK).ToList());
				db.Put("helper", new helper());
				db.Put("currTableOP", currTableOP);
				db.Put("PKInfo", pkInfo);
				db.Put("isView", isView);
				sqlCode = db.Display(config.TemplateName.Split('-')[3] + "db.tmp");
			}

			if (!string.IsNullOrEmpty(config.AdminPath)) {
				//AdminCode(tableName, tableName.LowerFirstChar2(), projectName, pkInfo.IsNull() || pkInfo.ColumnName.IsNullEmpty() ? false : true, isView, pkInfo.IsNull() || pkInfo.ColumnName.IsNullEmpty() ? string.Empty : pkInfo.ColumnType);

				StringBuilder dbCode = new StringBuilder();
				bool isOnePage = false; int addTotalCount = 0; int pks = 0; int idspks = 0; bool max200 = false; int max200Count = 0;
				string csCode = string.Empty; string aspxCode = string.Empty, entityName = tableName.LowerFirstChar2();
				string folder = (tableName.IndexOf("_") >= 0 ? tableName.Substring(0, tableName.IndexOf("_")) : tableName).ToLower();
				string aspxPath = config.AdminPath + "\\admin\\" + folder + "\\";

				foreach (TableStructureEntity entity2 in list) {
					if (entity2.IsPK) { pks++; };
					if (!max200 && (entity2.Length > 200 || entity2.DBType == "ntext" || entity2.DBType == "text" || entity2.Length <= 0)) max200 = true;
					if (entity2.IsPK && entity2.IsIdentity) { idspks++; };
					if (!entity2.IsIdentity && entity2.IsPK) addTotalCount++;
					if (!entity2.IsIdentity && !entity2.IsPK) addTotalCount++;
					if (entity2.Length > 200 || entity2.DBType == "ntext" || entity2.DBType == "text" || entity2.Length <= 0) max200Count++;
				}
				if (addTotalCount < 6 && !max200) isOnePage = true;
				if (pks > 1) isOnePage = true;
				int height = addTotalCount < 10 ? addTotalCount * 31 + 65 + (110 * max200Count) : ((addTotalCount / 2) + (addTotalCount % 2)) * 31 + 65;

				VelocityHelper manage = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
				manage.Put("Project", projectName);
				manage.Put("ProjectStartDate", config.ProjectStartDate);
				manage.Put("CopyRight", config.CopyRight);
				manage.Put("TableName", tableName);
				manage.Put("TableNameCN", title);
				manage.Put("Fields", list);
				manage.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
				manage.Put("PKFields", list.Where(p => p.IsPK).ToList());
				manage.Put("FKFields", list.Where(p => p.IsFK).ToList());
				manage.Put("helper", new helper());
				manage.Put("PKInfo", pkInfo);
				manage.Put("isOnePage", isOnePage);
				manage.Put("PagerSqlEnum", config.PagerSqlEnum);
				manage.Put("PageSize", config.PageSize);
				manage.Put("WinHeight", height);
				aspxCode = manage.Display("manage.tmp");

				VelocityHelper managecs = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
				managecs.Put("Project", projectName);
				managecs.Put("ProjectStartDate", config.ProjectStartDate);
				managecs.Put("CopyRight", config.CopyRight);
				managecs.Put("TableName", tableName);
				managecs.Put("TableNameCN", title);
				managecs.Put("Fields", list);
				managecs.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
				managecs.Put("PKFields", list.Where(p => p.IsPK).ToList());
				managecs.Put("FKFields", list.Where(p => p.IsFK).ToList());
				managecs.Put("helper", new helper());
				managecs.Put("PKInfo", pkInfo);
				managecs.Put("isOnePage", isOnePage);
				managecs.Put("PagerSqlEnum", config.PagerSqlEnum);
				managecs.Put("PageSizes", config.PageSize);
				csCode = managecs.Display("manage.cs.tmp");

				FileDirectory.DirectoryCreate(aspxPath);
				FileDirectory.FileDelete(aspxPath + entityName + "Manage.aspx");
				FileDirectory.FileWrite(aspxPath + entityName + "Manage.aspx", aspxCode.ToString());
				FileDirectory.FileDelete(aspxPath + entityName + "Manage.aspx.cs");
				FileDirectory.FileWrite(aspxPath + entityName + "Manage.aspx.cs", csCode.ToString());
				if (!string.IsNullOrEmpty(config.AdminPath) && config.IsAll) {
					dbCode.AppendFormat(string.Format("    <table cellspacing='0' cellpadding='0' style='width: 100%;'><tbody><tr class='MenuItemRow'><td style='width: 7px;' /><td align='RIGHT' style='width: 10px; vertical-align: top;'><div class='IconContainer' style='width: 10px;'></div></td><td style='padding: 0px 9px 0px 5px;'><font class='MenuItemLabel'><a href='#' onclick=\"parent.window.frames[1].location.href='{0}'\" style='text-decoration:none; color:#428eff'>{1}</a></font></td></tr></tbody></table>", "../" + folder + "/" + entityName + "Manage.aspx", (string.IsNullOrEmpty(title) ? tableName : title) + "管理"));
					FileDirectory.FileWrite(config.AdminPath + "\\xml\\db.aspx", dbCode.ToString());
				}
				if (!isOnePage) {
					VelocityHelper add = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
					add.Put("Project", projectName);
					add.Put("ProjectStartDate", config.ProjectStartDate);
					add.Put("CopyRight", config.CopyRight);
					add.Put("TableName", tableName);
					add.Put("TableNameCN", title);
					add.Put("Fields", list);
					add.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
					add.Put("PKFields", list.Where(p => p.IsPK).ToList());
					add.Put("FKFields", list.Where(p => p.IsFK).ToList());
					add.Put("helper", new helper());
					add.Put("currTableOP", currTableOP);
					add.Put("PKInfo", pkInfo);
					aspxCode = add.Display("add.tmp");

					VelocityHelper addcs = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
					addcs.Put("Project", projectName);
					addcs.Put("ProjectStartDate", config.ProjectStartDate);
					addcs.Put("CopyRight", config.CopyRight);
					addcs.Put("TableName", tableName);
					addcs.Put("TableNameCN", title);
					addcs.Put("Fields", list);
					addcs.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
					addcs.Put("PKFields", list.Where(p => p.IsPK).ToList());
					addcs.Put("FKFields", list.Where(p => p.IsFK).ToList());
					addcs.Put("helper", new helper());
					addcs.Put("currTableOP", currTableOP);
					addcs.Put("PKInfo", pkInfo);
					csCode = addcs.Display("add.cs.tmp");

					FileDirectory.FileDelete(aspxPath + entityName + "Add.aspx");
					FileDirectory.FileWrite(aspxPath + entityName + "Add.aspx", aspxCode.ToString());
					FileDirectory.FileDelete(aspxPath + entityName + "Add.aspx.cs");
					FileDirectory.FileWrite(aspxPath + entityName + "Add.aspx.cs", csCode.ToString());
				}
			}

			return code;
			//return sb.ToString();
		}
		public static string GetTableStructCode(Config config, string tableName, string projectName, out string factoryCode, out string sqlCode, bool isView) {
			string title = string.Empty;
			list = GetTableStructure(tableName, out title);

			VelocityHelper entity = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
			entity.Put("Project", projectName);
			entity.Put("ProjectStartDate", config.ProjectStartDate);
			entity.Put("CopyRight", config.CopyRight);
			entity.Put("TableName", tableName);
			entity.Put("TableNameCN", title);
			entity.Put("Now", DateTime.Now.ToString("yyyy-MM-dd"));
			entity.Put("Fields", list);
			entity.Put("helper", new helper());
			string code = entity.Display("entity.tmp");

			factoryCode = string.Empty; sqlCode = string.Empty;
			var pkInfo = list.Where(p => p.IsPK && p.IsIdentity).FirstOrDefault();
			var currTableOP = config.OPList.Where(p => p.Table == tableName).FirstOrDefault();
			if (!currTableOP.Insert && !currTableOP.Update && !currTableOP.DeleteByID && !currTableOP.SelectByID && !currTableOP.IsExistByID && !currTableOP.SelectListByAll && !currTableOP.SelectListByFK && !currTableOP.SelectPageList && !currTableOP.UpdateAndInsert) return code;
			VelocityHelper factory = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
			factory.Put("Project", projectName);
			factory.Put("ProjectStartDate", config.ProjectStartDate);
			factory.Put("CacheTime", config.CacheTime);
			factory.Put("DesignPatternExtName", config.DesignPatternExtName);
			factory.Put("CopyRight", config.CopyRight);
			factory.Put("TableName", tableName);
			factory.Put("TableNameCN", title);
			factory.Put("Now", DateTime.Now.ToString("yyyy-MM-dd"));
			factory.Put("Fields", list);
			factory.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
			factory.Put("PKFields", list.Where(p => p.IsPK).ToList());
			factory.Put("FKFields", list.Where(p => p.IsFK).ToList());
			factory.Put("helper", new helper());
			factory.Put("currTableOP", currTableOP);
			factory.Put("PKInfo", pkInfo);
			factory.Put("isView", isView);
			factoryCode = factory.Display("helper.tmp");
			//MessageBox.Show(list.Where(p => p.IsPK && p.IsIdentity).Count().ToString());

			string dbfile = "".GetMapPath() + "\\templates\\{0}\\db.tmp".FormatWith(config.TemplateName);
			if (FileDirectory.FileExists(dbfile)) {
				VelocityHelper db = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
				db.Put("Project", projectName);
				db.Put("ProjectStartDate", config.ProjectStartDate);
				db.Put("CacheTime", config.CacheTime);
				db.Put("DesignPatternExtName", config.DesignPatternExtName);
				db.Put("CopyRight", config.CopyRight);
				db.Put("TableName", tableName);
				db.Put("TableNameCN", title);
				db.Put("Now", DateTime.Now.ToString("yyyy-MM-dd"));
				db.Put("Fields", list);
				db.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
				db.Put("PKFields", list.Where(p => p.IsPK).ToList());
				db.Put("FKFields", list.Where(p => p.IsFK).ToList());
				db.Put("helper", new helper());
				db.Put("currTableOP", currTableOP);
				db.Put("PKInfo", pkInfo);
				db.Put("isView", isView);
				sqlCode = factory.Display("db.tmp");
			}

			if (!string.IsNullOrEmpty(config.AdminPath)) {
				//AdminCode(tableName, tableName.LowerFirstChar2(), projectName, pkInfo.IsNull() || pkInfo.ColumnName.IsNullEmpty() ? false : true, isView, pkInfo.IsNull() || pkInfo.ColumnName.IsNullEmpty() ? string.Empty : pkInfo.ColumnType);

				StringBuilder dbCode = new StringBuilder();
				bool isOnePage = false; int addTotalCount = 0; int pks = 0; int idspks = 0; bool max200 = false; int max200Count = 0;
				string csCode = string.Empty; string aspxCode = string.Empty, entityName = tableName.LowerFirstChar2();
				string folder = (tableName.IndexOf("_") >= 0 ? tableName.Substring(0, tableName.IndexOf("_")) : tableName).ToLower();
				string aspxPath = config.AdminPath + "\\admin\\" + folder + "\\";

				foreach (TableStructureEntity entity2 in list) {
					if (entity2.IsPK) { pks++; };
					if (!max200 && (entity2.Length > 200 || entity2.DBType == "ntext" || entity2.DBType == "text" || entity2.Length <= 0)) max200 = true;
					if (entity2.IsPK && entity2.IsIdentity) { idspks++; };
					if (!entity2.IsIdentity && entity2.IsPK) addTotalCount++;
					if (!entity2.IsIdentity && !entity2.IsPK) addTotalCount++;
					if (entity2.Length > 200 || entity2.DBType == "ntext" || entity2.DBType == "text" || entity2.Length <= 0) max200Count++;
				}
				if (addTotalCount < 6 && !max200) isOnePage = true;
				if (pks > 1) isOnePage = true;
				int height = addTotalCount < 10 ? addTotalCount * 31 + 65 + (110 * max200Count) : ((addTotalCount / 2) + (addTotalCount % 2)) * 31 + 65;

				VelocityHelper manage = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
				manage.Put("Project", projectName);
				manage.Put("ProjectStartDate", config.ProjectStartDate);
				manage.Put("CopyRight", config.CopyRight);
				manage.Put("TableName", tableName);
				manage.Put("TableNameCN", title);
				manage.Put("Fields", list);
				manage.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
				manage.Put("PKFields", list.Where(p => p.IsPK).ToList());
				manage.Put("FKFields", list.Where(p => p.IsFK).ToList());
				manage.Put("helper", new helper());
				manage.Put("PKInfo", pkInfo);
				manage.Put("isOnePage", isOnePage);
				manage.Put("PagerSqlEnum", config.PagerSqlEnum);
				manage.Put("PageSize", config.PageSize);
				manage.Put("WinHeight", height);
				aspxCode = manage.Display("manage.tmp");

				VelocityHelper managecs = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
				managecs.Put("Project", projectName);
				managecs.Put("ProjectStartDate", config.ProjectStartDate);
				managecs.Put("CopyRight", config.CopyRight);
				managecs.Put("TableName", tableName);
				managecs.Put("TableNameCN", title);
				managecs.Put("Fields", list);
				managecs.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
				managecs.Put("PKFields", list.Where(p => p.IsPK).ToList());
				managecs.Put("FKFields", list.Where(p => p.IsFK).ToList());
				managecs.Put("helper", new helper());
				managecs.Put("PKInfo", pkInfo);
				managecs.Put("isOnePage", isOnePage);
				managecs.Put("PagerSqlEnum", config.PagerSqlEnum);
				managecs.Put("PageSizes", config.PageSize);
				csCode = managecs.Display("manage.cs.tmp");

				FileDirectory.DirectoryCreate(aspxPath);
				FileDirectory.FileDelete(aspxPath + entityName + "Manage.aspx");
				FileDirectory.FileWrite(aspxPath + entityName + "Manage.aspx", aspxCode.ToString());
				FileDirectory.FileDelete(aspxPath + entityName + "Manage.aspx.cs");
				FileDirectory.FileWrite(aspxPath + entityName + "Manage.aspx.cs", csCode.ToString());
				if (!string.IsNullOrEmpty(config.AdminPath) && config.IsAll) {
					dbCode.AppendFormat(string.Format("    <table cellspacing='0' cellpadding='0' style='width: 100%;'><tbody><tr class='MenuItemRow'><td style='width: 7px;' /><td align='RIGHT' style='width: 10px; vertical-align: top;'><div class='IconContainer' style='width: 10px;'></div></td><td style='padding: 0px 9px 0px 5px;'><font class='MenuItemLabel'><a href='#' onclick=\"parent.window.frames[1].location.href='{0}'\" style='text-decoration:none; color:#428eff'>{1}</a></font></td></tr></tbody></table>", "../" + folder + "/" + entityName + "Manage.aspx", (string.IsNullOrEmpty(title) ? tableName : title) + "管理"));
					FileDirectory.FileWrite(config.AdminPath + "\\xml\\db.aspx", dbCode.ToString());
				}
				if (!isOnePage) {
					VelocityHelper add = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
					add.Put("Project", projectName);
					add.Put("ProjectStartDate", config.ProjectStartDate);
					add.Put("CopyRight", config.CopyRight);
					add.Put("TableName", tableName);
					add.Put("TableNameCN", title);
					add.Put("Fields", list);
					add.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
					add.Put("PKFields", list.Where(p => p.IsPK).ToList());
					add.Put("FKFields", list.Where(p => p.IsFK).ToList());
					add.Put("helper", new helper());
					add.Put("currTableOP", currTableOP);
					add.Put("PKInfo", pkInfo);
					aspxCode = add.Display("add.tmp");

					VelocityHelper addcs = new VelocityHelper("\\templates\\{0}\\".FormatWith(config.TemplateName));
					addcs.Put("Project", projectName);
					addcs.Put("ProjectStartDate", config.ProjectStartDate);
					addcs.Put("CopyRight", config.CopyRight);
					addcs.Put("TableName", tableName);
					addcs.Put("TableNameCN", title);
					addcs.Put("Fields", list);
					addcs.Put("NoPKFields", list.Where(p => !p.IsPK).ToList());
					addcs.Put("PKFields", list.Where(p => p.IsPK).ToList());
					addcs.Put("FKFields", list.Where(p => p.IsFK).ToList());
					addcs.Put("helper", new helper());
					addcs.Put("currTableOP", currTableOP);
					addcs.Put("PKInfo", pkInfo);
					csCode = addcs.Display("add.cs.tmp");

					FileDirectory.FileDelete(aspxPath + entityName + "Add.aspx");
					FileDirectory.FileWrite(aspxPath + entityName + "Add.aspx", aspxCode.ToString());
					FileDirectory.FileDelete(aspxPath + entityName + "Add.aspx.cs");
					FileDirectory.FileWrite(aspxPath + entityName + "Add.aspx.cs", csCode.ToString());
				}
			}

			return code;
			//return sb.ToString();
		}
	}
}
