//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Text;
using System.Reflection.Emit;
using System.Data;
using System.Reflection;

namespace Pub.Class {
    /// <summary>
    /// DataTable转实体IList
    /// 
    /// 修改纪录
    ///     2009.12.10 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// DataTableEntityBuilder&lt;TResult> eblist = DataTableEntityBuilder&lt;TResult>.CreateBuilder(dt.Rows[0]);
    /// foreach (DataRow info in dt.Rows) list.Add(eblist.Build(info));
    /// </example>
    /// </code>
    /// </summary>
    /// <typeparam name="Entity">实体类</typeparam>
    public class DataTableEntityBuilder<Entity> {
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
        private delegate Entity Load(DataRow dataRecord);
        private Load handler;
        private DataTableEntityBuilder() { }
        /// <summary>
        /// 绑定DataRow
        /// </summary>
        /// <param name="dataRecord">DataRow</param>
        /// <returns>绑定DataRow</returns>
        public Entity Build(DataRow dataRecord) { return handler(dataRecord); }
        /// <summary>
        /// DataRow转实体
        /// </summary>
        /// <param name="dataRecord">DataRow</param>
        /// <returns>DataRow转实体</returns>
        public static DataTableEntityBuilder<Entity> CreateBuilder(DataRow dataRecord) {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod method = new DynamicMethod("DataTableDynamicCreateEntity", typeof(Entity), new Type[] { typeof(DataRow) }, typeof(Entity), true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(Entity));
            generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < dataRecord.ItemArray.Length; i++) {
                PropertyInfo propertyInfo = typeof(Entity).GetProperty(dataRecord.Table.Columns[i].ColumnName);
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo.IsNotNull() && propertyInfo.GetSetMethod().IsNotNull()) {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }
    }
}
