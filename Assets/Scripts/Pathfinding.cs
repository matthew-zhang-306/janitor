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

    public float defaultUnitSize;

    private Tilemap pathMap;

    // a mapping of how close each cell position is to its nearest wall.
    // this will help produce pathfinding costs for units of different sizes.
    private Dictionary<Vector3Int, int> wallProximityMap;

    private List<Vector3> currentPath;

    private void Start() {
        pathMap = GetComponent<Tilemap>();
        InvokeRepeating("FindPath", 0.2f, 0.2f);
    }


    public void InitializePathfinding() {
        pathMap = GetComponent<Tilemap>();

        wallProximityMap = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> searchQueue = new Queue<Vector3Int>();

        // find all of the walls
        for (int x = dirtyMap.cellBounds.xMin - 1; x < dirtyMap.cellBounds.xMax + 1; x++) {
            for (int y = dirtyMap.cellBounds.yMin - 1; y < dirtyMap.cellBounds.yMax + 1; y++) {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (dirtyMap.GetTile(cell) == null) {
                    // this cell is a wall. we'll start searching from here
                    wallProximityMap[cell] = 0;
                    searchQueue.Enqueue(cell);
                }
            }
        }

        while (searchQueue.Count > 0) {
            Vector3Int cell = searchQueue.Dequeue();
            int proximity = wallProximityMap[cell];

            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0)
                        continue;
                    
                    Vector3Int neighborCell = new Vector3Int(cell.x + dx, cell.y + dy, 0);
                    if (!wallProximityMap.ContainsKey(neighborCell) && dirtyMap.GetTile(neighborCell) != null) {
                        // this is an empty cell next to a marked cell, so mark it
                        wallProximityMap[neighborCell] = proximity + 1;
                        searchQueue.Enqueue(neighborCell);
                    }
                }
            }
        }
    }

    private int GetWallProximity(Vector3Int cell) {
        if (wallProximityMap.ContainsKey(cell))
            return wallProximityMap[cell];
        return 0;
    }


    private void FixedUpdate() {
        if (currentPath != null) {
            Vector3 prevWaypoint = currentPath.First();
            foreach (Vector3 waypoint in currentPath) {
                Debug.DrawLine(prevWaypoint, waypoint, Color.white, Time.deltaTime);
                prevWaypoint = waypoint;
            }
        }
    }


    private void FindPath() {
        pathMap.ClearAllTiles();

        Vector2 startPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 destPos = player.position;

        Vector3Int startCell = pathMap.WorldToCell(startPos);
        Vector3Int destCell = pathMap.WorldToCell(destPos);

        int minimumProximity = Mathf.Max(Mathf.CeilToInt((defaultUnitSize - pathMap.cellSize.x) / 2f / pathMap.cellSize.x) + 1, 1);

        if (GetWallProximity(startCell) < minimumProximity || GetWallProximity(destCell) < minimumProximity) {
            return;
        }

        Debug.Log(wallProximityMap[startCell]);

        var visitedNodes = new Dictionary<Vector3Int, float>();
        var search = new SortedDictionary<(int,float), (Vector3Int,float)>(new NodeComp());
        int count = 0;
        search.Add((count, 0), (startCell, 0));
        
        while (search.Count > 0) {
            (int id, float totalCost) = search.Keys.First();
            (var node, float pathCost) = search[(id,totalCost)];
            search.Remove(search.Keys.First()); 
            pathMap.SetTile(node, pathTile2);

            if (visitedNodes.ContainsKey(node)) {
                continue;
            }

            visitedNodes.Add(node, pathCost);

            if (node == destCell) {
                // path has been found
                var rawPath = TracePath(visitedNodes, startCell, destCell);
                currentPath = SimplifyPath(rawPath);
                return;
            }

            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0)
                        continue;
                    
                    float nextCost = 1;
                    if (Mathf.Abs(dx) == Mathf.Abs(dy))
                        nextCost = 1.414f;

                    var nextNode = new Vector3Int (node.x + dx, node.y + dy, 0);
                    if (GetWallProximity(nextNode) >= minimumProximity) {
                        count += 1;
                        search.Add((count, pathCost + nextCost + GetHeuristic(nextNode, destCell)), (nextNode, pathCost + nextCost));
                    }
                }
            }
        }
    }

    private LinkedList<(Vector3Int, int)> TracePath(Dictionary<Vector3Int, float> nodeCosts, Vector3Int start, Vector3Int dest) {
        var path = new LinkedList<(Vector3Int, int)>();
        Vector3Int current = dest;

        int dir = -1;
        while (current != start) {
            path.AddFirst((current, dir));
            pathMap.SetTile(current, pathTile);

            Vector3Int bestNext = current;
            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0)
                        continue;

                    Vector3Int next = current + new Vector3Int(dx, dy, 0);
                    if (nodeCosts.ContainsKey(next) && nodeCosts[next] < nodeCosts[bestNext]) {
                        bestNext = next;
                        dir = (dx + 1) + (dy + 1) * 3;
                    }
                }
            }

            if (bestNext == current) {
                Debug.LogError("nope nope nope");
                return null;
            }

            current = bestNext;
        }

        path.AddFirst((start, dir));
        pathMap.SetTile(start, pathTile);
        return path;
    }

    private List<Vector3> SimplifyPath(LinkedList<(Vector3Int, int)> path) {
        List<Vector3> newPath = new List<Vector3>();

        var halfCellSize = pathMap.cellSize / 2f;
        int prevDir = -2;
        foreach ((Vector3Int node, int dir) in path) {
            if (dir != prevDir) {
                prevDir = dir;
                newPath.Add(pathMap.CellToWorld(node) + halfCellSize);
            }
        }

        return newPath;
    }


    private float GetHeuristic(Vector3Int node, Vector3Int dest) {
        // return Vector3Int.Distance(node, dest);
        return Mathf.Abs(dest.x - node.x) + Mathf.Abs(dest.y - node.y);
    }

}
