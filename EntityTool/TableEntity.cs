using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Pub.Class;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif

namespace EntityTool {
	public class TableEntity {
		public string Name = string.Empty;
		public bool isView = false;
	}

	public class helper {
		public string LowerFirstChar2(string str) {
			if (str.IsNullEmpty()) return string.Empty;
			string f = str.Substring(0, 1).ToLower();
			if (f == str.Substring(0, 1)) return "__" + str;
			if (str.Length == 1) return str.ToLower();
			return f + str.Substring(1, str.Length - 1);
		}
		public string SafeSql(string str) {
			return str + ".SafeSql()";
		}
		public bool IsNull(string str) {
			return str.IsNullEmpty();
		}
		public string GetMemo(string memo) {
			memo = memo.Replace("pk", "").Replace("PK", "").Replace("Pk", "").Trim().Split(' ')[0];
			return memo;
		}
		public string GetFirstColumnName(IList<TableStructureEntity> list) {
			return list.First().ColumnName;
		}
		public string MSSQLDBTypeToCSType(string dbType) {
			switch (dbType) {
				case "int": return "Int";
				case "tinyint": return "TinyInt";
				case "bigint": return "BigInt";
				case "float": return "Float";
				case "smallint": return "SmallInt";
				case "numeric": return "Decimal";
				case "decimal": return "Decimal";
				case "char": return "VarChar";
				case "nchar": return "NVarChar";
				case "ntext": return "NVarChar";
				case "nvarchar": return "NVarChar";
				case "text": return "NVarChar";
				case "varchar": return "VarChar";
				case "string":
				case "datetime": return "VarChar";
				case "smalldatetime": return "VarChar";
				case "bit": return "Bit";
				case "money": return "Money";
				case "uniqueidentifier": return "UniqueIdentifier";
				default: return dbType;
			}
		}
		public string MSSQLCSTypeToDBType(string csType, string dbType, int len, int decimals) {
			//string type = entity.ColumnType == "string" ?  :
			//        (entity.ColumnType == "DateTime" ? "varchar(" + entity.Length + ")" :
			//        (entity.ColumnType == "bool" ? "bit" :
			//        (entity.ColumnType == "decimal" ? "decimal(" + entity.Length + "," + entity.Decimals.ToString() + ")" :
			//        (entity.ColumnType == "numeric" ? "numeric(" + entity.Length + "," + entity.Decimals.ToString() + ")" :
			//        entity.DBType))));
			//    type = type.Replace("text(0)", "text").Replace("ntext(0)", "ntext").Replace("(-1)", "(MAX)");
			string leng = len <= 0 ? "max" : len.ToString();
			string value = string.Empty;
			switch (csType) {
				case "string": value = dbType + "(" + leng + ")"; break;
				case "DateTime": value = "varchar(" + leng + ")"; break;
				case "bool": value = "bit"; break;
				case "decimal": value = "decimal(" + leng + "," + decimals + ")"; break;
				case "numeric": value = "numeric(" + leng + "," + decimals + ")"; break;
				default: value = dbType; break;
			}
			value = value.Replace("text(0)", "text").Replace("ntext(0)", "ntext").Replace("text(max)", "text").Replace("ntext(max)", "ntext").Replace("(-1)", "(MAX)");
			return value;
		}
	}

	public class TableStructureEntity {
		public TableStructureEntity() {
			ColOrder = 0;
			ColumnName = string.Empty;
			ColumnType = string.Empty;
			DBType = string.Empty;
			CSType = string.Empty;
			Length = 0;
			IsIdentity = false;
			IsPK = false;
			IsFK = false;
			Bytes = 0;
			Decimals = 0;
			IsNull = false;
			Default = string.Empty;
			Memo = string.Empty;
			//ColumnType_ = string.Empty;
		}
		public int ColOrder { set; get; }
		public string ColumnName { set; get; }
		public string ColumnType { set; get; }
		//public string ColumnType_ { set; get; }
		public string DBType { set; get; }
		public string CSType { set; get; }
		public int Length { set; get; }
		public bool IsIdentity { set; get; }
		public bool IsPK { set; get; }
		public bool IsFK { set; get; }
		public int Bytes { set; get; }
		public int Decimals { set; get; }
		public bool IsNull { set; get; }
		public string Default { set; get; }
		public string Memo { set; get; }
	}

	[DefaultPropertyAttribute("Insert")]
	public class TableOperator {
		[ReadOnlyAttribute(true)]
		public string Table { get; set; }

		private bool isEntity = true;
		[CategoryAttribute("设置"), DefaultValueAttribute(true),]
		public bool Entity { get { return isEntity; } set { isEntity = value; } }

		private bool isInsert = true;
		[CategoryAttribute("设置"), DefaultValueAttribute(true),]
		public bool Insert { get { return isInsert; } set { isInsert = value; } }

		private bool isUpdate = true;
		[CategoryAttribute("设置"), DefaultValueAttribute(true),]
		public bool Update { get { return isUpdate; } set { isUpdate = value; } }

		private bool isUpdateAndInsert = false;
		[CategoryAttribute("设置"), DefaultValueAttribute(false),]
		public bool UpdateAndInsert { get { return isUpdateAndInsert; } set { isUpdateAndInsert = value; } }

		private bool isDelete = true;
		[CategoryAttribute("设置"), DefaultValueAttribute(true),]
		public bool DeleteByID { get { return isDelete; } set { isDelete = value; } }

		private bool isExistByID = false;
		[CategoryAttribute("设置"), DefaultValueAttribute(false)]
		public bool IsExistByID { get { return isExistByID; } set { isExistByID = value; } }

		private bool isSelectByID = true;
		[CategoryAttribute("设置"), DefaultValueAttribute(true)]
		public bool SelectByID { get { return isSelectByID; } set { isSelectByID = value; } }

		private bool isSelectPageList = true;
		[CategoryAttribute("设置"), DefaultValueAttribute(true)]
		public bool SelectPageList { get { return isSelectPageList; } set { isSelectPageList = value; } }

		private bool isSelectListByFK = true;
		[CategoryAttribute("设置"), DefaultValueAttribute(true)]
		public bool SelectListByFK { get { return isSelectListByFK; } set { isSelectListByFK = value; } }

		private bool isSelectListByAll = false;
		[CategoryAttribute("设置"), DefaultValueAttribute(false)]
		public bool SelectListByAll { get { return isSelectListByAll; } set { isSelectListByAll = value; } }
	}

}
