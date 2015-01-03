using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ServiceProcess;

namespace ToSwfService {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            ServiceBase[] ServiceToRun;
            ServiceToRun = new ServiceBase[] { new ToSwfServiceBase() };
            ServiceBase.Run(ServiceToRun);
        }
    }
}