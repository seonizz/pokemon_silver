using UnityEngine;

public static class TransitionData
{

    public static PokemonData wildPokemon;    
    public static PokemonData playerPokemon; 

    public static string returnSceneName;      
    public static Vector2 returnPosition;     
    public static bool returningFromBattle;    


    public static bool hasSpawn = false;
    public static Vector2 nextSpawnPos;
}