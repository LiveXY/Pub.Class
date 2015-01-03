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

namespace Pub.Class.Tests {
    [TestClass]
    public class DataTableTest {

        public DataTableTest() { }

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
            DataTable dt = new DataTable()
                .AddColumn<int>("id")
                .AddColumn<string>("name")
                .AddRow(1, "1")
                .AddRow(2, "2");

            dt.AddRow(3, "3");
            Console.WriteLine(dt.Rows.Count);
            Console.WriteLine("");
            Console.WriteLine(dt.ToJson());
            Console.WriteLine("");
            Console.WriteLine(dt.ToCSV());
            Console.WriteLine("");

            dt.ColumnRename("id", "ID");
            Console.WriteLine(dt.ToJson());
            Console.WriteLine("");

            Console.WriteLine(dt.SwapDTCR().ToJson());
            Console.WriteLine("");
        }
    }
}
