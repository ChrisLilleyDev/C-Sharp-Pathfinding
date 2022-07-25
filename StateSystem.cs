using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { START, PTURN, ETURN, WON, LOST }
public enum TurnState { CHOICE, MOVE, ATTACK, END }

public class StateSystem : MonoBehaviour {
    public UnitManager unitManager;
    public GridManager gridManager;
    public MoveManager moveManager;
    public AttackManager attackManager;

    public GameState gameState;
    public TurnState turnState;

    public KeyCode move;
    public KeyCode attack;
    public KeyCode escape;

    void Start() {
        gameState = GameState.START;
    }

    void Update() {
        switch(gameState) {
            case GameState.START: {
                turnState = TurnState.CHOICE;
                gameState = GameState.PTURN;
                break;
            }
            case GameState.PTURN: {
                switch(turnState) {
                    case TurnState.CHOICE: {
                        if(Input.GetKeyDown(move) && moveManager.moveState == MoveState.PREMOVE) {
                            moveManager.UpdateUI();
                            turnState = TurnState.MOVE;
                        }
                        if(Input.GetKeyDown(attack) && attackManager.attackState == AttackState.PREATTACK) {
                            attackManager.UpdateUI();
                            turnState = TurnState.ATTACK;
                        }
                        if(Input.GetKeyDown(escape)) turnState = TurnState.END;

                        break;
                    }
                    case TurnState.MOVE: {
                        if(moveManager.moveState != MoveState.STARTED) {
                            if(Input.GetKeyDown(attack) && attackManager.attackState == AttackState.PREATTACK) {
                                moveManager.ClearMesh();
                                attackManager.UpdateUI();
                                turnState = TurnState.ATTACK;
                            }
                            if(Input.GetKeyDown(escape)) {
                                moveManager.ClearMesh();
                                turnState = TurnState.CHOICE;
                            }
                        }
                        break;
                    }
                    case TurnState.ATTACK: {
                        if(attackManager.attackState != AttackState.STARTED) {
                            if(Input.GetKeyDown(move) && moveManager.moveState == MoveState.PREMOVE) {
                                attackManager.ClearMesh();
                                moveManager.UpdateUI();
                                turnState = TurnState.MOVE;
                            }
                            if(Input.GetKeyDown(escape)) {
                                attackManager.ClearMesh();
                                turnState = TurnState.CHOICE;
                            }
                        }
                        break;
                    }
                    case TurnState.END: {
                        moveManager.moveState = MoveState.PREMOVE;
                        attackManager.attackState = AttackState.PREATTACK;
                        unitManager.NextUnit();
                        turnState = TurnState.CHOICE;
                        gameState = GameState.ETURN;

                        break;
                    }
                    default: break;
                }

                break;
            }
            case GameState.ETURN: {
                switch(turnState) {
                    case TurnState.CHOICE: {
                        if(Input.GetKeyDown(move) && moveManager.moveState == MoveState.PREMOVE) {
                            moveManager.UpdateUI();
                            turnState = TurnState.MOVE;
                        }
                        if(Input.GetKeyDown(attack) && attackManager.attackState == AttackState.PREATTACK) {
                            attackManager.UpdateUI();
                            turnState = TurnState.ATTACK;
                        }
                        if(Input.GetKeyDown(escape)) turnState = TurnState.END;

                        break;
                    }
                    case TurnState.MOVE: {
                        if(moveManager.moveState != MoveState.STARTED) {
                            if(Input.GetKeyDown(attack) && attackManager.attackState == AttackState.PREATTACK) {
                                moveManager.ClearMesh();
                                attackManager.UpdateUI();
                                turnState = TurnState.ATTACK;
                            }
                            if(Input.GetKeyDown(escape)) {
                                moveManager.ClearMesh();
                                turnState = TurnState.CHOICE;
                            }
                        }
                        break;
                    }
                    case TurnState.ATTACK: {
                        if(attackManager.attackState != AttackState.STARTED) {
                            if(Input.GetKeyDown(move) && moveManager.moveState == MoveState.PREMOVE) {
                                attackManager.ClearMesh();
                                moveManager.UpdateUI();
                                turnState = TurnState.MOVE;
                            }
                            if(Input.GetKeyDown(escape)) {
                                attackManager.ClearMesh();
                                turnState = TurnState.CHOICE;
                            }
                        }
                        break;
                    }
                    case TurnState.END: {
                        moveManager.moveState = MoveState.PREMOVE;
                        attackManager.attackState = AttackState.PREATTACK;
                        unitManager.NextUnit();
                        turnState = TurnState.CHOICE;
                        gameState = GameState.PTURN;

                        break;
                    }
                    default: break;
                }

                break;
            }
            default: break;
        }
    }
}