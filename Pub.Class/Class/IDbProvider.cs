//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;

namespace Pub.Class {
    /// <summary>
    /// 数据库操作接口
    /// 
    /// 修改纪录
    ///     2011.12.18 版本：1.1 livexy 添加GetIdentifierStart/GetIdentifierEnd/GetParamIdentifier
    ///     2006.05.04 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface IDbProvider: IAddIn {
        /// <summary>
        /// 返回DbProviderFactory实例
        /// </summary>
        /// <returns></returns>
        DbProviderFactory Instance();
        /// <summary>
        /// 返回刚插入记录的自增ID值, 如不支持则为""
        /// </summary>
        /// <returns></returns>
        string GetLastIDSQL();
        /// <summary>
        /// 是否支持全文搜索
        /// </summary>
        /// <returns></returns>
        bool IsFullTextSearchEnabled();
        /// <summary>
        /// 是否支持压缩数据库
        /// </summary>
        /// <returns></returns>
        bool IsCompactDatabase();
        /// <summary>
        /// 是否支持备份数据库
        /// </summary>
        /// <returns></returns>
        bool IsBackupDatabase();
        /// <summary>
        /// 是否支持数据库优化
        /// </summary>
        /// <returns></returns>
        bool IsDbOptimize();
        /// <summary>
        /// 是否支持数据库收缩
        /// </summary>
        /// <returns></returns>
        bool IsShrinkData();
        /// <summary>
        /// 是否支持存储过程
        /// </summary>
        /// <returns></returns>
        bool IsStoreProc();
        /// <summary>
        /// 检索SQL参数信息并填充
        /// </summary>
        /// <param name="cmd"></param>
        void DeriveParameters(IDbCommand cmd);
        /// <summary>
        /// 创建SQL参数
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        DbParameter MakeParam(string ParamName, DbType DbType, Int32 Size);
        DbParameter MakeParam(string ParamName, object value);
        /// <summary>
        /// 起始字符
        /// </summary>
        string GetIdentifierStart();
        /// <summary>
        /// 结束字符
        /// </summary>
        string GetIdentifierEnd();
        /// <summary>
        /// 参数前导符号
        /// </summary>
        string GetParamIdentifier();
        /// <summary>
        /// 大数据复制
        /// </summary>
        /// <param name="dt">数据源 dt.TableName一定要和数据库表名对应</param>
        /// <param name="dbkey">数据库</param>
        /// <param name="options">选项 默认Default</param>
        /// <param name="isTran">是否使用事务 默认false</param>
        /// <param name="timeout">超时时间7200 2小时</param>
        /// <param name="batchSize">每一批次中的行数</param>
        /// <param name="error">错误处理</param>
        /// <returns>true/false</returns>
        bool DataBulkCopy(DataTable dt, string dbkey = "", BulkCopyOptions options = BulkCopyOptions.Default, bool isTran = false, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null);
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
        bool DataBulkCopy(IDbConnection conn, IDbTransaction tran, DataTable dt, BulkCopyOptions options = BulkCopyOptions.Default, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null);
        /// <summary>
        /// 大数据复制
        /// </summary>
        /// <param name="dr">数据源</param>
        /// <param name="tableName">一定要和数据库表名对应</param>
        /// <param name="dbkey">数据库</param>
        /// <param name="options">选项 默认Default</param>
        /// <param name="isTran">是否使用事务 默认false</param>
        /// <param name="timeout">超时时间7200 2小时</param>
        /// <param name="batchSize">每一批次中的行数</param>
        /// <param name="error">错误处理</param>
        /// <returns>true/false</returns>
        bool DataBulkCopy(IDataReader dr, string tableName, string dbkey = "", BulkCopyOptions options = BulkCopyOptions.Default, bool isTran = false, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null);
    }
}
