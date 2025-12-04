using UnityEngine;
using UnityEngine.UI;  
using TMPro;          

public class BattleUnit : MonoBehaviour
{


    public PokemonData pokemon;
    public int currentHP;

    [Header("HUD 텍스트 연결")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
  

    [Header("HP 게이지 이미지 연결")]
    public Image hpFullGauge;  
    public Image hpHalfGauge;   

    public void Setup(PokemonData data)
    {
        pokemon = data;
        currentHP = pokemon.maxHp;

        nameText.text = pokemon.pokemonName;
        levelText.text = "Lv " + pokemon.level;

        UpdateHP();
    }

    public void UpdateHP()
    {
        float hpPercent = 0f;
        if (pokemon.maxHp > 0) 
        {
            hpPercent = (float)currentHP / pokemon.maxHp;
        }

        hpFullGauge.gameObject.SetActive(false);
        hpHalfGauge.gameObject.SetActive(false);

        if (hpPercent > 0.5f) 
        {
            hpFullGauge.gameObject.SetActive(true);
        }
        else if (hpPercent > 0) 
        {
            hpHalfGauge.gameObject.SetActive(true);
        }
  
    }

    public bool TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        UpdateHP(); 

        return currentHP == 0; 
    }
}