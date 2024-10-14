using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{
    using SVP.Reflection;

    public static class UnityEventDrawerUtilities
    {
        #region Reflection Type Names

        private const string EMPTY_UNITY_EVENT_FUNCTION_STRUCT = "UnityEventFunction";

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

        #endregion Reflection Method Names

        #region Reflection Field Names

        public const string DUMMY_EVENT_FIELD = "m_DummyEvent";
        private const string LISTENERS_ARRAY_FIELD = "m_ListenersArray";

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
    }
}