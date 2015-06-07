using System;
using System.Web;
using System.IO;
using System.Text;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using Commons.Collections;
using Pub.Class;
using Pub.Class.Linq;
using System.Collections.Generic;

namespace EntityTool {
	public class CommandLine {
		private static Config config = TableStructureFactory.GetConfig();
		private static IList<string> tables = new List<string>();
		public static void Start(string[] args) {
			Data.UsePool("ConnString");
			if (args.Length == 0) Help(); else Parse(args.Join(" "), true);
		}
		public static void Input(bool exit = false) {
			if (exit) return;
			Console.Write("> ");
			Parse(Console.ReadLine());
		}
		private static void WriteLog(string msg) { Console.WriteLine(msg); }
		private static void WriteLog(string format, params object[] args) { Console.WriteLine(format, args); }
		public static void Parse(string cmd, bool exit = false) {
			string[] args = cmd.Split(' ');
			cmd = args[0].Trim().ToLower();
			switch (cmd) {
				case "c":
				case "config": Config(exit); break;
				case "i":
				case "init": Init(exit); break;
				case "r":
				case "run": Run(exit); break;
				case "h":
				case "help": Help(exit); break;
				case "cls":
				case "clear": Console.Clear(); Input(exit); break;
				case "q":
				case "quit": break;
				default: Input(exit); break;
			}
		}
		public static void Run(bool exit = false) {
			if (!string.IsNullOrEmpty(config.AdminPath) && config.IsAll) {
				Pub.Class.FileDirectory.DirectoryCreate(config.AdminPath + "\\xml\\");
				Pub.Class.FileDirectory.FileDelete(config.AdminPath + "\\xml\\db.aspx");
				FileDirectory.FileWrite(config.AdminPath + "\\xml\\db.aspx", "<div class='MenuTitlebar' style='top: 12px; left: 12px; width: 168px; height: 25px;' title='数据库管理'><table cellspacing='0' cellpadding='0'><tbody><tr style='width: 185px; height: 25px;'><td class='MenuTitlebarLeft_Head' style='width: 13px;'/><td class='MenuTitlebarMiddle_Head' style='width: 130px;'><div class='MenuTitle_Head' style='width: 130px; height: 25px; line-height: 25px;'>数据库管理</div></td><td class='MenuTitlebarRight_Open_Head' style='width: 25px;'/></tr></tbody></table></div>\n\r<div class='MenuBody_Head' style='border-width: 0px 1px 1px; padding: 9px 0px; overflow: hidden; top: 37px; left: 12px; width: 166px; opacity: 1;'>");
			}

			StringBuilder sbSqlCode = new StringBuilder();
			tables.Clear();
			config.OPList = TableStructureFactory.LoadOPList(config, entity => {
				tables.Add((entity.isView ? "* " : "") + entity.Name);
			});
			foreach (string key in tables) {
				string tabName = key; bool isView = false;
				if (tabName.IndexOf("* ") == 0) { tabName = tabName.Substring(2); isView = true; }

				TableOperator to = config.OPList.Where(p => p.Table == tabName).FirstOrDefault();

				string dalCode = string.Empty; string idalCode = string.Empty; string bllCode = string.Empty;
				string sqlCode = string.Empty; string baseCode = string.Empty;

				if (!string.IsNullOrEmpty(config.ModelPath) && to.Entity && config.DesignPattern == "Model-DAL-BLL") {
					string code = TableStructureFactory.GetTableStructCode(config, tabName, config.Project, out idalCode, out dalCode, out bllCode, out sqlCode, isView);
					Pub.Class.FileDirectory.DirectoryCreate(config.ModelPath + "\\Model\\");
					Pub.Class.FileDirectory.FileDelete(config.ModelPath + "\\Model\\" + tabName + ".cs");
					FileDirectory.FileWrite(config.ModelPath + "\\Model\\" + tabName + ".cs", code);
				}
				if (!string.IsNullOrEmpty(config.EntityPath) && to.Entity && config.DesignPattern != "Model-DAL-BLL") {
					string code = TableStructureFactory.GetTableStructCode(config, tabName, config.Project, out baseCode, out sqlCode, isView);
					Pub.Class.FileDirectory.DirectoryCreate(config.EntityPath + "\\Entity\\");
					Pub.Class.FileDirectory.FileDelete(config.EntityPath + "\\Entity\\" + tabName + ".cs");
					FileDirectory.FileWrite(config.EntityPath + "\\Entity\\" + tabName + ".cs", code);
				}

				if (config.DesignPattern == "Model-DAL-BLL" && !string.IsNullOrEmpty(config.DALPath) && to.Entity) {
					Pub.Class.FileDirectory.DirectoryCreate(config.DALPath + "\\" + config.TemplateName.Split('-')[3] + "DAL" + "\\");
					Pub.Class.FileDirectory.FileDelete(config.DALPath + "\\" + config.TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs");

					Pub.Class.FileDirectory.DirectoryCreate(config.IDALPath + "\\IDAL\\");
					Pub.Class.FileDirectory.FileDelete(config.IDALPath + "\\IDAL\\I" + tabName + "DAL.cs");

					if (!string.IsNullOrEmpty(dalCode.Trim())) {
						FileDirectory.FileWrite(config.DALPath + "\\" + config.TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs", dalCode);
						FileDirectory.FileWrite(config.IDALPath + "\\IDAL\\I" + tabName + "DAL.cs", idalCode);
					} else {
						Pub.Class.FileDirectory.FileDelete(config.DALPath + "\\" + config.TemplateName.Split('-')[3] + "DAL" + "\\" + tabName + "DAL.cs");
						Pub.Class.FileDirectory.FileDelete(config.IDALPath + "\\IDAL\\I" + tabName + "DAL.cs");
					}

					Pub.Class.FileDirectory.DirectoryCreate(config.BLLPath + "\\BLL\\");
					Pub.Class.FileDirectory.FileDelete(config.BLLPath + "\\BLL\\" + tabName + "BLL.cs");
					if (!string.IsNullOrEmpty(bllCode.Trim())) {
						FileDirectory.FileWrite(config.BLLPath + "\\BLL\\" + tabName + "BLL.cs", bllCode);

						sbSqlCode.AppendLine(sqlCode);
					} else {
						Pub.Class.FileDirectory.FileDelete(config.BLLPath + "\\BLL\\" + tabName + "BLL.cs");
					}
				}
				if (config.DesignPattern != "Model-DAL-BLL" && !string.IsNullOrEmpty(config.FactoryPath) && to.Entity) {
					Pub.Class.FileDirectory.DirectoryCreate(config.FactoryPath + "\\" + config.DesignPatternExtName + "\\");
					Pub.Class.FileDirectory.FileDelete(config.FactoryPath + "\\" + config.DesignPatternExtName + "\\" + tabName + "" + config.DesignPatternExtName + ".cs");
					if (!string.IsNullOrEmpty(baseCode.Trim())) {
						FileDirectory.FileWrite(config.FactoryPath + "\\" + config.DesignPatternExtName + "\\" + tabName + "" + config.DesignPatternExtName + ".cs", baseCode);

						sbSqlCode.AppendLine(sqlCode);
					} else {
						Pub.Class.FileDirectory.FileDelete(config.FactoryPath + "\\" + config.DesignPatternExtName + "\\" + tabName + "" + config.DesignPatternExtName + ".cs");
					}
				}
				WriteLog(tabName + " 生成成功！");
			}
			if (!string.IsNullOrEmpty(config.AdminPath) && config.IsAll) 
				FileDirectory.FileWrite(config.AdminPath + "\\xml\\db.aspx", "</div>");

			if (!string.IsNullOrEmpty(config.DALPath)) {
				string extFile = Server2.GetMapPath("") + "\\ext\\Sql\\SqlCode.sql";
				string extCode = FileDirectory.FileReadAll(extFile, Encoding.UTF8).ToString();
				Pub.Class.FileDirectory.DirectoryCreate(Server2.GetMapPath("") + "\\SQLCode\\");
				string code = sbSqlCode.ToString() + "\r\n" + extCode;
				if (code.Trim().Length > 10) FileDirectory.FileWrite(Server2.GetMapPath("") + "\\SQLCode\\SQLCode" + Rand.RndDateStr() + ".sql", sbSqlCode.ToString() + "\r\n" + extCode);
			}
			WriteLog("共 {0} 张表！", tables.Count);
			WriteLog("END");
			Input(exit);
		}
		public static void Init(bool exit = false) {
			tables.Clear();
			config.OPList = TableStructureFactory.LoadOPList(config, entity => {
				tables.Add((entity.isView ? "* " : "") + entity.Name);
			});
			Console.Write("Table".PadRight(20, ' '));
			Console.Write(" | E ");
			Console.Write("| I ");
			Console.Write("| U ");
			Console.Write("| UI ");
			Console.Write("| D ");
			Console.Write("| E ");
			Console.Write("| S ");
			Console.Write("| SP ");
			Console.Write("| FK ");
			Console.Write("| All ");
			Console.WriteLine();
			WriteLog("-".PadLeft(70, '-'));
			foreach (string key in tables) {
				string tabName = key;
				if (tabName.IndexOf("* ") == 0) tabName = tabName.Substring(2);

				TableOperator to = config.OPList.Where(p => p.Table == tabName).FirstOrDefault();
				Console.Write(key.SubString(20, "").PadRight(20, ' '));
				Console.Write(" | {0} ", to.Entity ? 1 : 0);
				Console.Write("| {0} ", to.Insert ? 1 : 0);
				Console.Write("| {0} ", to.Update ? 1 : 0);
				Console.Write("| {0}  ", to.UpdateAndInsert ? 1 : 0);
				Console.Write("| {0} ", to.DeleteByID ? 1 : 0);
				Console.Write("| {0} ", to.IsExistByID ? 1 : 0);
				Console.Write("| {0} ", to.SelectByID ? 1 : 0);
				Console.Write("| {0}  ", to.SelectPageList ? 1 : 0);
				Console.Write("| {0}  ", to.SelectListByFK ? 1 : 0);
				Console.Write("| {0}   ", to.SelectListByAll ? 1 : 0);
				Console.WriteLine();
			}
			WriteLog("END");
			Input(exit);
		}
		public static void Config(bool exit = false) {
			config = TableStructureFactory.GetConfig();
			WriteLog("EntityTool.exe.config");
			WriteLog("======数据库===================================================================");
			WriteLog("数据库：{0}", Data.DBType);
			WriteLog("连接字符串(ConnString)：{0}", Data.ConnString);
			WriteLog("======项目=====================================================================");
			WriteLog("项目名(Project)：{0}", config.Project);
			WriteLog("作者(Author)：{0}", config.Author);
			WriteLog("项目开始时间(ProjectStartDate)：{0}", config.ProjectStartDate);
			WriteLog("版权(CopyRight)：{0}", config.CopyRight);
			WriteLog("======设计模式=================================================================");
			WriteLog("设计模式(DesignPattern)：{0}", config.DesignPattern);
			WriteLog("生成代码模板(TemplateName)：{0}", config.TemplateName);
			if (config.DesignPattern == "Model-DAL-BLL") {
				WriteLog("实体类生成路径(ModelPath)：{0}", config.ModelPath);
				WriteLog("数据操作类生成路径(DALPath)：{0}", config.DALPath);
				WriteLog("数据操作接口生成路径(IDALPath)：{0}", config.IDALPath);
				WriteLog("业务处理类生成路径(BLLPath)：{0}", config.BLLPath);
			} else {
				WriteLog("设计模式后缀(DesignPatternExtName)：{0}", config.DesignPatternExtName);
				WriteLog("实体类生成路径(EntityPath)：{0}", config.EntityPath);
				WriteLog("工厂类生成路径(FactoryPath)：{0}", config.FactoryPath);
			}
			WriteLog("数据分页(PagerSqlEnum)：{0}", config.PagerSqlEnum);
			if (!config.AdminPath.IsNullEmpty()) {
				WriteLog("======后台=====================================================================");
				WriteLog("后台生成路径(AdminPath)：{0}", config.AdminPath);
				WriteLog("后台分页大小默认(PageSize)：{0}", config.PageSize);
				WriteLog("使用单页(UseOneProject)：{0}", config.UseOneProject.ToString());
			}
			WriteLog("END");
			Input(exit);
		}
		public static void Help(bool exit = false) {
			int len = 15;
			Console.Clear();
			WriteLog("实体类生成工具：");
			WriteLog("1,请先设置配置文件(vi EntityTool.exe.config)");
			Console.Write("  config(c)".PadRight(len, ' '));
			WriteLog("查看配置");
			WriteLog("2,然后初始化项目配置文件和设置项目配置文件(vi 项目名.xml)");
			Console.Write("  init(i)".PadRight(len, ' '));
			WriteLog("初始化项目配置文件");
			WriteLog("3,生成实体类代码");
			Console.Write("  run(r)".PadRight(len, ' '));
			WriteLog("生成代码");
			Console.Write("help(h)".PadRight(len, ' '));
			WriteLog("帮助");
			Console.Write("quit(q)".PadRight(len, ' '));
			WriteLog("退出");
			WriteLog("");
			Input(exit);
		}
	}
}