//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , LiveXY , Ltd. 
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using MongoDB;

namespace Pub.Class {

    /// <summary>
    /// Mongodb数据库实例
    /// 
    /// 修改纪录
    ///     2011.10.28 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public partial class Mongodb {
        private static readonly ISafeDictionary<string, IMongo> factorys = new SafeDictionarySlim<string, IMongo>();
        private static readonly ISafeDictionary<string, IMongoDatabase> pool = new SafeDictionarySlim<string, IMongoDatabase>();
        private static readonly IList<string> poolkey = new List<string>();
        public static IMongoDatabase Pool(string keys) {
            if (keys.IsNullEmpty()) keys = "";
            string[] _keys = keys.ToLower().Split(';');
            if (pool.Count == 0) Pool();
            string _key = _keys[0];
            if (_keys.Length == 1) {
                if (pool.ContainsKey(_key)) return pool[_key];
            } else { 
                _key = _keys[Rand.RndInt(0, _keys.Length)];
                if (pool.ContainsKey(_key)) return pool[_key];
            }
            return Pool();
        }
        public static IMongoDatabase Pool(params string[] key) {
            string _key = key[Rand.RndInt(0, key.Length)].ToLower();
            if (pool.Count == 0) Pool();
            if (pool.ContainsKey(_key)) return pool[_key];
            return Pool();
        }
        public static IMongoDatabase Pool() {
            int count = pool.Count;
            if (count != poolkey.Count || count == 0) {
                pool.Clear(); factorys.Clear(); poolkey.Clear(); count = 0;
                ConnectionStringSettingsCollection conns = WebConfig.GetConn();
                foreach (ConnectionStringSettings info in conns) {
                    string key = info.Name;
                    if (info.ProviderName != "MongoDB") continue;

                    Mongo factory = new Mongo(info.ConnectionString);
                    factory.Connect();
                    factorys[key.ToLower()] = factory;
                    pool[key.ToLower()] = factory.GetDatabase(key.IndexOf(".") == -1 ? key : key.Split('.')[1]);
                    poolkey.Add(key.ToLower());
                    count++;
                }
            }
            return pool[poolkey[count == 1 ? 0 : Rand.RndInt(0, count)]];
        }
        public static IMongoDatabase UsePool(params string[] keys) {
            int count = pool.Count;
            if (count != poolkey.Count || count == 0) {
                pool.Clear(); factorys.Clear(); poolkey.Clear(); count = 0;
                ConnectionStringSettingsCollection conns = WebConfig.GetConn();
                foreach (ConnectionStringSettings info in conns) {
                    string key = info.Name;
                    if (info.ProviderName != "MongoDB" || keys.IndexOf(key) == -1) continue;

                    Mongo factory = new Mongo(info.ConnectionString);
                    factory.Connect();
                    factorys[key.ToLower()] = factory;
                    pool[key.ToLower()] = factory.GetDatabase(key.IndexOf(".") == -1 ? key : key.Split('.')[1]);
                    poolkey.Add(key.ToLower());
                    count++;
                }
            }
            return pool[poolkey[count == 1 ? 0 : Rand.RndInt(0, count)]];
        }
        public static void ClosePool(string key = null) {
            if (key.IsNullEmpty()) {
                foreach (string info in factorys.Keys) { factorys[info].Disconnect(); factorys[info].Dispose(); }
                pool.Clear();
                poolkey.Clear();
                factorys.Clear();
            } else {
                key = key.ToLower();
                if (!pool.ContainsKey(key)) return;
                factorys[key].Disconnect();
                factorys[key].Dispose();
                pool[key] = null;
                poolkey.Remove(key);
                pool.Remove(key);
                factorys.Remove(key);
            }
        }
    }
}
