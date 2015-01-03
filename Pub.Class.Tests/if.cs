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
    public class IfTest {

        public IfTest() { }

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
            true.If(() => { Console.WriteLine("1"); return true; })
                .If(() => { Console.WriteLine("2"); return false; })
                .If(() => { Console.WriteLine("3"); return false; })
                .Else(() => { Console.WriteLine("4"); });
        }
    }
}
