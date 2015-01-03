//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , LiveXY , Ltd. 
//------------------------------------------------------------

namespace Pub.Class {
    using System;
    using System.Configuration;
    using MongoDB.Configuration;
    using MongoDB;

    /// <summary>
    /// Mongodb数据库实例
    /// 
    /// 修改纪录
    ///     2011.10.28 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public partial class MongodbDatabase {
        public MongodbDatabase(string pool) { key = pool; }
        protected string connString = null;
        protected string key = "ConnString";
        protected string dbType = null;
        private readonly object lockHelper = new object();
        private Mongo factory = null;
        public string ConnString {
            get {
                if (string.IsNullOrEmpty(connString)) {
                    if (ConfigurationManager.ConnectionStrings[key].IsNotNull()) {
                        connString = ConfigurationManager.ConnectionStrings[key].ToString();
                        dbType = ConfigurationManager.ConnectionStrings[key].ProviderName;
                    }
                }
                return connString;
            }
            set { connString = value; }
        }
        public string Pool {
            get { return key; }
            set { key = value; }
        }
        public string DBType {
            get {
                if (string.IsNullOrEmpty(dbType)) {
                    if (ConfigurationManager.ConnectionStrings[key].IsNotNull()) dbType = ConfigurationManager.ConnectionStrings[key].ProviderName; else dbType = "SqlServer";
                }
                return dbType;
            }
            set { dbType = value; }
        }
        public Mongo Factory {
            get {
                if (factory.IsNull()) {
                    lock (lockHelper) {
                        if (factory.IsNull()) {
                            //System.Web.HttpContext.Current.Response.Write(dbType);
                            //System.Web.HttpContext.Current.Response.End();
                            dbType = DBType;
                            //try {
                                MongoConfigurationBuilder config = new MongoConfigurationBuilder();
                                config.ConnectionString(ConnString);
                                factory = new Mongo(config.BuildConfiguration());
                                factory.Connect();
                            //} catch {
                            //    throw new Exception(dbType + " - 请检查web.config中DbType节点数据库类型是否正确，例如：MongoDB");
                            //}
                        }
                    }
                }
                return factory;
            }
        }
        public void Close() {
            factory.Disconnect();
            factory.Dispose();
        }
        public IMongoDatabase db {
            get {
                return Factory.GetDatabase(key.IndexOf(".") == -1 ? key : key.Split('.')[1]);
            }
        }
        public IMongoDatabase this[string db] {
            get {
                return Factory.GetDatabase(db);
            }
        }
    }
}