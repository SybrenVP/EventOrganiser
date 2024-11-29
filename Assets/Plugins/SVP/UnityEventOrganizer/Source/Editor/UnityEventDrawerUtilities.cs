using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SVP.Editor.Events
{
    using SVP.Reflection;
    using System.Reflection;
    using Unity.VisualScripting;

    public static class UnityEventDrawerUtilities
    {
        #region Reflection Type Names

        private const string EMPTY_UNITY_EVENT_FUNCTION_STRUCT = "UnityEventFunction";
        private const string VALID_METHOD_MAP_STRUCT = "ValidMethodMap";

        #endregion

        #region Reflection Method Names

        public const string CREATE_LIST_VIEW_METHOD = "CreateListView";
        public const string REGISTER_RIGHT_CLICK_MENU_METHOD = "RegisterRightClickMenu";
        public const string GET_DUMMY_EVENT_METHOD = "GetDummyEvent";
        public const string GET_FUNCTION_DROPDOWN_TEXT_METHOD = "GetFunctionDropdownText";
        public const string GET_ARGUMENT_METHOD = "GetArgument";
        public const string GET_ROW_RECTS_METHOD = "GetRowRects";
        public const string GET_MODE_METHOD = "GetMode";
        public const string CLEAR_EVENT_FUNCTION_METHOD = "ClearEventFunction";
        public const string GET_METHODS_FOR_TARGET_AND_MODE_METHOD = "GetMethodsForTargetAndMode";
        public const string GET_TYPE_NAME_METHOD = "GetTypeName";
        public const string ADD_FUNCTIONS_FOR_SCRIPT_METHOD = "AddFunctionsForScript";

        #endregion Reflection Method Names

        #region Reflection Field Names

        public const string DUMMY_EVENT_FIELD = "m_DummyEvent";
        private const string LISTENERS_ARRAY_FIELD = "m_ListenersArray";
        private const string VALID_METHOD_MAP_METHOD_INFO_FIELD = "methodInfo";

        #endregion Reflection Field Names

        public static SerializedProperty GetListenersArray(this object target)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.GetPrivateFieldValue<SerializedProperty>(target, LISTENERS_ARRAY_FIELD);
        }

        public static UnityEventBase GetDummyEvent(SerializedProperty eventProperty)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.InvokeStaticPrivateMethod<UnityEventBase>(GET_DUMMY_EVENT_METHOD, new object[] { eventProperty });
        }

        public static UnityEventBase GetDummyEventFieldValue(this object target)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.GetPrivateFieldValue<UnityEventBase>(target, DUMMY_EVENT_FIELD);
        }

        public static void SetDummyEventFieldValue(this object target, UnityEventBase value)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            eventDrawerType.SetPrivateFieldValue(target, DUMMY_EVENT_FIELD, value);
        }

        public static ListView CreateListView(this object target, SerializedProperty eventProperty)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.InvokePrivateMethod<ListView>(CREATE_LIST_VIEW_METHOD, target, new object[] { eventProperty });
        }

        public static void RegisterRightClickMenu(Label label, SerializedProperty eventProperty)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            eventDrawerType.InvokeStaticPrivateMethod(REGISTER_RIGHT_CLICK_MENU_METHOD, new object[] { label, eventProperty });
        }

        public static string GetFunctionDropdownText(this object target, SerializedProperty listener)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.InvokePrivateMethod<string>(GET_FUNCTION_DROPDOWN_TEXT_METHOD, target, new object[] { listener });
        }

        public static SerializedProperty GetArgument(this object target, SerializedProperty listener)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.InvokePrivateMethod<SerializedProperty>(GET_ARGUMENT_METHOD, target, new object[] { listener });
        }

        public static Rect[] GetRowRects(this object target, Rect rect)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.InvokePrivateMethod<Rect[]>(GET_ROW_RECTS_METHOD, target, new object[] { rect });
        }

        public static PersistentListenerMode GetMode(SerializedProperty modeProperty)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.InvokeStaticPrivateMethod<PersistentListenerMode>(GET_MODE_METHOD, new object[] { modeProperty });
        }

        public static void ClearEventFunction(object source)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            eventDrawerType.InvokeStaticPrivateMethod(CLEAR_EVENT_FUNCTION_METHOD, new object[] { source });
        }

        public static object CreateEmptyUnityEventFunction(SerializedProperty listener)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            System.Type emptyUnityEventFunctionType = eventDrawerType.GetPrivateNestedType(EMPTY_UNITY_EVENT_FUNCTION_STRUCT);
            object emptyUnityEventFunction = System.Activator.CreateInstance(emptyUnityEventFunctionType, listener, null, null, PersistentListenerMode.EventDefined);
            return emptyUnityEventFunction;
        }

        public static void GetMethodsForTargetAndMode(Object target, System.Type[] delegateArgumentsTypes, IList methods, PersistentListenerMode mode)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            eventDrawerType.InvokeStaticPrivateMethod(GET_METHODS_FOR_TARGET_AND_MODE_METHOD, new object[] { target, delegateArgumentsTypes.ToArray(), methods, mode });
        }

        public static IList CreateValidMethodMapList()
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            System.Type validMethodMapType = eventDrawerType.GetPrivateNestedType(VALID_METHOD_MAP_STRUCT);
            System.Type listType = typeof(List<>);
            listType = listType.MakeGenericType(validMethodMapType);
            return (IList)System.Activator.CreateInstance(listType);
        }

        public static SVP.Events.EventAttribute GetEventAttributeFromValidMethodMapMethodField(object target)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            System.Type validMethodMapType = eventDrawerType.GetPrivateNestedType(VALID_METHOD_MAP_STRUCT);
            MethodInfo methodInfoFieldValue = validMethodMapType.GetPublicFieldValue<MethodInfo>(target, VALID_METHOD_MAP_METHOD_INFO_FIELD);
            return methodInfoFieldValue.GetCustomAttribute<SVP.Events.EventAttribute>(false);
        }

        public static void SanitizeListForEventGroup(IList list, string group)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                SVP.Events.EventAttribute eventAttribute = GetEventAttributeFromValidMethodMapMethodField(list[i]);
                if (eventAttribute == null || eventAttribute.Group != group)
                {
                    list.RemoveAt(i);
                }
            }
        }

        public static void SanitizeListFromEventGroup(IList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                SVP.Events.EventAttribute eventAttribute = GetEventAttributeFromValidMethodMapMethodField(list[i]);
                if (eventAttribute != null)
                {
                    list.RemoveAt(i);
                }
            }
        }

        public static void SortEventMethodList(IList list)
        {
            for (int i = 1, count = list.Count; i < count; i++)
            {
                object listEntry = list[i];
                object previousListEntry = list[i - 1];

                SVP.Events.EventAttribute eventAttribute = GetEventAttributeFromValidMethodMapMethodField(listEntry);
                SVP.Events.EventAttribute previousEventAttribute = GetEventAttributeFromValidMethodMapMethodField(previousListEntry);

                if (previousEventAttribute.Order < eventAttribute.Order)
                {
                    list[i] = previousListEntry;
                    list[i - 1] = listEntry;
                    i--;
                }
            }
        }

        public static string GetTypeName(System.Type type)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            return eventDrawerType.InvokeStaticPrivateMethod<string>(GET_TYPE_NAME_METHOD, new object[] { type });
        }

        public static void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, object methodMap, string name)
        {
            System.Type eventDrawerType = typeof(UnityEditorInternal.UnityEventDrawer);
            eventDrawerType.InvokeStaticPrivateMethod(ADD_FUNCTIONS_FOR_SCRIPT_METHOD, new object[] { menu, listener, methodMap, name });
        }
    }
}