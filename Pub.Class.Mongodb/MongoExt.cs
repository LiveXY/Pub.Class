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
using System.Linq;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace Pub.Class {
    public static class MongoExt {
        public static IList<T> ToPage<T>(this MongoCollection<T> cols, int pageSize, IMongoQuery filterQuery, IMongoSortBy sort, out long totals, IMongoQuery currQuery = null) {
            sort = sort ?? new SortByDocument();
            IList<T> list = new List<T>();
            IMongoQuery query = new QueryDocument();
            if (!currQuery.IsNull()) query = Query.And(query, currQuery);
            totals = cols.Count(query);
            if (!filterQuery.IsNull()) query = Query.And(query, filterQuery);
            list = cols.Find(query).SetLimit(pageSize).SetSortOrder(sort).ToList();
            return list;
        }
        public static string ToBson<T>(this T obj) { return obj.ToJson<T>(); }
    }
}
