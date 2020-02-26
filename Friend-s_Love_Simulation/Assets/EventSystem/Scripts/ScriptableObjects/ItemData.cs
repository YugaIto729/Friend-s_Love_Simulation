using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName = "Scriptable/Create ItemData")]
public sealed class ItemData : ScriptableObject
{
    [SerializeField]
    /// <summary> アイテムの名前 </summary>
    public string itemName = "Item";

    [SerializeField]
    /// <summary> アイテムの説明 </summary>
    public string description = "Description";

    [SerializeField]
    /// <summary> アイテムイベントのファイル名 </summary>
    private string eventName = "";

    //[SerializeField]
    /// <summary> 0=直接出力 1=指定出力 </summary>
    //private int eventMode = 1;

    /// <summary> イベントデータ </summary>
    //[SerializeField]
    //private string eventData = "";

    public void Use()
    {
        Debug.LogFormat("{0}を使用しました", itemName);
        //TalkEventManager.instance.EventStart(eventName);
    }

}
