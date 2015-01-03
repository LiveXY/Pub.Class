//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;

namespace Pub.Class {
    /// <summary>
    /// Js操作类
    /// 
    /// 修改纪录
    ///     2006.05.02 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Js {
        //#region Run
        /// <summary>
        /// 运行JS代码
        /// </summary>
        /// <param name="Page">指定Page</param>
        /// <param name="strCode">要注册的代码</param>
        /// <param name="isTop">是否在头部/否则在尾部</param>
        public static void Run(System.Web.UI.Page Page, string strCode, bool isTop) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\">\n");
            sb.Append(strCode.Trim());
            sb.Append("\n</script>\n");
            if (isTop) Page.RegisterClientScriptBlock("RunTopJs", sb.ToString()); else Page.RegisterStartupScript("RunBottomJs", sb.ToString());
        }
        /// <summary>
        /// 运行JS代码
        /// </summary>
        /// <param name="Page">指定Page</param>
        /// <param name="strCode">要注册的代码</param>
        /// <param name="isTop">是否在头部/否则在尾部</param>
        /// <param name="IDStr">Key</param>
        public static void Run(System.Web.UI.Page Page, string strCode, bool isTop, string IDStr) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\">\n");
            sb.Append(strCode.Trim());
            sb.Append("\n</script>\n");
            if (isTop) Page.RegisterClientScriptBlock(IDStr, sb.ToString()); else Page.RegisterStartupScript(IDStr, sb.ToString());
        }
        /// <summary>
        /// 清空指定注册的JS代码
        /// </summary>
        /// <param name="Page">指定Page</param>
        /// <param name="isTop">是否在头部/否则在尾部</param>
        /// <param name="IDStr">Key</param>
        public static void Run(System.Web.UI.Page Page, bool isTop, string IDStr) {
            if (isTop) Page.RegisterClientScriptBlock(IDStr, ""); else Page.RegisterStartupScript(IDStr, "");
        }
        //#endregion
        //#region Alert
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Alert(string msg) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\"> \n");
            sb.Append("alert(\"" + msg.Trim() + "\"); \n");
            sb.Append("</script>\n");
            HttpContext.Current.Response.Write(sb.ToString());
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="msg">消息</param>
        public static void AlertEnd(string msg) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\"> \n");
            sb.Append("alert(\"" + msg.Trim() + "\"); \n");
            sb.Append("</script>\n");
            HttpContext.Current.Response.Write(sb.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="Page">指定页</param>
        /// <param name="msg">消息</param>
        public static void Alert(System.Web.UI.Page Page, string msg) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\"> \n");
            sb.Append("alert(\"" + msg.Trim() + "\"); \n");
            sb.Append("</script>\n");
            Page.RegisterClientScriptBlock("AlertJs", sb.ToString());
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="Page">指定页</param>
        /// <param name="msg">消息</param>
        /// <param name="isTop">是否在头部/否则在尾部</param>
        public static void Alert(System.Web.UI.Page Page, string msg, bool isTop) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\"> \n");
            sb.Append("alert(\"" + msg.Trim() + "\"); \n");
            sb.Append("</script>\n");
            if (isTop) Page.RegisterClientScriptBlock("AlertTopJs", sb.ToString()); else Page.RegisterStartupScript("AlertBottomJs", sb.ToString());
        }
        //#endregion
        //#region Import/loadCss/AddAttr/chkFormData
        /// <summary>
        /// 注册一个处部JS文件/或CSS文件
        /// </summary>
        /// <param name="Page">指定页</param>
        /// <param name="filePath">文件</param>
        /// <param name="isTop">是否在头部/否则在尾部</param>
        public static void Import(System.Web.UI.Page Page, string filePath, bool isTop) {
            StringBuilder sb = new StringBuilder();
            if (filePath.ToLower().Substring(filePath.Length - 3, 3) == ".js") {
                sb.Append("<script language=\"JavaScript\" src=\"" + filePath + "\" type=\"text/javascript\"></script>\n");
                if (isTop) Page.RegisterClientScriptBlock("TopJs", sb.ToString()); else Page.RegisterStartupScript("BottomJs", sb.ToString());
            }
            if (filePath.ToLower().Substring(filePath.Length - 4, 4) == ".css") {
                LoadCss(Page, filePath);
            }
        }
        /// <summary>
        /// 注册一个处部CSS文件
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="cssFile">CSS文件</param>
        public static void JsLoadCss(System.Web.UI.Page page, string cssFile) {
            Run(page, "setStyle(\"" + cssFile + "\");\n", true);
        }
        /// <summary>
        /// 注册一个处部CSS文件
        /// </summary>
        /// <param name="placeHolder">PlaceHolder组件</param>
        /// <param name="cssFile">CSS文件</param>
        public static void LoadCss(System.Web.UI.WebControls.PlaceHolder placeHolder, string cssFile) {
            HtmlGenericControl objLink = new HtmlGenericControl("LINK");
            objLink.Attributes["rel"] = "stylesheet";
            objLink.Attributes["type"] = "text/css";
            objLink.Attributes["href"] = cssFile;
            placeHolder.Controls.Add(objLink);
            //<asp:placeholder id="MyCSS" runat="server"></asp:placeholder> 
        }
        /// <summary>
        /// 注册一个处部CSS文件
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="cssFile">CSS文件</param>
        public static void LoadCss(System.Web.UI.Page page, string cssFile) {
            HtmlLink myHtmlLink = new HtmlLink();
            myHtmlLink.Href = cssFile;
            Js.AddAttr(myHtmlLink, "rel", "stylesheet");
            Js.AddAttr(myHtmlLink, "type", "text/css");
            page.Header.Controls.Add(myHtmlLink);
        }
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="Control">WebControl</param>
        /// <param name="eventStr">名称</param>
        /// <param name="MsgStr">内容</param>
        public static void AddAttr(System.Web.UI.WebControls.WebControl Control, string eventStr, string MsgStr) {
            Control.Attributes.Add(eventStr, MsgStr);
        }
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="Control">HtmlGenericControl</param>
        /// <param name="eventStr">名称</param>
        /// <param name="MsgStr">内容</param>
        public static void AddAttr(System.Web.UI.HtmlControls.HtmlGenericControl Control, string eventStr, string MsgStr) {
            Control.Attributes.Add(eventStr, MsgStr);
        }
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="Control">HtmlGenericControl</param>
        /// <param name="eventStr">名称</param>
        /// <param name="MsgStr">内容</param>
        public static void AddAttr(System.Web.UI.HtmlControls.HtmlControl Control, string eventStr, string MsgStr) {
            Control.Attributes.Add(eventStr, MsgStr);
        }
        /// <summary>
        /// 验证数据完整性
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="dataValue">内容</param>
        /// <param name="divObjStr">div对像 如果为空时以alert方法显示</param>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <param name="titleStr">标题</param>
        /// <param name="isNVarchar">是否Nvarchar类型</param>
        /// <returns>真/假</returns>
        /// <example>
        /// <code>
        /// private bool doSave()
        /// { 
        ///     if (!Cmn.Js.ChkFormData(this, "", "", 1, 20, "姓名", false)) return false;
        ///     if (!Cmn.Js.ChkFormData(this, "熊", "", 4, 20, "姓名", false)) return false;
        ///     if (!Cmn.Js.ChkFormData(this, "熊华春123456789123451", "", 4, 20, "姓名", false)) return false;
        ///     return true;
        /// }
        /// if (doSave()) Cmn.Js.Alert(this,"成功");
        /// </code>
        /// </example>
        public static bool ChkFormData(System.Web.UI.Page page, string dataValue, string divObjStr, int minLength, int maxLength, string titleStr, bool isNVarchar) {
            int txtObjLength = (isNVarchar) ? dataValue.Length : dataValue.CnLength();
            StringBuilder sb = new StringBuilder();
            bool _result = true;
            if (txtObjLength == 0 && minLength != 0) {
                if (divObjStr != "") {
                    sb.Append("document.getElementById(\"" + divObjStr + "\").innerHTML = \"<div class=ErrorMsg>" + titleStr + "不能为空！</div>\";");
                } else {
                    sb.Append("alert('" + titleStr + "不能为空！');");
                }
                _result = false;
            } else if (txtObjLength < minLength) {
                if (divObjStr != "") {
                    sb.Append("document.getElementById(\"" + divObjStr + "\").innerHTML = \"<div class=ErrorMsg>" + titleStr + "不能小于" + minLength + "个字符！</div>\";");
                } else {
                    sb.Append("alert('" + titleStr + "不能小于" + minLength + "个字符！');");
                }
                _result = false;
            } else if (txtObjLength > maxLength) {
                if (divObjStr != "") {
                    sb.Append("document.getElementById(\"" + divObjStr + "\").innerHTML = \"<div class=ErrorMsg>" + titleStr + "不能大于" + maxLength + "个字符！</div>\";");
                } else {
                    sb.Append("alert('" + titleStr + "不能大于" + maxLength + "个字符！');");
                }
                _result = false;
            } else {
                if (divObjStr != "") { sb.Append("document.getElementById(\"" + divObjStr + "\").innerHTML = \"\");"); }
                _result = true;
            }
            Js.Run(page, sb.ToString(), false, titleStr);
            return _result;
        }
        //#endregion
    }
}
