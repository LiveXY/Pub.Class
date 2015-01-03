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
using System.Data;
using System.Reflection;
using System.Data.Common;

namespace Pub.Class.Tests {
    //[Serializable]
    //public class ApplicationFunction {
    //    public int FunctionID { set; get; }
    //    public string FunctionCode { set; get; }
    //    public string FunctionName { set; get; }
    //    public DateTime CreateTime { set; get; }
    //    public ApplicationFunction() {
    //        this.FunctionID = 0;
    //        this.FunctionCode = string.Empty;
    //        this.FunctionName = string.Empty;
    //        this.CreateTime = DateTime.MinValue;
    //    }
    //}
    [Serializable]
    public class ApplicationFunction {
        private int functionID = 0;
        public int FunctionID { set { functionID = value; } get { return functionID; } }
        private string functionCode = string.Empty;
        public string FunctionCode { set { functionCode = value; } get { return functionCode; } }
        private string functionName = string.Empty;
        public string FunctionName { set { functionName = value; } get { return functionName; } }
        private DateTime createTime = DateTime.MinValue;
        public DateTime CreateTime { set { createTime = value; } get { return createTime; } }
    }
    /// <summary>
    /// ToJsonTest
    /// </summary>
    [TestClass]
    public class ToList {
        Database db = Data.Pool(dbs.slaves);

        public ToList() {
            
        }

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
        /// <summary>
        /// 直接循环DataTable
        /// </summary>
        /// <param name="i"></param>
        public void Foreach(int i = 0) {
            //DataTable dt = db.GetDataTable("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            //DbDataReader dt = db.GetDbDataReader("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            //Action<bool> action = (p) => {
            //    IList<ApplicationFunction> list = new List<ApplicationFunction>();
            //    foreach (DataRow info in dt.Rows) {
            //        list.Add(new ApplicationFunction() {
            //            FunctionID = (int)info["FunctionID"],
            //            FunctionCode = (string)info["FunctionCode"],
            //            FunctionName = (string)info["FunctionName"],
            //            CreateTime = (DateTime)info["CreateTime"]
            //        });
            //    }
            //    if (p) {
            //        Trace.WriteLine("直接循环DataTable：");
            //        Trace.WriteLine(list[0].ToJson());
            //    }
            //};
            //if (i < 1) action(true);
            //else {
            //    string data = ActionExtensions.Time(() => action(false), "直接循环DataTable", i);
            //    Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            //}
        }
        /// <summary>
        /// 反射 IL
        /// </summary>
        /// <param name="i"></param>
        public void ToList1(int i = 0) {
            DataTable dt = db.GetDataTable("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            //DbDataReader dt = db.GetDbDataReader("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            Action<bool> action = (p) => {
                IList<ApplicationFunction> list = dt.ToList<ApplicationFunction>();
                if (p) {
                    Trace.WriteLine("反射 IL：");
                    Trace.WriteLine(list.ToJson());
                }
            };
            if (i < 1) action(true);
            else {
                string data = ActionExtensions.Time(() => action(false), "反射 IL", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 反射2
        /// </summary>
        /// <param name="i"></param>
        public void ToList2(int i = 0) {
            DataTable dt = db.GetDataTable("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            //DbDataReader dt = db.GetDbDataReader("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            Action<bool> action = (p) => {
                IList<ApplicationFunction> list = dt.ToList2<ApplicationFunction>();
                if (p) {
                    Trace.WriteLine("反射2：");
                    Trace.WriteLine(list.ToJson());
                }
            };
            if (i < 1) action(true);
            else {
                string data = ActionExtensions.Time(() => action(false), "反射2", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 反射3
        /// </summary>
        /// <param name="i"></param>
        public void ToList3(int i = 0) {
            DataTable dt = db.GetDataTable("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            //DbDataReader dt = db.GetDbDataReader("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            Action<bool> action = (p) => {
                IList<ApplicationFunction> list = dt.ToList3<ApplicationFunction>();
                if (p) {
                    Trace.WriteLine("反射3：");
                    Trace.WriteLine(list.ToJson());
                }
            };
            if (i < 1) action(true);
            else {
                string data = ActionExtensions.Time(() => action(false), "反射3", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 反射4
        /// </summary>
        /// <param name="i"></param>
        public void ToList4(int i = 0) {
            DataTable dt = db.GetDataTable("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            //DbDataReader dt = db.GetDbDataReader("select FunctionID,FunctionCode,FunctionName,CreateTime from ApplicationFunction");
            Action<bool> action = (p) => {
                IList<ApplicationFunction> list = dt.ToList4<ApplicationFunction>();
                if (p) {
                    Trace.WriteLine("反射4：");
                    Trace.WriteLine(list.ToJson());
                }
            };
            if (i < 1) action(true);
            else {
                string data = ActionExtensions.Time(() => action(false), "反射4", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }

        [TestMethod]
        public void ToListTest() {
            int i = 1000;
            //Trace.WriteLine("原Json：");
            //Trace.WriteLine(dt.ToJson());
            //Trace.WriteLine("");

            Foreach(i);
            ToList1(i);
            ToList2(i);
            ToList3(i);
            ToList4(i);
        }
    }
}
