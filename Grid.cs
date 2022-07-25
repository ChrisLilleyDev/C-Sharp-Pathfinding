using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {

    public int width, height, cellSize;
    public Tile[,] gridArray;
    public Vector3 originPosition;

    public Grid(int width, int height, int cellSize, Vector3 originPosition) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new Tile[width, height];

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if(x >= 1 && x < width - 1 && y >= 1 && y < height - 1) gridArray[x, y] = new Tile(x, y, this, WorldPos(x, y));
                else gridArray[x, y] = new Tile(x, y, this, WorldPos(x, y), TileType.NONE);
            }
        }
    }

    public Vector3 WorldPos(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void XY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
}