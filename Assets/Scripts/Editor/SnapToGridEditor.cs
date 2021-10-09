using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnapToGrid))]
public class SnapToGridEditor : Editor
{
    SnapToGrid snapToGrid;

    private void OnEnable() {
        snapToGrid = target as SnapToGrid;
    }

    private void OnSceneGUI() {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C) {
            snapToGrid.SnapCollider();
            EditorUtility.SetDirty(snapToGrid.gameObject);
        }
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V) {
            snapToGrid.SnapPosition();
            EditorUtility.SetDirty(snapToGrid.gameObject);
        }
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.O) {
            snapToGrid.SnapPositionAndCollider();
            EditorUtility.SetDirty(snapToGrid.gameObject);
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUILayout.Space(20);
        GUILayout.Label("Keyboard shortcut: C");
        if (GUILayout.Button("Snap Collider Only")) {
            snapToGrid.SnapCollider();
            EditorUtility.SetDirty(snapToGrid.gameObject);
        }
        GUILayout.Label("Keyboard Shortcut: V");
        if (GUILayout.Button("Snap Position Only")) {
            snapToGrid.SnapPosition();
            EditorUtility.SetDirty(snapToGrid.gameObject);
        }
        GUILayout.Label("Keyboard Shortcut: O");
        if (GUILayout.Button("Snap Position and Collider")) {
            snapToGrid.SnapPositionAndCollider();
            EditorUtility.SetDirty(snapToGrid.gameObject);
        }

        GUILayout.Space(20);
        GUILayout.Label("These button will run the snap functions");
        GUILayout.Label("on ALL SnapToGrid objects in the scene.");
        GUILayout.Label("This cannot be undone.");
        if (GUILayout.Button("Snap All Colliders")) {
            foreach (var snap in GameObject.FindObjectsOfType<SnapToGrid>()) {
                snap.SnapCollider();
                EditorUtility.SetDirty(snap.gameObject);
            }
        }
        if (GUILayout.Button("Snap All Positions")) {
            foreach (var snap in GameObject.FindObjectsOfType<SnapToGrid>()) {
                snap.SnapPosition();
                EditorUtility.SetDirty(snap.gameObject);
            }
        }
        if (GUILayout.Button("Snap All Positions and Colliders")) {
            foreach (var snap in GameObject.FindObjectsOfType<SnapToGrid>()) {
                snap.SnapPositionAndCollider();
                EditorUtility.SetDirty(snap.gameObject);
            }
        }
        
    }
}
