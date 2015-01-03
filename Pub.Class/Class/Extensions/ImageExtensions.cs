//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Web.Script.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;

namespace Pub.Class {
    /// <summary>
    /// Image扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class ImageExtensions {
        /// <summary>
        /// 添加边框
        /// </summary>
        /// <param name="image">Image扩展</param>
        /// <param name="borderWidth">边框宽</param>
        /// <param name="color">边框颜色</param>
        /// <returns></returns>
        public static Image AppendBorder(this Image image, int borderWidth, Color color) {
            var newSize = new Size(image.Width + (borderWidth * 2), image.Height + (borderWidth * 2));
            var img = new Bitmap(newSize.Width, newSize.Height);
            var g = Graphics.FromImage(img);
            g.Clear(color);
            g.DrawImage(image, new Point(borderWidth, borderWidth));
            g.Dispose();
            return img;
        }
        /// <summary>
        /// 马赛克效果 原理：确定图像的随机位置点和确定马赛克块的大小,然后马赛克块图像覆盖随机点即可.
        /// </summary>
        /// <param name="original">Bitmap扩展</param>
        /// <param name="scale">分割成val*val像素的小区块</param>
        /// <returns></returns>
        public static Bitmap ToMosaic(this Bitmap original, int scale) {
            Bitmap result = new Bitmap(original.Width * scale, original.Height * scale);
            using (Graphics g = Graphics.FromImage(result)) {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(original, 0, 0, result.Width, result.Height);
            }
            return result;
        }
    }
}
