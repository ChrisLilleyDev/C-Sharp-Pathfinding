using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour {
    public StateSystem stateSystem;

    public ActiveUnitUI activeUnitUI;
    public SelectedUnitUI selectedUnitUI;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Unit activeUnit;
    public List<Unit> units;

    void Start() {
        units = new List<Unit>();
        InitialiseUnits();
        activeUnit = units[0];
        activeUnitUI.SetUI(activeUnit);

    }

    void Update() {
        Vector3 hover = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Tile hoverTile = stateSystem.gridManager.GetTile(hover);
        if(selectedUnitUI.selectState == SelectState.OFF && hoverTile != null && hoverTile.occupant != null) selectedUnitUI.SetState(SelectState.UNFOCUSED);
        if(selectedUnitUI.selectState == SelectState.UNFOCUSED && hoverTile != null && hoverTile.occupant != null) selectedUnitUI.SetUI(hoverTile.occupant);
    }
    //unit manager should listen for events?
    //if unit dies fire event?

    void OnUnitDied(object sender, Unit.UnitEventArgs e) {
        units.Remove(e.unit);
        e.unit.spriteRenderer.color = Color.grey;
    }

    public void NextUnit() {
        units.RemoveAt(0);
        units.Add(activeUnit);
        activeUnit = units[0];
        activeUnitUI.SetUI(activeUnit);
    }

    void InitialiseUnits() {
        units.Add(Instantiate(playerPrefab, new Vector3(-3f, -1f), Quaternion.Euler(0, 0, 0), transform).GetComponent<Unit>());
        units.Add(Instantiate(enemyPrefab, new Vector3(5f, 2f), Quaternion.Euler(0, 0, 0), transform).GetComponent<Unit>());
        units.Add(Instantiate(playerPrefab, new Vector3(-2f, -2f), Quaternion.Euler(0, 0, 0), transform).GetComponent<Unit>());
        units.Add(Instantiate(enemyPrefab, new Vector3(4f, 1f), Quaternion.Euler(0, 0, 0), transform).GetComponent<Unit>());
        foreach(Unit unit in units) {
            unit.UnitDied += OnUnitDied;
        }
    }
}
