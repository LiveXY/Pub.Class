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
    public class channel {
        public string c_content { set; get; }
        public string c_channel_code { set; get; }
        public string c_optc { set; get; }
        public string c_mobile { set; get; }
        public decimal m_amount { set; get; }
        public DateTime t_collect_time { set; get; }
        public string c_link_id { set; get; }
    }
    public class mrinfo {
        public int status { set; get; }
        public channel value { set; get; }
        public string act { set; get; }
    }
    /// <summary>
    /// FormJsonTest
    /// </summary>
    [TestClass]
    public class FormJsonTest {
        string _jsoninfo = "{\"status\":200,\"value\":{\"c_content\":\"P\",\"c_channel_code\":\"15073130421\",\"c_optc\":\"CMCC\",\"c_mobile\":\"13930018653\",\"m_amount\":1,\"t_collect_time\":\"2012-06-20 15:21:17\",\"c_link_id\":\"71813221087284979947\"},\"act\":\"ac\"}";

        public FormJsonTest() {

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
        /// 使用正则+扩展方法GetJson*  Regex.Match
        /// </summary>
        /// <param name="i"></param>
        public void GetJsonExtensionsFromJson(int i = 0) {
            Action<bool> action = (p) => {
                string jsoninfo = _jsoninfo.Replace(":200,", ":" + Rand.RndNum(4) + ",");
                mrinfo mr = new mrinfo();
                mr.status = jsoninfo.GetJsonInt("status");
                mr.act = jsoninfo.GetJsonString("act");
                channel cha = new channel();
                cha.c_content = jsoninfo.GetJsonString("c_content");
                cha.c_channel_code = jsoninfo.GetJsonString("c_channel_code");
                cha.c_optc = jsoninfo.GetJsonString("c_optc");
                cha.c_mobile = jsoninfo.GetJsonString("c_mobile");
                cha.m_amount = jsoninfo.GetJsonDecimal("m_amount");
                cha.t_collect_time = jsoninfo.GetJsonString("t_collect_time").ToDateTime();
                cha.c_link_id = jsoninfo.GetJsonString("c_link_id");
                mr.value = cha;
                if (p) {
                    Trace.WriteLine("GetJsonExtensions：");
                    Trace.WriteLine(mr.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "GetJsonExtensions", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 使用正则匹配IList&lt;string> Regex.Matches
        /// </summary>
        /// <param name="i"></param>
        public void GetMatchingValuesFromJson(int i = 0) {
            Action<bool> action = (p) => {
                string jsoninfo = _jsoninfo.Replace(":200,", ":" + Rand.RndNum(4) + ",");
                mrinfo mr = new mrinfo();
                mr.status = (jsoninfo.GetMatchingValues("\"status\":(\\d+)", "\"status\":", ",").FirstOrDefault() ?? "").ToInt(0);
                mr.act = jsoninfo.GetMatchingValues("\"act\":\"(.+?)\"", "\"act\":\"", "\"").FirstOrDefault() ?? "";
                channel cha = new channel();
                cha.c_content = jsoninfo.GetMatchingValues("\"c_content\":\"(.+?)\"", "\"c_content\":\"", "\"").FirstOrDefault() ?? "";
                cha.c_channel_code = jsoninfo.GetMatchingValues("\"c_channel_code\":\"(.+?)\"", "\"c_channel_code\":\"", "\"").FirstOrDefault() ?? "";
                cha.c_optc = (jsoninfo.GetMatchingValues("\"c_optc\":\"(.+?)\"", "\"c_optc\":\"", "\"").FirstOrDefault() ?? "");
                cha.c_mobile = jsoninfo.GetMatchingValues("\"c_mobile\":\"(.+?)\"", "\"c_mobile\":\"", "\"").FirstOrDefault() ?? "";
                cha.m_amount = (jsoninfo.GetMatchingValues("\"m_amount\":(\\d+)", "\"m_amount\":", ",").FirstOrDefault() ?? "").ToInt(0);
                cha.t_collect_time = (jsoninfo.GetMatchingValues("\"t_collect_time\":\"(.+?)\"", "\"t_collect_time\":\"", "\"").FirstOrDefault() ?? "").ToDateTime();
                cha.c_link_id = jsoninfo.GetMatchingValues("\"c_link_id\":\"(.+?)\"", "\"c_link_id\":\"", "\"").FirstOrDefault() ?? "";
                mr.value = cha;
                if (p) {
                    Trace.WriteLine("GetMatchingValues：");
                    Trace.WriteLine(mr.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "GetMatchingValues", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 使用正则匹配单个记录 Regex.Match + Remove
        /// </summary>
        /// <param name="i"></param>
        public void GetMatchingValueFromJson(int i = 0) {
            Action<bool> action = (p) => {
                string jsoninfo = _jsoninfo.Replace(":200,", ":" + Rand.RndNum(4) + ",");
                mrinfo mr = new mrinfo();
                mr.status = jsoninfo.GetMatchingValue("\"status\":(\\d+)", 9, 0).ToInt(0);
                mr.act = jsoninfo.GetMatchingValue("\"act\":\"(.+?)\"", 7, 1);
                channel cha = new channel();
                cha.c_content = jsoninfo.GetMatchingValue("\"c_content\":\"(.+?)\"", 13, 1);
                cha.c_channel_code = jsoninfo.GetMatchingValue("\"c_channel_code\":\"(.+?)\"", 18, 1);
                cha.c_optc = jsoninfo.GetMatchingValue("\"c_optc\":\"(.+?)\"", 10, 1);
                cha.c_mobile = jsoninfo.GetMatchingValue("\"c_mobile\":\"(.+?)\"", 12, 1);
                cha.m_amount = jsoninfo.GetMatchingValue("\"m_amount\":(\\d+)", 11, 0).ToInt(0);
                cha.t_collect_time = jsoninfo.GetMatchingValue("\"t_collect_time\":\"(.+?)\"", 18, 1).ToDateTime();
                cha.c_link_id = jsoninfo.GetMatchingValue("\"c_link_id\":\"(.+?)\"", 13, 1);
                mr.value = cha;
                if (p) {
                    Trace.WriteLine("GetMatchingValue：");
                    Trace.WriteLine(mr.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "GetMatchingValue", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// 使用正则匹配单个记录 Regex.Match + Replace
        /// </summary>
        /// <param name="i"></param>
        public void GetMatchingValue2FromJson(int i = 0) {
            Action<bool> action = (p) => {
                string jsoninfo = _jsoninfo.Replace(":200,", ":" + Rand.RndNum(4) + ",");
                mrinfo mr = new mrinfo();
                mr.status = jsoninfo.GetMatchingValue("\"status\":(\\d+)", "\"status\":", ",").ToInt(0);
                mr.act = jsoninfo.GetMatchingValue("\"act\":\"(.+?)\"", "\"act\":\"", "\"");
                channel cha = new channel();
                cha.c_content = jsoninfo.GetMatchingValue("\"c_content\":\"(.+?)\"", "\"c_content\":\"", "\"");
                cha.c_channel_code = jsoninfo.GetMatchingValue("\"c_channel_code\":\"(.+?)\"", "\"c_channel_code\":\"", "\"");
                cha.c_optc = jsoninfo.GetMatchingValue("\"c_optc\":\"(.+?)\"", "\"c_optc\":\"", "\"");
                cha.c_mobile = jsoninfo.GetMatchingValue("\"c_mobile\":\"(.+?)\"", "\"c_mobile\":\"", "\"");
                cha.m_amount = jsoninfo.GetMatchingValue("\"m_amount\":(\\d+)", "\"m_amount\":", ",").ToInt(0);
                cha.t_collect_time = jsoninfo.GetMatchingValue("\"t_collect_time\":\"(.+?)\"", "\"t_collect_time\":\"", "\"").ToDateTime();
                cha.c_link_id = jsoninfo.GetMatchingValue("\"c_link_id\":\"(.+?)\"", "\"c_link_id\":\"", "\"");
                mr.value = cha;
                if (p) {
                    Trace.WriteLine("GetMatchingValue2：");
                    Trace.WriteLine(mr.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "GetMatchingValue2", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }
        /// <summary>
        /// System.Web.Script.Serialization.JavaScriptSerializer
        /// </summary>
        /// <param name="i"></param>
        public void SerializationFromJson(int i = 0) {
            Action<bool> action = (p) => {
                string jsoninfo = _jsoninfo.Replace(":200,", ":" + Rand.RndNum(4) + ",");
                mrinfo mr = jsoninfo.FromJson<mrinfo>();
                if (p) {
                    Trace.WriteLine("Serialization：");
                    Trace.WriteLine(mr.ToJson());
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
        public void DynamicJsonFromJson(int i = 0) {
            Action<bool> action = (p) => {
                string jsoninfo = _jsoninfo.Replace(":200,", ":" + Rand.RndNum(4) + ",");
                var json = Pub.Class.DynamicJson.Parse(jsoninfo);
                string json1 = string.Format("{0}\"status\":{2},\"value\":{0}\"c_content\":\"{4}\",\"c_channel_code\":\"{5}\",\"c_optc\":\"{6}\",\"c_mobile\":\"{7}\",\"m_amount\":{8},\"t_collect_time\":\"{9}\",\"c_link_id\":\"{10}\"{1},\"act\":\"{3}\"{1}", "{", "}", json.status, json.act, json.value.c_content, json.value.c_channel_code, json.value.c_optc, json.value.c_mobile, json.value.m_amount, json.value.t_collect_time, json.value.c_link_id);
                if (p) {
                    Trace.WriteLine("DynamicJson：");
                    Trace.WriteLine(json1);
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
        public void NewtonsoftJsonFromJson(int i = 0) {
            Action<bool> action = (p) => {
                string jsoninfo = _jsoninfo.Replace(":200,", ":" + Rand.RndInt(1,1000) + ",");
                mrinfo mr = JsonConvert.DeserializeObject<mrinfo>(jsoninfo);
                if (p) {
                    Trace.WriteLine("NewtonsoftJson：");
                    Trace.WriteLine(mr.ToJson());
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
        public void FastJsonFromJson(int i = 0) {
            Action<bool> action = (p) => {
                string jsoninfo = _jsoninfo.Replace(":200,", ":" + Rand.RndNum(4) + ",");
                mrinfo mr = JSON.Instance.ToObject<mrinfo>(jsoninfo);
                if (p) {
                    Trace.WriteLine("FastJson：");
                    Trace.WriteLine(mr.ToJson());
                }
            };
            if (i < 1) action(true); else { 
                string data = ActionExtensions.Time(() => action(false), "FastJson", i);
                Trace.WriteLine(data.Replace("<br />", Environment.NewLine));
            }
        }

        [TestMethod]
        public void FromJson() {
            int i = 10;
            Trace.WriteLine("原Json：");
            Trace.WriteLine(_jsoninfo);
            Trace.WriteLine("");

            SerializationFromJson(i);
            GetMatchingValueFromJson(i);
            NewtonsoftJsonFromJson(i);
            GetMatchingValuesFromJson(i);
            GetMatchingValue2FromJson(i);
            GetJsonExtensionsFromJson(i);
            DynamicJsonFromJson(i);
            //FastJsonFromJson(i);
        }
    }
}
