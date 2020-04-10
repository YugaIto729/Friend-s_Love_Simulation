using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
[CreateAssetMenu(menuName = "Scriptable/Create ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    /// <summary> アイテムの名前 </summary>
    public string itemName = "Item";

    [SerializeField]
    /// <summary> アイテムの説明 </summary>
    public string description = "Description";

    [SerializeField]
    /// <summary> アイテムイベントのファイル名 </summary>
    public string eventName = "";

    [SerializeField]
    ///<summary> 0=指定出力 1=直接出力 </summary>
    public int eventMode = 0;

    /// <summary> イベントデータ </summary>
    [SerializeField]
    public string eventData = "";


}

#if UNITY_EDITOR

[CustomEditor(typeof(ItemData))]
public class Edit_ItemData : Editor
{
    public override void OnInspectorGUI()
    {
        ItemData item = target as ItemData;

        //base.OnInspectorGUI();

        item.itemName = EditorGUILayout.TextField("アイテムの名前", item.itemName);
        EditorGUILayout.LabelField("アイテムの説明");
        item.description = EditorGUILayout.TextArea(item.description, GUILayout.Height(50));

        item.eventMode = EditorGUILayout.Popup(item.eventMode, new string[] {"指定入力", "直接入力"});
        EditorGUILayout.Space();

        switch (item.eventMode)
        {
            case 0:
                EditorGUILayout.LabelField("text.txtファイルを参照する場合は「text」と入力してください。");
                item.eventName = EditorGUILayout.TextField("ファイルの名前", item.eventName);
                break;
            case 1:
                EditorGUILayout.LabelField("直接イベント内容を記入してください。");
                EditorGUILayout.LabelField("イベント内容");
                item.eventData = EditorGUILayout.TextArea(item.eventData, GUILayout.Height(250));
                break;
        }


    }
}

#endif