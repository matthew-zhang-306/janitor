using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

using WaveArray = RoomWaveAddon.WaveArray;
using Wave = RoomWaveAddon.Wave;
using WaveSpawn = RoomWaveAddon.WaveSpawn;

using EnemyType = EnemyTypesSO.EnemyType;

[CustomEditor(typeof(RoomWaveAddon))]
public class RoomWaveEditor : Editor
{
    // references
    RoomWaveAddon waveAddon;
    SerializedObject so;
    SerializedProperty propWaves;
    Dictionary<string, EnemyType> enemyTypes;
    Tilemap tm;

    // fields
    int waveNum;
    TextAsset jsonFile;
    string jsonFileName;

    private void OnEnable() {
        waveAddon = target as RoomWaveAddon;

        so = new SerializedObject(waveAddon);
        propWaves = so.FindProperty("waves");

        tm = waveAddon.roomManager?.dirtyTiles?.GetComponent<Tilemap>();
        enemyTypes = waveAddon.enemyTypesSO?.GetEnemyTypes();
    }

    private void OnSceneGUI() {
        if (propWaves.arraySize == 0 ||
            enemyTypes == null || tm == null)
            return;

        so.Update();

        // check for left/right inputs
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.V) {
            waveNum += 1;
        }
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.C) {
            waveNum -= 1;
        }
        waveNum = Mathf.Clamp(waveNum, 0, propWaves.arraySize - 1);

        // draw currently selected wave
        var propWave = propWaves.GetArrayElementAtIndex(waveNum);
        GUI.color = Color.white;
        Handles.Label(waveAddon.transform.position, "Wave " + (waveNum + 1));
        Debug.Log(propWave.FindPropertyRelative("thresh").floatValue);

        var propSpawns = propWave.FindPropertyRelative("spawns");
        for (var i = 0; i < propSpawns.arraySize; i++) {
            var propSpawn = propSpawns.GetArrayElementAtIndex(i);
            var propXCoord = propSpawn.FindPropertyRelative("xcoord");
            var propYCoord = propSpawn.FindPropertyRelative("ycoord");
            var propDelay = propSpawn.FindPropertyRelative("delay");
            var propEnemy = propSpawn.FindPropertyRelative("enemy");

            EnemyType enemy = new EnemyType("???", Color.white, null);
            if (enemyTypes.ContainsKey(propEnemy.stringValue)) {
                enemy = enemyTypes[propEnemy.stringValue];
            }

            Vector3 position = WaveSpawn.XYToPosition(propXCoord.floatValue, propYCoord.floatValue, tm);

            string label = enemy.name + "";
            if (propDelay.floatValue > 0) {
                label += " (" + propDelay.floatValue + ")";
            }

            Handles.color = enemy.color;
            GUI.color = enemy.color;
            Handles.DrawWireDisc(position, Vector3.forward, 1f);
            Handles.Label(position, label);
            
            // TODO: i do not know why the position handles snap to the grid even though these methods are interpolating values between adjacent tiles
            // but it feels good to use so i'll only fix this if we need to
            var newPosition = Handles.PositionHandle(position, Quaternion.identity);
            propXCoord.floatValue = WaveSpawn.PositionToX(newPosition, tm);
            propYCoord.floatValue = WaveSpawn.PositionToY(newPosition, tm);
        }
        
        so.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        so.Update();

        if (propWaves.arraySize > 1) {
            EditorGUILayout.LabelField("(You can also change the wave number with C and V)");
            waveNum = EditorGUILayout.IntSlider("Wave #: ", waveNum + 1, 1, propWaves.arraySize) - 1;
        }
        EditorGUILayout.Space(20);

        jsonFile = EditorGUILayout.ObjectField("Json: ", jsonFile, typeof(TextAsset), false) as TextAsset;
        if (GUILayout.Button("Load waves from JSON file")) {
            LoadJSON();
        }
        jsonFileName = EditorGUILayout.TextField("File name: ", jsonFileName);
        if (GUILayout.Button("Save waves to JSON file")) {
            SaveJSON();
        }
        EditorGUILayout.Space(20);
    
        base.OnInspectorGUI();
        so.ApplyModifiedProperties();
    }

    private void LoadJSON() {
        if (jsonFile == null) {
            Debug.LogError("please drag a JSON file into the inspector to load");
            return;
        }

        WaveArray waveArray = JsonUtility.FromJson<WaveArray>(jsonFile.text);
        if (waveArray.waves == null || waveArray.waves.Any(wave => wave.spawns == null)) {
            Debug.LogError("this JSON file doesn't look quite right");
            return;
        }

        propWaves.ClearArray();
        for (int w = 0; w < waveArray.waves.Length; w++) {
            propWaves.InsertArrayElementAtIndex(w);
            var propWave = propWaves.GetArrayElementAtIndex(w);
            propWave.FindPropertyRelative("thresh").floatValue = waveArray.waves[w].thresh;

            var propSpawns = propWave.FindPropertyRelative("spawns");
            propSpawns.ClearArray();
            for (int s = 0; s < waveArray.waves[w].spawns.Length; s++) {
                propSpawns.InsertArrayElementAtIndex(s);
                var propSpawn = propSpawns.GetArrayElementAtIndex(s);
                propSpawn.FindPropertyRelative("xcoord").floatValue = waveArray.waves[w].spawns[s].xcoord;
                propSpawn.FindPropertyRelative("ycoord").floatValue = waveArray.waves[w].spawns[s].ycoord;
                propSpawn.FindPropertyRelative("delay").floatValue = waveArray.waves[w].spawns[s].delay;
                propSpawn.FindPropertyRelative("enemy").stringValue = waveArray.waves[w].spawns[s].enemy;
            }
        }
    }

    private void SaveJSON() {
        if (jsonFileName == null || jsonFileName.Length == 0) {
            Debug.LogError("please enter a file name for the new JSON file that you want to save");
            return;
        }

        WaveArray waveArray = new WaveArray(new Wave[propWaves.arraySize]);
        for (int w = 0; w < propWaves.arraySize; w++) {
            Wave wave = new Wave();
            var propWave = propWaves.GetArrayElementAtIndex(w);
            wave.thresh = propWave.FindPropertyRelative("thresh").floatValue;

            var propSpawns = propWave.FindPropertyRelative("spawns");
            wave.spawns = new WaveSpawn[propSpawns.arraySize];
            for (int s = 0; s < propSpawns.arraySize; s++) {
                var propSpawn = propSpawns.GetArrayElementAtIndex(s);
                WaveSpawn spawn = new WaveSpawn();
                spawn.xcoord = propSpawn.FindPropertyRelative("xcoord").floatValue;
                spawn.ycoord = propSpawn.FindPropertyRelative("ycoord").floatValue;
                spawn.delay = propSpawn.FindPropertyRelative("delay").floatValue;
                spawn.enemy = propSpawn.FindPropertyRelative("enemy").stringValue;
                wave.spawns[s] = spawn;
            }
            waveArray.waves[w] = wave;
        }

        string jsonText = JsonUtility.ToJson(waveArray);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/" + jsonFileName + ".json", jsonText);
    }
}
