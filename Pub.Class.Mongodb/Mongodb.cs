//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , LiveXY , Ltd. 
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Pub.Class {

    /// <summary>
    /// Mongodb数据库实例
    /// 
    /// 修改纪录
    ///     2011.10.28 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public partial class Mongodb {
        private static readonly ISafeDictionary<string, MongoClient> clients = new SafeDictionarySlim<string, MongoClient>();
        private static readonly ISafeDictionary<string, MongoServer> servers = new SafeDictionarySlim<string, MongoServer>();
        private static readonly ISafeDictionary<string, MongoDatabase> pool = new SafeDictionarySlim<string, MongoDatabase>();
        private static readonly IList<string> poolkey = new List<string>();
        public static MongoDatabase Pool(string keys) {
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
        public static MongoDatabase Pool(params string[] key) {
            string _key = key[Rand.RndInt(0, key.Length)].ToLower();
            if (pool.Count == 0) Pool();
            if (pool.ContainsKey(_key)) return pool[_key];
            return Pool();
        }
        public static MongoDatabase Pool() {
            int count = pool.Count;
            if (count != poolkey.Count || count == 0) {
                pool.Clear(); servers.Clear(); poolkey.Clear(); count = 0;
                ConnectionStringSettingsCollection conns = WebConfig.GetConn();
                foreach (ConnectionStringSettings info in conns) {
                    string key = info.Name;
                    if (info.ProviderName != "MongoDB") continue;

                    MongoClient client = new MongoClient(info.ConnectionString);
                    clients[key.ToLower()] = client;
                    MongoServer server = client.GetServer();
                    servers[key.ToLower()] = server;
                    pool[key.ToLower()] = server.GetDatabase(key.IndexOf(".") == -1 ? key : key.Split('.')[1]);
                    poolkey.Add(key.ToLower());
                    count++;
                }
            }
            return pool[poolkey[count == 1 ? 0 : Rand.RndInt(0, count)]];
        }
        public static MongoDatabase UsePool(params string[] keys) {
            int count = pool.Count;
            if (count != poolkey.Count || count == 0) {
                pool.Clear(); servers.Clear(); poolkey.Clear(); count = 0;
                ConnectionStringSettingsCollection conns = WebConfig.GetConn();
                foreach (ConnectionStringSettings info in conns) {
                    string key = info.Name;
                    if (info.ProviderName != "MongoDB" || keys.IndexOf(key) == -1) continue;

                    MongoClient client = new MongoClient(info.ConnectionString);
                    clients[key.ToLower()] = client;
                    MongoServer server = client.GetServer();
                    servers[key.ToLower()] = server;
                    pool[key.ToLower()] = server.GetDatabase(key.IndexOf(".") == -1 ? key : key.Split('.')[1]);
                    poolkey.Add(key.ToLower());
                    count++;
                }
            }
            return pool[poolkey[count == 1 ? 0 : Rand.RndInt(0, count)]];
        }
        public static void ClosePool(string key = null) {
            if (key.IsNullEmpty()) {
                foreach (string info in servers.Keys) { servers[info].Disconnect(); }
                pool.Clear();
                poolkey.Clear();
                servers.Clear();
                clients.Clear();
            } else {
                key = key.ToLower();
                if (!pool.ContainsKey(key)) return;
                servers[key].Disconnect();
                pool[key] = null;
                poolkey.Remove(key);
                pool.Remove(key);
                servers.Remove(key);
                clients.Remove(key);
            }
        }
    }
}
