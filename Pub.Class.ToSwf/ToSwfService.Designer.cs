namespace Pub.Class.ToSwf {
    partial class ToSwfServiceBase {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.eventLog1 = new System.Diagnostics.EventLog();
            this.timer1 = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000D;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
            // 
            // ToSwfService
            // 
            this.AutoLog = false;
            this.CanHandlePowerEvent = true;
            this.CanShutdown = true;
            this.ServiceName = "ToSwfService";
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog eventLog1;
        private System.Timers.Timer timer1;
    }
}