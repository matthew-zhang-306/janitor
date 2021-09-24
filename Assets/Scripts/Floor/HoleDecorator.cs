using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class HoleDecorator : MonoBehaviour 
{
    //should be placed next to dirty floor (as it is initalized then)
    public List<Tile> edge;
    public List<Tile> shadow;

    private Tilemap tm;

    private HashSet<Vector3Int> holeSet;
    void Start() 
    {
        holeSet = new HashSet<Vector3Int> ();
        tm = this.GetComponent<Tilemap>();
        if (edge.Count == 0) {
            Debug.LogWarning("No tiles found for hole decoration");
            Texture2D red = new Texture2D (1, 1, TextureFormat.RGBA32, 1, true);
            red.SetPixel(0,0, Color.red);
            red.Apply(true);

            var t = ScriptableObject.CreateInstance<Tile>();                
            t.sprite = Sprite.Create(red, new Rect(0,0,1,1), Vector2.one / 2f, 2);
            edge.Add (t);
            shadow.Add (t);
        }
    }

    public void AddHole (Vector3Int cell)
    {
        holeSet.Add (cell);
    }

    public HashSet<Vector3Int> Apply ()
    {
        var changed = new HashSet<Vector3Int>();
        foreach (var hole in holeSet) 
        {   
            for (int xd = -1; xd <= 1; xd++) {
                for (int yd = -1; yd <= 1; yd++) {
                    var nHole = new Vector3Int (hole.x + xd, hole.y + yd, 0);
                    if (!changed.Contains(nHole) && !holeSet.Contains(nHole)) {
                        Debug.Log ("Setting tile");
                        tm.SetTile (nHole, edge[Random.Range(0, edge.Count)]);
                        changed.Add (nHole);
                    }
                }
            }

            var up = new Vector3Int (hole.x, hole.y + 1, 0);
            if (changed.Contains(up)) 
            {
                tm.SetTile (hole, shadow[Random.Range(0, shadow.Count)]);
            }
        }
        return changed;
    }
}