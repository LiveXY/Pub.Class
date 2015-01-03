using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Management;

namespace ToSwfService {
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer {
        public ProjectInstaller() {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e) {
            //SetServiceDesktopInsteract("ToSwfService");
            //System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController();
            //sc.ServiceName = "ToSwfService";
            //sc.Start();
        }

        private void SetServiceDesktopInsteract(string serviceName) {
            ConnectionOptions coOptions = new ConnectionOptions();
            coOptions.Impersonation = ImpersonationLevel.Impersonate;
            ManagementScope mgmtScope = new System.Management.ManagementScope(@"root\CIMV2", coOptions);
            mgmtScope.Connect();

            ManagementObject wmiService = new ManagementObject(string.Format("Win32_Service.Name='{0}'", serviceName));
            ManagementBaseObject changeMethod = wmiService.GetMethodParameters("Change");
            changeMethod["DesktopInteract"] = true;
            ManagementBaseObject OutParam = wmiService.InvokeMethod("Change", changeMethod, null);
        }

        private void serviceInstaller1_Committed(object sender, InstallEventArgs e) {
            SetServiceDesktopInsteract(serviceInstaller1.ServiceName);
        }

    }


}