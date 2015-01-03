//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using Pub.Class;
using System.Diagnostics;
using System.ServiceModel;

namespace Pub.Class.ToSwfWCF {
    /// <summary>
    /// 转换数据实体类
    /// 
    /// 修改纪录
    ///     2010.02.15 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class ToSwfData {
        /// <summary>
        /// 共享目录服务器IP
        /// </summary>
        public string IP { set; get; }
        /// <summary>
        /// 共享目录服务器登录账号
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 共享目录服务器登录密码 密码为空是不用账号，事先要先登录一次服务器并记住密码
        /// </summary>
        public string Password { set; get; }
        /// <summary>
        /// 共享目录服务器所共享的目录名 如：data、noahwm_data、xtt_data
        /// </summary>
        public string DataPath { set; get; }
        /// <summary>
        /// 共享目录服务器文件路径 km\1\test.doc 不包含共享目录名
        /// </summary>
        public string FilePath { set; get; }
        /// <summary>
        /// 要更新状态的数据库连接字符串的KEY
        /// </summary>
        public string DB { set; get; }
        /// <summary>
        /// 转换成功后更新状态SQL
        /// </summary>
        public string UpdateSQL { set; get; }
    }

    /// <summary>
    /// 转换服务操作类
    /// </summary>
    public class ToSwfBase {
        private static readonly object toLock = new object();
        /// <summary>
        /// 是否正在执行
        /// </summary>
        public static bool IsRun = false;
        /// <summary>
        /// 累计执行次数
        /// </summary>
        public static long Index = 0;
        /// <summary>
        /// 需要转换文件列表
        /// </summary>
        private static IList<ToSwfData> List = new List<ToSwfData>();
        /// <summary>
        /// 当前目录路径
        /// </summary>
        public static readonly string LocalPath = "".GetMapPath().TrimEnd('\\') + "\\";
        ///// <summary>
        ///// 服务器IP
        ///// </summary>
        //public static readonly string IP = WebConfig.GetApp("IP") ?? string.Empty;
        ///// <summary>
        ///// 服务器需要转换的文件通路
        ///// </summary>
        //public static readonly string DataPath = WebConfig.GetApp("DataPath") ?? "data";
        ///// <summary>
        ///// 服务器登录用户名
        ///// </summary>
        //public static readonly string UserName = WebConfig.GetApp("UserName") ?? "administrator";
        ///// <summary>
        ///// 服务器登录密码
        ///// </summary>
        //public static readonly string Password = WebConfig.GetApp("Password") ?? string.Empty;
        ///// <summary>
        ///// 服务器共享文件路径
        ///// </summary>
        //public static readonly string SharePath = "\\\\" + IP + "\\" + DataPath + "\\";
        /// <summary>
        /// 生成flashpaper还是flexpaper能使用的SWF文件
        /// </summary>
        public static readonly string UsePaper = (WebConfig.GetApp("UsePaper") ?? "flashpaper").ToLower();
        /// <summary>
        /// 如果使用flexpaper 是否每一页单独生成一个SWF
        /// </summary>
        public static readonly bool UsePaperSplit = (WebConfig.GetApp("UsePaperSplit") ?? "false").ToBool();
        /// <summary>
        /// 生成日志
        /// </summary>
        public static readonly bool UseLog = (WebConfig.GetApp("UseLog") ?? "false").ToBool();
        /// <summary>
        /// 添加水印
        /// </summary>
        public static readonly bool UseWatermark = (WebConfig.GetApp("UseWatermark") ?? "false").ToBool();
        /// <summary>
        /// 水印显示位置 0-9 9宫格
        /// </summary>
        public static readonly int WatermarkPosition = (WebConfig.GetApp("WatermarkPosition") ?? "0").ToInt(0);
        /// <summary>
        /// 水印图片
        /// </summary>
        public static readonly string WatermarkImage = WebConfig.GetApp("WatermarkImage") ?? "watermark.png";
        /// <summary>
        /// 使用iSpring转换
        /// </summary>
        public static readonly string UseiSpring = (WebConfig.GetApp("UseiSpring") ?? "").ToLower();
        /// <summary>
        /// ffmpeg转换的扩展名列表
        /// </summary>
        public static readonly string ffmpegExt = WebConfig.GetApp("ffmpegExt") ?? "*.asf;*.avi;*.mpg;*.3gp;*.mov;*.asx;*.wmv;*.mp4;";
        /// <summary>
        /// mencoder转换的扩展名列表
        /// </summary>
        public static readonly string mencoderExt = WebConfig.GetApp("mencoderExt") ?? "*.rmvb;*.rm;";
        /// <summary>
        /// office转换的扩展名列表
        /// </summary>
        public static readonly string officeExt = WebConfig.GetApp("officeExt") ?? "*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.pdf;*.dot;*.dotx;*.docm;*.dotm;*.xlsm;*.pptm;*.vsd;*.pub;*.pdf;";
        /// <summary>
        /// ispring转换的扩展名列表
        /// </summary>
        public static readonly string ispringExt = WebConfig.GetApp("ispringExt") ?? "*.ppt;*.pptx;";
        /// <summary>
        /// ffmpegApi路径
        /// </summary>
        public static readonly string ffmpegApiPath = LocalPath + "ffmpeg\\ffmpeg.exe";
        /// <summary>
        /// mencoderApi路径
        /// </summary>
        public static readonly string mencoderApiPath = LocalPath + "mencoder\\mencoder.exe";
        /// <summary>
        /// FlashPaperApi路径
        /// </summary>
        public static readonly string flashPrinterApiPath = LocalPath + "FlashPaper2.2\\FlashPrinter.exe";
        /// <summary>
        /// office2pdfApi路径
        /// </summary>
        public static readonly string officeToPDFApiPath = LocalPath + "office2pdf\\office2pdf.exe";
        /// <summary>
        /// pdfbgApi路径
        /// </summary>
        public static readonly string pdfbgApiPath = LocalPath + "pdfbg\\pdfbg.exe";
        /// <summary>
        /// pdf2swfApi路径
        /// </summary>
        public static readonly string pdfToSwfApiPath = LocalPath + "pdf2swf\\pdf2swf.exe";
        /// <summary>
        /// iSpringApi路径
        /// </summary>
        public static readonly string iSpringApiPath = LocalPath + "ispring\\" + UseiSpring + ".exe";

        /// <summary>
        /// 执行的路径
        /// </summary>
        public static string Url = string.Empty;
        /// <summary>
        /// 执行的事件
        /// </summary>
        public static event EventHandler OnNewOrDelete = null;
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="arguments"></param>
        public static void Run(string cmd, string arguments) {
            WriteLog(cmd + " " + arguments, "开始执行");
            string log = Safe.RunWait(cmd, ProcessWindowStyle.Hidden, arguments);
            WriteLog(cmd + " " + arguments, "执行成功");
            if (log.Length > 0) WriteLog(log, "生成日志");
        }
        /// <summary>
        /// 转换通知用接口，可转换office文档和各种视频
        /// </summary>
        /// <param name="filePath">全路径</param>
        public static void ToSwfFlv(ToSwfData data) {
            string api = string.Empty;
            string extName = "*" + data.FilePath.GetExtension().ToLower() + ";";
            if (ffmpegExt.IndexOf(extName) >= 0) api = "ffmpeg";
            if (mencoderExt.IndexOf(extName) >= 0) api = "mencoder";
            if (UseiSpring.Length > 0 && ispringExt.IndexOf(extName) >= 0) api = "ispring";
            else {
                if (officeExt.IndexOf(extName) >= 0) api = "flashpaper_doc";
                if (officeExt.IndexOf(extName) >= 0 && UsePaper == "flexpaper") api = "flexpaper_doc";
            }
            if (data.IP.IsIP() || data.IP.IsIPSect()) {
                data.FilePath = "\\\\" + data.IP + "\\" + data.DataPath + "\\" + data.FilePath;
            } else {
                data.FilePath = data.IP + "\\" + (data.DataPath.IsNullEmpty() ? "" : data.DataPath + "\\") + data.FilePath;
            }
            WriteLog(data.FilePath + "==================================================================================================================================", "==========启动转换文件");
            WriteLog(data, "ToSwfFlv " + api);
            if (!FileDirectory.FileExists(data.FilePath)) {
                WriteLog("文件不存在：", data.FilePath);
                return;
            }
            switch (api) {
                case "ffmpeg": FfmpegToFlv(data); break;
                case "mencoder": MencoderToFlv(data); break;
                case "flashpaper_doc": DocToFlashPaperSwf(data); break;
                case "flexpaper_doc": DocToFlexPaperSwf(data); break;
                case "ispring": iSpringPPTToSwf(data); break;
            }
            WriteLog(data.FilePath + "==================================================================================================================================", "==========结束转换文件");
        }
        /// <summary>
        /// 文档转FlashPaper可用SWF
        /// </summary>
        /// <param name="filePath"></param>
        public static void DocToFlashPaperSwf(ToSwfData data) {
            if (string.IsNullOrEmpty(data.Password)) { docToFlashPaperSwf(data); return; }
            using (new ShareConnect(data.IP, data.UserName, data.Password, LogonType.NewCredentials, LogonProvider.WinNT50)) { 
                WriteLog("与共享目录建立连接", "DocToFlashPaperSwf");
                docToFlashPaperSwf(data); 
            }
        }
        private static void docToFlashPaperSwf(ToSwfData data) {
            WriteLog("开始转换", "DocToFlashPaperSwf");
            if (!FileDirectory.FileExists(flashPrinterApiPath)) { WriteLog("接口不存在：", flashPrinterApiPath); return; }
            string swfName = System.IO.Path.ChangeExtension(data.FilePath, ".swf");
            string arguments = " \"{0}\" -o \"{1}\"".FormatWith(data.FilePath, swfName);
            Run(flashPrinterApiPath, arguments);
            Url = "ok - [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] - " + flashPrinterApiPath + arguments;
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
            if (!FileDirectory.FileExists(swfName)) { WriteLog(swfName + " 文件不存在", "DocToFlashPaperSwf"); return; }
            WriteLog(Url, "DocToFlashPaperSwf");
            if ((!data.DB.IsNullEmpty()) && (!data.UpdateSQL.IsNullEmpty())) Data.Pool(data.DB).ExecSql(data.UpdateSQL);
        }
        /// <summary>
        /// 文档转FlexPaper可用SWF
        /// </summary>
        /// <param name="filePath"></param>
        public static void DocToFlexPaperSwf(ToSwfData data) {
            if (string.IsNullOrEmpty(data.Password)) { docToFlexPaperSwf(data); return; }
            using (new ShareConnect(data.IP, data.UserName, data.Password, LogonType.NewCredentials, LogonProvider.WinNT50)) { 
                WriteLog("与共享目录建立连接", "DocToFlexPaperSwf");
                docToFlexPaperSwf(data); 
            }
        }
        private static void docToFlexPaperSwf(ToSwfData data) {
            string extName = "*" + data.FilePath.GetExtension().ToLower() + ";";
            if (extName != "*.pdf;") {
                OfficeToPDF(data);
                data.FilePath = System.IO.Path.ChangeExtension(data.FilePath, ".pdf");
                if (UseWatermark) PDFAddWatermark(data);
            }
            if (UsePaperSplit) PDFToSwfN(data); else PDFToSwf(data);
            if (extName != "*.pdf;") FileDirectory.FileDelete(data.FilePath);
            if ((!data.DB.IsNullEmpty()) && (!data.UpdateSQL.IsNullEmpty())) Data.Pool(data.DB).ExecSql(data.UpdateSQL);
        }
        /// <summary>
        /// pdf添加水印
        /// </summary>
        /// <param name="data"></param>
        private static void PDFAddWatermark(ToSwfData data) {
            WriteLog("开始添加水印", "PDFAddWatermark");
            if (!FileDirectory.FileExists(pdfbgApiPath)) { WriteLog("接口不存在：", pdfbgApiPath); return; }
            string pdfName = data.FilePath + ".pdf";
            string arguments = " \"{0}\" \"{1}\" \"{2}\" {3}".FormatWith(data.FilePath, WatermarkImage, pdfName, WatermarkPosition);
            Run(pdfbgApiPath, arguments);
            Url = "ok - [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] - " + pdfbgApiPath + arguments;
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
            if (!FileDirectory.FileExists(pdfName)) { WriteLog(pdfName + " 文件不存在", "PDFAddWatermark"); return; }
            FileDirectory.FileDelete(data.FilePath);
            FileDirectory.FileRename(pdfName, data.FilePath);
            WriteLog(Url, "PDFAddWatermark");
        }
        /// <summary>
        /// 文档转PDF
        /// </summary>
        /// <param name="filePath"></param>
        public static void OfficeToPDF(ToSwfData data) {
            WriteLog("开始转换", "OfficeToPDF");
            if (!FileDirectory.FileExists(officeToPDFApiPath)) { WriteLog("接口不存在：", officeToPDFApiPath); return; }
            string pdfName = System.IO.Path.ChangeExtension(data.FilePath, ".pdf");
            string arguments = " \"{0}\" \"{1}\"".FormatWith(data.FilePath, pdfName);
            Run(officeToPDFApiPath, arguments);
            Url = "ok - [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] - " + officeToPDFApiPath + arguments;
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
            if (!FileDirectory.FileExists(pdfName)) { WriteLog(pdfName + " 文件不存在", "OfficeToPDF"); return; }
            WriteLog(Url, "OfficeToPDF");
        }
        /// <summary>
        /// PDF转SWF 单个SWF
        /// </summary>
        /// <param name="filePath"></param>
        public static void PDFToSwf(ToSwfData data) {
            WriteLog("开始转换", "PDFToSwf");
            if (!FileDirectory.FileExists(pdfToSwfApiPath)) { WriteLog("接口不存在：", pdfToSwfApiPath); return; }
            string swfName = System.IO.Path.ChangeExtension(data.FilePath, ".swf");
            string arguments = " -t \"{0}\" -o \"{1}\" -s flashversion=9 -s poly2bitmap".FormatWith(data.FilePath, swfName);
            Run(pdfToSwfApiPath, arguments);
            Url = "ok - [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] - " + pdfToSwfApiPath + arguments;
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
            if (!FileDirectory.FileExists(swfName)) { WriteLog(swfName + " 文件不存在", "PDFToSwf"); return; }
            WriteLog(Url, "PDFToSwf");
        }
        /// <summary>
        /// PDF转SWF 多个SWF
        /// </summary>
        /// <param name="filePath"></param>
        public static void PDFToSwfN(ToSwfData data) {
            if (!FileDirectory.FileExists(pdfToSwfApiPath)) { WriteLog("接口不存在：", pdfToSwfApiPath); return; }
            string swfName = System.IO.Path.ChangeExtension(data.FilePath, ".swf");
            swfName = swfName.Substring(0, swfName.Length - 4) + "%.swf";
            //swfName = System.IO.Path.GetDirectoryName(swfName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(swfName) + "%.swf";
            string arguments = " \"{0}\" -o \"{1}\" -f -T 9 -t -s storeallcharacters -s poly2bitmap".FormatWith(data.FilePath, swfName);
            Run(pdfToSwfApiPath, arguments);
            Url = "ok - [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] - " + pdfToSwfApiPath + arguments;
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
            if (!FileDirectory.FileExists(swfName.Replace("%.swf", "1.swf"))) { WriteLog(swfName + " 文件不存在", "PDFToSwfN"); return; }
            WriteLog(Url, "PDFToSwfN");
        }
        /// <summary>
        /// Ffmpeg转Flv
        /// </summary>
        /// <param name="fileName"></param>
        public static void FfmpegToFlv(ToSwfData data) {
            if (string.IsNullOrEmpty(data.Password)) { ffmpegToFlv(data); return; }
            using (new ShareConnect(data.IP, data.UserName, data.Password, LogonType.NewCredentials, LogonProvider.WinNT50)) { 
                WriteLog("与共享目录建立连接", "FfmpegToFlv");
                ffmpegToFlv(data); 
            }
        }
        private static void ffmpegToFlv(ToSwfData data) {
            WriteLog("开始转换", "FfmpegToFlv");
            if (!FileDirectory.FileExists(ffmpegApiPath)) { WriteLog("接口不存在：", ffmpegApiPath); return; }
            string flvName = System.IO.Path.ChangeExtension(data.FilePath, ".flv");
            string arguments = " -i \"" + data.FilePath + "\" -y -ab 56 -ar 22050 -b 500k -r 15 \"" + flvName + "\"";
            Run(ffmpegApiPath, arguments);
            Url = "ok - [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] - " + ffmpegApiPath + arguments;
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
            if (!FileDirectory.FileExists(flvName)) { WriteLog(flvName + " 文件不存在", "FfmpegToFlv"); return; }
            WriteLog(Url, "FfmpegToFlv");
            if ((!data.DB.IsNullEmpty()) && (!data.UpdateSQL.IsNullEmpty())) Data.Pool(data.DB).ExecSql(data.UpdateSQL);
        }
        /// <summary>
        /// Mencoder转Flv
        /// </summary>
        /// <param name="fileName"></param>
        public static void MencoderToFlv(ToSwfData data) {
            if (string.IsNullOrEmpty(data.Password)) { mencoderToFlv(data); return; }
            //fileName = SharePath + fileName.Substring(5);
            using (new ShareConnect(data.IP, data.UserName, data.Password, LogonType.NewCredentials, LogonProvider.WinNT50)) { 
                WriteLog("与共享目录建立连接", "MencoderToFlv");
                mencoderToFlv(data); 
            }
        }
        private static void mencoderToFlv(ToSwfData data) {
            WriteLog("开始转换", "MencoderToFlv");
            if (!FileDirectory.FileExists(mencoderApiPath)) { WriteLog("接口不存在：", mencoderApiPath); return; }
            string flvName = System.IO.Path.ChangeExtension(data.FilePath, ".flv");
            //string arguments = " \"" + data.FilePath + "\" -o \"" + flvName + "\"  -quiet -oac mp3lame -lameopts abr:br=56 -srate 22050 -af channels=2 -ovc lavc -vf harddup,hqdn3d,scale=-3:-3 -lavcopts vcodec=flv:vbitrate=152:mbd=2:trell:v4mv:turbo:keyint=45 -ofps 15 -of lavf ";
            string arguments = " \"" + data.FilePath + "\" -o \"" + flvName + "\"  -of lavf -oac mp3lame -lameopts abr:br=56 -ovc lavc -lavcopts vcodec=flv:vbitrate=220:mbd=2:mv0:trell:v4mv:cbp:last_pred=3 -sws 3 -vf scale=320:240,expand=320:240:::1,crop=320:240:0:0 -ofps 30 -srate 22050 ";
            Run(mencoderApiPath, arguments);
            Url = "ok - [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] - " + mencoderApiPath + arguments;
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
            if (!FileDirectory.FileExists(flvName)) { WriteLog(flvName + " 文件不存在", "MencoderToFlv"); return; }
            WriteLog(Url, "MencoderToFlv");
            if ((!data.DB.IsNullEmpty()) && (!data.UpdateSQL.IsNullEmpty())) Data.Pool(data.DB).ExecSql(data.UpdateSQL);
        }
        /// <summary>
        /// iSpring转SWF
        /// </summary>
        /// <param name="data"></param>
        public static void iSpringPPTToSwf(ToSwfData data) {
            WriteLog("开始转换", "iSpringPPTToSwf");
            if (!FileDirectory.FileExists(iSpringApiPath)) { WriteLog("接口不存在：", iSpringApiPath); return; }
            string swfName = System.IO.Path.ChangeExtension(data.FilePath, ".swf");
            string arguments = " \"{0}\" \"{1}\"".FormatWith(data.FilePath, swfName);
            Run(iSpringApiPath, arguments);
            Url = "ok - [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] - " + iSpringApiPath + arguments;
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
            if (!FileDirectory.FileExists(swfName)) { WriteLog(swfName + " 文件不存在", "iSpringPPTToSwf"); return; }
            WriteLog(Url, "iSpringPPTToSwf");
        }
        /// <summary>
        /// 定时转换
        /// </summary>
        public static void DoRun() {
            if (Index > int.MaxValue) Index = 0;
            Url = "计数 - " + (++Index).ToString() + " - " + IsRun.ToString();
            if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }

            lock (toLock) {
                if (IsRun || List.Count == 0) return;
                IsRun = true;
                ToSwfData data = List[0];
                //Url = "开始转换 - " + fileName;
                //if (OnNewOrDelete != null) { OnNewOrDelete(null, null); }
                List.RemoveAt(0);
                ToSwfFlv(data);
                IsRun = false;
            }
        }
        /// <summary>
        /// 添加到队列
        /// </summary>
        /// <param name="data"></param>
        public static void AddToList(ToSwfData data) {
            lock (toLock) {
                List.Add(data);
            }
            ToSwfBase.WriteLog(data, "AddToList");
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="api">接口</param>
        public static void WriteLog(string data, string api = "") {
            if (!UseLog) return;
            data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + (api.IsNullEmpty() ? data : api + "：" + data);
            FileDirectory.FileWrite("log.txt".GetMapPath(), data);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="api">接口</param>
        public static void WriteLog(ToSwfData data, string api = "") {
            if (!UseLog) return;
            WriteLog(data.ToJson(), api);
        }
    }
}
