//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Web;
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// 上传文件类
    /// 
    /// 修改纪录
    ///     2006.05.13 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Upload {
        //#region 随机类型 enum
        /// <summary>
        /// 随机类型
        /// </summary>
        public enum RandFileType {
            /// <summary>
            /// 不随机
            /// </summary>
            None = 0,
            /// <summary>
            /// 按日期随机
            /// </summary>
            FileName_DateTime,
            /// <summary>
            /// 按数字随机
            /// </summary>
            FileName_RandNumber,
            /// <summary>
            /// 日期随机
            /// </summary>
            DateTime
        }
        //#endregion
        //#region 私有成员
        private int _State = 0;
        private int _fileSize = 0;//KB
        private string _fileName = "";
        private string _allowedExtensions = ".gif|.png|.jpg|.bmp";//".xls";
        private string _filePath = "~/";
        private double _maxFileSize = 4000;
        private bool _isCreateFolderForNotExits = false;
        private int _RandNumbers = 5;
        private RandFileType _RandFileType = RandFileType.None;
        //#endregion
        //#region 属性
        /// <summary>
        /// 允许上传的扩展名
        /// </summary>
        public string AllowedExtensions { set { _allowedExtensions = value; } }
        /// <summary>
        /// 文件保存到哪
        /// </summary>
        public string FilePath { set { _filePath = value; } }
        /// <summary>
        /// 最大可上传文件大小
        /// </summary>
        public int MaxFileSize { set { _maxFileSize = value; } }
        /// <summary>
        /// 随机数
        /// </summary>
        public int RandNumbers { set { _RandNumbers = value; } }
        /// <summary>
        /// 是否自动新建不存在的文件目录
        /// </summary>
        public bool isCreateFolderForNotExits { set { _isCreateFolderForNotExits = value; } }
        /// <summary>
        /// 文件随机类型
        /// </summary>
        public RandFileType RndFileType { set { _RandFileType = value; } }
        /// <summary>
        /// 返回文件大小
        /// </summary>
        public int FileSize { get { return _fileSize; } }
        /// <summary>
        /// 返回文件名
        /// </summary>
        public string FileName { get { return _fileName; } }
        /// <summary>
        /// 返回操作状态
        /// </summary>
        public int State { get { return _State; } }
        //#endregion
        //#region 构造器
        /// <summary>
        /// 构造器
        /// </summary>
        public Upload() { }
        //#endregion
        //#region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileUpload">fileUpload组件</param>
        public void DoUpLoad(System.Web.UI.WebControls.FileUpload fileUpload) {
            string __filePath = HttpContext.Current.Server.MapPath(_filePath);
            if (!System.IO.Directory.Exists(__filePath)) {
                if (_isCreateFolderForNotExits) {
                    FileDirectory.DirectoryVirtualCreate(_filePath);
                } else { _State = 3; return; }
            }
            if (fileUpload.PostedFile.ContentLength / 1024 > _maxFileSize) { _State = 4; return; }
            string randStr = "";
            switch (_RandFileType) {
                case RandFileType.None: randStr = ""; break;
                case RandFileType.FileName_DateTime: randStr = "_" + Rand.RndDateStr(); break;
                case RandFileType.FileName_RandNumber: randStr = "_" + Rand.RndCode(_RandNumbers); break;
                case RandFileType.DateTime: randStr = Rand.RndDateStr(); break;
            }
            bool isTrue = false;
            if (fileUpload.HasFile) {
                string _fileExt = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();
                string[] _allowedExt = _allowedExtensions.Split(new string[] { "|" }, StringSplitOptions.None);
                for (int i = 0; i < _allowedExt.Length; i++) {
                    if (_fileExt == _allowedExt[i]) { isTrue = true; }
                }
                if (isTrue) {
                    try {
                        string fNameNoExt = System.IO.Path.GetFileNameWithoutExtension(fileUpload.FileName);
                        if (_RandFileType == RandFileType.DateTime) fNameNoExt = "";
                        fileUpload.SaveAs(__filePath + fNameNoExt + randStr + System.IO.Path.GetExtension(fileUpload.FileName));
                        _State = 0;
                        _fileSize = fileUpload.PostedFile.ContentLength / 1024;
                        _fileName = _filePath + fNameNoExt + randStr + System.IO.Path.GetExtension(fileUpload.FileName);
                    } catch { _State = 2; }
                } else { _State = 1; }
            } else { _State = 2; }
        }
        //#endregion
    }
}
