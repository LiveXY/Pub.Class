//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Pub.Class {
    /// <summary>
    /// DataGrid GridView扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class WebControlExtensions {
        /// <summary>
        /// 鼠标移上时变颜色
        /// </summary>
        /// <param name="dg">DataGrid扩展</param>
        /// <param name="color">默认颜色</param>
        /// <param name="hoverColor">hover颜色</param>
        public static void HoverScript(this DataGrid dg, string color, string hoverColor) {
            //鼠标移上时变颜色
            for (int i = 0; i < dg.Items.Count; i++) {
                if (dg.Items[i].ItemType.ToString() == "Item" || dg.Items[i].ItemType.ToString() == "AlternatingItem") {
                    TableRow tr = (TableRow)dg.Items[i].Cells[0].Parent;
                    Js.AddAttr(tr, "onmouseover", "this.bgColor='" + hoverColor + "'");
                    Js.AddAttr(tr, "onmouseout", "this.bgColor='" + color + "'");
                }
            }
        }
        /// <summary>
        /// 鼠标移上时变颜色
        /// </summary>
        /// <param name="dv">GridView扩展</param>
        /// <param name="color">默认颜色</param>
        /// <param name="hoverColor">hover颜色</param>
        public static void HoverScript(this GridView dv, string color, string hoverColor) {
            //鼠标移上时变颜色
            for (int i = 0; i < dv.Rows.Count; i++) {
                if (dv.Rows[i].RowType.ToString() == "DataRow") {
                    TableRow tr = (TableRow)dv.Rows[i].Cells[0].Parent;
                    Js.AddAttr(tr, "onmouseover", "gvBgColor = this.bgColor; this.bgColor='" + hoverColor + "'");
                    Js.AddAttr(tr, "onmouseout", "this.bgColor=" + (color == "" ? "gvBgColor;" : "'" + color + "'"));
                }
            }
        }
        /// <summary>
        /// 强类型数据绑定
        /// </summary>
        /// <example>
        /// <code>
        /// this.Eval&lt;Student>(p => p.Age)
        /// </code>
        /// </example>
        /// <typeparam name="TEntity">类型</typeparam>
        /// <param name="page">Page扩展</param>
        /// <param name="func">函数</param>
        /// <returns></returns>
        public static string Eval<TEntity>(this Page page, Func<TEntity, string> func) {
            return func((TEntity)page.GetDataItem()).ToStr();
        }
        /// <summary>
        /// 强类型数据绑定
        /// </summary>
        /// <example>
        /// <code>
        /// this.Eval&lt;Student, int>(p => p.Age)
        /// </code>
        /// </example>
        /// <typeparam name="TEntity">类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="page">Page扩展</param>
        /// <param name="func">函数</param>
        /// <returns></returns>
        public static TResult Eval<TEntity, TResult>(this Page page, Func<TEntity, TResult> func) {
            return func((TEntity)page.GetDataItem());
        }
        /// <summary>
        /// 取控件的值
        /// </summary>
        /// <param name="page">Page扩展</param>
        /// <param name="ctrlID">控件ID</param>
        /// <returns></returns>
        public static string GetControlValue(this Page page, string ctrlID) {
            Control control = page.FindControl(ctrlID);
            if (control is TextBox) return ((TextBox)control).Text;
            if (control is DropDownList) return ((DropDownList)control).SelectedItem.Value;
            return "";
        }
        /// <summary>
        /// 设置控件的值
        /// </summary>
        /// <param name="page">Page扩展</param>
        /// <param name="ctrlID">控件ID</param>
        /// <param name="value">值</param>
        public static void SetControlValue(this Page page, string ctrlID, string value) {
            Control control = page.FindControl(ctrlID);
            if (control is TextBox) ((TextBox)control).Text = value;
            if (control is DropDownList) {
                DropDownList list = (DropDownList)control;
                foreach (ListItem item in list.Items) {
                    if (item.Value == value) {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }
    }
}
