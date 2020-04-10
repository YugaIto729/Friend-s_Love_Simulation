using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
[CreateAssetMenu(menuName = "Scriptable/Create EventSetingData")]
public class EventSetingData : ScriptableObject
{
    [SerializeField]
    public string[] eventpass;

    [SerializeField]
    public int[] date;

    [SerializeField]
    public string[] formales;

    [HideInInspector]
    public int length = 0;
    [HideInInspector]
    public int dumy_len = 0;

    public string[] Get_EventPass()
    {
        return eventpass;
    }

    public int[] Get_EventDate()
    {
        return date;
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(EventSetingData), true)]
public sealed class Editor_EventSetingData : Editor
{
    //int length = 0;
    //int dumy_len = 0;

    public override void OnInspectorGUI()
    {

        //base.OnInspectorGUI();

        var esd = target as EventSetingData;

        EditorGUILayout.BeginHorizontal();
        {
            esd.length = EditorGUILayout.IntField("length", esd.length);
            if (GUILayout.Button("更新"))
            {
                Create_Array(ref esd.date, ref esd.eventpass, ref esd.formales, esd.length);
            }
        }
        EditorGUILayout.EndHorizontal();


        for(int i=0; i < esd.eventpass.Length; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("時間帯", GUILayout.Width(40));
                    esd.date[i] = EditorGUILayout.IntField(esd.date[i], GUILayout.Width(30));
                    EditorGUILayout.LabelField("イベントパス", GUILayout.Width(70));
                    esd.eventpass[i] = EditorGUILayout.TextField(esd.eventpass[i]);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("発生条件", GUILayout.Width(60));
                    esd.formales[i] = EditorGUILayout.TextField(esd.formales[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        esd.dumy_len = esd.length;

    }

    private void Create_Array(ref int[] date, ref string[] evPass, ref string[] formales, int len)
    {

        int[] vs = new int[len];
        string[] vs1 = new string[len];
        string[] vs2 = new string[len];

        //Debug.Log("int[]: " + vs.Length);
        //Debug.Log("string[]: " + vs1.Length);

        for (int i=0; i < vs.Length; i++)
        { 
            Debug.Log("i: "+i);
            if (i < date.Length)
            {
                if (date.Length > 0)
                {
                    vs[i] = date[i];
                    vs1[i] = evPass[i];
                    vs2[i] = formales[i];
                }
                else
                {
                    vs[i] = 0;
                    vs1[i] = "";
                    vs2[i] = "";
                }
            }
            else
            {
                if (date.Length > 0)
                {
                    vs[i] = date[date.Length - 1];
                    vs1[i] = evPass[evPass.Length - 1];
                    vs2[i] = formales[formales.Length - 1];
                }
                else
                {
                    vs[i] = 0;
                    vs1[i] = "";
                    vs2[i] = "";
                }


            }
        }

        /*
        for (int i=0; i < vs1.Length; i++)
        {
            if (i <= evPass.Length)
            {
                vs1[i] = evPass[i];
            }
            else
            {
                vs1[i] = evPass[evPass.Length - 1];
            }
        }
        */

        date = vs;
        evPass = vs1;
        formales = vs2;
    }
}

#endif
