using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

using Waves = RoomWaveAddon.Waves;
using EnemyType = EnemyTypesSO.EnemyType;

[CustomEditor(typeof(RoomWaveAddon))]
public class RoomWaveEditor : Editor
{
    RoomWaveAddon waveAddon;
    Waves waves;
    Dictionary<string, EnemyType> enemyTypes;
    Tilemap tm;

    int waveNum;

    private void OnEnable() {
        waveAddon = target as RoomWaveAddon;

        tm = waveAddon.roomManager?.dirtyTiles?.GetComponent<Tilemap>();
        enemyTypes = waveAddon.enemyTypesSO?.GetEnemyTypes();

        if (waveAddon.jsonFile != null) {
            waves = JsonUtility.FromJson<Waves>(waveAddon.jsonFile.text);
        }
    }

    private void OnSceneGUI() {
        if (waves == null || waves.waves == null || waves.waves.Length == 0 ||
            enemyTypes == null || tm == null)
            return;

        // check for left/right inputs
        Event e = Event.current;
        Debug.Log(e.isKey);
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.V) {
            waveNum += 1;
        }
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.C) {
            waveNum -= 1;
        }
        waveNum = Mathf.Clamp(waveNum, 0, waves.waves.Length - 1);

        var wave = waves.waves[waveNum];
        if (wave == null)
            return;

        Handles.Label(waveAddon.transform.position, "Wave " + (waveNum + 1));

        foreach (var waveSpawn in wave.spawns) {
            EnemyType enemy = new EnemyType("???", Color.white, null);
            if (waveSpawn.enemy != null && enemyTypes.ContainsKey(waveSpawn.enemy)) {
                enemy = enemyTypes[waveSpawn.enemy];
            }

            Vector3 position = waveSpawn.GetPosition(tm);

            string label = enemy.name + "";
            if (waveSpawn.delay > 0) {
                label += " (" + waveSpawn.delay + ")";
            }

            Handles.color = enemy.color;
            GUI.color = enemy.color;
            Handles.DrawWireDisc(position, Vector3.forward, 1f);
            Handles.Label(position, label);
            waveSpawn.SetPosition(Handles.PositionHandle(position, Quaternion.identity), tm);
        }
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Move this slider to change the displayed wave number.");
        EditorGUILayout.LabelField("You can also click on the room (the green dot in the scene)");
        EditorGUILayout.LabelField("and press C and V to decrease and increase the wave number.");
        waveNum = EditorGUILayout.IntSlider(waveNum, 0, waves.waves?.Length - 1 ?? 0);
    }
}
