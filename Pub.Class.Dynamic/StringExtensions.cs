using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Pub.Class {
    public static class StringExtension2 {
        /// <summary>
        /// ToDynamicJson
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static dynamic ToDynamicJson(this string json) {
            return DynamicJson.Parse(json);
        }
        /// <summary>
        /// ToDynamicXML
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <returns></returns>
        public static dynamic ToDynamicXml(this string xml) {
            return new DynamicXml(xml);
        }
    }
}