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
    public class WhereTest {

        public WhereTest() { }

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
            string where = new Where()
                .And("UserName", "test1001", Operator.Equal) //=
                .AndBegin("MemberID", 1000, Operator.LessThan) //<
                .And("Score", 90, Operator.MoreThan) //>
                .And("Account", "test%", Operator.Like) //like
                .AndEnd("Status", "(1,2)", Operator.In) // in
                .And("CreateTime", "'2011-01-01' and '2011-12-12'", Operator.Between) //between
                .And("LastLoginTime", "2011-01-01", Operator.MoreThanOrEqual) //>=
                .And("LastLoginTime", "2011-12-12", Operator.LessThanOrEqual) //<=
                .And("UserName", "", Operator.Equal, true) //= 过滤为空的数据
                .Or("MemberID", 999, Operator.NotEqual) //<>
                .And("MemberID", "null", Operator.Is)//is null
                .And("MemberID", "null", Operator.IsNot)//is not null
                .Or("Money2", "Money1-Money3", Operator.Field)//Field
                .ToString();
            Console.WriteLine(where);
            Console.WriteLine("");

            //有参数
            Parameters pars = new Where()
                .And("UserName", "test1001", Operator.Equal) //=
                .AndBegin("MemberID", 1000, Operator.LessThan) //<
                .And("Score", 90, Operator.MoreThan) //>
                .And("Account", "test%", Operator.Like) //like
                .AndEnd("Status", "(1,2)", Operator.In) // in
                .And("CreateTime", "'2011-01-01' and '2011-12-12'", Operator.Between) //between
                .And("LastLoginTime", "2011-01-01", Operator.MoreThanOrEqual) //>=
                .And("LastLoginTime", "2011-12-12", Operator.LessThanOrEqual) //<=
                .And("UserName", "", Operator.Equal, true) //= 过滤为空的数据
                .Or("MemberID", 999, Operator.NotEqual) //<>
                .And("MemberID", "null", Operator.Is)//is null
                .And("MemberID", "null", Operator.IsNot)//is not null
                .Or("Money2", "Money1-Money3", Operator.Field)//Field
                .ToParameters();
            Console.WriteLine(pars.CommandText);
            Console.WriteLine(pars.ParameterList.ToJson());
            Console.WriteLine("");

            //有参数
            pars = new Where()
                .And("@UserName", "test1001", Operator.Equal) //=
                .AndBegin("@MemberID", 1000, Operator.LessThan) //<
                .And("@Score", 90, Operator.MoreThan) //>
                .And("@Account", "test%", Operator.Like) //like
                .AndEnd("Status", "(1,2)", Operator.In) // in
                .And("CreateTime", "'2011-01-01' and '2011-12-12'", Operator.Between) //between
                .And("@LastLoginTime", "2011-01-01", Operator.MoreThanOrEqual) //>=
                .And("@LastLoginTime", "2011-12-12", Operator.LessThanOrEqual) //<=
                .And("@UserName", "", Operator.Equal, true) //= 过滤为空的数据
                .Or("@MemberID", 999, Operator.NotEqual) //<>
                .And("MemberID", "null", Operator.Is)//is null
                .And("MemberID", "null", Operator.IsNot)//is not null
                .Or("Money2", "Money1-Money3", Operator.Field)//Field
                .ToParameters();
            Console.WriteLine(pars.CommandText);
            string par = "";
            foreach (var info in pars.ParameterList) { par += info.ParameterName + "|"; }
            Console.WriteLine(par);
        }
    }
}
