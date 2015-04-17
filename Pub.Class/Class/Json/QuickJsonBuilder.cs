using System;

namespace Pub.Class {
    /// <summary> 快速的将任意对象转换为Json字符串
    /// </summary>
    public class QuickJsonBuilder : JsonBuilder {
        /// <summary> 将未知对象按属性名和值转换为Json中的键值字符串写入Buffer
        /// </summary>
        /// <param name="obj">非null的位置对象</param>
        protected override void AppendOther(object obj) {
            Type type = obj.GetType();
            Literacy lit = Literacy.Cache(type, true);

            UnsafeAppend('{');
            var ee = lit.Property.GetEnumerator();
            var fix = "";
            while (ee.MoveNext()) {
                var p = ee.Current;
                var value = p.GetValue(obj);
                if (value != null) {
                    UnsafeAppend(fix);
                    AppendKey(p.Name, false);
                    AppendObject(value);
                    fix = ",";
                }
            }

            UnsafeAppend('}');
        }
    }

}
