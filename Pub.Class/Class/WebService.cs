//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Reflection;
using System.Web.Services.Description;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections;
using System.Xml;

namespace Pub.Class {
    /// <summary>
    /// WebService 调用
    /// 
    /// 修改纪录
    ///     2008.07.01 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class WebService {
        //#region InvokeWebService
        /// <summary>
        /// InvokeWebService 动态调用web服务
        /// </summary>
        /// <param name="wsUrl">WebService 地址</param>
        /// <param name="methodname">方法名</param>
        /// <param name="args">参数，仅仅支持简单类型</param>	
        /// <returns>接口</returns>
        public static object InvokeWebService(string wsUrl, string methodname, object[] args) {
            return InvokeWebService(wsUrl, null, methodname, args);
        }
        /// <summary>
        /// InvokeWebService 动态调用web服务
        /// </summary>
        /// <param name="wsUrl">WebService 地址</param>
        /// <param name="classname">类名</param>
        /// <param name="methodname">方法名</param>
        /// <param name="args">参数，仅仅支持简单类型</param>
        /// <returns>接口</returns>
        public static object InvokeWebService(string wsUrl, string classname, string methodname, object[] args) {
            try {
                Type wsProxyType = GetWsProxyType(wsUrl, classname);
                object obj = Activator.CreateInstance(wsProxyType);
                MethodInfo mi = wsProxyType.GetMethod(methodname);

                return mi.Invoke(obj, args);
            } catch (Exception ex) {
                throw ex;
            }
        }
        //#region GetWsClassName
        private static string GetWsClassName(string wsUrl) {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');

            return pps[0];
        }
        //#endregion
        //#endregion
        //#region GetWsProxyType ,using Cache
        private static readonly ISafeDictionary<string, Type> WSProxyTypeDictionary = new SafeDictionary<string, Type>();
        /// <summary>
        /// GetWsProxyType 获取目标Web服务对应的代理类型 CACHE
        /// </summary>
        /// <param name="wsUrl">目标Web服务的url</param>
        /// <param name="classname">Web服务的class名称，如果不需要指定，则传入null</param>  
        /// <returns>Type</returns>
        public static Type GetWsProxyType(string wsUrl, string classname) {
            string @namespace = "Pub.Class.WebServiceHelper.DynamicWebCalling";
            if ((classname.IsNull()) || (classname == "")) {
                classname = GetWsClassName(wsUrl);
            }
            string cacheKey = wsUrl + "@" + classname;
            if (WSProxyTypeDictionary.ContainsKey(cacheKey)) {
                return WSProxyTypeDictionary[cacheKey];
            }


            //获取WSDL
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(wsUrl + "?WSDL");
            ServiceDescription sd = ServiceDescription.Read(stream);
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace(@namespace);

            //生成客户端代理类代码
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider csc = new CSharpCodeProvider();
            ICodeCompiler icc = csc.CreateCompiler();

            //设定编译参数
            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");

            //编译代理类
            CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
            if (true == cr.Errors.HasErrors) {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors) {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                throw new Exception(sb.ToString());
            }

            //生成代理实例，并调用方法
            System.Reflection.Assembly assembly = cr.CompiledAssembly;
            Type wsProxyType = assembly.GetType(@namespace + "." + classname, true, true);

            if (!WSProxyTypeDictionary.ContainsKey(cacheKey)) WSProxyTypeDictionary.Add(cacheKey, wsProxyType);
            return wsProxyType;
        }
        //#endregion
        /// <summary>
        /// GET方式调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="methodName">方法名</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string GetWebService(string url, string methodName, string param) {
#if !MONO40
            return HttpHelper.SendGet(url + "/" + methodName + param, "application/x-www-form-urlencoded", CredentialCache.DefaultCredentials, 20000).ToXmlDOM().InnerText;
#else
            return HttpHelper.SendGet(url + "/" + methodName + param, "application/x-www-form-urlencoded", null, 20000).ToXmlDOM().InnerText;
#endif
            }
        /// <summary>
        /// GET方式调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string GetWebService(string url, string methodName, Hashtable parms) {
            string param = parms.IsNull() || parms.Count == 0 ? string.Empty : ("?" + parms.ToUrl());
            return GetWebService(url, methodName, param);
        }
        /// <summary>
        /// GET方式调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string GetWebService(string url, string methodName, IList<UrlParameter> parms) {
            string param = parms.IsNull() || parms.Count == 0 ? string.Empty : ("?" + parms.ToUrl());
            return GetWebService(url, methodName, param);
        }

        /// <summary>
        /// Post方式调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="methodName">方法名</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string PostWebService(string url, string methodName, string param) {
#if !MONO40
            return HttpHelper.SendPost(url + "/" + methodName, param, "application/x-www-form-urlencoded", CredentialCache.DefaultCredentials, 20000).ToXmlDOM().InnerText;
#else
            return HttpHelper.SendPost(url + "/" + methodName, param, "application/x-www-form-urlencoded", null, 20000).ToXmlDOM().InnerText;
#endif
        }
        /// <summary>
        /// Post方式调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string PostWebService(string url, string methodName, Hashtable parms) {
            return PostWebService(url, methodName, parms.ToUrl());
        }
        /// <summary>
        /// Post方式调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string PostWebService(string url, string methodName, IList<UrlParameter> parms) {
            return PostWebService(url, methodName, parms.ToUrl());
        }

        private static Hashtable _xmlNamespaces = new Hashtable();
        /// <summary>
        /// Soap方式调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string SoapWebService(string url, string methodName, Hashtable parms) {
            string xmlNS = string.Empty;
            if (!_xmlNamespaces.ContainsKey(url)) {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?WSDL");
#if !MONO40
                request.Credentials = CredentialCache.DefaultCredentials;
#endif
                request.ServicePoint.Expect100Continue = false;
                request.Timeout = 20000;
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                XmlDocument doc1 = new XmlDocument();
                doc1.LoadXml(sr.ReadToEnd());
                xmlNS = doc1.SelectSingleNode("//@targetNamespace").Value;
                _xmlNamespaces[url] = xmlNS;
            } else xmlNS = _xmlNamespaces[url].ToString();

            HttpWebRequest request2 = (HttpWebRequest)HttpWebRequest.Create(url);
            request2.Method = "POST";
            request2.ContentType = "text/xml; charset=utf-8";
            request2.Headers.Add("SOAPAction", "\"" + xmlNS + (xmlNS.EndsWith("/") ? "" : "/") + methodName + "\"");
#if !MONO40
            request2.Credentials = CredentialCache.DefaultCredentials;
#endif
            request2.Timeout = 20000;
            request2.ServicePoint.Expect100Continue = false;

            byte[] data = EncodeParamsToSoap(parms, xmlNS, methodName);
            request2.ContentLength = data.Length;
            Stream writer = request2.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();

            XmlDocument doc = new XmlDocument(), doc2 = new XmlDocument();
            StreamReader sr2 = new StreamReader(request2.GetResponse().GetResponseStream(), Encoding.UTF8);
            String retXml = sr2.ReadToEnd();
            sr2.Close();
            doc.LoadXml(retXml);

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            String RetXml = doc.SelectSingleNode("//soap:Body/*", mgr).InnerXml;
            doc2.LoadXml("<root>" + RetXml + "</root>");
            XmlDeclaration decl = doc2.CreateXmlDeclaration("1.0", "utf-8", null);
            doc2.InsertBefore(decl, doc2.DocumentElement);
            return doc2.InnerText;
            //return HttpHelper.SendPost(url + "/" + methodName, parms.ToUrl(), "application/x-www-form-urlencoded", CredentialCache.DefaultCredentials);
        }
        /// <summary>
        /// Soap方式调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string SoapWebService(string url, string methodName, IList<UrlParameter> parms) {
            string xmlNS = string.Empty;
            if (!_xmlNamespaces.ContainsKey(url)) {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?WSDL");
#if !MONO40
                request.Credentials = CredentialCache.DefaultCredentials;
#endif
                request.ServicePoint.Expect100Continue = false;
                request.Timeout = 20000;
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                XmlDocument doc1 = new XmlDocument();
                doc1.LoadXml(sr.ReadToEnd());
                xmlNS = doc1.SelectSingleNode("//@targetNamespace").Value;
                _xmlNamespaces[url] = xmlNS;
            } else xmlNS = _xmlNamespaces[url].ToString();

            HttpWebRequest request2 = (HttpWebRequest)HttpWebRequest.Create(url);
            request2.Method = "POST";
            request2.ContentType = "text/xml; charset=utf-8";
            request2.Headers.Add("SOAPAction", "\"" + xmlNS + (xmlNS.EndsWith("/") ? "" : "/") + methodName + "\"");
#if !MONO40
            request2.Credentials = CredentialCache.DefaultCredentials;
#endif
            request2.Timeout = 20000;
            request2.ServicePoint.Expect100Continue = false;

            byte[] data = EncodeParamsToSoap(parms, xmlNS, methodName);
            request2.ContentLength = data.Length;
            Stream writer = request2.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();

            XmlDocument doc = new XmlDocument(), doc2 = new XmlDocument();
            StreamReader sr2 = new StreamReader(request2.GetResponse().GetResponseStream(), Encoding.UTF8);
            String retXml = sr2.ReadToEnd();
            sr2.Close();
            doc.LoadXml(retXml);

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            String RetXml = doc.SelectSingleNode("//soap:Body/*", mgr).InnerXml;
            doc2.LoadXml("<root>" + RetXml + "</root>");
            XmlDeclaration decl = doc2.CreateXmlDeclaration("1.0", "utf-8", null);
            doc2.InsertBefore(decl, doc2.DocumentElement);
            return doc2.InnerText;
            //return HttpHelper.SendPost(url + "/" + methodName, parms.ToUrl(), "application/x-www-form-urlencoded", CredentialCache.DefaultCredentials);
        }
        private static byte[] EncodeParamsToSoap(Hashtable paras, String xmlNS, String methodName) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"></soap:Envelope>");
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(decl, doc.DocumentElement);
            XmlElement soapBody = doc.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlElement soapMethod = doc.CreateElement(methodName);
            soapMethod.SetAttribute("xmlns", xmlNS);
            if (!(paras.IsNull() || paras.Count == 0)) {
                foreach (string k in paras.Keys) {
                    XmlElement soapPar = doc.CreateElement(k);
                    soapPar.InnerText = paras[k].ToString();
                    soapMethod.AppendChild(soapPar);
                }
            }
            soapBody.AppendChild(soapMethod);
            doc.DocumentElement.AppendChild(soapBody);
            return Encoding.UTF8.GetBytes(doc.OuterXml);
        }
        private static byte[] EncodeParamsToSoap(IList<UrlParameter> paras, String xmlNS, String methodName) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"></soap:Envelope>");
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(decl, doc.DocumentElement);
            XmlElement soapBody = doc.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlElement soapMethod = doc.CreateElement(methodName);
            soapMethod.SetAttribute("xmlns", xmlNS);
            if (!(paras.IsNull() || paras.Count == 0)) {
                foreach (UrlParameter k in paras) {
                    XmlElement soapPar = doc.CreateElement(k.ParameterName);
                    soapPar.InnerText = k.ParameterValue;
                    soapMethod.AppendChild(soapPar);
                }
            }
            soapBody.AppendChild(soapMethod);
            doc.DocumentElement.AppendChild(soapBody);
            return Encoding.UTF8.GetBytes(doc.OuterXml);
        }
        /// <summary>
        /// 动态调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string DynamicWebService(string url, string className, string methodName, Hashtable parms) {
            object[] args = new object[parms.Keys.Count]; var i = 0;
            foreach (string k in parms.Keys) { args[i] = parms[k]; i++; }
            return WebService.InvokeWebService(url, className, methodName, args).ToString();
        }
        /// <summary>
        /// 动态调用WebService
        /// </summary>
        /// <param name="url">server接口</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string DynamicWebService(string url, string className, string methodName, IList<UrlParameter> parms) {
            object[] args = new object[parms.Count]; var i = 0;
            foreach (UrlParameter k in parms) { args[i] = k.ParameterValue; i++; }
            return WebService.InvokeWebService(url, className, methodName, args).ToString();
        }
    }
}
