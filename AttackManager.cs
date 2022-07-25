using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState { PREATTACK, STARTED, FINISHED }

public class AttackManager : MonoBehaviour {
    public StateSystem stateSystem;
    public AttackState attackState;

    private Mesh mesh;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    Unit selectedUnit;

    void Start() {
        attackState = AttackState.PREATTACK;
    }

    void Update() {
        selectedUnit = stateSystem.unitManager.activeUnit;
        if(stateSystem.turnState == TurnState.ATTACK) {
            switch(attackState) {
                case AttackState.PREATTACK: {
                    if(Input.GetMouseButtonDown(1)) {
                        Vector3 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Tile target = stateSystem.gridManager.GetTile(click);
                        if(target != null && InRange(target)) {
                            ClearMesh();
                            selectedUnit.target = target;
                            AttackGraphic();
                            attackState = AttackState.STARTED;
                            StartCoroutine(attack());
                        }
                    }
                    break;
                }
                case AttackState.STARTED: {
                    break;
                }
                case AttackState.FINISHED: {
                    //do checks with effects, fire an event?
                    stateSystem.turnState = TurnState.CHOICE;
                    break;
                }
                default: break;
            }
        }
    }

    IEnumerator attack() {
        //fire events and check results
        
        if(selectedUnit.target.occupant != null) selectedUnit.target.occupant.TakeDamage(selectedUnit.attack);
        //else missed
        
        //fire attacked events
        //consume events
        yield return new WaitForSeconds(1);
        ClearMesh();
        attackState = AttackState.FINISHED;
    }

    private bool InRange(Tile target) {
        int xDistance = Mathf.Abs(target.xPos - selectedUnit.tile.xPos);
        int yDistance = Mathf.Abs(target.yPos - selectedUnit.tile.yPos);
        return 10 * selectedUnit.attackRange >= 14 * Mathf.Min(xDistance, yDistance) + 10 * Mathf.Abs(xDistance - yDistance);
    }

    public void UpdateUI() {
        selectedUnit = stateSystem.unitManager.activeUnit;
        int aR = selectedUnit.attackRange;
        int xOrigin = selectedUnit.tile.xPos;
        int yOrigin = selectedUnit.tile.yPos;
        Tile forward, right, left, down;
        List<Tile> tiles = new List<Tile>(4 * aR * aR);

        for(int x = 0; x < aR; x++) {
            for(int y = 1; y <= aR; y++) {
                forward = stateSystem.gridManager.GetTile(xOrigin + x, yOrigin + y);
                if(InRange(forward)) {
                    if(forward != null && forward.walkable) tiles.Add(forward);
                    right = stateSystem.gridManager.GetTile(xOrigin + y, yOrigin - x);
                    if(right != null && right.walkable) tiles.Add(right);
                    left = stateSystem.gridManager.GetTile(xOrigin - y, yOrigin + x);
                    if(left != null && left.walkable) tiles.Add(left);
                    down = stateSystem.gridManager.GetTile(xOrigin - x, yOrigin - y);
                    if(down != null && down.walkable) tiles.Add(down);
                }
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

            uv[indexV] = new Vector2(0, .8f);
            uv[indexV + 1] = new Vector2(0, 1);
            uv[indexV + 2] = new Vector2(.5f, 1);
            uv[indexV + 3] = new Vector2(.5f, .8f);

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

    public void AttackGraphic() {
        vertices = new Vector3[4];
        uv = new Vector2[4];
        triangles = new int[6];

        Vector3 position = selectedUnit.tile.position;
        int cellSize = selectedUnit.tile.grid.cellSize;
        vertices[0] = position + new Vector3(.75f * cellSize, .25f * cellSize);
        vertices[1] = position + new Vector3(.75f * cellSize, .75f * cellSize);
        vertices[2] = position + new Vector3(cellSize, .75f * cellSize);
        vertices[3] = position + new Vector3(cellSize, .25f * cellSize);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, .8f);
        uv[2] = new Vector2(1, .8f);
        uv[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh = new Mesh {
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
