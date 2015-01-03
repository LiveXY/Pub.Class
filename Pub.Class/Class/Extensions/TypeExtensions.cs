//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Web.Script.Serialization;
using System.Linq;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Reflection.Emit;

namespace Pub.Class {
    /// <summary>
    /// Type 扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class TypeExtensions {
        private static readonly ISafeDictionary<Type, Dictionary<string, PropertyInfo>> t_cache = new SafeDictionary<Type, Dictionary<string, PropertyInfo>>();
        private static readonly ISafeDictionary<Type, PropertyInfo[]> t_cache2 = new SafeDictionary<Type, PropertyInfo[]>();
        /// <summary>
        /// 创建对像的实例
        /// </summary>
        /// <param name="type">Type扩展</param>
        /// <param name="constructorParameters">参数</param>
        /// <returns></returns>
        public static object CreateInstance(this Type type, params object[] constructorParameters) {
            return CreateInstance<object>(type, constructorParameters);
        }
        /// <summary>
        /// 创建对像的实例 泛形
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="type">Type扩展</param>
        /// <param name="constructorParameters">参数</param>
        /// <returns></returns>
        public static T CreateInstance<T>(this Type type, params object[] constructorParameters) {
            var instance = Activator.CreateInstance(type, constructorParameters);
            return (T)instance;
        }
        /// <summary>
        /// 将对像的属性CACHE
        /// </summary>
        /// <param name="type">Type扩展</param>
        /// <param name="propertyName">属性</param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyCache(this Type type, string propertyName) {
            if (!t_cache.ContainsKey(type)) t_cache[type] = new Dictionary<string, PropertyInfo>();
            if (!t_cache[type].ContainsKey(propertyName)) {
                PropertyInfo value = type.GetProperty(propertyName);
                t_cache[type][propertyName] = value;
                return value;
            }
            return t_cache[type][propertyName];
        }
        /// <summary>
        /// 将对像的属性CACHE
        /// </summary>
        /// <param name="type">Type扩展</param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertiesCache(this Type type) {
            if (!t_cache2.ContainsKey(type)) {
                PropertyInfo[] value = type.GetProperties();
                t_cache2[type] = value;
                return value;
            }
            return t_cache2[type];
        }
#if !MONO40
        /// <summary>
        /// Is Nullable
        /// </summary>
        /// <param name="t">Type扩展</param>
        /// <returns></returns>
        public static bool IsNullable(this Type t) {
            return !t.IsSubclassOf(typeof(ValueType)) || t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        /// <summary>
        /// 是否系统类型
        /// </summary>
        /// <param name="type">Type扩展</param>
        /// <returns></returns>
        public static bool IsSystemType(this Type type) {
            if (((((type != typeof(string)) && (type != typeof(short))) && ((type != typeof(short?)) && (type != typeof(int)))) && (((type != typeof(int?)) && (type != typeof(long))) && ((type != typeof(long?)) && (type != typeof(decimal))))) && ((((type != typeof(decimal?)) && (type != typeof(double))) && ((type != typeof(double?)) && (type != typeof(DateTime)))) && (((type != typeof(DateTime?)) && (type != typeof(Guid))) && ((type != typeof(Guid?)) && (type != typeof(bool)))))) {
                return (type == typeof(bool?));
            }
            return true;
        }
        /// <summary>
        /// 系统类型转DbType
        /// </summary>
        /// <param name="Type">Type扩展</param>
        /// <returns></returns>
        public static DbType ToDbType(this Type Type) {
            if (Type == typeof(byte)) return DbType.Byte;
            else if (Type == typeof(sbyte)) return DbType.SByte;
            else if (Type == typeof(short)) return DbType.Int16;
            else if (Type == typeof(ushort)) return DbType.UInt16;
            else if (Type == typeof(int)) return DbType.Int32;
            else if (Type == typeof(uint)) return DbType.UInt32;
            else if (Type == typeof(long)) return DbType.Int64;
            else if (Type == typeof(ulong)) return DbType.UInt64;
            else if (Type == typeof(float)) return DbType.Single;
            else if (Type == typeof(double)) return DbType.Double;
            else if (Type == typeof(decimal)) return DbType.Decimal;
            else if (Type == typeof(bool)) return DbType.Boolean;
            else if (Type == typeof(string)) return DbType.String;
            else if (Type == typeof(char)) return DbType.StringFixedLength;
            else if (Type == typeof(Guid)) return DbType.Guid;
            else if (Type == typeof(DateTime)) return DbType.DateTime2;
            else if (Type == typeof(DateTimeOffset)) return DbType.DateTimeOffset;
            else if (Type == typeof(byte[])) return DbType.Binary;
            else if (Type == typeof(byte?)) return DbType.Byte;
            else if (Type == typeof(sbyte?)) return DbType.SByte;
            else if (Type == typeof(short?)) return DbType.Int16;
            else if (Type == typeof(ushort?)) return DbType.UInt16;
            else if (Type == typeof(int?)) return DbType.Int32;
            else if (Type == typeof(uint?)) return DbType.UInt32;
            else if (Type == typeof(long?)) return DbType.Int64;
            else if (Type == typeof(ulong?)) return DbType.UInt64;
            else if (Type == typeof(float?)) return DbType.Single;
            else if (Type == typeof(double?)) return DbType.Double;
            else if (Type == typeof(decimal?)) return DbType.Decimal;
            else if (Type == typeof(bool?)) return DbType.Boolean;
            else if (Type == typeof(char?)) return DbType.StringFixedLength;
            else if (Type == typeof(Guid?)) return DbType.Guid;
            else if (Type == typeof(DateTime?)) return DbType.DateTime2;
            else if (Type == typeof(DateTimeOffset?)) return DbType.DateTimeOffset;
            return DbType.Int32;
        }
#endif
        /// <summary>
        /// DbType 转 SqlDbType
        /// </summary>
        /// <param name="dbType">Type扩展</param>
        /// <returns></returns>
        public static SqlDbType ToSqlDBType(this DbType dbType) {
            switch (dbType) {
                case DbType.AnsiString: return SqlDbType.VarChar;
                case DbType.Binary: return SqlDbType.VarBinary;
                case DbType.Byte: return SqlDbType.TinyInt;
                case DbType.Boolean: return SqlDbType.Bit;
                case DbType.Currency: return SqlDbType.Money;
                case DbType.Date: return SqlDbType.DateTime;
                case DbType.DateTime: return SqlDbType.DateTime;
                case DbType.Decimal: return SqlDbType.Decimal;
                case DbType.Double: return SqlDbType.Float;
                case DbType.Guid: return SqlDbType.UniqueIdentifier;
                case DbType.Int16: return SqlDbType.Int;
                case DbType.Int32: return SqlDbType.Int;
                case DbType.Int64: return SqlDbType.BigInt;
                case DbType.Object: return SqlDbType.Variant;
                case DbType.SByte: return SqlDbType.TinyInt;
                case DbType.Single: return SqlDbType.Real;
                case DbType.String: return SqlDbType.NVarChar;
                case DbType.Time: return SqlDbType.DateTime;
                case DbType.UInt16: return SqlDbType.Int;
                case DbType.UInt32: return SqlDbType.Int;
                case DbType.UInt64: return SqlDbType.BigInt;
                case DbType.VarNumeric: return SqlDbType.Decimal;
                case DbType.AnsiStringFixedLength: return SqlDbType.Char;
                case DbType.StringFixedLength: return SqlDbType.NChar;
            }
            return SqlDbType.VarChar;
        }
        /// <summary>
        /// DbType 转 SQLiteType
        /// </summary>
        /// <param name="dbType">Type扩展</param>
        /// <returns></returns>
        public static string ToSQLiteType(this DbType dbType) {
            switch (dbType) {
                case DbType.AnsiString:
                case DbType.Object:
                case DbType.String:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength: return "nvarchar";
                case DbType.Binary:
                case DbType.Byte:
                case DbType.SByte: return "longblob";
                case DbType.Boolean: return "tinyint";
                case DbType.Currency: return "money";
                case DbType.Date:
                case DbType.DateTime:
                case DbType.Time: return "datetime";
                case DbType.Decimal: return "decimal";
                case DbType.Double: return "float";
                case DbType.Guid: return "guid";
                case DbType.Int16:
                case DbType.UInt16: return "tinyint";
                case DbType.Int32:
                case DbType.UInt32: return "int";
                case DbType.Int64:
                case DbType.UInt64: return "integer";
                case DbType.Single: return "real";
                case DbType.VarNumeric: return "numeric";
                case DbType.Xml: return "xml";
            }
            return "nvarchar";
        }
        /// <summary>
        /// DbType 转 Sql2005Type
        /// </summary>
        /// <param name="dbType">Type扩展</param>
        /// <returns></returns>
        public static string ToSql2005Type(this DbType dbType) {
            switch (dbType) {
                case DbType.AnsiString:
                case DbType.Object:
                case DbType.String:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength: return "nvarchar";
                case DbType.Binary:
                case DbType.Byte:
                case DbType.SByte: return "image";
                case DbType.Boolean: return "bit";
                case DbType.Currency: return "money";
                case DbType.Date:
                case DbType.DateTime:
                case DbType.Time: return "datetime";
                case DbType.Decimal: return "decimal";
                case DbType.Double: return "float";
                case DbType.Guid: return "uniqueidentifier";
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64: return "int";
                case DbType.Single: return "real";
                case DbType.VarNumeric: return "numeric";
                case DbType.Xml: return "xml";
            }
            return "nvarchar";
        }
        /// <summary>
        /// DbType 转 MySql
        /// </summary>
        /// <param name="dbType">Type扩展</param>
        /// <returns></returns>
        public static string ToMySqlType(this DbType dbType) {
            switch (dbType) {
                case DbType.AnsiString:
                case DbType.Object:
                case DbType.String:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength: return "nvarchar";
                case DbType.Binary:
                case DbType.Byte:
                case DbType.SByte: return "longblob";
                case DbType.Boolean: return "tinyint";
                case DbType.Currency: return "money";
                case DbType.Date:
                case DbType.DateTime:
                case DbType.Time: return "datetime";
                case DbType.Decimal: return "decimal";
                case DbType.Double: return "float";
                case DbType.Guid: return "binary";
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64: return "int";
                case DbType.Single: return "real";
                case DbType.VarNumeric: return "numeric";
                case DbType.Xml: return "xml";
            }
            return "nvarchar";
        }
        /// <summary>
        /// FindMembers
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>IEnumerable MemberInfo</returns>
        public static IEnumerable<MemberInfo> FindMembers(this Type type) {
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
                var getMethod = prop.GetGetMethod(true);
                var setMethod = prop.GetSetMethod(true);
                if (getMethod.IsNull() || getMethod.IsPrivate || setMethod.IsNull()) continue;
                if (setMethod.GetParameters().Length != 1) continue;//an indexer
                yield return prop;
            }
            foreach (var field in type.GetFields()) { //all public fields
                if (!field.IsInitOnly && !field.IsLiteral) yield return field; //readonly
            }
        }
        /// <summary>
        /// FindMember
        /// </summary>
        /// <param name="classType">Type</param>
        /// <returns>MemberInfo</returns>
        public static MemberInfo FindMember(this Type classType) {
            return classType.FindMembers().ToList()[0];
        }
        /// <summary>
        /// DbType to Type
        /// </summary>
        /// <param name="Type">DbType</param>
        /// <returns>Type</returns>
        public static Type ToType(this DbType Type) {
            if (Type == DbType.Byte) return typeof(byte);
            else if (Type == DbType.SByte) return typeof(sbyte);
            else if (Type == DbType.Int16) return typeof(short);
            else if (Type == DbType.UInt16) return typeof(ushort);
            else if (Type == DbType.Int32) return typeof(int);
            else if (Type == DbType.UInt32) return typeof(uint);
            else if (Type == DbType.Int64) return typeof(long);
            else if (Type == DbType.UInt64) return typeof(ulong);
            else if (Type == DbType.Single) return typeof(float);
            else if (Type == DbType.Double) return typeof(double);
            else if (Type == DbType.Decimal) return typeof(decimal);
            else if (Type == DbType.Boolean) return typeof(bool);
            else if (Type == DbType.String) return typeof(string);
            else if (Type == DbType.StringFixedLength) return typeof(char);
            else if (Type == DbType.Guid) return typeof(Guid);
            else if (Type == DbType.DateTime2) return typeof(DateTime);
            else if (Type == DbType.DateTime) return typeof(DateTime);
            else if (Type == DbType.DateTimeOffset) return typeof(DateTimeOffset);
            else if (Type == DbType.Binary) return typeof(byte[]);
            return typeof(int);
        }
    }
}
