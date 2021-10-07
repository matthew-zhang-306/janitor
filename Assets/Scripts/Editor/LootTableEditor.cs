using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;


[CustomEditor(typeof(BaseLootTable), true)]
public class LootTableEditor : Editor
{
    BaseLootTable lt;
    SerializedObject so;
    SerializedProperty ltList;
    ItemTypesSO itypes;
    List <string> items;
    List <bool> itemUsed;
    List <(int, int)> pairings;
    bool toggleNorm = false;

    private void OnEnable() {

        if (target == null) return;

        lt = target as BaseLootTable;

        so = new SerializedObject(lt);
        ltList = so.FindProperty("lootTable");
        var isg = (GameObject) Resources.Load("InteractSpawn");
        itypes = isg.GetComponent<InteractableSpawner>().itemTypes;
        items = itypes.GetItemDict().Keys.ToList();
        //reorder here
        
    }

    public override void OnInspectorGUI()
    {
        so.Update();

        EditorGUILayout.LabelField("Available Items", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        int idx = 0;
        foreach (var i in items) {
            if (idx++ % 3 == 0) {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
            if (GUILayout.Button (i)) {
                ltList.InsertArrayElementAtIndex (ltList.arraySize);
                ltList.GetArrayElementAtIndex(ltList.arraySize - 1).FindPropertyRelative("name").stringValue = i;
                ltList.GetArrayElementAtIndex(ltList.arraySize - 1).FindPropertyRelative("probability").floatValue = 1f;
                // if (toggleNorm) Normalize();
            }

        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(20);

        toggleNorm = GUILayout.Toggle(toggleNorm, "Auto Normalize Probabilities");

        if (toggleNorm) Normalize();

        


        for (int i = 0; i < ltList.arraySize; i++)
        {
            var elem = ltList.GetArrayElementAtIndex(i);
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(elem.FindPropertyRelative("name").stringValue);
            
            if (!toggleNorm) {
                elem.FindPropertyRelative("probability").floatValue = 
                    EditorGUILayout.FloatField (elem.FindPropertyRelative("probability").floatValue);

            }
            else {
                elem.FindPropertyRelative("probability").floatValue = 
                    EditorGUILayout.Slider (elem.FindPropertyRelative("probability").floatValue, 0f, 1f);
            }
            if (GUILayout.Button ("x", GUILayout.Width(16), GUILayout.Height(16))) {
                ltList.DeleteArrayElementAtIndex(i);
            }
            // EditorGUILayout.Slider(new Rect(5,5,150,150), elem.FindPropertyRelative("probability").floatValue,
            //                     (float) 1e-6,1f, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
        }
        // base.OnInspectorGUI();
        so.ApplyModifiedProperties();
    }

    private void Normalize()
    {
        float total = 0;
        for (int i = 0; i < ltList.arraySize; i++)
        {
            var elem = ltList.GetArrayElementAtIndex(i);
            total += elem.FindPropertyRelative("probability").floatValue;
        }
        if (total == 0) total = 1;
        for (int i = 0; i < ltList.arraySize; i++)
        {
            var elem = ltList.GetArrayElementAtIndex(i);
            elem.FindPropertyRelative("probability").floatValue /= total;
        }
    }
}