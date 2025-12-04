using UnityEngine;

[CreateAssetMenu(fileName = "New Pokemon", menuName = "Pokemon/Create New Pokemon")]
public class PokemonData : ScriptableObject
{
    public string pokemonName;

    [Header("스프라이트")]
    public Sprite pokemonSprite; 
    public Sprite backSprite;   

    [Header("능력치")]
    public int level;   
    public int maxHp;
    public int attack;
    public int defense;
}
