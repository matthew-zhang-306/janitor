using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Priority_Queue;

public class Pathfinding : MonoBehaviour
{
    public Tile pathTile;
    public Transform player;
    public Tilemap dirtyMap;
    public Camera cam;

    private Tilemap pathMap;

    private void Start() {
        pathMap = GetComponent<Tilemap>();
        InvokeRepeating("FindPath", 0f, 0.2f);
    }


    private void FindPath() {
        pathMap.ClearAllTiles();

        Vector2 startPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 destPos = player.position;

        Vector3Int startCell = pathMap.WorldToCell(startPos);
        Vector3Int destCell = pathMap.WorldToCell(destPos);

        if (dirtyMap.GetTile(startCell) == null || dirtyMap.GetTile(destCell) == null) {
            return;
        }

        var visitedNodes = new Dictionary<Vector3Int, float>();
        var search = new SimplePriorityQueue<Vector3Int, float>();
        search.Enqueue(startCell, 0);

        while (search.Count > 0) {
            float cost = search.GetPriority(search.First);
            Vector3Int node = search.Dequeue();
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

                    Vector3Int nextNode = node + new Vector3Int(dx, dy, 0);
                    if (dirtyMap.GetTile(nextNode) != null) {
                        search.Enqueue(nextNode, cost + nextCost);
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
