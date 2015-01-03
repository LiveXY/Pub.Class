//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Linq;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;

namespace Pub.Class {
    /// <summary>
    /// XDocument XmlNode
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class XmlNodeExtensions {
#if !NET20
        /// <summary>
        /// XDocument 转对像
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="xdoc">XDocument扩展</param>
        /// <returns></returns>
        public static T FromXDoc<T>(this XDocument xdoc) {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (XmlReader r = xdoc.CreateReader()) return (T)xs.Deserialize(r);
        }
#endif
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="parentNode">XmlNode扩展</param>
        /// <param name="name">节点名</param>
        /// <returns></returns>
        public static XmlNode CreateChildNode(this XmlNode parentNode, string name) {
            XmlDocument document = parentNode is XmlDocument ? (XmlDocument)parentNode : parentNode.OwnerDocument;
            XmlNode node = document.CreateElement(name);
            parentNode.AppendChild(node);
            return node;
        }
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="parentNode">XmlNode扩展</param>
        /// <param name="name">节点名</param>
        /// <param name="namespaceUri"></param>
        /// <returns></returns>
        public static XmlNode CreateChildNode(this XmlNode parentNode, string name, string namespaceUri) {
            XmlDocument document = parentNode is XmlDocument ? (XmlDocument)parentNode : parentNode.OwnerDocument;
            XmlNode node = document.CreateElement(name, namespaceUri);
            parentNode.AppendChild(node);
            return node;
        }
        /// <summary>
        /// 添加CData节点
        /// </summary>
        /// <param name="parentNode">XmlNode扩展</param>
        /// <returns></returns>
        public static XmlCDataSection CreateCDataSection(this XmlNode parentNode) {
            return parentNode.CreateCDataSection(string.Empty);
        }
        /// <summary>
        /// 添加CData节点
        /// </summary>
        /// <param name="parentNode">XmlNode扩展</param>
        /// <param name="data">DATA</param>
        /// <returns></returns>
        public static XmlCDataSection CreateCDataSection(this XmlNode parentNode, string data) {
            XmlDocument document = parentNode is XmlDocument ? (XmlDocument)parentNode : parentNode.OwnerDocument;
            XmlCDataSection node = document.CreateCDataSection(data);
            parentNode.AppendChild(node);
            return node;
        }
        /// <summary>
        /// 取CData节点
        /// </summary>
        /// <param name="parentNode">XmlNode扩展</param>
        /// <returns></returns>
        public static string GetCDataSection(this XmlNode parentNode) {
            foreach (var node in parentNode.ChildNodes) {
                if (node is XmlCDataSection) return ((XmlCDataSection)node).Value;
            }

            return null;
        }
        /// <summary>
        /// 取节点属性
        /// </summary>
        /// <param name="node">XmlNode扩展</param>
        /// <param name="attributeName">属性名</param>
        /// <returns></returns>
        public static string GetAttribute(this XmlNode node, string attributeName) {
            return GetAttribute(node, attributeName, null);
        }
        /// <summary>
        /// 取节点属性
        /// </summary>
        /// <param name="node">XmlNode扩展</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetAttribute(this XmlNode node, string attributeName, string defaultValue) {
            XmlAttribute attribute = node.Attributes[attributeName];
            return attribute.IsNotNull() ? attribute.InnerText : defaultValue;
        }
        /// <summary>
        /// 取节点属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">XmlNode扩展</param>
        /// <param name="attributeName">属性名</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this XmlNode node, string attributeName) {
            return GetAttribute<T>(node, attributeName, default(T));
        }
        /// <summary>
        /// 取节点属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">XmlNode扩展</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this XmlNode node, string attributeName, T defaultValue) {
            var value = GetAttribute(node, attributeName);

            return !value.IsNullEmpty() ? value.ConvertTo<T>(defaultValue) : defaultValue;
        }
        /// <summary>
        /// 设置节点属性值
        /// </summary>
        /// <param name="node">XmlNode扩展</param>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        public static void SetAttribute(this XmlNode node, string name, object value) {
            SetAttribute(node, name, value.IsNotNull() ? value.ToString() : null);
        }
        /// <summary>
        /// 设置节点属性值
        /// </summary>
        /// <param name="node">XmlNode扩展</param>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        public static void SetAttribute(this XmlNode node, string name, string value) {
            if (node.IsNotNull()) {
                var attribute = node.Attributes[name, node.NamespaceURI];

                if (attribute.IsNull()) {
                    attribute = node.OwnerDocument.CreateAttribute(name, node.OwnerDocument.NamespaceURI);
                    node.Attributes.Append(attribute);
                }

                attribute.InnerText = value;
            }
        }
    }
}
