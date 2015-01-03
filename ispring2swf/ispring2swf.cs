using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Pub.Class;
using System.Threading;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Text;

namespace ispring2swf {
    public class ispring : System.Windows.Forms.Form {
        private ListBox listBox1;
        private System.ComponentModel.Container components = null;

        public ispring() {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(489, 330);
            this.listBox1.TabIndex = 0;
            // 
            // ispring
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(489, 330);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ispring";
            this.Text = "ispring2swf";
            this.Load += new System.EventHandler(this.ispring_Load);
            this.ResumeLayout(false);

        }

        public static string pptName = string.Empty;
        public static string swfName = string.Empty;
        public static string[] Keys = WebConfig.GetApp("SendKeys").Split('|');
        public static string DefaultPath = WebConfig.GetApp("DefaultPath").Trim('\\') + "\\";
        public static string[] KillProcessList = WebConfig.GetApp("KillProcessList").Split('|');
        public static bool UseLog = WebConfig.GetApp("UseLog").ToBool(false);
        public static string PPTCore = WebConfig.GetApp("PPTCore");

        [STAThread]
        static void Main(string[] args) {
            if (args.Length != 2 || args[0].IsNullEmpty() || args[1].IsNullEmpty()) {
                Environment.Exit(0);
            }
            pptName = args[0];
            swfName = args[1];
            Application.Run(new ispring());
        }

        private void ispring_Load(object sender, System.EventArgs e) {
            process(PPTCore);
        }

        public void process(string core) { 
            string chkcore = CheckPPTCore();
            if (core != chkcore &&! chkcore.IsNullEmpty()) core = chkcore;
            switch (core) {
                case "Microsoft PowerPoint 2010":
                    MPP2010();
                    break;
                case "Microsoft PowerPoint 2007":
                    MPP2007();
                    break;
                case "Microsoft PowerPoint 2003":
                    MPP2003();
                    break;
                case "Microsoft PowerPoint XP":
                    MPPXP();
                    break;
                case "Microsoft PowerPoint 2000":
                    MPP2000();
                    break;
                default:
                    WriteLog("未安装Microsoft PowerPoint组件！");
                    Exit();
                    break;
            }
        }

        public string CheckPPTCore() {
            List<string> officeKeys = Registry2.GetAllSubKey(RegistryEnum.LocalMachine, @"SOFTWARE\\Microsoft\\Office\\").ToList();
            if (officeKeys.Count < 1) return null;

            if (officeKeys.Contains("14.0")) return "Microsoft PowerPoint 2010";
            if (officeKeys.Contains("12.0")) return "Microsoft PowerPoint 2007";
            if (officeKeys.Contains("11.0")) return "Microsoft PowerPoint 2003";
            if (officeKeys.Contains("10.0")) return "Microsoft PowerPoint XP";
            if (officeKeys.Contains("9.0")) return "Microsoft PowerPoint 2000";
            return null;
        }

        public void MPP2000() {
            WriteLog("暂不支持Microsoft PowerPoint 2000...");
            Exit();
        }
        public void MPPXP() {
            WriteLog("暂不支持Microsoft PowerPoint XP...");
            Exit();
        }
        public void MPP2003() {
            WriteLog("暂不支持Microsoft PowerPoint 2003...");
            Exit();
        }
        public void MPP2007() {
            WriteLog("先关闭Microsoft PowerPoint 2007...");
            Safe.KillProcess("POWERPNT");
            WriteLog("ispring2swf \"" + pptName + "\" \"" + swfName + "\"");
            Safe.RunAsync("POWERPNT", System.Diagnostics.ProcessWindowStyle.Normal, "\"" + pptName + "\"");
            WriteLog("正在打开Microsoft PowerPoint 2007...");

            setTimeout((i2) => {
                IntPtr ppt = WinApi.FindWindow("PP12FrameClass", null);
                if (ppt != IntPtr.Zero) {
                    WriteLog("找到“Microsoft PowerPoint”主窗口并激活窗口！-" + ppt.ToString());
                    setTimeout(() => {
                        WinApi.SetActiveWindow(ppt);
                        foreach (string key in Keys) {
                            SendKeys.SendWait(key);
                            WriteLog("模拟用户按: " + (key == "%" ? "alt" : key) + "键！");
                            Thread.Sleep(100);
                        }
                        FileDirectory.FileDelete(swfName);
                        WriteLog("删除老的转换文件: " + swfName);
                        setTimeout(() => {
                            IList<WinApi.WindowInfo> childWindowNames = WinApi.GetAllDesktopWindows();
                            WinApi.WindowInfo info = childWindowNames.Where(p => p.szWindowName.IndexOf("Generating Flash Movie") != -1).FirstOrDefault();
                            if (info.IsNotNull() && info.hWnd != IntPtr.Zero) {
                                WriteLog("找到“Generating Flash Movie”窗口！-" + info.hWnd.ToString());
                                WriteLog("名称：" + info.szWindowName);
                                string fileName = info.szWindowName.Substring(23).Left(50);
                                string filePath = DefaultPath + fileName + "\\" + fileName + ".swf";

                                setTimeout((i3) => {
                                    if (!WinApi.IsWindow(info.hWnd)) {
                                        WriteLog("完成转换swf！");
                                        setTimeout(() => {
                                            IntPtr h = WinApi.FindWindow(null, fileName);
                                            if (h != IntPtr.Zero) {
                                                WriteLog("找到文件夹句柄，并关闭！");
                                                WinApi.SendMessage(h, WMessages.WM_CLOSE, 0, 0);
                                                return true;
                                            }
                                            WriteLog("找不到文件夹句柄：" + fileName);
                                            return false;
                                        }, 1000, 5);
                                        setTimeout(() => {
                                            if (FileDirectory.FileExists(filePath)) {
                                                setTimeout(() => {
                                                    FileDirectory.FileCopy(filePath, swfName, false);
                                                    FileDirectory.FileDelete(filePath);
                                                    FileDirectory.DirectoryDelete(filePath.GetParentPath('\\'));
                                                    Exit();
                                                    return true;
                                                }, 1000, 1);
                                                return true;
                                            }
                                            WriteLog("没找到转换后文件：" + filePath, true);
                                            return false;
                                        }, 1000, 60, () => {
                                            WriteLog("文件不存在，关闭！" + filePath);
                                            Exit();
                                        });
                                        return true;
                                    }
                                    WriteLog("正在转换swf-" + i3.ToString(), true);
                                    return false;
                                }, 2000, 0);
                                return true;
                            }
                            return false;
                        }, 1000, 60, () => {
                            WriteLog("找不到“Generating Flash Movie”窗口，退出！");
                            Exit();
                        });
                        return true;
                    }, 1000, 1);
                    return true;
                }
                WriteLog("查找“Microsoft PowerPoint”主窗口！-" + i2.ToString(), true);
                return false;
            }, 3000, 60, () => {
                WriteLog("找不到“Microsoft PowerPoint”主窗口，退出！");
                Exit();
            });
        }
        public void MPP2010() {
            WriteLog("先关闭Microsoft PowerPoint 2010...");
            Safe.KillProcess("POWERPNT");
            WriteLog("ispring2swf \"" + pptName + "\" \"" + swfName + "\"");
            Safe.RunAsync("POWERPNT", System.Diagnostics.ProcessWindowStyle.Normal, "\"" + pptName + "\"");
            WriteLog("正在打开Microsoft PowerPoint 2010...");

            setTimeout((i1) => {
                IntPtr splash = WinApi.FindWindow("MsoSplash", "正在打开 - Microsoft PowerPoint");
                if (splash != IntPtr.Zero) {
                    WriteLog("找到“正在打开 - Microsoft PowerPoint”窗口！-" + splash.ToString());
                    WriteLog("等待“正在打开 - Microsoft PowerPoint”窗口关闭！");
                    setTimeout(() => {
                        splash = WinApi.FindWindow("MsoSplash", "正在打开 - Microsoft PowerPoint");
                        if (splash == IntPtr.Zero) {
                            WriteLog("“正在打开 - Microsoft PowerPoint”窗口已关闭！");
                            setTimeout((i2) => {
                                IntPtr ppt = WinApi.FindWindow("PPTFrameClass", null);
                                if (ppt != IntPtr.Zero) {
                                    WriteLog("找到“Microsoft PowerPoint”主窗口并激活窗口！-" + ppt.ToString());
                                    setTimeout(() => {
                                        WinApi.SetActiveWindow(ppt);
                                        foreach (string key in Keys) {
                                            SendKeys.SendWait(key);
                                            WriteLog("模拟用户按: " + (key == "%" ? "alt" : key) + "键！");
                                            Thread.Sleep(100);
                                        }
                                        FileDirectory.FileDelete(swfName);
                                        WriteLog("删除老的转换文件: " + swfName);
                                        setTimeout(() => {
                                            IList<WinApi.WindowInfo> childWindowNames = WinApi.GetAllDesktopWindows();
                                            WinApi.WindowInfo info = childWindowNames.Where(p => p.szWindowName.IndexOf("Generating Flash Movie") != -1).FirstOrDefault();
                                            if (info.IsNotNull() && info.hWnd != IntPtr.Zero) {
                                                WriteLog("找到“Generating Flash Movie”窗口！-" + info.hWnd.ToString());
                                                WriteLog("名称：" + info.szWindowName);
                                                string fileName = info.szWindowName.Substring(23).Left(50);
                                                string filePath = DefaultPath + fileName + "\\" + fileName + ".swf";

                                                setTimeout((i3) => {
                                                    if (!WinApi.IsWindow(info.hWnd)) {
                                                        WriteLog("完成转换swf！");
                                                        setTimeout(() => {
                                                            IntPtr h = WinApi.FindWindow(null, fileName);
                                                            if (h != IntPtr.Zero) {
                                                                WriteLog("找到文件夹句柄，并关闭！");
                                                                WinApi.SendMessage(h, WMessages.WM_CLOSE, 0, 0);
                                                                return true;
                                                            }
                                                            WriteLog("找不到文件夹句柄：" + fileName);
                                                            return false;
                                                        }, 1000, 5);
                                                        setTimeout(() => {
                                                            if (FileDirectory.FileExists(filePath)) {
                                                                setTimeout(() => {
                                                                    FileDirectory.FileCopy(filePath, swfName, false);
                                                                    FileDirectory.FileDelete(filePath);
                                                                    FileDirectory.DirectoryDelete(filePath.GetParentPath('\\'));
                                                                    Exit();
                                                                    return true;
                                                                }, 1000, 1);
                                                                return true;
                                                            }
                                                            WriteLog("没找到转换后文件：" + filePath, true);
                                                            return false;
                                                        }, 1000, 60, () => {
                                                            WriteLog("文件不存在，关闭！" + filePath);
                                                            Exit();
                                                        });
                                                        return true;
                                                    }
                                                    WriteLog("正在转换swf-" + i3.ToString(), true);
                                                    return false;
                                                }, 2000, 0);
                                                return true;
                                            }
                                            return false;
                                        }, 1000, 60, () => {
                                            WriteLog("找不到“Generating Flash Movie”窗口，退出！");
                                            Exit();
                                        });
                                        return true;
                                    }, 1000, 1);
                                    return true;
                                }
                                WriteLog("查找“Microsoft PowerPoint”主窗口！-" + i2.ToString(), true);
                                return false;
                            }, 1000, 60, () => {
                                WriteLog("找不到“Microsoft PowerPoint”主窗口，退出！");
                                Exit();
                            });
                            return true;
                        }
                        return false;
                    }, 1000, 60, () => {
                        WriteLog("“正在打开 - Microsoft PowerPoint”未关闭，退出！");
                        Exit();
                    });
                    return true;
                }
                WriteLog("查找“正在打开 - Microsoft PowerPoint”窗口！-" + i1.ToString(), true);
                return false;
            }, 1000, 60, () => {
                WriteLog("找不到“正在打开 - Microsoft PowerPoint”窗口，退出！");
                Exit();
            });
        }
        public void Exit() {
            foreach (string pro in KillProcessList)
                if (!pro.IsNullEmpty()) Safe.KillProcess(pro);
            WriteLog("退出系统！");
            Environment.Exit(0);
        }
        public void WriteLog(string msg, bool selected = false) {
            listBox1.Items.Add(msg);
            if (selected) listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (UseLog) FileDirectory.FileWrite("log.txt".GetMapPath(), "[{0}] {1}".FormatWith(DateTime.Now.ToDateTime(), msg));
        }
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="run">执行函数</param>
        /// <param name="interval">计时器 1000为1秒</param>
        /// <param name="degree">跌代次数</param>
        /// <param name="error">连续返回false时执行</param>
        private static void setTimeout(Func<bool> run, int interval = 1000, int degree = 60, Action error = null) {
            int _degree = 0;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            Action end = () => {
                timer.Enabled = false;
                timer.Dispose();
            };
            timer.Interval = interval;
            timer.Enabled = true;
            timer.Tick += (s, e) => {
                if (degree < 1) {
                    if (run()) end();
                } else {
                    _degree++;
                    if (_degree >= degree) {
                        end();
                        if (!run() && error.IsNotNull()) error();
                    } else if (run()) end();
                }
            };
        }
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="run">执行函数</param>
        /// <param name="interval">计时器 1000为1秒</param>
        /// <param name="degree">跌代次数</param>
        /// <param name="error">连续返回false时执行</param>
        private static void setTimeout(Func<int, bool> run, int interval = 1000, int degree = 60, Action error = null) {
            int _degree = 0;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            Action end = () => {
                timer.Enabled = false;
                timer.Dispose();
            };
            timer.Interval = interval;
            timer.Enabled = true;
            timer.Tick += (s, e) => {
                if (degree < 1) {
                    _degree++;
                    if (run(_degree)) end();
                } else {
                    _degree++;
                    if (_degree >= degree) {
                        end();
                        if (!run(_degree) && error.IsNotNull()) error();
                    } else if (run(_degree)) end();
                }
            };
        }
    }
}
