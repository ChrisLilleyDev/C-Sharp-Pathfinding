using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SelectState { FOCUSED, UNFOCUSED, OFF, HIDDEN }

public class SelectedUnitUI : MonoBehaviour {
    public SelectState selectState;
    public RectTransform rectTransform;
    public Slider hpSlider, manaSlider;

    void Start() {
        SetState(SelectState.OFF);
    }

    public void SetUI(Unit unit) {
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentHp;
    }

    public void SetHp(int hp) {
        hpSlider.value = hp;
    }

    public void SetState(SelectState state) {
        selectState = state;
        switch(selectState) {
            case SelectState.FOCUSED: {
                rectTransform.anchoredPosition = new Vector2(-20, 20);
                break;
            }
            case SelectState.UNFOCUSED: {
                rectTransform.anchoredPosition = new Vector2(-20, 20);
                break;
            }
            case SelectState.OFF: {
                rectTransform.anchoredPosition = new Vector2(700, 20);
                break;
            }
            case SelectState.HIDDEN: {
                rectTransform.anchoredPosition = new Vector2(700, 20);
                break;
            }
            default: break;
        }
    }
}
