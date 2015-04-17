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
using System.Linq.Expressions;

namespace Pub.Class {
    /// <summary>
    /// Expression扩展
    /// 
    /// 修改纪录
    ///     2012.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class ExpressionExtensions {
        public static string GetColumnName(this Expression expression) {
            MemberExpression me = GetMemberExpression(expression);
            return me.Member.Name;
        }
        public static MemberExpression GetMemberExpression(this Expression expression) {
            if (expression is MemberExpression) return (MemberExpression)expression;
            else if (expression is UnaryExpression) return GetMemberExpression(((UnaryExpression)expression).Operand);
            else if (expression is LambdaExpression) return GetMemberExpression(((LambdaExpression)expression).Body);
            else if (expression is MethodCallExpression) return GetMemberExpression(((MethodCallExpression)expression).Arguments[1]);
            else return null;
        }
    }
}
