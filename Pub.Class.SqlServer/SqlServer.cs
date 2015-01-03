//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Pub.Class {
    /// <summary>
    /// SqlServer数据库实例
    /// 
    /// 修改纪录
    ///     2009.11.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class SqlServer : IDbProvider {
        /// <summary>
        /// SqlClient实例
        /// </summary>
        /// <returns>SqlClient实例</returns>
        public DbProviderFactory Instance() { return SqlClientFactory.Instance; }
        /// <summary>
        /// 返回刚插入记录的自增ID值
        /// </summary>
        /// <returns>SQL</returns>
        public string GetLastIDSQL() { return "SELECT SCOPE_IDENTITY()"; }
        /// <summary>
        /// 是否支持全文搜索
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsFullTextSearchEnabled() { return true; }
        /// <summary>
        /// 是否支持压缩数据库
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsCompactDatabase() { return true; }
        /// <summary>
        /// 是否支持备份数据库
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsBackupDatabase() { return true; }
        /// <summary>
        /// 是否支持数据库优化
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsDbOptimize() { return false; }
        /// <summary>
        /// 是否支持数据库收缩
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsShrinkData() { return true; }
        /// <summary>
        /// 是否支持存储过程
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsStoreProc() { return true; }
        /// <summary>
        /// 检索SQL参数信息并填充
        /// </summary>
        /// <param name="cmd"></param>
        public void DeriveParameters(IDbCommand cmd) {
            if ((cmd as SqlCommand) != null) SqlCommandBuilder.DeriveParameters(cmd as SqlCommand);
        }
        /// <summary>
        /// 创建SQL参数
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public DbParameter MakeParam(string ParamName, DbType DbType, Int32 Size) {
            if (Size > 0) return new SqlParameter(ParamName, (SqlDbType)DbType, Size);
            return new SqlParameter(ParamName, (SqlDbType)DbType);
        }
        public DbParameter MakeParam(string ParamName, object value) {
            return new SqlParameter(ParamName, value);
        }
        /// <summary>
        /// 起始字符
        /// </summary>
        public string GetIdentifierStart() { return "["; }
        /// <summary>
        /// 结束字符
        /// </summary>
        public string GetIdentifierEnd() { return "]"; }
        /// <summary>
        /// 参数前导符号
        /// </summary>
        public string GetParamIdentifier() { return "@"; }
        /// <summary>
        /// SqlServer大数据复制
        /// </summary>
        /// <param name="dt">数据源 dt.TableName一定要和数据库表名对应</param>
        /// <param name="dbkey">数据库</param>
        /// <param name="options">选项 默认Default</param>
        /// <param name="isTran">是否使用事务 默认false</param>
        /// <param name="timeout">超时时间7200 2小时</param>
        /// <param name="batchSize">每一批次中的行数</param>
        /// <param name="error">错误处理</param>
        /// <returns>true/false</returns>
        public bool DataBulkCopy(DataTable dt, string dbkey = "", BulkCopyOptions options = BulkCopyOptions.Default, bool isTran = false, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null) {
            if (Data.Pool(dbkey).DBType != "SqlServer") return false;
            SqlTransaction tran = null;
            using(SqlConnection conn = new SqlConnection(Data.Pool(dbkey).ConnString)) {
                conn.Open();
                if (isTran) tran = conn.BeginTransaction();
                using (System.Data.SqlClient.SqlBulkCopy bc = new System.Data.SqlClient.SqlBulkCopy(conn, ((int)options).ToEnum<SqlBulkCopyOptions>(), tran)) {
                    bc.BulkCopyTimeout = timeout;
                    bc.BatchSize = batchSize;
                    bc.DestinationTableName = dt.TableName;
                    try {
                        bc.WriteToServer(dt);
                        if (isTran) tran.Commit();
                    } catch(Exception ex) {
                        if (isTran) tran.Rollback();
                        if (error.IsNotNull()) error(ex);
                        return false;
                    }
                }
                conn.Close();
            }
            return true;
        }
        /// <summary>
        /// 大数据复制
        /// </summary>
        /// <param name="conn">连接源</param>
        /// <param name="tran">事务</param>
        /// <param name="dt">数据源 dt.TableName一定要和数据库表名对应</param>
        /// <param name="options">选项 默认Default</param>
        /// <param name="timeout">超时时间7200 2小时</param>
        /// <param name="batchSize">每一批次中的行数</param>
        /// <param name="error">错误处理</param>
        /// <returns></returns>
        public bool DataBulkCopy(IDbConnection conn, IDbTransaction tran, DataTable dt, BulkCopyOptions options = BulkCopyOptions.Default, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null) {
            using (System.Data.SqlClient.SqlBulkCopy bc = new System.Data.SqlClient.SqlBulkCopy((SqlConnection)conn, ((int)options).ToEnum<SqlBulkCopyOptions>(), (SqlTransaction)tran)) {
                bc.BulkCopyTimeout = timeout;
                bc.BatchSize = batchSize;
                bc.DestinationTableName = dt.TableName;
                try {
                    bc.WriteToServer(dt);
                } catch(Exception ex) {
                    if (error.IsNotNull()) error(ex);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// SqlServer大数据复制
        /// </summary>
        /// <param name="dr">数据源</param>
        /// <param name="tableName">对应的表名</param>
        /// <param name="dbkey">数据库</param>
        /// <param name="options">选项 默认Default</param>
        /// <param name="isTran">是否使用事务 默认false</param>
        /// <param name="timeout">超时时间7200 2小时</param>
        /// <param name="batchSize">每一批次中的行数</param>
        /// <param name="error">错误处理</param>
        /// <returns>true/false</returns>
        public bool DataBulkCopy(IDataReader dr, string tableName, string dbkey = "", BulkCopyOptions options = BulkCopyOptions.Default, bool isTran = false, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null) {
            if (Data.Pool(dbkey).DBType != "SqlServer") return false;
            SqlTransaction tran = null;
            using(SqlConnection conn = new SqlConnection(Data.Pool(dbkey).ConnString)) {
                if (isTran) tran = conn.BeginTransaction();
                using (System.Data.SqlClient.SqlBulkCopy bc = new System.Data.SqlClient.SqlBulkCopy(conn, ((int)options).ToEnum<SqlBulkCopyOptions>(), tran)) {
                    bc.BulkCopyTimeout = timeout;
                    bc.BatchSize = batchSize;
                    bc.DestinationTableName = tableName;
                    try {
                        bc.WriteToServer(dr);
                        if (isTran) tran.Commit();
                    } catch(Exception ex) {
                        if (isTran) tran.Rollback();
                        if (error.IsNotNull()) error(ex);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
