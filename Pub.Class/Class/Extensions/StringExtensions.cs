//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Linq.Expressions;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.VisualBasic;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Security;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Pub.Class {
    /// <summary>
    /// string 扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class StringExtensions {
        /// <summary>
        /// 过滤字符串
        /// </summary>
        public const string BANWORD = "(毛泽东|周恩来|刘少奇|朱德|彭德怀|林彪|刘伯承|陈毅|贺龙|聂荣臻|徐向前|罗荣桓|叶剑英|李大钊|陈独秀|孙中山|孙文|孙逸仙|邓小平|陈云|江泽民|李鹏|朱镕基|李瑞环|尉健行|李岚清|胡锦涛|罗干|温家宝|吴邦国|曾庆红|贾庆林|黄菊|吴官正|李长春|吴仪|回良玉|曾培炎|周永康|曹刚川|唐家璇|华建敏|陈至立|陈良宇|张德江|张立昌|俞正声|王乐泉|刘云山|王刚|王兆国|刘淇|贺国强|郭伯雄|胡耀邦|王乐泉|王兆国|周永康|李登辉|连战|陈水扁|宋楚瑜|吕秀莲|郁慕明|蒋介石|蒋中正|蒋经国|马英九本拉登|奥马尔|柴玲|达赖喇嘛|江青|张春桥|姚文元|王洪文|东条英机|希特勒|墨索里尼|冈村秀树|冈村宁次|高丽朴|赵紫阳|王丹|沃尔开西|李洪志|李大师|赖昌星|马加爵山本五十六|阿扁|六四|六四运动|六四大屠杀|6 4事件|中国1989 6 4事件视频|6 4事件视频|1989 6 4事件|中国1989 6 4事件|1989事件视频|北京民运89|1989年北京学生|1989年学运动|六四20年|八九民运|1989年学潮事件|四二六社论|426社论密宗|中共屠城|共产党屠城|民国|民进党|民运|民主潮|摩门教|纳粹|南华早报|南蛮|明慧网|起义|亲民党|瘸腿帮|人民报|法轮功|法轮大法|打倒共产党|台独万岁|圣战|示威|台独|台独分子|台联|台湾民国|台湾岛国|台湾独立|太子党|天安门事件|屠杀|小泉|新党|新疆独立|新疆分裂|新疆国|疆独|西藏独立|西藏分裂|西藏国|藏独|藏青会|藏妇会|学潮|学运|一党专政|一中一台|两个中国|一贯道|游行|圆满|造反|真善忍|镇压|政变|政治|政治反对派|政治犯|中共|反党|反共|政府|民主党|中国之春|转法轮|自焚|共党|共匪|苏家屯|基地组织|塔利班|东亚病夫|支那|高治联|高自联|核工业基地|核武器|铀|原子弹|氢弹|导弹|核潜艇|大参考|小参考|国内动态清样|升天|圣战 |白莲教 |东正教|大法|法轮|法轮功|瘸腿帮|真理教|真善忍|转法轮|自焚|走向圆满|黄大仙|风水|跳大神|神汉|神婆|真理教|大卫教 |藏独|高丽棒子|回回|疆独|蒙古鞑子|台独|台独分子|台联|台湾民国|西藏独立|新疆独立|南蛮|老毛子|习近平|开房|台湾岛国|藏青|藏复|本拉登|西藏国|强奸|轮奸|抢劫|先奸后杀|下注|押大|押小|抽头|坐庄|赌马|赌球|筹码|老虎机|轮盘赌|安非他命|大麻|可卡因|海洛因|冰毒|摇头丸|杜冷丁|鸦片|罂粟|迷幻药|白粉|嗑药|吸毒|屙|爱滋|淋病|梅毒|爱液|屄|逼|臭机八|臭鸡巴|吹喇叭|吹箫|催情药|屌|肛交|肛门|龟头|黄色|机八|机巴|鸡八|鸡巴|机掰|机巴|鸡叭|鸡鸡|鸡掰|鸡奸|妓女|奸|茎|精液|精子|尻|口交|滥交|乱交|轮奸|卖淫|屁眼|嫖娼|强奸|强奸犯|情色|肉棒|乳房|乳峰|乳交|乳头|乳晕|三陪|色情|射精|手淫|威而钢|威而柔|伟哥|性高潮|性交|性虐|性欲|穴|颜射|阳物|一夜情|阴部|阴唇|阴道|阴蒂|阴核|阴户|阴茎|阴门|淫|淫秽|淫乱|淫水|淫娃|淫液|淫汁|淫穴|淫洞|援交妹|做爱|梦遗|阳痿|早泄|奸淫|性欲|性交|K他命|白痴|笨蛋|屄|逼|变态|婊子|操她妈|操妳妈|操你|操你妈|操他妈|草你|肏|册那|侧那|测拿|插|蠢猪|荡妇|发骚|废物|干她妈|干妳|干妳娘|干你|干你妈|干你妈B|干你妈b|干你妈逼|干你娘|干他妈|狗娘养的|滚|鸡奸|贱货|贱人|烂人|老母|老土|妈比|妈的|马的|妳老母的|妳娘的|你妈逼|破鞋|仆街|去她妈|去妳的|去妳妈|去你的|去你妈|去死|去他妈|日你|赛她娘|赛妳娘|赛你娘|赛他娘|骚货|傻B|傻比|傻子|上妳|上你|神经病|屎|屎妳娘|屎你娘|他妈的|王八蛋|我日|乡巴佬|猪猡|屙|尿|掯|屌|骑你|湿了|操你|操他|操她|骑你|骑他|骑她|欠骑|欠人骑|来爽我|来插我|干你|干他|干她|干死|干爆|干机|FUCK|机叭|臭鸡|臭机|烂鸟|览叫|阳具|肉棒|肉壶|奶子|摸咪咪|干鸡|干入|小穴|强奸|插你|插你|爽你|爽你|干干|干X|他干|干它|干牠|干您|干汝|干林|操林|干尼|操尼|我咧干|干勒|干我|干到|干啦|干爽|欠干|狗干|我干|来干|轮干|轮流干|干一干|援交|骑你|轮奸|鸡奸|奸暴|再奸|我奸|奸你|奸你|奸他|奸她|奸一奸|淫水|淫湿|鸡歪|仆街|臭西|尻|吊|遗精|烂逼|大血比|叼你妈|靠你妈|草你|干你|日你|插你|奸你|戳你|逼你老母|挨球|我日你|草拟妈|卖逼|狗操卖逼|奸淫|日死|奶子|阴茎|奶娘|他娘|她娘|骚B|你妈了妹|逼毛|插你妈|叼你|渣波波|嫩b)";
        /// <summary>
        /// 字符串是否为string.Empty || null || ""
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static bool IsNullEmpty(this string str) {
            return string.IsNullOrEmpty(str);
        }
        /// <summary>
        /// Guid是否为 null || "000000-0000-0000-0000000000"
        /// </summary>
        /// <param name="guid">Guid扩展</param>
        /// <returns></returns>
        public static bool IsNull(this Guid? guid) {
            return guid.IsNull() || guid == Guid.Empty;
        }
        ///// <summary>
        ///// 如果字符串为string.Empty || null || "" 返回defaultValue 否则原字符串返回
        ///// </summary>
        ///// <param name="str">string扩展</param>
        ///// <param name="defaultValue">默认值</param>
        ///// <returns></returns>
        //public static string IsNullEmpty(this string str, string defaultValue) {
        //    return str.IsNullEmpty() ? defaultValue : str;
        //}
        /// <summary>
        /// 防止JS HTML代码被执行 "" &lt; > \n &amp; 空格
        /// </summary>
        /// <param name="htmlStr">string扩展</param>
        /// <returns></returns>
        public static string UnHtml(this string htmlStr) {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            return htmlStr.Replace("\"", "\\\"").ShowXmlHtml().Replace(" ", "&nbsp;").Replace("\n", "<br />");
        }
        /// <summary>
        /// 防止JS HTML代码被执行 "" &lt; > &amp; 空格 无\n
        /// </summary>
        /// <param name="htmlStr">string扩展</param>
        /// <returns></returns>
        public static string UnHtmlNoBR(this string htmlStr) {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            return htmlStr.Replace("\"", "\\\"").ShowXmlHtml().Replace(" ", "&nbsp;");
        }
        /// <summary>
        /// 转换为合法的XML文件
        /// </summary>
        /// <param name="htmlStr">string扩展</param>
        /// <returns></returns>
        public static string ShowXmlHtml(this string htmlStr) {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            string str = htmlStr.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
            return str;
        }
        /// <summary>
        /// 过滤JS事件
        /// </summary>
        /// <param name="htmlStr">string扩展</param>
        /// <returns></returns>
        public static string ShowHtml(this string htmlStr) {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            string str = htmlStr;
            str = Regex.Replace(str, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "_$1.$2", RegexOptions.IgnoreCase);
            str = new Regex("<script", RegexOptions.IgnoreCase).Replace(str, "<_script");
            str = new Regex("<object", RegexOptions.IgnoreCase).Replace(str, "<_object");
            str = new Regex("javascript:", RegexOptions.IgnoreCase).Replace(str, "_javascript:");
            str = new Regex("vbscript:", RegexOptions.IgnoreCase).Replace(str, "_vbscript:");
            str = new Regex("expression", RegexOptions.IgnoreCase).Replace(str, "_expression");
            str = new Regex("@import", RegexOptions.IgnoreCase).Replace(str, "_@import");
            str = new Regex("<iframe", RegexOptions.IgnoreCase).Replace(str, "<_iframe");
            str = new Regex("<frameset", RegexOptions.IgnoreCase).Replace(str, "<_frameset");
            str = Regex.Replace(str, @"(\<|\s+)o([a-z]+\s?=)", "$1_o$2", RegexOptions.IgnoreCase);
            str = new Regex(@" (on[a-zA-Z ]+)=", RegexOptions.IgnoreCase).Replace(str, " _$1=");
            return str;
        }
        /// <summary>
        /// UrlEncode
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string UrlEncode(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            return HttpUtility.UrlEncode(str);
        }
        /// <summary>
        /// UrlUpperEncode
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string UrlUpperEncode(this string str) {
            str = str.UrlEncode();
            if (str.IsNullEmpty()) return str;
            string[] list = str.Split("%");
            StringBuilder sbHtml = new StringBuilder();
            foreach (string s in list) {
                if (s.IsNullEmpty() || s.Length < 2) continue;
                sbHtml.Append("%").Append(s.Left(2).ToUpper());
                if (s.Length > 2) sbHtml.Append(s.Right(s.Length - 2));
            }
            return sbHtml.ToString();
        }
        /// <summary>
        /// UrlEncode
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string UrlEncode2(this string value) {
            StringBuilder result = new StringBuilder();
            string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            foreach (char symbol in value) {
                if (unreservedChars.IndexOf(symbol) != -1) {
                    result.Append(symbol);
                } else {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// UrlEncode
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlEncode(this string str, Encoding encoding) {
            if (str.IsNullEmpty()) return string.Empty;
            return HttpUtility.UrlEncode(str, encoding);
        }
        /// <summary>
        /// UrlDecode
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string UrlDecode(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            return HttpUtility.UrlDecode(str);
        }
        /// <summary>
        /// UrlDecode
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlDecode(this string str, Encoding encoding) {
            if (str.IsNullEmpty()) return string.Empty;
            return HttpUtility.UrlDecode(str, encoding);
        }
        /// <summary>
        /// UrlEncodeUnicode等同于JS的escape()
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string UrlEncodeUnicode(this string str) {
            return HttpUtility.UrlEncodeUnicode(str);
        }
        /// <summary>
        /// UrlPathEncode
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string UrlPathEncode(this string str) {
            return HttpUtility.UrlPathEncode(str);
        }
        /// <summary>
        /// 下载时Encode文件名称
        /// </summary>
        /// <param name="fileName">源文件名</param>
        /// <returns></returns>
        public static string EncodeFileName(this string fileName) {
            foreach (string s in Request2.Get("ALL_RAW").Split("\r\n")) {
                if (s.ToLower().StartsWith("user-agent")) {
                    if (s.ToLower().IndexOf("msie") != -1) {
                        return HttpUtility.UrlEncode(fileName);
                    } else {
                        break;
                    }
                }
            }
            return fileName;
        }
        /// <summary>
        /// HtmlEncode
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string HtmlEncode(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            return HttpUtility.HtmlEncode(str);
        }
        /// <summary>
        /// HtmlDecode
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string HtmlDecode(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            return HttpUtility.HtmlDecode(str);
        }
        /// <summary>
        /// 中文字符串长度
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static int CnLength(this string str) {
            if (str.IsNullEmpty()) return 0;
            return Encoding.Default.GetBytes(str).Length;
        }
        /// <summary>
        /// 取指定长度的字符串 中文字符占2个字符长度
        /// </summary>
        /// <param name="strInput">string扩展</param>
        /// <param name="len">长度</param>
        /// <param name="flg">后缀</param>
        /// <returns></returns>
        public static string SubString(this string strInput, int len, string flg) {
            if (strInput.IsNullEmpty()) return string.Empty;
            string myResult = string.Empty;
            if (len >= 0) {
                byte[] bsSrcString = Encoding.Default.GetBytes(strInput);
                if (bsSrcString.Length > len) {
                    int nRealLength = len;
                    int[] anResultFlag = new int[len];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = 0; i < len; i++) {
                        if (bsSrcString[i] > 127) {
                            nFlag++;
                            if (nFlag == 3) nFlag = 1;
                        } else nFlag = 0;
                        anResultFlag[i] = nFlag;
                    }
                    if ((bsSrcString[len - 1] > 127) && (anResultFlag[len - 1] == 1)) nRealLength = len + 1;
                    bsResult = new byte[nRealLength];
                    Array.Copy(bsSrcString, bsResult, nRealLength);
                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + (len >= strInput.CnLength() ? "" : flg);
                } else myResult = strInput;
            }
            return myResult;
        }
        /// <summary>
        /// 取文件扩展名 有.
        /// </summary>
        /// <param name="filename">string扩展</param>
        /// <returns></returns>
        public static string GetExtension(this string filename) {
            return System.IO.Path.GetExtension(filename);
        }
        /// <summary>
        /// 修改扩展名 有.
        /// </summary>
        /// <param name="filename">string扩展</param>
        /// <param name="ext">扩展名</param>
        /// <returns></returns>
        public static string ChangeExtension(this string filename, string ext) {
            return System.IO.Path.ChangeExtension(filename, ext);
        }
        /// <summary>
        /// 取文件名 有扩展名
        /// </summary>
        /// <param name="filename">string扩展</param>
        /// <returns></returns>
        public static string GetFileName(this string filename) {
            return System.IO.Path.GetFileName(filename);
        }
        /// <summary>
        /// 取文件名 无扩展名
        /// </summary>
        /// <param name="filename">string扩展</param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(this string filename) {
            return System.IO.Path.GetFileNameWithoutExtension(filename);
        }
        /// <summary>
        /// 取URL中的文件名 有扩展名
        /// </summary>
        /// <param name="url">string扩展</param>
        /// <returns></returns>
        public static string GetUrlFileName(this string url) {
            if (url.IsNullEmpty()) return string.Empty;
            string[] strs1 = url.Split(new char[] { '/' });
            return strs1[strs1.Length - 1].Split(new char[] { '?' })[0];
        }
        /// <summary>
        /// 取 href="中的连接">
        /// </summary>
        /// <param name="HtmlCode">string扩展</param>
        /// <returns></returns>
        public static IList<string> GetHref(this string HtmlCode) {
            IList<string> MatchVale = new List<string>();
            if (HtmlCode.IsNullEmpty()) return MatchVale;
            string Reg = @"(h|H)(r|R)(e|E)(f|F) *= *('|"")?((\w|\\|\/|\.|:|-|_)+)('|""| *|>)?";
            foreach (Match m in Regex.Matches(HtmlCode, Reg)) {
                MatchVale.Add((m.Value).ToLower().Replace("href=", "").Trim().TrimEnd('\'').TrimEnd('"').TrimStart('\'').TrimStart('"'));
            }
            return MatchVale;
        }
        /// <summary>
        /// 取 src="中的连接">
        /// </summary>
        /// <param name="HtmlCode">string扩展</param>
        /// <returns></returns>
        public static IList<string> GetSrc(this string HtmlCode) {
            IList<string> MatchVale = new List<string>();
            if (HtmlCode.IsNullEmpty()) return MatchVale;
            string Reg = @"(s|S)(r|R)(c|C) *= *('|"")?((\w|\\|\/|\.|:|-|_)+)('|""| *|>)?";
            foreach (Match m in Regex.Matches(HtmlCode, Reg)) {
                MatchVale.Add((m.Value).ToLower().Replace("src=", "").Trim().TrimEnd('\'').TrimEnd('"').TrimStart('\'').TrimStart('"'));
            }
            return MatchVale;
        }
        /// <summary>
        /// 取EMAIL地址中的@163.com 有@
        /// </summary>
        /// <param name="strEmail">string扩展</param>
        /// <returns></returns>
        public static string GetEmailHostName(this string strEmail) {
            if (strEmail.IsNullEmpty() || strEmail.IndexOf("@") < 0) return string.Empty;
            return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
        }
        /// <summary>
        /// 字符串转日期
        /// </summary>
        /// <param name="DateTimeStr">string扩展</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string DateTimeStr) {
            if (DateTimeStr.IsNullEmpty()) return DateTime.Now;
            return DateTime.Parse(DateTimeStr);
        }
        /// <summary>
        /// 字符串转日期字符串
        /// </summary>
        /// <param name="fDateTime">string扩展</param>
        /// <param name="formatStr">format</param>
        /// <returns></returns>
        public static string ToDateTimeString(this string fDateTime, string formatStr) {
            DateTime s = fDateTime.ToDateTime();
            return s.ToString(formatStr);
        }
        /// <summary>
        /// 字符串转日期字符串
        /// </summary>
        /// <param name="date">string扩展</param>
        /// <param name="format">format</param>
        /// <param name="format2">format</param>
        /// <returns></returns>
        public static string ToDateTimeString(this string date, string format, string format2 = "") {
            if (format2.IsNullEmpty()) format2 = format;
            return DateTime.ParseExact(date, format, System.Globalization.CultureInfo.CurrentCulture).ToString(format2);
        }
        /// <summary>
        /// 字符串转日期
        /// </summary>
        /// <param name="DateTimeStr">string扩展</param>
        /// <param name="defDate">默认值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string DateTimeStr, DateTime defDate) {
            DateTime.TryParse(DateTimeStr, out defDate);
            return defDate;
        }
        /// <summary>
        /// 字符串转日期
        /// </summary>
        /// <param name="DateTimeStr">string扩展</param>
        /// <param name="defDate">默认值</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string DateTimeStr, DateTime? defDate) {
            DateTime dt = DateTime.Now;
            DateTime dt2 = dt;
            DateTime.TryParse(DateTimeStr, out dt);
            if (dt == dt2) return defDate;
            return dt;
        }
        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value) {
            return value.ToBytes(null);
        }
        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="encoding">Encoding</param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value, Encoding encoding) {
            if (value.IsNullEmpty()) return null;
            encoding = (encoding ?? Encoding.UTF8);
            return encoding.GetBytes(value);
        }
        /// <summary>
        /// 字符串转字节 UTF8
        /// </summary>
        /// <param name="valueToExpand">string扩展</param>
        /// <returns></returns>
        public static byte[] ToUTF8Bytes(this string valueToExpand) {
            return Encoding.UTF8.GetBytes(valueToExpand);
        }
        /// <summary>
        /// 删除HTML标记
        /// </summary>
        /// <param name="HtmlCode">string扩展</param>
        /// <returns></returns>
        public static string RemoveHTML(this string HtmlCode) {
            if (HtmlCode.IsNullEmpty()) return string.Empty;
            string MatchVale = HtmlCode;
            MatchVale = new Regex("<br>", RegexOptions.IgnoreCase).Replace(MatchVale, "\n");
            foreach (Match s in Regex.Matches(HtmlCode, "<[^{><}]*>")) { MatchVale = MatchVale.Replace(s.Value, ""); }//"(<[^{><}]*>)"//@"<[\s\S-! ]*?>"//"<.+?>"//<(.*)>.*<\/\1>|<(.*) \/>//<[^>]+>//<(.|\n)+?>
            MatchVale = new Regex("\n", RegexOptions.IgnoreCase).Replace(MatchVale, "<br>");
            return MatchVale;
        }
        /// <summary>
        /// 删除HTML标记
        /// </summary>
        /// <param name="content">string扩展</param>
        /// <returns></returns>
        public static string RemoveAllHTML(this string content) {
            if (content.IsNullEmpty()) return string.Empty;
            string pattern = "<[^>]*>";
            return Regex.Replace(content, pattern, string.Empty, RegexOptions.IgnoreCase);
        }
#if MONO40
#elif MONO20
#else
		/// <summary>
        /// 转简体中文
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string ToSChinese(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);
        }
        /// <summary>
        /// 转繁体中文
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string ToTChinese(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0);
        }
        /// <summary>
        /// 取文件字符串的MIME
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns>取文件字符串的MIME</returns>
        public static string GetMimeType(this string str) {
            return Encoding.UTF8.GetBytes(str).GetMimeType();
        }
#endif
        ///// <summary>
        ///// 计算表达示值
        ///// </summary>
        ///// <param name="ExprStr">string扩展</param>
        ///// <returns></returns>
        //public static double Evel(this string ExprStr) {
        //    if (ExprStr.IsNullEmpty()) return 0;
        //    Expr expression = new Expr("return " + ExprStr + ";");
        //    return expression.Compute(0);
        //}
        /// <summary>
        /// 字符倒排列
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <returns></returns>
        public static string Reverse(this string value) {
            if (value.IsNullEmpty()) return string.Empty;

            var chars = value.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
        /// <summary>
        /// 过滤脏话
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="word">如：(TMD|MB)</param>
        /// <returns></returns>
        public static string Filter(this string str, string word = "") {
            if (str.IsNullEmpty()) return string.Empty;
            string k = word.IsNullEmpty() ? BANWORD : word;
            str = new Regex(k, RegexOptions.IgnoreCase).Replace(str, "*");
            return str;
        }
        /// <summary>
        /// UBB
        /// </summary>
        /// <param name="chr">string扩展</param>
        /// <returns></returns>
        public static string UBB(this string chr) {
            if (chr.IsNullEmpty()) return string.Empty;
            chr = chr.HtmlEncode();
            chr = Regex.Replace(chr, @"<script(?<x>[^\>]*)>(?<y>[^\>]*)            \</script\>", @"&lt;script$1&gt;$2&lt;/script&gt;", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[url=(?<x>[^\]]*)\](?<y>[^\]]*)\[/url\]", @"<a href=$1  target=_blank>$2</a>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[url\](?<x>[^\]]*)\[/url\]", @"<a href=$1 target=_blank>$1</a>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[email=(?<x>[^\]]*)\](?<y>[^\]]*)\[/email\]", @"<a href=$1>$2</a>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[email\](?<x>[^\]]*)\[/email\]", @"<a href=$1>$1</a>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[flash](?<x>[^\]]*)\[/flash]", @"<OBJECT codeBase=http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=4,0,2,0 classid=clsid:D27CDB6E-AE6D-11cf-96B8-444553540000 width=500 height=400><PARAM NAME=movie VALUE=""$1""><PARAM NAME=quality VALUE=high><embed src=""$1"" quality=high pluginspage='http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash' type='application/x-shockwave-flash' width=500 height=400>$1</embed></OBJECT>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[img](?<x>[^\]]*)\[/img]", @"<IMG SRC=""$1"" border=0>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[color=(?<x>[^\]]*)\](?<y>[^\]]*)\[/color\]", @"<font color=$1>$2</font>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[face=(?<x>[^\]]*)\](?<y>[^\]]*)\[/face\]", @"<font face=$1>$2</font>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[size=1\](?<x>[^\]]*)\[/size\]", @"<font size=1>$1</font>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[size=2\](?<x>[^\]]*)\[/size\]", @"<font size=2>$1</font>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[size=3\](?<x>[^\]]*)\[/size\]", @"<font size=3>$1</font>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[size=4\](?<x>[^\]]*)\[/size\]", @"<font size=4>$1</font>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[align=(?<x>[^\]]*)\](?<y>[^\]]*)\[/align\]", @"<align=$1>$2</align>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[fly](?<x>[^\]]*)\[/fly]", @"<marquee width=90% behavior=alternate scrollamount=3>$1</marquee>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[move](?<x>[^\]]*)\[/move]", @"<marquee scrollamount=3>$1</marquee>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[glow=(?<x>[^\]]*),(?<y>[^\]]*),(?<z>[^\]]*)\](?<w>[^\]]*)\[/glow\]", @"<table width=$1 style='filter:glow(color=$2, strength=$3)'>$4</table>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[shadow=(?<x>[^\]]*),(?<y>[^\]]*),(?<z>[^\]]*)\](?<w>[^\]]*)\[/shadow\]", @"<table width=$1 style='filter:shadow(color=$2, strength=$3)'>$4</table>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[b\](?<x>[^\]]*)\[/b\]", @"<b>$1</b>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[i\](?<x>[^\]]*)\[/i\]", @"<i>$1</i>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[u\](?<x>[^\]]*)\[/u\]", @"<u>$1</u>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[code\](?<x>[^\]]*)\[/code\]", @"<pre id=code><font size=1 face='Verdana, Arial' id=code>$1</font id=code></pre id=code>", RegexOptions.IgnoreCase);

            chr = Regex.Replace(chr, @"\[list\](?<x>[^\]]*)\[/list\]", @"<ul>$1</ul>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[list=1\](?<x>[^\]]*)\[/list\]", @"<ol type=1>$1</ol id=1>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[list=a\](?<x>[^\]]*)\[/list\]", @"<ol type=a>$1</ol id=a>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[\*\](?<x>[^\]]*)\[/\*\]", @"<li>$1</li>", RegexOptions.IgnoreCase);
            chr = Regex.Replace(chr, @"\[quote](?<x>.*)\[/quote]", @"<center>—— 以下是引用 ——<table border='1' width='80%' cellpadding='10' cellspacing='0' ><tr><td>$1</td></tr></table></center>", RegexOptions.IgnoreCase);
            return (chr);
        }
        /// <summary>
        /// ClearUBB
        /// </summary>
        /// <param name="sDetail">string扩展</param>
        /// <returns></returns>
        public static string ClearUBB(this string sDetail) {
            return Regex.Replace(sDetail, @"\[[^\]]*?\]", string.Empty, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="args">值</param>
        /// <returns></returns>
        public static string FormatWith(this string str, params object[] args) {
            return string.Format(str, args);
        }
        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="format">string扩展</param>
        /// <param name="args">值</param>
        /// <returns></returns>
        public static string AppendFormat(this string format, params object[] args) {
            return new StringBuilder().AppendFormat(format, args).ToString();
        }
        /// <summary>
        /// 字符串联接
        /// </summary>
        /// <param name="args">值</param>
        /// <returns></returns>
        public static string ConcatWith(params object[] args) {
            return string.Concat(args);
        }
        //public static string FormatWith2(this string format, params object[] args) {     
        //    if (format.IsNull() || args.IsNull()) throw new ArgumentNullException((format.IsNull()) ? "format" : "args");
        //    int capacity = format.Length + args.Where(p => p.IsNotNull()).Select(p => p.ToString()).Sum(p => p.Length);     
        //    return new StringBuilder(capacity).AppendFormat( format, args).ToString(); 
        //}
        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="text">string扩展</param>
        /// <param name="arg0">值</param>
        /// <returns></returns>
        public static string FormatWith(this string text, object arg0) {
            return string.Format(text, arg0);
        }
        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="text">string扩展</param>
        /// <param name="arg0">值</param>
        /// <param name="arg1">值</param>
        /// <returns></returns>
        public static string FormatWith(this string text, object arg0, object arg1) {
            return string.Format(text, arg0, arg1);
        }
        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="text">string扩展</param>
        /// <param name="arg0">值</param>
        /// <param name="arg1">值</param>
        /// <param name="arg2">值</param>
        /// <returns></returns>
        public static string FormatWith(this string text, object arg0, object arg1, object arg2) {
            return string.Format(text, arg0, arg1, arg2);
        }
        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="text">string扩展</param>
        /// <param name="provider">IFormatProvider</param>
        /// <param name="args">值</param>
        /// <returns></returns>
        public static string FormatWith(this string text, IFormatProvider provider, params object[] args) {
            return string.Format(provider, text, args);
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="replaceValue">替换值</param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue) {
            if (value.IsNullEmpty()) return string.Empty;
            return ReplaceWith(value, regexPattern, replaceValue, RegexOptions.None);
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="replaceValue">替换值</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue, RegexOptions options) {
            if (value.IsNullEmpty()) return string.Empty;
            return Regex.Replace(value, regexPattern, replaceValue, options);
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="evaluator">MatchEvaluator</param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, MatchEvaluator evaluator) {
            return ReplaceWith(value, regexPattern, RegexOptions.None, evaluator);
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="options">选项</param>
        /// <param name="evaluator">MatchEvaluator</param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, RegexOptions options, MatchEvaluator evaluator) {
            if (value.IsNullEmpty()) return string.Empty;
            return Regex.Replace(value, regexPattern, evaluator, options);
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="ReplaceString">替换</param>
        /// <param name="IsCaseInsensetive">不区分大小写</param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, string ReplaceString, bool IsCaseInsensetive) {
            if (value.IsNullEmpty()) return string.Empty;
            return Regex.Replace(value, regexPattern, ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="RegValue">string扩展</param>
        /// <param name="regStart">开始</param>
        /// <param name="regEnd">结束</param>
        /// <returns></returns>
        public static string Replace(this string RegValue, string regStart, string regEnd) {
            if (RegValue.IsNullEmpty()) return string.Empty;
            string s = RegValue;
            if (RegValue != "" && RegValue.IsNotNull()) {
                if (regStart != "" && regStart.IsNotNull()) { s = s.Replace(regStart, ""); }
                if (regEnd != "" && regEnd.IsNotNull()) { s = s.Replace(regEnd, ""); }
            }
            return s;
        }
        /// <summary>
        /// 正则取值 to MatchCollection
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <returns></returns>
        public static MatchCollection GetMatches(this string value, string regexPattern) {
            if (value.IsNullEmpty()) return null;
            return GetMatches(value, regexPattern, RegexOptions.None);
        }
        /// <summary>
        /// 正则取值 to MatchCollection
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public static MatchCollection GetMatches(this string value, string regexPattern, RegexOptions options) {
            if (value.IsNullEmpty()) return null;
            return Regex.Matches(value, regexPattern, options);
        }
        /// <summary>
        /// 正则取值 to MatchCollection
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="startString">开始</param>
        /// <param name="endString">结束</param>
        /// <returns></returns>
        public static MatchCollection FindBetween(this string s, string startString, string endString) {
            return s.FindBetween(startString, endString, true);
        }
        /// <summary>
        /// 正则取值 to MatchCollection
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="startString">开始</param>
        /// <param name="endString">结束</param>
        /// <param name="recursive">递归</param>
        /// <returns></returns>
        public static MatchCollection FindBetween(this string s, string startString, string endString, bool recursive) {
            if (s.IsNullEmpty()) return null;
            MatchCollection matches;
            startString = Regex.Escape(startString);
            endString = Regex.Escape(endString);
            Regex regex = new Regex("(?<=" + startString + ").*(?=" + endString + ")");
            matches = regex.Matches(s);
            if (!recursive) return matches;
            if (matches.Count > 0) {
                if (matches[0].ToString().IndexOf(Regex.Unescape(startString)) > -1) {
                    s = matches[0].ToString() + Regex.Unescape(endString);
                    return s.FindBetween(Regex.Unescape(startString), Regex.Unescape(endString));
                } else {
                    return matches;
                }
            } else {
                return matches;
            }
        }
        /// <summary>
        /// 正则取值 to list
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <returns></returns>
        public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern) {
            return GetMatchingValues(value, regexPattern, RegexOptions.None);
        }
        /// <summary>
        /// 正则取值 to list
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern, RegexOptions options) {
            foreach (Match match in GetMatches(value, regexPattern, options)) {
                if (match.Success) yield return match.Value;
            }
        }
        /// <summary>
        /// 正则取值 to list
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="rep1">替换1</param>
        /// <param name="rep2">替换2</param>
        /// <returns></returns>
        public static IList<string> GetMatchingValues(this string value, string regexPattern, string rep1, string rep2) {
            IList<string> txtTextArr = new List<string>();
            if (value.IsNullEmpty()) return txtTextArr;
            string MatchVale = "";
            foreach (Match m in Regex.Matches(value, regexPattern)) {
                MatchVale = m.Value.Replace(rep1, "").Replace(rep2, "").Trim();
                txtTextArr.Add(MatchVale);
            }
            return txtTextArr;
        }
        /// <summary>
        /// 正则取值
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="rep1">替换1</param>
        /// <param name="rep2">替换2</param>
        /// <returns></returns>
        public static string GetMatchingValue(this string value, string regexPattern, string rep1, string rep2) {
            if (value.IsNullEmpty()) return string.Empty;
            value = Regex.Match(value, regexPattern).Value;
            if (value.IsNullEmpty()) return string.Empty;
            return value.Replace(rep1, "").Replace(rep2, "").Trim();
        }
        /// <summary>
        /// 正则取值
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="left">左长</param>
        /// <param name="right">右长</param>
        /// <returns></returns>
        public static string GetMatchingValue(this string value, string regexPattern, int left, int right) {
            if (value.IsNullEmpty()) return string.Empty;
            value = Regex.Match(value, regexPattern).Value;
            if (value.IsNullEmpty()) return string.Empty;
            if (left > 0) value = value.Remove(0, left);
            if (right > 0) value = value.Remove(value.Length - right, right);
            return value.Trim();
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public static string[] Split(this string value, string regexPattern, RegexOptions options) {
            if (value.IsNullEmpty()) return new string[] { };
            return Regex.Split(value, regexPattern, options);
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="regexPattern">正则</param>
        /// <returns></returns>
        public static string[] Split(this string value, string regexPattern) {
            return value.Split(regexPattern, RegexOptions.None);
        }
#if !NET20
        /// <summary>
        /// xml字符串转 XDocument/XmlDocument/XPathNavigator
        /// </summary>
        /// <param name="xml">string扩展</param>
        /// <returns></returns>
        public static XDocument ToXDocument(this string xml) {
            return XDocument.Parse(xml);
        }
#endif
        /// <summary>
        /// xml字符串转 XmlDocument
        /// </summary>
        /// <param name="xml">string扩展</param>
        /// <returns></returns>
        public static XmlDocument ToXmlDOM(this string xml) {
            var document = new XmlDocument();
            document.LoadXml(xml);
            return document;
        }
        /// <summary>
        /// xml字符串转 XPathNavigator
        /// </summary>
        /// <param name="xml">string扩展</param>
        /// <returns></returns>
        public static XPathNavigator ToXPath(this string xml) {
            var document = new XPathDocument(new StringReader(xml));
            return document.CreateNavigator();
        }
        /// <summary>
        /// 转拼音
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <returns></returns>
        public static string ToPinyin(this string s) {
            if (s.IsNullEmpty()) return string.Empty;
            return PinYin.Instance().Search(s).ToLower();
        }
        /// <summary>
        /// 转拼音首字母
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <returns></returns>
        public static string ToPinyinChar(this string s) {
            if (s.IsNullEmpty()) return string.Empty;
            string strVal = PinYin.Instance().SearchCap(s);
            if (strVal.ToLower() == strVal.ToUpper()) return "*"; else return strVal.ToLower();
        }
        /// <summary>
        /// 转拼音首字母
        /// </summary>
        /// <param name="c">string扩展</param>
        /// <returns></returns>
        public static string ToPinyinChar2(this string c) {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "g";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";
            return "*";
        }
        /// <summary>
        /// 左截字符串
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="length">长</param>
        /// <returns></returns>
        public static string Left(this string str, int length) {
            if (length <= 0 || str.Length == 0) return string.Empty;
            if (str.Length <= length) return str;
            return str.Substring(0, length);
        }
        /// <summary>
        /// 右截字符串
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="length">长</param>
        /// <returns></returns>
        public static string Right(this string str, int length) {
            if (length <= 0 || str.Length == 0) return string.Empty;
            if (str.Length <= length) return str;
            return str.Substring(str.Length - length, length);
        }
        /// <summary>
        /// Json特符字符过滤，参见http://www.json.org/
        /// </summary>
        /// <param name="sourceStr">要过滤的源字符串</param>
        /// <returns>返回过滤的字符串</returns>
        public static string JsonEscape(string sourceStr) {
            var builder = new StringBuilder();
            foreach (var c in sourceStr)
                switch (c) {
                    case '\b': builder.Append(@"\b"); break;
                    case '\f': builder.Append(@"\f"); break;
                    case '\n': builder.Append(@"\n"); break;
                    case '\r': builder.Append(@"\r"); break;
                    case '\t': builder.Append(@"\t"); break;
                    case '\v': builder.Append(@"\v"); break;
                    case '\'': builder.Append(@"\'"); break;
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append(@"\\"); break;
                    default: if (c <= '\u001f') { builder.Append("\\u"); builder.Append(((int)c).ToString("x4")); } else builder.Append(c); break;
                }
            return builder.ToString();
        }
        /// <summary>
        /// 转枚举类型 如 public enum test { test1, test2 } Msg.WriteEnd("0".ToEnum&lt;test>()); 值是test1
        /// </summary>
        /// <example>
        /// <code>
        /// public enum test { test1, test2 } 
        /// Msg.WriteEnd("0".ToEnum&lt;test>()); //值是test1
        /// </code>
        /// </example>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="value">string扩展</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value) {
            return ToEnum<T>(value, false);
        }
        /// <summary>
        /// 转枚举类型
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="value">string扩展</param>
        /// <param name="ignorecase">大小写</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, bool ignorecase) {
            if (value.IsNull()) throw new ArgumentNullException("value不能为空");
            value = value.Trim();
            if (value.Length == 0) throw new ArgumentNullException("value不是有效Enum字符串", "value");
            Type t = typeof(T);
            if (!t.IsEnum) throw new ArgumentException("Type不是有效Enum", "T");
            return (T)Enum.Parse(t, value, ignorecase);
        }
        /// <summary>
        /// 转枚举类型
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="value">string扩展</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ToEnum<T>(string value, T defaultValue) where T : struct, IConvertible { if (!typeof(T).IsEnum) throw new ArgumentException("T必须是Enum类型"); if (string.IsNullOrEmpty(value)) return defaultValue; foreach (T item in Enum.GetValues(typeof(T))) { if (item.ToString().ToLower().Equals(value.Trim().ToLower())) return item; } return defaultValue; }
        /// <summary>
        /// 获取枚举所有成员名称
        /// </summary>
        /// <typeparam name="T">枚举名,比如Enum1</typeparam>
        public static string[] GetEnumNames<T>() {
            return System.Enum.GetNames(typeof(T));
        }
        /// <summary>
        /// 字符存在数量
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="character">字符</param>
        /// <returns></returns>
        public static int CharacterCount(this string value, char character) {
            int intReturnValue = 0;
            if (value.IsNullEmpty()) return 0;
            for (int intCharacter = 0; intCharacter <= (value.Length - 1); intCharacter++) {
                if (value.Substring(intCharacter, 1) == character.ToString()) intReturnValue += 1;
            }

            return intReturnValue;
        }
        /// <summary>
        /// 加前缀
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public static string ForcePrefix(this string s, string prefix) {
            if (s.IsNullEmpty()) return string.Empty;
            string result = s;
            if (!result.StartsWith(prefix)) result = prefix + result;
            return result;
        }
        /// <summary>
        /// 加后缀
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string ForceSuffix(this string s, string suffix) {
            if (s.IsNullEmpty()) return string.Empty;
            string result = s;
            if (!result.EndsWith(suffix)) result += suffix;
            return result;
        }
        /// <summary>
        /// 删除前缀
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public static string RemovePrefix(this string s, string prefix) {
            if (s.IsNullEmpty()) return string.Empty;
            return Regex.Replace(s, "^" + prefix, System.String.Empty, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 删除后缀
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string RemoveSuffix(this string s, string suffix) {
            if (s.IsNullEmpty()) return string.Empty;
            return Regex.Replace(s, suffix + "$", System.String.Empty, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 在右边补字符串
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="pad">附加</param>
        /// <returns></returns>
        public static string PadLeft(this string s, string pad) {
            return s.PadLeft(pad, s.Length + pad.Length, false);
        }
        /// <summary>
        /// 在右边补字符串
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="pad">附加</param>
        /// <param name="totalWidth">宽</param>
        /// <param name="cutOff">是否剪切</param>
        /// <returns></returns>
        public static string PadLeft(this string s, string pad, int totalWidth, bool cutOff) {
            if (s.IsNullEmpty()) return string.Empty;
            if (s.Length >= totalWidth) return s;
            int padCount = pad.Length;
            string paddedString = s;
            while (paddedString.Length < totalWidth) paddedString += pad;
            if (cutOff) paddedString = paddedString.Substring(0, totalWidth);
            return paddedString;
        }
        /// <summary>
        /// 在左边补字符串
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="pad">附加</param>
        /// <returns></returns>
        public static string PadRight(this string s, string pad) {
            return PadRight(s, pad, s.Length + pad.Length, false);
        }
        /// <summary>
        /// 在左边补字符串
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <param name="pad">附加</param>
        /// <param name="length">长</param>
        /// <param name="cutOff">是否剪切</param>
        /// <returns></returns>
        public static string PadRight(this string s, string pad, int length, bool cutOff) {
            if (s.IsNullEmpty()) return string.Empty;
            if (s.Length >= length) return s;
            string paddedString = string.Empty;
            while (paddedString.Length < length - s.Length) paddedString += pad;
            if (cutOff) paddedString = paddedString.Substring(0, length - s.Length);
            paddedString += s;
            return paddedString;
        }
        /// <summary>
        /// 字符串转颜色 如"ffffff".ToColor()
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <returns></returns>
        public static Color ToColor(this string s) {
            if (s.IsNullEmpty()) return new Color() { };
            s = s.Replace("#", string.Empty);
            byte a = System.Convert.ToByte("ff", 16);
            byte pos = 0;
            if (s.Length == 8) {
                a = System.Convert.ToByte(s.Substring(pos, 2), 16);
                pos = 2;
            }
            byte r = System.Convert.ToByte(s.Substring(pos, 2), 16);
            pos += 2;
            byte g = System.Convert.ToByte(s.Substring(pos, 2), 16);
            pos += 2;
            byte b = System.Convert.ToByte(s.Substring(pos, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// 字符串数组中是否包含 value
        /// </summary>
        /// <param name="value">string扩展</param>
        /// <param name="keywords">字符串</param>
        /// <returns></returns>
        public static bool ContainsArray(this string value, params string[] keywords) {
            return keywords.All((s) => value.Contains(s));
        }
        /// <summary>
        /// 字符串转T类型的NULL 如 "123".ToNullable&lt;long>() 可为null
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="s">string扩展</param>
        /// <returns></returns>
        public static Nullable<T> ToNullable<T>(this string s) where T : struct {
            T? result = null;
            if (!s.Trim().IsNullEmpty()) {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T?));
                result = (T?)converter.ConvertFrom(s);
            }
            return result;
        }
        /// <summary>
        /// 字符串转T类型的NULL 如 0.ToNullable&lt;long>() 可为null
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T? ToNullable<T>(this T value) where T : struct {
            return (value.IsDefault<T>() ? null : (T?)value);
        }
        /// <summary>
        /// 按行分隔字符串
        /// </summary>
        /// <param name="text">string扩展</param>
        /// <returns></returns>
        public static List<string> GetLines(this string text) {
            return text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }
        /// <summary>
        /// 正则匹配否
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="op">正则</param>
        /// <returns></returns>
        public static bool IsMatch(this string str, string op) {
            if (str.IsNullEmpty()) return false;
            Regex re = new Regex(op, RegexOptions.IgnoreCase);
            return re.IsMatch(str);
        }
        /// <summary>
        /// IP否
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsIP(this string input) {
            return input.IsMatch(@"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"); //@"^(([01]?[\d]{1,2})|(2[0-4][\d])|(25[0-5]))(\.(([01]?[\d]{1,2})|(2[0-4][\d])|(25[0-5]))){3}$";
        }
        public static bool IsIPPort(this string input) {
            return new Regex(@"^(((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?):([0-9]*))$", RegexOptions.IgnoreCase).IsMatch(input);
        }
        /// <summary>
        /// IP6否
        /// </summary>
        /// <param name="ip">string扩展</param>
        /// <returns></returns>
        public static bool IsIPSect(this string ip) {
            return ip.IsMatch(@"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }
        /// <summary>
        /// IsNumber
        /// </summary>
        /// <param name="strNumber">string扩展</param>
        /// <returns></returns>
        public static bool IsNumber(this string strNumber) {
            string pet = @"^([0-9])[0-9]*(\.\w*)?$"; //^-[0-9]+$|^[0-9]+$
            return strNumber.IsMatch(pet);
        }
        /// <summary>
        /// IsDouble
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsDouble(this string input) {
            string pet = @"^[0-9]*[1-9][0-9]*$";//@"^\d{1,}$"//整数校验常量//@"^-?(0|\d+)(\.\d+)?$"//数值校验常量 
            return input.IsMatch(pet);
        }
        /// <summary>
        /// 整数否
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsInt(this string input) {
            string pet = @"^[0-9]*$"; //@"^([0-9])[0-9]*(\.\w*)?$";
            return input.IsMatch(pet);
        }
        /// <summary>
        /// 数组里全是数字否
        /// </summary>
        /// <param name="strNumber">string扩展</param>
        /// <returns></returns>
        public static bool IsNumberArray(this string[] strNumber) {
            if (strNumber.IsNull()) return false;
            if (strNumber.Length < 1) return false;
            foreach (string id in strNumber)
                if (!id.IsNumber()) return false;
            return true;
        }
        /// <summary>
        /// 信箱否
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsEmail(this string input) {
            string pet = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";//@"^\w+((-\w+)|(\.\w+))*\@\w+((\.|-)\w+)*\.\w+$";
            return input.IsMatch(pet);
        }
        /// <summary>
        /// URL否
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsUrl(this string input) {
            string pet = @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$";//@"^http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            return input.IsMatch(pet);
        }
        /// <summary>
        /// 邮编否
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsZip(this string input) {
            return input.IsMatch(@"\d{6}");
        }
        /// <summary>
        /// 安全SQL字符串否
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static bool IsSafeSqlStr(this string str) {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
        /// <summary>
        /// 日期时间否 正则
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsDateTime(this string input) {
            //string pet = @"^(?:(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(\/|-|\.)(?:0?2\1(?:29))$)|(?:(?:1[6-9]|[2-9]\d)?\d{2})(\/|-|\.)(?:(?:(?:0?[13578]|1[02])\2(?:31))|(?:(?:0?[1,3-9]|1[0-2])\2(29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\2(?:0?[1-9]|1\d|2[0-8]))$";
            string pet = @"^(?=\d)(?:(?!(?:1582(?:\.|-|\/)10(?:\.|-|\/)(?:0?[5-9]|1[0-4]))|(?:1752(?:\.|-|\/)0?9(?:\.|-|\/)(?:0?[3-9]|1[0-3])))(?=(?:(?!000[04]|(?:(?:1[^0-6]|[2468][^048]|[3579][^26])00))(?:(?:\d\d)(?:[02468][048]|[13579][26]))\D0?2\D29)|(?:\d{4}\D(?!(?:0?[2469]|11)\D31)(?!0?2(?:\.|-|\/)(?:29|30))))(\d{4})([-\/.])(0?\d|1[012])\2((?!00)[012]?\d|3[01])(?:$|(?=\x20\d)\x20))?((?:(?:0?[1-9]|1[012])(?::[0-5]\d){0,2}(?:\x20[aApP][mM]))|(?:[01]?\d|2[0-3])(?::[0-5]\d){1,2})?$";
            return input.IsMatch(pet);
        }
        /// <summary>
        /// 日期时间否 try catch
        /// </summary>
        /// <param name="DateTimeStr">string扩展</param>
        /// <returns></returns>
        public static bool IsDateTime2(this string DateTimeStr) {
            try { DateTime _dt = DateTime.Parse(DateTimeStr); return true; } catch { return false; }
        }
        /// <summary>
        /// 日期否 try catch
        /// </summary>
        /// <param name="DateStr">string扩展</param>
        /// <returns></returns>
        public static bool IsDate(this string DateStr) {
            try { DateTime _dt = DateTime.Parse(DateStr); return true; } catch { return false; }
        }
        /// <summary>
        /// 时间否
        /// </summary>
        /// <param name="TimeStr">string扩展</param>
        /// <returns></returns>
        public static bool IsTime(this string TimeStr) {
            return TimeStr.IsMatch(@"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }
        /// <summary>
        /// 第一个字母是否a-zA-Z0-9
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsAlpha(this string input) {
            return input.IsMatch(@"[^a-zA-Z0-9]");
        }
        /// <summary>
        /// 电话否
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns>电话否</returns>
        public static bool IsTelepone(this string input) {
            return input.IsMatch(@"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");//："^(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}$
        }
        /// <summary>
        /// 手机号否
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns>手机号否</returns>
        public static bool IsMobile(this string input) {
            return input.IsMatch(@"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }
        /// <summary>
        /// 强密码否
        /// </summary>
        /// <param name="password">string扩展</param>
        /// <returns>强密码否</returns>
        public static bool IsStrongPassword(this string password) {
            return Regex.IsMatch(password, @"(?=^.{8,255}$)((?=.*\d)(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[^A-Za-z0-9])(?=.*[a-z])|(?=.*[^A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]))^.*");
        }
        /// <summary>
        /// 字符串是否在,分隔的字符串内
        /// </summary>
        /// <param name="stringarray">string扩展</param>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool IsInArray(this string stringarray, string str) {
            return stringarray.Split(",").IsInArray(str, false);
        }
        /// <summary>
        /// 字符串是否在,分隔的字符串内
        /// </summary>
        /// <param name="stringarray">string扩展</param>
        /// <param name="str">字符串</param>
        /// <param name="strsplit">分割符</param>
        /// <returns></returns>
        public static bool IsInArray(this string stringarray, string str, string strsplit) {
            return stringarray.Split(strsplit).IsInArray(str, false);
        }
        /// <summary>
        /// 字符串是否在,分隔的字符串内
        /// </summary>
        /// <param name="stringarray">string扩展</param>
        /// <param name="str">字符串</param>
        /// <param name="strsplit">分割符</param>
        /// <param name="caseInsensetive">区分大小写</param>
        /// <returns></returns>
        public static bool IsInArray(this string stringarray, string str, string strsplit, bool caseInsensetive) {
            return stringarray.Split(strsplit).IsInArray(str, caseInsensetive);
        }
        /// <summary>
        ///  BASE64否
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static bool IsBase64(this string str) {
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }
        /// <summary>
        /// 年否 >=1900 and &lt;=9999
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns></returns>
        public static bool IsYear(this string input) {
            int year = input.ToInt();
            return year >= 1900 && year <= 9999;
        }
        /// <summary>
        /// 图片否 jpg jpeg png bmp gif 按文件扩展名来比较
        /// </summary>
        /// <param name="filename">string扩展</param>
        /// <returns></returns>
        public static bool IsImgFileName(this string filename) {
            filename = filename.Trim();
            if (filename.EndsWith(".") || (filename.IndexOf(".") == -1)) return false;
            string str = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            if (((str != "jpg") && (str != "jpeg")) && ((str != "png") && (str != "bmp"))) return (str == "gif");
            return true;
        }
        /// <summary>
        /// 判断是否IMG文件 bmp/JPEG/GIF/PNG 按前几个字节比较
        /// </summary>
        /// <param name="filename">string扩展</param>
        /// <returns></returns>
        public static bool IsImgFile(this string filename) {
            if (!FileDirectory.FileExists(filename)) return false;

            ushort code = BitConverter.ToUInt16(File.ReadAllBytes(filename), 0);
            switch (code) {
                case 0x4D42://bmp
                    return true;
                case 0xD8FF://JPEG   
                    return true;
                case 0x4947://GIF   
                    return true;
                case 0x5089://PNG   
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 判断是否IMG文件 bmp/JPEG/GIF/PCX/PNG/PSD/RAS/SGI/TIFF 按前几个字节比较并支持更多格式
        /// </summary>
        /// <param name="filename">string扩展</param>
        /// <returns></returns>
        public static bool IsImgFile2(this string filename) {
            if (!FileDirectory.FileExists(filename)) return false;

            ushort code = BitConverter.ToUInt16(File.ReadAllBytes(filename), 0);
            switch (code) {
                case 0x4D42://bmp
                    return true;
                case 0xD8FF://JPEG   
                    return true;
                case 0x4947://GIF   
                    return true;
                case 0x050A://PCX   
                    return true;
                case 0x5089://PNG   
                    return true;
                case 0x4238://PSD   
                    return true;
                case 0xA659://RAS   
                    return true;
                case 0xDA01://SGI   
                    return true;
                case 0x4949://TIFF
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// GUID否
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <returns></returns>
        public static bool IsGuid(this string s) {
            if (s.IsNullEmpty()) return false;
            Regex format = new Regex("^[A-Fa-f0-9]{32}$|" +
                "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2},{0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
            Match match = format.Match(s);
            return match.Success;
        }
        /// <summary>
        /// 身份证否
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <returns></returns>
        public static bool IsCreditCard(this string s) {
            return new Regex(@"^(\d{14}|\d{17})(\d|[xX])$").IsMatch(s);
        }
        /// <summary>
        /// 判断是否是中文
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <returns></returns>
        public static bool IsCNStr(this string s) {
            string[] stringMatchs = new string[] {
                @"[\u3040-\u318f]+",
                @"[\u3300-\u337f]+",
                @"[\u3400-\u3d2d]+",
                @"[\u4e00-\u9fff]+",
                @"[\u4e00-\u9fa5]+",
                @"[\uf900-\ufaff]+"
            };
            foreach (string stringMatch in stringMatchs)
                if (Regex.IsMatch(s, stringMatch))
                    return true;
            return false;
        }
        /// <summary>
        /// IsColor
        /// </summary>
        /// <param name="color">string扩展</param>
        /// <returns></returns>
        public static bool IsColor(this string color) {
            if (color.IsNullEmpty()) return false;
            color = color.Trim().Trim('#');
            if (color.Length != 3 && color.Length != 6) return false;
            //不包含0-9  a-f以外的字符
            if (!Regex.IsMatch(color, "[^0-9a-f]", RegexOptions.IgnoreCase)) return true;
            return false;
        }
        /// <summary>
        /// 判断(E文 数字 下划线)
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static bool IsUserName(this string str) {
            return new Regex("^[a-zA-Z\\d_]+$", RegexOptions.Compiled).IsMatch(str);
        }
        /// <summary>
        /// 判断(E文 数字 中文 下划线)
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static bool IsNickName(this string str) {
            return new Regex(@"^[a-zA-Z\u4e00-\u9fa5\d_]+$", RegexOptions.Compiled).IsMatch(str);
        }
        /// <summary>
        /// 判断组名(不允许/\&lt;>{}:*?|")
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static bool IsGroupName(this string str) {
            return new Regex(@"^[^\/""{}<>:?*|]+$", RegexOptions.Compiled).IsMatch(str);
        }
        /// <summary>
        /// IsAscii > 127
        /// </summary>
        /// <param name="data">string扩展</param>
        /// <returns></returns>
        public static bool IsAscii(this string data) {
            if ((data.IsNull()) || (data.Length == 0)) return true;
            foreach (char c in data) {
                if ((int)c > 127) return false;
            }
            return true;
        }
        /// <summary>
        /// IsBinary
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static bool IsBinary(this string str) {
            for (int i = 0; i < str.Length; i++) {
                if (str[i] > '\x00ff') {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 取物理路径
        /// </summary>
        /// <param name="strPath">string扩展</param>
        /// <returns></returns>
        public static string GetMapPath(this string strPath) {
            if (HttpContext.Current.IsNotNull())
                return HttpContext.Current.Server.MapPath(strPath);
            else {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith(".\\")) strPath = strPath.Substring(2);
                strPath = strPath.TrimStart('~').TrimStart('\\');
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        /// <summary>
        /// 将虚拟路径转换为应用程序绝对路径
        /// </summary>
        /// <param name="strPath">虚拟路径</param>
        /// <returns></returns>
        public static string GetAbsolutePath(this string strPath) {
            return VirtualPathUtility.ToAbsolute(strPath);
        }
        /// <summary>
        /// 取物理路径 主要用在Global文件里
        /// </summary>
        /// <param name="strPath">string扩展</param>
        /// <returns></returns>
        public static string GetGlobalMapPath(this string strPath) {
            return System.Web.Hosting.HostingEnvironment.MapPath(strPath);
        }
        /// <summary>
        /// ToGUID
        /// </summary>
        /// <param name="target">string扩展</param>
        /// <returns></returns>
        public static Guid ToGuid(this string target) {
            if (target.IsGuid()) return new Guid(target);
            return Guid.Empty;
        }
        /// <summary>
        /// ToGUID
        /// </summary>
        /// <param name="target">GUID字符串</param>
        /// <returns></returns>
        public static Guid ToUniqueIdentifier(this string target) {
            return target.ToGuid();
        }
        /// <summary>
        /// "ture"/"1" 转为 true
        /// </summary>
        /// <param name="source">string扩展</param>
        /// <returns></returns>
        public static bool True(this string source) { return string.Compare(source, "true", true) == 0 || string.Compare(source, "1", true) == 0; }
        /// <summary>
        /// "false"/"0" 转为 false
        /// </summary>
        /// <param name="source">string扩展</param>
        /// <returns></returns>
        public static bool False(this string source) { return string.Compare(source, "false", true) == 0 || string.Compare(source, "0", true) == 0; }
        /// <summary>
        /// 将字符串转T类型数据 如"123".As&lt;int>()
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="source">string扩展</param>
        /// <returns></returns>
        public static T As<T>(this string source) {
            if (source.IsNull()) return default(T);
            try {
                return (T)Convert.ChangeType(source, typeof(T));
            } catch {
                return default(T);
            }
        }
        /// <summary>
        /// json 转对象 ToJson 反操作
        /// </summary>
        /// <example>
        /// <code>
        /// "{\"MemberID\":1, \"RealName\"}".FormJson&lt;UC_Member>();
        /// </code>
        /// </example>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="json">string扩展</param>
        /// <returns></returns>
        public static T FromJson<T>(this string json) {
            //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //return jsonSerializer.Deserialize<T>(json);
            return SerializeJson.FromJson<T>(json);
        }
        /// <summary>
        /// soapXml 转对像 ToSoap反操作
        /// </summary>
        /// <param name="soapXml">string扩展</param>
        /// <returns></returns>
        //public static T FromSoap<T>(this string soapXml) {
        //    using (MemoryStream ms = new MemoryStream((new UTF8Encoding()).GetBytes(soapXml))) {
        //        ms.Seek(0, SeekOrigin.Begin);
        //        SoapFormatter sf = new SoapFormatter(null, new StreamingContext(StreamingContextStates.Persistence));
        //        return (T)sf.Deserialize(ms);
        //    }
        //}
        /// <summary>
        /// XML字符串反序列化成对像
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="xml">object扩展</param>
        /// <returns>XML字符串反序列化成对像</returns>
        public static T FromXml<T>(this string xml) {
            return SerializeXml.FromXml<T>(xml);
        }
        /// <summary>
        /// 返序列化成对像
        /// </summary>
        /// <param name="xml">object扩展</param>
        /// <param name="type">类型</param>
        /// <returns>返序列化成对像</returns>
        //public static object FromXml(this string xml, Type type) {
        //    XmlSerializer serializer = new XmlSerializer(type);
        //    using (TextReader textReader = new StringReader(xml)) return serializer.Deserialize(textReader);
        //}
        /// <summary>
        /// 从XML文件中反序列化成对像
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="fileName">object扩展</param>
        /// <returns>从XML文件中反序列化成对像</returns>
        public static T FromXmlFile<T>(this string fileName) {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) return (T)serializer.Deserialize(stream);
        }
        /// <summary>
        /// 字符串转意 \n \r \a \b \t \" \' \\ \u***
        /// </summary>
        /// <param name="text">string扩展</param>
        /// <returns></returns>
        public static string ToOriginal(this string text) {
            StringBuilder sb = new StringBuilder();
            char[] chars = text.ToCharArray();
            foreach (char c in chars) {
                switch ((ushort)c) {
                    case 10: sb.Append("\\n"); continue;
                    case 13: sb.Append("\\r"); continue;
                    case 7: sb.Append("\\a"); continue;
                    case 8: sb.Append("\\b"); continue;
                    case 9: sb.Append("\\t"); continue;
                    case 0x0b: sb.Append("\\b"); continue;
                    case 34: sb.Append("\\\""); continue;
                    case 39: sb.Append("\\\'"); continue;
                    case 92: sb.Append("\\\\"); continue;
                }
                if ((ushort)c < 32 || (ushort)c == 127) sb.Append("\\u" + ((ushort)c).ToString("x4")); else sb.Append(c);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 首字母大小
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string UpperFirstChar(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            if (str.Length == 1) return str.ToUpper();
            return str.ToUpper().Substring(0, 1) + str.Substring(1, str.Length - 1);
        }
        /// <summary>
        /// 首字母大写 如果是大写前加_
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string UpperFirstChar2(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            string f = str.Substring(0, 1).ToUpper();
            if (f == str.Substring(0, 1)) return "_" + str;
            if (str.Length == 1) return str.ToUpper();
            return f + str.Substring(1, str.Length - 1);
        }
        /// <summary>
        /// 首字每小写
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string LowerFirstChar(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            if (str.Length == 1) return str.ToLower();
            return str.ToLower().Substring(0, 1) + str.Substring(1, str.Length - 1);
        }
        /// <summary>
        /// 首字母小写 如果是小写前加_
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string LowerFirstChar2(this string str) {
            if (str.IsNullEmpty()) return string.Empty;
            string f = str.Substring(0, 1).ToLower();
            if (f == str.Substring(0, 1)) return "_" + str;
            if (str.Length == 1) return str.ToLower();
            return f + str.Substring(1, str.Length - 1);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="c">分割符</param>
        /// <param name="isDelZero">是否去除小于等于0的ID</param>
        /// <returns></returns>
        public static string FormatToID(this string str, char c = ',', bool isDelZero = true) {
            string newstr = string.Empty;
            if (isDelZero) {
                foreach (string s in str.Split(c)) {
                    long id = s.Trim().ToBigInt();
                    if (id <= 0) continue;
                    newstr += id.ToString() + c.ToString();
                }
            } else {
                foreach (string s in str.Split(c)) {
                    long id = s.Trim().ToBigInt();
                    newstr += id <= 0 ? "0" : id.ToString() + c.ToString();
                }
            }
            if (newstr.Length > 0) newstr = newstr.Remove(newstr.Length - 1, 1);
            return newstr;
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="c">分割符</param>
        /// <param name="isDelZero">是否去除小于等于0的ID</param>
        /// <returns></returns>
        public static IList<long> FormatToLongID(this string str, char c = ',', bool isDelZero = true) {
            IList<long> list = new List<long>();
            if (isDelZero) {
                foreach (string s in str.Split(c)) {
                    long id = s.Trim().ToBigInt();
                    if (id <= 0) continue;
                    list.Add(id);
                }
            } else {
                foreach (string s in str.Split(c)) {
                    long id = s.Trim().ToBigInt();
                    list.Add(id <= 0 ? 0 : id);
                }
            }
            return list;
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="c">分割符</param>
        /// <param name="isDelZero">是否去除小于等于0的ID</param>
        /// <returns></returns>
        public static IList<int> FormatToIntID(this string str, char c = ',', bool isDelZero = true) {
            IList<int> list = new List<int>();
            if (isDelZero) {
                foreach (string s in str.Split(c)) {
                    int id = s.Trim().ToInt();
                    if (id <= 0) continue;
                    list.Add(id);
                }
            } else {
                foreach (string s in str.Split(c)) {
                    int id = s.Trim().ToInt();
                    list.Add(id <= 0 ? 0 : id);
                }
            }
            return list;
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="c">分割符</param>
        /// <returns></returns>
        public static string FormatToID(this string str, char c = ',') {
            return FormatToID(str, c, true);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="isDelZero">是否去除小于等于0的ID</param>
        /// <returns></returns>
        public static string FormatToID(this string str, bool isDelZero = true) {
            return FormatToID(str, ',', isDelZero);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string FormatToID(this string str) {
            return FormatToID(str, ',', true);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="c">分割符</param>
        /// <returns></returns>
        public static string FormatToID(this string str, string c = ",") {
            return FormatToID(str, c, true);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="c">分割符</param>
        /// <param name="isDelZero">是否去除小于等于0的ID</param>
        /// <returns></returns>
        public static string FormatToID(this string str, string c = ",", bool isDelZero = true) {
            string newstr = string.Empty;
            if (isDelZero) {
                foreach (string s in str.Split(c)) {
                    long id = s.Trim().ToBigInt();
                    if (id <= 0) continue;
                    newstr += id.ToString() + c.ToString();
                }
            } else {
                foreach (string s in str.Split(c)) {
                    long id = s.Trim().ToBigInt();
                    newstr += id <= 0 ? "0" : id.ToString() + c.ToString();
                }
            }
            if (newstr.Length > 0) newstr = newstr.Remove(newstr.Length - 1, 1);
            return newstr;
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="c">分割符</param>
        /// <returns></returns>
        public static IList<long> FormatToLongID(this string str, char c = ',') {
            return FormatToLongID(str, c, true);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="isDelZero">是否去除小于等于0的ID</param>
        /// <returns></returns>
        public static IList<long> FormatToLongID(this string str, bool isDelZero = true) {
            return FormatToLongID(str, ',', isDelZero);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static IList<long> FormatToLongID(this string str) {
            return FormatToLongID(str, ',', true);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="c">分割符</param>
        /// <returns></returns>
        public static IList<int> FormatToIntID(this string str, char c = ',') {
            return FormatToIntID(str, c, true);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="isDelZero">是否去除小于等于0的ID</param>
        /// <returns></returns>
        public static IList<int> FormatToIntID(this string str, bool isDelZero = true) {
            return FormatToIntID(str, ',', isDelZero);
        }
        /// <summary>
        /// 检查字符串ID是否合法（大于0），去除小于等于0的ID
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static IList<int> FormatToIntID(this string str) {
            return FormatToIntID(str, ',', true);
        }
        /// <summary>
        /// 安全SQL
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string SafeSql(this string str) {
            if (str.IsNull() || str.IsNullEmpty()) return string.Empty;
            str = str.Replace("'", "''");
            //str = Regex.Replace(str, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(exe)(c)", "$1&#32;$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(xp_cmdshel)(l)", "$1&#32;$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(master)(.)", "$1&#32;$2", RegexOptions.IgnoreCase);
            return str;
        }
        /// <summary>
        /// 安全SQL
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string SafeSql2(this string str) {
            if (str.IsNull() || str.IsNullEmpty()) return string.Empty;
            //str = str.Replace("'", "''");
            str = Regex.Replace(str, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(exe)(c)", "$1&#32;$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(xp_cmdshel)(l)", "$1&#32;$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(master)(.)", "$1&#32;$2", RegexOptions.IgnoreCase);
            return str;
        }
        /// <summary>
        /// 安全SQL '
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns></returns>
        public static string SafeSqlSimple(this string str) {
            str = str.IsNullEmpty() ? "" : str.Replace("'", "''");
            return str;
        }
        /// <summary>
        /// 还原SafeSql转换之后的数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ShowSafeSql(this string str) {
            if (str.IsNull() || str.IsNullEmpty()) return string.Empty;
            str = str.Replace("''", "'");
            //str = Regex.Replace(str, @"(\\)([\000\010\011\012\015\032\042\047\134\140])", "$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(exe)&#32;(c)", "$1$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(xp_cmdshel)&#32;(l)", "$1$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(master)&#32;(.)", "$1$2", RegexOptions.IgnoreCase);
            return str;
        }
        /// <summary>
        /// 还原SafeSql转换之后的数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ShowSafeSql2(this string str) {
            if (str.IsNull() || str.IsNullEmpty()) return string.Empty;
            //str = str.Replace("''", "'");
            str = Regex.Replace(str, @"(\\)([\000\010\011\012\015\032\042\047\134\140])", "$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(exe)&#32;(c)", "$1$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(xp_cmdshel)&#32;(l)", "$1$2", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"(master)&#32;(.)", "$1$2", RegexOptions.IgnoreCase);
            return str;
        }
        /// <summary>
        /// ToInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static int ToInt(this string strValue, int defValue) { int def = 0; int.TryParse(strValue, out def); return def == 0 ? defValue : def; }
        /// <summary>
        /// ToTinyInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static byte ToTinyInt(this string strValue, byte defValue) { byte def = 0; byte.TryParse(strValue, out def); return def == 0 ? defValue : def; }
        /// <summary>
        /// ToSmallInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static short ToSmallInt(this string strValue, short defValue) { short def = 0; short.TryParse(strValue, out def); return def == 0 ? defValue : def; }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string strValue, decimal defValue) { decimal def = 0; decimal.TryParse(strValue, out def); return def == 0 ? defValue : def; }
        /// <summary>
        /// ToFloat
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static float ToFloat(this string strValue, float defValue) { float def = 0; float.TryParse(strValue, out def); return def == 0 ? defValue : def; }
        /// <summary>
        /// ToBigInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static Int64 ToBigInt(this string strValue, Int64 defValue) { Int64 def = 0; Int64.TryParse(strValue, out def); return def == 0 ? defValue : def; }
        /// <summary>
        /// ToMoney
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static decimal ToMoney(this string strValue, decimal defValue) { decimal def = 0; decimal.TryParse(strValue, out def); return def == 0 ? defValue : def; }
        /// <summary>
        /// ToInteger
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static int ToInteger(this string strValue, int defValue) { int def = 0; int.TryParse(strValue, out def); return def == 0 ? defValue : def; }
        /// <summary>
        /// ToBool
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static bool ToBool(this string strValue, bool defValue) {
            if (!strValue.IsNullEmpty()) {
                if (string.Compare(strValue, "true", true) == 0) return true;
                if (string.Compare(strValue, "false", true) == 0) return false;
                if (string.Compare(strValue, "1", true) == 0) return true;
                if (string.Compare(strValue, "0", true) == 0) return false;
            }
            return defValue;
        }
        /// <summary>
        /// ToInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static int ToInt(this string strValue) { return strValue.ToInt(0); }
        /// <summary>
        /// ToInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static int ToInt(this char strValue) { return strValue.ToString().ToInt(0); }
        /// <summary>
        /// ToTinyInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static byte ToTinyInt(this string strValue) { return strValue.ToTinyInt(0); }
        /// <summary>
        /// ToSmallInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static short ToSmallInt(this string strValue) { return strValue.ToSmallInt(0); }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string strValue) { return strValue.ToDecimal(0); }
        /// <summary>
        /// ToFloat
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static float ToFloat(this string strValue) { return strValue.ToFloat(0); }
        /// <summary>
        /// ToBigInt
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static Int64 ToBigInt(this string strValue) { return strValue.ToBigInt(0); }
        /// <summary>
        /// ToMoney
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static decimal ToMoney(this string strValue) { return strValue.ToMoney(0); }
        /// <summary>
        /// ToInteger
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static int ToInteger(this string strValue) { return strValue.ToInteger(0); }
        /// <summary>
        /// ToBool
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static bool ToBool(this string strValue) { return strValue.ToBool(false); }
        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns>全角</returns>
        public static string ToSBC(this string input) {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++) {
                if (c[i] == 32) {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127) c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns>半角</returns>
        public static string ToDBC(this string input) {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++) {
                if (c[i] == 12288) {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375) c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// SQLite数据类型转DbType
        /// </summary>
        /// <param name="sqlType">string扩展</param>
        /// <returns>SQLite数据类型转DbType</returns>
        public static DbType ToDbTypeForSQLite(this string sqlType) {
            switch (sqlType.ToLowerInvariant()) {
                case "longtext":
                case "nchar":
                case "ntext":
                case "text":
                case "sysname":
                case "varchar":
                case "nvarchar": return DbType.String;
                case "bit":
                case "tinyint": return DbType.Boolean;
                case "decimal":
                case "float":
                case "newdecimal":
                case "numeric":
                case "double":
                case "real": return DbType.Decimal;
                case "int":
                case "int32": return DbType.Int32;
                case "integer": return DbType.Int64;
                case "int16":
                case "smallint": return DbType.Int16;
                case "date":
                case "time":
                case "datetime":
                case "smalldatetime": return DbType.DateTime;
                case "image":
                case "varbinary":
                case "binary":
                case "blob":
                case "longblob": return DbType.Binary;
                case "char": return DbType.AnsiStringFixedLength;
                case "currency":
                case "money":
                case "smallmoney": return DbType.Currency;
                case "timestamp": return DbType.DateTime;
                case "uniqueidentifier": return DbType.Guid;
                case "uint16": return DbType.UInt16;
                case "uint32": return DbType.UInt32;
            }
            return DbType.String;
        }
        /// <summary>
        /// Sql2005数据类型转DbType
        /// </summary>
        /// <param name="sqlType">string扩展</param>
        /// <returns>Sql2005数据类型转DbType</returns>
        public static DbType ToDbTypeForSql2005(this string sqlType) {
            switch (sqlType) {
                case "varchar": return DbType.AnsiString;
                case "nvarchar": return DbType.String;
                case "int": return DbType.Int32;
                case "uniqueidentifier": return DbType.Guid;
                case "datetime":
                case "datetime2": return DbType.DateTime;
                case "bigint": return DbType.Int64;
                case "binary": return DbType.Binary;
                case "bit": return DbType.Boolean;
                case "char": return DbType.AnsiStringFixedLength;
                case "decimal": return DbType.Decimal;
                case "float": return DbType.Double;
                case "image": return DbType.Binary;
                case "money": return DbType.Currency;
                case "nchar": return DbType.String;
                case "ntext": return DbType.String;
                case "numeric": return DbType.Decimal;
                case "real": return DbType.Single;
                case "smalldatetime": return DbType.DateTime;
                case "smallint": return DbType.Int16;
                case "smallmoney": return DbType.Currency;
                case "sql_variant": return DbType.String;
                case "sysname": return DbType.String;
                case "text": return DbType.AnsiString;
                case "timestamp": return DbType.Binary;
                case "tinyint": return DbType.Byte;
                case "varbinary": return DbType.Binary;
            }
            return DbType.AnsiString;
        }
        /// <summary>
        /// MySql数据类型转DbType
        /// </summary>
        /// <param name="mySqlType">string扩展</param>
        /// <returns>MySql数据类型转DbType</returns>
        public static DbType ToDbTypeForMySql(this string mySqlType) {
            switch (mySqlType.ToLowerInvariant()) {
                case "longtext":
                case "nchar":
                case "ntext":
                case "text":
                case "sysname":
                case "varchar":
                case "nvarchar": return DbType.String;
                case "bit":
                case "tinyint": return DbType.Boolean;
                case "decimal":
                case "float":
                case "newdecimal":
                case "numeric":
                case "double":
                case "real": return DbType.Decimal;
                case "bigint": return DbType.Int64;
                case "int":
                case "int32":
                case "integer": return DbType.Int32;
                case "int16":
                case "smallint": return DbType.Int16;
                case "date":
                case "time":
                case "datetime":
                case "smalldatetime": return DbType.DateTime;
                case "image":
                case "varbinary":
                case "binary":
                case "blob":
                case "longblob": return DbType.Binary;
                case "char": return DbType.AnsiStringFixedLength;
                case "currency":
                case "money":
                case "smallmoney": return DbType.Currency;
                case "timestamp": return DbType.DateTime;
                case "uniqueidentifier": return DbType.Binary;
                case "uint16": return DbType.UInt16;
                case "uint32": return DbType.UInt32;
            }
            return DbType.String;
        }
        /// <summary>
        /// 文件扩展名取HttpContentType 如.doc application/msword
        /// </summary>
        /// <param name="extension">文件扩展名</param>
        /// <returns>文件扩展名取HttpContentType 如.doc application/msword</returns>
        public static string GetContentType(string extension) {
            switch (extension.Trim('.')) {
                #region 常用文件类型
                case "jpeg": return "image/jpeg";
                case "jpg": return "image/jpeg";
                case "js": return "application/x-javascript";
                case "jsp": return "text/html";
                case "gif": return "image/gif";
                case "htm": return "text/html";
                case "html": return "text/html";
                case "asf": return "video/x-ms-asf";
                case "avi": return "video/avi";
                case "bmp": return "application/x-bmp";
                case "asp": return "text/asp";
                case "wma": return "audio/x-ms-wma";
                case "wav": return "audio/wav";
                case "wmv": return "video/x-ms-wmv";
                case "ra": return "audio/vnd.rn-realaudio";
                case "ram": return "audio/x-pn-realaudio";
                case "rm": return "application/vnd.rn-realmedia";
                case "rmvb": return "application/vnd.rn-realmedia-vbr";
                case "xhtml": return "text/html";
                case "png": return "image/png";
                case "ppt": return "application/x-ppt";
                case "tif": return "image/tiff";
                case "tiff": return "image/tiff";
                case "xls": return "application/x-xls";
                case "xlw": return "application/x-xlw";
                case "xml": return "text/xml";
                case "xpl": return "audio/scpls";
                case "swf": return "application/x-shockwave-flash";
                case "torrent": return "application/x-bittorrent";
                case "dll": return "application/x-msdownload";
                case "asa": return "text/asa";
                case "asx": return "video/x-ms-asf";
                case "au": return "audio/basic";
                case "css": return "text/css";
                case "doc": return "application/msword";
                case "exe": return "application/x-msdownload";
                case "mp1": return "audio/mp1";
                case "mp2": return "audio/mp2";
                case "mp2v": return "video/mpeg";
                case "mp3": return "audio/mp3";
                case "mp4": return "video/mpeg4";
                case "mpa": return "video/x-mpg";
                case "mpd": return "application/vnd.ms-project";
                case "mpe": return "video/x-mpeg";
                case "mpeg": return "video/mpg";
                case "mpg": return "video/mpg";
                case "mpga": return "audio/rn-mpeg";
                case "mpp": return "application/vnd.ms-project";
                case "mps": return "video/x-mpeg";
                case "mpt": return "application/vnd.ms-project";
                case "mpv": return "video/mpg";
                case "mpv2": return "video/mpeg";
                case "wml": return "text/vnd.wap.wml";
                case "wsdl": return "text/xml";
                case "xsd": return "text/xml";
                case "xsl": return "text/xml";
                case "xslt": return "text/xml";
                case "htc": return "text/x-component";
                case "mdb": return "application/msaccess";
                case "zip": return "application/zip";
                case "rar": return "application/x-rar-compressed";
                #endregion

                case "*": return "application/octet-stream";
                case "001": return "application/x-001";
                case "301": return "application/x-301";
                case "323": return "text/h323";
                case "906": return "application/x-906";
                case "907": return "drawing/907";
                case "a11": return "application/x-a11";
                case "acp": return "audio/x-mei-aac";
                case "ai": return "application/postscript";
                case "aif": return "audio/aiff";
                case "aifc": return "audio/aiff";
                case "aiff": return "audio/aiff";
                case "anv": return "application/x-anv";
                case "awf": return "application/vnd.adobe.workflow";
                case "biz": return "text/xml";
                case "bot": return "application/x-bot";
                case "c4t": return "application/x-c4t";
                case "c90": return "application/x-c90";
                case "cal": return "application/x-cals";
                case "cat": return "application/vnd.ms-pki.seccat";
                case "cdf": return "application/x-netcdf";
                case "cdr": return "application/x-cdr";
                case "cel": return "application/x-cel";
                case "cer": return "application/x-x509-ca-cert";
                case "cg4": return "application/x-g4";
                case "cgm": return "application/x-cgm";
                case "cit": return "application/x-cit";
                case "class": return "java/*";
                case "cml": return "text/xml";
                case "cmp": return "application/x-cmp";
                case "cmx": return "application/x-cmx";
                case "cot": return "application/x-cot";
                case "crl": return "application/pkix-crl";
                case "crt": return "application/x-x509-ca-cert";
                case "csi": return "application/x-csi";
                case "cut": return "application/x-cut";
                case "dbf": return "application/x-dbf";
                case "dbm": return "application/x-dbm";
                case "dbx": return "application/x-dbx";
                case "dcd": return "text/xml";
                case "dcx": return "application/x-dcx";
                case "der": return "application/x-x509-ca-cert";
                case "dgn": return "application/x-dgn";
                case "dib": return "application/x-dib";
                case "dot": return "application/msword";
                case "drw": return "application/x-drw";
                case "dtd": return "text/xml";
                case "dwf": return "application/x-dwf";
                case "dwg": return "application/x-dwg";
                case "dxb": return "application/x-dxb";
                case "dxf": return "application/x-dxf";
                case "edn": return "application/vnd.adobe.edn";
                case "emf": return "application/x-emf";
                case "eml": return "message/rfc822";
                case "ent": return "text/xml";
                case "epi": return "application/x-epi";
                case "eps": return "application/x-ps";
                case "etd": return "application/x-ebx";
                case "fax": return "image/fax";
                case "fdf": return "application/vnd.fdf";
                case "fif": return "application/fractals";
                case "fo": return "text/xml";
                case "frm": return "application/x-frm";
                case "g4": return "application/x-g4";
                case "gbr": return "application/x-gbr";
                case "gcd": return "application/x-gcd";

                case "gl2": return "application/x-gl2";
                case "gp4": return "application/x-gp4";
                case "hgl": return "application/x-hgl";
                case "hmr": return "application/x-hmr";
                case "hpg": return "application/x-hpgl";
                case "hpl": return "application/x-hpl";
                case "hqx": return "application/mac-binhex40";
                case "hrf": return "application/x-hrf";
                case "hta": return "application/hta";
                case "htt": return "text/webviewhtml";
                case "htx": return "text/html";
                case "icb": return "application/x-icb";
                case "ico": return "application/x-ico";
                case "iff": return "application/x-iff";
                case "ig4": return "application/x-g4";
                case "igs": return "application/x-igs";
                case "iii": return "application/x-iphone";
                case "img": return "application/x-img";
                case "ins": return "application/x-internet-signup";
                case "isp": return "application/x-internet-signup";
                case "IVF": return "video/x-ivf";
                case "java": return "java/*";
                case "jfif": return "image/jpeg";
                case "jpe": return "application/x-jpe";
                case "la1": return "audio/x-liquid-file";
                case "lar": return "application/x-laplayer-reg";
                case "latex": return "application/x-latex";
                case "lavs": return "audio/x-liquid-secure";
                case "lbm": return "application/x-lbm";
                case "lmsff": return "audio/x-la-lms";
                case "ls": return "application/x-javascript";
                case "ltr": return "application/x-ltr";
                case "m1v": return "video/x-mpeg";
                case "m2v": return "video/x-mpeg";
                case "m3u": return "audio/mpegurl";
                case "m4e": return "video/mpeg4";
                case "mac": return "application/x-mac";
                case "man": return "application/x-troff-man";
                case "math": return "text/xml";
                case "mfp": return "application/x-shockwave-flash";
                case "mht": return "message/rfc822";
                case "mhtml": return "message/rfc822";
                case "mi": return "application/x-mi";
                case "mid": return "audio/mid";
                case "midi": return "audio/mid";
                case "mil": return "application/x-mil";
                case "mml": return "text/xml";
                case "mnd": return "audio/x-musicnet-download";
                case "mns": return "audio/x-musicnet-stream";
                case "mocha": return "application/x-javascript";
                case "movie": return "video/x-sgi-movie";
                case "mpw": return "application/vnd.ms-project";
                case "mpx": return "application/vnd.ms-project";
                case "mtx": return "text/xml";
                case "mxp": return "application/x-mmxp";
                case "net": return "image/pnetvue";
                case "nrf": return "application/x-nrf";
                case "nws": return "message/rfc822";
                case "odc": return "text/x-ms-odc";
                case "out": return "application/x-out";
                case "p10": return "application/pkcs10";
                case "p12": return "application/x-pkcs12";
                case "p7b": return "application/x-pkcs7-certificates";
                case "p7c": return "application/pkcs7-mime";
                case "p7m": return "application/pkcs7-mime";
                case "p7r": return "application/x-pkcs7-certreqresp";
                case "p7s": return "application/pkcs7-signature";
                case "pc5": return "application/x-pc5";
                case "pci": return "application/x-pci";
                case "pcl": return "application/x-pcl";
                case "pcx": return "application/x-pcx";
                case "pdf": return "application/pdf";
                case "pdx": return "application/vnd.adobe.pdx";
                case "pfx": return "application/x-pkcs12";
                case "pgl": return "application/x-pgl";
                case "pic": return "application/x-pic";
                case "pko": return "application/vnd.ms-pki.pko";
                case "pl": return "application/x-perl";
                case "plg": return "text/html";
                case "pls": return "audio/scpls";
                case "plt": return "application/x-plt";
                case "pot": return "application/vnd.ms-powerpoint";
                case "ppa": return "application/vnd.ms-powerpoint";
                case "ppm": return "application/x-ppm";
                case "pps": return "application/vnd.ms-powerpoint";
                case "pr": return "application/x-pr";
                case "prf": return "application/pics-rules";
                case "prn": return "application/x-prn";
                case "prt": return "application/x-prt";
                case "ps": return "application/x-ps";
                case "ptn": return "application/x-ptn";
                case "pwz": return "application/vnd.ms-powerpoint";
                case "r3t": return "text/vnd.rn-realtext3d";
                case "ras": return "application/x-ras";
                case "rat": return "application/rat-file";
                case "rdf": return "text/xml";
                case "rec": return "application/vnd.rn-recording";
                case "red": return "application/x-red";
                case "rgb": return "application/x-rgb";
                case "rjs": return "application/vnd.rn-realsystem-rjs";
                case "rjt": return "application/vnd.rn-realsystem-rjt";
                case "rlc": return "application/x-rlc";
                case "rle": return "application/x-rle";
                case "rmf": return "application/vnd.adobe.rmf";
                case "rmi": return "audio/mid";
                case "rmj": return "application/vnd.rn-realsystem-rmj";
                case "rmm": return "audio/x-pn-realaudio";
                case "rmp": return "application/vnd.rn-rn_music_package";
                case "rms": return "application/vnd.rn-realmedia-secure";
                case "rmx": return "application/vnd.rn-realsystem-rmx";
                case "rnx": return "application/vnd.rn-realplayer";
                case "rp": return "image/vnd.rn-realpix";
                case "rpm": return "audio/x-pn-realaudio-plugin";
                case "rsml": return "application/vnd.rn-rsml";
                case "rt": return "text/vnd.rn-realtext";
                case "rtf": return "application/msword";
                case "rv": return "video/vnd.rn-realvideo";
                case "sam": return "application/x-sam";
                case "sat": return "application/x-sat";
                case "sdp": return "application/sdp";
                case "sdw": return "application/x-sdw";
                case "sit": return "application/x-stuffit";
                case "slb": return "application/x-slb";
                case "sld": return "application/x-sld";
                case "slk": return "drawing/x-slk";
                case "smi": return "application/smil";
                case "smil": return "application/smil";
                case "smk": return "application/x-smk";
                case "snd": return "audio/basic";
                case "sol": return "text/plain";
                case "sor": return "text/plain";
                case "spc": return "application/x-pkcs7-certificates";
                case "spl": return "application/futuresplash";
                case "spp": return "text/xml";
                case "ssm": return "application/streamingmedia";
                case "sst": return "application/vnd.ms-pki.certstore";
                case "stl": return "application/vnd.ms-pki.stl";
                case "stm": return "text/html";
                case "sty": return "application/x-sty";
                case "svg": return "text/xml";
                case "tdf": return "application/x-tdf";
                case "tg4": return "application/x-tg4";
                case "tga": return "application/x-tga";
                case "tld": return "text/xml";
                case "top": return "drawing/x-top";
                case "tsd": return "text/xml";
                case "txt": return "text/plain";
                case "uin": return "application/x-icq";
                case "uls": return "text/iuls";
                case "vcf": return "text/x-vcard";
                case "vda": return "application/x-vda";
                case "vdx": return "application/vnd.visio";
                case "vml": return "text/xml";
                case "vpg": return "application/x-vpeg005";
                case "vsd": return "application/vnd.visio";
                case "vss": return "application/vnd.visio";
                case "vst": return "application/vnd.visio";
                case "vsw": return "application/vnd.visio";
                case "vsx": return "application/vnd.visio";
                case "vtx": return "application/vnd.visio";
                case "vxml": return "text/xml";
                case "wax": return "audio/x-ms-wax";
                case "wb1": return "application/x-wb1";
                case "wb2": return "application/x-wb2";
                case "wb3": return "application/x-wb3";
                case "wbmp": return "image/vnd.wap.wbmp";
                case "wiz": return "application/msword";
                case "wk3": return "application/x-wk3";
                case "wk4": return "application/x-wk4";
                case "wkq": return "application/x-wkq";
                case "wks": return "application/x-wks";
                case "wm": return "video/x-ms-wm";
                case "wmd": return "application/x-ms-wmd";
                case "wmf": return "application/x-wmf";
                case "wmx": return "video/x-ms-wmx";
                case "wmz": return "application/x-ms-wmz";
                case "wp6": return "application/x-wp6";
                case "wpd": return "application/x-wpd";
                case "wpg": return "application/x-wpg";
                case "wpl": return "application/vnd.ms-wpl";
                case "wq1": return "application/x-wq1";
                case "wr1": return "application/x-wr1";
                case "wri": return "application/x-wri";
                case "wrk": return "application/x-wrk";
                case "ws": return "application/x-ws";
                case "ws2": return "application/x-ws";
                case "wsc": return "text/scriptlet";
                case "wvx": return "video/x-ms-wvx";
                case "xdp": return "application/vnd.adobe.xdp";
                case "xdr": return "text/xml";
                case "xfd": return "application/vnd.adobe.xfd";
                case "xfdf": return "application/vnd.adobe.xfdf";
                case "xq": return "text/xml";
                case "xql": return "text/xml";
                case "xquery": return "text/xml";
                case "xwd": return "application/x-xwd";
                case "x_b": return "application/x-x_b";
                case "x_t": return "application/x-x_t";
            }
            return "application/octet-stream";
        }
        /// <summary>
        /// 转SecureString
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns>SecureString</returns>
        public static SecureString ToSecureString(this string str) {
            SecureString secureString = new SecureString();
            foreach (Char c in str) secureString.AppendChar(c);
            return secureString;
        }
        /// <summary>
        /// 返回DLL文件的全路径
        /// </summary>
        /// <example>
        /// <code>
        /// Msg.Write("test".GetBinFileFullPath() + "&lt;br />");
        /// Msg.Write("test.dll".GetBinFileFullPath() + "&lt;br />");
        /// Msg.Write("test.dllx".GetBinFileFullPath() + "&lt;br />&lt;br />");
        /// Msg.Write("c:\\test".GetBinFileFullPath() + "&lt;br />");
        /// Msg.Write("c:\\test.dll".GetBinFileFullPath() + "&lt;br />");
        /// Msg.Write("c:\\test.dllx".GetBinFileFullPath() + "&lt;br />&lt;br />");
        /// Msg.Write("~/test".GetBinFileFullPath() + "&lt;br />");
        /// Msg.Write("~/test.dll".GetBinFileFullPath() + "&lt;br />");
        /// Msg.Write("~/test.dllx".GetBinFileFullPath() + "&lt;br />&lt;br />");
        /// </code>
        /// </example>
        /// <param name="fileName">文件名/文件绝对路径/文件相对路径</param>
        /// <param name="ext">指定扩展名</param>
        /// <returns></returns>
        public static string GetBinFileFullPath(this string fileName, string ext = ".dll") {
            if (!ext.IsNullEmpty() && !fileName.EndsWith(ext, true, null)) fileName = fileName + ".dll";
            if (fileName.IndexOf("\\") != -1 && fileName.IndexOf(":") != -1) return fileName;
            if (fileName.IndexOf("\\") != -1) fileName = fileName.Replace("\\", "/");
            if (HttpContext.Current.IsNotNull())
                return ("~/bin/" + fileName.Trim('/')).GetMapPath();
            else
                return fileName.GetMapPath();

            //if (fileName.IndexOf("/") != -1) fileName = fileName.GetMapPath();else
            //fileName = HttpContext.Current.IsNotNull() ? "~/".GetMapPath() + "bin\\" + fileName.Trim('/').Replace("/", "\\") : fileName.GetMapPath();
            //Msg.WriteEnd("~/".GetMapPath() + "bin\\" + fileName.Trim('/').Replace("/", "\\"));
            //return fileName;
        }
        /// <summary>
        /// 读文件流
        /// </summary>
        /// <param name="dllFullPath">dll路径或文件名</param>
        /// <returns></returns>
        public static byte[] LoadFileStream(this string dllFullPath) {
            return new FileStream(dllFullPath.GetBinFileFullPath(), FileMode.Open).ToBytes();
        }
        private static readonly ISafeDictionary<string, Assembly> assemblyCache = new SafeDictionary<string, Assembly>();
        /// <summary>
        /// 动态加载dll程序集 CACHE
        /// </summary>
        /// <param name="dllFullPath">dll路径或文件名</param>
        /// <returns></returns>
        public static Assembly LoadDynamicDLLAssembly(this string dllFullPath) {
            if (assemblyCache.ContainsKey(dllFullPath)) return assemblyCache[dllFullPath];

            Assembly assembly = null;
            byte[] addinStream = dllFullPath.GetBinFileFullPath().LoadFileStream();//先将插件拷贝到内存缓冲
            assembly = Assembly.Load(addinStream); //加载内存中的Dll
            if (!assemblyCache.ContainsKey(dllFullPath)) assemblyCache.Add(dllFullPath, assembly);
            return assembly;
        }
        /// <summary>
        /// 加载DLL程序集 CACHE
        /// </summary>
        /// <param name="dllFullPath">dll路径或文件名</param>
        /// <returns></returns>
        public static Assembly LoadDLLAssembly(this string dllFullPath) {
            if (assemblyCache.ContainsKey(dllFullPath)) return assemblyCache[dllFullPath];
            Assembly assembly = Assembly.LoadFrom(dllFullPath.GetBinFileFullPath());
            if (!assemblyCache.ContainsKey(dllFullPath)) assemblyCache.Add(dllFullPath, assembly);
            return assembly;
        }
        /// <summary>
        /// 加载程序集 CACHE
        /// </summary>
        /// <param name="assembly">程序集名称或dll路径或文件名</param>
        /// <returns></returns>
        public static Assembly LoadAssembly(this string assembly) {
            if (assembly.IndexOf("\\") != -1 || assembly.IndexOf("/") != -1 || assembly.EndsWith(".dll", true, null)) return assembly.LoadDLLAssembly();
            if (assemblyCache.ContainsKey(assembly)) return assemblyCache[assembly];
            Assembly _assembly = Assembly.Load(assembly);
            if (!assemblyCache.ContainsKey(assembly)) assemblyCache.Add(assembly, _assembly);
            return _assembly;
        }
        private static readonly ISafeDictionary<string, object> classCache = new SafeDictionary<string, object>();
        /// <summary>
        /// 动态加载指定目录下的DLL类 Activator.CreateInstance CACHE
        /// </summary>
        /// <param name="dllFileName">bin目录下的dll文件名</param>
        /// <param name="className">命名空间和类名</param>
        /// <returns></returns>
        public static object LoadDLLClass(this string dllFileName, string className = "") {
            dllFileName = dllFileName.GetBinFileFullPath();
            className = className.Length == 0 ? dllFileName.GetFileNameWithoutExtension() : className;
            if (classCache.ContainsKey(dllFileName + className)) return classCache[dllFileName + className];
            object obj = Activator.CreateInstance(dllFileName.LoadDLLAssembly().GetType(className, false, true));
            if (!classCache.ContainsKey(dllFileName + className)) classCache.Add(dllFileName + className, obj);
            return obj;
        }
        /// <summary>
        /// 动态加载类 Activator.CreateInstance CACHE
        /// </summary>
        /// <param name="assembly">程序集名称或dll文件路径 支持绝对路径和相对路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <returns></returns>
        public static object LoadClass(this string assembly, string className = "") {
            if (assembly.IndexOf("\\") != -1 || assembly.IndexOf("/") != -1 || assembly.EndsWith(".dll", true, null)) return assembly.LoadDLLClass(className);
            if (classCache.ContainsKey(assembly + className)) return classCache[assembly + className];
            object obj = Activator.CreateInstance(assembly.LoadAssembly().GetType(className, false, true));
            if (!classCache.ContainsKey(assembly + className)) classCache.Add(assembly + className, obj);
            return obj;
        }
        /// <summary>
        /// 动态加载指定目录下的DLL类 Activator.CreateInstance CACHE
        /// </summary>
        /// <param name="dllFileName">dll文件路径 支持绝对路径和相对路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <returns></returns>
        public static object LoadDynamicDLLClass(this string dllFileName, string className = "") {
            dllFileName = dllFileName.GetBinFileFullPath();
            className = className.Length == 0 ? dllFileName.GetFileNameWithoutExtension() : className;
            if (classCache.ContainsKey(dllFileName + className)) return classCache[dllFileName + className];
            object obj = Activator.CreateInstance(dllFileName.LoadDynamicDLLAssembly().GetType(className, false, true));
            if (!classCache.ContainsKey(dllFileName + className)) classCache.Add(dllFileName + className, obj);
            return obj;
        }
        /// <summary>
        /// 返回程序集的Cache
        /// </summary>
        /// <returns></returns>
        public static ISafeDictionary<string, object> GetClassCache() {
            return classCache;
        }
        /// <summary>
        /// 返回类的Cache
        /// </summary>
        /// <returns></returns>
        public static ISafeDictionary<string, Assembly> GetAssemblyCache() {
            return assemblyCache;
        }
        /// <summary>
        /// 动态加载类 CACHE
        /// </summary>
        /// <param name="assembly">程序集名或dll文件路径 支持绝对路径和相对路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <returns></returns>
        public static object LoadDynamicClass(this string assembly, string className = "") {
            if (assembly.IndexOf("\\") != -1 || assembly.IndexOf("/") != -1 || assembly.EndsWith(".dll", true, null)) return assembly.LoadDynamicDLLClass(className);
            if (classCache.ContainsKey(assembly + className)) return classCache[assembly + className];
            object obj = Activator.CreateInstance(assembly.LoadAssembly().GetType(className, false, true));
            if (!classCache.ContainsKey(assembly + className)) classCache.Add(assembly + className, obj);
            return obj;
        }
        /// <summary>
        /// 加载类 Activator.CreateInstance CACHE
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        /// <returns></returns>
        public static object LoadClass(this string classNameAndAssembly) {
            if (classNameAndAssembly.IndexOf("\\") != -1 || classNameAndAssembly.IndexOf("/") != -1 || classNameAndAssembly.EndsWith(".dll", true, null)) {
                string[] list = classNameAndAssembly.Split(',');
                return list[1].LoadDLLClass(list[0]);
            }
            if (classCache.ContainsKey(classNameAndAssembly)) return classCache[classNameAndAssembly];
            object obj = Activator.CreateInstance(Type.GetType(classNameAndAssembly, false, true));
            if (!classCache.ContainsKey(classNameAndAssembly)) classCache.Add(classNameAndAssembly, obj);
            return obj;
        }
        ///// <summary>
        ///// Hash值
        ///// </summary>
        ///// <param name="content">string扩展</param>
        ///// <returns>Hash值</returns>
        //public static string Hash(this string content) {
        //    return Encoding.UTF8.GetBytes(content).MD5();
        //}
        /// <summary>
        /// Unicode否
        /// </summary>
        /// <param name="s">string扩展</param>
        /// <returns>Unicode否</returns>
        //public static bool IsUnicode(this string Input) {
        //    if (string.IsNullOrEmpty(Input)) return true;
        //    UnicodeEncoding Encoding = new UnicodeEncoding();
        //    string UniInput = Encoding.GetString(Encoding.GetBytes(Input));
        //    ASCIIEncoding Encoding2 = new ASCIIEncoding();
        //    string ASCIIInput = Encoding2.GetString(Encoding2.GetBytes(Input));
        //    if (UniInput == ASCIIInput) return false;
        //    return true;
        //}
        ///// <summary>
        ///// Unicode否 使用正则
        ///// </summary>
        ///// <param name="Input">string扩展</param>
        ///// <returns>Unicode否</returns>
        //public static bool IsUnicodeRegex(this string Input) {
        //    return Input.IsNullEmpty() ? true : Regex.Replace(Input, @"[^\u0000-\u007F]", "") != Input;
        //}
        public static bool IsUnicode(this string s) {
            string text1 = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";
            return Regex.IsMatch(s, text1);
        }
        /// <summary>
        /// \r\n替换成空格
        /// </summary>
        /// <param name="input">要去除新行的字符串</param>
        /// <param name="replace">是否添加空格</param>
        /// <returns>已经去除新行的字符串</returns>
        public static string ReplaceRN(this string input, string replace = " ") {
            string pattern = @"[\r|\n]";
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(input, replace);
        }
        /// <summary>
        /// \r\n替换成&lt;br />
        /// </summary>
        /// <param name="input">string扩展</param>
        /// <returns>\r\n替换成br</returns>
        public static string ReplaceRNToBR(this string input) {
            return input.ReplaceRN("<br />");
        }
        /// <summary>
        /// 截断HTML代码长度
        /// </summary>
        /// <param name="htmlString">string扩展</param>
        /// <param name="maxLength">最大长</param>
        /// <param name="flg">扩展字符</param>
        /// <returns>截断HTML代码长度</returns>
        public static string SubHtmlString(this string htmlString, int maxLength, string flg) {
            bool isText = true; StringBuilder r = new StringBuilder(); int i = 0;
            char currentChar = ' '; int lastSpacePosition = -1; char lastChar = ' ';

            Dictionary<int, string> tagsArray = new Dictionary<int, string>();
            string currentTag = ""; int tagLevel = 0;
            int noTagLength = 0; int j = 0;

            for (j = 0; j < htmlString.Length; j++) {
                currentChar = htmlString[j];
                if (currentChar == '<') isText = false;
                if (isText) noTagLength++;
                if (currentChar == '>') isText = true;
            }
            for (j = 0; j < htmlString.Length; j++) {
                currentChar = htmlString[j];
                r.Append(currentChar);
                if (currentChar == '<') isText = false;
                if (isText) {
                    if (currentChar == ' ') lastSpacePosition = j; else lastChar = currentChar;
                    i++;
                } else currentTag += currentChar;
                if (currentChar == '>') {
                    isText = true;
                    if (currentTag.IndexOf("<") != -1 && currentTag.IndexOf("/>") == -1 && currentTag.IndexOf("</") == -1) {
                        if (currentTag.IndexOf(" ") != -1) {
                            currentTag = currentTag.Substring(1, currentTag.IndexOf(" ") - 1);
                        } else {
                            currentTag = currentTag.Substring(1, currentTag.Length - 2);
                        }
                        tagsArray[tagLevel] = currentTag;
                        tagLevel++;
                    } else if (currentTag.IndexOf("</") != -1) {
                        tagsArray[tagLevel - 1] = null;
                        tagLevel--;
                    }
                    currentTag = "";
                }

                if (i == maxLength) break;
            }
            if (maxLength < noTagLength) {
                if (lastSpacePosition != -1) r = new StringBuilder(htmlString.Substring(0, lastSpacePosition)); else r = new StringBuilder(htmlString.Substring(0, j));
            }
            for (int a = tagsArray.Count - 1; a >= 0; a--) {
                if (tagsArray[a].IsNotNull()) r.Append("</" + tagsArray[a] + ">");
            }
            if (maxLength < noTagLength) r.Append(flg);
            return r.ToString();
        }
        /// <summary>
        /// UTF8ToGB2312
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns>UTF8ToGB2312</returns>
        public static string UTF8ToGB2312(this string str) {
            Encoding utf8 = Encoding.GetEncoding(65001);
            Encoding gb2312 = Encoding.GetEncoding("gb2312");
            byte[] temp = utf8.GetBytes(str);
            byte[] temp1 = Encoding.Convert(utf8, gb2312, temp);
            string result = gb2312.GetString(temp1);
            return result;
        }
        /// <summary>
        /// GB2312ToUTF8
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <returns>GB2312ToUTF8</returns>
        public static string GB2312ToUTF8(this string str) {
            Encoding uft8 = Encoding.GetEncoding(65001);
            Encoding gb2312 = Encoding.GetEncoding("gb2312");
            byte[] temp = gb2312.GetBytes(str);
            byte[] temp1 = Encoding.Convert(gb2312, uft8, temp);
            string result = uft8.GetString(temp1);
            return result;
        }
        /// <summary>
        /// 取父路径
        /// </summary>
        /// <example>
        /// <code>
        /// Msg.Write("c:/test/www".GetParentPath());
        /// Msg.Write("c:/test/www/".GetParentPath());
        /// Msg.Write("c:\\test\\www".GetParentPath('\\'));
        /// </code>
        /// </example>
        /// <param name="path">string扩展</param>
        /// <param name="c">char</param>
        /// <returns>取父路径</returns>
        public static string GetParentPath(this string path, char c = '/') {
            if (string.IsNullOrEmpty(path)) return null;
            int length = path.TrimEnd(c).LastIndexOf(c);
            if (length == -1) return null;
            return path.Substring(0, length) + c;
        }
        /// <summary>
        /// 取父路径
        /// </summary>
        /// <param name="path">string扩展</param>
        /// <returns>取父路径</returns>
        public static string GetParentPath(this string path) {
            return path.GetParentPath('/');
        }
        /// <summary>
        /// 转换长文件名为短文件名
        /// </summary>
        /// <example>
        /// <code>
        /// string name = "http://www.testxt.test.com/test/test/test/test/test.aspx".GetSimpleFileName("...", 20, 10, 30)
        /// </code>
        /// </example>
        /// <param name="fullname">string扩展</param>
        /// <param name="repstring">替换的字符串</param>
        /// <param name="leftnum">左长</param>
        /// <param name="rightnum">右长</param>
        /// <param name="charnum">总长</param>
        /// <returns>转换长文件名为短文件名</returns>
        public static string GetSimpleFileName(this string fullname, string repstring, int leftnum, int rightnum, int charnum) {
            string simplefilename = "", leftstring = "", rightstring = "", filename = "";
            string extname = fullname.GetExtension();

            if (extname.IsNullEmpty()) return fullname;

            int filelength = 0, dotindex = 0;

            dotindex = fullname.LastIndexOf('.');
            filename = fullname.Substring(0, dotindex);
            filelength = filename.Length;
            if (dotindex > charnum) {
                leftstring = filename.Substring(0, leftnum);
                rightstring = filename.Substring(filelength - rightnum, rightnum);
                if (repstring == "" || repstring.IsNull()) simplefilename = leftstring + rightstring + extname;
                else simplefilename = leftstring + repstring + rightstring + extname;
            } else simplefilename = fullname;

            return simplefilename;
        }
        /// <summary>
        /// 向页面输出xml内容
        /// </summary>
        /// <param name="xmlnode">xml内容</param>
        public static void WriteXMLEnd(this string xmlnode) {
            Msg.WriteXMLEnd(xmlnode);
        }
        /// <summary>
        /// 输出json内容
        /// </summary>
        /// <param name="json">string扩展</param>
        public static void WriteJSONEnd(this string json) {
            Msg.WriteJSONEnd(json);
        }
        /// <summary>
        /// 输出jsonp内容
        /// </summary>
        /// <param name="json">string扩展</param>
        public static void WriteJSONPEnd(this string json) {
            Msg.WriteJSONPEnd(json);
        }
        /// <summary>
        /// 输出内容 并结束
        /// </summary>
        /// <param name="text">string扩展</param>
        public static void WriteEnd(this string text) {
            Msg.WriteEnd(text);
        }
        /// <summary>
        /// 输出内容
        /// </summary>
        /// <param name="text">string扩展</param>
        public static void Write(this string text) {
            Msg.Write(text);
        }
        /// <summary>
        /// 删除最后N个字符
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="lastchar">最后一个字符串</param>
        public static string TrimEnd(this string str, string lastchar) {
            if (str.EndsWith(lastchar, true, null)) { return str.Substring(0, str.Length - lastchar.Length); }
            return str;
        }
        /// <summary>
        /// 删除最后N个字符
        /// </summary>
        /// <param name="str">string扩展</param>
        public static string TrimEnd(this string str) {
            if (!str.IsNullEmpty()) return str.Remove(str.Length - 1, 1);
            return string.Empty;
        }
        /// <summary>
        /// 删除最后N个字符
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="len">长度</param>
        public static string TrimEnd(this string str, int len) {
            if (str.IsNullEmpty()) return string.Empty;
            if (str.Length >= len) return str.Remove(str.Length - len, len);
            return str;
        }
        /// <summary>
        /// 删除前面N个字符
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="prevchar">最后一个字符串</param>
        public static string TrimStart(this string str, string prevchar) {
            if (str.StartsWith(prevchar, true, null)) { return str.Substring(prevchar.Length); }
            //int length = str.IndexOf(prevchar);
            //if (length > 0) { return str.Substring(length); }
            return str;
        }
        /// <summary>
        /// 删除前面N个字符
        /// </summary>
        /// <param name="str">string扩展</param>
        public static string TrimStart(this string str) {
            if (!str.IsNullEmpty()) return str.Remove(0, 1);
            return string.Empty;
        }
        /// <summary>
        /// 删除前面N个字符
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="len">长度</param>
        public static string TrimStart(this string str, int len) {
            if (str.IsNullEmpty()) return string.Empty;
            if (str.Length >= len) return str.Remove(0, len);
            return str;
        }
        /// <summary>
        /// 删除前面和后面N个字符
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="len">长度</param>
        public static string Trim(this string str, int len) {
            str = str.TrimStart(len);
            str = str.TrimEnd(len);
            return str;
        }
        /// <summary>
        /// 删除前面和后面N个字符
        /// </summary>
        /// <param name="str">string扩展</param>
        /// <param name="nchar">最后一个字符串</param>
        public static string Trim(this string str, string nchar) {
            str = str.TrimStart(nchar);
            str = str.TrimEnd(nchar);
            return str;
        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="fileName">文件路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="isOver">是否重写</param>
        public static void ToFile(this string str, string fileName, Encoding encoding, bool isOver = true) {
            if (isOver) FileDirectory.FileDelete(fileName);
            FileDirectory.FileWrite(fileName, str, encoding);
        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="fileName">文件路径</param>
        public static void ToFile(this string str, string fileName) {
            str.ToFile(fileName, Encoding.UTF8, true);
        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="fileName">文件路径</param>
        /// <param name="encoding">编码</param>
        public static void ToFile(this string str, string fileName, Encoding encoding) {
            str.ToFile(fileName, encoding, true);
        }
        /// <summary>
        /// CheckOnNullEmpty 检测是否为空，如果为空提示异常
        /// </summary>
        /// <param name="str">object扩展</param>
        /// <param name="parameterName">参数名</param>
        public static void CheckOnNullEmpty(this string str, string parameterName) {
            if (str.IsNullEmpty()) throw new ArgumentNullException(parameterName);
        }
        /// <summary>
        /// CheckOnNullEmpty 检测是否为空，如果为空提示异常
        /// </summary>
        /// <param name="str">object扩展</param>
        /// <param name="parameterName">参数名</param>
        /// <param name="message">消息</param>
        public static void CheckOnNullEmpty(this string str, string parameterName, string message) {
            if (str.IsNullEmpty()) throw new ArgumentNullException(parameterName, message);
        }
        /// <summary>
        /// 取HTML内容编码 正则 utf8 gb_2312 utf_8 zh_cn
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="defaultEncoding">Encoding</param>
        /// <returns></returns>
        public static Encoding GetHtmlEncoding(this string contentType, Encoding defaultEncoding) {
            Encoding encoding = defaultEncoding;
            System.Text.RegularExpressions.Match m = new System.Text.RegularExpressions.Regex("charset\\s*=\\s*(?<encode>(\\w*(-|_|\\.|)*\\w*)+)\\s*", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled).Match(contentType);
            string encodename = m.Groups["encode"].Value.ToLower().Trim();
            Hashtable encodingNames = new Hashtable();
            encodingNames.Add("utf8", "utf-8");
            encodingNames.Add("gb_2312", "gb2312");
            encodingNames.Add("'utf_8'", "utf-8");
            encodingNames.Add("zh_cn", "gbk");
            encodingNames.Add("utf_8", "utf-8");

            if (encodingNames.Contains(encodename)) encodename = encodingNames[encodename].ToString();
            if (encodename.Length > 0) encoding = Encoding.GetEncoding(encodename); else encoding = defaultEncoding;
            return encoding;
        }
        /// <summary>
        /// 去除json的空属性
        /// </summary>
        /// <param name="json">json</param>
        /// <returns></returns>
        public static string RemoveJsonNull(this string json) {
            //return System.Text.RegularExpressions.Regex.Replace(json, @",?""\w*"":null,?", string.Empty);
            json = System.Text.RegularExpressions.Regex.Replace(json, @",""\w*"":null", string.Empty);
            json = System.Text.RegularExpressions.Regex.Replace(json, @"""\w*"":null,", string.Empty);
            json = System.Text.RegularExpressions.Regex.Replace(json, @"""\w*"":null", string.Empty);
            return json;
        }
        /// <summary>
        /// 去除xml中的空节点
        /// </summary>
        /// <param name="xml">xml</param>
        /// <returns>整理过的xml字符串</returns>
        public static string RemoveEmptyNodes(string xml) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeList nodes = doc.SelectNodes("//node()");

            foreach (XmlNode node in nodes) if (node.ChildNodes.Count == 0 && node.InnerText == string.Empty) node.ParentNode.RemoveChild(node);
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            xw.Indentation = 2;
            doc.PreserveWhitespace = true;
            doc.WriteTo(xw);
            xml = sw.ToString();
            sw.Close();
            xw.Close();
            return xml;
        }
        /// <summary>
        /// 如果长等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="length">长度</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfLengthEqual(this string obj, int length, string defaultValue) {
            return !obj.IsNullEmpty() && obj.Length == length ? defaultValue : obj;
        }
        /// <summary>
        /// 如果长不等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="length">长度</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfLengthNotEqual(this string obj, int length, string defaultValue) {
            return !obj.IsNullEmpty() && obj.Length != length ? defaultValue : obj;
        }
        /// <summary>
        /// 如果长大于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="length">长度</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfLengthMoreThan(this string obj, int length, string defaultValue) {
            return !obj.IsNullEmpty() && obj.Length > length ? defaultValue : obj;
        }
        /// <summary>
        /// 如果长小于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="length">长度</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfLengthLessThan(this string obj, int length, string defaultValue) {
            return !obj.IsNullEmpty() && obj.Length < length ? defaultValue : obj;
        }
        /// <summary>
        /// 如果长大于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="length">长度</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfLengthMoreThanOrEqual(this string obj, int length, string defaultValue) {
            return !obj.IsNullEmpty() && obj.Length >= length ? defaultValue : obj;
        }
        /// <summary>
        /// 如果长小于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="length">长度</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfLengthLessThanOrEqual(this string obj, int length, string defaultValue) {
            return !obj.IsNullEmpty() && obj.Length <= length ? defaultValue : obj;
        }
        /// <summary>
        /// 如果等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfEqual(this string obj, string value, string defaultValue) {
            return !obj.IsNullEmpty() && obj == value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果不等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfNotEqual(this string obj, string value, string defaultValue) {
            return !obj.IsNullEmpty() && obj != value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果空
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfNullOrEmpty(this string obj, string defaultValue) {
            return obj.IsNullEmpty() ? defaultValue : obj;
        }
        /// <summary>
        /// 如果非空
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IfNotNullOrEmpty(this string obj, string defaultValue) {
            return !obj.IsNullEmpty() ? defaultValue : obj;
        }
        /// <summary>
        /// 删除结尾处指定字符串以后的字符
        /// </summary>
        /// <param name="obj">源字符串</param>
        /// <param name="end">存在的字符串</param>
        /// <returns>删除结尾处指定字符串以后的字符</returns>
        public static string TrimIndexEnd(this string obj, string end) {
            int len = obj.LastIndexOf(end);
            if (len == -1) return obj;
            return obj.Substring(0, len);
        }
        /// <summary>
        /// 删除行首处指定字符串以前的字符
        /// </summary>
        /// <param name="obj">源字符串</param>
        /// <param name="start">存在的字符串</param>
        /// <returns>删除行首处指定字符串以前的字符</returns>
        public static string TrimIndexStart(this string obj, string start) {
            int len = obj.IndexOf(start);
            if (len == -1) return obj;
            if (obj.Length == start.Length) return string.Empty;
            return obj.Substring(len + start.Length);
        }
        /// <summary>
        /// unicode ascii码转中文 Native2Ascii反操作
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string Ascii2Native(this string str) {
            string outStr = "";
            if (!string.IsNullOrEmpty(str)) {
                string[] strlist = str.Replace("\\", "").Split('u');
                for (int i = 1; i < strlist.Length; i++) outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
            }
            return outStr.Trim();
        }
        /// <summary>
        /// 中文转unicode ascii码 Ascii2Native反操作
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string Native2Ascii(this string str) {
            string outStr = "";
            if (!string.IsNullOrEmpty(str)) {
                for (int i = 1; i < str.Length; i++) outStr += "\\u" + ((int)str[i]).ToString("x");
            }
            return outStr.Trim();
        }
        /// <summary>
        /// 16进制字节转字符串
        /// </summary>
        /// <param name="s">16进制字节</param>
        /// <returns>字符串</returns>
        public static byte[] To16Bytes(this string s) {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2) buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        /// <summary>
        /// ToStr()
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static string ToStr(this string strValue) { return strValue.IsNull() || strValue.IsDBNull() || strValue.IsNullEmpty() ? string.Empty : strValue.ShowSafeSql(); }
        /// <summary>
        /// ToStr()
        /// </summary>
        /// <param name="strValue">string扩展</param>
        /// <returns></returns>
        public static string ToStr2(this string strValue) { return strValue.IsNull() || strValue.IsDBNull() || strValue.IsNullEmpty() ? string.Empty : strValue.ShowSafeSql2(); }
        /// <summary>
        /// 读取文件成字节
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this string fileName) {
            return File.ReadAllBytes(fileName);
        }
        /// <summary>
        /// 字符串转日期
        /// </summary>
        /// <param name="date">string扩展</param>
        /// <param name="format">format</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string date, string format) {
            return DateTime.ParseExact(date, format, System.Globalization.CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// string转IDictionary ToDictionary("=","&amp;")
        /// </summary>
        /// <param name="parameters">string</param>
        /// <param name="split1">分隔符1</param>
        /// <param name="split2">分隔符2</param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(this string parameters, string split1 = "=", string split2 = "&") {
            IDictionary<string, string> list = new Dictionary<string, string>();
            if (parameters.IsNullEmpty()) return list;
            string[] list1 = parameters.Split(split2);
            string key = string.Empty; string value = string.Empty;
            foreach (string info in list1) {
                string[] list2 = info.Split(split1);
                if (list2.Length == 1) {
                    key = "";
                    value = list2[0];
                } else {
                    key = list2[0];
                    value = list2[1];
                }
                if (list.ContainsKey(key)) list[key] = value; else list.Add(key, value);
            }
            return list;
        }
        /// <summary>
        /// 过滤JSON “”｛｝【】“ ：
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CheckJson(this string str) {
            return str.Replace("\"\"", "\" \"").Replace("{}", "{ }").Replace("[]", "[ ]").Replace("\" : ", "\":").Replace("\" :", "\":");
        }
        /// <summary>
        /// 提取JSON指定key的值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetJsonString(this string json, string key) {
            return json.GetMatchingValue("\"{0}\":\"(.+?)\"".FormatWith(key), key.Length + 4, 1);
        }
        /// <summary>
        /// 提取JSON指定key的值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static int GetJsonInt(this string json, string key) {
            return json.GetMatchingValue("\"{0}\":(-?\\d+)".FormatWith(key), key.Length + 3, 0).ToInt(0);
        }
        /// <summary>
        /// 提取JSON指定key的值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static bool GetJsonBool(this string json, string key) {
            return json.GetMatchingValue("\"{0}\":(true|false)".FormatWith(key), key.Length + 3, 0).ToBool(false);
        }
        /// <summary>
        /// 提取JSON指定key的值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static long GetJsonLong(this string json, string key) {
            return json.GetMatchingValue("\"{0}\":(-?\\d+)".FormatWith(key), key.Length + 3, 0).ToBigInt(0);
        }
        /// <summary>
        /// 提取JSON指定key的值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static decimal GetJsonDecimal(this string json, string key) {
            return json.GetMatchingValue("\"{0}\":(-?(0|\\d+)(\\.\\d+)?)".FormatWith(key), key.Length + 3, 0).ToDecimal(0);
        }
        /// <summary>
        /// 提取JSON指定key的值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetJsonObject(this string json, string key) {
            return json.GetMatchingValue("\"{0}\":{1}(.+?){2}".FormatWith(key, "{", "}"), key.Length + 3, 0);
        }
        /// <summary>
        /// 提取JSON指定key的值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetJsonArray(this string json, string key) {
            return json.GetMatchingValue("\"{0}\":[(.+?)]".FormatWith(key), key.Length + 3, 0);
        }
        /// <summary>
        /// 提取JSON指定key的值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string[] GetJsonArrays(this string json, string key) {
            return json.GetMatchingValue("\"{0}\":[(.+?)]".FormatWith(key), key.Length + 4, 1).Split(',');
        }
        /// <summary>
        /// 提取XML指定节点的内容
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns></returns>
        public static IList<string> GetXmlNodeText(this string xml, string nodeName) {
            string reg = "<{0}>(.+?)</{0}>".FormatWith(nodeName);
            IList<string> list = new List<string>();
            xml.GetMatchingValues(reg, "<{0}>".FormatWith(nodeName), "</{0}>".FormatWith(nodeName)).Do(p => list.Add(p.Trim()));
            if (list.Count == 0) {
                reg = "<{0}.+?>(.+?)</{0}>".FormatWith(nodeName);
                xml.GetMatchingValues(reg, "<{0}".FormatWith(nodeName), "</{0}>".FormatWith(nodeName)).Do(p => list.Add(p.Substring(p.IndexOf(">") + 1).Trim()));
            }
            return list;
        }
        /// <summary>
        /// 提取XML指定节点的属性值
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">属性名称</param>
        /// <returns></returns>
        public static IList<string> GetXmlNodeAttrString(this string xml, string nodeName, string attrName) {
            xml = xml.Replace("\"\"", "\" \"").Replace("= \"", "=\"").Replace(" = \"", "=\"").Replace("\">", "\" >");
            string reg = "<{0}(.+?)>".FormatWith(nodeName);
            string reg2 = "{0}=\"(.+?)\"".FormatWith(attrName);
            IList<string> list = new List<string>();
            xml.GetMatchingValues(reg, "<{0}".FormatWith(nodeName), ">").Do(p => {
                p.GetMatchingValues(reg2, "{0}=\"".FormatWith(attrName), "\"").Do(v => list.Add(v.Trim()));
            });
            return list;
        }
        /// <summary>
        /// 提取XML指定节点的属性值
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">属性名称</param>
        /// <returns></returns>
        public static IList<int> GetXmlNodeAttrInt(this string xml, string nodeName, string attrName) {
            IList<string> list = xml.GetXmlNodeAttrString(nodeName, attrName);
            if (list.Count == 0) {
                xml = xml.Replace("\"\"", "\" \"").Replace("= \"", "=\"").Replace(" = \"", "=\"").Replace("\">", "\" >");
                string reg = "<{0}(.+?)>".FormatWith(nodeName);
                string reg2 = "{0}=(-?\\d+)".FormatWith(attrName);
                xml.GetMatchingValues(reg, "<{0}".FormatWith(nodeName), ">").Do(p => {
                    p.GetMatchingValues(reg2, "{0}=".FormatWith(attrName), ",").Do(v => list.Add(v.Trim()));
                });
            }
            IList<int> intlist = new List<int>();
            list.Do(p => intlist.Add(p.ToInt(0)));
            return intlist;
        }
        /// <summary>
        /// 提取XML指定节点的属性值
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">属性名称</param>
        /// <returns></returns>
        public static IList<long> GetXmlNodeAttrLong(this string xml, string nodeName, string attrName) {
            IList<string> list = xml.GetXmlNodeAttrString(nodeName, attrName);
            if (list.Count == 0) {
                xml = xml.Replace("\"\"", "\" \"").Replace("= \"", "=\"").Replace(" = \"", "=\"").Replace("\">", "\" >");
                string reg = "<{0}(.+?)>".FormatWith(nodeName);
                string reg2 = "{0}=(-?\\d+)".FormatWith(attrName);
                xml.GetMatchingValues(reg, "<{0}".FormatWith(nodeName), ">").Do(p => {
                    p.GetMatchingValues(reg2, "{0}=".FormatWith(attrName), ",").Do(v => list.Add(v.Trim()));
                });
            }
            IList<long> longlist = new List<long>();
            list.Do(p => longlist.Add(p.ToBigInt(0)));
            return longlist;
        }
        /// <summary>
        /// 提取XML指定节点的属性值
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">属性名称</param>
        /// <returns></returns>
        public static IList<decimal> GetXmlNodeAttrDecimal(this string xml, string nodeName, string attrName) {
            IList<string> list = xml.GetXmlNodeAttrString(nodeName, attrName);
            if (list.Count == 0) {
                xml = xml.Replace("\"\"", "\" \"").Replace("= \"", "=\"").Replace(" = \"", "=\"").Replace("\">", "\" >");
                string reg = "<{0}(.+?)>".FormatWith(nodeName);
                string reg2 = "{0}=(-?(0|\\d+)(\\.\\d+)?)".FormatWith(attrName);
                xml.GetMatchingValues(reg, "<{0}".FormatWith(nodeName), ">").Do(p => {
                    p.GetMatchingValues(reg2, "{0}=".FormatWith(attrName), ",").Do(v => list.Add(v.Trim()));
                });
            }
            IList<decimal> decimallist = new List<decimal>();
            list.Do(p => decimallist.Add(p.ToDecimal(0)));
            return decimallist;
        }
        /// <summary>
        /// base64转字节
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] FromBase64(this string str) { return Convert.FromBase64String(str); }
        /// <summary>
        /// base64转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string FromBase64(this string str, Encoding encoding = null) { return (encoding.IsNull() ? Encoding.UTF8 : encoding).GetString(str.FromBase64()); }
        /// <summary>
        /// 分解;分隔字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="split">分隔字符串</param>
        /// <returns></returns>
        public static string[] ToStrings(this string str, char split = ';') {
            str = str.Trim().Trim(';').Trim();
            if (str.IsNullEmpty()) return new string[] { };
            return str.Split(split);
        }
        /// <summary>
        /// 正则字符转意
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveRegexChar(this string str) {
            return str.Replace(".", "\\.").Replace("*", "\\*").Replace("?", "\\?")
                .Replace("\\", "\\\\").Replace("^", "\\^").Replace("$", "\\$")
                .Replace("+", "\\+").Replace("|", "\\|").Replace("(", "\\(").Replace(")", "\\)")
                .Replace("[", "\\[").Replace("]", "\\]");
        }
        /// <summary>
        /// 是否encode编码
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool IsEncode(this char chr) {
            if (chr > 127) return true;
            if (char.IsLetterOrDigit(chr) || "$-_.+!*'(),@=&".IndexOf(chr) >= 0) return false;
            return true;
        }
        /// <summary>
        /// 是否encode编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEncode(this string str) {
            foreach (char s in str) {
                if (!IsEncode(s)) return false;
            }
            return true;
        }
        /// <summary>
        /// 是否物理路径
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPhysicalPath(this string s) {
            string text1 = @"^\s*[a-zA-Z]:.*$";
            return Regex.IsMatch(s, text1);
        }
        /// <summary>
        /// 是否相对路径
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsRelativePath(this string s) {
            if ((s == null) || (s == "")) return false;
            if (s.StartsWith("/") || s.StartsWith("?")) return false;
            if (Regex.IsMatch(s, @"^\s*[a-zA-Z]{1,10}:.*$")) return false;
            return true;
        }
        /// <summary>
        /// ToSQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SQL ToSQL(this string sql) {
            return new SQL(sql);
        }
        /// <summary>
        /// SecureString to String
        /// </summary>
        /// <param name="s">SecureString</param>
        /// <returns>String</returns>
        public static string ToStringBySecureString(this SecureString s) {
            string str = string.Empty;
            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = Marshal.SecureStringToBSTR(s);
                str = Marshal.PtrToStringBSTR(ptr);
            } finally {
                Marshal.ZeroFreeBSTR(ptr);
            }
            return str;
        }
    }
}
