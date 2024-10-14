using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SVP.Reflection
{
    public static class ReflectionHelper
    {
        private const BindingFlags INSTANCE_PRIVATE_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags INSTANCE_PUBLIC_FLAGS = BindingFlags.Public | BindingFlags.Instance;
        private const BindingFlags STATIC_PRIVATE_FLAGS = BindingFlags.NonPublic | BindingFlags.Static;

        #region Types

        public static Type ByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }

            return null;
        }

        public static Type GetPrivateNestedType(this Type type, string name)
        {
            return type.GetNestedType(name, BindingFlags.NonPublic);
        }

        #endregion Types

        #region Methods

        public static T InvokePrivateMethod<T>(this Type type, string name, object target, object[] parameters)
        {
            MethodInfo method = type.GetPrivateMethod(name);
            if (method == null)
            {
                return default(T);
            }
            return (T)method.Invoke(target, parameters);
        }

        public static T InvokeStaticPrivateMethod<T>(this Type type, string name, object[] parameters)
        {
            MethodInfo method = type.GetStaticPrivateMethod(name);
            if (method == null)
            {
                return default(T);
            }
            return (T)method.Invoke(null, parameters);
        }

        public static void InvokeStaticPrivateMethod(this Type type, string name, object[] parameters)
        {
            MethodInfo method = type.GetStaticPrivateMethod(name);
            if (method == null)
            {
                return;
            }
            method.Invoke(null, parameters);
        }

        public static MethodInfo GetPrivateMethod(this Type type, string name)
        {
            return GetMethod(type, name, INSTANCE_PRIVATE_FLAGS);
        }
        
        public static MethodInfo GetStaticPrivateMethod(this Type type, string name)
        {
            return GetMethod(type, name, STATIC_PRIVATE_FLAGS);
        }

        private static MethodInfo GetMethod(Type type, string name, BindingFlags flags)
        {
            MethodInfo result = type.GetMethod(name, flags);

            if (result == null)
            {
                Debug.LogError($"Failed to retrieve method using reflection with name {name} from type {type.Name} using flags {flags}");
            }

            return result;
        }

        #endregion Methods

        #region Fields

        public static T GetPrivateFieldValue<T>(this Type type, object target, string name)
        {
            FieldInfo field = GetPrivateField(type, name);
            if (field == null)
            {
                return default(T);
            }
            return (T)field.GetValue(target);
        }

        public static void SetPrivateFieldValue(this Type type, object target, string name, object value)
        {
            FieldInfo field = GetPrivateField(type, name);
            if (field == null)
            {
                return;
            }
            field.SetValue(target, value);
        }

        public static FieldInfo GetPrivateField(this Type type, string name)
        {
            return GetField(type, name, INSTANCE_PRIVATE_FLAGS);
        }

        private static FieldInfo GetStaticPrivateField(this Type type, string name)
        {
            return GetField(type, name, STATIC_PRIVATE_FLAGS);
        }

        private static FieldInfo GetField(Type type, string name, BindingFlags flags)
        {
            FieldInfo result = type.GetField(name, flags);

            if (result == null)
            {
                Debug.LogError($"Failed to retrieve field using reflection with name {name} from type {type.Name} using flags {flags}");
            }

            return result;
        }

        #endregion
    }
}