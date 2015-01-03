//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Web.SessionState;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Pub.Class {
    /// <summary>
    /// HttpSessionState扩展
    /// </summary>
    public static class HttpSessionStateExtensions {
        public static T Get<T>(this HttpSessionState state, string key) {
            return state.Get<T>(key, default(T));
        }
        public static T Get<T>(this HttpSessionState state, string key, T defaultValue) {
            var value = state[key];
            return (T)(value ?? defaultValue);
        }
        public static T Ensure<T>(this HttpSessionState state, string key) where T : class, new() {
            var value = state.Get<T>(key);
            if (value == null) {
                value = new T();
                state.Set(key, value);
            }

            return value;
        }
        public static void Set(this HttpSessionState state, string key, object value) {
            state[key] = value;
        }
        public static void SetInSession<T>(this HttpSessionState session, String Key, T o) {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, o);
                byte[] bts = ms.ToArray();
                session[Key] = bts.Compress();
            }
        }
        public static T GetFromSession<T>(this HttpSessionState session, String Key) {
            byte[] bts = (byte[])session[Key];
            byte[] uncompressed = bts.Decompress();
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(uncompressed)) {
                T o = (T)bf.Deserialize(ms);
                return o;
            }
        }
    }
}