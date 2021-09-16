using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Priority_Queue;
public class Pathfinding : MonoBehaviour
{
    public class NodeComp : IComparer<(int, float)>
    {
        // Compares by Height, Length, and Width.
        public int Compare((int, float) x, (int, float) y)
        {
            if (x.Item1 == y.Item1) {
                return 0;
            }

            return x.Item2 <= y.Item2 ? -1 : 1;
        }
    }
    public Tile pathTile;
    public Tile pathTile2;
    public Transform player;
    public Tilemap dirtyMap;
    public Camera cam;

    private Tilemap pathMap;

    private void Start() {
        pathMap = GetComponent<Tilemap>();
        InvokeRepeating("FindPath", 3f, 1000f);
    }


    private void FindPath() {
        Debug.Log("Hi there");
        pathMap.ClearAllTiles();

        Vector2 startPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 destPos = player.position;

        Vector3Int startCell = pathMap.WorldToCell(startPos);
        Vector3Int destCell = pathMap.WorldToCell(destPos);

        if (dirtyMap.GetTile(startCell) == null || dirtyMap.GetTile(destCell) == null) {
            return;
        }

        var visitedNodes = new Dictionary<Vector3Int, float>();
        var search = new SortedDictionary<(int,float), Vector3Int>(new NodeComp());
        int count = 0;
        search.Add((count, 0), startCell);
        
        while (search.Count > 0) {
            (int id, float cost) = search.Keys.First();
            var node = search[(id,cost)];
            Debug.Log(cost);
            search.Remove(search.Keys.First()); 
            pathMap.SetTile(node, pathTile2);

            if (visitedNodes.ContainsKey(node)) {
                continue;
            }

            visitedNodes.Add(node, cost);

            if (node == destCell) {
                // path has been found
                TracePath(visitedNodes, startCell, destCell);
                return;
            }

            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0)
                        continue;
                    
                    float nextCost = 1;
                    if (Mathf.Abs(dx) == Mathf.Abs(dy))
                        nextCost = 1.4f;

                    // Vector3Int nextNode = node + new Vector3Int(dx, dy, 0);
                    var nextNode = new Vector3Int (node.x + dx, node.y + dy, 0);
                    if (dirtyMap.GetTile(nextNode) != null) {
                        count += 1;
                        search.Add((count,cost + nextCost), nextNode);
                    }
                }
            }
        }
    }


    private void TracePath(Dictionary<Vector3Int, float> nodeCosts, Vector3Int start, Vector3Int dest) {
        Debug.Log("has path");

        Vector3Int current = dest;
        while (current != start) {
            pathMap.SetTile(current, pathTile);

            Vector3Int bestNext = current;
            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (Mathf.Abs(dx) == Mathf.Abs(dy))
                        continue;

                    Vector3Int next = current + new Vector3Int(dx, dy, 0);
                    if (nodeCosts.ContainsKey(next) && nodeCosts[next] < nodeCosts[bestNext]) {
                        bestNext = next;
                    }
                }
            }

            if (bestNext == current) {
                Debug.LogError("nope nope nope");
                return;
            }

            current = bestNext;
        }

        pathMap.SetTile(start, pathTile);
    }

}
