using UnityEngine;
using System.Collections.Generic; 

public class TallGrassZone : MonoBehaviour
{
    public List<PokemonData> possibleEncounters;

    public PokemonData GetRandomPokemon()
    {
        if (possibleEncounters.Count == 0)
        {
            Debug.LogError("풀숲에 포켓몬이 설정되지 않음");
            return null;
        }

        int randomIndex = Random.Range(0, possibleEncounters.Count);
        return possibleEncounters[randomIndex];
    }
}