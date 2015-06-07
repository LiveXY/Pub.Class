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
		private static IDictionary<string, bool> tables = new Dictionary<string, bool>();
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
			WriteLog("END");
			Input(exit);
		}
		public static void Init(bool exit = false) {
			config.OPList = TableStructureFactory.LoadOPList(config, entity => { 
				tables.Add((entity.isView ? "* " : "") + entity.Name, entity.isView ? false : true);
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
			foreach(string key in tables.Keys) {
				TableOperator to = config.OPList.Where(p=>p.Table == key).FirstOrDefault();
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