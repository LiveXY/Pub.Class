using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pub.Class {
    ///<summary>
    /// 执行HTTP GET和POST请求操作
    /// 
    /// 修改纪录
    ///     2009.05.02 版本：1.0 livexy 创建此类
    /// 
    ///</summary>
    public class HttpHelper {
        /// <summary>
        /// POST请求发送到指定的URL
        /// </summary>
        /// <example>
        /// <code>
        ///     string postResponse = HttpHelper.SendPost("http://www.google.com", "t=sample post data");
        /// </code>
        /// </example>
        /// <param name="url">请求的URL</param>
        /// <param name="postData">要发送的数据</param>
        /// <returns>返回请求的数据</returns>
        public static string SendPost(string url, string postData) {
            return SendPost(url, postData, null);
        }
        /// <summary>
        /// POST请求发送到指定的URL
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="postData">要发送的数据</param>
        /// <param name="contentType">请求的内容类型。默认值是'application/x-www-form-urlencoded'.</param>
        /// <returns>返回请求的数据</returns>
        public static string SendPost(string url, string postData, string contentType) {
            return SendPost(url, postData, contentType, null);
        }
        /// <summary>
        /// POST请求发送到指定的URL
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="postData">要发送的数据</param>
        /// <param name="contentType">请求的内容类型。默认值是'application/x-www-form-urlencoded'</param>
        /// <param name="credentials">使用的凭据</param>
        /// <returns>返回请求的数据</returns>
        public static string SendPost(string url, string postData, string contentType, ICredentials credentials, Encoding encoding = null) {
            return SendRequest(url, postData, "POST", contentType, null, null, credentials, null, null, encoding.IfNull(Encoding.UTF8));
        }
        /// <summary>
        /// POST数据
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="postData">数据</param>
        /// <param name="contentType">请求的内容类型。默认值是'application/x-www-form-urlencoded'</param>
        /// <param name="credentials">使用的凭据</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回请求的数据</returns>
        public static string SendPost(string url, string postData, string contentType, ICredentials credentials, int? timeout, Encoding encoding = null) {
            return SendRequest(url, postData, "POST", contentType, null, null, credentials, null, timeout, encoding.IfNull(Encoding.UTF8));
        }
        /// <summary>
        /// 到指定的URL发送一个GET请求
        /// </summary>
        /// <example>
        /// <code>
        ///     string getResponse = HttpHelper.SendGet("http://www.google.com");
        /// </code>
        /// </example>
        /// <param name="url">请求的URL</param>
        /// <returns>返回请求的数据</returns>
        public static string SendGet(string url) {
            return SendGet(url, null);
        }
        /// <summary>
        /// 到指定的URL发送一个GET请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="contentType">请求的内容类型。默认值是'application/x-www-form-urlencoded'.</param>
        /// <returns>返回请求的数据</returns>
        public static string SendGet(string url, string contentType) {
            return SendGet(url, contentType, null);
        }
        /// <summary>
        /// 到指定的URL发送一个GET请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="contentType">请求的内容类型。默认值是'application/x-www-form-urlencoded'.</param>
        /// <param name="credentials">使用的凭据</param>
        /// <returns>返回请求的数据</returns>
        public static string SendGet(string url, string contentType, ICredentials credentials, Encoding encoding = null) {
            return SendRequest(url, null, "GET", contentType, null, null, credentials, null, null, encoding.IfNull(Encoding.UTF8));
        }
        /// <summary>
        /// GET数据
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="contentType">请求的内容类型。默认值是'application/x-www-form-urlencoded'</param>
        /// <param name="credentials">使用的凭据</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回请求的数据</returns>
        public static string SendGet(string url, string contentType, ICredentials credentials, int? timeout, Encoding encoding = null) {
            return SendRequest(url, null, "GET", contentType, null, null, credentials, null, timeout, encoding.IfNull(Encoding.UTF8));
        }
        /// <summary>
        /// 发送到指定的URL的请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="postData">要发送的数据</param>
        /// <param name="requestType">请求类型 POST GET</param>
        /// <param name="contentType">请求的内容类型。默认值是'application/x-www-form-urlencoded'.</param>
        /// <param name="userAgent">请求 UserAgent</param>
        /// <param name="keepAlive">KeepAlive true/false</param>
        /// <param name="credentials">使用的凭据</param>
        /// <param name="proxy">使用代理</param>
        /// <param name="timeout">超时</param>
        /// <param name="encoding">设置响应编码</param>
        /// <returns>返回请求的数据</returns>
        public static string SendRequest(string url, string postData, string requestType, string contentType, string userAgent, bool? keepAlive, ICredentials credentials, IWebProxy proxy, int? timeout, Encoding encoding) {
            return SendRequest(url, postData, requestType, contentType, userAgent, keepAlive, credentials, proxy, timeout, encoding, null);
        }
        /// <summary>
        /// 发送到指定的URL的请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="postData">要发送的数据</param>
        /// <param name="requestType">请求类型 POST GET</param>
        /// <param name="contentType">请求的内容类型。默认值是'application/x-www-form-urlencoded'.</param>
        /// <param name="userAgent">请求 UserAgent</param>
        /// <param name="keepAlive">KeepAlive true/false</param>
        /// <param name="credentials">使用的凭据</param>
        /// <param name="proxy">使用代理</param>
        /// <param name="timeout">超时</param>
        /// <param name="encoding">设置响应编码</param>
        /// <param name="postDataEncoding">设置POST数据编码</param>
        /// <returns>返回请求的数据</returns>
        public static string SendRequest(string url, string postData, string requestType, string contentType, string userAgent, bool? keepAlive, ICredentials credentials, IWebProxy proxy, int? timeout, Encoding encoding, Encoding postDataEncoding, WebHeaderCollection webHeaderCollection = null) {
            if (!String.IsNullOrEmpty(url)) {
                HttpWebRequest webReq;
                HttpWebResponse webResp = null;
                if (postDataEncoding == null) postDataEncoding = Encoding.UTF8;

                try {
                    webReq = (HttpWebRequest)WebRequest.Create(url);
                    webReq.Method = requestType;

                    if (webHeaderCollection.IsNotNull()) webReq.Headers = webHeaderCollection;
                    if (credentials.IsNotNull()) webReq.Credentials = credentials;
                    if (proxy.IsNotNull()) webReq.Proxy = proxy;
                    if (timeout.IsNotNull()) webReq.Timeout = (int)timeout;
                    webReq.ServicePoint.Expect100Continue = false;
                    //webReq.PreAuthenticate = true;
                    //webReq.AllowWriteStreamBuffering = true;
                    webReq.ContentType = !String.IsNullOrEmpty(contentType) ? contentType : "text/xml"; // "application/x-www-form-urlencoded";
                    if (!String.IsNullOrEmpty(userAgent)) webReq.UserAgent = userAgent;
                    if (keepAlive.IsNotNull()) webReq.KeepAlive = (bool)keepAlive;

                    byte[] data = !String.IsNullOrEmpty(postData) ? postDataEncoding.GetBytes(postData) : postDataEncoding.GetBytes(String.Empty);
                    webReq.ContentLength = data.Length;
                    if (requestType.ToUpper() != "GET") {
                        Stream writer = webReq.GetRequestStream();
                        writer.Write(data, 0, data.Length);
                        writer.Close();
                    }

                    webResp = (HttpWebResponse)webReq.GetResponse();

                    string response;
                    using (var receiveStream = webResp.GetResponseStream()) {
                        using (var readStream = new StreamReader(receiveStream, encoding))
                            response = readStream.ReadToEnd();
                    }

                    return response;
                } finally {
                    if (webResp.IsNotNull()) webResp.Close();
                }
            }

            throw new Exception("必须指定一个有效的URL。");
        }
        /// <summary>
        /// 通过HTTP下载文件
        /// </summary>
        /// <param name="sourceUrl">源文件所在的URL</param>
        /// <param name="destinationPath">该文件将被存储的本地目标路径</param>
        /// <returns>下载成功 true/false</returns>
        public static bool DownloadFile(string sourceUrl, string destinationPath) {
            var webClient = new WebClient();
            webClient.DownloadFile(sourceUrl, destinationPath);

            return true;
        }
        /// <summary>
        /// 获取一个网页的内容的GET方法
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="accept">Accept</param>
        /// <param name="encoding">设置响应编码</param>
        /// <param name="referer">Referer</param>
        /// <param name="language">Language</param>
        /// <param name="cc">Cookie</param>
        /// <param name="contentType">contentType</param>
        /// <param name="returnUrl">跳转URL</param>
        /// <returns>返回请求的数据</returns>
        public static string GetHtml(string url, string accept, string contentType, string referer, string language, CookieContainer cc, Encoding encoding, out string returnUrl) {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.CookieContainer = cc;
            req.Accept = accept;
            req.ContentType = contentType;
            if (!string.IsNullOrEmpty(referer)) {
                req.Referer = referer;
            }
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.0.04506; .NET CLR 3.5.21022)";
            if (!string.IsNullOrEmpty(language)) {
                req.Headers.Add("Accept-Language", language);
            }
            req.Headers.Add("UA-CPU", "x86");
            req.AllowAutoRedirect = true;
            req.KeepAlive = true;
            req.Method = "GET";
            req.UseDefaultCredentials = true;
            using (var res = (HttpWebResponse)req.GetResponse()) {
                var response = res.GetResponseStream();
                if (req.CookieContainer.IsNotNull()) {
                    res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    string cks = cc.GetCookieHeader(req.RequestUri);
                    Debug.WriteLine(cks);
                }
                string responseUrl = res.ResponseUri.ToString();
                returnUrl = responseUrl;
                using (StreamReader sr = new StreamReader(response, encoding)) {
                    string content = sr.ReadToEnd();
                    return content;
                }
            }
        }
        /// <summary>
        /// 取JS时间
        /// </summary>
        /// <returns></returns>
        public static string GetJavaScriptTime() {
            Int64 retval = 0;
            DateTime st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now - st);
            retval = (Int64)(t.TotalMilliseconds + 0.5);
            retval = retval + 1000000;
            return retval.ToString();
        }
        /// <summary>
        /// 获取一个网页的内容的GET方法
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="encoding">设置响应编码</param>
        /// <param name="accept">Accept</param>
        /// <param name="contentType">contentType</param>
        /// <param name="cookies">cookies</param>
        /// <param name="language">language</param>
        /// <param name="referer">referer</param>
        /// <param name="returnUrl">跳转URL</param>
        /// <returns>返回请求的数据</returns>
        public static string GetHtml(string url, string accept, string contentType, string referer, string language, string cookies, Encoding encoding, out string returnUrl) {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

            req.Accept = accept;
            req.ContentType = contentType;
            req.Referer = referer;
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.0.04506; .NET CLR 3.5.21022)";
            if (!string.IsNullOrEmpty(language)) {
                req.Headers.Add("Accept-Language", language);
            }
            req.Headers.Add("UA-CPU", "x86");
            req.AllowAutoRedirect = true;
            req.Method = "GET";
            if (!string.IsNullOrEmpty(cookies)) {
                req.Headers.Add("Cookie", cookies);
            }

            using (var res = (HttpWebResponse)req.GetResponse()) {
                var response = res.GetResponseStream();
                if (req.CookieContainer.IsNotNull()) {
                    res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);
                }
                string responseUrl = res.ResponseUri.ToString();
                returnUrl = responseUrl;
                using (StreamReader sr = new StreamReader(response, encoding)) {
                    string content = sr.ReadToEnd();
                    return content;
                }
            }
        }
        /// <summary>
        /// 获取一个网页的内容的POST方法
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="encoding">设置响应编码</param>
        /// <param name="accept">accept</param>
        /// <param name="cc">cookies</param>
        /// <param name="contentType">contentType</param>
        /// <param name="language">language</param>
        /// <param name="postContent">POST数据</param>
        /// <param name="referer">referer</param>
        /// <param name="returnUrl">跳转URL</param>
        /// <returns>返回请求的数据</returns>
        public static string Post(string url, string accept, string contentType, string referer, string language, string postContent, CookieContainer cc, Encoding encoding, out string returnUrl) {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.CookieContainer = cc;
            req.Accept = accept;
            req.ContentType = contentType;
            req.Referer = referer;
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.0.04506; .NET CLR 3.5.21022)";
            if (!string.IsNullOrEmpty(language)) {
                req.Headers.Add("Accept-Language", language);
            }
            req.Headers.Add("UA-CPU", "x86");
            req.AllowAutoRedirect = true;
            req.UseDefaultCredentials = true;
            req.Method = "POST";
            if (!string.IsNullOrEmpty(postContent)) {
                using (StreamWriter sw = new StreamWriter(req.GetRequestStream(), encoding)) {
                    sw.Write(postContent);
                    sw.Flush();
                }
            }
            using (var res = (HttpWebResponse)req.GetResponse()) {
                var response = res.GetResponseStream();
                if (req.CookieContainer.IsNotNull()) {
                    res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);
                }
                string responseUrl = res.ResponseUri.ToString();
                returnUrl = responseUrl;
                using (StreamReader sr = new StreamReader(response, encoding)) {
                    string content = sr.ReadToEnd();
                    return content;
                }
            }
        }
        /// <summary>
        /// 获取一个网页的内容的POST方法 不自动跳转
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="encoding">设置响应编码</param>
        /// <param name="accept">accept</param>
        /// <param name="cc">cookies</param>
        /// <param name="contentType">contentType</param>
        /// <param name="language">language</param>
        /// <param name="postContent">POST数据</param>
        /// <param name="referer">referer</param>
        /// <param name="returnUrl">跳转URL</param>
        /// <returns>返回请求的数据</returns>
        public static string PostNoAutoRedirect(string url, string accept, string contentType, string referer, string language, string postContent, ref CookieContainer cc, Encoding encoding, out string returnUrl) {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.CookieContainer = cc;
            req.Accept = accept;
            req.ContentType = contentType;
            req.Referer = referer;
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.0.04506; .NET CLR 3.5.21022)";
            if (!string.IsNullOrEmpty(language)) {
                req.Headers.Add("Accept-Language", language);
            }
            req.Headers.Add("UA-CPU", "x86");
            req.AllowAutoRedirect = false;
            req.UseDefaultCredentials = true;
            req.Method = "POST";
            if (!string.IsNullOrEmpty(postContent)) {
                using (StreamWriter sw = new StreamWriter(req.GetRequestStream(), encoding)) {
                    sw.Write(postContent);
                    sw.Flush();
                }
            }
            using (var res = (HttpWebResponse)req.GetResponse()) {
                var response = res.GetResponseStream();
                if (req.CookieContainer.IsNotNull()) {
                    res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    var cookies = GetAllCookiesFromHeader(res.Headers["Set-Cookie"], res.ResponseUri.Host);
                    cc = new CookieContainer();
                    cc.Add(cookies);
                }
                string responseUrl = res.Headers["Location"];
                returnUrl = responseUrl;
                using (StreamReader sr = new StreamReader(response, encoding)) {
                    string content = sr.ReadToEnd();
                    return content;
                }
            }
        }
        /// <summary>
        /// POST数据
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="accept">accept</param>
        /// <param name="contentType">contentType</param>
        /// <param name="referer">referer</param>
        /// <param name="language">language</param>
        /// <param name="postContent">POST数据</param>
        /// <param name="cc">cookies</param>
        /// <param name="encoding">响应编码</param>
        /// <param name="returnUrl">跳转URL</param>
        /// <returns>返回请求的数据</returns>
        public static string Post(string url, string accept, string contentType, string referer, string language, string postContent, ref CookieContainer cc, Encoding encoding, out string returnUrl) {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.CookieContainer = cc;
            req.Accept = accept;
            req.ContentType = contentType;
            req.Headers["Cache-Control"] = "no-cache";
            req.Referer = referer;
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.0.04506; .NET CLR 3.5.21022)";
            if (!string.IsNullOrEmpty(language)) {
                req.Headers.Add("Accept-Language", language);
            }
            req.Headers.Add("UA-CPU", "x86");
            req.AllowAutoRedirect = true;
            req.UseDefaultCredentials = true;
            req.Method = "POST";
            if (!string.IsNullOrEmpty(postContent)) {
                using (StreamWriter sw = new StreamWriter(req.GetRequestStream(), encoding)) {
                    sw.Write(postContent);
                    sw.Flush();
                }
            }
            using (var res = (HttpWebResponse)req.GetResponse()) {
                var response = res.GetResponseStream();
                if (req.CookieContainer.IsNotNull()) {
                    res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    var cookies = GetAllCookiesFromHeader(res.Headers["Set-Cookie"], res.ResponseUri.Host);
                    cc = new CookieContainer();
                    cc.Add(cookies);
                }
                string responseUrl = res.ResponseUri.ToString();
                returnUrl = responseUrl;
                using (StreamReader sr = new StreamReader(response, encoding)) {
                    string content = sr.ReadToEnd();
                    return content;
                }
            }
        }
        /// <summary>
        /// GetAllCookiesFromHeader
        /// </summary>
        /// <param name="strHeader"></param>
        /// <param name="strHost"></param>
        /// <returns></returns>
        public static CookieCollection GetAllCookiesFromHeader(string strHeader, string strHost) {
            ArrayList al = new ArrayList();
            CookieCollection cc = new CookieCollection();
            if (strHeader != string.Empty) {
                al = ConvertCookieHeaderToArrayList(strHeader);
                cc = ConvertCookieArraysToCookieCollection(al, strHost);
            }
            return cc;
        }
        private static ArrayList ConvertCookieHeaderToArrayList(string strCookHeader) {
            if (string.IsNullOrEmpty(strCookHeader)) {
                return new ArrayList();
            }
            strCookHeader = strCookHeader.Replace("\r", "");
            strCookHeader = strCookHeader.Replace("\n", "");
            string[] strCookTemp = strCookHeader.Split(',');
            ArrayList al = new ArrayList();
            int i = 0;
            int n = strCookTemp.Length;
            while (i < n) {
                if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0) {
                    al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
                    i = i + 1;
                } else {
                    al.Add(strCookTemp[i]);
                }
                i = i + 1;
            }
            return al;
        }
        private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList al, string strHost) {
            CookieCollection cc = new CookieCollection();

            int alcount = al.Count;
            string strEachCook;
            string[] strEachCookParts;
            for (int i = 0; i < alcount; i++) {
                strEachCook = al[i].ToString();
                strEachCookParts = strEachCook.Split(';');
                int intEachCookPartsCount = strEachCookParts.Length;
                string strCNameAndCValue = string.Empty;
                string strPNameAndPValue = string.Empty;
                string strDNameAndDValue = string.Empty;
                string[] NameValuePairTemp;
                Cookie cookTemp = new Cookie();

                for (int j = 0; j < intEachCookPartsCount; j++) {
                    if (j == 0) {
                        strCNameAndCValue = strEachCookParts[j];
                        if (strCNameAndCValue != string.Empty) {
                            int firstEqual = strCNameAndCValue.IndexOf("=");
                            string firstName = strCNameAndCValue.Substring(0, firstEqual);
                            string allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }
                        continue;
                    }
                    if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0) {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty) {
                            NameValuePairTemp = strPNameAndPValue.Split('=');
                            if (NameValuePairTemp[1] != string.Empty) {
                                cookTemp.Path = NameValuePairTemp[1];
                            } else {
                                cookTemp.Path = "/";
                            }
                        }
                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0) {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty) {
                            NameValuePairTemp = strPNameAndPValue.Split('=');

                            if (NameValuePairTemp[1] != string.Empty) {
                                cookTemp.Domain = NameValuePairTemp[1];
                            } else {
                                cookTemp.Domain = strHost;
                            }
                        }
                        continue;
                    }
                }

                if (cookTemp.Path == string.Empty) {
                    cookTemp.Path = "/";
                }
                if (cookTemp.Domain == string.Empty) {
                    cookTemp.Domain = strHost;
                }
                cc.Add(cookTemp);
            }
            return cc;
        }
        /// <summary>
        /// 获取QueryString值
        /// </summary>
        /// <param name="u">URL</param>
        /// <param name="key">QueryString key</param>
        /// <returns>QueryString 值</returns>
        public static string GetQ(Uri u, string key) {
            string q = u.Query;
            int index = q.IndexOf(key + "=");
            int len = key.Length + 1;
            int index2 = q.IndexOf("&", index + len);
            string val = "";
            if (index2 == -1) {
                val = q.Substring(index + len, q.Length - index - len);
            } else {
                val = q.Substring(index + len, index2 - index - len);
            }
            return val;
        }
        /// <summary>
        /// 从HTML代码的前缀和后缀获取中间部分内容 从上向下
        /// </summary>
        /// <param name="html">源HTML代码</param>
        /// <param name="prefix">前缀</param>
        /// <param name="subfix">后缀</param>
        /// <returns>中间部分数据</returns>
        public static string Resove(string html, string prefix, string subfix) {
            int inl = html.IndexOf(prefix);
            if (inl == -1) {
                return null;
            }
            inl += prefix.Length;
            int inl2 = html.IndexOf(subfix, inl);
            var s = html.Substring(inl, inl2 - inl);
            return s;
        }
        /// <summary>
        /// 从HTML代码的前缀和后缀获取中间部分内容 从下向上
        /// </summary>
        /// <param name="html">源HTML代码</param>
        /// <param name="subfix">前缀</param>
        /// <param name="prefix">后缀</param>
        /// <returns>中间部分数据</returns>
        public static string ResoveReverse(string html, string subfix, string prefix) {
            int inl = html.IndexOf(subfix);
            if (inl == -1) {
                return null;
            }
            string subString = html.Substring(0, inl);
            int inl2 = subString.LastIndexOf(prefix);
            if (inl2 == -1) {
                return null;
            }
            var s = subString.Substring(inl2 + prefix.Length, subString.Length - inl2 - prefix.Length);
            return s;
        }
        /// <summary>
        /// 从HTML代码的前缀和后缀获取中间部分内容列表 遍历
        /// </summary>
        /// <param name="html">源HTML代码</param>
        /// <param name="prefix">前缀</param>
        /// <param name="subfix">后缀</param>
        /// <returns>中间部分数据列表</returns>
        public static List<string> ResoveList(string html, string prefix, string subfix) {
            List<string> list = new List<string>();
            int index = prefix.Length * -1;
            do {
                index = html.IndexOf(prefix, index + prefix.Length);
                if (index == -1) {
                    break;
                }
                index += prefix.Length;
                int index4 = html.IndexOf(subfix, index);
                string s78 = html.Substring(index, index4 - index);
                list.Add(s78);
            }
            while (index > -1);
            return list;
        }
    }
}

