//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Web.Script.Serialization;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Linq.Expressions;
using System.Web.Caching;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Pub.Class {
    /// <summary>
    /// PropertyInfo扩展
    /// </summary>
    public static class PropertyInfoExtensions {
        public static T GetAttribute<T>(this PropertyInfo pi) where T : Attribute {
            object[] attributes = pi.GetCustomAttributes(typeof(T), true);
            if (attributes.Length == 0) return null;
            return attributes[0] as T;
        }
    }
}
