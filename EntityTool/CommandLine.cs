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
using System.Collections.Generic;

namespace EntityTool {
	public class CommandLine {
		private static Config config = null;
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
		}
		public static void Init(bool exit = false) {

		}
		public static void Config(bool exit = false) {
			int len = 25;
			Config config = TableStructureFactory.GetConfig();
			WriteLog("EntityTool.exe.config");
			Console.Write("项目名(Project)：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Console.Write("()：".PadRight(len, ' ')); WriteLog(config.Project);
			Input(exit);
		}
		public static void Help(bool exit = false) {
			int len = 25;
			Console.Clear();
			WriteLog("实体类生成工具：");
			WriteLog("1,请先设置配置文件(vi EntityTool.exe.config)");
			Console.Write("config(c)".PadRight(len, ' '));
			WriteLog("查看配置");
			WriteLog("2,然后初始化项目配置文件和设置项目配置文件(vi 项目名.xml)");
			Console.Write("init(i)".PadRight(len, ' '));
			WriteLog("初始化项目配置文件");
			WriteLog("3,生成实体类代码");
			Console.Write("run(r)".PadRight(len, ' '));
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