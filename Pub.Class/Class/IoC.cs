//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;

namespace Pub.Class {
    /// <summary>
    /// IoC依赖注入接口
    /// 
    /// 修改纪录
    ///     2012.06.09 版本：1.0 livexy 创建此接口
    /// 
    /// </summary>
    public interface IIoC {
        bool Has(Type type, string id = "");
        bool Has(Type type, params object[] args);
        IIoC Register(Type type, string id = "");
        IIoC Register(Type type, params object[] args);
        object Get(Type type, string id = "");
        object Get(Type type, params object[] args);
        void UnRegister(Type type, string id = "");
        void UnRegister(Type type, params object[] args);
    }
    /// <summary>
    /// IoC依赖注入扩展
    /// 
    /// 修改纪录
    ///     2012.06.09 版本：1.0 livexy 创建此接口
    /// 
    /// </summary>
    public static class IIoCExtensions { 
        public static IIoC Register<T>(this IIoC ioc) { return ioc.Register(typeof(T)); }
        public static IIoC Register(this IIoC ioc, Type type) { return ioc.Register(type); }
        public static IIoC Register<T>(this IIoC ioc, string id) { return ioc.Register(typeof(T), id); }
        public static IIoC Register<T>(this IIoC ioc, params object[] args) { return ioc.Register(typeof(T), args); }
        public static IIoC Register(this IIoC ioc, Type type, params object[] args) { return ioc.Register(type, args); }
    }
    /// <summary>
    /// IoC依赖注入接口的实现
    /// 
    /// 修改纪录
    ///     2012.06.09 版本：1.0 livexy 创建此接口
    /// 
    /// </summary>
    public class _IoC : IIoC {
        private static SafeDictionary<string, object> service = new SafeDictionary<string, object>();
        public bool Has(Type type, string id = "") { return service.ContainsKey(type.FullName + "/" + id); }
        public bool Has(Type type, params object[] args) { 
            string key = type.FullName;
            foreach (var arg in args) { key += "/" + arg.ToString(); }
            return service.ContainsKey(key); 
        }

        public IIoC Register(Type type, string id = "") {
            string key = type.FullName + "/" + id;
            object obj = Activator.CreateInstance(type);
            if (!service.ContainsKey(key)) service.Add(key, obj);
            return this; 
        }
        public IIoC Register(Type type, params object[] args) {
            string key = type.FullName;
            foreach (var arg in args) { key += "/" + arg.ToString(); }
            object obj = Activator.CreateInstance(type, args);
            if (!service.ContainsKey(key)) service.Add(key, obj);
            return this; 
        }

        public object Get(Type type, string id = "") {
            string key = type.FullName + "/" + id;
            return service.ContainsKey(key) ? service[key] : null; 
        }
        public object Get(Type type, params object[] args) {
            string key = type.FullName;
            foreach (var arg in args) { key += "/" + arg.ToString(); }
            return service.ContainsKey(key) ? service[key] : null; 
        }

        public void UnRegister(Type type, string id = "") {
            string key = type.FullName + "/" + id;
            if (service.ContainsKey(key)) {
                service[key] = null;
                service.Remove(key);
            }
        }
        public void UnRegister(Type type, params object[] args) { 
            string key = type.FullName;
            foreach (var arg in args) { key += "/" + arg.ToString(); }
            if (service.ContainsKey(key)) {
                service[key] = null;
                service.Remove(key);
            }
        }
    }
    /// <summary>
    /// IoC依赖注入
    /// 
    /// 修改纪录
    ///     2012.06.09 版本：1.0 livexy 创建此接口
    /// 
    /// </summary>
    public class IoC {
        private static IIoC _current = new _IoC();
        public static bool Has<T>() { return _current.Has(typeof(T)); }
        public static bool Has(Type type) { return _current.Has(type); }
        public static bool Has<T>(string id) { return _current.Has(typeof(T), id); }
        public static bool Has<T>(params object[] args) { return _current.Has(typeof(T), args); }
        public static bool Has(Type type, params object[] args) { return _current.Has(type, args); }

        public static IIoC Register<T>() { return _current.Register(typeof(T)); }
        public static IIoC Register(Type type) { return _current.Register(type); }
        public static IIoC Register<T>(string id) { return _current.Register(typeof(T), id); }
        public static IIoC Register<T>(params object[] args) { return _current.Register(typeof(T), args); }
        public static IIoC Register(Type type, params object[] args) { return _current.Register(type, args); }

        public static T Get<T>() { 
            if (!Has<T>()) Register<T>();
            return (T)_current.Get(typeof(T));
        }
        public static T Get<T>(Type type) { 
            if (!Has(type)) Register(type);
            return (T)_current.Get(type); 
        }
        public static T Get<T>(string id) { 
            if (!Has<T>(id)) Register<T>(id);
            return  (T)_current.Get(typeof(T), id); 
        }
        public static T Get<T>(params object[] args) {
            if (!Has<T>(args)) Register<T>(args);
            return (T)_current.Get(typeof(T), args); 
        }
        public static T Get<T>(Type type, params object[] args) {
            if (!Has(type, args)) Register(type, args);
            return (T)_current.Get(type, args); 
        }
    }
}
