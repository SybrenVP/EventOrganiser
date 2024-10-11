using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{
    using SVP.Reflection;

    public static class UnityEventDrawerUtilities
    {
        #region Reflection Method Names

        public const string CREATE_LIST_VIEW_METHOD = "CreateListView";
        public const string REGISTER_RIGHT_CLICK_MENU_METHOD = "RegisterRightClickMenu";
        public const string GET_DUMMY_EVENT_METHOD = "GetDummyEvent";
        public const string GET_FUNCTION_DROPDOWN_TEXT_METHOD = "GetFunctionDropdownText";
        public const string GET_ARGUMENT_METHOD = "GetArgument";

        #endregion Reflection Method Names

        #region Reflection Field Names

        public const string DUMMY_EVENT_FIELD = "m_DummyEvent";

        #endregion Reflection Field Names

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
    }
}