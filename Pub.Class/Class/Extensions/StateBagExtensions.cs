//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Web.UI;

namespace Pub.Class {
    /// <summary>
    /// StateBag扩展
    /// </summary>
    public static class StateBagExtensions {
        public static T Get<T>(this StateBag state, string key) {
            return state.Get<T>(key, default(T));
        }
        public static T Get<T>(this StateBag state, string key, T defaultValue) {
            var value = state[key];
            return (T)(value ?? defaultValue);
        }
        public static T Ensure<T>(this StateBag state, string key) where T : class, new() {
            var value = state.Get<T>(key);
            if (value == null) {
                value = new T();
                state.Set(key, value);
            }

            return value;
        }
        public static void Set(this StateBag state, string key, object value) {
            state[key] = value;
        }
    }
}