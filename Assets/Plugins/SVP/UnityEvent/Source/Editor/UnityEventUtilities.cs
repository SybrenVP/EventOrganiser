using UnityEditor;
using UnityEngine;

namespace SVP.Editor.Events
{
    public static class UnityEventUtilities
    {
        public struct PropertyData
        {
            public SerializedProperty Mode;
            public SerializedProperty Arguments;
            public SerializedProperty CallState;
            public SerializedProperty ListenerTarget;
            public SerializedProperty MethodName;
            public SerializedProperty ObjectArgument;
        }

        public const string CALLS_PROPERTY = "m_PersistentCalls.m_Calls";
        public const string ARGUMENTS_PROPERTY = "m_Arguments";
        public const string TARGET_PROPERTY = "m_Target";
        public const string MODE_PROPERTY = "m_Mode";
        public const string CALL_STATE_PROPERTY = "m_CallState";
        public const string METHOD_NAME_PROPERTY = "m_MethodName";
        public const string OBJECT_ARGUMENT_PROPERTY = "m_ObjectArgument";

        public static PropertyData CreatePropertyDataForListener(SerializedProperty listenerProperty)
        {
            SerializedProperty listenerModeProperty = listenerProperty.FindPropertyRelative(MODE_PROPERTY);
            SerializedProperty listenerArgumentsProperty = listenerProperty.FindPropertyRelative(ARGUMENTS_PROPERTY);
            SerializedProperty listenerCallStateProperty = listenerProperty.FindPropertyRelative(CALL_STATE_PROPERTY);
            SerializedProperty listenerTargetProperty = listenerProperty.FindPropertyRelative(TARGET_PROPERTY);
            SerializedProperty listenerMethodNameProperty = listenerProperty.FindPropertyRelative(METHOD_NAME_PROPERTY);
            SerializedProperty argumentsObjectArgumentProperty = listenerArgumentsProperty.FindPropertyRelative(OBJECT_ARGUMENT_PROPERTY);

            PropertyData propertyData = new PropertyData
            {
                Mode = listenerModeProperty,
                Arguments = listenerArgumentsProperty,
                CallState = listenerCallStateProperty,
                ListenerTarget = listenerTargetProperty,
                MethodName = listenerMethodNameProperty,
                ObjectArgument = argumentsObjectArgumentProperty,
            };

            return propertyData;
        }
    }
}