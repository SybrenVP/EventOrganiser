using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{
    using static UnityEventUtilities;

    public class UnityEventItem : VisualElement
    {
        internal void BindFields(PropertyData propertyData, Func<GenericMenu>, Func<string, string> formatSelectedValueCallback, Func<SerializedProperty> getArgumentCallback)
        {

        }
    }
}