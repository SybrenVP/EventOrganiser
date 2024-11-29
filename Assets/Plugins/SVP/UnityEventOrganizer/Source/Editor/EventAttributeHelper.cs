using System.Reflection;
using System.Collections.Generic;
using System;

namespace SVP.Editor.Events
{
    using SVP.Events;

    public class EventAttributeHelper
    {
        public static List<MethodInfo> GetAllEventAttributeMethodsForType(Type type)
        {
            MethodInfo[] allMethodsForType = type.GetMethods();

            List<MethodInfo> eventAttributeMethods = new List<MethodInfo>(allMethodsForType.Length);
            foreach (MethodInfo method in allMethodsForType)
            {
                EventAttribute customEventAttribute = method.GetCustomAttribute<EventAttribute>();
                if (customEventAttribute != null)
                {
                    eventAttributeMethods.Add(method);
                }
            }
            return eventAttributeMethods;
        }

        public static bool HasMethodsWithoutEventAttribute(Type type)
        {
            MethodInfo[] allMethodsForType = type.GetMethods();

            foreach (MethodInfo method in allMethodsForType)
            {
                EventAttribute customEventAttribute = method.GetCustomAttribute<EventAttribute>();
                if (customEventAttribute == null)
                {
                    return true;
                }
            }
            return false;
        }

        public static EventAttribute GetEventAttributeForMethod(MethodInfo method)
        {
            return method.GetCustomAttribute<EventAttribute>();
        }
    }
}