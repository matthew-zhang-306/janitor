using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;


[CustomEditor(typeof(StatUpgradeInteractable), true)]
public class StatUpgradeEditor : Editor
{

    StatUpgradeInteractable sui;
    SerializedObject so;
    
    private SerializedProperty ulist;

    private void OnEnable() {

        if (target == null) return;

        sui = target as StatUpgradeInteractable;

        so = new SerializedObject(sui);
        ulist = so.FindProperty("ulist");

        //reorder here
        
    }

    public override void OnInspectorGUI()
    {
        so.Update();
        EditorGUILayout.LabelField("Add a prefab of an object that can be upgraded (e.g. player) ");
        EditorGUILayout.PropertyField(so.FindProperty("context"));
        EditorGUILayout.Space(20);

        Upgradeable u = sui.context;
        var hs = u?.UpgradeableParameters();

        GUILayout.BeginHorizontal();
        int idx = 0;
        if (hs != null) {
            foreach (var key in hs) {
                if (idx++ % 3 == 0) {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                if (GUILayout.Button (key)) {
                    ulist.InsertArrayElementAtIndex (ulist.arraySize);
                    ulist.GetArrayElementAtIndex(ulist.arraySize - 1).FindPropertyRelative("parameter").stringValue = key;
                    ulist.GetArrayElementAtIndex(ulist.arraySize - 1).FindPropertyRelative("value").floatValue = 1f;
                    // if (toggleNorm) Normalize();
                }

            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(20);
        for (int i = 0; i < ulist.arraySize; i++)
        {
            var elem = ulist.GetArrayElementAtIndex(i);
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(elem.FindPropertyRelative("parameter").stringValue);
            
            elem.FindPropertyRelative("value").floatValue = 
                EditorGUILayout.FloatField (elem.FindPropertyRelative("value").floatValue);
            
            if (GUILayout.Button ("x", GUILayout.Width(16), GUILayout.Height(16))) {
                ulist.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }



        EditorGUILayout.LabelField("Tool tip plz");
        EditorGUILayout.PropertyField(so.FindProperty("_tooltip"));
        EditorGUILayout.Space(20);

        so.ApplyModifiedProperties();

    }

}