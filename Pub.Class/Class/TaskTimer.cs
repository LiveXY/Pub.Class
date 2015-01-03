//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Web;
using System.Timers;
using System.IO;
using System.Threading;

namespace Pub.Class {
    /// <summary>
    /// 时间单位
    /// </summary>
    public enum TimerFormat {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 天
        /// </summary>
        Day,
        /// <summary>
        /// 周
        /// </summary>
        Week,
        /// <summary>
        /// 月
        /// </summary>
        Month,
        /// <summary>
        /// 年
        /// </summary>
        Year
    }
    /// <summary>
    /// 定时执行任务实体类
    /// 
    /// 修改纪录
    ///     2012.02.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class TaskInfo {
        /// <summary>
        /// 构造器 初始化数据
        /// </summary>
        public TaskInfo() {
            Interval = 0;
            HHmm = null;
            Class = null;
            Assembly = null;
            Parameter = null;
            Format = TimerFormat.None;
            Degree = 0;
        }
        /// <summary>
        /// 指定每多少秒执行
        /// </summary>
        public int Interval { set; get; }
        /// <summary>
        /// 指定具体的时间执行
        /// </summary>
        public string HHmm { set; get; }
        /// <summary>
        /// 动态调用的类名 如:Pub.Class.SharpZip.Compress
        /// </summary>
        public string Class { set; get; }
        /// <summary>
        /// 动态调用的DLL或程序集 如：Pub.Class.SharpZip或plugin/Pub.Class.SharpZip.dll
        /// </summary>
        public string Assembly { set; get; }
        /// <summary>
        /// 动态调用的时候传参数
        /// </summary>
        public string Parameter { set; get; }
        /// <summary>
        /// 执行次数 0为无限次
        /// </summary>
        public int Degree { set; get; }
        /// <summary>
        /// 格式化
        /// </summary>
        public TimerFormat Format { set; get; }
    }

    /// <summary>
    /// 批量定时执行任务
    /// 
    /// 修改纪录
    ///     2012.02.17 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// TasksTimer.AddTask(new TaskInfo() { Assembly = "Pub.Class.JobTest", Class = "Pub.Class.JobTest.Job1", Interval = 2 });
    /// TasksTimer.AddTask(new TaskInfo() { Assembly = "Pub.Class.JobTest", Class = "Pub.Class.JobTest.Job2", Interval = 4 });
    /// TasksTimer.AddTask(new TaskInfo() { Assembly = "Pub.Class.JobTest", Class = "Pub.Class.JobTest.Job3", Interval = 1, Degree = 5 });
    /// TasksTimer.AddTask(new TaskInfo() { Assembly = "Pub.Class.JobTest", Class = "Pub.Class.JobTest.Job4", HHmm = "14:05", Parameter = "job4" });
    /// </code>
    /// </example>
    /// </summary>
    public static class TasksTimer {
        private static readonly ISafeDictionary<string, Timer> tasksInfo = new SafeDictionary<string, Timer>();
        /// <summary>
        /// 批量添加任务
        /// </summary>
        /// <param name="taskInfoList">任务列表</param>
        public static void AddTasks(IList<TaskInfo> taskInfoList) {
            foreach (var info in taskInfoList) AddTask(info);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task">任务</param>
        public static void AddTask(TaskInfo task) {
            if (tasksInfo.ContainsKey(task.Class)) RemoveTask(task.Class);

            Timer timer = new Timer(true);
            Action action = () => SingletonAddIn<IPlugin>.Instance(task.Class + "," + task.Assembly).Main(task.Parameter);
            if (task.Interval > 0) timer.Start(action, task.Interval, task.Degree);
            if (!task.HHmm.IsNullEmpty()) timer.Start(action, task.HHmm, task.Degree, task.Format);
            if (!tasksInfo.ContainsKey(task.Class)) tasksInfo.Add(task.Class, timer);
        }
        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="className">类名</param>
        public static void RemoveTask(string className) {
            if (tasksInfo.ContainsKey(className)) {
                tasksInfo[className].Stop();
                tasksInfo.Remove(className);
            }
        }
        /// <summary>
        /// 删除所有任务
        /// </summary>
        public static void RemoveAll() {
            foreach (var info in tasksInfo) RemoveTask(info.Key);
        }
    }

    /// <summary>
    /// 定时执行线程安全
    /// 
    /// 修改纪录
    ///     2011.07.08 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// TaskTimer.Instance().Start(()=>{},1);
    /// TaskTimer.Instance().Start(()=>{},"01:00");
    /// TaskTimer.Instance().Stop();
    /// </code>
    /// </example>
    /// </summary>
    public class TaskTimer {
        private static object lockHelper = new object();
        /// <summary>
        /// TaskTimer 实例
        /// </summary>
        protected static Timer task = null;
        /// <summary>
        /// 单实例
        /// </summary>
        /// <returns></returns>
        public static Timer Instance(bool useThread) {
            if (task.IsNull()) {
                lock (lockHelper) {
                    if (task.IsNull()) {
                        task = new Timer(useThread);
                    }
                }
            }
            return task;
        }
        /// <summary>
        /// 单实例
        /// </summary>
        /// <returns></returns>
        public static Timer Instance() {
            return Instance(false);
        }
    }

    /// <summary>
    /// 定时执行
    /// 
    /// 修改纪录
    ///     2012.02.17 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// Timer timer = new Timer();
    /// timer.Start(()=>{},"01:00");
    /// timer.Stop();
    /// 
    /// Timer timer = new Timer();
    /// timer.Action = ()=>{};
    /// timer.HHmm = "01:00"; 或 timer.Interval = 1;
    /// timer.Start();
    /// timer.Stop();
    /// </code>
    /// </example>
    /// </summary>
    public class Timer {
        /// <summary>
        /// 执行动作
        /// </summary>
        public Action exec = null;
        private System.Timers.Timer timer = null;
        private int initInterval = 0;
        private string hhmm = string.Empty;
        private bool useThread = false;
        private bool end = false;
        private long index = 0;
        private int degree = 0;
        private TimerFormat format = TimerFormat.None;
        /// <summary>
        /// 执行代码
        /// </summary>
        public Action Action { set { exec = value; } get { return exec; } }
        /// <summary>
        /// 每多少秒执行
        /// </summary>
        public int Interval { set { initInterval = value; hhmm = string.Empty; } get { return initInterval; } }
        /// <summary>
        /// 指定时间执行
        /// </summary>
        public string HHmm { set { hhmm = value; initInterval = 0; } get { return hhmm; } }
        /// <summary>
        /// 执行结束
        /// </summary>
        public bool End { get { return end; } }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="useThread">是否使线程</param>
        public Timer(bool useThread) { this.useThread = useThread; }
        /// <summary>
        /// 构造器
        /// </summary>
        public Timer() { useThread = false; }
        /// <summary>
        /// 每interval秒执行
        /// </summary>
        /// <param name="action">动作</param>
        /// <param name="interval">秒</param>
        /// <param name="degree">次数</param>
        public void Start(Action action, int interval = 1, int degree = 0) {

            Stop();
            if (interval < 1) interval = 1;
            this.degree = degree;
            if (timer.IsNull()) {
                exec = action;
                timer = new System.Timers.Timer(interval * 1000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                timer.Enabled = true;
                timer.Start();
            }
        }
        /// <summary>
        /// 具体什么时间执行
        /// </summary>
        /// <param name="action">动作</param>
        /// <param name="HHmm">时分</param>
        /// <param name="degree">次数</param>
        /// <param name="format">时间单位</param>
        public void Start(Action action, string HHmm, int degree = 0, TimerFormat format = TimerFormat.Day) {
            Stop();
            this.degree = degree;
            this.format = format;
            if (timer.IsNull()) {
                exec = action;
                hhmm = HHmm;
                timer = new System.Timers.Timer(1000 * 60);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer2_Elapsed);
                timer.Enabled = true;
                timer.Start();
            }
        }
        /// <summary>
        /// 每1秒执行
        /// </summary>
        /// <param name="action">动作</param>
        /// <param name="degree">次数</param>
        public void Start(Action action, int degree = 0) {
            Start(action, 1, degree);
        }
        /// <summary>
        /// 开始执行
        /// </summary>
        public void Start(int degree = 0) {
            if (initInterval > 0) Start(exec, initInterval, degree);
            if (!hhmm.IsNullEmpty()) Start(exec, hhmm, degree);
        }
        /// <summary>
        /// 每interval秒执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (!exec.IsNull() || !end) {
                if (useThread) new Thread(() => { exec(); }).Start(); else exec();
                if (degree > 0 && !end) index++;
                if (degree > 0 && degree <= index) { end = true; Stop(); }
            }
        }
        /// <summary>
        /// 具体什么时间执行 HH:mm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if ((!exec.IsNull() || !end)) {
                if (format == TimerFormat.Day && DateTime.Now.ToString("HH:mm") == hhmm) {
                    if (useThread) new Thread(() => { exec(); }).Start(); else exec();
                    if (degree > 0 && !end) index++;
                    if (degree > 0 && degree <= index) { end = true; Stop(); }
                } else if (format == TimerFormat.Week && DateTime.Now.ToString("dddd HH:mm") == hhmm) {
                    if (useThread) new Thread(() => { exec(); }).Start(); else exec();
                    if (degree > 0 && !end) index++;
                    if (degree > 0 && degree <= index) { end = true; Stop(); }
                } else if (format == TimerFormat.Month && DateTime.Now.ToString("dd HH:mm") == hhmm) {
                    if (useThread) new Thread(() => { exec(); }).Start(); else exec();
                    if (degree > 0 && !end) index++;
                    if (degree > 0 && degree <= index) { end = true; Stop(); }
                } else if (format == TimerFormat.Year && DateTime.Now.ToString("MM-dd HH:mm") == hhmm) {
                    if (useThread) new Thread(() => { exec(); }).Start(); else exec();
                    if (degree > 0 && !end) index++;
                    if (degree > 0 && degree <= index) { end = true; Stop(); }
                }
            }
        }
        /// <summary>
        /// 停止执行
        /// </summary>
        public void Stop() {
            if (timer.IsNotNull()) {
                timer.Stop();
                timer.Dispose();
                timer = null;
                //exec = null;
            }
        }
    }
}