using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pub.Class;
using System.Dynamic;
using System.Diagnostics;
using Newtonsoft.Json;
using fastJSON;
using System.IO;
using System.Security.Cryptography;
using System.Data;
using System.Data.Common;

namespace Pub.Class.Tests {
    [TestClass]
    public class SQLTestParam {

        public SQLTestParam() { }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void Test() {
            //Data.Pool("test").ExecTran<int>((db, tran) => {
            //    db.ExecSql(tran, "");
            //    db.GetDataTable(tran, CommandType.Text, "");
            //    db.GetDataTable(tran, "", new DbParameter[]{ });
            //    return 1;
            //});
            //有参数
            SQL sql = new SQL()
                .Select("MemberID", "RealName", "CustomerName")
                .Top(1)
                .Distinct()
                .From("UC_Member as a")
                .InnerJoin("EC_Customer as b").On("a.CustomerID", "=", "b.CustomerID")
                .Where("MemberID=@MemberID")
                .Where(new Where().And("@Account", "admin", Operator.Equal))
                .GroupBy("MemberID", "RealName", "CustomerName")
                .Having("MemberID=@MemberID")
                .OrderBy("MemberID")
                .OrderByDescending("RealName")
                .AddParameter("@MemberID", 1);
            string strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL()
                .Select("CategoryName", "NewsID")
                .From("News as a")
                .InnerJoin("News_Category as b").On("b.NCID", "=", "a.NCID")
                //.Count("NewsID")
                .Where("NewsID", "<", "@NewsID")
                .Where(new Where().And("@NewsID", 3, Operator.Equal))
                .Top(1)
                .Distinct()
                .GroupBy("CategoryName", "NewsID")
                .Having("NewsID", "<", "@NewsID")
                .OrderByDescending("NewsID")
                .AddParameter("@NewsID", 10);
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL()
                .Insert("News_Category")
                .Value("@CategoryName", "test")
                .Value("@ParentID", 0)
                //.Value("ParentID", null, true)
                .Value("@ExtUrl", "http://www.relaxlife.net")
                .Value("@OrderNum", 0);
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL()
                .Insert("News_Category", "@CategoryName", "@ParentID", "@ExtUrl", "@OrderNum")
                .Values("test", 1, "http://www.relaxlife.net", 0);
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL()
                .Insert("News_Category", "CategoryName", "ParentID", "ExtUrl", "OrderNum")
                .Select("CategoryName", "ParentID", "ExtUrl", "OrderNum")
                .From("News_Category");
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL()
                .Update("News_Category")
                .Set("OrderNum=OrderNum+1")
                .Set("@IsHide", false)
                .Set("@ParentID", null, true)
                .Where("NCID", "=", "@NCID")
                .AddParameter("@NCID", 1);
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL()
                .Delete()
                .From("News_Category")
                .Where("NCID", "=", "@NCID")
                .AddParameter("@NCID", 1);
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL()
                .Delete("News_Category")
                .Where("NCID=@NCID")
                .AddParameter("@NCID", 1);
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL()
                .Delete("News_Category")
                .From("News_Category")
                .Where("NCID=@NCID")
                .AddParameter("@NCID", 1);
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            sql = new SQL("select * from News_Category where NCID=@NCID")
                .AddParameter("@NCID", 1);
            strSql = sql.ToString();
            Console.WriteLine(strSql);
            Console.WriteLine(sql.ParametersToNames());
            Console.WriteLine("");

            Console.WriteLine(
                new SQL("select top 2 * from LC_Issue")
                    .Database(dbs.slaves)
                    .ToDataTable()
                    .ToJson()
            );

            Console.WriteLine(
                new SQL("select * from UC_Member where UserLevel=@UserLevel")
                    .Database(dbs.slaves)
                    .AddParameter("@UserLevel", 2)
                    .ToDataTable().Rows.Count
            );
            Console.WriteLine("");
        }
    }
}
