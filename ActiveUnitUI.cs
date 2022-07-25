using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveUnitUI : MonoBehaviour {
    public Slider hpSlider, manaSlider;

    public void SetUI(Unit unit) {
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentHp;
    }

    public void SetHp(int hp) {
        hpSlider.value = hp;
    }
}
