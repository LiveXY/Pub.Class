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
    /// <summary>
    /// ToJsonTest
    /// </summary>
    [TestClass]
    public class JavaDESEncode {
        
        public JavaDESEncode() {
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

        [TestMethod]
        public void DesDecode() {
            string str = "8rbSao7CbZc=";
            string key = "xF0gwba2RdU=";

            byte[] strbyte = str.FromBase64();
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            des.Key = Convert.FromBase64String(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(strbyte, 0, strbyte.Length);
            cs.FlushFinalBlock();
            Trace.WriteLine(ms.ToArray().ToUTF8());

            //SymmetryCryptor sc = new SymmetryCryptor();
            //sc.Encoding = Encoding.UTF8;
            //sc.SymmetricAlgorithmType = SymmetricAlgorithmType.DES;
            //sc.Initialize(CipherMode.ECB, PaddingMode.None);
            //Trace.WriteLine(sc.DecryptString(str, Convert.FromBase64String(key)));
        }

        [TestMethod]
        public void DesEncode() {
            string str = "1234";
            string key = "xF0gwba2RdU=";

            byte[] strbyte = str.ToBytes(Encoding.UTF8);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            des.Key = Convert.FromBase64String(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(strbyte, 0, strbyte.Length);
            cs.FlushFinalBlock();
            Trace.WriteLine(Convert.ToBase64String(ms.ToArray()));//8rbSao7CbZc=
        }
    }
}
