using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;


[CustomEditor(typeof(FloorTilePopulator), true)]
public class FloorTileEditor : Editor
{

    FloorTilePopulator ftp;
    SerializedObject so;
    
    private void OnEnable() {

        if (target == null) return;

        ftp = target as FloorTilePopulator;

        so = new SerializedObject(ftp);
        
        //reorder here
        
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button ("Generate Floor Tile")) {
            ftp.Load();
        }
        if (GUILayout.Button ("Clear Floor Tile")) {
            ftp.Clear();
        }
        base.OnInspectorGUI();

    }

}