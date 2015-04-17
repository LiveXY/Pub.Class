using System;

namespace Pub.Class {
    /// <summary> 操作Json序列化/反序列化的静态对象
    /// </summary>
    public static class Json {
        /// <summary> 将对象转换为Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString(object obj) {
            return new QuickJsonBuilder().ToJsonString(obj);
        }
        /// <summary> 将json字符串转换为指定对象
        /// </summary>
        public static T ToObject<T>(string json) {
            if (json == null || json.Length == 0) {
                return default(T);
            }
            return (T)ToObject(typeof(T), json);
        }
        /// <summary> 将json字符串转换IDictionary或者IList
        /// </summary>
        public static Object ToObject(string json) {
            if (json == null || json.Length == 0) {
                return null;
            }
            return new JsonParser().ToObject(null, json);
        }
        /// <summary> 将json字符串转换为指定对象
        /// </summary>
        public static Object ToObject(Type type, string json) {
            if (type == null) {
                return null;
            }
            if (json == null || json.Length == 0) {
                return null;
            }
            return new JsonParser().ToObject(type, json);
        }
    }
}
