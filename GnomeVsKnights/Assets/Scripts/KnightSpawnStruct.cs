using UnityEngine;
//Datastructure containing spawn information for a spawning instance
[System.Serializable]
public class KnightSpawnStruct
{
    public float spawnTime; //What time to spawn the knights at
    public bool isWave; //Whether spawning this set counts as spawning a wave of knights
    public int[] knights; //Which knights to spawn
}
