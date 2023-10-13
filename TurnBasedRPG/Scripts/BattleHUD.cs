using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Slider hpSlider;
    public Slider manaSlider;
    public Slider expSlider;
    public Sprite necromancerHUD;
    public Sprite thiefHUD;



    public void SetHUD(Unit unit)
    {
        nameText.text = unit.GetUnitName();
        hpSlider.maxValue = unit.GetMaxHP();
        hpSlider.value = unit.GetCurrentHP();
        manaSlider.maxValue = unit.GetMaxMana();
        manaSlider.value = unit.GetCurrentMana();
        expSlider.maxValue = unit.GetMaxExp();
        expSlider.value = unit.GetCurrentExp();

        if (unit.CompareTag("Necromancer")) 
            gameObject.GetComponent<Image>().sprite = necromancerHUD;

        else if(unit.CompareTag("Thief"))
            gameObject.GetComponent<Image>().sprite = thiefHUD;
        


    }

    public void SetHP(int HP)
    {
        hpSlider.value = HP;
    }

    public void SetMana(int Mana)
    {
        manaSlider.value = Mana;
    }

    public void SetExp(int Exp)
    {
        expSlider.value = Exp;
    }
}
