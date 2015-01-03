//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.UI;

namespace Pub.Class {
    /// <summary>
    /// 网络操作类
    /// 
    /// 修改纪录
    ///     2006.05.08 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Net2 {
        private static readonly object lockHelper = new object();
        //#region DownFile/ResponseFile/GetHttpFile
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileFile">文件路径（绝对路径）</param>
        /// <param name="newFileName">新文件名.扩展名（无路径）</param>
        /// <returns >返回文件是否存在，存在下载成功</returns>
        public static bool ResponseFile(string fileFile, string newFileName = "") {
            if (System.IO.File.Exists(fileFile)) {
                FileInfo _DownloadFile = new FileInfo(fileFile);
                newFileName = string.IsNullOrEmpty(newFileName) ? _DownloadFile.FullName : newFileName;
                if (Request2.GetBrowser().ToLower().IndexOf("ie") != -1) newFileName = HttpUtility.UrlEncode(newFileName, System.Text.Encoding.UTF8);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.Buffer = false;
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + newFileName);
                HttpContext.Current.Response.AppendHeader("Content-Length", _DownloadFile.Length.ToString());
                HttpContext.Current.Response.WriteFile(_DownloadFile.FullName);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
                return true;
            }

            return false;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static string DownFile(string url, string fileName) {
            using (WebClient wc = new WebClient()) wc.DownloadFile(url, fileName);
            return fileName;
        }
        /// <summary>
        /// 以指定的ContentType输出指定文件文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="filename">输出的文件名</param>
        /// <param name="filetype">将文件输出时设置的ContentType</param>
        public static void ResponseFile(string filepath, string filename, string filetype) {
            byte[] buffer = new Byte[10000];// 缓冲区为10k
            int length;// 文件长度
            long dataToRead;// 需要读的数据长度

            using (Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                dataToRead = iStream.Length;// 需要读的数据长度
                HttpContext.Current.Response.ContentType = filetype;
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename.Trim().UrlEncode().Replace("+", " "));
                while (dataToRead > 0) {
                    if (HttpContext.Current.Response.IsClientConnected) {// 检查客户端是否还处于连接状态
                        length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    } else {
                        dataToRead = -1;// 如果不再连接则跳出死循环
                    }
                }
            }
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 下载远程的文件
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="sSavePath">保存到</param>
        /// <returns>成功否</returns>
        /// <example>
        /// <code>
        /// TimeSpan oStartTime=DateTime.Now.TimeOfDay;
        /// Response.Write(GetHttpFile("http://www.spbdev.com/download/DotNetInfo1.0.rar",Request.MapPath("RecievedFile.rar")));
        /// Response.Write("&lt;br>&lt;br>\r\n执行时间：" + DateTime.Now.TimeOfDay.Subtract(oStartTime).TotalMilliseconds.ToString() + " 毫秒");
        /// </code>
        /// </example>
        public bool GetHttpFile(string url, string sSavePath) {
            string sException = null;
            bool bRslt = false;
            WebResponse oWebRps = null;
            WebRequest oWebRqst = WebRequest.Create(url);
            oWebRqst.Timeout = 50000;
            try {
                oWebRps = oWebRqst.GetResponse();
            } catch (WebException e) {
                sException = e.Message.ToString();
            } catch (Exception e) {
                sException = e.ToString();
            } finally {
                if (oWebRps.IsNotNull()) {
                    BinaryReader oBnyRd = new BinaryReader(oWebRps.GetResponseStream(), System.Text.Encoding.GetEncoding("GB2312"));
                    int iLen = Convert.ToInt32(oWebRps.ContentLength);
                    FileStream oFileStream;
                    try {
                        if (File.Exists(HttpContext.Current.Request.MapPath("RecievedData.tmp")))
                            oFileStream = File.OpenWrite(sSavePath);
                        else
                            oFileStream = File.Create(sSavePath);
                        oFileStream.SetLength((Int64)iLen);
                        oFileStream.Write(oBnyRd.ReadBytes(iLen), 0, iLen);
                        oFileStream.Close();
                    } finally {
                        oBnyRd.Close();
                        oWebRps.Close();
                    }
                    bRslt = true;
                }
            }
            return bRslt;
        }
        //#endregion
        //#region GetRemoteHtmlCode/GetRemoteHtmlCode2/GetRemoteHtmlCode3
        /// <summary>
        /// 获取远程文件源代码 自动取编码 WebClient DownloadData
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">编码</param>
        /// <returns>获取远程文件源代码</returns>
        public static string GetRemoteHtmlCode(string url, System.Text.Encoding encoding = null) {
            url += (url.IndexOf("?") >= 0 ? "&time=" : "?time=") + Rand.RndDateStr();
            using (WebClient wc = new WebClient()) {
#if !MONO40
                wc.Credentials = CredentialCache.DefaultCredentials;
#endif
                Byte[] pageData = wc.DownloadData(url);
                string content = pageData.GetHtmlEncoding(encoding.IfNull(Encoding.UTF8)).GetString(pageData);
                return content;
            }
        }
        /// <summary>
        /// 获取远程文件源代码 自动取编码 WebClient DownloadData
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">编码</param>
        /// <returns>获取远程文件源代码</returns>
        public static string GetRemoteHtmlCodeLock(string url, System.Text.Encoding encoding = null) {
            lock (lockHelper) {
                return GetRemoteHtmlCode(url, encoding);
            }
        }
        /// <summary>
        /// 获取远程文件源代码 good HttpWebRequest
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>获取远程文件源代码 线程安全</returns>
        public static string GetRemoteHtmlCode2(string url, System.Text.Encoding encoding = null, int timeout = 0) {
            url += (url.IndexOf("?") >= 0 ? "&time=" : "?time=") + Rand.RndDateStr();
            string s = ""; HttpWebResponse response = null; StreamReader stream = null;
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (timeout > 1) request.Timeout = timeout;
                response = (HttpWebResponse)request.GetResponse();
                stream = new StreamReader(response.GetResponseStream(), encoding.IfNull(Encoding.UTF8));
                s = stream.ReadToEnd();
            } catch {
            } finally {
                if (stream.IsNotNull()) stream.Close();
                if (response.IsNotNull()) response.Close();
            }
            return s;
        }
        /// <summary>
        /// 获取远程文件源代码 HttpWebRequest
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>获取远程文件源代码 线程安全</returns>
        public static string GetRemoteHtmlCode2Lock(string url, System.Text.Encoding encoding = null, int timeout = 0) {
            lock (lockHelper) {
                return GetRemoteHtmlCode2(url, encoding, timeout);
            }
        }
        /// <summary>
        /// 获取远程文件源代码 HttpWebRequest UserAgent + Referer + AllowAutoRedirect
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>获取远程文件源代码</returns>
        public static string GetRemoteHtmlCode3(string url, System.Text.Encoding encoding = null, int timeout = 0) {
            url += (url.IndexOf("?") >= 0 ? "&time=" : "?time=") + Rand.RndDateStr();
            string s = ""; HttpWebResponse response = null; StreamReader stream = null;
            try {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.AllowAutoRedirect = true;
                if (timeout > 1) request.Timeout = timeout;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                request.Referer = url;
                response = (HttpWebResponse)request.GetResponse();
                stream = new StreamReader(response.GetResponseStream(), encoding.IfNull(Encoding.UTF8));
                s = stream.ReadToEnd();
            } catch {
            } finally {
                if (stream.IsNotNull()) stream.Close();
                if (response.IsNotNull()) response.Close();
            }
            return s;
        }
        /// <summary>
        /// 获取远程文件源代码 HttpWebRequest UserAgent + Referer + AllowAutoRedirect
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>获取远程文件源代码</returns>
        public static string GetRemoteHtmlCode3Lock(string url, System.Text.Encoding encoding = null, int timeout = 0) {
            lock (lockHelper) {
                return GetRemoteHtmlCode3(url, encoding, timeout);
            }
        }
        /// <summary>
        /// 获取远程文件源代码 WebClient DownloadData
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">编码</param>
        /// <returns>获取远程文件源代码</returns>
        public static string GetRemoteHtmlCode4(string url, System.Text.Encoding encoding = null) {
            url += (url.IndexOf("?") >= 0 ? "&time=" : "?time=") + Rand.RndDateStr();
            using (WebClient wc = new WebClient()) {
#if !MONO40
                wc.Credentials = CredentialCache.DefaultCredentials;
#endif
                Byte[] pageData = wc.DownloadData(url);
                string content = encoding.IfNull(Encoding.UTF8).GetString(pageData);
                return content;
            }
        }
        /// <summary>
        /// 获取远程文件源代码 WebClient DownloadData
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">编码</param>
        /// <returns>获取远程文件源代码</returns>
        public static string GetRemoteHtmlCode4Lock(string url, System.Text.Encoding encoding = null) {
            lock (lockHelper) {
                return GetRemoteHtmlCode4(url, encoding);
            }
        }
        /// <summary>
        /// 获取远程文件源代码 WebClient UploadData
        /// </summary>
        /// <param name="url">远程url</param>
        /// <param name="encoding">编码</param>
        /// <returns>获取远程文件源代码</returns>
        public static string GetRemoteHtmlCode5(string url, System.Text.Encoding encoding = null) {
            url += (url.IndexOf("?") >= 0 ? "&time=" : "?time=") + Rand.RndDateStr();
            string postString = "";
            using (WebClient webClient = new WebClient()) {
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] postData = Encoding.ASCII.GetBytes(postString);
                byte[] responseData = webClient.UploadData(url, "POST", postData);
                string srcString = encoding.IfNull(Encoding.UTF8).GetString(responseData);
                return srcString;
            }
        }
        /// <summary>
        /// 获取远程文件源代码 WebClient UploadData
        /// </summary>
        /// <param name="url">远程url</param>
        /// <param name="encoding">编码</param>
        /// <returns>获取远程文件源代码</returns>
        public static string GetRemoteHtmlCode5Lock(string url, System.Text.Encoding encoding = null) {
            lock (lockHelper) {
                return GetRemoteHtmlCode5(url, encoding);
            }
        }

        //#endregion
        //#region TransHtml
        /// <summary>
        /// 转换为静态html
        /// </summary>
        /// <param name="path">网址</param>
        /// <param name="outpath">输出路径</param>
        /// <param name="encoding">编码</param>
        public static void TransHtml(string path, string outpath, System.Text.Encoding encoding) {
            Page page = new Page();
            StringWriter writer = new StringWriter();
            page.Server.Execute(path, writer);
            outpath = outpath.IndexOf("\\") > 0 ? outpath : outpath.GetMapPath();
            FileDirectory.FileDelete(outpath);
            FileDirectory.FileWrite(outpath, writer.ToString(), encoding);

            //Page page = new Page();
            //StringWriter writer = new StringWriter();
            //page.Server.Execute(path, writer);
            //FileStream fs;
            //if (File.Exists(page.Server.MapPath("~/") + "\\" + outpath)) {
            //    File.Delete(page.Server.MapPath("~/") + "\\" + outpath);
            //    fs = File.Create(page.Server.MapPath("~/") + "\\" + outpath);
            //} else {
            //    fs = File.Create(page.Server.MapPath("~/") + "\\" + outpath);
            //}
            //byte[] bt = encoding.GetBytes(writer.ToString());
            //fs.Write(bt, 0, bt.Length);
            //fs.Close();
        }
        //#endregion
    }
}
