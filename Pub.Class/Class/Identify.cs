//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.IO;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Web;

namespace Pub.Class {
    /// <summary>
    /// 验证码
    /// 
    /// 修改纪录
    ///     2007.02.04 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Identify {
        //#region DrawIdentifyCode
        /// <summary>
        /// 验证码绘图
        /// <example>
        /// <code>
        ///     string strIdentifyCode = Pub.Class.Identify.IdentifyCode(4);
        ///     Pub.Class.Identify.DrawIdentifyCode(strIdentifyCode, 50, 100);
        ///     Response.End();
        ///     在登录页面引用此文件:&lt;img src="Identify.aspx" border="0" style="cursor: pointer;cursor:hand;" onclick="javascript:this.src='Identify.aspx?iTime=' + Math.random();" title="单击可更换新的验证码" />
        ///     Session["IdentifyCode"]
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="strIdentifyCode">验证码</param>
        /// <param name="intFgNoise">文字噪音程度</param>
        /// <param name="intBgNoise">背景噪音程度</param>
        public static void DrawIdentifyCode(string strIdentifyCode, int intFgNoise, int intBgNoise) {
            if (strIdentifyCode.IsNull() || strIdentifyCode.Trim() == String.Empty) { return; } else {
                Bitmap bmpImage = new Bitmap((int)Math.Ceiling((strIdentifyCode.Length * 12.5)), 22);//建立一个位图文件 确立长宽
                Graphics grpGraphics = Graphics.FromImage(bmpImage);

                try {
                    Random rndRandom = new Random();//生成随机生成器
                    grpGraphics.Clear(Color.White);//清空图片背景色

                    for (int i = 0; i < intBgNoise; i++) {//画图片的背景噪音线
                        int int_x1 = rndRandom.Next(bmpImage.Width);
                        int int_x2 = rndRandom.Next(bmpImage.Width);
                        int int_y1 = rndRandom.Next(bmpImage.Height);
                        int int_y2 = rndRandom.Next(bmpImage.Height);

                        grpGraphics.DrawLine(new Pen(Color.Silver), int_x1, int_y1, int_x2, int_y2);
                    }
                    Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));//把产生的随机数以字体的形式写入画面
                    LinearGradientBrush brhBrush = new LinearGradientBrush(new Rectangle(0, 0, bmpImage.Width, bmpImage.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                    grpGraphics.DrawString(strIdentifyCode, font, brhBrush, 2, 2);

                    for (int i = 0; i < intFgNoise; i++) {//画图片的前景噪音点
                        int int_x = rndRandom.Next(bmpImage.Width);
                        int int_y = rndRandom.Next(bmpImage.Height);

                        bmpImage.SetPixel(int_x, int_y, Color.FromArgb(rndRandom.Next()));
                    }

                    grpGraphics.DrawRectangle(new Pen(Color.Silver), 0, 0, bmpImage.Width - 1, bmpImage.Height - 1);//画图片的边框线
                    MemoryStream memsMemoryStream = new MemoryStream();
                    bmpImage.Save(memsMemoryStream, ImageFormat.Gif);
                    System.Web.HttpContext.Current.Response.ClearContent();
                    System.Web.HttpContext.Current.Response.ContentType = "image/Gif";
                    System.Web.HttpContext.Current.Response.BinaryWrite(memsMemoryStream.ToArray());
                } finally {
                    grpGraphics.Dispose();
                    bmpImage.Dispose();
                }
            }
        }
        /// <summary>
        /// 运算验证码绘图
        /// </summary>
        /// <param name="checkCode">验证码</param>
        public static void DrawIdentifyCode2(string checkCode) {
            if (checkCode.IsNull() || checkCode.Trim() == String.Empty) return;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 20.5)), 28);
            Graphics g = Graphics.FromImage(image);
            try {
                //生成随机生成器 
                Random random = new Random();
                //清空图片背景色 
                g.Clear(Color.White);
                //画图片的背景噪音线 
                Font font = new System.Drawing.Font("Arial", 14, (System.Drawing.FontStyle.Bold));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                //画图片的边框线 
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ContentType = "image/Gif";
                HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            } finally {
                g.Dispose();
                image.Dispose();
            }
        }
        //#endregion
        //#region IdentifyCode
        /// <summary>
        /// 取得随机字符串，并设置Session值
        /// </summary>
        /// <example>
        /// <code>
        ///     string strIdentifyCode = Pub.Class.Identify.IdentifyCode(4);
        ///     Pub.Class.Identify.DrawIdentifyCode(strIdentifyCode, 50, 100);
        ///     Response.End();
        ///     在登录页面引用此文件:&lt;img src="Valid.aspx"  border="0" />
        ///     Session["IdentifyCode"]
        /// </code>
        /// </example>
        /// <param name="intLength">长</param>
        /// <returns>取得随机字符串</returns>
        public static string IdentifyCode(int intLength) {
            int intNumber;
            char chrCode;
            string strIdentifyCode = String.Empty;
            Random rndRandom = new Random();
            for (int i = 0; i < intLength; i++) {
                intNumber = rndRandom.Next();
                if (intNumber % 2 == 0) {
                    chrCode = (char)('0' + (char)(intNumber % 10));//如果随机数是偶数 取余
                } else {
                    chrCode = (char)('A' + (char)(intNumber % 26));//如果随机数是奇数 选择从[A-Z]
                }
                strIdentifyCode += chrCode.ToString();
            }
            Cookie2.Set("IdentifyCode", strIdentifyCode);
            return strIdentifyCode;
        }
        /// <summary>
        /// 取得运算随机字符串，并设置Session值
        /// </summary>
        public static void IdentifyCode2() {
            string[] number1 = new string[] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
            string[] number2 = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            string[] operate = new string[] { "加", "减" };
            string[] equals = new string[] { "=", "等于" };

            IList<string[]> number = new List<string[]>();
            number.Add(number1);
            number.Add(number2);

            Random r = new Random();
            int num1 = r.Next(1, 11);
            int num2 = r.Next(1, 11);
            int result = 0;

            int operation = r.Next(0, 2);

            if (operation == 0) {
                result = num1 + num2;
            } else {
                if (num1 < num2) {
                    int num3 = 0;

                    num3 = num1;
                    num1 = num2;
                    num2 = num3;
                }
                result = num1 - num2;
            }

            string checkCode = string.Empty;

            checkCode = ((string[])number[r.Next(0, 2)])[num1 - 1];
            checkCode += operate[operation];
            checkCode += ((string[])number[r.Next(0, 2)])[num2 - 1];
            checkCode += equals[r.Next(0, 2)];
            checkCode += "?";

            Cookie2.Set("IdentifyCode", result.ToString());

            DrawIdentifyCode2(checkCode);
        }
        //#endregion
    }
}