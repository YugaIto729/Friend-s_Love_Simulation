using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName = "Scriptable/Create DataBase_Image")]
public sealed class Image_Data : ScriptableObject
{
    [SerializeField]
    Sprite[] images;

    public List<Sprite> Get_Images()
    {
        return new List<Sprite>(images);
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(Image_Data), true)]
public sealed class Editor_Image_Data : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Begin");

        DrawDefaultInspector();

        EditorGUILayout.LabelField("End");
    }
}

#endif
