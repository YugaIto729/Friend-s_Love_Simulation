using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(ValueEditor))]
public sealed class ValueEditor : EditorWindow
{
    private int intter = 0;
    private ValuesManager vm;
    private float[] dmy_values;
    private string[] dmy_strings;

    bool loadValue = false;
    Vector2 lscrollPos = Vector2.zero;
    Vector2 rscrollPos = Vector2.zero;
    SerializedProperty m_vm;
    int m_index;


    [MenuItem("Window/ValueChecker")]
    private static void Create()
    {
        GetWindow<ValueEditor>("ValueChecker");
    }

    private void OnEnable()
    {
        //Character chara = target as Character;
        //m_list = new ReorderableList()
        //m_vm = serializedObject.FindProperty("m_MyInt");
    }


    private void OnGUI()
    {
        //Debug.Log("[ValueEditor]");

        // 基本的には縦並び

        EditorGUILayout.LabelField("変数モニター"); // いつまでも居座り続けるぜ！

        if (EditorApplication.isPlaying) {

            var vm = ValuesManager.instance;
            if (vm != null)
            {

                //Debug.Log(loadValue);
                if (!loadValue)
                {
                    loadValue = true;
                    //System.Array.Copy(vm.Get_Values(), dmy_values, vm.Get_Values().Length);
                    CopyArray(vm.Get_Values(), ref dmy_values);
                    CopyArray(vm.Get_Texts(), ref dmy_strings);
                    //dmy_values = vm.Get_Values();
                    Debug.Log("dmy: " + dmy_values.Length);
                }

                if (dmy_values.Length > 0 && dmy_strings.Length > 0)
                {

                    var tabs = new[] { "VALUES", "TEXTS"};
                    m_index = GUILayout.Toolbar(m_index, tabs);

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        lscrollPos = EditorGUILayout.BeginScrollView(lscrollPos, GUI.skin.box);
                        {
                            //===================================================================================

                            if (m_index == 0)
                            {
                                // ここの範囲は横並び
                                EditorGUILayout.BeginVertical();
                                {
                                    for (int i = 0; i < dmy_values.Length; i++)
                                    {
                                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                                        {
                                            //EditorGUILayout.LabelField("rrrr");

                                            EditorGUILayout.LabelField(string.Format("value[{0}] -> {1}", i, vm.Get_Value_Float(i)), GUILayout.Width(100));
                                            //EditorGUILayout.LabelField(string.Format("value[{0}] ->", i), GUILayout.Width(85));

                                            dmy_values[i] = EditorGUILayout.FloatField(dmy_values[i], GUILayout.Width(40));

                                            if (GUILayout.Button("代入", GUILayout.Width(50)))
                                            {
                                                vm.Set_Value(i, dmy_values[i]);
                                                loadValue = false;
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            else if (m_index == 1)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    Debug.Log(": " + dmy_strings.Length);
                                    for (int i = 0; i < dmy_strings.Length; i++)
                                    {
                                        EditorGUILayout.BeginVertical(GUI.skin.box);
                                        {
                                            EditorGUILayout.LabelField(string.Format("Text[{0}] -> 「{1}」", i, vm.Get_Text(i)));

                                            EditorGUILayout.BeginHorizontal();
                                            {
                                                //EditorGUILayout.LabelField(string.Format("value[{0}] ->", i), GUILayout.Width(85));
                                                dmy_strings[i] = EditorGUILayout.TextField(dmy_strings[i], GUILayout.Width(100));

                                                if (GUILayout.Button("代入", GUILayout.Width(50)))
                                                {
                                                    vm.Set_Text(i, dmy_strings[i]);
                                                    loadValue = false;
                                                }
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }


                            //==================================================================================

                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndHorizontal();
                    /*
                    else if (m_index == 1)
                    {
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        {
                            lscrollPos = EditorGUILayout.BeginScrollView(rscrollPos, GUI.skin.box);
                            {

                                // ここの範囲は横並び
                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                {
                                    for (int i = 0; i < dmy_strings.Length; i++)
                                    {
                                        EditorGUILayout.LabelField(string.Format("Text[{0}] -> 「{1}」", i, vm.Get_Text(i)), GUILayout.Width(150));

                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            //EditorGUILayout.LabelField(string.Format("value[{0}] ->", i), GUILayout.Width(85));
                                            dmy_strings[i] = EditorGUILayout.TextField(dmy_strings[i], GUILayout.Width(40));

                                            if (GUILayout.Button("代入", GUILayout.Width(50)))
                                            {
                                                vm.Set_Text(i, dmy_strings[i]);
                                                loadValue = false;
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndScrollView();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    */
                }
            }
        }
        else
        {
            loadValue = false;
            EditorGUILayout.LabelField("実行中にしか使用できません");
        }
    }

    private void CopyArray(float[] source, ref float[] destnation)
    {
        destnation = new float[source.Length];

        for(int i=0; i < destnation.Length; i++)
        {
            destnation[i] = source[i];
        }
    }

    private void CopyArray(string[] source, ref string[] destnation)
    {
        destnation = new string[source.Length];

        for (int i = 0; i < destnation.Length; i++)
        {
            destnation[i] = source[i];
        }
    }

}



#endif

