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

namespace Pub.Class.Tests {
    [TestClass]
    public class SQLTest {

        public SQLTest() { }

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
            //无参数
            string strSql = new SQL()
                .Select("MemberID", "RealName", "CustomerName")
                .Top(1)
                .Distinct()
                .From("UC_Member as a")
                .InnerJoin("EC_Customer as b").On("a.CustomerID", "=", "b.CustomerID")
                .Where("MemberID=1")
                .Where(new Where().And("Account", "admin", Operator.Equal))
                .GroupBy("MemberID", "RealName", "CustomerName")
                .Having("MemberID=1")
                .OrderBy("MemberID")
                .OrderByDescending("RealName")
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL()
                .Select("CategoryName", "NewsID")
                .From("News as a")
                .InnerJoin("News_Category as b").On("b.NCID", "=", "a.NCID")
                //.Count("NewsID")
                .Where("NewsID", "<", "10")
                .Where(new Where().And("NewsID", 3, Operator.Equal))
                .Top(1)
                .Distinct()
                .GroupBy("CategoryName", "NewsID")
                .Having("NewsID", "<", "10")
                .OrderByDescending("NewsID")
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL()
                .Insert("News_Category")
                .Value("CategoryName", "test")
                .Value("ParentID", 0)
                //.Value("ParentID", null, true)
                .Value("ExtUrl", "http://www.relaxlife.net")
                .Value("OrderNum", 0)
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL()
                .Insert("News_Category", "CategoryName", "ParentID", "ExtUrl", "OrderNum")
                .Values("test", 1, "http://www.relaxlife.net", 0)
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL()
                .Insert("News_Category", "CategoryName", "ParentID", "ExtUrl", "OrderNum")
                .Select("CategoryName", "ParentID", "ExtUrl", "OrderNum")
                .From("News_Category")
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL()
                .Update("News_Category")
                .Set("OrderNum=OrderNum+1")
                .Set("IsHide", false)
                .Set("ParentID", null, true)
                .Where("NCID", "=", "1")
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL()
                .Delete()
                .From("News_Category")
                .Where("NCID", "=", "1")
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL()
                .Delete("News_Category")
                .Where("NCID", "=", "1")
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL()
                .Delete("News_Category")
                .From("News_Category")
                .Where("NCID", "=", "1")
                .ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");

            strSql = new SQL("select * from News_Category").ToString();
            Console.WriteLine(strSql);
            Console.WriteLine("");
        }
    }
}
