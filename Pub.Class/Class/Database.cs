//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Xml;
using System.Data.Common;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Web;

namespace Pub.Class {
    /// <summary>
    /// 数据库访问类
    /// 
    /// 修改纪录
    ///     2010.08.01 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    /// <example>
    /// <code>
    /// 事务操作：
    /// Database db = new Database("dbkey");
    /// SqlConnection conn = new SqlConnection(db.ConnectionString);
    /// conn.Open();
    /// using (SqlTransaction trans = conn.BeginTransaction()) {
    ///     try {
    ///         db.ExecuteNonQuery(trans,);
    ///         db.ExecuteNonQuery(trans,);
    ///         trans.Commit();
    ///     } catch (Exception ex) {
    ///         trans.Rollback();
    ///         throw ex;
    ///     }
    /// }
    /// conn.Close();
    /// </code>
    /// </example>
    public partial class Database: IDisposable {
        #region 构造器
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="pool">连接名称</param>
        public Database(string pool) { key = pool; }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="dbType">数据库类型 SqlServer、Access、MySql、OleDb、Odbc、Oracle、SQLite</param>
        /// <param name="connString">连接字符串</param>
        /// <param name="pool">连接名称</param>
        public Database(string dbType, string connString, string pool = "") {
            this.connString = connString;
            this.dbType = dbType;
            this.key = pool;
        }
        void IDisposable.Dispose() {
            ResetDbProvider();
        }
        #endregion

        #region 私有变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string connString = null;
        /// <summary>
        /// 连接池
        /// </summary>
        protected string key = "ConnString";
        /// <summary>
        /// 数据库类型
        /// </summary>
        protected string dbType = null;

        /// <summary>
        /// DbProviderFactory实例
        /// </summary>
        private DbProviderFactory factory = null;

        /// <summary>
        /// 数据接口
        /// </summary>
        private IDbProvider provider = null;
        private int timeout = 30;
        /// <summary>
        /// 查询次数统计
        /// </summary>
        private Int64 queryCount = 0;
        /// <summary>
        /// Parameters缓存哈希表
        /// </summary>
        private Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
        private readonly object lockHelper = new object();
        #endregion

        #region 属性
        /// <summary>
        /// 查询次数统计
        /// </summary>
        public Int64 QueryCount { get { return queryCount; } set { queryCount = value; } }
        public int Timeout { get { return timeout; } set { timeout = value; } }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnString {
            get {
#if !MONO40
                if (string.IsNullOrEmpty(connString)) {
                    if (ConfigurationManager.ConnectionStrings[key].IsNotNull()) {
                        connString = ConfigurationManager.ConnectionStrings[key].ToString();
                        dbType = ConfigurationManager.ConnectionStrings[key].ProviderName;
                    }
                }
#endif
                return connString;
            }
            set { connString = value; }
        }
        /// <summary>
        /// 连接池
        /// </summary>
        public string Pool {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DBType {
            get {
#if !MONO40
                if (string.IsNullOrEmpty(dbType)) {
                    if (ConfigurationManager.ConnectionStrings[key].IsNotNull()) dbType = ConfigurationManager.ConnectionStrings[key].ProviderName; else dbType = "SqlServer";
                }
#endif
                return dbType;
            }
            set { dbType = value; }
        }

        /// <summary>
        /// IDbProvider接口
        /// </summary>
        public IDbProvider Provider {
            get {
                if (provider.IsNull()) {
                    lock (lockHelper) {
                        if (provider.IsNull()) {
                            //System.Web.HttpContext.Current.Response.Write(dbType);
                            //System.Web.HttpContext.Current.Response.End();
                            dbType = DBType;
                            try {
                                provider = (IDbProvider)"Pub.Class.{0},Pub.Class.{0}".FormatWith(dbType).LoadClass();
                                //string _path = (HttpContext.Current.IsNotNull() ? "~/bin/".GetMapPath() : "".GetMapPath()) + "Pub.Class.{0}.dll".FormatWith(dbType);
                                //provider = (IDbProvider)Activator.CreateInstance(Assembly.LoadFrom(_path).GetType("Pub.Class.{0}".FormatWith(dbType), true, true));
                            } catch {
                                throw new Exception(dbType + " - 请检查web.config中DbType节点数据库类型是否正确，例如：SqlServer、Access、MySql、OleDb、Odbc");
                            }
                        }
                    }
                }
                return provider;
            }
        }

        /// <summary>
        /// DbFactory实例
        /// </summary>
        public DbProviderFactory Factory {
            get {
                if (factory.IsNull()) factory = Provider.Instance();
                return factory;
            }
        }

        /// <summary>
        /// 刷新数据库提供者
        /// </summary>
        public void ResetDbProvider() {
            connString = null;
            factory = null;
            provider = null;
            dbType = null;
            key = null;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 将DbParameter参数数组(参数值)分配给DbCommand命令.
        /// 这个方法将给任何一个参数分配DBNull.Value;
        /// 该操作将阻止默认值的使用.
        /// </summary>
        /// <param name="command">命令名</param>
        /// <param name="commandParameters">DbParameters数组</param>
        private void AttachParameters(DbCommand command, DbParameter[] commandParameters) {
            if (command.IsNull()) throw new ArgumentNullException("command");
            if (commandParameters.IsNotNull()) {
                foreach (DbParameter p in commandParameters) {
                    if (p.IsNotNull()) {
                        // 检查未分配值的输出参数,将其分配以DBNull.Value.
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && (p.Value.IsNull())) p.Value = DBNull.Value;
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// 将DataRow类型的列值分配到DbParameter参数数组.
        /// </summary>
        /// <param name="commandParameters">要分配值的DbParameter参数数组</param>
        /// <param name="dataRow">将要分配给存储过程参数的DataRow</param>
        private void AssignParameterValues(DbParameter[] commandParameters, DataRow dataRow) {
            if ((commandParameters.IsNull()) || (dataRow.IsNull())) return;

            int i = 0;
            // 设置参数值
            foreach (DbParameter commandParameter in commandParameters) {
                // 创建参数名称,如果不存在,只抛出一个异常.
                if (commandParameter.ParameterName.IsNull() || commandParameter.ParameterName.Length <= 1) throw new Exception(string.Format("请提供参数{0}一个有效的名称{1}.", i, commandParameter.ParameterName));
                // 从dataRow的表中获取为参数数组中数组名称的列的索引.
                // 如果存在和参数名称相同的列,则将列值赋给当前名称的参数.
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1) commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }

        /// <summary>
        /// 将一个对象数组分配给DbParameter参数数组.
        /// </summary>
        /// <param name="commandParameters">要分配值的DbParameter参数数组</param>
        /// <param name="parameterValues">将要分配给存储过程参数的对象数组</param>
        private void AssignParameterValues(DbParameter[] commandParameters, object[] parameterValues) {
            if ((commandParameters.IsNull()) || (parameterValues.IsNull())) return;

            // 确保对象数组个数与参数个数匹配,如果不匹配,抛出一个异常.
            if (commandParameters.Length != parameterValues.Length) throw new ArgumentException("参数值个数与参数不匹配.");

            // 给参数赋值
            for (int i = 0, j = commandParameters.Length; i < j; i++) {
                // If the current array value derives from IDbDataParameter, then assign its Value property
                if (parameterValues[i] is IDbDataParameter) {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value.IsNull()) commandParameters[i].Value = DBNull.Value; else commandParameters[i].Value = paramInstance.Value;
                } else if (parameterValues[i].IsNull()) commandParameters[i].Value = DBNull.Value;
                else commandParameters[i].Value = parameterValues[i];
            }
        }

        /// <summary>
        /// 预处理用户提供的命令,数据库连接/事务/命令类型/参数
        /// </summary>
        /// <param name="command">要处理的DbCommand</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">一个有效的事务或者是null值</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名或都SQL命令文本</param>
        /// <param name="commandParameters">和命令相关联的DbParameter参数数组,如果没有参数为'null'</param>
        /// <param name="mustCloseConnection"><c>true</c> 如果连接是打开的,则为true,其它情况下为false.</param>
        private void PrepareCommand(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, out bool mustCloseConnection) {
            if (command.IsNull()) throw new ArgumentNullException("command");
            if (commandText.IsNull() || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open) {
                mustCloseConnection = true;
                connection.Open();
            } else mustCloseConnection = false;

            // 给命令分配一个数据库连接.
            command.Connection = connection;

            // 设置命令文本(存储过程名或SQL语句)
            command.CommandText = commandText;
            command.CommandTimeout = timeout;

            // 分配事务
            if (transaction.IsNotNull()) {
                if (transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // 设置命令类型.
            command.CommandType = commandType;

            // 分配命令参数
            if (commandParameters.IsNotNull()) AttachParameters(command, commandParameters);
            return;
        }

        /// <summary>
        /// 探索运行时的存储过程,返回DbParameter参数数组.
        /// 初始化参数值为 DBNull.Value.
        /// </summary>
        /// <param name="connection">一个有效的数据库连接</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>返回DbParameter参数数组</returns>
        private DbParameter[] DiscoverSpParameterSet(DbConnection connection, string spName, bool includeReturnValueParameter) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            // 检索cmd指定的存储过程的参数信息,并填充到cmd的Parameters参数集中.
            Provider.DeriveParameters(cmd);
            connection.Close();
            // 如果不包含返回值参数,将参数集中的每一个参数删除.
            if (!includeReturnValueParameter) cmd.Parameters.RemoveAt(0);

            // 创建参数数组
            DbParameter[] discoveredParameters = new DbParameter[cmd.Parameters.Count];
            // 将cmd的Parameters参数集复制到discoveredParameters数组.
            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // 初始化参数值为 DBNull.Value.
            foreach (DbParameter discoveredParameter in discoveredParameters) discoveredParameter.Value = DBNull.Value;
            return discoveredParameters;
        }

        /// <summary>
        /// DbParameter参数数组的深层拷贝.
        /// </summary>
        /// <param name="originalParameters">原始参数数组</param>
        /// <returns>返回一个同样的参数数组</returns>
        private DbParameter[] CloneParameters(DbParameter[] originalParameters) {
            DbParameter[] clonedParameters = new DbParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++) clonedParameters[i] = (DbParameter)((ICloneable)originalParameters[i]).Clone();

            return clonedParameters;
        }
        #endregion 私有方法结束

        #region ExecSql方法
        /// <summary>
        /// 执行指定连接字符串,类型的DbCommand.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql("SELECT * FROM [tableName]");
        /// </remarks>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecSql(string commandText) {
            return ExecSql(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定连接字符串,类型的DbCommand.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql("PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecSql(string commandText, params DbParameter[] commandParameters) {
            return ExecSql(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, commandParameters);
        }

        /// <summary>
        /// 执行指定连接字符串,类型的DbCommand.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(out id,"SELECT * FROM [tableName]");
        /// </remarks>
        /// <param name="id">返回自增ID</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecSql(out int id, string commandText) {
            return ExecSql(out id, commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定连接字符串,类型的DbCommand.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecSql(CommandType commandType, string commandText) {
            return ExecSql(commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定连接字符串,类型的DbCommand.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecSql(CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);

            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                return ExecSql(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 执行指定连接字符串,并返回刚插入的自增ID
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(out id, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="id">返回自增ID</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecSql(out int id, CommandType commandType, string commandText) {
            return ExecSql(out id, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定连接字符串并返回刚插入的自增ID,类型的DbCommand.
        /// </summary>
        /// <param name="id">返回自增ID</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecSql(out int id, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);

            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                return ExecSql(out id, connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令 
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(DbConnection connection, CommandType commandType, string commandText) {
            return ExecSql(connection, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(conn, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">T存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");

            // 创建DbCommand命令,并进行预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (DbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行DbCommand命令,并返回结果.
            int retval = cmd.ExecuteNonQuery();

            // 清除参数,以便再次使用.
            cmd.Parameters.Clear();
            if (mustCloseConnection) connection.Close();
            return retval;
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令并返回自增ID 
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="id">id</param>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(out int id, DbConnection connection, CommandType commandType, string commandText) {
            return ExecSql(out id, connection, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(conn, CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="id">id</param>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">T存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(out int id, DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (Provider.GetLastIDSQL().Trim() == "") throw new ArgumentNullException("GetLastIDSQL is \"\"");

            // 创建DbCommand命令,并进行预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (DbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行命令
            int retval = cmd.ExecuteNonQuery();
            // 清除参数,以便再次使用.
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Provider.GetLastIDSQL();

            int.TryParse(cmd.ExecuteScalar().ToString(), out id);
            queryCount++;

            if (mustCloseConnection) connection.Close();
            return retval;
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,将对象数组的值赋给存储过程参数.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值
        /// 示例:  
        ///  int result = ExecSql(conn, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(DbConnection connection, string spName, params object[] parameterValues) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值
            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中加载存储过程参数
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 给存储过程分配参数值
                AssignParameterValues(commandParameters, parameterValues);

                return ExecSql(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else return ExecSql(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,将对象数组的值赋给存储过程参数.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值
        /// 示例:  
        ///  int result = ExecSql("PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(string spName, params object[] parameterValues) {
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值
            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中加载存储过程参数
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                // 给存储过程分配参数值
                AssignParameterValues(commandParameters, parameterValues);

                return ExecSql(CommandType.StoredProcedure, spName, commandParameters);
            } else return ExecSql(CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行带事务的DbCommand.
        /// </summary>
        /// <remarks>
        /// 示例.:  
        ///  int result = ExecSql(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(DbTransaction transaction, CommandType commandType, string commandText) {
            return ExecSql(transaction, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行带事务的DbCommand(指定参数).
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(trans, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行DbCommand命令,并返回结果.
            int retval = cmd.ExecuteNonQuery();

            // 清除参数集,以便再次使用.
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// 执行带事务的DbCommand.
        /// </summary>
        /// <remarks>
        /// 示例.:  
        ///  int result = ExecSql(out id,trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="id">返回自增ID</param>
        /// <param name="transaction">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(out int id, DbTransaction transaction, CommandType commandType, string commandText) {
            return ExecSql(out id, transaction, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行带事务的DbCommand(指定参数).
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int result = ExecSql(out id,trans, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="id">返回自增ID</param>
        /// <param name="transaction">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSql(out int id, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行
            int retval = cmd.ExecuteNonQuery();
            // 清除参数,以便再次使用.
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Provider.GetLastIDSQL();
            int.TryParse(cmd.ExecuteScalar().ToString(), out id);
            return retval;
        }

        /// <summary>
        /// 执行带事务的DbCommand(指定参数值).
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值
        /// 示例:  
        ///  int result = ExecSql(conn, trans, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecSql(DbTransaction transaction, string spName, params object[] parameterValues) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值
            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数赋值
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法
                return ExecSql(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                // 没有参数值
                return ExecSql(transaction, CommandType.StoredProcedure, spName);
            }
        }
        /// <summary>
        /// 使用一个数据库链接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exec"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public T Connection<T>(Func<DbConnection, Database, T> exec, Action<Exception> error = null) {
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();
                try {
                    T res = exec(connection, this);
                    return res;
                } catch (Exception ex) {
                    if (!error.IsNull()) error(ex);
                }
            }
            return default(T);
        }
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exec"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public T ExecTran<T>(Func<DbTransaction, T> exec, Action<Exception> error = null) {
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();
                using (DbTransaction trans = connection.BeginTransaction()) {
                    try {
                        T res = exec(trans);
                        trans.Commit();
                        return res;
                    } catch (Exception ex) {
                        trans.Rollback();
                        if (!error.IsNull()) error(ex);
                    }
                }
            }
            return default(T);
        }
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exec"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public T ExecTran<T>(Func<Database, DbTransaction, T> exec, Action<Exception> error = null) {
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();
                using (DbTransaction trans = connection.BeginTransaction()) {
                    try {
                        T res = exec(this, trans);
                        trans.Commit();
                        return res;
                    } catch (Exception ex) {
                        trans.Rollback();
                        if (!error.IsNull()) error(ex);
                    }
                }
            }
            return default(T);
        }
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exec"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public T ExecTran<T>(Func<DbConnection, Database, DbTransaction, T> exec, Action<Exception> error = null) {
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();
                using (DbTransaction trans = connection.BeginTransaction()) {
                    try {
                        T res = exec(connection, this, trans);
                        trans.Commit();
                        return res;
                    } catch (Exception ex) {
                        trans.Rollback();
                        if (!error.IsNull()) error(ex);
                    }
                }
            }
            return default(T);
        }
        #endregion ExecSql方法结束

        #region ExecuteCommandWithSplitter方法
        /// <summary>
        /// 运行含有GO命令的多条SQL命令
        /// </summary>
        /// <param name="commandText">SQL命令字符串</param>
        /// <param name="splitter">分割字符串</param>
        public void ExecuteCommandWithSplitter(string commandText, string splitter) {
            int startPos = 0;

            do {
                int lastPos = commandText.IndexOf(splitter, startPos);
                int len = (lastPos > startPos ? lastPos : commandText.Length) - startPos;
                string query = commandText.Substring(startPos, len);

                if (query.Trim().Length > 0) {
                    try { ExecSql(CommandType.Text, query); } catch { ;}
                }

                if (lastPos == -1)
                    break;
                else
                    startPos = lastPos + splitter.Length;
            } while (startPos < commandText.Length);

        }

        /// <summary>
        /// 运行含有GO命令的多条SQL命令
        /// </summary>
        /// <param name="commandText">SQL命令字符串</param>
        public void ExecuteCommandWithSplitter(string commandText) {
            ExecuteCommandWithSplitter(commandText, "\r\nGO\r\n");
        }
        #endregion ExecuteCommandWithSplitter方法结束

        #region GetDataSet方法
        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataSet ds = GetDataSet("SELECT * FROM [table1]");
        /// </remarks>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(string commandText) {
            return GetDataSet(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataSet ds = GetDataSet("SELECT * FROM [table1]", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(string commandText, params DbParameter[] commandParameters) {
            return GetDataSet(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, commandParameters);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataSet ds = GetDataSet(CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(CommandType commandType, string commandText) {
            return GetDataSet(commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例: 
        ///  DataSet ds = GetDataSet(CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);

            // 创建并打开数据库连接对象,操作完成释放对象.
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                // 调用指定数据库连接字符串重载方法.
                return GetDataSet(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,直接提供参数值,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值.
        /// 示例: 
        ///  DataSet ds = GetDataSet("GetOrders", 24, 36);
        /// </remarks>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(string spName, params object[] parameterValues) {
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中检索存储过程参数
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                // 给存储过程参数分配值
                AssignParameterValues(commandParameters, parameterValues);

                return GetDataSet(CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataSet(CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataSet ds = GetDataSet(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(DbConnection connection, CommandType commandType, string commandText) {
            return GetDataSet(connection, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataSet ds = GetDataSet(conn, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");

            // 预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (DbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建DbDataAdapter和DataSet.
            using (DbDataAdapter da = Factory.CreateDataAdapter()) {
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                // 填充DataSet.
                da.Fill(ds);
                queryCount++;

                cmd.Parameters.Clear();

                if (mustCloseConnection) connection.Close();
                return ds;
            }
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,指定参数值,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输入参数和返回值.
        /// 示例.:  
        ///  DataSet ds = GetDataSet(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(DbConnection connection, string spName, params object[] parameterValues) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 比缓存中加载存储过程参数
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 给存储过程参数分配值
                AssignParameterValues(commandParameters, parameterValues);

                return GetDataSet(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataSet(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定事务的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataSet ds = GetDataSet(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(DbTransaction transaction, CommandType commandType, string commandText) {
            return GetDataSet(transaction, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定事务的命令,指定参数,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataSet ds = GetDataSet(trans, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建 DataAdapter & DataSet
            using (DbDataAdapter da = Factory.CreateDataAdapter()) {
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        /// <summary>
        /// 执行指定事务的命令,指定参数值,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输入参数和返回值.
        /// 示例.:  
        ///  DataSet ds = GetDataSet(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">事务</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet GetDataSet(DbTransaction transaction, string spName, params object[] parameterValues) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中加载存储过程参数
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数分配值
                AssignParameterValues(commandParameters, parameterValues);

                return GetDataSet(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else return GetDataSet(transaction, CommandType.StoredProcedure, spName);
        }
        #endregion GetDataSet数据集命令结束

        #region GetDataTable方法

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataTable dt = GetDataTable("SELECT * FROM [table1]");
        /// </remarks>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(string commandText) {
            return GetDataTable(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataTable dt = GetDataTable("SELECT * FROM [table1]", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(string commandText, params DbParameter[] commandParameters) {
            return GetDataTable(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, commandParameters);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataTable dt = GetDataTable(CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(CommandType commandType, string commandText) {
            return GetDataTable(commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例: 
        ///  DataTable dt = GetDataTable(CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);

            // 创建并打开数据库连接对象,操作完成释放对象.

            //using (DbConnection connection = (DbConnection)new System.Data.SqlClient.SqlConnection(ConnString))
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                // 调用指定数据库连接字符串重载方法.
                return GetDataTable(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,直接提供参数值,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值.
        /// 示例: 
        ///  DataTable dt = GetDataSet("GetOrders", 24, 36);
        /// </remarks>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(string spName, params object[] parameterValues) {
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中检索存储过程参数
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                // 给存储过程参数分配值
                AssignParameterValues(commandParameters, parameterValues);

                return GetDataTable(CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataTable(CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataTable dt = GetDataSet(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(DbConnection connection, CommandType commandType, string commandText) {
            return GetDataTable(connection, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataTable dt = GetDataSet(conn, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");

            // 预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (DbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建DbDataAdapter和DataSet.
            using (DbDataAdapter da = Factory.CreateDataAdapter()) {
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                dt.TableName = "NewTableName";
                // 填充DataSet.
                da.Fill(dt);
                queryCount++;

                cmd.Parameters.Clear();

                if (mustCloseConnection) connection.Close();

                return dt;
            }
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,指定参数值,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输入参数和返回值.
        /// 示例.:  
        ///  DataTable dt = GetDataTable(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(DbConnection connection, string spName, params object[] parameterValues) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 比缓存中加载存储过程参数
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 给存储过程参数分配值
                AssignParameterValues(commandParameters, parameterValues);

                return GetDataTable(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataTable(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定事务的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataTable dt = GetDataSet(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(DbTransaction transaction, CommandType commandType, string commandText) {
            return GetDataTable(transaction, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定事务的命令,指定参数,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DataTable dt = GetDataSet(trans, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建 DataAdapter & DataSet
            using (DbDataAdapter da = Factory.CreateDataAdapter()) {
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                dt.TableName = "NewTableName";
                da.Fill(dt);
                cmd.Parameters.Clear();
                return dt;
            }
        }

        /// <summary>
        /// 执行指定事务的命令,指定参数值,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输入参数和返回值.
        /// 示例.:  
        ///  DataTable dt = GetDataTable(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">事务</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回一个包含结果集的DataTable</returns>
        public DataTable GetDataTable(DbTransaction transaction, string spName, params object[] parameterValues) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中加载存储过程参数
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数分配值
                AssignParameterValues(commandParameters, parameterValues);

                return GetDataTable(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else return GetDataTable(transaction, CommandType.StoredProcedure, spName);
        }
        #endregion GetDataSet数据集命令结束

        #region GetDbDataReader 数据阅读器
        /// <summary>
        /// 枚举,标识数据库连接是由BaseDbHelper提供还是由调用者提供
        /// </summary>
        private enum DbConnectionOwnership {
            /// <summary>由BaseDbHelper提供连接</summary>
            Internal,
            /// <summary>由调用者提供连接</summary>
            External
        }

        /// <summary>
        /// 执行指定数据库连接对象的数据阅读器.
        /// </summary>
        /// <remarks>
        /// 如果是BaseDbHelper打开连接,当连接关闭DataReader也将关闭.
        /// 如果是调用都打开连接,DataReader由调用都管理.
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="transaction">一个有效的事务,或者为 'null'</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">DbParameters参数数组,如果没有参数则为'null'</param>
        /// <param name="connectionOwnership">标识数据库连接对象是由调用者提供还是由BaseDbHelper提供</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        private DbDataReader GetDbDataReader(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, DbConnectionOwnership connectionOwnership) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;
            // 创建命令
            DbCommand cmd = Factory.CreateCommand();
            try {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                // 创建数据阅读器
                DbDataReader dataReader;

                if (connectionOwnership == DbConnectionOwnership.External)
                    dataReader = cmd.ExecuteReader();
                else
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                queryCount++;
                // 清除参数,以便再次使用..
                // HACK: There is a problem here, the output parameter values are fletched 
                // when the reader is closed, so if the parameters are detached from the command
                // then the SqlReader can磘 set its values. 
                // When this happen, the parameters can磘 be used again in other command.
                bool canClear = true;
                foreach (DbParameter commandParameter in cmd.Parameters) {
                    if (commandParameter.Direction != ParameterDirection.Input) canClear = false;
                }

                if (canClear) cmd.Parameters.Clear();

                return dataReader;
            } catch {
                if (mustCloseConnection) connection.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行指定数据库连接字符串的数据阅读器.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader("GetOrders");
        /// </remarks>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(string commandText) {
            return GetDbDataReader(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的数据阅读器,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader("GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组(new DbParameter("@prodid", 24))</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(string commandText, params DbParameter[] commandParameters) {
            return GetDbDataReader(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, commandParameters);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的数据阅读器.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader(CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(CommandType commandType, string commandText) {
            return GetDbDataReader(commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的数据阅读器,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader(CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">DbParameter参数数组(new DbParameter("@prodid", 24))</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            DbConnection connection = null;
            try {
                connection = Factory.CreateConnection();
                connection.ConnectionString = ConnString;
                connection.Open();

                return GetDbDataReader(connection, null, commandType, commandText, commandParameters, DbConnectionOwnership.Internal);
            } catch {
                // If we fail to return the SqlDatReader, we need to close the connection ourselves
                if (connection.IsNotNull()) connection.Close();
                throw;
            }

        }

        /// <summary>
        /// 执行指定数据库连接字符串的数据阅读器,指定参数值.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader("GetOrders", 24, 36);
        /// </remarks>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(string spName, params object[] parameterValues) {
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                AssignParameterValues(commandParameters, parameterValues);

                return GetDbDataReader(CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDbDataReader(CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定数据库连接对象的数据阅读器.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(DbConnection connection, CommandType commandType, string commandText) {
            return GetDbDataReader(connection, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// [调用者方式]执行指定数据库连接对象的数据阅读器,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader(conn, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandParameters">DbParameter参数数组</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            return GetDbDataReader(connection, (DbTransaction)null, commandType, commandText, commandParameters, DbConnectionOwnership.External);
        }

        /// <summary>
        /// [调用者方式]执行指定数据库连接对象的数据阅读器,指定参数值.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">T存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(DbConnection connection, string spName, params object[] parameterValues) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return GetDbDataReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDbDataReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数值.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(DbTransaction transaction, CommandType commandType, string commandText) {
            return GetDbDataReader(transaction, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///   DbDataReader dr = GetDbDataReader(trans, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            return GetDbDataReader(transaction.Connection, transaction, commandType, commandText, commandParameters, DbConnectionOwnership.External);
        }

        /// <summary>
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数值.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 
        /// 示例:  
        ///  DbDataReader dr = GetDbDataReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReader(DbTransaction transaction, string spName, params object[] parameterValues) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值
            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return GetDbDataReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                // 没有参数值
                return GetDbDataReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion GetDbDataReader数据阅读器

        #region GetScalar 返回结果集中的第一行第一列

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int orderCount = (int)GetScalar("GetOrderCount");
        /// </remarks>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(string commandText) {
            return GetScalar(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int orderCount = (int)GetScalar(CommandType.StoredProcedure, "GetOrderCount", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(string commandText, params DbParameter[] commandParameters) {
            return GetScalar(commandText.IndexOf(" ") > 0 ? CommandType.Text : CommandType.StoredProcedure, commandText, commandParameters);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int orderCount = (int)GetScalar(CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(CommandType commandType, string commandText) {
            // 执行参数为空的方法
            return GetScalar(commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int orderCount = (int)GetScalar(CommandType.StoredProcedure, "GetOrderCount", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            // 创建并打开数据库连接对象,操作完成释放对象.
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                // 调用指定数据库连接字符串重载方法.
                return GetScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,指定参数值,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 
        /// 示例:  
        ///  int orderCount = (int)GetScalar("GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(string spName, params object[] parameterValues) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);

            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                if (connection.IsNull()) throw new ArgumentNullException("connection");
                if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

                // 如果有参数值
                if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                    // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                    DbParameter[] commandParameters = GetSpParameterSet(spName);

                    // 给存储过程参数赋值
                    AssignParameterValues(commandParameters, parameterValues);

                    // 调用重载方法
                    return GetScalar(connection, spName, commandParameters);
                } else {
                    // 没有参数值
                    return GetScalar(CommandType.StoredProcedure, spName);
                }
            }
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int orderCount = (int)GetScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(DbConnection connection, CommandType commandType, string commandText) {
            // 执行参数为空的方法
            return GetScalar(connection, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int orderCount = (int)GetScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");

            // 创建DbCommand命令,并进行预处理
            DbCommand cmd = Factory.CreateCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (DbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行DbCommand命令,并返回结果.
            object retval = cmd.ExecuteScalar();

            // 清除参数,以便再次使用.
            cmd.Parameters.Clear();

            if (mustCloseConnection) connection.Close();

            return retval;
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,指定参数值,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 
        /// 示例:  
        ///  int orderCount = (int)GetScalar(conn, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(DbConnection connection, string spName, params object[] parameterValues) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值
            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 给存储过程参数赋值
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法
                return GetScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                // 没有参数值
                return GetScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定数据库事务的命令,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int orderCount = (int)GetScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(DbTransaction transaction, CommandType commandType, string commandText) {
            // 执行参数为空的方法
            return GetScalar(transaction, commandType, commandText, (DbParameter[])null);
        }

        /// <summary>
        /// 执行指定数据库事务的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  int orderCount = (int)GetScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 创建DbCommand命令,并进行预处理
            DbCommand cmd = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行DbCommand命令,并返回结果.
            object retval = cmd.ExecuteScalar();
            queryCount++;
            // 清除参数,以便再次使用.
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// 执行指定数据库事务的命令,指定参数值,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 
        /// 示例:  
        ///  int orderCount = (int)GetScalar(trans, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalar(DbTransaction transaction, string spName, params object[] parameterValues) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值
            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // PPull the parameters for this stored procedure from the parameter cache ()
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数赋值
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法
                return GetScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                // 没有参数值
                return GetScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion GetScalar

        #region FillDataSet 填充数据集
        /// <summary>
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  FillDataSet(CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)</param>
        public void FillDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (dataSet.IsNull()) throw new ArgumentNullException("dataSet");

            // 创建并打开数据库连接对象,操作完成释放对象.
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                // 调用指定数据库连接字符串重载方法.
                FillDataSet(connection, commandType, commandText, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集.指定命令参数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  FillDataSet(CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>
        public void FillDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (dataSet.IsNull()) throw new ArgumentNullException("dataSet");
            // 创建并打开数据库连接对象,操作完成释放对象.
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                // 调用指定数据库连接字符串重载方法.
                FillDataSet(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集,指定存储过程参数值.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 
        /// 示例:  
        ///  FillDataSet(CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24);
        /// </remarks>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>    
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        public void FillDataSet(string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (dataSet.IsNull()) throw new ArgumentNullException("dataSet");
            // 创建并打开数据库连接对象,操作完成释放对象.
            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                connection.Open();

                // 调用指定数据库连接字符串重载方法.
                FillDataSet(connection, spName, dataSet, tableNames, parameterValues);
            }
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  FillDataSet(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>    
        public void FillDataSet(DbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames) {
            FillDataSet(connection, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  FillDataSet(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        public void FillDataSet(DbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters) {
            FillDataSet(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集,指定存储过程参数值.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 
        /// 示例:  
        ///  FillDataSet(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36);
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        public void FillDataSet(DbConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (dataSet.IsNull()) throw new ArgumentNullException("dataSet");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值
            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 给存储过程参数赋值
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法
                FillDataSet(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            } else {
                // 没有参数值
                FillDataSet(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 执行指定数据库事务的命令,映射数据表并填充数据集.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  FillDataSet(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>
        public void FillDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames) {
            FillDataSet(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// 执行指定数据库事务的命令,映射数据表并填充数据集,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  FillDataSet(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        public void FillDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters) {
            FillDataSet(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// 执行指定数据库事务的命令,映射数据表并填充数据集,指定存储过程参数值.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值参数.
        /// 
        /// 示例:  
        ///  FillDataSet(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36);
        /// </remarks>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        public void FillDataSet(DbTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (dataSet.IsNull()) throw new ArgumentNullException("dataSet");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果有参数值
            if ((parameterValues.IsNotNull()) && (parameterValues.Length > 0)) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数赋值
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法
                FillDataSet(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            } else {
                // 没有参数值
                FillDataSet(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary>
        /// [私有方法][内部调用]执行指定数据库连接对象/事务的命令,映射数据表并填充数据集,DataSet/TableNames/DbParameters.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  FillDataSet(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="dataSet">要填充结果集的DataSet实例</param>
        /// <param name="tableNames">表映射的数据表数组
        /// 用户定义的表名 (可有是实际的表名.)
        /// </param>
        /// <param name="commandParameters">分配给命令的DbParameter参数数组</param>
        private void FillDataSet(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (dataSet.IsNull()) throw new ArgumentNullException("dataSet");

            // 创建DbCommand命令,并进行预处理
            DbCommand command = Factory.CreateCommand();
            bool mustCloseConnection = false;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行命令
            using (DbDataAdapter dataAdapter = Factory.CreateDataAdapter()) {
                dataAdapter.SelectCommand = command;
                // 追加表映射
                if (tableNames.IsNotNull() && tableNames.Length > 0) {
                    string tableName = "Table";
                    for (int index = 0; index < tableNames.Length; index++) {
                        if (tableNames[index].IsNull() || tableNames[index].Length == 0) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        tableName += (index + 1).ToString();
                    }
                }

                // 填充数据集使用默认表名称
                dataAdapter.Fill(dataSet);

                // 清除参数,以便再次使用.
                command.Parameters.Clear();
            }

            if (mustCloseConnection) connection.Close();
        }
        #endregion

        #region UpdateDataSet 更新数据集
        /// <summary>
        /// 执行数据集更新到数据库,指定inserted, updated, or deleted命令.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  UpdateDataSet(insertCommand, deleteCommand, updateCommand, dataSet, "Order");
        /// </remarks>
        /// <param name="insertCommand">[追加记录]一个有效的SQL语句或存储过程</param>
        /// <param name="deleteCommand">[删除记录]一个有效的SQL语句或存储过程</param>
        /// <param name="updateCommand">[更新记录]一个有效的SQL语句或存储过程</param>
        /// <param name="dataSet">要更新到数据库的DataSet</param>
        /// <param name="tableName">要更新到数据库的DataTable</param>
        public void UpdateDataSet(DbCommand insertCommand, DbCommand deleteCommand, DbCommand updateCommand, DataSet dataSet, string tableName) {
            if (insertCommand.IsNull()) throw new ArgumentNullException("insertCommand");
            if (deleteCommand.IsNull()) throw new ArgumentNullException("deleteCommand");
            if (updateCommand.IsNull()) throw new ArgumentNullException("updateCommand");
            if (tableName.IsNull() || tableName.Length == 0) throw new ArgumentNullException("tableName");

            // 创建DbDataAdapter,当操作完成后释放.
            using (DbDataAdapter dataAdapter = Factory.CreateDataAdapter()) {
                // 设置数据适配器命令
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;

                // 更新数据集改变到数据库
                dataAdapter.Update(dataSet, tableName);

                // 提交所有改变到数据集.
                dataSet.AcceptChanges();
            }
        }
        #endregion

        #region CreateCommand 创建一条DbCommand命令
        /// <summary>
        /// 创建DbCommand命令,指定数据库连接对象,存储过程名和参数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        ///  DbCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="sourceColumns">源表的列名称数组</param>
        /// <returns>返回DbCommand命令</returns>
        public DbCommand CreateCommand(DbConnection connection, string spName, params string[] sourceColumns) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 创建命令
            DbCommand cmd = Factory.CreateCommand();
            cmd.CommandText = spName;
            cmd.Connection = connection;
            cmd.CommandType = CommandType.StoredProcedure;

            // 如果有参数值
            if ((sourceColumns.IsNotNull()) && (sourceColumns.Length > 0)) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 将源表的列到映射到DataSet命令中.
                for (int index = 0; index < sourceColumns.Length; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];

                // Attach the discovered parameters to the DbCommand object
                AttachParameters(cmd, commandParameters);
            }

            return cmd;
        }
        #endregion

        #region ExecSqlTypedParams 类型化参数(DataRow)
        /// <summary>
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回受影响的行数.
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSqlTypedParams(String spName, DataRow dataRow) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return ExecSql(CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return ExecSql(CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回受影响的行数.
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSqlTypedParams(DbConnection connection, String spName, DataRow dataRow) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return ExecSql(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return ExecSql(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库事物的存储过程,使用DataRow做为参数值,返回受影响的行数.
        /// </summary>
        /// <param name="transaction">一个有效的连接事务 object</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回影响的行数</returns>
        public int ExecSqlTypedParams(DbTransaction transaction, String spName, DataRow dataRow) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // Sf the row has values, the store procedure parameters must be initialized
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return ExecSql(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return ExecSql(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region GetDataSetTypedParams 类型化参数(DataRow)
        /// <summary>
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataSet.</returns>
        public DataSet GetDataSetTypedParams(String spName, DataRow dataRow) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            //如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDataSet(CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataSet(CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataSet.</returns>
        public DataSet GetDataSetTypedParams(DbConnection connection, String spName, DataRow dataRow) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDataSet(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataSet(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="transaction">一个有效的连接事务 object</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataSet.</returns>
        public DataSet GetDataSetTypedParams(DbTransaction transaction, String spName, DataRow dataRow) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDataSet(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataSet(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region GetDataTableTypedParams 类型化参数(DataRow)
        /// <summary>
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataTable.</returns>
        public DataTable GetDataTableTypedParams(String spName, DataRow dataRow) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            //如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDataTable(CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataTable(CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataTable.</returns>
        public DataTable GetDataTableTypedParams(DbConnection connection, String spName, DataRow dataRow) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDataTable(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataTable(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="transaction">一个有效的连接事务 object</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataTable.</returns>
        public DataTable GetDataTableTypedParams(DbTransaction transaction, String spName, DataRow dataRow) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDataTable(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDataTable(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region GetDbDataReaderTypedParams 类型化参数(DataRow)
        /// <summary>
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataReader.
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReaderTypedParams(String spName, DataRow dataRow) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDbDataReader(ConnString, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDbDataReader(ConnString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataReader.
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReaderTypedParams(DbConnection connection, String spName, DataRow dataRow) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDbDataReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDbDataReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库事物的存储过程,使用DataRow做为参数值,返回DataReader.
        /// </summary>
        /// <param name="transaction">一个有效的连接事务 object</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回包含结果集的DbDataReader</returns>
        public DbDataReader GetDbDataReaderTypedParams(DbTransaction transaction, String spName, DataRow dataRow) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetDbDataReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetDbDataReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region GetScalarTypedParams 类型化参数(DataRow)
        /// <summary>
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列.
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalarTypedParams(String spName, DataRow dataRow) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetScalar(CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetScalar(CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列.
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalarTypedParams(DbConnection connection, String spName, DataRow dataRow) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列.
        /// </summary>
        /// <param name="transaction">一个有效的连接事务 object</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object GetScalarTypedParams(DbTransaction transaction, String spName, DataRow dataRow) {
            if (transaction.IsNull()) throw new ArgumentNullException("transaction");
            if (transaction.IsNotNull() && transaction.Connection.IsNull()) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化.
            if (dataRow.IsNotNull() && dataRow.ItemArray.Length > 0) {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. ()
                DbParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值
                AssignParameterValues(commandParameters, dataRow);

                return GetScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            } else {
                return GetScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region 缓存方法
        /// <summary>
        /// 追加参数数组到缓存.
        /// </summary>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">要缓存的参数数组</param>
        public void CacheParameterSet(string commandText, params DbParameter[] commandParameters) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (commandText.IsNull() || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = ConnString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// 从缓存中获取参数数组.
        /// </summary>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>参数数组</returns>
        public DbParameter[] GetCachedParameterSet(string commandText) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (commandText.IsNull() || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = ConnString + ":" + commandText;

            DbParameter[] cachedParameters = paramCache[hashKey] as DbParameter[];
            if (cachedParameters.IsNull()) return null; else return CloneParameters(cachedParameters);
        }
        #endregion 缓存方法结束

        #region 检索指定的存储过程的参数集
        /// <summary>
        /// 返回指定的存储过程的参数集
        /// </summary>
        /// <remarks>
        /// 这个方法将查询数据库,并将信息存储到缓存.
        /// </remarks>
        /// <param name="spName">存储过程名</param>
        /// <returns>返回DbParameter参数数组</returns>
        public DbParameter[] GetSpParameterSet(string spName) {
            return GetSpParameterSet(spName, false);
        }

        /// <summary>
        /// 返回指定的存储过程的参数集
        /// </summary>
        /// <remarks>
        /// 这个方法将查询数据库,并将信息存储到缓存.
        /// </remarks>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>返回DbParameter参数数组</returns>
        public DbParameter[] GetSpParameterSet(string spName, bool includeReturnValueParameter) {
            if (ConnString.IsNull() || ConnString.Length == 0) throw new ArgumentNullException(key);
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            using (DbConnection connection = Factory.CreateConnection()) {
                connection.ConnectionString = ConnString;
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// [内部]返回指定的存储过程的参数集(使用连接对象).
        /// </summary>
        /// <remarks>
        /// 这个方法将查询数据库,并将信息存储到缓存.
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接字符</param>
        /// <param name="spName">存储过程名</param>
        /// <returns>返回DbParameter参数数组</returns>
        internal DbParameter[] GetSpParameterSet(DbConnection connection, string spName) {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        /// [内部]返回指定的存储过程的参数集(使用连接对象)
        /// </summary>
        /// <remarks>
        /// 这个方法将查询数据库,并将信息存储到缓存.
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">
        /// 是否包含返回值参数
        /// </param>
        /// <returns>返回DbParameter参数数组</returns>
        internal DbParameter[] GetSpParameterSet(DbConnection connection, string spName, bool includeReturnValueParameter) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            using (DbConnection clonedConnection = (DbConnection)((ICloneable)connection).Clone()) {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// [私有]返回指定的存储过程的参数集(使用连接对象)
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>返回DbParameter参数数组</returns>
        private DbParameter[] GetSpParameterSetInternal(DbConnection connection, string spName, bool includeReturnValueParameter) {
            if (connection.IsNull()) throw new ArgumentNullException("connection");
            if (spName.IsNull() || spName.Length == 0) throw new ArgumentNullException("spName");

            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            DbParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as DbParameter[];
            if (cachedParameters.IsNull()) {
                DbParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }
        #endregion 参数集检索结束

        #region 生成参数
        /// <summary>
        /// 输入参数
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <param name="Value">参数值</param>
        /// <returns></returns>
        public DbParameter MakeInParam(string ParamName, DbType DbType, int Size, object Value) {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }
        /// <summary>
        /// 输出参数
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <returns></returns>
        public DbParameter MakeOutParam(string ParamName, DbType DbType, int Size) {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }
        /// <summary>
        /// 参数
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <param name="Direction">输入/输出</param>
        /// <param name="Value">值</param>
        /// <returns></returns>
        public DbParameter MakeParam(string ParamName, DbType DbType, Int32 Size, ParameterDirection Direction, object Value) {
            DbParameter param;
            param = Provider.MakeParam(ParamName, DbType, Size);
            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value.IsNull())) param.Value = Value;
            return param;
        }
        public DbParameter MakeParam(string ParamName, object Value) {
            return Provider.MakeParam(ParamName, Value);
        }
        #endregion 生成参数结束

        #region 执行GetScalar,将结果以字符串类型输出。
        /// <summary>
        /// 取第一行第一列数据
        /// </summary>
        /// <param name="commandType">commandType</param>
        /// <param name="commandText">SQL</param>
        /// <returns></returns>
        public string GetScalarToStr(CommandType commandType, string commandText) {
            object ec = GetScalar(commandType, commandText);
            if (ec.IsNull()) return "";
            return ec.ToString();
        }
        /// <summary>
        /// 取第一行第一列数据
        /// </summary>
        /// <param name="commandType">commandType</param>
        /// <param name="commandText">SQL</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public string GetScalarToStr(CommandType commandType, string commandText, params DbParameter[] commandParameters) {
            object ec = GetScalar(commandType, commandText, commandParameters);
            if (ec.IsNull()) return "";
            return ec.ToString();
        }
        #endregion

        /// <summary>
        /// 起始字符
        /// </summary>
        public string GetIdentifierStart() { return Provider.GetIdentifierStart(); }
        /// <summary>
        /// 结束字符
        /// </summary>
        public string GetIdentifierEnd() { return Provider.GetIdentifierEnd(); }
        /// <summary>
        /// 参数前导符号
        /// </summary>
        public string GetParamIdentifier() { return Provider.GetParamIdentifier(); }
        /// <summary>
        /// SqlServer大数据复制
        /// </summary>
        /// <param name="dt">数据源 dt.TableName一定要和数据库表名对应</param>
        /// <param name="dbkey">数据库</param>
        /// <param name="sqlOptions">选项 默认Default</param>
        /// <param name="isTran">是否使用事务 默认false</param>
        /// <param name="timeout">超时时间7200 2小时</param>
        /// <param name="batchSize">每一批次中的行数</param>
        /// <param name="error">错误处理</param>
        /// <returns>true/false</returns>
        public bool DataBulkCopy(DataTable dt, BulkCopyOptions sqlOptions = BulkCopyOptions.Default, bool isTran = false, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null) {
            return Provider.DataBulkCopy(dt, key, sqlOptions, isTran, timeout, batchSize, error);
        }
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
        public bool DataBulkCopy(DataTable dt, Action<Exception> error = null, BulkCopyOptions options = BulkCopyOptions.Default, bool isTran = false, int timeout = 7200, int batchSize = 10000) {
            return Provider.DataBulkCopy(dt, key, options, isTran, timeout, batchSize, error);
        }
        /// <summary>
        /// SqlServer大数据复制
        /// </summary>
        /// <param name="dt">数据源 dt.TableName一定要和数据库表名对应</param>
        /// <param name="dbkey">数据库</param>
        /// <param name="options">选项 默认Default</param>
        /// <param name="timeout">超时时间7200 2小时</param>
        /// <param name="batchSize">每一批次中的行数</param>
        /// <param name="error">错误处理</param>
        /// <param name="isTran">使用事务</param>
        /// <returns>true/false</returns>
        public bool DataBulkCopy(DataTable dt, bool isTran = true, BulkCopyOptions options = BulkCopyOptions.Default, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null) {
            return Provider.DataBulkCopy(dt, key, options, true, timeout, batchSize, error);
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
            return Provider.DataBulkCopy(conn, tran, dt, options, timeout, batchSize, error);
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
        public bool DataBulkCopy(IDbConnection conn, IDbTransaction tran, DataTable dt, Action<Exception> error = null, BulkCopyOptions options = BulkCopyOptions.Default, int timeout = 7200, int batchSize = 10000) { 
            return Provider.DataBulkCopy(conn, tran, dt, options, timeout, batchSize, error);
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
        public bool DataBulkCopy(IDataReader dr, string tableName, BulkCopyOptions options = BulkCopyOptions.Default, bool isTran = false, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null) {
            return Provider.DataBulkCopy(dr, tableName, key, options, isTran, timeout, batchSize, error);
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
        public bool DataBulkCopy(IDataReader dr, string tableName, Action<Exception> error = null, BulkCopyOptions options = BulkCopyOptions.Default, bool isTran = false, int timeout = 7200, int batchSize = 10000) {
            return Provider.DataBulkCopy(dr, tableName, key, options, isTran, timeout, batchSize, error);
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
        public bool DataBulkCopy(IDataReader dr, string tableName, bool isTran = true, BulkCopyOptions options = BulkCopyOptions.Default, int timeout = 7200, int batchSize = 10000, Action<Exception> error = null) {
            return Provider.DataBulkCopy(dr, tableName, key, options, isTran, timeout, batchSize, error);
        }
    }
}
