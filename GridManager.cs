using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    public Grid grid;
    private Mesh mesh;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    public event EventHandler<TileEventArgs> TileChanged;
    public class TileEventArgs : EventArgs {
        public Tile tile;
    }

    void Start() {
        grid = new Grid(16, 8, 1, new Vector3(-8, -4));
        mesh = new Mesh();
        vertices = new Vector3[4 * grid.width * grid.height];
        uv = new Vector2[4 * grid.width * grid.height];
        triangles = new int[6 * grid.width * grid.height];

        for(int x = 0; x < grid.width; x++) {
            for(int y = 0; y < grid.height; y++) {
                int index = x * grid.height + y;
                int indexV = 4 * index;
                int indexT = 6 * index;

                if(grid.gridArray[x, y].tileType != TileType.NONE) {
                    vertices[indexV] = grid.gridArray[x, y].position;
                    vertices[indexV + 1] = grid.gridArray[x, y].position + new Vector3(0, grid.cellSize);
                    vertices[indexV + 2] = grid.gridArray[x, y].position + new Vector3(grid.cellSize, grid.cellSize);
                    vertices[indexV + 3] = grid.gridArray[x, y].position + new Vector3(grid.cellSize, 0);
                }

                switch(grid.gridArray[x, y].tileType) {
                    case TileType.GROUND:
                        uv[indexV] = new Vector2(0, 0);
                        uv[indexV + 1] = new Vector2(0, 1);
                        uv[indexV + 2] = new Vector2(.4f, 1);
                        uv[indexV + 3] = new Vector2(.4f, 0);
                        break;
                    case TileType.WALL:
                        uv[indexV] = new Vector2(.6f, 0);
                        uv[indexV + 1] = new Vector2(.6f, 1);
                        uv[indexV + 2] = new Vector2(1, 1);
                        uv[indexV + 3] = new Vector2(1, 0);
                        break;
                    default: break;
                }

                triangles[indexT] = indexV;
                triangles[indexT + 1] = indexV + 1;
                triangles[indexT + 2] = indexV + 2;
                triangles[indexT + 3] = indexV;
                triangles[indexT + 4] = indexV + 2;
                triangles[indexT + 5] = indexV + 3;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;

        TileChanged += TileToggled;

        Debug.Log(grid.width + " " + grid.height);
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            ToggleTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void TileToggled(object sender, TileEventArgs e) {
        int x = e.tile.xPos;
        int y = e.tile.yPos;
        int indexV = 4 * (x * grid.height + y);
        switch(e.tile.tileType) {
            case TileType.NONE: {
                e.tile.walkable = false;
                vertices[indexV] = vertices[indexV + 1] = vertices[indexV + 2] = vertices[indexV + 3] = Vector3.zero;
                uv[indexV] = uv[indexV + 1] = uv[indexV + 2] = uv[indexV + 3] = Vector2.zero;
                break;
            }
            case TileType.GROUND: {
                e.tile.walkable = true;
                vertices[indexV] = grid.gridArray[x, y].position;
                vertices[indexV + 1] = grid.gridArray[x, y].position + new Vector3(0, grid.cellSize);
                vertices[indexV + 2] = grid.gridArray[x, y].position + new Vector3(grid.cellSize, grid.cellSize);
                vertices[indexV + 3] = grid.gridArray[x, y].position + new Vector3(grid.cellSize, 0);

                uv[indexV] = new Vector2(0, 0);
                uv[indexV + 1] = new Vector2(0, 1);
                uv[indexV + 2] = new Vector2(.4f, 1);
                uv[indexV + 3] = new Vector2(.4f, 0);
                break;
            }
            case TileType.WALL: {
                e.tile.walkable = false;
                vertices[indexV] = grid.gridArray[x, y].position;
                vertices[indexV + 1] = grid.gridArray[x, y].position + new Vector3(0, grid.cellSize);
                vertices[indexV + 2] = grid.gridArray[x, y].position + new Vector3(grid.cellSize, grid.cellSize);
                vertices[indexV + 3] = grid.gridArray[x, y].position + new Vector3(grid.cellSize, 0);

                uv[indexV] = new Vector2(.6f, 0);
                uv[indexV + 1] = new Vector2(.6f, 1);
                uv[indexV + 2] = new Vector2(1, 1);
                uv[indexV + 3] = new Vector2(1, 0);
                break;
            }
            default: break;
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void ToggleTile(int x, int y) {
        if(x >= 0 && y >= 0 && x < grid.width && y < grid.height) {
            grid.gridArray[x, y].tileType++;
            if(grid.gridArray[x, y].tileType > TileType.WALL) grid.gridArray[x, y].tileType = TileType.NONE;
            TileChanged?.Invoke(this, new TileEventArgs { tile = grid.gridArray[x, y] });
            Debug.Log(x + " " + y);
        }
    }
    public void ToggleTile(Vector3 worldPosition) {
        grid.XY(worldPosition, out int x, out int y);
        ToggleTile(x, y);
    }

    public int GetType(int x, int y) {
        if(x >= 0 && y >= 0 && x < grid.width && y < grid.height) {
            return (int)grid.gridArray[x, y].tileType;
        } else {
            return 99;
        }
    }
    public int GetType(Vector3 worldPosition) {
        grid.XY(worldPosition, out int x, out int y);
        return GetType(x, y);
    }

    public Tile GetTile(int x, int y) {
        if(x >= 0 && y >= 0 && x < grid.width && y < grid.height) {
            return grid.gridArray[x, y];
        } else {
            return null;
        }
    }
    public Tile GetTile(Vector3 worldPosition) {
        grid.XY(worldPosition, out int x, out int y);
        return GetTile(x, y);
    }
}
