//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Xml;
using System.IO;

namespace Pub.Class {
    /// <summary>
    /// Xmls操作类
    /// 
    /// 修改纪录
    ///     2006.05.18 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    ///     string strXmlFile = Server.MapPath("~/web.config");
    ///     Xml2 _xml = new Xml2(strXmlFile);
    ///     _xml.AddNode("configuration//appSettings","add", "key|value", "12|1111111");
    ///     _xml.AddNode("configuration//appSettings", "add", "key|value", "12|1111111", "cexo255");
    ///     Response.Write(_xml.getNodeText("configuration//appSettings//add[@key='12']"));
    ///     _xml.SetAttr("configuration//appSettings//add[@key='']", "value|providerName", "aaaaaaaaaaaa3|System.Data.SqlClient3");
    ///     _xml.AddAttr("configuration//appSettings//add[@key='']", "value|providerName","aaaaaaaaaaaa|System.Data.SqlClient");
    ///     Response.Write(_xml.getAttr("configuration//appSettings//add[@key='']", "value|providerName"));
    ///     _xml.Save();
    ///     switch (_xml.State) { 
    ///         case 0:
    ///             Js.Alert(this, "操作成功！");
    ///             break;
    ///         case 1:
    ///             Js.Alert(this, "无法加载XML文件");
    ///             break;
    ///         case 2:
    ///             Js.Alert(this, "保存失败");
    ///             break;
    ///         case 3:
    ///             Js.Alert(this, "参数对应不正确");
    ///             break;
    ///         case 4:
    ///             Js.Alert(this, "操作错误");
    ///             break;
    ///     }
    ///     string xmlText = _xml.ToXmlText();
    /// </code>
    /// </example>
    /// </summary>
    public class Xml2 {
        //#region 私有成员
        private readonly string strXmlFile = string.Empty;
        private XmlDocument objXmlDoc = new XmlDocument();
        private int state = 0;
        //#endregion
        //#region 属性
        /// <summary>
        /// 返回操作状态
        /// </summary>
        public int State { get { return state; } }
        //#endregion
        //#region 构造器
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="xmlFile"></param>
        public Xml2(string xmlFile) {
            strXmlFile = xmlFile;
            try { objXmlDoc.Load(xmlFile); } catch { state = 1; } //无法加载XML文件
        }
        //#endregion
        //#region GetData
        /// <summary>
        /// 返回XML文件所有数据
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetData() {
            DataSet ds = new DataSet();
            ds.ReadXml(@strXmlFile);
            return ds;
        }
        /// <summary>
        /// 返回指定结点的所有数据
        /// </summary>
        /// <param name="node">结点</param>
        /// <returns>DataSet</returns>
        public DataSet GetData(string node) {
            string mainNode = node.TrimEnd('/');
            DataSet ds = new DataSet();
            StringReader read = new StringReader(objXmlDoc.SelectSingleNode(mainNode).OuterXml);
            ds.ReadXml(read);
            return ds;
        }
        //#endregion
        //#region Node/DelNode/GetNodeText/SetNodeText/AddNode
        /// <summary>
        /// 取结点的内容
        /// </summary>
        /// <param name="node">结点</param>
        /// <returns>内容</returns>
        public string GetNodeText(string node) {
            string mainNode = node.TrimEnd('/'), _value = "";
            XmlNode objNode = objXmlDoc.SelectSingleNode(mainNode);
            _value = objNode.InnerText;
            return _value;
        }
        /// <summary>
        /// 重新设置结点的内容
        /// </summary>
        /// <param name="node">结点</param>
        /// <param name="content">内容</param>
        public void SetNodeText(string node, string content) {
            string mainNode = node.TrimEnd('/');
            XmlNode objNode = objXmlDoc.SelectSingleNode(mainNode);
            objNode.InnerText = content;
        }
        /// <summary>
        /// 添加有内容的结点。属性名，属性值支持用“|”分开的字符串
        /// </summary>
        /// <param name="mainNode">当前结点</param>
        /// <param name="node">新结点名</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="attributeValue">属性值</param>
        /// <param name="content">内容</param>
        public void AddNode(string mainNode, string node, string attributeName, string attributeValue, string content) {
            string _mainNode = mainNode.TrimEnd('/');
            string[] attributeNameArr = attributeName.Split('|'), attributeValueArr = attributeValue.Split('|');
            if (attributeValueArr.Length != attributeNameArr.Length) { state = 3; return; }//参数不正确
            XmlNode objNode = objXmlDoc.SelectSingleNode(_mainNode);
            XmlElement objElement = objXmlDoc.CreateElement(node);
            if (attributeName.Trim() != "") {
                for (int i = 0; i <= attributeNameArr.Length - 1; i++) {
                    if (objNode.Attributes[attributeNameArr[i]].IsNull()) {
                        objElement.SetAttribute(attributeNameArr[i], attributeValueArr[i]);
                    }
                }
            }
            objElement.InnerText = content;
            objNode.AppendChild(objElement);
        }
        /// <summary>
        /// 添加无内容的结点。属性名，属性值支持用“|”分开的字符串
        /// </summary>
        /// <param name="mainNode">当前结点</param>
        /// <param name="node">新结点名</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="attributeValue">属性值</param>
        public void AddNode(string mainNode, string node, string attributeName, string attributeValue) {
            string _mainNode = mainNode.TrimEnd('/');
            string[] attributeNameArr = attributeName.Split('|'), attributeValueArr = attributeValue.Split('|');
            if (attributeValueArr.Length != attributeNameArr.Length) { state = 3; return; }//参数不正确
            XmlNode objNode = objXmlDoc.SelectSingleNode(_mainNode);
            XmlElement objElement = objXmlDoc.CreateElement(node);
            if (attributeName.Trim() != "") {
                for (int i = 0; i <= attributeNameArr.Length - 1; i++) {
                    if (objNode.Attributes[attributeNameArr[i]].IsNull()) {
                        objElement.SetAttribute(attributeNameArr[i], attributeValueArr[i]);
                    }
                }
            }
            objNode.AppendChild(objElement);
        }
        /// <summary>
        /// 删除结点
        /// </summary>
        /// <param name="mainNode"></param>
        public void DelNode(string mainNode) {
            string _mainNode = mainNode.TrimEnd('/');
            XmlNode objNode = objXmlDoc.SelectSingleNode(_mainNode);
            if (objNode.IsNotNull()) objNode.ParentNode.RemoveChild(objNode);
        }
        //#endregion
        //#region Attr
        /// <summary>
        /// 取指定结点的属性值，属性名支持用“|”分开的字符串
        /// </summary>
        /// <param name="node">结点</param>
        /// <param name="attributeName">属性名</param>
        /// <returns></returns>
        public string GetAttr(string node, string attributeName) {
            string mainNode = node.TrimEnd('/'), _value = "";
            string[] attributeNameArr = attributeName.Split('|');
            XmlNode objNode = objXmlDoc.SelectSingleNode(mainNode);
            for (int i = 0; i <= attributeNameArr.Length - 1; i++) {
                try {
                    _value += objNode.Attributes[attributeNameArr[i]].Value.ToString() + "|";
                } catch { _value += "|"; }
            }
            return _value.Substring(0, _value.Length - 1);
        }
        /// <summary>
        /// 为指定结点添加新的属性值，如果存在则不添加。属性名，属性值支持用“|”分开的字符串
        /// </summary>
        /// <param name="node">结点</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="attributeValue">属性值</param>
        public void AddAttr(string node, string attributeName, string attributeValue) {
            string _mainNode = node.TrimEnd('/');
            string[] attributeNameArr = attributeName.Split('|'), attributeValueArr = attributeValue.Split('|');
            if (attributeValueArr.Length != attributeNameArr.Length) { state = 3; return; }//参数不正确
            XmlElement objElement = (XmlElement)objXmlDoc.SelectSingleNode(_mainNode);
            try {
                for (int i = 0; i <= attributeNameArr.Length - 1; i++) {
                    if (objElement.Attributes[attributeNameArr[i]].IsNull()) {
                        objElement.SetAttribute(attributeNameArr[i], attributeValueArr[i]);
                    }
                }
            } catch { state = 4; }//操作错误
        }
        /// <summary>
        /// 设置指定的属性值,此属性必需存在。属性名，属性值支持用“|”分开的字符串
        /// </summary>
        /// <param name="node">结点</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="attributeValue">属性值</param>
        public void SetAttr(string node, string attributeName, string attributeValue) {
            string mainNode = node.TrimEnd('/');
            string[] attributeNameArr = attributeName.Split('|'), attributeValueArr = attributeValue.Split('|');
            if (attributeValueArr.Length != attributeNameArr.Length) { state = 3; return; }//参数不正确
            XmlNode objNode = objXmlDoc.SelectSingleNode(mainNode);
            for (int i = 0; i <= attributeNameArr.Length - 1; i++) {
                try {
                    objNode.Attributes[attributeNameArr[i]].Value = attributeValueArr[i];
                } catch { }
            }
        }
        //#endregion
        //#region 保存XML文件
        /// <summary>
        /// 保存XML文件
        /// </summary>
        /// <example>
        /// <code>
        ///     string strXmlFile = Server.MapPath("~/web.config");
        ///     Xml2 _xml = new Xml2(strXmlFile);
        ///     _xml.AddNode("configuration//appSettings","add", "key|value", "12|1111111");
        ///     _xml.AddNode("configuration//appSettings", "add", "key|value", "12|1111111", "cexo255");
        ///     Response.Write(_xml.getNodeText("configuration//appSettings//add[@key='12']"));
        ///     _xml.SetAttr("configuration//appSettings//add[@key='']", "value|providerName", "aaaaaaaaaaaa3|System.Data.SqlClient3");
        ///     _xml.AddAttr("configuration//appSettings//add[@key='']", "value|providerName","aaaaaaaaaaaa|System.Data.SqlClient");
        ///     Response.Write(_xml.getAttr("configuration//appSettings//add[@key='']", "value|providerName"));
        ///     _xml.Save();
        ///     switch (_xml.State) { 
        ///         case 0:
        ///             Js.Alert(this, "操作成功！");
        ///             break;
        ///         case 1:
        ///             Js.Alert(this, "无法加载XML文件");
        ///             break;
        ///         case 2:
        ///             Js.Alert(this, "保存失败");
        ///             break;
        ///         case 3:
        ///             Js.Alert(this, "参数对应不正确");
        ///             break;
        ///         case 4:
        ///             Js.Alert(this, "操作错误");
        ///             break;
        ///     }
        ///     string xmlText = _xml.ToXmlText();
        /// </code>
        /// </example>
        public void Save() {
            try { if (state == 0) objXmlDoc.Save(strXmlFile); } catch { state = 2; }//保存失败
            objXmlDoc = null;
        }
        /// <summary>
        /// 关闭XML对像
        /// </summary>
        public void Close() {
            if (objXmlDoc.IsNotNull()) { objXmlDoc = null; }
        }
        /// <summary>
        /// 转XML字符串
        /// </summary>
        /// <returns></returns>
        public string ToXmlText() {
            return objXmlDoc.OuterXml;
        }
        //#endregion
        //#region 全局方法Create
        /// <summary>
        /// 新建一个XML文件
        /// </summary>
        /// <example>
        /// <code>
        ///     Xml2.Create("c:\\rss\\rss.xml", "", "", "utf-8", "&lt;root>&lt;/root>")
        /// </code>
        /// </example>
        /// <param name="xmlFile">XML文件路径</param>
        /// <param name="cssFile">CSS文件路径</param>
        /// <param name="xlsFile">XLS文件路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="node">根结点</param>
        /// <returns>是否操作成功</returns>
        public static bool Create(string xmlFile, string cssFile, string xlsFile, string encoding, string node) {
            if (node.Trim().Equals("")) return false;
            if (encoding.Trim().Equals("")) encoding = "utf-8";
            string _str = "<?xml version=\"1.0\" encoding=\"" + encoding + "\"?>";
            if (!cssFile.Trim().Equals("")) _str += Environment.NewLine + "<?xml-stylesheet type=\"text/css\" href=\"" + cssFile + "\"?>";
            if (!xlsFile.Trim().Equals("")) _str += Environment.NewLine + "<?xml-stylesheet type=\"text/xsl\" href=\"" + xlsFile + "\" media=\"screen\"?>";
            _str += Environment.NewLine + node;
            return FileDirectory.FileWrite(xmlFile, _str, encoding);
        }
        //#endregion
    }
}


