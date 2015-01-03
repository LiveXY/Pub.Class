//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;

namespace Pub.Class {
    /// <summary>
    /// 生成随机数类
    /// 
    /// 修改纪录
    ///     2006.05.09 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Rand {
        //#region RndInt
        /// <summary>
        /// 数字随机数
        /// </summary>
        /// <param name="num1">开始</param>
        /// <param name="num2">结束</param>
        /// <returns>从多少到多少之间的数据 包括开始不包括结束</returns>
        public static int RndInt(int num1, int num2) {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.Next(num1, num2);
        }
        /// <summary>
        /// 数字随机数 列表
        /// </summary>
        /// <param name="num1">开始</param>
        /// <param name="num2">结束</param>
        /// <param name="len">长</param>
        /// <returns>数字随机数 列表</returns>
        public static IList<int> RndInt(int num1, int num2, int len) {
            IList<int> list = new List<int>();
            for (int i = 0; i < len; i++) list.Add(RndInt(num1, num2));
            return list;
        }
        /// <summary>
        /// 数字随机数 列表
        /// </summary>
        /// <param name="len">长</param>
        /// <returns>数字随机数 列表</returns>
        public static IList<int> RndInt(int len) {
            IList<int> list = RndInt(0, int.MaxValue, len);
            return list;
        }
        //#endregion
        //#region RndNum
        /// <summary>
        /// 数字随机数
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <returns>返回指定长度的数字随机串</returns>
        public static string RndNum(int len) {
            char[] arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < len; i++) {
                num.Append(arrChar[rnd.Next(0, 9)].ToString());
            }
            return num.ToString();
        }
        /// <summary>
        /// 数字随机数列表
        /// </summary>
        /// <param name="len">长</param>
        /// <param name="count">行</param>
        /// <returns>数字随机数列表</returns>
        public static IList<string> RndNum(int len, int count) {
            IList<string> list = new List<string>();
            for (int i = 0; i < count; i++) list.Add(RndNum(len));
            return list;
        }
        //#endregion
        //#region RndDateStr
        /// <summary>
        /// 日期随机函数
        /// </summary>
        /// <returns>返回日期随机串</returns>
        public static string RndDateStr() {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + Rand.RndInt(1000, 9999).ToString();
        }
        /// <summary>
        /// 日期随机函数列表
        /// </summary>
        /// <param name="len">长</param>
        /// <returns>日期随机函数列表</returns>
        public static IList<string> RndDateStr(int len) {
            IList<string> list = new List<string>();
            for (int i = 0; i < len; i++) list.Add(RndDateStr());
            return list;
        }
        //#endregion
        //#region RndCode
        /// <summary>
        /// 数字和字母随机数
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <returns>返回指定长度的数字和字母的随机串</returns>
        public static string RndCode(int len) {
            char[] arrChar = new char[]{
               'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
               '0','1','2','3','4','5','6','7','8','9',
               'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'};
            System.Text.StringBuilder num = new System.Text.StringBuilder();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < len; i++) {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }
        /// <summary>
        /// 数字和字母随机数表列
        /// </summary>
        /// <param name="len">长</param>
        /// <param name="count">行</param>
        /// <returns>数字和字母随机数表列</returns>
        public static IList<string> RndCodeList(int len, int count) {
            IList<string> list = new List<string>();
            for (int i = 0; i < count; i++) list.Add(RndCode(len));
            return list;
        }
        //#endregion
        //#region RndLetter
        /// <summary>
        /// 字母随机数
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <returns>返回指定长度的字母随机数</returns>
        public static string RndLetter(int len) {
            char[] arrChar = new char[]{
                'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
                '_',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'};
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < len; i++) {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }
        /// <summary>
        /// 字母随机数列表
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <param name="count">行</param>
        /// <returns>字母随机数列表</returns>
        public static IList<string> RndLetterList(int len, int count) {
            IList<string> list = new List<string>();
            for (int i = 0; i < count; i++) list.Add(RndLetter(len));
            return list;
        }
        //#endregion
        //#region RndGuid
        /// <summary>
        /// 生成GUID
        /// </summary>
        /// <returns>生成GUID</returns>
        public static string RndGuid() {
            return Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 生成GUID 列表
        /// </summary>
        /// <param name="len">长</param>
        /// <returns>生成GUID 列表</returns>
        public static IList<string> RndGuid(int len) {
            IList<string> list = new List<string>();
            for (int i = 0; i < len; i++) list.Add(RndGuid());
            return list;
        }
        //#endregion
        //#region RndColor
        /// <summary>
        /// 随机颜色
        /// </summary>
        /// <param name="iUseAlpha">使用透明</param>
        /// <returns>随机颜色</returns>
        public static Color RndColor(bool iUseAlpha) {
            int vAlpha = 255;
            if (!iUseAlpha) vAlpha = RndInt(0, 255);
            int vRed = RndInt(0, 255);
            int vBlue = RndInt(0, 255);
            int vGreen = RndInt(0, 255);
            Color vColor = Color.FromArgb(vAlpha, vRed, vGreen, vBlue);
            return vColor;
        }
        /// <summary>
        /// 随机颜色字符串
        /// </summary>
        /// <returns>随机颜色字符串</returns>
        public static string RandColor() {
            string vRed = Convert.ToString(RndInt(0, 255), 16); vRed = vRed.Length == 1 ? "0" + vRed : vRed;
            string vBlue = Convert.ToString(RndInt(0, 255), 16); vBlue = vBlue.Length == 1 ? "0" + vBlue : vBlue;
            string vGreen = Convert.ToString(RndInt(0, 255), 16); vGreen = vGreen.Length == 1 ? "0" + vGreen : vGreen;
            return vRed + vBlue + vGreen;
        }
        /// <summary>
        /// 随机颜色字符串列表
        /// </summary>
        /// <param name="len">长</param>
        /// <returns>随机颜色字符串列表</returns>
        public static IList<string> RandColor(int len) {
            IList<string> list = new List<string>();
            for (int i = 0; i < len; i++) list.Add(RandColor());
            return list;
        }
        //#endregion
    }
}
