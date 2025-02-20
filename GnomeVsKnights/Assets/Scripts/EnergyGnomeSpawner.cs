using UnityEngine;

public class EnergyGnomeSpawner : MonoBehaviour
{
    public GameObject energyGnomePrefab;
    void Start()
    {
        SpawnGnome();
    }

    public void SpawnGnome()
    {
        Instantiate(energyGnomePrefab);
        Invoke("SpawnGnome", Random.Range(8, 15));
    }
}
