//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace Pub.Class {
    /// <summary>
    /// 安全操作类
    /// 
    /// 修改纪录
    ///     2006.05.10 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Safe {
        //#region IsSafeUrl
        /// <summary>
        /// 不允许在本地提交数据
        /// </summary>
        /// <remarks>返回是否是安全URL</remarks>
        /// <param name="doMain">域名</param>
        public static bool IsSafeUrl(string doMain) {
            string url = Request2.GetReferrer().ToLower().Trim().Replace("http://", "").Replace("https://", "").Split('/')[0];
            doMain = doMain.ToLower().Trim();
            if (url.IndexOf(doMain) > -1) return true;
            return false;
        }
        //#endregion
        //#region Kill/Run/RestartIISProcess
        /// <summary>
        /// 进程启动数量
        /// </summary>
        /// <param name="processName">进程名</param>
        /// <returns></returns>
        public static int IsExistProcess(string processName) {
            int i = 0;
            Process[] Processes = Process.GetProcessesByName(processName);
            foreach (Process CurrentProcess in Processes) i++;
            return i;
        }
        /// <summary>
        /// 根据进程名获取PID
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static int GetPIDByProcessName(string processName) {
            Process[] arrayProcess = Process.GetProcessesByName(processName);

            foreach (Process p in arrayProcess) {
                return p.Id;
            }
            return 0;
        }
#if !MONO40
        /// <summary>
        /// 获取窗体的进程标识ID
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <returns></returns>
        public static int GetPIDByProcessWindowTitle(string windowTitle) {
            int rs = 0;
            Process[] arrayProcess = Process.GetProcesses();
            foreach (Process p in arrayProcess) {
                if (p.MainWindowTitle.IndexOf(windowTitle) != -1) {
                    rs = p.Id;
                    break;
                }
            }

            return rs;
        }
        /// <summary>
        /// 根据窗体标题查找窗口句柄（支持模糊匹配）
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static IntPtr FindWindowByProcessName(string title) {
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps) {
                if (p.MainWindowTitle.IndexOf(title) != -1) {
                    return p.MainWindowHandle;
                }
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// 进程启动数量
        /// </summary>
        /// <param name="processName">进程名</param>
        /// <returns></returns>
        public static IList<IntPtr> ProcessHandle(string processName) {
            IList<IntPtr> list = new List<IntPtr>();
            Process[] Processes = Process.GetProcessesByName(processName);
            foreach (Process CurrentProcess in Processes) { list.Add(CurrentProcess.MainWindowHandle); }
            return list;
        }
        /// <summary>
        /// 运行一个进程并等待它执行完成
        /// </summary>
        /// <param name="cmd">进程</param>
        /// <param name="arguments">参数</param>
        /// <param name="isOutput">是否返回输出数据</param>
        /// <param name="commands">执行多行命令</param>
        /// <returns>如果进程出错返回错误日志</returns>
        public static IntPtr RunWaitHandle(string cmd, string arguments = "", string[] commands = null) {
            cmd = "\"" + cmd + "\"";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process()) {
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                try {
                    p.Start();
                    if (commands.IsNotNull()) {
                        foreach (string command in commands) p.StandardInput.WriteLine(command);
                    }
                    p.WaitForExit();
                    p.Close();
                    return p.MainWindowHandle;
                } catch {
                    return IntPtr.Zero;
                }
            }
        }
        /// <summary>
        /// 运行一个进程并等待它执行完成 可指定窗口样式
        /// </summary>
        /// <param name="cmd">进程</param>
        /// <param name="arguments">参数</param>
        /// <param name="winStyle">窗口状态 最大化 最小化 隐藏</param>
        /// <returns>如果进程出错返回错误日志</returns>
        public static IntPtr RunWaitHandle(string cmd, System.Diagnostics.ProcessWindowStyle winStyle, string arguments = "") {
            cmd = "\"" + cmd + "\"";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process()) {
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.WindowStyle = winStyle;
                p.StartInfo.UseShellExecute = true;
                try {
                    p.Start();
                    p.WaitForExit();
                    p.Close();
                    return p.MainWindowHandle;
                } catch {
                    return IntPtr.Zero;
                }
            }
        }
#endif
        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="processName">进程名</param>
        public static void KillProcess(string processName) {
            if (string.IsNullOrEmpty(processName)) throw new ArgumentNullException("ProcessName");
            KillProcessAsync(processName, 0);
        }
        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="processName">进程名</param>
        /// <param name="TimeToKill">延时</param>
        public static void KillProcess(string processName, int TimeToKill) {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentNullException("ProcessName");
            ThreadPool.QueueUserWorkItem(delegate { KillProcessAsync(processName, TimeToKill); });
        }
        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="processName">进程名</param>
        /// <param name="TimeToKill">延时</param>
        public static void KillProcessAsync(string processName, int TimeToKill) {
            if (TimeToKill > 0) Thread.Sleep(TimeToKill);
            Process[] Processes = Process.GetProcessesByName(processName);
            foreach (Process CurrentProcess in Processes) {
                CurrentProcess.Kill();
            }
        }
        /// <summary>
        /// 运行一个进程并等待它执行完成
        /// </summary>
        /// <param name="cmd">进程</param>
        /// <param name="arguments">参数</param>
        /// <param name="isOutput">是否返回输出数据</param>
        /// <param name="commands">执行多行命令</param>
        /// <returns>如果进程出错返回错误日志</returns>
        public static string RunWait(string cmd, string arguments = "", bool isOutput = true, string[] commands = null) {
            cmd = "\"" + cmd + "\"";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process()) {
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = isOutput;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                try {
                    p.Start();
                    if (commands.IsNotNull()) {
                        foreach (string command in commands) p.StandardInput.WriteLine(command);
                    }
                    StringBuilder error = new StringBuilder();
                    error.AppendLine(p.StandardError.ReadToEnd().Trim());
                    if (isOutput) error.Append(" ").AppendLine(p.StandardOutput.ReadToEnd().Trim());
                    p.WaitForExit();
                    p.Close();
                    return error.ToString().Trim();
                } catch (Exception ex) {
                    return ex.ToExceptionDetail();
                }
            }
        }
        public static void RunWait(string cmd, string arguments, string[] commands, Action<string> msg = null) {
            cmd = "\"" + cmd + "\"";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process()) {
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                try {
                    p.Start();
                    if (commands.IsNotNull()) {
                        foreach (string command in commands) p.StandardInput.WriteLine(command);
                    }
                    while (p.WaitForExit(0) == false) {
                        if (msg != null) msg.BeginInvoke(p.StandardOutput.ReadLine(), null, null);
                    }
                    p.WaitForExit();
                    p.Close();
                } catch (Exception ex) {
                    if (msg != null) msg.BeginInvoke(ex.Message, null, null);
                }
            }
        }
        public static void RunWait(string cmd, string arguments, Action<string> msg) {
            RunWait(cmd, arguments, null, msg);
        }
        public static void RunWait(string cmd, string[] commands, Action<string> msg) {
            RunWait(cmd, "", commands, msg);
        }
        public static void RunWait(string cmd, Action<string> msg) {
            RunWait(cmd, "", null, msg);
        }
        /// <summary>
        /// 运行一个进程并等待它执行完成 可指定窗口样式
        /// </summary>
        /// <param name="cmd">进程</param>
        /// <param name="arguments">参数</param>
        /// <param name="winStyle">窗口状态 最大化 最小化 隐藏</param>
        /// <returns>如果进程出错返回错误日志</returns>
        public static string RunWait(string cmd, System.Diagnostics.ProcessWindowStyle winStyle, string arguments = "") {
            cmd = "\"" + cmd + "\"";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process()) {
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.WindowStyle = winStyle;
                p.StartInfo.UseShellExecute = true;
                try {
                    p.Start();
                    p.WaitForExit();
                    p.Close();
                    return string.Empty;
                } catch (Exception ex) {
                    return ex.ToExceptionDetail();
                }
            }
        }
        /// <summary>
        /// 异步执行进程
        /// </summary>
        /// <param name="cmd">进程</param>
        /// <param name="arguments">参数</param>
        /// <returns>如果进程出错返回错误日志</returns>
        public static string RunAsync(string cmd, string arguments = "") {
            cmd = "\"" + cmd + "\"";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process()) {
                p.StartInfo.FileName = cmd;
                if (!arguments.IsNullEmpty()) p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                try {
                    p.Start();
                    p.Close();
                    return string.Empty;
                } catch (Exception ex) {
                    return ex.ToExceptionDetail();
                }
            }
        }
        /// <summary>
        /// 异步执行进程
        /// </summary>
        /// <param name="cmd">进程</param>
        /// <param name="arguments">参数</param>
        /// <param name="winStyle">窗口状态 最大化 最小化 隐藏</param>
        /// <returns>如果进程出错返回错误日志</returns>
        public static string RunAsync(string cmd, System.Diagnostics.ProcessWindowStyle winStyle, string arguments = "") {
            cmd = "\"" + cmd + "\"";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process()) {
                p.StartInfo.FileName = cmd;
                if (!arguments.IsNullEmpty()) p.StartInfo.Arguments = arguments;
                p.StartInfo.WindowStyle = winStyle;
                p.StartInfo.UseShellExecute = true;
                try {
                    p.Start();
                    p.Close();
                    return string.Empty;
                } catch (Exception ex) {
                    return ex.ToExceptionDetail();
                }
            }
        }
        /// <summary>
        /// hack tip:通过更新web.config文件方式来重启IIS进程池（注：iis中web园数量须大于1,且为非虚拟主机用户才可调用该方法）
        /// </summary>
        public static void RestartIISProcess() {
            try {
                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                xmldoc.Load("~/web.config".GetMapPath());
                System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter("~/web.config".GetMapPath(), null);
                writer.Formatting = System.Xml.Formatting.Indented;
                xmldoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
            } catch { ; }
        }
        //#endregion
        //#region COOKIES防刷新页面代码
        /// <summary>
        /// 设置打开页面的时间
        /// </summary>
        public static void SetDateTime() {
            Cookie2.Set("__sysTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        /// <summary>
        /// 判断是否在指定多少秒内提交数据，来达到判断是否刷新页面的目的
        /// </summary>
        /// <param name="seconds">多少秒内</param>
        /// <returns>是/否</returns>
        public static bool IsRefresh(int seconds) {
            string _sysTime = Cookie2.Get("__sysTime");
            if (_sysTime.Trim() == "") return true;
            if (!_sysTime.IsDateTime()) return true;
            DateTime _startTime = DateTime.Parse(_sysTime);
            DateTime _endTime = DateTime.Now;
            TimeSpan _value = _startTime.GetTimeSpan(_endTime);
            if (_value.Seconds >= seconds) return false;
            else {
                Js.Alert("不允许刷新，或快速提交数据，请" + seconds.ToString() + "秒后提交数据。");
                return true;
            }
        }
        //#endregion
        //#region 安全提交/动态调用
        /// <summary>
        /// 安全提交，防本地提交
        /// </summary>
        /// <param name="doMain">域名</param>
        public static void SafeGetPost(string doMain) {
            if (string.IsNullOrEmpty(doMain)) return;
            bool isTrue = false;
            string[] doMainArr = doMain.Split('|');
            for (int i = 0; i <= doMainArr.Length - 1; i++) if (Safe.IsSafeUrl(doMainArr[i])) isTrue = true;
            if (!isTrue) { Msg.Write("不允许在本地提交数据。"); Msg.End(); }
        }
        /// <summary>
        /// 加载DLL的方法 不支持重载方法 速度慢
        /// </summary>
        /// <example>
        /// <code>
        /// Safe.DllInvoke("../bin/Pub.Class.dll".GetMapPath(), "Pub.Class", "Session2", "Set", new object[] { "test", "3" });
        /// Msg.Write(Safe.DllInvoke("../bin/Pub.Class.dll".GetMapPath(), "Pub.Class", "Session2", "Get", new object[] { "test" }));
        /// </code>
        /// </example>
        /// <param name="DllFileName">dll全路径</param>
        /// <param name="NameSpace">命名空间</param>
        /// <param name="ClassName">类名</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="ObjArrayParams">参数</param>
        /// <returns>返回值</returns>
        public static object DllInvoke(string DllFileName, string NameSpace, string ClassName, string MethodName, object[] ObjArrayParams) {
            Assembly DllAssembly = Assembly.LoadFrom(DllFileName);
            Type[] DllTypes = DllAssembly.GetTypes();
            foreach (Type DllType in DllTypes) {
                if (DllType.Namespace == NameSpace && DllType.Name == ClassName) {
                    MethodInfo MyMethod = DllType.GetMethod(MethodName);
                    if (MyMethod.IsNotNull()) {
                        object mObject = Activator.CreateInstance(DllType);
                        return MyMethod.Invoke(mObject, ObjArrayParams);
                    }
                }
            }
            return (object)0;
        }
        //#endregion
        //#region 错误消息
        /// <summary>
        /// 显示详细的出错信息
        /// </summary>
        /// <param name="ex">Exception ex</param>
        /// <returns></returns>
        public static string Expand(Exception ex) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*******************************************************************************************************");
            if (!HttpContext.Current.IsNull()) {
                sb.AppendLine(string.Format("* DateTime :   {0}	IP：{1}	MemberID：{2}	OS：{3}	Brower：{4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Request2.GetIP(), "", Request2.GetOS(), Request2.GetBrowser()));
                sb.AppendLine("* Url      :   " + Request2.GetUrl());
                sb.AppendLine("* Request  :   " + Request2.GetRequest());
            }
            _expandException(ex, 1, sb);
            sb.AppendLine("*******************************************************************************************************");
            sb.AppendLine("");
            return sb.ToString();
        }
        /// <summary>
        /// 显示详细的出错信息
        /// </summary>
        /// <param name="ex">Exception ex</param>
        /// <param name="offSet"></param>
        /// <param name="sb"></param>
        private static void _expandException(Exception ex, int offSet, StringBuilder sb) {
            if (ex.IsNull()) return;
            Type t = ex.GetType();
            string paddingString = "";
            if (offSet > 1) paddingString = new String(' ', offSet * 4);
            sb.AppendFormat("{0}Exception:   {1}{2}", paddingString, t.Name, Environment.NewLine);
            sb.AppendFormat("{0}Message:     {1}{2}", paddingString, ex.Message, Environment.NewLine);
            sb.AppendFormat("{0}Source:      {1}{2}", paddingString, ex.Source, Environment.NewLine);
            if (ex.StackTrace.IsNotNull()) sb.AppendFormat("{0}Stack Trace: {1}{2}", paddingString, ex.StackTrace.Trim(), Environment.NewLine);
            if (ex.TargetSite.IsNotNull()) sb.AppendFormat("{0}Method:      {1}{2}", paddingString, ex.TargetSite.Name, Environment.NewLine);
            sb.AppendFormat("{0}Native:      {1}{2}", paddingString, ex.ToString(), Environment.NewLine);
            sb.AppendFormat("{0}Data:        {1}{2}", paddingString, expandData(ex.Data, offSet), Environment.NewLine);

            //Exception baseException = ex.GetBaseException();
            //if (baseException.IsNotNull()) sb.AppendFormat("{0}Base:        {1}{2}", paddingString, ex.GetBaseException(), Environment.NewLine);

            _expandException(ex.InnerException, offSet + 1, sb);
        }
        /// <summary>
        /// 显示详细的出错信息
        /// </summary>
        /// <param name="iDictionary">IDictionary</param>
        /// <param name="offSet">offSet</param>
        /// <returns></returns>
        private static string expandData(System.Collections.IDictionary iDictionary, int offSet) {
            StringBuilder sb = new StringBuilder();
            offSet += 4;
            string paddingString = "";
            if (offSet > 1) paddingString = new string(' ', offSet);

            sb.AppendFormat("{0}Total Data Entries: {1}{2}", paddingString, iDictionary.Count, Environment.NewLine);
            int counter = 1;
            paddingString = new string(' ', paddingString.Length + 4);
            foreach (DictionaryEntry de in iDictionary) {
                sb.AppendFormat("{0}{1}:[{2} {3}]  ", paddingString, counter++, de.Key.GetType().FullName, de.Key.ToString());
                if (de.Value.IsNotNull()) sb.AppendFormat("{0}", de.Value.ToString()); else sb.AppendFormat("{0}", "(null)");
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
        //#endregion
        /// <summary>
        /// string 转 结构体 char[]
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static void ToStructChars(ref char[] chars, string str, int len) {
            chars = new char[len];
            Array.Copy(str.PadRight(len, '\0').ToCharArray(), chars, len);
        }
        /// <summary>
        /// char[] 转 结构体 char[]
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static void ToStructChars(ref char[] chars, char[] str, int len) {
            chars = new char[len];
            Array.Copy(new string(str).PadRight(len, '\0').ToCharArray(), chars, len);
        }

    }
}
