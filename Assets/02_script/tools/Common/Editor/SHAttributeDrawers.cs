using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
#endif

// 인스펙터에 함수이름을 버튼형식으로 노출시킵니다.
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class FuncButtonDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        InspectorGUIToFunctionButton();
    }

    void InspectorGUIToFunctionButton()
    {
#if UNITY_EDITOR
        Type pType              = target.GetType();
        MethodInfo[] pMethods   = pType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        for (int iLoop = 0; iLoop < pMethods.Length; ++iLoop)
        {
            MethodInfo pMethod  = pMethods[iLoop];
            object[] pAttribute = pMethod.GetCustomAttributes(typeof(FuncButton), true);
            if (0 >= pAttribute.Length)
                continue;

            if (true == GUILayout.Button(pMethod.Name))
            {
                if (false == Application.isPlaying)
                {
                    EditorUtility.DisplayDialog(pMethod.Name, "실행 후 이용해 주세요!!", "확인");
                    return;
                }

                ((Component)target).SendMessage(pMethod.Name, SendMessageOptions.DontRequireReceiver);
            }
        }
#endif
    }
}

// 인스펙터에 노출되는 필드를 읽기전용으로 노출합니다.
[CustomPropertyDrawer(typeof(ReadOnlyField))]
public class ReadOnlyFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pRect, SerializedProperty pProperty, GUIContent pLabel)
    {
#if UNITY_EDITOR
        GUI.enabled = false;
        switch (pProperty.propertyType)
        {
            case SerializedPropertyType.Integer:
                EditorGUI.LabelField(pRect, pLabel.text, pProperty.intValue.ToString());
                break;

            case SerializedPropertyType.Boolean:
                EditorGUI.LabelField(pRect, pLabel.text, pProperty.boolValue.ToString());
                break;

            case SerializedPropertyType.Float:
                EditorGUI.LabelField(pRect, pLabel.text, pProperty.floatValue.ToString("0.00000"));
                break;

            case SerializedPropertyType.String:
            case SerializedPropertyType.Character:
                EditorGUI.LabelField(pRect, pLabel.text, pProperty.stringValue);
                break;

            case SerializedPropertyType.Color:
                EditorGUI.ColorField(pRect, pLabel.text, pProperty.colorValue);
                break;

            case SerializedPropertyType.ObjectReference:
                EditorGUI.ObjectField(pRect, pLabel.text, pProperty.objectReferenceValue, typeof(System.Object), true);
                break;
                
            case SerializedPropertyType.Vector2:
                EditorGUI.Vector2Field(pRect, pLabel.text, pProperty.vector2Value);
                break;

            case SerializedPropertyType.Vector3:
                EditorGUI.Vector3Field(pRect, pLabel.text, pProperty.vector3Value);
                break;

            case SerializedPropertyType.Vector4:
                EditorGUI.Vector4Field(pRect, pLabel.text, pProperty.vector4Value);
                break;

            case SerializedPropertyType.Quaternion:
                EditorGUI.LabelField(pRect, pLabel.text, pProperty.quaternionValue.ToString());
                break;

            case SerializedPropertyType.Rect:
                EditorGUI.RectField(pRect, pLabel.text, pProperty.rectValue);
                break;
                
            case SerializedPropertyType.ArraySize:
                EditorGUI.LabelField(pRect, pLabel.text, pProperty.arraySize.ToString());
                break;
                
            case SerializedPropertyType.AnimationCurve:
                EditorGUI.CurveField(pRect, pLabel.text, pProperty.animationCurveValue);
                break;

            case SerializedPropertyType.Bounds:
                EditorGUI.BoundsField(pRect, pProperty.boundsValue);
                break;

            case SerializedPropertyType.Gradient:
            default:
                EditorGUI.LabelField(pRect, pLabel.text, "(not supported)");
                break;
        }
        GUI.enabled = true;
#endif
    }
}

// 인스펙터에 같은 타입의 자식오브젝트를 콤보박스 형태로 노출합니다.
[CustomPropertyDrawer(typeof(SelectOnChildren))]
public class SelectOnChildrenDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pRect, SerializedProperty pProperty, GUIContent pLabel)
    {
        System.Type pType = null;

        if (null == pProperty.objectReferenceValue)
        {
            var strParts     = pProperty.propertyPath.Split('.');
            var pCurrentType = pProperty.serializedObject.targetObject.GetType();

            for (int iLoop = 0; iLoop < strParts.Length; ++iLoop)
            {
                if (null == pCurrentType)
                    break;

                var pFileInfo = pCurrentType.GetField(strParts[iLoop], 
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
                if (null == pFileInfo)
                    break;

                pCurrentType = pFileInfo.FieldType;
            }

            pType = pCurrentType;
        }
        else
        {
            pType = pProperty.objectReferenceValue.GetType();
        }

        if (true == pType.IsArray)
        {
            pType = pType.GetElementType();
        }
        else if (true == pType.IsGenericType)
        {
            pType = pType.GetGenericArguments()[0];
        }

        var pRootObject   = pProperty.serializedObject.targetObject as Component;
        var pChildObjects = new List<UnityEngine.Object>();
        var strChildNames = new List<string>();

        pChildObjects.Add(null);
        strChildNames.Add("None");

        if (typeof(GameObject) == pType)
        {
            var pChilds = pRootObject.GetComponentsInChildren(typeof(Transform), true);
            var strName = "";
            foreach (var pChild in pChilds)
            {
                strName = "" + strChildNames.Count + ") " + pChild.gameObject.name;
                pChildObjects.Add(pChild.gameObject);
                strChildNames.Add(strName);
            }
        }
        else if (true == pType.IsSubclassOf(typeof(Component)))
        {
            var pChilds = pRootObject.GetComponentsInChildren(pType, true);
            var strName = "";
            foreach (var pChild in pChilds)
            {
                strName = "" + strChildNames.Count + ") " + pChild.gameObject.name;
                pChildObjects.Add(pChild);
                strChildNames.Add(strName);
            }
        }
        else
        {
            EditorGUI.BeginProperty(pRect, pLabel, pProperty);
            EditorGUI.HelpBox(pRect, "[SelectOnChildren] can only be used with GameObject or Component", MessageType.Error);
            EditorGUI.EndProperty();
            return;
        }
        
        EditorGUI.BeginProperty(pRect, pLabel, pProperty);

        var iOldLevel = EditorGUI.indentLevel;

        EditorStyles.popup.wordWrap = false;
        EditorGUI.indentLevel       = 0;
        
        int iIndex  = pChildObjects.FindIndex(pItem => (pItem == pProperty.objectReferenceValue));
        int iNewIdx = EditorGUI.Popup(EditorGUI.PrefixLabel(pRect, pLabel), iIndex, strChildNames.ToArray());

        if (iIndex != iNewIdx)
        {
            pProperty.objectReferenceValue = pChildObjects[iNewIdx];
            EditorGUIUtility.PingObject(pProperty.objectReferenceValue);
        }

        EditorGUI.indentLevel = iOldLevel;
        EditorGUI.EndProperty();
    }
}