//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , LiveXY , Ltd. 
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text;

namespace Pub.Class {
    //public class MongoDocuments {
    //    public BsonDocument BsonDocument { get; set; }
    //    public QueryDocument QueryDocument { get; set; }
    //    public UpdateDocument UpdateDocument { get; set; }
    //}

    public class MongoSQL {
        private BsonDocument bson = new BsonDocument();
        /// <summary>
        /// 值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <param name="useNULL">是否插入为null的数据</param>
        /// <returns>this</returns>
        public MongoSQL Value(string field, BsonValue value, bool useNULL) {
            if ((value.IsNull() || value == BsonNull.Value) && !useNULL) return this;
            bson[field] = value;
            return this;
        }
        /// <summary>
        /// 值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>this</returns>
        public MongoSQL Value(string field, BsonValue value) {
            return Value(field, value, false);
        }
        /// <summary>
        /// 返回BsonDocument
        /// </summary>
        /// <returns></returns>
        public BsonDocument BsonDocument() {
            return bson;
        }
        /// <summary>
        /// 返回UpdateDocument
        /// </summary>
        /// <returns></returns>
        public IMongoUpdate UpdateDocument() {
            return new UpdateDocument { { "$set", bson } };;
        }
        private QueryDocument query = new QueryDocument();
        public MongoSQL Where(string field, BsonValue value, bool useNULL) {
            if ((value.IsNull() || value == BsonNull.Value) && !useNULL) return this;
            query[field] = value;
            return this;
        }
        public MongoSQL Where(string field, BsonValue value) {
            return Where(field, value, false);
        }
        public IMongoQuery QueryDocument() {
            return query;
        }
    }
}
