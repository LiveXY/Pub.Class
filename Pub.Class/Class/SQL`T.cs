//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Pub.Class {
    /// <summary>
    /// SQL单表操作
    /// 
    /// 修改纪录
    ///     2012.10.29 版本：1.0 livexy 创建此类
    ///
    /// </summary>
    /// <typeparam name="T">表名实体类</typeparam>
    /// <example>
    /// <code>
    /// var sql = new SQL&ltUser>(true).Select().OrderBy(t => t.UserID).GroupBy(t => t.UserID);
    /// var sql = new SQL&ltUser>().Select(t => t.UserID);
    /// var sql = new SQL&ltUser>().Update().Set(t => t.UserName, "111").Where(t => t.UserID == 1 || t.UserID == 2 || t.UserName == "'t");
    /// var sql = new SQL&ltUser>().Insert().Value(p => p.UserID, 1).Value(p => p.UserName, "1");
    /// var where = new Where().And("UserID", 1, Operator.Equal);
    /// var sql = new SQL&ltUser>().Select().Where(where).ToSQL();
    /// var sql = new SQL&ltUser>().Select().Where(t => 1 == t.UserID).Where(t => "test" == t.UserName).ToSQL();
    /// </code>
    /// </example>
    public class SQL<T> {

        private SQL sql = new SQL();
        private bool select = false;
        private bool nolock = false;
        private bool isWhere = false;

        public SQL(bool nolock = false) { this.nolock = nolock; }
        public SQL(string dbkey) { sql.Database(dbkey); }

        /// <summary>
        /// 查询SQL *
        /// </summary>
        /// <returns>this</returns>
        public SQL<T> Select() {
            if (!select) {
                select = true;
                if (nolock) sql.FromNoLock(typeof(T).Name); else sql.From(typeof(T).Name);
            }
            sql.Select("*");
            return this;
        }
        /// <summary>
        /// 查询SQL
        /// </summary>
        /// <param name="expression">字段</param>
        /// <returns>this</returns>
        public SQL<T> Select(Expression<Func<T, object>> expression) {
            if (!select) {
                select = true;
                if (nolock) sql.FromNoLock(typeof(T).Name); else sql.From(typeof(T).Name);
            }
            sql.Select(expression.GetColumnName());
            return this;
        }

        /// <summary>
        /// Max
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL<T> Max(Expression<Func<T, object>> expression) {
            if (!select) {
                select = true;
                if (nolock) sql.FromNoLock(typeof(T).Name); else sql.From(typeof(T).Name);
            }
            sql.Max(expression.GetColumnName());
            return this;
        }
        
        /// <summary>
        /// Min
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL<T> Min(Expression<Func<T, object>> expression) {
            if (!select) {
                select = true;
                if (nolock) sql.FromNoLock(typeof(T).Name); else sql.From(typeof(T).Name);
            }
            sql.Min(expression.GetColumnName());
            return this;
        }
        /// <summary>
        /// Sum
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL<T> Sum(Expression<Func<T, object>> expression) {
            if (!select) {
                select = true;
                if (nolock) sql.FromNoLock(typeof(T).Name); else sql.From(typeof(T).Name);
            }
            sql.Sum(expression.GetColumnName());
            return this;
        }
        /// <summary>
        /// Count
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL<T> Count(Expression<Func<T, object>> expression) {
            if (!select) {
                select = true;
                if (nolock) sql.FromNoLock(typeof(T).Name); else sql.From(typeof(T).Name);
            }
            sql.Count(expression.GetColumnName());
            return this;
        }
        public SQL<T> Count() { 
            if (!select) {
                select = true;
                if (nolock) sql.FromNoLock(typeof(T).Name); else sql.From(typeof(T).Name);
            }
            sql.Count(); 
            return this; 
        }
        public SQL<T> Distinct() { sql.Distinct(); return this; }
        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL<T> GroupBy(Expression<Func<T, object>> expression) {
            sql.GroupBy(expression.GetColumnName());
            return this;
        }
        /// <summary>
        /// 正序
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL<T> OrderBy(Expression<Func<T, object>> expression) {
            sql.OrderBy(expression.GetColumnName());
            return this;
        }
        /// <summary>
        /// 倒序
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>this</returns>
        public SQL<T> OrderByDescending(Expression<Func<T, object>> expression) {
            sql.OrderByDescending(expression.GetColumnName());
            return this;
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public SQL<T> Where(Expression<Func<T, bool>> where) {
            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.Build(where.Body);

            if (!String.IsNullOrEmpty(conditionBuilder.Condition)) {
                if (isWhere) {
                    sql.Where(string.Format(" and {0} ", string.Format(conditionBuilder.Condition, conditionBuilder.Arguments)));
                } else {
                    isWhere = true;
                    sql.Where(string.Format("{0} ", string.Format(conditionBuilder.Condition, conditionBuilder.Arguments)));
                }
            }
            return this;
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public SQL<T> Where(Where where) { 
            sql.Where("1=1").Where(where);
            return this;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns>this</returns>
        public SQL<T> Update() {
            sql.Update(typeof(T).Name);
            return this;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="useNULL">是否允许更新为null的数据</param>
        /// <returns>this</returns>
        public SQL<T> Set(Expression<Func<T, object>> expression, object value, bool useNULL = false) {
            sql.SetP(expression.GetColumnName(), value, useNULL);
            return this;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tables">表</param>
        /// <returns>this</returns>
        public SQL<T> Delete() {
            sql.Delete(typeof(T).Name);
            return this;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="table">表</param>
        /// <returns>this</returns>
        public SQL<T> Insert() {
            sql.Insert(typeof(T).Name);
            return this;
        }
        /// <summary>
        /// 插入值
        /// </summary>
        /// <param name="expression">字段</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public SQL<T> Value(Expression<Func<T, object>> expression, object value) {
            sql.ValueP(expression.GetColumnName(), value);
            return this;
        }

        public SQL ToSQL() { return sql; }
    }

    public class ConditionBuilder : ExpressionVisitor {
        private List<object> m_arguments;

        private Stack<string> m_conditionParts;
        public string Condition { get; private set; }

        public object[] Arguments { get; private set; }

        public void Build(Expression expression) {
            PartialEvaluator evaluator = new PartialEvaluator();
            Expression evaluatedExpression = evaluator.Eval(expression);

            this.m_arguments = new List<object>();
            this.m_conditionParts = new Stack<string>();

            this.Visit(evaluatedExpression);

            this.Arguments = this.m_arguments.ToArray();
            if (evaluatedExpression.NodeType != ExpressionType.NewArrayInit)
                this.Condition = this.m_conditionParts.Count > 0 ? this.m_conditionParts.Pop() : null;
            else {
                foreach (var mConditionPart in m_conditionParts) {
                    this.Condition += mConditionPart + " ,";
                }
                this.Condition = this.Condition.Trim(',');
            }

        }

        protected override Expression VisitBinary(BinaryExpression b) {
            if (b == null) return b;

            string opr;
            switch (b.NodeType) {
                case ExpressionType.Equal:
                    opr = "=";
                    break;
                case ExpressionType.NotEqual:
                    opr = "<>";
                    break;
                case ExpressionType.GreaterThan:
                    opr = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    opr = ">=";
                    break;
                case ExpressionType.LessThan:
                    opr = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    opr = "<=";
                    break;
                case ExpressionType.AndAlso:
                    opr = "and";
                    break;
                case ExpressionType.OrElse:
                    opr = "or";
                    break;
                case ExpressionType.Add:
                    opr = "+";
                    break;
                case ExpressionType.Subtract:
                    opr = "-";
                    break;
                case ExpressionType.Multiply:
                    opr = "*";
                    break;
                case ExpressionType.Divide:
                    opr = "/";
                    break;
                default:
                    throw new NotSupportedException(b.NodeType + "is not supported.");
            }

            this.Visit(b.Left);
            this.Visit(b.Right);

            string right = this.m_conditionParts.Pop();
            string left = this.m_conditionParts.Pop();

            string condition = String.Format("({0} {1} {2})", left, opr, right);
            this.m_conditionParts.Push(condition);

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c) {
            if (c == null) return c;
            this.m_arguments.Add(c.Value.IsNull() ? "null" : Pub.Class.Where.ValueToStr(c.Value));
            this.m_conditionParts.Push(String.Format("{{{0}}}", this.m_arguments.Count - 1));

            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m) {
            if (m == null) return m;

            PropertyInfo propertyInfo = m.Member as PropertyInfo;
            if (propertyInfo == null) return m;

            this.m_conditionParts.Push(String.Format("[{0}]", propertyInfo.Name));
            return m;
        }
    }
    public class PartialEvaluator : ExpressionVisitor {
        private Func<Expression, bool> m_fnCanBeEvaluated;
        private HashSet<Expression> m_candidates;

        public PartialEvaluator()
            : this(CanBeEvaluatedLocally) { }

        public PartialEvaluator(Func<Expression, bool> fnCanBeEvaluated) {
            this.m_fnCanBeEvaluated = fnCanBeEvaluated;
        }

        public Expression Eval(Expression exp) {
            this.m_candidates = new Nominator(this.m_fnCanBeEvaluated).Nominate(exp);

            return this.Visit(exp);
        }

        protected override Expression Visit(Expression exp) {
            if (exp == null) {
                return null;
            }

            if (this.m_candidates.Contains(exp)) {
                return this.Evaluate(exp);
            }

            return base.Visit(exp);
        }

        private Expression Evaluate(Expression e) {
            if (e.NodeType == ExpressionType.Constant) {
                return e;
            }

            LambdaExpression lambda = Expression.Lambda(e);
            Delegate fn = lambda.Compile();

            return Expression.Constant(fn.DynamicInvoke(null), e.Type);
        }

        private static bool CanBeEvaluatedLocally(Expression exp) {
            return exp.NodeType != ExpressionType.Parameter;
        }

        #region Nominator

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor {
            private Func<Expression, bool> m_fnCanBeEvaluated;
            private HashSet<Expression> m_candidates;
            private bool m_cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated) {
                this.m_fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression) {
                this.m_candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this.m_candidates;
            }

            protected override Expression Visit(Expression expression) {
                if (expression != null) {
                    bool saveCannotBeEvaluated = this.m_cannotBeEvaluated;
                    this.m_cannotBeEvaluated = false;

                    base.Visit(expression);

                    if (!this.m_cannotBeEvaluated) {
                        if (this.m_fnCanBeEvaluated(expression)) {
                            this.m_candidates.Add(expression);
                        } else {
                            this.m_cannotBeEvaluated = true;
                        }
                    }

                    this.m_cannotBeEvaluated |= saveCannotBeEvaluated;
                }

                return expression;
            }
        }

        #endregion
    }
    public abstract class ExpressionVisitor {
        protected ExpressionVisitor() { }

        protected virtual Expression Visit(Expression exp) {
            if (exp == null)
                return exp;
            switch (exp.NodeType) {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding) {
            switch (binding.BindingType) {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer) {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments) {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression u) {
            Expression operand = this.Visit(u.Operand);
            if (operand != u.Operand) {
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;
        }

        protected virtual Expression VisitBinary(BinaryExpression b) {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            Expression conversion = this.Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion) {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }
            return b;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b) {
            Expression expr = this.Visit(b.Expression);
            if (expr != b.Expression) {
                return Expression.TypeIs(expr, b.TypeOperand);
            }
            return b;
        }

        protected virtual Expression VisitConstant(ConstantExpression c) {
            return c;
        }

        protected virtual Expression VisitConditional(ConditionalExpression c) {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse) {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        protected virtual Expression VisitParameter(ParameterExpression p) {
            return p;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression m) {
            Expression exp = this.Visit(m.Expression);
            if (exp != m.Expression) {
                return Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression m) {
            Expression obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments) {
                return Expression.Call(obj, m.Method, args);
            }
            return m;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original) {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++) {
                Expression p = this.Visit(original[i]);
                if (list != null) {
                    list.Add(p);
                } else if (p != original[i]) {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++) {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null) {
                return list.AsReadOnly();
            }
            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment) {
            Expression e = this.Visit(assignment.Expression);
            if (e != assignment.Expression) {
                return Expression.Bind(assignment.Member, e);
            }
            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding) {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings) {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding) {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers) {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original) {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++) {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null) {
                    list.Add(b);
                } else if (b != original[i]) {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++) {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original) {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++) {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null) {
                    list.Add(init);
                } else if (init != original[i]) {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++) {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda) {
            Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body) {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        protected virtual NewExpression VisitNew(NewExpression nex) {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments) {
                if (nex.Members != null)
                    return Expression.New(nex.Constructor, args, nex.Members);
                else
                    return Expression.New(nex.Constructor, args);
            }
            return nex;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init) {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings) {
                return Expression.MemberInit(n, bindings);
            }
            return init;
        }

        protected virtual Expression VisitListInit(ListInitExpression init) {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || initializers != init.Initializers) {
                return Expression.ListInit(n, initializers);
            }
            return init;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression na) {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            if (exprs != na.Expressions) {
                if (na.NodeType == ExpressionType.NewArrayInit) {
                    return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
                } else {
                    return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
                }
            }
            return na;
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv) {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expr = this.Visit(iv.Expression);
            if (args != iv.Arguments || expr != iv.Expression) {
                return Expression.Invoke(expr, args);
            }
            return iv;
        }
    }
}