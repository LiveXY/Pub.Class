//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace Pub.Class {
    /// <summary>
    /// Web.Config配置类
    /// 
    /// 修改纪录
    ///     2006.05.15 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class WebConfig {
        /// <summary>
        /// Web.Config 中配置DoMain区域
        /// </summary>
        public static readonly string DoMain = WebConfig.GetApp("DoMain") ?? string.Empty;
        //#region GetApp
        /// <summary>
        /// 取appSettings结点数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>返回值</returns>
        public static string GetApp(string key) {
            if (ConfigurationManager.AppSettings[key].IsNotNull()) return ConfigurationManager.AppSettings[key].ToString();
            return null;
        }
        /// <summary>
        /// 取appSettings结点数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">默认值</param>
        /// <returns>返回值</returns>
        public static T GetApp<T>(string key, T value) {
            if (ConfigurationManager.AppSettings[key].IsNotNull()) return ConfigurationManager.AppSettings[key].ToString().ConvertTo<T>();
            return default(T);
        }
        /// <summary>
        /// 取appSettings结点数据
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>取appSettings结点数据</returns>
        public static string GetApp(int i) {
            if (ConfigurationManager.AppSettings[i].IsNotNull()) return ConfigurationManager.AppSettings[i].ToString();
            return null;
        }
        /// <summary>
        /// 取appSettings结点数据
        /// </summary>
        /// <returns>取appSettings结点数据</returns>
        public static NameValueCollection GetApp() {
            return ConfigurationManager.AppSettings;
        }
        /// <summary>
        /// 修改appSettings结点数据 如果不存在 添加
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">取appSettings结点数据</param>
        public static void SetApp(string key, string value) {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection section = (AppSettingsSection)config.GetSection("appSettings");
            if (section.Settings[key].IsNull()) {
                section.Settings.Add(key, value);
            } else {
                section.Settings[key].Value = value;
            }
            config.Save();
        }
        /// <summary>
        /// 删除一个appSettings节点
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static void RemoveApp(string key) {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection section = config.AppSettings;
            if (section.Settings[key].IsNotNull()) section.Settings.Remove(key);
            config.Save();
        }
        //#endregion
        //#region GetConn
//#if !MONO40
        /// <summary>
        /// 取connectionStrings结点数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>取connectionStrings结点数据</returns>
        public static string GetConn(string key) {
            if (ConfigurationManager.ConnectionStrings[key].IsNotNull()) return ConfigurationManager.ConnectionStrings[key].ToString();
            return null;
        }
        /// <summary>
        /// 取connectionStrings结点数据
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>取connectionStrings结点数据</returns>
        public static string GetConn(int i) {
            if (ConfigurationManager.ConnectionStrings[i].IsNotNull()) return ConfigurationManager.ConnectionStrings[i].ToString();
            return null;
        }
        /// <summary>
        /// 取connectionStrings结点数据
        /// </summary>
        /// <returns>取connectionStrings结点数据</returns>
        public static ConnectionStringSettingsCollection GetConn() {
            return ConfigurationManager.ConnectionStrings;
        }
        /// <summary>
        /// 设置/重写一个数据库连接串
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="connString">连接字符串</param>
        /// <param name="providerName">数据库类型</param>
        /// <returns></returns>
        public static void SetConn(string key, string connString, string providerName) {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            ConnectionStringsSection section = config.ConnectionStrings;
            if (section.ConnectionStrings[key].IsNull()) {
                section.ConnectionStrings.Add(new ConnectionStringSettings(key, connString, providerName));
            } else {
                section.ConnectionStrings[key].ConnectionString = connString;
            }
            config.Save();
        }
//#endif
        //#endregion
        //#region GetSection
        /// <summary>
        /// GetSection&lt;appSettings>("configuration/appSettings")
        /// </summary>
        /// <typeparam name="TReturn">返回类型</typeparam>
        /// <param name="sectionName">节点</param>
        /// <returns></returns>
        public static TReturn GetSection<TReturn>(string sectionName) {
            return (TReturn)ConfigurationManager.GetSection(sectionName);
        }
        //#endregion
    }
}
