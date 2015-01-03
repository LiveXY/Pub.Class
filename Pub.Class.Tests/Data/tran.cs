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
    public class TranTest {

        public TranTest() { }

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
            dbs.init();

            string sql = @"
insert into Test(ID,Name) values(1,'1');
insert into Test(ID,Name) values(2,'2');
insert into Test(ID,Name) values(3,'3');
insert into Test(ID,Name) values(4,'4');
insert into Test(ID,Name) values(5,'5');
insert into Test(ID,Name) values(6,'6');
insert into Test(ID,Name) values(7,'7');
insert into Test(ID,Name) values(8,'8');
insert into Test(ID,Name) values(1,'1');
";
            int count = new SQL(sql)
                .Database(dbs.master1)
                .ToExecTran();
            Console.WriteLine(count);
            Console.WriteLine();

            Data.Pool(dbs.master1).ExecSql("truncate table test");
            count = Data.Pool(dbs.master1).ExecTran((db, tran) => {
                count = 0;
                //true.If(()=>{
                //    int len = db.ExecSql(tran, System.Data.CommandType.Text, "insert into Test(ID,Name) values(1,'1')");
                //    count += len;
                //    return len == 1 ? true : false;
                //}).If(() => {
                //    int len = db.ExecSql(tran, System.Data.CommandType.Text, "insert into Test(ID,Name) values(2,'2')");
                //    count += len;
                //    return len == 1 ? true : false;
                //}).If(() => {
                //    int len = db.ExecSql(tran, System.Data.CommandType.Text, "insert into Test(ID,Name) values(3,'3')");
                //    count += len;
                //    return len == 1 ? true : false;
                //}).If(() => {
                //    int len = db.ExecSql(tran, System.Data.CommandType.Text, "insert into Test(ID,Name) values(1,'1')");
                //    count += len;
                //    return len == 1 ? true : false;
                //});
                count += db.ExecSql(tran, System.Data.CommandType.Text, "insert into Test(ID,Name) values(1,'1')");
                count += db.ExecSql(tran, System.Data.CommandType.Text, "insert into Test(ID,Name) values(2,'2')");
                count += db.ExecSql(tran, System.Data.CommandType.Text, "insert into Test(ID,Name) values(3,'3')");
                count += db.ExecSql(tran, System.Data.CommandType.Text, "insert into Test(ID,Name) values(4,'4')");
                return count;
            });
            Console.WriteLine(count);
            Console.WriteLine();
        }
    }
}
