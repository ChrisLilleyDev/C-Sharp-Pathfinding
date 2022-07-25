using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState { PREMOVE, STARTED, FINISHED }

public class MoveManager : MonoBehaviour {
    public StateSystem stateSystem;
    public MoveState moveState;

    private Mesh mesh;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    private Unit selectedUnit;

    void Start() {
        moveState = MoveState.PREMOVE;
    }

    void Update() {
        selectedUnit = stateSystem.unitManager.activeUnit;
        if(stateSystem.turnState == TurnState.MOVE) {
            switch(moveState) {
                case MoveState.PREMOVE: {
                    if(Input.GetMouseButtonDown(1)) {
                        Vector3 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if(stateSystem.gridManager.GetTile(click) != null) {
                            List<Tile> path = MakePath(stateSystem.gridManager.GetTile(click));
                            if(path != null) {
                                ClearMesh();
                                selectedUnit.path = path;
                                selectedUnit.moving = true;
                                moveState = MoveState.STARTED;
                            }
                        }
                    }
                    break;
                }
                case MoveState.STARTED: {
                    if(!selectedUnit.moving) {
                        //do checks here
                        if(selectedUnit.path.Count > 1 || selectedUnit.tile != selectedUnit.path[0]) {
                            selectedUnit.moving = true;
                        } else {
                            moveState = MoveState.FINISHED;
                        }
                    }
                    break;
                }
                case MoveState.FINISHED: {
                    //do checks here too
                    stateSystem.turnState = TurnState.CHOICE;
                    break;
                }
                default: break;
            }
        }
    }
    //A* starts here
    private List<Tile> MakePath(Tile destination) {
        PathNode[,] grid = new PathNode[destination.grid.width, destination.grid.height];

        for(int x = 0; x < destination.grid.width; x++) {
            for(int y = 0; y < destination.grid.height; y++) {
                grid[x, y] = new PathNode(destination.grid.gridArray[x, y]);
            }
        }

        Tile startTile = selectedUnit.tile;
        PathNode startNode = grid[startTile.xPos, startTile.yPos];
        PathNode endNode = grid[destination.xPos, destination.yPos];
        if(!endNode.walkable) return null;

        List<PathNode> openList = new List<PathNode> { startNode };
        HashSet<PathNode> closedList = new HashSet<PathNode>();

        startNode.g = 0;
        startNode.h = Distance(startNode, endNode);
        startNode.f = startNode.h;

        while(openList.Count > 0) {
            PathNode currentNode = openList[0];
            for(int i = 1; i < openList.Count; i++) {
                if(openList[i].f < currentNode.f || openList[i].f == currentNode.f && openList[i].h < currentNode.h) {
                    currentNode = openList[i];
                }
            }

            if(currentNode == endNode) {
                return Path(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbour in Neighbours(grid, currentNode)) {
                if(!neighbour.walkable || closedList.Contains(neighbour)) continue;

                int testG = currentNode.g + Distance(currentNode, neighbour);
                if(testG < neighbour.g && testG <= 10 * selectedUnit.moveRange) {
                    neighbour.parent = currentNode;
                    neighbour.g = testG;
                    neighbour.h = Distance(neighbour, endNode);
                    neighbour.f = neighbour.g + neighbour.h;
                    if(!openList.Contains(neighbour)) openList.Add(neighbour);
                }
            }
        }

        return null;
    }
    //neighbour checking method
    private List<PathNode> Neighbours(PathNode[,] grid, PathNode p) {
        List<PathNode> neighbourList = new List<PathNode>();

        List<int> x = new List<int> { -1, 0, 1 };
        List<int> y = new List<int> { -1, 0, 1 };

        if(p.x - 1 < 0 || !grid[p.x - 1, p.y].walkable) x.Remove(-1);
        if(p.x + 1 >= grid.GetLength(0) || !grid[p.x + 1, p.y].walkable) x.Remove(1);
        if(p.y - 1 < 0 || !grid[p.x, p.y - 1].walkable) y.Remove(-1);
        if(p.y + 1 >= grid.GetLength(1) || !grid[p.x, p.y + 1].walkable) y.Remove(1);

        foreach(int i in x) {
            foreach(int j in y) {
                if(i == 0 && j == 0) continue;
                neighbourList.Add(grid[p.x + i, p.y + j]);
            }
        }
        return neighbourList;
    }

    private List<Tile> Path(PathNode endNode) {
        List<Tile> path = new List<Tile> { endNode.tile };
        PathNode currentNode = endNode;
        while(currentNode.parent.parent != null) {
            path.Add(currentNode.parent.tile);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private int Distance(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        return 14 * Mathf.Min(xDistance, yDistance) + 10 * Mathf.Abs(xDistance - yDistance);
    }

    public void UpdateUI() {
        selectedUnit = stateSystem.unitManager.activeUnit;
        int mR = selectedUnit.moveRange;
        int xOrigin = selectedUnit.tile.xPos;
        int yOrigin = selectedUnit.tile.yPos;
        Tile forward, right, left, down;
        List<Tile> tiles = new List<Tile>(4 * mR * mR);

        for(int x = 0; x < mR; x++) {
            for(int y = 1; y <= mR; y++) {
                forward = stateSystem.gridManager.GetTile(xOrigin + x, yOrigin + y);
                if(forward != null && MakePath(forward) != null) tiles.Add(forward);
                right = stateSystem.gridManager.GetTile(xOrigin + y, yOrigin - x);
                if(right != null && MakePath(right) != null) tiles.Add(right);
                left = stateSystem.gridManager.GetTile(xOrigin - y, yOrigin + x);
                if(left != null && MakePath(left) != null) tiles.Add(left);
                down = stateSystem.gridManager.GetTile(xOrigin - x, yOrigin - y);
                if(down != null && MakePath(down) != null) tiles.Add(down);
            }
        }
        tiles.TrimExcess();

        vertices = new Vector3[4 * tiles.Count];
        uv = new Vector2[4 * tiles.Count];
        triangles = new int[6 * tiles.Count];

        int index = 0;
        foreach(Tile tile in tiles) {
            int indexV = 4 * index;
            int indexT = 6 * index;

            vertices[indexV] = tile.position;
            vertices[indexV + 1] = tile.position + new Vector3(0, tile.grid.cellSize);
            vertices[indexV + 2] = tile.position + new Vector3(tile.grid.cellSize, tile.grid.cellSize);
            vertices[indexV + 3] = tile.position + new Vector3(tile.grid.cellSize, 0);

            uv[indexV] = new Vector2(0, 0);
            uv[indexV + 1] = new Vector2(0, 1);
            uv[indexV + 2] = new Vector2(1, 1);
            uv[indexV + 3] = new Vector2(1, 0);

            triangles[indexT] = indexV;
            triangles[indexT + 1] = indexV + 1;
            triangles[indexT + 2] = indexV + 2;
            triangles[indexT + 3] = indexV;
            triangles[indexT + 4] = indexV + 2;
            triangles[indexT + 5] = indexV + 3;

            index++;
        }

        mesh = new Mesh {
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void ClearMesh() {
        mesh = new Mesh {
            vertices = new Vector3[0],
            uv = new Vector2[0],
            triangles = new int[0]
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private class PathNode {
        public readonly Tile tile;
        public readonly int x, y;
        public readonly bool walkable;
        public int h;
        public int g = int.MaxValue;
        public int f = int.MaxValue;
        public PathNode parent;

        public PathNode(Tile tile) {
            this.tile = tile;
            x = tile.xPos;
            y = tile.yPos;
            walkable = tile.walkable;
            if(tile.occupant != null) walkable = false;
        }
    }
}
