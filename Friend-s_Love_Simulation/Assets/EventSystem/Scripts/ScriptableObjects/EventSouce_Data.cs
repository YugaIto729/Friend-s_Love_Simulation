using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create EventData")]
public sealed class EventSouce_Data : ScriptableObject
{
    [SerializeField]
    private GameObject[] events;
    
    public List<ScriptEvent> Get_EventSouce()
    {
        var list = new List<ScriptEvent>();

        /*
        foreach(var o in events)
        {
            if (o.GetComponent<ScriptEvent>() != null)
                list.Add(o.GetComponent<ScriptEvent>());
        }
        */

        return list;
    }

}


/// <summary>
/// 内部イベント
/// </summary>
public class ScriptEvent : MonoBehaviour
{
    public virtual void DoEvent()
    {

    }
}