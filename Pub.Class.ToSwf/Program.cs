using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ServiceProcess;

namespace Pub.Class.ToSwf {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            //ServiceBase[] ServiceToRun;
            //ServiceToRun = new ServiceBase[] { new ToSwfServiceBase() };
            //ServiceBase.Run(ServiceToRun);

            if (args.Length>0) {
                if (args[0] == "install") {
                    //Safe.Run("regsvr32", " /s \"" + "".GetMapPath() + "ispring\\sdk\\ActiveSWF\\ActiveSWF.dll\"");
                    //Safe.Run("regsvr32", " /s \"" + "".GetMapPath() + "ispring\\sdk\\iSpringSDK_2003.dll\"");
                    //Safe.Run("regsvr32", " /s \"" + "".GetMapPath() + "ispring\\sdk\\iSpringSDK_2007.dll\"");
                    Safe.RunWait("rundll32", " setupapi.dll,InstallHinfSection DefaultInstall 128 " + "install.inf".GetMapPath());
                    Safe.RunWait("sc", " config ToSwfService type= interact type= own");
                    System.Threading.Thread.Sleep(2000);
                    Safe.RunWait("sc", " start ToSwfService");
                } else if (args[0] == "uninstall") { 
                    //Safe.Run("regsvr32", " /u /s \"" + "".GetMapPath() + "ispring\\sdk\\ActiveSWF\\ActiveSWF.dll\"");
                    //Safe.Run("regsvr32", " /u /s \"" + "".GetMapPath() + "ispring\\sdk\\iSpringSDK_2003.dll\"");
                    //Safe.Run("regsvr32", " /u /s \"" + "".GetMapPath() + "ispring\\sdk\\iSpringSDK_2007.dll\"");
                    Safe.RunWait("rundll32", " setupapi.dll,InstallHinfSection DefaultUnInstall 128 " + "install.inf".GetMapPath());
                } else {
                    foreach (string file in args) {
                        string data = file.GetParentPath('\\').Trim();
                        string ip = data.GetParentPath('\\').Trim();
                        string[] list = data.Split('\\');
                        if (list.Length == 2) {
                            ip = file.GetParentPath('\\');
                            data = "";
                        } else data = list[list.Length - 2];
                        //MessageBox.Show(ip + "|" + data + "|" + file.GetFileName());
                        Pub.Class.ToSwfWCF.ToSwfBase.ToSwfFlv(new Pub.Class.ToSwfWCF.ToSwfData() {
                            IP = ip.Trim('\\'),
                            DataPath = data,
                            FilePath = file.GetFileName()
                        });
                    }
                }
                Application.Exit();
            } else {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                bool flag = false;
                System.Threading.Mutex mutex = new System.Threading.Mutex(true, System.Diagnostics.Process.GetCurrentProcess().ProcessName, out flag);
                if (flag) Application.Run(new frmMain());
                else { MessageBox.Show("程序已运行，请查看右下角图标！"); Environment.Exit(1);}//退出程序
            }
        }
    }
}