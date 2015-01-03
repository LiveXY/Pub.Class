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

namespace Pub.Class.Tests {
    /// <summary>
    /// ToJsonTest
    /// </summary>
    [TestClass]
    public class SorterTest {
        IList<int> list = Rand.RndInt(100000, 999999, 100000); 
        
        public SorterTest() {
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
        /// 堆排序
        /// </summary>
        /// <param name="i"></param>
        public void HeapSorter(int i = 0) {
            Action<bool> action = (p) => {
                IList<int> list2 = list; 
                HeapSorter<int>.Sort(list, true);
                if (p) {
                    Trace.WriteLine("堆排序：");
                    Trace.WriteLine(list2.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "堆排序", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 插入排序
        /// </summary>
        /// <param name="i"></param>
        public void InsertionSorter(int i = 0) {
            Action<bool> action = (p) => {
                IList<int> list2 = list; 
                InsertionSorter<int>.Sort(list, true);
                if (p) {
                    Trace.WriteLine("插入排序：");
                    Trace.WriteLine(list2.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "插入排序", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 归并排序
        /// </summary>
        /// <param name="i"></param>
        public void MergeSorter(int i = 0) {
            Action<bool> action = (p) => {
                IList<int> list2 = list; 
                MergeSorter<int>.Sort(list, true);
                if (p) {
                    Trace.WriteLine("归并排序：");
                    Trace.WriteLine(list2.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "归并排序", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="i"></param>
        public void QuickSorter(int i = 0) {
            Action<bool> action = (p) => {
                IList<int> list2 = list; 
                QuickSorter<int>.Sort(list, true);
                if (p) {
                    Trace.WriteLine("快速排序：");
                    Trace.WriteLine(list2.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "快速排序", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }

        [TestMethod]
        public void Sorter() {
            int i = 1;
            Trace.WriteLine("原Json：");
            Trace.WriteLine(list.ToJson());
            Trace.WriteLine("");

            //HeapSorter(i);
            //InsertionSorter(i);
            //MergeSorter(i);
            QuickSorter(i);
        }
    }
}
