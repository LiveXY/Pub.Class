using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ServiceProcess;

namespace EmailService {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            ServiceBase[] ServiceToRun;
            ServiceToRun = new ServiceBase[] { new MService() };
            ServiceBase.Run(ServiceToRun);
        }
    }
}