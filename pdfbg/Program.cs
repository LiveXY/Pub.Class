using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace pdfbg {
    public static class exts {
        /// <summary>
        /// 取物理路径
        /// </summary>
        /// <param name="strPath">string扩展</param>
        /// <returns></returns>
        public static string GetMapPath(this string strPath) {
            if (HttpContext.Current != null)
                return HttpContext.Current.Server.MapPath(strPath);
            else {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith(".\\")) strPath = strPath.Substring(2);
                strPath = strPath.TrimStart('~').TrimStart('\\');
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        public static int ToInt(this string strValue, int defValue) { int def = 0; int.TryParse(strValue, out def); return def == 0 ? defValue : def; }
    }
    class Program {
        static void Main(string[] args) {
            //Console.WriteLine(args.Length);
            if (args.Length != 4) { Console.WriteLine("参数错误，需要源文件、水印图片、输出文件、水印显示位置4个参数！"); Environment.Exit(1); }

            Regex fileMatch = new Regex(@"\.(pdf)$", RegexOptions.IgnoreCase);
            if (fileMatch.Matches(args[0]).Count != 1 || fileMatch.Matches(args[2]).Count != 1) { Console.WriteLine("源文件和输出文件必须是PDF文件！"); Environment.Exit(1); }

            fileMatch = new Regex(@"\.(jpg|gif|png)$", RegexOptions.IgnoreCase);
            if (fileMatch.Matches(args[1]).Count != 1) { Console.WriteLine("水印图片必须是jpg|gif|png图片文件！"); Environment.Exit(1); }

            String inputFile;
            String outputFile;
            String imageFile;

            FileInfo info = new FileInfo(args[0]);
            if (info == null || !info.Exists) { Console.WriteLine("源文件不存在"); Environment.Exit(1); }
            inputFile = info.FullName;

            FileInfo imgInfo = new FileInfo("".GetMapPath() + args[1]);
            if (imgInfo == null || !imgInfo.Exists) { Console.WriteLine("水印图片不存在：" + args[1]); Environment.Exit(1); }
            imageFile = imgInfo.FullName;

            FileInfo outputInfo = new FileInfo(args[2]);
            if (outputInfo != null && outputInfo.Exists) System.IO.File.Delete(outputInfo.FullName);

            if (!System.IO.Directory.Exists(outputInfo.DirectoryName)) { Console.WriteLine("输出文件目录不存在！"); Environment.Exit(1); }
            outputFile = outputInfo.FullName;

            int type = args[3].ToInt(0);

            var tempFile = System.IO.Path.GetTempFileName();
            //Console.WriteLine(inputFile);
            //Console.WriteLine(imageFile);
            //Console.WriteLine(outputFile);
            try {
                ImageBackground b = new ImageBackground();
                b.SetBackground(inputFile, tempFile, System.Drawing.Image.FromFile(imageFile), type);
                File.Copy(tempFile, outputFile);
                //Process.Start(dialog.FileName);
            } catch (Exception ex) {
                throw ex;
            } finally {
                File.Delete(tempFile);
            }

            Environment.Exit(0);
        }
    }
}
