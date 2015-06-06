using System;
using System.Collections.Generic;
using System.Text;

namespace EntityTool {
	public class Config {
		public string Project { set; get; }
		public string TemplateName { set; get; }
		public string ProjectStartDate { set; get; }
		public string CopyRight { set; get; }
		public IList<TableOperator> OPList { set; get; }
		public int CacheTime { set; get; }
		public bool UseOneProject { set; get; }
		public string AdminPath { set; get; }
		public string PagerSqlEnum { set; get; }
		public string PageSize { set; get; }
		public bool IsAll { set; get; }
		public string DesignPatternExtName { set; get; }
		public string DesignPattern { set; get; }
		public string ModelPath { set; get; }
		public string DALPath { set; get; }
		public string IDALPath { set; get; }
		public string BLLPath { set; get; }
		public string EntityPath { set; get; }
		public string FactoryPath { set; get; }
		public string Author { set; get; }
	}
}
