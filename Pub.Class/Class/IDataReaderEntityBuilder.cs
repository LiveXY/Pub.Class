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
    /// IDataReader 转实体IList
    /// 
    /// 修改纪录
    ///     2009.12.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    /// <typeparam name="Entity">实体类</typeparam>
    public class IDataReaderEntityBuilder<Entity> {
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private delegate Entity Load(IDataRecord dataRecord);
        private Load handler;
        private IDataReaderEntityBuilder() { }
        /// <summary>
        /// 绑定IDataRecord
        /// </summary>
        /// <param name="dataRecord">IDataRecord</param>
        /// <returns>绑定IDataRecord</returns>
        public Entity Build(IDataRecord dataRecord) { return handler(dataRecord); }
        /// <summary>
        /// IDataRecord转实体
        /// </summary>
        /// <param name="dataRecord">IDataRecord</param>
        /// <returns>IDataRecord转实体</returns>
        public static IDataReaderEntityBuilder<Entity> CreateBuilder(IDataRecord dataRecord) {
            IDataReaderEntityBuilder<Entity> dynamicBuilder = new IDataReaderEntityBuilder<Entity>();
            DynamicMethod method = new DynamicMethod("IDataReaderDynamicCreateEntity", typeof(Entity), new Type[] { typeof(IDataRecord) }, typeof(Entity), true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(Entity));
            generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < dataRecord.FieldCount; i++) {
                PropertyInfo propertyInfo = typeof(Entity).GetProperty(dataRecord.GetName(i));
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
