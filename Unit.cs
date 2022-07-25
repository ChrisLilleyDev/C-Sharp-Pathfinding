using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    private UnitManager unitManager;
    public SpriteRenderer spriteRenderer;
    public int id;
    public bool alive = true;
    public int maxHp, currentHp;
    public int attack, attackRange;
    public int moveRange;
    public Tile tile;
    public Tile target;

    public bool moving = false;
    public List<Tile> path = new List<Tile>();
    public float moveSpeed = 10f;

    public event EventHandler<UnitEventArgs> UnitDamaged;
    public event EventHandler<UnitEventArgs> UnitDied;
    public class UnitEventArgs : EventArgs {
        public Unit unit;
        public int value;
    }

    private void Start() {
        unitManager = transform.parent.gameObject.GetComponent<UnitManager>();
        tile = unitManager.stateSystem.gridManager.GetTile(transform.position);
        tile.occupant = this;
        path.Add(tile);
    }

    private void Update() {
        if(moving) {
            if(moveSpeed * Time.deltaTime > (path[0].position - transform.position).magnitude) {
                tile.occupant = null;
                transform.position = path[0].position;
                tile = path[0];
                tile.occupant = this;
                if(path.Count > 1) path.RemoveAt(0);
                moving = false;
            } else transform.position += (path[0].position - transform.position).normalized * moveSpeed * Time.deltaTime;
        }
    }

    public void TakeDamage(int damage) {
        currentHp -= damage;
        if(damage > 0) UnitDamaged?.Invoke(this, new UnitEventArgs { unit = this, value = damage });
        if(currentHp <= 0) {
            currentHp = 0;
            alive = false;
            UnitDied?.Invoke(this, new UnitEventArgs { unit = this });
        }
    }
}
