using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Pub.Class {
    /// <summary> 对象属性,字段访问组件
    /// </summary>
    public class Literacy : ILoadMember {
        #region Cache

        /// <summary> 一般缓存
        /// </summary>
        private static readonly Dictionary<Type, Literacy> Items = new Dictionary<Type, Literacy>(255);

        /// <summary> 忽略大小写缓存
        /// </summary>
        private static readonly Dictionary<Type, Literacy> IgnoreCaseItems = new Dictionary<Type, Literacy>(255);

        /// <summary> 方法缓存
        /// </summary>
        private static readonly Dictionary<MethodInfo, LiteracyCaller> Callers =
            new Dictionary<MethodInfo, LiteracyCaller>(255);

        /// <summary> 获取缓存
        /// </summary>
        /// <param name="type">反射对象类型</param>
        /// <param name="ignoreCase">属性/字段名称是否忽略大小写</param>
        /// <exception cref="ArgumentException">缓存中的对象类型与参数type不一致</exception>
        /// <exception cref="ArgumentNullException">参数type为null</exception>
        public static Literacy Cache(Type type, bool ignoreCase) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            Literacy lit;
            Dictionary<Type, Literacy> item = ignoreCase ? IgnoreCaseItems : Items;
            if (item.TryGetValue(type, out lit)) {
                if (lit.Type != type) {
                    throw new ArgumentException("缓存中的对象类型与参数type不一致!");
                }
            } else {
                lock (item) {
                    if (item.TryGetValue(type, out lit) == false) {
                        lit = new Literacy(type, ignoreCase);
                        item.Add(type, lit);
                    }
                }
            }
            return lit;
        }

        /// <summary> 获取缓存
        /// </summary>
        /// <param name="method">调用方法</param>
        /// <exception cref="ArgumentNullException">参数method为null</exception>
        public static LiteracyCaller Cache(MethodInfo method) {
            if (method == null) {
                throw new ArgumentNullException("method");
            }
            LiteracyCaller caller;
            if (Callers.TryGetValue(method, out caller)) {
                return caller;
            }
            lock (Callers) {
                if (Callers.TryGetValue(method, out caller) == false) {
                    caller = CreateCaller(method);
                    Callers.Add(method, caller);
                }
            }
            return caller;
        }

        #endregion

        #region static

        /// <summary> typeof(Object)
        /// </summary>
        private static readonly Type TypeObject = typeof(Object);

        /// <summary> [ typeof(Object) ]
        /// </summary>
        private static readonly Type[] TypesObject = { typeof(Object) };

        /// <summary> [ typeof(Object),typeof(Object) ]
        /// </summary>
        private static readonly Type[] Types2Object = { typeof(Object), typeof(Object) };

        /// <summary> [ typeof(object[]) ]
        /// </summary>
        private static readonly Type[] TypesObjects = { typeof(object[]) };

        /// <summary> [ typeof(Object), typeof(object[])  ]
        /// </summary>
        private static readonly Type[] TypesObjectObjects = { typeof(Object), typeof(object[]) };

        #endregion

        /// <summary> 对象类型
        /// </summary>
        public Type Type {
            get;
            private set;
        }

        /// <summary> 对象属性集合
        /// </summary>
        public ObjectPropertyCollection Property {
            get;
            private set;
        }

        /// <summary> 对象字段集合
        /// </summary>
        public ObjectPropertyCollection Field {
            get;
            private set;
        }

        #region 私有的

        /// <summary> 对象无参构造器
        /// </summary>
        private LiteracyNewObject _CallNewObject;

        /// <summary> 
        /// </summary>
        /// <param name="args"></param>
        private object PreNewObject(params object[] args) {
            _CallNewObject = CreateNewObject(Type) ?? ErrorNewObject;
            return _CallNewObject();
        }

        private object ErrorNewObject(params object[] args) {
            throw new Exception("没有无参的构造函数");
        }

        #endregion

        #region 构造函数

        /// <summary> 初始化对象属性,字段访问组件,建立大小写敏感的访问实例
        /// </summary>
        /// <param name="type">需快速访问的类型</param>
        public Literacy(Type type)
            : this(type, false) {
        }

        /// <summary> 初始化对象属性,字段访问组件,ignoreCase参数指示是否需要区分大小写
        /// </summary>
        /// <param name="type">需快速访问的类型</param>
        /// <param name="ignoreCase">是否区分大小写(不区分大小写时应保证类中没有同名的(仅大小写不同的)属性或字段)</param>
        public Literacy(Type type, bool ignoreCase) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            Type = type;
            _CallNewObject = PreNewObject;
            Property = new ObjectPropertyCollection(ignoreCase);
            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if (p.GetIndexParameters().Length == 0) //排除索引器
                {
                    if (!Property.ContainsKey(p.Name)) {
                        var a = new ObjectProperty(p);
                        Property.Add(a);
                    }
                }
            }
        }

        #endregion

        /// <summary> 调用对象的无参构造函数,新建对象
        /// </summary>
        public object NewObject() {
            return _CallNewObject();
        }

        #region ILoadMember

        /// <summary> 加载标识 
        /// <para>1  公共实例字段</para>
        /// <para>2  非公共实例字段</para>
        /// <para>4  静态字段</para>
        /// <para>8  非公共实例属性</para>
        /// <para>16 静态属性</para>
        /// </summary>
        private int _LoadFlag;

        /// <summary> 加载更多的属性或字段
        /// </summary>
        public ILoadMember Load {
            get {
                return this;
            }
        }

        #region ILoadMember
        /// <summary> 加载公开的实例字段
        /// </summary>
        void ILoadMember.PublicField() {
            if (Loaded(1)) {
                return;
            }
            if (Field == null) {
                Field = new ObjectPropertyCollection(Property.IgnoreCase);
            }
            const BindingFlags bf = BindingFlags.Public | BindingFlags.Instance;
            foreach (var f in Type.GetFields(bf)) {
                if (f.Name.Contains("<") == false) {
                    Field.Add(new ObjectProperty(f));
                }
            }
            Monitor.Exit(this);
        }

        /// <summary> 加载非公开的实例字段
        /// </summary>
        void ILoadMember.NonPublicField() {
            if (Loaded(2)) {
                return;
            }
            if (Field == null) {
                Field = new ObjectPropertyCollection(Property.IgnoreCase);
            }
            const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var f in Type.GetFields(bf)) {
                if (f.Name.Contains("<") == false) {
                    Field.Add(new ObjectProperty(f));
                }
            }
            Monitor.Exit(this);
        }

        /// <summary> 加载公开的静态的字段,参数hasNonPublic为true,则非公开的静态字段也一起加载
        /// </summary>
        /// <param name="hasNonPublic">是否一起加载非公开的静态字段</param>
        void ILoadMember.StaticField(bool hasNonPublic) {
            if (Loaded(hasNonPublic ? 3 : 4)) {
                return;
            }
            if (Field == null) {
                Field = new ObjectPropertyCollection(Property.IgnoreCase);
            }
            var bf = BindingFlags.Public | BindingFlags.Static;
            if (hasNonPublic) {
                bf |= BindingFlags.NonPublic;
            }
            foreach (var f in Type.GetFields(bf)) {
                if (f.Name.Contains("<") == false && Field.ContainsKey(f.Name) == false) {
                    Field.Add(new ObjectProperty(f));
                }
            }
            Monitor.Exit(this);
        }

        /// <summary> 加载非公开的实例属性
        /// </summary>
        void ILoadMember.NonPublicProperty() {
            if (Loaded(5)) {
                return;
            }
            const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var p in Type.GetProperties(bf)) {
                if (p.GetIndexParameters().Length == 0) {
                    Property.Add(new ObjectProperty(p));
                }
            }
            Monitor.Exit(this);
        }

        /// <summary> 加载公开的静态的属性,参数hasNonPublic为true,则非公开的静态属性也一起加载
        /// </summary>
        /// <param name="hasNonPublic">是否一起加载非公开的静态字段</param>
        void ILoadMember.StaticProperty(bool hasNonPublic) {
            if (Loaded(hasNonPublic ? 6 : 7)) {
                return;
            }
            var bf = BindingFlags.Public | BindingFlags.Static;
            if (hasNonPublic) {
                bf |= BindingFlags.NonPublic;
            }
            foreach (var p in Type.GetProperties(bf)) {
                if (p.GetIndexParameters().Length == 0) {
                    Property.Add(new ObjectProperty(p));
                }
            }
            Monitor.Exit(this);
        }
        #endregion

        /// <summary> 判断是否已加载
        /// </summary>
        /// <param name="flag">加载标识</param>
        private bool Loaded(int flag) {
            flag = 1 << flag;
            if ((_LoadFlag & flag) == 0) {
                Monitor.Enter(this);
                if ((_LoadFlag & flag) == 0) {
                    _LoadFlag |= flag;
                    return false;
                }
                Monitor.Exit(this);
            }
            return true;
        }

        #endregion

        #region 静态的

        /// <summary> IL构造一个用于调用对象构造函数的委托
        /// </summary>
        /// <param name="type">获取构造函数的对象</param>
        /// <param name="argTypes">构造函数的参数,默认null</param>
        /// <exception cref="ArgumentNullException">参数type为null</exception>
        public static LiteracyNewObject CreateNewObject(Type type, Type[] argTypes = null) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            if (type.IsValueType && (argTypes == null || argTypes.Length == 0)) {
                var dm = new DynamicMethod("", TypeObject, TypesObjects, true);
                var il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldloca_S, il.DeclareLocal(type));
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
                return (LiteracyNewObject)dm.CreateDelegate(typeof(LiteracyNewObject));
            }
            if (argTypes == null) {
                argTypes = Type.EmptyTypes;
            }
            const BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var ctor = type.GetConstructor(bf, null, argTypes, null) ?? type.TypeInitializer;
            if (ctor == null) {
                return null;
            }
            return CreateNewObject(ctor);
        }

        /// <summary> IL构造一个用于调用对象构造函数的委托
        /// </summary>
        /// <param name="ctor">构造函数</param>
        /// <exception cref="NotImplementedException">不支持结构的有参构造函数</exception>
        public static LiteracyNewObject CreateNewObject(ConstructorInfo ctor) {
            if (ctor == null) {
                return null;
            }
            Type type = ctor.DeclaringType;
            var dm = new DynamicMethod("", TypeObject, TypesObjects, true);
            var ps = ctor.GetParameters();
            var il = dm.GetILGenerator();

            for (int i = 0; i < ps.Length; i++) {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCast(il, ps[i].ParameterType, false);
            }
            il.Emit(OpCodes.Newobj, ctor);
            if (type.IsValueType) {
                il.Emit(OpCodes.Box, type);
            }

            il.Emit(OpCodes.Ret);
            return (LiteracyNewObject)dm.CreateDelegate(typeof(LiteracyNewObject));
        }

        /// <summary> IL构造一个用于获取对象属性值的委托
        /// </summary>
        public static LiteracyGetter CreateGetter(PropertyInfo prop) {
            if (prop == null) {
                return null;
            }
            var dm = new DynamicMethod("", TypeObject, TypesObject, true);
            var il = dm.GetILGenerator();
            var met = prop.GetGetMethod(true);
            if (met == null) {
                return null;
            }
            if (met.IsStatic) {
                il.Emit(OpCodes.Call, met);
            } else {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, prop.DeclaringType);
                if (prop.DeclaringType.IsValueType) {
                    il.Emit(OpCodes.Call, met);
                } else {
                    il.Emit(OpCodes.Callvirt, met);
                }
            }
            if (prop.PropertyType.IsValueType) {
                il.Emit(OpCodes.Box, prop.PropertyType);
            }
            il.Emit(OpCodes.Ret);
            return (LiteracyGetter)dm.CreateDelegate(typeof(LiteracyGetter));
        }

        /// <summary> IL构造一个用于获取对象字段值的委托
        /// </summary>
        public static LiteracyGetter CreateGetter(FieldInfo field) {
            if (field == null) {
                return null;
            }
            var dm = new DynamicMethod("", TypeObject, TypesObject, true);
            var il = dm.GetILGenerator();
            if (field.IsStatic) {
                il.Emit(OpCodes.Ldsfld, field);
            } else {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, field.DeclaringType);
                il.Emit(OpCodes.Ldfld, field);
            }
            if (field.FieldType.IsValueType) {
                il.Emit(OpCodes.Box, field.FieldType);
            }
            il.Emit(OpCodes.Ret);
            return (LiteracyGetter)dm.CreateDelegate(typeof(LiteracyGetter));
        }

        /// <summary> IL构造一个用于设置对象属性值的委托
        /// </summary>
        public static LiteracySetter CreateSetter(PropertyInfo prop) {
            if (prop == null) {
                return null;
            }
            if (prop.DeclaringType.IsValueType) //值类型无法通过方法给其属性或字段赋值
            {
                throw new NotSupportedException("不支持值类型成员的赋值操作");
            }
            var dm = new DynamicMethod("", null, Types2Object, true);
            var set = prop.GetSetMethod(true);
            if (set == null) {
                return null;
            }
            var il = dm.GetILGenerator();

            if (set.IsStatic) {
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, prop.PropertyType, false);
                il.Emit(OpCodes.Call, set);
            } else {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, prop.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, prop.PropertyType, false);
                il.Emit(OpCodes.Callvirt, set);
            }
            il.Emit(OpCodes.Ret);

            return (LiteracySetter)dm.CreateDelegate(typeof(LiteracySetter));
        }

        /// <summary> IL构造一个用于设置对象字段值的委托
        /// </summary>
        public static LiteracySetter CreateSetter(FieldInfo field) {
            if (field == null || field.IsInitOnly) {
                return null;
            }
            if (field.DeclaringType.IsValueType) //值类型无法通过方法给其属性或字段赋值
            {
                return null;
            }
            var dm = new DynamicMethod("", null, Types2Object, true);
            var il = dm.GetILGenerator();

            if (field.IsStatic) {
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, field.FieldType, false);
                il.Emit(OpCodes.Stsfld, field);
            } else {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, field.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, field.FieldType, false);
                il.Emit(OpCodes.Stfld, field);
            }
            il.Emit(OpCodes.Ret);
            return (LiteracySetter)dm.CreateDelegate(typeof(LiteracySetter));
        }

        /// <summary> IL构造一个用于执行方法的委托
        /// </summary>
        /// <param name="method">方法</param>
        public static LiteracyCaller CreateCaller(MethodInfo method) {
            if (method == null) {
                return null;
            }

            var dm = new DynamicMethod("", TypeObject, TypesObjectObjects, true);

            var il = dm.GetILGenerator();

            var isRef = false;

            var ps = method.GetParameters();
            LocalBuilder[] loc = new LocalBuilder[ps.Length];
            for (int i = 0; i < ps.Length; i++) {
                var p = ps[i];
                Type pt = p.ParameterType;
                if (pt.IsByRef) //ref,out获取他的实际类型
                {
                    isRef = true;
                    pt = pt.GetElementType();
                }

                loc[i] = il.DeclareLocal(pt);
                if (p.IsOut == false) {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    EmitCast(il, pt, false);
                    il.Emit(OpCodes.Stloc, loc[i]); //保存到本地变量
                }
            }

            if (method.IsStatic == false) {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, method.DeclaringType);
            }
            //将参数加载到参数堆栈
            foreach (var pa in method.GetParameters()) {
                if (pa.ParameterType.IsByRef) //out或ref
                {
                    il.Emit(OpCodes.Ldloca_S, loc[pa.Position]);
                } else {
                    il.Emit(OpCodes.Ldloc, loc[pa.Position]);
                    loc[pa.Position] = null;
                }
            }
            LocalBuilder ret = null;
            if (method.ReturnType != typeof(void)) {
                ret = il.DeclareLocal(method.ReturnType);
            }

            if (method.IsStatic || method.DeclaringType.IsValueType) {
                il.Emit(OpCodes.Call, method);
            } else {
                il.Emit(OpCodes.Callvirt, method);
            }

            //设置参数
            if (isRef) {
                for (int i = 0; i < loc.Length; i++) {
                    var l = loc[i];
                    if (l == null) {
                        continue;
                    }
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldloc, l);
                    if (l.LocalType.IsValueType) {
                        il.Emit(OpCodes.Box, l.LocalType);
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            if (ret == null) {
                il.Emit(OpCodes.Ldnull);
            } else if (method.ReturnType.IsValueType) {
                il.Emit(OpCodes.Box, method.ReturnType);
            } else {
                il.Emit(OpCodes.Castclass, typeof(object));
            }

            il.Emit(OpCodes.Ret);

            return (LiteracyCaller)dm.CreateDelegate(typeof(LiteracyCaller));
        }

        /// <summary> IL类型转换指令
        /// </summary>
        private static void EmitCast(ILGenerator il, Type type, bool check = true) {
            if (type.IsValueType) {
                il.Emit(OpCodes.Unbox_Any, type);
                if (check && Nullable.GetUnderlyingType(type) == null) {
                    var t = il.DeclareLocal(type);
                    il.Emit(OpCodes.Stloc, t);
                    il.Emit(OpCodes.Ldloca_S, t);
                }
            } else {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        #endregion
    }
}