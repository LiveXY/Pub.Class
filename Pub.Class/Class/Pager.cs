//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// 分页显示类
    /// 
    /// 修改纪录
    ///     2006.05.09 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Pager {
        //#region 对齐方式 enum
        /// <summary>
        /// 对齐方式
        /// </summary>
        public enum Align {
            /// <summary>
            /// 左对齐
            /// </summary>
            Left = 0,
            /// <summary>
            /// 居中
            /// </summary>
            Center,
            /// <summary>
            /// 右对齐
            /// </summary>
            Right
        }
        //#endregion
        //#region 私有成员
        private string _Url;
        private int _PageIndex;
        private int _TotalPage;
        private int _PageSize;
        private int _TotalRecord;
        private string _StyleName;
        private Align _Align;
        private int _Index = 1;
        private StringBuilder sbData = new StringBuilder();
        //#endregion
        //#region 属性
        /// <summary>
        /// URL
        /// </summary>
        public string Url { set { _Url = value; } }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { set { _PageIndex = value; } }
        /// <summary>
        /// 总页
        /// </summary>
        public int TotalPage { set { _TotalPage = value; } }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { set { _PageSize = value; } }
        /// <summary>
        /// 总记录
        /// </summary>
        public int TotalRecord { set { _TotalRecord = value; } }
        /// <summary>
        /// 样式
        /// </summary>
        public string StyleName { set { _StyleName = value; } }
        /// <summary>
        /// 对齐方式
        /// </summary>
        public Align HAlign { set { _Align = value; } }
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { set { _Index = value; } }
        //#endregion
        //#region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public Pager() {
            this._Url = "";
            this._PageIndex = 1;
            this._TotalPage = 1;
            this._PageSize = 20;
            this._TotalRecord = 0;
            this._StyleName = "BasePager";
            this._Align = Align.Left;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Url">URL</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="TotalPage">总页</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="TotalRecord">总记录数</param>
        /// <param name="StyleName">样式 默认BasePager</param>
        public Pager(string Url, int PageIndex, int TotalPage, int PageSize, int TotalRecord, string StyleName) {
            this._Url = Url;
            this._PageIndex = PageIndex;
            this._TotalPage = TotalPage;
            this._PageSize = PageSize;
            this._TotalRecord = TotalRecord;
            this._StyleName = StyleName;
        }
        //#endregion
        //#region 填充Html代码 生成分页代码
        /// <summary>
        /// 填充Html代码
        /// </summary>
        /// <param name="Page">当前page</param>
        /// <param name="CtrlID">输出到哪个HTML控件中</param>
        public void SetData(System.Web.UI.Page Page, string CtrlID) {
            _TotalPage = _TotalRecord / _PageSize;
            if (_TotalRecord % _PageSize != 0) _TotalPage++;
            sbData.Remove(0, sbData.Length);
            sbData.Append("var myPager" + _Index.ToString() + " = new " + this._StyleName + "();\n");
            sbData.Append("myPager" + _Index.ToString() + ".setUrl(\"" + _Url + "\");\n");
            sbData.Append("myPager" + _Index.ToString() + ".setPageIndex(" + _PageIndex.ToString() + ");\n");
            sbData.Append("myPager" + _Index.ToString() + ".setTotalPage(" + _TotalPage.ToString() + ");\n");
            sbData.Append("myPager" + _Index.ToString() + ".setPageSize(" + _PageSize.ToString() + ");\n");
            sbData.Append("myPager" + _Index.ToString() + ".setTotalRecord(" + _TotalRecord.ToString() + ");\n");
            string _str = "";
            switch (this._Align) {
                case Align.Left: _str = "left"; break;
                case Align.Center: _str = "center"; break;
                case Align.Right: _str = "right"; break;
            }
            sbData.Append("myPager" + _Index.ToString() + ".setAlign(\"" + _str + "\");\n");
            sbData.Append("myPager" + _Index.ToString() + ".Execute(\"" + CtrlID + "\");\n");
            Js.Run(Page, sbData.ToString(), false, "Js" + _Index.ToString());
        }
        /// <summary>
        /// 取分页JS代码
        /// </summary>
        /// <param name="CtrlID">输出到哪个HTML控件中</param>
        public string GetData(string CtrlID) {
            _TotalPage = _TotalRecord / _PageSize;
            if (_TotalRecord % _PageSize != 0) _TotalPage++;
            sbData.Remove(0, sbData.Length);
            sbData.Append("var myPager" + _Index.ToString() + " = new " + this._StyleName + "();\n");
            sbData.Append("myPager" + _Index.ToString() + ".setUrl(\"" + _Url + "\");\n");
            sbData.Append("myPager" + _Index.ToString() + ".setPageIndex(" + _PageIndex.ToString() + ");\n");
            sbData.Append("myPager" + _Index.ToString() + ".setTotalPage(" + _TotalPage.ToString() + ");\n");
            sbData.Append("myPager" + _Index.ToString() + ".setPageSize(" + _PageSize.ToString() + ");\n");
            sbData.Append("myPager" + _Index.ToString() + ".setTotalRecord(" + _TotalRecord.ToString() + ");\n");
            string _str = "";
            switch (this._Align) {
                case Align.Left: _str = "left"; break;
                case Align.Center: _str = "center"; break;
                case Align.Right: _str = "right"; break;
            }
            sbData.Append("myPager" + _Index.ToString() + ".setAlign(\"" + _str + "\");\n");
            sbData.Append("myPager" + _Index.ToString() + ".Execute(\"" + CtrlID + "\");\n");
            return sbData.ToString();
        }
        //#endregion
        //#region GetPager
        /// <summary>
        /// 生成分页HTML代码 « 1 2 3 4 5 6 »
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">URL</param>
        /// <param name="extendPage">输出页数</param>
        /// <returns>生成分页HTML代码 « 1 2 3 4 5 6 »</returns>
        public static string GetPager(int curPage, int countPage, string url, int extendPage) {
            if (countPage <= 1) return "";

            int startPage = 1;
            int endPage = 1;
            string t1 = "<a href=\"" + string.Format(url, 1) + "\">&laquo;</a>&nbsp;";
            string t2 = "<a href=\"" + string.Format(url, countPage) + "\">&raquo;</a>&nbsp;";
            if (countPage < 1) countPage = 1;
            if (extendPage < 3) extendPage = 2;
            if (countPage > extendPage) {
                if (curPage - (extendPage / 2) > 0) {
                    if (curPage + (extendPage / 2) < countPage) {
                        startPage = curPage - (extendPage / 2);
                        endPage = startPage + extendPage - 1;
                    } else {
                        endPage = countPage;
                        startPage = endPage - extendPage + 1;
                        t2 = "";
                    }
                } else {
                    endPage = extendPage;
                    t1 = "";
                }
            } else {
                startPage = 1;
                endPage = countPage;
                t1 = "";
                t2 = "";
            }
            System.Text.StringBuilder s = new System.Text.StringBuilder("");
            s.Append(t1);
            for (int i = startPage; i <= endPage; i++) {
                if (i == curPage) {
                    s.Append("&nbsp;<span>");
                    s.Append(i);
                    s.Append("</span>&nbsp;");
                } else {
                    s.Append("&nbsp;<a href=\"");
                    s.Append(string.Format(url, i));
                    s.Append("\">");
                    s.Append(i);
                    s.Append("</a>&nbsp;");
                }
            }
            s.Append(t2);
            return s.ToString();
        }
        /// <summary>
        /// 图片分页 « 1 2 3 4 5 6 »
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">URL</param>
        /// <param name="extendPage">输出页数</param>
        /// <returns>图片分页 « 1 2 3 4 5 6 »</returns>
        public static string GetImgPager(int curPage, int countPage, string url, int extendPage) {
            if (countPage <= 1) return "";

            int startPage = 1;
            int endPage = 1;
            string t1 = "<a class=\"first firstenable\" href=\"" + string.Format(url, 1) + "\"></a>";
            string t2 = "<a class=\"last lastenable\" href=\"" + string.Format(url, countPage) + "\"></a>";
            if (countPage < 1) countPage = 1;
            if (extendPage < 3) extendPage = 2;
            if (countPage > extendPage) {
                if (curPage - (extendPage / 2) > 0) {
                    if (curPage + (extendPage / 2) < countPage) {
                        startPage = curPage - (extendPage / 2);
                        endPage = startPage + extendPage - 1;
                    } else {
                        endPage = countPage;
                        startPage = endPage - extendPage + 1;
                        t2 = "";
                    }
                } else {
                    endPage = extendPage;
                }
            } else {
                startPage = 1;
                endPage = countPage;
                if (curPage <= 1) t1 = "<a class=\"first\"></a>";
                if (curPage >= countPage) t2 = "<a class=\"last\"></a>";
            }
            System.Text.StringBuilder s = new System.Text.StringBuilder("");
            s.Append(t1);
            for (int i = startPage; i <= endPage; i++) {
                if (i == curPage) {
                    s.Append("<a class=\"active\"></a>");
                } else {
                    s.Append("<a href=\"");
                    s.Append(string.Format(url, i));
                    s.Append("\"></a>");
                }
            }
            s.Append(t2);
            return s.ToString();
        }
        /// <summary>
        /// 生成分页HTML代码 上一页 « 1 2 3 4 5 6 » 下一页
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">URL</param>
        /// <param name="extendPage">输出页数</param>
        /// <returns>生成分页HTML代码 上一页 « 1 2 3 4 5 6 » 下一页</returns>
        public static string GetPager2(int curPage, int countPage, string url, int extendPage = 8) {
            StringBuilder sbPager = new StringBuilder();
            if (countPage != 1) {
                string pager = GetPager(curPage, countPage, url, extendPage);
                if (pager.IndexOf("&laquo;") > 0) sbPager.AppendFormat("<a class=\"prev\" href=\"{0}\">上一页</a>", string.Format(url, curPage - 1));
                sbPager.Append(pager);
                if (curPage != countPage) sbPager.AppendFormat("<a class=\"next\" href=\"{0}\">下一页</a>", string.Format(url, curPage + 1));
            }
            return sbPager.ToString();
        }
        /// <summary>
        /// DiscuzNT 生成分页HTML代码 上一页 « 1 2 3 4 5 6 » 页数 下一页
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">URL</param>
        /// <param name="extendPage">输出页数</param>
        /// <param name="jsCode">js代码</param>
        /// <returns>DiscuzNT 生成分页HTML代码 上一页 « 1 2 3 4 5 6 » 页数 下一页</returns>
        public static string GetDiscuzNTPager(int curPage, int countPage, string url, out string jsCode, int extendPage = 8) {
            StringBuilder sbPager = new StringBuilder();
            if (countPage != 1) {
                string pager = GetPager(curPage, countPage, url, extendPage);
                if (pager.IndexOf("&laquo;") > 0) sbPager.AppendFormat("<a class=\"prev\" href=\"{0}\">上一页</a>", string.Format(url, curPage - 1));
                sbPager.Append(pager);
                sbPager.AppendFormat("<kbd><input name=\"gopage\" type=\"text\" class=\"txt\" title=\"可以输入页码按回车键自动跳转\" value=\"{0}\" onfocus=\"this.value=this.defaultValue;this.select();\" onKeyDown=\"pageinputOnKeyDown(this,event);\" size=\"2\" maxlength=\"9\" />/ {1}</kbd>", curPage, countPage);
                if (curPage != countPage) sbPager.AppendFormat("<a class=\"next\" href=\"{0}\">下一页</a>", string.Format(url, curPage + 1));
            }
            jsCode = "function pageinputOnKeyDown(obj, event) { if (event.keyCode == 13) { window.location = '{0}'.format(parseInt(obj.value) > 0 ? parseInt(obj.value) : 1); }; return (event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 97 && event.keyCode <= 105) || event.keyCode == 8; };";
            return sbPager.ToString();
        }
        /// <summary>
        /// DiscuzNT 生成分页HTML代码 上一页 « 1 2 3 4 5 6 » 页数 记录数 下一页
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">URL</param>
        /// <param name="extendPage">输出页数</param>
        /// <param name="jsCode">js代码</param>
        /// <param name="totals">记录总数</param>
        /// <returns>DiscuzNT 生成分页HTML代码 上一页 « 1 2 3 4 5 6 » 页数 记录数 下一页</returns>
        public static string GetDiscuzNTPager(int curPage, int countPage, string url, out string jsCode, int extendPage, int totals) {
            StringBuilder sbPager = new StringBuilder();
            jsCode = "";
            if (countPage <= 1) return "";
            if (countPage != 1) {
                string pager = Pager.GetPager(curPage, countPage, url, extendPage);
                if (pager.IndexOf("&laquo;") > 0) sbPager.AppendFormat("<a class=\"prev\" href=\"{0}\">上一页</a>", string.Format(url, curPage - 1));
                sbPager.Append(pager);
                sbPager.AppendFormat("<input name=\"gopage\" type=\"text\" class=\"txt\" title=\"可以输入页码按回车键自动跳转\" onfocus=\"this.value=this.defaultValue;this.select();\" onKeyDown=\"pageinputOnKeyDown(this,event);\" size=\"2\" maxlength=\"9\" /> /&nbsp;{1}页&nbsp; {2}条记录&nbsp; ", curPage, countPage, totals);
                if (curPage != countPage) sbPager.AppendFormat("<a class=\"next\" href=\"{0}\">下一页</a>", string.Format(url, curPage + 1));
            }
            jsCode = "function pageinputOnKeyDown(obj, event) { if (event.keyCode == 13) { window.location = '" + url + "'.format(parseInt(obj.value) > 0 ? parseInt(obj.value) : 1); }; return (event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 97 && event.keyCode <= 105) || event.keyCode == 8; };";
            return sbPager.ToString();
        }
        /// <summary>
        /// 生成分页HTML代码 [首页] [上一页] [下一页] [尾页]
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">URL</param>
        /// <param name="extendPage">输出页数</param>
        /// <returns>生成分页HTML代码 [首页] [上一页] [下一页] [尾页]</returns>
        public static string GetBasePager(int curPage, int countPage, string url, int extendPage) {
            if (countPage <= 1) return "";
            System.Text.StringBuilder s = new System.Text.StringBuilder("");
            if (curPage == 1) {
                s.AppendFormat("<span>{0}</span>&nbsp;<span>{1}</span>&nbsp;<a href=\"{2}\">{3}</a>&nbsp;<a href=\"{4}\">{5}</a>", "[首页]", "[上一页]", string.Format(url, curPage + 1), "[下一页]", string.Format(url, countPage), "[尾页]");
            } else if (curPage == countPage) {
                s.AppendFormat("<a href=\"{2}\">{3}</a>&nbsp;<a href=\"{4}\">{5}</a>&nbsp;<span>{0}</span>&nbsp;<span>{1}</span>", "[下一页]", "[尾页]", string.Format(url, 1), "[首页]", string.Format(url, curPage - 1), "[上一页]");
            } else {
                s.AppendFormat("<a href=\"{0}\">{1}</a>&nbsp;<a href=\"{2}\">{3}</a>&nbsp;<a href=\"{4}\">{5}</a>&nbsp;<a href=\"{6}\">{7}</a>", string.Format(url, 1), "[首页]", string.Format(url, curPage - 1), "[上一页]", string.Format(url, curPage + 1), "[下一页]", string.Format(url, countPage), "[尾页]");
            }
            return s.ToString();
        }
        /// <summary>
        /// 生成分页HTML代码 1 2 3 4 5 6 
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">URL</param>
        /// <param name="extendPage">输出页数</param>
        /// <returns>生成分页HTML代码 1 2 3 4 5 6 </returns>
        public static string GetPager3(int curPage, int countPage, string url, int extendPage) {
            if (countPage <= 1) return "";

            int startPage = 1;
            int endPage = 1;
            if (countPage < 1) countPage = 1;
            if (extendPage < 3) extendPage = 2;
            if (countPage > extendPage) {
                if (curPage - (extendPage / 2) > 0) {
                    if (curPage + (extendPage / 2) < countPage) {
                        startPage = curPage - (extendPage / 2);
                        endPage = startPage + extendPage - 1;
                    } else {
                        endPage = countPage;
                        startPage = endPage - extendPage + 1;
                    }
                } else {
                    endPage = extendPage;
                }
            } else {
                startPage = 1;
                endPage = countPage;
            }
            System.Text.StringBuilder s = new System.Text.StringBuilder("");
            for (int i = startPage; i <= endPage; i++) {
                if (i == curPage) {
                    s.Append("&nbsp;<span class=\"active\">");
                    s.Append(i);
                    s.Append("</span>");
                } else {
                    s.Append("&nbsp;<a href=\"");
                    s.Append(string.Format(url, i));
                    s.Append("\">");
                    s.Append(i);
                    s.Append("</a>");
                }
            }
            return s.ToString();
        }
        /// <summary>
        /// 生成分页HTML代码 首页 上一页 1 2 3 4 5 6 下一页 尾页
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">URL</param>
        /// <param name="extendPage">输出页数</param>
        /// <returns>生成分页HTML代码 首页 上一页 1 2 3 4 5 6 下一页 尾页</returns>
        public static string GetBasePager2(int curPage, int countPage, string url, int extendPage) {
            if (countPage <= 1) return "";
            System.Text.StringBuilder s = new System.Text.StringBuilder("");
            if (curPage == 1) {
                s.AppendFormat("<span>{0}</span>&nbsp;<span>{1}</span>{6}&nbsp;<a href=\"{2}\">{3}</a>&nbsp;<a href=\"{4}\">{5}</a>", 
                    "首页", "上一页", string.Format(url, curPage + 1), "下一页", string.Format(url, countPage), "尾页", GetPager3(curPage, countPage, url, extendPage));
            } else if (curPage == countPage) {
                s.AppendFormat("<a href=\"{2}\">{3}</a>&nbsp;<a href=\"{4}\">{5}</a>{6}&nbsp;<span>{0}</span>&nbsp;<span>{1}</span>", 
                    "下一页", "尾页", string.Format(url, 1), "首页", string.Format(url, curPage - 1), "上一页", GetPager3(curPage, countPage, url, extendPage));
            } else {
                s.AppendFormat("<a href=\"{0}\">{1}</a>&nbsp;<a href=\"{2}\">{3}</a>{8}&nbsp;<a href=\"{4}\">{5}</a>&nbsp;<a href=\"{6}\">{7}</a>", string.Format(url, 1), "首页", string.Format(url, curPage - 1), "上一页", string.Format(url, curPage + 1), "下一页", string.Format(url, countPage), "尾页", GetPager3(curPage, countPage, url, extendPage));
            }
            return s.ToString();
        }
        //#endregion
    }
}
