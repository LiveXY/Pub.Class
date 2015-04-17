
namespace Pub.Class {
    /// <summary> 手动选择加载字段或属性
    /// </summary>
    public interface ILoadMember {
        /// <summary> 加载公开的实例字段
        /// </summary>
        void PublicField();
        /// <summary> 加载非公开的实例字段
        /// </summary>
        void NonPublicField();
        /// <summary> 加载公开的静态的字段,参数hasNonPublic为true,则非公开的静态字段也一起加载
        /// </summary>
        /// <param name="hasNonPublic">是否一起加载非公开的静态字段</param>
        void StaticField(bool hasNonPublic);
        /// <summary> 加载非公开的实例属性
        /// </summary>
        void NonPublicProperty();
        /// <summary> 加载公开的静态的属性,参数hasNonPublic为true,则非公开的静态属性也一起加载
        /// </summary>
        /// <param name="hasNonPublic">是否一起加载非公开的静态字段</param>
        void StaticProperty(bool hasNonPublic);
    }

}
