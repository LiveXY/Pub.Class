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
    public class ToJsonTest {
        string _jsoninfo = "{\"status\":200,\"value\":{\"c_content\":\"P\",\"c_channel_code\":\"15073130421\",\"c_optc\":\"CMCC\",\"c_mobile\":\"13930018653\",\"m_amount\":1,\"t_collect_time\":\"2012-06-20 15:21:17\",\"c_link_id\":\"71813221087284979947\"},\"act\":\"ac\"}";
        mrinfo tojson = new mrinfo();

        public ToJsonTest() {
            tojson.status = _jsoninfo.GetJsonInt("status");
            tojson.act = _jsoninfo.GetJsonString("act");
            channel cha = new channel();
            cha.c_content = _jsoninfo.GetJsonString("c_content");
            cha.c_channel_code = _jsoninfo.GetJsonString("c_channel_code");
            cha.c_optc = _jsoninfo.GetJsonString("c_optc");
            cha.c_mobile = _jsoninfo.GetJsonString("c_mobile");
            cha.m_amount = _jsoninfo.GetJsonDecimal("m_amount");
            cha.t_collect_time = _jsoninfo.GetJsonString("t_collect_time").ToDateTime();
            cha.c_link_id = _jsoninfo.GetJsonString("c_link_id");
            tojson.value = cha;
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
        /// 字符串+
        /// </summary>
        /// <param name="i"></param>
        public void StringAddToJson(int i = 0) {
            Action<bool> action = (p) => {
                string json = "{\"status\":" + tojson.status + ",\"value\":{\"c_content\":\"" + tojson.value.c_content + 
                    "\",\"c_channel_code\":\"" + tojson.value.c_channel_code + "\",\"c_optc\":\"" + tojson.value.c_optc + 
                    "\",\"c_mobile\":\"" + tojson.value.c_mobile + "\",\"m_amount\":" + tojson.value.m_amount + 
                    ",\"t_collect_time\":\"" + tojson.value.t_collect_time + "\",\"c_link_id\":\"" + tojson.value.c_link_id + 
                    "\"},\"act\":\"" + tojson.act + "\"}";
                if (p) {
                    Trace.WriteLine("StringAdd：");
                    Trace.WriteLine(json);
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "StringAdd", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// StringBuilder ToJson
        /// </summary>
        /// <param name="i"></param>
        public void StringBuilderToJson(int i = 0) {
            Action<bool> action = (p) => {
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"status\":").Append(tojson.status).Append(",\"value\":{\"c_content\":\"").Append(tojson.value.c_content) 
                    .Append("\",\"c_channel_code\":\"").Append(tojson.value.c_channel_code).Append("\",\"c_optc\":\"").Append(tojson.value.c_optc) 
                    .Append("\",\"c_mobile\":\"").Append(tojson.value.c_mobile).Append("\",\"m_amount\":").Append(tojson.value.m_amount)
                    .Append(",\"t_collect_time\":\"").Append(tojson.value.t_collect_time).Append("\",\"c_link_id\":\"").Append(tojson.value.c_link_id)
                    .Append("\"},\"act\":\"").Append(tojson.act).Append("\"}");
                if (p) {
                    Trace.WriteLine("StringBuilder：");
                    Trace.WriteLine(sb.ToString());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "StringBuilder", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// System.Web.Script.Serialization.JavaScriptSerializer
        /// </summary>
        /// <param name="i"></param>
        public void SerializationToJson(int i = 0) {
            Action<bool> action = (p) => {
                string json = tojson.ToJson();
                if (p) {
                    Trace.WriteLine("Serialization：");
                    Trace.WriteLine(json);
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "Serialization", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// dynamic
        /// </summary>
        /// <param name="i"></param>
        public void DynamicJsonToJson(int i = 0) {
            Action<bool> action = (p) => {
                string json = DynamicJson.Serialize(tojson);
                if (p) {
                    Trace.WriteLine("DynamicJson：");
                    Trace.WriteLine(json);
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "DynamicJson", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 第三方库Newtonsoft.Json
        /// </summary>
        /// <param name="i"></param>
        public void NewtonsoftJsonToJson(int i = 0) {
            Action<bool> action = (p) => {
                string json = JsonConvert.SerializeObject(tojson);
                if (p) {
                    Trace.WriteLine("NewtonsoftJson：");
                    Trace.WriteLine(json);
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "NewtonsoftJson", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 第三方库FastJson
        /// </summary>
        /// <param name="i"></param>
        public void FastJsonToJson(int i = 0) {
            Action<bool> action = (p) => {
                string json = JSON.Instance.ToJSON(tojson);
                if (p) {
                    Trace.WriteLine("FastJson：");
                    Trace.WriteLine(json);
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "FastJson", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }

        [TestMethod]
        public void ToJson() {
            int i = 10000;
            Trace.WriteLine("原Json：");
            Trace.WriteLine(_jsoninfo);
            Trace.WriteLine("");

            StringBuilderToJson(i);
            StringAddToJson(i);
            SerializationToJson(i);
            NewtonsoftJsonToJson(i);
            DynamicJsonToJson(i);
            FastJsonToJson(i);
        }
    }
}
