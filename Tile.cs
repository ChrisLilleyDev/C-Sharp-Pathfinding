using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TileType { NONE, GROUND, WALL }

public class Tile {
    public int xPos, yPos;
    public Grid grid;
    public Vector3 position;
    public TileType tileType;
    public bool walkable;
    public Unit occupant;

    public Tile(int xPos, int yPos, Grid grid, Vector3 position) {
        this.xPos = xPos;
        this.yPos = yPos;
        this.grid = grid;
        this.position = position;
        tileType = TileType.GROUND;
        walkable = true;
    }
    public Tile(int xPos, int yPos, Grid grid, Vector3 position, TileType tileType) {
        this.xPos = xPos;
        this.yPos = yPos;
        this.grid = grid;
        this.position = position;
        this.tileType = tileType;
        if(tileType == TileType.GROUND) walkable = true;
        else walkable = false;
    }
}
