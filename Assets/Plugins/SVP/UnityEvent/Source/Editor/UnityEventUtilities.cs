using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{
    using Reflection;

    public static class UnityEventUtilities
    {
        #region Unity Event Item

        private const string _unityEventItemFullName = "UnityEditor.UIElements.UnityEventItem";
        private const string _unityEventBindFieldsMethod = "BindFields";

        #endregion Unity Event Item

        #region Properties

        public const string CALLS_PROPERTY = "m_PersistentCalls.m_Calls";
        public const string ARGUMENTS_PROPERTY = "m_Arguments";
        public const string TARGET_PROPERTY = "m_Target";
        public const string MODE_PROPERTY = "m_Mode";
        public const string CALL_STATE_PROPERTY = "m_CallState";
        public const string METHOD_NAME_PROPERTY = "m_MethodName";
        public const string OBJECT_ARGUMENT_PROPERTY = "m_ObjectArgument";

        public static SerializedProperty GetModeProperty(this SerializedProperty listener)
        {
            return listener.FindPropertyRelative(MODE_PROPERTY);
        }

        public static SerializedProperty GetArgumentsProperty(this SerializedProperty listener)
        {
            return listener.FindPropertyRelative(ARGUMENTS_PROPERTY);
        }

        public static SerializedProperty GetTargetProperty(this SerializedProperty listener)
        {
            return listener.FindPropertyRelative(TARGET_PROPERTY);
        }

        public static SerializedProperty GetCallStateProperty(this SerializedProperty listener)
        {
            return listener.FindPropertyRelative(CALL_STATE_PROPERTY);
        }

        public static SerializedProperty GetMethodNameProperty(this SerializedProperty listener)
        {
            return listener.FindPropertyRelative(METHOD_NAME_PROPERTY);
        }

        public static SerializedProperty GetObjectArgumentProperty(this SerializedProperty argumentsProperty)
        {
            return argumentsProperty.FindPropertyRelative(OBJECT_ARGUMENT_PROPERTY);
        }

        #endregion Properties

        #region Property Data

        private const string _propertyDataTypeName = "PropertyData";
        private const string _modeField = "mode";
        private const string _argumentsField = "arguments";
        private const string _callStateField = "callState";
        private const string _listenerTargetField = "listenerTarget";
        private const string _methodNameField = "methodName";
        private const string _objectArgumentField = "objectArgument";

        public static object CreatePropertyDataForListener(SerializedProperty listenerProperty)
        {
            SerializedProperty listenerModeProperty = listenerProperty.GetModeProperty();
            SerializedProperty listenerArgumentsProperty = listenerProperty.GetArgumentsProperty();
            SerializedProperty listenerCallStateProperty = listenerProperty.GetCallStateProperty();
            SerializedProperty listenerTargetProperty = listenerProperty.GetTargetProperty();
            SerializedProperty listenerMethodNameProperty = listenerProperty.GetMethodNameProperty();
            SerializedProperty argumentsObjectArgumentProperty = listenerArgumentsProperty.GetObjectArgumentProperty();

            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            System.Type propertyDataType = eventDrawerType.GetPrivateNestedType(_propertyDataTypeName);
            object propertyDataInstance = System.Activator.CreateInstance(propertyDataType);

            FieldInfo[] fields = propertyDataType.GetFields();
            foreach (FieldInfo field in fields)
            {
                switch (field.Name)
                {
                    case _modeField:
                        field.SetValue(propertyDataInstance, listenerModeProperty);
                        break;

                    case _argumentsField:
                        field.SetValue(propertyDataInstance, listenerArgumentsProperty);
                        break;

                    case _callStateField:
                        field.SetValue(propertyDataInstance, listenerCallStateProperty);
                        break;

                    case _listenerTargetField:
                        field.SetValue(propertyDataInstance, listenerTargetProperty);
                        break;

                    case _methodNameField:
                        field.SetValue(propertyDataInstance, listenerMethodNameProperty);
                        break;

                    case _objectArgumentField:
                        field.SetValue(propertyDataInstance, argumentsObjectArgumentProperty);
                        break;
                }
            }

            return propertyDataInstance;
        }

        #endregion Property Data

        public static VisualElement CreateUnityEventItem()
        {
            System.Type unityEventItemType = ReflectionHelper.ByName(_unityEventItemFullName);
            return System.Activator.CreateInstance(unityEventItemType) as VisualElement;
        }

        public static void ItemBindFields(this VisualElement element, object propertyData, System.Func<GenericMenu> createMenuCallback, System.Func<string, string> formatSelectedValuCallback, System.Func<SerializedProperty> getArgumentCallback)
        {
            System.Type unityEventItemType = ReflectionHelper.ByName(_unityEventItemFullName);
            MethodInfo bindFieldsMethod = unityEventItemType.GetPrivateMethod(_unityEventBindFieldsMethod);
            bindFieldsMethod.Invoke(element, new object[] { propertyData, createMenuCallback, formatSelectedValuCallback, getArgumentCallback });
        }
    }
}