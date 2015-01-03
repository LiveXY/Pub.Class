//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Pub.Class;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;

namespace Pub.Class {
    /// <summary>    
    /// 实现IConfigurationSectionHandler接口，以对自定义节点进行访问    
    /// 
    /// 修改纪录
    ///     2011.07.09 版本：1.0 livexy 创建此类
    ///     
    /// </summary>    
    public class URLRewriterHandler : IConfigurationSectionHandler {
        /// <summary>        
        /// 该方法无需主动调用        
        /// 它在ConfigurationManager.GetSection()被调用时根据改配置节声明中所定义的类名和路径自动实例化配置节处理类        
        /// </summary>        
        public object Create(object parent, object configContext, System.Xml.XmlNode section) {
            XmlSerializer ser = new XmlSerializer(typeof(RewriterConfiguration));
            return ser.Deserialize(new XmlNodeReader(section));
        }
    }
}
