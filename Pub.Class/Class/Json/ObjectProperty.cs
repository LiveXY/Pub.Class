using System;
using System.Reflection;

namespace Pub.Class {
    /// <summary> 表示一个可以快速获取或者设置其对象属性/字段值的对象
    /// </summary>
    public sealed class ObjectProperty {
        /// <summary> 表示一个可以获取或者设置其内容的对象属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public ObjectProperty(PropertyInfo property) {
            Field = false;
            MemberInfo = property; //属性信息
            OriginalType = property.PropertyType;
            var get = property.GetGetMethod(true); //获取属性get方法,不论是否公开
            var set = property.GetSetMethod(true); //获取属性set方法,不论是否公开
            if (set != null) //set方法不为空                    
            {
                CanWrite = true; //属性可写
                Static = set.IsStatic; //属性是否为静态属性
                IsPublic = set.IsPublic;
            }
            if (get != null) //get方法不为空
            {
                CanRead = true; //属性可读
                Static = get.IsStatic; //get.set只要有一个静态就是静态
                IsPublic = IsPublic || get.IsPublic;
            }
            Init();
        }

        /// <summary> 表示一个可以获取或者设置其内容的对象字段
        /// </summary>
        /// <param name="field">字段信息</param>
        public ObjectProperty(FieldInfo field) {
            Field = true; //是一个字段
            MemberInfo = field; //字段信息
            OriginalType = field.FieldType; //字段值类型
            Static = field.IsStatic; //字段是否是静态的
            IsPublic = field.IsPublic; //字段是否是公开的
            CanWrite = !field.IsInitOnly; //是否可写取决于ReadOnly
            CanRead = true; //字段一定可以读
            Init();
        }

        #region 只读属性

        /// <summary> 
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary> 是否为可空值类型
        /// </summary>
        public bool Nullable { get; private set; }

        /// <summary> 属性/字段的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary> 属性/字段的类型
        /// </summary>
        public Type MemberType { get; private set; }

        /// <summary> 原始类型
        /// </summary>
        public Type OriginalType { get; private set; }

        /// <summary> 属性/字段所属对象的类型
        /// </summary>
        public Type ClassType { get; private set; }

        /// <summary> 属性/字段是否是静态
        /// </summary>
        public bool Static { get; private set; }

        /// <summary> 属性/字段是否可读
        /// </summary>
        public bool CanRead { get; private set; }

        /// <summary> 属性/字段是否可写
        /// </summary>
        public bool CanWrite { get; private set; }

        /// <summary> 当前对象是否是字段
        /// </summary>
        public bool Field { get; private set; }

        /// <summary> 当前对象是否是公开的属性/字段
        /// </summary>
        public bool IsPublic { get; private set; }

        #endregion

        #region 属性/字段访问,设置委托

        /// <summary> 用于读取对象当前属性/字段的委托
        /// </summary>
        public LiteracyGetter Getter { get; private set; }

        /// <summary> 用于设置对象当前属性/字段的委托
        /// </summary>
        public LiteracySetter Setter { get; private set; }

        #endregion

        #region 延迟编译

        private object PreGetter(object instance) {
            LoadGetter();
            return Getter(instance);
        }

        private void PreSetter(object instance, object value) {
            LoadSetter();
            Setter(instance, value);
        }

        #endregion

        #region 编译

        /// <summary> 加载Getter
        /// </summary> 
        public void LoadGetter() {
            if (Getter != PreGetter) {
                return;
            }

            lock (this) {
                if (Getter == PreGetter) //Getter未编译
                {
                    if (!CanRead) //当前对象不可读
                    {
                        Getter = ErrorGetter;
                    } else if (Field) {
                        Getter = Literacy.CreateGetter((FieldInfo)MemberInfo);
                    } else {
                        Getter = Literacy.CreateGetter((PropertyInfo)MemberInfo);
                    }
                }
            }
        }

        /// <summary> 加载Setter
        /// </summary> 
        public void LoadSetter() {
            if (Setter != PreSetter) {
                return;
            }
            lock (this) {
                if (Setter == PreSetter) //Setter未编译
                {
                    if (!CanWrite) //当前成员可读
                    {
                        Setter = ErrorSetter;
                    } else if (Field) {
                        Setter = Literacy.CreateSetter((FieldInfo)MemberInfo);
                    } else {
                        Setter = Literacy.CreateSetter((PropertyInfo)MemberInfo);
                    }
                }
            }
        }

        #endregion

        #region 异常

        private object ErrorGetter(object instance) {
            throw new MethodAccessException("属性没有公开的Get访问器");
        }

        private void ErrorSetter(object instance, object value) {
            if (Field) //如果当前成员不可写,则绑定异常方法
            {
                throw new FieldAccessException("无法设置ReadOnly字段");
            }
            throw new MethodAccessException("属性没有公开的Set访问器");
        }

        #endregion

        /// <summary> 初始化
        /// </summary>
        private void Init() {
            Name = MemberInfo.Name;
            ClassType = MemberInfo.DeclaringType;
            if (OriginalType.IsValueType) {
                MemberType = System.Nullable.GetUnderlyingType(OriginalType);
                Nullable = MemberType != null;
            }
            MemberType = MemberType ?? OriginalType;
            Getter = PreGetter;
            Setter = PreSetter;
        }

        /// <summary> 获取对象的属性/字段值
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的对象</param>
        /// <exception cref="ArgumentNullException">实例属性instance对象不能为null</exception>
        /// <exception cref="ArgumentException">对象无法获取属性/字段的值</exception>
        public object GetValue(object instance) {
            if (!CanRead) {
                ErrorGetter(null);
            } else if (instance == null) {
                if (Static == false) {
                    throw new ArgumentNullException("instance", "实例属性对象不能为null");
                }
            } else if (ClassType.IsInstanceOfType(instance) == false) {
                throw new ArgumentException("对象[" + instance + "]无法获取[" + MemberInfo + "]的值");
            }
            return Getter(instance);
        }

        /// <summary> 尝试获取对象的属性/字段值,失败返回false
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的对象</param>
        /// <param name="value">成功将值保存在value,失败返回null</param>
        public bool TryGetValue(object instance, out object value) {
            if (!CanWrite) {
                value = null;
                return false;
            }
            if (instance == null) {
                if (Static == false) {
                    value = null;
                    return false;
                }
            } else if (ClassType.IsInstanceOfType(instance) == false) {
                value = null;
                return false;
            }
            value = Getter(instance);
            return true;
        }

        /// <summary> 设置对象的属性/字段值
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的实例对象</param>
        /// <param name="value">将要设置的值</param>
        /// <exception cref="ArgumentNullException">实例属性instance对象不能为null</exception>
        public void SetValue(object instance, object value) {
            if (!CanRead) {
                ErrorSetter(null, null);
            } else if (instance == null) {
                if (Static == false) {
                    throw new ArgumentNullException("instance", "实例属性对象不能为null");
                }
            } else if ((OriginalType.IsClass || Nullable) && (value == null || value is DBNull)) {
                Setter(instance, null);
                return;
            }
            if (MemberType.IsInstanceOfType(value) == false) {
                value = Convert.ChangeType(value, MemberType);
            }
            Setter(instance, value);
        }

        /// <summary> 尝试设置对象的属性/字段值,失败返回false
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的对象</param>
        /// <param name="value">成功将值保存在value,失败返回null</param>
        public bool TrySetValue(object instance, object value) {
            if (!CanWrite) {
                return false;
            }
            if (instance == null) {
                if (Static == false) {
                    return false;
                }
            } else if (ClassType.IsInstanceOfType(instance) == false) {
                return false;
            } else if ((OriginalType.IsClass || Nullable) && (value == null || value is DBNull)) {
                Setter(instance, null);
                return true;
            }

            try {
                if (MemberType.IsInstanceOfType(value) == false) {
                    value = Convert.ChangeType(value, MemberType);
                }
                Setter(instance, value);
                return true;
            } catch {
                return false;
            }
        }
    }
}