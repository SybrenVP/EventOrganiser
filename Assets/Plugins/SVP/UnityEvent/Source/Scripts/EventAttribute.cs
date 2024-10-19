using System;
using UnityEngine;

namespace SVP.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventAttribute : PropertyAttribute
    {
        public string Group = string.Empty;
        public int Order = -1;

        public EventAttribute(string group)
        {
            Group = group;
        }

        public EventAttribute(int order)
        {
            Order = order;
        }

        public EventAttribute(string group, int order)
        {
            Group = group;
            Order = order;
        }
    }
}