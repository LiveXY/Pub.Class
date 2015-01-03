//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Pub.Class {
    /// <summary>
    /// XAttribute 扩展
    /// </summary>
    public static class XAttributeExtensions {
        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetValue(this XAttribute attribute) {
            if (attribute == null) {
                return string.Empty;
            } else {
                return attribute.Value;
            }
        }
    }
}
