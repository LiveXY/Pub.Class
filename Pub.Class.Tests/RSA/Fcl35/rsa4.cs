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
    public class rsa4Test {

        public rsa4Test() { }

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
            test2();
            Console.WriteLine();

            DateTime d1 = DateTime.Now;
            test1();
            TimeSpan t1 = DateTime.Now - d1;
            Console.WriteLine("用户登录用时:{0}", t1);
        }

        private void test1() { 
            //服务端生成好一对 公私钥
            RSACryptoServiceProvider rsaServer = new RSACryptoServiceProvider();
            //公钥给客户端，用来加密数据（内置，或第一次请求时下行给客户端）
            string clientPublicKey = rsaServer.ToXmlString(false);
            //私钥存在服务端，用来解密数据
            string serverPrivateKey = rsaServer.ToXmlString(true);

            //客户端也生成一对 公私钥
            RSACryptoServiceProvider rsaClient = new RSACryptoServiceProvider();
            //公钥加密后上行给服务端，用来加密数据
            string serverPublicKey = rsaClient.ToXmlString(false);
            //私钥存在客户端，用来解密下行数据
            string clientPrivateKey = rsaClient.ToXmlString(true);

            //客户端请求登录 生成上行数据
            string username = "test01";
            string password = "111111";
            login upPost = new login() { 
                username = RSAManaged4.Encrypt(username, clientPublicKey),
                password = RSAManaged4.Encrypt(password, clientPublicKey),
                key = RSAManaged4.Encrypt(serverPublicKey, clientPublicKey),
            };
            Console.WriteLine("上行数据：" + upPost.ToJson());

            //服务端解密 生成下行数据 
            //解密
            login userInfo = new login() { 
                username = RSAManaged4.Decrypt(upPost.username, serverPrivateKey),
                password = RSAManaged4.Decrypt(upPost.password, serverPrivateKey),
                key = RSAManaged4.Decrypt(upPost.key, serverPrivateKey),
            };
            Console.WriteLine("上行数据解密：" + userInfo.ToJson());

            //生成下行告诉客户断是否登录成功
            login downPost = new login() {
                username = RSAManaged4.Encrypt(userInfo.username, userInfo.key),
                message = RSAManaged4.Encrypt((username == "test01" && password == "111111") ? "登录成功！" : "登录失败", userInfo.key),
            };
            Console.WriteLine("下行数据：" + downPost.ToJson());

            //客户端取的下行数据，并解密
            userInfo = new login() {
                username = RSAManaged4.Decrypt(downPost.username, clientPrivateKey),
                message = RSAManaged4.Decrypt(downPost.message, clientPrivateKey),
            };
            Console.WriteLine("下行数据解密：" + userInfo.ToJson());

            Console.WriteLine(userInfo.username);
            Console.WriteLine(userInfo.message);
            Console.WriteLine();
        }

        private void test2() { 
            RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider();
            RSAParameters parameters = _rsa.ExportParameters(true);
            string Exponent = BitConverter.ToString(parameters.Exponent);
            string Mosulus = BitConverter.ToString(parameters.Modulus);
            string D = BitConverter.ToString(parameters.D);

            string publicKey = _rsa.ToXmlString(false);
            string privateKey = _rsa.ToXmlString(true);
            _rsa.Clear();


            RSAPublicKey _publicKey = RSAPublicKey.FromXmlString(publicKey);
            RSAPrivateKey _privateKey = RSAPrivateKey.FromXmlString(privateKey);

            string input = "这个极简单的 BigInteger 类的全部源程序代码可以在本随笔开头给出的 URL 中找到，只有五十多行。她是基于 10 进制的，内部使用一个 int[] 来存储，需要事先指定该数组的大小，不能动态增长，而且只能表示非负整数。";
            
            PublicKeyEncrypt(input, _publicKey, _privateKey);
            PublicKeySign(input, _publicKey, _privateKey);
        }

        private void PublicKeyEncrypt(string input, RSAPublicKey _publicKey, RSAPrivateKey _privateKey) {
            byte[] inputData = Encoding.UTF8.GetBytes(input);

            DateTime d1 = DateTime.Now;
            byte[] inputDateEnc = RSAManaged4.Encrypt(inputData, _publicKey);
            TimeSpan t1 = DateTime.Now - d1;
            Console.WriteLine("公钥加密用时:{0}", t1);

            DateTime d2 = DateTime.Now;
            byte[] inputDataDec = RSAManaged4.Decrypt(inputDateEnc, _privateKey);
            TimeSpan t2 = DateTime.Now - d2;
            Console.WriteLine("私钥解密用时:{0}", t2);

            string inputDec = Encoding.UTF8.GetString(inputDataDec, 0, inputDataDec.Length);
            Console.WriteLine(string.Format("私钥解密结果:{0}", inputDec));
            Console.WriteLine();
        }
        private void PublicKeySign(string input, RSAPublicKey _publicKey, RSAPrivateKey _privateKey) {
            byte[] inputData = Encoding.UTF8.GetBytes(input);
            SHA1Managed sha1 = new SHA1Managed();

            DateTime d1 = DateTime.Now;
            byte[] signature = RSAManaged4.Sign(inputData, _publicKey, sha1);
            TimeSpan t1 = DateTime.Now - d1;
            Console.WriteLine("公钥签名用时:{0}", t1);

            DateTime d2 = DateTime.Now;
            bool result = RSAManaged4.Verify(inputData, _privateKey, sha1, signature);
            TimeSpan t2 = DateTime.Now - d2;
            Console.WriteLine("私钥验证用时:{0}", t2);

            sha1.Clear();
            Console.WriteLine(string.Format("私钥验证结果:{0}", result));
            Console.WriteLine();
        }
    }
}
