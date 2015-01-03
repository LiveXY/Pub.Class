//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , LiveXY , Ltd. 
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;

namespace Pub.Class {

    /// <summary>
    /// Oracle数据库实例
    /// 
    /// 修改纪录
    ///     2011.10.28 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public partial class Mongodb {
        private static readonly ISafeDictionary<string, MongodbDatabase> pool = new SafeDictionarySlim<string, MongodbDatabase>();
        private static readonly IList<string> poolkey = new List<string>();
        public static MongodbDatabase Pool(string key) {
            key = key.ToLower();
            if (pool.ContainsKey(key)) return pool[key];
            return Pool();
        }
        public static MongodbDatabase Pool(params string[] key) {
            string _key = key[Rand.RndInt(0, key.Length)].ToLower();
            if (pool.ContainsKey(_key)) return pool[_key];
            return Pool();
        }
        public static MongodbDatabase Pool() {
            int count = pool.Count;
            if (count != poolkey.Count || count == 0) {
                pool.Clear(); poolkey.Clear(); count = 0;
                ConnectionStringSettingsCollection conns = WebConfig.GetConn();
                foreach (ConnectionStringSettings info in conns) {
                    string key = info.Name;
                    string type = info.ProviderName;
                    if (type != "MongoDB") continue;

                    MongodbDatabase db = new MongodbDatabase(key);
                    db.Pool = key;
                    pool[key.ToLower()] = db;
                    poolkey.Add(key.ToLower());
                    count++;
                }
            }
            return pool[poolkey[count == 1 ? 0 : Rand.RndInt(0, count)]];
        }
        public static MongodbDatabase UsePool(params string[] keys) {
            int count = pool.Count;
            if (count != poolkey.Count || count == 0) {
                pool.Clear(); poolkey.Clear(); count = 0;
                ConnectionStringSettingsCollection conns = WebConfig.GetConn();
                foreach (ConnectionStringSettings info in conns) {
                    string key = info.Name;
                    if (key != "MongoDB" || keys.IndexOf(key) == -1) continue;

                    MongodbDatabase db = new MongodbDatabase(key);
                    db.Pool = key;
                    pool[key.ToLower()] = db;
                    poolkey.Add(key.ToLower());
                    count++;
                }
            }
            return pool[poolkey[count == 1 ? 0 : Rand.RndInt(0, count)]];
        }
        public static void ClosePool(string key = null) {
            if (key.IsNullEmpty()) {
                pool.Clear();
                poolkey.Clear();
            } else {
                key = key.ToLower();
                if (!pool.ContainsKey(key)) return;
                pool[key].Close();
                pool[key] = null;
                poolkey.Remove(key);
                pool.Remove(key);
            }
        }
        public static void AddPool(string key, MongodbDatabase db = null) {
            if (key.IsNullEmpty()) return;
            key = key.ToLower();
            if (pool.ContainsKey(key)) return;
            if (db.IsNull()) {
                MongodbDatabase newdb = new MongodbDatabase(key);
                newdb.Pool = key;
                pool[key] = newdb;
                poolkey.Add(key);
            } else {
                pool[key] = db;
                poolkey.Add(key);
            }
        }
        public static void AddPool(MongodbDatabase db) {
            if (db.IsNull()) return;
            AddPool(db.Pool, db);
        }
    }
}
