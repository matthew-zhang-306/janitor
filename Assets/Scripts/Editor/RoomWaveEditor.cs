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

    // fields
    int waveNum;
    int enemyNum;
    TextAsset jsonFile;
    string jsonFileName;

    private void OnEnable() {

        if (target == null) return;

        waveAddon = target as RoomWaveAddon;

        so = new SerializedObject(waveAddon);
        propWaves = so.FindProperty("waves");

        waveNum = 0;
        enemyNum = 0;
    }

    private void OnSceneGUI() {
        var tm = waveAddon.roomManager?.dirtyTiles?.GetComponent<Tilemap>();
        var enemyTypes = waveAddon.enemyTypesSO?.GetEnemyTypes();

        if (propWaves.arraySize == 0 ||
            enemyTypes == null || tm == null)
            return;

        Event e = Event.current;
        so.Update();

        // wave selection
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.RightArrow) {
            waveNum += 1;
            enemyNum = 0;
        }
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftArrow) {
            waveNum -= 1;
            enemyNum = 0;
        }
        waveNum = Mathf.Clamp(waveNum, 0, Mathf.Max(propWaves.arraySize - 1, 0));

        // fetch serializedproperties
        var propWave = propWaves.GetArrayElementAtIndex(waveNum);
        var propSpawns = propWave.FindPropertyRelative("spawns");

        // enemy selection
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.DownArrow) {
            enemyNum += 1;
        }
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.UpArrow) {
            enemyNum -= 1;
        }
        enemyNum = Mathf.Clamp(enemyNum, 0, Mathf.Max(propSpawns.arraySize - 1, 0));

        // draw currently selected wave
        GUI.color = Color.white;
        Handles.Label(waveAddon.transform.position, "Wave " + (waveNum + 1));
        Debug.Log(propWave.FindPropertyRelative("thresh").floatValue);

        // draw wave spawns        
        for (var s = 0; s < propSpawns.arraySize; s++) {
            var propSpawn = propSpawns.GetArrayElementAtIndex(s);
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
            Handles.Label(position, label);
            
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            Handles.CircleHandleCap(controlId, position, Quaternion.identity, 0.5f, EventType.Repaint);
            if (enemyNum == s) {
                // TODO: i do not know why the position handles snap to the grid even though these methods are interpolating values between adjacent tiles
                // but it feels good to use so i'll only fix this if we need to
                var newPosition = Handles.PositionHandle(position, Quaternion.identity);
                propXCoord.floatValue = WaveSpawn.PositionToX(newPosition, tm);
                propYCoord.floatValue = WaveSpawn.PositionToY(newPosition, tm);
            }
        }
        
        so.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        so.Update();

        // enemy types scriptable object
        EditorGUILayout.LabelField("Modify this object to define new enemy types: ");
        EditorGUILayout.PropertyField(so.FindProperty("enemyTypesSO"));
        EditorGUILayout.Space(20);

        // a high-level printout, to see at a glance how much is in the room
        EditorGUILayout.LabelField("OVERVIEW", EditorStyles.boldLabel);
        if (propWaves.arraySize > 0) {
            for (int w = 0; w < propWaves.arraySize; w++) {
                var propSpawns = propWaves.GetArrayElementAtIndex(w).FindPropertyRelative("spawns");
                EditorGUILayout.LabelField("Wave " + (w + 1) + ": " + propSpawns.arraySize + " enemies.");
            }
        }
        else {
            EditorGUILayout.LabelField("No waves.");
        }
        EditorGUILayout.Space(20);

        // editors
        EditorGUILayout.LabelField("EDIT WAVE", EditorStyles.boldLabel);
        if (GUILayout.Button("Add New Wave")) {
            // ADD WAVE
            if (propWaves.arraySize > 0)
                waveNum += 1;
            propWaves.InsertArrayElementAtIndex(waveNum);
        }
        if (propWaves.arraySize > 0) {
            // WAVE SELECTION
            EditorGUILayout.LabelField("(You can also change the wave number with LEFT and RIGHT)");
            waveNum = EditorGUILayout.IntSlider("Wave #: ", waveNum + 1, 1, propWaves.arraySize) - 1;
            EditorGUILayout.Space(20);
            
            // WAVE EDITOR
            var propWave = propWaves.GetArrayElementAtIndex(waveNum);
            EditorGUILayout.LabelField("Wave " + (waveNum + 1) + "/" + propWaves.arraySize + ":");
            EditorGUILayout.PropertyField(propWave.FindPropertyRelative("thresh"));
            if (GUILayout.Button("Delete Wave")) {
                // DELETE WAVE
                propWaves.DeleteArrayElementAtIndex(waveNum);
                if (waveNum > 0 && waveNum == propWaves.arraySize)
                    waveNum -= 1;
            }
            EditorGUILayout.Space(20);

            var propSpawns = propWaves.GetArrayElementAtIndex(waveNum).FindPropertyRelative("spawns");
            EditorGUILayout.LabelField("EDIT ENEMY", EditorStyles.boldLabel);
            if (GUILayout.Button("Add New Enemy")) {
                // ADD ENEMY
                if (propSpawns.arraySize > 0)
                    enemyNum += 1;
                propSpawns.InsertArrayElementAtIndex(enemyNum);
            }
            if (propSpawns.arraySize > 0) {
                // ENEMY SELECTION
                EditorGUILayout.LabelField("(You can also change the enemy number with UP and DOWN)");
                enemyNum = EditorGUILayout.IntSlider("Enemy #: ", enemyNum + 1, 1, propSpawns.arraySize) - 1;
                EditorGUILayout.Space(20);

                // ENEMY EDITOR
                var propSpawn = propSpawns.GetArrayElementAtIndex(enemyNum);
                EditorGUILayout.LabelField("Enemy " + (enemyNum + 1) + "/" + propSpawns.arraySize + ":");
                EditorGUILayout.PropertyField(propSpawn.FindPropertyRelative("enemy"));
                EditorGUILayout.PropertyField(propSpawn.FindPropertyRelative("delay"));
                EditorGUILayout.PropertyField(propSpawn.FindPropertyRelative("xcoord"));
                EditorGUILayout.PropertyField(propSpawn.FindPropertyRelative("ycoord"));
                if (GUILayout.Button("Remove Enemy")) {
                    // REMOVE ENEMY
                    propSpawns.DeleteArrayElementAtIndex(enemyNum);
                    if (enemyNum > 0 && enemyNum == propSpawns.arraySize)
                        enemyNum -= 1;
                }
                EditorGUILayout.Space(20);
            }
            else {
                EditorGUILayout.LabelField("No enemies to select.");
                EditorGUILayout.Space(20);
            }
        }
        else {
            EditorGUILayout.LabelField("No waves to select.");
            EditorGUILayout.Space(20);
        }

        // load/save json file
        EditorGUILayout.LabelField("JSON", EditorStyles.boldLabel);
        jsonFile = EditorGUILayout.ObjectField("Json: ", jsonFile, typeof(TextAsset), false) as TextAsset;
        if (GUILayout.Button("Load waves from JSON file")) {
            LoadJSON();
        }
        jsonFileName = EditorGUILayout.TextField("File name: ", jsonFileName);
        if (GUILayout.Button("Save waves to JSON file")) {
            SaveJSON();
        }
        EditorGUILayout.Space(20);

        EditorGUILayout.PropertyField(so.FindProperty("spawnMarkerPrefab"));
    
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
