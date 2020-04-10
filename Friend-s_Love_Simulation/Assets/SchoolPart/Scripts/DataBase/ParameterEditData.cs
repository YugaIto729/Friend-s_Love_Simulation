using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
[CreateAssetMenu(menuName = "Scriptable/Create ParameterEditData")]
public sealed class ParameterEditData : ScriptableObject
{
    [SerializeField]
    public string[] vs;


    public string[] GetData()
    {
        return vs;
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(ParameterEditData), true)]
public sealed class Editor_ParameterEditData : Editor
{
    bool foldout = true;
    bool foldIns = false;

    public override void OnInspectorGUI()
    {

        //base.OnInspectorGUI();

        var ped = target as ParameterEditData;

        var _style = new GUIStyle()
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
        };

        EditorGUILayout.LabelField("「パラメータの上がりやすさ」", _style);

        if (foldout = EditorGUILayout.Foldout(foldout, "パラメーター一覧"))
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {

                
                EditorGUILayout.LabelField("[31] = 素直");
                EditorGUILayout.LabelField("[32] = 積極性");
                EditorGUILayout.LabelField("[33] = 優しさ");
                EditorGUILayout.LabelField("[34] = 面倒見");
                EditorGUILayout.LabelField("[35] = 寛容");
                EditorGUILayout.LabelField("[36] = 度胸");
                EditorGUILayout.LabelField("[37] = アホ");
                EditorGUILayout.LabelField("[38] = 性欲");
                EditorGUILayout.LabelField("[39] = 筋力");
                EditorGUILayout.LabelField("[40] = おしゃれ");
                EditorGUILayout.LabelField("[30] = 上昇値基準率(0.2固定)");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("※現状では符号が使えないため以下のように記述すること");
                EditorGUILayout.LabelField(@"-5 → (\rv * 5)   -1*([31]*[30]*0.1 → \rv*([31]*[30]*0.1");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("ex.");
                EditorGUILayout.SelectableLabel("少し上がりやすい面倒見 [34] * [30] * 1.2");
                EditorGUILayout.SelectableLabel("上がりにくい度胸 [36] * [30] * 0.5");

            }
            EditorGUILayout.EndVertical();
        }
        

        //EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            string[] vs = new string[] { "素直", "積極性", "優しさ", "面倒見", "寛容", "度胸", "アホ", "性欲", "筋力", "おしゃれ" };
            //EditorGUILayout.LabelField(ped.vs.Length.ToString());
            //Debug.Log(ped.vs.Length);
            for (int i = 0; i < ped.vs.Length; i++)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField(vs[i], GUILayout.Width(60));
                    ped.vs[i] = EditorGUILayout.TextField(ped.vs[i] /*, GUILayout.Width(120)*/);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        //EditorGUILayout.EndVertical();
        
        if (foldIns = EditorGUILayout.Foldout(foldIns, "baseInspector"))
        {
            base.OnInspectorGUI();
        }

    }
}

#endif