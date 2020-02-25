using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName = "Scriptable/Create DataBase_Item")]
public class Item_DataBase : ScriptableObject
{
    [SerializeField]
    ItemData[] items;

    public List<ItemData> Get_Items()
    {
        return new List<ItemData>(items);
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(Item_DataBase), true)]
public sealed class Editor_Item_Data : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Begin");

        DrawDefaultInspector();

        EditorGUILayout.LabelField("End");
    }
}

#endif
