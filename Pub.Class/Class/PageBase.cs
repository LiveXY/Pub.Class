//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Web;
using System.Configuration;
using System.Text;
using System.Data.Common;
using System.Web.UI;
using System.Collections.Generic;
using System.IO;
using System.Web.Security;

namespace Pub.Class {
    /// <summary>
    /// aspx页面继承类
    /// 
    /// 修改纪录
    ///     2006.05.08 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public partial class PageBase : Update {
        //#region 私有
        /// <summary>
        /// 语言CACHE
        /// </summary>
        private static readonly ISafeDictionary<string, ISafeDictionary<string, string>> langList = new SafeDictionary<string, ISafeDictionary<string, string>>();
        /// <summary>
        /// 语言
        /// </summary>
        protected string lang = string.Empty;
        /// <summary>
        /// 皮肤
        /// </summary>
        protected string skin = "default";
        /// <summary>
        /// 标题
        /// </summary>
        protected string title = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        protected string description = string.Empty;
        /// <summary>
        /// 关键字
        /// </summary>
        protected string keywords = string.Empty;
        /// <summary>
        /// 索引值
        /// </summary>
        protected int index = 0;
        /// <summary>
        /// 引用JS
        /// </summary>
        protected StringBuilder js = new StringBuilder();
        /// <summary>
        /// 引用CSS
        /// </summary>
        protected StringBuilder css = new StringBuilder();
        /// <summary>
        /// 相对根路径 /开头
        /// </summary>
        public string RootPath = Request2.GetRelativeRoot();
        //#endregion
        /// <summary>
        /// 语言
        /// </summary>
        public string Lang { get { return lang; } set { lang = value; } }
        /// <summary>
        /// 皮肤
        /// </summary>
        public string Skin { get { return skin; } set { skin = value; } }
        /// <summary>
        /// 标题
        /// </summary>
        public new string Title { get { return title; } set { description = value + ", "; keywords = value + ", "; title = value + " - "; } }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get { return description; } set { description = value.Length > 0 ? value + ", " : value; } }
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get { return keywords; } set { keywords = value.Length > 0 ? value + ", " : value; } }
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get { return index; } set { index = value; } }
        /// <summary>
        /// 引用JS
        /// </summary>
        public string JS { get { return js.ToString(); } set { value.Split(';').Do((s, i) => { js.AppendFormat("<script language=\"javascript\" type=\"text/javascript\" src=\"{0}\"></script>", s); }); } }
        /// <summary>
        /// 引用CSS
        /// </summary>
        public string CSS { get { return css.ToString(); } set { value.Split(';').Do((s, i) => { js.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />", s); }); } }
        /// <summary>
        /// 取所有语言
        /// </summary>
        /// <returns></returns>
        private ISafeDictionary<string, string> GetLang() {
            if (lang.IsNullEmpty()) Msg.WriteEnd("语言未设置！");
            string path = "".GetMapPath() + "\\lang\\{0}.lang".FormatWith(lang);
            if (!FileDirectory.FileExists(path)) Msg.WriteEnd("语言文件{0}.lang不存在！".FormatWith(lang));

            string lineText = string.Empty; ISafeDictionary<string, string> list = new SafeDictionary<string, string>();
            using (StreamReader reader = new StreamReader(path, System.Text.Encoding.UTF8)) {
                while ((lineText = reader.ReadLine()).IsNotNull()) {
                    int len = lineText.IndexOf('=');
                    if (lineText.IsNullEmpty() || len == -1) continue;
                    string key = lineText.Substring(0, len).Trim();
                    string value = lineText.Substring(len + 1).Trim();
                    if (!list.ContainsKey(key)) list.Add(key, value); else list[key] = value;
                }
            }
            return list;
        }
        /// <summary>
        /// 取语言
        /// </summary>
        /// <example>
        /// <code>
        ///     string aa = GetLang("lblCourseName")
        /// </code>
        /// </example>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetLang(string key) {
            if (!langList.ContainsKey(lang)) langList[lang] = GetLang();
            if (!langList[lang].ContainsKey(key)) return string.Empty;
            return langList[lang][key];
        }
        ///// <summary>
        ///// 从CACHE中取ViewState值
        ///// </summary>
        ///// <returns></returns>
        //protected override object LoadPageStateFromPersistenceMedium() {
        //    var viewStateID = (string)((Pair)base.LoadPageStateFromPersistenceMedium()).Second;
        //    var stateStr = (string)Cache2.Get(viewStateID);
        //    return stateStr.IsNullEmpty() ? string.Empty : new ObjectStateFormatter().Deserialize(stateStr);
        //}
        ///// <summary>
        ///// 保存ViewState值到CACHE
        ///// </summary>
        ///// <param name="state"></param>
        //protected override void SavePageStateToPersistenceMedium(object state) {
        //    var value = new ObjectStateFormatter().Serialize(state);
        //    var viewStateID = (DateTime.Now.Ticks + (long)this.GetHashCode()).ToString(); //产生离散的id号码
        //    Cache2.Insert(viewStateID, state, 1440);
        //    base.SavePageStateToPersistenceMedium(viewStateID);
        //}
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
        }
        //public void FormsAuthenticationLogout() {
        //    FormsAuthentication.SignOut();
        //}
        //public void FormsAuthenticationLogin(int userID, string userName = "", int days = 1) {
        //    var now = DateTime.UtcNow.ToLocalTime();
        //    var ticket = new FormsAuthenticationTicket(54, userName, now, now.Add(TimeSpan.FromDays(days)), true, userID.ToString(), FormsAuthentication.FormsCookiePath);
        //}
        //public int FormsAuthenticationUserID() {
        //    var formsIdentity = HttpContext.Current.User.Identity;
        //    if (formsIdentity.IsNull()) return 0;
        //    return ((FormsIdentity)formsIdentity).Ticket.UserData.ToInt(0);
        //}
    }
}
